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
/// <summary>Property Validation and Format model class: Property Validation and Format.</summary>
[Table("tbl_PropertyValidationandFormat")]
public partial class PropertyValidationandFormat : Entity
{
    /// <summary>Default constructor for Property Validation and Format.</summary>
    public PropertyValidationandFormat()
    {
        if(!this.IsEnabled.HasValue)
            this.IsEnabled = false;
    }
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Entity Name.</summary>
    ///
    /// <value>The EntityName.</value>
    [CustomDisplayName("EntityName","PropertyValidationandFormat.resx","Entity Name"), Column("EntityName")] [Required]
    public string EntityName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Property Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Property Name.</summary>
    ///
    /// <value>The PropertyName.</value>
    [CustomDisplayName("PropertyName","PropertyValidationandFormat.resx","Property Name"), Column("PropertyName")] [Required]
    public string PropertyName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "RegEx Pattern cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the RegEx Pattern.</summary>
    ///
    /// <value>The RegExPattern.</value>
    [CustomDisplayName("RegExPattern","PropertyValidationandFormat.resx","RegEx Pattern"), Column("RegExPattern")]
    public string RegExPattern
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Mask Pattern.</summary>
    ///
    /// <value>The MaskPattern.</value>
    [StringLength(255, ErrorMessage = "Mask Pattern cannot be longer than 255 characters.")]
    [CustomDisplayName("MaskPattern", "PropertyValidationandFormat.resx", "Mask Pattern"), Column("MaskPattern")]
    public string MaskPattern
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Error Message cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Error Message.</summary>
    ///
    /// <value>The ErrorMessage.</value>
    [CustomDisplayName("ErrorMessage","PropertyValidationandFormat.resx","Error Message"), Column("ErrorMessage")]
    public string ErrorMessage
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Lower Bound.</summary>
    ///
    /// <value>The LowerBound.</value>
    [CustomDisplayName("LowerBound","PropertyValidationandFormat.resx","Lower Bound"), Column("LowerBound")]
    [StringLength(255, ErrorMessage = "Lower Bound cannot be longer than 255 characters.")]
    public string LowerBound
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Upper Bound.</summary>
    ///
    /// <value>The UpperBound.</value>
    [StringLength(255, ErrorMessage = "Upper Bound cannot be longer than 255 characters.")]
    [CustomDisplayName("UpperBound","PropertyValidationandFormat.resx","Upper Bound"), Column("UpperBound")]
    public string UpperBound
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Display Format cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Display Format.</summary>
    ///
    /// <value>The DisplayFormat.</value>
    [CustomDisplayName("DisplayFormat","PropertyValidationandFormat.resx","Validation Display Format"), Column("DisplayFormat")]
    public string DisplayFormat
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Is Enabled.</summary>
    ///
    /// <value>The IsEnabled.</value>
    [CustomDisplayName("IsEnabled","PropertyValidationandFormat.resx","Is Enabled"), Column("IsEnabled")]
    public Boolean? IsEnabled
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Other1 cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Other1.</summary>
    ///
    /// <value>The Other1.</value>
    [CustomDisplayName("Other1","PropertyValidationandFormat.resx","UI Display Format"), Column("Other1")]
    public string Other1
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Other2 cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Other2.</summary>
    ///
    /// <value>The Other2.</value>
    [CustomDisplayName("Other2","PropertyValidationandFormat.resx","Other2"), Column("Other2")]
    public string Other2
    {
        get;
        set;
    }
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = (this.EntityName != null ?Convert.ToString(this.EntityName)+"-" : Convert.ToString(this.EntityName))+(this.PropertyName != null ?Convert.ToString(this.PropertyName)+" " : Convert.ToString(this.PropertyName));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
/// <summary>Gets display value model (Actual value to render on UI).</summary>
///
/// <returns>The display value model.</returns>
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.EntityName != null ?Convert.ToString(this.EntityName)+"-" : Convert.ToString(this.EntityName))+(this.PropertyName != null ?Convert.ToString(this.PropertyName)+" " : Convert.ToString(this.PropertyName));
            return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
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

