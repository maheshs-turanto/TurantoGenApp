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
/// <summary>ExportDataLog Status model class: ExportDataLog Status.</summary>
[Table("tbl_ExportDataLogstatus"), CustomDisplayName("T_ExportDataLogstatus", "T_ExportDataLogstatus.resx", "ExportDataLog Status")]
[Description("Normal")]
public partial class T_ExportDataLogstatus : Entity
{
    /// <summary>Default constructor for ExportDataLog Status.</summary>
    public T_ExportDataLogstatus()
    {
        this.t_associatedexportdatalogstatus = new HashSet<T_ExportDataLog>();
    }
    [PropertyTypeForEntity(null, true)]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name", "T_ExportDataLogstatus.resx", "Name"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("199365", "Basic Information", "Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The T_Description.</value>
    [CustomDisplayName("T_Description", "T_ExportDataLogstatus.resx", "Description"), Column("Description")]
    [PropertyInfoForEntity("199365", "Basic Information", "Group")]
    public string T_Description
    {
        get;
        set;
    }
    
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
    /// <summary>Gets or sets the t_associatedexportdatalogstatus.</summary>
    ///
    /// <value>The ICollection<T_ExportDataLog>.</value>
    
    [JsonIgnore]
    [DisplayName("ExportDataLogs")]
    public virtual ICollection<T_ExportDataLog> t_associatedexportdatalogstatus
    {
        get;
        set;
    }
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public string getDisplayValue()
    {
        try
        {
            var dispValue = "";
            dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
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
            var dispValue = "";
            dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
            return dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
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

