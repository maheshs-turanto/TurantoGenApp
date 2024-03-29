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
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling monthly repeat types.</summary>
public partial class T_MonthlyRepeatTypeController : BaseController
{
    /// <summary>GET: /T_MonthlyRepeatType/.</summary>
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
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype)
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
        ViewData["caller"] = caller;
        if(!string.IsNullOrEmpty(viewtype))
        {
            viewtype = viewtype.Replace("?IsAddPop=true", "");
            ViewBag.TemplatesName = viewtype;
        }
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstT_MonthlyRepeatType = from s in db.T_MonthlyRepeatTypes
                                     select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_MonthlyRepeatType = searchRecords(lstT_MonthlyRepeatType, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstT_MonthlyRepeatType = sortRecords(lstT_MonthlyRepeatType, sortBy, isAsc);
        }
        else lstT_MonthlyRepeatType = lstT_MonthlyRepeatType.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_MonthlyRepeatType"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_MonthlyRepeatType"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_MonthlyRepeatType"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_MonthlyRepeatType"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_MonthlyRepeatType"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_MonthlyRepeatType"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _T_MonthlyRepeatType = lstT_MonthlyRepeatType;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_T_MonthlyRepeatType.Count() > 0)
                pageSize = _T_MonthlyRepeatType.Count();
            return View("ExcelExport", _T_MonthlyRepeatType.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            ViewBag.TemplatesName = "IndexPartial";
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                ViewBag.TemplatesName = viewtype;
            return View(_T_MonthlyRepeatType.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
            {
                ViewData["BulkAssociate"] = BulkAssociate;
                if(caller != "")
                {
                    FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                    _T_MonthlyRepeatType = _fad.FilterDropdown<T_MonthlyRepeatType>(User,  _T_MonthlyRepeatType, "T_MonthlyRepeatType", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation",sortRecords(lstT_MonthlyRepeatType.Except(_T_MonthlyRepeatType),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstT_MonthlyRepeatType.Except(_T_MonthlyRepeatType).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _T_MonthlyRepeatType.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _T_MonthlyRepeatType.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                    return PartialView("IndexPartial", _T_MonthlyRepeatType.ToPagedList(pageNumber, pageSize));
                else
                    return PartialView(ViewBag.TemplatesName, _T_MonthlyRepeatType.ToPagedList(pageNumber, pageSize));
            }
        }
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/Details/5.</summary>
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
        T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
        if(t_monthlyrepeattype == null)
        {
            return HttpNotFound();
        }
        ViewBag.TemplatesName = "Details";
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_monthlyrepeattype);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_monthlyrepeattype, AssociatedType);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnDetails");
        return View(ViewBag.TemplatesName,t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/Create.</summary>
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
        if(!User.CanAdd("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType","OnCreate");
        return View();
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
    {
        if(!User.CanAdd("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype, string UrlReferrer)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            db.T_MonthlyRepeatTypes.Add(t_monthlyrepeattype);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(t_monthlyrepeattype);
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/CreateQuick.</summary>
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
        if(!User.CanAdd("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/CreateQuick To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsAddPop">           The is add pop.</param>
    /// <param name="AssociatedEntity">   The associated entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            db.T_MonthlyRepeatTypes.Add(t_monthlyrepeattype);
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
        LoadViewDataAfterOnCreate(t_monthlyrepeattype);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_monthlyrepeattype, AssociatedEntity);
        return View(t_monthlyrepeattype);
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
    
    /// <summary>POST: /T_MonthlyRepeatType/Create To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsDDAdd">            The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype, string UrlReferrer, bool? IsDDAdd)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.T_MonthlyRepeatTypes.Add(t_monthlyrepeattype);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = t_monthlyrepeattype.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(t_monthlyrepeattype);
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
        if(t_monthlyrepeattype == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        if(ViewData["T_MonthlyRepeatTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType/Edit/" + t_monthlyrepeattype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType/Create"))
            ViewData["T_MonthlyRepeatTypeParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_monthlyrepeattype);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnEdit");
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    /// <param name="IsAddPop">           The is add pop.</param>
    /// <param name="AssociatedEntity">   The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(t_monthlyrepeattype).State = EntityState.Modified;
            db.SaveChanges();
            return Json(UrlReferrer, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnEdit(t_monthlyrepeattype);
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
        if(t_monthlyrepeattype == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        if(ViewData["T_MonthlyRepeatTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType/Edit/" + t_monthlyrepeattype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType/Create"))
            ViewData["T_MonthlyRepeatTypeParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_monthlyrepeattype);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnEdit");
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/Edit/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype,  string UrlReferrer)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(t_monthlyrepeattype).State = EntityState.Modified;
            db.SaveChanges();
            if(command != "Save")
                return RedirectToAction("Edit", new { Id = t_monthlyrepeattype.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnEdit(t_monthlyrepeattype);
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
        if(t_monthlyrepeattype == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        if(ViewData["T_MonthlyRepeatTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType"))
            ViewData["T_MonthlyRepeatTypeParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(t_monthlyrepeattype);
        ViewBag.T_MonthlyRepeatTypeIsHiddenRule = checkHidden("T_MonthlyRepeatType", "OnEdit");
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype,string UrlReferrer)
    {
        CheckBeforeSave(t_monthlyrepeattype);
        if(ModelState.IsValid)
        {
            db.Entry(t_monthlyrepeattype).State = EntityState.Modified;
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
        LoadViewDataAfterOnEdit(t_monthlyrepeattype);
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>GET: /T_MonthlyRepeatType/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
        if(t_monthlyrepeattype == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_MonthlyRepeatTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MonthlyRepeatType"))
            ViewData["T_MonthlyRepeatTypeParentUrl"] = Request.UrlReferrer;
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>POST: /T_MonthlyRepeatType/Delete/5.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_MonthlyRepeatType t_monthlyrepeattype, string UrlReferrer)
    {
        if(!User.CanDelete("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(CheckBeforeDelete(t_monthlyrepeattype))
        {
//Delete Document
            db.Entry(t_monthlyrepeattype).State = EntityState.Deleted;
            db.T_MonthlyRepeatTypes.Remove(t_monthlyrepeattype);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if(ViewData["T_MonthlyRepeatTypeParentUrl"] != null)
            {
                string parentUrl = ViewData["T_MonthlyRepeatTypeParentUrl"].ToString();
                ViewData["T_MonthlyRepeatTypeParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
        return View(t_monthlyrepeattype);
    }
    
    /// <summary>Bulk associate.</summary>
    ///
    /// <param name="ids">            The identifiers.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the BulkAssociate View.</returns>
    
    public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
    {
        var HostingID = Convert.ToInt64(HostingEntityID);
        if(HostingID == 0)
            return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
            T_MonthlyRepeatType t_monthlyrepeattype = db.T_MonthlyRepeatTypes.Find(id);
            db.Entry(t_monthlyrepeattype).State = EntityState.Deleted;
            db.T_MonthlyRepeatTypes.Remove(t_monthlyrepeattype);
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) bulk update.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDUpdate">       The is dd update.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpGet]
    public ActionResult BulkUpdate(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
    {
        if(!User.CanEdit("T_MonthlyRepeatType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["T_MonthlyRepeatTypeParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        string ids = string.Empty;
        if(Request.QueryString["ids"] != null)
            ids = Request.QueryString["ids"];
        ViewBag.BulkUpdate = ids;
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) bulk update.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="collection">         The collection.</param>
    /// <param name="UrlReferrer">        The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_MonthlyRepeatType t_monthlyrepeattype,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_MonthlyRepeatType target = db.T_MonthlyRepeatTypes.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_monthlyrepeattype, target, chkUpdate);
                db.Entry(target).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
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
