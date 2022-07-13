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
/// <summary>List of reports.</summary>
[Table("tbl_ReportList"), CustomDisplayName("ReportList", "ReportList.resx", "ReportList")]
public partial class ReportList : Entity
{
    /// <summary>Default constructor.</summary>
    public ReportList()
    {
        if(!this.IsHidden.HasValue)
            this.IsHidden = false;
    }
    
    /// <summary>Gets or sets the report no.</summary>
    ///
    /// <value>The report no.</value>
    
    [CustomDisplayName("ReportNo", "ReportList.resx", "Report No"), Column("ReportNo")]
    public Nullable<Int32> ReportNo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the display.</summary>
    ///
    /// <value>The name of the display.</value>
    
    [StringLength(255, ErrorMessage = "Display Name cannot be longer than 255 characters.")]
    [CustomDisplayName("DisplayName", "ReportList.resx", "Display Name"), Column("DisplayName")]
    [Required]
    public string DisplayName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [CustomDisplayName("Description", "ReportList.resx", "Description"), Column("Description")]
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    [CustomDisplayName("Name", "ReportList.resx", "Name"), Column("Name")]
    [Required]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the report.</summary>
    ///
    /// <value>The identifier of the report.</value>
    
    [StringLength(255, ErrorMessage = "ReportID cannot be longer than 255 characters.")]
    [CustomDisplayName("ReportID", "ReportList.resx", "ReportID"), Column("ReportID")]
    public string ReportID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the full pathname of the report file.</summary>
    ///
    /// <value>The full pathname of the report file.</value>
    
    [CustomDisplayName("ReportPath", "ReportList.resx", "Report Path"), Column("ReportPath")]
    public string ReportPath
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type.</summary>
    ///
    /// <value>The type.</value>
    
    [StringLength(255, ErrorMessage = "Type cannot be longer than 255 characters.")]
    [CustomDisplayName("Type", "ReportList.resx", "Name"), Column("Type")]
    public string Type
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the is hidden.</summary>
    ///
    /// <value>The is hidden.</value>
    
    [CustomDisplayName("IsHidden", "ReportList.resx", "Is Hidden"), Column("IsHidden")]
    public Boolean? IsHidden
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    [CustomDisplayName("EntityName", "ReportList.resx", "Entity Name"), Column("EntityName")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the reports group ssrs report association.</summary>
    ///
    /// <value>The identifier of the reports group ssrs report association.</value>
    
    [CustomDisplayName("ReportsGroupSSRSReportAssociationID", "ReportList.resx", "Group"), Column("ReportsGroupSSRSReportAssociationID")]
    public Nullable<long> ReportsGroupSSRSReportAssociationID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the reportsgroupssrsreportassociation.</summary>
    ///
    /// <value>The reportsgroupssrsreportassociation.</value>
    
    public virtual ReportsGroup reportsgroupssrsreportassociation
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        try
        {
            var dispValue = (this.DisplayName != null ? Convert.ToString(this.DisplayName) + "-" : Convert.ToString(this.DisplayName));
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.DisplayName != null ? Convert.ToString(this.DisplayName) + "-" : Convert.ToString(this.DisplayName));
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets the calculation.</summary>
    public void setCalculation()
    {
    }
    /// <summary>Sets date time to client time.</summary>
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC.</summary>
    public void setDateTimeToUTC()
    {
    }
}
}

