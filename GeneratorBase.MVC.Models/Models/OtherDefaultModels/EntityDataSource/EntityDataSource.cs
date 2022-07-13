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
/// <summary>An entity data source (External WEB_API).</summary>
[Table("tbl_EntityDataSource"), DisplayName("Entity Data Source")]
public class EntityDataSource : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public EntityDataSource()
    {
        this.entitydatasourceparameters = new HashSet<DataSourceParameters>();
        this.entitypropertymapping = new HashSet<PropertyMapping>();
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Entity Name"), Column("EntityName")]
    [Required]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the data source.</summary>
    ///
    /// <value>The data source.</value>
    
    [DisplayName("Data Source"), Column("DataSource")]
    public string DataSource
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the source.</summary>
    ///
    /// <value>The type of the source.</value>
    
    [DisplayName("Source Type"), Column("SourceType")]
    public string SourceType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the method.</summary>
    ///
    /// <value>The type of the method.</value>
    
    [DisplayName("Method Type"), Column("MethodType")]
    public string MethodType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the action.</summary>
    ///
    /// <value>The action.</value>
    
    [DisplayName("Action"), Column("Action")]
    public string Action
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The flag.</value>
    
    [DisplayName("Disable"), Column("flag")]
    public Boolean? flag
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the other.</summary>
    ///
    /// <value>The other.</value>
    
    [DisplayName("APIKey"), Column("other")]
    public string other
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the root node.</summary>
    ///
    /// <value>The root node.</value>
    
    [DisplayName("Root Node"), Column("RootNode")]
    public string RootNode
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entitydatasourceparameters.</summary>
    ///
    /// <value>The entitydatasourceparameters.</value>
    
    public virtual ICollection<DataSourceParameters> entitydatasourceparameters
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entitypropertymapping.</summary>
    ///
    /// <value>The entitypropertymapping.</value>
    
    public virtual ICollection<PropertyMapping> entitypropertymapping
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) + " - " : Convert.ToString(this.EntityName)) + (this.SourceType != null ? Convert.ToString(this.SourceType) + " - " : Convert.ToString(this.SourceType)) + (this.Action != null ? Convert.ToString(this.Action) + " - " : Convert.ToString(this.Action));
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) + " - " : Convert.ToString(this.EntityName)) + (this.SourceType != null ? Convert.ToString(this.SourceType) + " - " : Convert.ToString(this.SourceType)) + (this.Action != null ? Convert.ToString(this.Action) + " - " : Convert.ToString(this.Action));
            return dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
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