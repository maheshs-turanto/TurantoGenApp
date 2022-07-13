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
using EntityFramework.DynamicFilters;
namespace GeneratorBase.MVC.Controllers
{

/// <summary>A controller for handling entity help pages.</summary>
public partial class EntityHelpPageController : BaseController
{
    /// <summary>GET: /EntityHelpPage/.</summary>
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
    /// <param name="ShowdDeleted">   (Optional) True if showd deleted.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowdDeleted = false)
    {
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        if(ShowdDeleted)
        {
            db.DisableFilter("IsDeleted");
            db.EnableFilter("Recycle");
            ViewData["ShowdDeleted"] = ShowdDeleted;
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
        var lstEntityHelpPage = from s in db.EntityHelpPages
                                select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstEntityHelpPage = searchRecords(lstEntityHelpPage, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstEntityHelpPage = sortRecords(lstEntityHelpPage, sortBy, isAsc);
        }
        else lstEntityHelpPage = lstEntityHelpPage.OrderByDescending(c => c.Id);
        lstEntityHelpPage = CustomSorting(lstEntityHelpPage, HostingEntity, AssociatedType, sortBy, isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "EntityHelpPage"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        var _EntityHelpPage = lstEntityHelpPage.Include(t => t.entityofentityhelp);
        if(HostingEntity == "EntityPage" && AssociatedType == "EntityOfEntityHelp")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _EntityHelpPage = _EntityHelpPage.Where(p => p.EntityOfEntityHelpID == hostid);
            }
            else
                _EntityHelpPage = _EntityHelpPage.Where(p => p.EntityOfEntityHelpID == null);
        }
        if(Convert.ToBoolean(IsExport))
        {
            if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "EntityHelpPage", User) || !User.CanView("EntityHelpPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            pageNumber = 1;
            if(_EntityHelpPage.Count() > 0)
                pageSize = _EntityHelpPage.Count();
            return View("ExcelExport", _EntityHelpPage.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _EntityHelpPage.Count();
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
            var list = _EntityHelpPage.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityEntityHelpPageDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["EntityHelpPagelist"] = list.Select(z => new
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
                    _EntityHelpPage = _fad.FilterDropdown<EntityHelpPage>(User, _EntityHelpPage, "EntityHelpPage", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstEntityHelpPage.Except(_EntityHelpPage), sortBy, isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstEntityHelpPage.Except(_EntityHelpPage).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _EntityHelpPage.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _EntityHelpPage.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _EntityHelpPage.Count() == 0 ? 1 : _EntityHelpPage.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _EntityHelpPage.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityEntityHelpPageDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["EntityHelpPagelist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _EntityHelpPage.Count() == 0 ? 1 : _EntityHelpPage.Count();
                    }
                    var list = _EntityHelpPage.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityEntityHelpPageDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["EntityHelpPagelist"] = list.Select(z => new
                    {
                        ID = z.Id, DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    
    /// <summary>GET: /EntityHelpPage/Create.</summary>
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
        if(!User.CanAdd("EntityHelpPage") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        //if (string.IsNullOrEmpty(viewtype))
        viewtype = "Create";
        ViewBag.TemplatesName = "Create";
        ViewData["EntityHelpPageParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType, false);
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnCreate");
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    /// <summary>POST: /EntityHelpPage/Create.</summary>
    ///
    /// <param name="entityhelppage">The entityhelppage.</param>
    /// <param name="UrlReferrer">   The URL referrer.</param>
    /// <param name="IsDDAdd">       The is dd add.</param>
    /// <param name="RenderPartial"> (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityOfEntityHelpID,SectionName,Order,SectionText,Disable,EntityName")] EntityHelpPage entityhelppage, string UrlReferrer, bool? IsDDAdd, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(entityhelppage, command);
        if(ModelState.IsValid)
        {
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(entityhelppage, out customcreate_hasissues, command))
            {
                db.EntityHelpPages.Add(entityhelppage);
                db.SaveChanges();
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(entityhelppage, "Create", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = entityhelppage.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = entityhelppage.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnCreate(entityhelppage);
        ViewData["EntityHelpPageParentUrl"] = UrlReferrer;
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnCreate");
        return View(entityhelppage);
    }
    
    /// <summary>GET: /EntityHelpPage/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="fromHome">         True to from home.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype, bool? fromHome)
    {
        if(!User.CanAdd("EntityHelpPage") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EntityHelpPageParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["fromHome"] = fromHome;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType, fromHome);
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnCreate");
        return View();
    }
    
    /// <summary>POST: /EntityHelpPage/CreateQuick.</summary>
    ///
    /// <param name="entityhelppage">   The entityhelppage.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="fromHome">         True to from home.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityOfEntityHelpID,SectionName,Order,SectionText,Disable,EntityName")] EntityHelpPage entityhelppage, string UrlReferrer, bool? IsAddPop, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool fromHome)
    {
        CheckBeforeSave(entityhelppage);
        if(ModelState.IsValid)
        {
            if(fromHome)
            {
                List<EntityHelpPage> entityhelpDup = db.EntityHelpPages.Where(p => p.SectionName.ToLower().Trim() == entityhelppage.SectionName.ToLower().Trim().ToString() && p.EntityName == entityhelppage.EntityName).ToList();
                if(entityhelpDup.Count() > 0)
                {
                    return Json("duplicate", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                entityhelppage.EntityOfEntityHelpID = AddEntityForHelp(HostingEntityName);
                entityhelppage.EntityName = HostingEntityName;
            }
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(entityhelppage, out customcreate_hasissues, "Create"))
            {
                db.EntityHelpPages.Add(entityhelppage);
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
        LoadViewDataAfterOnCreate(entityhelppage);
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnCreate", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnCreate");
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(entityhelppage, AssociatedEntity);
        return View(entityhelppage);
    }
    
    /// <summary>Adds an entity for help.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    ///
    /// <returns>A long.</returns>
    
    public long AddEntityForHelp(string HostingEntityName)
    {
        long id = 0;
        var lstEntityPage = from s in db.EntityPages
                            select s;
        var entobj = lstEntityPage.Where(p => p.EntityName == HostingEntityName).ToList();
        if(entobj.Count() > 0)
            id = entobj.FirstOrDefault().Id;
        if(id == 0)
        {
            EntityPage entitypageAdd = new EntityPage();
            entitypageAdd.EntityName = HostingEntityName;
            db.EntityPages.Add(entitypageAdd);
            db.SaveChanges();
            var entobjforGetId = db.EntityPages.Where(p => p.EntityName == HostingEntityName).ToList();
            if(entobjforGetId.Count() > 0)
                id = entobjforGetId.FirstOrDefault().Id;
        }
        return id;
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
                return RedirectToAction("Index");
        }
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>GET: /EntityHelpPage/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RenderPartial = false)
    {
        if(!User.CanEdit("EntityHelpPage") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EntityHelpPage entityhelppage = db.EntityHelpPages.Find(id);
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["EntityHelpPagelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.EntityHelpPages.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityEntityHelpPageDisplayValueEdit = TempData["EntityHelpPagelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["EntityHelpPagelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityEntityHelpPageDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(entityhelppage == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = "Edit";
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/EntityHelpPage/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = entityhelppage.DisplayValue, Value = entityhelppage.Id.ToString() }));
            ViewBag.EntityEntityHelpPageDisplayValueEdit = newList;
            TempData["EntityHelpPagelist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(entityhelppage.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = entityhelppage.DisplayValue;
                newList[0].Value = entityhelppage.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = entityhelppage.DisplayValue, Value = entityhelppage.Id.ToString() }));
            }
            ViewBag.EntityEntityHelpPageDisplayValueEdit = newList;
            TempData["EntityHelpPagelist"] = newList.Select(z => new
            {
                ID = z.Value, DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["EntityHelpPageParentUrl"] = UrlReferrer;
        if(ViewData["EntityHelpPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityHelpPage") && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityHelpPage/Edit/" + entityhelppage.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityHelpPage/Create"))
            ViewData["EntityHelpPageParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(entityhelppage);
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnEdit", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnEdit", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnEdit");
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, entityhelppage);
    }
    
    /// <summary>POST: /EntityHelpPage/Edit/5.</summary>
    ///
    /// <param name="entityhelppage">The entityhelppage.</param>
    /// <param name="UrlReferrer">   The URL referrer.</param>
    /// <param name="RenderPartial"> (Optional) True to render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityOfEntityHelpID,SectionName,Order,SectionText,Disable,EntityName")] EntityHelpPage entityhelppage, string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(entityhelppage, command);
        if(ModelState.IsValid)
        {
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(entityhelppage, out customsave_hasissues, command))
            {
                db.Entry(entityhelppage).State = EntityState.Modified;
                db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(entityhelppage, "Edit", command);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = entityhelppage.Id };
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
                        return RedirectToAction("Edit", new { Id = entityhelppage.Id, UrlReferrer = UrlReferrer });
                }
                if(!string.IsNullOrEmpty(UrlReferrer))
                {
                    var query = HttpUtility.ParseQueryString(UrlReferrer);
                    if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["EntityHelpPagelist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.EntityHelpPages.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }).ToList());
        }
        else
        {
            ViewBag.EntityEntityHelpPageDisplayValueEdit = TempData["EntityHelpPagelist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["EntityHelpPagelist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityEntityHelpPageDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        LoadViewDataAfterOnEdit(entityhelppage);
        ViewData["EntityHelpPageParentUrl"] = UrlReferrer;
        ViewBag.EntityHelpPageIsHiddenRule = checkHidden("EntityHelpPage", "OnEdit", false);
        ViewBag.EntityHelpPageIsGroupsHiddenRule = checkHidden("EntityHelpPage", "OnEdit", true);
        ViewBag.EntityHelpPageIsSetValueUIRule = checkSetValueUIRule("EntityHelpPage", "OnEdit");
        return View(entityhelppage);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) saves an entity help page for help
    /// text.</summary>
    ///
    /// <param name="entityhelppage">The entityhelppage.</param>
    ///
    /// <returns>A response stream to send to the SaveEntityHelpPageForHelpText View.</returns>
    
    [HttpPost]
    public ActionResult SaveEntityHelpPageForHelpText([Bind(Include = "Id,ConcurrencyKey,SectionText")] EntityHelpPage entityhelppage)
    {
        EntityHelpPage entityhelppage1 = db.EntityHelpPages.Find(entityhelppage.Id);
        entityhelppage1.ConcurrencyKey = entityhelppage.ConcurrencyKey;
        entityhelppage1.SectionText = entityhelppage.SectionText;
        db.Entry(entityhelppage1).State = EntityState.Modified;
        db.SaveChanges();
        var result = new { Result = "Success", Id = entityhelppage.Id };
        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Saves a property value.</summary>
    ///
    /// <param name="id">      The identifier.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">   The value.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyValue View.</returns>
    
    public ActionResult SavePropertyValue(long id, string property, string value)
    {
        if(!User.CanEdit("EntityHelpPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EntityHelpPage entityhelppage = db.EntityHelpPages.Find(id);
        if(entityhelppage == null)
        {
            return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        // business rule before load section
        var businessrule = User.businessrules.Where(p => p.EntityName == "EntityHelpPage").ToList();
        List<int> typelist = new List<int>();
        if((businessrule != null && businessrule.Count > 0))
        {
            var ruleids = businessrule.Select(q => q.Id).ToList();
            typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(entityhelppage).Where(p => p.Key.Contains("FailureMessage"));
                if(validateLockResult.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in validateLockResult)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + " | ";
                    }
                    var result = new { Result = "fail", data = stringResult };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            if(typelist.Contains(4))
            {
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(entityhelppage).Where(p => p.Key.Contains("FailureMessage"));
                if(validateMandatorypropertyResult.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in validateMandatorypropertyResult)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + " | ";
                    }
                    var result = new { Result = "fail", data = stringResult };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // business rule section
        CheckBeforeSave(entityhelppage, "SaveProperty");
        var propertyInfo = entityhelppage.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            entityhelppage.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(entityhelppage, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // business rule onsaving section
        if((businessrule != null && businessrule.Count > 0))
        {
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(entityhelppage, "OnEdit");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in resultBR)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + "  ";
                    }
                    var result = new { Result = "fail", data = stringResult };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            if(typelist.Contains(2))
            {
                var resultBR = GetMandatoryPropertiesDictionary(entityhelppage, "OnEdit");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(dic.Key.Contains("FailureMessage"))
                            BRID += dic.Key.Replace("FailureMessage", "BR");
                        else
                            stringResult += dic.Key + " is Required,";
                    }
                    var result = new { Result = "fail", data = BRID + " : " + stringResult };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // business rule onsaving section
        if(isSave && ValidateModel(entityhelppage))
        {
            bool customsave_hasissues = false;
            string resultvalue = "Success";
            if(!CustomSaveOnEdit(entityhelppage, out customsave_hasissues, "Save"))
            {
                if(propertyName == "SectionName")
                {
                    List<EntityHelpPage> entityhelpDup = db.EntityHelpPages.Where(p => p.SectionName.ToLower().Trim() == value.Trim().ToString() && p.EntityName == entityhelppage.EntityName).ToList();
                    if(entityhelpDup.Count() > 0)
                    {
                        resultvalue = "duplicate";
                    }
                }
                if(resultvalue != "duplicate")
                {
                    db.Entry(entityhelppage).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            var result = new { Result = resultvalue, data = value };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(entityhelppage))
            {
                errors += error.ErrorMessage + ".  ";
            }
            var result = new { Result = "fail", data = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
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
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "EntityHelpPage", User) || !User.CanDelete("EntityHelpPage")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            EntityHelpPage entityhelppage = db.EntityHelpPages.Find(id);
            if(entityhelppage != null)
            {
                if(CheckBeforeDelete(entityhelppage))
                {
                    if(!CustomDelete(entityhelppage, out customdelete_hasissues, "DeleteBulk"))
                    {
                        db.Entry(entityhelppage).State = EntityState.Deleted;
                        db.EntityHelpPages.Remove(entityhelppage);
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
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    /// <param name="appsetting">The Account.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(EntityHelpPage entityhelppage, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
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
