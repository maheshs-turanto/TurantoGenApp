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
/// <summary>A custom reports.</summary>
[Table("tbl_CustomReports"), CustomDisplayName("CustomReports", "CustomReports.resx", "CustomReports")]
public class CustomReports : EntityDefault
{
    /// <summary>Gets or sets the entity values.</summary>
    ///
    /// <value>The entity values.</value>
    
    [CustomDisplayName("EntityValues", "CustomReports.resx", "EntityValues"), Column("EntityValues")]
    public string EntityValues
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the cross tab property values.</summary>
    ///
    /// <value>The cross tab property values.</value>
    
    [CustomDisplayName("CrossTabPropertyValues", "CustomReports.resx", "CrossTabPropertyValues"), Column("CrossTabPropertyValues")]
    public string CrossTabPropertyValues
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the query condition values.</summary>
    ///
    /// <value>The query condition values.</value>
    
    [CustomDisplayName("QueryConditionValues", "CustomReports.resx", "QueryConditionValues"), Column("QueryConditionValues")]
    public string QueryConditionValues
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the relations values.</summary>
    ///
    /// <value>The relations values.</value>
    
    [CustomDisplayName("RelationsValues", "CustomReports.resx", "RelationsValues"), Column("RelationsValues")]
    public string RelationsValues
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("OtherValues", "CustomReports.resx", "OtherValues"), Column("OtherValues")]
    
    /// <summary>Gets or sets the other values.</summary>
    ///
    /// <value>The other values.</value>
    
    public string OtherValues
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("ReportName", "CustomReports.resx", "Report Name"), Column("ReportName")]
    [Required]
    
    /// <summary>Gets or sets the name of the report.</summary>
    ///
    /// <value>The name of the report.</value>
    
    public string ReportName
    {
        get;
        set;
    }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    
    [CustomDisplayName("CreatedOn", "CustomReports.resx", "Created On"), Column("CreatedOn")]
    [Required]
    
    /// <summary>Gets or sets the Date/Time of the created on.</summary>
    ///
    /// <value>The created on.</value>
    
    public DateTime CreatedOn
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("ReportType", "CustomReports.resx", "Report Type"), Column("ReportType")]
    [Required]
    
    /// <summary>Gets or sets the type of the report.</summary>
    ///
    /// <value>The type of the report.</value>
    
    public string ReportType
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("Description", "CustomReports.resx", "Description"), Column("Description")]
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    public string Description
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("EntityName", "CustomReports.resx", "Entity Name"), Column("EntityName")]
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    public string EntityName
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("ResultProperty", "CustomReports.resx", "Result Property"), Column("ResultProperty")]
    
    /// <summary>Gets or sets the result property.</summary>
    ///
    /// <value>The result property.</value>
    
    public string ResultProperty
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("ColumnOrder", "CustomReports.resx", "Column Order"), Column("ColumnOrder")]
    
    /// <summary>Gets or sets the column order.</summary>
    ///
    /// <value>The column order.</value>
    
    public Nullable<Int32> ColumnOrder
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("OrderBy", "CustomReports.resx", "Order By"), Column("OrderBy")]
    
    /// <summary>Gets or sets who order this object.</summary>
    ///
    /// <value>Describes who order this object.</value>
    
    public string OrderBy
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("GroupBy", "CustomReports.resx", "Group By"), Column("GroupBy")]
    
    /// <summary>Gets or sets the amount to group by.</summary>
    ///
    /// <value>Amount to group by.</value>
    
    public Boolean? GroupBy
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("CrossTabRow", "CustomReports.resx", "Cross Tab Row"), Column("CrossTabRow")]
    
    /// <summary>Gets or sets the cross tab row.</summary>
    ///
    /// <value>The cross tab row.</value>
    
    public string CrossTabRow
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("CrossTabColumn", "CustomReports.resx", "Cross Tab Column"), Column("CrossTabColumn")]
    
    /// <summary>Gets or sets the cross tab column.</summary>
    ///
    /// <value>The cross tab column.</value>
    
    public string CrossTabColumn
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("AggregateEntity", "CustomReports.resx", "Aggregate Entity"), Column("AggregateEntity")]
    
    /// <summary>Gets or sets the aggregate entity.</summary>
    ///
    /// <value>The aggregate entity.</value>
    
    public string AggregateEntity
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("AggregateProperty", "CustomReports.resx", "Aggregate Property"), Column("AggregateProperty")]
    
    /// <summary>Gets or sets the aggregate property.</summary>
    ///
    /// <value>The aggregate property.</value>
    
    public string AggregateProperty
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("AggregateFunction", "CustomReports.resx", "Aggregate Function"), Column("AggregateFunction")]
    
    /// <summary>Gets or sets the aggregate function.</summary>
    ///
    /// <value>The aggregate function.</value>
    
    public string AggregateFunction
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("FilterProperty", "CustomReports.resx", "Filter Property"), Column("FilterProperty")]
    
    /// <summary>Gets or sets the filter property.</summary>
    ///
    /// <value>The filter property.</value>
    
    public string FilterProperty
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("FilterCondition", "CustomReports.resx", "Filter Condition"), Column("FilterCondition")]
    
    /// <summary>Gets or sets the filter condition.</summary>
    ///
    /// <value>The filter condition.</value>
    
    public string FilterCondition
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("FilterType", "CustomReports.resx", "Filter Type"), Column("FilterType")]
    
    /// <summary>Gets or sets the type of the filter.</summary>
    ///
    /// <value>The type of the filter.</value>
    
    public string FilterType
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("FilterValue", "CustomReports.resx", "Filter Value"), Column("FilterValue")]
    
    /// <summary>Gets or sets the filter value.</summary>
    ///
    /// <value>The filter value.</value>
    
    public string FilterValue
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("SelectValueFromList", "CustomReports.resx", "Select Value From List"), Column("SelectValueFromList")]
    
    /// <summary>Gets or sets a list of select value froms.</summary>
    ///
    /// <value>A list of select value froms.</value>
    
    public string SelectValueFromList
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("SelectProperty", "CustomReports.resx", "Select Property"), Column("SelectProperty")]
    
    /// <summary>Gets or sets the select property.</summary>
    ///
    /// <value>The select property.</value>
    
    public string SelectProperty
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("RelatedEntity", "CustomReports.resx", "Related Entity"), Column("RelatedEntity")]
    
    /// <summary>Gets or sets the related entity.</summary>
    ///
    /// <value>The related entity.</value>
    
    public string RelatedEntity
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("ForeignKeyEntity", "CustomReports.resx", "ForeignKey Entity"), Column("ForeignKeyEntity")]
    
    /// <summary>Gets or sets the foreign key entity.</summary>
    ///
    /// <value>The foreign key entity.</value>
    
    public string ForeignKeyEntity
    {
        get;
        set;
    }
    
    
    
    [CustomDisplayName("RelationName", "CustomReports.resx", "Relation Name"), Column("RelationName")]
    
    /// <summary>Gets or sets the name of the relation.</summary>
    ///
    /// <value>The name of the relation.</value>
    
    public string RelationName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the created by user.</summary>
    ///
    /// <value>The identifier of the created by user.</value>
    
    [CustomDisplayName("CreatedByUserID", "CustomReports.resx", "Created By"), Column("CreatedByUserID")]
    public string CreatedByUserID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the createdbyuser.</summary>
    ///
    /// <value>The createdbyuser.</value>
    
    public virtual IdentityUser createdbyuser
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
            var dispValue = (this.ReportName != null ? Convert.ToString(this.ReportName) + "-" : Convert.ToString(this.ReportName)) + (this.ReportType != null ? Convert.ToString(this.ReportType) + "-" : Convert.ToString(this.ReportType));
            dispValue = dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
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
            var dispValue = (this.ReportName != null ? Convert.ToString(this.ReportName) + "-" : Convert.ToString(this.ReportName)) + (this.ReportType != null ? Convert.ToString(this.ReportType) + "-" : Convert.ToString(this.ReportType));
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
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

