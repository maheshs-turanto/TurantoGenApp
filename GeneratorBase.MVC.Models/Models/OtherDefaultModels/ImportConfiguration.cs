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
/// <summary>An import configuration.</summary>
[Table("tbl_ImportConfiguration")]
public class ImportConfiguration : EntityDefault
{
    /// <summary>Gets or sets the table column.</summary>
    ///
    /// <value>The table column.</value>
    
    [DisplayName("Table Column")]
    public string TableColumn
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sheet column.</summary>
    ///
    /// <value>The sheet column.</value>
    
    [DisplayName("Sheet Column")]
    public string SheetColumn
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the unique column.</summary>
    ///
    /// <value>The unique column.</value>
    
    [DisplayName("Unique Column")]
    public string UniqueColumn
    {
        get;
        set;
    }
    
    
    [DisplayName("Update Column")]
    public string UpdateColumn
    {
        get;
        set;
    }
    /// <summary>The last update.</summary>
    DateTime? m_LastUpdate = DateTime.UtcNow;
    
    /// <summary>Gets or sets the last update.</summary>
    ///
    /// <value>The last update.</value>
    
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public Nullable<DateTime> LastUpdate
    {
        get
        {
            return m_LastUpdate;
        }
        set
        {
            m_LastUpdate = value;
        }
    }
    /// <summary>The last update user.</summary>
    string m_LastUpdateUser = HttpContext.Current.User.Identity.Name;
    
    /// <summary>Gets or sets the last update user.</summary>
    ///
    /// <value>The last update user.</value>
    
    [DisplayName("LastUpdateUser")]
    public string LastUpdateUser
    {
        get
        {
            return m_LastUpdateUser;
        }
        set
        {
            m_LastUpdateUser = value;
        }
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Name")]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the mapping.</summary>
    ///
    /// <value>The name of the mapping.</value>
    
    [DisplayName("MappingName")]
    public string MappingName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is default mapping.</summary>
    ///
    /// <value>True if this object is default mapping, false if not.</value>
    
    [DisplayName("IsDefaultMapping")]
    public bool IsDefaultMapping
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.Name);
        //dispValue = dispValue.TrimEnd(" - ".ToCharArray());
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

