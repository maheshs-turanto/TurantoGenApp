using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace GeneratorBase.MVC.Models
{
/// <summary>A permission admin privilege for role.</summary>
[Table("tbl_PermissionAdminPrivilege")]
public class PermissionAdminPrivilege
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
    
    /// <summary>Gets or sets the admin feature.</summary>
    ///
    /// <value>The admin feature.</value>
    
    [DisplayName("AdminFeature")]
    public string AdminFeature
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the role.</summary>
    ///
    /// <value>The name of the role.</value>
    
    [DisplayName("RoleName")]
    public string RoleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is allow.</summary>
    ///
    /// <value>True if this object is allow, false if not.</value>
    
    [DisplayName("View")]
    public bool IsAllow
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is add.</summary>
    ///
    /// <value>True if this object is add, false if not.</value>
    
    [DisplayName("Add")]
    public bool IsAdd
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is edit.</summary>
    ///
    /// <value>True if this object is edit, false if not.</value>
    
    [DisplayName("Edit")]
    public bool IsEdit
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is delete.</summary>
    ///
    /// <value>True if this object is delete, false if not.</value>
    
    [DisplayName("Delete")]
    public bool IsDelete
    {
        get;
        set;
    }
    
    /// <summary>Gets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [NotMapped]
    public string DisplayValue
    {
        get
        {
            return AdminFeature + "-" + RoleName;
        }
    }
}

}
