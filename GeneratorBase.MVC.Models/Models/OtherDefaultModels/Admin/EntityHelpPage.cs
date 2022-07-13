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
/// <summary>An entity help page.</summary>
[Table("tbl_EntityHelpPage"), CustomDisplayName("EntityHelpPage", "EntityHelpPage.resx", "Entity Help Page")]
public partial class EntityHelpPage : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public EntityHelpPage()
    {
        if(!this.Disable.HasValue)
            this.Disable = false;
    }
    
    /// <summary>Gets or sets the name of the section.</summary>
    ///
    /// <value>The name of the section.</value>
    
    [StringLength(255, ErrorMessage = "Section Name cannot be longer than 255 characters.")]
    [CustomDisplayName("SectionName", "EntityHelpPage.resx", "Section Name"), Column("SectionName")]
    [Required]
    public string SectionName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the order.</summary>
    ///
    /// <value>The order.</value>
    
    [CustomDisplayName("Order", "EntityHelpPage.resx", "Order"), Column("Order")]
    [Required]
    public Int32 Order
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the section text.</summary>
    ///
    /// <value>The section text.</value>
    
    [System.Web.Mvc.AllowHtml]
    [CustomDisplayName("SectionText", "EntityHelpPage.resx", "Section Text"), Column("SectionText")]
    public string SectionText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the disable.</summary>
    ///
    /// <value>The disable.</value>
    
    [CustomDisplayName("Disable", "EntityHelpPage.resx", "Disable"), Column("Disable")]
    public Boolean? Disable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [CustomDisplayName("EntityName", "EntityHelpPage.resx", "Entity Name"), Column("EntityName")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the entity of entity help.</summary>
    ///
    /// <value>The identifier of the entity of entity help.</value>
    
    [CustomValidation(typeof(MandatoryDropdown), "ValidateDropdown")]
    [CustomDisplayName("EntityOfEntityHelpID", "EntityHelpPage.resx", "Entity Name"), Column("EntityOfEntityHelpID")]
    public Nullable<long> EntityOfEntityHelpID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entityofentityhelp.</summary>
    ///
    /// <value>The entityofentityhelp.</value>
    
    public virtual EntityPage entityofentityhelp
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
            var dispValue = (this.EntityOfEntityHelpID != null ? (new ApplicationContext(new SystemUser())).EntityPages.FirstOrDefault(p => p.Id == this.EntityOfEntityHelpID).DisplayValue : "");
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
            var dispValue = (this.entityofentityhelp != null ? entityofentityhelp.DisplayValue : "");
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
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

