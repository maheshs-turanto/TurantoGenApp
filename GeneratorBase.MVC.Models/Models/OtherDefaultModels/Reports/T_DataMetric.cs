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
/// <summary>Data Metric model class: Data Metric.</summary>
[Table("tbl_DataMetric"), CustomDisplayName("T_DataMetric", "T_DataMetric.resx", "Data Metric")]
public partial class T_DataMetric : Entity
{
    /// <summary>Default constructor for Data Metric.</summary>
    public T_DataMetric()
    {
        if(!this.T_Hide.HasValue)
            this.T_Hide = false;
    }
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name", "T_DataMetric.resx", "Name"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("55862", "Basic Information", "Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the ToolTip.</summary>
    ///
    /// <value>The T_ToolTip.</value>
    [CustomDisplayName("T_ToolTip", "T_DataMetric.resx", "ToolTip"), Column("ToolTip")]
    [PropertyInfoForEntity("55862", "Basic Information", "Group")]
    public string T_ToolTip
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Aggregate cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Aggregate.</summary>
    ///
    /// <value>The T_Aggregate.</value>
    [CustomDisplayName("T_Aggregate", "T_DataMetric.resx", "Aggregate"), Column("Aggregate")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public string T_Aggregate
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Entity Name.</summary>
    ///
    /// <value>The T_EntityName.</value>
    [CustomDisplayName("T_EntityName", "T_DataMetric.resx", "Entity Name"), Column("EntityName")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public string T_EntityName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Aggregate Property Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Aggregate Property Name.</summary>
    ///
    /// <value>The T_AggregatePropertyName.</value>
    [CustomDisplayName("T_AggregatePropertyName", "T_DataMetric.resx", "Aggregate Property Name"), Column("AggregatePropertyName")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public string T_AggregatePropertyName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Roles cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Roles.</summary>
    ///
    /// <value>The T_Roles.</value>
    [CustomDisplayName("T_Roles", "T_DataMetric.resx", "Roles"), Column("Roles")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public string T_Roles
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Background Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Background Color.</summary>
    ///
    /// <value>The T_BackGroundColor.</value>
    [CustomDisplayName("T_BackGroundColor", "T_DataMetric.resx", "Background Color"), Column("BackGroundColor")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
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
    [CustomDisplayName("T_FontColor", "T_DataMetric.resx", "Font Color"), Column("FontColor")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    [PropertyUIInfoType("fgcolor")]
    public string T_FontColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Hide?.</summary>
    ///
    /// <value>The T_Hide.</value>
    [CustomDisplayName("T_Hide", "T_DataMetric.resx", "Hide?"), Column("Hide")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public Boolean? T_Hide
    {
        get;
        set;
    }
    
    [StringLength(255, ErrorMessage = "Class Icon cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Class Icon.</summary>
    ///
    /// <value>The T_ClassIcon.</value>
    [CustomDisplayName("T_ClassIcon", "T_DataMetric.resx", "Class Icon"), Column("ClassIcon")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public string T_ClassIcon
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Background Image.</summary>
    ///
    /// <value>The T_BackgroundImage.</value>
    [PropertyTypeForEntity("Image", false)]
    [CustomDisplayName("T_BackgroundImage", "T_DataMetric.resx", "Background Image"), Column("BackgroundImage")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public Nullable<long> T_BackgroundImage
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Graph Type.</summary>
    ///
    /// <value>The Graph Type.</value>
    [CustomDisplayName("T_GraphType", "T_DataMetric.resx", "Graph Type"), Column("GraphType")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public string T_GraphType
    {
        get;
        set;
    }
    /// <summary>Gets or sets the QueryUrl.</summary>
    ///
    /// <value>The QueryUrl.</value>
    [CustomDisplayName("T_QueryUrl", "T_DataMetric.resx", "Query Url"), Column("QueryUrl")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public string T_QueryUrl
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Data Metric Type.</summary>
    ///
    /// <value>The T_AssociatedDataMetricType.</value>
    
    
    
    [CustomDisplayName("T_AssociatedDataMetricTypeID", "T_DataMetric.resx", "Data Metric Type"), Column("AssociatedDataMetricTypeID")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public Nullable<long> T_AssociatedDataMetricTypeID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Data Metric Type navigation property.</summary>
    ///
    /// <value>The t_associateddatametrictype object.</value>
    
    public virtual T_DataMetrictype t_associateddatametrictype
    {
        get;
        set;
    }
    [CustomDisplayName("T_AssociatedFacetedSearchID", "T_DataMetric.resx", "Query"), Column("AssociatedFacetedSearchID")]
    [PropertyInfoForEntity("55863", "Metric Display Information", "Group")]
    public Nullable<long> T_AssociatedFacetedSearchID
    {
        get;
        set;
    }
    public virtual T_FacetedSearch t_associatedfacetedsearch
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Display Order.</summary>
    ///
    /// <value>The T_DisplayOrder.</value>
    [CustomDisplayName("T_DisplayOrder", "T_DataMetric.resx", "Display Order"), Column("DisplayOrder")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public Nullable<Int32> T_DisplayOrder
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Display On cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Display On.</summary>
    ///
    /// <value>The T_DisplayOn.</value>
    [CustomDisplayName("T_DisplayOn", "T_DataMetric.resx", "Display On"), Column("DisplayOn")]
    [PropertyInfoForEntity("55865", "UI Information", "Group")]
    public string T_DisplayOn
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
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
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
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
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

