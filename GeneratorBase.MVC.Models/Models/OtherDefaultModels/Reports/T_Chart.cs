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
/// <summary>A chart.</summary>
[Table("tbl_Chart"), CustomDisplayName("T_Chart", "T_Chart.resx", "")]
public class T_Chart : EntityDefault
{
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [CustomDisplayName("EntityName", "T_Chart.resx", "Entity Name"), Column("EntityName")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the chart title.</summary>
    ///
    /// <value>The chart title.</value>
    
    [CustomDisplayName("ChartTitle", "T_Chart.resx", "Chart Title"), Column("ChartTitle")]
    public string ChartTitle
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the chart.</summary>
    ///
    /// <value>The type of the chart.</value>
    
    [CustomDisplayName("ChartType", "T_Chart.resx", "Chart Type"), Column("ChartType")]
    public string ChartType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the axis.</summary>
    ///
    /// <value>The x coordinate axis.</value>
    
    [CustomDisplayName("XAxis", "T_Chart.resx", "X Axis"), Column("XAxis")]
    public string XAxis
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the axis.</summary>
    ///
    /// <value>The y coordinate axis.</value>
    
    [CustomDisplayName("YAxis", "T_Chart.resx", "Y Axis"), Column("YAxis")]
    public string YAxis
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the in dash board is shown.</summary>
    ///
    /// <value>True if show in dash board, false if not.</value>
    
    [CustomDisplayName("ShowInDashBoard", "T_Chart.resx", "Show In DashBoard"), Column("ShowInDashBoard")]
    public bool ShowInDashBoard
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : "");
            dispValue = dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : "");
            return dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
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

