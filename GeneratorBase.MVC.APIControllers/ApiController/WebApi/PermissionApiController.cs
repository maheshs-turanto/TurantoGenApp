using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AttributeRouting.Web.Http;
using GeneratorBase.MVC.Models;
using GeneratorBase.MVC.Controllers;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System;
using System.Web.Http.OData;
using System.Data.Entity.SqlServer;
namespace GeneratorBase.MVC.ApiControllers
{
/// <summary>A controller for handling permissions.</summary>
[AuthorizationRequired]
[Microsoft.Web.Http.ApiVersionNeutral]
public class PermissionController : ApiBaseController
{
    /// <summary>GET api/GetPermission.</summary>
    ///
    /// <returns>The user permission.</returns>
    
    public HttpResponseMessage GetUserPermission()
    {
        return Request.CreateResponse(HttpStatusCode.OK, User);
    }
}
}