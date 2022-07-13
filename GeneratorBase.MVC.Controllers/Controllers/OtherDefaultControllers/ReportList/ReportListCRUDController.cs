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
using EntityFramework.DynamicFilters;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{

/// <summary>A controller for handling report lists.</summary>
public partial class ReportListController : BaseController
{
    /// <summary>GET: /ReportList/.</summary>
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
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     List of is homes.</param>
    /// <param name="IsDivRender">    (Optional) True if is div render, false if not.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, string HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype,string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false)
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
        ViewBag.IsDivRender = IsDivRender;
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
        var lstReportList = from s in db.ReportLists.AsNoTracking()
                            select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstReportList = searchRecords(lstReportList, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstReportList = sortRecords(lstReportList, sortBy, isAsc);
        }
        else lstReportList = lstReportList.OrderBy(c=>c.ReportNo).ThenBy(c=>c.reportsgroupssrsreportassociation.DisplayOrder);
        lstReportList = CustomSorting(lstReportList,HostingEntity,AssociatedType,sortBy,isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ReportList"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ReportList"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ReportList"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportList"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportList"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportList"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        if(TempData["ErrorInfo"] != null)
        {
            ViewBag.ErrorInfo = TempData["ErrorInfo"];
        }
        var _ReportList = lstReportList;
        if(HostingEntity == "ReportsGroup" && AssociatedType == "ReportsGroupSSRSReportAssociation")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _ReportList = _ReportList.Where(p => p.ReportsGroupSSRSReportAssociationID == hostid);
            }
            else
                _ReportList = _ReportList.Where(p => p.ReportsGroupSSRSReportAssociationID == null);
        }
        if(HostingEntity == "Role" && AssociatedType == "ReportsInRole")
        {
            if(HostingEntityID != null)
            {
                string hostid = Convert.ToString(HostingEntityID);
                var list = db.ReportsInRoles.Where(r => r.RoleId == hostid).Select(p => p.ReportId).Distinct().Select(long.Parse).ToList().ToList();
                _ReportList = _ReportList.Where(p => list.Contains(p.Id));
            }
            else
            {
                var list = db.ReportsInRoles.Select(p => p.ReportId).Distinct().Select(long.Parse).ToList().ToList();
                _ReportList = _ReportList.Where(p => !list.Contains(p.Id));
            }
        }
        if(Convert.ToBoolean(IsExport))
        {
            if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "ReportList", User) || !User.CanView("ReportList"))
            {
                return RedirectToAction("Index", "Error");
            }
            pageNumber = 1;
            if(_ReportList.Count() > 0)
                pageSize = _ReportList.Count();
            return View("ExcelExport", _ReportList.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _ReportList.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        if(string.IsNullOrEmpty(viewtype))
            viewtype = "IndexPartial";
        ViewBag.TemplatesName = "IndexPartial";
        TempData["ReportListlist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _ReportList.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityReportListDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["ReportListlist"] = list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            });
            return View(list);
        }
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
            {
                ViewBag.TemplatesName = "IndexPartial";
                ViewData["BulkAssociate"] = BulkAssociate;
                if(!string.IsNullOrEmpty(caller))
                {
                    FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                    _ReportList = _fad.FilterDropdown<ReportList>(User,  _ReportList, "ReportList", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation",sortRecords(lstReportList.Except(_ReportList),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstReportList.Except(_ReportList).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _ReportList.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _ReportList.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _ReportList.Count() == 0 ? 1 : _ReportList.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _ReportList.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityReportListDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["ReportListlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _ReportList.Count() == 0 ? 1 : _ReportList.Count();
                    }
                    var list = _ReportList.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityReportListDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["ReportListlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    
    /// <summary>GET: /ReportList/Details/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        if(!CustomAuthorizationBeforeDetails(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            db.DisableFilter("IsDeleted");
            ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport == null)
                return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = "Details";
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(ssrsreport);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(ssrsreport, AssociatedType);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnDetails", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnDetails", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnDetails",new long[] { 6, 8 });
        return View(ViewBag.TemplatesName,ssrsreport);
    }
    
    /// <summary>GET: /ReportList/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype,bool RenderPartial = false)
    {
        if(!User.CanAdd("ReportList") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        //if (string.IsNullOrEmpty(viewtype))
        viewtype = "Create";
        ViewBag.TemplatesName = "Create";
        ViewData["ReportListParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    /// <summary>GET: /ReportList/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
    {
        if(!User.CanAdd("ReportList") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "CreateWizard";
        // GetTemplatesForCreateWizard(viewtype);
        ViewData["ReportListParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        return View();
    }
    
    /// <summary>POST: /ReportList/CreateWizard To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport"> The ssrsreport.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport, string UrlReferrer)
    {
        CheckBeforeSave(ssrsreport);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(ssrsreport,out customcreate_hasissues,"Create"))
            {
                db.ReportLists.Add(ssrsreport);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(ssrsreport,"Create","");
                if(customRedirectAction != null) return customRedirectAction;
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        LoadViewDataAfterOnCreate(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        return View(ssrsreport);
    }
    
    /// <summary>GET: /ReportList/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("ReportList") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "CreateQuick";
        //GetTemplatesForCreateQuick(viewtype);
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ReportListParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        return View();
    }
    
    /// <summary>POST: /ReportList/CreateQuick To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport">       The ssrsreport.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName")] ReportList ssrsreport,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(ssrsreport);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(ssrsreport,out customcreate_hasissues,"Create"))
            {
                db.ReportLists.Add(ssrsreport);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
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
        LoadViewDataAfterOnCreate(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(ssrsreport, AssociatedEntity);
        return View(ssrsreport);
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
    
    /// <summary>POST: /ReportList/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport">       The ssrsreport.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(ssrsreport, command);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(ssrsreport,out customcreate_hasissues,command))
            {
                db.ReportLists.Add(ssrsreport);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(ssrsreport,"Create",command);
                if(customRedirectAction != null) return customRedirectAction;
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = ssrsreport.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = ssrsreport.Id, UrlReferrer = UrlReferrer });
                }
                if(command == "Create & Add another")
                    return Redirect(Convert.ToString(Request.UrlReferrer));
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(ssrsreport);
        ViewData["ReportListParentUrl"] = UrlReferrer;
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnCreate", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnCreate", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnCreate",new long[] { 6, 7});
        return View(ssrsreport);
    }
    
    /// <summary>GET: /ReportList/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("ReportList") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            db.DisableFilter("IsDeleted");
            ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport == null)
                return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ReportListParentUrl"] = UrlReferrer;
        if(ViewData["ReportListParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList/Edit/" + ssrsreport.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList/Create"))
            ViewData["ReportListParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        var objReportList = new List<ReportList>();
        ViewBag.ReportListDD = new SelectList(objReportList, "ID", "DisplayValue");
        return View(ssrsreport);
    }
    
    /// <summary>POST: /ReportList/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport">      The ssrsreport.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(ssrsreport, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(ssrsreport,out customsave_hasissues,command))
            {
                db.Entry(ssrsreport).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Succeed", UrlRefr =UrlReferrer };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
            var result = new { Result = "fail", UrlRefr = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if(db.Entry(ssrsreport).State == EntityState.Detached)
            ssrsreport = db.ReportLists.Find(ssrsreport.Id);
        LoadViewDataAfterOnEdit(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        return View(ssrsreport);
    }
    
    /// <summary>GET: /ReportList/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RenderPartial = false)
    {
        if(!User.CanEdit("ReportList") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            db.DisableFilter("IsDeleted");
            ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport == null)
                return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["ReportListlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.ReportLists.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityReportListDisplayValueEdit = TempData["ReportListlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["ReportListlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityReportListDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = "Edit";
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = ssrsreport.DisplayValue, Value = ssrsreport.Id.ToString() }));
            ViewBag.EntityReportListDisplayValueEdit = newList;
            TempData["ReportListlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(ssrsreport.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = ssrsreport.DisplayValue;
                newList[0].Value = ssrsreport.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = ssrsreport.DisplayValue, Value = ssrsreport.Id.ToString() }));
            }
            ViewBag.EntityReportListDisplayValueEdit = newList;
            TempData["ReportListlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["ReportListParentUrl"] = UrlReferrer;
        if(ViewData["ReportListParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList/Edit/" + ssrsreport.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList/Create"))
            ViewData["ReportListParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,ssrsreport);
    }
    
    /// <summary>POST: /ReportList/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport">   The ssrsreport.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport,  string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(ssrsreport, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(ssrsreport,out customsave_hasissues,command))
            {
                db.Entry(ssrsreport).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(ssrsreport,"Edit",command);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = ssrsreport.Id };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                if(command != "Save")
                {
                    if(command == "SaveNextPrev")
                    {
                        long NextPreId = Convert.ToInt64(Request.Form["hdnNextPrevId"]);
                        return RedirectToAction("Edit", new { Id = NextPreId, UrlReferrer = UrlReferrer });
                    }
                    else
                        return RedirectToAction("Edit", new { Id = ssrsreport.Id, UrlReferrer = UrlReferrer });
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
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["ReportListlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.ReportLists.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityReportListDisplayValueEdit = TempData["ReportListlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["ReportListlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityReportListDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(ssrsreport).State == EntityState.Detached)
            ssrsreport = db.ReportLists.Find(ssrsreport.Id);
        LoadViewDataAfterOnEdit(ssrsreport);
        ViewData["ReportListParentUrl"] = UrlReferrer;
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        return View(ssrsreport);
    }
    
    /// <summary>GET: /ReportList/EditWizard/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer,string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("ReportList") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            db.DisableFilter("IsDeleted");
            ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport == null)
                return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ReportListParentUrl"] = UrlReferrer;
        if(ViewData["ReportListParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList"))
            ViewData["ReportListParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        return View(ssrsreport);
    }
    
    /// <summary>POST: /ReportList/EditWizard/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="ssrsreport"> The ssrsreport.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport,string UrlReferrer)
    {
        CheckBeforeSave(ssrsreport);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(ssrsreport,out customsave_hasissues,"Save"))
            {
                db.Entry(ssrsreport).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(ssrsreport,"Edit","");
                if(customRedirectAction != null) return customRedirectAction;
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
        }
        if(db.Entry(ssrsreport).State == EntityState.Detached)
            ssrsreport = db.ReportLists.Find(ssrsreport.Id);
        LoadViewDataAfterOnEdit(ssrsreport);
        ViewBag.ReportListIsHiddenRule = checkHidden("ReportList", "OnEdit", false);
        ViewBag.ReportListIsGroupsHiddenRule = checkHidden("ReportList", "OnEdit", true);
        ViewBag.ReportListIsSetValueUIRule = checkSetValueUIRule("ReportList", "OnEdit",new long[] { 6, 8});
        return View(ssrsreport);
    }
    
    /// <summary>GET: /ReportList/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("ReportList") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["ReportListParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportList"))
            ViewData["ReportListParentUrl"] = Request.UrlReferrer;
        return View(ssrsreport);
    }
    
    /// <summary>POST: /ReportList/Delete/5.</summary>
    ///
    /// <param name="ssrsreport"> The ssrsreport.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(ReportList ssrsreport, string UrlReferrer)
    {
        if(!User.CanDelete("ReportList"))
        {
            return RedirectToAction("Index", "Error");
        }
        ssrsreport = db.ReportLists.Find(ssrsreport.Id);
        if(CheckBeforeDelete(ssrsreport))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(ssrsreport, out customdelete_hasissues, "Delete"))
            {
                db.Entry(ssrsreport).State = EntityState.Deleted;
                db.ReportLists.Remove(ssrsreport);
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["ReportListParentUrl"] != null)
                {
                    string parentUrl = ViewData["ReportListParentUrl"].ToString();
                    ViewData["ReportListParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(ssrsreport);
    }
    
    /// <summary>Saves the properties value.</summary>
    ///
    /// <param name="id">        The identifier.</param>
    /// <param name="properties">The properties.</param>
    ///
    /// <returns>A response stream to send to the SavePropertiesValue View.</returns>
    
    public ActionResult SavePropertiesValue(long id, Dictionary<string, string> properties)
    {
        if(id > 0 && properties != null)
        {
            if(!User.CanEdit("ReportList"))
            {
                return RedirectToAction("Index", "Error");
            }
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ReportList ssrsreport = db.ReportLists.Find(id);
            db.Configuration.LazyLoadingEnabled = false;
            ReportList ssrsreport = db.ReportLists.Where(p=>p.Id == id).FirstOrDefault();
            if(ssrsreport == null)
            {
                db.DisableFilter("IsDeleted");
                ssrsreport = db.ReportLists.Find(id);
                if(ssrsreport == null)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    return HttpNotFound();
                }
            }
            // business rule before load section
            var resultBefore = ApplyBusinessRuleBefore(ssrsreport, true);
            if(resultBefore.Data != null)
            {
                var result = new { Result = "fail", data = resultBefore.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            var strChkBeforeSave = CheckBeforeSave(ssrsreport, "SaveProperty");
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                var result = new { Result = "fail", data = strChkBeforeSave };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            bool isSave = false;
            ssrsreport.setDateTimeToClientTime();
            foreach(var item in properties)
            {
                var propertyName = item.Key;
                var propertyValue = item.Value;
                var propertyInfo = ssrsreport.GetType().GetProperty(propertyName);
                if(propertyInfo != null)
                {
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object newValue = string.IsNullOrEmpty(propertyValue) ? null : propertyValue;
                    isSave = true;
                    try
                    {
                        object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                        propertyInfo.SetValue(ssrsreport, safeValue, null);
                        isSave = true;
                    }
                    catch(Exception ex)
                    {
                        var result = new { Result = "fail", data = ex.Message };
                        db.Configuration.LazyLoadingEnabled = true;
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var resultOnSaving = ApplyBusinessRuleOnSaving(ssrsreport);
            if(resultOnSaving.Data != null)
            {
                var result = new { Result = "fail", data = resultOnSaving.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            // business rule onsaving section
            if(isSave && ValidateModel(ssrsreport))
            {
                bool customsave_hasissues = false;
                if(!CustomSaveOnEdit(ssrsreport, out customsave_hasissues, "Save"))
                {
                    db.Entry(ssrsreport).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.Configuration.LazyLoadingEnabled = true;
                var result = new { Result = "Success", data = ssrsreport.DisplayValue };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach(var error in ValidateModelWithErrors(ssrsreport))
                {
                    errors += error.ErrorMessage + ".  ";
                }
                var result = new { Result = "fail", data = errors };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        db.Configuration.LazyLoadingEnabled = true;
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP GET requests) saves a property value.</summary>
    ///
    /// <param name="id">      The identifier.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">   The value.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyValue View.</returns>
    
    [HttpGet]
    public ActionResult SavePropertyValue(long id, string property, string value)
    {
        if(!User.CanEdit("ReportList"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportList ssrsreport = db.ReportLists.Find(id);
        if(ssrsreport == null)
        {
            db.DisableFilter("IsDeleted");
            ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport == null)
                return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var resultBefore = ApplyBusinessRuleBefore(ssrsreport,true);
        if(resultBefore.Data != null)
        {
            var result = new { Result = "fail", data = resultBefore.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule section
        var strChkBeforeSave = CheckBeforeSave(ssrsreport, "SaveProperty");
        if(!string.IsNullOrEmpty(strChkBeforeSave))
        {
            var result = new { Result = "fail", data = strChkBeforeSave };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var propertyInfo = ssrsreport.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            ssrsreport.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(ssrsreport, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        var resultOnSaving = ApplyBusinessRuleOnSaving(ssrsreport);
        if(resultOnSaving.Data != null)
        {
            var result = new { Result = "fail", data = resultOnSaving.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule onsaving section
        if(isSave && ValidateModel(ssrsreport))
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(ssrsreport, out customsave_hasissues, "Save"))
            {
                db.Entry(ssrsreport).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Success", data = value };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(ssrsreport))
            {
                errors += error.ErrorMessage + ".  ";
            }
            var result = new { Result = "fail", data = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        if(HostingEntity == "ReportsGroup" && AssociatedType == "ReportsGroupSSRSReportAssociation")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                ReportList obj = db.ReportLists.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.ReportsGroupSSRSReportAssociationID = HostingID;
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "ReportList", User) || !User.CanDelete("ReportList")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            ReportList ssrsreport = db.ReportLists.Find(id);
            if(ssrsreport != null)
            {
                if(CheckBeforeDelete(ssrsreport))
                {
                    if(!CustomDelete(ssrsreport, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(ssrsreport).State = EntityState.Deleted;
                        db.ReportLists.Remove(ssrsreport);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "ReportList", User) || !User.CanEdit("ReportList") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["ReportListParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesReport();
        ViewBag.ReportsInRole = new MultiSelectList(RoleList, "Key", "Value", HostingEntityID);
        string ids = string.Empty;
        if(Request.QueryString["ids"] != null)
            ids = Request.QueryString["ids"];
        ViewBag.BulkUpdate = ids;
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) bulk update.</summary>
    ///
    /// <param name="ssrsreport"> The ssrsreport.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,ReportNo,DisplayName,Description,ReportsGroupSSRSReportAssociationID,Name,ReportID,ReportPath,EntityName,IsHidden,Type")] ReportList ssrsreport, FormCollection collection, string UrlReferrer, string RoleListValue)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                ReportList target = db.ReportLists.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(ssrsreport, target, chkUpdate);
                var reportsinrole = RoleListValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var objidstring = Convert.ToString(id);
                var objreportinrole = db.ReportsInRoles.Where(r => objidstring == r.ReportId).ToList();
                foreach(var obj in objreportinrole)
                {
                    db.ReportsInRoles.Remove(obj);
                    db.SaveChanges();
                }
                if(reportsinrole != null)
                {
                    foreach(var role in reportsinrole)
                    {
                        ReportsInRole objreportinroleadd = new ReportsInRole();
                        objreportinroleadd.ReportId = Convert.ToString(id);
                        objreportinroleadd.RoleId = role;
                        db.Entry(objreportinroleadd).State = EntityState.Added;
                        db.ReportsInRoles.Add(objreportinroleadd);
                    }
                    db.SaveChanges();
                }
                customsave_hasissues = false;
                CheckBeforeSave(target,"BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.reportsgroupssrsreportassociation != null)
                            db.Entry(target.reportsgroupssrsreportassociation).State = EntityState.Unchanged;
                        db.SaveChanges();
                    }
                    catch
                    {
                        db.Entry(target).State = EntityState.Detached;
                    }
                }
                else
                {
                    db.Entry(target).State = EntityState.Detached;
                }
            }
        }
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(ssrsreport,"BulkUpdate","");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>Reports list home.</summary>
    ///
    /// <param name="RenderPartial">The render partial.</param>
    ///
    /// <returns>A response stream to send to the ReportListHome View.</returns>
    
    public ActionResult ReportListHome(bool? RenderPartial)
    {
        var lstEntityHelpPage = from s in db.EntityHelpPages
                                select s;
        var _EntityHelpPage = lstEntityHelpPage.Where(p => p.EntityName == "ReportList" && p.Disable==false).OrderBy(p => p.Order);
        var list = _EntityHelpPage.ToList();
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
            ViewBag.FavoriteItem = lstFavoriteItem;
        if(!(RenderPartial == null ? false : RenderPartial.Value))
        {
            return View(list);
        }
        else
            return PartialView(list);
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db!=null) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
