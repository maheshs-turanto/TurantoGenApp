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
/// <summary>A multi tenant extra access.</summary>
[Table("tbl_MultiTenantExtraAccess"), CustomDisplayName("MultiTenantExtraAccess", "MultiTenantExtraAccess.resx", "Application Security Access")]
public class MultiTenantExtraAccess : EntityDefault
{
    [CustomDisplayName("T_User", "MultiTenantExtraAccess.resx", "User"), Column("User")]
    
    /// <summary>Gets or sets the user.</summary>
    ///
    /// <value>The t user.</value>
    
    public string T_User
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the main entity.</summary>
    ///
    /// <value>The t main entity.</value>
    
    [CustomDisplayName("T_MainEntity", "MultiTenantExtraAccess.resx", "MainEntity")]
    [NotMapped]
    public string T_MainEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the main entity.</summary>
    ///
    /// <value>The identifier of the main entity.</value>
    
    [Column("MainEntityID")]
    public long? T_MainEntityID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the main entity value.</summary>
    ///
    /// <value>The t main entity value.</value>
    
    [CustomDisplayName("T_MainEntityValue", "MultiTenantLoginSelected.resx", "MainEntityValue"), Column("MainEntityValue")]
    public string T_MainEntityValue
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        return "";
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    public string DisplayValue
    {
        get;
        set;
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        return "";
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

