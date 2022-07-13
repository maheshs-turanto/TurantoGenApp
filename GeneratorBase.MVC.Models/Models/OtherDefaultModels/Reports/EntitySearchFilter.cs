using GeneratorBase.MVC.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using System;
namespace GeneratorBase.MVC
{
/// <summary>An entity search filter (used in faced search with custom code hook).</summary>
public static class EntitySearchFilter
{
    /// <summary>A T extension method that applies the filter.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="query">     The query to act on.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="FilterName">Name of the filter.</param>
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    ///
    /// <returns>A T.</returns>
    public static T ApplyFilter<T>(this T query, string EntityName, string FilterName, IUser User, ApplicationContext db)
    {
        return query;
    }
}
}

