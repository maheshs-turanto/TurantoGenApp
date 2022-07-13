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
namespace GeneratorBase.MVC.Controllers
{

/// <summary>A controller for handling faceted searches.</summary>
public partial class T_FacetedSearchController : BaseController
{
    /// <summary>GET: /T_FacetedSearch/.</summary>
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
    /// <param name="RenderPartial">  True to render partial.</param>
    /// <param name="BulkOperation">  The bulk operation.</param>
    /// <param name="parent">         The parent.</param>
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     List of is homes.</param>
    /// <param name="IsDivRender">    (Optional) True if is div render, false if not.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome, bool? IsHomeList, bool IsDivRender = false)
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
        var lstT_FacetedSearch = from s in db.T_FacetedSearchs
                                 select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_FacetedSearch = searchRecords(lstT_FacetedSearch, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstT_FacetedSearch = sortRecords(lstT_FacetedSearch, sortBy, isAsc);
        }
        else lstT_FacetedSearch = lstT_FacetedSearch.OrderByDescending(c => c.Id);
        lstT_FacetedSearch = CustomSorting(lstT_FacetedSearch, HostingEntity, AssociatedType, sortBy, isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        var _T_FacetedSearch = lstT_FacetedSearch;
        if(Convert.ToBoolean(IsExport))
        {
            if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "T_FacetedSearch", User) || !User.CanView("T_FacetedSearch"))
            {
                return RedirectToAction("Index", "Error");
            }
            pageNumber = 1;
            if(_T_FacetedSearch.Count() > 0)
                pageSize = _T_FacetedSearch.Count();
            return View("ExcelExport", _T_FacetedSearch.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _T_FacetedSearch.Count();
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
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _T_FacetedSearch.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityT_FacetedSearchDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_FacetedSearchlist"] = list.Select(z => new
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
                    _T_FacetedSearch = _fad.FilterDropdown<T_FacetedSearch>(User, _T_FacetedSearch, "T_FacetedSearch", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstT_FacetedSearch.Except(_T_FacetedSearch), sortBy, isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstT_FacetedSearch.Except(_T_FacetedSearch).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _T_FacetedSearch.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _T_FacetedSearch.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _T_FacetedSearch.Count() == 0 ? 1 : _T_FacetedSearch.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _T_FacetedSearch.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityT_FacetedSearchDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_FacetedSearchlist"] = list.Select(z => new
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
                        pageSize = _T_FacetedSearch.Count() == 0 ? 1 : _T_FacetedSearch.Count();
                    }
                    var list = _T_FacetedSearch.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityT_FacetedSearchDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["T_FacetedSearchlist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    
    /// <summary>GET: /T_FacetedSearch/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
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
        T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
        if(t_facetedsearch == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = "Details";
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(t_facetedsearch);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(t_facetedsearch, AssociatedType);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnDetails", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnDetails", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnDetails");
        return View(ViewBag.TemplatesName, t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="RenderPartial">    (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype, bool RenderPartial = false)
    {
        if(!User.CanAdd("T_FacetedSearch") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        //if (string.IsNullOrEmpty(viewtype))
        viewtype = "Create";
        ViewBag.TemplatesName = "Create";
        ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    /// <summary>GET: /T_FacetedSearch/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, string viewtype)
    {
        if(!User.CanAdd("T_FacetedSearch") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "CreateWizard";
        // GetTemplatesForCreateWizard(viewtype);
        ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /T_FacetedSearch/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, string UrlReferrer)
    {
        CheckBeforeSave(t_facetedsearch);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_facetedsearch, out customcreate_hasissues, "Create"))
            {
                db.T_FacetedSearchs.Add(t_facetedsearch);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_facetedsearch, "Create", "");
                if(customRedirectAction != null) return customRedirectAction;
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        LoadViewDataAfterOnCreate(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        return View(t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/CreateQuick.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="Url">       URL of the resource.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string EntityName, string Url)
    {
        if(!User.CanAdd("T_FacetedSearch"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        //RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "", User.userroles.FirstOrDefault());
        //var model = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        //ViewBag.T_TargetEntity = new SelectList(model.Associations, "Target", "Target");
        if(!(string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(EntityName)))
        {
            ViewData["EntityName"] = EntityName;
            ViewData["Url"] = VirtualPathUtility.MakeRelative("~", Url);
            string LinkAddress = Convert.ToString(ViewData["Url"]).Trim();
            string LinkAddress1 = LinkAddress.EndsWith("&ShowDeleted=") ? LinkAddress.Replace("&ShowDeleted=", "") : LinkAddress;
            var Existing = db.T_FacetedSearchs.FirstOrDefault(p => p.T_EntityName == EntityName && (p.T_LinkAddress.Trim() == LinkAddress || p.T_LinkAddress.Trim() == LinkAddress1));
            if(Existing != null)
                return View(Existing);
            return View();
        }
        else
            return View();
    }
    
    /// <summary>POST: /T_FacetedSearch/CreateQuick To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch">  The facetedsearch.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag,T_TabName")] T_FacetedSearch t_facetedsearch, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID)
    {
        CheckBeforeSave(t_facetedsearch);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_facetedsearch, out customcreate_hasissues, "Create"))
            {
                var Existing = db.T_FacetedSearchs.FirstOrDefault(p => p.T_EntityName == t_facetedsearch.T_EntityName && p.T_LinkAddress == t_facetedsearch.T_LinkAddress);
                if(Existing != null)
                {
                    Existing.T_Name = t_facetedsearch.T_Name;
                    Existing.T_Description = t_facetedsearch.T_Description;
                    Existing.T_Roles = t_facetedsearch.T_Roles;
                    Existing.T_Disable = t_facetedsearch.T_Disable;
                    db.Entry(Existing).State = EntityState.Modified;
                }
                else db.T_FacetedSearchs.Add(t_facetedsearch);
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
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        LoadViewDataAfterOnCreate(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(t_facetedsearch, AssociatedEntity);
        return View(t_facetedsearch);
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
    
    /// <summary>POST: /T_FacetedSearch/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    /// <param name="IsDDAdd">        The is dd add.</param>
    /// <param name="RenderPartial">  (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, string UrlReferrer, bool? IsDDAdd, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_facetedsearch, command);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(t_facetedsearch, out customcreate_hasissues, command))
            {
                db.T_FacetedSearchs.Add(t_facetedsearch);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_facetedsearch, "Create", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = t_facetedsearch.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = t_facetedsearch.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnCreate(t_facetedsearch);
        ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnCreate", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnCreate");
        return View(t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("T_FacetedSearch") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "EditQuick";
        //GetTemplatesForEditQuick(viewtype);
        T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
        if(t_facetedsearch == null)
        {
            return HttpNotFound();
        }
        if(!User.IsAdmin && !string.IsNullOrEmpty(t_facetedsearch.T_Roles) && !User.IsInRole(User.userroles, t_facetedsearch.T_Roles.Split(",".ToCharArray())))
        {
            return RedirectToAction("Index", "Error");
        }
        if(UrlReferrer != null)
            ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        if(ViewData["T_FacetedSearchParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch/Edit/" + t_facetedsearch.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch/Create"))
            ViewData["T_FacetedSearchParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        var objT_FacetedSearch = new List<T_FacetedSearch>();
        ViewBag.T_FacetedSearchDD = new SelectList(objT_FacetedSearch, "ID", "DisplayValue");
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        //RoleList.Remove(adminString);
        ViewBag.RoleList = new SelectList(RoleList, "", "");
        return View(t_facetedsearch);
    }
    
    /// <summary>POST: /T_FacetedSearch/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch"> The facetedsearch.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string FsearchId = Request.Form["FsearchId"];
        CheckBeforeSave(t_facetedsearch, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_facetedsearch, out customsave_hasissues, command))
            {
                if(!string.IsNullOrEmpty(FsearchId))
                {
                    t_facetedsearch.T_LinkAddress = VirtualPathUtility.MakeRelative("~", UrlReferrer).Replace("?FsearchId=" + FsearchId + "&", "?").Replace("&FsearchId=" + FsearchId, "");
                }
                db.Entry(t_facetedsearch).State = EntityState.Modified;
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
        LoadViewDataAfterOnEdit(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        return View(t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RenderPartial = false)
    {
        if(!User.CanEdit("T_FacetedSearch") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["T_FacetedSearchlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_FacetedSearchs.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).ToList());
        }
        else
        {
            ViewBag.EntityT_FacetedSearchDisplayValueEdit = TempData["T_FacetedSearchlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_FacetedSearchlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_FacetedSearchDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(t_facetedsearch == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = "Edit";
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = t_facetedsearch.DisplayValue, Value = t_facetedsearch.Id.ToString() }));
            ViewBag.EntityT_FacetedSearchDisplayValueEdit = newList;
            TempData["T_FacetedSearchlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(t_facetedsearch.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = t_facetedsearch.DisplayValue;
                newList[0].Value = t_facetedsearch.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = t_facetedsearch.DisplayValue, Value = t_facetedsearch.Id.ToString() }));
            }
            ViewBag.EntityT_FacetedSearchDisplayValueEdit = newList;
            TempData["T_FacetedSearchlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        if(ViewData["T_FacetedSearchParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch/Edit/" + t_facetedsearch.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch/Create"))
            ViewData["T_FacetedSearchParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, t_facetedsearch);
    }
    
    /// <summary>POST: /T_FacetedSearch/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    /// <param name="RenderPartial">  (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(t_facetedsearch, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_facetedsearch, out customsave_hasissues, command))
            {
                db.Entry(t_facetedsearch).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_facetedsearch, "Edit", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = t_facetedsearch.Id };
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
                        return RedirectToAction("Edit", new { Id = t_facetedsearch.Id, UrlReferrer = UrlReferrer });
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
        if(TempData["T_FacetedSearchlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.T_FacetedSearchs.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityT_FacetedSearchDisplayValueEdit = TempData["T_FacetedSearchlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["T_FacetedSearchlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityT_FacetedSearchDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        LoadViewDataAfterOnEdit(t_facetedsearch);
        ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        return View(t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/EditWizard/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
    {
        if(!User.CanEdit("T_FacetedSearch") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        //if (string.IsNullOrEmpty(viewtype))
        //    viewtype = "EditWizard";
        //GetTemplatesForEditWizard(viewtype);
        T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
        if(t_facetedsearch == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
        if(ViewData["T_FacetedSearchParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch"))
            ViewData["T_FacetedSearchParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        return View(t_facetedsearch);
    }
    
    /// <summary>POST: /T_FacetedSearch/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, string UrlReferrer)
    {
        CheckBeforeSave(t_facetedsearch);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(t_facetedsearch, out customsave_hasissues, "Save"))
            {
                db.Entry(t_facetedsearch).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_facetedsearch, "Edit", "");
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
        LoadViewDataAfterOnEdit(t_facetedsearch);
        ViewBag.T_FacetedSearchIsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", false);
        ViewBag.T_FacetedSearchIsGroupsHiddenRule = checkHidden("T_FacetedSearch", "OnEdit", true);
        ViewBag.T_FacetedSearchIsSetValueUIRule = checkSetValueUIRule("T_FacetedSearch", "OnEdit");
        return View(t_facetedsearch);
    }
    
    /// <summary>GET: /T_FacetedSearch/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("T_FacetedSearch") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
        if(t_facetedsearch == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["T_FacetedSearchParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_FacetedSearch"))
            ViewData["T_FacetedSearchParentUrl"] = Request.UrlReferrer;
        return View(t_facetedsearch);
    }
    
    /// <summary>POST: /T_FacetedSearch/Delete/5.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(T_FacetedSearch t_facetedsearch, string UrlReferrer)
    {
        if(!User.CanDelete("T_FacetedSearch"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(CheckBeforeDelete(t_facetedsearch))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(t_facetedsearch, out customdelete_hasissues, "Delete"))
            {
                db.Entry(t_facetedsearch).State = EntityState.Deleted;
                db.T_FacetedSearchs.Remove(t_facetedsearch);
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["T_FacetedSearchParentUrl"] != null)
                {
                    string parentUrl = ViewData["T_FacetedSearchParentUrl"].ToString();
                    ViewData["T_FacetedSearchParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(t_facetedsearch);
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "T_FacetedSearch", User) || !User.CanDelete("T_FacetedSearch")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            T_FacetedSearch t_facetedsearch = db.T_FacetedSearchs.Find(id);
            if(t_facetedsearch != null)
            {
                if(CheckBeforeDelete(t_facetedsearch))
                {
                    if(!CustomDelete(t_facetedsearch, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(t_facetedsearch).State = EntityState.Deleted;
                        db.T_FacetedSearchs.Remove(t_facetedsearch);
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
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "T_FacetedSearch", User) || !User.CanEdit("T_FacetedSearch") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["T_FacetedSearchParentUrl"] = UrlReferrer;
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
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="collection">     The collection.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,T_Name,T_Description,T_EntityName,T_Roles,T_Disable,T_LinkAddress,T_TargetEntity,T_AssociationName,T_OtherInfo,T_Flag")] T_FacetedSearch t_facetedsearch, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_FacetedSearch target = db.T_FacetedSearchs.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(t_facetedsearch, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
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
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(t_facetedsearch, "BulkUpdate", "");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
