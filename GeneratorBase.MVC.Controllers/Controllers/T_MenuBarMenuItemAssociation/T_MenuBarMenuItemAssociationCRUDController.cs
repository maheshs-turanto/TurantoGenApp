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
/// <summary>A partial controller class for Menu Bar Menu Item Association actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class T_MenuBarMenuItemAssociationController : BaseController
{

    /// <summary>Menu Bar Menu Item Association Index Action, renders items in different UI format based on viewtype</summary>
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
    public ActionResult Index(T_MenuBarMenuItemAssociationIndexArgsOption args)
    {
        //IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        IndexViewBag(args);
        T_MenuBarMenuItemAssociationIndexViewModel model = new T_MenuBarMenuItemAssociationIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, model.HostingEntityID, model.AssociatedType);
        var lstT_MenuBarMenuItemAssociation = from s in db.T_MenuBarMenuItemAssociations
                                              select s;
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_MenuBarMenuItemAssociation = searchRecords(lstT_MenuBarMenuItemAssociation, model.searchString.ToUpper(), model.IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_MenuBarMenuItemAssociation = sortRecords(lstT_MenuBarMenuItemAssociation, model.sortBy, model.IsAsc);
        }
        else lstT_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation.OrderByDescending(c => c.Id);
        lstT_MenuBarMenuItemAssociation = CustomSorting(lstT_MenuBarMenuItemAssociation,model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc, model.CustomParameter);
        model = (T_MenuBarMenuItemAssociationIndexViewModel)SetPagination(model, "T_MenuBarMenuItemAssociation");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        // for Restrict Dropdown
        ViewBag.T_MenuBarMenuItemAssociationRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_MenuBarMenuItemAssociation", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var ExportData = db.T_ExportDataConfigurations.Where(p => p.T_EntityName == "T_MenuBarMenuItemAssociation" && !p.T_Disable.Value).OrderBy(p => p.Id).ToList();
        ViewBag.ExportDataTemplates = ExportData.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        //
        var _T_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation;
        _T_MenuBarMenuItemAssociation = FilterByHostingEntity(_T_MenuBarMenuItemAssociation, model.HostingEntity, model.AssociatedType, model.HostingEntityID);
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_MenuBarMenuItemAssociation.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_MenuBarMenuItemAssociation);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_MenuBarMenuItemAssociation.Count();
                if(model.BulkOperation != null && model.BulkAssociate != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
                {
                    totalListCount = lstT_MenuBarMenuItemAssociation.Except(_T_MenuBarMenuItemAssociation).Count();
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
        TempData["T_MenuBarMenuItemAssociationlist"] = null;
        if(!(model.RenderPartial == null ? false : model.RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_MenuBarMenuItemAssociationlist"] = list.Select(z => new
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
                return DoBulkOperations(model, _T_MenuBarMenuItemAssociation, lstT_MenuBarMenuItemAssociation);
            }
            else
            {
                if(model.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(model.isMobileHome))
                    {
                        model.PageSize = _T_MenuBarMenuItemAssociation.Count() == 0 ? 1 : _T_MenuBarMenuItemAssociation.Count();
                    }
                    ViewData["HomePartialList"] = model.IsHomeList;
                    var list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, Convert.ToBoolean(model.IsHomeList) ? 5 : model.PageSize);
                    ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_MenuBarMenuItemAssociationlist"] = list.Select(z => new
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
                        model.PageSize = _T_MenuBarMenuItemAssociation.Count() == 0 ? 1 : _T_MenuBarMenuItemAssociation.Count();
                    }
                    var list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
                    ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_MenuBarMenuItemAssociationlist"] = list.Select(z => new
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
        T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(id);
        if(t_menubarmenuitemassociation == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User,"T_MenuBarMenuItemAssociation",defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_menubarmenuitemassociation);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_menubarmenuitemassociation, AssociatedType);
        Dictionary<string, string> dict = GetLockBusinessRulesDictionary(User, db, t_menubarmenuitemassociation, "T_MenuBarMenuItemAssociation");
        if(dict != null && dict.Count > 0)
        {
            string lockerror = dict.FirstOrDefault().Key;
            if(!lockerror.ToLower().Contains("informationmessage"))
                ViewData["LockRecordMsg"] = dict.FirstOrDefault().Value;
        }
        Dictionary<string, string> dictVerbHidden = GetHiddenVerbDetails(t_menubarmenuitemassociation, "OnEdit", null);
        if(dictVerbHidden != null && dictVerbHidden.Count > 0)
        {
            ViewData["VerbHiddenForDetails"] = dictVerbHidden;
        }
        return View(ViewBag.TemplatesName,t_menubarmenuitemassociation);
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
        if(!User.CanAdd("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User,"T_MenuBarMenuItemAssociation",viewtype);
        ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
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
        if(!User.CanAdd("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
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
    /// <param name="t_menubarmenuitemassociationObj">        The T_MenuBarMenuItemAssociation object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_MenuBarID,T_MenuItemID,T_OrderNumber")] T_MenuBarMenuItemAssociation t_menubarmenuitemassociationObj, List<string> T_MenuBarIDSelected, List<string> T_MenuBarIDAvailable, List<string> T_MenuItemIDSelected, List<string> T_MenuItemIDAvailable,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID,string BulkAddDropDown = null)
    {
        CheckBeforeSave(t_menubarmenuitemassociationObj);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult=CustomSaveOnCreate(t_menubarmenuitemassociationObj,out customcreate_hasissues,"Create");
            if(!customcreate_hasissues && !createResult)
            {
                string path = Server.MapPath("~/Files/");
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(T_MenuBarIDSelected != null || T_MenuBarIDAvailable != null)
                {
                    t_menubarmenuitemassociationObj.t_menuitem = db.T_MenuItems.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuItemID);
                    var T_MenuBarIDSelectedlong =T_MenuBarIDSelected !=null ? T_MenuBarIDSelected.Select(long.Parse).ToList():new List<long>();
                    var deletedItems = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuItemID == t_menubarmenuitemassociationObj.T_MenuItemID && !T_MenuBarIDSelectedlong.Contains(p.T_MenuBarID.Value)).ToList();
                    foreach(var item in deletedItems)
                    {
                        if(item.t_menuitem !=null)
                            db.Entry(item.t_menuitem).State = EntityState.Unchanged;
                        if(item.t_menubar !=null)
                            db.Entry(item.t_menubar).State = EntityState.Unchanged;
                        db.T_MenuBarMenuItemAssociations.Remove(item);
                        db.SaveChanges();
                    }
                    if(T_MenuBarIDSelected != null)
                    {
                        var T_MenuBarIDSelectedLong = T_MenuBarIDSelected.Select(Int64.Parse).ToList();
                        var alreadyregistered = db.T_MenuBarMenuItemAssociations.Where(a => a.T_MenuItemID == t_menubarmenuitemassociationObj.T_MenuItemID && T_MenuBarIDSelectedLong.Contains(a.T_MenuBarID.Value)).Select(p => p.T_MenuBarID.Value).ToList();
                        T_MenuBarIDSelectedLong = T_MenuBarIDSelectedLong.Except(alreadyregistered).ToList();
                        if(T_MenuBarIDSelectedLong != null)
                            foreach(var longid in T_MenuBarIDSelectedLong)
                            {
                                {
                                    var obj = new T_MenuBarMenuItemAssociation();
                                    obj = t_menubarmenuitemassociationObj;
                                    obj.T_MenuBarID = longid;//Convert.ToInt64(id);
                                    db.Entry(obj).State = EntityState.Added;
                                    //obj.t_menubar = db.T_MenuBars.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuBarID);
                                    if(obj.t_menuitem !=null)
                                        db.Entry(obj.t_menuitem).State = EntityState.Unchanged;
                                    if(obj.t_menubar !=null)
                                        db.Entry(obj.t_menubar).State = EntityState.Unchanged;
                                    //
                                    //
                                    db.T_MenuBarMenuItemAssociations.Add(obj);
                                    db.SaveChanges();
                                }
                            }
                    }
                }
                if(T_MenuItemIDSelected != null || T_MenuItemIDAvailable != null)
                {
                    t_menubarmenuitemassociationObj.t_menubar = db.T_MenuBars.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuBarID);
                    var T_MenuItemIDSelectedlong =T_MenuItemIDSelected !=null ? T_MenuItemIDSelected.Select(long.Parse).ToList():new List<long>();
                    var deletedItems = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuBarID == t_menubarmenuitemassociationObj.T_MenuBarID && !T_MenuItemIDSelectedlong.Contains(p.T_MenuItemID.Value)).ToList();
                    foreach(var item in deletedItems)
                    {
                        if(item.t_menubar !=null)
                            db.Entry(item.t_menubar).State = EntityState.Unchanged;
                        if(item.t_menuitem !=null)
                            db.Entry(item.t_menuitem).State = EntityState.Unchanged;
                        db.T_MenuBarMenuItemAssociations.Remove(item);
                        db.SaveChanges();
                    }
                    if(T_MenuItemIDSelected != null)
                    {
                        var T_MenuItemIDSelectedLong = T_MenuItemIDSelected.Select(Int64.Parse).ToList();
                        var alreadyregistered = db.T_MenuBarMenuItemAssociations.Where(a => a.T_MenuBarID == t_menubarmenuitemassociationObj.T_MenuBarID && T_MenuItemIDSelectedLong.Contains(a.T_MenuItemID.Value)).Select(p => p.T_MenuItemID.Value).ToList();
                        T_MenuItemIDSelectedLong = T_MenuItemIDSelectedLong.Except(alreadyregistered).ToList();
                        if(T_MenuItemIDSelectedLong != null)
                            foreach(var longid in T_MenuItemIDSelectedLong)
                            {
                                {
                                    var obj = new T_MenuBarMenuItemAssociation();
                                    obj = t_menubarmenuitemassociationObj;
                                    obj.T_MenuItemID = longid;//Convert.ToInt64(id);
                                    db.Entry(obj).State = EntityState.Added;
                                    //obj.t_menuitem = db.T_MenuItems.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuItemID);
                                    if(obj.t_menubar !=null)
                                        db.Entry(obj.t_menubar).State = EntityState.Unchanged;
                                    if(obj.t_menuitem !=null)
                                        db.Entry(obj.t_menuitem).State = EntityState.Unchanged;
                                    //
                                    //
                                    db.T_MenuBarMenuItemAssociations.Add(obj);
                                    db.SaveChanges();
                                }
                            }
                    }
                }
                return Json(new {result = "FROMPOPUP", output = t_menubarmenuitemassociationObj.Id, editurl=Url.Action("Edit",new { id= t_menubarmenuitemassociationObj.Id })}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(t_menubarmenuitemassociationObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_menubarmenuitemassociationObj, AssociatedEntity);
        return View(t_menubarmenuitemassociationObj);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociationObj">The T_MenuBarMenuItemAssociation object.</param>
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
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_MenuBarID,T_MenuItemID,T_OrderNumber")] T_MenuBarMenuItemAssociation t_menubarmenuitemassociationObj, List<string> T_MenuBarIDSelected, List<string> T_MenuBarIDAvailable, List<string> T_MenuItemIDSelected, List<string> T_MenuItemIDAvailable, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_menubarmenuitemassociationObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            bool createResult= CustomSaveOnCreate(t_menubarmenuitemassociationObj,out customcreate_hasissues,command);
            if(!customcreate_hasissues && !createResult)
            {
                string path = Server.MapPath("~/Files/");
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                if(T_MenuBarIDSelected != null || T_MenuBarIDAvailable != null)
                {
                    t_menubarmenuitemassociationObj.t_menuitem = db.T_MenuItems.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuItemID);
                    var T_MenuBarIDSelectedlong =T_MenuBarIDSelected !=null ? T_MenuBarIDSelected.Select(long.Parse).ToList():new List<long>();
                    var deletedItems = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuItemID == t_menubarmenuitemassociationObj.T_MenuItemID && !T_MenuBarIDSelectedlong.Contains(p.T_MenuBarID.Value)).ToList();
                    foreach(var item in deletedItems)
                    {
                        if(item.t_menuitem !=null)
                            db.Entry(item.t_menuitem).State = EntityState.Unchanged;
                        if(item.t_menubar !=null)
                            db.Entry(item.t_menubar).State = EntityState.Unchanged;
                        db.Entry(item).State = EntityState.Deleted;
                        db.T_MenuBarMenuItemAssociations.Remove(item);
                        db.SaveChanges();
                    }
                    if(T_MenuBarIDSelected != null)
                    {
                        var T_MenuBarIDSelectedLong = T_MenuBarIDSelected.Select(Int64.Parse).ToList();
                        var alreadyregistered = db.T_MenuBarMenuItemAssociations.Where(a => a.T_MenuItemID == t_menubarmenuitemassociationObj.T_MenuItemID && T_MenuBarIDSelectedLong.Contains(a.T_MenuBarID.Value)).Select(p => p.T_MenuBarID.Value).ToList();
                        T_MenuBarIDSelectedLong = T_MenuBarIDSelectedLong.Except(alreadyregistered).ToList();
                        if(T_MenuBarIDSelectedLong != null)
                            foreach(var longid in T_MenuBarIDSelectedLong)
                            {
                                {
                                    var obj = new T_MenuBarMenuItemAssociation();
                                    obj = t_menubarmenuitemassociationObj;
                                    obj.T_MenuBarID = longid;
                                    db.Entry(obj).State = EntityState.Added;
                                    //obj.t_menubar = db.T_MenuBars.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuBarID);
                                    if(obj.t_menuitem !=null)
                                        db.Entry(obj.t_menuitem).State = EntityState.Unchanged;
                                    if(obj.t_menubar !=null)
                                        db.Entry(obj.t_menubar).State = EntityState.Unchanged;
                                    //
                                    //
                                    db.T_MenuBarMenuItemAssociations.Add(obj);
                                    db.SaveChanges();
                                }
                            }
                    }
                }
                if(T_MenuItemIDSelected != null || T_MenuItemIDAvailable != null)
                {
                    t_menubarmenuitemassociationObj.t_menubar = db.T_MenuBars.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuBarID);
                    var T_MenuItemIDSelectedlong =T_MenuItemIDSelected !=null ? T_MenuItemIDSelected.Select(long.Parse).ToList():new List<long>();
                    var deletedItems = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuBarID == t_menubarmenuitemassociationObj.T_MenuBarID && !T_MenuItemIDSelectedlong.Contains(p.T_MenuItemID.Value)).ToList();
                    foreach(var item in deletedItems)
                    {
                        if(item.t_menubar !=null)
                            db.Entry(item.t_menubar).State = EntityState.Unchanged;
                        if(item.t_menuitem !=null)
                            db.Entry(item.t_menuitem).State = EntityState.Unchanged;
                        db.Entry(item).State = EntityState.Deleted;
                        db.T_MenuBarMenuItemAssociations.Remove(item);
                        db.SaveChanges();
                    }
                    if(T_MenuItemIDSelected != null)
                    {
                        var T_MenuItemIDSelectedLong = T_MenuItemIDSelected.Select(Int64.Parse).ToList();
                        var alreadyregistered = db.T_MenuBarMenuItemAssociations.Where(a => a.T_MenuBarID == t_menubarmenuitemassociationObj.T_MenuBarID && T_MenuItemIDSelectedLong.Contains(a.T_MenuItemID.Value)).Select(p => p.T_MenuItemID.Value).ToList();
                        T_MenuItemIDSelectedLong = T_MenuItemIDSelectedLong.Except(alreadyregistered).ToList();
                        if(T_MenuItemIDSelectedLong != null)
                            foreach(var longid in T_MenuItemIDSelectedLong)
                            {
                                {
                                    var obj = new T_MenuBarMenuItemAssociation();
                                    obj = t_menubarmenuitemassociationObj;
                                    obj.T_MenuItemID = longid;
                                    db.Entry(obj).State = EntityState.Added;
                                    //obj.t_menuitem = db.T_MenuItems.FirstOrDefault(p => p.Id == t_menubarmenuitemassociationObj.T_MenuItemID);
                                    if(obj.t_menubar !=null)
                                        db.Entry(obj.t_menubar).State = EntityState.Unchanged;
                                    if(obj.t_menuitem !=null)
                                        db.Entry(obj.t_menuitem).State = EntityState.Unchanged;
                                    //
                                    //
                                    db.T_MenuBarMenuItemAssociations.Add(obj);
                                    db.SaveChanges();
                                }
                            }
                    }
                }
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_menubarmenuitemassociationObj,"Create",command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode=="wizard")
                    return RedirectToAction("Edit", new { Id = t_menubarmenuitemassociationObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_menubarmenuitemassociationObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_menubarmenuitemassociationObj.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep });
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
        LoadViewDataAfterOnCreate(t_menubarmenuitemassociationObj);
        ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(t_menubarmenuitemassociationObj);
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
        if(!User.CanEdit("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(id);
        if(t_menubarmenuitemassociation == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
        if(ViewData["T_MenuBarMenuItemAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation/Edit/" + t_menubarmenuitemassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation/Create"))
            ViewData["T_MenuBarMenuItemAssociationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(t_menubarmenuitemassociation);
        var objT_MenuBarMenuItemAssociation = new List<T_MenuBarMenuItemAssociation>();
        ViewBag.T_MenuBarMenuItemAssociationDD = new SelectList(objT_MenuBarMenuItemAssociation, "ID", "DisplayValue");
        return View(t_menubarmenuitemassociation);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociation">       The T_MenuBarMenuItemAssociation object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_MenuBarID,T_MenuItemID,T_OrderNumber")] T_MenuBarMenuItemAssociation t_menubarmenuitemassociation,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_menubarmenuitemassociation = UpdateHiddenProperties(t_menubarmenuitemassociation, cannotviewproperties);
        CheckBeforeSave(t_menubarmenuitemassociation, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult= CustomSaveOnEdit(t_menubarmenuitemassociation,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                string path = Server.MapPath("~/Files/");
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                db.Entry(t_menubarmenuitemassociation).State = EntityState.Modified;
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
        if(db.Entry(t_menubarmenuitemassociation).State == EntityState.Detached)
            t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(t_menubarmenuitemassociation.Id);
        LoadViewDataAfterOnEdit(t_menubarmenuitemassociation);
        return View(t_menubarmenuitemassociation);
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
        if(!User.CanEdit("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(id);
        if(t_menubarmenuitemassociation == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_MenuBarMenuItemAssociationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_MenuBarMenuItemAssociations.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = TempData["T_MenuBarMenuItemAssociationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_MenuBarMenuItemAssociationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User,"T_MenuBarMenuItemAssociation",defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_menubarmenuitemassociation.DisplayValue, Value = t_menubarmenuitemassociation.Id.ToString() }));
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = newList;
            TempData["T_MenuBarMenuItemAssociationlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_menubarmenuitemassociation.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_menubarmenuitemassociation.DisplayValue;
                newList[0].Value = t_menubarmenuitemassociation.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_menubarmenuitemassociation.DisplayValue, Value = t_menubarmenuitemassociation.Id.ToString() }));
            }
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = newList;
            TempData["T_MenuBarMenuItemAssociationlist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
        if(ViewData["T_MenuBarMenuItemAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation/Edit/" + t_menubarmenuitemassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation/Create"))
            ViewData["T_MenuBarMenuItemAssociationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step"+wizardstep;
        LoadViewDataBeforeOnEdit(t_menubarmenuitemassociation);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,t_menubarmenuitemassociation);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociation">    The T_MenuBarMenuItemAssociation object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_MenuBarID,T_MenuItemID,T_OrderNumber")] T_MenuBarMenuItemAssociation t_menubarmenuitemassociation,  string UrlReferrer, bool RenderPartial = false, string viewmode="edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"],out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) t_menubarmenuitemassociation = UpdateHiddenProperties(t_menubarmenuitemassociation, cannotviewproperties);
        CheckBeforeSave(t_menubarmenuitemassociation, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            bool editResult=CustomSaveOnEdit(t_menubarmenuitemassociation,out customsave_hasissues,command);
            if(!customsave_hasissues && !editResult)
            {
                string path = Server.MapPath("~/Files/");
                string ticks =  DateTime.UtcNow.Ticks.ToString();
                db.Entry(t_menubarmenuitemassociation).State = EntityState.Modified;
                db.SaveChanges();
                if(!customsave_hasissues)
                {
                    RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_menubarmenuitemassociation,"Edit",command, viewmode);
                    if(customRedirectAction != null) return customRedirectAction;
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_menubarmenuitemassociation.Id };
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
                            return RedirectToAction("Edit", new { Id = t_menubarmenuitemassociation.Id, UrlReferrer = UrlReferrer, viewmode=viewmode, wizardstep=wizardstep  });
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
        if(TempData["T_MenuBarMenuItemAssociationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_MenuBarMenuItemAssociations.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = TempData["T_MenuBarMenuItemAssociationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_MenuBarMenuItemAssociationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(t_menubarmenuitemassociation).State == EntityState.Detached)
            t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(t_menubarmenuitemassociation.Id);
        LoadViewDataAfterOnEdit(t_menubarmenuitemassociation);
        ViewData["T_MenuBarMenuItemAssociationParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(t_menubarmenuitemassociation);
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
        if(!User.CanDelete("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(id);
        if(t_menubarmenuitemassociation == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_MenuBarMenuItemAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_MenuBarMenuItemAssociation"))
            ViewData["T_MenuBarMenuItemAssociationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(t_menubarmenuitemassociation);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociation">  The T_MenuBarMenuItemAssociation object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, string UrlReferrer)
    {
        if(!User.CanDelete("T_MenuBarMenuItemAssociation"))
        {
            return RedirectToAction("Index", "Error");
        }
        t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(t_menubarmenuitemassociation.Id);
        if(CheckBeforeDelete(t_menubarmenuitemassociation))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_menubarmenuitemassociation, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_menubarmenuitemassociation).State = EntityState.Deleted;
                //db.T_MenuBarMenuItemAssociations.Remove(t_menubarmenuitemassociation); //issue delete with userbased security
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_MenuBarMenuItemAssociationParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_MenuBarMenuItemAssociationParentUrl"].ToString();
                    ViewData["T_MenuBarMenuItemAssociationParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_menubarmenuitemassociation);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_MenuBarMenuItemAssociation", User) || !User.CanDelete("T_MenuBarMenuItemAssociation")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(id);
            if(t_menubarmenuitemassociation != null)
            {
                if(CheckBeforeDelete(t_menubarmenuitemassociation))
                {
                    if(!CustomDelete(t_menubarmenuitemassociation, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_menubarmenuitemassociation).State = EntityState.Deleted;
                        db.T_MenuBarMenuItemAssociations.Remove(t_menubarmenuitemassociation);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_MenuBarMenuItemAssociation", User) || !User.CanEdit("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("T_MenuBarMenuItemAssociation", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociation">  The T_MenuBarMenuItemAssociation object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,T_MenuBarID,T_MenuItemID,T_OrderNumber")] T_MenuBarMenuItemAssociation t_menubarmenuitemassociation,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_MenuBarMenuItemAssociation target = db.T_MenuBarMenuItemAssociations.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_menubarmenuitemassociation, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target,"BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        if(target.t_menubar != null)
                            db.Entry(target.t_menubar).State = EntityState.Unchanged;
                        if(target.t_menuitem != null)
                            db.Entry(target.t_menuitem).State = EntityState.Unchanged;
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_menubarmenuitemassociation,"BulkUpdate","");
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
                T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = ctx.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(recId));
                ctx.Entry(t_menubarmenuitemassociation).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue= ctx.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
            T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = ctx.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(recId));
            ctx.Entry(t_menubarmenuitemassociation).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue= ctx.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
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
        T_MenuBarMenuItemAssociation _t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(id));
        return View(_t_menubarmenuitemassociation);
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
            if(db!=null && CacheHelper.NoCache("T_MenuBarMenuItemAssociation")) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
