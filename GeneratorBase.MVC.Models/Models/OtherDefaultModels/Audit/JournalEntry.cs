using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace GeneratorBase.MVC.Models
{
/// <summary>A journal entry.</summary>
[Table("tbl_JournalEntry")]
public class JournalEntry : EntityDefault
{
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Entity Name")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type.</summary>
    ///
    /// <value>The type.</value>
    
    [DisplayName("Entry Type")]
    public string Type
    {
        get;
        set;
    }
    
/// <summary>Gets or sets the browser Info.</summary>
    ///
    /// <value>The browser Info.</value>
    
    [DisplayName("Browser Info")]
    public string BrowserInfo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Date/Time of the date time of entry.</summary>
    ///
    /// <value>The m date time of entry.</value>
    
    [NotMapped]
    public DateTime m_DateTimeOfEntry
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Date/Time of the date time of entry.</summary>
    ///
    /// <value>The date time of entry.</value>
    
    [DisplayName("Entry DateTime")]
    [CustomDisplayFormat("DateTimeOfEntry", "JournalEntry", "{0:MM/dd/yyyy hh:mm:ss tt}", "MM/DD/YYYY hh:mm:ss A", true)]
    public DateTime DateTimeOfEntry
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets information describing the record.</summary>
    ///
    /// <value>Information describing the record.</value>
    
    [DisplayName("Record Info")]
    public string RecordInfo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    
    [DisplayName("PropertyName")]
    public string PropertyName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the old value.</summary>
    ///
    /// <value>The old value.</value>
    
    [DisplayName("OldValue")]
    public string OldValue
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the new value.</summary>
    ///
    /// <value>The new value.</value>
    
    [DisplayName("NewValue")]
    public string NewValue
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    [DisplayName("User Name")]
    public string UserName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the record.</summary>
    ///
    /// <value>The identifier of the record.</value>
    
    [DisplayName("Record Id")]
    public long RecordId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the role.</summary>
    ///
    /// <value>The name of the role.</value>
    
    [DisplayName("Role Name")]
    public string RoleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the source for the.</summary>
    ///
    /// <value>The source.</value>
    
    [DisplayName("Source")]
    public string Source
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the reason.</summary>
    ///
    /// <value>The reason.</value>
    
    [DisplayName("Reason")]
    public string Reason
    {
        get;
        set;
    }
    
    /// <summary>Sets date time to UTC.</summary>
    public void setDateTimeToUTC()
    {
        this.DateTimeOfEntry = DateTime.UtcNow;
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        return this.EntityName + "-" + this.PropertyName;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        return this.EntityName + "-" + this.PropertyName;
    }
}
}

