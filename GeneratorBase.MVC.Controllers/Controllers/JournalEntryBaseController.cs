using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling journal entry bases.</summary>
[NoCache]
public class JournalEntryBaseController : Controller
{
    /// <summary>Gets or sets the database.</summary>
    ///
    /// <value>The database.</value>
    
    public JournalEntryContext db
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Gets or sets the user.</summary>
    ///
    /// <value>The user.</value>
    
    public new IUser User
    {
        get;    //removed static for race condition
        private set;
    }
    
    // public string EntityNameJournal  { get;  set; }
    
    /// <summary>Called when authorization occurs.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnAuthorization(AuthorizationContext filterContext)
    {
        User = base.User as IUser;
        db = new JournalEntryContext(base.User as IUser);
    }
    
    /// <summary>Called before the action method is invoked.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if(!Request.IsAuthenticated)
        {
            // filterContext.Result = new RedirectResult("~/Account/Login");
            var values = new RouteValueDictionary(new
            {
                action = "Login",
                controller = "Account",
                returnUrl = HttpContext.Request.Url.PathAndQuery
            });
            var result = new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Bad Request");
            if(Request.IsAjaxRequest())
                filterContext.Result = result;
            else
                filterContext.Result = new RedirectToRouteResult(values);
            return;
        }
        string entity = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        if(User.CanView(entity))
        {
            base.OnActionExecuting(filterContext);
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Error");
        }
        base.OnActionExecuting(filterContext);
    }
    
}

}