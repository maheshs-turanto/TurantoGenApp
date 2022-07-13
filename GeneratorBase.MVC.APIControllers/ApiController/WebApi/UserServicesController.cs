using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GeneratorBase.MVC.Models;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling user services.</summary>
public class UserServicesController : Controller
{
    /// <summary>Gets or sets the manager for user.</summary>
    ///
    /// <value>The user manager.</value>
    
    public UserManager<ApplicationUser> UserManager
    {
        get;
        private set;
    }
    
    /// <summary>Authenticates.</summary>
    ///
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    ///
    /// <returns>A string.</returns>
    
    public string Authenticate(string userName, string password)
    {
        var Db = new AuthenticationDbContext();
        var ssoUsers = Db.Users.Where(p => p.UserName == userName).ToList();
        var user = ssoUsers[0];
        PasswordHasher ph = new PasswordHasher();
        //string hashedpwd = ph.HashPassword(password);
        var result = ph.VerifyHashedPassword(user.PasswordHash, password);
        if(result.ToString() == "Success")
        {
            if(user != null && !string.IsNullOrEmpty(user.Id))
            {
                return user.Id;
            }
        }
        return null;
    }
}
}