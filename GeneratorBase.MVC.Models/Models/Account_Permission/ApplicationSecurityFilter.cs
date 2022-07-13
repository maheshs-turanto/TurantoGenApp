using GeneratorBase.MVC.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using System;
using GeneratorBase.MVC.DynamicQueryable;
namespace GeneratorBase.MVC
{
/// <summary>A parameter rebinder.</summary>
public class ParameterRebinder : System.Linq.Expressions.ExpressionVisitor
{
    /// <summary>The map.</summary>
    private readonly Dictionary<ParameterExpression, ParameterExpression> map;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="map">The map.</param>
    
    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }
    
    /// <summary>Replace parameters.</summary>
    ///
    /// <param name="map">The map.</param>
    /// <param name="exp">The exponent.</param>
    ///
    /// <returns>An Expression.</returns>
    
    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }
    
    /// <summary>Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.</summary>
    ///
    /// <param name="p">A ParameterExpression to process.</param>
    ///
    /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns
    /// the original expression.</returns>
    
    protected override Expression VisitParameter(ParameterExpression p)
    {
        ParameterExpression replacement;
        if(map.TryGetValue(p, out replacement))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }
}
/// <summary>An utility.</summary>
public static class Utility
{
    /// <summary>An Expression&lt;T&gt; extension method that composes.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="first"> The first to act on.</param>
    /// <param name="second">The second.</param>
    /// <param name="merge"> The merge.</param>
    ///
    /// <returns>An Expression&lt;T&gt;</returns>
    
    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // build parameter map (from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new
        {
            f, s = second.Parameters[i]
        }).ToDictionary(p => p.s, p => p.f);
        // replace parameters in the second lambda expression with parameters from the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
        // apply composition of lambda expression bodies to parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }
    
    /// <summary>An Expression&lt;Func&lt;T,bool&gt;&gt; extension method that ands.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="first"> The first to act on.</param>
    /// <param name="second">The second.</param>
    ///
    /// <returns>An Expression&lt;Func&lt;T,bool&gt;&gt;</returns>
    
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.And);
    }
    
    /// <summary>An Expression&lt;Func&lt;T,bool&gt;&gt; extension method that ors.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="first"> The first to act on.</param>
    /// <param name="second">The second.</param>
    ///
    /// <returns>An Expression&lt;Func&lt;T,bool&gt;&gt;</returns>
    
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.Or);
    }
    
    /// <summary>An Expression&lt;Func&lt;T,bool&gt;&gt; extension method that and also.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="first"> The first to act on.</param>
    /// <param name="second">The second.</param>
    ///
    /// <returns>An Expression&lt;Func&lt;T,bool&gt;&gt;</returns>
    
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }
    
    /// <summary>An Expression&lt;Func&lt;T,bool&gt;&gt; extension method that or else.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="first"> The first to act on.</param>
    /// <param name="second">The second.</param>
    ///
    /// <returns>An Expression&lt;Func&lt;T,bool&gt;&gt;</returns>
    
    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.OrElse);
    }
}
/// <summary>An application security filter (used for basic, tenant and user-based security), it filters DbContext.</summary>
public class ApplicationSecurityFilter : IFilter<ApplicationContext>
{
    /// <summary>The user.</summary>
    IUser user;
    /// <summary>Identifier for the tenant.</summary>
    long tenantId;
    /// <summary>Gets or sets a context for the database.</summary>
    ///
    /// <value>The database context.</value>
    public ApplicationContext DbContext
    {
        get;
        set;
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="tenantId">(Optional) Identifier for the tenant.</param>
    public ApplicationSecurityFilter(IUser user, long tenantId=0)
    {
        this.user = user;
        this.tenantId = tenantId;
    }
    /// <summary>Applies the basic security (No View permission), e.g. Always returns null dbset for user not having permission to view an entity .</summary>
    public void ApplyBasicSecurity()
    {
        if(string.IsNullOrEmpty(user.Name))
            return;
        List<long?> doclist = new List<long?>();
        if(!user.CanView("FileDocument"))
        {
            DbContext.FileDocuments = new FilteredDbSet<FileDocument>(DbContext, d => d.Id == 0);
        }
        else
        {
            doclist.AddRange(DbContext.FileDocuments.Select(p => p.AttachDocument));
        }
        if(!user.CanView("T_Customer"))
        {
            DbContext.T_Customers = new FilteredDbSet<T_Customer>(DbContext, d => d.Id == 0);
        }
        else
        {
        }
        if(!user.CanView("CompanyInformation"))
        {
            DbContext.CompanyInformations = new FilteredDbSet<CompanyInformation>(DbContext, d => d.Id == 0);
        }
        else
        {
            //code for company documents
            CompanyInformation compInfo = DbContext.CompanyInformations.FirstOrDefault();
            if(compInfo != null)
            {
                var legal = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "legal information").FirstOrDefault();
                var termsOfservice = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "terms of service").FirstOrDefault();
                var privacypolicy = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "privacy policy").FirstOrDefault();
                var thirdParty = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "third-party licenses").FirstOrDefault();
                var cookieInformation = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "cookie policy").FirstOrDefault();
                if(legal != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => legal.DocumentUpload));
                if(termsOfservice != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => termsOfservice.DocumentUpload));
                if(privacypolicy != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => privacypolicy.DocumentUpload));
                if(thirdParty != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => thirdParty.DocumentUpload));
                if(cookieInformation != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => cookieInformation.DocumentUpload));
                doclist.AddRange(DbContext.CompanyInformations.Select(p => p.Icon));
            }
            //
        }
        if(user.CanView("Document"))
            DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => doclist.Contains(d.Id));
        else
            DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => d.Id == 0);
    }
    /// <summary>Applies the tenant based security, filters DbSets involved in security.</summary>
    public void ApplyMainSecurity()
    {
    }
    /// <summary>Applies the tenant based security, filters DbSets involved in security.</summary>
    public void ApplyHierarchicalSecurity()
    {
        if(user.CanView("FileDocument") && !string.IsNullOrEmpty(user.ViewRestrict("FileDocument")))
        {
            DbContext.FileDocuments = new FilteredDbSet<FileDocument>(DbContext, DbContext.FileDocuments.Where1(user.ViewRestrict("FileDocument"), user.Name));
        }
        if(user.CanView("T_Customer") && !string.IsNullOrEmpty(user.ViewRestrict("T_Customer")))
        {
            DbContext.T_Customers = new FilteredDbSet<T_Customer>(DbContext, DbContext.T_Customers.Where1(user.ViewRestrict("T_Customer"), user.Name));
        }
    }
    /// <summary>Applies the user-based security (logged in user will be able to see only records associated with its user).</summary>
    public void ApplyUserBasedSecurity()
    {
        if(string.IsNullOrEmpty(user.Name))
            return;
        if(user.userbasedsecurity==null || user.userbasedsecurity.Count() == 0) return;
        string mainEntity = user.userbasedsecurity.FirstOrDefault(p => p.IsMainEntity).EntityName;
        List<long?> doclist = new List<long?>();
        //document security
        if(user.userbasedsecurity.Count()>0)
        {
            //code for company profile downloads
            CompanyInformation compInfo = DbContext.CompanyInformations.FirstOrDefault();
            if(compInfo != null)
            {
                var legal = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "legal information").FirstOrDefault();
                var termsOfservice = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "terms of service").FirstOrDefault();
                var privacypolicy = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "privacy policy").FirstOrDefault();
                var thirdParty = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "third-party licenses").FirstOrDefault();
                var cookieInformation = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "cookie policy").FirstOrDefault();
                if(legal != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => legal.DocumentUpload));
                if(termsOfservice != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => termsOfservice.DocumentUpload));
                if(privacypolicy != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => privacypolicy.DocumentUpload));
                if(thirdParty != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => thirdParty.DocumentUpload));
                if(cookieInformation != null) doclist.AddRange(DbContext.CompanyInformations.Select(p => cookieInformation.DocumentUpload));
                doclist.AddRange(DbContext.CompanyInformations.Select(p => p.Icon));
            }
            //
        }
    }
    /// <summary>Custom code hook section to filter DbSets.</summary>
    public void CustomFilter()
    {
    }
}
/// <summary>Decide role dynamically (at runtime).</summary>
public class AddDynamicRoles
{
    /// <summary>Gets display value for association.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="id">        The identifier.</param>
    ///
    /// <returns>The display value for association.</returns>
    private static string GetDisplayValueForAssociation(string EntityName, string id)
    {
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                MethodInfo mc = controller.GetMethod("GetDisplayValue");
                object[] MethodParams = new object[] { id };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToString(obj == null ? "" : obj);
            }
        }
        catch
        {
            return id;
        }
    }
    /// <summary>Check condition for dynamic role assignment.</summary>
    ///
    /// <param name="obj1">      The object.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="PropName">  Name of the property.</param>
    /// <param name="value">     The value.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckCondition(object obj1, string EntityName, string PropName, string value)
    {
        if(ModelReflector.Entities == null) return false;
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        if(EntityInfo != null)
        {
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
            if(PropInfo != null)
            {
                var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                string DataType = PropInfo.DataType;
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                if(AssociationInfo != null)
                {
                    if(PropValue != null)
                        PropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(PropValue));
                    DataType = "Association";
                }
                return ValidateValueAgainstRule(PropValue, DataType, "=", value, Property);
            }
        }
        return false;
    }
    /// <summary>Validates the value against rule.</summary>
    ///
    /// <param name="PropValue">The property value.</param>
    /// <param name="DataType"> Type of the data.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="value">    The value.</param>
    /// <param name="property"> The property.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool ValidateValueAgainstRule(object PropValue, string DataType, string condition, string value, PropertyInfo property)
    {
        if(PropValue == null) return false;
        bool result = false;
        Type targetType = property.PropertyType;
        if(property.PropertyType.IsGenericType)
            targetType = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
        if(DataType == "Association")
        {
            targetType = typeof(System.String);
            PropValue = Convert.ToString(PropValue).Trim();
        }
        dynamic value1 = Convert.ChangeType(PropValue, targetType);
        dynamic value2 = Convert.ChangeType(value.Trim(), targetType);
        switch(condition)
        {
        case "=":
            if(value1 == value2) return true;
            break;
        case ">":
            if(value1 > value2) return true;
            break;
        case "<":
            if(value1 < value2) return true;
            break;
        case "<=":
            if(value1 <= value2) return true;
            break;
        case ">=":
            if(value1 >= value2) return true;
            break;
        case "!=":
            if(value1 != value2) return true;
            break;
        case "Contains":
            if((Convert.ToString(value1)).Contains(value2.ToString())) return true;
            break;
        }
        return result;
    }
    /// <summary>Adds the roles dynamic to 'userid' temporary for session.</summary>
    ///
    /// <param name="roles"> The roles.</param>
    /// <param name="userid">The userid.</param>
    ///
    /// <returns>A string[].</returns>
    public string[] AddRolesDynamic(string[] roles, string userid)
    {
        return roles;
    }
}

/// <summary>A check permission for owner (logged in user can edit only records associated with his/her user).</summary>
public class CheckPermissionForOwner
{
    /// <summary>Check user condition.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="original">The original.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckUserCondition(IUser user, Object original)
    {
        using(ApplicationDbContext userdb = new ApplicationDbContext(true))
        {
            var userObj = userdb.Users.FirstOrDefault(p => p.UserName == user.Name);
            if(userObj != null && original.ToString() == userObj.Id)
                return false;
        }
        return true;
    }
    /// <summary>Gets an object.</summary>
    ///
    /// <param name="user">       The user.</param>
    /// <param name="EntityName"> Name of the entity.</param>
    /// <param name="obj">        The object.</param>
    /// <param name="FromContext">True to from context.</param>
    /// <param name="propName">   Name of the property.</param>
    ///
    /// <returns>The object.</returns>
    private object getObject(IUser user, string EntityName, Object obj, bool FromContext, string propName)
    {
        Object original;
        propName = propName.Trim();
        try
        {
            if(FromContext)
            {
                obj = (System.Data.Entity.Infrastructure.DbPropertyValues)obj;
                original = ((System.Data.Entity.Infrastructure.DbPropertyValues)obj)[propName + "ID"];
            }
            else
            {
                PropertyInfo[] properties = (obj.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == propName + "ID");
                original = Property.GetValue(obj, null);
            }
            return original;
        }
        catch
        {
            return null;
        }
    }
    /// <summary>Check lock condition (business rule).</summary>
    ///
    /// <param name="EntityName"> Name of the entity.</param>
    /// <param name="obj">        The object.</param>
    /// <param name="user">       The user.</param>
    /// <param name="FromContext">True to from context.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool CheckLockCondition(string EntityName, Object obj, IUser user, bool FromContext)
    {
        try
        {
            bool result = false;
            using(ApplicationContext db = new ApplicationContext(new SystemUser()))
            {
                var BRMain = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
                if(BRMain.Count() > 0)
                {
                    var ResultOfBusinessRules = db.LockEntityRule(obj, BRMain, EntityName);
                    if(ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                    {
                        return true;
                    }
                }
                var type = obj.GetType();
                var assolist = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Associations.ToList();
                PropertyInfo[] properties = type.GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                foreach(var asso in assolist)
                {
                    if(asso.Target == null || asso.Target == "IdentityUser") continue;
                    var BR = user.businessrules.Where(p => p.EntityName == asso.Target).ToList();
                    if(BR.Count() == 0) continue;
                    object value = null;
                    if(!FromContext && type.GetProperty(asso.Name.ToLower()) != null)
                        value = type.GetProperty(asso.Name.ToLower()).GetValue(obj, null);
                    else
                    {
                        obj = (System.Data.Entity.Infrastructure.DbPropertyValues)obj;
                        var original = ((System.Data.Entity.Infrastructure.DbPropertyValues)obj)[asso.Name + "ID"];
                        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + asso.Target + "Controller, GeneratorBase.MVC.Controllers");
                        Type controller = new CreateControllerType(asso.Target).controllerType;
                        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                        {
                            MethodInfo mc = controller.GetMethod("GetRecordById");
                            object[] MethodParams = new object[] { Convert.ToString(original) };
                            value = mc.Invoke(objController, MethodParams);
                        }
                    }
                    if(value != null)
                    {
                        var ResultOfBusinessRules = db.LockEntityRule(value, BR, asso.Target);
                        if(ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                        {
                            return true;
                        }
                    }
                }
                return result;
            }
        }
        catch
        {
            return false;
        }
    }
    /// <summary>Check owner permission (logged in user can edit only records associated with his/her user).</summary>
    ///
    /// <param name="EntityName"> Name of the entity.</param>
    /// <param name="obj">        The object.</param>
    /// <param name="user">       The user.</param>
    /// <param name="FromContext">True to from context.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool CheckOwnerPermission(string EntityName, Object obj, IUser user, bool FromContext)
    {
        var result = true;
        var OwnerAssociationName = user.OwnerAssociation(EntityName);
        if(string.IsNullOrEmpty(OwnerAssociationName)) return result;
        ApplicationContext db = new ApplicationContext(new SystemUser());
        return result;
    }
}
}

