using EntityFramework.DynamicFilters;
using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling entity pages.</summary>
public partial class EntityPageController : BaseController
{
    /// <summary>Indexes.</summary>
    ///
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string HostingEntity, long? Id, string AssociatedType, bool? RenderPartial)
    {
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["Id"] = Id;
        ViewData["AssociatedType"] = AssociatedType;
        List<EntityPage> EntityPageList = new List<EntityPage>();
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name != "MultiTenantExtraAccess" && p.Name != "FileDocument" && p.Name != "MultiTenantLoginSelected" && p.Name != "CustomReports" && p.Name != "CustomReport" && p.Name != "ApiToken" && p.Name != "T_FacetedSearch" && p.Name != "T_Schedule" && p.Name != "EntityPage" && p.Name != "EntityHelpPage" && p.Name != "PropertyHelpPage");
        var lstEntityPage = from s in db.EntityPages
                            select s;
        foreach(var item in entitys)
        {
            if(!User.CanView(item.Name))
                continue;
            EntityPage EntityPageObj = new EntityPage();
            var entid = lstEntityPage.Where(p => p.EntityName == item.Name).ToList();
            if(entid.Count() > 0)
            {
                EntityPageObj.Id = entid.FirstOrDefault().Id;
                EntityPageObj.Disable = entid.FirstOrDefault().Disable;
                EntityPageObj.EnableDataCache = entid.FirstOrDefault().EnableDataCache;
            }
            else
            {
                EntityPageObj.Id = 0;
                EntityPageObj.Disable = true;
                EntityPageObj.EnableDataCache = false;
            }
            EntityPageObj.EntityName = item.Name;
            EntityPageObj.Description = item.DisplayName;
            EntityPageList.Add(EntityPageObj);
        }
        IPagedList<EntityPage> list = EntityPageList.ToPagedList(1, 1000);
        return View(list);
    }
    
    /// <summary>Edits.</summary>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="Description">      The description.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(string UrlReferrer, string HostingEntityName, string Description, bool? Disable, bool? EnableDataCache)
    {
        if(!User.CanEdit("EntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        long? id = 0;
        int count = 0;
        EntityPage entitypage = new EntityPage();
        var lstEntityPage = from s in db.EntityPages
                            select s;
        var entobj = lstEntityPage.Where(p => p.EntityName == HostingEntityName).ToList();
        if(entobj.Count() > 0)
        {
            id = entobj.FirstOrDefault().Id;
            entitypage = db.EntityPages.Find(id);
            count = entitypage.entityofentityhelp.Count();
        }
        if(id == 0)
        {
            entitypage.Id = 0;
            entitypage.EntityName = HostingEntityName;
            entitypage.Description = Description;
            entitypage.Disable = Disable;
            entitypage.EnableDataCache = EnableDataCache;
        }
        if(UrlReferrer != null)
            ViewData["EntityPageParentUrl"] = UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewBag.EntityOfEntityHelpCount = count;
        return View(entitypage);
    }
    /// <summary>Enables the disable rule.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the EnableDisableRule View.</returns>
    ///
    public ActionResult EnableDisableEntityAndSave(string HostingEntityName, string Description, bool? Disable, bool? EnableDataCache, string from)
    {
        if(!User.CanEdit("EntityPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        long? id = 0;
        int count = 0;
        EntityPage entitypage = new EntityPage();
        var lstEntityPage = from s in db.EntityPages
                            select s;
        var entobj = lstEntityPage.Where(p => p.EntityName == HostingEntityName).ToList();
        if(entobj.Count() > 0)
        {
            id = entobj.FirstOrDefault().Id;
            entitypage = db.EntityPages.Find(id);
            count = entitypage.entityofentityhelp.Count();
            EntityPage entityPage = db.EntityPages.Find(id);
            if(from == "help")
                entityPage.Disable = !Disable;
            else
                entityPage.Disable = Disable;
            if(from == "caching")
                entityPage.EnableDataCache = !EnableDataCache;
            else
                entityPage.EnableDataCache = EnableDataCache;
            db.Entry(entityPage).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(id == 0)
        {
            EntityPage entitypageAdd = new EntityPage();
            entitypageAdd.EntityName = HostingEntityName;
            entitypageAdd.Description = Description;
            if(from == "help")
                entitypageAdd.Disable = !Disable;
            else
                entitypageAdd.Disable = Disable;
            if(from == "caching")
                entitypageAdd.EnableDataCache = !EnableDataCache;
            else
                entitypageAdd.EnableDataCache = EnableDataCache;
            db.EntityPages.Add(entitypageAdd);
            db.SaveChanges();
            var entobjforGetId = db.EntityPages.Where(p => p.EntityName == HostingEntityName).ToList();
            if(entobjforGetId.Count() > 0)
                id = entobjforGetId.FirstOrDefault().Id;
        }
        EntityPage entitypage1 = db.EntityPages.Find(id);
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewBag.EntityOfEntityHelpCount = count;
        return RedirectToAction("Edit", new { HostingEntityName = entitypage1.EntityName, Id = id, Description = entitypage1.Description, Disable = entitypage1.Disable,EnableDataCache = entitypage1.EnableDataCache   });
    }
    public ActionResult EnableDisableEntity(int? id)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("EntityPages"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EntityPage entityPage = db.EntityPages.Find(id);
        bool? disable = entityPage.Disable;
        entityPage.Disable = !disable;
        db.Entry(entityPage).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    public ActionResult DisableCachingforAll()
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("EntityPages"))
        {
            return RedirectToAction("Index", "Error");
        }
        List<EntityPage> EntityPageList = new List<EntityPage>();
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name != "MultiTenantExtraAccess" && p.Name != "FileDocument" && p.Name != "MultiTenantLoginSelected" && p.Name != "CustomReports" && p.Name != "CustomReport" && p.Name != "ApiToken" && p.Name != "T_FacetedSearch" && p.Name != "T_Schedule" && p.Name != "EntityPage" && p.Name != "EntityHelpPage" && p.Name != "PropertyHelpPage");
        var lstEntityPage = db.EntityPages.AsNoTracking().ToList();
        foreach(var item in entitys)
        {
            if(!User.CanView(item.Name))
                continue;
            EntityPage EntityPageObj = new EntityPage();
            var obj = lstEntityPage.FirstOrDefault(p => p.EntityName == item.Name);
            if(obj != null)
            {
                obj.EnableDataCache = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        return RedirectToAction("Index");
    }
    public ActionResult EnableCachingforAll()
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("EntityPages"))
        {
            return RedirectToAction("Index", "Error");
        }
        List<EntityPage> EntityPageList = new List<EntityPage>();
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name != "MultiTenantExtraAccess" && p.Name != "FileDocument" && p.Name != "MultiTenantLoginSelected" && p.Name != "CustomReports" && p.Name != "CustomReport" && p.Name != "ApiToken" && p.Name != "T_FacetedSearch" && p.Name != "T_Schedule" && p.Name != "EntityPage" && p.Name != "EntityHelpPage" && p.Name != "PropertyHelpPage");
        var lstEntityPage = db.EntityPages.AsNoTracking().ToList();
        foreach(var item in entitys)
        {
            if(!User.CanView(item.Name))
                continue;
            EntityPage EntityPageObj = new EntityPage();
            var obj = lstEntityPage.FirstOrDefault(p => p.EntityName == item.Name);
            if(obj != null)
            {
                obj.EnableDataCache = true;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                EntityPage objNew = new EntityPage();
                objNew.EntityName = item.Name;
                objNew.Description = item.Name;
                objNew.Disable = true;
                objNew.EnableDataCache = true;
                db.EntityPages.Add(objNew);
                db.SaveChanges();
            }
        }
        return RedirectToAction("Index");
    }
    public ActionResult RefreshCaching()
    {
        CacheHelper.RemoveAllCache();
        return RedirectToAction("Index");
    }
    public ActionResult EnableDisableEntityCaching(int? id, string HostingEntityName)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("EntityPages"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null || id == 0)
        {
            EntityPage entitypageAdd = new EntityPage();
            entitypageAdd.EntityName = HostingEntityName;
            entitypageAdd.Description = "";
            entitypageAdd.Disable = true;
            entitypageAdd.EnableDataCache = true;
            db.EntityPages.Add(entitypageAdd);
            db.SaveChanges();
        }
        else
        {
            EntityPage entityPage = db.EntityPages.Find(id);
            bool? disable = entityPage.EnableDataCache;
            entityPage.EnableDataCache = !disable.HasValue ? true : !disable;
            db.Entry(entityPage).State = EntityState.Modified;
        }
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyValueByEntityId action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var obj1 = db1.EntityPages.Find(Id);
        if(obj1 == null)
            return Json("0", JsonRequestBehavior.AllowGet);
        System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == PropName);
        object PropValue = Property.GetValue(obj1, null);
        PropValue = PropValue == null ? 0 : PropValue;
        return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
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
