using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Z.EntityFramework.Plus;
namespace System.Data.Entity
{
public static class QueryableExtensions
{

    public static IPagedList<Tsource> ToCachedPagedList<Tsource>(this IQueryable<Tsource> source, int pageNumber, int pageSize)
    where Tsource : class
    {
        var name = typeof(Tsource).Name;
        if(new GeneratorBase.MVC.ApplicationContext(new SystemUser()).EntityPages.FromCache("entitypage").Any(p => p.EntityName == name && p.EnableDataCache.Value))
        {
            return source.GetFromCache<IQueryable<Tsource>, Tsource>().ToPagedList(pageNumber, pageSize);
        }
        else return source.ToPagedList(pageNumber, pageSize);
        
    }
    public static List<Tsource> ToCachedList<Tsource>(this IQueryable<Tsource> source)
    where Tsource : class
    {
        var name = typeof(Tsource).Name;
        if(new GeneratorBase.MVC.ApplicationContext(new SystemUser()).EntityPages.FromCache("entitypage").Any(p => p.EntityName == name && p.EnableDataCache.Value))
        {
            return source.GetFromCache<IQueryable<Tsource>, Tsource>().ToList();
        }
        else return source.ToList();
        
    }
}
}
namespace GeneratorBase.MVC.Models
{
public class cookieParameter
{
    public DateTime expiration
    {
        get;
        set;
    }
    public bool secure
    {
        get;
        set;
    }
    public bool httponly
    {
        get;
        set;
    }
    public cookieParameter(double expirationday)
    {
        expiration = DateTime.UtcNow.AddDays(expirationday);
        secure = true;
        httponly = true;
    }
    public cookieParameter(double expirationday, bool issecure, bool httponly)
    {
        expiration = DateTime.UtcNow.AddDays(expirationday);
        secure = issecure;
        this.httponly = httponly;
    }
}
public static class CacheHelper
{
    public static bool NoCache(string name)
    {
        if(QueryCacheManager.CacheTags.Count(p => p.Key == "Z.EntityFramework.Plus.QueryCacheManager;" + name.ToLower()) == 0)
            return true;
        return false;
    }
    public static void RemoveAllCache()
    {
        QueryCacheManager.ExpireAll();
    }
    public static void RemoveCache(string name)
    {
        QueryCacheManager.ExpireTag(name.ToLower());
    }
    public static T GetFromCache<T, U>(this T query)
    where T : IQueryable
        where U : class
    {
        // return if app setting value is null or 0
        CommonFunction AppSetting = GeneratorBase.MVC.Models.CommonFunction.Instance;
        //if(!AppSetting.EnableQueryCache())
        //    return query;
        
        var timeout = AppSetting.QueryCacheTimeOut();
        double time = 0;
        Double.TryParse(timeout, out time);
        if(time == 0)
        {
            //if(Enum.IsDefined(typeof(DefaultCachedEntities), typeof(U).Name))
            //    timeout = "60";
            //else
            return query;
        }
        //int time = 0;
        //Int32.TryParse(timeout, out time);
        //if(time == 0) return query;
        //
        
        var options = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(time)
        };
        try
        {
            return (T)((IQueryable<U>)query).FromCache(options, typeof(U).Name.ToLower()).AsQueryable();
        }
        catch
        {
            return query;
        }
    }
}
public enum DefaultCachedEntities
{
    BusinessRule,
    CompanyInformation,
    CompanyInformationCompanyListAssociation,
    FooterSection,
    PropertyHelpPage,
    Permission,
    AdminPrivileges,
    ActionType,
    UserBasedSecurity,
    PermissionAdminPrivilege,
    ActionArgs,
    Condition,
    DefaultEntityPage,
    RuleAction,
    UserDefinePages,
    UserDefinePagesRole,
    User,
    Role,
    EntityPage,
    PropertyValidationandFormat,
    T_Chart
}
}