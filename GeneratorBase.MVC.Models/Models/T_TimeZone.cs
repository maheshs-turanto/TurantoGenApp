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
/// <summary>A time zone.</summary>
[Table("tbl_TimeZone"), CustomDisplayName("T_TimeZone", "T_TimeZone.resx", "Time Zone")]
public partial class T_TimeZone : Entity
{
    /// <summary>Default constructor.</summary>
    public T_TimeZone()
    {
        //this.t_casetimezoneassociation = new HashSet<T_Case>();
    }
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    
    
    
    
    
    [CustomDisplayName("T_Name", "T_TimeZone.resx", "Name"), Column("Name")]
    [Required]
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The t name.</value>
    
    public string T_Name
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Offset cannot be longer than 255 characters.")]
    
    
    
    
    
    [CustomDisplayName("T_Offset", "T_TimeZone.resx", "Offset"), Column("Offset")]
    
    /// <summary>Gets or sets the offset.</summary>
    ///
    /// <value>The t offset.</value>
    
    public string T_Offset
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Time Zone Full Name cannot be longer than 255 characters.")]
    
    
    
    
    
    [CustomDisplayName("T_TimeZoneFullName", "T_TimeZone.resx", "Time Zone Full Name"), Column("TimeZoneFullName")]
    
    /// <summary>Gets or sets the name of the full.</summary>
    ///
    /// <value>The name of the full.</value>
    
    public string T_FullName
    {
        get;
        set;
    }
    
    //[JsonIgnore]
    //public virtual ICollection<T_Case> t_casetimezoneassociation { get; set; }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        try
        {
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name)) + (this.T_Offset != null ? Convert.ToString(this.T_Offset) + "-" : Convert.ToString(this.T_Offset));
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
            var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + " " : Convert.ToString(this.T_Name)) + (this.T_Offset != null ? Convert.ToString(this.T_Offset) + "-" : Convert.ToString(this.T_Offset));
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

