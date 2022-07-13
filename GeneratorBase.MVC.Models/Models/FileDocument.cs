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
/// <summary>Document model class: A document or binary object saved by the system..</summary>
[Table("tbl_FileDocument"),CustomDisplayName("FileDocument", "FileDocument.resx", "Document")]
[Description("Normal")]
public partial class FileDocument : Entity
{
    [PropertyTypeForEntity(null,true)]
    [StringLength(255, ErrorMessage = "Document Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Document Name.</summary>
    ///
    /// <value>The DocumentName.</value>
    [CustomDisplayName("DocumentName","FileDocument.resx","Document Name"), Column("DocumentName")] [Required]
    public string DocumentName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The Description.</value>
    [CustomDisplayName("Description","FileDocument.resx","Description"), Column("Description")]
    public string Description
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Attach Document.</summary>
    ///
    /// <value>The AttachDocument.</value>
    [CustomDisplayName("AttachDocument","FileDocument.resx","Attach Document"), Column("AttachDocument")]
    public Nullable<long> AttachDocument
    {
        get;
        set;
    }
    /// <summary>Gets or sets the m_DateCreated (DateTime specific).</summary>
    ///
    /// <value>The m_DateCreated.</value>
    [NotMapped]
    public DateTime m_DateCreated
    {
        get;
        set;
    }
    [CustomDate("01/01/1900", "12/31/9999")]
    [DataType(DataType.DateTime)][CustomDisplayFormat("DateCreated", "FileDocument", "{0:MM/dd/yyyy hh:mm tt}", "MM/DD/YYYY hh:mm A", true,"DateTime")]
    /// <summary>Gets or sets the Created.</summary>
    ///
    /// <value>The DateCreated.</value>
    [CustomDisplayName("DateCreated","FileDocument.resx","Created"), Column("DateCreated")] [Required]
    public DateTime DateCreated
    {
        get;
        set;
    }
    /// <summary>Gets or sets the m_DateLastUpdated (DateTime specific).</summary>
    ///
    /// <value>The m_DateLastUpdated.</value>
    [NotMapped]
    public DateTime m_DateLastUpdated
    {
        get;
        set;
    }
    [CustomDate("01/01/1900", "12/31/9999")]
    [DataType(DataType.DateTime)][CustomDisplayFormat("DateLastUpdated", "FileDocument", "{0:MM/dd/yyyy hh:mm tt}", "MM/DD/YYYY hh:mm A", true,"DateTime")]
    /// <summary>Gets or sets the Last Updated.</summary>
    ///
    /// <value>The DateLastUpdated.</value>
    [CustomDisplayName("DateLastUpdated","FileDocument.resx","Last Updated"), Column("DateLastUpdated")] [Required]
    public DateTime DateLastUpdated
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
            var dispValue="";
            dispValue =(this.DocumentName != null ?Convert.ToString(this.DocumentName)+" " : Convert.ToString(this.DocumentName));
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
            var dispValue="";
            dispValue =(this.DocumentName != null ?Convert.ToString(this.DocumentName)+" " : Convert.ToString(this.DocumentName));
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
        try {  }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
/// <summary>Sets date time to client time (calls with entity object).</summary>

    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
        this.DateCreated =  TimeZoneInfo.ConvertTimeFromUtc(this.DateCreated, this.m_Timezone) ;
        this.DateLastUpdated =  TimeZoneInfo.ConvertTimeFromUtc(this.DateLastUpdated, this.m_Timezone) ;
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
        this.DateCreated =  TimeZoneInfo.ConvertTimeToUtc(this.DateCreated, this.m_Timezone) ;
        this.DateLastUpdated =  TimeZoneInfo.ConvertTimeToUtc(this.DateLastUpdated, this.m_Timezone) ;
    }
}
}

