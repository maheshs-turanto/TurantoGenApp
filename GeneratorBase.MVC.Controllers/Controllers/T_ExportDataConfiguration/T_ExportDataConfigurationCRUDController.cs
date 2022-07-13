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
/// <summary>A partial controller class for Export Data Configuration actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class T_ExportDataConfigurationController : BaseController
{

    [Audit("ViewList")]
    /// <summary>Export Data Configuration Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(T_ExportDataConfigurationIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        T_ExportDataConfigurationIndexViewModel model = new T_ExportDataConfigurationIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstT_ExportDataConfiguration = from s in db.T_ExportDataConfigurations
                                           select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_ExportDataConfiguration = searchRecords(lstT_ExportDataConfiguration, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_ExportDataConfiguration = sortRecords(lstT_ExportDataConfiguration, model.sortBy, model.IsAsc);
        }
        else lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.OrderByDescending(c => c.Id);
        lstT_ExportDataConfiguration = CustomSorting(lstT_ExportDataConfiguration, model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter);
        model = (T_ExportDataConfigurationIndexViewModel)SetPagination(model, "T_ExportDataConfiguration");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.T_ExportDataConfigurationRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_ExportDataConfiguration", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        var _T_ExportDataConfiguration = lstT_ExportDataConfiguration;
        _T_ExportDataConfiguration = FilterByHostingEntity(_T_ExportDataConfiguration, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_ExportDataConfiguration.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_ExportDataConfiguration);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_ExportDataConfiguration.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstT_ExportDataConfiguration.Except(_T_ExportDataConfiguration).Count();
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
        TempData["T_ExportDataConfigurationlist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_ExportDataConfigurationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_ExportDataConfigurationlist"] = list.Select(z => new
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
                return DoBulkOperations(model, _T_ExportDataConfiguration, lstT_ExportDataConfiguration);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _T_ExportDataConfiguration.Count() == 0 ? 1 : _T_ExportDataConfiguration.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityT_ExportDataConfigurationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataConfigurationlist"] = list.Select(z => new
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
                        model.PageSize = _T_ExportDataConfiguration.Count() == 0 ? 1 : _T_ExportDataConfiguration.Count();
                    }
                    var list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityT_ExportDataConfigurationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataConfigurationlist"] = list.Select(z => new
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
        T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
        if(t_exportdataconfiguration == null)
        {
            db.DisableFilter("IsDeleted");
            t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
            if(t_exportdataconfiguration == null)
                return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "T_ExportDataConfiguration", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_exportdataconfiguration);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_exportdataconfiguration, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, t_exportdataconfiguration, "T_ExportDataConfiguration");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = null;// GetHiddenVerbDetails(t_exportdataconfiguration, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName, t_exportdataconfiguration);
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
        if(!User.CanAdd("T_ExportDataConfiguration") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "T_ExportDataConfiguration", viewtype);
        ViewData["T_ExportDataConfigurationParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_ExportDataConfiguration") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_ExportDataConfigurationParentUrl"] = UrlReferrer;
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
    /// <param name="t_exportdataconfigurationObj">The T_ExportDataConfiguration object.</param>
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
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,T_AutoNo,T_Name,T_EntityName,T_AllowedRoles,T_Description,T_Disable,T_EnableDelete,T_UploadOneDrive,T_UploadGoogleDrive,T_BackGroundColor,T_FontColor,T_ToolTip,T_IsRootDeleted")] T_ExportDataConfiguration t_exportdataconfigurationObj, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_exportdataconfigurationObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_exportdataconfigurationObj, out customcreate_hasissues, command))
            {
                var result = ExportDetails(Request.Form, t_exportdataconfigurationObj.T_EntityName);
                foreach(var item in result)
                {
                    t_exportdataconfigurationObj.t_exportdataconfigurationexportdatadetailsassociation.Add(item);
                }
                db.T_ExportDataConfigurations.Add(t_exportdataconfigurationObj);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdataconfigurationObj, "Create", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode == "wizard")
                    return RedirectToAction("Edit", new { Id = t_exportdataconfigurationObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_exportdataconfigurationObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_exportdataconfigurationObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        LoadViewDataAfterOnCreate(t_exportdataconfigurationObj);
        ViewData["T_ExportDataConfigurationParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdataconfigurationObj);
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
        if(!User.CanEdit("T_ExportDataConfiguration") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
        if(t_exportdataconfiguration == null)
        {
            db.DisableFilter("IsDeleted");
            t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
            if(t_exportdataconfiguration == null)
                return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_ExportDataConfigurationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataConfigurations.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = TempData["T_ExportDataConfigurationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataConfigurationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "T_ExportDataConfiguration", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataConfiguration/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_exportdataconfiguration.DisplayValue, Value = t_exportdataconfiguration.Id.ToString() }));
            ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = newList;
            TempData["T_ExportDataConfigurationlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_exportdataconfiguration.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_exportdataconfiguration.DisplayValue;
                newList[0].Value = t_exportdataconfiguration.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_exportdataconfiguration.DisplayValue, Value = t_exportdataconfiguration.Id.ToString() }));
            }
            ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = newList;
            TempData["T_ExportDataConfigurationlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_ExportDataConfigurationParentUrl"] = UrlReferrer;
        if(ViewData["T_ExportDataConfigurationParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataConfiguration") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataConfiguration/Edit/" + t_exportdataconfiguration.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataConfiguration/Create"))
            ViewData["T_ExportDataConfigurationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step" + wizardstep;
        LoadViewDataBeforeOnEdit(t_exportdataconfiguration);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, t_exportdataconfiguration);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdataconfiguration">    The T_ExportDataConfiguration object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_Name,T_EntityName,T_AllowedRoles,T_Description,T_Disable,T_EnableDelete,T_UploadOneDrive,T_UploadGoogleDrive,T_BackGroundColor,T_FontColor,T_ToolTip,T_IsRootDeleted")] T_ExportDataConfiguration t_exportdataconfiguration, string UrlReferrer, bool RenderPartial = false, string viewmode = "edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_exportdataconfiguration = UpdateHiddenProperties(t_exportdataconfiguration, cannotviewproperties);
        CheckBeforeSave(t_exportdataconfiguration, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_exportdataconfiguration, out customsave_hasissues, command))
            {
                db.Entry(t_exportdataconfiguration).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdataconfiguration, "Edit", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = t_exportdataconfiguration.Id };
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
                        return RedirectToAction("Edit", new { Id = t_exportdataconfiguration.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        if(TempData["T_ExportDataConfigurationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataConfigurations.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = TempData["T_ExportDataConfigurationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataConfigurationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataConfigurationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_exportdataconfiguration).State == EntityState.Detached)
            t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(t_exportdataconfiguration.Id);
        LoadViewDataAfterOnEdit(t_exportdataconfiguration);
        ViewData["T_ExportDataConfigurationParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdataconfiguration);
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
        if(!User.CanDelete("T_ExportDataConfiguration") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
        if(t_exportdataconfiguration == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_ExportDataConfigurationParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataConfiguration"))
            ViewData["T_ExportDataConfigurationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_exportdataconfiguration);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdataconfiguration">  The T_ExportDataConfiguration object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_ExportDataConfiguration t_exportdataconfiguration, string UrlReferrer)
    {
        if(!User.CanDelete("T_ExportDataConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(t_exportdataconfiguration.Id);
        if(CheckBeforeDelete(t_exportdataconfiguration))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_exportdataconfiguration, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_exportdataconfiguration).State = EntityState.Deleted;
                //db.T_ExportDataConfigurations.Remove(t_exportdataconfiguration); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_ExportDataConfigurationParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_ExportDataConfigurationParentUrl"].ToString();
                    ViewData["T_ExportDataConfigurationParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_exportdataconfiguration);
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
            if(!User.CanEdit("T_ExportDataConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
            db.Configuration.LazyLoadingEnabled = false;
            T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Where(p => p.Id == id).FirstOrDefault();
            if(t_exportdataconfiguration == null)
            {
                db.DisableFilter("IsDeleted");
                t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
                if(t_exportdataconfiguration == null)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    return HttpNotFound();
                }
            }
            // business rule before load section
            var resultBefore = ApplyBusinessRuleBefore(t_exportdataconfiguration, true);
            if(resultBefore.Data != null)
            {
                var result = new { Result = "fail", data = resultBefore.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            var strChkBeforeSave = CheckBeforeSave(t_exportdataconfiguration, "SaveProperty");
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                var result = new { Result = "fail", data = strChkBeforeSave };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            bool isSave = false;
            t_exportdataconfiguration.setDateTimeToClientTime();
            foreach(var item in properties)
            {
                var propertyName = item.Key;
                var propertyValue = item.Value;
                var propertyInfo = t_exportdataconfiguration.GetType().GetProperty(propertyName);
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
                        propertyInfo.SetValue(t_exportdataconfiguration, safeValue, null);
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
            var resultOnSaving = ApplyBusinessRuleOnSaving(t_exportdataconfiguration);
            if(resultOnSaving.Data != null && !IsConfirm.Value)
            {
                var result = new { Result = "fail", data = resultOnSaving.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            // business rule onsaving section
            if(isSave && ValidateModel(t_exportdataconfiguration))
            {
                bool customsave_hasissues = false;
                if(!CustomSaveOnEdit(t_exportdataconfiguration, out customsave_hasissues, "Save"))
                {
                    try
                    {
                        db.Entry(t_exportdataconfiguration).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        var errors = "";
                        foreach(var error in ValidateModelWithErrors(t_exportdataconfiguration))
                        {
                            errors += error.ErrorMessage + ".  ";
                        }
                        var res = new { Result = "fail", data = errors };
                        db.Configuration.LazyLoadingEnabled = true;
                        return Json(res, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
                db.Configuration.LazyLoadingEnabled = true;
                var result = new { Result = "Success", data = t_exportdataconfiguration.DisplayValue };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach(var error in ValidateModelWithErrors(t_exportdataconfiguration))
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
    // <summary>
    ///
    /// </summary>
    /// <param name="bulkIds"></param>
    /// <param name="properties"></param>
    /// <param name="IsConfirm"></param>
    /// <returns></returns>
    public ActionResult SavePropertiesValuebulk(string bulkIds, Dictionary<string, string> properties, bool? IsConfirm = false)
    {
        string errorMsg = "";
        var bulkId = bulkIds.Split(',');
        if(bulkId.Length > 0 && properties != null && !string.IsNullOrEmpty(bulkIds))
        {
            bool customsave_hasissues = false;
            var propertyKeyValue = properties.FirstOrDefault();
            foreach(var id in bulkId)
            {
                long objId = long.Parse(id);
                T_ExportDataConfiguration target = db.T_ExportDataConfigurations.Find(objId);
                target.setDateTimeToClientTime();
                customsave_hasissues = false;
                errorMsg += CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                    }
                    catch(Exception ex)
                    {
                        var errors = "";
                        foreach(var error in ValidateModelWithErrors(target))
                        {
                            errors += error.ErrorMessage + ".  ";
                        }
                        errorMsg += errors;
                        var res = new { Result = "fail", data = errorMsg };
                        db.Entry(target).State = EntityState.Detached;
                    }
                }
                else
                {
                    db.Entry(target).State = EntityState.Detached;
                }
            }
        }
        if(string.IsNullOrEmpty(errorMsg))
        {
            var result = new { Result = "Success", data = errorMsg };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var res = new { Result = "fail", data = errorMsg };
            return Json(res, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
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
        if(!User.CanEdit("T_ExportDataConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
        if(t_exportdataconfiguration == null)
        {
            db.DisableFilter("IsDeleted");
            t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
            if(t_exportdataconfiguration == null)
                return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var resultBefore = ApplyBusinessRuleBefore(t_exportdataconfiguration, true);
        if(resultBefore.Data != null)
        {
            var result = new { Result = "fail", data = resultBefore.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule section
        var strChkBeforeSave = CheckBeforeSave(t_exportdataconfiguration, "SaveProperty");
        if(!string.IsNullOrEmpty(strChkBeforeSave))
        {
            var result = new { Result = "fail", data = strChkBeforeSave };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var propertyInfo = t_exportdataconfiguration.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            t_exportdataconfiguration.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(t_exportdataconfiguration, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        var resultOnSaving = ApplyBusinessRuleOnSaving(t_exportdataconfiguration);
        if(resultOnSaving.Data != null)
        {
            var result = new { Result = "fail", data = resultOnSaving.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule onsaving section
        if(isSave && ValidateModel(t_exportdataconfiguration))
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_exportdataconfiguration, out customsave_hasissues, "Save"))
            {
                try
                {
                    db.Entry(t_exportdataconfiguration).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    var errors = "";
                    foreach(var error in ValidateModelWithErrors(t_exportdataconfiguration))
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                    var res = new { Result = "fail", data = errors };
                    db.Configuration.LazyLoadingEnabled = true;
                    return Json(res, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            var result = new { Result = "Success", data = value };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(t_exportdataconfiguration))
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_ExportDataConfiguration", User) || !User.CanDelete("T_ExportDataConfiguration")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_ExportDataConfiguration t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(id);
            if(t_exportdataconfiguration != null)
            {
                if(CheckBeforeDelete(t_exportdataconfiguration))
                {
                    if(!CustomDelete(t_exportdataconfiguration, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_exportdataconfiguration).State = EntityState.Deleted;
                        db.T_ExportDataConfigurations.Remove(t_exportdataconfiguration);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_ExportDataConfiguration", User) || !User.CanEdit("T_ExportDataConfiguration") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_ExportDataConfiguration", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdataconfiguration">  The T_ExportDataConfiguration object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_Name,T_EntityName,T_AllowedRoles,T_Description,T_Disable,T_BackGroundColor,T_FontColor,T_ToolTip")] T_ExportDataConfiguration t_exportdataconfiguration, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_ExportDataConfiguration target = db.T_ExportDataConfigurations.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_exportdataconfiguration, target, chkUpdate);
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdataconfiguration, "BulkUpdate", "");
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
                T_ExportDataConfiguration t_exportdataconfiguration = ctx.T_ExportDataConfigurations.Find(Convert.ToInt64(recId));
                ctx.Entry(t_exportdataconfiguration).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue = ctx.T_ExportDataConfigurations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            T_ExportDataConfiguration t_exportdataconfiguration = ctx.T_ExportDataConfigurations.Find(Convert.ToInt64(recId));
            ctx.Entry(t_exportdataconfiguration).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue = ctx.T_ExportDataConfigurations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
        T_ExportDataConfiguration _t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(Convert.ToInt64(id));
        return View(_t_exportdataconfiguration);
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
            if(db != null && CacheHelper.NoCache("T_ExportDataConfiguration")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
