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
using System.Globalization;
using System.Reflection;


namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling business rules.</summary>
public class BusinessRuleController : BaseController
{
    /// <summary>The database.</summary>
    private BusinessRuleContext db = new BusinessRuleContext();
    /// <summary>Type of the database associated business rule.</summary>
    private BusinessRuleTypeContext dbAssociatedBusinessRuleType = new BusinessRuleTypeContext();
    
    /// <summary>GET: /BusinessRule/.</summary>
    ///
    /// <param name="currentFilter">    A filter specifying the current.</param>
    /// <param name="searchString">     The search string.</param>
    /// <param name="sortBy">           Describes who sort this object.</param>
    /// <param name="isAsc">            The is ascending.</param>
    /// <param name="page">             The page.</param>
    /// <param name="itemsPerPage">     The items per page.</param>
    /// <param name="HostingEntity">    The hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsExport">         The is export.</param>
    /// <param name="IsFilter">         A filter specifying the is.</param>
    /// <param name="RenderPartial">    True to render partial.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="IsDisable">        The is disable.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsFilter, bool? RenderPartial, string HostingEntityName, string IsDisable, bool? showInvalid)
    {
        if(!((CustomPrincipal)User).CanViewAdminFeature("BusinessRule"))
            return RedirectToAction("Index", "Home");
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["IsFilter"] = IsFilter;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["IsDisable"] = IsDisable;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstBusinessRule = from s in db.BusinessRules
                              select s;
        if(showInvalid.HasValue && showInvalid == true)
        {
            var invalidids = getAllInvalidBR();
            lstBusinessRule = lstBusinessRule.Where(p => invalidids.Contains(p.Id));
            ViewData["showInvalid"] = true;
        }
        if(!String.IsNullOrEmpty(searchString))
        {
            lstBusinessRule = searchRecords(lstBusinessRule, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstBusinessRule = sortRecords(lstBusinessRule, sortBy, isAsc);
        }
        else
            lstBusinessRule = lstBusinessRule.OrderByDescending(s => s.Id);
        var _BusinessRule = lstBusinessRule;//.Include(t => t.associatedbusinessruletypes);
        if(!string.IsNullOrEmpty(HostingEntityName))
        {
            if(!string.IsNullOrEmpty(HostingEntityName))
            {
                string hostName = Convert.ToString(HostingEntityName);
                _BusinessRule = _BusinessRule.Where(p => p.EntityName == hostName);
            }
            else
                _BusinessRule = _BusinessRule.Where(p => p.EntityName == "");
        }
        if(!string.IsNullOrEmpty(HostingEntity) && HostingEntity == "RuleAction" && HostingEntityID > 0)
        {
            _BusinessRule = _BusinessRule.Where(p => p.ruleaction.Any(a => a.AssociatedActionTypeID == HostingEntityID));
        }
        if(!string.IsNullOrEmpty(IsDisable))
        {
            if(!string.IsNullOrEmpty(IsDisable))
            {
                bool IsDisableVal = Convert.ToBoolean(IsDisable);
                _BusinessRule = _BusinessRule.Where(p => p.Disable == IsDisableVal);
            }
            else
                _BusinessRule = _BusinessRule.Where(p => p.EntityName == "");
        }
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        pageSize = pageSize > 100 ? 100 : pageSize;
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + HostingEntityName + "BusinessRule"].Value);
            ViewBag.Pages = pageNumber;
        }
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        if(pageSize == -1)
        {
            pageNumber = 1;
            var totalcount = _BusinessRule.Count();
            pageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(pageNumber > 1)
        {
            var totalListCount = _BusinessRule.Count();
            int quotient = totalListCount / pageSize;
            var remainder = totalListCount % pageSize;
            var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
            if(pageNumber > maxpagenumber)
                pageNumber = 1;
        }
        ViewBag.Pages = pageNumber;
        //Cookies for pagesize
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_BusinessRule.Count() > 0)
                pageSize = _BusinessRule.Count();
            return View("ExcelExport", _BusinessRule.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_BusinessRule.ToPagedList(pageNumber, pageSize));
        else
            return PartialView("IndexPartial", _BusinessRule.ToPagedList(pageNumber, pageSize));
    }
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public List<long> getAllInvalidBR()
    {
        var InvalidBRIds = "";
        var EntityList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault);
        var lstBusinessRule = (from s in db.BusinessRules
                               select s).ToList();
        foreach(var BR in lstBusinessRule)
        {
            var propertyValue = "";
            var entity = EntityList.Where(p => p.Name == BR.EntityName).ToList();
            if(entity.Count == 0)
                InvalidBRIds += BR.Id + ",";
            else
            {
                var check = BR.ruleaction.ToList();
                var groupnames = GetGroupsofEntity(BR.EntityName).Data as List<KeyValuePair<string, string>>;
                var propertynames = GetPropertiesofEntityWithInLineProperties(BR.EntityName).Data as Dictionary<string, string>;
                var verbnames = GetVerbsofEntity(BR.EntityName).Data as List<KeyValuePair<string, string>>;
                foreach(var actionarg in check)
                {
                    var typeno = actionarg.AssociatedActionTypeID;
                    var actionargslist = actionarg.actionarguments.ToList();
                    var paramnames = actionargslist.Select(p => p.ParameterName).ToList();
                    var paramvalues = actionargslist.Select(p => p.ParameterValue).ToList();
                    if(paramnames.Count() > 0)
                    {
                        foreach(var param in paramnames)
                        {
                            if(typeno == 12)
                            {
                                var name = param.Split('|');
                                propertyValue = "";
                                if(groupnames.Count() > 0)
                                {
                                    if(groupnames.Where(p => p.Value.Remove(p.Value.Length - 2) == name[1]).Count() > 0)
                                        propertyValue = name[1];
                                    else
                                        break;
                                }
                            }
                            else if(typeno == 2 || typeno == 4 || typeno == 6)
                            {
                                propertyValue = "";
                                if(propertynames.Count() > 0)
                                {
                                    if(propertynames.Where(p => p.Key == param).Count() > 0)
                                        propertyValue = param;
                                    else
                                        break;
                                }
                            }
                            else if(typeno == 16)
                            {
                                if(verbnames.Count() > 0)
                                {
                                    foreach(var paramvalue in paramvalues)
                                    {
                                        propertyValue = "";
                                        var selectedverb = param + "." + paramvalue;
                                        if(verbnames.Where(p => p.Key == selectedverb).Count() > 0)
                                            propertyValue = param + "." + paramvalue;
                                        else
                                            break;
                                    }
                                }
                            }
                            else
                                propertyValue = "Type not found";
                        }
                    }
                    else
                        propertyValue = "No parameter found";
                }
                if(propertyValue == "")
                    InvalidBRIds += BR.Id + ",";
            }
        }
        var ids = InvalidBRIds.Split(',');
        IEnumerable<BusinessRule> _InvalidBusinessrulelist = Enumerable.Empty<BusinessRule>();
        foreach(var id in ids)
        {
            if(id != "")
                _InvalidBusinessrulelist = _InvalidBusinessrulelist.Concat(lstBusinessRule.Where(p => p.Id == Convert.ToInt64(id)));
        }
        return _InvalidBusinessrulelist.Select(p => p.Id).ToList();
    }
    
    /// <summary>Task history.</summary>
    ///
    /// <param name="RenderPartial">  True to render partial.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the TaskHistory View.</returns>
    
    public ActionResult TaskHistory(bool RenderPartial, string HostingEntity, string HostingEntityID)
    {
        ScheduledTaskHistoryContext contextHistory = new ScheduledTaskHistoryContext();
        var BizId = Convert.ToInt64(HostingEntityID);
        var data = contextHistory.ScheduledTaskHistorys.Where(p => p.BusinessRuleId == BizId);
        return View(data.ToList());
    }
    
    /// <summary>Restart daily task.</summary>
    ///
    /// <returns>A response stream to send to the RestartDailyTask View.</returns>
    
    public ActionResult RestartDailyTask()
    {
        var lstBusinessRule = db.BusinessRules.Where(p => p.AssociatedBusinessRuleTypeID == 5);
        ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext();
        foreach(var rule in lstBusinessRule)
        {
            var callbacks = sthcontext.ScheduledTaskHistorys.Where(p => p.BusinessRuleId == rule.Id && p.Status == "Pending");
            foreach(var c in callbacks)
            {
                Uri myUri = new Uri(c.CallbackUri);
                try
                {
                    Revalee.Client.RevaleeRegistrar.CancelCallback(Guid.Parse(c.GUID), myUri);
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                DateTimeOffset dt = new DateTimeOffset(Convert.ToDateTime(c.RunDateTime));
                Revalee.Client.RevaleeRegistrar.ScheduleCallback(dt, myUri);
            }
        }
        return RedirectToAction("Index");
    }
    
    /// <summary>Gets business rule list.</summary>
    ///
    /// <param name="businessRule">The business rule.</param>
    ///
    /// <returns>The business rule list.</returns>
    
    private Object getBusinessRuleList(IQueryable<BusinessRule> businessRule)
    {
        var _businessRule = from obj in businessRule
                            select new
        {
            RuleName = obj.RuleName,
            EntityName = obj.EntityName,
            Roles = obj.Roles,
            DateCreated1 = obj.DateCreated1,
            ID = obj.Id
        };
        return businessRule;
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstBusinessRule">The list business rule.</param>
    /// <param name="searchString">   The search string.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<BusinessRule> searchRecords(IQueryable<BusinessRule> lstBusinessRule, string searchString)
    {
        searchString = searchString.Trim();
        lstBusinessRule = lstBusinessRule.Where(s => (s.Id != null && SqlFunctions.StringConvert((double)s.Id).Contains(searchString)) || (!String.IsNullOrEmpty(s.RuleName) && s.RuleName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (s.Roles != null && (s.Roles.ToUpper().Contains(searchString))) || (s.associatedbusinessruletype != null && (s.associatedbusinessruletype.DisplayValue.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        try
        {
            var datevalue = Convert.ToDateTime(searchString);
            lstBusinessRule = lstBusinessRule.Union(db.BusinessRules.Where(s => (s.DateCreated1 == datevalue)));
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
        return lstBusinessRule;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstBusinessRule">The list business rule.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<BusinessRule> sortRecords(IQueryable<BusinessRule> lstBusinessRule, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        ParameterExpression paramExpression = Expression.Parameter(typeof(BusinessRule), "BusinessRule");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<BusinessRule>)lstBusinessRule.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstBusinessRule.ElementType, lambda.Body.Type },
                       lstBusinessRule.Expression,
                       lambda));
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var entitylist = db.BusinessRules.Select(p => p.EntityName).Distinct();
        IDictionary<string, string> finallist = new Dictionary<string, string>();
        foreach(var ent in entitylist)
        {
            var displaynameObj = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == ent);
            var displayname = displaynameObj != null ? displaynameObj.DisplayName : "N/A";
            finallist.Add(new KeyValuePair<string, string>(ent, displayname));
        }
        return Json(finallist, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter Rules action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilterRules()
    {
        var ruleactions = (db.BusinessRules.Where(wh => wh.ruleaction.Count() > 0).SelectMany(sm => sm.ruleaction)).Where(wh => wh.associatedactiontype != null).Select(p => new
        {
            p.associatedactiontype.Id,
            p.associatedactiontype.ActionTypeName
        }).Distinct();
        IDictionary<string, string> finallist = new Dictionary<string, string>();
        foreach(var action in ruleactions)
        {
            finallist.Add(new KeyValuePair<string, string>(action.Id.ToString(), action.ActionTypeName));
        }
        return Json(finallist, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>GET: /BusinessRule/Details/5.</summary>
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
        BusinessRule businessrule = db.BusinessRules.Find(id);
        ViewBag.EntityNameRuleAction = businessrule.EntityName;
        if(businessrule == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == id).ToList();
        var _objAssociatedBusinessRuleType = dbAssociatedBusinessRuleType.BusinessRuleTypes;
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(_objAssociatedBusinessRuleType, "ID", "TypeName", businessrule.AssociatedBusinessRuleTypeID);
        LoadViewDataBeforeOnEdit(businessrule);
        return View(businessrule);
    }
    
    /// <summary>GET: /BusinessRule/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "BusinessRuleType" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objAssociatedBusinessRuleType = new List<BusinessRuleType>();
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(objAssociatedBusinessRuleType, "ID", "DisplayValue");
        }
        ViewData["BusinessRuleParentUrl"] = UrlReferrer;
        //
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName");
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "");
        //
        return View();
    }
    
    /// <summary>Creates business rule.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateBusinessRule View.</returns>
    
    public ActionResult CreateBusinessRule(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["BusinessRuleParentUrl"] = UrlReferrer;
        //ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue");
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName");
        Dictionary<string, string> list = new Dictionary<string, string>();
        ViewBag.EntityVerb = new SelectList(list, "key", "value");
        Dictionary<string, string> listhiddenVerb = new Dictionary<string, string>();
        ViewBag.EntityHiddenVerb = new SelectList(listhiddenVerb, "key", "value");
        //list.Add("Owner", "LoggedInUser");
        ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
        Dictionary<string, string> PropertyList = new Dictionary<string, string>();
        PropertyList.Add("0", "--Select Property--");
        ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
        ViewBag.PropertyList7 = ViewBag.PropertyList1 = ViewBag.GroupList = new SelectList(PropertyList, "key", "value");
        ViewBag.AssociationList17 = new SelectList(PropertyList, "key", "value");
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "");
        //NotifyRoleList
        var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
        ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");
        Dictionary<string, string> TimeRuleApplyOn = new Dictionary<string, string>();
        TimeRuleApplyOn.Add("Add", "OnAdd");
        TimeRuleApplyOn.Add("Update", "OnUpdate");
        TimeRuleApplyOn.Add("Add,Update", "OnAdd&Update");
        // TimeRuleApplyOn.Add("Update", "Update");
        TimeRuleApplyOn.Add("OnPropertyChange", "OnPropertyChange");
        ViewBag.TimeRuleApplyOn = new SelectList(TimeRuleApplyOn, "key", "value");
        ViewBag.Dropdown = new SelectList(list, "key", "value");
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                          = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                  = ViewBag.SuggestedDynamicValueInCondition7 = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(list, "key", "value");
        ViewBag.SuggestedDynamicValue721 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                           = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                   = ViewBag.SuggestedDynamicValueInCondition72 = ViewBag.SuggestedDynamicValueInCondition721 = new SelectList(list, "key", "value");
        var objBusinessRuleType = dbAssociatedBusinessRuleType.BusinessRuleTypes;
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(objBusinessRuleType, "TypeNo", "TypeName");
        ViewBag.SuggestedPropertyValue17 = ViewBag.AssociationPropertyList17 = new SelectList(list, "key", "value");
        Dictionary<string, string> IsElseActionList = new Dictionary<string, string>();
        IsElseActionList.Add("0", "True");
        IsElseActionList.Add("1", "False");
        ViewBag.IsElseActionList = new SelectList(IsElseActionList, "key", "value");
        LoadViewDataBeforeOnCreate();
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates business rule new.</summary>
    ///
    /// <param name="businessrule">          The businessrule.</param>
    /// <param name="UrlReferrer">           The URL referrer.</param>
    /// <param name="TimeValue">             The time value.</param>
    /// <param name="NotifyTo">              The notify to.</param>
    /// <param name="NotifyToExtra">         The notify to extra.</param>
    /// <param name="AlertMessage">          Message describing the alert.</param>
    /// <param name="PropertyName">          Name of the property.</param>
    /// <param name="ConditionOperator">     The condition operator.</param>
    /// <param name="ConditionValue">        The condition value.</param>
    /// <param name="PropertyRuleValue">     The property rule value.</param>
    /// <param name="PropertyList">          List of properties.</param>
    /// <param name="RoleListValue">         The role list value.</param>
    /// <param name="PropertyList1Value">    The property list 1 value.</param>
    /// <param name="PropertyList7Value">    The property list 7 value.</param>
    /// <param name="Emailids">              The emailids.</param>
    /// <param name="TimeRuleApplyOnValue">  The time rule apply on value.</param>
    /// <param name="Dropdown">              The dropdown.</param>
    /// <param name="lblrulecondition">      The lblrulecondition.</param>
    /// <param name="NotifyToRole">          The notify to role.</param>
    /// <param name="ScheduledDateTimeEnd">  The scheduled date time end.</param>
    /// <param name="ScheduledDateTimeStart">The scheduled date time start.</param>
    /// <param name="ScheduledDailyTime">    The scheduled daily time.</param>
    /// <param name="lblruletype1">          The first lblruletype.</param>
    /// <param name="lblruletype2">          The second lblruletype.</param>
    /// <param name="lblruletype3">          The third lblruletype.</param>
    /// <param name="lblruletype4">          The fourth lblruletype.</param>
    /// <param name="lblruletype5">          The fifth lblruletype.</param>
    /// <param name="lblruletype6">          The lblruletype 6.</param>
    /// <param name="lblruletype7">          The lblruletype 7.</param>
    /// <param name="lblruletype8">          The lblruletype 8.</param>
    /// <param name="lblruletype10">         The lblruletype 10.</param>
    /// <param name="lblruletype13">         The lblruletype 13.</param>
    /// <param name="lblruletype11">         The lblruletype 11.</param>
    /// <param name="lblruletype12">         The lblruletype 12.</param>
    /// <param name="lblruletype1else">      The lblruletype 1else.</param>
    /// <param name="lblruletype2else">      The lblruletype 2else.</param>
    /// <param name="lblruletype3else">      The lblruletype 3else.</param>
    /// <param name="lblruletype4else">      The lblruletype 4else.</param>
    /// <param name="lblruletype5else">      The lblruletype 5else.</param>
    /// <param name="lblruletype6else">      The lblruletype 6else.</param>
    /// <param name="lblruletype7else">      The lblruletype 7else.</param>
    /// <param name="lblruletype8else">      The lblruletype 8else.</param>
    /// <param name="lblruletype10else">     The lblruletype 10else.</param>
    /// <param name="lblruletype13else">     The lblruletype 13else.</param>
    /// <param name="lblruletype11else">     The lblruletype 11else.</param>
    /// <param name="lblruletype12else">     The lblruletype 12else.</param>
    /// <param name="lblAPIData">            Information describing the label a pi.</param>
    /// <param name="lblAPIJsonNode">        The label a pi JSON node.</param>
    /// <param name="lblAPIHeader">          The label a pi header.</param>
    /// <param name="lblruletype14">         The lblruletype 14.</param>
    /// <param name="lblruletype15">         The lblruletype 15.</param>
    /// <param name="ExternalURL">           URL of the external.</param>
    ///
    /// <returns>A response stream to send to the CreateBusinessRuleNew View.</returns>
    
    [HttpPost]
    [ValidateInput(false)]
    [ValidateAntiForgeryToken]
    public ActionResult CreateBusinessRuleNew([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID,Description,Disable,Freeze,InformationMessage,FailureMessage,T_SchedulerTaskID,t_schedulertask")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage, string PropertyName, string ConditionOperator, string ConditionValue, string PropertyRuleValue, string PropertyList, string RoleListValue, string PropertyList1Value, string PropertyList7Value, string Emailids, string TimeRuleApplyOnValue, string Dropdown, string lblrulecondition, string NotifyToRole, string ScheduledDateTimeEnd, string ScheduledDateTimeStart, string ScheduledDailyTime,
            string lblruletype1, string lblruletype2, string lblruletype3, string lblruletype4, string lblruletype5, string lblruletype6, string lblruletype7, string lblruletype8, string lblruletype10, string lblruletype13, string lblruletype11, string lblruletype12,
            string lblruletype1else, string lblruletype2else, string lblruletype3else, string lblruletype4else, string lblruletype5else, string lblruletype6else, string lblruletype7else, string lblruletype8else,
            string lblruletype10else, string lblruletype13else, string lblruletype11else, string lblruletype12else,
            string lblAPIData, string lblAPIJsonNode, string lblAPIHeader, string lblruletype14, string lblruletype15, string lblruletype16, string lblruletype16else, string ExternalURL,
            bool IsEmailNotification, bool IsWebNotification, string lblruletype17, string lblruletype17else, string lblruletype7forassocprop, HttpPostedFileBase Template)
    {
        if(businessrule.AssociatedBusinessRuleTypeID != 5)
        {
            ModelState.Remove("t_schedulertask.T_Name");
            ModelState.Remove("t_schedulertask.T_StartDateTime");
            ModelState.Remove("T_SchedulerTaskID");
            businessrule.t_schedulertask = null;
            businessrule.T_SchedulerTaskID = null;
        }
        if(ModelState.IsValid)
        {
            if(businessrule.Id > 0)
            {
                BusinessRule businessrule1 = db.BusinessRules.Find(businessrule.Id);
                db.BusinessRules.Remove(businessrule1);
                db.SaveChanges();
            }
            if(RoleListValue == "")
                RoleListValue = "All";
            else
            {
                string[] RolesIdstr = RoleListValue.Split(",".ToCharArray());
                var target = "All";
                var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                if(results.FirstOrDefault() != null && results[0].ToString() == "All")
                    RoleListValue = results[0].ToString();
            }
            businessrule.Roles = RoleListValue;
            businessrule.DateCreated1 = DateTime.UtcNow;
            if(!string.IsNullOrEmpty(lblruletype1) || !string.IsNullOrEmpty(lblruletype1else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "LockEntity";
                newrule.AssociatedActionTypeID = 1;
                if(!string.IsNullOrEmpty(lblruletype1else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype11) || !string.IsNullOrEmpty(lblruletype11else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "LockEntity&Associations";
                newrule.AssociatedActionTypeID = 11;
                if(!string.IsNullOrEmpty(lblruletype11else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype7) || !string.IsNullOrEmpty(lblruletype7else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "SetValue";
                newrule.AssociatedActionTypeID = 7;
                string lblrule = lblruletype7;
                if(!string.IsNullOrEmpty(lblruletype7forassocprop))
                {
                    lblrule = lblruletype7forassocprop;
                }
                if(!string.IsNullOrEmpty(lblruletype7else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype7else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split("?".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(",".ToCharArray());
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = param[0];
                    newactionargs.ParameterValue = param[2];
                    //todo dyanmic
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype8) || !string.IsNullOrEmpty(lblruletype8else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "InvokeAction";
                newrule.AssociatedActionTypeID = 8;
                string lblrule = lblruletype8;
                if(!string.IsNullOrEmpty(lblruletype8else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype8else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split(",".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(".".ToCharArray(), 2);
                    ActionArgs newactionargs = new ActionArgs();
                    //newactionargs.ParameterName = param[0].Trim();
                    //newactionargs.ParameterValue = param[1].Trim();
                    newactionargs.ParameterName = param[0].Trim();
                    var parmval = String.Join(",", param.Skip(2)).Trim();
                    if(parmval == "")
                        newactionargs.ParameterValue = String.Join(",", param.Skip(1)).Trim();
                    else
                        newactionargs.ParameterValue = param[1].Trim();
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype16) || !string.IsNullOrEmpty(lblruletype16else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "MakeVerbsHidden";
                newrule.AssociatedActionTypeID = 16;
                string lblrule = lblruletype16;
                if(!string.IsNullOrEmpty(lblruletype16else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype16else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split(",".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(".".ToCharArray(), 2);
                    ActionArgs newactionargs = new ActionArgs();
                    //newactionargs.ParameterName = param[0].Trim();
                    //newactionargs.ParameterValue = param[1].Trim();
                    newactionargs.ParameterName = param[0].Trim();
                    var parmval = String.Join(",", param.Skip(2)).Trim();
                    if(parmval == "")
                        newactionargs.ParameterValue = String.Join(",", param.Skip(1)).Trim();
                    else
                        newactionargs.ParameterValue = param[1].Trim();
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype6) || !string.IsNullOrEmpty(lblruletype6else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 6;
                string lblrule = lblruletype6;
                if(!string.IsNullOrEmpty(lblruletype6else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype6else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Hidden";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype12) || !string.IsNullOrEmpty(lblruletype12else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 12;
                string lblrule = lblruletype12;
                if(!string.IsNullOrEmpty(lblruletype12else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype12else;
                }
                else
                    newrule.IsElseAction = false;
                var lblrule1 = lblrule.Split("|".ToArray());
                var lblruleId = lblrule1[0].Split(",".ToCharArray());
                var lblruleName = lblrule1[1].Split(",".ToCharArray());
                //foreach (string str in lblrule1[0].Split(",".ToCharArray()))
                for(int cnt = 0; cnt < lblruleId.Length; cnt++)
                {
                    if(string.IsNullOrEmpty(lblruleId[cnt])) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = lblruleId[cnt] + "|" + lblruleName[cnt];
                    newactionargs.ParameterValue = "GroupsHidden";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            //
            //Add For WebHook
            if(!string.IsNullOrEmpty(lblruletype14))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "WEBHOOK";
                newrule.AssociatedActionTypeID = 14;
                string lblrule = lblAPIData;
                var ApiDataSource = lblrule.Split(",".ToArray());
                if(ApiDataSource.Count() > 0)
                {
                    ApplicationContext dbExternalUrl = new ApplicationContext(User);
                    EntityDataSource newWebHook = new EntityDataSource();
                    newWebHook.EntityName = businessrule.EntityName;
                    newWebHook.DataSource = ExternalURL;
                    newWebHook.SourceType = ApiDataSource[0];
                    newWebHook.MethodType = ApiDataSource[1];
                    newWebHook.Action = ApiDataSource[2];
                    //newWebHook.RootNode = ApiDataSource[3];
                    newWebHook.other = "BizRule";
                    newWebHook.flag = false;
                    newWebHook.DisplayValue = businessrule.EntityName + "-" + newWebHook.SourceType + "-" + "-" + newWebHook.Action;
                    var ApiParam = lblAPIHeader.Split("?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if(ApiParam.Count() > 0)
                    {
                        foreach(var param in ApiParam)
                        {
                            var Param = param.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                            if(Param.Count() > 1)
                            {
                                newWebHook.entitydatasourceparameters.Add(new DataSourceParameters { ArgumentName = Param[0], ArgumentValue = Param[1] });
                            }
                        }
                    }
                    var APIJsonNode = lblAPIJsonNode.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if(APIJsonNode.Count() > 0)
                    {
                        foreach(var param in APIJsonNode)
                        {
                            var Param = param.Split("-".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                            if(Param.Count() > 1)
                            {
                                newWebHook.entitypropertymapping.Add(new PropertyMapping { PropertyName = Param[0], DataName = Param[1] });
                            }
                        }
                    }
                    dbExternalUrl.EntityDataSources.Add(newWebHook);
                    dbExternalUrl.SaveChanges();
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = newWebHook.Id.ToString();
                    newactionargs.ParameterValue = "DataSourceId";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            //Confirmation Before Save
            if(!string.IsNullOrEmpty(lblruletype15))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "ConfirmationBeforeSave";
                newrule.DisplayValue = "Confirmation Before Save";
                newrule.AssociatedActionTypeID = 15;
                businessrule.ruleaction.Add(newrule);
            }
            //
            if(!string.IsNullOrEmpty(lblruletype3) || !string.IsNullOrEmpty(lblruletype3else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 2;
                string lblrule = lblruletype3;
                if(!string.IsNullOrEmpty(lblruletype3else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype3else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Mandatory";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype2) || !string.IsNullOrEmpty(lblruletype2else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 4;
                string lblrule = lblruletype2;
                if(!string.IsNullOrEmpty(lblruletype2else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype2else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Readonly";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype5))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "FilterDropdown";
                newrule.AssociatedActionTypeID = 5;
                string lblrule = lblruletype5;
                if(!string.IsNullOrEmpty(lblruletype5else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype5else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "FilterDropdown";
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype4) || !string.IsNullOrEmpty(lblruletype4else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "TimeBasedAlert";
                newrule.ErrorMessage = AlertMessage;
                newrule.AssociatedActionTypeID = 3;
                if(Template != null)
                    newrule.TemplateId = long.Parse(SaveAnyDocument(Template, "byte"));
                string lblrule = lblruletype4;
                if(!string.IsNullOrEmpty(lblruletype4else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype4else;
                }
                else
                    newrule.IsElseAction = false;
                ActionArgs newActionArgs1 = new ActionArgs();
                newActionArgs1.ParameterName = "NotifyTo";
                newActionArgs1.ParameterValue = lblrule;
                newrule.actionarguments.Add(newActionArgs1);
                ActionArgs newActionArgs = new ActionArgs();
                newActionArgs.ParameterName = "TimeValue";
                newActionArgs.ParameterValue = TimeValue;
                newrule.actionarguments.Add(newActionArgs);
                if(!string.IsNullOrEmpty(NotifyToExtra))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToExtra";
                    newActionArgs2.ParameterValue = NotifyToExtra;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                if(!string.IsNullOrEmpty(NotifyToRole))
                {
                    string[] RolesIdstr = NotifyToRole.Split(",".ToCharArray());
                    var target = "0";
                    var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                    if(results.FirstOrDefault() != null && results[0].Equals("0"))
                        NotifyToRole = "0";
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToRole";
                    newActionArgs2.ParameterValue = NotifyToRole;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                // for web notification
                if(IsEmailNotification)
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsEmailNotification";
                    newActionArgs2.ParameterValue = "1";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                else
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsEmailNotification";
                    newActionArgs2.ParameterValue = "0";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                if(IsWebNotification)
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsWebNotification";
                    newActionArgs2.ParameterValue = "1";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                else
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsWebNotification";
                    newActionArgs2.ParameterValue = "0";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                //
                if(!string.IsNullOrEmpty(TimeRuleApplyOnValue))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyOn";
                    if(!string.IsNullOrEmpty(lblrulecondition))
                        newActionArgs2.ParameterValue = "Add,Update";
                    else
                        newActionArgs2.ParameterValue = TimeRuleApplyOnValue;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype17) || !string.IsNullOrEmpty(lblruletype17else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "RestrictDropdown";
                newrule.AssociatedActionTypeID = 17;
                string lblrule = lblruletype17;
                if(!string.IsNullOrEmpty(lblruletype17else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype17else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split("?".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split("=".ToCharArray());
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = param[0];
                    newactionargs.ParameterValue = param[1].Trim(',');
                    //todo dyanmic
                    newrule.actionarguments.Add(newactionargs);
                }
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblrulecondition))
            {
                //var lblrulecondition1 = lblrulecondition.Remove(lblrulecondition.LastIndexOf(','));
                //lblrulecondition1 = lblrulecondition1 + ",-";
                var conditions = lblrulecondition.Split("?".ToCharArray());
                foreach(var cond in conditions)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(",".ToCharArray());
                    Condition newcondition = new Condition();
                    newcondition.PropertyName = param[0];
                    newcondition.Operator = param[1];
                    if(param[1] == "Match")
                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                    else
                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                    newcondition.LogicalConnector = param[3];
                    businessrule.ruleconditions.Add(newcondition);
                }
            }
            if(!string.IsNullOrEmpty(lblruletype10) || !string.IsNullOrEmpty(lblruletype10else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "ValidateBeforeSave";
                newrule.AssociatedActionTypeID = 10;
                if(!string.IsNullOrEmpty(lblruletype10else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype13) || !string.IsNullOrEmpty(lblruletype13else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "UIAlert";
                newrule.AssociatedActionTypeID = 13;
                if(!string.IsNullOrEmpty(lblruletype13else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                businessrule.ruleaction.Add(newrule);
            }
            if(businessrule.AssociatedBusinessRuleTypeID == 5)
            {
                if(businessrule.t_schedulertask != null)
                    if(businessrule.t_schedulertask.T_RecurringRepeatFrequencyID == null || businessrule.t_schedulertask.T_RecurringRepeatFrequencyID == 0)
                        businessrule.t_schedulertask.T_RecurringRepeatFrequencyID = 1;
            }
            businessrule = db.BusinessRules.Add(businessrule);
            db.SaveChanges();
            //
            if(businessrule.AssociatedBusinessRuleTypeID == 5)
            {
                ApplicationContext db1 = new ApplicationContext(User);
                bool flagT_RepeatOn = false;
                foreach(var obj in db1.T_RepeatOns.Where(a => a.T_ScheduleID == businessrule.t_schedulertask.Id))
                {
                    db1.T_RepeatOns.Remove(obj);
                    flagT_RepeatOn = true;
                }
                if(flagT_RepeatOn)
                    db1.SaveChanges();
                if(businessrule.t_schedulertask.SelectedT_RecurrenceDays_T_RepeatOn != null)
                {
                    foreach(var pgs in businessrule.t_schedulertask.SelectedT_RecurrenceDays_T_RepeatOn)
                    {
                        T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                        objT_RepeatOn.T_ScheduleID = businessrule.t_schedulertask.Id;
                        objT_RepeatOn.T_RecurrenceDaysID = pgs;
                        db1.T_RepeatOns.Add(objT_RepeatOn);
                    }
                    db1.SaveChanges();
                }
                //RegisterScheduledTask task = new RegisterScheduledTask();
                //task.RegisterTask(businessrule.EntityName, businessrule.Id);
            }
            //
            return RedirectToAction("Index");
        }
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "TypeNo", "DisplayValue");
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName");
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("Owner", "LoggedInUser");
        ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
        Dictionary<string, string> objPropertyList = new Dictionary<string, string>();
        objPropertyList.Add("0", "--Select Property--");
        ViewBag.PropertyList = new SelectList(objPropertyList, "key", "value");
        ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(objPropertyList, "key", "value");
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "");
        //NotifyRoleList
        var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
        ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");
        return View(businessrule);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUserAssociation action.</returns>
    
    public JsonResult GetUserAssociation(string Entity)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        foreach(var asso in Ent.Associations)
        {
            if(asso.Target == "IdentityUser")
            {
                list.Add("Owner", "LoggedInUser");
                list.Add(asso.AssociationProperty, asso.DisplayName);
            }
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
        return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllAssociationsofEntity action.</returns>
    
    public JsonResult GetAllAssociationsofEntity(string Entity)
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
            if(asso.Name == "TenantId") continue;
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
                            list.Add(asso.Name + "." + asso1.AssociationProperty, asso.Name + "." + asso1.DisplayName);
                    }
                    else
                    {
                        var lstassoemailProperties = assoEntity.Properties.Where(p => p.IsEmailValidation);
                        foreach(var emailprop in lstassoemailProperties)
                        {
                            if(!list.ContainsKey(asso.Name + "." + emailprop.Name))
                                list.Add(asso.Name + "." + emailprop.Name, asso.Name + "." + emailprop.DisplayName);
                        }
                    }
                }
            }
        }
        return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAssociationsofEntity action.</returns>
    
    public JsonResult GetAssociationsofEntity(string Entity)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        foreach(var asso in Ent.Associations)
        {
            if(asso.Target == "IdentityUser" || asso.Name == "TenantId") continue;
            list.Add(asso.Name, asso.DisplayName);
        }
        return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertiesofEntity action.</returns>
    
    public JsonResult GetPropertiesofEntity(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var list = Ent.Properties.Where(p => p.Name != "TenantId");
        return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertiesofEntityWithInLineProperties
    /// action.</returns>
    
    public JsonResult GetPropertiesofEntityWithInLineProperties(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        //var list = Ent.Properties;
        Dictionary<string, string> list = new Dictionary<string, string>();
        foreach(var prop in Ent.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId").OrderBy(p => p.DisplayName))
        {
            list.Add(prop.Name, prop.DisplayName);
        }
        Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + Entity + "Controller, GeneratorBase.MVC.Controllers");
        object objController = Activator.CreateInstance(controller, null);
        MethodInfo mc = controller.GetMethod("getInlineAssociationsOfEntity");
        object[] MethodParams = new object[] { };
        var obj = mc.Invoke(objController, MethodParams);
        List<string> objStr = (List<string>)((System.Web.Mvc.JsonResult)(obj)).Data;
        if(Ent.Associations.Count > 0)
        {
            foreach(var assoc in Ent.Associations.OrderBy(p => p.DisplayName))
            {
                if(objStr.Contains(assoc.AssociationProperty))
                {
                    list.Remove(assoc.AssociationProperty);
                    var proplist = ModelReflector.Entities.First(p => p.Name == assoc.Target).Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId").OrderBy(p => p.DisplayName);
                    foreach(var assocprop in proplist)
                        list.Add(assoc.AssociationProperty + "." + assocprop.Name, assoc.DisplayName + "." + assocprop.DisplayName);
                }
            }
        }
        return Json(list, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetVerbsofEntity action.</returns>
    
    public JsonResult GetVerbsofEntity(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        Dictionary<string, string> verblist = new Dictionary<string, string>();
        foreach(var item in Ent.Verbs.Where(p => p.Name != "BulkUpdate" && p.Name != "BulkDelete"))
        {
            verblist.Add(Ent.Name + "." + item.Name, Ent.DisplayName + "." + item.DisplayName);
        }
        ApplicationContext db1 = new ApplicationContext(User);
        var DocumentTemplateslist = db1.T_DocumentTemplates.Where(p => p.T_EntityName == Entity && !p.T_Disable.Value).OrderBy(p => p.T_DisplayOrder).ToList();
        var DocumentTemplates = DocumentTemplateslist.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        foreach(var item in DocumentTemplates)
        {
            verblist.Add(Ent.DisplayName + "._" + item.T_Name, Ent.DisplayName + "." + item.T_Name);
        }
        foreach(var item in Ent.Associations)
        {
            if(item.Target == "IdentityUser") continue;
            var TargetEnt = ModelReflector.Entities.First(p => p.Name == item.Target);
            foreach(var itemTarget in TargetEnt.Verbs.Where(p => p.Name != "BulkUpdate" && p.Name != "BulkDelete"))
            {
                verblist.Add(item.AssociationProperty + "." + TargetEnt.Name + "." + itemTarget.Name, item.DisplayName + "." + TargetEnt.DisplayName + "." + itemTarget.DisplayName);
            }
        }
        return Json(verblist.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">       The entity.</param>
    /// <param name="AttributeName">Name of the attribute.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAttributesofTargetEntity action.</returns>
    
    public JsonResult GetAttributesofTargetEntity(string Entity, string AttributeName)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var asso = Ent.Associations.FirstOrDefault(p => p.AssociationProperty == AttributeName);
        if(asso != null)
        {
            if(asso != null)
            {
                if(asso.Target == "IdentityUser")
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            var EntTarget = ModelReflector.Entities.First(p => p.Name == asso.Target);
            var listTarget = EntTarget.Properties;
            return Json(listTarget.ToList(), JsonRequestBehavior.AllowGet);
        }
        return Json("", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">       The entity.</param>
    /// <param name="AttributeName">Name of the attribute.</param>
    ///
    /// <returns>A JSON response stream to send to the GetTargetEntityOfAssociationProperty action.</returns>
    
    public JsonResult GetTargetEntityOfAssociationProperty(string Entity, string AttributeName)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        if(Ent != null)
        {
            var asso = Ent.Associations.FirstOrDefault(p => p.AssociationProperty == AttributeName);
            if(asso != null)
            {
                var EntTarget = ModelReflector.Entities.First(p => p.Name == asso.Target);
                return Json(EntTarget, JsonRequestBehavior.AllowGet);
            }
            return Json(Ent, JsonRequestBehavior.AllowGet);
        }
        return Json("", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetTabsofEntity action.</returns>
    
    public JsonResult GetTabsofEntity(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var AssoList = new List<ModelReflector.Association>();
        foreach(var ent in ModelReflector.Entities.Where(p => p != Ent))
        {
            if(ent.Associations.Any(p => p.Target == Ent.Name))
            {
                // var association  = ent.Associations.Where(p => p.Target == Ent.Name);
                foreach(var association in ent.Associations.Where(p => p.Target == Ent.Name))
                {
                    ModelReflector.Association newAsso = new ModelReflector.Association();
                    newAsso.Name = ent.DisplayName + "->" + association.Name;
                    newAsso.Target = association.Target;
                    newAsso.DisplayName = association.DisplayName;
                    newAsso.AssociationProperty = association.AssociationProperty;
                    if(Ent.Name == association.Name)
                        newAsso.AssociationProperty = ent.Name + "." + association.AssociationProperty;
                    AssoList.Add(newAsso);
                }
            }
        }
        return Json(AssoList.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">  The entity.</param>
    /// <param name="Property">The property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetDropdown action.</returns>
    
    public JsonResult GetDropdown(string Entity, string Property)
    {
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == Entity);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == Property);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == Property);
        if(AssociationInfo != null)
        {
            return Json(AssociationInfo.Target, JsonRequestBehavior.AllowGet);
        }
        return Json("Failure", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetGroupsofEntity action.</returns>
    
    public JsonResult GetGroupsofEntity(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        Dictionary<string, string> grouplist = new Dictionary<string, string>();
        foreach(var item in Ent.Groups)
        {
            var groupVal = Ent.Name + GetInternalName(item.Name);
            bool gGroup = grouplist.Any(g => g.Key == groupVal);
            if(!gGroup)
            {
                grouplist.Add(groupVal, item.Name + "-G");
            }
        }
        Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + Entity + "Controller");
        object objController = Activator.CreateInstance(controller, null);
        MethodInfo mc = controller.GetMethod("getInlineAssociationsOfEntity");
        object[] MethodParams = new object[] { };
        var obj = mc.Invoke(objController, MethodParams);
        List<string> objStr = (List<string>)((System.Web.Mvc.JsonResult)(obj)).Data;
        if(Ent.Associations.Count > 0)
        {
            foreach(var onetoonitm in objStr)
            {
                var groupVal = onetoonitm;
                var dispName = Ent.Associations.Where(p => p.AssociationProperty == onetoonitm).FirstOrDefault().DisplayName;
                bool ginline = grouplist.Any(g => g.Key == groupVal);
                if(!ginline)
                {
                    grouplist.Add(groupVal, dispName + "-I");
                }
            }
            foreach(var assoc in Ent.Associations.OrderBy(p => p.DisplayName))
            {
                var targetEnt = assoc.Target;
                if(targetEnt != "IdentityUser")
                {
                    var EntAssoc = ModelReflector.Entities.First(p => p.Name == assoc.Target);
                    if(objStr.Contains(assoc.Name + "ID"))
                    {
                        foreach(var item in EntAssoc.Groups)
                        {
                            var groupVal = assoc.Name + GetInternalName(item.Name);
                            bool gAsso = grouplist.Any(g => g.Key == groupVal);
                            if(!gAsso)
                            {
                                grouplist.Add(groupVal, item.Name + "(" + EntAssoc.DisplayName + ")" + "-I");
                            }
                        }
                    }
                }
            }
        }
        foreach(var ent in ModelReflector.Entities.Where(p => p != Ent))
        {
            if(ent.Associations.Any(p => p.Target == Ent.Name))
            {
                foreach(var association in ent.Associations.Where(p => p.Target == Ent.Name))
                {
                    var groupVal = association.AssociationProperty.TrimEnd("ID".ToCharArray());
                    bool gTab = grouplist.Any(g => g.Key == groupVal);
                    if(!gTab)
                    {
                        grouplist.Add(groupVal, ModelReflector.Entities.FirstOrDefault(p => p.Name == ent.Name).DisplayName + "-T");
                    }
                }
            }
        }
        //IdentityUser
        return Json(grouplist.ToList(), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets line break details by identifier.</summary>
    ///
    /// <param name="Id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the GetBRDetailsById View.</returns>
    
    public ActionResult GetBRDetailsById(string Id)
    {
        long LongId = Convert.ToInt64(Id);
        var br = db.BusinessRules.FirstOrDefault(p => p.Id == LongId);
        return View("ShortDetails", br);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates quick time rule.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuickTimeRule View.</returns>
    
    public ActionResult CreateQuickTimeRule(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["BusinessRuleParentUrl"] = UrlReferrer;
        //
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName");
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("Owner", "LoggedInUser");
        ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
        //
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates quick time rule.</summary>
    ///
    /// <param name="businessrule"> The businessrule.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="TimeValue">    The time value.</param>
    /// <param name="NotifyTo">     The notify to.</param>
    /// <param name="NotifyToExtra">The notify to extra.</param>
    /// <param name="AlertMessage"> Message describing the alert.</param>
    ///
    /// <returns>A response stream to send to the CreateQuickTimeRule View.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuickTimeRule([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage)
    {
        if(ModelState.IsValid)
        {
            businessrule.EntitySubscribe = true;
            businessrule.DateCreated1 = DateTime.UtcNow;
            RuleAction newrule = new RuleAction();
            newrule.ActionName = "TimeBasedAlert";
            newrule.ErrorMessage = AlertMessage;
            //NotifyTo
            //NotifyToExtra
            ActionArgs newActionArgs = new ActionArgs();
            newActionArgs.ParameterName = "TimeValue";
            newActionArgs.ParameterValue = TimeValue;
            newrule.actionarguments.Add(newActionArgs);
            ActionArgs newActionArgs1 = new ActionArgs();
            newActionArgs1.ParameterName = "NotifyTo";
            newActionArgs1.ParameterValue = NotifyTo;
            newrule.actionarguments.Add(newActionArgs1);
            if(!string.IsNullOrEmpty(NotifyToExtra))
            {
                ActionArgs newActionArgs2 = new ActionArgs();
                newActionArgs2.ParameterName = "NotifyToExtra";
                newActionArgs2.ParameterValue = NotifyToExtra;
                newrule.actionarguments.Add(newActionArgs2);
            }
            businessrule.ruleaction.Add(newrule);
            businessrule.Roles = "All";
            db.BusinessRules.Add(businessrule);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return Redirect(Request.UrlReferrer.ToString());
        }
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
        return View(businessrule);
    }
    
    /// <summary>Cancels.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    
    public ActionResult Cancel(string UrlReferrer)
    {
        return RedirectToAction("Index");
    }
    
    /// <summary>POST: /BusinessRule/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="businessrule">The businessrule.</param>
    /// <param name="UrlReferrer"> The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.BusinessRules.Add(businessrule);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = businessrule.Id });
            //return Redirect(Request.UrlReferrer.ToString());
        }
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
        return View(businessrule);
    }
    
    /// <summary>Enables the disable rule.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the EnableDisableRule View.</returns>
    
    public ActionResult EnableDisableRule(int? id)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        BusinessRule businessrule = db.BusinessRules.Find(id);
        businessrule.Disable = !businessrule.Disable;
        db.Entry(businessrule).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    
    /// <summary>Freeze rule.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the FreezeRule View.</returns>
    
    public ActionResult FreezeRule(int? id)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        BusinessRule businessrule = db.BusinessRules.Find(id);
        businessrule.Freeze = !businessrule.Freeze;
        db.Entry(businessrule).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    
    /// <summary>GET: /BusinessRule/Edit/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        BusinessRule businessrule = db.BusinessRules.Find(id);
        ViewBag.EntityNameRuleAction = businessrule.EntityName;
        if(businessrule == null)
        {
            return HttpNotFound();
        }
        var _objAssociatedBusinessRuleType = dbAssociatedBusinessRuleType.BusinessRuleTypes;
        //var _objAssociatedBusinessRuleType = new List<BusinessRuleType>();
        //_objAssociatedBusinessRuleType.Add(businessrule.associatedbusinessruletype);
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(_objAssociatedBusinessRuleType, "TypeNo", "TypeName", businessrule.AssociatedBusinessRuleTypeID);
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == id).ToList();
        if(ViewData["BusinessRuleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/BusinessRule"))
            ViewData["BusinessRuleParentUrl"] = Request.UrlReferrer;
        //
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName", businessrule.EntityName);
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "");
        //NotifyRoleList
        var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
        ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");
        Dictionary<string, string> list = new Dictionary<string, string>();
        ViewBag.EntityVerb = new SelectList(list, "key", "value");
        Dictionary<string, string> listhiddenVerb = new Dictionary<string, string>();
        ViewBag.EntityHiddenVerb = new SelectList(listhiddenVerb, "key", "value");
        //list.Add("Owner", "LoggedInUser");
        ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
        Dictionary<string, string> PropertyList = new Dictionary<string, string>();
        PropertyList.Add("0", "--Select Property--");
        ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
        ViewBag.PropertyList7 = ViewBag.GroupList = ViewBag.PropertyList1 = new SelectList(PropertyList, "key", "value");
        Dictionary<string, string> IsElseActionList = new Dictionary<string, string>();
        IsElseActionList.Add("0", "True");
        IsElseActionList.Add("1", "False");
        ViewBag.IsElseActionList = new SelectList(IsElseActionList, "key", "value");
        Dictionary<string, string> TimeRuleApplyOn = new Dictionary<string, string>();
        TimeRuleApplyOn.Add("Add", "OnAdd");
        TimeRuleApplyOn.Add("Update", "OnUpdate");
        TimeRuleApplyOn.Add("Add,Update", "OnAdd&Update");
        TimeRuleApplyOn.Add("OnPropertyChange", "OnPropertyChange");
        ViewBag.TimeRuleApplyOn = new SelectList(TimeRuleApplyOn, "key", "value");
        ViewBag.Dropdown = new SelectList(list, "key", "value");
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7 =
                                              ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                      = ViewBag.SuggestedDynamicValueInCondition7 = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(list, "key", "value");
        ViewBag.SuggestedDynamicValue721 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                           = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                   = ViewBag.SuggestedDynamicValueInCondition72 = ViewBag.SuggestedDynamicValueInCondition721 = new SelectList(list, "key", "value");
        ViewBag.AssociationList17 = new SelectList(PropertyList, "key", "value");
        ViewBag.SuggestedPropertyValue17 = ViewBag.AssociationPropertyList17 = new SelectList(list, "key", "value");
        LoadViewDataBeforeOnEdit(businessrule);
        return View(businessrule);
    }
    
    /// <summary>POST: /BusinessRule/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for.</summary>
    ///
    /// <param name="businessrule">          The businessrule.</param>
    /// <param name="UrlReferrer">           The URL referrer.</param>
    /// <param name="TimeValue">             The time value.</param>
    /// <param name="NotifyTo">              The notify to.</param>
    /// <param name="NotifyToExtra">         The notify to extra.</param>
    /// <param name="AlertMessage">          Message describing the alert.</param>
    /// <param name="PropertyName">          Name of the property.</param>
    /// <param name="ConditionOperator">     The condition operator.</param>
    /// <param name="ConditionValue">        The condition value.</param>
    /// <param name="PropertyRuleValue">     The property rule value.</param>
    /// <param name="PropertyList">          List of properties.</param>
    /// <param name="RoleListValue">         The role list value.</param>
    /// <param name="PropertyList1Value">    The property list 1 value.</param>
    /// <param name="PropertyList7Value">    The property list 7 value.</param>
    /// <param name="Emailids">              The emailids.</param>
    /// <param name="TimeRuleApplyOnValue">  The time rule apply on value.</param>
    /// <param name="lblruletype1">          The first lblruletype.</param>
    /// <param name="lblruletype2">          The second lblruletype.</param>
    /// <param name="lblruletype3">          The third lblruletype.</param>
    /// <param name="lblruletype4">          The fourth lblruletype.</param>
    /// <param name="lblruletype5">          The fifth lblruletype.</param>
    /// <param name="lblruletype6">          The lblruletype 6.</param>
    /// <param name="lblruletype7">          The lblruletype 7.</param>
    /// <param name="lblruletype8">          The lblruletype 8.</param>
    /// <param name="lblruletype10">         The lblruletype 10.</param>
    /// <param name="lblruletype13">         The lblruletype 13.</param>
    /// <param name="lblruletype11">         The lblruletype 11.</param>
    /// <param name="lblruletype12">         The lblruletype 12.</param>
    /// <param name="lblruletype15">         The lblruletype 15.</param>
    /// <param name="Dropdown">              The dropdown.</param>
    /// <param name="lblrulecondition">      The lblrulecondition.</param>
    /// <param name="NotifyToRole">          The notify to role.</param>
    /// <param name="ScheduledDateTimeEnd">  The scheduled date time end.</param>
    /// <param name="ScheduledDateTimeStart">The scheduled date time start.</param>
    /// <param name="ScheduledDailyTime">    The scheduled daily time.</param>
    /// <param name="lblruletype1else">      The lblruletype 1else.</param>
    /// <param name="lblruletype2else">      The lblruletype 2else.</param>
    /// <param name="lblruletype3else">      The lblruletype 3else.</param>
    /// <param name="lblruletype4else">      The lblruletype 4else.</param>
    /// <param name="lblruletype5else">      The lblruletype 5else.</param>
    /// <param name="lblruletype6else">      The lblruletype 6else.</param>
    /// <param name="lblruletype7else">      The lblruletype 7else.</param>
    /// <param name="lblruletype8else">      The lblruletype 8else.</param>
    /// <param name="lblruletype10else">     The lblruletype 10else.</param>
    /// <param name="lblruletype13else">     The lblruletype 13else.</param>
    /// <param name="lblruletype11else">     The lblruletype 11else.</param>
    /// <param name="lblruletype12else">     The lblruletype 12else.</param>
    /// <param name="IsEmailNotification">   The Is Email Notification</param>
    /// <param name="IsWebNotification">     The Is Web Notification</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    public ActionResult Edit([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID,EntitySubscribe,Disable,Description,Freeze,InformationMessage,FailureMessage,T_SchedulerTaskID,t_schedulertask")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage, string PropertyName, string ConditionOperator, string ConditionValue, string PropertyRuleValue, string PropertyList, string RoleListValue, string PropertyList1Value, string PropertyList7Value, string Emailids, string TimeRuleApplyOnValue,
                             string lblruletype1, string lblruletype2, string lblruletype3, string lblruletype4, string lblruletype5, string lblruletype6, string lblruletype7, string lblruletype8, string lblruletype10, string lblruletype13, string lblruletype11, string lblruletype12, string lblruletype15, string Dropdown, string lblrulecondition, string NotifyToRole, string ScheduledDateTimeEnd, string ScheduledDateTimeStart, string ScheduledDailyTime,
                             string lblruletype1else, string lblruletype2else, string lblruletype3else, string lblruletype4else, string lblruletype5else, string lblruletype6else, string lblruletype7else, string lblruletype8else, string lblruletype10else, string lblruletype13else, string lblruletype11else, string lblruletype12else, string lblruletype16, string lblruletype16else,
                             bool IsEmailNotification, bool IsWebNotification,
                             string lblruletype17, string lblruletype17else, string lblruletype7forassocprop, HttpPostedFileBase Template)
    {
        if(businessrule.AssociatedBusinessRuleTypeID != 5)
        {
            ModelState.Remove("t_schedulertask.T_Name");
            ModelState.Remove("T_SchedulerTaskID");
            ModelState.Remove("t_schedulertask.T_StartDateTime");
            businessrule.t_schedulertask = null;
            businessrule.T_SchedulerTaskID = null;
        }
        else
        {
            if(businessrule.t_schedulertask != null)
                if(businessrule.t_schedulertask.T_RecurringRepeatFrequencyID == null || businessrule.t_schedulertask.T_RecurringRepeatFrequencyID == 0)
                    businessrule.t_schedulertask.T_RecurringRepeatFrequencyID = 1;
        }
        RuleActionContext dbRuleAction = new RuleActionContext();
        ConditionContext dbCondition = new ConditionContext();
        //ActionArgsContext dbActionArgs = new ActionArgsContext();
        if(ModelState.IsValid)
        {
            // businessrule.Roles = RoleListValue;
            if(businessrule.Id > 0)
            {
                BusinessRule businessrule1 = db.BusinessRules.Find(businessrule.Id);
                businessrule1.RuleName = businessrule.RuleName;
                if(RoleListValue == "")
                    RoleListValue = "All";
                else
                {
                    string[] RolesIdstr = RoleListValue.Split(",".ToCharArray());
                    var target = "All";
                    var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                    if(results.FirstOrDefault() != null && results[0].ToString() == "All")
                        RoleListValue = results[0].ToString();
                }
                businessrule1.Roles = RoleListValue;
                businessrule1.EntityName = businessrule.EntityName;
                businessrule1.EntitySubscribe = businessrule.EntitySubscribe;
                businessrule1.AssociatedBusinessRuleTypeID = businessrule.AssociatedBusinessRuleTypeID;
                businessrule1.DisplayValue = businessrule.DisplayValue;
                // businessrule1.DateCreated1 = DateTime.UtcNow;
                businessrule1.Disable = businessrule.Disable;
                businessrule1.Description = businessrule.Description;
                businessrule1.Freeze = businessrule.Freeze;
                businessrule1.InformationMessage = businessrule.InformationMessage;
                businessrule1.FailureMessage = businessrule.FailureMessage;
                businessrule1.t_schedulertask = businessrule.t_schedulertask;
                db.Entry(businessrule1).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype1) || !string.IsNullOrEmpty(lblruletype1else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "LockEntity";
                newrule.AssociatedActionTypeID = 1;
                newrule.RuleActionID = businessrule.Id;
                if(!string.IsNullOrEmpty(lblruletype1else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype11) || !string.IsNullOrEmpty(lblruletype11else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "LockEntity&Associations";
                newrule.AssociatedActionTypeID = 11;
                newrule.RuleActionID = businessrule.Id;
                if(!string.IsNullOrEmpty(lblruletype11else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype7) || !string.IsNullOrEmpty(lblruletype7else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "SetValue";
                newrule.AssociatedActionTypeID = 7;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype7;
                if(!string.IsNullOrEmpty(lblruletype7forassocprop))
                {
                    lblrule = lblruletype7forassocprop;
                }
                if(!string.IsNullOrEmpty(lblruletype7else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype7else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split("?".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(",".ToCharArray());
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = param[0];
                    newactionargs.ParameterValue = param[2];
                    //todo dyanmic
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype8) || !string.IsNullOrEmpty(lblruletype8else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "InvokeAction";
                newrule.AssociatedActionTypeID = 8;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype8;
                if(!string.IsNullOrEmpty(lblruletype8else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype8else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split(",".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(".".ToCharArray(), 2);
                    ActionArgs newactionargs = new ActionArgs();
                    //newactionargs.ParameterName = param[0].Trim();
                    //newactionargs.ParameterValue = param[1].Trim();
                    newactionargs.ParameterName = param[0].Trim();
                    var parmval = String.Join(",", param.Skip(2)).Trim();
                    if(parmval == "")
                        newactionargs.ParameterValue = String.Join(",", param.Skip(1)).Trim();
                    else
                        newactionargs.ParameterValue = param[1].Trim();
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype16) || !string.IsNullOrEmpty(lblruletype16else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "MakeVerbsHidden";
                newrule.AssociatedActionTypeID = 16;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype16;
                if(!string.IsNullOrEmpty(lblruletype16else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype16else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split(",".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(".".ToCharArray(), 2);
                    ActionArgs newactionargs = new ActionArgs();
                    //newactionargs.ParameterName = param[0].Trim();
                    //newactionargs.ParameterValue = param[1].Trim();
                    newactionargs.ParameterName = param[0].Trim();
                    var parmval = String.Join(",", param.Skip(2)).Trim();
                    if(parmval == "")
                        newactionargs.ParameterValue = String.Join(",", param.Skip(1)).Trim();
                    else
                        newactionargs.ParameterValue = param[1].Trim();
                    newrule.actionarguments.Add(newactionargs);
                }
                // businessrule.ruleaction.Add(newrule);
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype6) || !string.IsNullOrEmpty(lblruletype6else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 6;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype6;
                if(!string.IsNullOrEmpty(lblruletype6else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype6else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Hidden";
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
                // businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype12) || !string.IsNullOrEmpty(lblruletype12else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 12;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype12;
                if(!string.IsNullOrEmpty(lblruletype12else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype12else;
                }
                else
                    newrule.IsElseAction = false;
                var lblrule1 = lblrule.Split("|".ToArray());
                var lblruleId = lblrule1[0].Split(",".ToCharArray());
                var lblruleName = lblrule1[1].Split(",".ToCharArray());
                //foreach (string str in lblrule1[0].Split(",".ToCharArray()))
                for(int cnt = 0; cnt < lblruleId.Length; cnt++)
                {
                    if(string.IsNullOrEmpty(lblruleId[cnt])) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = lblruleId[cnt] + "|" + lblruleName[cnt];
                    newactionargs.ParameterValue = "GroupsHidden";
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges(); ;
            }
            //
            if(!string.IsNullOrEmpty(lblruletype3) || !string.IsNullOrEmpty(lblruletype3else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 2;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype3;
                if(!string.IsNullOrEmpty(lblruletype3else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype3else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Mandatory";
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
                //businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype2) || !string.IsNullOrEmpty(lblruletype2else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "PropertiesRule";
                newrule.AssociatedActionTypeID = 4;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype2;
                if(!string.IsNullOrEmpty(lblruletype2else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype2else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "Readonly";
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
                //businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype5) || !string.IsNullOrEmpty(lblruletype5else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "FilterDropdown";
                newrule.AssociatedActionTypeID = 5;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype5;
                if(!string.IsNullOrEmpty(lblruletype5else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype5else;
                }
                else
                    newrule.IsElseAction = false;
                foreach(string str in lblrule.Split(",".ToCharArray()))
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = str;
                    newactionargs.ParameterValue = "FilterDropdown";
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
                //businessrule.ruleaction.Add(newrule);
            }
            //Confirmation Before Save
            if(!string.IsNullOrEmpty(lblruletype15))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "ConfirmationBeforeSave";
                newrule.DisplayValue = "Confirmation Before Save";
                newrule.AssociatedActionTypeID = 15;
                newrule.RuleActionID = businessrule.Id;
                businessrule.ruleaction.Add(newrule);
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            //
            if(!string.IsNullOrEmpty(lblruletype4) || !string.IsNullOrEmpty(lblruletype4else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "TimeBasedAlert";
                newrule.ErrorMessage = AlertMessage;
                newrule.AssociatedActionTypeID = 3;
                if(Template != null)
                    newrule.TemplateId = long.Parse(SaveAnyDocument(Template, "byte"));
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype4;
                if(!string.IsNullOrEmpty(lblruletype4else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype4else;
                }
                else
                    newrule.IsElseAction = false;
                ActionArgs newActionArgs1 = new ActionArgs();
                newActionArgs1.ParameterName = "NotifyTo";
                newActionArgs1.ParameterValue = lblrule;
                newrule.actionarguments.Add(newActionArgs1);
                ActionArgs newActionArgs = new ActionArgs();
                newActionArgs.ParameterName = "TimeValue";
                newActionArgs.ParameterValue = TimeValue;
                newrule.actionarguments.Add(newActionArgs);
                if(!string.IsNullOrEmpty(NotifyToExtra))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToExtra";
                    newActionArgs2.ParameterValue = NotifyToExtra;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                if(!string.IsNullOrEmpty(NotifyToRole))
                {
                    string[] RolesIdstr = NotifyToRole.Split(",".ToCharArray());
                    var target = "0";
                    var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                    if(results.FirstOrDefault() != null && results[0].Equals("0"))
                        NotifyToRole = "0";
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToRole";
                    newActionArgs2.ParameterValue = NotifyToRole;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                // for web notification
                if(IsEmailNotification)
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsEmailNotification";
                    newActionArgs2.ParameterValue = "1";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                else
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsEmailNotification";
                    newActionArgs2.ParameterValue = "0";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                if(IsWebNotification)
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsWebNotification";
                    newActionArgs2.ParameterValue = "1";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                else
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "IsWebNotification";
                    newActionArgs2.ParameterValue = "0";
                    newrule.actionarguments.Add(newActionArgs2);
                }
                //
                if(!string.IsNullOrEmpty(TimeRuleApplyOnValue))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyOn";
                    if(!string.IsNullOrEmpty(lblrulecondition))
                        newActionArgs2.ParameterValue = "Add,Update";
                    else
                        newActionArgs2.ParameterValue = TimeRuleApplyOnValue;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
                //businessrule.ruleaction.Add(newrule);
            }
            if(!string.IsNullOrEmpty(lblruletype17) || !string.IsNullOrEmpty(lblruletype17else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "RestrictDropdown";
                newrule.AssociatedActionTypeID = 17;
                newrule.RuleActionID = businessrule.Id;
                string lblrule = lblruletype17;
                if(!string.IsNullOrEmpty(lblruletype17else))
                {
                    newrule.IsElseAction = true;
                    lblrule = lblruletype17else;
                }
                else
                    newrule.IsElseAction = false;
                var setvalues = lblrule.Split("?".ToCharArray());
                foreach(var cond in setvalues)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split("=".ToCharArray());
                    ActionArgs newactionargs = new ActionArgs();
                    newactionargs.ParameterName = param[0];
                    newactionargs.ParameterValue = param[1].Trim(',');
                    //todo dyanmic
                    newrule.actionarguments.Add(newactionargs);
                }
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblrulecondition))
            {
                bool alwaysFlag = false;
                var conditions = lblrulecondition.Split("?".ToCharArray());
                foreach(var cond in conditions)
                {
                    if(string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(",".ToCharArray());
                    if(param[0] == "Id" && param[1] == ">")
                        alwaysFlag = true;
                    Condition newcondition = new Condition();
                    newcondition.RuleConditionsID = businessrule.Id;
                    newcondition.PropertyName = param[0];
                    newcondition.Operator = param[1];
                    if(param[1] == "Match")
                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                    else
                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?"); ;
                    newcondition.LogicalConnector = param[3];
                    dbCondition.Conditions.Add(newcondition);
                    dbCondition.SaveChanges();
                    //businessrule.ruleconditions.Add(newcondition);
                }
                if(!alwaysFlag)
                {
                    var alwaysCondition = dbCondition.Conditions.Where(p => p.RuleConditionsID == businessrule.Id && p.PropertyName == "Id" && p.Operator == ">");
                    if(alwaysCondition.Count() > 0)
                    {
                        dbCondition.Conditions.Remove(alwaysCondition.FirstOrDefault());
                        dbCondition.SaveChanges();
                    }
                }
            }
            if(!string.IsNullOrEmpty(lblruletype10) || !string.IsNullOrEmpty(lblruletype10else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "ValidateBeforeSave";
                newrule.AssociatedActionTypeID = 10;
                newrule.RuleActionID = businessrule.Id;
                if(!string.IsNullOrEmpty(lblruletype10else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(!string.IsNullOrEmpty(lblruletype13) || !string.IsNullOrEmpty(lblruletype13else))
            {
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "UIAlert";
                newrule.AssociatedActionTypeID = 113;
                newrule.RuleActionID = businessrule.Id;
                if(!string.IsNullOrEmpty(lblruletype13else))
                    newrule.IsElseAction = true;
                else
                    newrule.IsElseAction = false;
                dbRuleAction.RuleActions.Add(newrule);
                dbRuleAction.SaveChanges();
            }
            if(businessrule.AssociatedBusinessRuleTypeID == 5)
            {
                ApplicationContext db1 = new ApplicationContext(User);
                bool flagT_RepeatOn = false;
                foreach(var obj in db1.T_RepeatOns.Where(a => a.T_ScheduleID == businessrule.t_schedulertask.Id))
                {
                    db1.T_RepeatOns.Remove(obj);
                    flagT_RepeatOn = true;
                }
                if(flagT_RepeatOn)
                    db1.SaveChanges();
                if(businessrule.t_schedulertask.SelectedT_RecurrenceDays_T_RepeatOn != null)
                {
                    foreach(var pgs in businessrule.t_schedulertask.SelectedT_RecurrenceDays_T_RepeatOn)
                    {
                        T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                        objT_RepeatOn.T_ScheduleID = businessrule.t_schedulertask.Id;
                        objT_RepeatOn.T_RecurrenceDaysID = pgs;
                        db1.T_RepeatOns.Add(objT_RepeatOn);
                    }
                    db1.SaveChanges();
                }
                // RegisterScheduledTask task = new RegisterScheduledTask();
                // task.RegisterTask(businessrule.EntityName, businessrule.Id);
            }
            //return RedirectToAction("Edit", "BusinessRule", new { id = businessrule.Id });
            return RedirectToAction("Index");
        }
        ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "TypeNo", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == businessrule.Id).ToList();
        return View(businessrule);
    }
    
    /// <summary>GET: /BusinessRule/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int? id)
    {
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        BusinessRule businessrule = db.BusinessRules.Find(id);
        if(businessrule == null)
        {
            return HttpNotFound();
        }
        if(ViewData["BusinessRuleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/BusinessRule"))
            ViewData["BusinessRuleParentUrl"] = Request.UrlReferrer;
        return View(businessrule);
    }
    
    /// <summary>POST: /BusinessRule/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
        BusinessRule businessrule = db.BusinessRules.Find(id);
        db.BusinessRules.Remove(businessrule);
        db.SaveChanges();
        if(ViewData["BusinessRuleParentUrl"] != null)
        {
            string parentUrl = ViewData["BusinessRuleParentUrl"].ToString();
            ViewData["BusinessRuleParentUrl"] = null;
            return Redirect(parentUrl);
        }
        else return RedirectToAction("Index");
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        var _Obj = db.BusinessRules.Find(Convert.ToInt64(id));
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
        IQueryable<BusinessRule> list = db.BusinessRules.Unfiltered();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.BusinessRules;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(BusinessRule), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<BusinessRule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<BusinessRule>)q);
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
    
    /// <summary>Gets date type.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>A response stream to send to the GetDateType View.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetDateType(string entityName, string propertyName)
    {
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        string DataType = PropInfo.DataType;
        if(DataType == "DateTime" && PropInfo.DataTypeAttribute == "Time")
            DataType = "Time";
        return Json(DataType, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets date type for association properties.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="assocName">   Name of the associated.</param>
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>A response stream to send to the GetDateTypeForAssociationProperties View.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetDateTypeForAssociationProperties(string entityName, string assocName, string propertyName)
    {
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var AssocInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == assocName);
        var AssocEntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssocInfo.Target);
        var PropInfo = AssocEntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        string DataType = PropInfo.DataType;
        if(DataType == "DateTime" && PropInfo.DataTypeAttribute == "Time")
            DataType = "Time";
        return Json(DataType, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Verify property and value data type.</summary>
    ///
    /// <param name="entityName">    Name of the entity.</param>
    /// <param name="propertyName">  Name of the property.</param>
    /// <param name="conditionValue">The condition value.</param>
    /// <param name="valueType">     Type of the value.</param>
    /// <param name="actionType">    Type of the action.</param>
    ///
    /// <returns>A response stream to send to the VerifyPropertyAndValueDataType View.</returns>
    
    public ActionResult VerifyPropertyAndValueDataType(string entityName, string propertyName, string conditionValue, string valueType, string actionType)
    {
        string result = "";
        if(string.IsNullOrEmpty(conditionValue) && valueType == "Changes to anything")
            return Json(result, JsonRequestBehavior.AllowGet);
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        ModelReflector.Property targetPropInfo = null;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                    }
                }
            }
        }
        string DataType = PropInfo.DataType;
        var targetType = typeof(System.String);
        var targetTypeDynamic = typeof(System.String);
        if(AssociationInfo != null && actionType != "condition")
        {
            DataType = "String";
            targetType = typeof(System.String);
        }
        if(conditionValue.ToLower().Trim() == "null")
        {
            DataType = "String";
        }
        switch(DataType)
        {
        case "Int32":
            targetType = typeof(System.Int32);
            break;
        case "Int64":
            targetType = typeof(System.Int64);
            break;
        case "Double":
            targetType = typeof(System.Double);
            break;
        case "Decimal":
            targetType = typeof(System.Decimal);
            break;
        case "Text":
            targetType = typeof(System.String);
            break;
        case "Boolean":
            targetType = typeof(System.Boolean);
            break;
        case "DateTime":
            targetType = typeof(System.DateTime);
            break;
        }
        if(conditionValue.Trim().ToLower().Contains("today"))
            targetType = typeof(System.String);
        dynamic value2 = null;
        try
        {
            if(valueType.Trim().ToLower() == "dynamic")
            {
                if(conditionValue.Trim().StartsWith("[") && conditionValue.Trim().EndsWith("]"))
                {
                    var targetpropertyName = conditionValue.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                    if(targetpropertyName.Contains("."))
                    {
                        var targetPropertiesDynamic = targetpropertyName.Split(".".ToCharArray());
                        if(targetPropertiesDynamic.Length > 1)
                        {
                            var AssociationInfoDynamic = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetPropertiesDynamic[0]);
                            if(AssociationInfoDynamic != null)
                            {
                                var EntityInfoDynamic = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfoDynamic.Target);
                                targetPropInfo = EntityInfoDynamic.Properties.FirstOrDefault(p => p.Name == targetPropertiesDynamic[1]);
                            }
                        }
                    }
                    else
                    {
                        targetPropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetpropertyName);
                    }
                }
                else if(conditionValue.Trim().StartsWith("[") && (conditionValue.Trim().ToLower().EndsWith("days") || conditionValue.Trim().ToLower().EndsWith("months") || conditionValue.Trim().ToLower().EndsWith("weeks") | conditionValue.Trim().ToLower().EndsWith("years")))
                {
                    var condValueArray = conditionValue.Split(" ".ToCharArray());
                    if(condValueArray[0].Trim().StartsWith("[") && condValueArray[0].Trim().EndsWith("]"))
                    {
                        var targetpropertyName = condValueArray[0].TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                        if(targetpropertyName.Contains("."))
                        {
                            var targetPropertiesDynamic = targetpropertyName.Split(".".ToCharArray());
                            if(targetPropertiesDynamic.Length > 1)
                            {
                                var AssociationInfoDynamic = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetPropertiesDynamic[0]);
                                if(AssociationInfoDynamic != null)
                                {
                                    var EntityInfoDynamic = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfoDynamic.Target);
                                    targetPropInfo = EntityInfoDynamic.Properties.FirstOrDefault(p => p.Name == targetPropertiesDynamic[1]);
                                }
                            }
                        }
                        else
                        {
                            targetPropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetpropertyName);
                        }
                    }
                }
                if(targetPropInfo != null)
                    targetTypeDynamic = ApplyRule.getTypeForAssocProperty(targetPropInfo.DataType);
                if(targetTypeDynamic != targetType && targetTypeDynamic != ApplyRule.getTypeForAssocProperty(PropInfo.DataType))
                    result = "Selected properties are not compatible. Please select properties of same type.";
            }
            else
                value2 = Convert.ChangeType(conditionValue.Trim(), targetType);
            if(PropInfo.DataTypeAttribute == "Time")
            {
                DateTime dateTimeTemp;
                if(!DateTime.TryParseExact(conditionValue.Trim(), "hh:mm tt", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out dateTimeTemp))
                    result = "Please enter time in correct format. Ex: 10:10 PM";
            }
        }
        catch(Exception ex)
        {
            result = ex.Message;
        }
        return Json(result, JsonRequestBehavior.AllowGet);
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
            if(dbAssociatedBusinessRuleType != null) dbAssociatedBusinessRuleType.Dispose();
        }
        base.Dispose(disposing);
    }
    
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="br">The line break.</param>
    
    private void LoadViewDataBeforeOnEdit(BusinessRule br)
    {
        if(br.t_schedulertask == null)
        {
            LoadViewDataBeforeOnCreate();
            return;
        }
        ApplicationContext db1 = new ApplicationContext(User);
        var T_RecurrenceDays_T_RepeatOnlist = db1.T_RecurrenceDayss.OrderBy(p => p.DisplayValue).Take(10).Distinct();
        ViewBag.SelectedT_RecurrenceDays_T_RepeatOn = new MultiSelectList(T_RecurrenceDays_T_RepeatOnlist, "ID", "DisplayValue", br.t_schedulertask.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID));
        var objT_AssociatedScheduleType = db1.T_Scheduletypes.ToList();
        ViewBag.T_AssociatedScheduleTypeID = new SelectList(objT_AssociatedScheduleType, "ID", "DisplayValue", br.t_schedulertask.T_AssociatedScheduleTypeID);
        var objT_AssociatedRecurringScheduleDetailsType = db1.T_RecurringScheduleDetailstypes.ToList();
        ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(objT_AssociatedRecurringScheduleDetailsType, "ID", "DisplayValue", br.t_schedulertask.T_AssociatedRecurringScheduleDetailsTypeID);
        //var objT_RecurringRepeatFrequency = new List<T_RecurringFrequency>();
        var objT_RecurringRepeatFrequency = db1.T_RecurringFrequencys.OrderBy(p => p.Id);
        ViewBag.T_RecurringRepeatFrequencyID = new SelectList(objT_RecurringRepeatFrequency, "ID", "DisplayValue", br.t_schedulertask.T_RecurringRepeatFrequencyID);
        var objT_RepeatBy = db1.T_MonthlyRepeatTypes.ToList();
        ViewBag.T_RepeatByID = new SelectList(objT_RepeatBy, "ID", "DisplayValue", br.t_schedulertask.T_RepeatByID);
        var objT_RecurringTaskEndType = db1.T_RecurringEndTypes.ToList();
        ViewBag.T_RecurringTaskEndTypeID = new SelectList(objT_RecurringTaskEndType, "ID", "DisplayValue", br.t_schedulertask.T_RecurringTaskEndTypeID);
    }
    /// <summary>Loads view data before on create.</summary>
    private void LoadViewDataBeforeOnCreate()
    {
        ApplicationContext db1 = new ApplicationContext(User);
        var T_RecurrenceDays_T_RepeatOnlist = db1.T_RecurrenceDayss.OrderBy(p => p.DisplayValue).Take(10).Distinct();
        ViewBag.SelectedT_RecurrenceDays_T_RepeatOn = new MultiSelectList(T_RecurrenceDays_T_RepeatOnlist, "ID", "DisplayValue");
        var objT_AssociatedScheduleType = db1.T_Scheduletypes.ToList();
        ViewBag.T_AssociatedScheduleTypeID = new SelectList(objT_AssociatedScheduleType, "ID", "DisplayValue", 1);
        var objT_AssociatedRecurringScheduleDetailsType = db1.T_RecurringScheduleDetailstypes.ToList();
        ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(objT_AssociatedRecurringScheduleDetailsType, "ID", "DisplayValue", 1);
        var objT_RecurringRepeatFrequency = db1.T_RecurringFrequencys.OrderBy(p => p.Id);
        ViewBag.T_RecurringRepeatFrequencyID = new SelectList(objT_RecurringRepeatFrequency, "ID", "DisplayValue", 1);
        var objT_RepeatBy = db1.T_MonthlyRepeatTypes.ToList();
        ViewBag.T_RepeatByID = new SelectList(objT_RepeatBy, "ID", "DisplayValue", 1);
        var objT_RecurringTaskEndType = db1.T_RecurringEndTypes.ToList();
        ViewBag.T_RecurringTaskEndTypeID = new SelectList(objT_RecurringTaskEndType, "ID", "DisplayValue", 1);
    }
    
    /// <summary>(An Action that handles HTTP GET requests) export business rule.</summary>
    ///
    /// <returns>A response stream to send to the ExportBusinessRule View.</returns>
    
    [HttpGet]
    public ActionResult ExportBusinessRule()
    {
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        List<BusinessRule> list = db.BusinessRules.ToList();
        // var data  = Newtonsoft.Json.JsonSerializer(list);
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        serSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        var data = Newtonsoft.Json.JsonConvert.SerializeObject(list, serSettings);
        Response.Clear();
        Response.ClearHeaders();
        Response.AppendHeader("Content-Length", data.Length.ToString());
        Response.ContentType = "application/json; charset=utf-8";
        Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + commonObj.AppName() + "_BusinessRules.json\"");
        Response.Write(data);
        Response.End();
        //return File(filedata, "application/force-download", Path.GetFileName(FileName));
        return Json("", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [HttpGet]
    public ActionResult Upload()
    {
        if(!User.IsAdmin)
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.Title = "Upload File";
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <param name="FileUpload">       The file upload.</param>
    /// <param name="collection">       The collection.</param>
    /// <param name="CleanOldBR">       The clean old line break.</param>
    /// <param name="ImportOnlyEnabled">The import only enabled.</param>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Upload([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection, bool? CleanOldBR, bool? ImportOnlyEnabled)
    {
        if(FileUpload != null)
        {
            string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();
            if(fileExtension.ToUpper() == ".JSON")
            {
                string rename = string.Empty;
                rename = System.IO.Path.GetFileName(FileUpload.FileName.ToLower().Replace(fileExtension, ".json"));
                fileExtension = ".json";
                string fileLocation = string.Format("{0}\\{1}", Server.MapPath("~/ExcelFiles"), rename);
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                FileUpload.SaveAs(fileLocation);
                string content = System.IO.File.ReadAllText(fileLocation);
                Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
                serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                serSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BusinessRule>>(content, serSettings);
                var importOnlyEnabled = true;
                importOnlyEnabled = ImportOnlyEnabled.HasValue ? ImportOnlyEnabled.Value : false;
                if(importOnlyEnabled)
                    data = data.Where(p => !p.Disable).ToList();
                if(data.Count() > 0)
                {
                    var deleteOldRules = true;
                    deleteOldRules = CleanOldBR.HasValue ? CleanOldBR.Value : false;
                    if(deleteOldRules)
                    {
                        var listbr = db.BusinessRules.ToList();
                        foreach(var br in listbr)
                        {
                            BusinessRule businessrule = db.BusinessRules.Find(br.Id);
                            if(businessrule.T_SchedulerTaskID.HasValue)
                            {
                                var schedulerid = businessrule.T_SchedulerTaskID;
                                using(var context = new ApplicationContext(new SystemUser()))
                                {
                                    var obj = context.T_Schedules.FirstOrDefault(p => p.Id == schedulerid);
                                    if(obj != null)
                                    {
                                        context.T_Schedules.Remove(obj);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            db.BusinessRules.Remove(businessrule);
                            db.SaveChanges();
                        }
                        //db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('tbl_BusinessRule', RESEED, 0)");
                        //db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('tbl_Condition', RESEED, 0)");
                        //db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('tbl_RuleAction', RESEED, 0)");
                    }
                    foreach(var br in data)
                    {
                        db.Entry(br).State = EntityState.Added;
                        db.SaveChanges();
                    }
                }
            }
        }
        return RedirectToAction("Index");
    }
    /// <summary>Gets extra journal entry - journal entries for tabs(1.m,m.m) associations.</summary>
    ///
    /// <param name="id">  The identifier.</param>
    /// <param name="user">The logged-in user.</param>
    /// <param name="jedb">The JournalEntryContext.</param>
    ///
    /// <returns>The extra journal entry.</returns>
    public IQueryable<JournalEntry> GetExtraJournalEntry(int? id, Models.IUser user, JournalEntryContext jedb, string RelatedEntityRecords)
    {
        var listjournaliquery = jedb.JournalEntries.AsNoTracking().Where(p => p.Id == 0);
        Expression<Func<JournalEntry, bool>> predicateJournalEntry = n => false;
        listjournaliquery = new FilteredDbSet<JournalEntry>(jedb, predicateJournalEntry);
        return listjournaliquery;
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyForMapping action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetPropertyForMapping(string EntityName)
    {
        Dictionary<GeneratorBase.MVC.ModelReflector.Property, string> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, string>();
        var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == EntityName);
        if(entList != null)
        {
            var properties = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "DeleteDateTime" && p.Name != "IsDeleted" && p.Name != "TenantId").ToList();
            properties.Add(new ModelReflector.Property { DisplayName = "Id (Primary Key)", Name = "Id", IsRequired = true });
            foreach(var prop in properties.OrderBy(p => p.DisplayName))
            {
                var selectedVal = "";
                selectedVal = prop.Name;
                objColMap.Add(prop, selectedVal);
            }
        }
        string str = string.Empty;
        foreach(var col in objColMap)
        {
            var dispvalue = (col.Key as GeneratorBase.MVC.ModelReflector.Property).DisplayName;
            var internalname = (col.Key as GeneratorBase.MVC.ModelReflector.Property).Name;
            str += "<tr ><td><label class='PropertyName' datavalue='" + internalname + "'>" + dispvalue + "</label>";
            if((col.Key as GeneratorBase.MVC.ModelReflector.Property).IsRequired == true)
            {
                str += "<span class='text-danger-reg'>*</span>";
            }
            str += "</td><td><input id='colList" + internalname + "' value='" + col.Value + "' class='DataName' /></td>";
            str += "<td>";
            if((col.Key as GeneratorBase.MVC.ModelReflector.Property).IsRequired != true)
            {
                str += "<i name=\"lblruleconditionMap\" onclick=\"deleteRow(this);\" class=\"fa fa-times\"></i>";
            }
            str += "</td></tr>";
        }
        return Json(str, JsonRequestBehavior.AllowGet);
    }
    /// <summary>After save.</summary>
    ///
    /// <param name="businessrule">The Footer Section .</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(BusinessRule businessrule, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
}

}