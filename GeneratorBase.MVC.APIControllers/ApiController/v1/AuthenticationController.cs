using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using NSwag.Annotations;
using System.Data.Entity;
using Microsoft.Web.Http;
using System.Threading.Tasks;
using GeneratorBase.MVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeneratorBase.MVC.ApiControllers.v1
{
/// <summary>A controller for handling authentications.</summary>
[ApiVersion("1.0")]
public partial class AuthenticationController : ApiController
{
    /// <summary>(An Action that handles HTTP POST requests) forgot password.</summary>
    ///
    /// <param name="name">The name.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/v{version:apiVersion}/Authentication/forgot")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string))]
    public async Task<IHttpActionResult> ForgotPassword([Required][FromBody] string name)
    {
        var helper = await AuthenticationHelper.Create(name);
        var hasUser = helper.HasUser();
        if(!hasUser) return Ok();   // don't let bad guys know the user doesn't exist
        //var notLockedOut = helper.NotLockedOut();
        //if (!notLockedOut) return Unauthorized(notLockedOut);
        var user = helper.User;
        string code = await helper.UserManager.GeneratePasswordResetTokenAsync(user.Id);
        var db = new ApplicationContext(new SystemUser());
        SendEmail email = new SendEmail();
        var template = db.EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Forgot Password");
        if(template == null) template = new EmailTemplate();   // go with defaults?
        string subject = CommonFunction.Instance.AppName() + ": a password reset has been requested";
        subject = !String.IsNullOrEmpty(template.EmailSubject) ? template.EmailSubject : subject;
        var uiServer = System.Configuration.ConfigurationManager.AppSettings["UIServer"];
        var url = uiServer + "/reset?user=" + user.UserName + "&token=" + System.Web.HttpUtility.UrlEncode(code);
        string body = String.Format("Click <a href='{0}'>here</a> to reset your password.", url);
        if(!string.IsNullOrEmpty(template.EmailContent))
        {
            try
            {
                var temp = template.EmailContent;
                temp = body.Replace("###FullName###", user.FirstName + " " + user.LastName)
                       .Replace("###Username###", user.UserName)
                       .Replace("###Password###", "")
                       .Replace("###URL###", " <a href='" + url + "'>here</a>")
                       .Replace("###AppName###", CommonFunction.Instance.AppName());
                body = temp;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        email.Notify(user.Id, user.Email, body, subject);
        return Ok();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) resets the password.</summary>
    ///
    /// <param name="name">       The name.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="token">      The token.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/v{version:apiVersion}/Authentication/reset")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string))]
    public async Task<IHttpActionResult> ResetPassword([Required][FromBody] ResetPasswordViewModel vm)
    {
        string name = vm.Name, token = vm.Token, newPassword = vm.NewPassword;
        if(String.IsNullOrEmpty(name) || String.IsNullOrEmpty(token) || String.IsNullOrEmpty(newPassword)) return BadRequest();
        var helper = await AuthenticationHelper.Create(name);
        var hasUser = helper.HasUser();
        if(!hasUser) return Ok();
        var validToken = await helper.UserManager.VerifyUserTokenAsync(helper.User.Id, "ResetPassword", token);
        if(!validToken) return BadRequest();
        var newPasswordIsValid = await helper.NewPasswordIsValid(newPassword);
        if(!newPasswordIsValid) return BadRequest(newPasswordIsValid);
        var passwordNotRecentlyUsed = await helper.PasswordNotRecentlyUsed(newPassword);
        if(!passwordNotRecentlyUsed) return BadRequest(passwordNotRecentlyUsed);
        var result = await helper.UserManager.ResetPasswordAsync(helper.User.Id, token, newPassword);
        if(!result.Succeeded) return BadRequest();
        helper.SaveNewPasswordInHistory();
        return Ok();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) change password.</summary>
    ///
    /// <param name="name">       The name.</param>
    /// <param name="password">   The password.</param>
    /// <param name="newPassword">The new password.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpPost]
    [Route("api/v{version:apiVersion}/Authentication/change")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string))]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string))]
    public async Task<IHttpActionResult> ChangePassword([Required][FromBody] ChangePasswordViewModel vm)
    {
        string name = vm.Name, password = vm.Password, newPassword = vm.NewPassword;
        var helper = await AuthenticationHelper.Create(name);
        var hasUser = helper.HasUser();
        if(!hasUser) return Ok();
        var notLockedOut = helper.NotLockedOut();
        if(!notLockedOut) return Unauthorized(notLockedOut);
        var passwordIsCorrect = await helper.UserManager.CheckPasswordAsync(helper.User, password);
        if(!passwordIsCorrect) return Unauthorized();
        var newPasswordIsValid = await helper.NewPasswordIsValid(newPassword);
        if(!newPasswordIsValid) return BadRequest(newPasswordIsValid);
        var passwordNotRecentlyUsed = await helper.PasswordNotRecentlyUsed(newPassword);
        if(!passwordNotRecentlyUsed) return BadRequest(passwordNotRecentlyUsed);
        var result = await helper.UserManager.ChangePasswordAsync(helper.User.Id, password, newPassword);
        if(!result.Succeeded) return Unauthorized();
        helper.SaveNewPasswordInHistory();
        return Ok();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) login.</summary>
    ///
    /// <param name="UserName">Name of the user.</param>
    /// <param name="Password">The password.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/v{version:apiVersion}/Authentication/login")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(ApiToken))]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string))]
    public async Task<IHttpActionResult> Login([Required][FromBody] LoginViewModel vm)
    {
        string name = vm.Name, password = vm.Password;
        var helper = await AuthenticationHelper.Create(name);
        var hasUser = helper.HasUser();
        if(!hasUser) return Unauthorized(hasUser);
        var notLockedOut = helper.NotLockedOut();
        if(!notLockedOut) return Unauthorized(notLockedOut);
        var passwordIsCorrect = await helper.PasswordIsCorrectOrIncrementAsync(password);
        if(!passwordIsCorrect) return Unauthorized(passwordIsCorrect);
        var passwordHasNotExpired = await helper.PasswordHasNotExpiredAsync();
        if(!passwordHasNotExpired) return Unauthorized(passwordHasNotExpired);
        await helper.ConfirmSuccessfulLogin();
        var token = await helper.LoginApiAsync();
        return Ok(token);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) logout.</summary>
    ///
    /// <param name="token">The token.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpPost]
    [Route("api/v{version:apiVersion}/Authentication/logout")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
    public async Task<IHttpActionResult> Logout()
    {
        IEnumerable<string> tokenHeaders;
        if(!this.Request.Headers.TryGetValues("Token", out tokenHeaders) ||
                !tokenHeaders.Any()) return Ok();
        var token = tokenHeaders.First();
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var apiToken = await db.ApiTokens.FirstOrDefaultAsync(p => p.T_AuthToken == token);
        if(apiToken != null)
        {
            apiToken.T_ExpiresOn = DateTime.UtcNow;
            db.SaveChanges();
        }
        return Ok();
    }
    
    /// <summary>Bad request.</summary>
    ///
    /// <param name="result">The result.</param>
    ///
    /// <returns>An IHttpActionResult.</returns>
    
    private IHttpActionResult BadRequest(AuthenticationHelper.AuthenticationResult result)
    {
        return StatusCode(HttpStatusCode.BadRequest, result);
    }
    
    /// <summary>Unauthorized the given result.</summary>
    ///
    /// <param name="result">The result.</param>
    ///
    /// <returns>An IHttpActionResult.</returns>
    
    private IHttpActionResult Unauthorized(AuthenticationHelper.AuthenticationResult result)
    {
        return StatusCode(HttpStatusCode.Unauthorized, result);
    }
    
    /// <summary>Status code.</summary>
    ///
    /// <param name="code">  The code.</param>
    /// <param name="result">The result.</param>
    ///
    /// <returns>An IHttpActionResult.</returns>
    
    private IHttpActionResult StatusCode(HttpStatusCode code, AuthenticationHelper.AuthenticationResult result)
    {
        var obj = result.Value == null ?
                  (object)new { message = result.Message } :
                  (object)new { message = result.Message, value = result.Value };
        var json = JsonConvert.SerializeObject(obj);
        var response = new HttpResponseMessage(code);
        response.Content = new StringContent(json, System.Text.Encoding.Default, "application/json");
        return ResponseMessage(response);
    }
    
    public class ResetPasswordViewModel
    {
        [Required]
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
        [Required]
        [JsonProperty("token")]
        public string Token
        {
            get;
            set;
        }
        [Required]
        [JsonProperty("newPassword")]
        public string NewPassword
        {
            get;
            set;
        }
    }
    
    public class ChangePasswordViewModel
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
        [Required]
        [JsonProperty("newPassword")]
        public string NewPassword
        {
            get;
            set;
        }
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