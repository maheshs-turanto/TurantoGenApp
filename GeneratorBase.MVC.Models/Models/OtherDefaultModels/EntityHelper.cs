using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>An entity helper.</summary>
public static class EntityHelper
{
    /// <summary>An IEntity extension method that gets a name.</summary>
    ///
    /// <param name="entity">The entity to act on.</param>
    ///
    /// <returns>The name.</returns>
    
    public static string GetName(this IEntity entity)
    {
        // TODO: make sure that we find the base class, sometimes we get Order_8383 instead of Order
        return entity.GetType().ToString();
    }
}
}