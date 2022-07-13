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
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling property mappings.</summary>
public partial class PropertyMappingController : BaseController
{
    /// <summary>GET: /PropertyMapping/.</summary>
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
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate)
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
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstPropertyMapping = from s in db.PropertyMappings
                                 select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstPropertyMapping = searchRecords(lstPropertyMapping, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstPropertyMapping = sortRecords(lstPropertyMapping, sortBy, isAsc);
        }
        else lstPropertyMapping = lstPropertyMapping.OrderByDescending(c => c.Id);
        int pageSize = 50;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        var _PropertyMapping = lstPropertyMapping.Include(t => t.entitypropertymapping);
        if(HostingEntity == "EntityDataSource" && AssociatedType == "EntityPropertyMapping")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _PropertyMapping = _PropertyMapping.Where(p => p.EntityPropertyMappingID == hostid);
            }
            else
                _PropertyMapping = _PropertyMapping.Where(p => p.EntityPropertyMappingID == null);
        }
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_PropertyMapping.Count() > 0)
                pageSize = _PropertyMapping.Count();
            return View("ExcelExport", _PropertyMapping.ToPagedList(pageNumber, pageSize));
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_PropertyMapping.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
            {
                ViewData["BulkAssociate"] = BulkAssociate;
                if(caller != "")
                {
                    FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                    _PropertyMapping = _fad.FilterDropdown<PropertyMapping>(User, _PropertyMapping, "PropertyMapping", caller);
                }
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstPropertyMapping.Except(_PropertyMapping), sortBy, isAsc).ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstPropertyMapping.Except(_PropertyMapping).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", _PropertyMapping.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", _PropertyMapping.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                }
            }
            else
                return PartialView("IndexPartial", _PropertyMapping.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>GET: /PropertyMapping/Details/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyMapping propertymapping = db.PropertyMappings.Find(id);
        if(propertymapping == null)
        {
            return HttpNotFound();
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(propertymapping);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(propertymapping, AssociatedType);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/Create.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDAdd">          The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
    {
        if(!User.CanAdd("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View();
    }
    
    /// <summary>GET: /PropertyMapping/CreateWizard.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        if(!User.CanAdd("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View();
    }
    
    /// <summary>POST: /PropertyMapping/CreateWizard To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the CreateWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
    {
        CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            db.PropertyMappings.Add(propertymapping);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        LoadViewDataAfterOnCreate(propertymapping);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/CreateQuick.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
    {
        if(!User.CanAdd("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        var HostingObject = db.EntityDataSources.Find(Convert.ToInt64(HostingEntityID));
        var EntityName= string.Empty;
        if(HostingObject != null)
        {
            EntityName = HostingObject.EntityName;
            var properymappings = db.PropertyMappings.Where(p => p.EntityPropertyMappingID == HostingObject.Id);
            Dictionary<GeneratorBase.MVC.ModelReflector.Property, string> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, string>();
            // var col = new List<SelectListItem>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == EntityName);
            if(entList != null)
            {
                var properties = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "DeleteDateTime" && p.Name != "IsDeleted").ToList();
                properties.Add(new ModelReflector.Property {DisplayName="Id (Primary Key)", Name = "Id", IsRequired= true });
                foreach(var prop in properties.OrderBy(p=>p.DisplayName))
                {
                    var selectedVal = "";
                    var map = properymappings.FirstOrDefault(p => p.PropertyName == prop.Name);
                    if(map != null)
                        selectedVal = map.DataName;
                    else
                        selectedVal = prop.Name;
                    // objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                    objColMap.Add(prop, selectedVal);
                }
            }
            ViewBag.ColumnMapping = objColMap;
        }
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View();
    }
    
    /// <summary>POST: /PropertyMapping/CreateQuick To protect from overposting attacks, please enable
    /// the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping"> The propertymapping.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the CreateQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        //CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            var properties = propertymapping.PropertyName.Split(",".ToCharArray());
            var jsonproperties = propertymapping.DataName.Split(",".ToCharArray());
            if(properties.Count() == jsonproperties.Count())
            {
                foreach(var item in db.PropertyMappings.Where(p=>p.EntityPropertyMappingID== propertymapping.EntityPropertyMappingID).ToList())
                {
                    db.Entry(item).State = EntityState.Deleted;
                    db.PropertyMappings.Remove(item);
                    db.SaveChanges();
                }
                for(var i = 0; i < properties.Count(); i++)
                {
                    if(!string.IsNullOrEmpty(properties[i].Trim()))
                    {
                        var obj = new PropertyMapping();
                        obj.EntityPropertyMappingID = propertymapping.EntityPropertyMappingID;
                        obj.PropertyName = properties[i].Trim();
                        obj.DataName = jsonproperties[i].Trim();
                        obj.SourceType = propertymapping.SourceType;
                        obj.MethodType = propertymapping.MethodType;
                        db.PropertyMappings.Add(obj);
                        db.SaveChanges();
                    }
                }
            }
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
        LoadViewDataAfterOnCreate(propertymapping);
        if(!string.IsNullOrEmpty(AssociatedEntity))
            LoadViewDataForCount(propertymapping, AssociatedEntity);
        return View(propertymapping);
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
    
    /// <summary>POST: /PropertyMapping/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    /// <param name="IsDDAdd">        The is dd add.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsDDAdd)
    {
        CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.PropertyMappings.Add(propertymapping);
            db.SaveChanges();
            if(command == "Create & Continue")
                return RedirectToAction("Edit", new { Id = propertymapping.Id, UrlReferrer = UrlReferrer });
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else return RedirectToAction("Index");
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(propertymapping);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyMapping propertymapping = db.PropertyMappings.Find(id);
        if(propertymapping == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        if(ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Edit/" + propertymapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Create"))
            ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(propertymapping);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View(propertymapping);
    }
    
    /// <summary>POST: /PropertyMapping/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping"> The propertymapping.</param>
    /// <param name="UrlReferrer">     The URL referrer.</param>
    /// <param name="IsAddPop">        The is add pop.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(propertymapping).State = EntityState.Modified;
            db.SaveChanges();
            return Json(UrlReferrer, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        LoadViewDataAfterOnEdit(propertymapping);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/Edit/5.</summary>
    ///
    /// <param name="id">               The Identifier to delete.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
    {
        if(!User.CanEdit("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyMapping propertymapping = db.PropertyMappings.Find(id);
        if(propertymapping == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        if(ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Edit/" + propertymapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Create"))
            ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        LoadViewDataBeforeOnEdit(propertymapping);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View(propertymapping);
    }
    
    /// <summary>POST: /PropertyMapping/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
    {
        CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            db.Entry(propertymapping).State = EntityState.Modified;
            db.SaveChanges();
            if(command != "Save")
                return RedirectToAction("Edit", new { Id = propertymapping.Id, UrlReferrer = UrlReferrer });
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
        LoadViewDataAfterOnEdit(propertymapping);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/EditWizard/5.</summary>
    ///
    /// <param name="id">         The Identifier to delete.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    public ActionResult EditWizard(int? id, string UrlReferrer)
    {
        if(!User.CanEdit("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyMapping propertymapping = db.PropertyMappings.Find(id);
        if(propertymapping == null)
        {
            return HttpNotFound();
        }
        if(UrlReferrer != null)
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
        if(ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping"))
            ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
        LoadViewDataBeforeOnEdit(propertymapping);
        ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
        return View(propertymapping);
    }
    
    /// <summary>POST: /PropertyMapping/EditWizard/5 To protect from overposting attacks, please
    /// enable the specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the EditWizard View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
    {
        CheckBeforeSave(propertymapping);
        if(ModelState.IsValid)
        {
            db.Entry(propertymapping).State = EntityState.Modified;
            db.SaveChanges();
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
        LoadViewDataAfterOnEdit(propertymapping);
        return View(propertymapping);
    }
    
    /// <summary>GET: /PropertyMapping/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PropertyMapping propertymapping = db.PropertyMappings.Find(id);
        if(propertymapping == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping"))
            ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
        return View(propertymapping);
    }
    
    /// <summary>POST: /PropertyMapping/Delete/5.</summary>
    ///
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(PropertyMapping propertymapping, string UrlReferrer)
    {
        if(!User.CanDelete("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(CheckBeforeDelete(propertymapping))
        {
            //Delete Document
            db.Entry(propertymapping).State = EntityState.Deleted;
            db.PropertyMappings.Remove(propertymapping);
            db.SaveChanges();
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if(ViewData["PropertyMappingParentUrl"] != null)
            {
                string parentUrl = ViewData["PropertyMappingParentUrl"].ToString();
                ViewData["PropertyMappingParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
        return View(propertymapping);
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
        if(HostingEntity == "EntityDataSource" && AssociatedType == "EntityPropertyMapping")
        {
            foreach(var id in ids.Where(p => p > 0))
            {
                PropertyMapping obj = db.PropertyMappings.Find(id);
                db.Entry(obj).State = EntityState.Modified;
                obj.EntityPropertyMappingID = HostingID;
                db.SaveChanges();
            }
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
        foreach(var id in ids.Where(p => p > 0))
        {
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            db.Entry(propertymapping).State = EntityState.Deleted;
            db.PropertyMappings.Remove(propertymapping);
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
        if(!User.CanEdit("PropertyMapping"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData["PropertyMappingParentUrl"] = UrlReferrer;
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
    /// <param name="propertymapping">The propertymapping.</param>
    /// <param name="collection">     The collection.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                PropertyMapping target = db.PropertyMappings.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(propertymapping, target, chkUpdate);
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
