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
/// <summary>A partial controller class for ExportDataLog Status actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class T_ExportDataLogstatusController : BaseController
{

    /// <summary>ExportDataLog Status Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(T_ExportDataLogstatusIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        T_ExportDataLogstatusIndexViewModel model = new T_ExportDataLogstatusIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstT_ExportDataLogstatus = from s in db.T_ExportDataLogstatuss
                                       select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_ExportDataLogstatus = searchRecords(lstT_ExportDataLogstatus, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_ExportDataLogstatus = sortRecords(lstT_ExportDataLogstatus, model.sortBy, model.IsAsc);
        }
        else lstT_ExportDataLogstatus = lstT_ExportDataLogstatus.OrderByDescending(c => c.Id);
        lstT_ExportDataLogstatus = CustomSorting(lstT_ExportDataLogstatus,model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter);
        model = (T_ExportDataLogstatusIndexViewModel)SetPagination(model, "T_ExportDataLogstatus");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.T_ExportDataLogstatusRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_ExportDataLogstatus", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        var _T_ExportDataLogstatus = lstT_ExportDataLogstatus;
        _T_ExportDataLogstatus = FilterByHostingEntity(_T_ExportDataLogstatus, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_ExportDataLogstatus.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_ExportDataLogstatus);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_ExportDataLogstatus.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstT_ExportDataLogstatus.Except(_T_ExportDataLogstatus).Count();
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
        TempData["T_ExportDataLogstatuslist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_ExportDataLogstatus.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_ExportDataLogstatusDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_ExportDataLogstatuslist"] = list.Select(z => new
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
                return DoBulkOperations(model, _T_ExportDataLogstatus, lstT_ExportDataLogstatus);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _T_ExportDataLogstatus.Count() == 0 ? 1 : _T_ExportDataLogstatus.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _T_ExportDataLogstatus.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityT_ExportDataLogstatusDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataLogstatuslist"] = list.Select(z => new
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
                        model.PageSize = _T_ExportDataLogstatus.Count() == 0 ? 1 : _T_ExportDataLogstatus.Count();
                    }
                    var list = _T_ExportDataLogstatus.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityT_ExportDataLogstatusDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataLogstatuslist"] = list.Select(z => new
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
        T_ExportDataLogstatus t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(id);
        if(t_exportdatalogstatus == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User,"T_ExportDataLogstatus",defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_exportdatalogstatus);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_exportdatalogstatus, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, t_exportdatalogstatus, "T_ExportDataLogstatus");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = GetHiddenVerbDetails(t_exportdatalogstatus, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName,t_exportdatalogstatus);
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
        if(!User.CanAdd("T_ExportDataLogstatus") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User,"T_ExportDataLogstatus",viewtype);
        ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_ExportDataLogstatus") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_ExportDataLogstatus") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
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
    /// <param name="t_exportdatalogstatusObj">        The T_ExportDataLogstatus object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_ExportDataLogstatus t_exportdatalogstatusObj,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID,string BulkAddDropDown = null)
    {
        CheckBeforeSave(t_exportdatalogstatusObj);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult=CustomSaveOnCreate(t_exportdatalogstatusObj,out customcreate_hasissues,"Create");
            if(!customcreate_hasissues && !createResult)
            {
                var ValueForMultiselect = Request.Form["ValueForMultiselect"];
                var idsBulk = ValueForMultiselect.Split(',').ToList();
                if(idsBulk.Count() > 1)
                {
                    foreach(var ddId in idsBulk)
                    {
                        var target = new T_ExportDataLogstatus();
                        EntityCopy.CopyValuesForSameObjectType(t_exportdatalogstatusObj, target);
                        db.T_ExportDataLogstatuss.Add(target);
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.T_ExportDataLogstatuss.Add(t_exportdatalogstatusObj);
                    db.SaveChanges();
                }
                if(!customcreate_hasissues)
                    return Json(new {result = "FROMPOPUP", output = t_exportdatalogstatusObj.Id, editurl=Url.Action("Edit",new { id= t_exportdatalogstatusObj.Id })}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(t_exportdatalogstatusObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_exportdatalogstatusObj, AssociatedEntity);
        return View(t_exportdatalogstatusObj);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalogstatusObj">The T_ExportDataLogstatus object.</param>
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
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_ExportDataLogstatus t_exportdatalogstatusObj, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_exportdatalogstatusObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult= CustomSaveOnCreate(t_exportdatalogstatusObj,out customcreate_hasissues,command);
            if(!customcreate_hasissues && !createResult)
            {
                db.T_ExportDataLogstatuss.Add(t_exportdatalogstatusObj);
                db.SaveChanges();
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalogstatusObj,"Create",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode=="wizard")
                    return RedirectToAction("Edit", new { Id = t_exportdatalogstatusObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_exportdatalogstatusObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_exportdatalogstatusObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
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
        LoadViewDataAfterOnCreate(t_exportdatalogstatusObj);
        ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdatalogstatusObj);
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
        if(!User.CanEdit("T_ExportDataLogstatus") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_ExportDataLogstatus t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(id);
        if(t_exportdatalogstatus == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
        if(ViewData["T_ExportDataLogstatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus/Edit/" + t_exportdatalogstatus.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus/Create"))
            ViewData["T_ExportDataLogstatusParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(t_exportdatalogstatus);
        var objT_ExportDataLogstatus = new List<T_ExportDataLogstatus>();
        ViewBag.T_ExportDataLogstatusDD = new SelectList(objT_ExportDataLogstatus, "ID", "DisplayValue");
        return View(t_exportdatalogstatus);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalogstatus">       The T_ExportDataLogstatus object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_Description")] T_ExportDataLogstatus t_exportdatalogstatus,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_exportdatalogstatus = UpdateHiddenProperties(t_exportdatalogstatus, cannotviewproperties);
        CheckBeforeSave(t_exportdatalogstatus, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult= CustomSaveOnEdit(t_exportdatalogstatus,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                db.Entry(t_exportdatalogstatus).State = EntityState.Modified;
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
        if(db.Entry(t_exportdatalogstatus).State == EntityState.Detached)
            t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(t_exportdatalogstatus.Id);
        LoadViewDataAfterOnEdit(t_exportdatalogstatus);
        return View(t_exportdatalogstatus);
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
        if(!User.CanEdit("T_ExportDataLogstatus") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_ExportDataLogstatus t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(id);
        if(t_exportdatalogstatus == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_ExportDataLogstatuslist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataLogstatuss.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = TempData["T_ExportDataLogstatuslist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataLogstatuslist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User,"T_ExportDataLogstatus",defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_exportdatalogstatus.DisplayValue, Value = t_exportdatalogstatus.Id.ToString() }));
            ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = newList;
            TempData["T_ExportDataLogstatuslist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_exportdatalogstatus.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_exportdatalogstatus.DisplayValue;
                newList[0].Value = t_exportdatalogstatus.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_exportdatalogstatus.DisplayValue, Value = t_exportdatalogstatus.Id.ToString() }));
            }
            ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = newList;
            TempData["T_ExportDataLogstatuslist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
        if(ViewData["T_ExportDataLogstatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus/Edit/" + t_exportdatalogstatus.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus/Create"))
            ViewData["T_ExportDataLogstatusParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step"+wizardstep;
        LoadViewDataBeforeOnEdit(t_exportdatalogstatus);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,t_exportdatalogstatus);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalogstatus">    The T_ExportDataLogstatus object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_Description")] T_ExportDataLogstatus t_exportdatalogstatus,  string UrlReferrer, bool RenderPartial = false, string viewmode="edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_exportdatalogstatus = UpdateHiddenProperties(t_exportdatalogstatus, cannotviewproperties);
        CheckBeforeSave(t_exportdatalogstatus, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult=CustomSaveOnEdit(t_exportdatalogstatus,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                db.Entry(t_exportdatalogstatus).State = EntityState.Modified;
                db.SaveChanges();
                if(!customsave_hasissues)
                {
                    RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalogstatus,"Edit",command, viewmode);
                    if(customRedirectAction != null) return customRedirectAction;
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_exportdatalogstatus.Id };
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
                            return RedirectToAction("Edit", new { Id = t_exportdatalogstatus.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep  });
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
        if(TempData["T_ExportDataLogstatuslist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataLogstatuss.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = TempData["T_ExportDataLogstatuslist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataLogstatuslist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataLogstatusDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_exportdatalogstatus).State == EntityState.Detached)
            t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(t_exportdatalogstatus.Id);
        LoadViewDataAfterOnEdit(t_exportdatalogstatus);
        ViewData["T_ExportDataLogstatusParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdatalogstatus);
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
        if(!User.CanDelete("T_ExportDataLogstatus") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataLogstatus t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(id);
        if(t_exportdatalogstatus == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_ExportDataLogstatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLogstatus"))
            ViewData["T_ExportDataLogstatusParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_exportdatalogstatus);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalogstatus">  The T_ExportDataLogstatus object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_ExportDataLogstatus t_exportdatalogstatus, string UrlReferrer)
    {
        if(!User.CanDelete("T_ExportDataLogstatus"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(t_exportdatalogstatus.Id);
        if(CheckBeforeDelete(t_exportdatalogstatus))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_exportdatalogstatus, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_exportdatalogstatus).State = EntityState.Deleted;
                //db.T_ExportDataLogstatuss.Remove(t_exportdatalogstatus); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_ExportDataLogstatusParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_ExportDataLogstatusParentUrl"].ToString();
                    ViewData["T_ExportDataLogstatusParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_exportdatalogstatus);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_ExportDataLogstatus", User) || !User.CanDelete("T_ExportDataLogstatus")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_ExportDataLogstatus t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(id);
            if(t_exportdatalogstatus != null)
            {
                if(CheckBeforeDelete(t_exportdatalogstatus))
                {
                    if(!CustomDelete(t_exportdatalogstatus, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_exportdatalogstatus).State = EntityState.Deleted;
                        db.T_ExportDataLogstatuss.Remove(t_exportdatalogstatus);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_ExportDataLogstatus", User) || !User.CanEdit("T_ExportDataLogstatus") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_ExportDataLogstatus", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalogstatus">  The T_ExportDataLogstatus object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_Name,T_Description")] T_ExportDataLogstatus t_exportdatalogstatus,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_ExportDataLogstatus target = db.T_ExportDataLogstatuss.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_exportdatalogstatus, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target,"BulkUpdate");
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalogstatus,"BulkUpdate","");
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
                T_ExportDataLogstatus t_exportdatalogstatus = ctx.T_ExportDataLogstatuss.Find(Convert.ToInt64(recId));
                ctx.Entry(t_exportdatalogstatus).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue= ctx.T_ExportDataLogstatuss.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            T_ExportDataLogstatus t_exportdatalogstatus = ctx.T_ExportDataLogstatuss.Find(Convert.ToInt64(recId));
            ctx.Entry(t_exportdatalogstatus).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue= ctx.T_ExportDataLogstatuss.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
        T_ExportDataLogstatus _t_exportdatalogstatus = db.T_ExportDataLogstatuss.Find(Convert.ToInt64(id));
        return View(_t_exportdatalogstatus);
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
            if(db!=null && CacheHelper.NoCache("T_ExportDataLogstatus")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
