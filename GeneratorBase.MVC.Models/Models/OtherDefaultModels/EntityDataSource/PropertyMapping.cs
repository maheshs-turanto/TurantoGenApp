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
/// <summary>A property mapping (External WEB_API).</summary>
[Table("tbl_PropertyMapping"),DisplayName("Property Mapping")]
public class PropertyMapping : EntityDefault
{



    [DisplayName("Property Name"), Column("PropertyName")] [Required]
    
    /// <summary>Gets or sets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    
    public string PropertyName
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Data Name"), Column("DataName")]
    
    /// <summary>Gets or sets the name of the data.</summary>
    ///
    /// <value>The name of the data.</value>
    
    public string DataName
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Data Source"), Column("DataSource")]
    
    /// <summary>Gets or sets the data source.</summary>
    ///
    /// <value>The data source.</value>
    
    public string DataSource
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Source Type"), Column("SourceType")]
    
    /// <summary>Gets or sets the type of the source.</summary>
    ///
    /// <value>The type of the source.</value>
    
    public string SourceType
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Method Type"), Column("MethodType")]
    
    /// <summary>Gets or sets the type of the method.</summary>
    ///
    /// <value>The type of the method.</value>
    
    public string MethodType
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Action"), Column("Action")]
    
    /// <summary>Gets or sets the action.</summary>
    ///
    /// <value>The action.</value>
    
    public string Action
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the entity property mapping.</summary>
    ///
    /// <value>The identifier of the entity property mapping.</value>
    
    [DisplayName("Entity Data Source"), Column("EntityPropertyMappingID")]
    public Nullable<long> EntityPropertyMappingID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entitypropertymapping.</summary>
    ///
    /// <value>The entitypropertymapping.</value>
    
    public virtual EntityDataSource entitypropertymapping
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = (this.PropertyName != null ?Convert.ToString(this.PropertyName)+"  " : Convert.ToString(this.PropertyName));
            dispValue = dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
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
            var dispValue = (this.PropertyName != null ?Convert.ToString(this.PropertyName)+"  " : Convert.ToString(this.PropertyName));
            return dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
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

