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
using System.ComponentModel.DataAnnotations;
using System.Drawing;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling import configurations.</summary>
public partial class ImportConfigurationController : BaseController
{
    /// <summary>private ApplicationContext db = new ApplicationContext(User);
    ///  GET: /ImportConfiguration/.</summary>
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
    /// <param name="IsMobileRequest">The is mobile request.</param>
    /// <param name="IsFilter">       A filter specifying the is.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    /// <param name="BulkOperation">  The bulk operation.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsMobileRequest, bool? IsFilter, bool? RenderPartial, string BulkOperation)
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
        var lstImportConfiguration = from s in db.ImportConfigurations
                                     select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstImportConfiguration = searchRecords(lstImportConfiguration, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstImportConfiguration = sortRecords(lstImportConfiguration, sortBy, isAsc);
        }
        else lstImportConfiguration = lstImportConfiguration.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.JavaScriptStringEncode(User.Name)  + "ImportConfiguration"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ImportConfiguration"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.JavaScriptStringEncode(User.Name)  + "ImportConfiguration"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //
        var _ImportConfiguration = lstImportConfiguration;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ImportConfiguration.Count() > 0)
                pageSize = _ImportConfiguration.Count();
            return View("ExcelExport", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
        }
        if(IsMobileRequest != true)
        {
            if(Request.AcceptTypes.Contains("text/html"))
            {
                if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
                    return View(_ImportConfiguration.ToPagedList(pageNumber, pageSize));
                else
                {
                    if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                        return PartialView("BulkOperation", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("IndexPartial", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
                }
            }
            else if(Request.AcceptTypes.Contains("application/json"))
            {
                var Result = getImportConfigurationList(_ImportConfiguration);
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            var Result = getImportConfigurationList(_ImportConfiguration);
            return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_ImportConfiguration.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /ImportConfiguration/Details/5.</summary>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
        if(importconfiguration == null)
        {
            return HttpNotFound();
        }
        if(Request.AcceptTypes.Contains("text/html"))
        {
            return View(importconfiguration);
        }
        else if(Request.AcceptTypes.Contains("application/json"))
        {
            var Result = getImportConfigurationItem(importconfiguration);
            return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View(importconfiguration);
    }
    
    /// <summary>GET: /ImportConfiguration/Create.</summary>
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
        if(!User.CanAdd("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
        if(Request.AcceptTypes.Contains("text/html"))
        {
            return View();
        }
        else if(Request.AcceptTypes.Contains("application/json"))
        {
            var Result = getImportConfigurationItem(new ImportConfiguration());
            return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>GET: /ImportConfiguration/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID)
    {
        if(!User.CanAdd("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>POST: /ImportConfiguration/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name,MappingName,IsDefaultMapping")] ImportConfiguration importconfiguration, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            importconfiguration.LastUpdate = DateTime.UtcNow;
            importconfiguration.LastUpdateUser = User.Name;
            db.ImportConfigurations.Add(importconfiguration);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        return View(importconfiguration);
    }
    
    /// <summary>GET: /ImportConfiguration/CreateQuick.</summary>
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
        if(!User.CanAdd("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        return View();
    }
    
    /// <summary>POST: /ImportConfiguration/CreateQuick To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsAddPop">           The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,Name,MappingName,IsDefaultMapping")] ImportConfiguration importconfiguration, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            importconfiguration.LastUpdate = DateTime.UtcNow;
            importconfiguration.LastUpdateUser = User.Name;
            db.ImportConfigurations.Add(importconfiguration);
            db.SaveChanges();
            //if (IsAddPop != null && IsAddPop == true)
            //  return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //else
            //return Redirect(Request.UrlReferrer.ToString());
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View(importconfiguration);
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
    
    /// <summary>POST: /ImportConfiguration/Create To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsDDAdd">            The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name,MappingName,IsDefaultMapping")] ImportConfiguration importconfiguration, string UrlReferrer, bool? IsDDAdd)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            importconfiguration.LastUpdate = DateTime.UtcNow;
            importconfiguration.LastUpdateUser = User.Name;
            db.ImportConfigurations.Add(importconfiguration);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = importconfiguration.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        return View(importconfiguration);
    }
    
    /// <summary>GET: /ImportConfiguration/Edit/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
        if(importconfiguration == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
        if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration/Edit/" + importconfiguration.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration/Create"))
            ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
        if(Request.AcceptTypes.Contains("text/html"))
        {
            return View(importconfiguration);
        }
        else if(Request.AcceptTypes.Contains("application/json"))
        {
            var Result = getImportConfigurationItem(importconfiguration);
            return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View(importconfiguration);
    }
    
    /// <summary>POST: /ImportConfiguration/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name,MappingName,IsDefaultMapping")] ImportConfiguration importconfiguration, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            importconfiguration.LastUpdate = DateTime.UtcNow;
            importconfiguration.LastUpdateUser = User.Name;
            db.Entry(importconfiguration).State = EntityState.Modified;
            db.SaveChanges();
            if(command == "Save & Continue")
                return RedirectToAction("Edit", new { Id = importconfiguration.Id, UrlReferrer = UrlReferrer });
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
        return View(importconfiguration);
    }
    
    /// <summary>GET: /ImportConfiguration/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
        importconfiguration.LastUpdate = DateTime.UtcNow;
        importconfiguration.LastUpdateUser = User.Name;
        if(importconfiguration == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
        if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration"))
            ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
        return View(importconfiguration);
    }
    
    /// <summary>POST: /ImportConfiguration/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name,MappingName,IsDefaultMapping")] ImportConfiguration importconfiguration, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            db.Entry(importconfiguration).State = EntityState.Modified;
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
        return View(importconfiguration);
    }
    
    /// <summary>GET: /ImportConfiguration/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
        if(importconfiguration == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration"))
            ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
        return View(importconfiguration);
    }
    
    /// <summary>POST: /ImportConfiguration/Delete/5.</summary>
    ///
    /// <param name="importconfiguration">The importconfiguration.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(ImportConfiguration importconfiguration, string UrlReferrer)
    {
        if(!User.CanDelete("ImportConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        //ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
        db.Entry(importconfiguration).State = EntityState.Deleted;
        db.ImportConfigurations.Remove(importconfiguration);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["ImportConfigurationParentUrl"] != null)
        {
            string parentUrl = ViewData["ImportConfigurationParentUrl"].ToString();
            ViewData["ImportConfigurationParentUrl"] = null;
            return Redirect(parentUrl);
        }
        else return RedirectToAction("Index");
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
