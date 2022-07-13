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
/// <summary>An application setting group.</summary>
[Table("tbl_AppSettingGroup")]
public class AppSettingGroup : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public AppSettingGroup()
    {
        this.associatedappsetting = new HashSet<AppSetting>();
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [Unique(typeof(ApplicationContext), ErrorMessage = "Must be unique")]
    [DisplayName("Group Name")]
    [Required]
    public string Name
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
    /// <summary>The last updated by user.</summary>
    string m_LastUpdatedByUser = HttpContext.Current.User.Identity.Name;
    
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
    
    /// <summary>Gets or sets the associatedappsetting.</summary>
    ///
    /// <value>The associatedappsetting.</value>
    
    public virtual ICollection<AppSetting> associatedappsetting
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.Name);
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
        return Convert.ToString(this.Name);
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