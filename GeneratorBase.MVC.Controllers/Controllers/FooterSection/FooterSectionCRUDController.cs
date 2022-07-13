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
/// <summary>A partial controller class for Footer Section  actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class FooterSectionController : BaseController
{

    /// <summary>Footer Section  Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype,string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false, string ExportType = null)
    {
        IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        CustomLoadViewDataListOnIndex(HostingEntity, HostingEntityID, AssociatedType);
        var lstFooterSection = from s in db.FooterSections
                               select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstFooterSection = searchRecords(lstFooterSection, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(ViewBag.isAsc))
        {
            lstFooterSection = sortRecords(lstFooterSection, sortBy, ViewBag.isAsc);
        }
        else lstFooterSection = lstFooterSection.OrderBy(c => c.Id);
        lstFooterSection = CustomSorting(lstFooterSection,HostingEntity,AssociatedType,sortBy,ViewBag.isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FooterSection"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "FooterSection"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FooterSection"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FooterSection"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FooterSection"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FooterSection"].Value);
            ViewBag.Pages = pageNumber;
        }
        // for Restrict Dropdown
        ViewBag.FooterSectionRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "FooterSection", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        ViewBag.PageSize = pageSize;
        var _FooterSection = lstFooterSection;
        _FooterSection = FilterByHostingEntity(_FooterSection, HostingEntity, AssociatedType, HostingEntityID);
        if(Convert.ToBoolean(IsExport))
        {
            if(ExportType == "csv")
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportCSV", "FooterSection", User) || !User.CanView("FooterSection"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_FooterSection.Count() > 0)
                    pageSize = _FooterSection.Count();
                var csvdata = _FooterSection.ToCachedPagedList(pageNumber, pageSize);
                csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
                csvdata.ToList().ForEach(fr => fr.ApplyHiddenRule(User.businessrules, "FooterSection"));
                return new CsvResult<FooterSection>(csvdata.ToList(), "Footer Section .csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] { "DocumentUpload" });
            }
            else
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "FooterSection", User) || !User.CanView("FooterSection"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_FooterSection.Count() > 0)
                    pageSize = _FooterSection.Count();
                //return View("ExcelExport", _FooterSection.ToCachedPagedList(pageNumber, pageSize));
                return DownloadExcel(_FooterSection.ToCachedPagedList(pageNumber, pageSize).ToList());
            }
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _FooterSection.Count();
                if(BulkOperation != null && BulkAssociate != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                {
                    totalListCount = lstFooterSection.Except(_FooterSection).Count();
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
        ViewBag.TemplatesName = GetTemplatesForList(User,"FooterSection",viewtype);
        TempData["FooterSectionlist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _FooterSection.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityFooterSectionDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["FooterSectionlist"] = list.Select(z => new
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
                    _FooterSection = _fad.FilterDropdown<FooterSection>(User,  _FooterSection, "FooterSection", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation",sortRecords(lstFooterSection.Except(_FooterSection),sortBy,isAsc).ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstFooterSection.Except(_FooterSection).OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _FooterSection.ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _FooterSection.OrderBy(q=>q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _FooterSection.Count() == 0 ? 1 : _FooterSection.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _FooterSection.ToCachedPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityFooterSectionDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["FooterSectionlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _FooterSection.Count() == 0 ? 1 : _FooterSection.Count();
                    }
                    var list = _FooterSection.ToCachedPagedList(pageNumber, pageSize);
                    ViewBag.EntityFooterSectionDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["FooterSectionlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
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
        FooterSection footersection = db.FooterSections.Find(id);
        if(footersection == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User,"FooterSection",defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(footersection);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(footersection, AssociatedType);
        return View(ViewBag.TemplatesName,footersection);
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
    public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype,bool RenderPartial = false, string viewmode="create", string wizardstep="")
    {
        if(!User.CanAdd("FooterSection") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User,"FooterSection",viewtype);
        ViewData["FooterSectionParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("FooterSection") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["FooterSectionParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("FooterSection") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["FooterSectionParentUrl"] = UrlReferrer;
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
    /// <param name="footersectionObj">        The FooterSection object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,CompanyInformationFooterSectionAssociationID,Name,AssociatedFooterSectionTypeID,WebLinkTitle,WebLink")] FooterSection footersectionObj, HttpPostedFileBase DocumentUpload, String CamerafileUploadDocumentUpload,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(footersectionObj);
        if(DocumentUpload != null || (Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"] != ""))
            IsFileTypeAndSizeAllowed(DocumentUpload);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks =  DateTime.UtcNow.Ticks.ToString();
            if(DocumentUpload != null)
            {
                long documentID = SaveDocument(DocumentUpload);
                footersectionObj.DocumentUpload = documentID;
            }
            if(Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"]  != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    footersectionObj.DocumentUpload = documentIDCamara;
                }
            }
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(footersectionObj,out customcreate_hasissues,"Create"))
            {
                footersectionObj.TenantId = footersectionObj.CompanyInformationFooterSectionAssociationID;
                db.FooterSections.Add(footersectionObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
                return Json(new {result = "FROMPOPUP", output = footersectionObj.Id}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(footersectionObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(footersectionObj, AssociatedEntity);
        return View(footersectionObj);
    }
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="footersectionObj">The FooterSection object.</param>
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
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,CompanyInformationFooterSectionAssociationID,Name,AssociatedFooterSectionTypeID,WebLinkTitle,WebLink")] FooterSection footersectionObj, HttpPostedFileBase DocumentUpload, String CamerafileUploadDocumentUpload, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(footersectionObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(DocumentUpload != null || (Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"] != ""))
            IsFileTypeAndSizeAllowed(DocumentUpload);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks =  DateTime.UtcNow.Ticks.ToString();
            if(DocumentUpload != null)
            {
                long documentID = SaveDocument(DocumentUpload);
                footersectionObj.DocumentUpload = documentID;
            }
            if(Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"]  != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    footersectionObj.DocumentUpload = documentIDCamara;
                }
            }
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(footersectionObj,out customcreate_hasissues,command))
            {
                db.FooterSections.Add(footersectionObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(footersectionObj,"Create",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode=="wizard")
                    return RedirectToAction("Edit", new { Id = footersectionObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = footersectionObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = footersectionObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
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
        LoadViewDataAfterOnCreate(footersectionObj);
        ViewData["FooterSectionParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(footersectionObj);
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
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview,bool RecordReadOnly= false, bool RenderPartial = false, string viewmode="edit", string wizardstep="")
    {
        if(!User.CanEdit("FooterSection") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        FooterSection footersection = db.FooterSections.Find(id);
        if(footersection == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["FooterSectionlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.FooterSections.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityFooterSectionDisplayValueEdit = TempData["FooterSectionlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["FooterSectionlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityFooterSectionDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User,"FooterSection",defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/FooterSection/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = footersection.DisplayValue, Value = footersection.Id.ToString() }));
            ViewBag.EntityFooterSectionDisplayValueEdit = newList;
            TempData["FooterSectionlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(footersection.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = footersection.DisplayValue;
                newList[0].Value = footersection.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = footersection.DisplayValue, Value = footersection.Id.ToString() }));
            }
            ViewBag.EntityFooterSectionDisplayValueEdit = newList;
            TempData["FooterSectionlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["FooterSectionParentUrl"] = UrlReferrer;
        if(ViewData["FooterSectionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FooterSection")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FooterSection/Edit/" + footersection.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FooterSection/Create"))
            ViewData["FooterSectionParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step"+wizardstep;
        LoadViewDataBeforeOnEdit(footersection);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,footersection);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="footersection">    The FooterSection object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,CompanyInformationFooterSectionAssociationID,Name,AssociatedFooterSectionTypeID,WebLinkTitle,WebLink,DocumentUpload")] FooterSection footersection, HttpPostedFileBase File_DocumentUpload, String CamerafileUploadDocumentUpload,  string UrlReferrer, bool RenderPartial = false, string viewmode="edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) footersection = UpdateHiddenProperties(footersection, cannotviewproperties);
        CheckBeforeSave(footersection, command);
        if(File_DocumentUpload != null || (Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"] != ""))
            IsFileTypeAndSizeAllowed(File_DocumentUpload);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks =  DateTime.UtcNow.Ticks.ToString();
            if(File_DocumentUpload != null)
            {
                long documentID = 0;
                if(footersection.DocumentUpload != null)
                    documentID = UpdateDocument(File_DocumentUpload, footersection.DocumentUpload);
                else
                    documentID = SaveDocument(File_DocumentUpload);
                footersection.DocumentUpload = documentID;
            }
            if(Request.Form["CamerafileUploadDocumentUpload"] != null && Request.Form["CamerafileUploadDocumentUpload"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadDocumentUpload"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(footersection.DocumentUpload != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, footersection.DocumentUpload);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    footersection.DocumentUpload = documentIDCamara;
                }
            }
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(footersection,out customsave_hasissues,command))
            {
                footersection.TenantId = footersection.CompanyInformationFooterSectionAssociationID;
                db.Entry(footersection).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(footersection,"Edit",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = footersection.Id };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                if(command != "Save")
                {
                    if(command == "SaveNextPrev")
                    {
                        long NextPreId = Convert.ToInt64(Request.Form["hdnNextPrevId"]);
                        return RedirectToAction("Edit", new { Id = NextPreId, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                    }
                    else
                        return RedirectToAction("Edit", new { Id = footersection.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep  });
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
        if(TempData["FooterSectionlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.FooterSections.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityFooterSectionDisplayValueEdit = TempData["FooterSectionlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["FooterSectionlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityFooterSectionDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(footersection).State == EntityState.Detached)
            footersection = db.FooterSections.Find(footersection.Id);
        LoadViewDataAfterOnEdit(footersection);
        ViewData["FooterSectionParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(footersection);
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
        if(!User.CanDelete("FooterSection") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FooterSection footersection = db.FooterSections.Find(id);
        if(footersection == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["FooterSectionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FooterSection"))
            ViewData["FooterSectionParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(footersection);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="footersection">  The FooterSection object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(FooterSection footersection, string UrlReferrer)
    {
        if(!User.CanDelete("FooterSection"))
        {
            return RedirectToAction("Index", "Error");
        }
        footersection = db.FooterSections.Find(footersection.Id);
        if(CheckBeforeDelete(footersection))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(footersection, out customdelete_hasissues, "Delete"))
            {
                db.Entry(footersection).State = EntityState.Deleted;
                //db.FooterSections.Remove(footersection); //issue delete with userbased security
                if(footersection.DocumentUpload != null)
                {
                    DeleteDocument(footersection.DocumentUpload);
                }
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["FooterSectionParentUrl"] != null)
                {
                    string parentUrl = ViewData["FooterSectionParentUrl"].ToString();
                    ViewData["FooterSectionParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(footersection);
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
    /// <summary>Saves the properties value (inline grid edit).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">        The identifier.</param>
    /// <param name="properties">The properties.</param>
    ///
    /// <returns>A response stream to send to the SavePropertiesValue View.</returns>
    public ActionResult SavePropertiesValue(long id, Dictionary<string, string> properties, bool? IsConfirm = false)
    {
        if(id > 0 && properties != null)
        {
            if(!User.CanEdit("FooterSection"))
            {
                return RedirectToAction("Index", "Error");
            }
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //FooterSection footersection = db.FooterSections.Find(id);
            db.Configuration.LazyLoadingEnabled = false;
            FooterSection footersection = db.FooterSections.Where(p=>p.Id == id).FirstOrDefault();
            if(footersection == null)
            {
                if(footersection == null)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    return HttpNotFound();
                }
            }
            // business rule before load section
            var resultBefore = ApplyBusinessRuleBefore(footersection, true);
            if(resultBefore.Data != null)
            {
                var result = new { Result = "fail", data = resultBefore.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            var strChkBeforeSave = CheckBeforeSave(footersection, "SaveProperty");
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                var result = new { Result = "fail", data = strChkBeforeSave };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            bool isSave = false;
            footersection.setDateTimeToClientTime();
            foreach(var item in properties)
            {
                var propertyName = item.Key;
                var propertyValue = item.Value;
                var propertyInfo = footersection.GetType().GetProperty(propertyName);
                if(propertyInfo != null)
                {
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object newValue = string.IsNullOrEmpty(propertyValue) ? null : propertyValue;
                    isSave = true;
                    try
                    {
                        if(propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && targetType.Name.ToLower() == "decimal" && newValue == null)
                            newValue = 0M;
                        object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                        propertyInfo.SetValue(footersection, safeValue, null);
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
            var resultOnSaving = ApplyBusinessRuleOnSaving(footersection);
            if(resultOnSaving.Data != null && !IsConfirm.Value)
            {
                var result = new { Result = "fail", data = resultOnSaving.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            // business rule onsaving section
            if(isSave && ValidateModel(footersection))
            {
                bool customsave_hasissues = false;
                if(!CustomSaveOnEdit(footersection, out customsave_hasissues, "Save"))
                {
                    db.Entry(footersection).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.Configuration.LazyLoadingEnabled = true;
                var result = new { Result = "Success", data = footersection.DisplayValue };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach(var error in ValidateModelWithErrors(footersection))
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
    /// <remarks></remarks>
    ///
    /// <param name="id">      The identifier.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">   The value.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyValue View.</returns>
    [HttpGet]
    public ActionResult SavePropertyValue(long id, string property, string value)
    {
        if(!User.CanEdit("FooterSection"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FooterSection footersection = db.FooterSections.Find(id);
        if(footersection == null)
        {
            return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var resultBefore = ApplyBusinessRuleBefore(footersection,true);
        if(resultBefore.Data != null)
        {
            var result = new { Result = "fail", data = resultBefore.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule section
        var strChkBeforeSave = CheckBeforeSave(footersection, "SaveProperty");
        if(!string.IsNullOrEmpty(strChkBeforeSave))
        {
            var result = new { Result = "fail", data = strChkBeforeSave };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var propertyInfo = footersection.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            footersection.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(footersection, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        var resultOnSaving = ApplyBusinessRuleOnSaving(footersection);
        if(resultOnSaving.Data != null)
        {
            var result = new { Result = "fail", data = resultOnSaving.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule onsaving section
        if(isSave && ValidateModel(footersection))
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(footersection, out customsave_hasissues, "Save"))
            {
                db.Entry(footersection).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Success", data = value };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(footersection))
            {
                errors += error.ErrorMessage + ".  ";
            }
            var result = new { Result = "fail", data = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        if(HostingEntity == "CompanyInformation" && AssociatedType == "CompanyInformationFooterSectionAssociation")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                FooterSection obj = db.FooterSections.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.CompanyInformationFooterSectionAssociationID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "FooterSectiontype" && AssociatedType == "AssociatedFooterSectionType")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                FooterSection obj = db.FooterSections.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.AssociatedFooterSectionTypeID = HostingID;
                db.SaveChanges();
            }
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "FooterSection", User) || !User.CanDelete("FooterSection")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            FooterSection footersection = db.FooterSections.Find(id);
            if(footersection != null)
            {
                if(CheckBeforeDelete(footersection))
                {
                    if(!CustomDelete(footersection, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(footersection).State = EntityState.Deleted;
                        db.FooterSections.Remove(footersection);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "FooterSection", User) || !User.CanEdit("FooterSection") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("FooterSection", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="footersection">  The FooterSection object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,CompanyInformationFooterSectionAssociationID,Name,AssociatedFooterSectionTypeID,WebLinkTitle,WebLink,DocumentUpload")] FooterSection footersection, HttpPostedFileBase File_DocumentUpload, String CamerafileUploadDocumentUpload,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                FooterSection target = db.FooterSections.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(footersection, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target,"BulkUpdate");
            }
        }
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(footersection,"BulkUpdate","");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    public ActionResult DeleteImageGalleryDocumentAndUpdate(string ID, long recId, string PropName, string IDs)
    {
        var outputres = "";
        byte[] ConcurrencyKeyvalue=null;
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
                FooterSection footersection = ctx.FooterSections.Find(Convert.ToInt64(recId));
                if(PropName=="DocumentUpload")
                    footersection.DocumentUpload = null;
                ctx.Entry(footersection).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue= ctx.FooterSections.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            FooterSection footersection = ctx.FooterSections.Find(Convert.ToInt64(recId));
            ctx.Entry(footersection).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue= ctx.FooterSections.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            if(db!=null && CacheHelper.NoCache("FooterSection")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
