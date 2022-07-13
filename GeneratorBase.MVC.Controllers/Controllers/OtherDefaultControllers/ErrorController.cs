using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling errors.</summary>

public class ErrorController : Controller
{
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    [Audit("Unauthorized access")]
    public ActionResult Index()
    {
        return View();
    }
    
    /// <summary>Activation link.</summary>
    ///
    /// <returns>A response stream to send to the ActivationLink View.</returns>
    
    public ActionResult ActivationLink()
    {
        return View();
    }
    
    /// <summary>Concurrency error.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="Message">    The message.</param>
    ///
    /// <returns>A response stream to send to the ConcurrencyError View.</returns>
    [Audit("ConcurrencyError")]
    public ActionResult ConcurrencyError(string UrlReferrer, string Message)
    {
        ViewData["ConcurrencyReferrer"] = UrlReferrer;
        ViewData["ConcurrencyMessage"] = Message;
        return View();
    }
    
    /// <summary>Concurrency button.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the ConcurrencyButton View.</returns>
    
    public ActionResult ConcurrencyButton(string UrlReferrer)
    {
        return Redirect(UrlReferrer);
        // return View();
    }
    
    /// <summary>Not found 404.</summary>
    ///
    /// <returns>A response stream to send to the NotFound404 View.</returns>
    
    public ActionResult NotFound404()
    {
        return View("NotFound404");
    }
    
    /// <summary>Under maintenance.</summary>
    ///
    /// <returns>A response stream to send to the UnderMaintenance View.</returns>
    [Audit("UnderMaintenanceError")]
    public ActionResult UnderMaintenance()
    {
        return View();
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
}