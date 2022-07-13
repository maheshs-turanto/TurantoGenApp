using GeneratorBase.MVC.Models;
using RecurrenceGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.OleDb;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{

/// <summary>A controller for handling time based alerts.</summary>
public partial class TimeBasedAlertController : Controller
{
    /// <summary>Gets generic data.</summary>
    ///
    /// <param name="context">The context.</param>
    /// <param name="_type">  The type.</param>
    ///
    /// <returns>The generic data.</returns>
    
    public DbSet GetGenericData(ApplicationContext context, Type _type)
    {
        return context.Set(_type);
    }
    
    /// <summary>Gets table object.</summary>
    ///
    /// <param name="context">  The context.</param>
    /// <param name="tableName">Name of the table.</param>
    ///
    /// <returns>The table object.</returns>
    
    public object GetTableObject(ApplicationContext context, string tableName)
    {
        PropertyInfo[] properties = typeof(ApplicationContext).GetProperties();
        var prop = properties.FirstOrDefault(p => p.Name == tableName + "s");
        if(prop != null)
        {
            var table = prop.GetValue(context);
            return table;
        }
        else return null;
    }
    
    /// <summary>Scheduled task.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="BizId">     Identifier for the biz.</param>
    ///
    /// <returns>A response stream to send to the ScheduledTask View.</returns>
    
    public ActionResult ScheduledTask(string EntityName, long BizId)
    {
        var user = new InternalUser();
        var MainBiz = new BusinessRule();
        List<BusinessRule> businessrules = new List<BusinessRule>();
        using(var br = new BusinessRuleContext())
        {
            br.Configuration.LazyLoadingEnabled = false;
            br.Configuration.AutoDetectChangesEnabled = false;
            var brcompletelist = br.BusinessRules.Include(t => t.ruleconditions).Include(t => t.ruleaction).GetFromCache<IQueryable<BusinessRule>, BusinessRule>().ToList();
            List<BusinessRule> brList = brcompletelist.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable).ToList();
            List<long> brIds = brList.Select(q => q.Id).ToList();
            var actiontypes = (new GeneratorBase.MVC.Models.ActionTypeContext()).ActionTypes.GetFromCache<IQueryable<ActionType>, ActionType>().ToList();
            var actionargs = (new GeneratorBase.MVC.Models.ActionArgsContext()).ActionArgss.GetFromCache<IQueryable<ActionArgs>, ActionArgs>().ToList();
            foreach(var rules in brList)
            {
                if(rules.Roles.Split(",".ToCharArray()).Contains("All"))
                {
                    rules.ruleaction.ToList().ForEach(p =>
                    {
                        p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID);
                        p.actionarguments = actionargs.Where(q => q.ActionArgumentsID == p.Id).ToList();
                    });
                    //rules.ruleaction.ToList().ForEach(p => p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID));
                    rules.ActionTypeID = rules.ruleaction.Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
                    businessrules.Add(rules);
                }
            }
        }
        MainBiz = businessrules.FirstOrDefault(p => p.Id == BizId);
        if(MainBiz != null)
        {
            (user).businessrules = businessrules.ToList();
            var database = new ApplicationContext(user, 0);
            var myType = Type.GetType("GeneratorBase.MVC.Models." + EntityName + ", GeneratorBase.MVC.Models");
            //var data = GetGenericData(database, myType).ToListAsync();
            //foreach (var item in data.Result)
            var data = GetTableObject(database, EntityName);
            var dataList = (IQueryable<object>)data;
            foreach(var item in dataList.ToList())
            {
                if(ApplyRule.CheckRule<object>(item, MainBiz, MainBiz.EntityName))
                {
                    database.Entry(item).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext();
            var itemhistory = sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == MainBiz.Id);
            itemhistory.Status = "Processed";
            sthcontext.Entry(itemhistory).State = EntityState.Modified;
            sthcontext.SaveChanges();
            // RegisterScheduledTask nexttask = new RegisterScheduledTask();
            // nexttask.RegisterTask(MainBiz.EntityName, MainBiz.Id);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    //[AllowAnonymous]
    //[HttpGet]
    public ActionResult Run()
    {
        double CallBackSpan = CommonFunction.Instance.ScheduledTaskCallbackTime();
        RunBusinessRules(CallBackSpan);
        Custom();
        var appurl = System.Configuration.ConfigurationManager.AppSettings["AppURL"];
        Uri callbackUrl = new Uri(string.Format("http://localhost//" + appurl + "//TimeBasedAlert/Run"));
        if(CommonFunction.Instance.UseRevalee())
        {
            using(ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext())
            {
                foreach(var task in sthcontext.ScheduledTaskHistorys.Where(p => p.Status == "System" && p.TaskName == appurl))
                    sthcontext.Entry(task).State = EntityState.Deleted;
                var callbackid = Revalee.Client.RevaleeRegistrar.ScheduleCallback(DateTime.Now.AddMinutes(CallBackSpan), callbackUrl);
                ScheduledTaskHistory nextitemhistory = new ScheduledTaskHistory();
                nextitemhistory.Status = "System";
                nextitemhistory.BusinessRuleId = null;
                sthcontext.Entry(nextitemhistory).State = EntityState.Added;
                nextitemhistory.TaskName = appurl;
                nextitemhistory.CallbackUri = Convert.ToString(callbackUrl);
                nextitemhistory.GUID = Convert.ToString(callbackid);
                sthcontext.SaveChanges();
            }
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    
    private void RunBusinessRules(double CallBackSpan)
    {
        List<BusinessRule> businessrules = new List<BusinessRule>();
        using(var br = new BusinessRuleContext())
        {
            br.Configuration.LazyLoadingEnabled = false;
            br.Configuration.AutoDetectChangesEnabled = false;
            var brcompletelist = br.BusinessRules.Include(t => t.ruleconditions).Include(t => t.ruleaction).GetFromCache<IQueryable<BusinessRule>, BusinessRule>().ToList();
            List<BusinessRule> brList = brcompletelist.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID == 5).ToList();
            List<long> brIds = brList.Select(q => q.Id).ToList();
            var actiontypes = (new GeneratorBase.MVC.Models.ActionTypeContext()).ActionTypes.GetFromCache<IQueryable<ActionType>, ActionType>().ToList();
            var actionargs = (new GeneratorBase.MVC.Models.ActionArgsContext()).ActionArgss.GetFromCache<IQueryable<ActionArgs>, ActionArgs>().ToList();
            foreach(var rules in brList)
            {
                if(rules.Roles.Split(",".ToCharArray()).Contains("All"))
                {
                    rules.ruleaction.ToList().ForEach(p =>
                    {
                        p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID);
                        p.actionarguments = actionargs.Where(q => q.ActionArgumentsID == p.Id).ToList();
                    });
                    //rules.ruleaction.ToList().ForEach(p => p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID));
                    rules.ActionTypeID = rules.ruleaction.Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
                    businessrules.Add(rules);
                }
            }
        }
        var user = new InternalUser();
        (user).businessrules = businessrules.ToList();
        foreach(var rule in businessrules)
        {
            RegisterScheduledTask nexttask = new RegisterScheduledTask();
            DateTime nextrun = nexttask.getNextRunTimeOfTask(rule);
            if(nextrun >= DateTime.UtcNow && nextrun <= DateTime.UtcNow.AddMinutes(CallBackSpan))
            {
                var database = new ApplicationContext(user, 0);
                var myType = Type.GetType("GeneratorBase.MVC.Models." + rule.EntityName + ", GeneratorBase.MVC.Models");
                var data = GetTableObject(database, rule.EntityName);
                var dataList = (IQueryable<object>)data;
                foreach(var item in dataList.ToList())
                {
                    if(ApplyRule.CheckRule<object>(item, rule, rule.EntityName))
                    {
                        database.Entry(item).State = EntityState.Modified;
                        database.SaveChanges();
                    }
                }
                using(ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext())
                {
                    var itemhistory = sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id);
                    if(itemhistory != null)
                    {
                        itemhistory.Status = "Processed";
                        sthcontext.Entry(itemhistory).State = EntityState.Modified;
                        sthcontext.SaveChanges();
                    }
                    else
                    {
                        ScheduledTaskHistory nextitemhistory = new ScheduledTaskHistory();//sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id);
                        nextitemhistory.Status = "Processed";
                        nextitemhistory.BusinessRuleId = rule.Id;
                        nextitemhistory.RunDateTime = Convert.ToString(nextrun);
                        sthcontext.Entry(nextitemhistory).State = EntityState.Added;
                        nextitemhistory.TaskName = rule.RuleName;
                        sthcontext.SaveChanges();
                    }
                    DateTime nextRun = nexttask.getNextRunTimeOfTask(rule, DateTime.UtcNow.AddMinutes(CallBackSpan));
                    var itemhistorypending = sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id && p.Status == "Pending");
                    if(itemhistorypending == null)
                    {
                        ScheduledTaskHistory nextitemhistory = new ScheduledTaskHistory();//sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id);
                        nextitemhistory.Status = "Pending";
                        nextitemhistory.BusinessRuleId = rule.Id;
                        nextitemhistory.RunDateTime = Convert.ToString(nextRun);
                        sthcontext.Entry(nextitemhistory).State = EntityState.Added;
                        nextitemhistory.TaskName = rule.RuleName;
                        sthcontext.SaveChanges();
                    }
                }
            }
            else
            {
                if(nextrun > DateTime.UtcNow.AddMinutes(CallBackSpan))
                {
                    ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext();
                    var itemhistorypending = sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id && p.Status == "Pending");
                    if(itemhistorypending == null)
                    {
                        ScheduledTaskHistory itemhistory = new ScheduledTaskHistory();//sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == rule.Id);
                        itemhistory.Status = "Pending";
                        itemhistory.BusinessRuleId = rule.Id;
                        itemhistory.RunDateTime = Convert.ToString(nextrun);
                        sthcontext.Entry(itemhistory).State = EntityState.Added;
                        itemhistory.TaskName = rule.RuleName;
                        sthcontext.SaveChanges();
                    }
                }
            }
        }
    }
    private void Custom()
    {
    }
    
    public void OnDocGenerating(GemBox.Document.DocumentModel document, object data, BusinessRule br, string EntityName, ApplicationUser user)
    {
    }
    
    
    
    /// <summary>Notifies an one time.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="Id">        The identifier.</param>
    /// <param name="actionid">  The actionid.</param>
    /// <param name="userName">  Name of the user.</param>
    ///
    /// <returns>A response stream to send to the NotifyOneTime View.</returns>
    
    public ActionResult NotifyOneTime(string EntityName, long Id, long actionid, string userName, IUser user, object entityobj)
    {
        var AppURL = CommonFunction.Instance.AppURL();
        var server = CommonFunction.Instance.Server();
        string NotifyTo = "";
        string NotifyToExtra = "";
        string NotifyToRole = "";
        string emailTo = "";
        var alertMessage = "";
        var ruleactiondb = new RuleActionContext();
        var act = ruleactiondb.RuleActions.First(p => p.Id == actionid);
        var TemplateId = act.TemplateId;
        var ruledb = new BusinessRuleContext();
        var br = ruledb.BusinessRules.Find(act.RuleActionID);
        var subject = br.RuleName;
        alertMessage += act.ErrorMessage;
        var argslist = act.actionarguments.ToList();
        foreach(var args in argslist)
        {
            if(args.ParameterName == "NotifyTo")
                NotifyTo = args.ParameterValue;
            if(args.ParameterName == "NotifyToExtra")
                NotifyToExtra = args.ParameterValue;
            if(args.ParameterName == "NotifyToRole")
                NotifyToRole = args.ParameterValue;
        }
        var userinfo = new ApplicationUser();
        var flag = false;
        //object entityobj = null;
        if(!string.IsNullOrEmpty(userName))
        {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            MethodInfo mc = controller.GetMethod("GetRecordById");
            object[] MethodParams = new object[] { Convert.ToString(Id) };
            var entry = mc.Invoke(objController, MethodParams);
            getEmail objgetEmail = new getEmail();
            if(NotifyTo.Contains("none"))
                NotifyTo = "";
            var emails1 = objgetEmail.getUsers(new SystemUser(), NotifyTo.Split(",".ToCharArray()), entry, NotifyToRole.Split(",".ToCharArray()), userName).Distinct();
            if(emails1.Count() == 1)
            {
                userinfo = emails1.FirstOrDefault();
                flag = true;
            }
            emailTo = string.Join(",", emails1.Select(p => p.Email));
            if(!string.IsNullOrEmpty(NotifyToRole))
            {
                //emailTo += "," + objgetEmail.getUserEmailidsFromRoles(NotifyToRole.Split(",".ToCharArray()), user);
                var roleemailIds = objgetEmail.getUserEmailidsFromRoles(NotifyToRole.Split(",".ToCharArray()), user);
                if(!string.IsNullOrEmpty(roleemailIds))
                    emailTo += "," + roleemailIds;
            }
            if(!string.IsNullOrEmpty(NotifyTo))
            {
                //emailTo += "," + objgetEmail.getEmailids(new SystemUser(), NotifyTo.Split(",".ToCharArray()), entry, NotifyToRole.Split(",".ToCharArray()), userName);
                var emailIds = objgetEmail.getEmailids(new SystemUser(), NotifyTo.Split(",".ToCharArray()), entry, NotifyToRole.Split(",".ToCharArray()), userName);
                if(!string.IsNullOrEmpty(emailIds))
                    emailTo += "," + emailIds;
            }
            var emailIdValue = objgetEmail.getEmailIdValue(new SystemUser(), NotifyTo.Split(",".ToCharArray()), entry, NotifyToRole.Split(",".ToCharArray()), userName);
            if(!string.IsNullOrEmpty(emailIdValue))
                emailTo += "," + emailIdValue;
        }
        if(!string.IsNullOrEmpty(NotifyToExtra))
            emailTo += "," + NotifyToExtra;
        emailTo = emailTo.Trim(',');
        emailTo = emailTo.Trim().TrimEnd(",".ToCharArray());
        emailTo = emailTo.Replace(",,", ",");
        //
        if(alertMessage.ToUpper().Contains("###RECORD###"))
        {
            try
            {
                Type controller1 = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController1 = Activator.CreateInstance(controller1, null);
                MethodInfo mc1 = controller1.GetMethod("GetRecordById_Reflection");
                object[] MethodParams1 = new object[] { Convert.ToString(Id) };
                var msgDetails1 = Convert.ToString(mc1.Invoke(objController1, MethodParams1));
                alertMessage = alertMessage.Replace("###Record###", msgDetails1);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(alertMessage.ToUpper().Contains("###RECORDLINK###"))
        {
            var request = System.Web.HttpContext.Current.Request;
            //string url = request.Url.Scheme + server;
            var url = string.Format("{0}://{1}", request.Url.Scheme, server);
            var fdenable = CommonFunction.Instance.FrontDoorEnable();
            if(fdenable.ToLower() == "yes")
            {
                var fdurl = CommonFunction.Instance.FrontDoorUrl();
                url = fdurl;
            }
            //
            alertMessage = alertMessage.Replace("###RecordLink###", "<a href=\"" + "http://" + server + "/" + AppURL + "/" + EntityName + "/Edit/" + Id + "\">Link</a>");
            //alertMessage = alertMessage.Replace("###RecordLink###", "<a href=\"" + url + "/" + EntityName + "/Edit/" + Id + "\">Link</a>");
            //alertMessage += "<br/><a href=\"" + "http://" + server + Url.Action("Edit", EntityName, new { Id = Id }) + "\">Click to review</a>";
        }
        if(alertMessage.ToUpper().Contains("###RECORDURL###"))
        {
            var request = System.Web.HttpContext.Current.Request;
            //  string url = request.Url.Scheme + server;
            var url = string.Format("{0}://{1}", request.Url.Scheme, server);
            var fdenable = CommonFunction.Instance.FrontDoorEnable();
            if(fdenable.ToLower() == "yes")
            {
                var fdurl = CommonFunction.Instance.FrontDoorUrl();
                url = fdurl;
            }
            //
            alertMessage = alertMessage.Replace("###RecordURL###", "http://" + server + "/" + AppURL + "/" + EntityName + "/Edit/" + Id);
            //alertMessage = alertMessage.Replace("###RecordURL###", url + "/" + EntityName + "/Edit/" + Id);
        }
        if(!string.IsNullOrEmpty(emailTo))
        {
            //
            SendEmail mail = new SendEmail();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string mailbody = string.Empty;
            string subjecttemplate = string.Empty;
            var mailbodydata = string.Empty;
            if(TemplateId != null)
            {
                mailbodydata = TemplateData(mailbodydata, TemplateId, Id, br, EntityName, userinfo, user);
            }
            dictionary.Add("###Message###", alertMessage);
            if(flag)
            {
                dictionary.Add("###Username###", userinfo.UserName);
                dictionary.Add("###FullName###", userinfo.FirstName + " " + userinfo.LastName);
                dictionary.Add("###FirstName###", userinfo.FirstName);
                dictionary.Add("###LastName###", userinfo.LastName);
            }
            dictionary = CustomEmailBodyMapping(dictionary, userinfo, entityobj, EntityName);
            var result = mail.getEmailBodyAndSubjectFromTemplate("Business Rule", dictionary);
            if(result.Count() > 0)
            {
                mailbody = result[1];
                subjecttemplate = result[0];
            }
            if(!string.IsNullOrEmpty(subjecttemplate))
                subject = subjecttemplate;
            emailTo = string.Join(",", emailTo.Split(',').Distinct().ToArray());
            if(!string.IsNullOrEmpty(mailbodydata))
                mail.Notify("", emailTo, mailbodydata, subject);
            else
                mail.Notify("", emailTo, mailbody, subject);
        }
        return null;
    }
    
    public string TemplateData(string mailbodydata, long? TemplateId, long RecordId, BusinessRule br, string EntityName, ApplicationUser user, IUser modeluser)
    {
        using(var appcontext = new ApplicationContext(new SystemUser()))
        {
            var bytedata = appcontext.Documents.AsNoTracking().Where(p => p.Id == TemplateId).Select(p => p.Byte).FirstOrDefault();
            if(bytedata != null)
            {
                using(Stream stream = new MemoryStream(bytedata))
                {
                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                    object objController = Activator.CreateInstance(controller, null);
                    MethodInfo mc = controller.GetMethod("GetDetailsRecordById");
                    object[] MethodParams = new object[] { Convert.ToString(RecordId), appcontext };
                    var entrydata = mc.Invoke(objController, MethodParams);
                    var document = GemBox.Document.DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);
                    var commonobj = CommonFunction.Instance.getCompanyProfile(modeluser);
                    if(commonobj != null)
                    {
                        if(!commonobj.Icon.Contains("logo"))
                        {
                            var logo = appcontext.Documents.Find(Convert.ToInt64(commonobj.Icon));
                            if(logo != null && logo.Byte != null)
                            {
                                var wi = new WebImage(logo.Byte);
                                document.MailMerge.Execute(new { CompanyProfileLogo = wi.GetBytes() });
                            }
                        }
                        if(commonobj.Address != null)
                        {
                            document.MailMerge.Execute(new { CompanyProfileAddress = commonobj.Address });
                        }
                    }
                    OnDocGenerating(document, entrydata, br, EntityName, user);
                    document.MailMerge.ClearOptions = GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyRanges | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveUnusedFields | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyParagraphs;
                    document.MailMerge.Execute(entrydata);
                    Stream outstream = new MemoryStream();
                    var saveOptions = new GemBox.Document.HtmlSaveOptions()
                    {
                        HtmlType = GemBox.Document.HtmlType.Html,
                        EmbedImages = true,
                        UseSemanticElements = true
                    };
                    document.Save(outstream, saveOptions);
                    StreamReader rdr = new StreamReader(outstream);
                    while(!rdr.EndOfStream)
                    {
                        mailbodydata += rdr.ReadLine();
                    }
                }
            }
        }
        return mailbodydata;
    }
}
}
