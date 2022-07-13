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
/// <summary>The reports group.</summary>
[Table("tbl_ReportsGroup"),CustomDisplayName("ReportsGroup", "ReportsGroup.resx", "Reports Group")]
public partial class ReportsGroup : Entity
{
    /// <summary>Default constructor.</summary>
    public ReportsGroup()
    {
        this.reportsgroupssrsreportassociation = new HashSet<ReportList>();
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    [CustomDisplayName("Name","ReportsGroup.resx","Name"), Column("Name")] [Required]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [CustomDisplayName("Description","ReportsGroup.resx","Description"), Column("Description")]
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display order.</summary>
    ///
    /// <value>The display order.</value>
    
    [CustomDisplayName("DisplayOrder","ReportsGroup.resx","DisplayOrder"), Column("DisplayOrder")]
    public Nullable<Int32> DisplayOrder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the reportsgroupssrsreportassociation.</summary>
    ///
    /// <value>The reportsgroupssrsreportassociation.</value>
    
    [JsonIgnore]
    public virtual ICollection<ReportList> reportsgroupssrsreportassociation
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
            var dispValue = (this.Name != null ?Convert.ToString(this.Name)+" " : Convert.ToString(this.Name));
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
            var dispValue = (this.Name != null ?Convert.ToString(this.Name)+" " : Convert.ToString(this.Name));
            return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
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

