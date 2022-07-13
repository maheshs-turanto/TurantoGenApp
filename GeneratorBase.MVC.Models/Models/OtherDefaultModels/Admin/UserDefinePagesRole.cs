using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>A user define pages role.</summary>
[Table("tbl_UserDefinePagesRole")]
public class UserDefinePagesRole
{
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the page.</summary>
    ///
    /// <value>The identifier of the page.</value>
    
    [DisplayName("Page Id")]
    public long PageId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the role.</summary>
    ///
    /// <value>The name of the role.</value>
    
    [DisplayName("Role Name")]
    public string RoleName
    {
        get;
        set;
    }
}
}