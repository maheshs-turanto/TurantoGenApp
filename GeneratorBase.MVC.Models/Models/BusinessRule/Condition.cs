using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace GeneratorBase.MVC.Models
{
/// <summary>A condition.</summary>
[Table("tbl_Condition")]
public class Condition
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
    
    /// <summary>Gets or sets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    
    [DisplayName("Property Name")]
    [Required]
    public string PropertyName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the operator.</summary>
    ///
    /// <value>The operator.</value>
    
    [DisplayName("Operator")]
    public string Operator
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the value.</summary>
    ///
    /// <value>The value.</value>
    
    [DisplayName("Value")]
    public string Value
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the logical connector.</summary>
    ///
    /// <value>The logical connector.</value>
    
    [DisplayName("Logical Connector")]
    public string LogicalConnector
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the rule conditions.</summary>
    ///
    /// <value>The identifier of the rule conditions.</value>
    
    [DisplayName("Business Rule")]
    public Nullable<long> RuleConditionsID
    {
        get;
        set;
    }
    
    [DisplayName("Dynamic Rule")]
    public Nullable<long> DynamicRuleConditionsID
    {
        get;
        set;
    }
    
    [DisplayName("Export Detail")]
    public Nullable<long> ExportDetailConditionsID
    {
        get;
        set;
    }
    //  [ForeignKey("RuleConditionsID")]
    [JsonIgnore]
    
    //[ScriptIgnore]
    
    /// <summary>Gets or sets the ruleconditions.</summary>
    ///
    /// <value>The ruleconditions.</value>
    
    public virtual BusinessRule ruleconditions
    {
        get;
        set;
    }
    public virtual DynamicRoleMapping dynamicruleconditions
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [DisplayName("DisplayValue")]
    public string DisplayValue
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        return Convert.ToString(this.PropertyName);
    }
}
}

