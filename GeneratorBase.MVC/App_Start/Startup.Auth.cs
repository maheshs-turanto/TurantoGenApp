using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
namespace GeneratorBase.MVC
{
public partial class Startup
{
    // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
    internal static IDataProtectionProvider DataProtectionProvider
    {
        get;
        private set;
    }
    public void ConfigureAuth(IAppBuilder app)
    {
        DataProtectionProvider = app.GetDataProtectionProvider();
        GeneratorBase.MVC.Controllers.UserDataProtectionProvider.dataProtectionProvider = DataProtectionProvider;
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(!Convert.ToBoolean(UseActiveDirectory))
        {
            // Enable the application to use a cookie to store information for the signed in user
            //set session time out value from appsetting
            string AppUrl = "TestNewTuranto74v4";
            Int64 TimeOutValue = 525600;
            bool preventmultiplelogin = false;
            using(var context = (new ApplicationContext(new SystemUser())))
            {
                var appSettings = context.AppSettings;
                string timeout = appSettings.Where(p => p.Key == "ApplicationSessionTimeOut").FirstOrDefault().Value;
                var appurlObj = appSettings.Where(p => p.Key == "AppURL").FirstOrDefault();
                if(appurlObj != null)
                    AppUrl = appurlObj.Value;
                var preventmultipleloginObj = appSettings.Where(p => p.Key == "PreventMultipleLogin").FirstOrDefault();
                if(preventmultipleloginObj != null)
                    preventmultiplelogin = !string.IsNullOrEmpty(preventmultipleloginObj.Value) && preventmultipleloginObj.Value.ToLower() == "yes" ? true : false;
                if(!string.IsNullOrEmpty(timeout))
                {
                    if(Int64.TryParse(timeout, out TimeOutValue))
                    {
                        if(TimeOutValue == 0)
                            TimeOutValue = Convert.ToInt64(525600);
                    }
                    else
                        TimeOutValue = 525600;
                }
            }
            //
            var cookieProvider = new CookieAuthenticationProvider();
            cookieProvider.OnApplyRedirect = ctx =>
            {
                if(!ctx.Request.Path.HasValue || !ctx.Request.Path.Value.StartsWith("/api/"))
                {
                    ctx.Response.Redirect(ctx.RedirectUri);
                }
            };
            if(preventmultiplelogin)
                cookieProvider.OnValidateIdentity = ctx =>
            {
                var ret = System.Threading.Tasks.Task.Run(() =>
                {
                    System.Security.Claims.Claim claim = ctx.Identity.FindFirst("SecurityStamp");
                    if(claim != null)
                    {
                        UserManager<ApplicationUser> userManager = new UserManager<Models.ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var user = userManager.FindById(ctx.Identity.GetUserId());
                        // invalidate session, if SecurityStamp has changed
                        if(user != null && user.SecurityStamp != null && user.SecurityStamp != claim.Value)
                        {
                            ctx.RejectIdentity();
                        }
                    }
                });
                return ret;
            };
            var authoption = new CookieAuthenticationOptions
            {
                CookieName = AppUrl,
                ExpireTimeSpan = TimeSpan.FromMinutes(TimeOutValue),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = cookieProvider
                
            };
            app.UseCookieAuthentication(authoption);
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}
}
