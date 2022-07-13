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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling schedules.</summary>
public partial class T_ScheduleController : BaseController
{
    /// <summary>GET: /T_Schedule/.</summary>
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
    
    [Audit("ViewList")]
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype)
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
        var lstT_Schedule = from s in db.T_Schedules
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_Schedule = searchRecords(lstT_Schedule, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstT_Schedule = sortRecords(lstT_Schedule, sortBy, isAsc);
        }
        else lstT_Schedule = lstT_Schedule.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _T_Schedule = lstT_Schedule.Include(t => t.t_associatedscheduletype).Include(t => t.t_associatedrecurringscheduledetailstype).Include(t => t.t_recurringrepeatfrequency).Include(t => t.t_repeatby).Include(t => t.t_recurringtaskendtype);
        if(HostingEntity == "T_Scheduletype" && AssociatedType == "T_AssociatedScheduleType")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Schedule = _T_Schedule.Where(p => p.T_AssociatedScheduleTypeID == hostid);
            }
            else
                _T_Schedule = _T_Schedule.Where(p => p.T_AssociatedScheduleTypeID == null);
        }
        if(HostingEntity == "T_RecurringScheduleDetailstype" && AssociatedType == "T_AssociatedRecurringScheduleDetailsType")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Schedule = _T_Schedule.Where(p => p.T_AssociatedRecurringScheduleDetailsTypeID == hostid);
            }
            else
                _T_Schedule = _T_Schedule.Where(p => p.T_AssociatedRecurringScheduleDetailsTypeID == null);
        }
        if(HostingEntity == "T_RecurringFrequency" && AssociatedType == "T_RecurringRepeatFrequency")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Schedule = _T_Schedule.Where(p => p.T_RecurringRepeatFrequencyID == hostid);
            }
            else
                _T_Schedule = _T_Schedule.Where(p => p.T_RecurringRepeatFrequencyID == null);
        }
        if(HostingEntity == "T_MonthlyRepeatType" && AssociatedType == "T_RepeatBy")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Schedule = _T_Schedule.Where(p => p.T_RepeatByID == hostid);
            }
            else
                _T_Schedule = _T_Schedule.Where(p => p.T_RepeatByID == null);
        }
        if(HostingEntity == "T_RecurringEndType" && AssociatedType == "T_RecurringTaskEndType")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Schedule = _T_Schedule.Where(p => p.T_RecurringTaskEndTypeID == hostid);
            }
            else
                _T_Schedule = _T_Schedule.Where(p => p.T_RecurringTaskEndTypeID == null);
        }
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_T_Schedule.Count() > 0)
                pageSize = _T_Schedule.Count();
            return View("ExcelExport", _T_Schedule.ToPagedList(pageNumber, pageSize));
        }
        var PagedListT_Schedule = _T_Schedule.ToList();
        PagedListT_Schedule.ForEach(p => p.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList());
        PagedListT_Schedule.ForEach(p => p.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == p.Id).Select(m => m.T_RecurrenceDaysID).ToList());
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            ViewBag.TemplatesName = "IndexPartial";
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                ViewBag.TemplatesName = viewtype;
            return View(_T_Schedule.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _T_Schedule.ToPagedList(pageNumber, pageSize));
            else
            {
                if(ViewBag.TemplatesName == null)
                    return PartialView("IndexPartial", _T_Schedule.ToPagedList(pageNumber, pageSize));
                else
                    return PartialView(ViewBag.TemplatesName, _T_Schedule.ToPagedList(pageNumber, pageSize));
            }
        }
    }
    
    /// <summary>GET: /T_Schedule/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    [Audit("View")]
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_Schedule t_schedule = db.T_Schedules.Find(id);
        if(t_schedule == null)
        {
            return HttpNotFound();
        }
        t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        ViewBag.TemplatesName = "Details";
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_schedule);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_schedule, AssociatedType);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnDetails");
        return View(ViewBag.TemplatesName, t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/Create.</summary>
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
        if(!User.CanAdd("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnCreate");
        return View();
    }
    
    /// <summary>GET: /T_Schedule/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        if(!User.CanAdd("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /T_Schedule/CreateWizard To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            db.T_Schedules.Add(t_schedule);
            db.SaveChanges();
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach(var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                    objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                    objT_RepeatOn.T_RecurrenceDaysID = pgs;
                    db.T_RepeatOns.Add(objT_RepeatOn);
                }
                db.SaveChanges();
            }
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        LoadViewDataAfterOnCreate(t_schedule);
        return View(t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/CreateQuick.</summary>
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
        if(!User.CanAdd("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /T_Schedule/CreateQuick To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule">      The schedule.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            db.T_Schedules.Add(t_schedule);
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
        LoadViewDataAfterOnCreate(t_schedule);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_schedule, AssociatedEntity);
        return View(t_schedule);
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
    
    /// <summary>POST: /T_Schedule/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsDDAdd">    The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer, bool? IsDDAdd)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.T_Schedules.Add(t_schedule);
            db.SaveChanges();
            //bool flagT_RepeatOn = false;
            //foreach (var obj in db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id))
            //{
            //    db.T_RepeatOns.Remove(obj);
            //    flagT_RepeatOn = true;
            //}
            //if (flagT_RepeatOn)
            //    db.SaveChanges();
            //if (t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            //{
            //    foreach (var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
            //    {
            //        T_RepeatOn objT_RepeatOn = new T_RepeatOn();
            //        objT_RepeatOn.T_ScheduleID = t_schedule.Id;
            //        objT_RepeatOn.T_RecurrenceDaysID = pgs;
            //        db.T_RepeatOns.Add(objT_RepeatOn);
            //    }
            //    db.SaveChanges();
            //}
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = t_schedule.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(t_schedule);
        return View(t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [Audit("View")]
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_Schedule t_schedule = db.T_Schedules.Find(id);
        if(t_schedule == null)
        {
            return HttpNotFound();
        }
        //t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        if(UrlReferrer != null)
            ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        if(ViewData["T_ScheduleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule/Edit/" + t_schedule.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule/Create"))
            ViewData["T_ScheduleParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_schedule);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnEdit");
        return View(t_schedule);
    }
    
    /// <summary>POST: /T_Schedule/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule">      The schedule.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(t_schedule).State = EntityState.Modified;
            db.SaveChanges();
            bool flagT_RepeatOn = false;
            var obj_repeatons = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id);
            foreach(var obj in obj_repeatons)
            {
                db.T_RepeatOns.Remove(obj);
                flagT_RepeatOn = true;
            }
            if(flagT_RepeatOn)
                db.SaveChanges();
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach(var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                    objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                    objT_RepeatOn.T_RecurrenceDaysID = pgs;
                    db.T_RepeatOns.Add(objT_RepeatOn);
                }
                db.SaveChanges();
            }
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
        LoadViewDataAfterOnEdit(t_schedule);
        return View(t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [Audit("View")]
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_Schedule t_schedule = db.T_Schedules.Find(id);
        if(t_schedule == null)
        {
            return HttpNotFound();
        }
        //t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        if(UrlReferrer != null)
            ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        if(ViewData["T_ScheduleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule/Edit/" + t_schedule.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule/Create"))
            ViewData["T_ScheduleParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_schedule);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnEdit");
        return View(t_schedule);
    }
    
    /// <summary>POST: /T_Schedule/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(t_schedule).State = EntityState.Modified;
            db.SaveChanges();
            bool flagT_RepeatOn = false;
            var obj_repeatons = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id);
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
                obj_repeatons = obj_repeatons.Where(a => !t_schedule.SelectedT_RecurrenceDays_T_RepeatOn.Contains(a.T_RecurrenceDaysID));
            foreach(var obj in obj_repeatons)
            {
                db.T_RepeatOns.Remove(obj);
                flagT_RepeatOn = true;
            }
            if(flagT_RepeatOn)
                db.SaveChanges();
            flagT_RepeatOn = false;
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach(var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    if(db.T_RepeatOns.FirstOrDefault(a => a.T_ScheduleID == t_schedule.Id && a.T_RecurrenceDaysID == pgs) == null)
                    {
                        T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                        objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                        objT_RepeatOn.T_RecurrenceDaysID = pgs;
                        db.Entry(objT_RepeatOn).State = EntityState.Added;
                        db.T_RepeatOns.Add(objT_RepeatOn);
                        flagT_RepeatOn = true;
                    }
                }
                if(flagT_RepeatOn)
                    db.SaveChanges();
            }
            if(command != "Save")
                return RedirectToAction("Edit", new { Id = t_schedule.Id, UrlReferrer = UrlReferrer });
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
        t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        LoadViewDataAfterOnEdit(t_schedule);
        return View(t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [Audit("View")]
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_Schedule t_schedule = db.T_Schedules.Find(id);
        if(t_schedule == null)
        {
            return HttpNotFound();
        }
        //t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        if(UrlReferrer != null)
            ViewData["T_ScheduleParentUrl"] = UrlReferrer;
        if(ViewData["T_ScheduleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule"))
            ViewData["T_ScheduleParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(t_schedule);
        ViewBag.T_ScheduleIsHiddenRule = checkHidden("T_Schedule", "OnEdit");
        return View(t_schedule);
    }
    
    /// <summary>POST: /T_Schedule/EditWizard/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, string UrlReferrer)
    {
        CheckBeforeSave(t_schedule);
        if(ModelState.IsValid)
        {
            db.Entry(t_schedule).State = EntityState.Modified;
            db.SaveChanges();
            bool flagT_RepeatOn = false;
            var obj_repeatons = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id);
            foreach(var obj in obj_repeatons)
            {
                db.T_RepeatOns.Remove(obj);
                flagT_RepeatOn = true;
            }
            if(flagT_RepeatOn)
                db.SaveChanges();
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach(var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                    objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                    objT_RepeatOn.T_RecurrenceDaysID = pgs;
                    db.T_RepeatOns.Add(objT_RepeatOn);
                }
                db.SaveChanges();
            }
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
        t_schedule.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.ToList();
        t_schedule.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id).Select(p => p.T_RecurrenceDaysID).ToList();
        LoadViewDataAfterOnEdit(t_schedule);
        return View(t_schedule);
    }
    
    /// <summary>GET: /T_Schedule/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    [Audit("Delete")]
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_Schedule t_schedule = db.T_Schedules.Find(id);
        if(t_schedule == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_ScheduleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Schedule"))
            ViewData["T_ScheduleParentUrl"] = Request.UrlReferrer;
        return View(t_schedule);
    }
    
    /// <summary>POST: /T_Schedule/Delete/5.</summary>
    ///
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_Schedule t_schedule, string UrlReferrer)
    {
        if(!User.CanDelete("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(CheckBeforeDelete(t_schedule))
        {
            //Delete Document
            db.Entry(t_schedule).State = EntityState.Deleted;
            db.T_Schedules.Remove(t_schedule);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if(ViewData["T_ScheduleParentUrl"] != null)
            {
                string parentUrl = ViewData["T_ScheduleParentUrl"].ToString();
                ViewData["T_ScheduleParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
        return View(t_schedule);
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
        if(HostingEntity == "T_Scheduletype" && AssociatedType == "T_AssociatedScheduleType")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_Schedule obj = db.T_Schedules.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_AssociatedScheduleTypeID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "T_RecurringScheduleDetailstype" && AssociatedType == "T_AssociatedRecurringScheduleDetailsType")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_Schedule obj = db.T_Schedules.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_AssociatedRecurringScheduleDetailsTypeID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "T_RecurringFrequency" && AssociatedType == "T_RecurringRepeatFrequency")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_Schedule obj = db.T_Schedules.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_RecurringRepeatFrequencyID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "T_MonthlyRepeatType" && AssociatedType == "T_RepeatBy")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_Schedule obj = db.T_Schedules.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_RepeatByID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "T_RecurringEndType" && AssociatedType == "T_RecurringTaskEndType")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_Schedule obj = db.T_Schedules.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_RecurringTaskEndTypeID = HostingID;
                db.SaveChanges();
            }
        }
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
            T_Schedule t_schedule = db.T_Schedules.Find(id);
            db.Entry(t_schedule).State = EntityState.Deleted;
            db.T_Schedules.Remove(t_schedule);
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
        if(!User.CanEdit("T_Schedule"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["T_ScheduleParentUrl"] = UrlReferrer;
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
    /// <param name="t_schedule"> The schedule.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_AssociatedScheduleTypeID,T_StartDateTime,T_AssociatedRecurringScheduleDetailsTypeID,T_RecurringRepeatFrequencyID,T_RepeatByID,T_RecurringTaskEndTypeID,T_EndDate,T_OccurrenceLimitCount,T_Summary,SelectedT_RecurrenceDays_T_RepeatOn,T_StartTime,T_EndTime,T_EntityName")] T_Schedule t_schedule, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_Schedule target = db.T_Schedules.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_schedule, target, chkUpdate);
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
