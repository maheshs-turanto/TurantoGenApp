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
/// <summary>A partial controller class for Export Data Log actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class T_ExportDataLogController : BaseController
{

    [Audit("ViewList")]
    /// <summary>Export Data Log Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(T_ExportDataLogIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        T_ExportDataLogIndexViewModel model = new T_ExportDataLogIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstT_ExportDataLog = from s in db.T_ExportDataLogs
                                 select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_ExportDataLog = searchRecords(lstT_ExportDataLog, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_ExportDataLog = sortRecords(lstT_ExportDataLog, model.sortBy, model.IsAsc);
        }
        else lstT_ExportDataLog = lstT_ExportDataLog.OrderByDescending(c => c.Id);
        lstT_ExportDataLog = CustomSorting(lstT_ExportDataLog,model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter);
        model = (T_ExportDataLogIndexViewModel)SetPagination(model, "T_ExportDataLog");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.T_ExportDataLogRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_ExportDataLog", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        var _T_ExportDataLog = lstT_ExportDataLog;
        _T_ExportDataLog = FilterByHostingEntity(_T_ExportDataLog, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_ExportDataLog.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_ExportDataLog);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_ExportDataLog.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstT_ExportDataLog.Except(_T_ExportDataLog).Count();
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
        TempData["T_ExportDataLoglist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_ExportDataLog.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_ExportDataLogDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_ExportDataLoglist"] = list.Select(z => new
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
                return DoBulkOperations(model, _T_ExportDataLog, lstT_ExportDataLog);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _T_ExportDataLog.Count() == 0 ? 1 : _T_ExportDataLog.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _T_ExportDataLog.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityT_ExportDataLogDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataLoglist"] = list.Select(z => new
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
                        model.PageSize = _T_ExportDataLog.Count() == 0 ? 1 : _T_ExportDataLog.Count();
                    }
                    var list = _T_ExportDataLog.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityT_ExportDataLogDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_ExportDataLoglist"] = list.Select(z => new
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
    [Audit("View")]
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
        T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
        if(t_exportdatalog == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "T_ExportDataLog" && p.Type != "View" && p.RecordId == id).ToList();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User,"T_ExportDataLog",defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_exportdatalog);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_exportdatalog, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, t_exportdatalog, "T_ExportDataLog");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = GetHiddenVerbDetails(t_exportdatalog, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName,t_exportdatalog);
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
        if(!User.CanAdd("T_ExportDataLog") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User,"T_ExportDataLog",viewtype);
        ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_ExportDataLog") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
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
    /// <param name="t_exportdatalogObj">The T_ExportDataLog object.</param>
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
    public ActionResult Create([Bind(Include= "Id,ConcurrencyKey,T_AutoNo,T_ExportDataConfigurationExportDataLogAssociationID,T_Tag,T_Notes,T_AssociatedExportDataLogStatusID,T_OneDriveUrl,T_GoogleDriveUrl")] T_ExportDataLog t_exportdatalogObj, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_exportdatalogObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult= CustomSaveOnCreate(t_exportdatalogObj,out customcreate_hasissues,command);
            if(!customcreate_hasissues && !createResult)
            {
                db.T_ExportDataLogs.Add(t_exportdatalogObj);
                db.SaveChanges();
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalogObj,"Create",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode=="wizard")
                    return RedirectToAction("Edit", new { Id = t_exportdatalogObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_exportdatalogObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_exportdatalogObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
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
        LoadViewDataAfterOnCreate(t_exportdatalogObj);
        ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdatalogObj);
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
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype, bool RecordReadOnly= false)
    {
        if(!User.CanEdit("T_ExportDataLog") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
        if(t_exportdatalog == null)
        {
            return HttpNotFound();
        }
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "T_ExportDataLog" && p.Type != "View" && p.RecordId == id).ToList();
        }
        if(UrlReferrer != null)
            ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
        if(ViewData["T_ExportDataLogParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog/Edit/" + t_exportdatalog.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog/Create"))
            ViewData["T_ExportDataLogParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(t_exportdatalog);
        var objT_ExportDataLog = new List<T_ExportDataLog>();
        ViewBag.T_ExportDataLogDD = new SelectList(objT_ExportDataLog, "ID", "DisplayValue");
        return View(t_exportdatalog);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalog">       The T_ExportDataLog object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_ExportDataConfigurationExportDataLogAssociationID,T_Tag,T_Notes,T_AssociatedExportDataLogStatusID")] T_ExportDataLog t_exportdatalog,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_exportdatalog = UpdateHiddenProperties(t_exportdatalog, cannotviewproperties);
        CheckBeforeSave(t_exportdatalog, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult= CustomSaveOnEdit(t_exportdatalog,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                db.Entry(t_exportdatalog).State = EntityState.Modified;
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
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "T_ExportDataLog" && p.Type != "View" && p.RecordId == t_exportdatalog.Id).ToList();
        }
        if(db.Entry(t_exportdatalog).State == EntityState.Detached)
            t_exportdatalog = db.T_ExportDataLogs.Find(t_exportdatalog.Id);
        LoadViewDataAfterOnEdit(t_exportdatalog);
        return View(t_exportdatalog);
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
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview,bool RecordReadOnly= false, bool RenderPartial = false, string viewmode="edit", string wizardstep="")
    {
        if(!User.CanEdit("T_ExportDataLog") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
        if(t_exportdatalog == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_ExportDataLoglist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataLogs.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataLogDisplayValueEdit = TempData["T_ExportDataLoglist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataLoglist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataLogDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "T_ExportDataLog" && p.Type != "View" && p.RecordId == id).ToList();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User,"T_ExportDataLog",defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_exportdatalog.DisplayValue, Value = t_exportdatalog.Id.ToString() }));
            ViewBag.EntityT_ExportDataLogDisplayValueEdit = newList;
            TempData["T_ExportDataLoglist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_exportdatalog.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_exportdatalog.DisplayValue;
                newList[0].Value = t_exportdatalog.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_exportdatalog.DisplayValue, Value = t_exportdatalog.Id.ToString() }));
            }
            ViewBag.EntityT_ExportDataLogDisplayValueEdit = newList;
            TempData["T_ExportDataLoglist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
        if(ViewData["T_ExportDataLogParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog/Edit/" + t_exportdatalog.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog/Create"))
            ViewData["T_ExportDataLogParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step"+wizardstep;
        LoadViewDataBeforeOnEdit(t_exportdatalog);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,t_exportdatalog);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalog">    The T_ExportDataLog object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include= "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_ExportDataConfigurationExportDataLogAssociationID,T_Tag,T_Notes,T_AssociatedExportDataLogStatusID,T_Summary,T_OneDriveUrl,T_GoogleDriveUrl")] T_ExportDataLog t_exportdatalog,  string UrlReferrer, bool RenderPartial = false, string viewmode="edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_exportdatalog = UpdateHiddenProperties(t_exportdatalog, cannotviewproperties);
        CheckBeforeSave(t_exportdatalog, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult=CustomSaveOnEdit(t_exportdatalog,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                db.Entry(t_exportdatalog).State = EntityState.Modified;
                db.SaveChanges();
                if(!customsave_hasissues)
                {
                    RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalog,"Edit",command, viewmode);
                    if(customRedirectAction != null) return customRedirectAction;
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_exportdatalog.Id };
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
                            return RedirectToAction("Edit", new { Id = t_exportdatalog.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep  });
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
        using(JournalEntryContext jedb = new JournalEntryContext())
        {
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "T_ExportDataLog" && p.Type != "View" && p.RecordId == t_exportdatalog.Id).ToList();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_ExportDataLoglist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_ExportDataLogs.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_ExportDataLogDisplayValueEdit = TempData["T_ExportDataLoglist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_ExportDataLoglist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_ExportDataLogDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_exportdatalog).State == EntityState.Detached)
            t_exportdatalog = db.T_ExportDataLogs.Find(t_exportdatalog.Id);
        LoadViewDataAfterOnEdit(t_exportdatalog);
        ViewData["T_ExportDataLogParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_exportdatalog);
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
        if(!User.CanDelete("T_ExportDataLog") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
        if(t_exportdatalog == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_ExportDataLogParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_ExportDataLog"))
            ViewData["T_ExportDataLogParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_exportdatalog);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalog">  The T_ExportDataLog object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_ExportDataLog t_exportdatalog, string UrlReferrer)
    {
        if(!User.CanDelete("T_ExportDataLog"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_exportdatalog = db.T_ExportDataLogs.Find(t_exportdatalog.Id);
        if(CheckBeforeDelete(t_exportdatalog))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_exportdatalog, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_exportdatalog).State = EntityState.Deleted;
                //db.T_ExportDataLogs.Remove(t_exportdatalog); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_ExportDataLogParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_ExportDataLogParentUrl"].ToString();
                    ViewData["T_ExportDataLogParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_exportdatalog);
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
            if(!User.CanEdit("T_ExportDataLog"))
            {
                return RedirectToAction("Index", "Error");
            }
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
            db.Configuration.LazyLoadingEnabled = false;
            T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Where(p=>p.Id == id).FirstOrDefault();
            if(t_exportdatalog == null)
            {
                if(t_exportdatalog == null)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    return HttpNotFound();
                }
            }
            // business rule before load section
            var resultBefore = ApplyBusinessRuleBefore(t_exportdatalog, true);
            if(resultBefore.Data != null)
            {
                var result = new { Result = "fail", data = resultBefore.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            var strChkBeforeSave = CheckBeforeSave(t_exportdatalog, "SaveProperty");
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                var result = new { Result = "fail", data = strChkBeforeSave };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            bool isSave = false;
            t_exportdatalog.setDateTimeToClientTime();
            foreach(var item in properties)
            {
                var propertyName = item.Key;
                var propertyValue = item.Value;
                var propertyInfo = t_exportdatalog.GetType().GetProperty(propertyName);
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
                        propertyInfo.SetValue(t_exportdatalog, safeValue, null);
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
            var resultOnSaving = ApplyBusinessRuleOnSaving(t_exportdatalog);
            if(resultOnSaving.Data != null && !IsConfirm.Value)
            {
                var result = new { Result = "fail", data = resultOnSaving.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            // business rule onsaving section
            if(isSave && ValidateModel(t_exportdatalog))
            {
                bool customsave_hasissues = false;
                if(!CustomSaveOnEdit(t_exportdatalog, out customsave_hasissues, "Save"))
                {
                    try
                    {
                        db.Entry(t_exportdatalog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        var errors = "";
                        foreach(var error in ValidateModelWithErrors(t_exportdatalog))
                        {
                            errors += error.ErrorMessage + ".  ";
                        }
                        var res = new { Result = "fail", data = errors };
                        db.Configuration.LazyLoadingEnabled = true;
                        return Json(res, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
                db.Configuration.LazyLoadingEnabled = true;
                var result = new { Result = "Success", data = t_exportdatalog.DisplayValue };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach(var error in ValidateModelWithErrors(t_exportdatalog))
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
        if(bulkId.Length > 0 && properties != null  && !string.IsNullOrEmpty(bulkIds))
        {
            bool customsave_hasissues = false;
            var propertyKeyValue = properties.FirstOrDefault();
            foreach(var id in bulkId)
            {
                long objId = long.Parse(id);
                T_ExportDataLog target = db.T_ExportDataLogs.Find(objId);
                target.setDateTimeToClientTime();
                customsave_hasissues = false;
                errorMsg += CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.T_AssociatedExportDataLogStatusID != long.Parse(propertyKeyValue.Value))
                        {
                            if(target.t_associatedexportdatalogstatus != null && propertyKeyValue.Key == "T_AssociatedExportDataLogStatusID")
                            {
                                target.T_AssociatedExportDataLogStatusID = long.Parse(propertyKeyValue.Value);
                                db.Entry(target.t_associatedexportdatalogstatus).State = EntityState.Unchanged;
                                db.SaveChanges();
                            }
                        }
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
        if(!User.CanEdit("T_ExportDataLog"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
        if(t_exportdatalog == null)
        {
            return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var resultBefore = ApplyBusinessRuleBefore(t_exportdatalog,true);
        if(resultBefore.Data != null)
        {
            var result = new { Result = "fail", data = resultBefore.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule section
        var strChkBeforeSave = CheckBeforeSave(t_exportdatalog, "SaveProperty");
        if(!string.IsNullOrEmpty(strChkBeforeSave))
        {
            var result = new { Result = "fail", data = strChkBeforeSave };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var propertyInfo = t_exportdatalog.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            t_exportdatalog.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(t_exportdatalog, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        var resultOnSaving = ApplyBusinessRuleOnSaving(t_exportdatalog);
        if(resultOnSaving.Data != null)
        {
            var result = new { Result = "fail", data = resultOnSaving.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule onsaving section
        if(isSave && ValidateModel(t_exportdatalog))
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_exportdatalog, out customsave_hasissues, "Save"))
            {
                try
                {
                    db.Entry(t_exportdatalog).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    var errors = "";
                    foreach(var error in ValidateModelWithErrors(t_exportdatalog))
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
            foreach(var error in ValidateModelWithErrors(t_exportdatalog))
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
        if(HostingEntity == "T_ExportDataConfiguration" && AssociatedType == "T_ExportDataConfigurationExportDataLogAssociation")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_ExportDataLog obj = db.T_ExportDataLogs.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_ExportDataConfigurationExportDataLogAssociationID = HostingID;
                db.SaveChanges();
            }
        }
        if(HostingEntity == "T_ExportDataLogstatus" && AssociatedType == "T_AssociatedExportDataLogStatus")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                T_ExportDataLog obj = db.T_ExportDataLogs.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.T_AssociatedExportDataLogStatusID = HostingID;
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_ExportDataLog", User) || !User.CanDelete("T_ExportDataLog")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_ExportDataLog t_exportdatalog = db.T_ExportDataLogs.Find(id);
            if(t_exportdatalog != null)
            {
                if(CheckBeforeDelete(t_exportdatalog))
                {
                    if(!CustomDelete(t_exportdatalog, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_exportdatalog).State = EntityState.Deleted;
                        db.T_ExportDataLogs.Remove(t_exportdatalog);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_ExportDataLog", User) || !User.CanEdit("T_ExportDataLog") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_ExportDataLog", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdatalog">  The T_ExportDataLog object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_AutoNo,T_ExportDataConfigurationExportDataLogAssociationID,T_Tag,T_Notes,T_AssociatedExportDataLogStatusID")] T_ExportDataLog t_exportdatalog,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_ExportDataLog target = db.T_ExportDataLogs.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_exportdatalog, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target,"BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.t_exportdataconfigurationexportdatalogassociation != null)
                            db.Entry(target.t_exportdataconfigurationexportdatalogassociation).State = EntityState.Unchanged;
                        if(target.t_associatedexportdatalogstatus != null)
                            db.Entry(target.t_associatedexportdatalogstatus).State = EntityState.Unchanged;
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_exportdatalog,"BulkUpdate","");
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
                T_ExportDataLog t_exportdatalog = ctx.T_ExportDataLogs.Find(Convert.ToInt64(recId));
                ctx.Entry(t_exportdatalog).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue= ctx.T_ExportDataLogs.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            T_ExportDataLog t_exportdatalog = ctx.T_ExportDataLogs.Find(Convert.ToInt64(recId));
            ctx.Entry(t_exportdatalog).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue= ctx.T_ExportDataLogs.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
        T_ExportDataLog _t_exportdatalog = db.T_ExportDataLogs.Find(Convert.ToInt64(id));
        return View(_t_exportdatalog);
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
            if(db!=null && CacheHelper.NoCache("T_ExportDataLog")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
