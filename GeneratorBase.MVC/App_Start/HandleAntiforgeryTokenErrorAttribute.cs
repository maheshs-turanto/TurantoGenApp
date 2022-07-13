using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GeneratorBase.MVC
{
/// <summary>Attribute for handle antiforgery token error.</summary>
public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
{
    /// <summary>Called when an exception occurs.</summary>
    ///
    /// <param name="filterContext">The action-filter context.</param>
    
    public override void OnException(ExceptionContext filterContext)
    {
        if(filterContext.Exception is HttpAntiForgeryException)
        {
            filterContext.ExceptionHandled = true;
            HttpContext contxt = HttpContext.Current;
            var values = new RouteValueDictionary(new
            {
                action = "Login",
                controller = "Account",
                returnUrl = contxt.Request.Url.PathAndQuery.ToString()
            });
            filterContext.Result = new RedirectToRouteResult(values);
        }
    }
}
}