using GeneratorBase.MVC.Models;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>An API base controller v1.</summary>
[NoCache]
public class ApiBaseControllerv1 : ApiController
{
    /// <summary>Size of the page.</summary>
    private const int PageSize = 10;
    /// <summary>The token.</summary>
    private const string Token = "Token";
    /// <summary>The time zone.</summary>
    private const string TimeZone = "TimeZone";
    
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
    
    /// <summary>Gets or sets the timezone.</summary>
    ///
    /// <value>The timezone.</value>
    
    public string timezone
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
    
    [SwaggerIgnore]
    public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
    {
        bool isCorsRequest = controllerContext.Request.Headers.Contains(Origin);
        bool isPreflightRequest = controllerContext.Request.Method == HttpMethod.Options;
        TokenServicesController provider = new TokenServicesController();
        if(controllerContext.Request.Headers.Contains(TimeZone))
        {
            timezone = controllerContext.Request.Headers.GetValues(TimeZone).FirstOrDefault();
        }
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
                            var listEdit = raw.Select(p => p.NoEdit).ToList();
                            var listView = raw.Select(p => p.NoView).ToList();
                            var resultEdit = "";
                            var resultView = "";
                            foreach(var str in listEdit)
                            {
                                if(str != null)
                                    resultEdit += str;
                            }
                            foreach(var str in listView)
                            {
                                if(str != null)
                                    resultView += str;
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
                _apiuser.businessrules = new List<BusinessRule>();//businessrules.ToList()
                 * */
                User = _apiuser;
                db = new ApplicationContext(_apiuser);
                db.Configuration.LazyLoadingEnabled = false;
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
                        resp.Headers.Add(TimeZone, controllerContext.Request.Headers.GetValues(TimeZone).First());
                        return resp;
                    });
                    //
                }
            }
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
            {
                HttpResponseMessage resp = t.Result;
                resp.Headers.Add(Token, controllerContext.Request.Headers.GetValues(Token).First());
                resp.Headers.Add(TimeZone, controllerContext.Request.Headers.GetValues(TimeZone).First());
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
                return resp;
            });
        }
    }
    
    /// <summary>Sets date time to client time.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="timezone">The timezone.</param>
    /// <param name="obj">     The object.</param>
    
    protected void setDateTimeToClientTime<T>(string timezone, object obj)
    {
        try
        {
            System.Reflection.MethodInfo method = typeof(T).GetMethod("setDateTimeToClientTimeAPI");
            object[] MethodParams = new object[] { timezone };
            method.Invoke(obj, MethodParams);
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
    /// <summary>Pages.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="page">The page.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    protected async Task<IHttpActionResult> Page<T>(IQueryable<T> list, int page, int pagesize = 20)
    {
        var count = await list.CountAsync();
        var results = await list.Skip(pagesize * (page - 1))
                      .Take(pagesize)
                      .ToListAsync();
        results.ForEach(p => setDateTimeToClientTime<T>(timezone,p));
        return Ok(new PaginatedList<T>()
        {
            Results = results,
            CurrentPage = page,
            TotalPages = 1 + (count / pagesize),
            TotalResults = count,
            Pagination = new PaginationInformation()
            {
                More = count - (pagesize * page) > 0
            }
        });
    }
    
    /// <summary>Sorts.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="list">     The list.</param>
    /// <param name="sortBy">   Describes who sort this object.</param>
    /// <param name="ascending">True to ascending.</param>
    ///
    /// <returns>The sorted values.</returns>
    
    protected IQueryable<T> Sort<T>(IQueryable<T> list, string sortBy, bool ascending) where T : IEntity
    {
        string methodName = ascending ? "OrderBy" : "OrderByDescending";
        ParameterExpression paramExpression = Expression.Parameter(typeof(T));
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T>)list.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { list.ElementType, lambda.Body.Type },
                       list.Expression,
                       lambda));
    }
    
    /// <summary>Condition expression.</summary>
    ///
    /// <param name="entityname">The entityname.</param>
    /// <param name="property">  The property.</param>
    /// <param name="operators"> The operators.</param>
    /// <param name="value">     The value.</param>
    ///
    /// <returns>A string.</returns>
    
    protected string ConditionExpression(string entityname, string property, string operators, string value)
    {
        string expression = string.Empty;
        var PrpertyType = GetDataTypeOfProperty(entityname, property);
        var dataType = PrpertyType[0];
        property = PrpertyType[1];
        if(value.StartsWith("[") && value.EndsWith("]"))
        {
            var ValueType = GetDataTypeOfProperty(entityname, value, true);
            if(ValueType != null && ValueType[0] == "dynamic")
            {
                dataType = ValueType[0];
                value = ValueType[1];
            }
        }
        if(value.ToLower().Trim() == "null")
        {
            expression = string.Format(" " + property + " " + operators + " {0}", "null");
            return expression;
        }
        switch(dataType)
        {
        case "Int32":
        case "Int64":
        case "Double":
        case "Boolean":
        case "Decimal":
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        case "DateTime":
        {
            if(value.Trim().ToLower() == "today")
            {
                expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).Date);
            }
            else
            {
                DateTime val = Convert.ToDateTime(value);
                expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
            }
            break;
        }
        case "Text":
        case "String":
        {
            if(operators.ToLower() == "contains")
                expression = string.Format(" " + property + "." + operators + "(\"{0}\")", value);
            else
                expression = string.Format(" " + property + " " + operators + " \"{0}\"", value);
            break;
        }
        default:
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        }
        return expression;
    }
    
    /// <summary>Gets data type of property.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="valueType">   (Optional) True to value type.</param>
    ///
    /// <returns>The data type of property.</returns>
    
    protected List<string> GetDataTypeOfProperty(string entityName, string propertyName, bool valueType = false)
    {
        var listString = new List<string>();
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        if(EntityInfo == null) return listString;
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        var associatedprop = string.Empty;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        associatedprop = AssociationInfo.Name.ToLower() + "." + PropInfo.Name;
                    }
                }
            }
        }
        string DataType = string.Empty;
        if(valueType)
            DataType = "dynamic";
        else
            DataType = PropInfo.DataType;
        listString.Add(DataType);
        if(AssociationInfo != null)
            listString.Add(associatedprop);
        else
            listString.Add(propertyName);
        return listString;
    }
    
    /// <summary>Deletes the document described by docID.</summary>
    ///
    /// <param name="docID">Identifier for the document.</param>
    
    protected void DeleteDocument(long? docID)
    {
        var document = db.Documents.Find(docID);
        db.Entry(document).State = EntityState.Deleted;
        db.Documents.Remove(document);
        db.SaveChanges();
    }
    
    /// <summary>Deletes the image gallery document described by docIDs.</summary>
    ///
    /// <param name="docIDs">The document i ds.</param>
    
    protected void DeleteImageGalleryDocument(string docIDs)
    {
        if(!String.IsNullOrEmpty(docIDs))
        {
            string[] strDocIds = docIDs.Split(',');
            foreach(string strDocId in strDocIds)
            {
                var document = db.Documents.Find(Convert.ToInt64(strDocId));
                db.Entry(document).State = EntityState.Deleted;
                db.Documents.Remove(document);
                db.SaveChanges();
            }
        }
    }
    
    
    
}
}
