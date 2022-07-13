using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Data.OleDb;


namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling action arguments.</summary>
public class ActionArgsController : BaseController
{
    /// <summary>The database.</summary>
    private ActionArgsContext db = new ActionArgsContext();
    /// <summary>The database action arguments.</summary>
    private RuleActionContext dbActionArguments = new RuleActionContext();
    
    /// <summary>GET: /ActionArgs/.</summary>
    ///
    /// <param name="currentFilter">  A filter specifying the current.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="IsExport">       The is export.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport)
    {
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstActionArgs = from s in db.ActionArgss
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionArgs = searchRecords(lstActionArgs, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstActionArgs = sortRecords(lstActionArgs, sortBy, isAsc);
        }
        else
            lstActionArgs = lstActionArgs.OrderByDescending(s => s.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        var _ActionArgs = lstActionArgs.Include(t => t.actionarguments);
        if(HostingEntity == "RuleAction" && HostingEntityID != null && AssociatedType == "ActionArguments")
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            _ActionArgs = _ActionArgs.Where(p => p.ActionArgumentsID == hostid);
        }
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ActionArgs.Count() > 0)
                pageSize = _ActionArgs.Count();
            return View("ExcelExport", _ActionArgs.ToPagedList(pageNumber, pageSize));
        }
        return View(_ActionArgs.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstActionArgs">The list action arguments.</param>
    /// <param name="searchString"> The search string.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<ActionArgs> searchRecords(IQueryable<ActionArgs> lstActionArgs, string searchString)
    {
        lstActionArgs = lstActionArgs.Where(s => (!String.IsNullOrEmpty(s.ParameterName) && s.ParameterName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ParameterValue) && s.ParameterValue.ToUpper().Contains(searchString)) || (s.actionarguments != null && (s.actionarguments.DisplayValue.ToUpper().Contains(searchString))));
        return lstActionArgs;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstActionArgs">The list action arguments.</param>
    /// <param name="sortBy">       Describes who sort this object.</param>
    /// <param name="isAsc">        The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<ActionArgs> sortRecords(IQueryable<ActionArgs> lstActionArgs, string sortBy, string isAsc)
    {
        var elementType = typeof(ActionArgs);
        var param = Expression.Parameter(elementType, "actionargs");
        var prop = elementType.GetProperty(sortBy);
        Type type = Nullable.GetUnderlyingType(prop.PropertyType);
        if(type == null)
            type = prop.PropertyType;
        if(type.Equals(typeof(string)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, string>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(char)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, char?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(int)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, int?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(float)) || type.Equals(typeof(double)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, double?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(decimal)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, decimal?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(long)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, long?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(DateTime)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionArgs, DateTime?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionArgs = lstActionArgs.OrderBy(mySortExpression);
            else
                lstActionArgs = lstActionArgs.OrderByDescending(mySortExpression);
        }
        // This last part won't work but I left it so that it can compile (all routes need to return value etc.)
        return lstActionArgs;
    }
    
    /// <summary>GET: /ActionArgs/Details/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionArgs actionargs = db.ActionArgss.Find(id);
        if(actionargs == null)
        {
            return HttpNotFound();
        }
        return View(actionargs);
    }
    
    /// <summary>GET: /ActionArgs/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "RuleAction" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objActionArguments = new List<RuleAction>();
            ViewBag.ActionArgumentsID = new SelectList(objActionArguments, "ID", "DisplayValue");
        }
        ViewData["ActionArgsParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>GET: /ActionArgs/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "RuleAction" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objActionArguments = new List<RuleAction>();
            ViewBag.ActionArgumentsID = new SelectList(objActionArguments, "ID", "DisplayValue");
        }
        ViewData["ActionArgsParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>POST: /ActionArgs/CreateWizard To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actionargs"> The actionargs.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public ActionResult CreateWizard([Bind(Include = "Id,ParameterName,ParameterValue,ActionArgumentsID")] ActionArgs actionargs, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.ActionArgss.Add(actionargs);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        return View(actionargs);
    }
    
    /// <summary>GET: /ActionArgs/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="ActionTypeId">     Identifier for the action type.</param>
    /// <param name="BusinessRuleId">   Identifier for the business rule.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string ActionTypeId, string BusinessRuleId)
    {
        if(!User.CanAdd("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(ActionTypeId != null && ActionTypeId.Trim() == "2")
        {
            ViewBag.MandatoryRule = true;
        }
        if(ActionTypeId != null && ActionTypeId.Trim() == "4")
        {
            ViewBag.ReadonlyRule = true;
        }
        if(ActionTypeId != null && ActionTypeId.Trim() == "5")
        {
            ViewBag.FilterDropdownRule = true;
        }
        // Sanjeev
        if(ActionTypeId != null && ActionTypeId.Trim() == "6")
        {
            ViewBag.HiddenRule = true;
        }
        //
        if(ActionTypeId != null && ActionTypeId.Trim() == "12")
        {
            ViewBag.GroupsHiddenRule = true;
        }
        if(ActionTypeId != null && ActionTypeId.Trim() == "16")
        {
            ViewBag.VerbsHiddenRule = true;
        }
        if(ActionTypeId != null && ActionTypeId.Trim() == "8")
        {
            ViewBag.InvokeVerbsRule = true;
        }
        if(BusinessRuleId != null)
        {
            BusinessRuleContext dbRuleConditions = new BusinessRuleContext();
            long brid = Convert.ToInt64(BusinessRuleId);
            var brlist = dbRuleConditions.BusinessRules.Where(p => p.Id == brid).ToList();
            ViewBag.EntityName = brlist[0].EntityName;
            //customized
            if(brlist.Count > 0)
            {
                ViewBag.PropertyListActionArg = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => !p.IsAdminEntity && p.Name == brlist[0].EntityName).Properties, "Name", "DisplayName");
            } //customized
        }
        if(HostingEntityName == "RuleAction" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objActionArguments = new List<RuleAction>();
            ViewBag.ActionArgumentsID = new SelectList(objActionArguments, "ID", "DisplayValue");
        }
        if(HostingEntityID != "")
        {
            long actionArgumentsID = Convert.ToInt64(HostingEntityID);
            List<ActionArgs> listArgs = db.ActionArgss.Where(a => a.ActionArgumentsID == actionArgumentsID).ToList();
            if(listArgs.Count() > 0)
            {
                foreach(var item in listArgs)
                {
                    if(ActionTypeId != null && (ActionTypeId.Trim() == "8" || ActionTypeId.Trim() == "16"))
                        ViewBag.Parameters += item.ParameterName + "." + item.ParameterValue + ",";
                    else
                        ViewBag.Parameters += item.ParameterName + ",";
                }
            }
        }
        ViewData["ActionArgsParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>GET: /ActionArgs/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">         The URL referrer.</param>
    /// <param name="HostingEntityName">   Name of the hosting entity.</param>
    /// <param name="HostingEntityID">     Identifier for the hosting entity.</param>
    /// <param name="ActionTypeId">        Identifier for the action type.</param>
    /// <param name="BusinessRuleId">      Identifier for the business rule.</param>
    /// <param name="EntityNameRuleAction">The entity name rule action.</param>
    ///
    /// <returns>A response stream to send to the CreateQuickEmail View.</returns>
    
    public ActionResult CreateQuickEmail(string UrlReferrer, string HostingEntityName, string HostingEntityID, string ActionTypeId, string BusinessRuleId, string EntityNameRuleAction)
    {
        ViewBag.HostingEntityID = HostingEntityID;
        ViewBag.HostingEntityName = HostingEntityName;
        ViewBag.UrlReferrer = UrlReferrer;
        ViewBag.ActionTypeId = ActionTypeId;
        ViewBag.BusinessRuleId = BusinessRuleId;
        ViewBag.EntityNameRuleAction = EntityNameRuleAction;
        if(!User.CanAdd("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        //
        if(HostingEntityID != "")
        {
            long actionArgumentsID = Convert.ToInt64(HostingEntityID);
            string ErrorMessage = dbActionArguments.RuleActions.Where(p => p.Id == actionArgumentsID).FirstOrDefault(p => p.ActionName == "TimeBasedAlert").ErrorMessage;
            if(ErrorMessage != "")
                ViewBag.AlertMessageArg = ErrorMessage;
            var TemplateId = dbActionArguments.RuleActions.Where(p => p.Id == actionArgumentsID).FirstOrDefault(p => p.ActionName == "TimeBasedAlert").TemplateId;
            if(TemplateId != null)
                ViewBag.TemplateId = TemplateId;
            List<ActionArgs> listArgs = db.ActionArgss.Where(a => a.ActionArgumentsID == actionArgumentsID).ToList();
            if(listArgs.Count() > 0)
            {
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
                //NotifyRoleList
                var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
                ViewBag.NotifyRoleListArg = new SelectList(RoleListNotify, "Key", "Value");
                ViewBag.cmbNotifyToArg = new SelectList(GetAllAssociationsofEntity(EntityNameRuleAction), "Key", "Value");
                foreach(var item in listArgs)
                {
                    if(item.ParameterName == "TimeValue")
                        ViewBag.TimeValueArg = item.ParameterValue;
                    if(item.ParameterName == "NotifyToExtra")
                        ViewBag.NotifyToExtraArg = item.ParameterValue;
                    if(item.ParameterName == "NotifyToRole")
                    {
                        ViewBag.NotifyRoleListArgs = item.ParameterValue;
                    }
                    if(item.ParameterName == "NotifyTo")
                    {
                        ViewBag.NotifyToArgs = item.ParameterValue;
                    }
                    if(item.ParameterName == "IsEmailNotification")
                    {
                        ViewBag.IsEmailNotification = item.ParameterValue == "1" ? true : false;
                    }
                    if(item.ParameterName == "IsWebNotification")
                    {
                        ViewBag.IsWebNotification = item.ParameterValue == "1" ? true : false; ;
                    }
                }
            }
        }
        ViewData["ActionArgsParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>Gets user association.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>The user association.</returns>
    
    public Dictionary<string, string> GetUserAssociation(string Entity)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("Owner", "Owner");
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        foreach(var asso in Ent.Associations)
        {
            if(asso.Target == "IdentityUser")
                list.Add(asso.AssociationProperty, asso.DisplayName);
            else
            {
                var assoEntity = ModelReflector.Entities.First(p => p.Name == asso.Target);
                foreach(var asso1 in assoEntity.Associations)
                {
                    if(asso1.Target == "IdentityUser")
                        list.Add(asso.AssociationProperty, asso.DisplayName);
                }
            }
        }
        var directUserAsso = Ent.Associations.Where(p => p.Target == "IdentityUser");
        return list;
    }
    
    /// <summary>Gets all associationsof entity.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>all associationsof entity.</returns>
    
    public Dictionary<string, string> GetAllAssociationsofEntity(string Entity)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        list.Add("Owner", "LoggedInUser");
        var lstemailProp = Ent.Properties.Where(p => p.IsEmailValidation);
        if(lstemailProp.Count() > 0)
        {
            foreach(var emailprop in lstemailProp)
            {
                list.Add(Ent.Name + "." + emailprop.Name, Ent.DisplayName + "." + emailprop.DisplayName);
            }
        }
        foreach(var asso in Ent.Associations)
        {
            if(asso.Target == "IdentityUser")
                list.Add(Ent.Name + "." + asso.AssociationProperty, Ent.DisplayName + "." + asso.DisplayName);
            else
            {
                var assoEntity = ModelReflector.Entities.First(p => p.Name == asso.Target);
                foreach(var asso1 in assoEntity.Associations)
                {
                    if(asso1.Target == "IdentityUser")
                    {
                        if(!list.ContainsKey(asso1.Name + "." + asso1.AssociationProperty))
                            list.Add(asso.Name + "." + asso1.AssociationProperty, asso.DisplayName + "[" + asso1.DisplayName + "]");
                    }
                    else
                    {
                        var lstassoemailProperties = assoEntity.Properties.Where(p => p.IsEmailValidation);
                        foreach(var emailprop in lstassoemailProperties)
                        {
                            if(!list.ContainsKey(asso.Name + "." + emailprop.Name))
                                list.Add(asso.Name + "." + emailprop.Name, asso.DisplayName + "[" + emailprop.DisplayName + "]");
                        }
                    }
                }
            }
        }
        return list;
        //return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates quick email.</summary>
    ///
    /// <param name="UrlReferrer">         The URL referrer.</param>
    /// <param name="HostingEntityID">     Identifier for the hosting entity.</param>
    /// <param name="ActionTypeId">        Identifier for the action type.</param>
    /// <param name="BusinessRuleId">      Identifier for the business rule.</param>
    /// <param name="TimeValueArg">        The time value argument.</param>
    /// <param name="NotifyToArg">         The notify to argument.</param>
    /// <param name="NotifyToRoleArg">     The notify to role argument.</param>
    /// <param name="NotifyToExtraArg">    The notify to extra argument.</param>
    /// <param name="AlertMessageArg">     The alert message argument.</param>
    /// <param name="EntityNameRuleAction">The entity name rule action.</param>
    ///
    /// <returns>A response stream to send to the CreateQuickEmail View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    public ActionResult CreateQuickEmail(string UrlReferrer, string HostingEntityID, string ActionTypeId, string BusinessRuleId, string TimeValueArg, string NotifyToArg, string NotifyToRoleArg, string NotifyToExtraArg, string AlertMessageArg, string EntityNameRuleAction, bool? IsEmailNotification, bool? IsWebNotification, HttpPostedFileBase Template)
    {
        if(ModelState.IsValid)
        {
            if(HostingEntityID != "")
            {
                bool HasNotifyToExtra = false;
                bool HasNotifyToRole = false;
                long actionID = Convert.ToInt64(HostingEntityID);
                var hasEmail = false;
                var hasWeb = false;
                //save Error Message
                RuleAction rulerction1 = dbActionArguments.RuleActions.Where(p => p.Id == actionID).FirstOrDefault(p => p.ActionName == "TimeBasedAlert");
                if(rulerction1.ErrorMessage != AlertMessageArg || Template != null)
                {
                    if(Template != null)
                        rulerction1.TemplateId = long.Parse(SaveAnyDocument(Template, "byte", rulerction1.TemplateId));
                    rulerction1.ErrorMessage = AlertMessageArg;
                    dbActionArguments.Entry(rulerction1).State = EntityState.Modified;
                    dbActionArguments.SaveChanges();
                }
                //
                //save action args
                List<ActionArgs> listArgs = db.ActionArgss.Where(a => a.ActionArgumentsID == actionID).ToList();
                foreach(var item in listArgs)
                {
                    if(item.ParameterName == "TimeValue" && item.ParameterValue != TimeValueArg)
                    {
                        ActionArgs ActionArgTimeValue = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "TimeValue");
                        ActionArgTimeValue.ParameterValue = TimeValueArg;
                        db.Entry(ActionArgTimeValue).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if(item.ParameterName == "NotifyToExtra" && item.ParameterName != NotifyToExtraArg)
                    {
                        ActionArgs ActionArgNotifyToExtra = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "NotifyToExtra");
                        if(ActionArgNotifyToExtra != null)
                        {
                            if(!string.IsNullOrEmpty(NotifyToExtraArg))
                            {
                                ActionArgNotifyToExtra.ParameterValue = NotifyToExtraArg;
                                db.Entry(ActionArgNotifyToExtra).State = EntityState.Modified;
                                db.SaveChanges();
                                HasNotifyToExtra = true;
                            }
                            else
                            {
                                db.ActionArgss.Remove(ActionArgNotifyToExtra);
                                db.SaveChanges();
                            }
                        }
                    }
                    if(item.ParameterName == "NotifyToRole")
                    {
                        ActionArgs ActionArgNotifyToRole = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "NotifyToRole");
                        if(ActionArgNotifyToRole != null)
                        {
                            if(!string.IsNullOrEmpty(NotifyToRoleArg))
                            {
                                string[] RolesIdstr = NotifyToRoleArg.Split(",".ToCharArray());
                                var target = "0";
                                var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                                if(results.FirstOrDefault() != null && results[0].Equals("0"))
                                    NotifyToRoleArg = "0";
                                ActionArgNotifyToRole.ParameterValue = NotifyToRoleArg;
                                db.Entry(ActionArgNotifyToRole).State = EntityState.Modified;
                                db.SaveChanges();
                                HasNotifyToRole = true;
                            }
                            else
                            {
                                //ActionArgNotifyToRole.ParameterValue = "0";
                                //db.Entry(ActionArgNotifyToRole).State = EntityState.Modified;
                                //db.SaveChanges();
                                db.ActionArgss.Remove(ActionArgNotifyToRole);
                                db.SaveChanges();
                            }
                        }
                    }
                    if(item.ParameterName == "NotifyTo" && item.ParameterValue != NotifyToArg)
                    {
                        ActionArgs ActionArgNotifyToArg = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "NotifyTo");
                        if(ActionArgNotifyToArg != null)
                        {
                            if(!string.IsNullOrEmpty(NotifyToArg))
                            {
                                ActionArgNotifyToArg.ParameterValue = NotifyToArg;
                                db.Entry(ActionArgNotifyToArg).State = EntityState.Modified;
                                db.SaveChanges();
                                // HasNotifyToExtra = true;
                            }
                            else
                            {
                                ActionArgNotifyToArg.ParameterValue = "Owner";
                                db.Entry(ActionArgNotifyToArg).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    //for notification
                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers.NotificationController");
                    if(controller != null)
                    {
                        if(IsEmailNotification.HasValue)
                        {
                            ActionArgs ActionArgIsEmailNotification = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "IsEmailNotification");
                            if(ActionArgIsEmailNotification != null)
                            {
                                hasEmail = true;
                                var isEmail = IsEmailNotification.Value == true ? "1" : "0";
                                ActionArgIsEmailNotification.ParameterValue = isEmail;
                                db.Entry(ActionArgIsEmailNotification).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        if(IsWebNotification.HasValue)
                        {
                            ActionArgs ActionArgIsWebNotification = db.ActionArgss.Where(p => p.Id == item.Id).FirstOrDefault(p => p.ParameterName == "IsWebNotification");
                            if(ActionArgIsWebNotification != null)
                            {
                                hasWeb = true;
                                var isWeb = IsWebNotification.Value == true ? "1" : "0";
                                ActionArgIsWebNotification.ParameterValue = isWeb;
                                db.Entry(ActionArgIsWebNotification).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    //
                }
                //for notifiction
                Type controller1 = Type.GetType("GeneratorBase.MVC.Controllers.NotificationController");
                if(controller1 != null)
                {
                    if(!hasEmail)
                    {
                        var isEmail = IsEmailNotification == true ? "1" : "0";
                        ActionArgs newNotifyToExtraArg = new ActionArgs();
                        newNotifyToExtraArg.ParameterName = "IsEmailNotification";
                        newNotifyToExtraArg.ParameterValue = isEmail;
                        newNotifyToExtraArg.ActionArgumentsID = actionID;
                        db.ActionArgss.Add(newNotifyToExtraArg);
                        db.SaveChanges();
                    }
                    if(!hasWeb)
                    {
                        var isWeb = IsWebNotification == true ? "1" : "0";
                        ActionArgs newNotifyToExtraArg = new ActionArgs();
                        newNotifyToExtraArg.ParameterName = "IsWebNotification";
                        newNotifyToExtraArg.ParameterValue = isWeb;
                        newNotifyToExtraArg.ActionArgumentsID = actionID;
                        db.ActionArgss.Add(newNotifyToExtraArg);
                        db.SaveChanges();
                    }
                }
                //
                if(!string.IsNullOrEmpty(NotifyToExtraArg) && !HasNotifyToExtra)
                {
                    ActionArgs newNotifyToExtraArg = new ActionArgs();
                    newNotifyToExtraArg.ParameterName = "NotifyToExtra";
                    newNotifyToExtraArg.ParameterValue = NotifyToExtraArg;
                    newNotifyToExtraArg.DisplayValue = "NotifyToExtra";
                    newNotifyToExtraArg.ActionArgumentsID = actionID;
                    db.ActionArgss.Add(newNotifyToExtraArg);
                    db.SaveChanges();
                }
                if(!string.IsNullOrEmpty(NotifyToRoleArg) && !HasNotifyToRole)
                {
                    ActionArgs newNotifyToRole = new ActionArgs();
                    string[] RolesIdstr = NotifyToRoleArg.Split(",".ToCharArray());
                    var target = "0";
                    var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                    if(results.FirstOrDefault() != null && results[0].Equals("0"))
                        NotifyToRoleArg = "0";
                    newNotifyToRole.ParameterName = "NotifyToRole";
                    newNotifyToRole.ParameterValue = NotifyToRoleArg;
                    newNotifyToRole.DisplayValue = "NotifyToRole";
                    newNotifyToRole.ActionArgumentsID = actionID;
                    db.ActionArgss.Add(newNotifyToRole);
                    db.SaveChanges();
                }
            }
            return Redirect(UrlReferrer);
        }
        if(!User.CanAdd("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        //
        if(HostingEntityID != "")
        {
            long actionArgumentsID = Convert.ToInt64(HostingEntityID);
            string ErrorMessage = dbActionArguments.RuleActions.Where(p => p.Id == actionArgumentsID).FirstOrDefault(p => p.ActionName == "TimeBasedAlert").ErrorMessage;
            if(ErrorMessage != "")
                ViewBag.AlertMessageArg = ErrorMessage;
            List<ActionArgs> listArgs = db.ActionArgss.Where(a => a.ActionArgumentsID == actionArgumentsID).ToList();
            if(listArgs.Count() > 0)
            {
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
                //NotifyRoleList
                var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
                ViewBag.NotifyRoleListArg = new SelectList(RoleListNotify, "Key", "Value");
                ViewBag.cmbNotifyToArg = new SelectList(GetAllAssociationsofEntity(EntityNameRuleAction), "Key", "Value");
                foreach(var item in listArgs)
                {
                    if(item.ParameterName == "TimeValue")
                        ViewBag.TimeValueArg = item.ParameterValue;
                    if(item.ParameterName == "NotifyToExtra")
                        ViewBag.NotifyToExtraArg = item.ParameterValue;
                    if(item.ParameterName == "NotifyToRole")
                    {
                        ViewBag.NotifyRoleListArgs = item.ParameterValue;
                    }
                    if(item.ParameterName == "NotifyTo")
                    {
                        ViewBag.NotifyToArgs = item.ParameterValue;
                    }
                }
            }
        }
        ViewData["ActionArgsParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>POST: /ActionArgs/CreateQuick To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actionargs"> The actionargs.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ParameterName,ParameterValue,ActionArgumentsID")] ActionArgs actionargs, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            List<ActionArgs> listArgs = db.ActionArgss.Where(a => a.ActionArgumentsID == actionargs.ActionArgumentsID).ToList();
            if(listArgs.Count() > 0)
            {
                foreach(var item in listArgs)
                {
                    ActionArgs actionargsDel = db.ActionArgss.Where(s => s.ActionArgumentsID == item.ActionArgumentsID).FirstOrDefault();
                    db.ActionArgss.Remove(actionargsDel);
                    db.SaveChanges();
                }
            }
            //foreach (string param in actionargs.ParameterName.Split(",".ToCharArray()))
            //{
            //    var paramname = param.Trim();
            //    if (!string.IsNullOrEmpty(paramname))
            //    {
            //        ActionArgs obj = new ActionArgs();
            //        obj = actionargs;
            //        obj.ParameterName = paramname;
            //        db.ActionArgss.Add(obj);
            //        db.SaveChanges();
            //    }
            //}
            bool isVerbsHidden = actionargs.ParameterValue.Trim() == "Set as Entity Verb(S) Hidden" ? true : false;
            bool isInvokeAction = actionargs.ParameterValue.Trim() == "Set as Invoke Action" ? true : false;
            foreach(string param in actionargs.ParameterName.Split(",".ToCharArray()))
            {
                var paramname = param.Trim();
                if(!string.IsNullOrEmpty(paramname))
                {
                    ActionArgs obj = new ActionArgs();
                    obj = actionargs;
                    if(isVerbsHidden || isInvokeAction)
                    {
                        var verbs = paramname.Split(".".ToCharArray(), 2);
                        obj.ParameterName = verbs[0].Trim();
                        var parmval = String.Join(",", verbs.Skip(2)).Trim();
                        if(parmval == "")
                            obj.ParameterValue = String.Join(",", verbs.Skip(1)).Trim();
                        else
                            obj.ParameterValue = verbs[1].Trim();
                    }
                    else
                        obj.ParameterName = paramname;
                    db.ActionArgss.Add(obj);
                    db.SaveChanges();
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        return View(actionargs);
    }
    
    /// <summary>Cancels.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    
    public ActionResult Cancel(string UrlReferrer)
    {
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>POST: /ActionArgs/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actionargs"> The actionargs.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ParameterName,ParameterValue,ActionArgumentsID")] ActionArgs actionargs, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.ActionArgss.Add(actionargs);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        return View(actionargs);
    }
    
    /// <summary>GET: /ActionArgs/Edit/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id)
    {
        if(!User.CanEdit("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionArgs actionargs = db.ActionArgss.Find(id);
        if(actionargs == null)
        {
            return HttpNotFound();
        }
        var _objActionArguments = new List<RuleAction>();
        _objActionArguments.Add(actionargs.actionarguments);
        ViewBag.ActionArgumentsID = new SelectList(_objActionArguments, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        if(ViewData["ActionArgsParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionArgs"))
            ViewData["ActionArgsParentUrl"] = Request.UrlReferrer;
        return View(actionargs);
    }
    
    /// <summary>POST: /ActionArgs/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actionargs"> The actionargs.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ParameterName,ParameterValue,ActionArgumentsID")] ActionArgs actionargs, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(actionargs).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        return View(actionargs);
    }
    
    /// <summary>GET: /ActionArgs/EditWizard/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id)
    {
        if(!User.CanEdit("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionArgs actionargs = db.ActionArgss.Find(id);
        if(actionargs == null)
        {
            return HttpNotFound();
        }
        var _objActionArguments = new List<RuleAction>();
        _objActionArguments.Add(actionargs.actionarguments);
        ViewBag.ActionArgumentsID = new SelectList(_objActionArguments, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        if(ViewData["ActionArgsParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionArgs"))
            ViewData["ActionArgsParentUrl"] = Request.UrlReferrer;
        return View(actionargs);
    }
    
    /// <summary>POST: /ActionArgs/EditWizard/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actionargs"> The actionargs.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ParameterName,ParameterValue,ActionArgumentsID")] ActionArgs actionargs, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(actionargs).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        ViewBag.ActionArgumentsID = new SelectList(dbActionArguments.RuleActions, "ID", "DisplayValue", actionargs.ActionArgumentsID);
        return View(actionargs);
    }
    
    /// <summary>GET: /ActionArgs/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int? id)
    {
        if(!User.CanDelete("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionArgs actionargs = db.ActionArgss.Find(id);
        if(actionargs == null)
        {
            return HttpNotFound();
        }
        if(ViewData["ActionArgsParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionArgs"))
            ViewData["ActionArgsParentUrl"] = Request.UrlReferrer;
        return View(actionargs);
    }
    
    /// <summary>POST: /ActionArgs/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        if(!User.CanDelete("ActionArgs"))
        {
            return RedirectToAction("Index", "Error");
        }
        ActionArgs actionargs = db.ActionArgss.Find(id);
        db.ActionArgss.Remove(actionargs);
        db.SaveChanges();
        if(ViewData["ActionArgsParentUrl"] != null)
        {
            string parentUrl = ViewData["ActionArgsParentUrl"].ToString();
            ViewData["ActionArgsParentUrl"] = null;
            return Redirect(parentUrl);
        }
        else return RedirectToAction("Index");
    }
    
    /// <summary>Sets f search.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="HostingEntity">The hosting entity.</param>
    ///
    /// <returns>A response stream to send to the SetFSearch View.</returns>
    
    public ActionResult SetFSearch(string searchString, string HostingEntity)
    {
        ViewBag.CurrentFilter = searchString;
        var lstActionArgs = from s in db.ActionArgss
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionArgs = searchRecords(lstActionArgs, searchString.ToUpper());
        }
        else
            lstActionArgs = lstActionArgs.OrderByDescending(s => s.Id);
        if(lstActionArgs.Where(p => p.ActionArgumentsID != null).Select(p => p.ActionArgumentsID).Distinct().Count() <= 50)
            ViewBag.actionarguments = new SelectList(lstActionArgs.Where(p => p.actionarguments != null).Select(P => P.actionarguments).Distinct(), "ID", "DisplayValue");
        return View();
    }
    
    /// <summary>GET: /ActionArgs/FSearch/.</summary>
    ///
    /// <param name="currentFilter">  A filter specifying the current.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="FSFilter">       A filter specifying the file system.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="search">         The search.</param>
    /// <param name="IsExport">       The is export.</param>
    /// <param name="actionarguments">The actionarguments.</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string actionarguments)//, string HostingEntity
    {
        ViewBag.SearchResult = "";
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        if(!string.IsNullOrEmpty(searchString) && FSFilter == null)
            ViewBag.FSFilter = "Fsearch";
        var lstActionArgs = from s in db.ActionArgss
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionArgs = searchRecords(lstActionArgs, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstActionArgs = sortRecords(lstActionArgs, sortBy, isAsc);
        }
        else
            lstActionArgs = lstActionArgs.OrderByDescending(s => s.Id);
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search;
            lstActionArgs = searchRecords(lstActionArgs, search);
        }
        lstActionArgs = lstActionArgs.Include(t => t.actionarguments);
        var _ActionArgs = lstActionArgs;
        if(lstActionArgs.Where(p => p.actionarguments != null).Count() <= 50)
            ViewBag.actionarguments = new SelectList(lstActionArgs.Where(p => p.actionarguments != null).Select(P => P.actionarguments).Distinct(), "ID", "DisplayValue");
        if(actionarguments != null)
        {
            var ids = actionarguments.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            ViewBag.SearchResult += "\r\n Action= ";
            foreach(var str in ids)
                if(!string.IsNullOrEmpty(str))
                {
                    ids1.Add(Convert.ToInt64(str));
                    ViewBag.SearchResult += dbActionArguments.RuleActions.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                }
            _ActionArgs = _ActionArgs.Where(p => ids1.ToList().Contains(p.ActionArgumentsID));
        }
        ViewBag.SearchResult = ((string)ViewBag.SearchResult).TrimStart("\r\n".ToCharArray());
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ActionArgs.Count() > 0)
                pageSize = _ActionArgs.Count();
            return View("ExcelExport", _ActionArgs.ToPagedList(pageNumber, pageSize));
        }
        return View("Index", _ActionArgs.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        var _Obj = db.ActionArgss.Find(Convert.ToInt64(id));
        return _Obj == null ? "" : _Obj.DisplayValue;
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string key, string AssoNameWithParent, string AssociationID)
    {
        IQueryable<ActionArgs> list = db.ActionArgss.Unfiltered();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.ActionArgss;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(ActionArgs), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<ActionArgs, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<ActionArgs>)q);
            }
        }
        if(key != null && key.Length > 0)
        {
            var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <param name="file">The file.</param>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    public ActionResult Upload(HttpPostedFileBase file)
    {
        if(Request.Files["FileUpload"].ContentLength > 0)
        {
            string fileExtension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);
            if(fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelFiles"), Request.Files["FileUpload"].FileName);
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                Request.Files["FileUpload"].SaveAs(fileLocation);
                string excelConnectionString = string.Empty;
                if(fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if(fileExtension == ".xlsx")
                {
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();
                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if(dt == null)
                {
                    return null;
                }
                String[] excelSheets = new String[dt.Rows.Count];
                int t = 0;
                foreach(DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                excelConnection.Close();
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                DataSet objDataSet = new DataSet();
                string query = string.Format("Select * from [{0}]", excelSheets[0]);
                using(OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                {
                    dataAdapter.Fill(objDataSet);
                }
                excelConnection1.Close();
                var col = new List<SelectListItem>();
                if(objDataSet.Tables.Count > 0)
                {
                    int iCols = objDataSet.Tables[0].Columns.Count;
                    if(iCols > 0)
                    {
                        for(int i = 0; i < iCols; i++)
                        {
                            col.Add(new SelectListItem { Value = (i + 1).ToString(), Text = objDataSet.Tables[0].Columns[i].Caption });
                        }
                    }
                }
                col.Insert(0, new SelectListItem { Value = "", Text = "Select Column" });
                Dictionary<GeneratorBase.MVC.ModelReflector.Property, List<SelectListItem>> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, List<SelectListItem>>();
                var entList = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == "Issue").ToList()[0];
                if(entList != null)
                {
                    foreach(var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                    {
                        objColMap.Add(prop, col);
                    }
                }
                ViewBag.ColumnMapping = objColMap;
                ViewBag.FilePath = fileLocation;
            }
            else
            {
                ModelState.AddModelError("", "Plese select Excel File.");
            }
        }
        return View("Index");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) import data.</summary>
    ///
    /// <param name="collection">The collection.</param>
    ///
    /// <returns>A response stream to send to the ImportData View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ImportData(FormCollection collection)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["lblColumn"];
        var selectedlist = collection["colList"];
        string fileLocation = FilePath;
        string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation);
        if(fileExtension == ".xls" || fileExtension == ".xlsx")
        {
            if(fileExtension == ".xls")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if(fileExtension == ".xlsx")
            {
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            excelConnection.Open();
            DataTable dt = new DataTable();
            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if(dt == null)
            {
                return null;
            }
            String[] excelSheets = new String[dt.Rows.Count];
            int t = 0;
            foreach(DataRow row in dt.Rows)
            {
                excelSheets[t] = row["TABLE_NAME"].ToString();
                t++;
            }
            excelConnection.Close();
            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
            DataSet objDataSet = new DataSet();
            string query = string.Format("Select * from [{0}]", excelSheets[0]);
            using(OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
            {
                dataAdapter.Fill(objDataSet);
            }
            excelConnection1.Close();
            if(selectedlist != null && columnlist != null)
            {
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    ActionArgs model = new ActionArgs();
                    var tblColumns = columnlist.Split(',').ToList();
                    var sheetColumns = selectedlist.Split(',').ToList();
                    for(int j = 0; j < sheetColumns.Count; j++)
                    {
                        string columntable = tblColumns[j];
                        int columnSheet = 0;
                        if(string.IsNullOrEmpty(sheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                        string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString();
                        if(string.IsNullOrEmpty(columnValue))
                            continue;
                        switch(columntable)
                        {
                        case "ParameterName":
                            model.ParameterName = columnValue;
                            break;
                        case "ParameterValue":
                            model.ParameterValue = columnValue;
                            break;
                        case "ActionArgumentsID":
                            long actionargumentsId = dbActionArguments.RuleActions.Where(p => p.DisplayValue == columnValue).ToList()[0].Id;
                            model.ActionArgumentsID = actionargumentsId;
                            break;
                        default:
                            break;
                        }
                    }
                    db.ActionArgss.Add(model);
                }
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            return RedirectToAction("Index");
        }
        return View();
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
    
}
}