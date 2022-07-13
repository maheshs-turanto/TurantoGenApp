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
/// <summary>Menu Item model class: Menu Item.</summary>
[Table("tbl_MenuItem"), CustomDisplayName("T_MenuItem", "T_MenuItem.resx", "Menu Item")]
[Description("Menu Item")]
public partial class T_MenuItem : Entity
{
    /// <summary>Default constructor for Menu Item.</summary>
    public T_MenuItem()
    {
        this.Self_t_menuitemmenuitemassociation = new HashSet<T_MenuItem>();
        this.T_MenuBarMenuItemAssociation_t_menuitem = new HashSet<T_MenuBarMenuItemAssociation>();
    }
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo", "T_MenuItem.resx", "Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("208384", "Basic Information", "Group")]
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
    [CustomDisplayName("T_Name", "T_MenuItem.resx", "Menu Label"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("208384", "Basic Information", "Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the ToolTip.</summary>
    ///
    /// <value>The T_ToolTip.</value>
    [CustomDisplayName("T_ToolTip", "T_MenuItem.resx", "ToolTip"), Column("ToolTip")]
    [PropertyInfoForEntity("208384", "Basic Information", "Group")]
    public string T_ToolTip
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Entity cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Entity.</summary>
    ///
    /// <value>The T_Entity.</value>
    [CustomDisplayName("T_Entity", "T_MenuItem.resx", "Entity"), Column("Entity")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    [Required]
    public string T_Entity
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Action cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Action.</summary>
    ///
    /// <value>The T_Action.</value>
    [CustomDisplayName("T_Action", "T_MenuItem.resx", "Action"), Column("Action")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    public string T_Action
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "LinkAddress cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the LinkAddress.</summary>
    ///
    /// <value>The T_LinkAddress.</value>
    [CustomDisplayName("T_LinkAddress", "T_MenuItem.resx", "LinkAddress"), Column("LinkAddress")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    public string T_LinkAddress
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "EntityValue cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the DisplayValue.</summary>
    ///
    /// <value>The T_EntityValue.</value>
    [CustomDisplayName("T_EntityValue", "T_MenuItem.resx", "EntityValue"), Column("EntityValue")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    public string T_EntityValue
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "SavedSearch cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the SavedSearch.</summary>
    ///
    /// <value>The T_SavedSearch.</value>
    [CustomDisplayName("T_SavedSearch", "T_MenuItem.resx", "SavedSearch"), Column("SavedSearch")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    public string T_SavedSearch
    {
        get;
        set;
    }
    
    [StringLength(255, ErrorMessage = "Display Order cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Display Order.</summary>
    ///
    /// <value>The T_DisplayOrder.</value>
    [CustomDisplayName("T_DisplayOrder", "T_MenuItem.resx", "Display Order"), Column("DisplayOrder")]
    [PropertyInfoForEntity("208571", "UI Information", "Group")]
    public string T_DisplayOrder
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Class Icon cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Class Icon.</summary>
    ///
    /// <value>The T_ClassIcon.</value>
    [CustomDisplayName("T_ClassIcon", "T_MenuItem.resx", "Class Icon"), Column("ClassIcon")]
    [PropertyInfoForEntity("208571", "UI Information", "Group")]
    public string T_ClassIcon
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Parent.</summary>
    ///
    /// <value>The T_MenuItemMenuItemAssociation.</value>
    
    
    
    [CustomDisplayName("T_MenuItemMenuItemAssociationID", "T_MenuItem.resx", "Parent Menu"), Column("MenuItemMenuItemAssociationID")]
    [PropertyInfoForEntity("208385", "Menu Item Display Information", "Group")]
    public Nullable<long> T_MenuItemMenuItemAssociationID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Parent navigation property.</summary>
    ///
    /// <value>The t_menuitemmenuitemassociation object.</value>
    
    public virtual T_MenuItem t_menuitemmenuitemassociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Self_t_menuitemmenuitemassociation.</summary>
    ///
    /// <value>The ICollection<T_MenuItem>.</value>
    
    [JsonIgnore]
    
    
    
    [DisplayName("Child Items")]
    public virtual ICollection<T_MenuItem> Self_t_menuitemmenuitemassociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the T_MenuBarMenuItemAssociation_t_menuitem.</summary>
    ///
    /// <value>The ICollection<T_MenuBarMenuItemAssociation>.</value>
    
    
    
    [DisplayName("Menu Bar")]
    public virtual ICollection<T_MenuBarMenuItemAssociation> T_MenuBarMenuItemAssociation_t_menuitem
    {
        get;
        set;
    }
    [NotMapped]
    [JsonIgnore]
    //[DisplayName("Menu Bar")]
    public virtual ICollection<T_MenuBar> T_MenuBar_T_MenuBarMenuItemAssociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the SelectedT_MenuBar_T_MenuBarMenuItemAssociation many to many association.</summary>
    ///
    /// <value>The ICollection<long?>.</value>
    [NotMapped]
    public virtual ICollection<long?> SelectedT_MenuBar_T_MenuBarMenuItemAssociation
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

