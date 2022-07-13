using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeneratorBase.MVC.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeneratorBase.MVC.ApiControllers
{
// [AuthorizationRequired]
/// <summary>A controller for handling authentications.</summary>
[Microsoft.Web.Http.ApiVersionNeutral]
public class AuthenticationController : ApiController
{
    /// <summary>Authenticate user.</summary>
    ///
    /// <param name="UserName">Name of the user.</param>
    /// <param name="Password">The password.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    [AllowAnonymous]
    public HttpResponseMessage AuthenticateUser([Required][FromBody] LoginViewModel vm)
    {
        ApplicationDbContext UserDb = new ApplicationDbContext();
        ApplicationContext db = new ApplicationContext(new SystemUser());
        string usesrName = vm.Name;
        var ssoUsers = UserDb.Users.First(p => p.UserName == usesrName);
        //var ssoUsers = UserDb.Users.Where(p => p.UserName == req.Headers.GetValues("UserName").First()).ToList();
        if(ssoUsers != null)
        {
            var user = ssoUsers;
            //var roles = user.Roles;
            PasswordHasher ph = new PasswordHasher();
            //string hashedpwd = ph.HashPassword(Password);
            var result = ph.VerifyHashedPassword(user.PasswordHash, vm.Password);
            if(result.ToString() == "Success")
            {
                var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == user.Id);
                if(obj != null)
                    return Request.CreateResponse(HttpStatusCode.OK, obj.T_AuthToken);
                else
                {
                    var token = GenerateToken.GetToken(user.Id);
                    return Request.CreateResponse(HttpStatusCode.OK, token.T_AuthToken);
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Password !");
        }
        return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Username/Password");
    }
    
    /// <summary>Gets token by user identifier.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The token by user identifier.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/AuthenticationByUserId")]
    public HttpResponseMessage GetTokenByUserId([Required][FromBody] string userId)
    {
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userId);
        if(obj != null)
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        else
        {
            var token = GenerateToken.GetToken(userId);
            return Request.CreateResponse(HttpStatusCode.OK, token);
        }
    }
    
    /// <summary>Gets token by user name password.</summary>
    ///
    /// <param name="UserName">Name of the user.</param>
    /// <param name="Password">The password.</param>
    ///
    /// <returns>The token by user name password.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/AuthenticationByUserNamePassword")]
    public HttpResponseMessage GetTokenByUserNamePassword([Required][FromBody] LoginViewModel vm)
    {
        ApplicationContext db = new ApplicationContext(new SystemUser());
        ApplicationDbContext userdb = new ApplicationDbContext();
        string usesrName = vm.Name;
        var userinfo = userdb.Users.FirstOrDefault(p => p.UserName == usesrName);
        if(userinfo != null)
        {
            var user = userinfo;
            PasswordHasher ph = new PasswordHasher();
            //string hashedpwd = ph.HashPassword(Password);
            var result = ph.VerifyHashedPassword(user.PasswordHash, vm.Password);
            if(result.ToString() == "Success")
            {
                var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userinfo.Id);
                if(obj != null)
                    return Request.CreateResponse(HttpStatusCode.OK, obj);
                else
                {
                    var token = GenerateToken.GetToken(userinfo.Id);
                    return Request.CreateResponse(HttpStatusCode.OK, token);
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Password !");
        }
        return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Username/Password");
    }
    public class LoginViewModel
    {
        [Required]
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
        [Required]
        [JsonProperty("password")]
        public string Password
        {
            get;
            set;
        }
    }
}
}
