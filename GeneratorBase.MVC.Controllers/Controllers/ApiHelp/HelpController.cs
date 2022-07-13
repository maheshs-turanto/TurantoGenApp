using GeneratorBase.MVC.Models;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>The controller that will handle requests for the help page.</summary>
[Authorize]
public class ApiHelpController : BaseController
{
    /// <summary>Add new Configuration Property.</summary>
    ///
    /// <value>The configuration.</value>
    protected static HttpConfiguration Configuration
    {
        get
        {
            return GlobalConfiguration.Configuration;
        }
    }
    
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A System.Web.Mvc.ActionResult.</returns>
    public System.Web.Mvc.ActionResult Index()
    {
        ApplicationDbContext userdb = new ApplicationDbContext();
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var userinfo = userdb.Users.FirstOrDefault(p => p.UserName == User.Name);
        if(userinfo != null)
        {
            var tokeninfo = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userinfo.Id && p.T_ExpiresOn >= DateTime.UtcNow);
            if(tokeninfo != null)
                ViewData["Token"] = tokeninfo.T_AuthToken;
            else
            {
                var token = GenerateToken.GetToken(userinfo.Id);
                ViewData["Token"] = token.T_AuthToken;
            }
        }
        string version = Configuration.GetApiVersioningOptions().DefaultApiVersion.ToString();
        ViewData["version"] = version;
        Collection<ApiDescription> apiDescriptionsLst = new Collection<ApiDescription>();
        Collection<ApiDescription> apiDescriptions = Configuration.Services.GetApiExplorer().ApiDescriptions;
        //Configuration.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerSelector), new NamespaceHttpControllerSelector(Configuration));
        foreach(ApiDescription apiDescription in apiDescriptions)
        {
            if(apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName != ((ControllerName)0).ToString() &&
                    apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName != ((ControllerName)1).ToString() &&
                    apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName != ((ControllerName)2).ToString() &&
                    apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName != ((ControllerName)3).ToString())
            {
                apiDescription.RelativePath = apiDescription.RelativePath.Replace("{version}", version);
                apiDescriptionsLst.Add(apiDescription);
            }
        }
        return View(apiDescriptionsLst);
    }
    
    /// <summary>Apis.</summary>
    ///
    /// <param name="apiId">  Identifier for the API.</param>
    /// <param name="Token">  The token.</param>
    /// <param name="version">The version.</param>
    ///
    /// <returns>A System.Web.Mvc.ActionResult.</returns>
    public System.Web.Mvc.ActionResult Api(string apiId, string Token, string version)
    {
        ViewData["Token"] = Token;
        ViewData["version"] = version;
        if(!String.IsNullOrEmpty(apiId))
        {
            HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId, version);
            if(apiModel != null)
            {
                return View(apiModel);
            }
        }
        return View("Error");
    }
    
    enum ControllerName
    {
        SwitchRolesApi,
        SwitchTenantsApi,
        TenantApi,
        UserPermissionApi
    }
}
}