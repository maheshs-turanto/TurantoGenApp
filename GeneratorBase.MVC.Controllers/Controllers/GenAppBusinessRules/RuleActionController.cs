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
/// <summary>A controller for handling rule actions.</summary>
public class RuleActionController : BaseController
{
    /// <summary>The database.</summary>
    private RuleActionContext db = new RuleActionContext();
    /// <summary>The database rule action.</summary>
    private BusinessRuleContext dbRuleAction = new BusinessRuleContext();
    /// <summary>Type of the database associated action.</summary>
    private ActionTypeContext dbAssociatedActionType = new ActionTypeContext();
    /// <summary>The database action arguments.</summary>
    private ActionArgsContext dbActionArgs = new ActionArgsContext();
    
    /// <summary>GET: /RuleAction/.</summary>
    ///
    /// <param name="currentFilter">       A filter specifying the current.</param>
    /// <param name="searchString">        The search string.</param>
    /// <param name="sortBy">              Describes who sort this object.</param>
    /// <param name="isAsc">               The is ascending.</param>
    /// <param name="page">                The page.</param>
    /// <param name="itemsPerPage">        The items per page.</param>
    /// <param name="HostingEntity">       The hosting entity.</param>
    /// <param name="HostingEntityID">     Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">      Type of the associated.</param>
    /// <param name="IsExport">            The is export.</param>
    /// <param name="RenderPartial">       The render partial.</param>
    /// <param name="EntityNameRuleAction">The entity name rule action.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? RenderPartial,string EntityNameRuleAction)
    {
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["EntityNameRuleAction"] = EntityNameRuleAction;
        var lstRuleAction = from s in db.RuleActions
                            select s;
        lstRuleAction = lstRuleAction.OrderByDescending(s => s.Id);
        int pageSize = 100;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        var _RuleAction = lstRuleAction.Include(t => t.ruleaction).Include(t => t.associatedactiontype);
        if(HostingEntity == "BusinessRule" && HostingEntityID != null && AssociatedType == "RuleAction")
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            _RuleAction = _RuleAction.Where(p => p.RuleActionID == hostid);
        }
        if(HostingEntity == "ActionType" && HostingEntityID != null && AssociatedType == "AssociatedActionType")
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            _RuleAction = _RuleAction.Where(p => p.AssociatedActionTypeID == hostid);
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_RuleAction.ToPagedList(pageNumber, pageSize));
        else
        {
            return PartialView("IndexPartial", _RuleAction.ToPagedList(pageNumber, pageSize));
        }
        return View(_RuleAction.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstRuleAction">The list rule action.</param>
    /// <param name="searchString"> The search string.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<RuleAction> searchRecords(IQueryable<RuleAction> lstRuleAction, string searchString)
    {
        lstRuleAction = lstRuleAction.Where(s => (!String.IsNullOrEmpty(s.ActionName) && s.ActionName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ErrorMessage) && s.ErrorMessage.ToUpper().Contains(searchString)) || (s.ruleaction != null && (s.ruleaction.DisplayValue.ToUpper().Contains(searchString))) || (s.associatedactiontype != null && (s.associatedactiontype.DisplayValue.ToUpper().Contains(searchString))));
        return lstRuleAction;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstRuleAction">The list rule action.</param>
    /// <param name="sortBy">       Describes who sort this object.</param>
    /// <param name="isAsc">        The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<RuleAction> sortRecords(IQueryable<RuleAction> lstRuleAction, string sortBy, string isAsc)
    {
        var elementType = typeof(RuleAction);
        var param = Expression.Parameter(elementType, "ruleaction");
        var prop = elementType.GetProperty(sortBy);
        Type type = Nullable.GetUnderlyingType(prop.PropertyType);
        if(type == null)
            type = prop.PropertyType;
        if(type.Equals(typeof(string)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, string>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(char)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, char?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(int)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, int?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(float)) || type.Equals(typeof(double)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, double?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(decimal)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, decimal?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(long)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, long?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(DateTime)))
        {
            var mySortExpression = Expression.Lambda<Func<RuleAction, DateTime?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstRuleAction = lstRuleAction.OrderBy(mySortExpression);
            else
                lstRuleAction = lstRuleAction.OrderByDescending(mySortExpression);
        }
        // This last part won't work but I left it so that it can compile (all routes need to return value etc.)
        return lstRuleAction;
    }
    
    /// <summary>GET: /RuleAction/Details/5.</summary>
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
        RuleAction ruleaction = db.RuleActions.Find(id);
        if(ruleaction == null)
        {
            return HttpNotFound();
        }
        return View(ruleaction);
    }
    
    /// <summary>GET: /RuleAction/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objRuleAction = new List<BusinessRule>();
            ViewBag.RuleActionID = new SelectList(objRuleAction, "ID", "DisplayValue");
        }
        if(HostingEntityName == "ActionType" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objAssociatedActionType = new List<ActionType>();
            ViewBag.AssociatedActionTypeID = new SelectList(objAssociatedActionType, "ID", "DisplayValue");
        }
        ViewData["RuleActionParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>GET: /RuleAction/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objRuleAction = new List<BusinessRule>();
            ViewBag.RuleActionID = new SelectList(objRuleAction, "ID", "DisplayValue");
        }
        if(HostingEntityName == "ActionType" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objAssociatedActionType = new List<ActionType>();
            ViewBag.AssociatedActionTypeID = new SelectList(objAssociatedActionType, "ID", "DisplayValue");
        }
        ViewData["RuleActionParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>POST: /RuleAction/CreateWizard To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ruleaction"> The ruleaction.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ActionName,ErrorMessage,RuleActionID,AssociatedActionTypeID")] RuleAction ruleaction, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.RuleActions.Add(ruleaction);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules, "ID", "DisplayValue", ruleaction.RuleActionID);
        ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        return View(ruleaction);
    }
    
    /// <summary>GET: /RuleAction/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "Owner");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
        }
        else
        {
            var objRuleAction = new List<BusinessRule>();
            ViewBag.RuleActionID = new SelectList(objRuleAction, "ID", "DisplayValue");
        }
        if(HostingEntityName == "ActionType" && Convert.ToInt64(HostingEntityID) > 0)
        {
            long hostid = Convert.ToInt64(HostingEntityID);
            ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
        }
        else
        {
            var objAssociatedActionType = new List<ActionType>();
            ViewBag.AssociatedActionTypeID = new SelectList(objAssociatedActionType, "ID", "DisplayValue");
        }
        ViewData["RuleActionParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>POST: /RuleAction/CreateQuick To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ruleaction">   The ruleaction.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="TimeValue">    The time value.</param>
    /// <param name="NotifyTo">     The notify to.</param>
    /// <param name="NotifyToExtra">The notify to extra.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ActionName,ErrorMessage,RuleActionID,AssociatedActionTypeID")] RuleAction ruleaction, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra)
    {
        if(ModelState.IsValid)
        {
            if(ruleaction.AssociatedActionTypeID == 3)
            {
                ActionArgs newActionArgs = new ActionArgs();
                newActionArgs.ParameterName = "TimeValue";
                newActionArgs.ParameterValue = TimeValue;
                ruleaction.actionarguments.Add(newActionArgs);
                ActionArgs newActionArgs1 = new ActionArgs();
                newActionArgs1.ParameterName = "NotifyTo";
                newActionArgs1.ParameterValue = NotifyTo;
                ruleaction.actionarguments.Add(newActionArgs1);
                if(!string.IsNullOrEmpty(NotifyToExtra))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToExtra";
                    newActionArgs2.ParameterValue = NotifyToExtra;
                    ruleaction.actionarguments.Add(newActionArgs2);
                }
                ActionArgs newActionArgs3 = new ActionArgs();
                newActionArgs3.ParameterName = "NotifyOn";
                newActionArgs3.ParameterValue = "Add,Update";
                ruleaction.actionarguments.Add(newActionArgs3);
            }
            db.RuleActions.Add(ruleaction);
            db.SaveChanges();
            //,Hash = "Action"
            return RedirectToAction("Edit", "BusinessRule", new { id = ruleaction.RuleActionID });
        }
        ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules, "ID", "DisplayValue", ruleaction.RuleActionID);
        ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        return View(ruleaction);
    }
    
    /// <summary>Cancels.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    
    public ActionResult Cancel(string UrlReferrer)
    {
        long BusinessRuleId = Convert.ToInt64(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["BusinessRuleId"]);
        //,Hash = "Action"
        return RedirectToAction("Edit", "BusinessRule", new { id = BusinessRuleId });
    }
    
    /// <summary>POST: /RuleAction/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ruleaction"> The ruleaction.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ActionName,ErrorMessage,RuleActionID,AssociatedActionTypeID")] RuleAction ruleaction, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.RuleActions.Add(ruleaction);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules, "ID", "DisplayValue", ruleaction.RuleActionID);
        ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        return View(ruleaction);
    }
    
    /// <summary>GET: /RuleAction/Edit/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id)
    {
        if(!User.CanEdit("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        RuleAction ruleaction = db.RuleActions.Find(id);
        if(ruleaction == null)
        {
            return HttpNotFound();
        }
        var _objRuleAction = new List<BusinessRule>();
        _objRuleAction.Add(ruleaction.ruleaction);
        ViewBag.RuleActionID = new SelectList(_objRuleAction, "ID", "DisplayValue", ruleaction.RuleActionID);
        var _objAssociatedActionType = new List<ActionType>();
        _objAssociatedActionType.Add(ruleaction.associatedactiontype);
        ViewBag.AssociatedActionTypeID = new SelectList(_objAssociatedActionType, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        if(ViewData["RuleActionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/RuleAction"))
            ViewData["RuleActionParentUrl"] = Request.UrlReferrer;
        return View(ruleaction);
    }
    
    /// <summary>POST: /RuleAction/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ruleaction"> The ruleaction.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ActionName,ErrorMessage,RuleActionID,AssociatedActionTypeID")] RuleAction ruleaction, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(ruleaction).State = EntityState.Modified;
            db.SaveChanges();
            //,Hash = "Action"
            return RedirectToAction("Edit", "BusinessRule", new { id = ruleaction.RuleActionID });
        }
        ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules, "ID", "DisplayValue", ruleaction.RuleActionID);
        ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        return View(ruleaction);
    }
    
    /// <summary>GET: /RuleAction/EditWizard/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id)
    {
        if(!User.CanEdit("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        RuleAction ruleaction = db.RuleActions.Find(id);
        if(ruleaction == null)
        {
            return HttpNotFound();
        }
        var _objRuleAction = new List<BusinessRule>();
        _objRuleAction.Add(ruleaction.ruleaction);
        ViewBag.RuleActionID = new SelectList(_objRuleAction, "ID", "DisplayValue", ruleaction.RuleActionID);
        var _objAssociatedActionType = new List<ActionType>();
        _objAssociatedActionType.Add(ruleaction.associatedactiontype);
        ViewBag.AssociatedActionTypeID = new SelectList(_objAssociatedActionType, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        if(ViewData["RuleActionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/RuleAction"))
            ViewData["RuleActionParentUrl"] = Request.UrlReferrer;
        return View(ruleaction);
    }
    
    /// <summary>POST: /RuleAction/EditWizard/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ruleaction"> The ruleaction.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ActionName,ErrorMessage,RuleActionID,AssociatedActionTypeID")] RuleAction ruleaction, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(ruleaction).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        ViewBag.RuleActionID = new SelectList(dbRuleAction.BusinessRules, "ID", "DisplayValue", ruleaction.RuleActionID);
        ViewBag.AssociatedActionTypeID = new SelectList(dbAssociatedActionType.ActionTypes, "ID", "DisplayValue", ruleaction.AssociatedActionTypeID);
        return View(ruleaction);
    }
    
    /// <summary>GET: /RuleAction/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int? id)
    {
        if(!User.CanDelete("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        RuleAction ruleaction = db.RuleActions.Find(id);
        if(ruleaction == null)
        {
            return HttpNotFound();
        }
        if(ViewData["RuleActionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/RuleAction"))
            ViewData["RuleActionParentUrl"] = Request.UrlReferrer;
        return View(ruleaction);
    }
    
    /// <summary>POST: /RuleAction/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        if(!User.CanDelete("RuleAction"))
        {
            return RedirectToAction("Index", "Error");
        }
        List<ActionArgs> listArgs = dbActionArgs.ActionArgss.Where(a => a.ActionArgumentsID == id).ToList();
        if(listArgs.Count() > 0)
        {
            foreach(var item in listArgs)
            {
                ActionArgs actionargsDel = dbActionArgs.ActionArgss.Where(s => s.ActionArgumentsID == item.ActionArgumentsID).FirstOrDefault();
                dbActionArgs.ActionArgss.Remove(actionargsDel);
                dbActionArgs.SaveChanges();
            }
        }
        RuleAction ruleaction = db.RuleActions.Find(id);
        BusinessRule BR = dbRuleAction.BusinessRules.Where(br => br.Id == ruleaction.RuleActionID).FirstOrDefault();
        db.RuleActions.Remove(ruleaction);
        db.SaveChanges();
        //,Hash = "Action"
        return RedirectToAction("Edit", "BusinessRule", new { id = BR.Id });
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
        var lstRuleAction = from s in db.RuleActions
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstRuleAction = searchRecords(lstRuleAction, searchString.ToUpper());
        }
        else
            lstRuleAction = lstRuleAction.OrderByDescending(s => s.Id);
        if(lstRuleAction.Where(p => p.RuleActionID != null).Select(p => p.RuleActionID).Distinct().Count() <= 50)
            ViewBag.ruleaction = new SelectList(lstRuleAction.Where(p => p.ruleaction != null).Select(P => P.ruleaction).Distinct(), "ID", "DisplayValue");
        if(lstRuleAction.Where(p => p.AssociatedActionTypeID != null).Select(p => p.AssociatedActionTypeID).Distinct().Count() <= 50)
            ViewBag.associatedactiontype = new SelectList(lstRuleAction.Where(p => p.associatedactiontype != null).Select(P => P.associatedactiontype).Distinct(), "ID", "DisplayValue");
        return View();
    }
    
    /// <summary>GET: /RuleAction/FSearch/.</summary>
    ///
    /// <param name="currentFilter">       A filter specifying the current.</param>
    /// <param name="searchString">        The search string.</param>
    /// <param name="FSFilter">            A filter specifying the file system.</param>
    /// <param name="sortBy">              Describes who sort this object.</param>
    /// <param name="isAsc">               The is ascending.</param>
    /// <param name="page">                The page.</param>
    /// <param name="itemsPerPage">        The items per page.</param>
    /// <param name="search">              The search.</param>
    /// <param name="IsExport">            The is export.</param>
    /// <param name="ruleaction">          The ruleaction.</param>
    /// <param name="associatedactiontype">The associatedactiontype.</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string ruleaction, string associatedactiontype)//, string HostingEntity
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
        var lstRuleAction = from s in db.RuleActions
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstRuleAction = searchRecords(lstRuleAction, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstRuleAction = sortRecords(lstRuleAction, sortBy, isAsc);
        }
        else
            lstRuleAction = lstRuleAction.OrderByDescending(s => s.Id);
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search;
            lstRuleAction = searchRecords(lstRuleAction, search);
        }
        lstRuleAction = lstRuleAction.Include(t => t.ruleaction).Include(t => t.associatedactiontype);
        var _RuleAction = lstRuleAction;
        if(lstRuleAction.Where(p => p.ruleaction != null).Count() <= 50)
            ViewBag.ruleaction = new SelectList(lstRuleAction.Where(p => p.ruleaction != null).Select(P => P.ruleaction).Distinct(), "ID", "DisplayValue");
        if(ruleaction != null)
        {
            var ids = ruleaction.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            ViewBag.SearchResult += "\r\n Business Rule= ";
            foreach(var str in ids)
                if(!string.IsNullOrEmpty(str))
                {
                    ids1.Add(Convert.ToInt64(str));
                    ViewBag.SearchResult += dbRuleAction.BusinessRules.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                }
            _RuleAction = _RuleAction.Where(p => ids1.ToList().Contains(p.RuleActionID));
        }
        if(lstRuleAction.Where(p => p.associatedactiontype != null).Count() <= 50)
            ViewBag.associatedactiontype = new SelectList(lstRuleAction.Where(p => p.associatedactiontype != null).Select(P => P.associatedactiontype).Distinct(), "ID", "DisplayValue");
        if(associatedactiontype != null)
        {
            var ids = associatedactiontype.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            ViewBag.SearchResult += "\r\n Action Type= ";
            foreach(var str in ids)
                if(!string.IsNullOrEmpty(str))
                {
                    ids1.Add(Convert.ToInt64(str));
                    ViewBag.SearchResult += dbAssociatedActionType.ActionTypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                }
            _RuleAction = _RuleAction.Where(p => ids1.ToList().Contains(p.AssociatedActionTypeID));
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
            if(_RuleAction.Count() > 0)
                pageSize = _RuleAction.Count();
            return View("ExcelExport", _RuleAction.ToPagedList(pageNumber, pageSize));
        }
        return View("Index", _RuleAction.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        var _Obj = db.RuleActions.Find(Convert.ToInt64(id));
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
        IQueryable<RuleAction> list = db.RuleActions.Unfiltered();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.RuleActions;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(RuleAction), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<RuleAction, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<RuleAction>)q);
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
    [ValidateAntiForgeryToken]
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
                    RuleAction model = new RuleAction();
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
                        case "ActionName":
                            model.ActionName = columnValue;
                            break;
                        case "RuleActionID":
                            long ruleactionId = dbRuleAction.BusinessRules.Where(p => p.DisplayValue == columnValue).ToList()[0].Id;
                            model.RuleActionID = ruleactionId;
                            break;
                        case "AssociatedActionTypeID":
                            long associatedactiontypeId = dbAssociatedActionType.ActionTypes.Where(p => p.DisplayValue == columnValue).ToList()[0].Id;
                            model.AssociatedActionTypeID = associatedactiontypeId;
                            break;
                        default:
                            break;
                        }
                    }
                    db.RuleActions.Add(model);
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
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    /// <summary>After save.</summary>
    ///
    /// <param name="RuleAction">The Footer Section .</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(RuleAction ruleaction, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
            if(dbRuleAction !=null) dbRuleAction.Dispose();
            if(dbAssociatedActionType != null) dbAssociatedActionType.Dispose();
            if(dbActionArgs != null) dbActionArgs.Dispose();
        }
        base.Dispose(disposing);
    }
}
}