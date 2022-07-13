using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GeneratorBase.MVC.ApiControllers
{
/// <summary>Attribute for concurrency exception filter.</summary>
public class ConcurrencyExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <summary>Raises the exception event.</summary>
    ///
    /// <param name="actionExecutedContext">The context for the action.</param>
    
    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
        if(actionExecutedContext.Exception is DbUpdateConcurrencyException)
        {
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Conflict);
        }
    }
}
}