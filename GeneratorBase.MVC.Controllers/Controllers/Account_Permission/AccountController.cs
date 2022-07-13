using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Web.Security;
using System.Configuration;
using Microsoft.Web.WebPages.OAuth;
using System.Web.Routing;
using System.Web.WebPages;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity.Owin;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling user login, logout and userrole related actions.</summary>
[Authorize]
[NoCache]
public partial class AccountController : IdentityBaseController
{
    /// <summary>captchastring propert rendom ganrated.</summary>
    ///
    /// <value>The captchastring.</value>
    public static string captchastring
    {
        get;
        set;
    }
    /// <summary>Default constructor.</summary>
    public AccountController()
    {
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="userManager">Manager for user.</param>
    public AccountController(UserManager<ApplicationUser> userManager)
    {
    }
    /// <summary>Gets or sets the urlfor forgotpassword.</summary>
    ///
    /// <value>The url for forgotpassword (one time use and with expiration).</value>
    public static string UrlforForgotpassword
    {
        get;
        private set;
    }
    
    public ActionResult ExportConfiguration()
    {
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        List<ApplicationRole> list = Identitydb.Roles.ToList();
        List<Permission> permissions = new PermissionContext().Permissions.ToList();
        List<AppRoles> roles = new List<AppRoles>();
        AppConfiguration configuration = new AppConfiguration();
        foreach(var item in list)
        {
            AppRoles role = new AppRoles();
            role.role = item;
            role.permissions = permissions.Where(p => p.RoleName == item.Name).ToList();
            roles.Add(role);
        }
        configuration.rolespermission = roles;
        //configuration.appSettings = new ApplicationContext(new SystemUser()).AppSettings.ToList();
        //configuration.companyProfiles = new ApplicationContext(new SystemUser()).CompanyInformations.ToList();
        //configuration.userBasedSecurities = new UserBasedSecurityContext().UserBasedSecurities.ToList();
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        serSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        var data = Newtonsoft.Json.JsonConvert.SerializeObject(configuration, serSettings);
        Response.Clear();
        Response.ClearHeaders();
        Response.AppendHeader("Content-Length", data.Length.ToString());
        Response.ContentType = "application/json; charset=utf-8";
        Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + commonObj.AppName() + "_Configuration.json\"");
        Response.Write(data);
        Response.End();
        //return File(filedata, "application/force-download", Path.GetFileName(FileName));
        return Json("", JsonRequestBehavior.AllowGet);
    }
    [HttpGet]
    public ActionResult ImportConfiguration()
    {
        if(!((CustomPrincipal)User).IsAdmin)
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.Title = "Import Configuration";
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <param name="FileUpload">       The file upload.</param>
    /// <param name="collection">       The collection.</param>
    /// <param name="CleanOldBR">       The clean old line break.</param>
    /// <param name="ImportOnlyEnabled">The import only enabled.</param>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    public ActionResult ImportConfiguration([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection, bool? CleanOldBR, bool? ImportOnlyEnabled)
    {
        if(FileUpload != null)
        {
            string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();
            if(fileExtension.ToUpper() == ".JSON")
            {
                string rename = string.Empty;
                rename = System.IO.Path.GetFileName(FileUpload.FileName.ToLower().Replace(fileExtension, ".json"));
                fileExtension = ".json";
                string fileLocation = string.Format("{0}\\{1}", Server.MapPath("~/ExcelFiles"), rename);
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                FileUpload.SaveAs(fileLocation);
                string content = System.IO.File.ReadAllText(fileLocation);
                Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
                serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                serSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfiguration>(content, serSettings);
                TempData["original"] = Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfiguration>(content, serSettings); ;
                List<ApplicationRole> list = Identitydb.Roles.ToList();
                foreach(var role in data.rolespermission)
                {
                    if(list.Any(p => p.Name == role.role.Name))
                        role.type = "Update";
                    else
                        role.type = "New";
                }
                return View("ConfirmImportConfiguration", data);
            }
        }
        return RedirectToAction("Index");
    }
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    public ActionResult ConfirmImportConfiguration(AppConfiguration model, AppConfiguration original)
    {
        if(!((CustomPrincipal)User).IsAdmin)
        {
            return RedirectToAction("Index", "Error");
        }
        if(TempData.ContainsKey("original"))
            original = (AppConfiguration)TempData["original"];
        List<ApplicationRole> list = Identitydb.Roles.ToList();
        using(var permissioncontext = new PermissionContext())
            foreach(var item in model.rolespermission)
            {
                if(item.isselected)
                {
                    if(list.Select(p => p.Name).Contains(item.role.Name))
                    {
                        var existingpermission = permissioncontext.Permissions.Where(p => p.RoleName == item.role.Name);
                        foreach(var extpermission in existingpermission)
                            permissioncontext.Permissions.Remove(extpermission);
                        permissioncontext.SaveChanges();
                        var permissions = original.rolespermission.FirstOrDefault(p => p.role.Name == item.role.Name);
                        foreach(var perm in permissions.permissions)
                        {
                            perm.Id = 0;
                            permissioncontext.Permissions.Add(perm);
                        }
                        permissioncontext.SaveChanges();
                    }
                    else
                    {
                        var obj = new ApplicationRole();
                        obj.Name = item.role.Name;
                        obj.RoleType = item.role.RoleType;
                        obj.TenantId = item.role.TenantId;
                        Identitydb.Roles.Add(obj);
                        Identitydb.SaveChanges();
                        var permissions = original.rolespermission.FirstOrDefault(p => p.role.Name == item.role.Name);
                        foreach(var perm in permissions.permissions)
                        {
                            perm.Id = 0;
                            permissioncontext.Permissions.Add(perm);
                        }
                        permissioncontext.SaveChanges();
                    }
                }
            }
        TempData["original"] = null;
        // return RedirectToAction("Index");
        ViewBag.ImportError = "success";
        return View("ConfirmImportConfiguration",original);
    }
    
    /// <summary>Creates a result that redirects to the given URL.</summary>
    ///
    /// <param name="mobile">   True to mobile redirect to mobile view.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>A RedirectResult.</returns>
    [Audit(0)]
    public RedirectResult SwitchView(bool mobile, string returnUrl)
    {
        if(Request.Browser.IsMobileDevice == mobile)
        {
            HttpContext.ClearOverriddenBrowser();
        }
        else
        {
            HttpContext.SetOverriddenBrowser(mobile ? BrowserOverride.Mobile : BrowserOverride.Desktop);
        }
        return Redirect(returnUrl);
    }
    /// <summary>(An Action that handles HTTP GET requests) user login.</summary>
    ///
    /// <param name="returnUrl">           URL of the return.</param>
    /// <param name="ThirdPartyLoginError">The third party login error.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult (Login View).</returns>
    [AllowAnonymous]
    [Audit(0)]
    public ActionResult Login(string returnUrl,string ThirdPartyLoginError)
    {
        if(!Request.IsAjaxRequest())
            UrlforForgotpassword = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
        if(returnUrl != null && returnUrl.Contains("LogOff"))
            returnUrl = Url.Action("LogOff", "Account",null, protocol: Request.Url.Scheme, defaultPort: true);
        ViewBag.ReturnUrl = returnUrl;
        if(User != null && User.Identity is System.Security.Principal.WindowsIdentity)
            return RedirectToAction("Index", "Home");
        ViewData["ThirdPartyLoginError"] = ThirdPartyLoginError;
        var theme = System.Configuration.ConfigurationManager.AppSettings["AppTheme"];
        if(theme.ToLower() == "defaultcompress")
            return View("~/Views/Account/Login.cshtml");
        else
            return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) user login.</summary>
    ///
    /// <param name="model">    The user model.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult (redirects to returnUrl).</returns>
    [HttpPost]
    [AllowAnonymous]
    [Audit(0)]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
        var theme = System.Configuration.ConfigurationManager.AppSettings["AppTheme"];
        string viewName = "~/Views/Account/Login.cshtml";
        if(GeneratorBase.MVC.Models.CommonFunction.Instance.NeedSharedUserSystem() == "yes")
        {
            if(ModelState.IsValid)
            {
                //var Db = new AuthenticationDbContext();
                var ssoUsers = (new AuthenticationDbContext()).Users.Where(p => p.UserName == model.UserName).ToList();
                if(ssoUsers != null && ssoUsers.Count > 0)
                {
                    var user = ssoUsers[0];
                    PasswordHasher ph = new PasswordHasher();
                    var result = ph.VerifyHashedPassword(user.PasswordHash, model.Password);
                    if(result.ToString() == "Success")
                    {
                        // ApplicationDbContext localAppDB = new ApplicationDbContext();
                        var localAppUser = Identitydb.Users.Where(p => p.UserName == model.UserName).ToList();
                        if(localAppUser != null && localAppUser.Count > 0)
                        {
                            await SignInAsync(localAppUser[0], isPersistent: false);
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            var newUserInLocalDB = new ApplicationUser()
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email
                            };
                            var result1 = await UserManager.CreateAsync(newUserInLocalDB);
                            if(result1.Succeeded)
                            {
                                AfterUserCreate(newUserInLocalDB, "SharedUserSystem");
                                await SignInAsync(newUserInLocalDB, isPersistent: false);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                else
                {
                    var localDBUser = await UserManager.FindAsync(model.UserName, model.Password);
                    if(localDBUser != null)
                    {
                        await SignInAsync(localDBUser, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                        ModelState.AddModelError("", "Invalid username or password.");
                }
            }
        }
        else
        {
            if(ModelState.IsValid)
            {
                using(ApplicationContext db = new ApplicationContext(new SystemUser()))
                {
                    string IsVerifyUserEmail = ConfigurationManager.AppSettings["VerifyUserEmail"];
                    string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if(user != null)
                    {
                        if((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                        {
                            if(await UserManager.IsLockedOutAsync(user.Id))
                            {
                                if(user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc.Value.Date == DateTime.MaxValue.Date)
                                    ModelState.AddModelError("", string.Format("Your account has been locked."));
                                else
                                    ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                if(theme.ToLower() == "defaultcompress")
                                    return View(viewName, model);
                                else
                                    return View(model);
                            }
                            else
                            {
                                //Two factor change
                                string twofactorauthenticationenable = GeneratorBase.MVC.Models.CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                                if(twofactorauthenticationenable.ToLower() == "true" && user.TwoFactorEnabled)
                                {
                                    if(!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                                    {
                                        if(!string.IsNullOrEmpty(IsVerifyUserEmail))
                                            return RedirectToAction("EmailCodeNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                        else return RedirectToAction("EmailNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                    }
                                    var sendcode = new SendCodeViewModel { UserId = user.Id, Providers = null, ReturnUrl = returnUrl };
                                    return RedirectToAction("SendCode", "Account", sendcode);
                                }
                                else
                                {
                                    if(!(await UserManager.IsEmailConfirmedAsync(user.Id)) && !string.IsNullOrEmpty(IsVerifyUserEmail))
                                    {
                                        return RedirectToAction("EmailCodeNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                    }
                                    DoAuditEntry.AddJournalEntry("Account", model.UserName, "Login", "Successful", Request.UserHostAddress);
                                    await SignInAsync(user, model.RememberMe);
                                    // When token is verified correctly, clear the access failed count used for lockout
                                    await UserManager.ResetAccessFailedCountAsync(user.Id);
                                }
                            }
                        }
                        else if(await UserManager.IsLockedOutAsync(user.Id))
                        {
                            DoAuditEntry.AddJournalEntry("Account", model.UserName, "Login", "Unsuccessful", Request.UserHostAddress);
                            ModelState.AddModelError("", string.Format("Your account has been locked.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                            if(theme.ToLower() == "defaultcompress")
                                return View(viewName, model);
                            else
                                return View(model);
                        }
                        else
                        {
                            //Two factor change
                            string twofactorauthenticationenable = GeneratorBase.MVC.Models.CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                            if(twofactorauthenticationenable.ToLower() == "true" && user.TwoFactorEnabled)
                            {
                                if(!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                                {
                                    if(!string.IsNullOrEmpty(IsVerifyUserEmail))
                                        return RedirectToAction("EmailCodeNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                    else return RedirectToAction("EmailNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                }
                                var sendcode = new SendCodeViewModel { UserId = user.Id, Providers = null, ReturnUrl = returnUrl };
                                return RedirectToAction("SendCode", "Account", sendcode);
                            }
                            else
                            {
                                if(!(await UserManager.IsEmailConfirmedAsync(user.Id)) && !string.IsNullOrEmpty(IsVerifyUserEmail))
                                {
                                    return RedirectToAction("EmailCodeNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
                                }
                                DoAuditEntry.AddJournalEntry("Account", model.UserName, "Login", "Successful", Request.UserHostAddress);
                                await SignInAsync(user, model.RememberMe);
                            }
                        }
                        using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
                        {
                            var userid = user.Id;
                            LoginAttempts history = new LoginAttempts();
                            history.UserId = userid;
                            history.Date = DateTime.UtcNow;
                            history.IsSuccessfull = true;
                            history.IPAddress = Request.UserHostAddress;
                            usercontext.LoginAttempts.Add(history);
                            usercontext.SaveChanges();
                        }
                        if(ModelState.IsValid)
                            return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        DoAuditEntry.AddJournalEntry("Account", model.UserName, "Login", "Unsuccessful", Request.UserHostAddress);
                        var user1 = UserManager.FindByName(model.UserName);
                        if(user1 != null)
                        {
                            if((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                            {
                                if(await UserManager.IsLockedOutAsync(user1.Id))
                                {
                                    if(user1.LockoutEndDateUtc.HasValue && user1.LockoutEndDateUtc.Value.Date == DateTime.MaxValue.Date)
                                        ModelState.AddModelError("", string.Format("Your account has been locked."));
                                    else
                                        ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                }
                                // if user is subject to lockouts and the credentials are invalid  // record the failure and check if user is lockedout and display message, otherwise,
                                // display the number of attempts remaining before lockout
                                else if(await UserManager.GetLockoutEnabledAsync(user1.Id))
                                {
                                    // Record the failure which also may cause the user to be locked out
                                    await UserManager.AccessFailedAsync(user1.Id);
                                    string message;
                                    if(await UserManager.IsLockedOutAsync(user1.Id))
                                    {
                                        if(user1.LockoutEndDateUtc.HasValue && user1.LockoutEndDateUtc.Value.Date == DateTime.MaxValue.Date)
                                            message = string.Format("Your account has been locked.");
                                        else
                                            message = string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value);
                                    }
                                    else
                                    {
                                        //int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user1.Id);
                                        //int attemptsLeft = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value) - accessFailedCount;
                                        message = "Invalid credentials.";
                                    }
                                    ModelState.AddModelError("", message);
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid username or password.");
                                }
                            }
                            else
                            {
                                if(await UserManager.IsLockedOutAsync(user1.Id))
                                    ModelState.AddModelError("", string.Format("Your account has been locked out. Please contact your administrator.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                else
                                    ModelState.AddModelError("", "Invalid username or password.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password.");
                        }
                    }
                }
            }
        }
        if(Request.AcceptTypes.Contains("text/html"))
        {
            if(theme.ToLower() == "defaultcompress")
                return View(viewName);
            else
                return View(model);
        }
        else if(Request.AcceptTypes.Contains("application/json"))
        {
            return RedirectToLocal(returnUrl);
        }
        // If we got this far, something failed, redisplay form
        if(theme.ToLower() == "defaultcompress")
            return View(viewName);
        else
            return View(model);
    }
    /// <summary>Creates a user.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Id for the tenant.</param>
    ///
    /// <returns>A response stream to send to the CreateUser View.</returns>
    [AllowAnonymous]
    public ActionResult CreateUser(string UrlReferrer, long? TenantId)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(((CustomPrincipal)User).CanAddAdminFeature("User"))
        {
            var role = Identitydb.Roles;
            if(!((GeneratorBase.MVC.Models.CustomPrincipal)User).IsAdmin)
            {
                var adminRoles = (new GeneratorBase.MVC.Models.CustomPrincipal(User)).GetAdminRoles().Split(",".ToCharArray());
                ViewBag.RoleList = role.Where(p => !adminRoles.Contains(p.Name)).ToList().OrderBy(p => p.Name);
            }
            else
                ViewBag.RoleList = role.ToList().OrderBy(p => p.Name);
            ViewData["UrlReferrer"] = UrlReferrer;
            ViewData["TenantId"] = TenantId;
            return View();
        }
        else
            return RedirectToAction("Index", "Home");
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [Audit(0)]
    public FileResult DisplayLoginImage(long id)
    {
        using(var context = new ApplicationContext(new SystemUser()))
        {
            var doc = context.Documents.Find(id);
            if(doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            //string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
    }
    [AllowAnonymous]
    [Audit(0)]
    public JsonResult CheckPasswordLength(string Password)
    {
        var appSettings = new ApplicationContext(new SystemUser()).AppSettings;
        //var result = await UserManager.PasswordValidator.ValidateAsync(Password);
        var min = 6;
        long max = -1;
        var pwdmax = appSettings.Where(p => p.Key == "PasswordMaximumLength").FirstOrDefault();
        var pwdmin = appSettings.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault();
        if(pwdmin != null)
            Int32.TryParse(pwdmin.Value, out min);
        if(pwdmax != null)
            Int64.TryParse(pwdmax.Value, out max);
        //if (!result.Succeeded)
        //    return Json(string.Join(",", result.Errors), JsonRequestBehavior.AllowGet);
        //return Json(true, JsonRequestBehavior.AllowGet);
        if(max < 0)
            max = Int64.MaxValue;
        if(Password.Length < min)
            return Json("Passwords must be at least "+min+" characters.", JsonRequestBehavior.AllowGet);
        if(Password.Length > max)
            return Json("Passwords must not be more than "+max+" characters.", JsonRequestBehavior.AllowGet);
        return Json(Password.Length >=min && Password.Length<=max,JsonRequestBehavior.AllowGet);
    }
    [AllowAnonymous]
    [Audit(0)]
    public JsonResult CheckPasswordLengthOldPassword(string OldPassword)
    {
        return CheckPasswordLength(OldPassword);
    }
    [AllowAnonymous]
    [Audit(0)]
    public JsonResult CheckPasswordLengthNewPassword(string NewPassword)
    {
        return CheckPasswordLength(NewPassword);
    }
    /// <summary>(An Action that handles HTTP GET requests) New user registration.</summary>
    ///
    /// <returns>An asynchronous result that yields an ActionResult (Register View).</returns>
    [AllowAnonymous]
    [Audit(0)]
    public ActionResult Register()
    {
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) New user registration with email notification.</summary>
    ///
    /// <param name="model">      The user model.</param>
    /// <param name="CaptchaText">The captcha text.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model, string CaptchaText)
    {
        if(ModelState.IsValid)
        {
            var User = await UserManager.FindByEmailAsync(model.Email);
            if(User != null)
            {
                return Json(new {result="emailExist"}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            string IsVerifyUserEmail = ConfigurationManager.AppSettings["VerifyUserEmail"];
            var user = model.GetUser();
            var userExist = await UserManager.FindByNameAsync(user.UserName);
            if(userExist == null)
            {
                if(CaptchaText == captchastring)
                {
                    string validationresult = CheckBeforeAction(user, "Register");
                    if(!string.IsNullOrEmpty(validationresult))
                        return Json(new {result=validationresult}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if(result.Succeeded)
                    {
                        AfterUserCreate(user, "Register");
                        //Two factor change
                        string twofactorauthenticationenable = CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                        if(twofactorauthenticationenable.ToLower() == "true")
                        {
                            await UserManager.SetTwoFactorEnabledAsync(user.Id, true);
                        }
                        AssignDefaultRoleToNewUser(user.Id);
                        SendEmail sendEmail = new SendEmail();
                        if(twofactorauthenticationenable.ToLower() == "true" && string.IsNullOrEmpty(IsVerifyUserEmail))
                        {
                            var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Verify Email");
                            if(EmailTemplate != null)
                            {
                                string mailbody = string.Empty;
                                string mailsubject = string.Empty;
                                if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                                {
                                    mailbody = EmailTemplate.EmailContent;
                                    try
                                    {
                                        var code = UserManager.GenerateShortEmailConfirmationToken(user.Id);
                                        var callbackUrl = Url.Action("EmailCodeNotVerified", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme, defaultPort: true);
                                        mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###CODE###", "" + code + "",StringComparison.OrdinalIgnoreCase).Replace("###URL###", "" + callbackUrl + "");
                                    }
                                    catch(Exception ex)
                                    {
                                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                    }
                                }
                                mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Account Verification Email " : EmailTemplate.EmailSubject; ;
                                var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                                if(emailstatus != "EmailSent")
                                    return Json(new {result=emailstatus}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Verify Email");
                            if(string.IsNullOrEmpty(IsVerifyUserEmail))
                            {
                                EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                            }
                            if(EmailTemplate != null)
                            {
                                string mailbody = string.Empty;
                                string mailsubject = string.Empty;
                                if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                                {
                                    mailbody = EmailTemplate.EmailContent;
                                    var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                                    if(!string.IsNullOrEmpty(IsVerifyUserEmail))
                                    {
                                        var code = UserManager.GenerateShortEmailConfirmationToken(user.Id);
                                        callbackUrl = Url.Action("EmailCodeNotVerified", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme, defaultPort: true);
                                        mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###CODE###", "" + code + "",StringComparison.OrdinalIgnoreCase).Replace("###URL###", "" + callbackUrl + "");
                                    }
                                    else
                                        mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###CODE###", "",StringComparison.OrdinalIgnoreCase);
                                }
                                mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Account Verification Email " : EmailTemplate.EmailSubject; ;
                                //sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                                var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                                if(emailstatus != "EmailSent")
                                    return Json(new {result=emailstatus}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new {result="Ok",userid = user.Id}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string strError = "";
                        var lstError = result.Errors;
                        foreach(var errormsg in lstError)
                        {
                            foreach(var message in errormsg.Split('.'))
                            {
                                string msg = message;
                                if(msg.Trim().ToLower() == "passwords must have at least one non letter or digit character")
                                    msg = "Passwords must have at least one special character";
                                strError += msg + ".\r\n";
                            }
                        }
                        strError = (strError.Length > 0) ? strError.Substring(0, strError.Length - 4) : strError;
                        return Json(new {result=strError}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new {result="Captchaverification"}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new {result="UserExist"}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    /// <summary>Assign default role to new user.</summary>
    ///
    /// <param name="userId">Id for the user.</param>
    private void AssignDefaultRoleToNewUser(string userId)
    {
        AdminSettingsRepository _adminSettings = new AdminSettingsRepository();
        IdentityManager idManager = new IdentityManager();
        if(!string.IsNullOrEmpty(_adminSettings.GetDefaultRoleForNewUser()) && _adminSettings.GetDefaultRoleForNewUser().ToUpper() != "NONE")
            idManager.AddUserToRole(LogggedInUser, userId, _adminSettings.GetDefaultRoleForNewUser());
        else
            idManager.AddUserToRole(LogggedInUser, userId, "ReadOnly");
    }
    /// <summary>User Profile.</summary>
    ///
    /// <returns>A response stream to send to the UserProfile View.</returns>
    public ActionResult UserProfile(string UrlReferrer, string userid)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(string.IsNullOrEmpty(userid))
            userid = User.Identity.GetUserId();
        var user = Identitydb.Users.Find(userid);
        ViewData["UserParentUrl"] = UrlReferrer;
        ViewData["Roles"] = string.Join(",", Identitydb.Roles.Where(p => p.Users.Any(q => q.UserId == user.Id)).Select(p => p.Name));
        return View(user);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies user profile.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="user">    The ApplicationUser object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UserProfile(ApplicationUser user, string UrlReferrer, bool RenderPartial = false)
    {
        string command = Request.Form["hdncommand"];
        var useroriginal = Identitydb.Users.Find(user.Id);
        useroriginal.FirstName = user.FirstName;
        useroriginal.LastName = user.LastName;
        Identitydb.Entry(useroriginal).State = System.Data.Entity.EntityState.Modified;
        Identitydb.SaveChanges();
        if(command != "Save")
        {
            return RedirectToAction("UserProfile", new { UrlReferrer = UrlReferrer });
        }
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Home","Index");
        ViewData["Roles"] =string.Join(",", Identitydb.Roles.Where(p => p.Users.Any(q => q.UserId == user.Id)).Select(p=>p.Name));
        ViewData["UserParentUrl"] = UrlReferrer;
        return View(useroriginal);
    }
    public void SaveUserExtraProperties(string id, RegisterViewModel model)
    {
    }
    /// <summary>(An Action that handles HTTP POST requests) registers the user.</summary>
    ///
    /// <param name="model">        The user model.</param>
    /// <param name="selectedRoles">The selected roles for user.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="TenantId">     Id for the tenant (to associate user with tenant).</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> RegisterUser(RegisterViewModel model, string selectedRoles, string UrlReferrer,string TenantId)
    {
        if(ModelState.IsValid)
        {
            var User = await UserManager.FindByEmailAsync(model.Email);
            string emailerror = "";
            if(User != null)
            {
                emailerror = "E-mail is already taken";
                var errors = new List<string>();
                errors.Add(emailerror);
                //return Json(errors);
                return Json(new { success = false, status = emailerror });
            }
            var user = model.GetUser();
            if(model.NotifyForEmail)
            {
                model.Password = CreateRandomPassword();
            }
            string validationresult = CheckBeforeAction(user, "RegisterUser");
            if(!string.IsNullOrEmpty(validationresult))
            {
                return Json(new { success = false, status = validationresult });
            }
            var result = await UserManager.CreateAsync(user, model.Password);
            if(result.Succeeded)
            {
                AfterUserCreate(user, "RegisterUser");
                var currentUser = (new ApplicationDbContext()).Users.First(u => u.UserName == model.UserName);
                //Two factor change
                string twofactorauthenticationenable = CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                if(twofactorauthenticationenable.ToLower() == "true")
                {
                    if(!string.IsNullOrEmpty(selectedRoles))
                    {
                        var appsettinglist = new ApplicationContext(new SystemUser()).AppSettings.ToList();
                        var allowedroles2fa = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "allowedroles2fa");
                        var allowedroles2falist = allowedroles2fa == null || allowedroles2fa.Value == "None" ? new List<string>() : allowedroles2fa.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> lstSelectedRoles = selectedRoles.Split(',').ToList();
                        lstSelectedRoles = lstSelectedRoles.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                        if(allowedroles2falist.Intersect(lstSelectedRoles).Count() > 0)
                            await UserManager.SetTwoFactorEnabledAsync(user.Id, false);
                        else
                            await UserManager.SetTwoFactorEnabledAsync(user.Id, true);
                    }
                    else
                    {
                        await UserManager.SetTwoFactorEnabledAsync(user.Id, true);
                    }
                }
                //SaveUserExtraProperties(currentUser.Id,model);
                if(!string.IsNullOrEmpty(selectedRoles))
                {
                    selectedRoles = selectedRoles.TrimEnd(',');
                    var lstRoles = selectedRoles.Split(',');
                    if(currentUser != null)
                    {
                        var idManager = new IdentityManager();
                        idManager.ClearUserRoles(LogggedInUser, currentUser.Id);
                        foreach(var role in lstRoles)
                        {
                            idManager.AddUserToRole(LogggedInUser, user.Id, role);
                        }
                    }
                }
                //SendEmailToUser(currentUser);
                //return Json(new { success = true });
                var emailstatus = SendEmailToUser(currentUser);
                if(emailstatus != "EmailSent")
                    return Json(new { success = false, status = emailstatus });
                else
                    return Json(new { success = true });
            }
            else
            {
                var errors = new List<string>();
                foreach(var error in result.Errors)
                {
                    errors.Add(error + "\r\n");
                }
                //return Json(errors);
                return Json(new { success = false, status = errors });
            }
        }
        else
        {
            var errors = new List<string>();
            foreach(var modelState in ViewData.ModelState.Values)
            {
                errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
            }
            //return Json(errors);
            return Json(new { success = false, status = errors });
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    
    /// <summary> Send Email to register user.</summary>
    ///
    /// <param name="user">The Application User.</param>
    /// <returns>string</returns>
    public string SendEmailToUser(ApplicationUser User)
    {
        //var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
        SendEmail sendEmail = new SendEmail();
        var SelectEmailTemplate = (User.NotifyForEmail.HasValue && User.NotifyForEmail.Value) ? "Notification to Create Password for New Account" : "User Registration";
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == SelectEmailTemplate);
        if(EmailTemplate != null)
        {
            string mailbody = string.Empty;
            string mailsubject = string.Empty;
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                if(User.NotifyForEmail.HasValue && User.NotifyForEmail.Value)
                {
                    mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Create Password for New Account" : EmailTemplate.EmailSubject;
                    var code = UserManager.GenerateShortEmailConfirmationToken(User.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = User.Id, code = System.Web.HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme, defaultPort: true);
                    mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###UserName###", User.UserName).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###AppName###", CommonFunction.Instance.AppName());
                }
                else
                {
                    //Two Factor Changes
                    string twofactorauthenticationenable = CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                    if(twofactorauthenticationenable.ToLower() == "true")
                    {
                        EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Verify Email");
                        if(EmailTemplate != null)
                        {
                            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                            {
                                mailbody = EmailTemplate.EmailContent;
                                try
                                {
                                    var code = UserManager.GenerateShortEmailConfirmationToken(User.Id);
                                    var callbackUrl = Url.Action("VerifyEmail", "Account", new { UserId = User.Id, code = code }, protocol: Request.Url.Scheme, defaultPort: true);
                                    mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###CODE###", code,StringComparison.OrdinalIgnoreCase).Replace("###AppName###", CommonFunction.Instance.AppName());
                                }
                                catch(Exception ex)
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                }
                            }
                            mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Account Verification Email " : EmailTemplate.EmailSubject; ;
                        }
                    }
                    else
                    {
                        mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Your registration is successful " : EmailTemplate.EmailSubject;
                        var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                        mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>");
                    }
                }
            }
            //sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
            var emailstatus = sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
            if(emailstatus != "EmailSent")
                return emailstatus;
            else
                return "EmailSent";
        }
        return "";
    }
    
    
    
    /// <summary>(An Action that handles HTTP POST requests) forgot password.</summary>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    [Audit(0)]
    public async Task<ActionResult> ForgotPassword()
    {
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) forgot password with checks and email notification.</summary>
    ///
    /// <param name="model">   The model.</param>
    /// <param name="IsAddPop">The is add pop.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            //var appURL = UrlforForgotpassword;
            SendEmail sendEmail = new SendEmail();
            //var Db = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            ApplicationUser User = new ApplicationUser();
            if(model.Username != null)
                User = await UserManager.FindByNameAsync(model.Username);
            else
                User = await UserManager.FindByEmailAsync(model.Email);
            if(User != null)
            {
                int accessFailedCount = User.AccessFailedCount;
                int attempts = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value);
                if(accessFailedCount >= attempts)
                    return Json(new { result = "UserLoginAttempt" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                if(User.LockoutEndDateUtc != null)
                    return Json(new { result = "UserIslocked" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Forgot Password");
                if(EmailTemplate != null)
                {
                    string mailbody = string.Empty;
                    string mailsubject = string.Empty;
                    if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                    {
                        mailbody = EmailTemplate.EmailContent;
                        try
                        {
                            var code =  UserManager.GenerateShortEmailConfirmationToken(User.Id);
                            var callbackUrl = Url.Action("ResetPassword", "Account",
                                                         new { UserId = User.Id, code = System.Web.HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme, defaultPort: true);
                            mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###Username###", User.UserName).Replace("###Password###", "").Replace("###CODE###", code,StringComparison.OrdinalIgnoreCase).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###AppName###", CommonFunction.Instance.AppName());
                        }
                        catch(Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        }
                    }
                    mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " :Reset your password " : EmailTemplate.EmailSubject; ;
                    //sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
                    var emailstatus = sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
                    if(emailstatus != "EmailSent")
                        return Json(new {result = emailstatus,userid=User.Id}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = "Ok", userid = User.Id }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { result = "UserNotExist" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json(new { result = "Failure"}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Resets the password.</summary>
    ///
    /// <param name="userId">Id for the user.</param>
    /// <param name="code">  The code.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPassword(ResendViewModel model, string EmailCode, string returnUrl)
    {
        if(model.UserId == null || EmailCode == null)
        {
            return View("Error");
        }
        bool result;
        ViewData["EmailCode"] = EmailCode;
        ViewBag.user = (new ApplicationDbContext(new SystemUser())).Users.Find(model.UserId);
        try
        {
            result = UserManager.ConfirmShortEmail(model.UserId, HttpUtility.UrlDecode(EmailCode));
        }
        catch(InvalidOperationException ioe)
        {
            // ConfirmEmailAsync throws when the userId is not found.
            ViewBag.errorMessage = ioe.Message;
            //return View("Error");
            return Json(new { result = "Failure", message=ioe.Message }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if(result)
        {
            var token = await UserManager.GeneratePasswordResetTokenAsync(model.UserId);
            // return RedirectToAction("Manage", new { token = token, provider = model.UserId });
            return Json(new { result = "ok", url = Url.Action("Manage", new { token = token, provider = model.UserId }) }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        // If we got this far, something failed.
        // AddErrors(result);
        ViewBag.errorMessage = "Invalid Code !";
        // return RedirectToAction("ActivationLink", "Error");
        //return View(model);
        return Json(new { result = "Failure", message = "Invalid Code !" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPassword(string UserId, string code, string returnUrl)
    {
        if(UserId == null)
        {
            return View("Error");
        }
        var user = await UserManager.FindByIdAsync(UserId);
        ViewBag.user = user;
        ViewData["EmailCode"] = code;
        if(!string.IsNullOrEmpty(code))
        {
            try
            {
                if(UserManager.ConfirmShortEmail(UserId, HttpUtility.UrlDecode(code)))
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(UserId);
                    return RedirectToAction("Manage", new { token = token, provider = UserId });
                }
            }
            catch
            {
            }
        }
        return View(new ResendViewModel()
        {
            UserId = user.Id,
            ReturnUrl = returnUrl
        });
    }
    [HttpGet]
    [AllowAnonymous]
    public ActionResult ResendPasswordResetCode(string userId)
    {
        var User = new ApplicationDbContext(new SystemUser()).Users.Find(userId);
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Forgot Password");
        if(EmailTemplate != null)
        {
            SendEmail sendEmail = new SendEmail();
            string mailbody = string.Empty;
            string mailsubject = string.Empty;
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                try
                {
                    var code = UserManager.GenerateShortEmailConfirmationToken(User.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account",
                                                 new { UserId = User.Id, code = System.Web.HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme, defaultPort: true);
                    mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###Username###", User.UserName).Replace("###Password###", "").Replace("###CODE###", "" + code + "",StringComparison.OrdinalIgnoreCase).Replace("###URL###", "" + callbackUrl + "").Replace("###AppName###", CommonFunction.Instance.AppName());
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " :Reset your password " : EmailTemplate.EmailSubject; ;
            //sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
            var emailstatus = sendEmail.Notify(User.Id, User.Email, mailbody, mailsubject);
            return Json(emailstatus, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Failed", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a role.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>An asynchronous result that yields the new role.</returns>
    [AllowAnonymous]
    public ActionResult CreateRole(string UrlReferrer, long? TenantId)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(((CustomPrincipal)User).CanAddAdminFeature("Role"))
        {
            ViewData["UrlReferrer"] = UrlReferrer;
            ViewData["TenantId"] = TenantId;
            var role = Identitydb.Roles;
            var adminRoles = (new GeneratorBase.MVC.Models.CustomPrincipal(User)).GetAdminRoles().Split(",".ToCharArray());
            ViewBag.RoleList = new SelectList(role.Where(p => !adminRoles.Contains(p.Name)).ToList().OrderBy(p => p.Name), "Name", "DisplayValue");
            return View();
        }
        else
            return RedirectToAction("Index", "Home");
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a new role.</summary>
    ///
    /// <param name="model">   The model.</param>
    /// <param name="RoleList">List of roles.</param>
    ///
    /// <returns>An asynchronous result that yields the new role.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateRole(CreateRoleViewModel model, string RoleList)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        model.Name = model.Name.Trim();
        if(((CustomPrincipal)User).CanAddAdminFeature("Role"))
            if(ModelState.IsValid)
            {
                //var role = model.GetRole();
                string roleName = model.Name;
                IdentityResult roleResult;
                roleResult = RoleManager.Create(new ApplicationRole(roleName, model.Description, model.RoleType, model.TenantId));
                if(roleResult.Succeeded)
                {
                    DoAuditEntry.AddJournalEntryCommon(LogggedInUser, Identitydb, roleName, "ApplicationRole");
                    if(!string.IsNullOrEmpty(RoleList))
                    {
                        PermissionContext permissiondb = new PermissionContext();
                        foreach(var ent in permissiondb.Permissions.Where(p => p.RoleName == RoleList))
                        {
                            Permission permission = new Permission();
                            permission.CanAdd = ent.CanAdd;
                            permission.CanDelete = ent.CanDelete;
                            permission.CanView = ent.CanView;
                            permission.CanEdit = ent.CanEdit;
                            permission.IsOwner = ent.IsOwner;
                            permission.SelfRegistration = ent.SelfRegistration;
                            permission.EntityName = ent.EntityName;
                            permission.RoleName = roleName;
                            permission.Verbs = ent.Verbs;
                            permission.NoEdit = ent.NoEdit;
                            permission.NoView = ent.NoView;
                            permission.UserAssociation = ent.UserAssociation;
                            permissiondb.Permissions.Add(permission);
                        }
                        foreach(var ent in permissiondb.AdminPrivileges.Where(p => p.RoleName == RoleList))
                        {
                            PermissionAdminPrivilege permission = new PermissionAdminPrivilege();
                            permission.AdminFeature = ent.AdminFeature;
                            permission.IsAdd = ent.IsAdd;
                            permission.IsAllow = ent.IsAllow;
                            permission.IsDelete = ent.IsDelete;
                            permission.IsEdit = ent.IsEdit;
                            permission.RoleName = roleName;
                            permissiondb.AdminPrivileges.Add(permission);
                        }
                        permissiondb.SaveChanges();
                    }
                    return Json(new { success = true });
                }
                else
                {
                    var errors = new List<string>();
                    foreach(var error in roleResult.Errors)
                    {
                        errors.Add(error);
                    }
                    return Json(errors);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach(var modelState in ViewData.ModelState.Values)
                {
                    errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
                }
                return Json(errors);
            }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a role.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>An asynchronous result that yields the new rule-based role.</returns>
    [AllowAnonymous]
    public ActionResult CreateRuleBasedRole(string UrlReferrer, long? TenantId)//mahesh
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(((CustomPrincipal)User).CanAddAdminFeature("Role"))
        {
            ViewData["UrlReferrer"] = UrlReferrer;
            ViewData["TenantId"] = TenantId;
            var role = Identitydb.Roles;
            var adminRoles = (new GeneratorBase.MVC.Models.CustomPrincipal(User)).GetAdminRoles().Split(",".ToCharArray());
            ViewBag.RoleList = new SelectList(role.Where(p => !adminRoles.Contains(p.Name)).ToList().OrderBy(p => p.Name), "Name", "DisplayValue");
            var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => p.Name != "ApiToken" && p.Name != "CustomReports" && !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).ToList();
            ViewBag.EntityName = new SelectList(EntList, "Name", "DisplayName");
            Dictionary<string, string> list = new Dictionary<string, string>();
            ViewBag.UserRelation = new SelectList(list, "key", "value");
            ViewBag.Condition = new SelectList(list, "key", "value");
            Dictionary<string, string> PropertyList = new Dictionary<string, string>();
            PropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = ViewBag.GroupList = new SelectList(PropertyList, "key", "value");
            ViewBag.AssociationList17 = new SelectList(PropertyList, "key", "value");
            ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                              = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                      = ViewBag.SuggestedDynamicValueInCondition7 = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(new Dictionary<string, string>(), "key", "value");
            return View();
        }
        else
            return RedirectToAction("Index", "Home");
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a new role.</summary>
    ///
    /// <param name="model">   The model.</param>
    /// <param name="RoleList">List of roles.</param>
    ///
    /// <returns>An asynchronous result that yields the new rule based role.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateRuleBasedRole(CreateRoleViewModel model, string RoleList, string EntityName, string UserRelation, string lblrulecondition)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        model.Name = model.Name.Trim();
        if(((CustomPrincipal)User).CanAddAdminFeature("Role"))
            if(ModelState.IsValid)
            {
                //var role = model.GetRole();
                string roleName = model.Name;
                IdentityResult roleResult;
                roleResult = RoleManager.Create(new ApplicationRole(roleName, model.Description, model.RoleType, model.TenantId));
                if(roleResult.Succeeded)
                {
                    using(var appdb = new ApplicationContext(new SystemUser()))
                    {
                        DynamicRoleMapping dynamicrole = new DynamicRoleMapping();
                        dynamicrole.EntityName = EntityName;
                        dynamicrole.UserRelation = UserRelation;
                        dynamicrole.RoleId = roleName;
                        dynamicrole.Value = "NA";
                        dynamicrole.Condition = "NA";
                        dynamicrole.Other = "Rulebased";
                        appdb.DynamicRoleMappings.Add(dynamicrole);
                        appdb.SaveChanges();
                        using(var conditiondb = new ConditionContext())
                        {
                            if(!string.IsNullOrEmpty(lblrulecondition))
                            {
                                var conditions = lblrulecondition.Split("?".ToCharArray());
                                foreach(var cond in conditions)
                                {
                                    if(string.IsNullOrEmpty(cond)) continue;
                                    var param = cond.Split(",".ToCharArray());
                                    Condition newcondition = new Condition();
                                    newcondition.PropertyName = param[0];
                                    newcondition.Operator = param[1];
                                    if(param[1] == "Match")
                                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                                    else
                                        newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                                    newcondition.LogicalConnector = param[3];
                                    newcondition.DynamicRuleConditionsID = dynamicrole.Id;
                                    newcondition.RuleConditionsID = null;
                                    conditiondb.Conditions.Add(newcondition);
                                    conditiondb.SaveChanges();
                                }
                            }
                        }
                    }
                    DoAuditEntry.AddJournalEntryCommon(LogggedInUser, Identitydb, roleName, "ApplicationRole");
                    if(!string.IsNullOrEmpty(RoleList))
                    {
                        PermissionContext permissiondb = new PermissionContext();
                        foreach(var ent in permissiondb.Permissions.Where(p => p.RoleName == RoleList))
                        {
                            Permission permission = new Permission();
                            permission.CanAdd = ent.CanAdd;
                            permission.CanDelete = ent.CanDelete;
                            permission.CanView = ent.CanView;
                            permission.CanEdit = ent.CanEdit;
                            permission.IsOwner = ent.IsOwner;
                            permission.SelfRegistration = ent.SelfRegistration;
                            permission.EntityName = ent.EntityName;
                            permission.RoleName = roleName;
                            permission.Verbs = ent.Verbs;
                            permission.NoEdit = ent.NoEdit;
                            permission.NoView = ent.NoView;
                            permission.UserAssociation = ent.UserAssociation;
                            permissiondb.Permissions.Add(permission);
                        }
                        foreach(var ent in permissiondb.AdminPrivileges.Where(p => p.RoleName == RoleList))
                        {
                            PermissionAdminPrivilege permission = new PermissionAdminPrivilege();
                            permission.AdminFeature = ent.AdminFeature;
                            permission.IsAdd = ent.IsAdd;
                            permission.IsAllow = ent.IsAllow;
                            permission.IsDelete = ent.IsDelete;
                            permission.IsEdit = ent.IsEdit;
                            permission.RoleName = roleName;
                            permissiondb.AdminPrivileges.Add(permission);
                        }
                        permissiondb.SaveChanges();
                    }
                    return Json(new { success = true });
                }
                else
                {
                    var errors = new List<string>();
                    foreach(var error in roleResult.Errors)
                    {
                        errors.Add(error);
                    }
                    return Json(errors);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach(var modelState in ViewData.ModelState.Values)
                {
                    errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
                }
                return Json(errors);
            }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    [AllowAnonymous]
    public ActionResult EditRuleBasedRole(string id, string UrlReferrer, long? TenantId)//mahesh
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        var roles = Identitydb.Roles.First(u => u.Name == id);
        var model = new EditRoleViewModel(roles);
        DynamicRoleMapping dynamicRoleDetails = new DynamicRoleMapping();
        using(var appdb = new ApplicationContext(new SystemUser()))
        {
            dynamicRoleDetails = appdb.DynamicRoleMappings.FirstOrDefault(p => p.RoleId == id);
            if(dynamicRoleDetails != null)
            {
                ViewBag.DynamicRoleId = dynamicRoleDetails.Id;
            }
            else
            {
                //error
            }
        }
        //
        ViewData["UrlReferrer"] = UrlReferrer;
        ViewData["TenantId"] = TenantId;
        var role = Identitydb.Roles;
        var adminRoles = (new GeneratorBase.MVC.Models.CustomPrincipal(User)).GetAdminRoles().Split(",".ToCharArray());
        ViewBag.RoleList = new SelectList(role.Where(p => !adminRoles.Contains(p.Name)).ToList().OrderBy(p => p.Name), "Name", "DisplayValue");
        var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => p.Name != "ApiToken" && p.Name != "CustomReports" && !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).ToList();
        ViewBag.EntityName = new SelectList(EntList, "Name", "DisplayName", dynamicRoleDetails.EntityName);
        Dictionary<string, string> list = new Dictionary<string, string>();
        ViewBag.UserRelation = new SelectList(list, "key", "value");
        ViewBag.SelectedUserReleated = dynamicRoleDetails.UserRelation;
        ViewBag.Condition = new SelectList(list, "key", "value");
        Dictionary<string, string> PropertyList = new Dictionary<string, string>();
        PropertyList.Add("0", "--Select Property--");
        ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
        ViewBag.PropertyList7 = ViewBag.PropertyList1 = ViewBag.GroupList = new SelectList(PropertyList, "key", "value");
        ViewBag.AssociationList17 = new SelectList(PropertyList, "key", "value");
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                          = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                  = ViewBag.SuggestedDynamicValueInCondition7 = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(new Dictionary<string, string>(), "key", "value");
        return View(model);
        //
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditRuleBasedRole(EditRoleViewModel model, string RoleList, string EntityName, string UserRelation, string lblrulecondition)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        model.Name = model.Name.Trim();
        if(ModelState.IsValid)
        {
            if(Identitydb.Roles.Where(u => u.Name == model.Name && u.Name != model.OriginalName).Count() <= 0)
            {
                var roles = Identitydb.Roles.First(u => u.Name == model.OriginalName);
                var roleName = roles.Name = model.Name;
                roles.Description = model.Description;
                Identitydb.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                await Identitydb.SaveChangesAsync();
                PermissionContext db = new PermissionContext();
                List<Permission> lstprm = db.Permissions.Where(q => q.RoleName == model.OriginalName).ToList();
                lstprm.ForEach(p => p.RoleName = model.Name);
                db.SaveChanges();
                UserDefinePagesRoleContext dbUserPages = new UserDefinePagesRoleContext();
                List<UserDefinePagesRole> lstUserPagesprm = dbUserPages.UserDefinePagesRoles.Where(q => q.RoleName == model.OriginalName).ToList();
                lstUserPagesprm.ForEach(p => p.RoleName = model.Name);
                dbUserPages.SaveChanges();
                AdminSettingsRepository _adminSettingsRepository = new AdminSettingsRepository();
                if(_adminSettingsRepository.GetDefaultRoleForNewUser() == model.OriginalName)
                {
                    AdminSettings adminSettings = new AdminSettings();
                    adminSettings.DefaultRoleForNewUser = model.Name;
                    _adminSettingsRepository.EditAdminSettings(adminSettings);
                }
                using(var appdb = new ApplicationContext(new SystemUser()))
                {
                    DynamicRoleMapping dynamicrole = new DynamicRoleMapping();
                    dynamicrole = appdb.DynamicRoleMappings.FirstOrDefault(p => p.RoleId == model.OriginalName);
                    dynamicrole.EntityName = EntityName;
                    dynamicrole.UserRelation = UserRelation;
                    dynamicrole.RoleId = roleName;
                    dynamicrole.Value = "NA";
                    dynamicrole.Condition = "NA";
                    dynamicrole.Other = "Rulebased";
                    appdb.SaveChanges();
                    using(var conditiondb = new ConditionContext())
                    {
                        if(!string.IsNullOrEmpty(lblrulecondition))
                        {
                            var conditions = lblrulecondition.Split("?".ToCharArray());
                            foreach(var cond in conditions)
                            {
                                if(string.IsNullOrEmpty(cond)) continue;
                                var param = cond.Split(",".ToCharArray());
                                Condition newcondition = new Condition();
                                newcondition.PropertyName = param[0];
                                newcondition.Operator = param[1];
                                if(param[1] == "Match")
                                    newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                                else
                                    newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                                newcondition.LogicalConnector = param[3];
                                newcondition.DynamicRuleConditionsID = dynamicrole.Id;
                                newcondition.RuleConditionsID = null;
                                conditiondb.Conditions.Add(newcondition);
                                conditiondb.SaveChanges();
                            }
                        }
                    }
                }
                //DoAuditEntry.AddJournalEntryCommon(LogggedInUser, Identitydb, roleName, "ApplicationRole");
                return Json(new { success = true });
                // return RedirectToAction("RoleList");
            }
            else
            {
                ModelState.AddModelError("", model.Name + " is already exist.");
                return View(model);
            }
        }
        else
        {
            var errors = new List<string>();
            foreach(var modelState in ViewData.ModelState.Values)
            {
                errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
            }
            return Json(errors);
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) manages user actions for different conditions.</summary>
    ///
    /// <param name="message"> The message.</param>
    /// <param name="token">   The token.</param>
    /// <param name="provider">The provider.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    public ActionResult Manage(ManageMessageId? message, string token, string provider)
    {
        ViewBag.StatusMessage =
            message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            : message == ManageMessageId.Error ? "An error has occurred."
            : "";
        ViewBag.HasLocalPassword = HasPassword();
        ViewBag.ReturnUrl =  Url.Action("Manage","Account",null,null, defaultPort: true);
        ViewBag.token = token;
        ViewBag.provider = provider;
        ////Added by  start
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var appSettings1 = db1.AppSettings;
        string applySecurityPolicy1 = appSettings1.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
        string RequiredLengthMessage = string.Empty;
        string RequireSpecialCharacterMessage = string.Empty;
        string RequireDigitMessage = string.Empty;
        string RequireLowercaseMessage = string.Empty;
        string RequireUppercaseMessage = string.Empty;
        if(applySecurityPolicy1.ToLower() == "yes")
        {
            bool RequireNonLetterOrDigit = appSettings1.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireDigit = appSettings1.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireLowercase = appSettings1.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireUppercase = appSettings1.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            int RequiredLength = Convert.ToInt32(appSettings1.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value);
            RequiredLengthMessage = appSettings1.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Description;
            if(!string.IsNullOrEmpty(RequiredLengthMessage))
                RequiredLengthMessage = RequiredLengthMessage.Replace("(value)", "(" + RequiredLength + ")");
            if(RequireNonLetterOrDigit == true)
                RequireSpecialCharacterMessage = appSettings1.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Description;
            if(RequireDigit == true)
                RequireDigitMessage = appSettings1.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Description;
            if(RequireLowercase == true)
                RequireLowercaseMessage = appSettings1.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Description;
            if(RequireUppercase == true)
                RequireUppercaseMessage = appSettings1.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Description;
        }
        ViewBag.RequiredLengthMessage = RequiredLengthMessage;
        ViewBag.RequireSpecialCharacterMessage = RequireSpecialCharacterMessage;
        ViewBag.RequireDigitMessage = RequireDigitMessage;
        ViewBag.RequireLowercaseMessage = RequireLowercaseMessage;
        ViewBag.RequireUppercaseMessage = RequireUppercaseMessage;
        ////Added by end
        if(!string.IsNullOrEmpty(token))
            ViewBag.HasLocalPassword = true;
        if(ViewBag.HasLocalPassword == false) //nrew line
            return RedirectToAction("Login", "Account");
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) manages the given ManageUserViewModel model.</summary>
    ///
    /// <param name="model">The ManageUserViewModel model.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<ActionResult> Manage(ManageUserViewModel model)
    {
        bool hasPassword = HasPassword();
        ViewBag.HasLocalPassword = hasPassword;
        ViewBag.ReturnUrl = Url.Action("Manage","Account",null,null, defaultPort: true);
        if(hasPassword || !string.IsNullOrEmpty(model.token))
        {
            if(ModelState.IsValid)
            {
                ApplicationContext db = new ApplicationContext(new SystemUser());
                var appSettings = db.AppSettings;
                string applySecurityPolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                if(applySecurityPolicy.ToLower() == "yes")
                {
                    var pwdcount = Convert.ToInt32(appSettings.Where(p => p.Key == "OldPasswordGenerationCount").FirstOrDefault().Value);
                    if(string.IsNullOrEmpty(model.token))
                    {
                        if(isPreviousUsedPassword(model, pwdcount, User.Identity.GetUserId()))
                        {
                            DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ChangePassword", "Unsuccessful", Request.UserHostAddress);
                            return View(model);
                        }
                    }
                    else
                    {
                        if(isPreviousUsedPassword(model, pwdcount, model.provider))
                        {
                            DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ResetPassword", "Unsuccessful", Request.UserHostAddress);
                            ViewBag.HasLocalPassword = true;
                            ViewBag.token = model.token;
                            ViewBag.provider = model.provider;
                            return View(model);
                        }
                    }
                }
                //IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if(string.IsNullOrEmpty(model.token))
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        SavePasswordHistory(User.Identity.GetUserId());
                        LockUnlockUserChangePassword(User.Identity.GetUserId(), "false");
                        DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ChangePassword", "Successful", Request.UserHostAddress);
                        LogOff();
                        ViewBag.ChangeMessage = "OK";
                        //return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        return View(model);
                    }
                    else
                    {
                        DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ChangePassword", "Unsuccessful", Request.UserHostAddress);
                        var error = AddErrorsForPassword(result);
                        ViewBag.ChangeMessage = error;
                        AddErrors(result);
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.HasLocalPassword = true;
                    ViewBag.token = model.token;
                    ViewBag.provider = model.provider;
                    IdentityResult result = await UserManager.ResetPasswordAsync(model.provider, model.token, model.NewPassword);
                    if(result.Succeeded)
                    {
                        DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ResetPassword", "Successful", Request.UserHostAddress);
                        SavePasswordHistory(model.provider);
                        LogOff();
                        ViewBag.ChangeMessage = "OK1";
                        return View(model);
                        //return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess, token = model.token, provider = model.provider });
                    }
                    else
                    {
                        DoAuditEntry.AddJournalEntry("Account", User.Identity.Name, "ResetPassword", "Unsuccessful", Request.UserHostAddress);
                        AddErrors(result);
                    }
                }
            }
        }
        else
        {
            // User does not have a password so remove any validation errors caused by a missing OldPassword field
            ModelState state = ModelState["OldPassword"];
            if(state != null)
            {
                state.Errors.Clear();
            }
            if(ModelState.IsValid)
            {
                IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if(result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="strPassword"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> CheckPasswordStrength(string strPassword)
    {
        bool hasPassword = HasPassword();
        ViewBag.HasLocalPassword = hasPassword;
        //ViewBag.ReturnUrl = Url.Action("Manage", "Account", null, null, defaultPort: true);
        ///Added by Rachana start
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var appSettings1 = db1.AppSettings;
        string applySecurityPolicy1 = appSettings1.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
        string RequiredLengthMessage = string.Empty;
        string RequireSpecialCharacterMessage = string.Empty;
        string RequireDigitMessage = string.Empty;
        string RequireLowercaseMessage = string.Empty;
        string RequireUppercaseMessage = string.Empty;
        List<KeyValuePair<string, string>> validationDictionary = new List<KeyValuePair<string, string>>();
        if(applySecurityPolicy1.ToLower() == "yes")
        {
            int RequiredLength = Convert.ToInt32(appSettings1.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value);
            RequiredLengthMessage = appSettings1.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Description;
            bool RequireNonLetterOrDigit = appSettings1.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireDigit = appSettings1.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireLowercase = appSettings1.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool RequireUppercase = appSettings1.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            if(!string.IsNullOrEmpty(strPassword) && strPassword.Length < RequiredLength)
            {
                if(!string.IsNullOrEmpty(RequiredLengthMessage))
                    RequiredLengthMessage = RequiredLengthMessage.Replace("(value)", "(" + RequiredLength + ")");
                validationDictionary.Add(new KeyValuePair<string, string>("LengthValidation", RequiredLengthMessage));
            }
            if(RequireDigit == true)
            {
                if(!string.IsNullOrEmpty(strPassword) && strPassword.Any(char.IsDigit) == false)
                {
                    RequireDigitMessage = appSettings1.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Description;
                    validationDictionary.Add(new KeyValuePair<string, string>("PasswordRequireDigit", RequireDigitMessage));
                }
            }
            ViewBag.ChangeMessage = "";
            var validationResults = await UserManager.PasswordValidator.ValidateAsync(strPassword);
            if(validationResults.Succeeded)
            {
                ViewBag.ChangeMessage = "OK";
                validationDictionary.Add(new KeyValuePair<string, string>("OK", ViewBag.ChangeMessage));
                // return View(model);
                return Json(new { Data = validationDictionary }, JsonRequestBehavior.AllowGet);
            }
            if(!validationResults.Succeeded)
            {
                //foreach (var item in validationResults.Errors)
                //{
                //    ModelState.AddModelError("PasswordStrength", item);
                //    ViewBag.ChangeMessage = item + Environment.NewLine;
                //}
                ViewBag.ChangeMessage = validationResults.Errors;
                foreach(var error in validationResults.Errors)
                {
                    var strError = "";
                    foreach(string message in error.Split('.'))
                    {
                        string msg = message;
                        if(msg.Trim().ToLower() == "incorrect password")
                            msg = "Current Password is incorrect, please try again";
                        else if(msg.Trim().ToLower() == "passwords must have at least one non letter or digit character")
                        {
                            if(RequireNonLetterOrDigit == true)
                                RequireSpecialCharacterMessage = appSettings1.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Description;
                            msg = RequireSpecialCharacterMessage;
                            // validationDictionary.Add(new CustomKeyValuePairWrapper("SpecialCharacterValidation", new KeyValuePair<string, bool>(RequireSpecialCharacterMessage, false)));
                            validationDictionary.Add(new KeyValuePair<string, string>("SpecialCharacterValidation", RequireSpecialCharacterMessage));
                        }
                        else if(msg.Trim().ToLower().Contains("passwords must have at least one lowercase"))
                        {
                            if(RequireLowercase == true)
                                RequireLowercaseMessage = appSettings1.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Description;
                            msg = RequireLowercaseMessage;
                            // validationDictionary.Add(new CustomKeyValuePairWrapper("PasswordRequireLowerCase", new KeyValuePair<string, bool>(RequireLowercaseMessage, false)));
                            validationDictionary.Add(new KeyValuePair<string, string>("PasswordRequireLowerCase", RequireLowercaseMessage));
                        }
                        else if(msg.Trim().ToLower().Contains("passwords must have at least one uppercase"))
                        {
                            if(RequireUppercase == true)
                                RequireUppercaseMessage = appSettings1.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Description;
                            msg = RequireUppercaseMessage;
                            // validationDictionary.Add(new CustomKeyValuePairWrapper("PasswordRequireUpperCase", new KeyValuePair<string, bool>(RequireUppercaseMessage, false)));
                            validationDictionary.Add(new KeyValuePair<string, string>("PasswordRequireUpperCase", RequireUppercaseMessage));
                        }
                        strError += msg + "." + Environment.NewLine;
                    }
                    strError = (strError.Length > 0) ? strError.Substring(0, strError.Length - 4) : strError;
                    ViewBag.ChangeMessage = strError;
                    ModelState.AddModelError("", strError);
                }
                ViewBag.validationDictionary = validationDictionary;
                ViewBag.RequiredLengthMessage = RequiredLengthMessage;
                ViewBag.RequireSpecialCharacterMessage = RequireSpecialCharacterMessage;
                ViewBag.RequireDigitMessage = RequireDigitMessage;
                ViewBag.RequireLowercaseMessage = RequireLowercaseMessage;
                ViewBag.RequireUppercaseMessage = RequireUppercaseMessage;
                // ViewBag.ChangeMessage = "OK";
                // return View(model);
                return Json(new { Data = validationDictionary }, JsonRequestBehavior.AllowGet);
            }
        }
        return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Change password history.</summary>
    ///
    /// <param name="userID">Identifier for the user.</param>
    public void SavePasswordHistory(string userID)
    {
        PasswordHistory history = new PasswordHistory();
        var user = UserManager.FindById(userID);
        history.Date = DateTime.UtcNow;
        history.UserId = user.Id;
        history.HashedPassword = user.PasswordHash;
        Identitydb.PasswordHistorys.Add(history);
        Identitydb.SaveChanges();
    }
    /// <summary>Query if 'model' has previously used password.</summary>
    ///
    /// <param name="model">   The ManageUserViewModel.</param>
    /// <param name="pwdcount">The password count.</param>
    /// <param name="userid">  The userid.</param>
    ///
    /// <returns>True if previous used password, false if not.</returns>
    public bool isPreviousUsedPassword(ManageUserViewModel model, int pwdcount, string userid)
    {
        if(string.IsNullOrEmpty(userid)) return false;
        using(ApplicationDbContext appDb = new ApplicationDbContext(true))
        {
            var user = appDb.Users.Find(userid); //UserManager.FindById(userid);
            if(user == null) return false;
            var lstpwdhistory = appDb.PasswordHistorys.Where(p => p.UserId == user.Id).OrderByDescending(p => p.Date).Take(pwdcount).ToList();
            foreach(var pwd in lstpwdhistory)
            {
                var result = UserManager.PasswordHasher.VerifyHashedPassword(pwd.HashedPassword, model.NewPassword);
                if(result == PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "You can't use password from last " + pwdcount + " passwords.");
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>User log out.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the LogOff View.</returns>
    [Audit("LogOff")]
    public ActionResult LogOff(string UrlReferrer)
    {
        return RedirectToAction("Index", "Home");
    }
    /// <summary>(An Action that handles HTTP POST requests) user log off (remove cookies and clear session).</summary>
    ///
    /// <returns>A response stream to send to the LogOff View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit("LogOff")]
    [AllowAnonymous]
    public ActionResult LogOff()
    {
        AuthenticationManager.SignOut();
        Session.Abandon();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        RemoveCookie(AppUrl + "CurrentRole");
        RemoveCookie("PageId");
        Response.Cookies["fltCookie"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies["fltCookieFltTabId"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies.Clear();
        Session.Clear();
        Response.Cookies[AppUrl + "CurrentRole"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies["PageId"].Expires = DateTime.UtcNow.AddDays(-1);
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
        bool preventmultiplelogin = false;
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        preventmultiplelogin =!string.IsNullOrEmpty(commonObj.PreventMultipleLogin()) && commonObj.PreventMultipleLogin().ToLower() == "yes" ? true : false;
        if(preventmultiplelogin)
        {
            var localAppUser = Identitydb.Users.Where(p => p.UserName == ((CustomPrincipal)User).Name).ToList();
            if(preventmultiplelogin && localAppUser != null && localAppUser.Count > 0)
            {
                UserManager.UpdateSecurityStamp(localAppUser[0].Id);
            }
        }
        return RedirectToAction("Index", "Home");
    }
    /// <summary>(An Action that handles HTTP GET requests) browser close.</summary>
    [HttpGet]
    [Audit("LogOff")]
    public void BrowserClose()
    {
        AuthenticationManager.SignOut();
        Session.Abandon();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        RemoveCookie(AppUrl + "CurrentRole");
        RemoveCookie("PageId");
        Response.Cookies.Clear();
        Session.Clear();
        Response.Cookies[AppUrl + "CurrentRole"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies["PageId"].Expires = DateTime.UtcNow.AddDays(-1);
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
        bool preventmultiplelogin = false;
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        preventmultiplelogin =!string.IsNullOrEmpty(commonObj.PreventMultipleLogin()) && commonObj.PreventMultipleLogin().ToLower() == "yes" ? true : false;
        if(preventmultiplelogin)
        {
            var localAppUser = Identitydb.Users.Where(p => p.UserName == ((CustomPrincipal)User).Name).ToList();
            if(preventmultiplelogin && localAppUser != null && localAppUser.Count > 0)
            {
                UserManager.UpdateSecurityStamp(localAppUser[0].Id);
            }
        }
        //return null;
    }
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(UserManager != null) UserManager.Dispose();
            if(Identitydb != null) Identitydb.Dispose();
            if(RoleManager != null) RoleManager.Dispose();
        }
        base.Dispose(disposing);
    }
    /// <summary>Indexes.</summary>
    ///
    /// <param name="currentFilter">Is current filter on.</param>
    /// <param name="searchString"> The search string.</param>
    /// <param name="sortBy">       Describes which column sorts list.</param>
    /// <param name="isAsc">        The is ascending.</param>
    /// <param name="page">         The page.</param>
    /// <param name="itemsPerPage"> The items per page.</param>
    /// <param name="IsExport">     Is export.</param>
    /// <param name="RenderPartial">Is render partial.</param>
    ///
    /// <returns>A response stream to send to the Index View (User List).</returns>
    [AllowAnonymous]
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? IsExport, bool? RenderPartial)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanViewAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewData["userprofile"] = true;
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        List<ApplicationUser> users = null;
        users = Identitydb.Users.ToList();
        var model = new List<EditUserViewModel>();
        foreach(var user in users)
        {
            var u = new EditUserViewModel(user);
            model.Add(u);
        }
        var _model = from s in model
                     select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            _model = searchRecords(_model.AsQueryable(), searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            _model = sortRecords(_model.AsQueryable(), sortBy, isAsc);
        }
        else _model = _model.OrderByDescending(c => c.Id);
        //
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value);
            ViewBag.Pages = pageNumber;
        }
        ViewBag.PageSize = pageSize;
        if(pageNumber > 1)
        {
            var totalListCount = _model.Count();
            int quotient = totalListCount / pageSize;
            var remainder = totalListCount % pageSize;
            var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
            if(pageNumber > maxpagenumber)
                pageNumber = 1;
        }
        //
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_model.ToPagedList(pageNumber, pageSize));
        else
            return PartialView("IndexPartial", _model.ToPagedList(pageNumber, pageSize));
    }
    /// <summary>Searches in the list.</summary>
    ///
    /// <param name="users">       The users.</param>
    /// <param name="searchString">The search string.</param>
    ///
    /// <returns>IQueryable<EditUserViewModel>.</returns>
    private IQueryable<EditUserViewModel> searchRecords(IQueryable<EditUserViewModel> users, string searchString)
    {
        searchString = searchString.Trim();
        users = users.Where(s => (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Email) && s.Email.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FirstName) && s.FirstName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LastName) && s.LastName.ToUpper().Contains(searchString)));
        return users;
    }
    /// <summary>Sort records.</summary>
    ///
    /// <param name="users"> The users.</param>
    /// <param name="sortBy">Describes who sort this object.</param>
    /// <param name="isAsc"> The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<EditUserViewModel> sortRecords(IQueryable<EditUserViewModel> users, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        ParameterExpression paramExpression = Expression.Parameter(typeof(EditUserViewModel), "ApplicationUser");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<EditUserViewModel>)users.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { users.ElementType, lambda.Body.Type },
                       users.Expression,
                       lambda));
    }
    /// <summary>Role list.</summary>
    ///
    /// <returns>A response stream to send to the RoleList View.</returns>
    [AllowAnonymous]
    public ActionResult RoleList()
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanViewAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        // var Db = new ApplicationDbContext();
        var roles = Identitydb.Roles.OrderBy(p=>p.Name);
        var model = new List<EditRoleViewModel>();
        foreach(var role in roles)
        {
            var u = new EditRoleViewModel(role);
            model.Add(u);
        }
        AdminSettingsRepository _adminSettings = new AdminSettingsRepository();
        ViewBag.DefaultRoleForNewUser = _adminSettings.GetDefaultRoleForNewUser();
        return View(model);
    }
    /// <summary>(An Action that handles HTTP GET requests) edits the given user.</summary>
    ///
    /// <param name="id">     The identifier.</param>
    /// <param name="Message">(Optional) The message.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    public ActionResult Edit(string id, ManageMessageId? Message = null)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        var user = (new ApplicationDbContext()).Users.First(u => u.Id == id);
        var model = new EditUserViewModel(user);
        ViewBag.MessageId = Message;
        return View(model);
    }
    /// <summary>(An Action that handles HTTP POST requests) edits the user.</summary>
    ///
    /// <param name="model">The user model.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(EditUserViewModel model)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            //  var Db = new ApplicationDbContext();
            var userInfo = Identitydb.Users;
            //var username = user.First(p => p.UserName == model.UserName);
            var userEmail = userInfo.FirstOrDefault(p => p.Email == model.Email);
            if(userEmail != null)
            {
                var user = userInfo.First(p => p.UserName == model.UserName);
                if(user.FirstName != model.FirstName || user.LastName != model.LastName  || user.PhoneNumber != model.PhoneNumber)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    await Identitydb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else if(user.Email == userEmail.Email)
                {
                    return RedirectToAction("Index");
                }
                //if (user.Email == model.Email)
                //    return RedirectToAction("Index");
                ViewBag.DuplicacyMessage = "E-Mail already exist. Please try another one.";
                return View(model);
            }
            else
            {
                var user = userInfo.First(p => p.UserName == model.UserName);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Identitydb.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    /// <summary>Lock or Unlock user.</summary>
    ///
    /// <param name="id">      The identifier.</param>
    /// <param name="lockuser">The lockuser true or false.</param>
    ///
    /// <returns>A response stream to send to the LockUnlockUser View.</returns>
    [AllowAnonymous]
    public ActionResult LockUnlockUser(string id, string lockuser)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            //var Db = new ApplicationDbContext();
            var user = Identitydb.Users.First(u => u.Id == id);
            if(Convert.ToBoolean(lockuser))
            {
                //user.LockoutEnabled = true;
                user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(100);
            }
            else
            {
                //user.LockoutEnabled = false;
                user.LockoutEndDateUtc = null;
            }
            Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
            Identitydb.SaveChanges();
            return RedirectToAction("Index");
        }
        // If we got this far, something failed, redisplay form
        return RedirectToAction("Index");
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="lockuser"></param>
    [AllowAnonymous]
    public void LockUnlockUserChangePassword(string id, string lockuser)
    {
        //var user = Identitydb.Users.First(u => u.Id == id);
        //if (user.LockoutEndDateUtc != null)
        //{
        //user.LockoutEndDateUtc = null;
        //}
        //Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
        //Identitydb.SaveChanges();
        using(ApplicationContext dblockunlock= new ApplicationContext(new SystemUser()))
        {
            var user = dblockunlock.T_Users.FirstOrDefault(u => u.Id == id);
            if(user != null && user.LockoutEndDateUtc != null)
            {
                user.LockoutEndDateUtc = null;
            }
            dblockunlock.Entry(user).State = System.Data.Entity.EntityState.Modified;
            dblockunlock.SaveChanges();
        };
        // If we got this far, something failed, redisplay form
    }
    
    /// <summary>ConfirmEmail user.</summary>
    ///
    /// <param name="id">      The identifier.</param>
    /// <param name="lockuser">The lockuser true or false.</param>
    ///
    /// <returns>A response stream to send to the ConfirmEmail View.</returns>
    [AllowAnonymous]
    public ActionResult ConfirmEmail(string id, bool confirm)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanEditAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            //var Db = new ApplicationDbContext();
            var user = Identitydb.Users.First(u => u.Id == id);
            user.EmailConfirmed = confirm;
            Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
            Identitydb.SaveChanges();
            if(confirm)
            {
                SendEmail sendEmail = new SendEmail();
                var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                if(EmailTemplate != null)
                {
                    string mailbody = string.Empty;
                    string mailsubject = string.Empty;
                    if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                    {
                        mailbody = EmailTemplate.EmailContent;
                        var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                        mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>");
                    }
                    mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Your registration is successful " : EmailTemplate.EmailSubject; ;
                    var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                }
            }
            return RedirectToAction("Index");
        }
        // If we got this far, something failed, redisplay form
        return RedirectToAction("Index");
    }
    /// <summary>(An Action that handles HTTP GET requests) edit role.</summary>
    ///
    /// <param name="id">     The identifier.</param>
    /// <param name="Message">(Optional) The message.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    public ActionResult EditRole(string id, ManageMessageId? Message = null)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        var roles = Identitydb.Roles.First(u => u.Id == id);
        if(!((CustomPrincipal)User).IsAdmin && roles.RoleType == "Global")
        {
            return RedirectToAction("Index", "Home");
        }
        var model = new EditRoleViewModel(roles);
        ViewBag.MessageId = Message;
        return View(model);
    }
    /// <summary>(An Action that handles HTTP POST requests) edit role.</summary>
    ///
    /// <param name="model">      The model.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditRole(EditRoleViewModel model,string UrlReferrer)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        model.Name = model.Name.Trim();
        model.OriginalName = model.OriginalName.Trim();
        if(!((CustomPrincipal)User).CanEditAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            if(Identitydb.Roles.Where(u => u.Name == model.Name && u.Name != model.OriginalName).Count() <= 0)
            {
                var roles = Identitydb.Roles.First(u => u.Name == model.OriginalName);
                roles.Name = model.Name;
                roles.Description = model.Description;
                Identitydb.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                await Identitydb.SaveChangesAsync();
                PermissionContext db = new PermissionContext();
                List<Permission> lstprm = db.Permissions.Where(q => q.RoleName == model.OriginalName).ToList();
                lstprm.ForEach(p => p.RoleName = model.Name);
                db.SaveChanges();
                UserDefinePagesRoleContext dbUserPages = new UserDefinePagesRoleContext();
                List<UserDefinePagesRole> lstUserPagesprm = dbUserPages.UserDefinePagesRoles.Where(q => q.RoleName == model.OriginalName).ToList();
                lstUserPagesprm.ForEach(p => p.RoleName = model.Name);
                dbUserPages.SaveChanges();
                AdminSettingsRepository _adminSettingsRepository = new AdminSettingsRepository();
                if(_adminSettingsRepository.GetDefaultRoleForNewUser() == model.OriginalName)
                {
                    AdminSettings adminSettings = new AdminSettings();
                    adminSettings.DefaultRoleForNewUser = model.Name;
                    _adminSettingsRepository.EditAdminSettings(adminSettings);
                }
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                return RedirectToAction("RoleList");
            }
            else
                ModelState.AddModelError("", model.Name + " is already exist.");
        }
        return View(model);
    }
    /// <summary>Deletes the user for given ID.</summary>
    ///
    /// <param name="id">(Optional) The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    [AllowAnonymous]
    public ActionResult Delete(string id = null)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        var user = Identitydb.Users.First(u => u.Id == id);
        var model = new EditUserViewModel(user);
        if(user == null)
        {
            return HttpNotFound();
        }
        return View(model);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the user after confirmation.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public ActionResult DeleteConfirmed(string id)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("User"))
            return RedirectToAction("Index", "Home");
        var user = Identitydb.Users.First(u => u.Id == id);
        Identitydb.Users.Remove(user);
        Identitydb.SaveChanges();
        return RedirectToAction("Index");
    }
    /// <summary>Deletes the role described by ID.</summary>
    ///
    /// <param name="id">(Optional) The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteRole View.</returns>
    [AllowAnonymous]
    public ActionResult DeleteRole(string id = null)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        // var Db = new ApplicationDbContext();
        var roles = Identitydb.Roles.First(u => u.Id == id);
        if(!((CustomPrincipal)User).IsAdmin && roles.RoleType == "Global")
        {
            return RedirectToAction("Index", "Home");
        }
        var model = new EditRoleViewModel(roles);
        if(roles == null)
        {
            return HttpNotFound();
        }
        return View(model);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the DeleteRole Action) deletes the role confirmed described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteRoleConfirmed View.</returns>
    [HttpPost, ActionName("DeleteRole")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public ActionResult DeleteRoleConfirmed(string id)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("Role"))
            return RedirectToAction("Index", "Home");
        var roles = Identitydb.Roles.First(u => u.Id == id);
        if(roles == null)
        {
            return HttpNotFound();
        }
        Identitydb.Roles.Remove(roles);
        Identitydb.SaveChanges();
        PermissionContext db = new PermissionContext();
        List<Permission> lstprm = db.Permissions.Where(q => q.RoleName == roles.Name).ToList();
        db.Permissions.RemoveRange(lstprm);
        db.SaveChanges();
        UserDefinePagesRoleContext dbUserPages = new UserDefinePagesRoleContext();
        List<UserDefinePagesRole> lstUserPagesprm = dbUserPages.UserDefinePagesRoles.Where(q => q.RoleName == id).ToList();
        dbUserPages.UserDefinePagesRoles.RemoveRange(lstUserPagesprm);
        dbUserPages.SaveChanges();
        AdminSettingsRepository _adminSettingsRepository = new AdminSettingsRepository();
        if(_adminSettingsRepository.GetDefaultRoleForNewUser() == roles.Name)
        {
            AdminSettings adminSettings = new AdminSettings();
            adminSettings.DefaultRoleForNewUser = "None";
            _adminSettingsRepository.EditAdminSettings(adminSettings);
        }
        using(var dbdynamicrole = new ApplicationContext(new SystemUser()))
        {
            foreach(var dync in dbdynamicrole.DynamicRoleMappings.Where(p => p.RoleId == roles.Name))
            {
                dbdynamicrole.Entry<DynamicRoleMapping>(dync).State = System.Data.Entity.EntityState.Deleted;
            }
            dbdynamicrole.SaveChanges();
        }
        return RedirectToAction("Rolelist");
    }
    /// <summary>(An Action that handles HTTP GET requests) user roles.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the UserRoles View.</returns>
    [AllowAnonymous]
    public ActionResult UserRoles(string id)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
            return RedirectToAction("Index", "Home");
        var user = Identitydb.Users.First(u => u.Id == id);
        var model = new SelectUserRolesViewModel(user,LogggedInUser);
        var adminRoles = (new GeneratorBase.MVC.Models.CustomPrincipal(User)).GetAdminRoles().Split(",".ToCharArray());
        if(!((GeneratorBase.MVC.Models.CustomPrincipal)User).IsAdmin)
            model.Roles.RemoveAll(p =>adminRoles.Contains(p.RoleName));
        return View(model);
    }
    /// <summary>(An Action that handles HTTP POST requests) user roles.</summary>
    ///
    /// <param name="model">      The model.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the UserRoles View.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult UserRoles(SelectUserRolesViewModel model, string UrlReferrer)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            var idManager = new IdentityManager();
            var user = Identitydb.Users.First(u => u.UserName == model.UserName);
            var Db = new GeneratorBase.MVC.Models.ApplicationDbContext(true);
            var userRoles = user.Roles.Select(p => p.RoleId).ToList();
            var modelRoles = model.Roles.Select(p => p.RoleName).ToList();
            var notToDeleteRoles = Db.Roles.Where(p => userRoles.Contains(p.Id) && !modelRoles.Contains(p.Name)).Select(p => p.Id).ToList();
            idManager.ClearUserRoles(LogggedInUser, user.Id, notToDeleteRoles);
            foreach(var role in model.Roles)
            {
                if(role.Selected)
                {
                    idManager.AddUserToRole(LogggedInUser, user.Id, role.RoleName);
                }
            }
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            return RedirectToAction("index");
        }
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) users in role.</summary>
    ///
    /// <param name="id">           The identifier.</param>
    /// <param name="currentFilter">A filter specifying the current.</param>
    /// <param name="searchString"> The search string.</param>
    /// <param name="sortBy">       Describes column to sort list.</param>
    /// <param name="isAsc">        The is ascending.</param>
    /// <param name="page">         The page.</param>
    /// <param name="itemsPerPage"> The items per page.</param>
    /// <param name="IsExport">     The is export.</param>
    /// <param name="RenderPartial">The render partial.</param>
    ///
    /// <returns>A response stream to send to the UsersInRole View.</returns>
    [AllowAnonymous]
    public ActionResult UsersInRole(string id, string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? IsExport, bool? RenderPartial)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanViewAdminFeature("AssignUserRole"))
            return RedirectToAction("Index", "Home");
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        // var Db = new ApplicationDbContext();
        var role = Identitydb.Roles.First(u => u.Id == id);
        if(!((CustomPrincipal)User).IsAdmin)
        {
            if(role.RoleType == "Global")
                return RedirectToAction("Index", "Home");
        }
        var model = new SelectUsersInRoleViewModel(LogggedInUser,role);
        ViewBag.RolesName = model.RoleName;
        ViewBag.Count = model.UserCount;
        var model1 = from s in model.Users.ToList()
                     select s;
        if(!((GeneratorBase.MVC.Models.CustomPrincipal)LogggedInUser).IsAdmin)
            model1 = model1.Where(p=>!((GeneratorBase.MVC.Models.CustomPrincipal)LogggedInUser).IsAdminUser(p.UserName) && ((GeneratorBase.MVC.Models.CustomPrincipal)LogggedInUser).Name != p.UserName);
        //model1 = model1.OrderBy(c => c.UserName);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"] != null)
        {
            pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"] != null)
        {
            pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        if(!String.IsNullOrEmpty(searchString))
        {
            model1 = searchRecordsRoles(model1.AsQueryable(), searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            model1 = sortRecordsRoles(model1.AsQueryable(), sortBy, isAsc);
        }
        if(pageNumber > 1)
        {
            var totalListCount = model1.Count();
            int quotient = totalListCount / pageSize;
            var remainder = totalListCount % pageSize;
            var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
            if(pageNumber > maxpagenumber)
            {
                pageNumber = 1;
            }
        }
        ViewBag.PageSize = pageSize;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(model1.ToPagedList(pageNumber, pageSize));
        else
            return PartialView("UsersInRolePartial", model1.ToPagedList(pageNumber, pageSize));
        // return View(model);
    }
    /// <summary>Searches for the user.</summary>
    ///
    /// <param name="users">       The users.</param>
    /// <param name="searchString">The search string.</param>
    ///
    /// <returns>The found records roles.</returns>
    private IQueryable<SelectUserEditorViewModel> searchRecordsRoles(IQueryable<SelectUserEditorViewModel> users, string searchString)
    {
        searchString = searchString.Trim();
        users = users.Where(s => (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
        return users;
    }
    /// <summary>Sorts roles.</summary>
    ///
    /// <param name="users"> The users.</param>
    /// <param name="sortBy">Describes who sort this object.</param>
    /// <param name="isAsc"> The is ascending.</param>
    ///
    /// <returns>The sorted records roles.</returns>
    private IQueryable<SelectUserEditorViewModel> sortRecordsRoles(IQueryable<SelectUserEditorViewModel> users, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        ParameterExpression paramExpression = Expression.Parameter(typeof(SelectUserEditorViewModel), "ApplicationUser");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<SelectUserEditorViewModel>)users.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { users.ElementType, lambda.Body.Type },
                       users.Expression,
                       lambda));
    }
    /// <summary>(An Action that handles HTTP POST requests) users in role.</summary>
    ///
    /// <param name="model">      The model.</param>
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the UsersInRole View.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult UsersInRole(List<SelectUserEditorViewModel> model, string id, string UrlReferrer)
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if(!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            var idManager = new IdentityManager();
            foreach(var user in model)
            {
                var userId = Identitydb.Users.First(p => p.UserName == user.UserName).Id;
                if(user.Selected)
                    idManager.AddUserToRole(LogggedInUser, userId, id);
                else if(user.UserName != "Admin")
                    idManager.ClearUsersFromRole(LogggedInUser, userId, id);
            }
            if(!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            return RedirectToAction("RoleList", "Account");
        }
        return View();
    }
    /// <summary>Returns to users in role.</summary>
    ///
    /// <param name="urlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the ReturnToUsersInRole View.</returns>
    
    public ActionResult ReturnToUsersInRole(string urlReferrer)
    {
        if(!string.IsNullOrEmpty(urlReferrer))
            return Redirect(urlReferrer);
        return RedirectToAction("RoleList", "Account");
    }
    /// <summary>Switch role of logged-in user.</summary>
    [HttpGet]
    [Audit("SwitchRole")]
    public ActionResult SwitchRole()
    {
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        RemoveCookie(AppUrl + "CurrentRole");
        using(ApplicationDbContext ac = new ApplicationDbContext(true))
        {
            var oldAccess = ac.LoginSelectedRoles.FirstOrDefault(p => p.User == ((CustomPrincipal)User).Name);
            if(oldAccess != null)
            {
                ac.LoginSelectedRoles.Remove(oldAccess);
                ac.SaveChanges();
            }
        }
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
        return RedirectToAction("Index", "Home");
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetPropertyValueByEntityId(string Id, string PropName)
    {
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            var obj1 = context.T_Users.Find(Id);
            if(obj1 == null)
                return Json("0", JsonRequestBehavior.AllowGet);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            if(Property != null)
            {
                object PropValue = Property.GetValue(obj1, null);
                PropValue = PropValue == null ? 0 : PropValue;
                return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
            }
            else return Json("", JsonRequestBehavior.AllowGet);
        }
    }
    
    /// <summary>Associate user with tenant.</summary>
    ///
    /// <param name="TenantId">Identifier for the tenant.</param>
    /// <param name="userId">  Identifier for the user.</param>
    private void AssociateWithTenant(long? TenantId, string userId)
    {
    }
    
    
    #region Helpers
/// <summary>Gets the manager for authentication.</summary>
    ///
    /// <value>The authentication manager.</value>
    private IAuthenticationManager AuthenticationManager
    {
        get
        {
            return HttpContext.GetOwinContext().Authentication;
        }
    }
    /// <summary>Sign in asynchronous.</summary>
    ///
    /// <param name="user">        The user.</param>
    /// <param name="isPersistent">True if is persistent, false if not.</param>
    ///
    /// <returns>An asynchronous result.</returns>
    private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
        ClearCookies();
        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        var result = CustomValidateBeforeLogin(user);
        if(string.IsNullOrEmpty(result))
        {
            await UserManager.UpdateSecurityStampAsync(user.Id);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim("SecurityStamp", user.SecurityStamp));
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                IsPersistent = isPersistent
            }, identity);
        }
        else ModelState.AddModelError("", result);
    }
    /// <summary>Adds the errors.</summary>
    ///
    /// <param name="result">The model with error.</param>
    private void AddErrors(IdentityResult result)
    {
        foreach(var error in result.Errors)
        {
            var strError = "";
            foreach(var message in error.Split('.'))
            {
                string msg = message;
                if(msg.Trim().ToLower() == "incorrect password")
                    msg = "Current Password is incorrect, please try again";
                else if(msg.Trim().ToLower() == "passwords must have at least one non letter or digit character")
                    msg = "Passwords must have at least one special character";
                strError += msg + ".\r\n";
            }
            strError = (strError.Length > 0) ? strError.Substring(0, strError.Length - 4) : strError;
            ModelState.AddModelError("", strError);
        }
    }
    /// <summary>Adds the errors for password.</summary>
    ///
    /// <param name="result">The result.</param>
    ///
    /// <returns>A string.</returns>
    private string AddErrorsForPassword(IdentityResult result)
    {
        var strError = "";
        foreach(var error in result.Errors)
        {
            foreach(var msg in error.Split('.'))
            {
                strError += msg + ".\r\n";
            }
        }
        return strError;
    }
    /// <summary>Query if this user object has password.</summary>
    ///
    /// <returns>True if password, false if not.</returns>
    private bool HasPassword()
    {
        var user = UserManager.FindById(User.Identity.GetUserId());
        if(user != null)
        {
            return user.PasswordHash != null;
        }
        return false;
    }
    public enum ManageMessageId
    {
        /// <summary>An enum constant representing the change password success option.</summary>
        ChangePasswordSuccess,
        /// <summary>An enum constant representing the set password success option.</summary>
        SetPasswordSuccess,
        /// <summary>An enum constant representing the remove login success option.</summary>
        RemoveLoginSuccess,
        /// <summary>An enum constant representing the other error option.</summary>
        Error
    }
    /// <summary>Redirect to local.</summary>
    ///
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>A response stream to send to the RedirectToLocal View.</returns>
    private ActionResult RedirectToLocal(string returnUrl)
    {
        string IsAdmin = "false";
        if(((CustomPrincipal)User).IsAdmin)
            IsAdmin = "true";
        else
            IsAdmin = "false";
        if(Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    #endregion
    #region Helpers External Login ex google,yahoo etc
    /// <summary>POST: /Account/Disassociate Used for XSRF protection when adding external logins.</summary>
    private const string XsrfKey = "XsrfId";
    /// <summary>(An Action that handles HTTP POST requests) disassociates.</summary>
    ///
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="providerKey">  The provider key.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
    {
        ManageMessageId? message = null;
        IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        if(result.Succeeded)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            await SignInAsync(user, isPersistent: false);
            message = ManageMessageId.RemoveLoginSuccess;
        }
        else
        {
            message = ManageMessageId.Error;
        }
        return RedirectToAction("Manage", new { Message = message });
    }
    /// <summary>(An Action that handles HTTP POST requests) external login.</summary>
    ///
    /// <param name="provider"> The provider.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>A response stream to send to the ExternalLogin View.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider
        return View();
    }
    /// <summary>GET: /Account/ExternalLoginCallback.</summary>
    ///
    /// <param name="provider"> The provider.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string provider, string returnUrl)
    {
        // ApplicationDbContext localAppDB = new ApplicationDbContext();
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        if(loginInfo == null)
            return RedirectToAction("Login");
        var userExist = await UserManager.FindByNameAsync(loginInfo.Email);
        if(userExist != null)
        {
            var localAppUser = new ApplicationDbContext(true).Users.Where(p => p.UserName == userExist.UserName).ToList();
            if(localAppUser != null && localAppUser.Count > 0)
            {
                ApplicationContext db = new ApplicationContext(new SystemUser());
                string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                if(localAppUser != null && localAppUser.Count > 0)
                {
                    if((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                    {
                        if(await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                        {
                            ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                            return View("Login");
                        }
                    }
                    else
                    {
                        if(await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                        {
                            ModelState.AddModelError("", string.Format("Your account has been locked. Please contact your administrator. ", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                            return View("Login");
                        }
                    }
                    await SignInAsync(localAppUser[0], isPersistent: false);
                    return RedirectToAction("Index", "Home", new { isThirdParty = true });
                }
            }
        }
        if(loginInfo == null)
        {
            return RedirectToAction("Login");
        }
        // Sign in the user with this external login provider if the user already has a login
        var user = await UserManager.FindAsync(loginInfo.Login);
        if(user != null)
        {
            await SignInAsync(user, isPersistent: false);
            //return RedirectToLocal(returnUrl);
            return RedirectToAction("Index", "Home", new { isThirdParty = true });
        }
        else
        {
            // If the user does not have an account, then prompt the user to create an account
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            RegisterViewModel usermodel = new RegisterViewModel();
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }
            if(ModelState.IsValid)
            {
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if(info == null)
                {
                    return View("ExternalLoginFailure");
                }
                if(provider.ToLower() == "facebook")
                {
                    usermodel.FirstName = info.Email;
                    usermodel.LastName = info.Email;
                }
                else if(provider.ToLower() == "microsoft")
                {
                    usermodel.FirstName = info.Email;
                    usermodel.LastName = info.Email;
                }
                else
                {
                    usermodel.FirstName = info.DefaultUserName;
                    usermodel.LastName = info.DefaultUserName;
                }
                usermodel.UserName = info.Email;
                //password will genrate randomly
                string randomPassword = Membership.GeneratePassword(8, 2);
                usermodel.ConfirmPassword = randomPassword;
                usermodel.Password = randomPassword;
                //
                usermodel.Email = info.Email;
                var LogedInuser = usermodel.GetUser();
                var localAppUser = Identitydb.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
                if(localAppUser != null && localAppUser.Count > 0)
                {
                    await SignInAsync(localAppUser[0], isPersistent: false);
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Home", new { isThirdParty = true });
                }
                var result = await UserManager.CreateAsync(LogedInuser, usermodel.Password);
                if(result.Succeeded)
                {
                    var idManager = new IdentityManager();
                    // var Db = new ApplicationDbContext();
                    idManager.ClearUserRoles(LogggedInUser, LogedInuser.Id);
                    AssignDefaultRoleToNewUser(LogedInuser.Id);
                    var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                    SendEmail sendEmail = new SendEmail();
                    var UserMail = await UserManager.FindByNameAsync(usermodel.UserName);
                    if(UserMail != null)
                    {
                        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                        if(EmailTemplate != null)
                        {
                            string mailbody = string.Empty;
                            string mailsubject = string.Empty;
                            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                            {
                                mailbody = EmailTemplate.EmailContent;
                                var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                                mailbody = mailbody.Replace("###FullName###", UserMail.FirstName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + appURL + "'>here</a>");
                            }
                            mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Your registration is successful " : EmailTemplate.EmailSubject; ;
                            sendEmail.Notify(UserMail.Id, UserMail.Email, mailbody, mailsubject);
                        }
                    }
                    var Registerlogin = await AuthenticationManager.GetExternalLoginInfoAsync();
                    var RegisteruserExist = await UserManager.FindByNameAsync(Registerlogin.Email);
                    if(RegisteruserExist != null)
                    {
                        var LogedInAppUser = Identitydb.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
                        if(LogedInAppUser != null && LogedInAppUser.Count > 0)
                        {
                            await SignInAsync(LogedInAppUser[0], isPersistent: false);
                            return RedirectToLocal(returnUrl);
                            return RedirectToAction("Index", "Home", new { isThirdParty = true });
                        }
                    }
                }
                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(usermodel);
        }
    }
    /// <summary>External login callback yahoo.</summary>
    ///
    /// <param name="provider"> The provider.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallbackYahoo(string provider, string returnUrl)
    {
        // ApplicationDbContext localAppDB = new ApplicationDbContext();
        var yahooResult = OAuthWebSecurity.VerifyAuthentication();
        if(yahooResult.IsSuccessful)
        {
            var userExist = await UserManager.FindByNameAsync(yahooResult.ExtraData["email"]);
            if(userExist != null)
            {
                var localAppUser = Identitydb.Users.Where(p => p.UserName == userExist.UserName).ToList();
                if(localAppUser != null && localAppUser.Count > 0)
                {
                    ApplicationContext db = new ApplicationContext(new SystemUser());
                    string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                    if(localAppUser != null && localAppUser.Count > 0)
                    {
                        if((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                        {
                            if(await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                return View("Login");
                            }
                        }
                        else
                        {
                            if(await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked. Please contact your administrator. ", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                return View("Login");
                            }
                        }
                        await SignInAsync(localAppUser[0], isPersistent: false);
                        return RedirectToAction("Index", "Home", new { isThirdParty = true });
                    }
                }
            }
            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindByNameAsync(yahooResult.ExtraData["email"]);
            if(user != null)
            {
                await SignInAsync(user, isPersistent: false);
                //return RedirectToLocal(returnUrl);
                return RedirectToAction("Index", "Home", new { isThirdParty = true });
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = yahooResult.Provider;
                RegisterViewModel usermodel = new RegisterViewModel();
                if(User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Manage");
                }
                if(ModelState.IsValid)
                {
                    var info = OAuthWebSecurity.VerifyAuthentication();
                    if(info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    usermodel.FirstName = info.ExtraData["fullName"];
                    usermodel.LastName = info.ExtraData["fullName"];
                    usermodel.UserName = info.ExtraData["email"];
                    //password will genrate randomly
                    string randomPassword = Membership.GeneratePassword(8, 2);
                    usermodel.ConfirmPassword = randomPassword;
                    usermodel.Password = randomPassword;
                    //
                    usermodel.Email = info.UserName;
                    var LogedInuser = usermodel.GetUser();
                    var localAppUser = Identitydb.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
                    if(localAppUser != null && localAppUser.Count > 0)
                    {
                        await SignInAsync(localAppUser[0], isPersistent: false);
                        // return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Home", new { isThirdParty = true });
                    }
                    var result = await UserManager.CreateAsync(LogedInuser, usermodel.Password);
                    if(result.Succeeded)
                    {
                        var idManager = new IdentityManager();
                        //  var Db = new ApplicationDbContext();
                        idManager.ClearUserRoles(LogggedInUser, LogedInuser.Id);
                        AssignDefaultRoleToNewUser(LogedInuser.Id);
                        //idManager.AddUserToRole(LogedInuser.Id, "ReadOnly");
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                        var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                        SendEmail sendEmail = new SendEmail();
                        var UserMail = await UserManager.FindByNameAsync(usermodel.UserName);
                        if(UserMail != null)
                        {
                            string EmailBody = "Dear " + UserMail.FirstName + ", <br/><br/>";
                            EmailBody += "User Name : " + UserMail.Email + "<br/>";
                            EmailBody += "Password  : " + randomPassword + "<br/>";
                            EmailBody += "Click <a href='" + appURL + "'>here</a> to login in <b>" + CommonFunction.Instance.AppName() + "</b>";
                            sendEmail.Notify(UserMail.Id, UserMail.Email, EmailBody, CommonFunction.Instance.AppName() + " : You have been registered successfully!");
                        }
                        var Registerlogin = OAuthWebSecurity.VerifyAuthentication();
                        var RegisteruserExist = await UserManager.FindByNameAsync(Registerlogin.ExtraData["email"]);
                        if(RegisteruserExist != null)
                        {
                            var LogedInAppUser = Identitydb.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
                            if(LogedInAppUser != null && LogedInAppUser.Count > 0)
                            {
                                await SignInAsync(LogedInAppUser[0], isPersistent: false);
                                //return RedirectToLocal(returnUrl);
                                return RedirectToAction("Index", "Home", new { isThirdParty = true });
                            }
                        }
                    }
                    AddErrors(result);
                }
                ViewBag.ReturnUrl = returnUrl;
                return View(usermodel);
            }
        }
        else
        {
            return RedirectToAction("Login");
        }
    }
    /// <summary>POST: /Account/LinkLogin.</summary>
    ///
    /// <param name="provider">The provider.</param>
    ///
    /// <returns>A response stream to send to the LinkLogin View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LinkLogin(string provider)
    {
        // Request a redirect to the external login provider to link a login for the current user
        return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account",null,null,defaultPort:true), User.Identity.GetUserId());
    }
    /// <summary>GET: /Account/LinkLoginCallback.</summary>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    
    public async Task<ActionResult> LinkLoginCallback()
    {
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        if(loginInfo == null)
        {
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }
        IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        if(result.Succeeded)
        {
            return RedirectToAction("Manage");
        }
        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    }
    /// <summary> POST: /Account/ExternalLoginConfirmation.</summary>
    ///
    /// <param name="model">    The model.</param>
    /// <param name="returnUrl">URL of the return.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    {
        RegisterViewModel usermodel = new RegisterViewModel();
        if(User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Manage");
        }
        if(ModelState.IsValid)
        {
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                return View("ExternalLoginFailure");
            }
            usermodel.FirstName = info.DefaultUserName;
            usermodel.LastName = info.DefaultUserName;
            usermodel.UserName = info.DefaultUserName;
            usermodel.ConfirmPassword = "test123";
            usermodel.Password = "test123";
            usermodel.Email = info.Email;
            var user = usermodel.GetUser();
            var result = await UserManager.CreateAsync(user, usermodel.Password);
            if(result.Succeeded)
            {
                //result = await UserManager.AddLoginAsync(user.Id, info.Login);
                //if (result.Succeeded)
                //{
                await SignInAsync(user, isPersistent: false);
                var idManager = new IdentityManager();
                // var Db = new ApplicationDbContext();
                //var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(LogggedInUser, user.Id);
                AssignDefaultRoleToNewUser(user.Id);
                //idManager.AddUserToRole(user.Id, "ReadOnly");
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                return RedirectToAction("Index", "Home");
                //}
            }
            AddErrors(result);
        }
        ViewBag.ReturnUrl = returnUrl;
        return View(usermodel);
    }
    /// <summary>GET: /Account/ExternalLoginFailure.</summary>
    ///
    /// <returns>A response stream to send to the ExternalLoginFailure View.</returns>
    [AllowAnonymous]
    public ActionResult ExternalLoginFailure()
    {
        return View();
    }
    /// <summary>Clears the cookies.</summary>
    private void ClearCookies()
    {
        RemoveCookie("PageId");
        Response.Cookies["fltCookie"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies["fltCookieFltTabId"].Expires = DateTime.UtcNow.AddDays(-1);
        Response.Cookies.Clear();
        Session.Clear();
        Response.Cookies["PageId"].Expires = DateTime.UtcNow.AddDays(-1);
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
    }
    
    /// <summary>Removes the account list.</summary>
    ///
    /// <returns>A response stream to send to the RemoveAccountList View.</returns>
    [ChildActionOnly]
    public ActionResult RemoveAccountList()
    {
        var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
    }
    /// <summary>Encapsulates the result of a challenge.</summary>
    private class ChallengeResult : HttpUnauthorizedResult
    {
        /// <summary>Constructor.</summary>
        ///
        /// <param name="provider">   The provider.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }
        /// <summary>Gets or sets the login provider.</summary>
        ///
        /// <value>The login provider.</value>
        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }
        
        /// <summary>Gets or sets the login provider.</summary>
        ///
        /// <value>The login provider.</value>
        public string LoginProvider
        {
            get;
            set;
        }
        /// <summary>Gets or sets URI of the redirect.</summary>
        ///
        /// <value>The redirect URI.</value>
        public string RedirectUri
        {
            get;
            set;
        }
        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        public string UserId
        {
            get;
            set;
        }
        /// <summary>Enables processing of the result of an action method by a custom type that inherits
        /// from the <see cref="T:System.Web.Mvc.ActionResult" /> class.</summary>
        ///
        /// <param name="context">  The context in which the result is executed. The context information
        ///                         includes the controller, HTTP content, request context, and route
        ///                         data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties()
            {
                RedirectUri = RedirectUri
            };
            if(UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
    #endregion
    /// <summary>Generate JWT token for user.</summary>
    ///
    /// <returns>Return JSON token.</returns>
    [HttpGet]
    public ActionResult GenerateJWT()
    {
        if(!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        JWTHelper helper = new JWTHelper();
        var user = new ApplicationDbContext().Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
        var result = helper.GetJWToken(user);
        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ValidateJWT(string token)
    {
        JWTHelper helper = new JWTHelper();
        bool result = false;
        try
        {
            result = helper.VerifyJWT(token);
        }
        catch
        {
            Response.Redirect(Request.UrlReferrer.ToString(),true);
        }
        if(result)
        {
            var principal = helper.GetJWTClaims(token);
            var claims = principal.Claims;
            var user = new ApplicationUser()
            {
                UserName = claims.FirstOrDefault(p => p.Type == "UserName").Value,
                FirstName = claims.FirstOrDefault(p => p.Type == "FirstName").Value,
                LastName = claims.FirstOrDefault(p => p.Type == "LastName").Value,
                Email = claims.FirstOrDefault(p => p.Type == "Email").Value,
                NotifyForEmail=false,
            };
            var userbyEmail = await UserManager.FindByEmailAsync(user.Email);
            var userbyName = await UserManager.FindByNameAsync(user.UserName);
            if(userbyName == null && userbyEmail == null)
            {
                var resultuser = await UserManager.CreateAsync(user, "test123");
                if(resultuser.Succeeded)
                {
                    AfterUserCreate(user, "JWT");
                    AssignDefaultRoleToNewUser(user.Id);
                }
            }
            var localAppUser = Identitydb.Users.FirstOrDefault(p => p.UserName == user.UserName);
            if(localAppUser != null)
            {
                await SignInAsync(localAppUser, isPersistent: false);
            }
        }
        return RedirectToAction("Index", "Home");
    }
    
    //Two factor change
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> EmailNotVerified(string UserId, string returnUrl)
    {
        if(UserId == null)
        {
            return View("Error");
        }
        var user = await UserManager.FindByIdAsync(UserId);
        return View(new ResendViewModel()
        {
            UserId = user.Id, ReturnUrl = returnUrl
        });
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> EmailNotVerified(ResendViewModel model, string returnUrl)
    {
        if(model.UserId == null)
        {
            return View("Error");
        }
        var user = await UserManager.FindByIdAsync(model.UserId);
        SendEmail sendEmail = new SendEmail();
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Verify Email");
        if(EmailTemplate != null)
        {
            string mailbody = string.Empty;
            string mailsubject = string.Empty;
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                try
                {
                    var code = UserManager.GenerateShortEmailConfirmationToken(user.Id);
                    var callbackUrl = Url.Action("VerifyEmail", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme,defaultPort:true);
                    mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###CODE###", code,StringComparison.OrdinalIgnoreCase).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###AppName###", CommonFunction.Instance.AppName());
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Account Verification Email " : EmailTemplate.EmailSubject; ;
            //sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
            var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
            if(emailstatus != "EmailSent")
                ViewBag.Message = emailstatus;
            else
                return RedirectToAction("EmailCodeNotVerified", new { UserId = user.Id, returnUrl = returnUrl });
        }
        ViewBag.Message = "Verification email has been sent to your registered email.";
        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> EmailCodeNotVerified(string UserId, string code, string mode, string returnUrl)
    {
        if(UserId == null)
        {
            return View("Error");
        }
        var user = await UserManager.FindByIdAsync(UserId);
        ViewBag.user = user;
        ViewData["EmailCode"] = code;
        ViewData["mode"] = mode;
        return View(new ResendViewModel()
        {
            UserId = user.Id,
            ReturnUrl = returnUrl
        });
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> EmailCodeNotVerified(ResendViewModel model, string EmailCode, string mode, string returnUrl)
    {
        if(model.UserId == null || EmailCode == null)
        {
            return View("Error");
        }
        bool result;
        try
        {
            ViewData["EmailCode"] = EmailCode;
            ViewData["mode"] = mode;
            ViewBag.user = (new ApplicationDbContext(new SystemUser())).Users.Find(model.UserId);
            if(!(await UserManager.IsEmailConfirmedAsync(model.UserId)))
            {
                result = UserManager.ConfirmShortEmail(model.UserId, EmailCode);
                if(result)
                {
                    ApplicationUser user = null;
                    using(var db = (new ApplicationDbContext(new SystemUser())))
                    {
                        user = db.Users.Find(model.UserId);
                        user.EmailConfirmed = true;
                        db.SaveChanges();
                    }
                    ViewBag.Successful = true;
                    SendEmail sendEmail = new SendEmail();
                    var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                    if(EmailTemplate != null)
                    {
                        string mailbody = string.Empty;
                        string mailsubject = string.Empty;
                        if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                        {
                            mailbody = EmailTemplate.EmailContent;
                            var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                            mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>");
                        }
                        mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Your registration is successful " : EmailTemplate.EmailSubject; ;
                        //sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                        var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                        if(emailstatus != "EmailSent")
                        {
                            //AddErrors(result);
                            ViewBag.errorMessage = emailstatus;
                            return Json(new { result = "failure", message = emailstatus }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                            //return View(model);
                        }
                    }
                    ViewBag.message = "Your email address has been verified successfully.";
                    return Json(new {result="ok",message="Your email address has been verified successfully."}, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    // return View(model);
                }
            }
            else
            {
                ViewBag.message = "You have already verified your email address.";
                return Json(new { result = "failure", message = "Your email address has already verified." }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                //return View(model);
            }
        }
        catch(InvalidOperationException ioe)
        {
            // ConfirmEmailAsync throws when the userId is not found.
            //ViewBag.errorMessage = ioe.Message;
            return Json(new { result = "failure", message = ioe.Message }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //return View();
        }
        // If we got this far, something failed.
        //AddErrors(result);
        ViewBag.errorMessage = "Verify email failed";
        return Json(new { result = "failure", message = "Verification Failed ! Please re-enter code." }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public ActionResult ResendShortEmailCode(string userId)
    {
        var user = new ApplicationDbContext(new SystemUser()).Users.Find(userId);
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Verify Email");
        if(EmailTemplate != null)
        {
            SendEmail sendEmail = new SendEmail();
            string mailbody = string.Empty;
            string mailsubject = string.Empty;
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                string IsVerifyUserEmail = ConfigurationManager.AppSettings["VerifyUserEmail"];
                var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme, defaultPort: true);
                if(!string.IsNullOrEmpty(IsVerifyUserEmail))
                {
                    var code = UserManager.GenerateShortEmailConfirmationToken(user.Id);
                    callbackUrl = Url.Action("EmailCodeNotVerified", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme, defaultPort: true);
                    mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", "" + callbackUrl + "").Replace("###CODE###", "" + code + "",StringComparison.OrdinalIgnoreCase);
                }
                else
                    mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###CODE###", "" + "" + "",StringComparison.OrdinalIgnoreCase);
            }
            mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Account Verification Email " : EmailTemplate.EmailSubject; ;
            var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
            return Json(emailstatus, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Failed", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    // GET: /Account/SendCode
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(SendCodeViewModel model, string returnUrl)
    {
        if(model.UserId == null)
        {
            return View("Error");
        }
        var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(model.UserId);
        var factorOptions = userFactors.Select(purpose => new SelectListItem
        {
            Text = purpose, Value = purpose
        }).ToList();
        using(ApplicationContext db = new ApplicationContext(new SystemUser()))
        {
            var appSettings = db.AppSettings;
            var twofactorauthenticationenablephonecodeinfo = appSettings.Where(p => p.Key == "TwoFactorAuthenticationEnablePhoneCode").FirstOrDefault();
            if(twofactorauthenticationenablephonecodeinfo != null)
            {
                if(twofactorauthenticationenablephonecodeinfo.Value.ToLower() == "no")
                {
                    factorOptions.Remove(factorOptions.Find(p => p.Value == "Phone Code"));
                }
            }
        }
        //if (factorOptions.Count == 1)
        //{
        //    // No manual selection required
        //    return RedirectToAction("VerifyCode", "Account", new { UserId = model.UserId, Provider = factorOptions[0].Value, ReturnUrl = model.ReturnUrl });
        //}
        return View(new SendCodeViewModel { UserId = model.UserId, Providers = factorOptions, ReturnUrl = model.ReturnUrl });
    }
    
    //
    // POST: /Account/SendCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SendCode(SendCodeViewModel model)
    {
        // Generate the token and send it
        if(!ModelState.IsValid)
        {
            return View();
        }
        var token = await UserManager.GenerateTwoFactorTokenAsync(model.UserId, model.SelectedProvider);
        // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
        var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(model.UserId);
        var factorOptions = userFactors.Select(purpose => new SelectListItem
        {
            Text = purpose, Value = purpose
        }).ToList();
        try
        {
            await UserManager.NotifyTwoFactorTokenAsync(model.UserId, model.SelectedProvider, token);
        }
        catch(Exception ex)
        {
            ViewBag.SMSFailureMessage = "Twilio Setting Issue : " + ex.Message;
            return View(new SendCodeViewModel { UserId = model.UserId, Providers = factorOptions, ReturnUrl = model.ReturnUrl });
        }
        ViewBag.SMSFailureMessage = "";
        return RedirectToAction("VerifyCode", "Account", new { UserId = model.UserId, Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
    }
    
    //
    // GET: /Account/VerifyCode
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyCode(string UserId, string Provider, string ReturnUrl)
    {
        if(UserId == null)
        {
            return View("Error");
        }
        var user = await UserManager.FindByIdAsync(UserId);
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        if(Request.Cookies[AppUrl + "CurrentRole"] == null)
        {
            return View(new VerifyCodeViewModel { UserId = UserId, Provider = Provider, ReturnUrl = ReturnUrl });
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    
    //
    // POST: /Account/VerifyCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var result = new SignInStatus();
        if(model.UserId == null)
        {
            result = SignInStatus.Failure;
        }
        var user = await UserManager.FindByIdAsync(model.UserId);
        if(user == null)
        {
            result = SignInStatus.Failure;
        }
        if(await UserManager.IsLockedOutAsync(user.Id))
        {
            result = SignInStatus.LockedOut;
        }
        if(await UserManager.VerifyTwoFactorTokenAsync(user.Id, model.Provider, model.Code))
        {
            // When token is verified correctly, clear the access failed count used for lockout
            await UserManager.ResetAccessFailedCountAsync(user.Id);
            await SignInAsync(user, false);
            result = SignInStatus.Success;
        }
        else
        {
            await UserManager.AccessFailedAsync(user.Id);
            result = SignInStatus.Failure;
        }
        switch(result)
        {
        case SignInStatus.Success:
            DoAuditEntry.AddJournalEntry("Account", user.UserName, "Login", "Successful", Request.UserHostAddress);
            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
            {
                var userid = user.Id;
                LoginAttempts history = new LoginAttempts();
                history.UserId = userid;
                history.Date = DateTime.UtcNow;
                history.IsSuccessfull = true;
                history.IPAddress = Request.UserHostAddress;
                usercontext.LoginAttempts.Add(history);
                usercontext.SaveChanges();
            }
            return RedirectToLocal(model.ReturnUrl);
        case SignInStatus.LockedOut:
            ModelState.AddModelError("", "Locked out");
            return View(model);
        case SignInStatus.Failure:
        default:
            ModelState.AddModelError("Code", "Invalid code.");
            return View(model);
        }
    }
    
    [AllowAnonymous]
    public async Task<ActionResult> VerifyEmail(string userId, string code)
    {
        if(userId == null || code == null)
        {
            return View("Error");
        }
        IdentityResult result;
        try
        {
            if(!(await UserManager.IsEmailConfirmedAsync(userId)))
            {
                result = await UserManager.ConfirmEmailAsync(userId, code);
                if(result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    SendEmail sendEmail = new SendEmail();
                    var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                    if(EmailTemplate != null)
                    {
                        string mailbody = string.Empty;
                        string mailsubject = string.Empty;
                        if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                        {
                            mailbody = EmailTemplate.EmailContent;
                            var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme,defaultPort:true);
                            mailbody = mailbody.Replace("###FullName###", user.FirstName + " " + user.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>");
                        }
                        mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " : Your registration is successful " : EmailTemplate.EmailSubject; ;
                        //sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                        var emailstatus = sendEmail.Notify(user.Id, user.Email, mailbody, mailsubject);
                        if(emailstatus != "EmailSent")
                        {
                            AddErrors(result);
                            ViewBag.errorMessage = emailstatus;
                            return View();
                        }
                    }
                    ViewBag.message = "Your email address has been verified successfully. Thank you";
                    return View();
                }
            }
            else
            {
                ViewBag.message = "You have already verified your email address. Thank you";
                return View();
            }
        }
        catch(InvalidOperationException ioe)
        {
            // ConfirmEmailAsync throws when the userId is not found.
            ViewBag.errorMessage = ioe.Message;
            return View();
        }
        // If we got this far, something failed.
        AddErrors(result);
        ViewBag.errorMessage = "Verify email failed";
        return View();
    }
}
}
