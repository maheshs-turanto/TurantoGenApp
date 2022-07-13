using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

using GeneratorBase.MVC.Controllers;
using System.Configuration;
using System.Web.Http;
using GeneratorBase.MVC.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.Web;


namespace GeneratorBase.MVC.Controllers
{
/// <summary>A API base controller.</summary>
[NoCache]
public class ApiBaseController : ApiController
{

    /// <summary>The user token (for authentication).</summary>
    private const string Token = "Token";
    
    /// <summary>Gets or sets the database.</summary>
    ///
    /// <value>The database.</value>
    
    public ApplicationContext db
    {
        get;    //removed static for race condition
        private set;
    }
    /// <summary>Context for the user.</summary>
    private ApplicationDbContext UserContext = new ApplicationDbContext();
    
    /// <summary>Gets or sets the user.</summary>
    ///
    /// <value>The user.</value>
    
    public new IUser User
    {
        get;    //removed static for race condition
        private set;
    }
    /// <summary>The origin.</summary>
    private const string Origin = "Origin";
    /// <summary>The access control request method.</summary>
    private const string AccessControlRequestMethod = "Access-Control-Request-Method";
    /// <summary>The access control request headers.</summary>
    private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
    /// <summary>The access control allow origin.</summary>
    private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
    /// <summary>The access control allow methods.</summary>
    private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
    /// <summary>The access control allow headers.</summary>
    private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
    
    /// <summary>Executes the asynchronous operation.</summary>
    ///
    /// <param name="controllerContext">The controller context for a single HTTP operation.</param>
    /// <param name="cancellationToken">    The cancellation token assigned for the HTTP operation.</param>
    ///
    /// <returns>An asynchronous result that yields the execute.</returns>
    
    public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
    {
        //var controllerName = controllerContext.ControllerDescriptor.ControllerName;
        bool isCorsRequest = controllerContext.Request.Headers.Contains(Origin);
        bool isPreflightRequest = controllerContext.Request.Method == HttpMethod.Options;
        TokenServicesController provider = new TokenServicesController();
        if(controllerContext.Request.Headers.Contains(Token))
        {
            var tokenValue = controllerContext.Request.Headers.GetValues(Token).FirstOrDefault();
            if(tokenValue == null || (provider != null && !provider.ValidateToken(tokenValue)))
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Invalid Request"
                };
                controllerContext.Request.CreateResponse(responseMessage);
            }
            else
            {
                ApplicationContext db1 = new ApplicationContext(new SystemUser());
                var _tokenInfo = db1.ApiTokens.FirstOrDefault(p => p.T_AuthToken == tokenValue);
                var _userId = _tokenInfo.T_UsersID;
                ApplicationDbContext userdb = new ApplicationDbContext();
                var _userInfo = userdb.Users.FirstOrDefault(p => p.Id == _userId);
                ApiUser _apiuser = new ApiUser(_userInfo.UserName);
                _apiuser.JavaScriptEncodedName = _userInfo.Email;
                //
                var roles = _apiuser.GetRoles();
                var isAdmin = _apiuser.IsAdminUser();
                _apiuser.IsAdmin = isAdmin;
                _apiuser.userroles = roles.ToList();
                List<Permission> permissions = new List<Permission>();
                using(var pc = new PermissionContext())
                {
                    // so we only make one database call instead of one per entity?
                    var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).ToList();
                    foreach(var entity in GeneratorBase.MVC.ModelReflector.Entities)
                    {
                        var calculated = new Permission();
                        var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                        calculated.EntityName = entity.Name;
                        calculated.CanEdit = isAdmin || raw.Any(p => p.CanEdit);
                        calculated.CanDelete = isAdmin || raw.Any(p => p.CanDelete);
                        calculated.CanAdd = isAdmin || raw.Any(p => p.CanAdd);
                        calculated.CanView = isAdmin || raw.Any(p => p.CanView);
                        calculated.IsOwner = raw.Any(p => p.IsOwner != null && p.IsOwner.Value);
                        if(!isAdmin)
                            calculated.SelfRegistration = raw.Any(p => p.SelfRegistration != null && p.SelfRegistration.Value);
                        else calculated.SelfRegistration = false;
                        if(calculated.IsOwner != null && calculated.IsOwner.Value)
                            calculated.UserAssociation = raw.FirstOrDefault().UserAssociation;
                        else
                            calculated.UserAssociation = string.Empty;
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
                _apiuser.permissions = permissions;
                List<BusinessRule> businessrules = new List<BusinessRule>();
                using(var br = new BusinessRuleContext())
                {
                    br.Configuration.LazyLoadingEnabled = false;
                    br.Configuration.AutoDetectChangesEnabled = false;
                    List<BusinessRule> brList = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID != 5).Include(t => t.ruleconditions).Include(t => t.ruleaction).ToList();
                    List<long> brIds = brList.Select(q => q.Id).ToList();
                    var actiontypes = (new GeneratorBase.MVC.Models.ActionTypeContext()).ActionTypes.ToList();
                    foreach(var rules in brList)
                    {
                        if((_apiuser).IsInRole(rules.Roles.Split(",".ToCharArray()), roles))
                        {
                            rules.ruleaction.ToList().ForEach(p => p.associatedactiontype = actiontypes.FirstOrDefault(q => q.Id == p.AssociatedActionTypeID));
                            rules.ActionTypeID = rules.ruleaction.Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
                            businessrules.Add(rules);
                        }
                    }
                }
                _apiuser.businessrules = businessrules.ToList();
                /*
                using(var br = new BusinessRuleContext())
                {
                    var rolebr = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID != 5).ToList();
                    foreach(var rules in rolebr)
                    {
                        if(_apiuser.IsInRole(rules.Roles.Split(",".ToCharArray())))
                        {
                            businessrules.Add(rules);
                        }
                    }
                }
                _apiuser.businessrules = new List<BusinessRule>();// businessrules.ToList();
                 * */
                using(var UBS = new UserBasedSecurityContext())
                {
                    _apiuser.userbasedsecurity = UBS.UserBasedSecurities.ToList();
                }
                User = _apiuser;
                db = new ApplicationContext(_apiuser);
                if(isCorsRequest)
                {
                    if(isPreflightRequest)
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add(AccessControlAllowOrigin, (controllerContext.Request.Headers.GetValues(Origin).First()));
                        string accessControlRequestMethod = controllerContext.Request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                        if(accessControlRequestMethod != null)
                        {
                            response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                        }
                        string requestedHeaders = string.Join(", ", controllerContext.Request.Headers.GetValues(AccessControlRequestHeaders));
                        if(!string.IsNullOrEmpty(requestedHeaders))
                        {
                            response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                        }
                        var tcs = new TaskCompletionSource<HttpResponseMessage>();
                        tcs.SetResult(response);
                        return tcs.Task;
                    }
                    return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
                    {
                        HttpResponseMessage resp = t.Result;
                        resp.Headers.Add(Token, controllerContext.Request.Headers.GetValues(Token).First());
                        return resp;
                    });
                    //
                }
            }
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
            {
                HttpResponseMessage resp = t.Result;
                resp.Headers.Add(Token, controllerContext.Request.Headers.GetValues(Token).First());
                return resp;
            });
        }
        else
        {
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
            {
                HttpResponseMessage resp = t.Result;
                resp.StatusCode = HttpStatusCode.NotFound;
                resp.ReasonPhrase = "Unauthorized access !";
                //resp.Headers.Add(Token, "UnAuthorized");
                return resp;
            });
        }
    }
}
/// <summary>Attribute for no cache.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class NoCacheAttribute : ActionFilterAttribute
{
    /// <summary>Called by the ASP.NET MVC framework before the action result executes.</summary>
    ///
    /// <param name="filterContext">The filter context.</param>
    
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        base.OnResultExecuting(filterContext);
        if(filterContext.IsChildAction) return;
        var response = filterContext.HttpContext.Response;
        var cache = response.Cache;
        cache.SetCacheability(HttpCacheability.NoCache);
        cache.SetExpires(DateTime.Today.AddDays(-1));
        cache.SetMaxAge(TimeSpan.FromSeconds(0));
        cache.SetNoStore();
        cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        response.AppendHeader("Pragma", "no-cache");
    }
}
}

