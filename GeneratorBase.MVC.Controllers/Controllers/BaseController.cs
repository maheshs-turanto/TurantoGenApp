using GeneratorBase.MVC.DynamicQueryable;
using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using System.Web.Security;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using System.ComponentModel;
using MimeDetective;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>A Base class for controllers.</summary>
[NoCache]
[Audit(true)]
public class BaseController : Controller
{
    /// <summary>Gets or sets the database context.</summary>
    ///
    /// <value>The database.</value>
    
    public ApplicationContext db
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Gets or sets the user.</summary>
    ///
    /// <value>The user.</value>
    
    public new IUser User
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Called when authorization occurs.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnAuthorization(AuthorizationContext filterContext)
    {
        User = base.User as IUser;
        db = new ApplicationContext(base.User as IUser);
    }
    
    /// <summary>Called before the action method is invoked.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if(!Request.IsAuthenticated)
        {
            // filterContext.Result = new RedirectResult("~/Account/Login");
            var values = new RouteValueDictionary(new
            {
                action = "Login",
                controller = "Account",
                returnUrl = HttpContext.Request.Url.PathAndQuery
            });
            var result = new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Bad Request");
            if(Request.IsAjaxRequest())
                filterContext.Result = result;
            else
                filterContext.Result = new RedirectToRouteResult(values);
            return;
        }
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))
        {
            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
            {
                var userid = usercontext.Users.FirstOrDefault(p => p.UserName == User.Name).Id;
                //LoginAttempts history = new LoginAttempts();
                //history.UserId = userid;
                //history.Date = DateTime.UtcNow;
                //history.IsSuccessfull = true;
                //history.IPAddress = Request.UserHostAddress;
                //usercontext.LoginAttempts.Add(history);
                //usercontext.SaveChanges();
                string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                int duration = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "PasswordExpirationInDays").FirstOrDefault().Value);
                if((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                    if(IsPasswordExpired(duration, userid))
                        filterContext.Result = new RedirectResult("~/Account/Manage");
            }
        }
        if(Request.Url.PathAndQuery.ToUpper().Contains("/HOME?ISTHIRDPARTY=TRUE") || (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login") && !Request.Url.PathAndQuery.Contains("/Home?RegistrationEntity")))
        {
            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
            {
                var userid = usercontext.Users.FirstOrDefault(p => p.UserName == User.Name).Id;
                if(IsAutoRegistration(userid))
                {
                    filterContext.Result = Redirect(Url.Action("Index", "Home", new { RegistrationEntity = string.Join(",", User.permissions.Where(p => p.SelfRegistration != null && p.SelfRegistration.Value).Select(p => p.EntityName)), TokenId = userid }));
                }
                if(Request.Url.PathAndQuery.ToUpper().Contains("/BULKUPDATE"))
                {
                    filterContext.Result = Redirect(Url.Action("Index", "Home"));
                }
            }
        }
        //objFavorite = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name && HttpContext.Request.Url.PathAndQuery.EndsWith(p.LinkAddress)).FirstOrDefault();
        string entity = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        if(User.CanView(entity))
        {
            base.OnActionExecuting(filterContext);
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Error");
        }
        base.OnActionExecuting(filterContext);
    }
    
    /// <summary>Called after the action method is invoked.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        //var EntityName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        //if (!(Request.IsAjaxRequest() || EntityName.ToLower() == "document"))
        //{
        //    FavoriteUrlEntityName = EntityName;
        //    FavoriteUrl = getfavoriteUrl();
        //}
        base.OnActionExecuted(filterContext);
    }
    #region Advanced(Deep) lavel Excel,Csv import
    public ActionResult LoaderPage()
    {
        return View();
    }
    public IEnumerable<PropertyInfo> GetCollectionData(string EntityName)
    {
        EntityName = EntityName.Trim();
        IEntity obj = (IEntity)Activator.CreateInstance(Type.GetType("GeneratorBase.MVC.Models." + EntityName + ", GeneratorBase.MVC.Models"), null);
        PropertyInfo[] properties = (obj.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return properties.Where(wh => wh.PropertyType.Name == "ICollection`1" && wh.PropertyType.GenericTypeArguments[0].Name != "FileDocument" && wh.PropertyType.GenericTypeArguments[0].Name != "Nullable`1" && wh.GetCustomAttribute<DisplayNameAttribute>(false) != null);
    }
    public List<ExportData> ExportAllData(string EntityName, int id = 0)
    {
        long exportid = id;
        var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == exportid);
        var allAssociation = GetCollectionData(EntityName);
        var objList = new List<ExportData>();
        foreach(PropertyInfo pi in allAssociation)
        {
            var nameentity = pi.PropertyType.GenericTypeArguments[0].Name;
            var entityinfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == nameentity);
            if(entityinfo == null) continue;
            var objED = new GeneratorBase.MVC.Models.ExportData();
            objED.EntityName = entityinfo.Name;
            objED.DisplayName = pi.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName;
            objED.IsSelf = pi.Name.StartsWith("Self_");
            objED.IsNested = objED.IsSelf ? false : IsNestedCollection(entityinfo.Name);
            objED.AssociationName = EntityName + "?" + entityinfo.Name + "?" + pi.Name + "?" + objED.IsNested;
            objED.AssociationValue = pi.Name;
            objED.FilterQuery = pi.Name;
            objED.IsSelected = exportid == 0 ? false : exportdetails.Any(a => a.T_ParentEntity == EntityName && a.T_AssociationName == pi.Name);
            objList.Add(objED);
        }
        return objList;
    }
    
    [Audit(0)]
    public string getBRTemplate(int typeno)
    {
        using(ActionTypeContext atc = new ActionTypeContext())
        {
            var actiontype = atc.ActionTypes.GetFromCache<IQueryable<ActionType>, ActionType>().ToList().Where(p => p.TypeNo == typeno).FirstOrDefault();
            //var actiontype = atc.ActionTypes.Where(p => p.TypeNo == typeno).FirstOrDefault();
            var template = "";
            if(actiontype != null)
                template = actiontype.Template;
            return template;
        }
    }
    public bool IsNestedCollection(string EntityName)
    {
        if(EntityName == "ApplicationUser") return false;
        return (GetCollectionData(EntityName).Any());
    }
    public JsonResult NestedExportAllData(string EntityName, string FilterQuery, int id = 0)
    {
        long exportid = id;
        var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == exportid);
        var allAssociation = GetCollectionData(EntityName);
        var objList = new List<ExportData>();
        foreach(PropertyInfo pi in allAssociation)
        {
            var nameentity = pi.PropertyType.GenericTypeArguments[0].Name;
            var entityinfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == nameentity);
            if(entityinfo == null) continue;
            var objED = new GeneratorBase.MVC.Models.ExportData();
            objED.EntityName = entityinfo.Name;
            objED.DisplayName = pi.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName;
            objED.IsSelf = pi.Name.StartsWith("Self_");
            objED.IsNested = objED.IsSelf ? false : IsNestedCollection(entityinfo.Name);
            objED.AssociationName = EntityName + "?" + entityinfo.Name + "?" + pi.Name + "?" + objED.IsNested;
            objED.AssociationValue = pi.Name;
            objED.FilterQuery = FilterQuery + "?" + pi.Name;
            objED.IsSelected = (exportid == 0) ? false : exportdetails.Any(a => a.T_ParentEntity == EntityName && a.T_Hierarchy == objED.FilterQuery);
            objList.Add(objED);
        }
        return Json(objList, JsonRequestBehavior.AllowGet);
    }
    public IEntity LoadObjectFromSheet(IEntity model, List<string> sheetColumns, string[] tblColumns, int i, DataSet objDataSet, string AssociatedType, string HostingEntityID, Dictionary<string, string> lstEntityProp, List<string> mappedColumns, string mainEntity, ApplicationContext dbnew, int mapCount = 0)
    {
        var o = model;
        o = UpdateProperty(o, sheetColumns, tblColumns, i, objDataSet, AssociatedType, HostingEntityID, lstEntityProp, mappedColumns, mainEntity, dbnew, mapCount);
        return o;
    }
    public IEntity LoadNavigationProperty(IEntity model, string modelname, string[] navigations, string value, ApplicationContext dbnew, int mapCount = 0)
    {
        if(navigations.Count() > 2)
        {
            if(navigations.Count() == 3)
            {
                //T_State$T_EmployeeAddress$T_AddressState$T_StateCountryID
                string entityname = navigations[0];
                string associationname = navigations[1];
                string propname = navigations[navigations.Length - 1];
                if(!string.IsNullOrEmpty(associationname))
                {
                    IEntity obj;
                    var propertyInfo = (model.GetType()).GetProperties().FirstOrDefault(p => p.Name == associationname.ToLower());
                    Type type = Type.GetType("GeneratorBase.MVC.Models." + entityname + ", GeneratorBase.MVC.Models");
                    var Assocation = ModelReflector.Entities.Single(e => e.Name == entityname).Associations.Where(p => p.Name.ToLower() != "tenantid" && p.AssociationProperty == propname).FirstOrDefault();
                    if(propertyInfo != null && propertyInfo.GetValue(model) == null)
                    {
                        obj = (IEntity)Activator.CreateInstance(type, null);
                        if(Assocation == null)
                            obj = SetModeForAssocationProperty(model, obj, type, associationname.ToLower(), propname, value, entityname, propname, dbnew);
                        if(obj == null)
                            obj = (IEntity)Activator.CreateInstance(type, null);
                        if(!string.IsNullOrEmpty(value) && obj.Id == 0)
                        {
                            var propertyInfochild = (obj.GetType()).GetProperties().FirstOrDefault(p => p.Name == propname);
                            Type propertyType = propertyInfochild.PropertyType;
                            var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                            var valueorg = Convert.ChangeType(value, targetType);
                            propertyInfochild.SetValue(obj, valueorg, null);
                        }
                        if(obj.Id != 0)
                        {
                            model.GetType().GetProperty(associationname + "ID").SetValue(model, obj.Id);
                        }
                        propertyInfo.SetValue(model, obj, null);
                        if(obj.Id != 0)
                            dbnew.Entry(obj).State = EntityState.Modified;
                    }
                    else
                    {
                        obj = (IEntity)propertyInfo.GetValue(model);
                        IEntity NotInlineobj = SetModeForAssocationProperty(model, obj, type, associationname.ToLower(), propname, value, entityname, propname, dbnew);
                        if(NotInlineobj == null)
                            NotInlineobj = obj;
                        if(!string.IsNullOrEmpty(value))
                        {
                            var propertyInfochild = (obj.GetType()).GetProperties().FirstOrDefault(p => p.Name == propname);
                            Type propertyType = propertyInfochild.PropertyType;
                            var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                            var valueorg = Convert.ChangeType(value, targetType);
                            propertyInfochild.SetValue(obj, valueorg, null);
                        }
                        if(IsInlineAssociation(modelname, associationname + "ID"))
                        {
                            propertyInfo.SetValue(model, obj, null);
                        }
                        else if(NotInlineobj.Id != 0)
                        {
                            model.GetType().GetProperty(associationname + "ID").SetValue(model, NotInlineobj.Id);
                        }
                        else
                            propertyInfo.SetValue(model, NotInlineobj, null);
                    }
                    //
                }
            }
            else
            {
                //modify navigations here
                string associationname = navigations[1];
                string entityname = ModelReflector.Entities.FirstOrDefault(p => p.Name == modelname).Associations.FirstOrDefault(p => p.Name == associationname).Target;
                string propname = navigations[2];
                IEntity obj;
                var propertyInfo = (model.GetType()).GetProperties().FirstOrDefault(p => p.Name == associationname.ToLower());
                if(propertyInfo.GetValue(model) == null)
                {
                    obj = (IEntity)Activator.CreateInstance(Type.GetType("GeneratorBase.MVC.Models." + entityname + ", GeneratorBase.MVC.Models"), null);
                }
                else
                {
                    obj = (IEntity)propertyInfo.GetValue((IEntity)model);
                }
                //create new array
                string newentityname = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityname).Associations.FirstOrDefault(p => p.Name == propname).Target;
                string newassociationname = propname;
                string newpropname = navigations.Last();
                List<string> newnavigations = new List<string>();
                newnavigations.Add(newentityname);
                newnavigations.Add(newassociationname);
                foreach(var nav in navigations.Skip(3))
                {
                    newnavigations.Add(nav);
                }
                LoadNavigationProperty(obj, entityname, newnavigations.ToArray(), value, dbnew, mapCount);
            }
        }
        return model;
    }
    public IEntity UpdateProperty(IEntity model, List<string> sheetColumns, string[] tblColumns, int i, DataSet objDataSet, string AssociatedType, string HostingEntityID, Dictionary<string, string> lstEntityProp, List<string> mappedColumns, string mainEntity, ApplicationContext dbnew, int mapCount = 0)
    {
        IEntity ModelAssoc = model;
        for(int j = 0; j < sheetColumns.Count; j++)
        {
            string columntable = tblColumns[j];
            int columnSheet = 0;
            if(string.IsNullOrEmpty(sheetColumns[j]))
                continue;
            else
                columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
            string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
            if(string.IsNullOrEmpty(columnValue))
                continue;
            mappedColumns.Add(columntable);
            var splitedcolunm = columntable.Split('$');
            var propertyName = splitedcolunm[splitedcolunm.Length - 1];
            var entityName = splitedcolunm[0];
            if(splitedcolunm.Length == 1)
                entityName = mainEntity;
            var Assocation = ModelReflector.Entities.Single(e => e.Name == entityName).Associations.Where(p => p.Name.ToLower() != "tenantid" && p.AssociationProperty == propertyName).FirstOrDefault();
            if(Assocation != null)
            {
                string AssocationName = Assocation.Name;
                if(AssociatedType != null && AssociatedType == AssocationName && HostingEntityID != null)
                {
                    long id = Convert.ToInt64(HostingEntityID);
                    model.GetType().GetProperty(columntable).SetValue(model, id);
                }
                else
                {
                    if(Assocation != null)
                    {
                        if(Assocation.Target != "IdentityUser")
                        {
                            Type BaseEntityType = Type.GetType("GeneratorBase.MVC.Models." + entityName + ", GeneratorBase.MVC.Models");
                            IEntity BaseModel = (IEntity)Activator.CreateInstance(BaseEntityType, null);
                            var strProperty = lstEntityProp.FirstOrDefault(p => p.Key == propertyName).Value;
                            ModelReflector.Property propT_Associated = ModelReflector.Entities.FirstOrDefault(p => p.Name == Assocation.Target).Properties.FirstOrDefault(p => p.DisplayName == strProperty);
                            if(propT_Associated != null && propT_Associated.Name != "DisplayValue")
                                columntable = propT_Associated.Name;
                            else
                                columntable = splitedcolunm[splitedcolunm.Length - 1].Replace(propertyName, " DisplayValue");
                            string ChildTableName = Assocation.Target;
                            IEntity ChildObj = SetModeForAssocationProperty(model, BaseModel, BaseEntityType, AssocationName.ToLower(), columntable, columnValue, ChildTableName, propertyName, dbnew);
                            //var ChlidAssocation = ModelReflector.Entities.Single(e => e.Name == entityName).Associations.Where(p => p.Name.ToLower() != "tenantid" && p.AssociationProperty == propertyName).FirstOrDefault();
                            bool IsNextAssoc = false;
                            //if (ChildObj != null && ChlidAssocation != null && splitedcolunm.Length > 2)
                            //{
                            //    string displayValue = Convert.ToString(ChildObj.GetType().GetProperty("DisplayValue").GetValue(ChildObj));
                            //    ModelReflector.Property property = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName).Properties.FirstOrDefault(p => p.Name == propertyName);
                            //    if (property != null && property.IsDisplayProperty)
                            //        ChildObj = SetModeForAssocationProperty(model, BaseModel, BaseEntityType, AssocationName.ToLower(), columntable, displayValue, ChildTableName, " DisplayValue", dbnew);
                            //    else
                            //        ChildObj = null;
                            //    IsNextAssoc = true;
                            //}
                            if(ChildObj == null)
                            {
                                Type AssocationTypeObj = Type.GetType("GeneratorBase.MVC.Models." + ChildTableName + ", GeneratorBase.MVC.Models");
                                IEntity AssocationEntityObj = (IEntity)Activator.CreateInstance(AssocationTypeObj, null);
                                List<ModelReflector.Property> properties = ModelReflector.Entities.FirstOrDefault(p => p.Name == ChildTableName).Properties;
                                IEntity entobj = null;
                                foreach(ModelReflector.Property propertyInfo in properties)
                                {
                                    string AssocationPropName = "";
                                    if(properties.Any(r => r.IsRequired))
                                    {
                                        if(propertyInfo.IsRequired || propertyInfo.IsDisplayProperty)
                                        {
                                            AssocationPropName = propertyInfo.Name;
                                            try
                                            {
                                                if(!string.IsNullOrEmpty(AssocationPropName))
                                                {
                                                    entobj = AssocationChild(AssocationEntityObj, columnValue, propertyInfo, AssocationPropName);
                                                }
                                            }
                                            catch(Exception)
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(propertyInfo.IsDisplayProperty)
                                        {
                                            AssocationPropName = propertyInfo.Name;
                                            try
                                            {
                                                if(!string.IsNullOrEmpty(AssocationPropName))
                                                {
                                                    entobj = AssocationChild(entobj, columnValue, propertyInfo, AssocationPropName);
                                                }
                                            }
                                            catch(Exception)
                                            {
                                                continue;
                                            }
                                            break;
                                        }
                                    }
                                }
                                if(entobj != null)
                                    SaveRecordAssoaction(entobj, AssocationTypeObj, ModelAssoc, propertyName, AssocationName, splitedcolunm, mainEntity, dbnew, IsNextAssoc, mapCount);
                            }
                            else
                            {
                                if(model.GetType().GetProperty(propertyName) != null)
                                    model.GetType().GetProperty(propertyName).SetValue(model, ChildObj.Id);
                                Type AssocationTypeObj = Type.GetType("GeneratorBase.MVC.Models." + ChildTableName + ", GeneratorBase.MVC.Models");
                                IEntity AssocationEntityObj = (IEntity)Activator.CreateInstance(AssocationTypeObj, null);
                                if(ChildObj != null)
                                    SaveRecordAssoaction(ChildObj, AssocationTypeObj, ModelAssoc, propertyName, AssocationName, splitedcolunm, mainEntity, dbnew, IsNextAssoc, mapCount);
                            }
                        }
                        else
                        {
                            using(ApplicationDbContext usercontext = new ApplicationDbContext(true))
                            {
                                var loginID = usercontext.Users.FirstOrDefault(p => p.UserName == columnValue);
                                if(loginID != null)
                                {
                                    columntable = splitedcolunm[1];
                                    model.GetType().GetProperty(columntable).SetValue(model, loginID.Id);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    if(splitedcolunm.Length == 1)
                        columntable = splitedcolunm[0];
                    else
                        columntable = splitedcolunm[1];
                    if(model.GetType().GetProperty(columntable) != null)
                    {
                        Type type = model.GetType();
                        PropertyInfo propertyInfo = type.GetProperty(columntable);
                        Type propertyType = propertyInfo.PropertyType;
                        var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                        var value = Convert.ChangeType(columnValue, targetType);
                        propertyInfo.SetValue(model, value, null);
                    }
                    if(splitedcolunm.Count() > 2)
                    {
                        LoadNavigationProperty(model, mainEntity, splitedcolunm, columnValue, dbnew, mapCount);
                    }
                }
                catch(Exception ex)
                {
                    string exa = ex.Message;
                }
            }
        }
        return model;
    }
    public IEntity SetModeForAssocationProperty(IEntity model, IEntity BaseModel, Type BaseEntityType, string basePropLower, string columntable, string columnValue, string ChildtableName, string propertyName, ApplicationContext dbnew)
    {
        try
        {
            IQueryable<dynamic> selectQuery = null;
            IEnumerable<dynamic> qurey = dbnew.GetType().GetProperty(ChildtableName + "s").GetValue(dbnew, null) as IEnumerable<dynamic>;
            selectQuery = (IQueryable<dynamic>)qurey;
            //var IsUnique = BaseModel.GetType().GetProperty(propertyName).CustomAttributes();
            string whereCondition = columntable + "=\"" + columnValue + "\"";
            var entityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == ChildtableName);
            var propInfo = entityInfo.Properties.FirstOrDefault(p => p.Name == columntable);
            var propDataType = string.Empty;
            if(propInfo != null)
            {
                propDataType = propInfo.DataType;
            }
            if(!string.IsNullOrEmpty(propDataType) && !propDataType.ToLower().Equals("string"))
            {
                long l = 0;
                int no = 0;
                if(long.TryParse(columnValue, out l) && int.TryParse(columnValue, out no))
                    whereCondition = columntable + "=" + columnValue;
            }
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition).AsNoTracking();
            List<object> t_associatedId = (from a in selectQuery select a).ToList() as List<object>;
            IEntity ChildObj = null;
            if(t_associatedId != null && t_associatedId.Count != 0)
            {
                ChildObj = ((IEntity)t_associatedId.FirstOrDefault());
                var Id = ChildObj.Id == 0 ? 0 : ChildObj.Id;
                if(BaseModel.GetType().GetProperty(propertyName) != null)
                {
                    BaseModel.GetType().GetProperty(propertyName).SetValue(BaseModel, Id);
                }
                if(model.GetType().GetProperty(basePropLower) != null && Id == 0)
                    model.GetType().GetProperty(basePropLower).SetValue(model, ChildObj, null);
            }
            return ChildObj;
        }
        catch
        {
            return BaseModel;
        }
    }
    public List<object> AssocationPropertyExist(IQueryable<dynamic> selectQuery, string propname, string Value)
    {
        string whereCondition = propname + "=\"" + Value + "\"";
        selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition).AsNoTracking();
        List<object> t_associated = (from a in selectQuery select a).ToList() as List<object>;
        return t_associated;
    }
    public IEntity AssocationChild(IEntity obj, string columnValue, ModelReflector.Property propertyInfo, string propName)
    {
        try
        {
            object Value = GetConvertStringType(propertyInfo.DataType, columnValue);
            obj.GetType().GetProperty(propName).SetValue(obj, Value);
        }
        catch
        {
            if(propertyInfo.DataType.ToLower() == "datetime")
                obj.GetType().GetProperty(propName).SetValue(obj, DateTime.MinValue);
            else
            {
                var property = obj.GetType().GetProperty(propName);
                Type targetType = property.PropertyType;
                if(property.PropertyType.IsGenericType)
                    targetType = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
                if(targetType.IsValueType)
                    obj.GetType().GetProperty(propName).SetValue(obj, Activator.CreateInstance(targetType));
                else
                    obj.GetType().GetProperty(propName).SetValue(obj, null);
            }
        }
        return obj;
    }
    public IEntity SaveRecordAssoaction(IEntity AssocationEntityObj, Type AssocationTypeObj, IEntity BaseModel, string propertyName, string AssocationName, string[] splitedcolunm, string mainEntity, ApplicationContext dbnew, bool IsNextAssoc, int mapCount = 0)
    {
        if(AssocationEntityObj.Id == 0 && !IsNextAssoc)
            dbnew.Set(AssocationTypeObj).Add(AssocationEntityObj);
        try
        {
            if(AssocationEntityObj.Id == 0 && !IsNextAssoc)
                dbnew.SaveChanges();
        }
        catch
        {
            dbnew.Entry(AssocationEntityObj).State = EntityState.Detached;
        }
        if(AssocationEntityObj.Id > 0)
        {
            if(BaseModel.GetType().GetProperty(propertyName) != null)
                BaseModel.GetType().GetProperty(propertyName).SetValue(BaseModel, AssocationEntityObj.Id);
            if(BaseModel.GetType().GetProperty(AssocationName.ToLower()) != null && AssocationEntityObj.Id == 0)
                BaseModel.GetType().GetProperty(AssocationName.ToLower()).SetValue(BaseModel, AssocationEntityObj);
        }
        if(splitedcolunm.Length == 3)
        {
            if(AssocationEntityObj.Id > 0)
                LoadNavigationProperty(BaseModel, mainEntity, splitedcolunm, Convert.ToString(AssocationEntityObj.Id), dbnew, mapCount);
            else
            {
                var asscoName = splitedcolunm[splitedcolunm.Length - 2];
                LoadNavigationProperty(BaseModel, mainEntity, splitedcolunm, Convert.ToString(AssocationEntityObj.Id), dbnew, mapCount);
                Type nextobjtype = Type.GetType("GeneratorBase.MVC.Models." + splitedcolunm[0] + ", GeneratorBase.MVC.Models");
                IEntity nextobj = BaseModel.GetType().GetProperty(asscoName.ToLower()).GetValue(BaseModel) as IEntity;
                if(nextobj.GetType().GetProperty(propertyName) != null)
                {
                    var AssoId = nextobj.GetType().GetProperty(propertyName).GetValue(nextobj);
                    nextobj.GetType().GetProperty(propertyName).SetValue(nextobj, AssoId);
                }
                if(nextobj.Id == 0)
                    dbnew.Set(nextobjtype).Add(nextobj);
                try
                {
                    if(nextobj.Id == 0)
                        dbnew.SaveChanges();
                }
                catch
                {
                    dbnew.Entry(AssocationEntityObj).State = EntityState.Detached;
                }
                if(BaseModel.GetType().GetProperty(AssocationName + "ID") != null)
                    BaseModel.GetType().GetProperty(AssocationName + "ID").SetValue(BaseModel, nextobj.Id);
                if(BaseModel.GetType().GetProperty(AssocationName.ToLower()) != null && nextobj.Id == 0)
                    BaseModel.GetType().GetProperty(AssocationName.ToLower()).SetValue(BaseModel, nextobj);
            }
        }
        if(splitedcolunm.Length > 3 && AssocationEntityObj.Id > 0)
        {
            if(AssocationEntityObj.Id > 0)
                LoadNavigationProperty(BaseModel, mainEntity, splitedcolunm, Convert.ToString(AssocationEntityObj.Id), dbnew, mapCount);
            else
            {
                IEntity Nextobj = LoadNavigationProperty(BaseModel, mainEntity, splitedcolunm, Convert.ToString(AssocationEntityObj.Id), dbnew, mapCount);
            }
        }
        return AssocationEntityObj;
    }
    public object GetConvertStringType(string dataType, string value)
    {
        string _Return = dataType;
        string _DataTypeLowered = dataType.ToLower();
        object convertedValue = null;
        switch(_DataTypeLowered)
        {
        case "int32":
        {
            convertedValue = Convert.ToInt32(value);
            return convertedValue;
        }
        case "int64":
        {
            convertedValue = Convert.ToInt64(value);
            return convertedValue;
        }
        case "integer":
        {
            convertedValue = Convert.ToInt32(value);
            return convertedValue;
        }
        case "auto":
        {
            convertedValue = Convert.ToInt64(value);
            return convertedValue;
        }
        case "double":
        {
            convertedValue = Convert.ToDecimal(value);
            return convertedValue;
        }
        case "datetime":
        {
            convertedValue = Convert.ToDateTime(value);
            return convertedValue;
        }
        case "boolean":
        {
            convertedValue = Convert.ToBoolean(value);
            return convertedValue;
        }
        case "decimal":
        {
            convertedValue = Convert.ToDecimal(value);
            return convertedValue;
        }
        default:
        {
            convertedValue = Convert.ToString(value);
            return convertedValue;
        }
        }
    }
    private static bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
    public string[] GetInternalColumnOfSheet(string columnlist, string entityname, object MainEntityObj)
    {
        List<string> lstMap = new List<string>();
        var listofmapedcolumn = columnlist.Split(',');
        for(int i = 0; i < listofmapedcolumn.Length; i++)
        {
            lstMap.Add(listofmapedcolumn[i]);
        }
        return lstMap.ToArray();
    }
    [Audit(0)]
    public JsonResult GetEntityDetails(string entNametarget)
    {
        //.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime")
        var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == entNametarget);
        return Json(entList, JsonRequestBehavior.AllowGet);
    }
    private bool TrySetProperty(object obj, string property, object value)
    {
        var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
        if(prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value, null);
            return true;
        }
        return false;
    }
    public List<string> SaveMapedObjectOfSheet(string JsonData, string EntityName, long? RecordId = null)
    {
        var errors = new Dictionary<string, string>();
        List<string> resulterr = new List<string>();
        var addCnt = 0;
        var updateCnt = 0;
        #region For Create New Record
        var type = Type.GetType("GeneratorBase.MVC.Models." + EntityName + ", GeneratorBase.MVC.Models");
        IEntity obj = null;
        var values = new Dictionary<string, object>();
        var change = obj;
        if(!string.IsNullOrEmpty(JsonData))
        {
            obj = (IEntity)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, type);
            values = deserializeToDictionary(JsonData);
        }
        var tenantId = 0;
        change = LoadChangeOfObject("Accept", EntityName, obj, values, errors, tenantId);
        #region Add or Update Record for status Update in ChangeData
        var context = new ValidationContext(change, null, null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(change, context, results, true);
        if(isValid)
        {
            try
            {
                if(change.Id == 0)
                {
                    var _MatchOn = values.FirstOrDefault(p => p.Key == "_MatchOn");
                    var flagUpdate = false;
                    if(_MatchOn.Key != null && _MatchOn.Value != null)
                    {
                        flagUpdate = MatchUpdate.Update(change, Convert.ToString(_MatchOn.Value), db, EntityName, values.Select(p => (string)p.Key).ToList());
                        if(flagUpdate)
                        {
                            updateCnt++;
                        }
                    }
                    if(!flagUpdate)
                        db.Set(type).Add(change);
                }
                else
                    db.Entry(change).State = EntityState.Modified;
                foreach(var entry in db.ChangeTracker.Entries<IEntity>())
                {
                    if(entry.Entity.Id == 0) entry.State = EntityState.Added;
                    else if(entry.Entity.Id > 0) entry.State = EntityState.Modified;
                }
                int result = db.SaveChanges();
                if(result > 0)
                {
                    if(change.Id == 0)
                        addCnt++;
                    else
                        updateCnt++;
                }
                foreach(var entry in db.ChangeTracker.Entries<IEntity>())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch(Exception ex)
            {
                if(ex is System.Data.Entity.Validation.DbEntityValidationException)
                {
                    var error = (System.Data.Entity.Validation.DbEntityValidationException)ex;
                    foreach(System.Data.Entity.Validation.DbEntityValidationResult err in error.EntityValidationErrors)
                    {
                        //errors.Add(error.PropertyName + " : " + error.ErrorMessage, change.Id.ToString());
                        var EntityDisplayNameReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == err.Entry.Entity.GetType().BaseType.Name);
                        var EntityspecificDisplayName = EntityDisplayNameReflector != null ? EntityDisplayNameReflector.DisplayName : err.Entry.Entity.GetType().BaseType.Name;
                        resulterr.Add("Entity of type " + EntityspecificDisplayName + " in state " + err.Entry.State + " has the following validation errors");
                        foreach(var ve in err.ValidationErrors)
                        {
                            resulterr.Add(ve.PropertyName + " : " + ve.ErrorMessage);
                        }
                    }
                }
                else
                    resulterr.Add(ex.Message);
                //errors.Add(ex.Message, (change.Id).ToString());
                //errors.Add("Invalid JSON " + "(" + ex.Message + ")", (change.Id).ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                try
                {
                    db.Entry(change).State = EntityState.Detached;
                    //db.Entry(cd).State = EntityState.Detached;
                }
                catch
                {
                }
            }
        }
        else
        {
            int i = 0;
            foreach(var result in results)
            {
                if(result.MemberNames.FirstOrDefault() != null && !string.IsNullOrEmpty(result.MemberNames.FirstOrDefault().ToString()))
                {
                    var membername = result.MemberNames.FirstOrDefault().ToString();
                    var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Properties.FirstOrDefault(p => p.Name == membername).DisplayName;
                    if(!string.IsNullOrEmpty(result.ErrorMessage))
                        results[i].ErrorMessage = result.ErrorMessage;
                    else
                        results[i].ErrorMessage = "The " + dispName + " field is required.";
                }
                i++;
            }
            resulterr = results.Select(s => s.ErrorMessage).ToList();
        }
        #endregion
        //
        #endregion
        #region For Existing Record
        if(RecordId != null && RecordId > 0)
        {
            #region For Update Record
            try
            {
                IEntity obj1 = null;
                if(!string.IsNullOrEmpty(JsonData))
                {
                    if(obj == null)
                        obj1 = obj = (IEntity)JsonConvert.DeserializeObject(JsonData, type);
                    else
                        obj1 = (IEntity)JsonConvert.DeserializeObject(JsonData, type);
                    var values1 = deserializeToDictionary(JsonData);
                    foreach(KeyValuePair<string, object> d in values1)
                    {
                        if(values.ContainsKey(d.Key)) continue;
                        values.Add(d.Key, d.Value);
                    }
                    if(obj != null && obj1 != null && values1 != null && values1.Count() > 0)
                        MatchUpdate.CopyValues(obj1, obj, new List<string>(), values1.Select(p => p.Key).ToList());
                }
            }
            catch(Exception ex)
            {
                resulterr.Add("Invalid Object " + "(" + ex.Message + ")");
            }
            #endregion
        }
        else
        {
            #region For Create New Record
            if(!string.IsNullOrEmpty(JsonData))
            {
                obj = (IEntity)JsonConvert.DeserializeObject(JsonData, type);
                values = deserializeToDictionary(JsonData);
            }
            #endregion
        }
        #endregion
        if(errors.Count > 0)
            return resulterr;
        return resulterr;
    }
    public Dictionary<string, object> deserializeToDictionary(string jsondata)
    {
        var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsondata);
        var values2 = new Dictionary<string, object>();
        foreach(KeyValuePair<string, object> d in values)
        {
            if(d.Value != null && d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
            {
                values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
            }
            else
            {
                values2.Add(d.Key, d.Value);
            }
        }
        return values2;
    }
    public IEntity LoadChangeOfObject(string callFor, string entity, IEntity obj, Dictionary<string, object> values, Dictionary<string, string> errors, long? tenantId)
    {
        var o = obj;
        if(obj.Id > 0) o = UpdatePropertiesOfObject(obj, values);
        UpdateAssociationsOfObject(callFor, entity, o, values, errors, tenantId);
        ////mahesh
        //if (obj.Id > 0)
        //dbNew.Entry(o).State = EntityState.Modified;
        //else
        //dbNew.Entry(o).State = EntityState.Added;
        return o;
    }
    private IEntity UpdatePropertiesOfObject(IEntity obj, Dictionary<string, object> values)
    {
        var original = GetRecordById(obj.GetType().Name, obj.Id);
        foreach(var p in obj.GetType().GetProperties())
        {
            Type Datatype = Nullable.GetUnderlyingType(p.PropertyType);
            if(Datatype == null)
                Datatype = p.PropertyType;
            var val = p.GetValue(obj);
            if(val != null && Datatype.Equals(typeof(DateTime)) && ((DateTime)val).Equals(new DateTime(1, 1, 1)))
            {
                val = null;
            }
            if(val != null && Datatype.Equals(typeof(bool)) && (val).Equals(false))
            {
                val = null;
            }
            if(val != null && Datatype.Equals(typeof(decimal)) && ((decimal)val).Equals(0))
            {
                val = null;
            }
            if(val != null && Datatype.Equals(typeof(int)) && ((int)val).Equals(0))
            {
                val = null;
            }
            if(val != null && Datatype.Equals(typeof(long)) && ((long)val).Equals(0))
            {
                val = null;
            }
            if(val != null && (Datatype.Equals(typeof(float)) || Datatype.Equals(typeof(double))) && ((double)val).Equals(0))
            {
                val = null;
            }
            if(val != null ||
                    (values.ContainsKey(p.Name) && (!p.PropertyType.IsValueType || (Nullable.GetUnderlyingType(p.PropertyType) != null))))
                p.SetValue(original, val);
        }
        return original;
    }
    private void UpdateAssociationsOfObject(string callFor, string entity, IEntity obj, Dictionary<string, object> values, Dictionary<string, string> errors, long? tenantId)
    {
        var type = Type.GetType("GeneratorBase.MVC.Models." + entity + ", GeneratorBase.MVC.Models");
        var associations = ModelReflector.Entities.Single(e => e.Name == entity).Associations.Where(p => p.Name.ToLower() != "tenantid" && p.Target != "IdentityUser");
        foreach(var a in associations)
        {
            var property = type.GetProperty(a.Name.ToLower());
            var target = property.GetValue(obj) as IEntity;
            if(target == null)
            {
                var isExplicitlySet = values.ContainsKey(a.Name.ToLower());
                //long? associationId = (long?)type.GetProperty(a.AssociationProperty).GetValue(obj);
                long associationId = Convert.ToInt64(type.GetProperty(a.AssociationProperty).GetValue(obj));
                if(associationId > 0)
                {
                    var original = GetRecordById(a.Target, associationId);
                    if(original == null && Convert.ToInt64(obj.Id) == 0)       //comment for update time not sowing below msg
                    {
                        var EntityDisplayNameReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == a.Target);
                        var EntityspecificDisplayName = EntityDisplayNameReflector != null ? EntityDisplayNameReflector.DisplayName : a.Target;
                        errors.Add(EntityspecificDisplayName, a.AssociationProperty + " : " + Convert.ToString(associationId) + " does not exist");
                    }
                }
                //var idHasValue = ((long?)type.GetProperty(a.AssociationProperty).GetValue(obj)).HasValue;
                var idHasValue = false;
                if(associationId > 0) idHasValue = true;
                if(IsInlineAssociation(entity, a.AssociationProperty) && !idHasValue && !isExplicitlySet)
                {
                    var iType = Type.GetType("GeneratorBase.MVC.Models." + a.Target + ", GeneratorBase.MVC.Models");
                    var i = Activator.CreateInstance(iType, null);
                    //property.SetValue(obj, i);
                    property.SetValue(obj, null);
                }
                if(isExplicitlySet && !idHasValue)
                {
                    type.GetProperty(a.AssociationProperty).SetValue(obj, null);
                }
                if(callFor.ToLower() == "preview" && associationId > 0)
                {
                    var original = GetRecordById(a.Target, associationId);
                    type.GetProperty(a.Name.ToLower()).SetValue(obj, original);
                }
                continue;
            }
            if(target.Id == 0)
            {
                var dv = target.DisplayValue;
                var id = GetIdFromDisplayValueOfObject(a.Target, dv, tenantId);
                if(id == 0)
                {
                    target.Id = 0;
                }
                if(id > 0)
                {
                    type.GetProperty(a.AssociationProperty).SetValue(obj, id);
                    if(callFor.ToLower() == "preview")
                    {
                        var original = GetRecordById(a.Target, id.Value);
                        type.GetProperty(a.Name.ToLower()).SetValue(obj, original);
                    }
                    else
                        property.SetValue(obj, null);//original
                    continue;
                }
            }
            // explicitly create a new entity, do not search
            /// it doesn't make sense to have both the Target and AssociationProperty
            /// either the SSIS package made a mistake, or we are trying to overwrite
            /// an existing AssociationProperty
            type.GetProperty(a.AssociationProperty).SetValue(obj, target.Id);
            var subValues =
                (values.ContainsKey(property.Name) && values[property.Name] is Dictionary<string, object>) ?
                values[property.Name] as Dictionary<string, object> :
                new Dictionary<string, object>();
            property.SetValue(obj, LoadChangeOfObject(callFor, a.Target, target, subValues, errors, tenantId));
        }
    }
    private long? GetIdFromDisplayValueOfObject(string target, string dv, long? tenantId)
    {
        var type = Type.GetType("GeneratorBase.MVC.Controllers." + target + "Controller");
        var instance = GetControllerInstance(type);
        var method = type.GetMethod("GetIdFromDisplayValue");
        var p = new object[] { dv };
        if(tenantId != null && method.GetParameters().FirstOrDefault(q => q.Name == "tenantId") != null)
            p = new object[] { dv, tenantId };
        return (long?)method.Invoke(instance, p);
    }
    public string[] GetAllColumnForAdvanceImport(string mainEnt, string mappingName, List<ModelReflector.Property> prorlist, List<SelectListItem> col, Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap, Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList> objColMapAssocProperties)
    {
        var colprop = new List<string>();
        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == mainEnt && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.TableColumn))).ToList();
        foreach(var tableCol in DefaultMapping)
        {
            string[] colstring = tableCol.TableColumn.Split('$');
            if(colstring.Length > 2)
            {
                string entName = colstring[0];
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == entName);
                if(entList == null)
                    continue;
                var entityprop = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime");
                string propName = colstring[colstring.Length - 1];
                var propInternalName = entList.Properties.Where(p => p.Name == propName).FirstOrDefault().Name;
                colprop.Add(entName + "$" + propInternalName);
                if(entityprop.Select(s => s.Name).Contains(propInternalName))
                {
                    ModelReflector.Property prop = entityprop.FirstOrDefault(fd => fd.Name == propInternalName);
                    SetColumnMapingForProperty(col, entList, objColMap, prop, propName);
                    prorlist.Add(entityprop.FirstOrDefault(fd => fd.Name == propInternalName));
                }
                SetColumnMapingForAssociations(entList, objColMapAssocProperties);
            }
            else
            {
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == mainEnt);
                if(entList == null)
                    continue;
                string propdispName = "";
                if(colstring.Length == 1)
                    propdispName = colstring[0];
                if(colstring.Length == 2)
                    propdispName = colstring[1];
                var entityprop = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime");
                var propInternal = entList.Properties.Where(p => p.Name == propdispName && p.Name != "TenantId");
                var propInternalName = "";
                if(propInternal.Count() == 0)
                    propInternal = entList.Properties.Where(p => p.DisplayName == propdispName && p.Name != "TenantId");
                if(propInternal.Count() == 0)
                    continue;
                propInternalName = propInternal.FirstOrDefault().Name;
                if(string.IsNullOrEmpty(propInternalName))
                    continue;
                colprop.Add(propInternalName);
                if(entityprop.Select(s => s.Name).Contains(propInternalName))
                {
                    ModelReflector.Property prop = entityprop.FirstOrDefault(fd => fd.Name == propInternalName);
                    SetColumnMapingForProperty(col, entList, objColMap, prop, propdispName);
                    prorlist.Add(entityprop.FirstOrDefault(fd => fd.Name == propInternalName));
                }
                SetColumnMapingForAssociations(entList, objColMapAssocProperties);
            }
        }
        return colprop.ToArray();
    }
    
    public void SetColumnMapingForProperty(List<SelectListItem> col, ModelReflector.Entity entList, Dictionary<ModelReflector.Property, SelectList> objColMap, ModelReflector.Property prop, string propdispName)
    {
        long selectedVal = 0;
        bool isReq = false;
        var colSelected = col.FirstOrDefault(p => p.Text.Trim().ToLower() == propdispName.ToLower());
        var propAssoc = entList.Associations.FirstOrDefault(ae => ae.AssociationProperty == prop.Name);
        if(propAssoc != null)
        {
            isReq = propAssoc.IsRequired;
            if(isReq && colSelected != null)
                selectedVal = long.Parse(colSelected.Value);
        }
        else
        {
            if(colSelected != null)
                selectedVal = long.Parse(colSelected.Value);
        }
        //if (!objColMap.ContainsKey(prop))
        objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
    }
    public void SetColumnMapingForAssociations(ModelReflector.Entity entList, Dictionary<ModelReflector.Association, SelectList> objColMapAssocProperties)
    {
        List<GeneratorBase.MVC.ModelReflector.Association> assocList = entList.Associations;
        if(assocList != null)
        {
            foreach(var assoc in assocList)
            {
                if(assoc.Target == "IdentityUser")
                    continue;
                Dictionary<string, string> lstProperty = new Dictionary<string, string>();
                var assocEntity = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == assoc.Target);
                var assocProperties = assocEntity.Properties.Where(p => p.Name != "DisplayValue");
                lstProperty.Add("DisplayValue", "DisplayValue-" + assoc.AssociationProperty);
                foreach(var prop in assocProperties)
                {
                    if(!lstProperty.ContainsKey(prop.DisplayName))
                        lstProperty.Add(prop.DisplayName, prop.DisplayName + "-" + assoc.AssociationProperty);
                }
                //var dispValue = lstProperty.Keys.FirstOrDefault();
                if(!objColMapAssocProperties.Keys.Any(a => a.AssociationProperty == assoc.AssociationProperty))
                    objColMapAssocProperties.Add(assoc, new SelectList(lstProperty.AsEnumerable(), "Value", "Key", "Key"));
            }
        }
    }
    #endregion
    
    protected string SaveAnyCameraFile(HttpRequestBase formRequest, string fieldname, string filetype, long? Id = null, string EntityName = null)
    {
        string result = string.Empty;
        string path = Server.MapPath("~/Files/");
        string ticks = DateTime.UtcNow.Ticks.ToString();
        if(filetype.ToLower() == "file")
        {
            using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(formRequest.Form[fieldname]))))
            {
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                string fileName = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                string fileext = ".jpeg";
                byte[] bytes = Convert.FromBase64String(formRequest.Form[fieldname]);
                long _contentLength = bytes.Length;
                int Imglen = bytes.Length;
                var document = db.Documents.Find(Id);
                if(document == null)
                    document = new Document();
                long fileSize = 0;
                byte[] fileData = null;
                using(var binaryReader = new BinaryReader(new MemoryStream(bytes)))
                {
                    binaryReader.BaseStream.Position = 0;
                    fileData = binaryReader.ReadBytes(Imglen);
                }
                document.DocumentName = fileName;
                document.DateCreated = System.DateTime.UtcNow.Date;
                document.DateLastUpdated = System.DateTime.UtcNow.Date;
                document.FileExtension = fileext;
                document.DisplayValue = fileName;
                document.FileName = fileName;
                document.FileSize = fileSize;
                document.MIMEType = "image/png";
                document.Byte = fileData;
                document.FileType = filetype;
                document.EntityName = EntityName;
                if(document != null && Id > 0)
                    db.Entry(document).State = EntityState.Modified;
                else
                    db.Documents.Add(document);
                db.SaveChanges();
                result = document.Id.ToString();
            }
        }
        else if(filetype.ToLower() == "onedrive")
        {
            using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(formRequest.Form[fieldname]))))
            {
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                string fileext = ".jpeg";
                string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                string oneDrivePath = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
                string AccessToken = string.Empty;
                if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                    AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
                if(AccessToken != null)
                {
                    byte[] bytes = Convert.FromBase64String(formRequest.Form[fieldname]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long fileSize = 0;
                    byte[] fileData = null;
                    using(var binaryReader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        binaryReader.BaseStream.Position = 0;
                        fileData = binaryReader.ReadBytes(Imglen);
                    }
                    var fileresult = new FileContentResult(bytes, MimeMapping.GetMimeMapping(oneDrivePath));
                    string linkresult = OfficeAccessSession.GetFilesList(fileresult, oneDrivePath).GetAwaiter().GetResult();
                    if(!string.IsNullOrEmpty(linkresult))
                    {
                        result = linkresult;
                        var document = db.Documents.Find(Id);
                        if(document == null)
                            document = new Document();
                        document.DocumentName = fileName;
                        document.DateCreated = System.DateTime.UtcNow.Date;
                        document.DateLastUpdated = System.DateTime.UtcNow.Date;
                        document.FileExtension = fileext;
                        document.DisplayValue = fileName;
                        document.FileName = fileName;
                        document.FileSize = fileSize;
                        document.MIMEType = "image/png";
                        document.Byte = fileData;
                        document.FileType = filetype;
                        document.EntityName = EntityName;
                        if(document != null && Id > 0)
                            db.Entry(document).State = EntityState.Modified;
                        else
                            db.Documents.Add(document);
                        db.SaveChanges();
                        result = document.Id.ToString();
                    }
                }
            }
        }
        else if(filetype.ToLower() == "byte")
        {
            using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(formRequest.Form[fieldname]))))
            {
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                string fileext = ".jpeg";
                string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                byte[] bytes = Convert.FromBase64String(formRequest.Form[fieldname]);
                long _contentLength = bytes.Length;
                int Imglen = bytes.Length;
                var document = db.Documents.Find(Id);
                if(document == null)
                    document = new Document();
                long fileSize = 0;
                byte[] fileData = null;
                using(var binaryReader = new BinaryReader(new MemoryStream(bytes)))
                {
                    binaryReader.BaseStream.Position = 0;
                    fileData = binaryReader.ReadBytes(Imglen);
                }
                document.DocumentName = fileName;
                document.DateCreated = System.DateTime.UtcNow.Date;
                document.DateLastUpdated = System.DateTime.UtcNow.Date;
                document.FileExtension = fileext;
                document.DisplayValue = fileName;
                document.FileName = fileName;
                document.FileSize = fileSize;
                document.MIMEType = "image/png";
                document.Byte = fileData;
                document.FileType = filetype;
                document.EntityName = EntityName;
                if(document != null && Id > 0)
                    db.Entry(document).State = EntityState.Modified;
                else
                    db.Documents.Add(document);
                db.SaveChanges();
                result = document.Id.ToString();
            }
        }
        return result;
    }
    protected string SaveAnyDocument(HttpPostedFileBase file, string filetype, long? Id = null, string EntityName = null)
    {
        string result = string.Empty;
        string path = Server.MapPath("~/Files/");
        string ticks = DateTime.UtcNow.Ticks.ToString();
        if(filetype.ToLower() == "file")
        {
            file.SaveAs(path + ticks + System.IO.Path.GetFileName(file.FileName.Trim().Replace(" ", "")));
            string filename = ticks + System.IO.Path.GetFileName(file.FileName.Trim().Replace(" ", ""));
            var document = db.Documents.Find(Id);
            if(document == null)
                document = new Document();
            string fileExt = "";
            long fileSize = 0;
            fileExt = System.IO.Path.GetExtension(file.FileName);
            fileSize = file.ContentLength;
            byte[] fileData = null;
            using(var binaryReader = new BinaryReader(file.InputStream))
            {
                binaryReader.BaseStream.Position = 0;
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            document.DocumentName = file.FileName;
            document.DateCreated = System.DateTime.UtcNow.Date;
            document.DateLastUpdated = System.DateTime.UtcNow.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = file.ContentType;
            document.FileType = filetype;
            document.Byte = fileData;
            document.EntityName = EntityName;
            if(document != null && Id > 0)
                db.Entry(document).State = EntityState.Modified;
            else
                db.Documents.Add(document);
            db.SaveChanges();
            result = document.Id.ToString();
        }
        else if(filetype.ToLower() == "onedrive")
        {
            file.SaveAs(path + ticks + System.IO.Path.GetFileName(file.FileName.Trim().Replace(" ", "")));
            string fileExt = System.IO.Path.GetExtension(file.FileName);
            string oneDrivePath = file.FileName.Replace(fileExt, "").Trim() + "_" + ticks + fileExt;
            var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
            string AccessToken = string.Empty;
            if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
            if(AccessToken != null)
            {
                byte[] fileData = null;
                using(var binaryReader = new BinaryReader(file.InputStream))
                {
                    binaryReader.BaseStream.Position = 0;
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                var fileresult = new FileContentResult(fileData, MimeMapping.GetMimeMapping(file.FileName));
                string linkresult = OfficeAccessSession.GetFilesList(fileresult, oneDrivePath).GetAwaiter().GetResult();
                if(!string.IsNullOrEmpty(linkresult))
                {
                    var document = db.Documents.Find(Id);
                    if(document == null)
                        document = new Document();
                    long fileSize = 0;
                    fileExt = System.IO.Path.GetExtension(file.FileName);
                    fileSize = file.ContentLength;
                    document.DocumentName = file.FileName;
                    document.DateCreated = System.DateTime.UtcNow.Date;
                    document.DateLastUpdated = System.DateTime.UtcNow.Date;
                    document.FileExtension = fileExt;
                    document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
                    document.FileName = linkresult;
                    document.FileSize = fileSize;
                    document.MIMEType = file.ContentType;
                    document.FileType = filetype;
                    document.Byte = fileData;
                    document.EntityName = EntityName;
                    if(document != null && Id > 0)
                        db.Entry(document).State = EntityState.Modified;
                    else
                        db.Documents.Add(document);
                    db.SaveChanges();
                    result = document.Id.ToString();
                }
            }
        }
        else if(filetype.ToLower() == "byte")
        {
            var document = db.Documents.Find(Id);
            if(document == null)
                document = new Document();
            string fileExt = "";
            long fileSize = 0;
            fileExt = System.IO.Path.GetExtension(file.FileName);
            fileSize = file.ContentLength;
            byte[] fileData = null;
            using(var binaryReader = new BinaryReader(file.InputStream))
            {
                binaryReader.BaseStream.Position = 0;
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            document.DocumentName = file.FileName;
            document.DateCreated = System.DateTime.UtcNow.Date;
            document.DateLastUpdated = System.DateTime.UtcNow.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
            document.FileName = System.IO.Path.GetFileName(file.FileName);
            document.FileSize = fileSize;
            document.MIMEType = file.ContentType;
            document.FileType = filetype;
            document.Byte = fileData;
            document.EntityName = EntityName;
            if(document != null && Id > 0)
                db.Entry(document).State = EntityState.Modified;
            else
                db.Documents.Add(document);
            db.SaveChanges();
            result = document.Id.ToString();
        }
        return result;
    }
    
    /// <summary>Saves a document.</summary>
    ///
    /// <param name="file">The file.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long SaveDocument(HttpPostedFileBase file)
    {
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = file.FileName;
        filename = System.IO.Path.GetFileName(file.FileName);
        fileExt = System.IO.Path.GetExtension(file.FileName);
        fileSize = file.ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(file.InputStream))
        {
            binaryReader.BaseStream.Position = 0;
            fileData = binaryReader.ReadBytes(file.ContentLength);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
        document.FileName = System.IO.Path.GetFileName(file.FileName);
        document.FileSize = fileSize;
        document.MIMEType = file.ContentType;
        document.Byte = fileData;
        document.FileType = "Byte";
        db.Documents.Add(document);
        db.SaveChanges();
        return document.Id;
    }
    
    /// <summary>Updates the document.</summary>
    ///
    /// <param name="file"> The file.</param>
    /// <param name="docId">Identifier for the document.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long UpdateDocument(HttpPostedFileBase file, long? docId)
    {
        var document = db.Documents.Find(docId);
        if(document == null)
            return SaveDocument(file);
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        //Document document = new Document();
        document.DocumentName = file.FileName;
        filename = System.IO.Path.GetFileName(file.FileName);
        fileExt = System.IO.Path.GetExtension(file.FileName);
        fileSize = file.ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(file.InputStream))
        {
            binaryReader.BaseStream.Position = 0;
            fileData = binaryReader.ReadBytes(file.ContentLength);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = file.ContentType;
        document.Byte = fileData;
        document.FileType = "Byte";
        //db.Documents.Add(document);
        db.Entry(document).State = EntityState.Modified;
        db.SaveChanges();
        return document.Id;
    }
    
    /// <summary>add/upadte for camera.</summary>
    ///
    /// <param name="ext">          The extent.</param>
    /// <param name="camfilename">  The camfilename.</param>
    /// <param name="filebyte">     The filebyte.</param>
    /// <param name="ContentLength">Length of the content.</param>
    /// <param name="len">          The length.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long SaveBarCode(string ext, string camfilename, byte[] filebyte, long ContentLength, int len, ApplicationContext db)
    {
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = filename;
        filename = camfilename;
        fileExt = ext;
        fileSize = ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
        {
            binaryReader.BaseStream.Position = 0;
            fileData = binaryReader.ReadBytes(len);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = "image/png";
        document.Byte = fileData;
        document.FileType = "Byte";
        db.Documents.Add(document);
        db.SaveChanges();
        return document.Id;
    }
    
    
    /// <summary>add/upadte for camera.</summary>
    ///
    /// <param name="ext">          The extent.</param>
    /// <param name="camfilename">  The camfilename.</param>
    /// <param name="filebyte">     The filebyte.</param>
    /// <param name="ContentLength">Length of the content.</param>
    /// <param name="len">          The length.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long SaveDocumentCameraImage(string ext, string camfilename, byte[] filebyte, long ContentLength, int len)
    {
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = filename;
        filename = camfilename;
        fileExt = ext;
        fileSize = ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
        {
            binaryReader.BaseStream.Position = 0;
            fileData = binaryReader.ReadBytes(len);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = "image/png";
        document.Byte = fileData;
        document.FileType = "Byte";
        db.Documents.Add(document);
        db.SaveChanges();
        return document.Id;
    }
    
    /// <summary>Updates the document camera.</summary>
    ///
    /// <param name="ext">          The extent.</param>
    /// <param name="camfilename">  The camfilename.</param>
    /// <param name="filebyte">     The filebyte.</param>
    /// <param name="ContentLength">Length of the content.</param>
    /// <param name="len">          The length.</param>
    /// <param name="docId">        Identifier for the document.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long UpdateDocumentCamera(string ext, string camfilename, byte[] filebyte, long ContentLength, int len, long? docId)
    {
        var document = db.Documents.Find(docId);
        if(document == null)
            return SaveDocumentCameraImage(ext, camfilename, filebyte, ContentLength, len);
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        document.DocumentName = camfilename;
        filename = camfilename;
        fileExt = ext;
        fileSize = ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
        {
            binaryReader.BaseStream.Position = 0;
            fileData = binaryReader.ReadBytes(len);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = "image/png";
        document.Byte = fileData;
        document.FileType = "Byte";
        db.Entry(document).State = EntityState.Modified;
        db.SaveChanges();
        return document.Id;
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
    
    /// <summary>Query if 'file' is file type and size allowed.</summary>
    ///
    /// <param name="file">The file.</param>
    ///
    /// <returns>True if file type and size allowed, false if not.</returns>
    
    protected bool IsFileTypeAndSizeAllowed(HttpPostedFileBase file)
    {
        var fileType = db.AppSettings.Where(p => p.Key == "FileTypes").FirstOrDefault();
        var fileSize = db.AppSettings.Where(p => p.Key == "FileSize").FirstOrDefault();
        var RegExObj = db.AppSettings.Where(p => p.Key == "RegExValidationForFileName" && p.Value != "<No Regular Expression>").FirstOrDefault();
        var fileExt = System.IO.Path.GetExtension(file.FileName).Replace(".", "").Trim();
        bool isFileValid = true;
        string alertMsg = "Validation Alert - ";
        if(fileType != null)
        {
            var allowed = fileType.Value.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            if(!(fileType.Value.ToLower().Contains(fileExt.ToLower()))) // || allowed.Intersect(existing).Count() == 0)
            {
                alertMsg = alertMsg + "This file type is not supported. Application accepts only [" + fileType.Value + "] file types)";
                isFileValid = false;
            }
            else
            {
                MemoryStream fs = new MemoryStream();
                file.InputStream.CopyTo(fs);
                var fileData = fs.ToArray();
                FileType mimeType2 = fileData.GetFileType();
                if(mimeType2 != null)
                {
                    var existing = mimeType2.Extension.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                    if(allowed.Intersect(existing).Count() == 0)
                    {
                        alertMsg = alertMsg + "This file type is not supported. Application accepts only [" + fileType.Value + "] file types)";
                        isFileValid = false;
                    }
                }
            }
        }
        if(RegExObj != null)
            if(!DocumentHelper.IsAlphaNumeric(System.IO.Path.GetFileNameWithoutExtension(file.FileName), RegExObj.Value))
            {
                alertMsg = alertMsg + "File Name is not proper.";
                isFileValid = false;
            }
        if(fileSize != null)
            if(!(Convert.ToInt32(fileSize.Value) * 1024 * 1024 >= file.ContentLength))
            {
                alertMsg = alertMsg + " File size is large. Application accepts file of size less than [ " + fileSize.Value + " MB ])";
                isFileValid = false;
            }
        if(!isFileValid)
        {
            ModelState.AddModelError("CustomError", alertMsg);
            ViewBag.ApplicationError = alertMsg;
        }
        return isFileValid;
    }
    
    /// <summary>Query if 'duration' is password expired.</summary>
    ///
    /// <param name="duration">The duration.</param>
    /// <param name="userId">  Identifier for the user.</param>
    ///
    /// <returns>True if password expired, false if not.</returns>
    
    private bool IsPasswordExpired(int duration, string userId)
    {
        using(ApplicationDbContext db = new ApplicationDbContext(true))
        {
            var lstLastPasswordChangedDate = db.PasswordHistorys.Where(p => p.UserId == userId).OrderBy(p => p.Date).ToList();
            if(lstLastPasswordChangedDate != null && lstLastPasswordChangedDate.Count() > 0)
            {
                var LastPasswordChangedDate = lstLastPasswordChangedDate.LastOrDefault();
                if(LastPasswordChangedDate.Date.AddDays(duration) < DateTime.UtcNow)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
    
    /// <summary>Query if 'userid' is automatic registration.</summary>
    ///
    /// <param name="userid">The userid.</param>
    ///
    /// <returns>True if automatic registration, false if not.</returns>
    
    private bool IsAutoRegistration(string userid)
    {
        var result = false;
        var Permission = User.permissions.Where(p => p.SelfRegistration != null && p.SelfRegistration.Value);
        foreach(var ent in Permission)
        {
            result = true;
            var EntityName = ent.EntityName;
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("IsAlreadyRegistred");
            object[] MethodParams = new object[] { userid, db };
            var result1 = mc.Invoke(objController, MethodParams);
            if(Convert.ToBoolean(result1))
                return false;
        }
        return result;
    }
    
    /// <summary>Gets base URL.</summary>
    ///
    /// <returns>The base URL.</returns>
    
    public static string GetBaseUrl()
    {
        var request = System.Web.HttpContext.Current.Request;
        var appUrl = HttpRuntime.AppDomainAppVirtualPath;
        if(appUrl != "/")
            appUrl = appUrl + "/";
        var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
        var fdenable = CommonFunction.Instance.FrontDoorEnable();
        if(fdenable.ToLower() == "yes")
        {
            var fdurl = CommonFunction.Instance.FrontDoorUrl();
            baseUrl = fdurl;
        }
        //
        return baseUrl;
    }
    public List<T_DocumentTemplate> FilterDocumentTemplateForSelectedTenant(List<T_DocumentTemplate> DocumentTemplates)
    {
        if(User.MultiTenantLoginSelected == null) return DocumentTemplates;
        var tenantList = User.MultiTenantLoginSelected.Select(p => Convert.ToString(p.T_MainEntity)).ToList();
        if(tenantList.Count() == 0 || tenantList.Contains("-1")) return DocumentTemplates;
        foreach(var template in DocumentTemplates)
        {
            if(template.T_Tenants == null) continue;
            List<string> test = template.T_Tenants.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            var allowed = test.Intersect(tenantList).ToList();
            if(allowed.Count() == 0)
                template.T_AllowedRoles = "Document not allowed";
        }
        return DocumentTemplates;
    }
    /// <summary>Getfavorite URL.</summary>
    ///
    /// <returns>A string.</returns>
    
    public string getfavoriteUrl()
    {
        string baseUri = "";
        Uri uri = new Uri(Request.Url.AbsoluteUri);
        string pathQuery = uri.PathAndQuery;
        string hostName = uri.ToString().Replace(pathQuery, "");
        var virtualurl = VirtualPathUtility.ToAbsolute("~/");
        if(virtualurl == "/" || string.IsNullOrEmpty(virtualurl))
            baseUri = pathQuery.ToString().Replace(hostName, "/");
        else
            baseUri = pathQuery.ToString().Replace(virtualurl, ""); ;
        return baseUri;
    }
    
    #region Business Rules
    
    /// <summary>Gets mandatory properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The mandatory properties dictionary.</returns>
    
    public Dictionary<string, string> GetMandatoryPropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        return BusinessRuleHelper.GetMandatoryPropertiesDictionary(User, db, OModel, ruleType, entityName);
    }
    
    /// <summary>Gets validate before save properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The validate before save properties dictionary.</returns>
    
    public Dictionary<string, string> GetValidateBeforeSavePropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        return BusinessRuleHelper.GetValidateBeforeSavePropertiesDictionary(User, db, OModel, ruleType, entityName);
    }
    /// <summary>Gets validate before save properties dictionary For Popup Confirm.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The validate before save properties dictionary Popup Confirm.</returns>
    
    public Dictionary<string, string> GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        return BusinessRuleHelper.GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, entityName);
    }
    /// <summary>Gets lock business rules dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The lock business rules dictionary.</returns>
    
    public Dictionary<string, string> GetLockBusinessRulesDictionary(IUser User, ApplicationContext db, dynamic OModel, string entityName)
    {
        return BusinessRuleHelper.GetLockBusinessRulesDictionary(User, db, OModel, entityName);
    }
    
    /// <summary>Check mandatory properties.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    
    public List<string> CheckMandatoryProperties(IUser User, ApplicationContext db, dynamic OModel, string entityName)
    {
        return BusinessRuleHelper.CheckMandatoryProperties(User, db, OModel, entityName);
    }
    
    /// <summary>Gets read only properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="ruleType">page from.</param>
    ///
    /// <returns>The read only properties dictionary.</returns>
    public Dictionary<string, string> GetReadOnlyPropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string entityName, string ruleType = null)
    {
        return BusinessRuleHelper.GetReadOnlyPropertiesDictionary(User, db, OModel, entityName, ruleType);
    }
    /// <summary>Check hidden.</summary>
    ///
    /// <param name="User">           The user.</param>
    /// <param name="entityName">     Name of the entity.</param>
    /// <param name="brType">         Type of the line break.</param>
    /// <param name="isHiddenGroup">  True if is hidden group, false if not.</param>
    /// <param name="rbList">         List of rbs.</param>
    /// <param name="inlinesuffix">   (Optional) The inlinesuffix.</param>
    /// <param name="inlinedivsuffix">(Optional) The inlinedivsuffix.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkHidden(IUser User, string entityName, string brType, bool isHiddenGroup, string[] rbList, string inlinesuffix = "", string inlinedivsuffix = "")
    {
        return BusinessRuleHelper.checkHidden(User, entityName, brType, isHiddenGroup, rbList, inlinesuffix, inlinedivsuffix);
    }
    
    /// <summary>Check set value user interface rule.</summary>
    ///
    /// <param name="User">          The user.</param>
    /// <param name="entityName">    Name of the entity.</param>
    /// <param name="brType">        Type of the line break.</param>
    /// <param name="idsOfBRType">   Type of the identifiers of line break.</param>
    /// <param name="rbList">        List of rbs.</param>
    /// <param name="inlineAssoList">List of inline assoes.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string checkSetValueUIRule(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        return BusinessRuleHelper.checkSetValueUIRule(User, entityName, brType, idsOfBRType, rbList, inlineAssoList);
    }
    
    /// <summary>Check set value user interface rule.</summary>
    ///
    /// <param name="User">          The user.</param>
    /// <param name="entityName">    Name of the entity.</param>
    /// <param name="brType">        Type of the line break.</param>
    /// <param name="idsOfBRType">   Type of the identifiers of line break.</param>
    /// <param name="rbList">        List of rbs.</param>
    /// <param name="inlineAssoList">List of inline assoes.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string RestrictDropdownValueRule(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        return BusinessRuleHelper.RestrictDropdownValueRule(User, entityName, brType, idsOfBRType, rbList, inlineAssoList);
    }
    public static string RestrictDropdownValueRuleInLineEdit(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        return BusinessRuleHelper.RestrictDropdownValueRuleInLineEdit(User, entityName, brType, idsOfBRType, rbList, inlineAssoList);
    }
    
    #endregion
    
    /// <summary>
    /// /
    /// </summary>
    /// <param name="entityObject"></param>
    /// <param name="entityName"></param>
    /// <param name="addresses"></param>
    /// <returns></returns>
    public string ValidateAddress(dynamic entityObject, string entityName, string addresses)
    {
        string NotValidAddressName = "";
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        string Apikey = commonObj.GetGoogleMapApiKey();
        if(string.IsNullOrEmpty(Apikey))
            return "";
        var addressPropetyIds = addresses.Split(',');
        foreach(var addressPropetyId in addressPropetyIds)
        {
            var address = "";
            var NotValidAddressNameVal = "";
            var entityModel = ModelReflector.Entities;
            var Entity = entityModel.FirstOrDefault(p => p.Name == entityName);
            var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == addressPropetyId);
            var AssociationInfo = Entity.Associations.FirstOrDefault(p => p.Name == addressPropetyId);
            if(AssociationInfo != null)
            {
                NotValidAddressNameVal = AssociationInfo.DisplayName;
                object assocObj = (object)entityObject;
                object assocVal = assocObj.GetType().GetProperty(addressPropetyId.ToLower()).GetValue(assocObj);
                Type type = Type.GetType("GeneratorBase.MVC.Models." + AssociationInfo.Target + ", GeneratorBase.MVC.Models");
                System.Reflection.MethodInfo mc = type.GetMethod("getDisplayValue");
                var result1 = mc.Invoke(assocVal, null);
                address = Convert.ToString(result1);
            }
            if(PropInfo != null)
            {
                NotValidAddressNameVal = PropInfo.DisplayName;
                object assocObj = (object)entityObject;
                object assocVal = assocObj.GetType().GetProperty(addressPropetyId).GetValue(assocObj);
                address = Convert.ToString(assocVal);
            }
            if(!(string.IsNullOrEmpty(address)))
            {
                var url = "https://maps.googleapis.com/maps/api/geocode/xml?address=" + Url.Encode(address) + "&key=" + Apikey + "&mode=driving&language=en-EN&sensor=false&libraries=places";
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse response = webRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string responsereader = reader.ReadToEnd();
                response.Close();
                DataSet ds = new DataSet();
                ds.ReadXml(new System.Xml.XmlTextReader(new StringReader(responsereader)));
                if(ds.Tables.Count > 0)
                {
                    if(ds.Tables["GeocodeResponse"].Rows[0]["status"].ToString() == "OK")
                    {
                        int Addresscount = ds.Tables["result"].Rows.Count;
                        if(Addresscount > 1)
                        {
                            return NotValidAddressNameVal;
                        }
                    }
                    else
                        return NotValidAddressNameVal;
                }
            }
        }
        return NotValidAddressName;
    }
    
    #region Import Excel
    
    /// <summary>Determine if we are all columns empty.</summary>
    ///
    /// <param name="dr">          The dr.</param>
    /// <param name="sheetColumns">The sheet columns.</param>
    ///
    /// <returns>True if all columns empty, false if not.</returns>
    
    public bool AreAllColumnsEmpty(DataRow dr, List<string> sheetColumns)
    {
        return ExcelImportHelper.AreAllColumnsEmpty(dr, sheetColumns);
    }
    
    /// <summary>Data import.</summary>
    ///
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="fileLocation"> The file location.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    public DataSet DataImport(string fileExtension, string fileLocation)
    {
        return ExcelImportHelper.DataImport(fileExtension, fileLocation);
    }
    
    /// <summary>Without blank row.</summary>
    ///
    /// <param name="ds">The ds.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    public DataSet WithoutBlankRow(DataSet ds)
    {
        return ExcelImportHelper.WithoutBlankRow(ds);
    }
    
    /// <summary>Validates the model described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ValidateModel(object validate)
    {
        return ExcelImportHelper.ValidateModel(validate);
    }
    
    /// <summary>Validates the model with errors described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>A list of.</returns>
    
    public ICollection<ValidationResult> ValidateModelWithErrors(object validate)
    {
        return ExcelImportHelper.ValidateModelWithErrors(validate);
    }
    #endregion
    
    #region Search
    
    /// <summary>Gets data type of property.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="valueType">   (Optional) True to value type.</param>
    ///
    /// <returns>The data type of property.</returns>
    
    public List<string> GetDataTypeOfProperty(string entityName, string propertyName, bool valueType = false)
    {
        return SearchHelper.GetDataTypeOfProperty(entityName, propertyName, valueType);
    }
    
    /// <summary>Gets property dp.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>The property dp.</returns>
    
    public string GetPropertyDP(string entityName, string propertyName)
    {
        return SearchHelper.GetPropertyDP(entityName, propertyName);
    }
    
    /// <summary>Condition f search.</summary>
    ///
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="property">  The property.</param>
    /// <param name="operators"> The operators.</param>
    /// <param name="value">     The value.</param>
    ///
    /// <returns>A string.</returns>
    
    public string conditionFSearch(string entityName, string property, string operators, string value, string type = null)
    {
        return SearchHelper.conditionFSearch(entityName, property, operators, value, type);
    }
    #endregion
    
    #region Entity Default Page
    
    /// <summary>Gets templates for list.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for list.</returns>
    
    public string GetTemplatesForList(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForList(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for details.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for details.</returns>
    
    public string GetTemplatesForDetails(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForDetails(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for edit.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit.</returns>
    
    public string GetTemplatesForEdit(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForEdit(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for create quick.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create quick.</returns>
    
    public string GetTemplatesForCreateQuick(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForCreateQuick(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for create.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create.</returns>
    
    public string GetTemplatesForCreate(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForCreate(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for edit quick.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit quick.</returns>
    
    public string GetTemplatesForEditQuick(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForEditQuick(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for create wizard.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create wizard.</returns>
    
    public string GetTemplatesForCreateWizard(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForCreateWizard(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for edit wizard.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit wizard.</returns>
    
    public string GetTemplatesForEditWizard(IUser User, string entityName, string defaultview)
    {
        return EntityDefaultPageHelper.GetTemplatesForEditWizard(User, entityName, defaultview);
    }
    
    /// <summary>Gets templates for search.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The templates for search.</returns>
    
    public string GetTemplatesForSearch(IUser User, string entityName)
    {
        return EntityDefaultPageHelper.GetTemplatesForSearch(User, entityName);
    }
    #endregion
    
    #region Common ViewBags and ViewData
    /// <summary>Sets viewbag for SetFSearch.</summary>
    ///
    /// <param name="entityName">      The Entity Name.</param>
    ///
    /// <returns>Sets viewbag for SetFSearch action</returns>
    public void SetFSearchViewBag(string entityName)
    {
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
            ViewBag.FavoriteItem = lstFavoriteItem;
        ViewBag.EntityName = entityName;
        var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == ViewBag.EntityName);
        var Prop = Entity.Properties.Where(wh => wh.Name != "TenantId").Select(z => new
        {
            z.DisplayName,
            z.Name
        });
        var sortlist = Prop;
        ViewBag.PropertyList = new SelectList(Prop, "Name", "DisplayName");
        ViewBag.SuggestedDynamicValueInCondition7 = new SelectList(Prop, "Name", "DisplayName");
        Dictionary<string, string> Dumplist = new Dictionary<string, string>();
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                          = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(Dumplist, "key", "value");
        ViewBag.SortOrder1 = new SelectList(sortlist, "Name", "DisplayName");
        ViewBag.GroupByColumn = new SelectList(sortlist, "Name", "DisplayName");
    }
    /// <summary>Sets viewbag for FSearch.</summary>
    public void FSearchViewBag(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, string FilterCondition, string HostingEntity, string AssociatedType, string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn, bool? IsReports, bool? IsdrivedTab, bool? IsFilter, string EntityName)
    {
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewBag.SearchResult = "";
        ViewData["HideColumns"] = HideColumns;
        ViewBag.GroupByColumn = GroupByColumn;
        ViewBag.IsdrivedTab = IsdrivedTab;
        ViewData["IsFilter"] = IsFilter;
        if(!string.IsNullOrEmpty(viewtype))
            ViewBag.TemplatesName = viewtype.Replace("?IsAddPop=true", "");
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
        if(!string.IsNullOrEmpty(searchString) && FSFilter == null)
            ViewBag.FSFilter = "Fsearch";
        if(!string.IsNullOrEmpty(SortOrder))
        {
            ViewBag.SearchResult += " \r\n Sort Order = ";
            var sortString = "";
            var sortProps = SortOrder.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            foreach(var prop in sortProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                if(prop.Contains("."))
                {
                    sortString += prop + ",";
                    continue;
                }
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    sortString += asso.DisplayName + ">";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    sortString += propInfo.DisplayName + ">";
                }
            }
            ViewBag.SearchResult += sortString.TrimEnd(">".ToCharArray());
        }
        if(!string.IsNullOrEmpty(GroupByColumn))
        {
            ViewBag.SearchResult += " \r\n Group By = ";
            var groupbyString = "";
            var GroupProps = GroupByColumn.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            foreach(var prop in GroupProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    groupbyString += asso.DisplayName + " > ";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    groupbyString += propInfo.DisplayName + " > ";
                }
            }
            ViewBag.SearchResult += groupbyString.TrimEnd(" > ".ToCharArray());
        }
    }
    public object SetPagination(object model, string entityName)
    {
        int pageSize = 10;
        int pageNumber = (((IndexArgsOption)model).page ?? 1);
        if(((IndexArgsOption)model).itemsPerPage != null)
        {
            pageSize = (int)((IndexArgsOption)model).itemsPerPage;
            ((IndexArgsOption)model).CurrentItemsPerPage = ((IndexArgsOption)model).itemsPerPage;
        }
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + entityName] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + entityName].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + entityName].Value);
            ((IndexArgsOption)model).CurrentItemsPerPage = ((IndexArgsOption)model).itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + entityName] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + entityName].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + entityName].Value);
        }
        ((IndexArgsOption)model).PageSize = pageSize;
        ((IndexArgsOption)model).Pages = pageNumber;
        ((IndexArgsOption)model).page = pageNumber;
        ((IndexArgsOption)model).pageNumber = pageNumber;
        return model;
    }
    /// <summary>Sets viewdata for Index action.</summary>
    public void IndexViewBag(IndexArgsOption args)
    {
        ViewData["HostingEntity"] = args.HostingEntity;
        ViewData["HostingEntityID"] = args.HostingEntityID;
        ViewData["AssociatedType"] = args.AssociatedType;
        ViewData["IsFilter"] = args.IsFilter;
        ViewData["BulkOperation"] = args.BulkOperation;
        ViewData["caller"] = args.caller;
        ViewData["CustomParameter"] = args.CustomParameter;
        ViewData["IsdrivedTab"] = args.IsdrivedTab;
        ViewData["HideColumns"] = args.HideColumns; // added by rachana
        ViewBag.SearchResult = "";
        ViewBag.GroupByColumn = args.GroupByColumn;
        ViewBag.IsdrivedTab = args.IsdrivedTab;
        ViewData["IsFilter"] = args.IsFilter;
        if(!string.IsNullOrEmpty(args.viewtype))
            ViewBag.TemplatesName = args.viewtype.Replace("?IsAddPop=true", "");
        if(string.IsNullOrEmpty(args.IsAsc) && !string.IsNullOrEmpty(args.sortBy))
        {
            args.IsAsc = "ASC";
        }
        ViewBag.isAsc = args.IsAsc;
        ViewBag.CurrentSort = args.sortBy;
        ViewBag.isList = args.isList;
        if(args.searchString != null)
            args.page = null;
        else
            args.searchString = args.currentFilter;
        ViewBag.CurrentFilter = args.searchString;
        if(!string.IsNullOrEmpty(args.searchString) && args.FSFilter == null)
            ViewBag.FSFilter = "Fsearch";
        if(!string.IsNullOrEmpty(args.SortOrder))
        {
            ViewBag.SearchResult += " \r\n Sort Order = ";
            var sortString = "";
            var sortProps = args.SortOrder.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == args.EntityName);
            foreach(var prop in sortProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                if(prop.Contains("."))
                {
                    sortString += prop + ",";
                    continue;
                }
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    sortString += asso.DisplayName + ">";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    sortString += propInfo.DisplayName + ">";
                }
            }
            ViewBag.SearchResult += sortString.TrimEnd(">".ToCharArray());
        }
        if(!string.IsNullOrEmpty(args.GroupByColumn))
        {
            ViewBag.SearchResult += " \r\n Group By = ";
            var groupbyString = "";
            var GroupProps = args.GroupByColumn.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == args.EntityName);
            foreach(var prop in GroupProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    groupbyString += asso.DisplayName + " > ";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    groupbyString += propInfo.DisplayName + " > ";
                }
            }
            ViewBag.SearchResult += groupbyString.TrimEnd(" > ".ToCharArray());
        }
    }
    /// <summary>Sets viewbag for Index action.</summary>
    public void IndexViewBag(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, bool IsDivRender, string CustomParameter = null)
    {
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["IsFilter"] = IsFilter;
        ViewData["BulkOperation"] = BulkOperation;
        ViewData["caller"] = caller;
        ViewData["CustomParameter"] = CustomParameter;
        ViewBag.IsDivRender = IsDivRender;
        if(!string.IsNullOrEmpty(viewtype))
        {
            viewtype = viewtype.Replace("?IsAddPop=true", "");
            ViewBag.TemplatesName = viewtype;
        }
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
    }
    /// <summary>Sets viewbag for BulkUpdate action.</summary>
    public void BulkUpdateViewBag(string entityName, string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
    {
        if(IsDDUpdate != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
        ViewData[entityName + "ParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        string ids = string.Empty;
        if(Request.QueryString["ids"] != null)
            ids = Request.QueryString["ids"];
        ViewBag.BulkUpdate = ids;
    }
    /// <summary>Sets viewbags to render multiselect dropdown on faceted search screen.</summary>
    public void SetFSearchDropdownViewBag(IQueryable<Entity> query, string condition, string entityName, string entityDisplayName, string associationName)
    {
        if(condition != null)
        {
            var ids = condition.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            ViewBag.SearchResult += "\r\n " + entityDisplayName + "= ";
            foreach(var str in ids)
            {
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        ViewBag.SearchResult += "";
                    }
                    else if(str == "0")
                    {
                        ids1.Add(Convert.ToInt64(str));
                        ViewBag.SearchResult += "LoggedInUser, ";
                    }
                    else
                    {
                        var value = Convert.ToInt64(str);
                        ids1.Add(value);
                        var obj = query.Where(p => p.Id == value).FirstOrDefault();
                        ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                    }
                }
            }
            ids1 = ids1.ToList();
            var list = query.OrderBy(p => p.DisplayValue).Take(10).Union(query.Where(p => ids1.Contains(p.Id))).Distinct().ToList();
            if(ids1.Contains(0))
            {
                var list_item = new SelectList(list, "ID", "DisplayValue").ToList();
                list_item.Insert(0, (new SelectListItem { Text = "LoggedInUser", Value = "0" }));
                ViewData.Add(associationName, list_item);
            }
            else
                ViewData.Add(associationName, new SelectList(list, "ID", "DisplayValue"));
        }
        else
        {
            var list = query.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
            ViewData.Add(associationName, new SelectList(list, "ID", "DisplayValue"));
        }
    }
    #endregion
    /// <summary>Set Cookie.</summary>
    ///
    /// <param name="cookiename"> The cookiename.</param>
    /// <param name="value">value.</param>
    public void SetCookie(string cookiename, string value)
    {
        cookieParameter param = new cookieParameter(1);
        HttpCookie cookie = new HttpCookie(cookiename);
        cookie.HttpOnly = param.httponly;
        //browsers which support the secure flag will only send cookies with the secure flag when the request is going to a HTTPS page
        //web.config has default global setting for secured application cookies, removing web.config httpcookie tag will insecure cookies.
        //cookie.Secure = param.secure;
        cookie.Expires = param.expiration;
        cookie.Value = value;
        Response.Cookies.Add(cookie);
    }
    /// <summary>Remove Cookie.</summary>
    ///
    /// <param name="cookiename"> The cookiename.</param>
    public void RemoveCookie(string cookiename)
    {
        cookieParameter param = new cookieParameter(-1);
        HttpCookie cookie = new HttpCookie(cookiename);
        cookie.HttpOnly = param.httponly;
        //browsers which support the secure flag will only send cookies with the secure flag when the request is going to a HTTPS page
        //web.config has default global setting for secured application cookies, removing web.config httpcookie tag will insecure cookies.
        //cookie.Secure = param.secure;
        cookie.Value = null;
        cookie.Expires = param.expiration;
        Response.Cookies.Add(cookie);
    }
    
    /// <summary>
    ///
    /// </summary>
    /// <param name="dispName"></param>
    /// <returns></returns>
    public string GetInternalName(string dispName)
    {
        if(string.IsNullOrEmpty(dispName))
            return string.Empty;
        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[;\\\\/:*?#$%!~^,@&()\"<>|&'\\[\\]]");
        var groupName = r.Replace(dispName, " ");
        groupName = groupName.Replace(" ", "");
        groupName = groupName.Replace(".", "");
        groupName = groupName.Replace("+", "");
        groupName = groupName.Replace("-", "");
        return groupName;
    }
    public string GetProperFileDirName(string dispName)
    {
        if(string.IsNullOrEmpty(dispName))
            return string.Empty;
        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[;\\\\/:*?#$%!~^,@&()\"<>|&'\\[\\]]");
        var dirname = r.Replace(dispName, " ");
        return dirname;
    }
    
    #region Update Association Value
    
    public IEntity UpdateAssociationValue(string entity, IEntity obj, long? tenantId)
    {
        var o = obj;
        UpdateAssoc(entity, o, tenantId);
        return o;
    }
    
    private void UpdateAssoc(string entity, IEntity obj, long? tenantId)
    {
        var type = Type.GetType("GeneratorBase.MVC.Models." + entity + ", GeneratorBase.MVC.Models");
        var associations = ModelReflector.Entities.Single(e => e.Name == entity).Associations.Where(p => p.Name.ToLower() != "tenantid");
        foreach(var a in associations)
        {
            var property = type.GetProperty(a.Name.ToLower());
            var target = property.GetValue(obj) as IEntity;
            if(target == null)
            {
                if(a.Target == "IdentityUser") continue;
                long associationId = Convert.ToInt64(type.GetProperty(a.AssociationProperty).GetValue(obj));
                var idHasValue = false;
                if(associationId > 0) idHasValue = true;
                if(IsInlineAssociation(entity, a.AssociationProperty) && !idHasValue)
                {
                    var iType = Type.GetType("GeneratorBase.MVC.Models." + a.Target + ", GeneratorBase.MVC.Models");
                    var i = Activator.CreateInstance(iType, null);
                    //property.SetValue(obj, i);
                    property.SetValue(obj, null);
                }
                if(associationId > 0)
                {
                    var original = GetRecordById(a.Target, associationId);
                    type.GetProperty(a.Name.ToLower()).SetValue(obj, original);
                }
                continue;
            }
            if(target.Id == 0)
            {
                var dv = target.DisplayValue;
                var id = GetIdFromDisplayValue(a.Target, dv, tenantId);
                if(id == 0)
                {
                    target.Id = 0;
                }
                if(id > 0)
                {
                    type.GetProperty(a.AssociationProperty).SetValue(obj, id);
                    var original = GetRecordById(a.Target, id.Value);
                    type.GetProperty(a.Name.ToLower()).SetValue(obj, original);
                    continue;
                }
            }
            // explicitly create a new entity, do not search
            /// it doesn't make sense to have both the Target and AssociationProperty
            /// either the SSIS package made a mistake, or we are trying to overwrite
            /// an existing AssociationProperty
            type.GetProperty(a.AssociationProperty).SetValue(obj, target.Id);
            property.SetValue(obj, UpdateAssociationValue(a.Target, target, tenantId));
        }
    }
    
    private long? GetIdFromDisplayValue(string target, string dv, long? tenantId = null)
    {
        var type = Type.GetType("GeneratorBase.MVC.Controllers." + target + "Controller");
        var instance = GetControllerInstance(type);
        var p = new object[] { dv };
        var method = type.GetMethod("GetIdFromDisplayValue");
        if(tenantId != null && method.GetParameters().FirstOrDefault(q => q.Name == "tenantId") != null)
            p = new object[] { dv, tenantId };
        return (long?)method.Invoke(instance, p);
    }
    private IEntity GetRecordById(string target, long id)
    {
        var type = Type.GetType("GeneratorBase.MVC.Controllers." + target + "Controller");
        var instance = GetControllerInstance(type);
        var method = type.GetMethod("GetRecordById");
        var p = new object[] { id.ToString() };
        return (IEntity)method.Invoke(instance, p);
    }
    
    private Dictionary<Type, object> controllers;
    private object GetControllerInstance(Type type)
    {
        if(controllers == null) controllers = new Dictionary<Type, object>();
        if(!controllers.ContainsKey(type))
        {
            object instance = Activator.CreateInstance(type, null);
            controllers.Add(type, instance);
        }
        return controllers[type];
    }
    private bool IsInlineAssociation(string target, string name)
    {
        var type = Type.GetType("GeneratorBase.MVC.Controllers." + target + "Controller");
        var instance = GetControllerInstance(type);
        var method = type.GetMethod("InlineAssociationList");
        var result = (IEnumerable<string>)method.Invoke(instance, null) ?? new string[] { };
        return result.Contains(name);
    }
    
    #endregion
    
    #region MultiFileDownload
    public long? TryGetInt64(string item)
    {
        long i;
        bool success = long.TryParse(item, out i);
        return success ? (long?)i : (long?)null;
    }
    public async Task<byte[]> ZipDirectory(DownloadDirectory dir, bool Isflat = false)
    {
        using(var zipStream = new MemoryStream())
        {
            using(var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                bool isDupFile = false;
                var DupFile = string.Empty;
                foreach(var f in dir.AllFiles)
                {
                    var doc = await db.Documents.Where(d => d.Id == f.Id).FirstOrDefaultAsync();
                    if(doc == null) continue;
                    var entryName = "";
                    var concatId = "";
                    byte[] filedata = null;
                    var filename = doc.FileName;
                    if(doc.FileType.ToLower() == "onedrive")
                    {
                        filename = doc.DocumentName;
                        var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
                        string AccessToken = string.Empty;
                        if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                            AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
                        if(AccessToken != null)
                        {
                            filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
                        }
                    }
                    else if(doc.FileType.ToLower() == "file")
                    {
                        filename = doc.DocumentName;
                        string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + doc.FileName;
                        filedata = System.IO.File.ReadAllBytes(filepath);
                    }
                    else
                    {
                        filedata = doc.Byte;
                    }
                    if(DupFile == filename)
                    {
                        isDupFile = true;
                        DupFile = string.Empty;
                    }
                    DupFile = filename;
                    if(isDupFile == true)
                        concatId = f.Id + "_";
                    if(Isflat)
                        entryName = CleanEntryName(string.Format("{0}", concatId + filename));
                    else
                        entryName = CleanEntryName(string.Format("{0} - {1}", f.Path, concatId + filename));
                    var zae = zip.CreateEntry(entryName);
                    using(var zaeStream = zae.Open())
                    {
                        await zaeStream.WriteAsync(filedata, 0, filedata.Length);
                    }
                }
            }
            return zipStream.ToArray();
        }
    }
    public string CleanEntryName(string name)
    {
        return string.Join("_", name.Split(Path.GetInvalidPathChars()));
    }
    public async Task AddDownloadDirectoryForEntity(DownloadDirectory parent, string entityName, IEntity entity, Dictionary<string, string> derived = null)
    {
        var docProps = entity.GetType().GetProperties()
                       .Where(p => p.IsDefined(typeof(PropertyTypeForEntity), false))
                       .Where(p =>
        {
            var attr = (PropertyTypeForEntity[])p.GetCustomAttributes(typeof(PropertyTypeForEntity), false);
            return attr.Any(a => "Document".Equals(a.PropCheck, StringComparison.OrdinalIgnoreCase));
        })
        .Where(p => User.CanView(entityName, p.Name) && User.CanView("Document"))
        .Where(p => p.GetValue(entity, null) != null);
        var d = new DownloadDirectory()
        {
            Parent = parent,
            Name = entity.DisplayValue.Replace("/", "_")
        };
        foreach(var prop in docProps)
        {
            if(derived != null && derived.Count > 0 && derived.Where(wh => wh.Key == entityName && wh.Value == prop.Name).Count() > 0)
                continue;
            var name = prop.Name;
            var cdn = (CustomDisplayNameAttribute[])prop.GetCustomAttributes(typeof(CustomDisplayNameAttribute), false);
            if(cdn.Any()) name = cdn.First().DisplayName;
            var val = (long)prop.GetValue(entity, null);
            var file = new DownloadFile()
            {
                Parent = d,
                Id = val,
                PropertyName = name
            };
            if(d.Files == null)
                d.Files = new List<DownloadFile>();
            d.Files.Add(file);
        }
        await AddDownloadDirectoriesForAssociations(d, entityName, entity, derived);
        if(d.AllFiles.Any())
        {
            if(d.Children == null)
                d.Children = new List<DownloadDirectory>();
            parent.Children.Add(d);
        }
    }
    public async Task AddDownloadDirectoriesForAssociations(DownloadDirectory parent, string entityName, IEntity entity, Dictionary<string, string> derived = null)
    {
        var dirs = new List<DownloadDirectory>();
        foreach(var assoc in GetOneToMany(entityName))
        {
            string associatedEntityName = assoc.Name;
            if(User.CanView(associatedEntityName) && User.CanView("Document"))
            {
                IEnumerable<dynamic> query = typeof(ApplicationContext).GetProperty(associatedEntityName + "s").GetValue(db, null) as IEnumerable<dynamic>;
                var selectQuery = (IQueryable<dynamic>)query;
                string whereCondition = assoc.AssociationProperty + "=" + entity.Id;
                selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition);
                List<IEntity> associatedEntities = (await(from a in selectQuery select a).ToListAsync()).Cast<IEntity>().ToList();
                var d = new DownloadDirectory()
                {
                    Parent = parent,
                    Name = assoc.DisplayName
                };
                foreach(var ae in associatedEntities)
                {
                    await AddDownloadDirectoryForEntity(d, associatedEntityName, ae, derived);
                }
                if(d.AllFiles.Any())
                {
                    if(d.Children == null)
                        d.Children = new List<DownloadDirectory>();
                    parent.Children.Add(d);
                }
            }
        }
    }
    public List<ModelReflector.Association> GetOneToMany(string Entity)
    {
        var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
        var AssoList = new List<ModelReflector.Association>();
        foreach(var ent in ModelReflector.Entities.Where(p => p != Ent))
        {
            if(ent.Associations.Any(p => p.Target == Ent.Name))
            {
                // var association  = ent.Associations.Where(p => p.Target == Ent.Name);
                foreach(var association in ent.Associations.Where(p => p.Target == Ent.Name))
                {
                    var dispName = association.DisplayName;
                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + Entity + "Controller");
                    object objController = Activator.CreateInstance(controller, null);
                    MethodInfo mc = controller.GetMethod("getInlineAssociationsOfEntity");
                    object[] MethodParams = new object[] { };
                    var obj = mc.Invoke(objController, MethodParams);
                    List<string> objStr = (List<string>)((System.Web.Mvc.JsonResult)(obj)).Data;
                    if(objStr.Count > 0)
                    {
                        var assocdp = Ent.Associations.Where(p => p.AssociationProperty == association.AssociationProperty);
                        if(assocdp.Count() > 0)
                            dispName = assocdp.FirstOrDefault().DisplayName;
                    }
                    else
                        dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == ent.Name).DisplayName;
                    ModelReflector.Association newAsso = new ModelReflector.Association();
                    newAsso.Name = ent.Name;
                    newAsso.Target = association.Target;
                    newAsso.DisplayName = dispName;// association.DisplayName;
                    newAsso.AssociationProperty = association.AssociationProperty;
                    if(Ent.Name == association.Name)
                        newAsso.AssociationProperty = association.AssociationProperty;
                    AssoList.Add(newAsso);
                }
            }
        }
        return AssoList;
    }
    public class DownloadDirectory
    {
        public DownloadDirectory Parent
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        
        public List<DownloadDirectory> Children = new List<DownloadDirectory>();
        
        public List<DownloadFile> Files = new List<DownloadFile>();
        
        
        public IEnumerable<DownloadFile> AllFiles
        {
            get
            {
                return Children == null ? new List<DownloadFile>() : Children.SelectMany(c => c.AllFiles).Concat(Files);
            }
        }
        public string Path
        {
            get
            {
                return Parent == null ? "" : System.IO.Path.Combine(Parent.Path, Name);
            }
        }
    }
    public class DownloadFile
    {
        public DownloadDirectory Parent
        {
            get;
            set;
        }
        public long Id
        {
            get;
            set;
        }
        public string PropertyName
        {
            get;
            set;
        }
        public string DocumentName
        {
            get;
            set;
        }
        public byte[] Bytes
        {
            get;
            set;
        }
        public string DocumentType
        {
            get;
            set;
        }
        public string Path
        {
            get
            {
                return System.IO.Path.Combine(Parent.Path, PropertyName);
            }
        }
    }
    #endregion
    
    #region Generate BarCode and QRCode
    
    public static Byte[] BitmapToBytesCode(Bitmap image)
    {
        using(MemoryStream stream = new MemoryStream())
        {
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
    
    
    #region BarCode and QRCode Using ZXing.Net Library
    
    [Audit(0)]
    public Byte[] GenerateBarCode(Enum typeOfBarcode, int width, int height, string entityName, IEntity obj)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter();
        EncodingOptions encodingOptions = new EncodingOptions();
        encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
        barcodeWriter.Renderer = new BitmapRenderer();
        //barcodeWriter.Renderer = new BitmapRenderer()
        //{
        //    Foreground = Color.Orange,
        //};
        //string barcodeType = Enum.GetName(typeof(TypeOfBarcode), typeOfBarcode);
        Bitmap bitmap;
        if(typeOfBarcode != null && (TypeOfBarcode)typeOfBarcode != TypeOfBarcode.QR_CODE)   //Barcode
        {
            string displayStr = entityName + "_" + obj.Id;
            encodingOptions = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 2,
                PureBarcode = true
            }; //GS1Format = true
            barcodeWriter.Options = encodingOptions;
            if((TypeOfBarcode)typeOfBarcode == TypeOfBarcode.CODE_128)
                barcodeWriter.Format = BarcodeFormat.CODE_128;
            if((TypeOfBarcode)typeOfBarcode == TypeOfBarcode.CODE_39)
                barcodeWriter.Format = BarcodeFormat.CODE_39;
            if((TypeOfBarcode)typeOfBarcode == TypeOfBarcode.CODE_93)
                barcodeWriter.Format = BarcodeFormat.CODE_93;
            if((TypeOfBarcode)typeOfBarcode == TypeOfBarcode.ITF)
                barcodeWriter.Format = BarcodeFormat.ITF;
            bitmap = barcodeWriter.Write(displayStr);//$"{(char)0x00F1}01234567890{(char)0x00F1}45678"
        }
        else //QRcode
        {
            string displayStr = string.Empty;
            //Create URL
            var request = System.Web.HttpContext.Current.Request;
            string URLStr = (request != null) ? string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new UrlHelper(request.RequestContext)).Content("~")) : null;
            if(!string.IsNullOrEmpty(URLStr))
            {
                URLStr += entityName + "/Details/" + obj.Id;
                displayStr += URLStr;
            }
            encodingOptions = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 1,
                PureBarcode = false
            };
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            bitmap = barcodeWriter.Write(displayStr);
            //set logo in Center of  QRCode
            Bitmap logo = new Bitmap(Bitmap.FromFile(System.Web.HttpContext.Current.Server.MapPath("~/logo/logoBW.png")));
            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(logo, new Point((bitmap.Width - logo.Width) / 2, (bitmap.Height - logo.Height) / 2));
        }
        return BitmapToBytesCode(bitmap);
    }
    public enum TypeOfBarcode
    {
        //
        // Summary:
        //     Aztec 2D barcode format.
        AZTEC = 1,
        //
        // Summary:
        //     CODABAR 1D format.
        CODABAR = 2,
        //
        // Summary:
        //     Code 39 1D format.
        CODE_39 = 4,
        //
        // Summary:
        //     Code 93 1D format.
        CODE_93 = 8,
        //
        // Summary:
        //     Code 128 1D format.
        CODE_128 = 16,
        //
        // Summary:
        //     Data Matrix 2D barcode format.
        DATA_MATRIX = 32,
        //
        // Summary:
        //     EAN-8 1D format.
        EAN_8 = 64,
        //
        // Summary:
        //     EAN-13 1D format.
        EAN_13 = 128,
        //
        // Summary:
        //     ITF (Interleaved Two of Five) 1D format.
        ITF = 256,
        //
        // Summary:
        //     MaxiCode 2D barcode format.
        MAXICODE = 512,
        //
        // Summary:
        //     PDF417 format.
        PDF_417 = 1024,
        //
        // Summary:
        //     QR Code 2D barcode format.
        QR_CODE = 2048,
        //
        // Summary:
        //     RSS 14
        RSS_14 = 4096,
        //
        // Summary:
        //     RSS EXPANDED
        RSS_EXPANDED = 8192,
        //
        // Summary:
        //     UPC-A 1D format.
        UPC_A = 16384,
        //
        // Summary:
        //     UPC-E 1D format.
        UPC_E = 32768,
        //
        // Summary:
        //     UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 | ITF
        //     | RSS_14 | RSS_EXPANDED without MSI (to many false-positives) and IMB (not enough
        //     tested, and it looks more like a 2D)
        All_1D = 61918,
        //
        // Summary:
        //     UPC/EAN extension format. Not a stand-alone format.
        UPC_EAN_EXTENSION = 65536,
        //
        // Summary:
        //     MSI
        MSI = 131072,
        //
        // Summary:
        //     Plessey
        PLESSEY = 262144,
        //
        // Summary:
        //     Intelligent Mail barcode
        IMB = 524288,
        //
        // Summary:
        //     Pharmacode format.
        PHARMA_CODE = 1048576
    }
    
    #endregion
    
    #endregion
    
    public FileContentResult DownloadZipFile(string directoryToZip, string ZipName)
    {
        var fileName = string.Format("{0}.zip", ZipName);
        var temppath = Server.MapPath("~/Files/");
        if(!Directory.Exists(temppath))
        {
            Directory.CreateDirectory(temppath);
        }
        var tempOutPutPath = Path.Combine(temppath, fileName);
        var filenames = Directory.GetFiles(directoryToZip, "*.*", SearchOption.AllDirectories);
        using(var s = new ZipOutputStream(System.IO.File.Create(tempOutPutPath)))
        {
            s.SetLevel(9);
            var buffer = new byte[4096];
            foreach(var file in filenames)
            {
                var relativePath = file.Substring(directoryToZip.Length).TrimStart('\\');
                var entry = new ZipEntry(relativePath);
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);
                using(var fs = System.IO.File.OpenRead(file))
                {
                    int sourceBytes;
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, sourceBytes);
                    }
                    while(sourceBytes > 0);
                }
            }
            s.Finish();
            s.Flush();
            s.Close();
        }
        byte[] finalResult = System.IO.File.ReadAllBytes(tempOutPutPath);
        if(System.IO.File.Exists(tempOutPutPath))
            System.IO.File.Delete(tempOutPutPath);
        if(Directory.Exists(directoryToZip))
            Directory.Delete(directoryToZip, true);
        if(finalResult == null || !finalResult.Any())
            throw new Exception(String.Format("No Files found with Image"));
        return File(finalResult, "application/zip", fileName);
    }
    
    public IQueryable filtercondtion(IQueryable datalist, long Id, string entity)
    {
        StringBuilder whereCondition = new StringBuilder();
        var conditions = (new ConditionContext()).Conditions.Where(p => p.ExportDetailConditionsID == Id).ToList();
        int iCnt = 1;
        foreach(var cond in conditions)
        {
            var PropertyName = cond.PropertyName;
            var Operator = cond.Operator;
            var Value = cond.Value;
            var LogicalConnector = cond.LogicalConnector;
            var type = string.Empty;
            if(iCnt == conditions.Count())
                LogicalConnector = "";
            whereCondition.Append(conditionFSearch(entity, PropertyName, Operator, Value.Trim(), type) + LogicalConnector);
            iCnt++;
        }
        if(!string.IsNullOrEmpty(whereCondition.ToString()))
            return datalist.Where(whereCondition.ToString());
        else
            return datalist;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="VerbEntityName"></param>
    /// <returns></returns>
    [Audit(0)]
    public List<(long, string)> GetGroupListForAll(string VerbEntityName = "")
    {
        List<(long, string)> grouplist = new List<(long, string)>();
        if(!string.IsNullOrEmpty(VerbEntityName))
        {
            Type controller = new CreateControllerType(VerbEntityName).controllerType;
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("GetGroupList");
            object[] MethodParams = new object[] { };
            dynamic GetGroupList = mc.Invoke(objController, MethodParams);
            // var enitty= GeneratorBase.MVC.ModelReflector.Entities.Where(p => p.Name
            //var attr = property.GetCustomAttribute<DisplayNameAttribute>(false);
            //List<PropertyInfoForEntity> gropname =
            Dictionary<long, string> result = GetGroupList;
            grouplist = new List<(long, string)>(result.Select(x => (x.Key, x.Value)));
            //if (key != null && key.Trim().Length > 0)
            //    grouplist = grouplist.Where(p => p.Item2.Contains(key));
        }
        return grouplist;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="VerbEntityName"></param>
    /// <returns></returns>
    ///
    [Audit(0)]
    public List<VerbInformationDetails> GetVerbListForAll(string VerbEntityName = "")
    {
        List<VerbInformationDetails> getVerbOfEntitylist = new List<VerbInformationDetails>();
        if(!string.IsNullOrEmpty(VerbEntityName))
        {
            Type controller = new CreateControllerType(VerbEntityName).controllerType;
            if(controller != null)
            {
                object objController = Activator.CreateInstance(controller, null);
                System.Reflection.MethodInfo mc = controller.GetMethod("getVerbsDetails");
                object[] MethodParams = new object[] { };
                dynamic GetVerbOfEntitylist = mc.Invoke(objController, MethodParams);
                getVerbOfEntitylist = ((List<VerbInformationDetails>)GetVerbOfEntitylist).ToList();
            }
            else
                return getVerbOfEntitylist;
        }
        return getVerbOfEntitylist;
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

/// <summary>A local date time converter.</summary>
public class LocalDateTimeConverter : ActionFilterAttribute
{
    /// <summary>Called by the ASP.NET MVC framework before the action method executes.</summary>
    ///
    /// <param name="filterContext">The filter context.</param>
    
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //var model = filterContext.Controller.ViewData.Model;
    }
    
    /// <summary>Called by the ASP.NET MVC framework after the action method executes.</summary>
    ///
    /// <param name="filterContext">The filter context.</param>
    
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        var model = filterContext.Controller.ViewData.Model;
        if(model != null && filterContext.HttpContext.Request.Cookies["_timezone"] != null)
            ProcessDateTimeProperties(model, filterContext);
        base.OnActionExecuted(filterContext);
    }
    
    /// <summary>Process the date time properties.</summary>
    ///
    /// <param name="obj">          The object.</param>
    /// <param name="filterContext">Context for the filter.</param>
    
    private void ProcessDateTimeProperties(object obj, ActionExecutedContext filterContext)
    {
        var result = "Eastern Standard Time";
        if(System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != null && System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != "Default")
        {
            var timeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
            if(timeZone != null)
            {
                result = Convert.ToString(timeZone);
            }
        }
        else if(HttpContext.Current != null)
        {
            var timeZoneCookie = HttpContext.Current.Request.Cookies["_timezone"];
            if(timeZoneCookie != null)
            {
                result = Convert.ToString(timeZoneCookie.Value);
            }
        }
        if(obj.GetType().IsSubclassOf(typeof(IndexArgsOption)))
        {
            try
            {
                var listobj = obj.GetType().GetProperty("list");
                if(listobj != null)
                {
                    var list = ((PagedList.IPagedList<object>)listobj.GetValue(obj, null));
                    foreach(var item in list)
                    {
                        ProcessDateTimeProperties(item, filterContext);
                    }
                }
            }
            catch(Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        else if(obj.GetType().IsGenericType)
        {
            try
            {
                var list = ((PagedList.IPagedList<object>)obj);
                foreach(var item in list)
                {
                    ProcessDateTimeProperties(item, filterContext);
                }
            }
            catch(Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        else
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            props.AddRange(obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).ToList());
            foreach(PropertyInfo propertyInfo in props)
            {
                if(propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                {
                    if(propertyInfo.Name.StartsWith("m_")) continue;
                    var original = propertyInfo.GetValue(obj, new object[] { });
                    var tempobj = props.FirstOrDefault(p => p.Name == "m_" + propertyInfo.Name);
                    if(tempobj != null)
                    {
                        //var originalValue  = original != null ? GeneratorBase.MVC.Controllers.DateTimeExtention.toLocal(Convert.ToDateTime(original), result) : null;
                        var timezoneresult = CheckCustomTimeZone(obj, props, propertyInfo.Name, result, tempobj.DeclaringType.Name);
                        var originalValue = original != null ? GeneratorBase.MVC.Controllers.DateTimeExtention.toLocal(Convert.ToDateTime(original), timezoneresult) : null;
                        tempobj.SetValue(obj, originalValue, new object[] { });
                        propertyInfo.SetValue(obj, originalValue, new object[] { });
                    }
                }
            }
        }
    }
    
    /// <summary>Check custom time zone.</summary>
    ///
    /// <param name="objEntity">           The object entity.</param>
    /// <param name="listProps">           The list properties.</param>
    /// <param name="propName">            Name of the property.</param>
    /// <param name="timezonecustomresult">The timezonecustomresult.</param>
    /// <param name="entityName">          Name of the entity.</param>
    ///
    /// <returns>A string.</returns>
    
    public string CheckCustomTimeZone(object objEntity, List<PropertyInfo> listProps, string propName, string timezonecustomresult, string entityName)
    {
        var prop = listProps.FirstOrDefault(p => p.Name == "T_AssociatedTimeZone" + entityName.Replace("T_", "") + propName.Replace("T_", "") + "ID");
        if(prop != null)
        {
            var timezonecustomid = prop.GetValue(objEntity, new object[] { });
            if(timezonecustomid != null)
            {
                using(ApplicationContext dbtz = new ApplicationContext(new SystemUser()))
                {
                    //need to handle T_TimeZone with code prefix
                    var timezonecustominfo = dbtz.T_TimeZones.Find(Convert.ToInt64(timezonecustomid));
                    if(timezonecustominfo != null)
                    {
                        timezonecustomresult = timezonecustominfo.T_FullName;
                    }
                }
            }
        }
        return timezonecustomresult;
    }
}
/// <summary>A date time extention.</summary>
public static class DateTimeExtention
{
    /// <summary>A DateTime? extension method that converts this object to a local.</summary>
    ///
    /// <param name="datetime">The datetime to act on.</param>
    /// <param name="timeZone">The time zone.</param>
    ///
    /// <returns>The given data converted to a DateTime?</returns>
    
    public static DateTime? toLocal(this DateTime? datetime, string timeZone)
    {
        if(timeZone.ToLower() != "unknown time zone" && timeZone.ToLower() != "greenwich mean time" && timeZone.ToLower() != "coordinated universal time")
        {
            string easternZoneId = timeZone;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(datetime.Value, easternZone);
        }
        return datetime;
    }
}
}