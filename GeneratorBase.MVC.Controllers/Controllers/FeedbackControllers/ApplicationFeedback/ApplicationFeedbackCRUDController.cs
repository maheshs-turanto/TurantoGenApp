﻿using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling application feedbacks.</summary>
public partial class ApplicationFeedbackController : BaseController
{
    /// <summary>GET: /ApplicationFeedback/.</summary>
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
        var lstApplicationFeedback = from s in db.ApplicationFeedbacks
                                     select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstApplicationFeedback = searchRecords(lstApplicationFeedback, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstApplicationFeedback = sortRecords(lstApplicationFeedback, sortBy, isAsc);
        }
        else lstApplicationFeedback = lstApplicationFeedback.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _ApplicationFeedback = lstApplicationFeedback.Include(t => t.associatedapplicationfeedbacktype).Include(t => t.associatedapplicationfeedbackstatus).Include(t => t.applicationfeedbackpriority).Include(t => t.applicationfeedbackseverity).Include(t => t.applicationfeedbackresource);
        if(HostingEntity == "ApplicationFeedbackType" && AssociatedType == "AssociatedApplicationFeedbackType")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.AssociatedApplicationFeedbackTypeID == hostid);
            }
            else
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.AssociatedApplicationFeedbackTypeID == null);
        }
        if(HostingEntity == "ApplicationFeedbackStatus" && AssociatedType == "AssociatedApplicationFeedbackStatus")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.AssociatedApplicationFeedbackStatusID == hostid);
            }
            else
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.AssociatedApplicationFeedbackStatusID == null);
        }
        if(HostingEntity == "FeedbackPriority" && AssociatedType == "ApplicationFeedbackPriority")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackPriorityID == hostid);
            }
            else
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackPriorityID == null);
        }
        if(HostingEntity == "FeedbackSeverity" && AssociatedType == "ApplicationFeedbackSeverity")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackSeverityID == hostid);
            }
            else
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackSeverityID == null);
        }
        if(HostingEntity == "FeedbackResource" && AssociatedType == "ApplicationFeedbackResource")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackResourceID == hostid);
            }
            else
                _ApplicationFeedback = _ApplicationFeedback.Where(p => p.ApplicationFeedbackResourceID == null);
        }
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_ApplicationFeedback.Count() > 0)
                pageSize = _ApplicationFeedback.Count();
            return View("ExcelExport", _ApplicationFeedback.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_ApplicationFeedback.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _ApplicationFeedback.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _ApplicationFeedback.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /ApplicationFeedback/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    [Audit]
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedback applicationfeedback = db.ApplicationFeedbacks.Find(id);
        if(applicationfeedback == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "ApplicationFeedback" && p.RecordId == id).ToList();
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        return View(applicationfeedback);
    }
    
    /// <summary>GET: /ApplicationFeedback/Create.</summary>
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
        if(!User.CanAdd("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["ApplicationFeedbackParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>GET: /ApplicationFeedback/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        if(!User.CanAdd("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["ApplicationFeedbackParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /ApplicationFeedback/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedback">The applicationfeedback.</param>
    /// <param name="AttachImage">        The attach image.</param>
    /// <param name="AttachDocument">     The attach document.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,PageName,PageUrlTitle,UIControlName,PageUrl,CommentId,AssociatedApplicationFeedbackTypeID,AssociatedApplicationFeedbackStatusID,ApplicationFeedbackPriorityID,ApplicationFeedbackSeverityID,ApplicationFeedbackResourceID,ReportedBy,ReportedByUser,Summary,Description,AttachImage,AttachDocument")] ApplicationFeedback applicationfeedback, HttpPostedFileBase AttachImage, HttpPostedFileBase AttachDocument, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(AttachImage != null)
            {
                AttachImage.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachImage.FileName));
                applicationfeedback.AttachImage = ticks + System.IO.Path.GetFileName(AttachImage.FileName);
            }
            if(AttachDocument != null)
            {
                AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachDocument.FileName));
                applicationfeedback.AttachDocument = ticks + System.IO.Path.GetFileName(AttachDocument.FileName);
            }
            applicationfeedback.ReportedBy = DateTime.UtcNow;
            applicationfeedback.ReportedByUser = User.Name;
            db.ApplicationFeedbacks.Add(applicationfeedback);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>GET: /ApplicationFeedback/CreateQuick.</summary>
    ///
    /// <param name="EntityName">       Name of the entity.</param>
    /// <param name="UIControlName">    Name of the control.</param>
    /// <param name="FieldName">        Name of the field.</param>
    /// <param name="PageName">         Name of the page.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string EntityName, string UIControlName, string FieldName, string PageName, string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
    {
        if(!User.CanAdd("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ApplicationFeedbackParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["EntityName"] = EntityName;
        ViewData["FieldName"] = FieldName;
        ViewData["PageName"] = PageName;
        ViewData["UIControlName"] = UIControlName;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>POST: /ApplicationFeedback/CreateQuick To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedback">The applicationfeedback.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsAddPop">           The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,PageName,PageUrlTitle,UIControlName,PageUrl,CommentId,AssociatedApplicationFeedbackTypeID,AssociatedApplicationFeedbackStatusID,ApplicationFeedbackPriorityID,ApplicationFeedbackSeverityID,ApplicationFeedbackResourceID,ReportedBy,ReportedByUser,Summary,Description")] ApplicationFeedback applicationfeedback, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            applicationfeedback.ReportedBy = DateTime.UtcNow;
            applicationfeedback.ReportedByUser = User.Name;
            db.ApplicationFeedbacks.Add(applicationfeedback);
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
        LoadViewDataAfterOnCreate(applicationfeedback);
        return View(applicationfeedback);
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
    
    /// <summary>POST: /ApplicationFeedback/Create To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedback">           The applicationfeedback.</param>
    /// <param name="AttachImage">                   The attach image.</param>
    /// <param name="CamerafileUploadAttachImage">   The camerafile upload attach image.</param>
    /// <param name="AttachDocument">                The attach document.</param>
    /// <param name="CamerafileUploadAttachDocument">The camerafile upload attach document.</param>
    /// <param name="UrlReferrer">                   The URL referrer.</param>
    /// <param name="IsDDAdd">                       The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,PageName,PageUrlTitle,UIControlName,PageUrl,CommentId,AssociatedApplicationFeedbackTypeID,AssociatedApplicationFeedbackStatusID,ApplicationFeedbackPriorityID,ApplicationFeedbackSeverityID,ApplicationFeedbackResourceID,ReportedBy,ReportedByUser,Summary,Description,AttachImage,AttachDocument")] ApplicationFeedback applicationfeedback, HttpPostedFileBase AttachImage, String CamerafileUploadAttachImage, HttpPostedFileBase AttachDocument, String CamerafileUploadAttachDocument, string UrlReferrer, bool? IsDDAdd)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(AttachImage != null)
            {
                AttachImage.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachImage.FileName));
                applicationfeedback.AttachImage = ticks + System.IO.Path.GetFileName(AttachImage.FileName);
            }
            if(Request.Form["CamerafileUploadAttachImage"] != null && Request.Form["CamerafileUploadAttachImage"] != "")
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachImage"])));
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                applicationfeedback.AttachImage = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
            }
            if(AttachDocument != null)
            {
                AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachDocument.FileName));
                applicationfeedback.AttachDocument = ticks + System.IO.Path.GetFileName(AttachDocument.FileName);
            }
            if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"])));
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                applicationfeedback.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
            }
            applicationfeedback.ReportedBy = DateTime.UtcNow;
            applicationfeedback.ReportedByUser = User.Name;
            db.ApplicationFeedbacks.Add(applicationfeedback);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = applicationfeedback.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>GET: /ApplicationFeedback/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedback applicationfeedback = db.ApplicationFeedbacks.Find(id);
        if(applicationfeedback == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "ApplicationFeedback" && p.RecordId == id).ToList();
        if(UrlReferrer != null)
            ViewData["ApplicationFeedbackParentUrl"] = UrlReferrer;
        if(ViewData["ApplicationFeedbackParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedback") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedback/Edit/" + applicationfeedback.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedback/Create"))
            ViewData["ApplicationFeedbackParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>POST: /ApplicationFeedback/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedback">           The applicationfeedback.</param>
    /// <param name="File_AttachImage">              The file attach image.</param>
    /// <param name="CamerafileUploadAttachImage">   The camerafile upload attach image.</param>
    /// <param name="File_AttachDocument">           The file attach document.</param>
    /// <param name="CamerafileUploadAttachDocument">The camerafile upload attach document.</param>
    /// <param name="UrlReferrer">                   The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,PageName,PageUrlTitle,UIControlName,PageUrl,CommentId,AssociatedApplicationFeedbackTypeID,AssociatedApplicationFeedbackStatusID,ApplicationFeedbackPriorityID,ApplicationFeedbackSeverityID,ApplicationFeedbackResourceID,ReportedBy,ReportedByUser,Summary,Description,AttachImage,AttachDocument")] ApplicationFeedback applicationfeedback, HttpPostedFileBase File_AttachImage, String CamerafileUploadAttachImage, HttpPostedFileBase File_AttachDocument, String CamerafileUploadAttachDocument, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_AttachImage != null)
            {
                File_AttachImage.SaveAs(path + ticks + System.IO.Path.GetFileName(File_AttachImage.FileName));
                applicationfeedback.AttachImage = ticks + System.IO.Path.GetFileName(File_AttachImage.FileName);
            }
            if(Request.Form["CamerafileUploadAttachImage"] != null && Request.Form["CamerafileUploadAttachImage"] != "")
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachImage"])));
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                applicationfeedback.AttachImage = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
            }
            if(File_AttachDocument != null)
            {
                File_AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName));
                applicationfeedback.AttachDocument = ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName);
            }
            if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"])));
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                applicationfeedback.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
            }
            applicationfeedback.ReportedBy = DateTime.UtcNow;
            applicationfeedback.ReportedByUser = User.Name;
            db.Entry(applicationfeedback).State = EntityState.Modified;
            db.SaveChanges();
            if(command != "Save")
                return RedirectToAction("Edit", new { Id = applicationfeedback.Id, UrlReferrer = UrlReferrer });
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
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "ApplicationFeedback" && p.RecordId == applicationfeedback.Id).ToList();
        LoadViewDataAfterOnEdit(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>GET: /ApplicationFeedback/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedback applicationfeedback = db.ApplicationFeedbacks.Find(id);
        applicationfeedback.ReportedBy = DateTime.UtcNow;
        applicationfeedback.ReportedByUser = User.Name;
        if(applicationfeedback == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "ApplicationFeedback" && p.RecordId == id).ToList();
        if(UrlReferrer != null)
            ViewData["ApplicationFeedbackParentUrl"] = UrlReferrer;
        if(ViewData["ApplicationFeedbackParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedback"))
            ViewData["ApplicationFeedbackParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>POST: /ApplicationFeedback/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="applicationfeedback">           The applicationfeedback.</param>
    /// <param name="File_AttachImage">              The file attach image.</param>
    /// <param name="CamerafileUploadAttachImage">   The camerafile upload attach image.</param>
    /// <param name="File_AttachDocument">           The file attach document.</param>
    /// <param name="CamerafileUploadAttachDocument">The camerafile upload attach document.</param>
    /// <param name="UrlReferrer">                   The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,PageName,PageUrlTitle,UIControlName,PageUrl,CommentId,AssociatedApplicationFeedbackTypeID,AssociatedApplicationFeedbackStatusID,ApplicationFeedbackPriorityID,ApplicationFeedbackSeverityID,ApplicationFeedbackResourceID,ReportedBy,ReportedByUser,Summary,Description,AttachImage,AttachDocument")] ApplicationFeedback applicationfeedback, HttpPostedFileBase File_AttachImage, String CamerafileUploadAttachImage, HttpPostedFileBase File_AttachDocument, String CamerafileUploadAttachDocument, string UrlReferrer)
    {
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_AttachImage != null)
            {
                File_AttachImage.SaveAs(path + ticks + System.IO.Path.GetFileName(File_AttachImage.FileName));
                applicationfeedback.AttachImage = ticks + System.IO.Path.GetFileName(File_AttachImage.FileName);
            }
            if(File_AttachDocument != null)
            {
                File_AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName));
                applicationfeedback.AttachDocument = ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName);
            }
            db.Entry(applicationfeedback).State = EntityState.Modified;
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
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "ApplicationFeedback" && p.RecordId == applicationfeedback.Id).ToList();
        }
        LoadViewDataAfterOnEdit(applicationfeedback);
        return View(applicationfeedback);
    }
    
    /// <summary>GET: /ApplicationFeedback/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ApplicationFeedback applicationfeedback = db.ApplicationFeedbacks.Find(id);
        if(applicationfeedback == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["ApplicationFeedbackParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedback"))
            ViewData["ApplicationFeedbackParentUrl"] = Request.UrlReferrer;
        return View(applicationfeedback);
    }
    
    /// <summary>POST: /ApplicationFeedback/Delete/5.</summary>
    ///
    /// <param name="applicationfeedback">The applicationfeedback.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(ApplicationFeedback applicationfeedback, string UrlReferrer)
    {
        if(!User.CanDelete("ApplicationFeedback"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(applicationfeedback).State = EntityState.Deleted;
        db.ApplicationFeedbacks.Remove(applicationfeedback);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["ApplicationFeedbackParentUrl"] != null)
        {
            string parentUrl = ViewData["ApplicationFeedbackParentUrl"].ToString();
            ViewData["ApplicationFeedbackParentUrl"] = null;
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
            ApplicationFeedback applicationfeedback = db.ApplicationFeedbacks.Find(id);
            db.Entry(applicationfeedback).State = EntityState.Deleted;
            db.ApplicationFeedbacks.Remove(applicationfeedback);
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
