using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeneratorBase.MVC.DynamicQueryable;
using Z.EntityFramework.Plus;
using System.Runtime.Caching;
namespace GeneratorBase.MVC.Models
{
/// <summary>Dropdown Helper class.</summary>
public class DropdownHelper
{
    /// <summary>Gets items for single select dropdown.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueryable list.</param>
    /// <param name="caller">            The caller to filter items.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The association name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="ExtraVal">          The existing selected item value.</param>
    /// <param name="entityName">        Name of the entity.</param>
    /// <param name="RestrictDropdownVal">        The RestrictDropdownVal.</param>
    ///
    /// <returns>all value.</returns>
    
    public static List<Entity> GetAllValue<T>(IUser User, IQueryable<T> list, string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string entityName, string RestrictDropdownVal = "")
    {
        var returnlist = new List<Entity>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(',').ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller, RestrictDropdownVal);
        var baseList = (IQueryable<Entity>)list;
        var sortcolumn = CustomHelperMethod.sortDropDown(entityName);
        if(!string.IsNullOrEmpty(sortcolumn))
        {
            list = list.Sort<T>(sortcolumn);
            baseList = (IQueryable<Entity>)list;
            if(key != null && key.Length > 0)
            {
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(baseList.Where(p => p.Id == val));
                }
                else
                {
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).Take(10);
                }
            }
            else
            {
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    returnlist = baseList.Where(p => p.Id != val).Take(9).Union(baseList.Where(p => p.Id == val)).Distinct();
                }
                else
                {
                    returnlist = baseList.Take(10);
                }
            }
            return checkCache(returnlist, typeof(T).Name); //returnlist.FromCache(typeof(T).Name).ToList();
        }
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && p.Id != val).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(9).Union(baseList.Where(p => p.Id == val)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue);
            }
            else
            {
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                returnlist = baseList.Where(p => p.Id != val).OrderBy(q => q.DisplayValue).Take(9).Union(baseList.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue);
            }
            else
            {
                returnlist = baseList.OrderBy(q => q.DisplayValue).Take(10);
            }
        }
        return checkCache(returnlist, typeof(T).Name);//returnlist.FromCache(typeof(T).Name).ToList();
    }
    /// <summary>Gets items for single select dropdown.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueryable list.</param>
    /// <param name="caller">            The caller to filter items.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The association name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="ExtraVal">          The existing selected item value.</param>
    /// <param name="entityName">        Name of the entity.</param>
    /// <param name="RestrictDropdownVal">        The RestrictDropdownVal.</param>
    /// <param name="sortorder">        The sortorder.</param>
    ///
    /// <returns>all value.</returns>
    public static List<Entity> GetAllValueDropDown<T>(IUser User, IQueryable<T> list, string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string entityName, string RestrictDropdownVal = "", string sortorder = "")
    {
        var returnlist = new List<Entity>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(',').ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller, RestrictDropdownVal);
        var baseList = (IQueryable<Entity>)list;
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && p.Id != val).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(9).Union(baseList.Where(p => p.Id == val)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue);
            }
            else
            {
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                if(!string.IsNullOrEmpty(sortorder))
                {
                    var baselist1 = list;
                    returnlist = (IQueryable<Entity>)baselist1.Where("Id != " + val).OrderBy(sortorder).Take(9).Union(baselist1.Where("Id = " + val)).Distinct().OrderBy(sortorder).Take(10);
                    //returnlist = baseList.OrderBy(sortorder).Take(10).Where(p => p.Id != val).Union(baseList.Where(p => p.Id == val)).Distinct().Take(10);
                }
                else
                    returnlist = baseList.Where(p => p.Id != val).OrderBy(p => p.DisplayValue).Take(9).Union(baseList.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue);
            }
            else
            {
                if(!string.IsNullOrEmpty(sortorder))
                    returnlist = baseList.OrderBy(sortorder).Take(10);
                else
                    returnlist = baseList.OrderBy(q => q.DisplayValue).Take(10);
            }
        }
        //if (!string.IsNullOrEmpty(sortorder))
        //    returnlist = returnlist.OrderBy(sortorder);
        // returnlist = returnlist.Take(10);
        return checkCache(returnlist, typeof(T).Name);//returnlist.FromCache(typeof(T).Name).ToList();
    }
    /// <summary>Gets items for single select dropdown.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueryable list.</param>
    /// <param name="caller">            The caller to filter items.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The association name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="ExtraVal">          The existing selected item value.</param>
    /// <param name="entityName">        Name of the entity.</param>
    /// <param name="RestrictDropdownVal">        The RestrictDropdownVal.</param>
    ///
    /// <returns>all value.</returns>
    public static List<ApplicationUser> GetAllValueLoginUser<T>(IUser User, IQueryable<T> list, string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string entityName, string RestrictDropdownVal = "")
    {
        var returnlist = new List<ApplicationUser>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(',').ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller, RestrictDropdownVal);
        var baseList = (IQueryable<ApplicationUser>)list;
        var sortcolumn = CustomHelperMethod.sortDropDown(entityName);
        if(!string.IsNullOrEmpty(sortcolumn))
        {
            list = list.Sort<T>(sortcolumn);
            baseList = (IQueryable<ApplicationUser>)list;
            if(key != null && key.Length > 0)
            {
                if(!string.IsNullOrEmpty(ExtraVal))
                {
                    string val = ExtraVal;
                    returnlist = baseList.Where(p => p.UserName.Contains(key) && p.Id != val).Take(9).Union(baseList.Where(p => p.Id == val));
                }
                else
                {
                    returnlist = baseList.Where(p => p.UserName.Contains(key)).Take(10);
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(ExtraVal))
                {
                    string val = ExtraVal;
                    returnlist = baseList.Where(p => p.Id != val).Take(9).Union(baseList.Where(p => p.Id == val)).Distinct();
                }
                else
                {
                    returnlist = baseList.Take(10);
                }
            }
            return checkCacheUserLogin(returnlist, typeof(T).Name); //returnlist.FromCache(typeof(T).Name).ToList();
        }
        if(key != null && key.Length > 0)
        {
            if(!string.IsNullOrEmpty(ExtraVal))
            {
                string val = ExtraVal;
                returnlist = baseList.Where(p => p.UserName.Contains(key) && p.Id != val).OrderBy(s => s.UserName.IndexOf(key)).ThenBy(p => p.UserName.Length).ThenBy(p => p.UserName).Take(9).Union(baseList.Where(p => p.Id == val)).OrderBy(s => s.UserName.IndexOf(key)).ThenBy(p => p.UserName.Length).ThenBy(p => p.UserName);
            }
            else
            {
                returnlist = baseList.Where(p => p.UserName.Contains(key)).OrderBy(s => s.UserName.IndexOf(key)).ThenBy(p => p.UserName.Length).ThenBy(p => p.UserName).Take(10);
            }
        }
        else
        {
            if(!string.IsNullOrEmpty(ExtraVal))
            {
                string val = ExtraVal;
                returnlist = baseList.Where(p => p.Id != val).OrderBy(q => q.UserName).Take(9).Union(baseList.Where(p => p.Id == val)).Distinct().OrderBy(q => q.UserName);
            }
            else
            {
                returnlist = baseList.OrderBy(q => q.UserName).Take(10);
            }
        }
        return checkCacheUserLogin(returnlist, typeof(T).Name);//returnlist.FromCache(typeof(T).Name).ToList();
    }
    private static List<ApplicationUser> checkCacheUserLogin(IQueryable<ApplicationUser> returnlist, string name)
    {
        CommonFunction AppSetting = GeneratorBase.MVC.Models.CommonFunction.Instance;
        var timeout = AppSetting.QueryCacheTimeOut();
        if(string.IsNullOrEmpty(timeout))
            return returnlist.ToList();
        int time = 0;
        Int32.TryParse(timeout, out time);
        if(time == 0) return returnlist.ToList();
        var options = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromHours(time)
        };
        if(new GeneratorBase.MVC.ApplicationContext(new SystemUser()).EntityPages.FromCache("entitypage").Any(p => p.EntityName == name && p.EnableDataCache.Value))
        {
            return returnlist.FromCache(options, name.ToLower()).ToList();
        }
        else
        {
            return returnlist.ToList();
        }
        return returnlist.ToList();
    }
    private static List<Entity> checkCache(IQueryable<Entity> returnlist, string name)
    {
        CommonFunction AppSetting = GeneratorBase.MVC.Models.CommonFunction.Instance;
        var timeout = AppSetting.QueryCacheTimeOut();
        if(string.IsNullOrEmpty(timeout))
            return returnlist.ToList();
        int time = 0;
        Int32.TryParse(timeout, out time);
        if(time == 0) return returnlist.ToList();
        var options = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromHours(time)
        };
        if(new GeneratorBase.MVC.ApplicationContext(new SystemUser()).EntityPages.FromCache("entitypage").Any(p => p.EntityName == name && p.EnableDataCache.Value))
        {
            return returnlist.FromCache(options, name.ToLower()).ToList();
        }
        else
        {
            return returnlist.ToList();
        }
        return returnlist.ToList();
    }
    /// <summary>Gets items for Radio button.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueryable list.</param>
    /// <param name="caller">            The caller to filter items.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="ExtraVal">          The existing selected item value.</param>
    /// <param name="entityName">        Name of the entity.</param>
    ///
    /// <returns>all value for rb.</returns>
    
    public static List<Entity> GetAllValueForRB<T>(IUser User, IQueryable<T> list, string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string entityName)
    {
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = list;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T>)q);
            }
        }
        var baseList = (IQueryable<Entity>)list;
        var data = baseList.OrderBy(q => q.DisplayValue).ToList();
        return data;
    }
    
    /// <summary>Gets items for Multiselect dropdown.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueyable list.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="entityName">        Name of the entity.</param>
    /// <param name="caller">            The caller to filter items.</param>
    ///
    /// <returns>all multi select value.</returns>
    public static List<Entity> GetAllMultiSelectValue<T>(IUser User, IQueryable<T> list, string key, string AssoNameWithParent, string AssociationID, string entityName, string caller = null, string ExtraVal = null)
    {
        var returnlist = new List<Entity>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller);
        IQueryable<Entity> baseList = (IQueryable<Entity>)list;
        var sortcolumn = CustomHelperMethod.sortMultiSelectDropDown(entityName);
        if(!string.IsNullOrEmpty(sortcolumn))
        {
            if(key != null && key.Length > 0)
            {
                //returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).Take(10);
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                    var records = 10 - val.Count;
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && !(val.Contains(p.Id))).Take(9).Union(baseList.Where(p => val.Contains(p.Id)));
                }
                else
                {
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).Take(10);
                }
            }
            else
            {
                //returnlist = baseList.Take(10);
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                    var records = 10 - val.Count;
                    returnlist = baseList.Where(p => !(val.Contains(p.Id))).Take(9).Union(baseList.Where(p => val.Contains(p.Id))).Distinct();
                }
                else
                {
                    returnlist = baseList.Take(10);
                }
            }
            return checkCache(returnlist, typeof(T).Name);
        }
        if(key != null && key.Length > 0)
        {
            //returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10);
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                var records = 10 - val.Count;
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && !(val.Contains(p.Id))).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(9).Union(baseList.Where(p => (val.Contains(p.Id)))).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue);
            }
            else
            {
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10);
            }
        }
        else
        {
            //returnlist = baseList.OrderBy(q => q.DisplayValue).Take(10);
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                var records = 10 - val.Count;
                returnlist = baseList.Where(p => !(val.Contains(p.Id))).OrderBy(q => q.DisplayValue).Take(9).Union(baseList.Where(p => (val.Contains(p.Id)))).Distinct().OrderBy(q => q.DisplayValue);
            }
            else
            {
                returnlist = baseList.OrderBy(q => q.DisplayValue).Take(10);
            }
        }
        return checkCache(returnlist, typeof(T).Name);
    }
    
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="User"></param>
    /// <param name="list"></param>
    /// <param name="key"></param>
    /// <param name="AssoNameWithParent"></param>
    /// <param name="AssociationID"></param>
    /// <param name="entityName"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public static List<Entity> GetMultiSelectValueAllSelection<T>(IUser User, IQueryable<T> list, string key, string AssoNameWithParent, string AssociationID, string entityName, string caller = null, string ExtraVal = null)
    {
        var returnlist = new List<Entity>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(',').ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller);
        IQueryable<Entity> baseList = (IQueryable<Entity>)list;
        var sortcolumn = CustomHelperMethod.sortMultiSelectDropDown(entityName);
        if(!string.IsNullOrEmpty(sortcolumn))
        {
            if(key != null && key.Length > 0)
            {
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && !(val.Contains(p.Id))).Union(baseList.Where(p => val.Contains(p.Id)));
                }
                else
                {
                    returnlist = baseList.Where(p => p.DisplayValue.Contains(key));
                }
            }
            else
            {
                if(ExtraVal != null && ExtraVal.Length > 0)
                {
                    var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                    returnlist = baseList.Where(p => !(val.Contains(p.Id))).Union(baseList.Where(p => val.Contains(p.Id))).Distinct();
                }
                else
                {
                    returnlist = baseList;
                }
            }
            return checkCache(returnlist, typeof(T).Name);
        }
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key) && !(val.Contains(p.Id))).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Union(baseList.Where(p => (val.Contains(p.Id)))).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue);
            }
            else
            {
                returnlist = baseList.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => !string.IsNullOrWhiteSpace(wh)).Select(long.Parse).ToList();
                returnlist = baseList.Where(p => !(val.Contains(p.Id))).OrderBy(q => q.DisplayValue).Union(baseList.Where(p => (val.Contains(p.Id)))).Distinct().OrderBy(q => q.DisplayValue);
            }
            else
            {
                returnlist = baseList.OrderBy(q => q.DisplayValue);
            }
        }
        return checkCache(returnlist, typeof(T).Name);
    }
    /// <summary>Gets items for Multiselect dropdown.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">              The user.</param>
    /// <param name="list">              The IQueyable list.</param>
    /// <param name="key">               The key to search.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Id for the association.</param>
    /// <param name="entityName">        Name of the entity.</param>
    /// <param name="caller">            The caller to filter items.</param>
    ///
    /// <returns>all multi select value.</returns>
    public static List<Entity> GetAllMultiSelectValueControl<T>(IUser User, IQueryable<T> list, string key, string AssoNameWithParent, string AssociationID, string entityName, string caller = null, string ExtraVal = null)
    {
        var returnlist = new List<Entity>().AsQueryable();
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            var AssoIDs = AssociationID.TrimEnd(',').Split(',').ToList();
            string lambdastr = "";
            IQueryable queryforcondtion = list;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i]);
                if(AssoID != null && AssoID > 0)
                {
                    foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambdastr += asso + "=" + AssoID + " OR ";
                    }
                }
                else if(AssoID == 0)
                {
                    IQueryable query = list;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T>)q);
                }
            }
            if(lambdastr.Length > 0)
            {
                lambdastr = lambdastr.TrimEnd(" OR ".ToCharArray());
                queryforcondtion = queryforcondtion.Where(lambdastr);
                list = ((IQueryable<T>)queryforcondtion);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T>(User, list, entityName, caller);
        IQueryable<Entity> baseList = (IQueryable<Entity>)list;
        returnlist = baseList;
        return checkCache(returnlist, typeof(T).Name);
    }
}
}