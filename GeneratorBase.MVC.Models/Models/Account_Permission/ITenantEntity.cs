using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>Interface for tenant entity.</summary>
public interface ITenantEntity
{
    /// <summary>Gets or sets the identifier of the tenant.</summary>
    ///
    /// <value>The identifier of the tenant.</value>
    
    Nullable<long> TenantID
    {
        get;
        set;
    }
    
}
}