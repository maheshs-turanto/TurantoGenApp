using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Http;
using System.Web.Routing;
using System.Globalization;
using System.Data.Entity;
namespace GeneratorBase.MVC
{
/// <summary>Global event handler of a MVC web application.</summary>
public class MvcApplication : System.Web.HttpApplication
{
    /// <summary> Code that runs on every request.</summary>
    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        if(CommonFunction.Instance.FrontDoorEnable() != "Yes")
            return;
        var fdId = CommonFunction.Instance.FrontDoorId();
        var fdUrl = CommonFunction.Instance.FrontDoorUrl();
        if(String.IsNullOrEmpty(fdId) || String.IsNullOrEmpty(fdUrl))
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new ArgumentException("Front Door is Enabled, but Front Door Id and/or Front Doot Url are missing."));
            return;
        }
        if(fdId.Equals(this.Request.Headers["X-Azure-FDID"]))
            return;
        Response.StatusCode = (int)System.Net.HttpStatusCode.MovedPermanently;
        Response.Redirect(fdUrl);
        Response.End();
    }
    /// <summary> Code that runs on application startup.</summary>
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        ViewEngines.Engines.Clear();
        ViewEngines.Engines.Add(new RazorViewEngine());
        var engine = ViewEngines.Engines.OfType<RazorViewEngine>().First();
        var theme = System.Configuration.ConfigurationManager.AppSettings["AppTheme"];
        if(theme == null || theme == "DefaultCompress")
        {
            theme = "Default";    //Change By Ashok 3/31/2020
        }
        if(theme == "Default1")
        {
            theme = "Default1";    //Change By Ashok 2/23/2021
        }
        if(theme == "Angular") theme = "";
        engine.ViewLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        engine.MasterLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        engine.PartialViewLocationFormats = new[]
        {
            "~/Views"+theme+"/{1}/{0}.cshtml",
            "~/Views"+theme+"/{1}/{0}.vbhtml",
            "~/Views"+theme+"/Shared/{0}.cshtml",
            "~/Views"+theme+"/Shared/{0}.vbhtml",
            "~/Views"+theme+"/Shared/PartialViews/{0}.cshtml",
            "~/Views"+theme+"/Shared/Layouts/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/{0}.vbhtml",
        };
        ModelBinders.Binders.Add(typeof(Decimal), new DecimalModelBinder());
        ModelBinders.Binders.Add(typeof(Nullable<Decimal>), new DecimalModelBinder());
        //ModelBinders.Binders.Add(typeof(string), new TrimModelBinder());
    }
    /// <summary>Application post authenticate request.</summary>
    ///
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">     Event information.</param>
    protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
    {
        HttpContext.Current.User = new CustomPrincipal(User);
    }
    /// <summary>Application authorize request.</summary>
    ///
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">     Event information.</param>
    protected void Application_AuthorizeRequest(Object sender, EventArgs e)
    {
        if(User.Identity.IsAuthenticated)
        {
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            var maintenancemode = commonObj.MaintenanceMode().ToLower() == "true" ? true : false;
            var isAdmin = ((CustomPrincipal)User).IsAdminUser();
            var roles = ((CustomPrincipal)User).GetRoles();
            if(maintenancemode && !isAdmin)
            {
                var allroles = ((CustomPrincipal)User).GetAllRoles();
                if(!(((CustomPrincipal)User).IsInRole(commonObj.MaintenanceModeRoles().Split(",".ToCharArray()), allroles)))
                    if(!HttpContext.Current.Request.Url.AbsolutePath.Contains("Account/LogOff"))
                        HttpContext.Current.RewritePath("~/Error/UnderMaintenance");
            }
            List<Permission> permissions = new List<Permission>();
            ((CustomPrincipal)User).IsAdmin = isAdmin;
            List<PermissionAdminPrivilege> adminprivilegeslist = new List<PermissionAdminPrivilege>();
            ((CustomPrincipal)User).userroles = roles.ToList();
            using(var pc = new PermissionContext())
            {
                pc.Configuration.LazyLoadingEnabled = false;
                pc.Configuration.AutoDetectChangesEnabled = false;
                // so we only make one database call instead of one per entity?
                var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).GetFromCache<IQueryable<Permission>, Permission>().ToList();
                var adminprivileges = pc.AdminPrivileges.Where(p => roles.Contains(p.RoleName)).GetFromCache<IQueryable<PermissionAdminPrivilege>, PermissionAdminPrivilege>().ToList();
                foreach(var item in (new AdminFeaturesDictionary()).getDictionary())
                {
                    var adminprivilege = new PermissionAdminPrivilege();
                    var raw = adminprivileges.Where(p => p.AdminFeature == item.Key);
                    adminprivilege.AdminFeature = item.Key;
                    adminprivilege.IsAllow = isAdmin || raw.Any(p => p.IsAllow);
                    adminprivilege.IsAdd = isAdmin || raw.Any(p => p.IsAdd);
                    adminprivilege.IsEdit = isAdmin || raw.Any(p => p.IsEdit);
                    adminprivilege.IsDelete = isAdmin || raw.Any(p => p.IsDelete);
                    adminprivilegeslist.Add(adminprivilege);
                }
                ((CustomPrincipal)User).adminprivileges = adminprivilegeslist;
                foreach(var entity in GeneratorBase.MVC.ModelReflector.Entities)
                {
                    var calculated = new Permission();
                    var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                    calculated.EntityName = entity.Name;
                    calculated.CanEdit = isAdmin || raw.Any(p => p.CanEdit);
                    calculated.CanDelete = isAdmin || raw.Any(p => p.CanDelete);
                    calculated.CanAdd = isAdmin || raw.Any(p => p.CanAdd);
                    calculated.CanView = isAdmin || raw.Any(p => p.CanView);
                    calculated.IsOwner = raw.Any(p => p.IsOwner!=null && p.IsOwner.Value);
                    calculated.ViewR = raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.ViewR)) != null ? raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.ViewR)).ViewR.Replace(",", " && ") : "";
                    calculated.EditR = calculated.EditR = raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.EditR)) != null ? raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.EditR)).EditR.Replace(",", " && ") : "";
                    calculated.DeleteR = calculated.DeleteR = raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.DeleteR)) != null ? raw.FirstOrDefault(p => !string.IsNullOrEmpty(p.DeleteR)).DeleteR.Replace(",", " && ") : "";
                    if(!isAdmin)
                        calculated.SelfRegistration = raw.Any(p => p.SelfRegistration != null && p.SelfRegistration.Value);
                    else calculated.SelfRegistration = false;
                    if(calculated.IsOwner != null && calculated.IsOwner.Value)
                        calculated.UserAssociation = raw.FirstOrDefault(p => p.IsOwner != null && p.IsOwner.Value).UserAssociation;
                    else
                        calculated.UserAssociation = string.Empty;
                    //code for verb action security
                    var verblist = raw.Select(x => x.Verbs).ToList();
                    //var verbrolecount = verblist.Count();
                    List<string> allverbs = new List<string>();
                    foreach(var verb in verblist)
                    {
                        if(verb != null)
                            allverbs.AddRange(verb.Split(",".ToCharArray()).ToList());
                    }
                    var blockedverbs = allverbs.GroupByMany(p => p);
                    if(blockedverbs.Count() > 0)
                        calculated.Verbs = string.Join(",", blockedverbs.Select(b => b.Key).ToList());
                    else
                        calculated.Verbs = string.Empty;
                    //
                    //FLS
                    if(!isAdmin)
                    {
                        var listEdit = raw.Where(p => p.CanEdit).GroupBy(w => w.RoleName).Select(g => new
                        {
                            rolename = g.Key, propertynames = string.Join(",", g.Select(p => p.NoEdit == null ? "" : p.NoEdit))
                        }).ToList();
                        var listView = raw.Where(p => p.CanView).GroupBy(w => w.RoleName).Select(g => new
                        {
                            rolename = g.Key, propertynames = string.Join(",", g.Select(p => p.NoView == null ? "" : p.NoView))
                        }).ToList();
                        var resultEdit = "";
                        var resultView = "";
                        if(listView.Count > 0)
                        {
                            HashSet<string> set = new HashSet<string>(listView[0].propertynames.Split(','));
                            foreach(var item in listView.Skip(1))
                            {
                                set.IntersectWith(item.propertynames.Split(','));
                            }
                            resultView = string.Join(",", set);
                        }
                        if(listEdit.Count > 0)
                        {
                            HashSet<string> set = new HashSet<string>(listEdit[0].propertynames.Split(','));
                            foreach(var item in listEdit.Skip(1))
                            {
                                set.IntersectWith(item.propertynames.Split(','));
                            }
                            resultEdit = string.Join(",", set);
                        }
                        calculated.NoEdit = resultEdit;
                        calculated.NoView = resultView;
                    }
                    //
                    permissions.Add(calculated);
                }
            }
            ((CustomPrincipal)User).permissions = permissions;
            List<BusinessRule> businessrules = new List<BusinessRule>();
            using(var br = new BusinessRuleContext())
            {
                br.Configuration.LazyLoadingEnabled = false;
                br.Configuration.AutoDetectChangesEnabled = false;
                List<BusinessRule> brList = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID != 5).Include(t => t.ruleconditions).Include(t => t.ruleaction).GetFromCache<IQueryable<BusinessRule>, BusinessRule>().ToList();
                List<long> brIds = brList.Select(q => q.Id).ToList();
                var actiontypes = (new GeneratorBase.MVC.Models.ActionTypeContext()).ActionTypes.GetFromCache<IQueryable<ActionType>, ActionType>().ToList();
                var actionargs = (new GeneratorBase.MVC.Models.ActionArgsContext()).ActionArgss.GetFromCache<IQueryable<ActionArgs>, ActionArgs>().ToList();
                foreach(var rules in brList)
                {
                    if(((CustomPrincipal)User).IsInRole(rules.Roles.Split(",".ToCharArray()), roles))
                    {
                        rules.ruleaction.ToList().ForEach(p =>
                        {
                            p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID);
                            p.actionarguments = actionargs.Where(q => q.ActionArgumentsID == p.Id).ToList();
                        });
                        rules.ActionTypeID = rules.ruleaction.Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
                        businessrules.Add(rules);
                    }
                }
                //foreach (var rules in brList)
                //{
                //if (((CustomPrincipal)User).IsInRole(rules.Roles.Split(",".ToCharArray()), roles))
                //{
                //rules.ruleaction.ToList().ForEach(p => p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID));
                //rules.ActionTypeID = rules.ruleaction.Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
                //businessrules.Add(rules);
                //}
                //}
            }
            ((CustomPrincipal)User).businessrules = businessrules.ToList();
            using(var UBS = new UserBasedSecurityContext())
            {
                ((CustomPrincipal)User).userbasedsecurity = UBS.UserBasedSecurities.GetFromCache<IQueryable<UserBasedSecurity>, UserBasedSecurity>().ToList();
            }
            var applysecuritypolicy = commonObj.ApplySecurityPolicy().ToLower() == "true" ? true : false;
            if(!isAdmin && applysecuritypolicy && !(User.Identity is System.Security.Principal.WindowsIdentity))
            {
                var enforcechangepassword = commonObj.EnforceChangePassword().ToLower() == "true" ? true : false;
                if(enforcechangepassword)
                {
                    using(var userdb = new ApplicationDbContext(true))
                    {
                        var userinfo = userdb.Users.FirstOrDefault(p => p.UserName == ((CustomPrincipal)User).Name);
                        if(userinfo != null)
                        {
                            var pwdhistorycount = userdb.PasswordHistorys.Where(p => p.UserId == userinfo.Id).Count();
                            if(pwdhistorycount == 0)
                                if(!HttpContext.Current.Request.Url.AbsolutePath.Contains("Account/LogOff"))
                                    HttpContext.Current.RewritePath("~/Account/Manage");
                        }
                    }
                }
                var passwordExpirationInDays = commonObj.PasswordExpirationInDays();
                if(passwordExpirationInDays > 0)
                {
                    using(ApplicationDbContext db = new ApplicationDbContext(true))
                    {
                        var userinfo = db.Users.FirstOrDefault(p => p.UserName == ((CustomPrincipal)User).Name);
                        if(userinfo != null)
                        {
                            var lstLastPasswordChangedDate = db.PasswordHistorys.Where(p => p.UserId == userinfo.Id).OrderBy(p => p.Date).ToList();
                            if(lstLastPasswordChangedDate != null && lstLastPasswordChangedDate.Count() > 0)
                            {
                                var LastPasswordChangedDate = lstLastPasswordChangedDate.LastOrDefault();
                                if(LastPasswordChangedDate.Date.AddDays(passwordExpirationInDays) < DateTime.UtcNow)
                                    HttpContext.Current.RewritePath("~/Account/Manage");
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>Event handler. Called by Application for error events.</summary>
    ///
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">     Event information.</param>
    protected void Application_Error(object sender, EventArgs e)
    {
        //Add code to handle any error here.
        if((Context.Server.GetLastError() is HttpException) && ((Context.Server.GetLastError() as HttpException).GetHttpCode() == 404))
        {
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            Response.StatusCode = 404;
            routeData.Values["action"] = "NotFound404";
            Response.TrySkipIisCustomErrors = true; // If you are using IIS7, have this line
            IController errorsController = new GeneratorBase.MVC.Controllers.ErrorController();
            HttpContextWrapper wrapper = new HttpContextWrapper(Context);
            var rc = new System.Web.Routing.RequestContext(wrapper, routeData);
            errorsController.Execute(rc);
        }
    }
}
/// <summary>A trim model binder, trims value before form submission -  not in use currently (hanlded diffently in app).</summary>
public class TrimModelBinder : IModelBinder
{
    /// <summary>Binds the model to a value by using the specified controller context and binding
    /// context.</summary>
    ///
    /// <param name="controllerContext">The controller context.</param>
    /// <param name="bindingContext">   The binding context.</param>
    ///
    /// <returns>The bound value.</returns>
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if(valueResult == null || valueResult.AttemptedValue == null)
            return null;
        var modelState = new ModelState { Value = valueResult };
        object actualValue = null;
        try
        {
            actualValue = valueResult.AttemptedValue.Trim();
        }
        catch(FormatException e)
        {
            modelState.Errors.Add(e);
        }
        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        return actualValue;
    }
}
/// <summary>A decimal model binder (comma separated).</summary>
public class DecimalModelBinder : IModelBinder
{
    /// <summary>Binds the model to a value by using the specified controller context and binding
    /// context.</summary>
    ///
    /// <param name="controllerContext">The controller context.</param>
    /// <param name="bindingContext">   The binding context.</param>
    ///
    /// <returns>The bound value.</returns>
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if(string.IsNullOrEmpty(valueResult.AttemptedValue))
        {
            return 0m;
        }
        var modelState = new ModelState { Value = valueResult };
        object actualValue = null;
        try
        {
            actualValue = Convert.ToDecimal(
                              valueResult.AttemptedValue.Replace(",", ""),
                              CultureInfo.InvariantCulture
                          );
        }
        catch(FormatException e)
        {
            modelState.Errors.Add(e);
        }
        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        return actualValue;
    }
}
}


