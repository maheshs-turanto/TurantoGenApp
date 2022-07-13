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
/// <summary>A data source parameters (External WEB_API).</summary>
[Table("tbl_DataSourceParameters"),DisplayName("Data Source Parameters")]
public class DataSourceParameters : EntityDefault
{



    [DisplayName("Argument Name"), Column("ArgumentName")] [Required]
    
    /// <summary>Gets or sets the name of the argument.</summary>
    ///
    /// <value>The name of the argument.</value>
    
    public string ArgumentName
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Argument Value"), Column("ArgumentValue")]
    
    /// <summary>Gets or sets the argument value.</summary>
    ///
    /// <value>The argument value.</value>
    
    public string ArgumentValue
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Hosting Entity"), Column("HostingEntity")]
    
    /// <summary>Gets or sets the hosting entity.</summary>
    ///
    /// <value>The hosting entity.</value>
    
    public string HostingEntity
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Disable"), Column("flag")]
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The flag.</value>
    
    public Boolean? flag
    {
        get;
        set;
    }
    
    
    
    [DisplayName("other"), Column("other")]
    
    /// <summary>Gets or sets the other.</summary>
    ///
    /// <value>The other.</value>
    
    public string other
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the entity data source parameters.</summary>
    ///
    /// <value>The identifier of the entity data source parameters.</value>
    
    [DisplayName("Entity Data Source"), Column("EntityDataSourceParametersID")]
    public Nullable<long> EntityDataSourceParametersID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entitydatasourceparameters.</summary>
    ///
    /// <value>The entitydatasourceparameters.</value>
    
    public virtual EntityDataSource entitydatasourceparameters
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
            var dispValue = (this.ArgumentName != null ?Convert.ToString(this.ArgumentName)+"  " : Convert.ToString(this.ArgumentName));
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
            var dispValue = (this.ArgumentName != null ?Convert.ToString(this.ArgumentName)+"  " : Convert.ToString(this.ArgumentName));
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

