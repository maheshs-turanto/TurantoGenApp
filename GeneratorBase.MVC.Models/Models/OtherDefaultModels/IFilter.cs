using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneratorBase.MVC.Models
{
/// <summary>Interface for filter (Tenant, User Based Security and Basic Security data filter).</summary>
///
/// <typeparam name="T">Generic type parameter.</typeparam>

public interface IFilter<T> where T : DbContext
{
    /// <summary>Gets or sets a context for the database.</summary>
    ///
    /// <value>The database context.</value>
    
    T DbContext
    {
        get;
        set;
    }
    /// <summary>Applies the basic security.</summary>
    void ApplyBasicSecurity();
    /// <summary>Applies the main security.</summary>
    void ApplyMainSecurity();
    /// <summary>Applies the HierarchicalSecurity.</summary>
    void ApplyHierarchicalSecurity();
    /// <summary>Applies the user based security.</summary>
    void ApplyUserBasedSecurity();
    /// <summary>Custom filter.</summary>
    void CustomFilter();
}
}
