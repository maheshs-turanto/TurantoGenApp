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
/// <summary>A property help page.</summary>
[Table("tbl_PropertyHelpPage"), CustomDisplayName("PropertyHelpPage", "PropertyHelpPage.resx", "Property Help Page")]
public partial class PropertyHelpPage : EntityDefault
{

    /// <summary>Default constructor.</summary>
    public PropertyHelpPage()
    {
        if(!this.Disable.HasValue)
            this.Disable = false;
        if(!this.GroupId.HasValue)
            this.GroupId = 0;
    }
    
    /// <summary>Gets or sets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    
    [StringLength(255, ErrorMessage = "Property Name cannot be longer than 255 characters.")]
    [CustomDisplayName("PropertyName", "PropertyHelpPage.resx", "Property Name"), Column("PropertyName")]
    [Required]
    public string PropertyName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the property data.</summary>
    ///
    /// <value>The type of the property data.</value>
    
    [StringLength(255, ErrorMessage = "Property Data Type cannot be longer than 255 characters.")]
    [CustomDisplayName("PropertyDataType", "PropertyHelpPage.resx", "Property Data Type"), Column("PropertyDataType")]
    public string PropertyDataType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the object.</summary>
    ///
    /// <value>The type of the object.</value>
    
    [StringLength(255, ErrorMessage = "Object Type cannot be longer than 255 characters.")]
    [CustomDisplayName("ObjectType ", "PropertyHelpPage.resx", "Property Type "), Column("ObjectType")]
    public string ObjectType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tooltip.</summary>
    ///
    /// <value>The tooltip.</value>
    
    [CustomDisplayName("Tooltip", "PropertyHelpPage.resx", "Tooltip"), Column("Tooltip")]
    public string Tooltip
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the help text.</summary>
    ///
    /// <value>The help text.</value>
    
    [System.Web.Mvc.AllowHtml]
    [CustomDisplayName("HelpText", "PropertyHelpPage.resx", "Help Text"), Column("HelpText")]
    public string HelpText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [CustomDisplayName("EntityName", "PropertyHelpPage.resx", "Entity Name"), Column("EntityName")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the group.</summary>
    ///
    /// <value>The identifier of the group.</value>
    
    [CustomDisplayName("GroupId", "PropertyHelpPage.resx", "Group Id"), Column("GroupId")]
    public int? GroupId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the group.</summary>
    ///
    /// <value>The name of the group.</value>
    
    [CustomDisplayName("GroupName", "PropertyHelpPage.resx", "Group Name"), Column("GroupName")]
    public string GroupName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the disable.</summary>
    ///
    /// <value>The disable.</value>
    
    [CustomDisplayName("Disable", "PropertyHelpPage.resx", "Disable"), Column("Disable")]
    public Boolean? Disable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the property help of entity.</summary>
    ///
    /// <value>The identifier of the property help of entity.</value>
    
    [CustomDisplayName("PropertyHelpOfEntityID", "PropertyHelpPage.resx", "Entity Name"), Column("PropertyHelpOfEntityID")]
    public Nullable<long> PropertyHelpOfEntityID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the propertyhelpofentity.</summary>
    ///
    /// <value>The propertyhelpofentity.</value>
    
    public virtual EntityPage propertyhelpofentity
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
            var dispValue = (this.PropertyHelpOfEntityID != null ? (new ApplicationContext(new SystemUser())).EntityPages.FirstOrDefault(p => p.Id == this.PropertyHelpOfEntityID).DisplayValue : "");
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
            var dispValue = (this.propertyhelpofentity != null ? propertyhelpofentity.DisplayValue : "");
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

