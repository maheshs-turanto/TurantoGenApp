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
/// <summary>A faceted search.</summary>
[Table("tbl_FacetedSearch"), CustomDisplayName("T_FacetedSearch", "T_FacetedSearch.resx", "Faceted Search")]
public partial class T_FacetedSearch : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public T_FacetedSearch()
    {
        if(!this.T_Disable.HasValue)
            this.T_Disable = false;
        if(!this.T_Flag.HasValue)
            this.T_Flag = false;
        if(string.IsNullOrEmpty(this.T_Roles))
            this.T_Roles = "All";
        this.t_associatedfacetedsearch = new HashSet<T_DataMetric>();
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The t name.</value>
    
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    [CustomDisplayName("T_Name", "T_FacetedSearch.resx", "Name"), Column("Name")]
    [Required]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The t description.</value>
    
    [CustomDisplayName("T_Description", "T_FacetedSearch.resx", "Description"), Column("Description")]
    public string T_Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [CustomDisplayName("T_EntityName", "T_FacetedSearch.resx", "EntityName"), Column("EntityName")]
    public string T_EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The t roles.</value>
    
    [CustomDisplayName("T_Roles", "T_FacetedSearch.resx", "Roles"), Column("Roles")]
    [Required]
    public string T_Roles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the disable.</summary>
    ///
    /// <value>The t disable.</value>
    
    [CustomDisplayName("T_Disable", "T_FacetedSearch.resx", "Disable"), Column("Disable")]
    public Boolean? T_Disable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the link address.</summary>
    ///
    /// <value>The t link address.</value>
    
    [CustomDisplayName("T_LinkAddress", "T_FacetedSearch.resx", "LinkAddress"), Column("LinkAddress")]
    public string T_LinkAddress
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the tab.</summary>
    ///
    /// <value>The name of the tab.</value>
    
    [CustomDisplayName("T_TabName", "T_FacetedSearch.resx", "EntityName"), Column("TabName")]
    public string T_TabName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets target entity.</summary>
    ///
    /// <value>The t target entity.</value>
    
    [CustomDisplayName("T_TargetEntity", "T_FacetedSearch.resx", "TargetEntity"), Column("TargetEntity")]
    public string T_TargetEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the association.</summary>
    ///
    /// <value>The name of the association.</value>
    
    [CustomDisplayName("T_AssociationName", "T_FacetedSearch.resx", "Association Name"), Column("AssociationName")]
    public string T_AssociationName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets information describing the other.</summary>
    ///
    /// <value>Information describing the other.</value>
    
    [CustomDisplayName("T_OtherInfo", "T_FacetedSearch.resx", "OtherInfo"), Column("OtherInfo")]
    public string T_OtherInfo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The t flag.</value>
    
    [CustomDisplayName("T_Flag", "T_FacetedSearch.resx", "Flag"), Column("Flag")]
    public Boolean? T_Flag
    {
        get;
        set;
    }
    [JsonIgnore]
    public virtual ICollection<T_DataMetric> t_associatedfacetedsearch
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
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + "-" : Convert.ToString(this.T_Name)) + (this.T_EntityName != null ? Convert.ToString(ModelReflector.Entities.FirstOrDefault(p => p.Name == this.T_EntityName).DisplayName) + " " : Convert.ToString(this.T_EntityName));
            dispValue = dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
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
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + "-" : Convert.ToString(this.T_Name)) + (this.T_EntityName != null ? Convert.ToString(ModelReflector.Entities.FirstOrDefault(p => p.Name == this.T_EntityName).DisplayName) + " " : Convert.ToString(this.T_EntityName));
            return dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
            //return m_DisplayValue;
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
