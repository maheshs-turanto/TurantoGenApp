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
/// <summary>A theme settings.</summary>
public class ThemeSettings
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="Id">          The identifier.</param>
    /// <param name="Name">        The name.</param>
    /// <param name="CssEditor">   The CSS editor.</param>
    /// <param name="IsActive">    True if this object is active, false if not.</param>
    /// <param name="IsDefault">   True if this object is default, false if not.</param>
    /// <param name="DisplayValue">The display value.</param>
    
    public ThemeSettings(long Id, string Name, string CssEditor, bool IsActive,bool IsDefault, string DisplayValue)
    {
        // TODO: Complete member initialization
        this.Id = Id;
        this.Name = Name;
        this.CssEditor = CssEditor;
        this.IsActive = IsActive;
        this.IsDefault = IsDefault;
        this.DisplayValue = DisplayValue;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [DisplayName("Id")]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Theme Name")]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the CSS editor.</summary>
    ///
    /// <value>The CSS editor.</value>
    
    [DisplayName("CSS Editor")]
    [Required]
    public string CssEditor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is active.</summary>
    ///
    /// <value>True if this object is active, false if not.</value>
    
    [DisplayName("IsActive")]
    public bool IsActive
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is default.</summary>
    ///
    /// <value>True if this object is default, false if not.</value>
    
    [DisplayName("IsDefault")]
    public bool IsDefault
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [DisplayName("Display Value")]
    public string DisplayValue
    {
        get;
        set;
    }
}
/// <summary>Class For Mobile Theme.</summary>
public class ThemeSetingMobile
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="Name">    The name.</param>
    /// <param name="CssName"> The name of the CSS.</param>
    /// <param name="IsActive">True if this object is active, false if not.</param>
    
    public ThemeSetingMobile(long Id, string Name, string CssName, bool IsActive)
    {
        // TODO: Complete member initialization
        this.Id = Id;
        this.Name = Name;
        this.CssName = CssName;
        this.IsActive = IsActive;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [DisplayName("Id")]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Theme Name")]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the CSS.</summary>
    ///
    /// <value>The name of the CSS.</value>
    
    [DisplayName("CSS Name")]
    [Required]
    public string CssName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is active.</summary>
    ///
    /// <value>True if this object is active, false if not.</value>
    
    [DisplayName("IsActive")]
    public bool IsActive
    {
        get;
        set;
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
//public interface IThemeSettingsRepository
//{
//    //void EditThemeSettings(ThemeSettings themeSettings);
//    // void AddThemeSettings();
//    void List<ThemeSettings> GetThemeSettings();
//}
}