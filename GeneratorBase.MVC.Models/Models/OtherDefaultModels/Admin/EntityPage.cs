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
/// <summary>An entity page.</summary>
[Table("tbl_EntityPage"), CustomDisplayName("EntityPage", "EntityPage.resx", "Entity Page")]
public partial class EntityPage : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public EntityPage()
    {
        this.entityofentityhelp = new HashSet<EntityHelpPage>();
        this.propertyhelpofentity = new HashSet<PropertyHelpPage>();
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    [Unique(typeof(ApplicationContext), ErrorMessage = "Entity Name is Unique.")]
    [CustomDisplayName("EntityName", "EntityPage.resx", "Entity Name"), Column("EntityName")]
    [Required]
    public string EntityName
    {
        get;
        set;
    }
    /// <summary>Gets or sets the disable.</summary>
    ///
    /// <value>The disable.</value>
    [CustomDisplayName("Disable", "EntityPage.resx", "Disable"), Column("Disable")]
    public Boolean? Disable
    {
        get;
        set;
    }
    /// <summary>Gets or sets the option to cache query.</summary>
    ///
    /// <value>The data chache option.</value>
    [CustomDisplayName("EnableDataCache", "EntityPage.resx", "Enable Data Cache"), Column("EnableDataCache")]
    public Boolean? EnableDataCache
    {
        get;
        set;
    }
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [CustomDisplayName("Description", "EntityPage.resx", "Description "), Column("Description")]
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entityofentityhelp.</summary>
    ///
    /// <value>The entityofentityhelp.</value>
    
    [JsonIgnore]
    public virtual ICollection<EntityHelpPage> entityofentityhelp
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the propertyhelpofentity.</summary>
    ///
    /// <value>The propertyhelpofentity.</value>
    
    [JsonIgnore]
    public virtual ICollection<PropertyHelpPage> propertyhelpofentity
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : Convert.ToString(this.EntityName));
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : Convert.ToString(this.EntityName));
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

