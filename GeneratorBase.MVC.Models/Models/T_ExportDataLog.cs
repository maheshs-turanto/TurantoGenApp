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
/// <summary>Export Data Log model class: Export Data Log.</summary>
[Table("tbl_ExportDataLog"), CustomDisplayName("T_ExportDataLog", "T_ExportDataLog.resx", "Export Data Log")]
[Description("Export Data Log")]
public partial class T_ExportDataLog : Entity
{
    [PropertyTypeForEntity(null, true)]
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo", "T_ExportDataLog.resx", "Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public Nullable<long> T_AutoNo
    {
        get;
        set;
    }
    [PropertyTypeForEntity(null, true)]
    
    /// <summary>Gets or sets the Tag.</summary>
    ///
    /// <value>The T_Tag.</value>
    [CustomDisplayName("T_Tag", "T_ExportDataLog.resx", "Tag"), Column("Tag")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public string T_Tag
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Notes.</summary>
    ///
    /// <value>The T_Notes.</value>
    [CustomDisplayName("T_Notes", "T_ExportDataLog.resx", "Notes"), Column("Notes")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public string T_Notes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Notes.</summary>
    ///
    /// <value>The T_Notes.</value>
    [CustomDisplayName("T_Summary", "T_ExportDataLog.resx", "Summary"), Column("Summary")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public string T_Summary
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
    /// <summary>Gets or sets the Export Data Configuration.</summary>
    ///
    /// <value>The T_ExportDataConfigurationExportDataLogAssociation.</value>
    
    /// <summary>Gets or sets the One Drive Url.</summary>
    ///
    /// <value>The T_OneDriveUrl.</value>
    [CustomDisplayName("T_OneDriveUrl", "T_ExportDataLog.resx", "One Drive Url"), Column("OneDriveUrl")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public string T_OneDriveUrl
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Google Drive Url.</summary>
    ///
    /// <value>The T_GoogleDriveUrl.</value>
    [CustomDisplayName("T_GoogleDriveUrl", "T_ExportDataLog.resx", "Google Drive Url"), Column("GoogleDriveUrl")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public string T_GoogleDriveUrl
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("T_ExportDataConfigurationExportDataLogAssociationID", "T_ExportDataLog.resx", "Export Data Configuration"), Column("ExportDataConfigurationExportDataLogAssociationID")]
    [PropertyInfoForEntity("199362", "Basic Information", "Group")]
    public Nullable<long> T_ExportDataConfigurationExportDataLogAssociationID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Export Data Configuration navigation property.</summary>
    ///
    /// <value>The t_exportdataconfigurationexportdatalogassociation object.</value>
    
    public virtual T_ExportDataConfiguration t_exportdataconfigurationexportdatalogassociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Status.</summary>
    ///
    /// <value>The T_AssociatedExportDataLogStatus.</value>
    
    
    
    [CustomDisplayName("T_AssociatedExportDataLogStatusID", "T_ExportDataLog.resx", "Status"), Column("AssociatedExportDataLogStatusID")]
    [PropertyInfoForEntity("199366", "Status Information", "Group")]
    public Nullable<long> T_AssociatedExportDataLogStatusID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Status navigation property.</summary>
    ///
    /// <value>The t_associatedexportdatalogstatus object.</value>
    
    public virtual T_ExportDataLogstatus t_associatedexportdatalogstatus
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
            dispValue = (this.T_AutoNo != null ? Convert.ToString(this.T_AutoNo) + "-" : Convert.ToString(this.T_AutoNo)) + (this.T_Tag != null ? Convert.ToString(this.T_Tag) + "-" : Convert.ToString(this.T_Tag));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
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
            dispValue = (this.T_AutoNo != null ? Convert.ToString(this.T_AutoNo) + "-" : Convert.ToString(this.T_AutoNo)) + (this.T_Tag != null ? Convert.ToString(this.T_Tag) + "-" : Convert.ToString(this.T_Tag));
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
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

