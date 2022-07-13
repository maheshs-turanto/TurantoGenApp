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

/// <summary>A controller for handling reports groups.</summary>
public partial class ReportsGroupController : BaseController
{
    /// <summary>GET: /ReportsGroup/.</summary>
    ///
    /// <param name="currentFilter">  A filter specifying the current.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="IsExport">       The is export.</param>
    /// <param name="IsDeepSearch">   The is deep search.</param>
    /// <param name="IsFilter">       A filter specifying the is.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    /// <param name="BulkOperation">  The bulk operation.</param>
    /// <param name="parent">         The parent.</param>
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     List of is homes.</param>
    /// <param name="IsDivRender">    (Optional) True if is div render, false if not.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype,string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false)
    {
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["IsFilter"] = IsFilter;
        ViewData["BulkOperation"] = BulkOperation;
        ViewData["caller"] = caller;
        ViewBag.IsDivRender = IsDivRender;
        if(!string.IsNullOrEmpty(viewtype))
        {
            viewtype = viewtype.Replace("?IsAddPop=true", "");
            ViewBag.TemplatesName = viewtype;
        }
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        CustomLoadViewDataListOnIndex(HostingEntity, HostingEntityID, AssociatedType);
        var lstReportsGroup = from s in db.ReportsGroups.AsNoTracking()
                              select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstReportsGroup = searchRecords(lstReportsGroup, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstReportsGroup = sortRecords(lstReportsGroup, sortBy, isAsc);
        }
        else lstReportsGroup = lstReportsGroup.OrderByDescending(c => c.Id);
        lstReportsGroup = CustomSorting(lstReportsGroup,HostingEntity,AssociatedType,sortBy,isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ReportsGroup"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ReportsGroup"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ReportsGroup"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportsGroup"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportsGroup"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ReportsGroup"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        var _ReportsGroup = lstReportsGroup;
        if(Convert.ToBoolean(IsExport))
        {
            if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "ReportsGroup", User) || !User.CanView("ReportsGroup"))
            {
                return RedirectToAction("Index", "Error");
            }
            pageNumber = 1;
            if(_ReportsGroup.Count() > 0)
                pageSize = _ReportsGroup.Count();
            return View("ExcelExport", _ReportsGroup.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _ReportsGroup.Count();
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
        ViewBag.TemplatesName = "IndexPartial";
        TempData["ReportsGrouplist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _ReportsGroup.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityReportsGroupDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["ReportsGrouplist"] = list.Select(z => new
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
                    _ReportsGroup = _fad.FilterDropdown<ReportsGroup>(User,  _ReportsGroup, "ReportsGroup", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation",sortRecords(lstReportsGroup.Except(_ReportsGroup),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstReportsGroup.Except(_ReportsGroup).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _ReportsGroup.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _ReportsGroup.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _ReportsGroup.Count() == 0 ? 1 : _ReportsGroup.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _ReportsGroup.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityReportsGroupDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["ReportsGrouplist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _ReportsGroup.Count() == 0 ? 1 : _ReportsGroup.Count();
                    }
                    var list = _ReportsGroup.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityReportsGroupDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["ReportsGrouplist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    
    /// <summary>GET: /ReportsGroup/Details/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
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
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            db.DisableFilter("IsDeleted");
            reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup == null)
                return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = "Details";
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(reportsgroup);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(reportsgroup, AssociatedType);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnDetails", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnDetails", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnDetails",new long[] { 6, 8 });
        return View(ViewBag.TemplatesName,reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype,bool RenderPartial = false)
    {
        if(!User.CanAdd("ReportsGroup") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        //if (string.IsNullOrEmpty(viewtype))
        viewtype = "Create";
        ViewBag.TemplatesName = "Create";
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    /// <summary>GET: /ReportsGroup/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
    {
        if(!User.CanAdd("ReportsGroup") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "CreateWizard";
        // GetTemplatesForCreateWizard(viewtype);
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        return View();
    }
    
    /// <summary>POST: /ReportsGroup/CreateWizard To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="UrlReferrer"> The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description,DisplayOrder")] ReportsGroup reportsgroup, string UrlReferrer)
    {
        CheckBeforeSave(reportsgroup);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(reportsgroup,out customcreate_hasissues,"Create"))
            {
                db.ReportsGroups.Add(reportsgroup);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(reportsgroup,"Create","");
                if(customRedirectAction != null) return customRedirectAction;
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        LoadViewDataAfterOnCreate(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        return View(reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
    {
        if(!User.CanAdd("ReportsGroup") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "CreateQuick";
        //GetTemplatesForCreateQuick(viewtype);
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        return View();
    }
    
    /// <summary>POST: /ReportsGroup/CreateQuick To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup">     The reportsgroup.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description,DisplayOrder")] ReportsGroup reportsgroup,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(reportsgroup);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(reportsgroup,out customcreate_hasissues,"Create"))
            {
                db.ReportsGroups.Add(reportsgroup);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnCreate(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(reportsgroup, AssociatedEntity);
        return View(reportsgroup);
    }
    
    /// <summary>Cancels.</summary>
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
    
    /// <summary>POST: /ReportsGroup/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup">     The reportsgroup.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description,DisplayOrder")] ReportsGroup reportsgroup, string UrlReferrer, bool? IsDDAdd,string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(reportsgroup, command);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(reportsgroup,out customcreate_hasissues,command))
            {
                db.ReportsGroups.Add(reportsgroup);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(reportsgroup,"Create",command);
                if(customRedirectAction != null) return customRedirectAction;
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = reportsgroup.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = reportsgroup.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnCreate(reportsgroup);
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnCreate", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnCreate", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnCreate",new long[] { 6, 7});
        return View(reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("ReportsGroup") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            db.DisableFilter("IsDeleted");
            reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup == null)
                return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        if(ViewData["ReportsGroupParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup/Edit/" + reportsgroup.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup/Create"))
            ViewData["ReportsGroupParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        var objReportsGroup = new List<ReportsGroup>();
        ViewBag.ReportsGroupDD = new SelectList(objReportsGroup, "ID", "DisplayValue");
        return View(reportsgroup);
    }
    
    /// <summary>POST: /ReportsGroup/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup">    The reportsgroup.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,Name,Description,DisplayOrder")] ReportsGroup reportsgroup,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(reportsgroup, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(reportsgroup,out customsave_hasissues,command))
            {
                db.Entry(reportsgroup).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Succeed", UrlRefr =UrlReferrer };
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
        if(db.Entry(reportsgroup).State == EntityState.Detached)
            reportsgroup = db.ReportsGroups.Find(reportsgroup.Id);
        LoadViewDataAfterOnEdit(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        return View(reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RenderPartial = false)
    {
        if(!User.CanEdit("ReportsGroup") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            db.DisableFilter("IsDeleted");
            reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup == null)
                return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["ReportsGrouplist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.ReportsGroups.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityReportsGroupDisplayValueEdit = TempData["ReportsGrouplist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["ReportsGrouplist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityReportsGroupDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = "Edit";
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = reportsgroup.DisplayValue, Value = reportsgroup.Id.ToString() }));
            ViewBag.EntityReportsGroupDisplayValueEdit = newList;
            TempData["ReportsGrouplist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(reportsgroup.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = reportsgroup.DisplayValue;
                newList[0].Value = reportsgroup.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = reportsgroup.DisplayValue, Value = reportsgroup.Id.ToString() }));
            }
            ViewBag.EntityReportsGroupDisplayValueEdit = newList;
            TempData["ReportsGrouplist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        if(ViewData["ReportsGroupParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup/Edit/" + reportsgroup.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup/Create"))
            ViewData["ReportsGroupParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName,reportsgroup);
    }
    
    /// <summary>POST: /ReportsGroup/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup"> The reportsgroup.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,Name,Description,DisplayOrder")] ReportsGroup reportsgroup,  string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(reportsgroup, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(reportsgroup,out customsave_hasissues,command))
            {
                db.Entry(reportsgroup).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(reportsgroup,"Edit",command);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = reportsgroup.Id };
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
                        return RedirectToAction("Edit", new { Id = reportsgroup.Id, UrlReferrer = UrlReferrer });
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
        if(TempData["ReportsGrouplist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.ReportsGroups.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityReportsGroupDisplayValueEdit = TempData["ReportsGrouplist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["ReportsGrouplist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new    Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityReportsGroupDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(reportsgroup).State == EntityState.Detached)
            reportsgroup = db.ReportsGroups.Find(reportsgroup.Id);
        LoadViewDataAfterOnEdit(reportsgroup);
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        return View(reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/EditWizard/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer,string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("ReportsGroup") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            db.DisableFilter("IsDeleted");
            reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup == null)
                return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        if(ViewData["ReportsGroupParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup"))
            ViewData["ReportsGroupParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        return View(reportsgroup);
    }
    
    /// <summary>POST: /ReportsGroup/EditWizard/5 To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="UrlReferrer"> The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,Name,Description,DisplayOrder")] ReportsGroup reportsgroup,string UrlReferrer)
    {
        CheckBeforeSave(reportsgroup);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(reportsgroup,out customsave_hasissues,"Save"))
            {
                db.Entry(reportsgroup).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(reportsgroup,"Edit","");
                if(customRedirectAction != null) return customRedirectAction;
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
        if(db.Entry(reportsgroup).State == EntityState.Detached)
            reportsgroup = db.ReportsGroups.Find(reportsgroup.Id);
        LoadViewDataAfterOnEdit(reportsgroup);
        ViewBag.ReportsGroupIsHiddenRule = checkHidden("ReportsGroup", "OnEdit", false);
        ViewBag.ReportsGroupIsGroupsHiddenRule = checkHidden("ReportsGroup", "OnEdit", true);
        ViewBag.ReportsGroupIsSetValueUIRule = checkSetValueUIRule("ReportsGroup", "OnEdit",new long[] { 6, 8});
        return View(reportsgroup);
    }
    
    /// <summary>GET: /ReportsGroup/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("ReportsGroup") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["ReportsGroupParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ReportsGroup"))
            ViewData["ReportsGroupParentUrl"] = Request.UrlReferrer;
        return View(reportsgroup);
    }
    
    /// <summary>POST: /ReportsGroup/Delete/5.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="UrlReferrer"> The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(ReportsGroup reportsgroup, string UrlReferrer)
    {
        if(!User.CanDelete("ReportsGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        reportsgroup = db.ReportsGroups.Find(reportsgroup.Id);
        if(CheckBeforeDelete(reportsgroup))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(reportsgroup, out customdelete_hasissues, "Delete"))
            {
                db.Entry(reportsgroup).State = EntityState.Deleted;
                db.ReportsGroups.Remove(reportsgroup);
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["ReportsGroupParentUrl"] != null)
                {
                    string parentUrl = ViewData["ReportsGroupParentUrl"].ToString();
                    ViewData["ReportsGroupParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(reportsgroup);
    }
    
    /// <summary>Saves the properties value.</summary>
    ///
    /// <param name="id">        The identifier.</param>
    /// <param name="properties">The properties.</param>
    ///
    /// <returns>A response stream to send to the SavePropertiesValue View.</returns>
    
    public ActionResult SavePropertiesValue(long id, Dictionary<string, string> properties)
    {
        if(id > 0 && properties != null)
        {
            if(!User.CanEdit("ReportsGroup"))
            {
                return RedirectToAction("Index", "Error");
            }
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
            db.Configuration.LazyLoadingEnabled = false;
            ReportsGroup reportsgroup = db.ReportsGroups.Where(p=>p.Id == id).FirstOrDefault();
            if(reportsgroup == null)
            {
                db.DisableFilter("IsDeleted");
                reportsgroup = db.ReportsGroups.Find(id);
                if(reportsgroup == null)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    return HttpNotFound();
                }
            }
            // business rule before load section
            var resultBefore = ApplyBusinessRuleBefore(reportsgroup, true);
            if(resultBefore.Data != null)
            {
                var result = new { Result = "fail", data = resultBefore.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            var strChkBeforeSave = CheckBeforeSave(reportsgroup, "SaveProperty");
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                var result = new { Result = "fail", data = strChkBeforeSave };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            bool isSave = false;
            reportsgroup.setDateTimeToClientTime();
            foreach(var item in properties)
            {
                var propertyName = item.Key;
                var propertyValue = item.Value;
                var propertyInfo = reportsgroup.GetType().GetProperty(propertyName);
                if(propertyInfo != null)
                {
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object newValue = string.IsNullOrEmpty(propertyValue) ? null : propertyValue;
                    isSave = true;
                    try
                    {
                        object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                        propertyInfo.SetValue(reportsgroup, safeValue, null);
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
            var resultOnSaving = ApplyBusinessRuleOnSaving(reportsgroup);
            if(resultOnSaving.Data != null)
            {
                var result = new { Result = "fail", data = resultOnSaving.Data };
                db.Configuration.LazyLoadingEnabled = true;
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            // business rule onsaving section
            if(isSave && ValidateModel(reportsgroup))
            {
                bool customsave_hasissues = false;
                if(!CustomSaveOnEdit(reportsgroup, out customsave_hasissues, "Save"))
                {
                    db.Entry(reportsgroup).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.Configuration.LazyLoadingEnabled = true;
                var result = new { Result = "Success", data = reportsgroup.DisplayValue };
                if(!customsave_hasissues)
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach(var error in ValidateModelWithErrors(reportsgroup))
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
    /// <param name="id">      The identifier.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">   The value.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyValue View.</returns>
    
    [HttpGet]
    public ActionResult SavePropertyValue(long id, string property, string value)
    {
        if(!User.CanEdit("ReportsGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
        if(reportsgroup == null)
        {
            db.DisableFilter("IsDeleted");
            reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup == null)
                return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var resultBefore = ApplyBusinessRuleBefore(reportsgroup,true);
        if(resultBefore.Data != null)
        {
            var result = new { Result = "fail", data = resultBefore.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule section
        var strChkBeforeSave = CheckBeforeSave(reportsgroup, "SaveProperty");
        if(!string.IsNullOrEmpty(strChkBeforeSave))
        {
            var result = new { Result = "fail", data = strChkBeforeSave };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var propertyInfo = reportsgroup.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            reportsgroup.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(reportsgroup, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        var resultOnSaving = ApplyBusinessRuleOnSaving(reportsgroup);
        if(resultOnSaving.Data != null)
        {
            var result = new { Result = "fail", data = resultOnSaving.Data };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // business rule onsaving section
        if(isSave && ValidateModel(reportsgroup))
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(reportsgroup, out customsave_hasissues, "Save"))
            {
                db.Entry(reportsgroup).State = EntityState.Modified;
                db.SaveChanges();
            }
            var result = new { Result = "Success", data = value };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(reportsgroup))
            {
                errors += error.ErrorMessage + ".  ";
            }
            var result = new { Result = "fail", data = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Bulk associate.</summary>
    ///
    /// <param name="ids">            The identifiers.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
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
    
    
    
    
    /// <summary>Deletes the bulk.</summary>
    ///
    /// <param name="ids">        The identifiers.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteBulk View.</returns>
    
    public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
    {
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "ReportsGroup", User) || !User.CanDelete("ReportsGroup")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            ReportsGroup reportsgroup = db.ReportsGroups.Find(id);
            if(reportsgroup != null)
            {
                if(CheckBeforeDelete(reportsgroup))
                {
                    if(!CustomDelete(reportsgroup, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(reportsgroup).State = EntityState.Deleted;
                        db.ReportsGroups.Remove(reportsgroup);
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
    
    /// <summary>(An Action that handles HTTP POST requests) bulk update.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDUpdate">       The is dd update.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpGet]
    public ActionResult BulkUpdate(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
    {
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "ReportsGroup", User) || !User.CanEdit("ReportsGroup") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["ReportsGroupParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        string ids = string.Empty;
        if(Request.QueryString["ids"] != null)
            ids = Request.QueryString["ids"];
        ViewBag.BulkUpdate = ids;
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) bulk update.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="collection">  The collection.</param>
    /// <param name="UrlReferrer"> The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,IsDeleted,DeleteDateTime,Name,Description,DisplayOrder")] ReportsGroup reportsgroup,FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                ReportsGroup target = db.ReportsGroups.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(reportsgroup, target, chkUpdate);
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(reportsgroup,"BulkUpdate","");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>Reports group home.</summary>
    ///
    /// <param name="RenderPartial">The render partial.</param>
    ///
    /// <returns>A response stream to send to the ReportsGroupHome View.</returns>
    
    public ActionResult ReportsGroupHome(bool? RenderPartial)
    {
        var lstEntityHelpPage = from s in db.EntityHelpPages
                                select s;
        var _EntityHelpPage = lstEntityHelpPage.Where(p => p.EntityName == "ReportsGroup" && p.Disable==false).OrderBy(p => p.Order);
        var list = _EntityHelpPage.ToList();
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
            ViewBag.FavoriteItem = lstFavoriteItem;
        if(!(RenderPartial == null ? false : RenderPartial.Value))
        {
            return View(list);
        }
        else
            return PartialView(list);
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db!=null) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
