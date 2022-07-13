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
/// <summary>Export Data Configuration model class: Export Data Configuration.</summary>
[Table("tbl_ExportDataConfiguration"), CustomDisplayName("T_ExportDataConfiguration", "T_ExportDataConfiguration.resx", "Export Data Configuration")]
[Description("Export Data Configuration")]
public partial class T_ExportDataConfiguration : Entity
{
    /// <summary>Default constructor for Export Data Configuration.</summary>
    public T_ExportDataConfiguration()
    {
        if(!this.T_Disable.HasValue)
            this.T_Disable = false;
        if(!this.T_EnableDelete.HasValue)
            this.T_EnableDelete = false;
        if(!this.T_UploadOneDrive.HasValue)
            this.T_UploadOneDrive = false;
        if(!this.T_UploadGoogleDrive.HasValue)
            this.T_UploadGoogleDrive = false;
        if(!this.T_IsRootDeleted.HasValue)
            this.T_IsRootDeleted = false;
        this.t_exportdataconfigurationexportdatadetailsassociation = new HashSet<T_ExportDataDetails>();
        this.t_exportdataconfigurationexportdatalogassociation = new HashSet<T_ExportDataLog>();
    }
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo", "T_ExportDataConfiguration.resx", "Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Nullable<long> T_AutoNo
    {
        get;
        set;
    }
    [PropertyTypeForEntity(null, true)]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name", "T_ExportDataConfiguration.resx", "Name"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public string T_Name
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Entity Name.</summary>
    ///
    /// <value>The T_EntityName.</value>
    [CustomDisplayName("T_EntityName", "T_ExportDataConfiguration.resx", "Entity Name"), Column("EntityName")]
    [Required]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public string T_EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Allowed Roles.</summary>
    ///
    /// <value>The T_AllowedRoles.</value>
    [CustomDisplayName("T_AllowedRoles", "T_ExportDataConfiguration.resx", "Allowed Roles"), Column("AllowedRoles")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public string T_AllowedRoles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The T_Description.</value>
    [CustomDisplayName("T_Description", "T_ExportDataConfiguration.resx", "Description"), Column("Description")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public string T_Description
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Disable.</summary>
    ///
    /// <value>The T_Disable.</value>
    [CustomDisplayName("T_Disable", "T_ExportDataConfiguration.resx", "Disable"), Column("Disable")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Boolean? T_Disable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Root Delete.</summary>
    ///
    /// <value>The T_IsRootDeleted.</value>
    [CustomDisplayName("T_IsRootDeleted", "T_ExportDataConfiguration.resx", "Delete Root Item?"), Column("IsRootDeleted")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Boolean? T_IsRootDeleted
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Disable.</summary>
    ///
    /// <value>The T_Disable.</value>
    [CustomDisplayName("T_EnableDelete", "T_ExportDataConfiguration.resx", "Enable Delete"), Column("EnableDelete")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Boolean? T_EnableDelete
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Enable Upload One Drive.</summary>
    ///
    /// <value>The T_UploadOneDrive.</value>
    [CustomDisplayName("T_UploadOneDrive", "T_ExportDataConfiguration.resx", "Upload One Drive"), Column("UploadOneDrive")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Boolean? T_UploadOneDrive
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Enable Upload on Google Drive.</summary>
    ///
    /// <value>The T_UploadGoogleDrive.</value>
    [CustomDisplayName("T_UploadGoogleDrive", "T_ExportDataConfiguration.resx", "Upload Google Drive"), Column("UploadGoogleDrive")]
    [PropertyInfoForEntity("186418", "Basic Information", "Group")]
    public Boolean? T_UploadGoogleDrive
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Background Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Background Color.</summary>
    ///
    /// <value>The T_BackGroundColor.</value>
    [CustomDisplayName("T_BackGroundColor", "T_ExportDataConfiguration.resx", "Background Color"), Column("BackGroundColor")]
    [PropertyInfoForEntity("186419", "UI Information", "Group")]
    [PropertyUIInfoType("bgcolor")]
    public string T_BackGroundColor
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Font Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Font Color.</summary>
    ///
    /// <value>The T_FontColor.</value>
    [CustomDisplayName("T_FontColor", "T_ExportDataConfiguration.resx", "Font Color"), Column("FontColor")]
    [PropertyInfoForEntity("186419", "UI Information", "Group")]
    [PropertyUIInfoType("fgcolor")]
    public string T_FontColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Tool Tip.</summary>
    ///
    /// <value>The T_ToolTip.</value>
    [CustomDisplayName("T_ToolTip", "T_ExportDataConfiguration.resx", "Tool Tip"), Column("ToolTip")]
    [PropertyInfoForEntity("186419", "UI Information", "Group")]
    public string T_ToolTip
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
    /// <summary>Gets or sets the t_exportdataconfigurationexportdatadetailsassociation.</summary>
    ///
    /// <value>The ICollection<T_ExportDataDetails>.</value>
    
    [JsonIgnore]
    [DisplayName("Export Data Details")]
    public virtual ICollection<T_ExportDataDetails> t_exportdataconfigurationexportdatadetailsassociation
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the t_exportdataconfigurationexportdatalogassociation.</summary>
    ///
    /// <value>The ICollection<T_ExportDataLog>.</value>
    
    [JsonIgnore]
    [DisplayName("Export Data Log")]
    public virtual ICollection<T_ExportDataLog> t_exportdataconfigurationexportdatalogassociation
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

