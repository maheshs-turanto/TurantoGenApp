using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace GeneratorBase.MVC
{
/// <summary>A route configuration.</summary>
public class RouteConfig
{
    /// <summary>Registers the routes described by routes.</summary>
    ///
    /// <param name="routes">The routes.</param>
    
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Preload", action = "Index", id = UrlParameter.Optional }
        );
    }
}
}
