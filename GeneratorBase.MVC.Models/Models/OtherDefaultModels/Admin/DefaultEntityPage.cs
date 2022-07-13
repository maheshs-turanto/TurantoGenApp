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
/// <summary>A default entity page.</summary>
[Table("tbl_DefaultEntityPage")]
public class DefaultEntityPage : EntityDefault
{
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Page Name")]
    [Required]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The roles.</value>
    
    [DisplayName("Roles")]
    public string Roles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the page.</summary>
    ///
    /// <value>The type of the page.</value>
    
    [DisplayName("Page Type")]
    public string PageType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets URL of the page.</summary>
    ///
    /// <value>The page URL.</value>
    
    [DisplayName("Page Url")]
    public string PageUrl
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the other.</summary>
    ///
    /// <value>The other.</value>
    
    [DisplayName("Other")]
    public string Other
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The flag.</value>
    private Boolean? flag = false;
    [DisplayName("Flag")]
    public Boolean? Flag
    {
        get
        {
            return flag;
        }
        set
        {
            flag = value;
        }
    }
    
    /// <summary>Gets or sets the view entity page.</summary>
    ///
    /// <value>The view entity page.</value>
    
    [DisplayName("View Entity  Page")]
    public string ViewEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the list entity page.</summary>
    ///
    /// <value>The list entity page.</value>
    
    [DisplayName("List Entity Page")]
    public string ListEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the edit entity page.</summary>
    ///
    /// <value>The edit entity page.</value>
    
    [DisplayName("Edit Entity Page")]
    public string EditEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the search entity page.</summary>
    ///
    /// <value>The search entity page.</value>
    
    [DisplayName("Search Entity Page")]
    public string SearchEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the create quick entity page.</summary>
    ///
    /// <value>The create quick entity page.</value>
    
    [DisplayName("Create Quick Entity Page")]
    public string CreateQuickEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the create entity page.</summary>
    ///
    /// <value>The create entity page.</value>
    
    [DisplayName("Create Entity Page")]
    public string CreateEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the edit quick entity page.</summary>
    ///
    /// <value>The edit quick entity page.</value>
    
    [DisplayName("Edit Quick Entity Page")]
    public string EditQuickEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the create wizard entity page.</summary>
    ///
    /// <value>The create wizard entity page.</value>
    
    [DisplayName("Create Wizard Entity Page")]
    public string CreateWizardEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the edit wizard entity page.</summary>
    ///
    /// <value>The edit wizard entity page.</value>
    
    [DisplayName("Edit Wizard Entity Page")]
    public string EditWizardEntityPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the home page.</summary>
    ///
    /// <value>The home page.</value>
    
    [DisplayName("Layout/Home Page")]
    public string HomePage
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
            var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) + "" : Convert.ToString(this.EntityName));
            dispValue = dispValue != null ? dispValue.TrimEnd("".ToCharArray()) : "";
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
        if(!string.IsNullOrEmpty(m_DisplayValue))
            return m_DisplayValue;
        return Convert.ToString(this.EntityName);
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

