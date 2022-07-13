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
public static class HiearchicalSecurityHelper
{
    public static System.Data.Entity.IDbSet<T> ReturnDbSet<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext, string EntityName)
    where T : class
    {
    
        return new FilteredDbSet<T>(DbContext, source.Where1(user.ViewRestrict(EntityName), user.Name));
        
        
    }
    public static bool CheckValidity<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext,string query, string EntityName)
    where T : class
    {
        try
        {
            new FilteredDbSet<T>(DbContext, source.Where1(query, user.Name));
            return true;
        }
        catch
        {
            return false;
        }
        
    }
    public static bool CheckEdit<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext, string query, string EntityName)
    where T : class
    {
        try
        {
            new FilteredDbSet<T>(DbContext, source.Where1(query, user.Name));
            return true;
        }
        catch
        {
            return false;
        }
        
    }
    public static bool CheckEdit<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext, string query, string EntityName,object obj)
    where T : class
    {
        try
        {
            PropertyInfo[] properties = (obj.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == "Id");
            var value = Property.GetValue(obj, null);
            query = query + " && Id=" + value;
            var queryresult =  new FilteredDbSet<T>(DbContext, source.Where1(query, user.Name)).FirstOrDefault();
            return queryresult == null ? false : true;
        }
        catch
        {
            return true;
        }
        
    }
    public static bool CheckExpressionValidity<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext, string query, string EntityName)
    where T : class
    {
        try
        {
            var texpression = source.Where1(query, user.Name);
            return true;
        }
        catch
        {
            return false;
        }
        
    }
    public static Expression GetValidExpression<T>(this System.Data.Entity.IDbSet<T> source, IUser user, ApplicationContext DbContext, string query, string EntityName)
    where T : class
    {
        try
        {
            var texpression = source.Where1(query, user.Name);
            return texpression;
        }
        catch
        {
            return null;
        }
        
    }
    public static bool HierarchicalSecurityEditCheck(string EntityName, IUser user, ApplicationContext DbContext, string query, object obj)
    {
        bool result = true;
        result = HierarchicalSecurityCheck(EntityName, user, DbContext, query, 1,obj);
        return result;
    }
    public static bool HierarchicalSecurityDeleteCheck(string EntityName, IUser user, ApplicationContext DbContext, string query, object obj)
    {
        bool result = true;
        result = HierarchicalSecurityCheck(EntityName, user, DbContext, query, 1, obj);
        return result;
    }
    public static bool HierarchicalSecurityCheck(string EntityName, IUser user, ApplicationContext DbContext, string query,int restrictmode =0,object obj=null)
    {
        bool result = true;
        if(EntityName == "FileDocument")
        {
            result = CheckExpressionValidity<FileDocument>(DbContext.FileDocuments, user, DbContext, query, EntityName);
            if(result && restrictmode == 1 && obj != null)
                result = CheckEdit<FileDocument>(DbContext.FileDocuments, user, DbContext, query, EntityName,obj);
        }
        if(EntityName == "T_Customer")
        {
            result = CheckExpressionValidity<T_Customer>(DbContext.T_Customers, user, DbContext, query, EntityName);
            if(result && restrictmode == 1 && obj != null)
                result = CheckEdit<T_Customer>(DbContext.T_Customers, user, DbContext, query, EntityName,obj);
        }
        return result;
    }
}
}
