using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using GeneratorBase.MVC.Controllers;
using GeneratorBase.MVC.Models;
using System.Data.Entity;
using Microsoft.Web.Http;

namespace GeneratorBase.MVC.ApiControllers.v1
{
[ApiVersion("1.0")]
public class SwitchRolesApiController : ApiBaseControllerv1
{
    /// <summary>Switch role of logged-in user.</summary>
    //[Audit("SwitchRole")]
    [HttpGet]
    [Route("api/v{version:apiVersion}/SwitchRole")]
    public HttpResponseMessage SwitchRole(string username)
    {
        using(ApplicationDbContext ac = new ApplicationDbContext(true))
        {
            var oldAccess = ac.LoginSelectedRoles.FirstOrDefault(p => p.User == username);
            if(oldAccess != null)
            {
                ac.LoginSelectedRoles.Remove(oldAccess);
                ac.SaveChanges();
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }
    
    [HttpGet]
    [Route("api/v{version:apiVersion}/SwitchRole")]
    public HttpResponseMessage SwitchRole(string key, string username)
    {
        using(ApplicationDbContext ac = new ApplicationDbContext(true))
        {
            var oldAccess = ac.LoginSelectedRoles.AsNoTracking().FirstOrDefault(p => p.User == username);
            if(oldAccess != null)
            {
                key = oldAccess.Roles;
            }
            else
            {
                if(!string.IsNullOrEmpty(key))
                {
                    LoginSelectedRoles obj = new LoginSelectedRoles();
                    obj.Roles = key;
                    obj.User = username;
                    ac.LoginSelectedRoles.Add(obj);
                    ac.SaveChanges();
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}
}
