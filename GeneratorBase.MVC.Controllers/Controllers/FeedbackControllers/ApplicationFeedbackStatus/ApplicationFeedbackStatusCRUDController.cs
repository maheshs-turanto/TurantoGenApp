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
/// <summary>A controller for handling application feedback status.</summary>
public partial class ApplicationFeedbackStatusController : BaseController
{
    /// <summary>GET: /ApplicationFeedbackStatus/.</summary>
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
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent)
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
        var lstApplicationFeedbackStatus = from s in db.ApplicationFeedbackStatuss
                                           select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstApplicationFeedbackStatus = searchRecords(lstApplicationFeedbackStatus, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstApplicationFeedbackStatus = sortRecords(lstApplicationFeedbackStatus, sortBy, isAsc);
        }
        else lstApplicationFeedbackStatus = lstApplicationFeedbackStatus.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackStatus"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackStatus"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _ApplicationFeedbackStatus = lstApplicationFeedbackStatus;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ApplicationFeedbackStatus.Count() > 0)
                pageSize = _ApplicationFeedbackStatus.Count();
            return View("ExcelExport", _ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _ApplicationFeedbackStatus.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
        if(applicationfeedbackstatus == null)
        {
            return HttpNotFound();
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
    {
        if(!User.CanAdd("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
    {
        if(!User.CanAdd("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /ApplicationFeedbackStatus/CreateWizard To protect from overposting attacks,
    /// please enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/CreateQuick.</summary>
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
        if(!User.CanAdd("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /ApplicationFeedbackStatus/CreateQuick To protect from overposting attacks,
    /// please enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    /// <param name="IsAddPop">                 The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
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
                    errors+=error.ErrorMessage+".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        LoadViewDataAfterOnCreate(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
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
    
    /// <summary>POST: /ApplicationFeedbackStatus/Create To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    /// <param name="IsDDAdd">                  The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer, bool? IsDDAdd)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = applicationfeedbackstatus.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
        if(applicationfeedbackstatus == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
        if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus/Edit/" + applicationfeedbackstatus.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus/Create"))
            ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>POST: /ApplicationFeedbackStatus/Edit/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(applicationfeedbackstatus).State = EntityState.Modified;
            db.SaveChanges();
            if(command != "Save")
                return RedirectToAction("Edit", new { Id = applicationfeedbackstatus.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnEdit(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
        if(applicationfeedbackstatus == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
        if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus"))
            ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>POST: /ApplicationFeedbackStatus/EditWizard/5 To protect from overposting attacks,
    /// please enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(applicationfeedbackstatus).State = EntityState.Modified;
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
        LoadViewDataAfterOnEdit(applicationfeedbackstatus);
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>GET: /ApplicationFeedbackStatus/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
        if(applicationfeedbackstatus == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus"))
            ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
        return View(applicationfeedbackstatus);
    }
    
    /// <summary>POST: /ApplicationFeedbackStatus/Delete/5.</summary>
    ///
    /// <param name="applicationfeedbackstatus">The applicationfeedbackstatus.</param>
    /// <param name="UrlReferrer">              The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(ApplicationFeedbackStatus applicationfeedbackstatus, string UrlReferrer)
    {
        if(!User.CanDelete("ApplicationFeedbackStatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(applicationfeedbackstatus).State = EntityState.Deleted;
        db.ApplicationFeedbackStatuss.Remove(applicationfeedbackstatus);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["ApplicationFeedbackStatusParentUrl"] != null)
        {
            string parentUrl = ViewData["ApplicationFeedbackStatusParentUrl"].ToString();
            ViewData["ApplicationFeedbackStatusParentUrl"] = null;
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
            ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
            db.Entry(applicationfeedbackstatus).State = EntityState.Deleted;
            db.ApplicationFeedbackStatuss.Remove(applicationfeedbackstatus);
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
