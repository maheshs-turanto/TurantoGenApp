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
/// <summary>An application setting.</summary>
[Table("tbl_AppSetting")]
public class AppSetting : EntityDefault
{
    /// <summary>Gets or sets the key.</summary>
    ///
    /// <value>The key.</value>
    
    [Unique(typeof(ApplicationContext), ErrorMessage = "Must be unique")]
    [DisplayName("Key")]
    [Required]
    public string Key
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the value.</summary>
    ///
    /// <value>The value.</value>
    [AllowHtml]
    [DisplayName("Value")]
    [Required]
    public string Value
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
    
    /// <summary>Gets or sets the identifier of the associated application setting group.</summary>
    ///
    /// <value>The identifier of the associated application setting group.</value>
    
    [DisplayName("Group Name")]
    public Nullable<long> AssociatedAppSettingGroupID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedappsettinggroup.</summary>
    ///
    /// <value>The associatedappsettinggroup.</value>
    
    public virtual AppSettingGroup associatedappsettinggroup
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is default.</summary>
    ///
    /// <value>True if this object is default, false if not.</value>
    
    [DisplayName("IsDefault")]
    public bool IsDefault
    {
        get;
        set;
    }
    /// <summary>Amount to last updated by.</summary>
    DateTime? m_LastUpdatedBy = DateTime.UtcNow;
    
    /// <summary>Gets or sets the amount to last updated by.</summary>
    ///
    /// <value>Amount to last updated by.</value>
    
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public Nullable<DateTime> LastUpdatedBy
    {
        get
        {
            return m_LastUpdatedBy;
        }
        set
        {
            m_LastUpdatedBy = value;
        }
    }
    /// <summary>.</summary>
    string m_LastUpdatedByUser = HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : "";
    
    /// <summary>Gets or sets the last updated by user.</summary>
    ///
    /// <value>The last updated by user.</value>
    
    [DisplayName("LastUpdatedByUser")]
    public string LastUpdatedByUser
    {
        get
        {
            return m_LastUpdatedByUser;
        }
        set
        {
            m_LastUpdatedByUser = value;
        }
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        var  dispValue = Convert.ToString(this.Key);
        dispValue = dispValue.TrimEnd(" - ".ToCharArray());
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
        return Convert.ToString(this.Key);
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