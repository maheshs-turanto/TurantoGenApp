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
/// <summary>A document.</summary>
[Table("tbl_Document"), CustomDisplayName("Document", null, "Document")]
public class Document:IEntity
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
    /// <summary>Gets or sets the concurrency key.</summary>
    ///
    /// <value>The concurrency key.</value>
    
    [Timestamp]
    [ConcurrencyCheck]
    public byte[] ConcurrencyKey
    {
        get;
        set;
    }
    
    /// <summary>Used to mark an Entity as 'Deleted'.</summary>
    ///
    /// <value>The is deleted.</value>
    
    [DisplayName("IsDeleted")]
    public Boolean? IsDeleted
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the delete date time.</summary>
    ///
    /// <value>The delete date time.</value>
    
    [DisplayName("DeleteDateTime")]
    public DateTime? DeleteDateTime
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Export Data Log Id.</summary>
    ///
    /// <value>The Export Data Log Id.</value>
    
    [DisplayName("ExportDataLogId")]
    public long? ExportDataLogId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the document.</summary>
    ///
    /// <value>The name of the document.</value>
    
    [DisplayName("DocumentName")]
    public string DocumentName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Date/Time of the date created.</summary>
    ///
    /// <value>The date created.</value>
    
    [DisplayName("DateCreated")]
    public DateTime DateCreated
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Date/Time of the date last updated.</summary>
    ///
    /// <value>The date last updated.</value>
    
    [DisplayName("DateLastUpdated")]
    public DateTime DateLastUpdated
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
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [DisplayName("DisplayValue")]
    public string DisplayValue
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the file extension.</summary>
    ///
    /// <value>The file extension.</value>
    
    [DisplayName("FileExtension")]
    public string FileExtension
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the filename of the file.</summary>
    ///
    /// <value>The name of the file.</value>
    
    [DisplayName("FileName")]
    public string FileName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the file size.</summary>
    ///
    /// <value>The size of the file.</value>
    
    [DisplayName("FileSize")]
    public long FileSize
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the mime.</summary>
    ///
    /// <value>The type of the mime.</value>
    
    [DisplayName("MIMEType")]
    public string MIMEType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the searchable text.</summary>
    ///
    /// <value>The searchable text.</value>
    
    [DisplayName("SearchableText")]
    public string SearchableText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the byte.</summary>
    ///
    /// <value>The byte.</value>
    
    [DisplayName("Byte")]
    public byte[] Byte
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entity name value.</summary>
    ///
    /// <value>The EntityName value.</value>
    
    [DisplayName("Entity Name")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the filetype of the file.</summary>
    ///
    /// <value>The type of the file.</value>
    
    [DisplayName("File DataType")]
    public string FileType
    {
        get;
        set;
    }
    
    
    /// <summary>Sets date time to UTC.</summary>
    public void setDateTimeToUTC()
    {
        this.DateLastUpdated = DateTime.UtcNow;
        this.DateCreated = this.DateCreated == null ? DateTime.UtcNow : this.DateCreated;
    }
    /// <summary>Sets the calculation.</summary>
    public void setCalculation()
    {
    }
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        return Convert.ToString(this.DocumentName);
    }
}
}


