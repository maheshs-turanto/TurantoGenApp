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
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Document Template actions (CRUD and others).</summary>
///
/// <remarks></remarks>
[LocalDateTimeConverter]
public partial class T_DocumentTemplateController : BaseController
{

    [Audit("ViewList")]
    /// <summary>Document Template Index Action, renders items in different UI format based on viewtype</summary>
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
        var lstT_DocumentTemplate = from s in db.T_DocumentTemplates
                                    select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_DocumentTemplate = searchRecords(lstT_DocumentTemplate, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(ViewBag.isAsc))
        {
            lstT_DocumentTemplate = sortRecords(lstT_DocumentTemplate, sortBy, ViewBag.isAsc);
        }
        else lstT_DocumentTemplate = lstT_DocumentTemplate.OrderByDescending(c => c.Id);
        lstT_DocumentTemplate = CustomSorting(lstT_DocumentTemplate, HostingEntity, AssociatedType, sortBy, ViewBag.isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_DocumentTemplate"].Value);
            ViewBag.Pages = pageNumber;
        }
        // for Restrict Dropdown
        ViewBag.T_DocumentTemplateRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_DocumentTemplate", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        ViewBag.PageSize = pageSize;
        var _T_DocumentTemplate = lstT_DocumentTemplate;
        _T_DocumentTemplate = FilterByHostingEntity(_T_DocumentTemplate, HostingEntity, AssociatedType, HostingEntityID);
        if(Convert.ToBoolean(IsExport))
        {
            if(ExportType == "csv")
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportCSV", "T_DocumentTemplate", User) || !User.CanView("T_DocumentTemplate"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_T_DocumentTemplate.Count() > 0)
                    pageSize = _T_DocumentTemplate.Count();
                var csvdata = _T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize);
                csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
                csvdata.ToList().ForEach(fr => fr.ApplyHiddenRule(User.businessrules, "T_DocumentTemplate"));
                return new CsvResult<T_DocumentTemplate>(csvdata.ToList(), "Document Template.csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] { "T_Document" });
            }
            else
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "T_DocumentTemplate", User) || !User.CanView("T_DocumentTemplate"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_T_DocumentTemplate.Count() > 0)
                    pageSize = _T_DocumentTemplate.Count();
                //return View("ExcelExport", _T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize));
                return DownloadExcel(_T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize).ToList());
            }
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _T_DocumentTemplate.Count();
                if(BulkOperation != null && BulkAssociate != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                {
                    totalListCount = lstT_DocumentTemplate.Except(_T_DocumentTemplate).Count();
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
        ViewBag.TemplatesName = GetTemplatesForList(User, "T_DocumentTemplate", viewtype);
        TempData["T_DocumentTemplatelist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityT_DocumentTemplateDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_DocumentTemplatelist"] = list.Select(z => new
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
                    _T_DocumentTemplate = _fad.FilterDropdown<T_DocumentTemplate>(User, _T_DocumentTemplate, "T_DocumentTemplate", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstT_DocumentTemplate.Except(_T_DocumentTemplate), sortBy, isAsc).ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstT_DocumentTemplate.Except(_T_DocumentTemplate).OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _T_DocumentTemplate.OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _T_DocumentTemplate.Count() == 0 ? 1 : _T_DocumentTemplate.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _T_DocumentTemplate.ToCachedPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityT_DocumentTemplateDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_DocumentTemplatelist"] = list.Select(z => new
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
                        pageSize = _T_DocumentTemplate.Count() == 0 ? 1 : _T_DocumentTemplate.Count();
                    }
                    var list = _T_DocumentTemplate.ToCachedPagedList(pageNumber, pageSize);
                    ViewBag.EntityT_DocumentTemplateDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_DocumentTemplatelist"] = list.Select(z => new
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
        T_DocumentTemplate t_documenttemplate = db.T_DocumentTemplates.Find(id);
        if(t_documenttemplate == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "T_DocumentTemplate", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_documenttemplate);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_documenttemplate, AssociatedType);
        return View(ViewBag.TemplatesName, t_documenttemplate);
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
        if(!User.CanAdd("T_DocumentTemplate") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "T_DocumentTemplate", viewtype);
        ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_DocumentTemplate") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntity"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_documenttemplateObj">The T_DocumentTemplate object.</param>
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
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,T_AutoNo,T_EntityName,T_Name,T_Description,T_DocumentType,T_ActionType,T_DefaultOutputFormat,T_AllowedRoles,T_AttachDocumentTo,T_DisplayType,T_DisplayOrder,T_ToolTip,T_BackGroundColor,T_FontColor,T_Disable,T_EnableDownload,T_EnablePreview,T_RecordAdded,T_RecordAddedUser,T_Tenants")] T_DocumentTemplate t_documenttemplateObj, HttpPostedFileBase T_Document, String CamerafileUploadT_Document, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, string T_Tenants, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_documenttemplateObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(T_Document != null || (Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != ""))
            IsFileTypeAndSizeAllowed(T_Document);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(T_Document != null)
            {
                long documentID = SaveDocument(T_Document);
                t_documenttemplateObj.T_Document = documentID;
            }
            if(Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_documenttemplateObj.T_Document = documentIDCamara;
                }
            }
            //t_documenttemplateObj.T_RecordAdded = DateTime.UtcNow;
            //t_documenttemplateObj.T_RecordAddedUser = User.Name;
            //t_documenttemplateObj.T_RecordAddedInsertDate = DateTime.UtcNow;
            //t_documenttemplateObj.T_RecordAddedInsertBy = User.Name;
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_documenttemplateObj, out customcreate_hasissues, command))
            {
                db.T_DocumentTemplates.Add(t_documenttemplateObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_documenttemplateObj, "Create", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode == "wizard")
                    return RedirectToAction("Edit", new { Id = t_documenttemplateObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_documenttemplateObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_documenttemplateObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        LoadViewDataAfterOnCreate(t_documenttemplateObj);
        ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_documenttemplateObj);
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
        if(!User.CanEdit("T_DocumentTemplate") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_DocumentTemplate t_documenttemplate = db.T_DocumentTemplates.Find(id);
        if(t_documenttemplate == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
        if(ViewData["T_DocumentTemplateParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate/Edit/" + t_documenttemplate.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate/Create"))
            ViewData["T_DocumentTemplateParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(t_documenttemplate);
        var objT_DocumentTemplate = new List<T_DocumentTemplate>();
        ViewBag.T_DocumentTemplateDD = new SelectList(objT_DocumentTemplate, "ID", "DisplayValue");
        return View(t_documenttemplate);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_documenttemplate">       The T_DocumentTemplate object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_EntityName,T_Name,T_Description,T_Document,T_DocumentType,T_ActionType,T_DefaultOutputFormat,T_AllowedRoles,T_AttachDocumentTo,T_DisplayType,T_DisplayOrder,T_ToolTip,T_BackGroundColor,T_FontColor,T_Disable,T_EnableDownload,T_EnablePreview,T_RecordAdded,T_RecordAddedUser,T_Tenants")] T_DocumentTemplate t_documenttemplate, HttpPostedFileBase File_T_Document, String CamerafileUploadT_Document, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_documenttemplate = UpdateHiddenProperties(t_documenttemplate, cannotviewproperties);
        CheckBeforeSave(t_documenttemplate, command);
        if(File_T_Document != null || (Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != ""))
            IsFileTypeAndSizeAllowed(File_T_Document);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_T_Document != null)
            {
                long documentID = 0;
                if(t_documenttemplate.T_Document != null)
                    documentID = UpdateDocument(File_T_Document, t_documenttemplate.T_Document);
                else
                    documentID = SaveDocument(File_T_Document);
                t_documenttemplate.T_Document = documentID;
            }
            if(Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(t_documenttemplate.T_Document != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, t_documenttemplate.T_Document);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_documenttemplate.T_Document = documentIDCamara;
                }
            }
            //t_documenttemplate.T_RecordAdded = DateTime.UtcNow;
            //t_documenttemplate.T_RecordAddedUser = User.Name;
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_documenttemplate, out customsave_hasissues, command))
            {
                db.Entry(t_documenttemplate).State = EntityState.Modified;
                //db.Entry(t_documenttemplate).Property(x => x.T_RecordAddedInsertDate).IsModified = false;
                //db.Entry(t_documenttemplate).Property(x => x.T_RecordAddedInsertBy).IsModified = false;
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
        if(db.Entry(t_documenttemplate).State == EntityState.Detached)
            t_documenttemplate = db.T_DocumentTemplates.Find(t_documenttemplate.Id);
        LoadViewDataAfterOnEdit(t_documenttemplate);
        return View(t_documenttemplate);
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
        if(!User.CanEdit("T_DocumentTemplate") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_DocumentTemplate t_documenttemplate = db.T_DocumentTemplates.Find(id);
        if(t_documenttemplate == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_DocumentTemplatelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_DocumentTemplates.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_DocumentTemplateDisplayValueEdit = TempData["T_DocumentTemplatelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_DocumentTemplatelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_DocumentTemplateDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "T_DocumentTemplate", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_documenttemplate.DisplayValue, Value = t_documenttemplate.Id.ToString() }));
            ViewBag.EntityT_DocumentTemplateDisplayValueEdit = newList;
            TempData["T_DocumentTemplatelist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_documenttemplate.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_documenttemplate.DisplayValue;
                newList[0].Value = t_documenttemplate.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_documenttemplate.DisplayValue, Value = t_documenttemplate.Id.ToString() }));
            }
            ViewBag.EntityT_DocumentTemplateDisplayValueEdit = newList;
            TempData["T_DocumentTemplatelist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
        if(ViewData["T_DocumentTemplateParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate/Edit/" + t_documenttemplate.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate/Create"))
            ViewData["T_DocumentTemplateParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step" + wizardstep;
        LoadViewDataBeforeOnEdit(t_documenttemplate);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, t_documenttemplate);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_documenttemplate">    The T_DocumentTemplate object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_EntityName,T_Name,T_Description,T_Document,T_DocumentType,T_ActionType,T_DefaultOutputFormat,T_AllowedRoles,T_AttachDocumentTo,T_DisplayType,T_DisplayOrder,T_ToolTip,T_BackGroundColor,T_FontColor,T_Disable,T_EnableDownload,T_EnablePreview,T_RecordAdded,T_RecordAddedUser,T_Tenants")] T_DocumentTemplate t_documenttemplate, HttpPostedFileBase File_T_Document, String CamerafileUploadT_Document, string UrlReferrer, string OriginalTenants, bool RenderPartial = false, string viewmode = "edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_documenttemplate = UpdateHiddenProperties(t_documenttemplate, cannotviewproperties);
        CheckBeforeSave(t_documenttemplate, command);
        if(File_T_Document != null || (Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != ""))
            IsFileTypeAndSizeAllowed(File_T_Document);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_T_Document != null)
            {
                long documentID = 0;
                if(t_documenttemplate.T_Document != null)
                    documentID = UpdateDocument(File_T_Document, t_documenttemplate.T_Document);
                else
                    documentID = SaveDocument(File_T_Document);
                t_documenttemplate.T_Document = documentID;
            }
            var listtenant = CommonFunction.Instance.getTenantList(((CustomPrincipal)User));
            if(listtenant != null)
            {
                if(!string.IsNullOrEmpty(t_documenttemplate.T_Tenants) && !string.IsNullOrEmpty(OriginalTenants))
                {
                    var tenantList = t_documenttemplate.T_Tenants.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var OriginalTenantsList = OriginalTenants.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var list = listtenant.Select(p => p.Key);
                    var newtenantlist = new List<string>();
                    foreach(var item in OriginalTenantsList)
                    {
                        if(!list.Contains(item))
                            newtenantlist.Add(item);
                    }
                    if(tenantList.Contains("-1"))
                    {
                        foreach(var tenant in list)
                        {
                            newtenantlist.Add(tenant);
                        }
                    }
                    else
                        foreach(var tenant in tenantList)
                        {
                            newtenantlist.Add(tenant);
                        }
                    t_documenttemplate.T_Tenants = string.Join(",", newtenantlist);
                }
            }
            if(Request.Form["CamerafileUploadT_Document"] != null && Request.Form["CamerafileUploadT_Document"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Document"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(t_documenttemplate.T_Document != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, t_documenttemplate.T_Document);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_documenttemplate.T_Document = documentIDCamara;
                }
            }
            //t_documenttemplate.T_RecordAdded = DateTime.UtcNow;
            //t_documenttemplate.T_RecordAddedUser = User.Name;
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_documenttemplate, out customsave_hasissues, command))
            {
                db.Entry(t_documenttemplate).State = EntityState.Modified;
                //db.Entry(t_documenttemplate).Property(x => x.T_RecordAddedInsertDate).IsModified = false;
                //db.Entry(t_documenttemplate).Property(x => x.T_RecordAddedInsertBy).IsModified = false;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_documenttemplate, "Edit", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = t_documenttemplate.Id };
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
                        return RedirectToAction("Edit", new { Id = t_documenttemplate.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        if(TempData["T_DocumentTemplatelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_DocumentTemplates.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_DocumentTemplateDisplayValueEdit = TempData["T_DocumentTemplatelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_DocumentTemplatelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_DocumentTemplateDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_documenttemplate).State == EntityState.Detached)
            t_documenttemplate = db.T_DocumentTemplates.Find(t_documenttemplate.Id);
        LoadViewDataAfterOnEdit(t_documenttemplate);
        ViewData["T_DocumentTemplateParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_documenttemplate);
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
        if(!User.CanDelete("T_DocumentTemplate") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_DocumentTemplate t_documenttemplate = db.T_DocumentTemplates.Find(id);
        if(t_documenttemplate == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_DocumentTemplateParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_DocumentTemplate"))
            ViewData["T_DocumentTemplateParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_documenttemplate);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_documenttemplate">  The T_DocumentTemplate object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_DocumentTemplate t_documenttemplate, string UrlReferrer)
    {
        if(!User.CanDelete("T_DocumentTemplate"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_documenttemplate = db.T_DocumentTemplates.Find(t_documenttemplate.Id);
        if(CheckBeforeDelete(t_documenttemplate))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_documenttemplate, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_documenttemplate).State = EntityState.Deleted;
                //db.T_DocumentTemplates.Remove(t_documenttemplate); //issue delete with userbased security
                if(t_documenttemplate.T_Document != null)
                {
                    DeleteDocument(t_documenttemplate.T_Document);
                }
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_DocumentTemplateParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_DocumentTemplateParentUrl"].ToString();
                    ViewData["T_DocumentTemplateParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_documenttemplate);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_DocumentTemplate", User) || !User.CanDelete("T_DocumentTemplate")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_DocumentTemplate t_documenttemplate = db.T_DocumentTemplates.Find(id);
            if(t_documenttemplate != null)
            {
                if(CheckBeforeDelete(t_documenttemplate))
                {
                    if(!CustomDelete(t_documenttemplate, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_documenttemplate).State = EntityState.Deleted;
                        db.T_DocumentTemplates.Remove(t_documenttemplate);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_DocumentTemplate", User) || !User.CanEdit("T_DocumentTemplate") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_DocumentTemplate", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_documenttemplate">  The T_DocumentTemplate object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_EntityName,T_Name,T_Description,T_Document,T_DocumentType,T_ActionType,T_DefaultOutputFormat,T_AllowedRoles,T_AttachDocumentTo,T_DisplayType,T_DisplayOrder,T_ToolTip,T_BackGroundColor,T_FontColor,T_Disable,T_EnableDownload,T_EnablePreview,T_RecordAdded,T_RecordAddedUser,T_Tenants")] T_DocumentTemplate t_documenttemplate, HttpPostedFileBase File_T_Document, String CamerafileUploadT_Document, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_DocumentTemplate target = db.T_DocumentTemplates.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_documenttemplate, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_documenttemplate, "BulkUpdate", "");
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
                T_DocumentTemplate t_documenttemplate = ctx.T_DocumentTemplates.Find(Convert.ToInt64(recId));
                if(PropName == "T_Document")
                    t_documenttemplate.T_Document = null;
                ctx.Entry(t_documenttemplate).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue = ctx.T_DocumentTemplates.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            T_DocumentTemplate t_documenttemplate = ctx.T_DocumentTemplates.Find(Convert.ToInt64(recId));
            ctx.Entry(t_documenttemplate).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue = ctx.T_DocumentTemplates.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            if(db != null && CacheHelper.NoCache("T_DocumentTemplate")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
