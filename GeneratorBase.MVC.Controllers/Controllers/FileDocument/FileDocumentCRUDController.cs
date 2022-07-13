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
/// <summary>A partial controller class for Document actions (CRUD and others).</summary>
///
/// <remarks></remarks>
[LocalDateTimeConverter]
public partial class FileDocumentController : BaseController
{

    /// <summary>Document Index Action, renders items in different UI format based on viewtype</summary>
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
    // public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype,string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false, string ExportType = null)
    public ActionResult Index(FileDocumentIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        FileDocumentIndexViewModel model = new FileDocumentIndexViewModel(User, args);
        if(model.TemplatesName != null && model.TemplatesName.ToLower() == "indexpartiallist")
        {
            model.TemplatesName = "IndexPartialGallery";
            ViewBag.isList = true;
        }
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstFileDocument = from s in db.FileDocuments
                              select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstFileDocument = searchRecords(lstFileDocument, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstFileDocument = sortRecords(lstFileDocument, model.sortBy, model.IsAsc);
        }
        else lstFileDocument = lstFileDocument.OrderByDescending(c => c.Id);
        lstFileDocument = CustomSorting(lstFileDocument,model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter, model);
        model = (FileDocumentIndexViewModel)SetPagination(model, "FileDocument");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.FileDocumentRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "FileDocument", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var ExportData = db.T_ExportDataConfigurations.Where(p => p.T_EntityName == "FileDocument" && !p.T_Disable.Value).OrderBy(p => p.Id).ToList();
        ViewBag.ExportDataTemplates = ExportData.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        //
        var _FileDocument = lstFileDocument;
        _FileDocument = FilterByHostingEntity(_FileDocument, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _FileDocument.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _FileDocument);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _FileDocument.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstFileDocument.Except(_FileDocument).Count();
                }
                int quotient = totalListCount / model.PageSize;
                var remainder = totalListCount % model.PageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(model.pageNumber > maxpagenumber)
                {
                    model.pageNumber = 1;
                }
            }
        }
        model.Pages = model.pageNumber;
        TempData["FileDocumentlist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _FileDocument.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["FileDocumentlist"] = list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            });
            model.list = list;
            return View(model);
        }
        else
        {
            if(model.BulkOperation != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
            {
                return DoBulkOperations(model, _FileDocument, lstFileDocument);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _FileDocument.Count() == 0 ? 1 : _FileDocument.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _FileDocument.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["FileDocumentlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    model.list = list;
                    return PartialView(Convert.ToBoolean(model.IsHomeList) ? "HomePartialList" : "IndexPartial", model);
                }
                else
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _FileDocument.Count() == 0 ? 1 : _FileDocument.Count();
                    }
                    var list = _FileDocument.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["FileDocumentlist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    model.list = list;
                    return PartialView(model.TemplatesName, model);
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
        FileDocument filedocument = db.FileDocuments.Find(id);
        if(filedocument == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User,"FileDocument",defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(filedocument);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(filedocument, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, filedocument, "FileDocument");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = GetHiddenVerbDetails(filedocument, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName,filedocument);
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
        if(!User.CanAdd("FileDocument") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User,"FileDocument",viewtype);
        ViewData["FileDocumentParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("FileDocument") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["FileDocumentParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("FileDocument") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["FileDocumentParentUrl"] = UrlReferrer;
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
    /// <param name="filedocumentObj">        The FileDocument object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,DateCreated,DateLastUpdated")] FileDocument filedocumentObj, HttpPostedFileBase AttachDocument, String CamerafileUploadAttachDocument,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID,string BulkAddDropDown = null)
    {
        CheckBeforeSave(filedocumentObj);
        if(AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
            IsFileTypeAndSizeAllowed(AttachDocument);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult=CustomSaveOnCreate(filedocumentObj,out customcreate_hasissues,"Create");
            if(!customcreate_hasissues && !createResult)
            {
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(AttachDocument != null)
                {
                    filedocumentObj.AttachDocument = long.Parse(SaveAnyDocument(AttachDocument, "file", null, "FileDocument"));
                }
                if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"]  != "")
                {
                    filedocumentObj.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", null, "FileDocument"));
                }
                var ValueForMultiselect = Request.Form["ValueForMultiselect"];
                var idsBulk = ValueForMultiselect.Split(',').ToList();
                if(idsBulk.Count() > 1)
                {
                    foreach(var ddId in idsBulk)
                    {
                        var target = new FileDocument();
                        EntityCopy.CopyValuesForSameObjectType(filedocumentObj, target);
                        db.FileDocuments.Add(target);
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.FileDocuments.Add(filedocumentObj);
                    db.SaveChanges();
                }
                if(!customcreate_hasissues)
                    return Json(new {result = "FROMPOPUP", output = filedocumentObj.Id, editurl=Url.Action("Edit",new { id= filedocumentObj.Id })}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
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
        LoadViewDataAfterOnCreate(filedocumentObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(filedocumentObj, AssociatedEntity);
        return View(filedocumentObj);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="filedocumentObj">The FileDocument object.</param>
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
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,DateCreated,DateLastUpdated")] FileDocument filedocumentObj, HttpPostedFileBase AttachDocument, String CamerafileUploadAttachDocument, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(filedocumentObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
            IsFileTypeAndSizeAllowed(AttachDocument);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult= CustomSaveOnCreate(filedocumentObj,out customcreate_hasissues,command);
            if(!customcreate_hasissues && !createResult)
            {
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(AttachDocument != null)
                {
                    filedocumentObj.AttachDocument = long.Parse(SaveAnyDocument(AttachDocument, "file", null, "FileDocument"));
                }
                if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"]  != "")
                {
                    filedocumentObj.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", null, "FileDocument"));
                }
                db.FileDocuments.Add(filedocumentObj);
                db.SaveChanges();
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(filedocumentObj,"Create",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode=="wizard")
                {
                    if(!User.CanEdit("FileDocument"))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Edit", new { Id = filedocumentObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                    }
                }
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = filedocumentObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = filedocumentObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
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
        LoadViewDataAfterOnCreate(filedocumentObj);
        ViewData["FileDocumentParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(filedocumentObj);
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
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype, bool RecordReadOnly= false)
    {
        if(!User.CanEdit("FileDocument") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        FileDocument filedocument = db.FileDocuments.Find(id);
        if(filedocument == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["FileDocumentParentUrl"] = UrlReferrer;
        if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Edit/" + filedocument.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Create"))
            ViewData["FileDocumentParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(filedocument);
        var objFileDocument = new List<FileDocument>();
        ViewBag.FileDocumentDD = new SelectList(objFileDocument, "ID", "DisplayValue");
        return View(filedocument);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="filedocument">       The FileDocument object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument, String CamerafileUploadAttachDocument,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) filedocument = UpdateHiddenProperties(filedocument, cannotviewproperties);
        CheckBeforeSave(filedocument, command);
        if(File_AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
            IsFileTypeAndSizeAllowed(File_AttachDocument);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult= CustomSaveOnEdit(filedocument,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                //string path = Server.MapPath("~/Files/");
                //string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(File_AttachDocument != null)
                {
                    if(filedocument.AttachDocument != null)
                        filedocument.AttachDocument = long.Parse(SaveAnyDocument(File_AttachDocument, "file", filedocument.AttachDocument, "FileDocument"));
                    else
                        filedocument.AttachDocument = long.Parse(SaveAnyDocument(File_AttachDocument, "file", null, "FileDocument"));
                }
                if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
                {
                    //filedocument.AttachDocument = SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file");
                    if(filedocument.AttachDocument != null)
                        filedocument.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", filedocument.AttachDocument, "FileDocument"));
                    else
                        filedocument.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", null, "FileDocument"));
                }
                db.Entry(filedocument).State = EntityState.Modified;
                db.SaveChanges();
                var result = new { Result = "Succeed", UrlRefr =UrlReferrer };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
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
        if(db.Entry(filedocument).State == EntityState.Detached)
            filedocument = db.FileDocuments.Find(filedocument.Id);
        LoadViewDataAfterOnEdit(filedocument);
        return View(filedocument);
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
        if(!User.CanEdit("FileDocument") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        FileDocument filedocument = db.FileDocuments.Find(id);
        if(filedocument == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["FileDocumentlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.FileDocuments.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityFileDocumentDisplayValueEdit = TempData["FileDocumentlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["FileDocumentlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityFileDocumentDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User,"FileDocument",defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = filedocument.DisplayValue, Value = filedocument.Id.ToString() }));
            ViewBag.EntityFileDocumentDisplayValueEdit = newList;
            TempData["FileDocumentlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(filedocument.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = filedocument.DisplayValue;
                newList[0].Value = filedocument.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = filedocument.DisplayValue, Value = filedocument.Id.ToString() }));
            }
            ViewBag.EntityFileDocumentDisplayValueEdit = newList;
            TempData["FileDocumentlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["FileDocumentParentUrl"] = UrlReferrer;
        if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Edit/" + filedocument.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Create"))
            ViewData["FileDocumentParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step"+wizardstep;
        LoadViewDataBeforeOnEdit(filedocument);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,filedocument);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="filedocument">    The FileDocument object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument, String CamerafileUploadAttachDocument,  string UrlReferrer, bool RenderPartial = false, string viewmode="edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) filedocument = UpdateHiddenProperties(filedocument, cannotviewproperties);
        CheckBeforeSave(filedocument, command);
        if(File_AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
            IsFileTypeAndSizeAllowed(File_AttachDocument);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult=CustomSaveOnEdit(filedocument,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                //string path = Server.MapPath("~/Files/");
                //string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(File_AttachDocument != null)
                {
                    //filedocument.AttachDocument = SaveAnyDocument(File_AttachDocument, "file");
                    if(filedocument.AttachDocument != null)
                        filedocument.AttachDocument = long.Parse(SaveAnyDocument(File_AttachDocument, "file", filedocument.AttachDocument, "FileDocument"));
                    else
                        filedocument.AttachDocument = long.Parse(SaveAnyDocument(File_AttachDocument, "file", null, "FileDocument"));
                }
                if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
                {
                    //filedocument.AttachDocument = SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file");
                    if(filedocument.AttachDocument != null)
                        filedocument.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", filedocument.AttachDocument, "FileDocument"));
                    else
                        filedocument.AttachDocument = long.Parse(SaveAnyCameraFile(Request, "CamerafileUploadAttachDocument", "file", null, "FileDocument"));
                }
                db.Entry(filedocument).State = EntityState.Modified;
                db.SaveChanges();
                if(!customsave_hasissues)
                {
                    RedirectToRouteResult customRedirectAction = CustomRedirectUrl(filedocument,"Edit",command, viewmode);
                    if(customRedirectAction != null) return customRedirectAction;
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = filedocument.Id };
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
                            return RedirectToAction("Edit", new { Id = filedocument.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep  });
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
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["FileDocumentlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.FileDocuments.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityFileDocumentDisplayValueEdit = TempData["FileDocumentlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["FileDocumentlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityFileDocumentDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(filedocument).State == EntityState.Detached)
            filedocument = db.FileDocuments.Find(filedocument.Id);
        LoadViewDataAfterOnEdit(filedocument);
        ViewData["FileDocumentParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(filedocument);
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
        if(!User.CanDelete("FileDocument") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FileDocument filedocument = db.FileDocuments.Find(id);
        if(filedocument == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument"))
            ViewData["FileDocumentParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(filedocument);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="filedocument">  The FileDocument object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(FileDocument filedocument, string UrlReferrer)
    {
        if(!User.CanDelete("FileDocument"))
        {
            return RedirectToAction("Index", "Error");
        }
        filedocument = db.FileDocuments.Find(filedocument.Id);
        if(CheckBeforeDelete(filedocument))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(filedocument, out customdelete_hasissues, "Delete"))
            {
                db.Entry(filedocument).State = EntityState.Deleted;
                //db.FileDocuments.Remove(filedocument); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer) && !UrlReferrer.Contains("/Delete/"))
                    return Redirect(UrlReferrer);
                if(ViewData["FileDocumentParentUrl"] != null)
                {
                    string parentUrl = ViewData["FileDocumentParentUrl"].ToString();
                    ViewData["FileDocumentParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(filedocument);
    }
    
    /// <summary>Deletes the document described by docID.</summary>
    ///
    /// <param name="docID">Identifier for the document.</param>
    public ActionResult DocumentDeassociate(long? docid, long Id, string propname)
    {
        if(!User.CanDelete("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        var document = db.Documents.Find(docid);
        db.Entry(document).State = EntityState.Deleted;
        db.Documents.Remove(document);
        FileDocument filedocument = db.FileDocuments.Find(Id);
        if(propname == "AttachDocument")
        {
            filedocument.AttachDocument = null;
        }
        db.Entry(filedocument).State = EntityState.Modified;
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "FileDocument", User) || !User.CanDelete("FileDocument")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            FileDocument filedocument = db.FileDocuments.Find(id);
            if(filedocument != null)
            {
                if(CheckBeforeDelete(filedocument))
                {
                    if(!CustomDelete(filedocument, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(filedocument).State = EntityState.Deleted;
                        db.FileDocuments.Remove(filedocument);
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
                FileDocument filedocument = ctx.FileDocuments.Find(Convert.ToInt64(recId));
                ctx.Entry(filedocument).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue= ctx.FileDocuments.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            FileDocument filedocument = ctx.FileDocuments.Find(Convert.ToInt64(recId));
            ctx.Entry(filedocument).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue= ctx.FileDocuments.Find(Convert.ToInt64(recId)).ConcurrencyKey;
        }
        return Json(new { result = "POP", ConcurrencyKey = ConcurrencyKeyvalue }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>
    /// .
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Audit(0)]
    public ActionResult PopupCard(string id)
    {
        FileDocument _filedocument = db.FileDocuments.Find(Convert.ToInt64(id));
        return View(_filedocument);
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
            if(db!=null && CacheHelper.NoCache("FileDocument")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
