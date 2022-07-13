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
/// <summary>A dynamic role mapping.</summary>
[Table("tbl_DynamicRoleMapping")]
public class DynamicRoleMapping : EntityDefault
{
    public DynamicRoleMapping()
    {
        this.dynamicruleconditions = new HashSet<Condition>();
    }
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Entity Name")] [Required]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the role.</summary>
    ///
    /// <value>The identifier of the role.</value>
    
    [DisplayName("RoleId")] [Required]
    public string RoleId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the condition.</summary>
    ///
    /// <value>The condition.</value>
    
    [DisplayName("Condition")] [Required]
    public string Condition
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the value.</summary>
    ///
    /// <value>The value.</value>
    
    [DisplayName("Value")] [Required]
    public string Value
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the user relation.</summary>
    ///
    /// <value>The user relation.</value>
    
    [DisplayName("User Relation")] [Required]
    public string UserRelation
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the other.</summary>
    ///
    /// <value>The other.</value>
    
    [DisplayName("Other")]
    public string Other
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The flag.</value>
    
    [DisplayName("Flag")]
    public Boolean? Flag
    {
        get;
        set;
    }
    public virtual ICollection<Condition> dynamicruleconditions
    {
        get;
        set;
    }
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.EntityName);
        dispValue = !string.IsNullOrEmpty(dispValue) ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
        this.m_DisplayValue = dispValue;
        return dispValue;
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        if(!string.IsNullOrEmpty(m_DisplayValue))
            return m_DisplayValue;
        return Convert.ToString(this.EntityName);
    }
    /// <summary>Sets the calculation.</summary>
    public void setCalculation()
    {
    }
}
}

