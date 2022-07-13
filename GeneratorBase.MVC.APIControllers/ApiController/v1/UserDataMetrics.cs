using GeneratorBase.MVC.Controllers;
using GeneratorBase.MVC.Models;
using Microsoft.Web.Http;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
namespace GeneratorBase.MVC.ApiControllers.v1
{
/// <summary>A controller for handling permissions.</summary>
[AuthorizationRequired]
[ApiVersion("1.0")]
public partial class UserPermissionApiController : ApiBaseControllerv1
{
    /// <summary>GET api/GetPermission.</summary>
    ///
    /// <returns>The user permission.</returns>
    
    //[Route("api/v{version:apiVersion}/UserPermission")]
    //[SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Permission>))]
    //public IHttpActionResult GetUserPermission()
    //{
    //    return Ok(this.User.permissions);
    //}
    [Route("api/v{version:apiVersion}/UserDataMetrics")]
    public IHttpActionResult GetUserDataMetrics()
    {
        db.Configuration.LazyLoadingEnabled = true;
        var listGraph = db.T_DataMetrics.Where(p => p.t_associateddatametrictype.T_Name == "Graph" && !p.T_Hide.Value).OrderBy(p => p.T_DisplayOrder).ToList();
        listGraph = listGraph.Where(p => User.CanView(p.T_EntityName) && (string.IsNullOrEmpty(p.T_Roles) || User.IsInRole(User.userroles, p.T_Roles.Split(",".ToCharArray()))) && p.T_AssociatedFacetedSearchID.HasValue).ToList();
        return Ok(listGraph);
    }
}
}