using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling resource localizations (UI label settings).</summary>
public class ResourceLocalizationController : BaseController
{
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index()
    {
        if(!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
            return RedirectToAction("Index", "Home");
        var model = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name.ToLower() != "filedocument" && (!Enum.IsDefined(typeof(GeneratorBase.MVC.ModelReflector.IgnoreEntities), p.Name))).ToList();
        return View(model);
    }
    
    /// <summary>Resets the given name.</summary>
    ///
    /// <param name="name">The name.</param>
    ///
    /// <returns>A response stream to send to the Reset View.</returns>
    
    public ActionResult Reset(string name)
    {
        var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
        var fileexist = System.IO.File.Exists(filepath + name + ".resx");
        if(fileexist)
        {
            System.IO.File.Delete(filepath + name + ".resx");
            var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            myConfiguration.AppSettings.Settings["ResourceLocalizationChange"].Value = Convert.ToString(DateTime.UtcNow.Ticks);
            myConfiguration.Save();
        }
        return RedirectToAction("Index", "ResourceLocalization");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a new ActionResult.</summary>
    ///
    /// <param name="name">The name.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string name)
    {
        ResourceLocalization model = new ResourceLocalization();
        model.Name = name;
        var Entity = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault).FirstOrDefault(p => p.Name == name);
        var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
        var fileexist = System.IO.File.Exists(filepath + name + ".resx");
        if(fileexist)
        {
            using(System.Resources.ResourceSet resxSet = new System.Resources.ResourceSet(filepath + name + ".resx"))
            {
                ColumnMapping entobj = new ColumnMapping();
                entobj.source = name;
                entobj.target = resxSet.GetString(name);
                model.columnmapping.Add(entobj);
                foreach(var item in Entity.Properties.OrderBy(p => p.DisplayName))
                {
                    if(item.Name == "DisplayValue" || item.Name == "TenantId") continue;
                    var result = resxSet.GetString(item.Name);
                    ColumnMapping obj = new ColumnMapping();
                    obj.source = item.Name;
                    obj.target = result;
                    model.columnmapping.Add(obj);
                }
            }
        }
        else
        {
            ColumnMapping entobj = new ColumnMapping();
            entobj.source = name;
            entobj.target = Entity.DisplayName;
            model.columnmapping.Add(entobj);
            foreach(var item in Entity.Properties.OrderBy(p => p.DisplayName))
            {
                if(item.Name == "DisplayValue" || item.Name == "TenantId") continue;
                ColumnMapping obj = new ColumnMapping();
                obj.source = item.Name;
                obj.target = item.DisplayName;
                model.columnmapping.Add(obj);
            }
        }
        return View(model);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a new ActionResult.</summary>
    ///
    /// <param name="model">The model.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    public ActionResult Create(ResourceLocalization model)
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
            var fileexist = System.IO.File.Exists(filepath + model.Name + ".resx");
            if(fileexist)
            {
                System.IO.File.Delete(filepath + model.Name + ".resx");
                //remove old file
            }
            using(System.Resources.ResourceWriter resxSet = new System.Resources.ResourceWriter(filepath + model.Name + ".resx"))
            {
                foreach(var _model in model.columnmapping)
                {
                    resxSet.AddResource(_model.source, _model.target);
                    //Create new file
                }
            }
            var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            myConfiguration.AppSettings.Settings["ResourceLocalizationChange"].Value = Convert.ToString(DateTime.UtcNow.Ticks);
            myConfiguration.Save();
        }
        return RedirectToAction("Index", "ResourceLocalization");
    }
    //protected override void Dispose(bool disposing)
    //{
    //    if (disposing)
    //    {
    //        db.Dispose();
    //    }
    //    base.Dispose(disposing);
    //}
}

/// <summary>A resource localization.</summary>
public class ResourceLocalization
{
    /// <summary>Default constructor.</summary>
    public ResourceLocalization()
    {
        this.columnmapping = new List<ColumnMapping>();
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="list">The list.</param>
    
    public ResourceLocalization(List<ColumnMapping> list)
        : this()
    {
        foreach(var lst in list)
            this.columnmapping.Add(lst);
    }
    
    /// <summary>Gets or sets the columnmapping.</summary>
    ///
    /// <value>The columnmapping.</value>
    
    public List<ColumnMapping> columnmapping
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    public string Name
    {
        get;
        set;
    }
}
/// <summary>A column mapping.</summary>
public class ColumnMapping
{
    /// <summary>Gets or sets the source for the.</summary>
    ///
    /// <value>The source.</value>
    
    public string source
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Target for the.</summary>
    ///
    /// <value>The target.</value>
    
    public string target
    {
        get;
        set;
    }
}
}
