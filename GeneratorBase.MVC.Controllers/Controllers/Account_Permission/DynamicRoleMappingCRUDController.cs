// file:	Controllers\Account_Permission\DynamicRoleMappingCRUDController.cs
//
// summary:	Implements the dynamic role mapping crud controller class

using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling dynamic role mappings.</summary>
public partial class DynamicRoleMappingController : BaseController
{
    /// <summary>GET: /DynamicRoleMapping/.</summary>
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
    /// <param name="IsDeepSearch">   The is deep search.</param>
    /// <param name="IsFilter">       A filter specifying the is.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    /// <param name="BulkOperation">  The bulk operation.</param>
    /// <param name="parent">         The parent.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent)
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
        ViewData["IsFilter"] = IsFilter;
        ViewData["BulkOperation"] = BulkOperation;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstDynamicRoleMapping = from s in db.DynamicRoleMappings
                                    select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstDynamicRoleMapping = searchRecords(lstDynamicRoleMapping, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstDynamicRoleMapping = sortRecords(lstDynamicRoleMapping, sortBy, isAsc);
        }
        else lstDynamicRoleMapping = lstDynamicRoleMapping.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _DynamicRoleMapping = lstDynamicRoleMapping;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_DynamicRoleMapping.Count() > 0)
                pageSize = _DynamicRoleMapping.Count();
            return View("ExcelExport", _DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _DynamicRoleMapping.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /DynamicRoleMapping/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
        if(dynamicrolemapping == null)
        {
            return HttpNotFound();
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        return View(dynamicrolemapping);
    }
    
    /// <summary>GET: /DynamicRoleMapping/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
    {
        if(!User.CanAdd("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>GET: /DynamicRoleMapping/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        if(!User.CanAdd("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /DynamicRoleMapping/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.DynamicRoleMappings.Add(dynamicrolemapping);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>GET: /DynamicRoleMapping/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
    {
        if(!User.CanAdd("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /DynamicRoleMapping/CreateQuick To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    /// <param name="IsAddPop">          The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.DynamicRoleMappings.Add(dynamicrolemapping);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        LoadViewDataAfterOnCreate(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>Cancels.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    
    public ActionResult Cancel(string UrlReferrer)
    {
        if(!string.IsNullOrEmpty(UrlReferrer))
        {
            var query = HttpUtility.ParseQueryString(UrlReferrer);
            if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                return RedirectToAction("Index");
            else
                return Redirect(UrlReferrer);
        }
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>POST: /DynamicRoleMapping/Create To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    /// <param name="IsDDAdd">           The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer, bool? IsDDAdd)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.DynamicRoleMappings.Add(dynamicrolemapping);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = dynamicrolemapping.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>GET: /DynamicRoleMapping/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
        if(dynamicrolemapping == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
        if(ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping/Edit/" + dynamicrolemapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping/Create"))
            ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>POST: /DynamicRoleMapping/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(dynamicrolemapping).State = EntityState.Modified;
            db.SaveChanges();
            if(command == "Save & Continue")
                return RedirectToAction("Edit", new { Id = dynamicrolemapping.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
            {
                var query = HttpUtility.ParseQueryString(UrlReferrer);
                if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                    if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                        return RedirectToAction("Index");
                    else
                        return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }
        LoadViewDataAfterOnEdit(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>GET: /DynamicRoleMapping/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
        if(dynamicrolemapping == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
        if(ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping"))
            ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>POST: /DynamicRoleMapping/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(dynamicrolemapping).State = EntityState.Modified;
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
            {
                var query = HttpUtility.ParseQueryString(UrlReferrer);
                if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                    return RedirectToAction("Index");
                else
                    return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }
        LoadViewDataAfterOnEdit(dynamicrolemapping);
        return View(dynamicrolemapping);
    }
    
    /// <summary>GET: /DynamicRoleMapping/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
        if(dynamicrolemapping == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping"))
            ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
        return View(dynamicrolemapping);
    }
    
    /// <summary>POST: /DynamicRoleMapping/Delete/5.</summary>
    ///
    /// <param name="dynamicrolemapping">The dynamicrolemapping.</param>
    /// <param name="UrlReferrer">       The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
    {
        if(!User.CanDelete("DynamicRoleMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(dynamicrolemapping).State = EntityState.Deleted;
        db.DynamicRoleMappings.Remove(dynamicrolemapping);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["DynamicRoleMappingParentUrl"] != null)
        {
            string parentUrl = ViewData["DynamicRoleMappingParentUrl"].ToString();
            ViewData["DynamicRoleMappingParentUrl"] = null;
            return Redirect(parentUrl);
        }
        else return RedirectToAction("Index");
    }
    
    /// <summary>Deletes the bulk.</summary>
    ///
    /// <param name="ids">        The identifiers.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteBulk View.</returns>
    
    public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
    {
        foreach(var id in ids.Where(p => p > 0))
        {
            DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
            db.Entry(dynamicrolemapping).State = EntityState.Deleted;
            db.DynamicRoleMappings.Remove(dynamicrolemapping);
            db.SaveChanges();
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
