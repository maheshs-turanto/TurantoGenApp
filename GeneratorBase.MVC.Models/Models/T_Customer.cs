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
/// <summary>Customer model class: Customer.</summary>
[Table("tbl_Customer"),CustomDisplayName("T_Customer", "T_Customer.resx", "Customer")]
[Description("Normal")]
public partial class T_Customer : Entity
{
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo","T_Customer.resx","Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("276327","Basic Information","Group")]
    public Nullable<long> T_AutoNo
    {
        get;
        set;
    }
    [PropertyTypeForEntity(null,true)]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name","T_Customer.resx","Name"), Column("Name")] [Required]
    [PropertyInfoForEntity("276327","Basic Information","Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The T_Description.</value>
    [CustomDisplayName("T_Description","T_Customer.resx","Description"), Column("Description")]
    [PropertyInfoForEntity("276328","More Information","Group")]
    public string T_Description
    {
        get;
        set;
    }
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public  string getDisplayValue()
    {
        try
        {
            var dispValue="";
            dispValue =(this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
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
            var dispValue="";
            dispValue =(this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name));
            return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
        try {  }
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

