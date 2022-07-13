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
/// <summary>Verbs Name model class: User-Defined Verb.</summary>
[Table("tbl_VerbsName"), CustomDisplayName("VerbsName", "VerbsName.resx", "Verbs Name")]
[Description("Normal")]
public partial class VerbsName : Entity
{
    /// <summary>Gets or sets the Display Order.</summary>
    ///
    /// <value>The DisplayOrder.</value>
    [CustomDisplayName("DisplayOrder", "VerbsName.resx", "Display Order"), Column("DisplayOrder")]
    [PropertyInfoForEntity("324066", "Basic Information", "Group")]
    public Nullable<Int32> DisplayOrder
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Verb Icon cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Verb Icon.</summary>
    ///
    /// <value>The VerbIcon.</value>
    [CustomDisplayName("VerbIcon", "VerbsName.resx", "Verb Icon"), Column("VerbIcon")]
    [PropertyInfoForEntity("324066", "Basic Information", "Group")]
    [AllowHtml]
    public string VerbIcon
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Background Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Background Color.</summary>
    ///
    /// <value>The BackGroundColor.</value>
    [CustomDisplayName("BackGroundColor", "VerbsName.resx", "Background Color"), Column("BackGroundColor")]
    [PropertyInfoForEntity("324067", "UI Information", "Group")]
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
    [CustomDisplayName("FontColor", "VerbsName.resx", "Font Color"), Column("FontColor")]
    [PropertyInfoForEntity("324067", "UI Information", "Group")]
    [PropertyUIInfoType("fgcolor")]
    public string FontColor
    {
        get;
        set;
    }
    /// <summary>Gets or sets the VerbId.</summary>
    ///
    /// <value>The VerbId.</value>
    [CustomDisplayName("VerbId", "VerbsName.resx", "Verbs Name"), Column("VerbId")]
    [PropertyInfoForEntity("324068", "Internal Use Only", "Group")]
    public string VerbId
    {
        get;
        set;
    }
    /// <summary>Gets or sets the VerbTypeID.</summary>
    ///
    /// <value>The VerbTypeID.</value>
    [CustomDisplayName("VerbTypeID", "VerbsName.resx", "Verb Type"), Column("VerbTypeID")]
    [PropertyInfoForEntity("324068", "Internal Use Only", "Group")]
    public Nullable<Int64> VerbTypeID
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "VerbName cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the VerbName.</summary>
    ///
    /// <value>The VerbName.</value>
    [CustomDisplayName("VerbName", "VerbsName.resx", "VerbName"), Column("VerbName")]
    [PropertyInfoForEntity("324068", "Internal Use Only", "Group")]
    public string VerbName
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Group Name.</summary>
    ///
    /// <value>The VerbNameSelect.</value>
    
    [CustomValidation(typeof(MandatoryDropdown), "ValidateDropdown")]
    
    [CustomDisplayName("VerbNameSelectID", "VerbsName.resx", "Group Name"), Column("VerbNameSelectID")]
    [PropertyInfoForEntity("324066", "Basic Information", "Group")]
    public Nullable<long> VerbNameSelectID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Group Name navigation property.</summary>
    ///
    /// <value>The verbnameselect object.</value>
    
    public virtual VerbGroup verbnameselect
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Verb Name.</summary>
    ///
    /// <value>The VerbsNameVerbNameAssociation.</value>
    
    
    [CustomDisplayName("VerbIds", "VerbIds.resx", "Verb Name"), Column("VerbIds")]
    [PropertyInfoForEntity("324066", "Basic Information", "Group")]
    public string VerbIds
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
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var dispValue = "";
                dispValue = (this.VerbNameSelectID != null ? context.VerbGroups.FirstOrDefault(p => p.Id == this.VerbNameSelectID).DisplayValue + "-" : "");
                //this.m_DisplayValue = dispValue;
                this.m_DisplayValue = dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
                return dispValue;
            }
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
            dispValue = (this.verbnameselect != null ? verbnameselect.DisplayValue + "-" : "");
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
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

