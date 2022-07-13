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
/// <summary>Menu Bar model class: Menu Bar.</summary>
[Table("tbl_MenuBar"), CustomDisplayName("T_MenuBar", "T_MenuBar.resx", "Menu Bar")]
[Description("Menu Bar")]
public partial class T_MenuBar : Entity
{
    /// <summary>Default constructor for Menu Bar.</summary>
    public T_MenuBar()
    {
        if(!this.T_Disabled.HasValue)
            this.T_Disabled = false;
        if(!this.T_Horizontal.HasValue)
            this.T_Horizontal = false;
        this.T_MenuBarMenuItemAssociation_t_menubar = new HashSet<T_MenuBarMenuItemAssociation>();
    }
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo", "T_MenuBar.resx", "Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("208381", "Basic Information", "Group")]
    public Nullable<long> T_AutoNo
    {
        get;
        set;
    }
    [PropertyTypeForEntity(null, true)]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name", "T_MenuBar.resx", "Name"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("208381", "Basic Information", "Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Roles.</summary>
    ///
    /// <value>The T_Roles.</value>
    [CustomDisplayName("T_Roles", "T_MenuBar.resx", "Roles"), Column("Roles")]
    [PropertyInfoForEntity("208381", "Basic Information", "Group")]
    [Required]
    public string T_Roles
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Disabled?.</summary>
    ///
    /// <value>The T_Disabled.</value>
    [CustomDisplayName("T_Disabled", "T_MenuBar.resx", "Disabled?"), Column("Disabled")]
    [PropertyInfoForEntity("208383", "Menu Bar Display Information", "Group")]
    public Boolean? T_Disabled
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Horizontal?.</summary>
    ///
    /// <value>The T_Horizontal.</value>
    [CustomDisplayName("T_Horizontal", "T_MenuBar.resx", "Horizontal?"), Column("Horizontal")]
    [PropertyInfoForEntity("208383", "Menu Bar Display Information", "Group")]
    public Boolean? T_Horizontal
    {
        get;
        set;
    }
    /// <summary>Gets or sets the T_MenuBarMenuItemAssociation_t_menubar.</summary>
    ///
    /// <value>The ICollection<T_MenuBarMenuItemAssociation>.</value>
    
    
    
    [DisplayName("Menu Item")]
    public virtual ICollection<T_MenuBarMenuItemAssociation> T_MenuBarMenuItemAssociation_t_menubar
    {
        get;
        set;
    }
    [NotMapped]
    [JsonIgnore]
    //[DisplayName("Menu Item")]
    public virtual ICollection<T_MenuItem> T_MenuItem_T_MenuBarMenuItemAssociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the SelectedT_MenuItem_T_MenuBarMenuItemAssociation many to many association.</summary>
    ///
    /// <value>The ICollection<long?>.</value>
    [NotMapped]
    public virtual ICollection<long?> SelectedT_MenuItem_T_MenuBarMenuItemAssociation
    {
        get;
        set;
    }
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public string getDisplayValue()
    {
        try
        {
            var dispValue = "";
            dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Gets display value model (Actual value to render on UI).</summary>
    ///
    /// <returns>The display value model.</returns>
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = "";
            dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name));
            return dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
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
}

