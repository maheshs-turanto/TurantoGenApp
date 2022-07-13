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
using System.ComponentModel.DataAnnotations;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling default entity pages.</summary>
public partial class DefaultEntityPageController : BaseController
{
    /// <summary>GET: /DefaultEntityPage/.</summary>
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
        if(!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
            return RedirectToAction("Index", "Error");
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
        var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                   select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstDefaultEntityPage = searchRecords(lstDefaultEntityPage, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstDefaultEntityPage = sortRecords(lstDefaultEntityPage, sortBy, isAsc);
        }
        else lstDefaultEntityPage = lstDefaultEntityPage.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _DefaultEntityPage = lstDefaultEntityPage;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_DefaultEntityPage.Count() > 0)
                pageSize = _DefaultEntityPage.Count();
            return View("ExcelExport", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_DefaultEntityPage.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _DefaultEntityPage.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /DefaultEntityPage/Details/5.</summary>
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
        DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
        if(defaultentitypage == null)
        {
            return HttpNotFound();
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        return View(defaultentitypage);
    }
    
    /// <summary>GET: /DefaultEntityPage/Create.</summary>
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
        if(!User.CanAdd("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>GET: /DefaultEntityPage/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        if(!User.IsAdmin)
            return RedirectToAction("Index", "Error");
        if(!User.CanAdd("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /DefaultEntityPage/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer)
    {
        if(!User.IsAdmin)
            return RedirectToAction("Index", "Error");
        if(ModelState.IsValid)
        {
            db.DefaultEntityPages.Add(defaultentitypage);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>GET: /DefaultEntityPage/CreateQuick.</summary>
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
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(!User.CanAdd("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /DefaultEntityPage/CreateQuick To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag,ViewEntityPage,ListEntityPage,EditEntityPage,SearchEntityPage,CreateEntityPage,HomePage")] DefaultEntityPage defaultentitypage, string UrlReferrer, bool? IsAddPop)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(ModelState.IsValid)
        {
            if(defaultentitypage.HomePage != null)
            {
                var last4 = defaultentitypage.HomePage.Substring(defaultentitypage.HomePage.Length - 4);
                if(last4.ToLower() == "home")
                {
                    defaultentitypage.HomePage = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == defaultentitypage.EntityName).DisplayName;
                }
            }
            //var page = db.DefaultEntityPages.FirstOrDefault(p => p.EntityName == defaultentitypage.EntityName);
            //if (page == null)
            //{
            db.DefaultEntityPages.Add(defaultentitypage);
            //}
            //else
            //{
            //    page.EntityName = defaultentitypage.EntityName;
            //    page.PageType = defaultentitypage.PageType;
            //    page.PageUrl = defaultentitypage.PageUrl;
            //    page.Roles = defaultentitypage.Roles;
            //    page.Other = defaultentitypage.Other;
            //    page.Flag = defaultentitypage.Flag;
            //    db.Entry(defaultentitypage).State = EntityState.Modified;
            //}
            db.SaveChanges();
            return RedirectToAction("Index");
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
        LoadViewDataAfterOnCreate(defaultentitypage);
        return View(defaultentitypage);
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
    
    /// <summary>POST: /DefaultEntityPage/Create To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer, bool? IsDDAdd)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            if(defaultentitypage.HomePage != null)
            {
                var last4 = defaultentitypage.HomePage.Substring(defaultentitypage.HomePage.Length - 4);
                if(last4.ToLower() == "home")
                {
                    defaultentitypage.HomePage = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == defaultentitypage.EntityName).DisplayName;
                }
            }
            db.DefaultEntityPages.Add(defaultentitypage);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = defaultentitypage.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>GET: /DefaultEntityPage/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(!User.CanEdit("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
        if(defaultentitypage == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
        if(ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage/Edit/" + defaultentitypage.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage/Create"))
            ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>POST: /DefaultEntityPage/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag,ViewEntityPage,ListEntityPage,EditEntityPage,SearchEntityPage,CreateEntityPage,HomePage")] DefaultEntityPage defaultentitypage, string UrlReferrer)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            if(defaultentitypage.HomePage != null)
            {
                var last4 = defaultentitypage.HomePage.Substring(defaultentitypage.HomePage.Length - 4);
                if(last4.ToLower() == "home")
                {
                    defaultentitypage.HomePage = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == defaultentitypage.EntityName).DisplayName;
                }
            }
            db.Entry(defaultentitypage).State = EntityState.Modified;
            db.SaveChanges();
            if(command == "Save & Continue")
                return RedirectToAction("Edit", new { Id = defaultentitypage.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnEdit(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>GET: /DefaultEntityPage/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.IsAdmin)
            return RedirectToAction("Index", "Error");
        if(!User.CanEdit("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
        if(defaultentitypage == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
        if(ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage"))
            ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>POST: /DefaultEntityPage/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer)
    {
        if(!User.IsAdmin)
            return RedirectToAction("Index", "Error");
        if(ModelState.IsValid)
        {
            db.Entry(defaultentitypage).State = EntityState.Modified;
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
        LoadViewDataAfterOnEdit(defaultentitypage);
        return View(defaultentitypage);
    }
    
    /// <summary>GET: /DefaultEntityPage/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(!User.CanDelete("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
        if(defaultentitypage == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage"))
            ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
        return View(defaultentitypage);
    }
    
    /// <summary>POST: /DefaultEntityPage/Delete/5.</summary>
    ///
    /// <param name="defaultentitypage">The defaultentitypage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(DefaultEntityPage defaultentitypage, string UrlReferrer)
    {
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        if(!User.CanDelete("DefaultEntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(defaultentitypage).State = EntityState.Deleted;
        db.DefaultEntityPages.Remove(defaultentitypage);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["DefaultEntityPageParentUrl"] != null)
        {
            string parentUrl = ViewData["DefaultEntityPageParentUrl"].ToString();
            ViewData["DefaultEntityPageParentUrl"] = null;
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
        //if (!User.IsAdmin)
        //    return RedirectToAction("Index", "Error");
        foreach(var id in ids.Where(p => p > 0))
        {
            DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
            db.Entry(defaultentitypage).State = EntityState.Deleted;
            db.DefaultEntityPages.Remove(defaultentitypage);
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
    /// <summary>Bindtemplates index.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A response stream to send to the BindtemplatesIndex View.</returns>
    
    public ActionResult BindtemplatesIndex(string EntityName)
    {
        //Dictionary object for all templates
        Dictionary<string, string> dicIndexFile = new Dictionary<string, string>();
        Dictionary<string, string> dicDetailsFile = new Dictionary<string, string>();
        Dictionary<string, string> dicEditFile = new Dictionary<string, string>();
        Dictionary<string, string> dicSearchFile = new Dictionary<string, string>();
        Dictionary<string, string> dicCreateFile = new Dictionary<string, string>();
        Dictionary<string, string> dicHomeFile = new Dictionary<string, string>();
        //
        //get all cshtml page from view dirctory for an entity.
        var viewsName = System.IO.Directory.EnumerateFiles(Server.MapPath(@"~/Views/" + EntityName)).ToList();
        //bind file name of an entity page in Dictionary
        foreach(var item in viewsName)
        {
            string fileName = Path.GetFileName(item).Replace(".cshtml", "");
            string dispalyName = "";
            if(fileName.ToLower().Contains(".mobile"))
                continue;
            // getting index page
            if(fileName.ToLower().Contains("indexpartial"))
            {
                if(fileName.ToLower() == "indexpartial")
                {
                    dispalyName = "Table(Default)";
                }
                else
                {
                    dispalyName = fileName.Replace("IndexPartial", "");
                    if(dispalyName.ToLower() == "gallery")
                        dicIndexFile.Add("List", "IndexPartialList");
                }
                dicIndexFile.Add(dispalyName, fileName);
            }
            //
            //getting details page
            if(fileName.ToLower().Contains("details"))
            {
                if(fileName.ToLower() == "details")
                    dispalyName = "(Detail)Default";
                else
                    dispalyName = fileName.Replace("Details", "");
                dicDetailsFile.Add(dispalyName, fileName);
            }
            //
            //getting Edit page
            if(fileName.ToLower().Contains("edit"))
            {
                if(fileName.ToLower() == "edit")
                    dispalyName = "(Edit)Default";
                else
                    dispalyName = fileName.Replace("Edit", "");
                dicEditFile.Add(dispalyName, fileName);
            }
            //
            //getting Search page
            if(fileName.ToLower().Contains("search"))
            {
                if(fileName.ToLower() == "setfsearch")
                    dispalyName = "Faceted Search(Default)";
                else
                    dispalyName = fileName.Replace("Search", "");
                dicSearchFile.Add(dispalyName, fileName);
            }
            //
            //getting Search page
            if(fileName.ToLower().Contains("create"))
            {
                if(fileName.ToLower() == "create")
                    dispalyName = "(Create)Default";
                else
                    dispalyName = fileName.Replace("Create", "");
                dicCreateFile.Add(dispalyName, fileName);
            }
            //
            //getting Home page
            if(fileName.ToLower().Contains("home") || EntityName.ToLower() == "home")
            {
                if(fileName.ToLower() == "Index")
                    dispalyName = "(Home)Default";
                else if(fileName.ToLower() == "about" || fileName.ToLower() == "contact")
                    continue;
                else
                    dispalyName = fileName;
                dicHomeFile.Add(dispalyName, fileName);
            }
        }
        //
        return Json(new
        {
            IndexPages = dicIndexFile,
            DetailsPage = dicDetailsFile,
            EditPage = dicEditFile,
            SearchPage = dicSearchFile,
            CreatePage = dicCreateFile,
            HomePage = dicHomeFile
        }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    public ActionResult GetTemplateViews(string EntityName)
    {
        ////Added by rachana
        //Dictionary object for all templates
        Dictionary<string, string> dicIndexFile = new Dictionary<string, string>();
        Dictionary<string, string> dicDetailsFile = new Dictionary<string, string>();
        Dictionary<string, string> dicEditFile = new Dictionary<string, string>();
        Dictionary<string, string> dicSearchFile = new Dictionary<string, string>();
        Dictionary<string, string> dicCreateFile = new Dictionary<string, string>();
        Dictionary<string, string> dicPopupCardFile = new Dictionary<string, string>();
        Dictionary<string, string> dicKanbanFile = new Dictionary<string, string>();
        Dictionary<string, string> dicHomeFile = new Dictionary<string, string>();
        if(!string.IsNullOrWhiteSpace(EntityName))
        {
            //
            //get all cshtml page from view dirctory for an entity.
            var viewsName = System.IO.Directory.EnumerateFiles(Server.MapPath(@"~/Views/" + EntityName)).ToList();
            //bind file name of an entity page in Dictionary
            foreach(var item in viewsName)
            {
                string fileName = Path.GetFileName(item).Replace(".cshtml", "");
                string dispalyName = "";
                if(fileName.ToLower().Contains(".mobile") || fileName.ToLower().Contains("createpartial")
                        || fileName.ToLower().Contains("quick")) //|| fileName.ToLower().Contains("indexpartiallist")
                    continue;
                // getting index page
                if(fileName.ToLower().Contains("indexpartial"))
                {
                    //  viewtype = "IndexPartialList";
                    //  viewtype = "IndexPartialGallery";
                    if(fileName.ToLower() == "indexpartial" || fileName.ToLower() == "indexpartiallist")
                        dispalyName = "List"; //"Table(Default)";
                    else if(fileName.ToLower() == "IndexPartialGallery")
                        dispalyName = "Gallery View";
                    else
                        dispalyName = fileName.Replace("IndexPartial", "");
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicIndexFile.Add(dispalyName, fileName);
                }
                //
                //getting details page
                if(fileName.ToLower().Contains("details") || fileName.ToLower().Contains("detail"))
                {
                    if(fileName.ToLower() == "details")
                        dispalyName = "Detail";
                    //dispalyName = "(Detail)Default";
                    else
                        dispalyName = fileName.Replace("Details", "");
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicDetailsFile.Add(dispalyName, fileName);
                }
                //
                //getting Edit page
                if(fileName.ToLower().Contains("edit"))
                {
                    if(fileName.ToLower() == "edit")
                        dispalyName = "Edit";
                    //dispalyName = "(Edit)Default";
                    else
                        dispalyName = fileName.Replace("Edit", "");
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicEditFile.Add(dispalyName, fileName);
                }
                //
                //getting Search page
                if(fileName.ToLower().Contains("search"))
                {
                    if(fileName.ToLower() == "setfsearch")
                        dispalyName = "Search";
                    //dispalyName = "Faceted Search(Default)";
                    else
                        dispalyName = fileName.Replace("Search", "");
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicSearchFile.Add(dispalyName, fileName);
                }
                //
                if(fileName.ToLower().Contains("create"))
                {
                    if(fileName.ToLower() == "create")
                        dispalyName = "Create";
                    //dispalyName = "(Create)Default";
                    else
                        dispalyName = fileName.Replace("Create", "");
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicCreateFile.Add(dispalyName, fileName);
                }
                //
                //getting PopupCard page
                if(fileName.ToLower().Contains("popupcard"))
                {
                    if(fileName.ToLower() == "popupcard")
                        dispalyName = "PopupCard";
                    if(!dicIndexFile.ContainsKey(dispalyName))
                        dicPopupCardFile.Add(dispalyName, fileName);
                }
                //getting KanBan page
                if(fileName.ToLower().Contains("kanban"))
                {
                    //if (fileName.ToLower() == "kanban")
                    //    dispalyName = "KanBan";
                    //if (!dicIndexFile.ContainsKey(dispalyName))
                    //    dicKanbanFile.Add(dispalyName, fileName);
                    //var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == EntityName);
                    //List<GeneratorBase.MVC.ModelReflector.Association> assocList = entList.Associations;
                    //ViewBag.assocList = assocList;
                }
                //
                ////getting Home page
                //if (fileName.ToLower().Contains("home") || EntityName.ToLower() == "home")
                //{
                //    if (fileName.ToLower() == "Index")
                //        dispalyName = "(Home)Default";
                //    else if (fileName.ToLower() == "about" || fileName.ToLower() == "contact")
                //        continue;
                //    else
                //        dispalyName = fileName;
                //  if (!dicIndexFile.ContainsKey(dispalyName))
                //    dicHomeFile.Add(dispalyName, fileName);
                //}
            }
            //
        }
        return Json(new
        {
            IndexPages = dicIndexFile,
            DetailsPage = dicDetailsFile,
            EditPage = dicEditFile,
            SearchPage = dicSearchFile,
            CreatePage = dicCreateFile,
            HomePage = dicHomeFile,
            PopupCardPage = dicPopupCardFile,
            KanbanPage = dicKanbanFile
            
        }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    public ActionResult GetViewNames()
    {
        //Dictionary object for all templates
        Dictionary<string, string> dicLayoutFolder = new Dictionary<string, string>();
        //
        //get all cshtml page from view dirctory for an entity.
        var FoldersName = System.IO.Directory.EnumerateDirectories(Server.MapPath(@"~/")).ToList();
        //bind file name of an entity page in Dictionary
        foreach(var folder in FoldersName)
        {
            string folderName = folder.Split(new string[] { @"\"
                                                          }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(); ;
            string partvalue = "";
            // getting index page
            if(folderName == "Views")
            {
                partvalue = folderName.ToLower().Substring(folderName.ToLower().IndexOf("views") + "views".Length);
                dicLayoutFolder.Add("Angular" + partvalue, folderName);
            }
            else if(folderName.ToLower().Contains("viewsdefault"))
            {
                partvalue = folderName.ToLower().Substring(folderName.ToLower().IndexOf("viewsdefault") + "viewsdefault".Length);
                dicLayoutFolder.Add("Default" + partvalue, folderName);
            }
        }
        //
        return Json(new
        {
            LayoutNames = dicLayoutFolder
        }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    public ActionResult GetPageNamesofLayouts(string viewname, string pagename)
    {
        Dictionary<string, string> dicLayoutpage = new Dictionary<string, string>();
        //
        //get all cshtml page from view dirctory for an entity.
        var viewsName = System.IO.Directory.EnumerateFiles(Server.MapPath(@"~/" + viewname + "/Shared/")).ToList();
        //bind file name of an entity page in Dictionary
        foreach(var filename in viewsName)
        {
            string fileName = Path.GetFileName("_" + filename).Replace(".cshtml", "");
            if(fileName.ToLower().Contains(".mobile"))
                continue;
            // getting layout page
            if(fileName.ToLower().Contains(pagename.ToLower()))
            {
                dicLayoutpage.Add(fileName, fileName);
            }
        }
        //
        return Json(new
        {
            Layoutpages = dicLayoutpage
        }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
}
}
