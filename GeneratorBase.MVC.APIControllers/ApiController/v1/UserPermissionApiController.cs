using GeneratorBase.MVC.Controllers;
using GeneratorBase.MVC.Models;
using Microsoft.Web.Http;
using NSwag.Annotations;
using System.Collections.Generic;
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
    [Route("api/v{version:apiVersion}/UserPermission")]
    public IHttpActionResult GetUserPermission()
    {
        return Ok(this.User);
    }
}
}