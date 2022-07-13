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
using Z.EntityFramework.Plus;
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Data Metric actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class T_DataMetricController : BaseController
{

    [Audit("ViewList")]
    /// <summary>Data Metric Index Action, renders items in different UI format based on viewtype</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="currentFilter">  Specifying the current Filter.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="sortBy">         Describes the column sort this list.</param>
    /// <param name="isAsc">          Sort order.</param>
    /// <param name="page">           List page number.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> The associated type (association name).</param>
    /// <param name="IsExport">       Is export.</param>
    /// <param name="IsDeepSearch">   Is search search.</param>
    /// <param name="IsFilter">       Specify filter.</param>
    /// <param name="RenderPartial">  Render partial.</param>
    /// <param name="BulkOperation">  Render Bulk View.</param>
    /// <param name="parent">         The parent.</param>
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype (grid, gallery, list or custom view).</param>
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     Is called at home page.</param>
    /// <param name="IsDivRender">    (Optional) True if is div render, false if not.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted.</param>
    /// <param name="ExportType">     (Optional) Type of the export.</param>
    ///
    /// <returns>A response stream to send to the Grid/Gallery/List view.</returns>
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false, string ExportType = null)
    {
        IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        CustomLoadViewDataListOnIndex(HostingEntity, HostingEntityID, AssociatedType);
        var lstT_DataMetric = from s in db.T_DataMetrics
                              select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_DataMetric = searchRecords(lstT_DataMetric, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(ViewBag.isAsc))
        {
            lstT_DataMetric = sortRecords(lstT_DataMetric, sortBy, ViewBag.isAsc);
        }
        else lstT_DataMetric = lstT_DataMetric.OrderByDescending(c => c.Id);
        lstT_DataMetric = CustomSorting(lstT_DataMetric, HostingEntity, AssociatedType, sortBy, ViewBag.isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DataMetric"].Value);
            ViewBag.Pages = pageNumber;
        }
        // for Restrict Dropdown
        ViewBag.T_DataMetricRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_DataMetric", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        ViewBag.PageSize = pageSize;
        var _T_DataMetric = lstT_DataMetric;
        _T_DataMetric = FilterByHostingEntity(_T_DataMetric, HostingEntity, AssociatedType, HostingEntityID);
        if(Convert.ToBoolean(IsExport))
        {
            if(ExportType == "csv")
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportCSV", "T_DataMetric", User) || !User.CanView("T_DataMetric"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_T_DataMetric.Count() > 0)
                    pageSize = _T_DataMetric.Count();
                var csvdata = _T_DataMetric.ToCachedPagedList(pageNumber, pageSize);
                csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
                csvdata.ToList().ForEach(fr => fr.ApplyHiddenRule(User.businessrules, "T_DataMetric"));
                return new CsvResult<T_DataMetric>(csvdata.ToList(), "Data Metric.csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] { "T_MetricIcon" });
            }
            else
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "T_DataMetric", User) || !User.CanView("T_DataMetric"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_T_DataMetric.Count() > 0)
                    pageSize = _T_DataMetric.Count();
                //return View("ExcelExport", _T_DataMetric.ToCachedPagedList(pageNumber, pageSize));
                return DownloadExcel(_T_DataMetric.ToCachedPagedList(pageNumber, pageSize).ToList());
            }
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _T_DataMetric.Count();
                if(BulkOperation != null && BulkAssociate != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                {
                    totalListCount = lstT_DataMetric.Except(_T_DataMetric).Count();
                }
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
        ViewBag.TemplatesName = GetTemplatesForList(User, "T_DataMetric", viewtype);
        TempData["T_DataMetriclist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_DataMetric.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityT_DataMetricDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_DataMetriclist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
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
                    _T_DataMetric = _fad.FilterDropdown<T_DataMetric>(User, _T_DataMetric, "T_DataMetric", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstT_DataMetric.Except(_T_DataMetric), sortBy, isAsc).ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstT_DataMetric.Except(_T_DataMetric).OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _T_DataMetric.ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _T_DataMetric.OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _T_DataMetric.Count() == 0 ? 1 : _T_DataMetric.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _T_DataMetric.ToCachedPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityT_DataMetricDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_DataMetriclist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _T_DataMetric.Count() == 0 ? 1 : _T_DataMetric.Count();
                    }
                    var list = _T_DataMetric.ToCachedPagedList(pageNumber, pageSize);
                    ViewBag.EntityT_DataMetricDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_DataMetriclist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    /// <summary>Details (ReadOnly View)</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="defaultview">      The defaultview to render.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    [Audit("View")]
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType, string defaultview)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        if(!CustomAuthorizationBeforeDetails(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        T_DataMetric t_datametric = db.T_DataMetrics.Find(id);
        if(t_datametric == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "T_DataMetric", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_datametric);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_datametric, AssociatedType);
        return View(ViewBag.TemplatesName, t_datametric);
    }
    /// <summary>Create (including details properties)</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="IsDDAdd">          The is dropdown add.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype, bool RenderPartial = false, string viewmode = "create", string wizardstep = "")
    {
        if(!User.CanAdd("T_DataMetric") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "T_DataMetric", viewtype);
        ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    /// <summary>(An Action that handles HTTP GET requests) Creates a new record (Model Popup UI, only grid properties).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   association name.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    public ActionResult CreateInline(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("T_DataMetric") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntity"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>(An Action that handles HTTP GET requests) Creates a new record (Model Popup UI, only grid properties).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   association name.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("T_DataMetric") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    
    /// <summary>(An Action that handles HTTP POST requests) Create new record through model popup).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametricObj">        The T_DataMetric object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,T_Name,T_ToolTip,T_AssociatedDataMetricTypeID,,T_AssociatedFacetedSearchID,T_GraphType,T_Aggregate,T_EntityName,T_AggregatePropertyName,T_Roles,T_BackGroundColor,T_FontColor,T_Hide,T_ClassIcon,T_DisplayOrder,T_DisplayOn")] T_DataMetric t_datametricObj, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(t_datametricObj);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_datametricObj, out customcreate_hasissues, "Create"))
            {
                db.T_DataMetrics.Add(t_datametricObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
                return Json(new { result = "FROMPOPUP", output = t_datametricObj.Id }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(t_datametricObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_datametricObj, AssociatedEntity);
        return View(t_datametricObj);
    }
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametricObj">The T_DataMetric object.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="IsDDAdd">          Add from dropdown.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,T_Name,T_ToolTip,T_AssociatedDataMetricTypeID,,T_AssociatedFacetedSearchID,T_GraphType,T_Aggregate,T_EntityName,T_AggregatePropertyName,T_Roles,T_BackGroundColor,T_FontColor,T_Hide,T_ClassIcon,T_DisplayOrder,T_DisplayOn")] T_DataMetric t_datametricObj, HttpPostedFileBase T_BackgroundImage, String CamerafileUploadT_BackgroundImage, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_datametricObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(T_BackgroundImage != null || (Request.Form["CamerafileUploadT_BackgroundImage"] != null && Request.Form["CamerafileUploadT_BackgroundImage"] != ""))
            IsFileTypeAndSizeAllowed(T_BackgroundImage);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(T_BackgroundImage != null)
            {
                long documentID = SaveDocument(T_BackgroundImage);
                t_datametricObj.T_BackgroundImage = documentID;
            }
            if(Request.Form["CamerafileUploadT_BackgroundImage"] != null && Request.Form["CamerafileUploadT_BackgroundImage"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_BackgroundImage"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_BackgroundImage"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_datametricObj.T_BackgroundImage = documentIDCamara;
                }
            }
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_datametricObj, out customcreate_hasissues, command))
            {
                db.T_DataMetrics.Add(t_datametricObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_datametricObj, "Create", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode == "wizard")
                    return RedirectToAction("Edit", new { Id = t_datametricObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_datametricObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_datametricObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                }
                if(command == "Create & Add another")
                    return Redirect(Convert.ToString(Request.UrlReferrer.PathAndQuery));
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(t_datametricObj);
        ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_datametricObj);
    }
    /// <summary>(An Action that handles HTTP GET requests) Renders UI to edit a record on model popup.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [Audit("View")]
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype, bool RecordReadOnly = false)
    {
        if(!User.CanEdit("T_DataMetric") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_DataMetric t_datametric = db.T_DataMetrics.Find(id);
        if(t_datametric == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        if(ViewData["T_DataMetricParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric/Edit/" + t_datametric.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric/Create"))
            ViewData["T_DataMetricParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(t_datametric);
        var objT_DataMetric = new List<T_DataMetric>();
        ViewBag.T_DataMetricDD = new SelectList(objT_DataMetric, "ID", "DisplayValue");
        return View(t_datametric);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametric">       The T_DataMetric object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_ToolTip,T_AssociatedDataMetricTypeID,,T_AssociatedFacetedSearchID,T_GraphType,T_Aggregate,T_EntityName,T_AggregatePropertyName,T_Roles,T_BackGroundColor,T_FontColor,T_Hide,T_ClassIcon,T_DisplayOrder,T_DisplayOn")] T_DataMetric t_datametric, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_datametric = UpdateHiddenProperties(t_datametric, cannotviewproperties);
        CheckBeforeSave(t_datametric, command);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_datametric, out customsave_hasissues, command))
            {
                db.Entry(t_datametric).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Succeed", UrlRefr = UrlReferrer };
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
        if(db.Entry(t_datametric).State == EntityState.Detached)
            t_datametric = db.T_DataMetrics.Find(t_datametric.Id);
        LoadViewDataAfterOnEdit(t_datametric);
        return View(t_datametric);
    }
    /// <summary>(An Action that handles HTTP GET requests) Renders UI to edit a record (full screen).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [Audit("View")]
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RecordReadOnly = false, bool RenderPartial = false, string viewmode = "edit", string wizardstep = "")
    {
        if(!User.CanEdit("T_DataMetric") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_DataMetric t_datametric = db.T_DataMetrics.Find(id);
        if(t_datametric == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_DataMetriclist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_DataMetrics.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_DataMetricDisplayValueEdit = TempData["T_DataMetriclist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_DataMetriclist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_DataMetricDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "T_DataMetric", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_datametric.DisplayValue, Value = t_datametric.Id.ToString() }));
            ViewBag.EntityT_DataMetricDisplayValueEdit = newList;
            TempData["T_DataMetriclist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_datametric.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_datametric.DisplayValue;
                newList[0].Value = t_datametric.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_datametric.DisplayValue, Value = t_datametric.Id.ToString() }));
            }
            ViewBag.EntityT_DataMetricDisplayValueEdit = newList;
            TempData["T_DataMetriclist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        if(ViewData["T_DataMetricParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric/Edit/" + t_datametric.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric/Create"))
            ViewData["T_DataMetricParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step" + wizardstep;
        LoadViewDataBeforeOnEdit(t_datametric);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, t_datametric);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametric">    The T_DataMetric object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_ToolTip,T_AssociatedDataMetricTypeID,,T_AssociatedFacetedSearchID,T_GraphType,T_Aggregate,T_EntityName,T_AggregatePropertyName,T_Roles,T_BackGroundColor,T_FontColor,T_Hide,T_ClassIcon,T_DisplayOrder,T_DisplayOn,T_BackgroundImage")] T_DataMetric t_datametric, HttpPostedFileBase File_T_BackgroundImage, String CamerafileUploadT_BackgroundImage, string UrlReferrer, bool RenderPartial = false, string viewmode = "edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_datametric = UpdateHiddenProperties(t_datametric, cannotviewproperties);
        CheckBeforeSave(t_datametric, command);
        if(File_T_BackgroundImage != null || (Request.Form["CamerafileUploadT_BackgroundImage"] != null && Request.Form["CamerafileUploadT_BackgroundImage"] != ""))
            IsFileTypeAndSizeAllowed(File_T_BackgroundImage);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_T_BackgroundImage != null)
            {
                long documentID = 0;
                if(t_datametric.T_BackgroundImage != null)
                    documentID = UpdateDocument(File_T_BackgroundImage, t_datametric.T_BackgroundImage);
                else
                    documentID = SaveDocument(File_T_BackgroundImage);
                t_datametric.T_BackgroundImage = documentID;
            }
            if(Request.Form["CamerafileUploadT_BackgroundImage"] != null && Request.Form["CamerafileUploadT_BackgroundImage"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_BackgroundImage"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_BackgroundImage"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(t_datametric.T_BackgroundImage != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, t_datametric.T_BackgroundImage);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_datametric.T_BackgroundImage = documentIDCamara;
                }
            }
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_datametric, out customsave_hasissues, command))
            {
                db.Entry(t_datametric).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_datametric, "Edit", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = t_datametric.Id };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                if(command != "Save")
                {
                    if(command == "SaveNextPrev")
                    {
                        long NextPreId = Convert.ToInt64(Request.Form["hdnNextPrevId"]);
                        return RedirectToAction("Edit", new { Id = NextPreId, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_datametric.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        if(TempData["T_DataMetriclist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_DataMetrics.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_DataMetricDisplayValueEdit = TempData["T_DataMetriclist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_DataMetriclist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_DataMetricDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_datametric).State == EntityState.Detached)
            t_datametric = db.T_DataMetrics.Find(t_datametric.Id);
        LoadViewDataAfterOnEdit(t_datametric);
        ViewData["T_DataMetricParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_datametric);
    }
    
    /// <summary>Deletes the record for given ID.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <exception cref="Exception">Thrown when an exception error when id not found.</exception>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("T_DataMetric") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_DataMetric t_datametric = db.T_DataMetrics.Find(id);
        if(t_datametric == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_DataMetricParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DataMetric"))
            ViewData["T_DataMetricParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_datametric);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametric">  The T_DataMetric object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_DataMetric t_datametric, string UrlReferrer)
    {
        if(!User.CanDelete("T_DataMetric"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_datametric = db.T_DataMetrics.Find(t_datametric.Id);
        if(CheckBeforeDelete(t_datametric))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_datametric, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_datametric).State = EntityState.Deleted;
                //db.T_DataMetrics.Remove(t_datametric); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_DataMetricParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_DataMetricParentUrl"].ToString();
                    ViewData["T_DataMetricParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_datametric);
    }
    
    /// <summary>Deletes the document described by docID.</summary>
    ///
    /// <param name="docID">Identifier for the document.</param>
    public ActionResult DocumentDeassociate(long? docid)
    {
        if(!User.CanDelete("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        var document = db.Documents.Find(docid);
        db.Entry(document).State = EntityState.Deleted;
        db.Documents.Remove(document);
        db.SaveChanges();
        return Json("Success", JsonRequestBehavior.AllowGet);
    }
    /// <summary>Bulk associate (Action associate multiple selected items with hosted entity).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="ids">            The identifiers.</param>
    /// <param name="AssociatedType"> Type of the associated entity.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the BulkAssociate View.</returns>
    public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
    {
        var HostingID = Convert.ToInt64(HostingEntityID);
        if(HostingID == 0)
            return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        if(HostingEntity == "T_DataMetrictype" && AssociatedType == "T_AssociatedDataMetricType")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_DataMetric obj = db.T_DataMetrics.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_AssociatedDataMetricTypeID = HostingID;
                db.SaveChanges();
            }
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [Audit("Restore")]
    /// <summary>Deletes selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="ids">        The identifiers.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteBulk View.</returns>
    public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
    {
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_DataMetric", User) || !User.CanDelete("T_DataMetric")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_DataMetric t_datametric = db.T_DataMetrics.Find(id);
            if(t_datametric != null)
            {
                if(CheckBeforeDelete(t_datametric))
                {
                    if(!CustomDelete(t_datametric, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_datametric).State = EntityState.Deleted;
                        db.T_DataMetrics.Remove(t_datametric);
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
    /// <summary>Cancels the action and return back to previous page.</summary>
    ///
    /// <remarks></remarks>
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
    /// <summary>(An Action that handles HTTP POST requests) Renders UI to update records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDUpdate">       Dropdown update.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpGet]
    public ActionResult BulkUpdate(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
    {
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_DataMetric", User) || !User.CanEdit("T_DataMetric") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_DataMetric", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametric">  The T_DataMetric object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_ToolTip,T_AssociatedDataMetricTypeID,T_GraphType,T_Aggregate,T_EntityName,T_AggregatePropertyName,T_Roles,T_BackGroundColor,T_FontColor,T_Hide,T_ClassIcon,T_DisplayOrder,T_DisplayOn")] T_DataMetric t_datametric, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_DataMetric target = db.T_DataMetrics.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_datametric, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.t_associateddatametrictype != null)
                            db.Entry(target.t_associateddatametrictype).State = EntityState.Unchanged;
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_datametric, "BulkUpdate", "");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    public ActionResult DeleteImageGalleryDocumentAndUpdate(string ID, long recId, string PropName, string IDs)
    {
        var outputres = "";
        byte[] ConcurrencyKeyvalue = null;
        if(!String.IsNullOrEmpty(ID))
        {
            var document = db.Documents.Find(Convert.ToInt64(ID));
            db.Entry(document).State = EntityState.Deleted;
            db.Documents.Remove(document);
            db.SaveChanges();
            var output = string.Join(",", from v in IDs.Split(',')
                                     where v.Trim() != ID
                                     select v);
            outputres = output;
            using(var ctx = new ApplicationContext(new SystemUser()))
            {
                T_DataMetric t_datametric = ctx.T_DataMetrics.Find(Convert.ToInt64(recId));
                ctx.Entry(t_datametric).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue = ctx.T_DataMetrics.Find(Convert.ToInt64(recId)).ConcurrencyKey;
            }
        }
        return Json(new { result = "POP", output = outputres, ConcurrencyKey = ConcurrencyKeyvalue }, JsonRequestBehavior.AllowGet);
    }
    
    public ActionResult DeleteImageGalleryDocumentAndUpdateAll(long recId, string IDs, string PropName)
    {
        var outputres = "";
        byte[] ConcurrencyKeyvalue = null;
        DeleteImageGalleryDocument(IDs);
        using(var ctx = new ApplicationContext(new SystemUser()))
        {
            T_DataMetric t_datametric = ctx.T_DataMetrics.Find(Convert.ToInt64(recId));
            ctx.Entry(t_datametric).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue = ctx.T_DataMetrics.Find(Convert.ToInt64(recId)).ConcurrencyKey;
        }
        return Json(new { result = "POP", ConcurrencyKey = ConcurrencyKeyvalue }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null && CacheHelper.NoCache("T_DataMetric")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
