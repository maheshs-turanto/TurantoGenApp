using GeneratorBase.MVC.DynamicQueryable;
using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Property Validation and Format actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class PropertyValidationandFormatController : BaseController
{
    private PropertyValidationandFormatContext db1 = new PropertyValidationandFormatContext();
    /// <summary>Property Validation and Format Index Action, renders items in different UI format based on viewtype</summary>
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
        var lstPropertyValidationandFormat = from s in db1.PropertyValidationandFormats.AsNoTracking()
                                             select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstPropertyValidationandFormat = searchRecords(lstPropertyValidationandFormat, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(ViewBag.isAsc))
        {
            lstPropertyValidationandFormat = sortRecords(lstPropertyValidationandFormat, sortBy, ViewBag.isAsc);
        }
        else lstPropertyValidationandFormat = lstPropertyValidationandFormat.OrderByDescending(c => c.Id);
        lstPropertyValidationandFormat = CustomSorting(lstPropertyValidationandFormat, HostingEntity, AssociatedType, sortBy, ViewBag.isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        var _PropertyValidationandFormat = lstPropertyValidationandFormat;
        _PropertyValidationandFormat = FilterByHostingEntity(_PropertyValidationandFormat, HostingEntity, AssociatedType, HostingEntityID);
        if(Convert.ToBoolean(IsExport))
        {
            if(ExportType == "csv")
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportCSV", "PropertyValidationandFormat", User) || !User.CanView("PropertyValidationandFormat"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_PropertyValidationandFormat.Count() > 0)
                    pageSize = _PropertyValidationandFormat.Count();
                var csvdata = _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize);
                string[] allcolumns = new string[] { "EntityName", "PropertyName", "RegExPattern", "MaskPattern", "ErrorMessage", "LowerBound", "UpperBound", "DisplayFormat", "IsEnabled", "Other1", "Other2" };
                var viewcolumns = new List<string>();
                var entityfields = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == "PropertyValidationandFormat");
                foreach(var column in allcolumns)
                {
                    var assoInfo = entityfields.Associations.FirstOrDefault(fd => fd.AssociationProperty == column);
                    if(assoInfo != null)
                    {
                        if(User.CanView("PropertyValidationandFormat", column))
                            viewcolumns.Add(assoInfo.AssociationProperty);
                    }
                    else
                    {
                        var propInfo = entityfields.Properties.FirstOrDefault(fd => fd.Name == column);
                        if(propInfo != null)
                        {
                            if(User.CanView("PropertyValidationandFormat", column))
                                viewcolumns.Add(column);
                        }
                    }
                }
                return new CsvResult<PropertyValidationandFormat>(csvdata.ToList(), "Property Validation and Format.csv", viewcolumns.ToArray());
            }
            else
            {
                if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "PropertyValidationandFormat", User) || !User.CanView("PropertyValidationandFormat"))
                {
                    return RedirectToAction("Index", "Error");
                }
                pageNumber = 1;
                if(_PropertyValidationandFormat.Count() > 0)
                    pageSize = _PropertyValidationandFormat.Count();
                return View("ExcelExport", _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize));
            }
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _PropertyValidationandFormat.Count();
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
        ViewBag.TemplatesName = GetTemplatesForList(User, "PropertyValidationandFormat", viewtype);
        TempData["PropertyValidationandFormatlist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityPropertyValidationandFormatDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["PropertyValidationandFormatlist"] = list.Select(z => new
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
                    _PropertyValidationandFormat = _fad.FilterDropdown<PropertyValidationandFormat>(User, _PropertyValidationandFormat, "PropertyValidationandFormat", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstPropertyValidationandFormat.Except(_PropertyValidationandFormat), sortBy, isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstPropertyValidationandFormat.Except(_PropertyValidationandFormat).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _PropertyValidationandFormat.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _PropertyValidationandFormat.Count() == 0 ? 1 : _PropertyValidationandFormat.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _PropertyValidationandFormat.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityPropertyValidationandFormatDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["PropertyValidationandFormatlist"] = list.Select(z => new
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
                        pageSize = _PropertyValidationandFormat.Count() == 0 ? 1 : _PropertyValidationandFormat.Count();
                    }
                    var list = _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityPropertyValidationandFormatDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["PropertyValidationandFormatlist"] = list.Select(z => new
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
        PropertyValidationandFormat propertyvalidationandformat = db1.PropertyValidationandFormats.Find(id);
        if(propertyvalidationandformat == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "PropertyValidationandFormat", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(propertyvalidationandformat);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(propertyvalidationandformat, AssociatedType);
        return View(ViewBag.TemplatesName, propertyvalidationandformat);
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
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype, bool RenderPartial = false)
    {
        if(!User.CanAdd("PropertyValidationandFormat") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "PropertyValidationandFormat", viewtype);
        ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
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
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("PropertyValidationandFormat") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
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
    public ActionResult GlobalDateTimeSetting(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("PropertyValidationandFormat") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewBag.DateFormat = ConfigurationManager.AppSettings["DateFormat"];
        ViewBag.TimeFormat = ConfigurationManager.AppSettings["TimeFormat"];
        var TimeZone = ConfigurationManager.AppSettings["TimeZone"];
        if(TimeZone == null) TimeZone = "Default";
        var list = TimeZoneInfo.GetSystemTimeZones().Select(p => p.StandardName).OrderBy(p => p).ToList();
        list.Insert(0, "Default");
        ViewBag.ErrorMessage = new SelectList(list, TimeZone);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Create new record through model popup).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformatObj">        The PropertyValidationandFormat object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GlobalDateTimeSetting([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat")] PropertyValidationandFormat propertyvalidationandformatObj, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        myConfiguration.AppSettings.Settings["DateFormat"].Value = propertyvalidationandformatObj.RegExPattern;
        myConfiguration.AppSettings.Settings["TimeFormat"].Value = propertyvalidationandformatObj.MaskPattern;
        myConfiguration.AppSettings.Settings["TimeZone"].Value = propertyvalidationandformatObj.ErrorMessage;
        myConfiguration.Save();
        return Redirect(Convert.ToString(UrlReferrer));
    }
    
    /// <summary>(An Action that handles HTTP POST requests) Create new record through model popup).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformatObj">        The PropertyValidationandFormat object.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         Add popup.</param>
    /// <param name="AssociatedEntity"> Associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat,IsEnabled")] PropertyValidationandFormat propertyvalidationandformatObj, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(propertyvalidationandformatObj);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(propertyvalidationandformatObj, out customcreate_hasissues, "Create"))
            {
                db1.PropertyValidationandFormats.Add(propertyvalidationandformatObj);
                db1.SaveChanges();
            }
            if(!customcreate_hasissues)
                return Json(new { result = "FROMPOPUP", output = propertyvalidationandformatObj.Id }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(propertyvalidationandformatObj);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(propertyvalidationandformatObj, AssociatedEntity);
        return View(propertyvalidationandformatObj);
    }
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformatObj">The PropertyValidationandFormat object.</param>
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
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat,IsEnabled,Other1,Other2")] PropertyValidationandFormat propertyvalidationandformatObj, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(propertyvalidationandformatObj, command);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(propertyvalidationandformatObj, out customcreate_hasissues, command))
            {
                db1.PropertyValidationandFormats.Add(propertyvalidationandformatObj);
                db1.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(propertyvalidationandformatObj, "Create", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = propertyvalidationandformatObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = propertyvalidationandformatObj.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnCreate(propertyvalidationandformatObj);
        ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        return View(propertyvalidationandformatObj);
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
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("PropertyValidationandFormat") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyValidationandFormat propertyvalidationandformat = db1.PropertyValidationandFormats.Find(id);
        if(propertyvalidationandformat == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        if(ViewData["PropertyValidationandFormatParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat/Edit/" + propertyvalidationandformat.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat/Create"))
            ViewData["PropertyValidationandFormatParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(propertyvalidationandformat);
        var objPropertyValidationandFormat = new List<PropertyValidationandFormat>();
        ViewBag.PropertyValidationandFormatDD = new SelectList(objPropertyValidationandFormat, "ID", "DisplayValue");
        return View(propertyvalidationandformat);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformat">       The PropertyValidationandFormat object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat,IsEnabled,Other1,Other2")] PropertyValidationandFormat propertyvalidationandformat, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(propertyvalidationandformat, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(propertyvalidationandformat, out customsave_hasissues, command))
            {
                db1.Entry(propertyvalidationandformat).State = EntityState.Modified;
                db1.SaveChanges();
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
        if(db1.Entry(propertyvalidationandformat).State == EntityState.Detached)
            propertyvalidationandformat = db1.PropertyValidationandFormats.Find(propertyvalidationandformat.Id);
        LoadViewDataAfterOnEdit(propertyvalidationandformat);
        return View(propertyvalidationandformat);
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
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RenderPartial = false)
    {
        if(!User.CanEdit("PropertyValidationandFormat") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyValidationandFormat propertyvalidationandformat = db1.PropertyValidationandFormats.Find(id);
        if(propertyvalidationandformat == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["PropertyValidationandFormatlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db1.PropertyValidationandFormats.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = TempData["PropertyValidationandFormatlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["PropertyValidationandFormatlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "PropertyValidationandFormat", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = propertyvalidationandformat.DisplayValue, Value = propertyvalidationandformat.Id.ToString() }));
            ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = newList;
            TempData["PropertyValidationandFormatlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(propertyvalidationandformat.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = propertyvalidationandformat.DisplayValue;
                newList[0].Value = propertyvalidationandformat.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = propertyvalidationandformat.DisplayValue, Value = propertyvalidationandformat.Id.ToString() }));
            }
            ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = newList;
            TempData["PropertyValidationandFormatlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        if(ViewData["PropertyValidationandFormatParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat/Edit/" + propertyvalidationandformat.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat/Create"))
            ViewData["PropertyValidationandFormatParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(propertyvalidationandformat);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, propertyvalidationandformat);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformat">    The PropertyValidationandFormat object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat,IsEnabled,Other1,Other2")] PropertyValidationandFormat propertyvalidationandformat, string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(propertyvalidationandformat, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(propertyvalidationandformat, out customsave_hasissues, command))
            {
                db1.Entry(propertyvalidationandformat).State = EntityState.Modified;
                db1.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(propertyvalidationandformat, "Edit", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = propertyvalidationandformat.Id };
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
                        return RedirectToAction("Edit", new { Id = propertyvalidationandformat.Id, UrlReferrer = UrlReferrer });
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
        if(TempData["PropertyValidationandFormatlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db1.PropertyValidationandFormats.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = TempData["PropertyValidationandFormatlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["PropertyValidationandFormatlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityPropertyValidationandFormatDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db1.Entry(propertyvalidationandformat).State == EntityState.Detached)
            propertyvalidationandformat = db1.PropertyValidationandFormats.Find(propertyvalidationandformat.Id);
        LoadViewDataAfterOnEdit(propertyvalidationandformat);
        ViewData["PropertyValidationandFormatParentUrl"] = UrlReferrer;
        return View(propertyvalidationandformat);
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
        if(!User.CanDelete("PropertyValidationandFormat") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyValidationandFormat propertyvalidationandformat = db1.PropertyValidationandFormats.Find(id);
        if(propertyvalidationandformat == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["PropertyValidationandFormatParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyValidationandFormat"))
            ViewData["PropertyValidationandFormatParentUrl"] = Request.UrlReferrer;
        return View(propertyvalidationandformat);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformat">  The PropertyValidationandFormat object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(PropertyValidationandFormat propertyvalidationandformat, string UrlReferrer)
    {
        if(!User.CanDelete("PropertyValidationandFormat"))
        {
            return RedirectToAction("Index", "Error");
        }
        propertyvalidationandformat = db1.PropertyValidationandFormats.Find(propertyvalidationandformat.Id);
        if(CheckBeforeDelete(propertyvalidationandformat))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(propertyvalidationandformat, out customdelete_hasissues, "Delete"))
            {
                db1.Entry(propertyvalidationandformat).State = EntityState.Deleted;
                db1.PropertyValidationandFormats.Remove(propertyvalidationandformat);
                db1.SaveChanges();
            }
            var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            myConfiguration.AppSettings.Settings["PropertyValidationandFormat"].Value = Convert.ToString(DateTime.UtcNow.Ticks);
            myConfiguration.Save();
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["PropertyValidationandFormatParentUrl"] != null)
                {
                    string parentUrl = ViewData["PropertyValidationandFormatParentUrl"].ToString();
                    ViewData["PropertyValidationandFormatParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(propertyvalidationandformat);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "PropertyValidationandFormat", User) || !User.CanDelete("PropertyValidationandFormat")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            PropertyValidationandFormat propertyvalidationandformat = db1.PropertyValidationandFormats.Find(id);
            if(propertyvalidationandformat != null)
            {
                if(CheckBeforeDelete(propertyvalidationandformat))
                {
                    if(!CustomDelete(propertyvalidationandformat, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db1.Entry(propertyvalidationandformat).State = EntityState.Deleted;
                        db1.PropertyValidationandFormats.Remove(propertyvalidationandformat);
                        try
                        {
                            db1.SaveChanges();
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "PropertyValidationandFormat", User) || !User.CanEdit("PropertyValidationandFormat") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("PropertyValidationandFormat", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="propertyvalidationandformat">  The PropertyValidationandFormat object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,EntityName,PropertyName,RegExPattern,MaskPattern,ErrorMessage,LowerBound,UpperBound,DisplayFormat,IsEnabled,Other1,Other2")] PropertyValidationandFormat propertyvalidationandformat, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                PropertyValidationandFormat target = db1.PropertyValidationandFormats.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(propertyvalidationandformat, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db1.Entry(target).State = EntityState.Modified;
                    try
                    {
                        db1.SaveChanges();
                    }
                    catch
                    {
                        db1.Entry(target).State = EntityState.Detached;
                    }
                }
                else
                {
                    db1.Entry(target).State = EntityState.Detached;
                }
            }
        }
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(propertyvalidationandformat, "BulkUpdate", "");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(PropertyValidationandFormat propertyvalidationandformat, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    private void LoadViewDataAfterOnEdit(PropertyValidationandFormat propertyvalidationandformat)
    {
        LoadViewDataBeforeOnEdit(propertyvalidationandformat, false);
        CustomLoadViewDataListAfterEdit(propertyvalidationandformat);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="propertyvalidationandformat">         The Property Validation and Format.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(PropertyValidationandFormat propertyvalidationandformat, bool loadCustomViewData = true)
    {
        var EntityList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Properties.Any(q => !string.IsNullOrEmpty(q.RegExPattern) || !string.IsNullOrEmpty(q.DisplayFormat)));
        ViewBag.EntityName = new SelectList(EntityList, "Name", "DisplayName", propertyvalidationandformat.EntityName);
        if(!string.IsNullOrEmpty(propertyvalidationandformat.EntityName))
        {
            var list = EntityList.FirstOrDefault(p => p.Name == propertyvalidationandformat.EntityName).Properties.Where(p => !string.IsNullOrEmpty(p.RegExPattern) || !string.IsNullOrEmpty(p.DisplayFormat));
            ViewBag.PropertyName = new SelectList(list, "Name", "DisplayName", propertyvalidationandformat.PropertyName);
        }
        else
            ViewBag.PropertyName = new SelectList(new Dictionary<string, string>(), "Key", "Value");
        ViewBag.PropertyValidationandFormatIsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnEdit", false, null);
        ViewBag.PropertyValidationandFormatIsGroupsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnEdit", true, null);
        ViewBag.PropertyValidationandFormatIsSetValueUIRule = checkSetValueUIRule(User, "PropertyValidationandFormat", "OnEdit", new long[] { 6, 8 }, null, null);
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(propertyvalidationandformat);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    private void LoadViewDataAfterOnCreate(PropertyValidationandFormat propertyvalidationandformat)
    {
        ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Properties.Any(q => !string.IsNullOrEmpty(q.RegExPattern) || !string.IsNullOrEmpty(q.DisplayFormat))), "Name", "DisplayName");
        ViewBag.PropertyName = new SelectList(new Dictionary<string, string>(), "Key", "Value");
        CustomLoadViewDataListAfterOnCreate(propertyvalidationandformat);
        ViewBag.PropertyValidationandFormatIsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnCreate", false, null);
        ViewBag.PropertyValidationandFormatIsGroupsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnCreate", true, null);
        ViewBag.PropertyValidationandFormatIsSetValueUIRule = checkSetValueUIRule(User, "PropertyValidationandFormat", "OnCreate", new long[] { 6, 7 }, null, null);
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Properties.Any(q => !string.IsNullOrEmpty(q.RegExPattern) || !string.IsNullOrEmpty(q.DisplayFormat))), "Name", "DisplayName");
        ViewBag.PropertyName = new SelectList(new Dictionary<string, string>(), "Key", "Value");
        ViewBag.PropertyValidationandFormatIsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnCreate", false, null);
        ViewBag.PropertyValidationandFormatIsGroupsHiddenRule = checkHidden(User, "PropertyValidationandFormat", "OnCreate", true, null);
        ViewBag.PropertyValidationandFormatIsSetValueUIRule = checkSetValueUIRule(User, "PropertyValidationandFormat", "OnCreate", new long[] { 6, 7 }, null, null);
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertiesofEntity action.</returns>
    
    public JsonResult GetPropertiesofEntity(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var list = Ent.Properties.Where(p => !string.IsNullOrEmpty(p.RegExPattern) || !string.IsNullOrEmpty(p.DisplayFormat));
        return Json(list.ToList(), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertiesofEntity action.</returns>
    
    public JsonResult GetRegExofProperty(string Entity, string Property)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var obj = Ent.Properties.FirstOrDefault(p => p.Name == Property && (!string.IsNullOrEmpty(p.RegExPattern) || !string.IsNullOrEmpty(p.DisplayFormat)));
        return Json(new { mask = obj.MaskPattern, regex = obj.RegExPattern, displayformat = obj.DisplayFormat, uidisplayformat = obj.UIDisplayFormat }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstPropertyValidationandFormat">The list Property Validation and Format.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<PropertyValidationandFormat> searchRecords(IQueryable<PropertyValidationandFormat> lstPropertyValidationandFormat, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstPropertyValidationandFormat = lstPropertyValidationandFormat.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.MaskPattern) && s.MaskPattern.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.RegExPattern) && s.RegExPattern.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ErrorMessage) && s.ErrorMessage.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LowerBound) && s.LowerBound.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.UpperBound) && s.UpperBound.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayFormat) && s.DisplayFormat.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Other1) && s.Other1.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Other2) && s.Other2.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstPropertyValidationandFormat = lstPropertyValidationandFormat.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.MaskPattern) && s.MaskPattern.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ErrorMessage) && s.ErrorMessage.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LowerBound) && s.LowerBound.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.UpperBound) && s.UpperBound.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayFormat) && s.DisplayFormat.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        bool boolvalue;
        if(Boolean.TryParse(searchString, out boolvalue))
            lstPropertyValidationandFormat = lstPropertyValidationandFormat.Union(db1.PropertyValidationandFormats.Where(s => (s.IsEnabled == boolvalue)));
        return lstPropertyValidationandFormat;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstPropertyValidationandFormat">The IQueryable list Property Validation and Format.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<PropertyValidationandFormat> sortRecords(IQueryable<PropertyValidationandFormat> lstPropertyValidationandFormat, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstPropertyValidationandFormat.Sort(sortBy, true) : lstPropertyValidationandFormat.Sort(sortBy, false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(PropertyValidationandFormat), "propertyvalidationandformat");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<PropertyValidationandFormat>)lstPropertyValidationandFormat.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstPropertyValidationandFormat.ElementType, lambda.Body.Type },
                       lstPropertyValidationandFormat.Expression,
                       lambda));
    }
    /// <summary>Searches for the similar records (Match Making).</summary>
    ///
    /// <param name="id">          The identifier.</param>
    /// <param name="sourceEntity">Source entity.</param>
    ///
    /// <returns>A response stream to send to the FindFSearch View.</returns>
    public ActionResult FindFSearch(string id, string sourceEntity)
    {
        return null;
    }
    /// <summary>Renders UI to define faceted search.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="HostingEntity">The hosting entity.</param>
    /// <param name="RenderPartial">The render partial.</param>
    /// <param name="ShowDeleted">  (Optional) True to show, false to hide the deleted (when soft-delete is on).</param>
    ///
    /// <returns>A response stream to send to the SetFSearch View.</returns>
    public ActionResult SetFSearch(string searchString, string HostingEntity, bool? RenderPartial, bool ShowDeleted = false)
    {
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        if(Qcount > 0)
        {
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        }
        else
        {
        }
        SetFSearchViewBag("PropertyValidationandFormat");
        Dictionary<string, string> columns = new Dictionary<string, string>();
        columns.Add("2", "Entity Name");
        columns.Add("3", "Property Name");
        columns.Add("4", "RegEx Pattern");
        columns.Add("5", "Error Message");
        columns.Add("6", "Lower Bound");
        columns.Add("7", "Upper Bound");
        columns.Add("8", "Display Format");
        ViewBag.HideColumns = new MultiSelectList(columns, "Key", "Value");
        return View(new PropertyValidationandFormat());
    }
    
    /// <summary>Renders result of faceted search.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="currentFilter">  Specifying the current filter.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="FSFilter">       A filter specifying the file system.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="search">         The search.</param>
    /// <param name="IsExport">       Is export excel.</param>
    /// <param name="FilterCondition">The filter condition.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="AssociatedType"> Association name.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="SortOrder">      The sort order.</param>
    /// <param name="HideColumns">    The hide columns.</param>
    /// <param name="GroupByColumn">  The group by column.</param>
    /// <param name="IsReports">      Is reports.</param>
    /// <param name="IsdrivedTab">    Isdrived tab.</param>
    /// <param name="IsFilter">       (Optional) IsFilter.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted (when soft-delete is on).</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string LowerBoundFrom, string LowerBoundTo, string UpperBoundFrom, string UpperBoundTo, string IsEnabled, string FilterCondition, string HostingEntity, string AssociatedType, string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn, bool? IsReports, bool? IsdrivedTab, bool? IsFilter = false, bool ShowDeleted = false)
    {
        FSearchViewBag(currentFilter, searchString, FSFilter, sortBy, isAsc, page, itemsPerPage, search, FilterCondition, HostingEntity, AssociatedType, HostingEntityID, viewtype, SortOrder, HideColumns, GroupByColumn, IsReports, IsdrivedTab, IsFilter, "PropertyValidationandFormat");
        CustomLoadViewDataListOnIndex(HostingEntity, Convert.ToInt32(HostingEntityID), AssociatedType);
        var lstPropertyValidationandFormat = from s in db1.PropertyValidationandFormats.AsNoTracking()
                                             select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstPropertyValidationandFormat = searchRecords(lstPropertyValidationandFormat, searchString.ToUpper(), true);
        }
        if(!string.IsNullOrEmpty(search))
            search = search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search;
            lstPropertyValidationandFormat = searchRecords(lstPropertyValidationandFormat, search, true);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstPropertyValidationandFormat = sortRecords(lstPropertyValidationandFormat, sortBy, isAsc);
        }
        else lstPropertyValidationandFormat = lstPropertyValidationandFormat.OrderByDescending(c => c.Id);
        lstPropertyValidationandFormat = CustomSorting(lstPropertyValidationandFormat, HostingEntity, AssociatedType, sortBy, isAsc);
        lstPropertyValidationandFormat = lstPropertyValidationandFormat;
        if(!string.IsNullOrEmpty(FilterCondition))
        {
            StringBuilder whereCondition = new StringBuilder();
            var conditions = FilterCondition.Split("?".ToCharArray()).Where(lrc => lrc != "");
            int iCnt = 1;
            foreach(var cond in conditions)
            {
                if(string.IsNullOrEmpty(cond)) continue;
                var param = cond.Split(",".ToCharArray());
                var PropertyName = param[0];
                var Operator = param[1];
                var Value = string.Empty;
                var LogicalConnector = string.Empty;
                Value = param[2];
                LogicalConnector = (param[3] == "AND" ? " And" : " Or");
                if(iCnt == conditions.Count())
                    LogicalConnector = "";
                ViewBag.SearchResult += " " + GetPropertyDP("PropertyValidationandFormat", PropertyName) + " " + Operator + " " + Value + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch("PropertyValidationandFormat", PropertyName, Operator, Value) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstPropertyValidationandFormat = lstPropertyValidationandFormat.Where(whereCondition.ToString());
            ViewBag.FilterCondition = FilterCondition;
        }
        var DataOrdering = string.Empty;
        if(!string.IsNullOrEmpty(GroupByColumn))
        {
            DataOrdering = GroupByColumn;
            ViewBag.IsGroupBy = true;
        }
        if(!string.IsNullOrEmpty(SortOrder))
            DataOrdering += SortOrder;
        if(!string.IsNullOrEmpty(DataOrdering))
            lstPropertyValidationandFormat = Sorting.Sort<PropertyValidationandFormat>(lstPropertyValidationandFormat, DataOrdering);
        var _PropertyValidationandFormat = lstPropertyValidationandFormat;
        //if(LowerBoundFrom != null || LowerBoundTo != null)
        //{
        //    try
        //    {
        //        int from = LowerBoundFrom == null ? 0 : Convert.ToInt32(LowerBoundFrom);
        //        int to = LowerBoundTo == null ? int.MaxValue : Convert.ToInt32(LowerBoundTo);
        //        _PropertyValidationandFormat = _PropertyValidationandFormat.Where(o => o.LowerBound >= from && o.LowerBound <= to);
        //        ViewBag.SearchResult += "\r\n Lower Bound= " + from + "-" + to;
        //    }
        //    catch { }
        //}
        //if(UpperBoundFrom != null || UpperBoundTo != null)
        //{
        //    try
        //    {
        //        int from = UpperBoundFrom == null ? 0 : Convert.ToInt32(UpperBoundFrom);
        //        int to = UpperBoundTo == null ? int.MaxValue : Convert.ToInt32(UpperBoundTo);
        //        _PropertyValidationandFormat = _PropertyValidationandFormat.Where(o => o.UpperBound >= from && o.UpperBound <= to);
        //        ViewBag.SearchResult += "\r\n Upper Bound= " + from + "-" + to;
        //    }
        //    catch { }
        //}
        if(IsEnabled != null)
        {
            try
            {
                bool boolvalue = Convert.ToBoolean(IsEnabled);
                _PropertyValidationandFormat = _PropertyValidationandFormat.Where(o => o.IsEnabled == boolvalue);
                ViewBag.SearchResult += "\r\n Is Enabled= " + IsEnabled;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        _PropertyValidationandFormat = FilterByHostingEntity(_PropertyValidationandFormat, HostingEntity, AssociatedType, Convert.ToInt32(HostingEntityID));
        ViewBag.SearchResult = ((string)ViewBag.SearchResult).TrimStart("\r\n".ToCharArray());
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyValidationandFormat"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        ViewBag.PageSize = pageSize;
        //
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_PropertyValidationandFormat.Count() > 0)
                pageSize = _PropertyValidationandFormat.Count();
            return View("ExcelExport", _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _PropertyValidationandFormat.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        ViewBag.Pages = pageNumber;
        if(!Request.IsAjaxRequest())
        {
            if(string.IsNullOrEmpty(viewtype))
                viewtype = "IndexPartial";
            ViewBag.TemplatesName = GetTemplatesForList(User, "PropertyValidationandFormat", viewtype);
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
                ViewBag.TemplatesName = viewtype;
            var list = _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityPropertyValidationandFormatDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["PropertyValidationandFormatlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "PropertyValidationandFormat", tagsSplit.ToArray());
                }
            return View("Index", list);
        }
        else
        {
            var list = _PropertyValidationandFormat.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityPropertyValidationandFormatDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["PropertyValidationandFormatlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "PropertyValidationandFormat", tagsSplit.ToArray());
                }
            if(ViewBag.TemplatesName == null)
                return PartialView("IndexPartial", list);
            else
                return PartialView(ViewBag.TemplatesName, list);
        }
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_PropertyValidationandFormat">IQueryable<PropertyValidationandFormat>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<PropertyValidationandFormat>.</returns>
    private IQueryable<PropertyValidationandFormat> FilterByHostingEntity(IQueryable<PropertyValidationandFormat> _PropertyValidationandFormat, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        return _PropertyValidationandFormat;
    }
    /// <summary>Gets display value by record id.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    public string GetDisplayValue(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        long idvalue = Convert.ToInt64(id);
        using(var appcontext = (new PropertyValidationandFormatContext()))
        {
            var _Obj = appcontext.PropertyValidationandFormats.FirstOrDefault(p => p.Id == idvalue);
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
    }
    
    /// <summary>Gets record by identifier.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier.</returns>
    public object GetRecordbyId(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        using(var appcontext = (new PropertyValidationandFormatContext()))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.PropertyValidationandFormats.Find(Convert.ToInt64(id));
            return _Obj;
        }
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A JSON response stream to send to the GetJsonRecor(new PropertyValidationandFormatContext())yId action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetJsonRecordbyId(string id)
    {
        if(string.IsNullOrEmpty(id)) return Json(new PropertyValidationandFormat(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new PropertyValidationandFormatContext()))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.PropertyValidationandFormats.Find(Convert.ToInt64(id));
            return Json(_Obj, JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>Gets record by identifier reflection.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier reflection.</returns>
    public string GetRecordbyId_Reflection(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        using(var context = (new PropertyValidationandFormatContext()))
        {
            context.Configuration.LazyLoadingEnabled = false;
            var _Obj = context.PropertyValidationandFormats.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<PropertyValidationandFormat>(_Obj, "PropertyValidationandFormat", new string[] { "" });
        }
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<PropertyValidationandFormat> list = db1.PropertyValidationandFormats;
        var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                   select new { Id = x.Id, Name = x.DisplayValue };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var result = DropdownHelper.GetAllValue<PropertyValidationandFormat>(User, db1.PropertyValidationandFormats, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "PropertyValidationandFormat");
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (used in business rule).</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForRB action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var result = DropdownHelper.GetAllValueForRB<PropertyValidationandFormat>(User, db1.PropertyValidationandFormats, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "PropertyValidationandFormat");
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown for businessrule).</summary>
    ///
    /// <param name="propNameBR">The property name line break.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValueForBR action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
    {
        IQueryable<PropertyValidationandFormat> list = db1.PropertyValidationandFormats;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            var result = list.Select("new(Id," + propNameBR + " as value)");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown on UI).</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValue(string key, string AssoNameWithParent, string AssociationID)
    {
        var result = DropdownHelper.GetAllMultiSelectValue(User, db1.PropertyValidationandFormats, key, AssoNameWithParent, AssociationID, "PropertyValidationandFormat");
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP GET requests) uploads the given excel/csv file.</summary>
    ///
    /// <param name="FileType">The file uploaded.</param>
    /// <returns>A response stream to send to the Upload View.</returns>
    [HttpGet]
    
    
    /// <summary>Gets field value by entity identifier.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>The field value by entity identifier.</returns>
    public object GetFieldValueByEntityId(long Id, string PropName)
    {
        try
        {
            using(var appcontext = (new PropertyValidationandFormatContext()))
            {
                var obj1 = appcontext.PropertyValidationandFormats.Find(Id);
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                return PropValue;
            }
        }
        catch
        {
            return null;
        }
    }
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool CheckBeforeDelete(PropertyValidationandFormat propertyvalidationandformat)
    {
        var result = true;
        // Write your logic here
        if(!result)
        {
            var AlertMsg = "Validation Alert - Before Delete !! Information not deleted.";
            ModelState.AddModelError("CustomError", AlertMsg);
            ViewBag.ApplicationError = AlertMsg;
        }
        return result;
    }
    /// <summary>Check before save.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public string CheckBeforeSave(PropertyValidationandFormat propertyvalidationandformat, string command = "")
    {
        var AlertMsg = "";
        using(PropertyValidationandFormatContext db = new PropertyValidationandFormatContext())
        {
            var flag = db.PropertyValidationandFormats.Any(p => p.Id != propertyvalidationandformat.Id && p.EntityName == propertyvalidationandformat.EntityName && p.PropertyName == propertyvalidationandformat.PropertyName);
            if(flag)
            {
                AlertMsg = "A record with this entity and property combination already exist, please delete/modify existing record.";
                ModelState.AddModelError("CustomError", AlertMsg);
                ViewBag.ApplicationError = AlertMsg;
            }
        }
        // Write your logic here
        //Make sure to assign AlertMsg with proper message
        //AlertMsg = "Validation Alert - Before Save !! Information not saved.";
        //ModelState.AddModelError("CustomError", AlertMsg);
        //ViewBag.ApplicationError = AlertMsg;
        return AlertMsg;
    }
    /// <summary>Executes the deleting action.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="unitdb">   The unitdb.</param>
    public void OnDeleting(PropertyValidationandFormat propertyvalidationandformat, ApplicationContext unitdb)
    {
        // Write your logic here
    }
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="db">       The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(PropertyValidationandFormat propertyvalidationandformat, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        // Write your logic here
    }
    /// <summary>After save.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(PropertyValidationandFormat propertyvalidationandformat, GeneratorBase.MVC.Models.IUser aftersaveuser)
    {
        // Write your logic here
    }
    /// <summary>Custom authorization before create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomAuthorizationBeforeCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        var result = true;
        return result;
    }
    /// <summary>Custom authorization before edit.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomAuthorizationBeforeEdit(int? id, string HostingEntityName, string AssociatedType)
    {
        var result = true;
        return result;
    }
    /// <summary>Custom authorization before details.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomAuthorizationBeforeDetails(int? id, string HostingEntityName, string AssociatedType)
    {
        var result = true;
        return result;
    }
    /// <summary>Custom authorization before delete.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomAuthorizationBeforeDelete(int? id)
    {
        var result = true;
        return result;
    }
    /// <summary>Custom authorization before bulk update.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomAuthorizationBeforeBulkUpdate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        var result = true;
        return result;
    }
    /// <summary>Custom load view data list before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    private void CustomLoadViewDataListBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
    }
    /// <summary>Custom load view data list after on create.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    
    private void CustomLoadViewDataListAfterOnCreate(PropertyValidationandFormat propertyvalidationandformat)
    {
    }
    /// <summary>Custom load view data list before edit.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    private void CustomLoadViewDataListBeforeEdit(PropertyValidationandFormat propertyvalidationandformat)
    {
    }
    /// <summary>Custom load view data list after edit.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    private void CustomLoadViewDataListAfterEdit(PropertyValidationandFormat propertyvalidationandformat)
    {
    }
    /// <summary>Custom load view data list on index.</summary>
    ///
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    private void CustomLoadViewDataListOnIndex(string HostingEntity, int? HostingEntityID, string AssociatedType)
    {
    }
    /// <summary>Custom save on create.</summary>
    ///
    /// <param name="propertyvalidationandformat">             The Property Validation and Format.</param>
    /// <param name="customcreate_hasissues">[out] True to customcreate hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnCreate(PropertyValidationandFormat propertyvalidationandformat, out bool customcreate_hasissues, string command = "")
    {
        var result = false;
        customcreate_hasissues = false;
        return result;
    }
    /// <summary>Custom save on edit.</summary>
    ///
    /// <param name="propertyvalidationandformat">           The Property Validation and Format.</param>
    /// <param name="customsave_hasissues">[out] True to customsave hasissues.</param>
    /// <param name="command">             (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnEdit(PropertyValidationandFormat propertyvalidationandformat, out bool customsave_hasissues, string command = "")
    {
        var result = false;
        customsave_hasissues = false;
        return result;
    }
    /// <summary>Custom save on import.</summary>
    ///
    /// <param name="propertyvalidationandformat">  The Property Validation and Format.</param>
    /// <param name="customerror">[out] The customerror.</param>
    /// <param name="i">          Zero-based index of the.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnImport(PropertyValidationandFormat propertyvalidationandformat, out string customerror, int i)
    {
        var result = false;
        customerror = "";
        return result;
    }
    /// <summary>Custom import data validate.</summary>
    ///
    /// <param name="objDataSet"> Set the object data belongs to.</param>
    /// <param name="columnName"> Name of the column.</param>
    /// <param name="columnValue">The column value.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomImportDataValidate(DataSet objDataSet, string columnName, string columnValue)
    {
        var result = true;
        //create ViewBag.CustomErrorsConfirmImportData for extra validation msg here
        return result;
    }
    /// <summary>Custom delete.</summary>
    ///
    /// <param name="propertyvalidationandformat">             The Property Validation and Format.</param>
    /// <param name="customdelete_hasissues">[out] True to customdelete hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomDelete(PropertyValidationandFormat propertyvalidationandformat, out bool customdelete_hasissues, string command = "")
    {
        var result = false;
        customdelete_hasissues = false;
        return result;
    }
    /// <summary>Custom sorting.</summary>
    ///
    /// <param name="list">          The IQueryable<PropertyValidationandFormat> list.</param>
    /// <param name="HostingEntity"> The hosting entity.</param>
    /// <param name="AssociatedType">Type of the associated.</param>
    /// <param name="sortBy">        Describes who sort this object.</param>
    /// <param name="isAsc">         The is ascending.</param>
    ///
    /// <returns>An IQueryable<PropertyValidationandFormat></returns>
    private IQueryable<PropertyValidationandFormat> CustomSorting(IQueryable<PropertyValidationandFormat> list, string HostingEntity, string AssociatedType, string sortBy, string isAsc)
    {
        IQueryable<PropertyValidationandFormat> orderedList = list;
        // Do custom sorting here, you can also use this method to do custom filter
        return orderedList;
    }
    /// <summary>Creates a result that redirects to the given route.</summary>
    ///
    /// <param name="propertyvalidationandformat">The Property Validation and Format.</param>
    /// <param name="action">   The action.</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A RedirectToRouteResult.</returns>
    private RedirectToRouteResult CustomRedirectUrl(PropertyValidationandFormat propertyvalidationandformat, string action, string command = "")
    {
        // Sample Custom implemention below
        // return RedirectToAction("Index", "PropertyValidationandFormat");
        return null;
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
            if(db != null)
            {
                db1.Dispose();
                db.Dispose();
            }
        }
        base.Dispose(disposing);
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult ApplySettings(bool IsReset)
    {
        if(IsReset)
        {
            foreach(var item in db1.PropertyValidationandFormats)
            {
                item.IsEnabled = false;
                db1.Entry(item).State = EntityState.Modified;
            }
            db1.SaveChanges();
        }
        var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        myConfiguration.AppSettings.Settings["PropertyValidationandFormat"].Value = Convert.ToString(DateTime.UtcNow.Ticks);
        myConfiguration.Save();
        return RedirectToAction("Index");
    }
}
}
