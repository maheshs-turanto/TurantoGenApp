using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace GeneratorBase.MVC.Models
{
/// <summary>Verb Group model class: Verb Group.</summary>
[Table("tbl_VerbGroup"), CustomDisplayName("VerbGroup", "VerbGroup.resx", "Verb Group")]
[Description("Normal")]
public partial class VerbGroup : Entity
{
    /// <summary>Default constructor for Verb Group.</summary>
    public VerbGroup()
    {
        if(!this.Flag1.HasValue)
            this.Flag1 = false;
        this.verbnameselect = new HashSet<VerbsName>();
    }
    [PropertyTypeForEntity(null, true)]
    [StringLength(255, ErrorMessage = "Group Name cannot be longer than 255 characters.")]
    
    [Unique(typeof(ApplicationContext), ErrorMessage = "Group Name is Unique.")]
    //[CustomRemoteValidation("NameIsUnique", "VerbGroup", AdditionalFields = "Id", ErrorMessage = "Group Name is Unique.")]
    /// <summary>Gets or sets the Group Name.</summary>
    ///
    /// <value>The Name.</value>
    [CustomDisplayName("Name", "VerbGroup.resx", "Group Name"), Column("Name")]
    [Required]
    [PropertyInfoForEntity("324061", "Basic Information", "Group")]
    public string Name
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Display Order.</summary>
    ///
    /// <value>The DisplayOrder.</value>
    [CustomDisplayName("DisplayOrder", "VerbGroup.resx", "Display Order"), Column("DisplayOrder")]
    [PropertyInfoForEntity("324061", "Basic Information", "Group")]
    public Nullable<Int32> DisplayOrder
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Disable .</summary>
    ///
    /// <value>The Flag1.</value>
    [CustomDisplayName("Flag1", "VerbGroup.resx", "Disable "), Column("Flag1")]
    [PropertyInfoForEntity("324061", "Basic Information", "Group")]
    public Boolean? Flag1
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Icon cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Icon.</summary>
    ///
    /// <value>The Icon.</value>
    [CustomDisplayName("Icon", "VerbGroup.resx", "Icon"), Column("Icon")]
    [PropertyInfoForEntity("324061", "Basic Information", "Group")]
    [AllowHtml]
    public string Icon
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The Description.</value>
    [CustomDisplayName("Description", "VerbGroup.resx", "Description"), Column("Description")]
    [PropertyInfoForEntity("324061", "Basic Information", "Group")]
    public string Description
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "InternalName cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the InternalName.</summary>
    ///
    /// <value>The InternalName.</value>
    [CustomDisplayName("InternalName", "VerbGroup.resx", "InternalName"), Column("InternalName")]
    [PropertyInfoForEntity("324063", "Internal Use", "Group")]
    public string InternalName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "EntityInternalName cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the EntityInternalName.</summary>
    ///
    /// <value>The EntityInternalName.</value>
    [CustomDisplayName("EntityInternalName", "VerbGroup.resx", "Entity Name"), Column("EntityInternalName")]
    [PropertyInfoForEntity("324063", "Internal Use", "Group")]
    [Required]
    public string EntityInternalName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "UIGroupInternalName cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the UIGroupInternalName.</summary>
    ///
    /// <value>The UIGroupInternalName.</value>
    [CustomDisplayName("UIGroupInternalName", "VerbGroup.resx", "UI Group"), Column("UIGroupInternalName")]
    [PropertyInfoForEntity("324063", "Internal Use", "Group")]
    public string UIGroupInternalName
    {
        get;
        set;
    }
    /// <summary>Gets or sets the GroupId.</summary>
    ///
    /// <value>The GroupId.</value>
    [CustomDisplayName("GroupId", "VerbGroup.resx", "Verb Type"), Column("GroupId")]
    [PropertyInfoForEntity("324063", "Internal Use", "Group")]
    [Required]
    public Nullable<Int32> GroupId
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Background Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Background Color.</summary>
    ///
    /// <value>The BackGroundColor.</value>
    [CustomDisplayName("BackGroundColor", "VerbGroup.resx", "Background Color"), Column("BackGroundColor")]
    [PropertyInfoForEntity("324062", "UI Information", "Group")]
    [PropertyUIInfoType("bgcolor")]
    public string BackGroundColor
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Font Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Font Color.</summary>
    ///
    /// <value>The FontColor.</value>
    [CustomDisplayName("FontColor", "VerbGroup.resx", "Font Color"), Column("FontColor")]
    [PropertyInfoForEntity("324062", "UI Information", "Group")]
    [PropertyUIInfoType("fgcolor")]
    public string FontColor
    {
        get;
        set;
    }
    /// <summary>Gets or sets the verbnameselect.</summary>
    ///
    /// <value>The ICollection<VerbsName>.</value>
    
    [JsonIgnore]
    
    
    
    [DisplayName("Verbs Name")]
    public virtual ICollection<VerbsName> verbnameselect
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
            dispValue = (this.Name != null ? Convert.ToString(this.Name) + " " : Convert.ToString(this.Name));
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
            dispValue = (this.Name != null ? Convert.ToString(this.Name) + " " : Convert.ToString(this.Name));
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

