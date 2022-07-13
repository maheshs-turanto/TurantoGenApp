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
/// <summary>Export Data Details model class: Export Data Details.</summary>
[Table("tbl_ExportDataDetails"), CustomDisplayName("T_ExportDataDetails", "T_ExportDataDetails.resx", "Export Data Details")]
[Description("Export Data Details")]
public partial class T_ExportDataDetails : Entity
{
    /// <summary>Default constructor for Export Data Details.</summary>
    public T_ExportDataDetails()
    {
        if(!this.T_IsNested.HasValue)
            this.T_IsNested = false;
    }
    [PropertyTypeForEntity(null, true)]
    
    /// <summary>Gets or sets the Entity.</summary>
    ///
    /// <value>The T_ChildEntity.</value>
    [CustomDisplayName("T_ChildEntity", "T_ExportDataDetails.resx", "Entity"), Column("ChildEntity")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public string T_ChildEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Parent Entity.</summary>
    ///
    /// <value>The T_ParentEntity.</value>
    [CustomDisplayName("T_ParentEntity", "T_ExportDataDetails.resx", "Parent Entity"), Column("ParentEntity")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public string T_ParentEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Association Name.</summary>
    ///
    /// <value>The T_AssociationName.</value>
    [CustomDisplayName("T_AssociationName", "T_ExportDataDetails.resx", "Association Name"), Column("AssociationName")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public string T_AssociationName
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Is Nested.</summary>
    ///
    /// <value>The T_IsNested.</value>
    [CustomDisplayName("T_IsNested", "T_ExportDataDetails.resx", "Is Nested"), Column("IsNested")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public Boolean? T_IsNested
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Hierarchy.</summary>
    ///
    /// <value>The T_Hierarchy.</value>
    [CustomDisplayName("T_Hierarchy", "T_ExportDataDetails.resx", "Hierarchy"), Column("Hierarchy")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public string T_Hierarchy
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
    /// <value>The T_ExportDataConfigurationExportDataDetailsAssociation.</value>
    
    
    
    [CustomDisplayName("T_ExportDataConfigurationExportDataDetailsAssociationID", "T_ExportDataDetails.resx", "Export Data Configuration"), Column("ExportDataConfigurationExportDataDetailsAssociationID")]
    [PropertyInfoForEntity("186420", "Basic Information", "Group")]
    public Nullable<long> T_ExportDataConfigurationExportDataDetailsAssociationID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Export Data Configuration navigation property.</summary>
    ///
    /// <value>The t_exportdataconfigurationexportdatadetailsassociation object.</value>
    
    public virtual T_ExportDataConfiguration t_exportdataconfigurationexportdatadetailsassociation
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
            dispValue = (this.T_ChildEntity != null ? Convert.ToString(this.T_ChildEntity) + "-" : Convert.ToString(this.T_ChildEntity));
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
            dispValue = (this.T_ChildEntity != null ? Convert.ToString(this.T_ChildEntity) + "-" : Convert.ToString(this.T_ChildEntity));
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

