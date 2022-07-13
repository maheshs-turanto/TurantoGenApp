using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling accounts.</summary>
[Authorize]
public partial class AccountController : IdentityBaseController
{

    /// <summary>Check before save.</summary>
    ///
    /// <param name="ApplicationUser">The ApplicationUser .</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public string CheckBeforeAction(ApplicationUser user, string command = "")
    {
        var AlertMsg = "";
        //put your custom validation here
        return AlertMsg;
    }
    /// <summary>After save.</summary>
    ///
    /// <param name="ApplicationUser">The ApplicationUser .</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public void AfterUserCreate(ApplicationUser user, string command = "")
    {
    }
    /// <summary>CustomValidateBeforeLogin.</summary>
    ///
    /// <param name="ApplicationUser">The ApplicationUser .</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public string CustomValidateBeforeLogin(ApplicationUser user, string command = "")
    {
        var AlertMsg = "";
        return AlertMsg;
    }
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="entitydatasource">The entitydatasource.</param>
    /// <param name="db">              The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(ApplicationUser applicationuser, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        // Write your logic here
    }
}
}