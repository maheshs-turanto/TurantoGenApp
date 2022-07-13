using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>Interface for entity.</summary>
public interface IEntity
{
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the concurrency key.</summary>
    ///
    /// <value>The concurrency key.</value>
    
    byte[] ConcurrencyKey
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    string DisplayValue
    {
        get;
        set;
    }
}
}