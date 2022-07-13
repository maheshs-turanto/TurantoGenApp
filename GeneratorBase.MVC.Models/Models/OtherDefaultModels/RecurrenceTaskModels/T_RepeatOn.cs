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
/// <summary>A repeat on.</summary>
[Table("tbl_RepeatOn"),DisplayName("Repeat On")]
public class T_RepeatOn : EntityDefault
{
    /// <summary>Gets or sets the identifier of the schedule.</summary>
    ///
    /// <value>The identifier of the schedule.</value>
    
    [DisplayName("Schedule"), Column("ScheduleID")]
    public Nullable<long> T_ScheduleID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the schedule.</summary>
    ///
    /// <value>The t schedule.</value>
    
    public virtual T_Schedule t_schedule
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the recurrence days.</summary>
    ///
    /// <value>The identifier of the recurrence days.</value>
    
    [DisplayName("Recurrence Days"), Column("RecurrenceDaysID")]
    public Nullable<long> T_RecurrenceDaysID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the recurrencedays.</summary>
    ///
    /// <value>The t recurrencedays.</value>
    [Newtonsoft.Json.JsonIgnore]
    public virtual T_RecurrenceDays t_recurrencedays
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
            var dispValue = "";
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
            var dispValue = "";
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

