using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using GeneratorBase.MVC.Controllers;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>Attribute for authorization required.</summary>
public class AuthorizationRequiredAttribute : ActionFilterAttribute
{
    /// <summary>The token.</summary>
    private const string Token = "Token";
    
    /// <summary>Occurs before the action method is invoked.</summary>
    ///
    /// <param name="filterContext">The action context.</param>
    
    public override void OnActionExecuting(HttpActionContext filterContext)
    {
        //  Get API key provider
        TokenServicesController provider = new TokenServicesController();
        if(filterContext.Request.Headers.Contains(Token))
        {
            var tokenValue = filterContext.Request.Headers.GetValues(Token).First();
            // Validate Token
            if(provider != null && !provider.ValidateToken(tokenValue))
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("{ \"message\": \"session\" }", System.Text.Encoding.Default, "application/json")
                };
                filterContext.Response = responseMessage;
            }
        }
        else
        {
            filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("{ \"message\": \"token\" }", System.Text.Encoding.Default, "application/json")
            };
        }
        base.OnActionExecuting(filterContext);
    }
}
}