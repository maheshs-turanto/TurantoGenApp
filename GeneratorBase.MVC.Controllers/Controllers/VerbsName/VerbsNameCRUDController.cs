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
/// <summary>A partial controller class for Verbs Name actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class VerbsNameController : BaseController
{

    [Audit("ViewList")]
    /// <summary>Verbs Name Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(VerbsNameIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        VerbsNameIndexViewModel model = new VerbsNameIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstVerbsName = from s in db.VerbsNames
                           select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstVerbsName = searchRecords(lstVerbsName, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstVerbsName = sortRecords(lstVerbsName, model.sortBy, model.IsAsc);
        }
        else lstVerbsName = lstVerbsName.OrderByDescending(c => c.Id);
        lstVerbsName = CustomSorting(lstVerbsName, model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter, model);
        model = (VerbsNameIndexViewModel)SetPagination(model, "VerbsName");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.VerbsNameRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "VerbsName", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var ExportData = db.T_ExportDataConfigurations.Where(p => p.T_EntityName == "VerbsName" && !p.T_Disable.Value).OrderBy(p => p.Id).ToList();
        ViewBag.ExportDataTemplates = ExportData.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        //
        var _VerbsName = lstVerbsName;
        _VerbsName = FilterByHostingEntity(_VerbsName, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _VerbsName.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _VerbsName);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _VerbsName.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstVerbsName.Except(_VerbsName).Count();
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
        TempData["VerbsNamelist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityVerbsNameDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["VerbsNamelist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            model.list = list;
            return View(model);
        }
        else
        {
            if(model.BulkOperation != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
            {
                return DoBulkOperations(model, _VerbsName, lstVerbsName);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _VerbsName.Count() == 0 ? 1 : _VerbsName.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _VerbsName.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityVerbsNameDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["VerbsNamelist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    model.list = list;
                    return PartialView(Convert.ToBoolean(model.IsHomeList) ? "HomePartialList" : "IndexPartial", model);
                }
                else
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _VerbsName.Count() == 0 ? 1 : _VerbsName.Count();
                    }
                    var list = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityVerbsNameDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["VerbsNamelist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
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
        VerbsName verbsname = db.VerbsNames.Find(id);
        if(verbsname == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "VerbsName", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(verbsname);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(verbsname, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, verbsname, "VerbsName");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = GetHiddenVerbDetails(verbsname, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName, verbsname);
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
        if(!User.CanAdd("VerbsName") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "VerbsName", viewtype);
        ViewData["VerbsNameParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("VerbsName") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["VerbsNameParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("VerbsName") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["VerbsNameParentUrl"] = UrlReferrer;
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
    /// <param name="verbsnameObj">        The VerbsName object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,VerbNameSelectID,VerbId,DisplayOrder,VerbIcon,BackGroundColor,FontColor,VerbTypeID,VerbName")] VerbsName verbsnameObj, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID, string BulkAddDropDown = null)
    {
        CheckBeforeSave(verbsnameObj);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult = CustomSaveOnCreate(verbsnameObj, out customcreate_hasissues, "Create");
            if(!customcreate_hasissues && !createResult)
            {
                var entityName = db.VerbGroups.Where(p => p.Id == verbsnameObj.VerbNameSelectID).FirstOrDefault().EntityInternalName;
                verbsnameObj.VerbIds = GetVerbListForAll(entityName).Where(p => p.VerbName == verbsnameObj.VerbId).FirstOrDefault().Id.ToString();
                db.VerbsNames.Add(verbsnameObj);
                db.SaveChanges();
                if(!customcreate_hasissues)
                    return Json(new { result = "FROMPOPUP", output = verbsnameObj.Id, editurl = Url.Action("Edit", new { id = verbsnameObj.Id }) }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        LoadViewDataAfterOnCreate(verbsnameObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(verbsnameObj, AssociatedEntity);
        return View(verbsnameObj);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsnameObj">The VerbsName object.</param>
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
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,VerbNameSelectID,VerbId,DisplayOrder,VerbIcon,BackGroundColor,FontColor,VerbId,VerbTypeID,VerbName")] VerbsName verbsnameObj, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(verbsnameObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult = CustomSaveOnCreate(verbsnameObj, out customcreate_hasissues, command);
            if(!customcreate_hasissues && !createResult)
            {
                var entityName = db.VerbGroups.Where(p => p.Id == verbsnameObj.VerbNameSelectID).FirstOrDefault().EntityInternalName;
                verbsnameObj.VerbIds = GetVerbListForAll(entityName).Where(p => p.VerbName == verbsnameObj.VerbId).FirstOrDefault().Id.ToString();
                db.VerbsNames.Add(verbsnameObj);
                db.SaveChanges();
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(verbsnameObj, "Create", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode == "wizard")
                {
                    if(!User.CanEdit("VerbsName"))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Edit", new { Id = verbsnameObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                    }
                }
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = verbsnameObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = verbsnameObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        LoadViewDataAfterOnCreate(verbsnameObj);
        ViewData["VerbsNameParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(verbsnameObj);
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
        if(!User.CanEdit("VerbsName") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        VerbsName verbsname = db.VerbsNames.Find(id);
        if(verbsname == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["VerbsNameParentUrl"] = UrlReferrer;
        if(ViewData["VerbsNameParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName") && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName/Edit/" + verbsname.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName/Create"))
            ViewData["VerbsNameParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(verbsname);
        var objVerbsName = new List<VerbsName>();
        ViewBag.VerbsNameDD = new SelectList(objVerbsName, "ID", "DisplayValue");
        return View(verbsname);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsname">       The VerbsName object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,VerbNameSelectID,ValueForMultiselectVerb,DisplayOrder,VerbIcon,BackGroundColor,FontColor,VerbId,VerbTypeID,VerbName")] VerbsName verbsname, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) verbsname = UpdateHiddenProperties(verbsname, cannotviewproperties);
        CheckBeforeSave(verbsname, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult = CustomSaveOnEdit(verbsname, out customsave_hasissues, command);
            if(!customsave_hasissues && !editResult)
            {
                db.Entry(verbsname).State = EntityState.Modified;
                db.SaveChanges();
                var result = new { Result = "Succeed", UrlRefr = UrlReferrer };
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
        if(db.Entry(verbsname).State == EntityState.Detached)
            verbsname = db.VerbsNames.Find(verbsname.Id);
        LoadViewDataAfterOnEdit(verbsname);
        return View(verbsname);
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
        if(!User.CanEdit("VerbsName") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        VerbsName verbsname = db.VerbsNames.Find(id);
        if(verbsname == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["VerbsNamelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.VerbsNames.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityVerbsNameDisplayValueEdit = TempData["VerbsNamelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["VerbsNamelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityVerbsNameDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "VerbsName", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = verbsname.DisplayValue, Value = verbsname.Id.ToString() }));
            ViewBag.EntityVerbsNameDisplayValueEdit = newList;
            TempData["VerbsNamelist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(verbsname.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = verbsname.DisplayValue;
                newList[0].Value = verbsname.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = verbsname.DisplayValue, Value = verbsname.Id.ToString() }));
            }
            ViewBag.EntityVerbsNameDisplayValueEdit = newList;
            TempData["VerbsNamelist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["VerbsNameParentUrl"] = UrlReferrer;
        if(ViewData["VerbsNameParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName") && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName/Edit/" + verbsname.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName/Create"))
            ViewData["VerbsNameParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step" + wizardstep;
        LoadViewDataBeforeOnEdit(verbsname);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, verbsname);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsname">    The VerbsName object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,VerbNameSelectID,VerbId,DisplayOrder,VerbIcon,BackGroundColor,FontColor,VerbTypeID,VerbName")] VerbsName verbsname, string UrlReferrer, bool RenderPartial = false, string viewmode = "edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) verbsname = UpdateHiddenProperties(verbsname, cannotviewproperties);
        CheckBeforeSave(verbsname, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult = CustomSaveOnEdit(verbsname, out customsave_hasissues, command);
            if(!customsave_hasissues && !editResult)
            {
                var entityName = db.VerbGroups.Where(p => p.Id == verbsname.VerbNameSelectID).FirstOrDefault().EntityInternalName;
                verbsname.VerbIds = GetVerbListForAll(entityName).Where(p => p.VerbName == verbsname.VerbId).FirstOrDefault().Id.ToString();
                db.Entry(verbsname).State = EntityState.Modified;
                db.SaveChanges();
                if(!customsave_hasissues)
                {
                    RedirectToRouteResult customRedirectAction = CustomRedirectUrl(verbsname, "Edit", command, viewmode);
                    if(customRedirectAction != null) return customRedirectAction;
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = verbsname.Id };
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
                            return RedirectToAction("Edit", new { Id = verbsname.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        if(TempData["VerbsNamelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.VerbsNames.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityVerbsNameDisplayValueEdit = TempData["VerbsNamelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["VerbsNamelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityVerbsNameDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(verbsname).State == EntityState.Detached)
            verbsname = db.VerbsNames.Find(verbsname.Id);
        LoadViewDataAfterOnEdit(verbsname);
        ViewData["VerbsNameParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(verbsname);
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
        if(!User.CanDelete("VerbsName") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        VerbsName verbsname = db.VerbsNames.Find(id);
        if(verbsname == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["VerbsNameParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/VerbsName"))
            ViewData["VerbsNameParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(verbsname);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsname">  The VerbsName object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(VerbsName verbsname, string UrlReferrer)
    {
        if(!User.CanDelete("VerbsName"))
        {
            return RedirectToAction("Index", "Error");
        }
        verbsname = db.VerbsNames.Find(verbsname.Id);
        if(CheckBeforeDelete(verbsname))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(verbsname, out customdelete_hasissues, "Delete"))
            {
                db.Entry(verbsname).State = EntityState.Deleted;
                //db.VerbsNames.Remove(verbsname); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer) && !UrlReferrer.Contains("/Delete/"))
                    return Redirect(UrlReferrer);
                if(ViewData["VerbsNameParentUrl"] != null)
                {
                    string parentUrl = ViewData["VerbsNameParentUrl"].ToString();
                    ViewData["VerbsNameParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(verbsname);
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
        VerbsName verbsname = db.VerbsNames.Find(Id);
        db.Entry(verbsname).State = EntityState.Modified;
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
        if(HostingEntity == "VerbGroup" && AssociatedType == "VerbNameSelect")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                VerbsName obj = db.VerbsNames.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.VerbNameSelectID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "VerbName" && AssociatedType == "VerbsNameVerbNameAssociation")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                VerbsName obj = db.VerbsNames.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.VerbId = Convert.ToString(HostingID);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "VerbsName", User) || !User.CanDelete("VerbsName")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            VerbsName verbsname = db.VerbsNames.Find(id);
            if(verbsname != null)
            {
                if(CheckBeforeDelete(verbsname))
                {
                    if(!CustomDelete(verbsname, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(verbsname).State = EntityState.Deleted;
                        db.VerbsNames.Remove(verbsname);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "VerbsName", User) || !User.CanEdit("VerbsName") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("VerbsName", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType, true);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsname">  The VerbsName object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,VerbNameSelectID,VerbId,DisplayOrder,VerbIcon,BackGroundColor,FontColor,VerbId,VerbTypeID,VerbName")] VerbsName verbsname, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                VerbsName target = db.VerbsNames.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(verbsname, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.verbnameselect != null)
                            db.Entry(target.verbnameselect).State = EntityState.Unchanged;
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(verbsname, "BulkUpdate", "");
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
                VerbsName verbsname = ctx.VerbsNames.Find(Convert.ToInt64(recId));
                ctx.Entry(verbsname).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue = ctx.VerbsNames.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            VerbsName verbsname = ctx.VerbsNames.Find(Convert.ToInt64(recId));
            ctx.Entry(verbsname).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue = ctx.VerbsNames.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
        VerbsName _verbsname = db.VerbsNames.Find(Convert.ToInt64(id));
        return View(_verbsname);
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
            if(db != null && CacheHelper.NoCache("VerbsName")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
