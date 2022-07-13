using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(GeneratorBase.MVC.App_Start.RazorGeneratorMvcStart), "Start")]
namespace GeneratorBase.MVC.App_Start
{
/// <summary>A razor generator MVC start.</summary>
public static class RazorGeneratorMvcStart
{
    /// <summary>Starts this object.</summary>
    public static void Start()
    {
        var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly)
        {
            UsePhysicalViewsIfNewer = true,
        };
        var theme = System.Configuration.ConfigurationManager.AppSettings["AppTheme"];
        if(theme == null) theme = "Default";
        if(theme == "Angular") theme = "";
        engine.ViewLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        engine.MasterLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        engine.PartialViewLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        //ViewEngines.Engines.Clear();
        //ViewEngines.Engines.Add(engine);
        ViewEngines.Engines.Insert(0, engine);
        // StartPage lookups are done by WebPages.
        VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
    }
}
}
