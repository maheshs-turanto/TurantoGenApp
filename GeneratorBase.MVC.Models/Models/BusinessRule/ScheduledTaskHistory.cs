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
/// <summary>A scheduled task history.</summary>
[Table("tbl_ScheduledTaskHistory")]
public class ScheduledTaskHistory
{
    /// <summary>Default constructor.</summary>
    public ScheduledTaskHistory()
    {
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the task.</summary>
    ///
    /// <value>The name of the task.</value>
    
    [DisplayName("TaskName")]
    [Required]
    public string TaskName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the status.</summary>
    ///
    /// <value>The status.</value>
    
    [DisplayName("Status")]
    public string Status
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a unique identifier.</summary>
    ///
    /// <value>The identifier of the unique.</value>
    
    [DisplayName("GUID")]
    public string GUID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets URI of the callback.</summary>
    ///
    /// <value>The callback URI.</value>
    
    [DisplayName("CallbackUri")]
    public string CallbackUri
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the business rule.</summary>
    ///
    /// <value>The identifier of the business rule.</value>
    
    [DisplayName("BusinessRuleId")]
    public long? BusinessRuleId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [DisplayName("Description")]
    public string Description
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
    
    /// <summary>Gets or sets the run date time.</summary>
    ///
    /// <value>The run date time.</value>
    
    [DisplayName("RunDateTime")]
    public string RunDateTime
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
        return Convert.ToString(this.TaskName);
    }
}
}

