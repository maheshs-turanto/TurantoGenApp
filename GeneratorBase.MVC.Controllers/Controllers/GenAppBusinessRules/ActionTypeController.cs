﻿using System;
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
/// <summary>A controller for handling action types.</summary>
public class ActionTypeController : BaseController
{
    /// <summary>The database.</summary>
    private ActionTypeContext db = new ActionTypeContext();
    
    /// <summary>GET: /ActionType/.</summary>
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
        if(!((CustomPrincipal)User).CanAddAdminFeature("BusinessRule"))
        {
            return RedirectToAction("Index", "Error");
        }
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
        var lstActionType = from s in db.ActionTypes
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionType = searchRecords(lstActionType, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstActionType = sortRecords(lstActionType, sortBy, isAsc);
        }
        else
            lstActionType = lstActionType.OrderBy(s => s.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        var _ActionType = lstActionType;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ActionType.Count() > 0)
                pageSize = _ActionType.Count();
            return View("ExcelExport", _ActionType.ToPagedList(pageNumber, pageSize));
        }
        return View(_ActionType.ToPagedList(pageNumber, pageSize));
    }
    
    //public void ResetTemplate()
    //{
    //    var actiontypedata = db.ActionTypes.ToList();
    // //   ViewBag.OriginalTemplate = OriginalTemplate;
    //    for(int i = 0; i< actiontypedata.Count(); i ++)
    //    {
    //        var originaltemplate = actiontypedata[i].OriginalTemplate;
    //        actiontypedata[i].Template = originaltemplate;
    //        db.SaveChanges();
    //    }
    
    
    //}
    [Audit("Reset Alert Style")]
    public JsonResult ResetTemplate()
    {
        var actiontypedata = db.ActionTypes.ToList();
        //   ViewBag.OriginalTemplate = OriginalTemplate;
        for(int i = 0; i < actiontypedata.Count(); i++)
        {
            var originaltemplate = actiontypedata[i].OriginalTemplate;
            if(actiontypedata[i].Template != null && originaltemplate == actiontypedata[i].Template)
                continue;
            actiontypedata[i].Template = originaltemplate;
        }
        db.SaveChanges();
        return Json("success", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstActionType">Type of the list action.</param>
    /// <param name="searchString"> The search string.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<ActionType> searchRecords(IQueryable<ActionType> lstActionType, string searchString)
    {
        lstActionType = lstActionType.Where(s => (s.TypeNo != null && SqlFunctions.StringConvert((double)s.TypeNo).Contains(searchString)) || (!String.IsNullOrEmpty(s.ActionTypeName) && s.ActionTypeName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)));
        return lstActionType;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstActionType">Type of the list action.</param>
    /// <param name="sortBy">       Describes who sort this object.</param>
    /// <param name="isAsc">        The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<ActionType> sortRecords(IQueryable<ActionType> lstActionType, string sortBy, string isAsc)
    {
        var elementType = typeof(ActionType);
        var param = Expression.Parameter(elementType, "actiontype");
        var prop = elementType.GetProperty(sortBy);
        Type type = Nullable.GetUnderlyingType(prop.PropertyType);
        if(type == null)
            type = prop.PropertyType;
        if(type.Equals(typeof(string)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, string>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(char)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, char?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(int)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, int?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(float)) || type.Equals(typeof(double)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, double?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(decimal)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, decimal?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(long)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, long?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        if(type.Equals(typeof(DateTime)))
        {
            var mySortExpression = Expression.Lambda<Func<ActionType, DateTime?>>(Expression.Property(param, sortBy), param);
            if(isAsc == "ASC")
                lstActionType = lstActionType.OrderBy(mySortExpression);
            else
                lstActionType = lstActionType.OrderByDescending(mySortExpression);
        }
        // This last part won't work but I left it so that it can compile (all routes need to return value etc.)
        return lstActionType;
    }
    
    /// <summary>GET: /ActionType/Details/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    //public ActionResult Details(int? id)
    //{
    //    if (id == null)
    //    {
    //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //    }
    //    ActionType actiontype = db.ActionTypes.Find(id);
    //    if (actiontype == null)
    //    {
    //        return HttpNotFound();
    //    }
    //    return View(actiontype);
    //}
    
    /// <summary>GET: /ActionType/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    //public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    //{
    //    if (!User.CanAdd("ActionType"))
    //    {
    //        return RedirectToAction("Index", "Error");
    //    }
    //    ViewData["ActionTypeParentUrl"] = UrlReferrer;
    //    return View();
    //}
    
    /// <summary>GET: /ActionType/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    //public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    //{
    //    if (!User.CanAdd("ActionType"))
    //    {
    //        return RedirectToAction("Index", "Error");
    //    }
    //    ViewData["ActionTypeParentUrl"] = UrlReferrer;
    //    return View();
    //}
    
    /// <summary>POST: /ActionType/CreateWizard To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actiontype"> The actiontype.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult CreateWizard([Bind(Include = "Id,TypeNo,ActionTypeName,Description")] ActionType actiontype, string UrlReferrer)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        db.ActionTypes.Add(actiontype);
    //        db.SaveChanges();
    //        if (!string.IsNullOrEmpty(UrlReferrer))
    //            return Redirect(UrlReferrer);
    //        else return RedirectToAction("Index");
    //    }
    //    return View(actiontype);
    //}
    
    /// <summary>GET: /ActionType/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    //public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID)
    //{
    //    if (!User.CanAdd("ActionType"))
    //    {
    //        return RedirectToAction("Index", "Error");
    //    }
    //    ViewData["ActionTypeParentUrl"] = UrlReferrer;
    //    return View();
    //}
    
    /// <summary>POST: /ActionType/CreateQuick To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actiontype"> The actiontype.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult CreateQuick([Bind(Include = "Id,TypeNo,ActionTypeName,Description")] ActionType actiontype, string UrlReferrer)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        db.ActionTypes.Add(actiontype);
    //        db.SaveChanges();
    //        return Redirect(Request.UrlReferrer.ToString());
    //    }
    //    return View(actiontype);
    //}
    
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
    
    /// <summary>POST: /ActionType/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actiontype"> The actiontype.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Create([Bind(Include = "Id,TypeNo,ActionTypeName,Description")] ActionType actiontype, string UrlReferrer)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        db.ActionTypes.Add(actiontype);
    //        db.SaveChanges();
    //        if (!string.IsNullOrEmpty(UrlReferrer))
    //            return Redirect(UrlReferrer);
    //        else return RedirectToAction("Index");
    //    }
    //    return View(actiontype);
    //}
    
    /// <summary>GET: /ActionType/Edit/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id)
    {
        if(!User.CanEdit("ActionType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionType actiontype = db.ActionTypes.Find(id);
        if(actiontype == null)
        {
            return HttpNotFound();
        }
        ViewBag.orginaltemplate = actiontype.OriginalTemplate;
        if(ViewData["ActionTypeParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionType"))
            ViewData["ActionTypeParentUrl"] = Request.UrlReferrer;
        return View(actiontype);
    }
    
    /// <summary>POST: /ActionType/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actiontype"> The actiontype.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,TypeNo,ActionTypeName,Description,Template,OriginalTemplate")] ActionType actiontype, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            if(actiontype.Template != null && actiontype.Template.Trim() == "<p><br></p>")
                actiontype.Template = null;
            //actiontype.OriginalTemplate = Convert.ToString(ViewBag.orginaltemplate);
            db.Entry(actiontype).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        return View(actiontype);
    }
    
    /// <summary>GET: /ActionType/EditWizard/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id)
    {
        if(!User.CanEdit("ActionType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ActionType actiontype = db.ActionTypes.Find(id);
        if(actiontype == null)
        {
            return HttpNotFound();
        }
        if(ViewData["ActionTypeParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionType"))
            ViewData["ActionTypeParentUrl"] = Request.UrlReferrer;
        return View(actiontype);
    }
    
    /// <summary>POST: /ActionType/EditWizard/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="actiontype"> The actiontype.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,TypeNo,ActionTypeName,Description")] ActionType actiontype, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(actiontype).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        return View(actiontype);
    }
    
    /// <summary>GET: /ActionType/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    //public ActionResult Delete(int? id)
    //{
    //    if (!User.CanDelete("ActionType"))
    //    {
    //        return RedirectToAction("Index", "Error");
    //    }
    //    if (id == null)
    //    {
    //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //    }
    //    ActionType actiontype = db.ActionTypes.Find(id);
    //    if (actiontype == null)
    //    {
    //        return HttpNotFound();
    //    }
    //    if (ViewData["ActionTypeParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ActionType"))
    //        ViewData["ActionTypeParentUrl"] = Request.UrlReferrer;
    //    return View(actiontype);
    //}
    
    /// <summary>POST: /ActionType/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public ActionResult DeleteConfirmed(int id)
    //{
    //    if (!User.CanDelete("ActionType"))
    //    {
    //        return RedirectToAction("Index", "Error");
    //    }
    //    ActionType actiontype = db.ActionTypes.Find(id);
    //    db.ActionTypes.Remove(actiontype);
    //    db.SaveChanges();
    //    if (ViewData["ActionTypeParentUrl"] != null)
    //    {
    //        string parentUrl = ViewData["ActionTypeParentUrl"].ToString();
    //        ViewData["ActionTypeParentUrl"] = null;
    //        return Redirect(parentUrl);
    //    }
    //    else return RedirectToAction("Index");
    //}
    
    /// <summary>Sets f search.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="HostingEntity">The hosting entity.</param>
    ///
    /// <returns>A response stream to send to the SetFSearch View.</returns>
    
    public ActionResult SetFSearch(string searchString, string HostingEntity)
    {
        ViewBag.CurrentFilter = searchString;
        var lstActionType = from s in db.ActionTypes
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionType = searchRecords(lstActionType, searchString.ToUpper());
        }
        else
            lstActionType = lstActionType.OrderByDescending(s => s.Id);
        return View();
    }
    
    /// <summary>GET: /ActionType/FSearch/.</summary>
    ///
    /// <param name="currentFilter">A filter specifying the current.</param>
    /// <param name="searchString"> The search string.</param>
    /// <param name="FSFilter">     A filter specifying the file system.</param>
    /// <param name="sortBy">       Describes who sort this object.</param>
    /// <param name="isAsc">        The is ascending.</param>
    /// <param name="page">         The page.</param>
    /// <param name="itemsPerPage"> The items per page.</param>
    /// <param name="search">       The search.</param>
    /// <param name="IsExport">     The is export.</param>
    /// <param name="TypeNoFrom">   The type no from.</param>
    /// <param name="TypeNoTo">     The type no to.</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string TypeNoFrom, string TypeNoTo)//, string HostingEntity
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
        var lstActionType = from s in db.ActionTypes
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstActionType = searchRecords(lstActionType, searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstActionType = sortRecords(lstActionType, sortBy, isAsc);
        }
        else
            lstActionType = lstActionType.OrderByDescending(s => s.Id);
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search;
            lstActionType = searchRecords(lstActionType, search);
        }
        lstActionType = lstActionType;
        var _ActionType = lstActionType;
        if(TypeNoFrom != null && TypeNoTo != null)
        {
            try
            {
                ViewBag.SearchResult += "\r\n TypeNo= " + TypeNoFrom + "-" + TypeNoTo;
                int from = Convert.ToInt32(TypeNoFrom);
                int to = Convert.ToInt32(TypeNoTo);
                _ActionType = _ActionType.Where(o => o.TypeNo >= from && o.TypeNo <= to);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
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
            if(_ActionType.Count() > 0)
                pageSize = _ActionType.Count();
            return View("ExcelExport", _ActionType.ToPagedList(pageNumber, pageSize));
        }
        return View("Index", _ActionType.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        var _Obj = db.ActionTypes.Find(Convert.ToInt64(id));
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
        IQueryable<ActionType> list = db.ActionTypes.Unfiltered();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.ActionTypes;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(ActionType), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<ActionType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<ActionType>)q);
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
                    ActionType model = new ActionType();
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
                        case "TypeNo":
                            model.TypeNo = Int32.Parse(columnValue);
                            break;
                        case "ActionTypeName":
                            model.ActionTypeName = columnValue;
                            break;
                        default:
                            break;
                        }
                    }
                    db.ActionTypes.Add(model);
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