using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling preloads.</summary>
public class PreloadController : Controller
{
    /// <summary>GET: /Preload/.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    [AllowAnonymous]
    public ActionResult Index()
    {
        return View();
    }
    
    /// <summary>GET: /Preload/Start.</summary>
    ///
    /// <returns>A response stream to send to the Start View.</returns>
    
    [AllowAnonymous]
    public ActionResult Start()
    {
        var warmup = new AppWarmUp();
        warmup.Preload(new string[] {});
        return Content("OK!");
    }
}
}
