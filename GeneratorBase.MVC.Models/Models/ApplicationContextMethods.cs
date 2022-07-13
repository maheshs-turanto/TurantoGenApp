using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using RecurrenceGenerator;
namespace GeneratorBase.MVC
{
/// <summary>Application context helper partial class, containing helper methods.</summary>
public partial class ApplicationContext : DbContext
{
    /// <summary>Determine if modified.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="OriginalObj">  The original object.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool CheckIfModified(DbEntityEntry dbEntityEntry, DbPropertyValues OriginalObj)
    {
        bool changed = false;
        if(dbEntityEntry.State == EntityState.Modified)
        {
            var CurrentObj = dbEntityEntry.CurrentValues;
            if(OriginalObj != null && CurrentObj != null)
                foreach(var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    if(property == "DisplayValue" || property == "ConcurrencyKey" || property == "TenantId") continue;
                    var original = OriginalObj.GetValue<object>(property);
                    var current = CurrentObj.GetValue<object>(property);
                    if(original != current && (original == null || !original.Equals(current)))
                    {
                        changed = true;
                        break;
                    }
                }
            if(!changed)
            {
                dbEntityEntry.State = EntityState.Unchanged;
            }
        }
        else changed= true;
        return changed;
    }
    /// <summary>Cancel changes (detach entity from db transaction).</summary>
    ///
    /// <param name="entry">The entry.</param>
    private static void CancelChanges(DbEntityEntry entry)
    {
        if(entry.State == EntityState.Added)
            entry.State = EntityState.Detached;
        else
            entry.State = EntityState.Unchanged;
    }
    /// <summary>Check 1 m threshold condition.</summary>
    ///
    /// <param name="entry">The entry.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool Check1MThresholdCondition(DbEntityEntry entry)
    {
        var entity = entry.Entity.GetType();
        var ResourceId_CurrentObj = entry.CurrentValues;
        return true;
    }
    /// <summary>Sets auto increment property.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    private void SetAutoProperty(DbEntityEntry dbEntityEntry)
    {
        if(dbEntityEntry.State == EntityState.Modified)
        {
            //var EntityName = dbEntityEntry.Entity.GetType().Name;
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
            if(EntityName == "T_Customer")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                object objT_AutoNo_CurrentValue = T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo");
                long T_AutoNo_CurrentValue = Convert.ToInt64(objT_AutoNo_CurrentValue);
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + 0;
                if(objT_AutoNo_CurrentValue == null && T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    using(var dbforAuto = new ApplicationContext(new SystemUser(), true))
                    {
                        dbforAuto.Configuration.LazyLoadingEnabled = false;
                        dbforAuto.Configuration.AutoDetectChangesEnabled = false;
                        var obj = dbforAuto.T_Customers.AsNoTracking().FirstOrDefault(p => p.T_AutoNo == T_AutoNo_CorrectValue);
                        if(obj != null)
                        {
                            var context = new System.Web.Routing.RequestContext(
                                new HttpContextWrapper(System.Web.HttpContext.Current),
                                new System.Web.Routing.RouteData());
                            var urlHelper = new System.Web.Mvc.UrlHelper(context);
                            var url = urlHelper.Action("ConcurrencyError", "Error", new { UrlReferrer = HttpContext.Current.Request.Url, Message = "Auto No. is unique." });
                            System.Web.HttpContext.Current.Response.Redirect(url);
                            return;
                        }
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
            }
            if(EntityName == "T_ExportDataConfiguration")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                long T_AutoNo_CurrentValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo")));
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "T_ExportDataLog")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                long T_AutoNo_CurrentValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo")));
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "T_MenuBar")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                long T_AutoNo_CurrentValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo")));
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "T_MenuItem")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                long T_AutoNo_CurrentValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo")));
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "T_DocumentTemplate")
            {
                var T_AutoNo_CurrentObj = dbEntityEntry.CurrentValues;
                long T_AutoNo_CurrentValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("T_AutoNo")));
                long T_AutoNo_CorrectValue = Convert.ToInt64((T_AutoNo_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(T_AutoNo_CurrentValue != T_AutoNo_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_AutoNo");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(T_AutoNo_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "ApplicationFeedback")
            {
                var CommentId_CurrentObj = dbEntityEntry.CurrentValues;
                long CommentId_CurrentValue = Convert.ToInt64((CommentId_CurrentObj.GetValue<object>("CommentId")));
                long CommentId_CorrectValue = Convert.ToInt64((CommentId_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(CommentId_CurrentValue != CommentId_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("CommentId");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(CommentId_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
            if(EntityName == "FeedbackResource")
            {
                var ResourceId_CurrentObj = dbEntityEntry.CurrentValues;
                long ResourceId_CurrentValue = Convert.ToInt64((ResourceId_CurrentObj.GetValue<object>("ResourceId")));
                long ResourceId_CorrectValue = Convert.ToInt64((ResourceId_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                if(ResourceId_CurrentValue != ResourceId_CorrectValue)
                {
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("ResourceId");
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = Convert.ChangeType(ResourceId_CorrectValue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
        }
    }
    /// <summary>Sets auto increment property.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void SetAutoProperty(Dictionary<DbEntityEntry, EntityState> originals)
    {
        bool flag = false;
        foreach(var kvp in originals.Where(e => e.Value.HasFlag(EntityState.Added)))
        {
            var entry = kvp.Key;
            if(entry.Entity is T_Customer)
            {
                var entity = ((GeneratorBase.MVC.Models.T_Customer)(entry.Entity));
                if(entity.T_AutoNo !=  entity.Id)
                {
                    entity.T_AutoNo =  entity.Id;
                    using(var dbforAuto = new ApplicationContext(new SystemUser(), true))
                    {
                        dbforAuto.Configuration.LazyLoadingEnabled = false;
                        dbforAuto.Configuration.AutoDetectChangesEnabled = false;
                        var obj = dbforAuto.T_Customers.FirstOrDefault(p => p.T_AutoNo == entity.T_AutoNo);
                        if(obj != null)
                        {
                            var context = new System.Web.Routing.RequestContext(
                                new HttpContextWrapper(System.Web.HttpContext.Current),
                                new System.Web.Routing.RouteData());
                            var urlHelper = new System.Web.Mvc.UrlHelper(context);
                            var url = urlHelper.Action("ConcurrencyError", "Error", new { UrlReferrer = urlHelper.Action("Index","T_Customer"), Message = "Auto No. is not created and is null for this record because it has already been used elsewhere." });
                            System.Web.HttpContext.Current.Response.Redirect(url);
                            flag = false;
                        }
                        else
                        {
                            entity.DisplayValue = entity.getDisplayValue();
                            this.Entry(entity).State = EntityState.Modified;
                            flag= true;
                        }
                    }
                }
            }
            if(entry.Entity is T_ExportDataConfiguration)
            {
                var entity = ((GeneratorBase.MVC.Models.T_ExportDataConfiguration)(entry.Entity));
                if(entity.T_AutoNo != entity.Id + (1 - 1))
                {
                    entity.T_AutoNo = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is T_ExportDataLog)
            {
                var entity = ((GeneratorBase.MVC.Models.T_ExportDataLog)(entry.Entity));
                if(entity.T_AutoNo != entity.Id + (1 - 1))
                {
                    entity.T_AutoNo = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is T_MenuBar)
            {
                var entity = ((GeneratorBase.MVC.Models.T_MenuBar)(entry.Entity));
                if(entity.T_AutoNo != entity.Id + (1 - 1))
                {
                    entity.T_AutoNo = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is T_MenuItem)
            {
                var entity = ((GeneratorBase.MVC.Models.T_MenuItem)(entry.Entity));
                if(entity.T_AutoNo != entity.Id + (1 - 1))
                {
                    entity.T_AutoNo = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is ApplicationFeedback)
            {
                var entity = ((GeneratorBase.MVC.Models.ApplicationFeedback)(entry.Entity));
                if(entity.CommentId != entity.Id + (1 - 1))
                {
                    entity.CommentId = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is T_DocumentTemplate)
            {
                var entity = ((GeneratorBase.MVC.Models.T_DocumentTemplate)(entry.Entity));
                if(entity.T_AutoNo != entity.Id + (1 - 1))
                {
                    entity.T_AutoNo = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
            if(entry.Entity is FeedbackResource)
            {
                var entity = ((GeneratorBase.MVC.Models.FeedbackResource)(entry.Entity));
                if(entity.ResourceId != entity.Id + (1 - 1))
                {
                    entity.ResourceId = entity.Id + (1 - 1);
                    entity.DisplayValue = entity.getDisplayValue();
                    flag = true;
                }
            }
        }
        if(flag)
            base.SaveChanges();
    }
    /// <summary>Encrypts a value.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void EncryptValue(Dictionary<DbEntityEntry, EntityState> originals, DbPropertyValues OriginalObj)
    {
        bool flag = false;
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
        }
        if(flag)
            base.SaveChanges();
    }
    /// <summary>Check owner permission.</summary>
    ///
    /// <param name="dbEntityEntry"> The database entity entry.</param>
    /// <param name="DataBaseValues">The data base values.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckOwnerPermission(DbEntityEntry dbEntityEntry, DbPropertyValues DataBaseValues)
    {
        var result = false;
        if(dbEntityEntry.State == EntityState.Modified || dbEntityEntry.State == EntityState.Deleted)
        {
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
            if(user.ImposeOwnerPermission(EntityName))
            {
                CheckPermissionForOwner obj = new CheckPermissionForOwner();
                result = obj.CheckOwnerPermission(EntityName, DataBaseValues, user,true);
            }
        }
        return result;
    }
    /// <summary>Check lock condition.</summary>
    ///
    /// <param name="dbEntityEntry"> The database entity entry.</param>
    /// <param name="DataBaseValues">The data base values.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckLockCondition(DbEntityEntry dbEntityEntry, DbPropertyValues DataBaseValues)
    {
        var result = false;
        if(dbEntityEntry.State == EntityState.Modified || dbEntityEntry.State == EntityState.Deleted)
        {
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
            CheckPermissionForOwner obj = new CheckPermissionForOwner();
            result = obj.CheckLockCondition(EntityName, DataBaseValues, user, true);
        }
        return result;
    }
    /// <summary>Check CheckHierarchicalPermissionOnRecord.</summary>
    ///
    /// <param name="EntityName"> The EntityName.</param>
    /// <param name="dbEntityEntry"> The database entity entry.</param>
    /// <param name="DataBaseValues">The data base values.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckHierarchicalPermissionOnRecord(string EntityName,DbEntityEntry dbEntityEntry, DbPropertyValues DataBaseValues)
    {
        var result = true;
        if(dbEntityEntry.State == EntityState.Modified)
        {
            return user.CanEditItemInHierarchy(EntityName, DataBaseValues.ToObject(), user);
        }
        if(dbEntityEntry.State == EntityState.Deleted)
        {
            return user.CanDeleteItemInHierarchy(EntityName, DataBaseValues.ToObject(), user);
        }
        return result;
    }
    /// <summary>Check field level security.</summary>
    ///
    /// <param name="dbEntityEntry"> The database entity entry.</param>
    /// <param name="DataBaseValues">The data base values.</param>
    private void CheckFieldLevelSecurity(DbEntityEntry dbEntityEntry, DbPropertyValues DataBaseValues)
    {
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
        var EntityBR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
        if(dbEntityEntry.State == EntityState.Modified)
        {
            string FLSProperties = user.FLSAppliedOnProperties(EntityName);
            if(!string.IsNullOrEmpty(FLSProperties))
            {
                foreach(string propertyName in FLSProperties.Split(",".ToCharArray()))
                {
                    if(!string.IsNullOrEmpty(propertyName))
                    {
                        var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                        if(propertyInfo != null)
                        {
                            Type targetType = propertyInfo.PropertyType;
                            if(propertyInfo.PropertyType.IsGenericType)
                                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                            if(DataBaseValues == null || !DataBaseValues.PropertyNames.Contains(propertyName)) continue;
                            object propertyValue = DataBaseValues[propertyName];
                            object safeValue = (propertyValue == null) ? null : Convert.ChangeType(propertyValue, targetType);
                            propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                        }
                    }
                }
            }
            //Readonly Properties (Business Rule)
            if(EntityBR != null && EntityBR.Count > 0 && DataBaseValues != null)
            {
                var ResultOfBusinessRules = ReadOnlyPropertiesRule(DataBaseValues.ToObject(), user.businessrules, EntityName);
                var BR = EntityBR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                    foreach(string propertyName in Args.Select(p => p.ParameterName))
                    {
                        var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                        if(propertyInfo == null || propertyInfo.CustomAttributes.Where(wh => wh.AttributeType.Name == "NotMappedAttribute").Count() > 0) continue;
                        object propertyValue = DataBaseValues[propertyName];
                        Type targetType = propertyInfo.PropertyType;
                        if(propertyInfo.PropertyType.IsGenericType)
                            targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                        object safeValue = (propertyValue == null) ? null : Convert.ChangeType(propertyValue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
            }
        }
        var ResultOfBusinessSetValueRules = SetValueRule(dbEntityEntry.Entity, user.businessrules.Where(p => p.EntityName == EntityName).ToList(), EntityName,dbEntityEntry.State);
        if(ResultOfBusinessSetValueRules.Keys.Select(p => p.TypeNo).Contains(7))
        {
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessSetValueRules.Where(p => p.Key.TypeNo == 7).Select(v => v.Value.ActionID).ToList());
            foreach(var property in Args)
            {
                dynamic finalvalue=null;
                dynamic finalvalueDisplay = null;
                bool flagDynamic = false;
                var paramValue = property.ParameterValue;
                var paramValue2 = "";
                if(paramValue.Contains("[") && paramValue.Contains("]"))
                {
                    paramValue = paramValue.Substring(paramValue.IndexOf('['), paramValue.IndexOf(']') + 1);
                    paramValue2 = property.ParameterValue.Substring(paramValue.Length);
                }
                if(paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                {
                    var targetProperty = paramValue;
                    targetProperty = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                    if(targetProperty.Contains("."))
                    {
                        var targetProperties = targetProperty.Split(".".ToCharArray());
                        if(targetProperties.Length>1)
                        {
                            var propInfo = dbEntityEntry.Entity.GetType().GetProperty(targetProperties[0]);
                            var firstprop = propInfo.GetValue(dbEntityEntry.Entity, new object[] { });
                            finalvalueDisplay = firstprop;
                            var AssoInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propInfo.Name);
                            if(AssoInfo != null)
                            {
                                //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssoInfo.Target + "Controller, GeneratorBase.MVC.Controllers");
                                Type controller = new CreateControllerType(AssoInfo.Target).controllerType;
                                using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                                {
                                    MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                                    object[] MethodParams = new object[] { firstprop, targetProperties[1] };
                                    //var firstvalue =(mc.Invoke(objController, MethodParams));
                                    //finalvalue = firstvalue.GetType().GetProperty(targetProperties[1]).GetValue(firstvalue, new object[] { });
                                    finalvalue = (mc.Invoke(objController, MethodParams));
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                    else
                    {
                        var targetpropInfo = dbEntityEntry.Entity.GetType().GetProperty(targetProperty);
                        if(targetpropInfo == null)
                            continue;
                        finalvalue = targetpropInfo.GetValue(dbEntityEntry.Entity, new object[] { });
                    }
                    flagDynamic = true;
                }
                else finalvalue = property.ParameterValue;
                var propertyName = property.ParameterName;
                var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                //set value not working for association
                if(propertyInfo == null)
                {
                    if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
                    {
                        var parameterSplit = propertyName.Substring(1, propertyName.Length - 2).Split('.');
                        string[] inlineAssoList = { };
                        if(inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                            propertyName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                        else
                            propertyName = parameterSplit[0];
                        propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                        if(parameterSplit.Length > 1)
                        {
                            var AssociationInfoVal = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == parameterSplit[0]);
                            if(AssociationInfoVal!=null)
                            {
                                Type type = Type.GetType("GeneratorBase.MVC.Models." + AssociationInfoVal.Target + ", GeneratorBase.MVC.Models");
                                if(parameterSplit[1].ToLower() == "displayvalue")
                                    finalvalue = finalvalueDisplay;
                            }
                        }
                        if(propertyInfo == null) continue;
                    }
                    else continue;
                }
                //
                var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
                if(AssociationInfo != null && !flagDynamic)
                {
                    //
                    var parmName = property.ParameterName;
                    object parmValue = property.ParameterValue;
                    string propname = "";
                    string propnameAsso = "";
                    long id = 0;
                    if(parmName.StartsWith("[") && parmName.EndsWith("]"))
                    {
                        var parameterSplit = parmName.Substring(1, parmName.Length - 2).Split('.'); ;
                        propname = parameterSplit[1];
                        propnameAsso = parameterSplit[0];
                        if(DataBaseValues != null)
                            id = Convert.ToInt64(DataBaseValues[propnameAsso]);
                    }
                    //
                    //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller, GeneratorBase.MVC.Controllers");
                    Type controller = new CreateControllerType(AssociationInfo.Target).controllerType;
                    using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                    {
                        if(!(string.IsNullOrEmpty(propname) && string.IsNullOrEmpty(propnameAsso)) && parmName.StartsWith("[") && parmName.EndsWith("]"))
                        {
                            System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromPropertyValue");
                            object[] MethodParams = new object[] { propname, parmValue.ToString() };
                            var obj = mc.Invoke(objController, MethodParams);
                            object PropValue = obj;
                            propertyInfo.SetValue(dbEntityEntry.Entity, PropValue, null);
                        }
                        else if(!(string.IsNullOrEmpty(propname) && string.IsNullOrEmpty(propnameAsso)))
                        {
                            System.Reflection.MethodInfo mc = controller.GetMethod("SetPropertyValueByEntityId");
                            object[] MethodParams = new object[] { id, propname, parmValue.ToString() };
                            var obj = mc.Invoke(objController, MethodParams);
                        }
                        //#21824 - Set value does not work on second time edit
                        //if (!(string.IsNullOrEmpty(propname) && string.IsNullOrEmpty(propnameAsso)) && id > 0)
                        //{
                        //    System.Reflection.MethodInfo mc = controller.GetMethod("SetPropertyValueByEntityId");
                        //    object[] MethodParams = new object[] { id, propname, parmValue.ToString() };
                        //    var obj = mc.Invoke(objController, MethodParams);
                        //  }
                        //else if (!(string.IsNullOrEmpty(propname) && string.IsNullOrEmpty(propnameAsso)) && id == 0 && parmName.StartsWith("[") && parmName.EndsWith("]"))
                        // {
                        //     System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromPropertyValue");
                        //    object[] MethodParams = new object[] { propname, parmValue.ToString() };
                        //    var obj = mc.Invoke(objController, MethodParams);
                        //   object PropValue = obj;
                        //     propertyInfo.SetValue(dbEntityEntry.Entity, PropValue, null);
                        // }
                        else
                        {
                            System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromDisplayValue");
                            object[] MethodParams = new object[] { property.ParameterValue };
                            var obj = mc.Invoke(objController, MethodParams);
                            object PropValue = obj;
                            propertyInfo.SetValue(dbEntityEntry.Entity, PropValue, null);
                        }
                    }
                }
                else
                {
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    if(targetType.Name == "DateTime")  //changes to set value as today(current date time)
                    {
                        if(flagDynamic)
                        {
                            if(finalvalue != null)
                                finalvalue = ApplyRule.EvaluateDateTime(Convert.ToString(finalvalue), paramValue2);
                        }
                        else
                        {
                            Type type = Type.GetType("GeneratorBase.MVC.Models." + EntityName);
                            var displayformatattributes = type.GetProperty(propertyName) == null ? null : type.GetProperty(propertyName).CustomAttributes.Where(a => a.ConstructorArguments.Count() > 0);
                            if(displayformatattributes.Count() > 0)
                            {
                                var displayformatArgument = displayformatattributes.FirstOrDefault(p => p.AttributeType.Name == "CustomDisplayFormat").ConstructorArguments;
                                var formatstring = Convert.ToString(displayformatArgument[2].Value);
                                formatstring = formatstring.Substring((formatstring.LastIndexOf("0")) + 2, formatstring.Length - 4);
                                finalvalue = ApplyRule.EvaluateDateForActionInTarget(finalvalue, formatstring, true, true);
                            }
                            else
                                finalvalue = ApplyRule.EvaluateDateForActionInTarget(finalvalue);
                        }
                    }
                    object safeValue = (finalvalue == null || Convert.ToString(finalvalue) == "null") ? null : Convert.ChangeType(finalvalue, targetType);
                    propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                }
            }
        }
    }
    /// <summary>Is external entity.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>Returns true if external web-api entity else returns false</returns>
    private bool IsExternalEntity(string EntityName)
    {
        return false;
    }
    /// <summary>Is external entity.</summary>
    ///
    /// <param name="entity">    The entity.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="state">     The state.</param>
    ///
    /// <returns>Returns true if external web-api entity else returns false</returns>
    private async System.Threading.Tasks.Task<bool> IsExternalEntity(object entity, string EntityName, EntityState state)
    {
        return false;
    }
    /// <summary>Journal entry for update actions.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="db">           The database.</param>
    /// <param name="OriginalObj">  The original object.</param>
    private void MakeUpdateJournalEntry(DbEntityEntry dbEntityEntry, JournalEntryContext db, DbPropertyValues OriginalObj)
    {
        if(dbEntityEntry.State != EntityState.Modified) return;
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
        if(EntityName == "T_Customer" || EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart" || EntityName == "T_Schedule" || EntityName == "Document" || EntityName == "T_ExportDataConfiguration" || EntityName == "T_ExportDataDetails" || EntityName == "T_ExportDataLog")
        {
            var CurrentObj =dbEntityEntry.CurrentValues;
            string id = Convert.ToString(CurrentObj.GetValue<object>("Id"));
            string dispValue = Convert.ToString(CurrentObj.GetValue<object>("DisplayValue"));
            var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            foreach(var property in dbEntityEntry.OriginalValues.PropertyNames.Where(wh => dbEntityEntry.Property(wh).IsModified))
            {
                if(property == "DisplayValue"  || property == "TenantId" || property == "ConcurrencyKey" || (EntityName == "Document" && (property == "Byte" || property == "DocumentName" || property == "MIMEType" || property == "DateCreated" || property == "DateLastUpdated" || property == "FileExtension"))) continue;
                var Encryptedpropertyname = entityModel.Properties.FirstOrDefault(p => p.Name == property);
                var original = OriginalObj.GetValue<object>(property);
                var current = CurrentObj.GetValue<object>(property);
                if(Encryptedpropertyname != null && Encryptedpropertyname.PropCheck == "Encrypted")
                {
                    EncryptDecrypt ed = new EncryptDecrypt();
                    original = ed.DecryptString(original);
                }
                if(original != current && (original == null || !original.Equals(current)))
                {
                    JournalEntry Je = new JournalEntry();
                    Je.DateTimeOfEntry = DateTime.UtcNow;
                    Je.EntityName = EntityName;
                    Je.UserName =journaluser !=null ? journaluser.Name : user.Name;
                    Je.RoleName = journaluser !=null ? string.Join(",", journaluser.userroles): string.Join(",", user.userroles);
                    Je.Type = dbEntityEntry.State.ToString();
                    var displayValue = dispValue;
                    Je.RecordInfo = displayValue;
                    Je.BrowserInfo = DoAuditEntry.GetBrowserDetails();
                    Je.PropertyName = property;
                    Je.Source = this.JournalSource;
                    var assolist = entityModel.Associations.Where(p => p.AssociationProperty == property).ToList();
                    if(Encryptedpropertyname != null && (Encryptedpropertyname.PropCheck == "Encrypted" || Encryptedpropertyname.PropCheck == "Password"))
                    {
                        Je.OldValue = "";
                        Je.NewValue = "Value has been changed";
                    }
                    else
                    {
                        if(assolist.Count() > 0)
                        {
                            Je.PropertyName = assolist[0].DisplayName;
                            if(original != null)
                                Je.OldValue = EntityComparer.GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(original));
                            if(current != null)
                                Je.NewValue = EntityComparer.GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(current));
                        }
                        else
                        {
                            Je.OldValue = Convert.ToString(original);
                            Je.NewValue = Convert.ToString(current);
                        }
                    }
                    Je.RecordId = Convert.ToInt64(id);
                    db.JournalEntries.Add(Je);
                    //db.SaveChanges(); //do not commit here, commited jedb after base.SaveChanges successfully completed.
                }
            }
        }
    }
    /// <summary>Check permission on entity (Role based access control).</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="state">     The state.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckPermissionOnEntity(string EntityName, EntityState state)
    {
        if(!user.CanAdd(EntityName) && state == EntityState.Added)
            return false;
        if(!user.CanEdit(EntityName) && state == EntityState.Modified)
            return false;
        if(!user.CanDelete(EntityName) && state == EntityState.Deleted)
            return false;
        return true;
    }
    /// <summary>Sets display value.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    private void SetDisplayValue(DbEntityEntry dbEntityEntry)
    {
        Type EntityType = dbEntityEntry.Entity.GetType();
        dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
        EntityObj.DisplayValue = EntityObj.getDisplayValue();
    }
    /// <summary>Sets date time to UTC.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    private void SetDateTimeToUTC(DbEntityEntry dbEntityEntry)//mahesh
    {
        Type EntityType = dbEntityEntry.Entity.GetType();
        dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
        try
        {
            EntityObj.setDateTimeToUTC();
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets calculated value.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    private void SetCalculatedValue(DbEntityEntry dbEntityEntry)
    {
        Type EntityType = dbEntityEntry.Entity.GetType();
        dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
        try
        {
            EntityObj.setCalculation();
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Apply logic after save.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void AfterSave(Dictionary<DbEntityEntry, EntityState> originals)
    {
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            //dynamic EntityObj = Convert.ChangeType(entry.Entity, entityType);
            try
            {
                //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
                Type controller = new CreateControllerType(EntityName).controllerType;
                if(controller!=null)
                {
                    using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                    {
                        System.Reflection.MethodInfo mc = controller.GetMethod("AfterSave");
                        object[] MethodParams = new object[] { entry.Entity, user, kvp.Value };
                        if(mc != null)
                            mc.Invoke(objController, MethodParams);
                    }
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
    
    /// <summary>Apply logic after delete.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void AfterDelete(Dictionary<DbEntityEntry, EntityState> originals, DbPropertyValues OriginalObj)
    {
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            var EntityObj = OriginalObj.ToObject();
            try
            {
                Type controller = new CreateControllerType(EntityName).controllerType;
                if(controller!=null)
                {
                    using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                    {
                        System.Reflection.MethodInfo mc = controller.GetMethod("AfterDelete");
                        object[] MethodParams = new object[] { EntityObj, user, kvp.Value };
                        if(mc != null)
                            mc.Invoke(objController, MethodParams);
                    }
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
    
    
    /// <summary>Ordered list (update property marked for entity ordering by traversing whole list).</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="OriginalObj">  The original object.</param>
    private void OrderedListCheck(DbEntityEntry dbEntityEntry, DbPropertyValues OriginalObj)
    {
        if(dbEntityEntry.State == EntityState.Deleted) return;
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
    }
    /// <summary>Assign one to many history on update.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="OriginalObj">  The original object.</param>
    private void AssignOneToManyCurrentOnUpdate(DbEntityEntry dbEntityEntry, DbPropertyValues OriginalObj)
    {
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
    }
    /// <summary>Assign one to many history on add.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void AssignOneToManyCurrentOnAdd(Dictionary<DbEntityEntry, EntityState> originals)
    {
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
        }
    }
    /// <summary>Journal entry for add actions.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void MakeAddJournalEntry(Dictionary<DbEntityEntry, EntityState> originals)
    {
        using(JournalEntryContext db = new JournalEntryContext())
        {
            foreach(var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
                if(EntityName == "T_Customer" || EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart" || EntityName == "T_Schedule" || EntityName == "Document" || EntityName == "T_ExportDataConfiguration" || EntityName == "T_ExportDataDetails" || EntityName == "T_ExportDataLog")
                {
                    if(kvp.Value.HasFlag(EntityState.Added))
                    {
                        var entity = (IEntity)entry.Entity;
                        var id =  entry.CurrentValues.GetValue<object>("Id");//entry.OriginalValues["Id"];
                        JournalEntry Je = new JournalEntry();
                        Je.DateTimeOfEntry = DateTime.UtcNow;
                        Je.EntityName = EntityName;
                        Je.UserName = journaluser !=null ? journaluser.Name : user.Name;
                        Je.RoleName = journaluser !=null ? string.Join(",", journaluser.userroles) : string.Join(",", user.userroles);
                        Je.Type = "Added";
                        Je.Source = this.JournalSource;
                        Je.RecordId = Convert.ToInt64(id);
                        Type EntityType = entry.Entity.GetType();
                        dynamic EntityObj = Convert.ChangeType(entry.Entity, EntityType);
                        var displayValue = EntityObj.DisplayValue;
                        Je.RecordInfo = displayValue;
                        Je.BrowserInfo = DoAuditEntry.GetBrowserDetails();
                        db.JournalEntries.Add(Je);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
    /// <summary>Journal entry for delete actions.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="db">           The database.</param>
    /// <param name="CurrentObj">   The current object.</param>
    private void MakeDeleteJournalEntry(DbEntityEntry dbEntityEntry, JournalEntryContext db, DbPropertyValues CurrentObj)
    {
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
        if(EntityName == "T_Customer" || EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart" || EntityName == "T_Schedule" || EntityName == "Document" || EntityName == "T_ExportDataConfiguration" || EntityName == "T_ExportDataDetails" || EntityName == "T_ExportDataLog")
        {
            if(CurrentObj == null) return;
            string id = Convert.ToString(CurrentObj.GetValue<object>("Id"));
            string dispValue = Convert.ToString(CurrentObj.GetValue<object>("DisplayValue"));
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.UtcNow;
            Je.EntityName = EntityName;
            Je.UserName = journaluser !=null ? journaluser.Name : user.Name;
            Je.RoleName = journaluser !=null ? string.Join(",", journaluser.userroles) : string.Join(",", user.userroles);
            Je.Type = "Deleted";
            Je.Source = this.JournalSource;
            Je.RecordId = Convert.ToInt64(id);
            Je.RecordInfo = dispValue;
            Je.BrowserInfo = DoAuditEntry.GetBrowserDetails();
            db.JournalEntries.Add(Je);
            //db.SaveChanges();
        }
    }
    /// <summary>Violating business rules (check before save).</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ViolatingBusinessRules(DbEntityEntry dbEntityEntry)
    {
        var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
        var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
        if(BR.Count > 0 && dbEntityEntry.State == EntityState.Modified)
        {
            var CurrentObj = dbEntityEntry.CurrentValues;
            var id = Convert.ToString((CurrentObj.GetValue<object>("Id")));
            if(id != null && Convert.ToInt64(id) > 0)
            {
                var ResultOfBusinessRules = LockEntityRule(dbEntityEntry.Entity, BR, dbEntityEntry.Entity.GetType().Name);
                if(ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
                    return true;
            }
        }
        if(BR.Count > 0)
        {
            if(dbEntityEntry.State != EntityState.Deleted)
            {
                var ResultOfBusinessRules = ValidateBeforeSavePropertiesRule(dbEntityEntry.Entity, BR, EntityName,dbEntityEntry.State);
                if(ResultOfBusinessRules != null && (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(10)))
                    return true;
            }
        }
        return false;
    }
    /// <summary>Lock business rule on entity.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> LockEntityRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 1 || p.associatedactiontype.TypeNo == 11));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    obj.BRTID = br.AssociatedBusinessRuleTypeID;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Business rule before save on properties.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    /// <param name="state"> The state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ValidateBeforeSavePropertiesRule(object entity, List<BusinessRule> BR, string name, EntityState state)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(state == EntityState.Modified && br.AssociatedBusinessRuleTypeID == 1)
                    continue;
                if(state == EntityState.Added && br.AssociatedBusinessRuleTypeID == 2)
                    continue;
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 10));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype;//atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Business rule before save on properties.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    /// <param name="state"> The state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ValidateBeforeSavePropertiesRuleConfirmPop(object entity, List<BusinessRule> BR, string name, EntityState state)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(state == EntityState.Modified && br.AssociatedBusinessRuleTypeID == 1)
                    continue;
                if(state == EntityState.Added && br.AssociatedBusinessRuleTypeID == 2)
                    continue;
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 15));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>UI Alert business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> UIAlertRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 13);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    //var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeobj = new ActionType();
                    EntityCopy.CopyValuesForSameObjectType1(act.associatedactiontype, typeobj);
                    typeobj.BRid = br.Id;
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Hidden Verb business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> GetHiddenVerb(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 16);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    
    /// <summary>Mandatory properties business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> MandatoryPropertiesRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 2);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Reads only properties business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ReadOnlyPropertiesRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    obj.BRTID = br.AssociatedBusinessRuleTypeID;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    
    /// <summary>Hidden Properties business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="IsEdit">      The IsEdit.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> HiddenPropertiesRule(object entity, List<BusinessRule> BR, string name, bool IsEdit = false)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 6);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    
    /// <summary>Hidden Groups business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="IsEdit">      The IsEdit.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> HiddenGroupsRule(object entity, List<BusinessRule> BR, string name, bool IsEdit = false)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 12);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    
    /// <summary>Set value business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="currentState">The current state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> SetValueRule(object entity, List<BusinessRule> BR, string name, EntityState currentState)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(currentState) != "Added")
                    continue;
                else if((br.AssociatedBusinessRuleTypeID == 5 || br.AssociatedBusinessRuleTypeID == 2) && Convert.ToString(currentState) != "Modified")
                    continue;
                else if(br.AssociatedBusinessRuleTypeID == 6 || br.AssociatedBusinessRuleTypeID == 7 || br.AssociatedBusinessRuleTypeID == 8)
                    continue;
                var ruleactions7 = br.ruleaction.Where(p=>p.associatedactiontype.TypeNo == 7);
                if(ruleactions7.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = ruleactions7.Where(p =>!p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = ruleactions7.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Executes the web hook on a different thread, and waits for the result (external web-api).</summary>
    ///
    /// <param name="dbentityentry">The dbentityentry.</param>
    ///
    /// <returns>A System.Threading.Tasks.Task.</returns>
    public async System.Threading.Tasks.Task InvokeWebHook(DbEntityEntry dbentityentry)
    {
        var entry = dbentityentry;
        var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
        var EntityName = entityType.Name;
        var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
        if(BR != null && BR.Count() > 0)
        {
            bool addflag = dbentityentry.State.HasFlag(EntityState.Added);
            //var ruleactiondb = new RuleActionContext();
            //var ruleconditiondb = new ConditionContext();
            foreach(var br in BR)
            {
                if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(dbentityentry.State) != "Added")
                    continue;
                else if(br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(dbentityentry.State) != "Modified")
                    continue;
                foreach(var act in br.ruleaction.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 14))
                {
                    var condition = br.ruleconditions;
                    ActionArgsContext actionargs = new ActionArgsContext();
                    var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                    var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                    if(!ConditionResult)
                        continue;
                    var datasource = argslist.FirstOrDefault();
                    if(datasource != null)
                    {
                        var EntityObj = Convert.ChangeType(entry.Entity, entry.Entity.GetType());
                        await CallWebHook(EntityObj, EntityName, EntityState.Added, entityType, Convert.ToInt64(datasource.ParameterName));
                    }
                }
            }
        }
    }
    /// <summary>Call web hook (external web-api).</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="EntityName">  Name of the entity.</param>
    /// <param name="state">       The state.</param>
    /// <param name="entityType">  Type of the entity.</param>
    /// <param name="dataSourceId">Identifier for the data source.</param>
    ///
    /// <returns>A System.Threading.Tasks.Task<bool></returns>
    private async System.Threading.Tasks.Task<bool> CallWebHook(object entity, string EntityName, EntityState state, Type entityType, long dataSourceId)
    {
        await ExternalService.ExecuteAPIPostRequest(EntityName, entity, null, dataSourceId);
        return true;
    }
    /// <summary>Executes the action rule on a different thread, and waits for the result (business rule invoke custom action on condition).</summary>
    ///
    /// <param name="originals">The originals.</param>
    public void InvokeActionRule(Dictionary<DbEntityEntry, EntityState> originals)
    {
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
            if(BR != null && BR.Count() > 0)
            {
                bool addflag = kvp.Value.HasFlag(EntityState.Added);
                //var ruleactiondb = new RuleActionContext();
                //var ruleconditiondb = new ConditionContext();
                foreach(var br in BR)
                {
                    if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(kvp.Value) != "Added")
                        continue;
                    else if(br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(kvp.Value) != "Modified")
                        continue;
                    foreach(var act in br.ruleaction.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 8)) //&& p.associatedactiontype.TypeNo == 3))
                    {
                        var condition = br.ruleconditions;
                        ActionArgsContext actionargs = new ActionArgsContext();
                        var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                        var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                        if(!ConditionResult)
                            argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                        foreach(var args in argslist)
                        {
                            var arguments = args.ParameterValue.Split(".".ToCharArray());
                            if(arguments.Length == 2)
                            {
                                var propInfo = entry.Entity.GetType().GetProperty(args.ParameterName);
                                var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                InvokeAction(arguments[0], arguments[1], Convert.ToInt32(propvalue), user, this);
                            }
                            else if(arguments[0].Contains("CopyTo"))
                            {
                                var propInfo = entry.Entity.GetType().GetProperty("Id");
                                var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                InvokeCopyAction(EntityName, arguments[0], Convert.ToInt32(propvalue), user, this);
                            }
                            else
                            {
                                var propInfo = entry.Entity.GetType().GetProperty("Id");
                                var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                InvokeAction(EntityName, arguments[0], Convert.ToInt32(propvalue), user, this);
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>Time based alert (Scheduled business rule).</summary>
    ///
    /// <param name="originals">The original entities.</param>
    public void TimeBasedAlert(Dictionary<DbEntityEntry, EntityState> originals)
    {
        foreach(var kvp in originals)
        {
            var entry = kvp.Key;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
            if(BR != null && BR.Count() > 0)
            {
                bool addflag = kvp.Value.HasFlag(EntityState.Added);
                //var ruleactiondb = new RuleActionContext();
                //var ruleconditiondb = new ConditionContext();
                var alertMessage = "";
                string days = "";
                string NotifyTo = "";
                string NotifyToRole = "";
                long actionid = 0;
                foreach(var br in BR)
                {
                    if(br.AssociatedBusinessRuleTypeID == 4 && Convert.ToString(kvp.Value) == "Added")  //on property change work on edit only
                        continue;
                    if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(kvp.Value) != "Added")
                        continue;
                    else if(br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(kvp.Value) != "Modified")
                        continue;
                    foreach(var act in br.ruleaction.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 3)) //&& p.associatedactiontype.TypeNo == 3))
                    {
                        var condition = br.ruleconditions.Where(p => p.RuleConditionsID == br.Id);
                        ActionArgsContext actionargs = new ActionArgsContext();
                        var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                        var notifyon = "Add,Update";
                        var notifyonParam = argslist.FirstOrDefault(p => p.ParameterName == "NotifyOn");
                        if(notifyonParam != null)
                            notifyon = notifyonParam.ParameterValue;
                        if((addflag && notifyon.Contains("Add")) || (!addflag && notifyon.Contains("Update")))
                            if(condition.Count() > 0)
                            {
                                var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                                if(!ConditionResult)
                                    argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                                alertMessage += act.ErrorMessage;
                                actionid = act.Id;
                                foreach(var args in argslist)
                                {
                                    if(args.ParameterName == "TimeValue")
                                        days = args.ParameterValue;
                                    if(args.ParameterName == "NotifyTo")
                                        NotifyTo = args.ParameterValue;
                                    if(args.ParameterName == "NotifyToRole")
                                    {
                                        NotifyToRole = args.ParameterValue;
                                    }
                                }
                            }
                            else
                            {
                                alertMessage += act.ErrorMessage;
                                actionid = act.Id;
                                foreach(var args in argslist)
                                {
                                    if(args.ParameterName == "TimeValue")
                                        days = args.ParameterValue;
                                    if(args.ParameterName == "NotifyTo")
                                        NotifyTo = args.ParameterValue;
                                    if(args.ParameterName == "NotifyToRole")
                                    {
                                        NotifyToRole = args.ParameterValue;
                                    }
                                }
                            }
                        if(argslist.Count() > 0 && !string.IsNullOrEmpty(days))
                        {
                            DateTimeOffset callbackTime;
                            if(Convert.ToInt32(days) == 0)
                            {
                                SendEmail obj = new SendEmail();
                                obj.InstantNotification(EntityName, Convert.ToInt64(entry.OriginalValues["Id"]), actionid, user.Name,user,entry.Entity);
                            }
                            else
                            {
                                callbackTime = DateTimeOffset.Now.AddDays(Convert.ToDouble(days));
                                Uri callbackUrl = new Uri(string.Format("http://localhost//TestNewTuranto74v4//TimeBasedAlert//NotifyOneTime?EntityName=" + EntityName + "&ID=" + entry.OriginalValues["Id"] + "&actionid=" + actionid + "&userName=" + user.Name+"&entityobj="));
                                Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);
                            }
                        }
                    }//
                }
            }
        }
    }
    /// <summary>Time based alert business rule.</summary>
    ///
    /// <param name="originals"> The original entities.</param>
    /// <param name="IsOnSaving">(Optional) True if is on saving, false if not.</param>
    public void TimeBasedAlert(DbEntityEntry originals, bool IsOnSaving= false)
    {
        var entityType = ObjectContext.GetObjectType(originals.Entity.GetType());
        var EntityName = entityType.Name;
        var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
        string id = "";
        if(originals.State != EntityState.Added)
            id = Convert.ToString(originals.OriginalValues["Id"]);
        else
            id = Convert.ToString(originals.CurrentValues["Id"]);
        if(string.IsNullOrEmpty(id) || Convert.ToInt64(id) == 0) return;
        if(BR != null && BR.Count() > 0)
        {
            bool addflag = originals.State.HasFlag(EntityState.Added);
            //var ruleactiondb = new RuleActionContext();
            //var ruleconditiondb = new ConditionContext();
            var alertMessage = "";
            string days = "";
            string NotifyTo = "";
            string NotifyToRole = "";
            long actionid = 0;
            bool IsWebNotification = false;
            bool IsEmailNotification = true;
            foreach(var br in BR)
            {
                if(br.AssociatedBusinessRuleTypeID != 4 && IsOnSaving) continue;
                if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(originals.State) != "Added")
                    continue;
                else if(br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(originals.State) != "Modified")
                    continue;
                foreach(var act in br.ruleaction.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 3)) //&& p.associatedactiontype.TypeNo == 3))
                {
                    var condition = br.ruleconditions.Where(p => p.RuleConditionsID == br.Id);
                    ActionArgsContext actionargs = new ActionArgsContext();
                    var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                    var notifyon = "Add,Update";
                    var notifyonParam = argslist.FirstOrDefault(p => p.ParameterName == "NotifyOn");
                    if(notifyonParam != null)
                        notifyon = notifyonParam.ParameterValue;
                    if((addflag && notifyon.Contains("Add")) || (!addflag && notifyon.Contains("Update")))
                        if(condition.Count() > 0)
                        {
                            var ConditionResult = ApplyRule.CheckRule<object>(originals.Entity, br, EntityName);
                            if(!ConditionResult)
                                argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                            alertMessage += act.ErrorMessage;
                            actionid = act.Id;
                            foreach(var args in argslist)
                            {
                                if(args.ParameterName == "TimeValue")
                                    days = args.ParameterValue;
                                if(args.ParameterName == "NotifyTo")
                                    NotifyTo = args.ParameterValue;
                                if(args.ParameterName == "NotifyToRole")
                                {
                                    NotifyToRole = args.ParameterValue;
                                }
                                if(args.ParameterName == "IsWebNotification")
                                    IsWebNotification = Convert.ToBoolean(Convert.ToInt16(args.ParameterValue));
                                if(args.ParameterName == "IsEmailNotification")
                                    IsEmailNotification = Convert.ToBoolean(Convert.ToInt16(args.ParameterValue));
                            }
                        }
                        else
                        {
                            alertMessage += act.ErrorMessage;
                            actionid = act.Id;
                            foreach(var args in argslist)
                            {
                                if(args.ParameterName == "TimeValue")
                                    days = args.ParameterValue;
                                if(args.ParameterName == "NotifyTo")
                                    NotifyTo = args.ParameterValue;
                                if(args.ParameterName == "NotifyToRole")
                                {
                                    NotifyToRole = args.ParameterValue;
                                }
                                if(args.ParameterName == "IsWebNotification")
                                    IsWebNotification = Convert.ToBoolean(Convert.ToInt16(args.ParameterValue));
                                if(args.ParameterName == "IsEmailNotification")
                                    IsEmailNotification = Convert.ToBoolean(Convert.ToInt16(args.ParameterValue));
                            }
                        }
                    if(argslist.Count() > 0 && !string.IsNullOrEmpty(days))
                    {
                        DateTimeOffset callbackTime;
                        if(Convert.ToInt32(days) == 0)
                        {
                            SendEmail obj = new SendEmail();
                            obj.InstantNotification(EntityName, Convert.ToInt64(id), actionid, user.Name,user,originals.Entity);
                        }
                        else
                        {
                            callbackTime = DateTimeOffset.Now.AddDays(Convert.ToDouble(days));
                            Uri callbackUrl = new Uri(string.Format("http://localhost//TestNewTuranto74v4//TimeBasedAlert//NotifyOneTime?EntityName=" + EntityName + "&ID=" + id + "&actionid=" + actionid + "&userName=" + user.Name));
                            Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);
                        }
                        //added notification from br
                        if(IsWebNotification)
                        {
                            if(Convert.ToInt32(days) == 0)
                            {
                                SendEmail obj = new SendEmail();
                                obj.InstantWebNotification(EntityName, Convert.ToInt64(originals.OriginalValues["Id"]), actionid, user.Name, originals.Entity);
                                //obj.InstantWebNotification(Convert.ToInt64(entry.OriginalValues["Id"]), actionid, user.Name);
                            }
                            else
                            {
                                //Type controller = Type.GetType("GeneratorBase.MVC.Controllers.NotificationController, GeneratorBase.MVC.Controllers");
                                Type controller = new CreateControllerType("Notification").controllerType;
                                if(controller != null)
                                {
                                    callbackTime = DateTimeOffset.Now.AddDays(Convert.ToDouble(days));
                                    Uri callbackUrl = new Uri(string.Format("http://localhost//NitinVRTGlobalHR_v2810//Notification//CreateNotificationBR??ID=" + originals.OriginalValues["Id"] + "&actionid=" + actionid + "&userName=" + user.Name + "&EntityName=" + EntityName + "&entityobj="));
                                    Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);
                                }
                            }
                        }
                    }
                }//
            }
        }
    }
    /// <summary>Check before save (calls beforesave code hook).</summary>
    ///
    /// <param name="entity">    The entity.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A string (error message or default empty).</returns>
    private string CheckBeforeSave(object entity, string EntityName)
    {
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("CheckBeforeSave");
                object[] MethodParams = new object[] { entity, "" };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToString(obj);
            }
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Executes the action on a different thread, and waits for the result (business rule invoke custom action).</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="VerbName">  Name of the verb.</param>
    /// <param name="Param">     The parameter.</param>
    /// <param name="user">      The user.</param>
    /// <param name="db">        The database.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool InvokeAction(string EntityName, string VerbName, int? Param, IUser user, ApplicationContext db)
    {
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod(VerbName, BindingFlags.Public | BindingFlags.Instance);
                // object[] MethodParams = new object[] { Param, user, db, true };
                object[] MethodParams = new object[] { Param };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToBoolean(obj);
            }
        }
        catch
        {
            return true;
        }
    }
    /// <summary>Executes the copy action on a different thread, and waits for the result (invoke copy action business rule).</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="VerbName">  Name of the verb.</param>
    /// <param name="Param">     The parameter.</param>
    /// <param name="user">      The user.</param>
    /// <param name="db">        The database.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool InvokeCopyAction(string EntityName, string VerbName, int? Param, IUser user, ApplicationContext db)
    {
        string sourceId = Convert.ToString(Param);
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod(VerbName, BindingFlags.Public | BindingFlags.Instance);
                object[] MethodParams = new object[] { sourceId, "", null, "", "", "",db, "" };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToBoolean(obj);
            }
        }
        catch
        {
            return true;
        }
    }
    /// <summary>Executes the on saving custom code hoook.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="db">           The database.</param>
    private void OnSavingCustom(DbEntityEntry dbEntityEntry, ApplicationContext db)
    {
        try
        {
            var EntityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = EntityType.Name;
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, dbEntityEntry.Entity.GetType());
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("OnSaving");
                object[] MethodParams = new object[] { EntityObj,db,user };
                mc.Invoke(objController, MethodParams);
            }
        }
        catch
        {
            return;
        }
    }
    /// <summary>Check before delete custom code hoook.</summary>
    ///
    /// <param name="entity">    The entity.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckBeforeDelete(object entity, string EntityName)
    {
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("CheckBeforeDelete");
                object[] MethodParams = new object[] { entity };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToBoolean(obj);
            }
        }
        catch
        {
            return true;
        }
    }
    /// <summary>Executes the deleting custom code hook.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    /// <param name="CurrentObj">   The current object.</param>
    /// <param name="db">           The database.</param>
    private void OnDeletingCustom(DbEntityEntry dbEntityEntry, DbPropertyValues CurrentObj, ApplicationContext db)
    {
        try
        {
            var EntityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = EntityType.Name;
            var EntityObj = CurrentObj.ToObject();
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("OnDeleting");
                object[] MethodParams;
                if(mc.GetParameters().Count() > 2)
                    MethodParams = new object[] { EntityObj, db, user };
                else
                    MethodParams = new object[] { EntityObj, db };
                mc.Invoke(objController, MethodParams);
            }
        }
        catch
        {
            return;
        }
    }
    /// <summary>Applies the security filters (basic, tenant and user based security).</summary>
    ///
    /// <param name="filters">The filters.</param>
    public void ApplyFilters(IList<IFilter<ApplicationContext>> filters)
    {
        foreach(var filter in filters)
        {
            filter.DbContext = this;
            if(user != null && (!string.IsNullOrEmpty(user.Name)))
            {
                filter.ApplyBasicSecurity();
                filter.ApplyMainSecurity();
                filter.ApplyHierarchicalSecurity();
                filter.ApplyUserBasedSecurity();
                filter.CustomFilter();
            }
        }
    }
    /// <summary>This method is called when the model for a derived context has been initialized, but
    /// before the model has been locked down and used to initialize the context.  The default
    /// implementation of this method does nothing, but it can be overridden in a derived class such
    /// that the model can be further configured before it is locked down.</summary>
    ///
    /// <remarks>Typically, this method is called only once when the first instance of a derived
    /// context is created.  The model for that context is then cached and is for all further
    /// instances of the context in the app domain.  This caching can be disabled by setting the
    /// ModelCaching property on the given ModelBuidler, but note that this can seriously degrade
    /// performance. More control over caching is provided through use of the DbModelBuilder and
    /// DbContextFactory classes directly.</remarks>
    ///
    /// <param name="modelBuilder"> The builder that defines the model for the context being created.</param>
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<IdentityUser>()
                   .ToTable("AspNetUsers");
        user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
        user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
        user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
        user.Property(u => u.UserName).IsRequired();
        modelBuilder.Entity<IdentityUserRole>()
        .HasKey(r => new
        {
            r.UserId, r.RoleId
        })
        .ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserLogin>()
        .HasKey(l => new
        {
            l.UserId, l.LoginProvider, l.ProviderKey
        })
        .ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityUserClaim>()
        .ToTable("AspNetUserClaims");
        var role = modelBuilder.Entity<IdentityRole>()
                   .ToTable("AspNetRoles");
        role.Property(r => r.Name).IsRequired();
        role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        modelBuilder.Entity<FooterSection>().HasOptional<CompanyInformation>(p => p.companyinformationfootersectionassociation).WithMany(s => s.companyinformationfootersectionassociation).HasForeignKey(f => f.CompanyInformationFooterSectionAssociationID).WillCascadeOnDelete(false);
        modelBuilder.Entity<T_MenuItem>().HasOptional<T_MenuItem>(p => p.t_menuitemmenuitemassociation).WithMany(s => s.Self_t_menuitemmenuitemassociation).HasForeignKey(f => f.T_MenuItemMenuItemAssociationID).WillCascadeOnDelete(false);
        modelBuilder.Entity<T_DataMetric>().HasOptional<T_DataMetrictype>(p => p.t_associateddatametrictype).WithMany(s => s.t_associateddatametrictype).HasForeignKey(f => f.T_AssociatedDataMetricTypeID).WillCascadeOnDelete(false);
        modelBuilder.Entity<T_DataMetric>().HasOptional<T_FacetedSearch>(p => p.t_associatedfacetedsearch).WithMany(s => s.t_associatedfacetedsearch).HasForeignKey(f => f.T_AssociatedFacetedSearchID).WillCascadeOnDelete(false);
        modelBuilder.Entity<VerbsName>().HasOptional<VerbGroup>(p => p.verbnameselect).WithMany(s => s.verbnameselect).HasForeignKey(f => f.VerbNameSelectID).WillCascadeOnDelete(false);
    }
}
public enum PermissionFreeContext
{
    Home,
    Admin,
    NearByLocations,
    ResourceLocalization,
    Chart,
    ApiHelp,
    
    BusinessRule,
    CompanyInformation,
    CompanyInformationCompanyListAssociation,
    FooterSection,
    PropertyHelpPage
}
}
/// <summary>Register scheduled task.</summary>
public class RegisterScheduledTask
{
    /// <summary>Gets the next run time of task.</summary>
    ///
    /// <param name="br">The business rule.</param>
    ///
    /// <returns>The next run time of task.</returns>
    public DateTime getNextRunTimeOfTask(BusinessRule br)
    {
        GeneratorBase.MVC.ApplicationContext db1 = new GeneratorBase.MVC.ApplicationContext(new InternalUser());
        var result = DateTime.MinValue;
        var task = db1.T_Schedules.Find(br.T_SchedulerTaskID); //br.t_schedulertask;
        if(task.T_AssociatedScheduleTypeID == 1)
            if(task.T_StartDateTime > DateTime.UtcNow)
            {
                var strStartDatetime = Convert.ToString(task.T_StartDateTime);
                ScheduledTaskHistoryContext historycontext = new ScheduledTaskHistoryContext();
                var taskhistory = historycontext.ScheduledTaskHistorys.FirstOrDefault(wh => wh.BusinessRuleId == br.Id && wh.RunDateTime == strStartDatetime);
                if(taskhistory == null)
                    return task.T_StartDateTime;
                else
                    return DateTime.MinValue;
            }
            else
                return DateTime.MinValue;
        var ScheduledTime = task.T_StartDateTime.ToShortTimeString();
        var ScheduledDateTimeEnd = DateTime.UtcNow.AddYears(10);
        var occurrences = task.T_OccurrenceLimitCount != null ? task.T_OccurrenceLimitCount : 0;
        var skip = task.t_recurringrepeatfrequency != null ? task.t_recurringrepeatfrequency.T_Name : 0;
        if(task.t_recurringtaskendtype.DisplayValue == "On Specified Date")
            ScheduledDateTimeEnd = task.T_EndDate.Value;
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Daily")
        {
            DailyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new DailyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new DailyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(skip);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(DateTime.UtcNow);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Weekly")
        {
            WeeklyRecurrenceSettings we;
            SelectedDayOfWeekValues selectedValues = new SelectedDayOfWeekValues();
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            selectedValues.Sunday = task.T_RepeatOn_t_schedule.Select(p=>p.T_RecurrenceDaysID).ToList().Contains(1);
            selectedValues.Monday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(2);
            selectedValues.Tuesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(3);
            selectedValues.Wednesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(4);
            selectedValues.Thursday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(5);
            selectedValues.Friday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(6);
            selectedValues.Saturday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(7);
            var values = we.GetValues(skip, selectedValues);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(DateTime.UtcNow);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Monthly")
        {
            MonthlyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            we.AdjustmentValue = 0;
            skip = skip++;
            RecurrenceValues value;
            if(task.T_RepeatByID == 3)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.Day, skip);
            if(task.T_RepeatByID == 4)
                value = we.GetValues(MonthlySpecificDatePartOne.First, MonthlySpecificDatePartTwo.Day, skip);
            if(task.T_RepeatByID == 1)
                value = we.GetValues(task.T_StartDateTime.Day, skip);
            if(task.T_RepeatByID == 2)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.WeekendDay, skip);
            var nextdate = we.GetNextDate(DateTime.UtcNow);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Yearly")
        {
            YearlyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(task.T_StartDateTime.Day, task.T_StartDateTime.Month);
            var nextdate = we.GetNextDate(DateTime.UtcNow);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        return result;
    }
    /// <summary>Gets the next run time of task.</summary>
    ///
    /// <param name="br">The business rule.</param>
    /// <param name="baseDateTime">The baseDateTime.</param>
    ///
    /// <returns>The next run time of task.</returns>
    public DateTime getNextRunTimeOfTask(BusinessRule br, DateTime baseDateTime)
    {
        GeneratorBase.MVC.ApplicationContext db1 = new GeneratorBase.MVC.ApplicationContext(new InternalUser());
        var result = DateTime.MinValue;
        var task = db1.T_Schedules.Find(br.T_SchedulerTaskID); //br.t_schedulertask;
        if(task.T_AssociatedScheduleTypeID == 1)
            if(task.T_StartDateTime > baseDateTime)
            {
                var strStartDatetime = Convert.ToString(task.T_StartDateTime);
                ScheduledTaskHistoryContext historycontext = new ScheduledTaskHistoryContext();
                var taskhistory = historycontext.ScheduledTaskHistorys.FirstOrDefault(wh => wh.BusinessRuleId == br.Id && wh.RunDateTime == strStartDatetime);
                if(taskhistory == null)
                    return task.T_StartDateTime;
                else
                    return DateTime.MinValue;
            }
            else
                return DateTime.MinValue;
        var ScheduledTime = task.T_StartDateTime.ToShortTimeString();
        var ScheduledDateTimeEnd = baseDateTime.AddYears(10);
        var occurrences = task.T_OccurrenceLimitCount != null ? task.T_OccurrenceLimitCount : 0;
        var skip = task.t_recurringrepeatfrequency != null ? task.t_recurringrepeatfrequency.T_Name : 0;
        if(task.t_recurringtaskendtype.DisplayValue == "On Specified Date")
            ScheduledDateTimeEnd = task.T_EndDate.Value;
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Daily")
        {
            DailyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new DailyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new DailyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(skip);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(baseDateTime);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Weekly")
        {
            WeeklyRecurrenceSettings we;
            SelectedDayOfWeekValues selectedValues = new SelectedDayOfWeekValues();
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            selectedValues.Sunday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(1);
            selectedValues.Monday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(2);
            selectedValues.Tuesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(3);
            selectedValues.Wednesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(4);
            selectedValues.Thursday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(5);
            selectedValues.Friday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(6);
            selectedValues.Saturday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(7);
            var values = we.GetValues(skip, selectedValues);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(baseDateTime);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Monthly")
        {
            MonthlyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            we.AdjustmentValue = 0;
            skip = skip++;
            RecurrenceValues value;
            if(task.T_RepeatByID == 3)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.Day, skip);
            if(task.T_RepeatByID == 4)
                value = we.GetValues(MonthlySpecificDatePartOne.First, MonthlySpecificDatePartTwo.Day, skip);
            if(task.T_RepeatByID == 1)
                value = we.GetValues(task.T_StartDateTime.Day, skip);
            if(task.T_RepeatByID == 2)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.WeekendDay, skip);
            var nextdate = we.GetNextDate(baseDateTime);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if(task.t_associatedrecurringscheduledetailstype.DisplayValue == "Yearly")
        {
            YearlyRecurrenceSettings we;
            if(task.T_RecurringTaskEndTypeID == 2)
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(task.T_StartDateTime.Day, task.T_StartDateTime.Month);
            var nextdate = we.GetNextDate(baseDateTime);
            if(nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        return result;
    }
    /// <summary>Registers the task.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="BizId">     Identifier for the business rule.</param>
    public void RegisterTask(string EntityName, long BizId)
    {
        using(BusinessRuleContext brcontext = new BusinessRuleContext())
        {
            var MainBiz = brcontext.BusinessRules.Find(BizId);
            var task = MainBiz.t_schedulertask;
            RegisterScheduledTask RegisterTask = new RegisterScheduledTask();
            var nextDate = getNextRunTimeOfTask(MainBiz);
            if(nextDate > DateTime.MinValue)
            {
                Uri callbackUrl = new Uri(string.Format("http://localhost//TestNewTuranto74v4//TimeBasedAlert//ScheduledTask?EntityName=" + EntityName + "&BizId=" + BizId));
                var callbackid = Revalee.Client.RevaleeRegistrar.ScheduleCallback(nextDate, callbackUrl);
                ScheduledTaskHistoryContext historycontext = new ScheduledTaskHistoryContext();
                ScheduledTaskHistory historyItem = new ScheduledTaskHistory();
                historyItem.BusinessRuleId = MainBiz.Id;
                historyItem.CallbackUri = Convert.ToString(callbackUrl);
                historyItem.Status = "Pending";
                historyItem.GUID = callbackid.ToString();
                historyItem.TaskName = MainBiz.DisplayValue;
                historyItem.RunDateTime = Convert.ToString(nextDate);
                historycontext.ScheduledTaskHistorys.Add(historyItem);
                historycontext.SaveChanges();
            }
        }
    }
}

