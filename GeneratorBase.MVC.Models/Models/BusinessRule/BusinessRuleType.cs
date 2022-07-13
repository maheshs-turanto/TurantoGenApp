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
/// <summary>The business rule type.</summary>
[Table("tbl_BusinessRuleType")]
public class BusinessRuleType
{
    /// <summary>Default constructor.</summary>
    public BusinessRuleType()
    {
        this.associatedbusinessruletype = new HashSet<BusinessRule>();
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
    
    /// <summary>Gets or sets the type no.</summary>
    ///
    /// <value>The type no.</value>
    
    [DisplayName("TypeNo")]
    public Nullable<Int32> TypeNo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the type.</summary>
    ///
    /// <value>The name of the type.</value>
    
    [DisplayName("Type Name")]
    [Required]
    public string TypeName
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
    
    /// <summary>Gets or sets the associatedbusinessruletype.</summary>
    ///
    /// <value>The associatedbusinessruletype.</value>
    
    [JsonIgnore]
    public virtual ICollection<BusinessRule> associatedbusinessruletype
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
        return Convert.ToString(this.TypeName);
    }
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}

