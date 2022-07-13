using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Configuration;
using Microsoft.Owin.Security.Cookies;

using Owin;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Controllers.OtherDefaultControllers
{
/// <summary>A controller for handling application settings.</summary>
public class AppSettingController : BaseController
{
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index()
    {
        var lstAppSetting = from s in db.AppSettings
                            select s;
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder").Value != ConfigurationManager.AppSettings["ReportFolder"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder");
            obj.Value = CommonFunction.Instance.ReportFolder();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "needsharedusersystem").Value))
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "needsharedusersystem");
            obj.Value = CommonFunction.Instance.NeedSharedUserSystem();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl").Value != ConfigurationManager.AppSettings["AppURL"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl");
            obj.Value = CommonFunction.Instance.AppURL();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname").Value != ConfigurationManager.AppSettings["AppName"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname");
            obj.Value = CommonFunction.Instance.AppName();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(lstAppSetting.Where(s => s.Key.ToLower() == "domainname").Count() > 0)
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "domainname");
            obj.Value = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(lstAppSetting.Where(s => s.Key.ToLower() == "useactivedirectory").Count() > 0)
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "useactivedirectory");
            obj.Value = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"];
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(lstAppSetting.Where(s => s.Key.ToLower() == "useactivedirectoryrole").Count() > 0)
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "useactivedirectoryrole");
            obj.Value = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(lstAppSetting.Where(s => s.Key.ToLower() == "administratorroles").Count() > 0)
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "administratorroles");
            obj.Value = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        //google analytics settings
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics").Value != ConfigurationManager.AppSettings["enableGA"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics");
            obj.Value = CommonFunction.Instance.EnableGoogleAnalytics();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id").Value != ConfigurationManager.AppSettings["trackingID"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id");
            obj.Value = CommonFunction.Instance.TrackingID();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        if(string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name").Value != ConfigurationManager.AppSettings["customdimensionname"])
        {
            AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name");
            obj.Value = CommonFunction.Instance.CustomDimensionName();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        //
        var _AppSetting = lstAppSetting.Include(t => t.associatedappsettinggroup);
        ViewBag.AppSettingGroup = db.AppSettingGroups.ToList();
        bool HasThirdParty = CommonFunction.Instance.HasThirdParty();
        ViewBag.HasThirdParty = HasThirdParty;
        if(!Request.IsAjaxRequest())
            return View(_AppSetting);
        else
            return PartialView("IndexPartial", _AppSetting);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a setting.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateSetting View.</returns>
    
    public ActionResult CreateSetting(string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("AppSetting"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingParentUrl"] = UrlReferrer;
        var objAssociatedAppSettingGroup = new List<AppSettingGroup>();
        ViewBag.AssociatedAppSettingGroupID = new SelectList(objAssociatedAppSettingGroup, "ID", "DisplayValue");
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a setting.</summary>
    ///
    /// <param name="appsetting"> The appsetting.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateSetting View.</returns>
    
    [HttpPost]
    public ActionResult CreateSetting([Bind(Include = "Id,ConcurrencyKey,Key,Value,AssociatedAppSettingGroupID,Description,IsDefault")] AppSetting appsetting, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.AppSettings.Add(appsetting);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit setting.</summary>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditSetting View.</returns>
    
    public ActionResult EditSetting(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("AppSetting"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AppSetting appsetting = db.AppSettings.Find(id);
        if(appsetting == null)
        {
            return HttpNotFound();
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingParentUrl"] = UrlReferrer;
        var objAssociatedAppSettingGroup = new List<AppSettingGroup>();
        objAssociatedAppSettingGroup.Add(appsetting.associatedappsettinggroup);
        ViewBag.AssociatedAppSettingGroupID = new SelectList(objAssociatedAppSettingGroup, "ID", "DisplayValue", appsetting.AssociatedAppSettingGroupID);
        return View(appsetting);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit setting.</summary>
    ///
    /// <param name="appsetting"> The appsetting.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditSetting View.</returns>
    
    [HttpPost]
    public ActionResult EditSetting([Bind(Include = "Id,ConcurrencyKey,Key,Value,AssociatedAppSettingGroupID,Description,IsDefault")] AppSetting appsetting, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            if(appsetting.Key.ToLower() == "reportpass")
                appsetting.Value = (new EncryptDecrypt()).EncryptString(appsetting.Value);
            if(appsetting.Key.ToLower() == "googlemapapikey")
            {
                if(appsetting.Value != "****")
                {
                    appsetting.Value = Encryptdata(appsetting.Value);
                    db.Entry(appsetting).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    appsetting.Value = appsetting.Value;
                    db.Entry(appsetting).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            db.Entry(appsetting).State = EntityState.Modified;
            db.SaveChanges();
            //Two Factor Changes
            //set session time out
            if(appsetting.Key == "ApplicationSessionTimeOut")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["SessionTimeOut"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            //
            //Changing Application Name
            if(appsetting.Key == "AppName")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["AppName"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            //google analytics settings
            if(appsetting.Key.ToLower() == "enable google analytics")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["enableGA"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key.ToLower() == "tracking id")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["trackingID"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key.ToLower() == "custom dimension name")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["customdimensionname"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key.ToLower() == "preventmultiplelogin")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["ResourceLocalizationChange"].Value = Convert.ToString(DateTime.UtcNow.Ticks);
                myConfiguration.Save();
            }
            if(appsetting.Key.ToLower() == "userevalee" || appsetting.Key.ToLower() == "scheduledtaskcallbacktime")
            {
                var appurl = System.Configuration.ConfigurationManager.AppSettings["AppURL"];
                ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext();
                {
                    var callbacks = sthcontext.ScheduledTaskHistorys.Where(p => p.Status == "System" && p.TaskName == appurl);
                    foreach(var c in callbacks)
                    {
                        Uri myUri = new Uri(c.CallbackUri);
                        try
                        {
                            Revalee.Client.RevaleeRegistrar.CancelCallback(Guid.Parse(c.GUID), myUri);
                        }
                        catch(Exception ex)
                        {
                            continue;
                        }
                    }
                }
                var userevalee = CommonFunction.Instance.UseRevalee();
                double CallBackSpan = CommonFunction.Instance.ScheduledTaskCallbackTime();
                if(Convert.ToString(appsetting.Value).ToLower() == "yes" || double.TryParse(appsetting.Value, out CallBackSpan))
                {
                    if(appsetting.Key.ToLower() == "scheduledtaskcallbacktime")
                    {
                        if(appsetting.Value != Convert.ToString(CallBackSpan) && double.TryParse(appsetting.Value, out CallBackSpan))
                            CallBackSpan = Convert.ToInt64(CallBackSpan);
                    }
                    Uri callbackUrl = new Uri(string.Format("http://localhost//" + appurl + "//TimeBasedAlert/Run"));
                    var callbackid = Revalee.Client.RevaleeRegistrar.ScheduleCallback(DateTime.Now.AddMinutes(CallBackSpan), callbackUrl);
                    ScheduledTaskHistory nextitemhistory = new ScheduledTaskHistory();
                    nextitemhistory.Status = "System";
                    nextitemhistory.BusinessRuleId = null;
                    sthcontext.Entry(nextitemhistory).State = EntityState.Added;
                    nextitemhistory.TaskName = appurl;
                    nextitemhistory.CallbackUri = Convert.ToString(callbackUrl);
                    nextitemhistory.GUID = Convert.ToString(callbackid);
                    sthcontext.SaveChanges();
                }
            }
            //
            //For Third Party.
            if(appsetting.Key == "GoogleAuthenticationId")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["GoogleAuthenticationId"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key == "GoogleAuthenticationSecret")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["GoogleAuthenticationSecret"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key == "FacbookAuthenticationId")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["FacbookAuthenticationId"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key == "FacbookAuthenticationSecret")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["FacbookAuthenticationSecret"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            if(appsetting.Key == "ThirdPartyLoginEnabled")
            {
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["ThirdPartyLoginEnabled"].Value = Convert.ToString(appsetting.Value);
                myConfiguration.Save();
            }
            //
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="confirm"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public ActionResult SetVerifyUserEmail(string confirm)
    {
        // If we got this far, something failed, redisplay form
        var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        myConfiguration.AppSettings.Settings["VerifyUserEmail"].Value = confirm;
        myConfiguration.Save();
        return RedirectToAction("Index");
    }
    /// <summary>Deletes the setting.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the DeleteSetting View.</returns>
    
    public ActionResult DeleteSetting(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanDelete("AppSetting"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AppSetting appsetting = db.AppSettings.Find(id);
        if(appsetting == null)
        {
            throw(new Exception("Deleted"));
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingParentUrl"] = UrlReferrer;
        return View(appsetting);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) deletes the setting confirmed.</summary>
    ///
    /// <param name="appsetting"> The appsetting.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteSettingConfirmed View.</returns>
    
    [HttpPost]
    public ActionResult DeleteSettingConfirmed(AppSetting appsetting, string UrlReferrer)
    {
        if(!User.CanDelete("AppSetting"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(appsetting).State = EntityState.Deleted;
        db.AppSettings.Remove(appsetting);
        db.SaveChanges();
        return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a group.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateGroup View.</returns>
    
    public ActionResult CreateGroup(string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("AppSettingGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingGroupParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a group.</summary>
    ///
    /// <param name="appsettinggroup">The appsettinggroup.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    /// <param name="IsAddPop">       The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateGroup View.</returns>
    
    [HttpPost]
    public ActionResult CreateGroup([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] AppSettingGroup appsettinggroup, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.AppSettingGroups.Add(appsettinggroup);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit group.</summary>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditGroup View.</returns>
    
    public ActionResult EditGroup(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("AppSettingGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AppSettingGroup appsettinggroup = db.AppSettingGroups.Find(id);
        if(appsettinggroup == null)
        {
            return HttpNotFound();
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingGroupParentUrl"] = UrlReferrer;
        return View(appsettinggroup);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit group.</summary>
    ///
    /// <param name="appsettinggroup">The appsettinggroup.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    /// <param name="IsAddPop">       The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditGroup View.</returns>
    
    [HttpPost]
    public ActionResult EditGroup([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] AppSettingGroup appsettinggroup, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.Entry(appsettinggroup).State = EntityState.Modified;
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>Deletes the group.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the DeleteGroup View.</returns>
    
    public ActionResult DeleteGroup(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanDelete("AppSettingGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AppSettingGroup appsettinggroup = db.AppSettingGroups.Find(id);
        if(appsettinggroup == null)
        {
            throw(new Exception("Deleted"));
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["AppSettingGroupParentUrl"] = UrlReferrer;
        return View(appsettinggroup);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) deletes the group confirmed.</summary>
    ///
    /// <param name="appsettinggroup">The appsettinggroup.</param>
    /// <param name="UrlReferrer">    The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteGroupConfirmed View.</returns>
    
    [HttpPost]
    public ActionResult DeleteGroupConfirmed(AppSettingGroup appsettinggroup, string UrlReferrer)
    {
        if(!User.CanDelete("AppSettingGroup"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(appsettinggroup).State = EntityState.Deleted;
        db.AppSettingGroups.Remove(appsettinggroup);
        db.SaveChanges();
        return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Applies the setting.</summary>
    ///
    /// <returns>A response stream to send to the ApplySetting View.</returns>
    
    public ActionResult ApplySetting()
    {
        var appsettinglist = new ApplicationContext(new SystemUser()).AppSettings.ToList();
        var twofactorenabled = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twofactorauthenticationenable");
        var allowedroles2fa = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "allowedroles2fa");
        var allowedroles2falist = allowedroles2fa == null || allowedroles2fa.Value == "None" ? new List<string>() : allowedroles2fa.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        if(twofactorenabled.Value.ToLower() == "yes")
        {
            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
            {
                var userlist = usercontext.Users.ToList();//.Where(u => u.UserName.ToLower() != "admin").ToList();
                foreach(var user in userlist)
                {
                    var roleIds = user.Roles.Select(r => r.RoleId);
                    var roles = usercontext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();
                    var rolelist = roles.OrderBy(p => p).ToList();
                    if(allowedroles2falist.Intersect(rolelist).Count() > 0)
                        user.TwoFactorEnabled = false;
                    else
                        user.TwoFactorEnabled = true;
                    user.PhoneNumberConfirmed = true;
                    usercontext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                }
                if(userlist.Count > 0)
                    usercontext.SaveChanges();
            }
        }
        else
        {
            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
            {
                var userlist = usercontext.Users.Where(u => u.TwoFactorEnabled).ToList();
                foreach(var user in userlist)
                {
                    user.TwoFactorEnabled = false;
                    user.PhoneNumberConfirmed = false;
                    usercontext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                }
                if(userlist.Count > 0)
                    usercontext.SaveChanges();
            }
        }
        QueryCacheManager.ExpireAll();
        CommonFunction.ResetInstance();
        return RedirectToAction("Index", "AppSetting");
    }
    
    /// <summary>Cancels.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    
    public ActionResult Cancel(string UrlReferrer)
    {
        if(!string.IsNullOrEmpty(UrlReferrer))
        {
            var query = HttpUtility.ParseQueryString(UrlReferrer);
            if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                return RedirectToAction("Index");
            else
                return Redirect(UrlReferrer);
        }
        else
            return RedirectToAction("Index");
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<AppSettingGroup> list = db.AppSettingGroups;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.AppSettingGroups;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(AppSettingGroup), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<AppSettingGroup, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<AppSettingGroup>)q);
            }
        }
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    ///                               /// <summary>After save.</summary>
    ///
    /// <param name="appsetting">The Account.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(AppSetting appsetting, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
    private string Encryptdata(string key)
    {
        string strmsg = string.Empty;
        byte[] encode = new byte[key.Length];
        encode = System.Text.Encoding.UTF8.GetBytes(key);
        strmsg = Convert.ToBase64String(encode);
        return strmsg;
    }
}

}
/// <summary>An application builder provider.</summary>
public class AppBuilderProvider : IDisposable
{
    /// <summary>The application.</summary>
    private Owin.IAppBuilder _app;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="app">The application.</param>
    
    public AppBuilderProvider(Owin.IAppBuilder app)
    {
        _app = app;
    }
    
    /// <summary>Gets the get.</summary>
    ///
    /// <returns>An Owin.IAppBuilder.</returns>
    
    public Owin.IAppBuilder Get()
    {
        return _app;
    }
    
    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.</summary>
    
    public void Dispose() { }
}