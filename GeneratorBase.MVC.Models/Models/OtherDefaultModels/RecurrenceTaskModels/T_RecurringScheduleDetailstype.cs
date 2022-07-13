﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>A recurring schedule detailstype.</summary>
[Table("tbl_RecurringScheduleDetailstype"),DisplayName("RecurringScheduleDetails Type")]
public class T_RecurringScheduleDetailstype : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public T_RecurringScheduleDetailstype()
    {
        this.t_associatedrecurringscheduledetailstype = new HashSet<T_Schedule>();
    }
    
    
    
    [DisplayName("Name"), Column("Name")] [Required]
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The t name.</value>
    
    public string T_Name
    {
        get;
        set;
    }
    
    
    
    [DisplayName("Description"), Column("Description")]
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The t description.</value>
    
    public string T_Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedrecurringscheduledetailstype.</summary>
    ///
    /// <value>The t associatedrecurringscheduledetailstype.</value>
    [Newtonsoft.Json.JsonIgnore]
    public virtual ICollection<T_Schedule> t_associatedrecurringscheduledetailstype
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+"  " : Convert.ToString(this.T_Name));
            dispValue = dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
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
            var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+"  " : Convert.ToString(this.T_Name));
            return dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
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
}
}

