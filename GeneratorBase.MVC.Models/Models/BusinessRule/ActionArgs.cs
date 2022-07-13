using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
/// <summary>Arguments for action.</summary>
[Table("tbl_ActionArgs")]
public class ActionArgs
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
    
    /// <summary>Gets or sets the name of the parameter.</summary>
    ///
    /// <value>The name of the parameter.</value>
    
    [DisplayName("Parameter Name")]
    [Required]
    public string ParameterName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the parameter value.</summary>
    ///
    /// <value>The parameter value.</value>
    
    [DisplayName("Parameter Value")]
    [Required]
    public string ParameterValue
    {
        get;
        set;
    }
    // web notification changes
    /// <summary>Gets or sets the Is Email Notification value.</summary>
    ///
    /// <value>The Is Email Notification.</value>
    [NotMapped]
    [DisplayName("Is Email Notification")]
    public bool IsEmailNotification
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Is Web Notification value.</summary>
    ///
    /// <value>The Is Web Notification.</value>
    [NotMapped]
    [DisplayName("Is System Notification")]
    public bool IsWebNotification
    {
        get;
        set;
    }
    //changes
    
    /// <summary>Gets or sets the identifier of the action arguments.</summary>
    ///
    /// <value>The identifier of the action arguments.</value>
    
    [DisplayName("Action")]
    public Nullable<long> ActionArgumentsID
    {
        get;
        set;
    }
    //  [ForeignKey("ActionArgumentsID")]
    
    /// <summary>Gets or sets the actionarguments.</summary>
    ///
    /// <value>The actionarguments.</value>
    
    public virtual RuleAction actionarguments
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
        return Convert.ToString(this.ParameterName);
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

