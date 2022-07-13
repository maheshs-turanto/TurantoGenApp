using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using RecurrenceGenerator;
namespace GeneratorBase.MVC.Models
{
//[Table("tbl_ScheduleEvents"), DisplayName("Schedule Events")]
/// <summary>A template events.</summary>
public class TemplateEvents : EntityDefault//mahesh
{
    /// <summary>Gets or sets the title.</summary>
    ///
    /// <value>The title.</value>
    
    public string Title
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the start time.</summary>
    ///
    /// <value>The start time.</value>
    
    public Nullable<DateTime> StartTime
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the end time.</summary>
    ///
    /// <value>The end time.</value>
    
    public Nullable<DateTime> EndTime
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is cancelled.</summary>
    ///
    /// <value>True if this object is cancelled, false if not.</value>
    
    public bool IsCancelled
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is notify.</summary>
    ///
    /// <value>True if this object is notify, false if not.</value>
    
    public bool IsNotify
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email to.</summary>
    ///
    /// <value>The email to.</value>
    
    public string EmailTo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the notes.</summary>
    ///
    /// <value>The notes.</value>
    
    public string Notes
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the main meeting.</summary>
    ///
    /// <value>The identifier of the main meeting.</value>
    
    public Nullable<long> MainMeetingID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the event date.</summary>
    ///
    /// <value>The event date.</value>
    
    public Nullable<DateTime> EventDate
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the schedule.</summary>
    ///
    /// <value>The identifier of the schedule.</value>
    
    [DisplayName("Schedule"), Column("ScheduleID")]
    public Nullable<long> ScheduleID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the schedule.</summary>
    ///
    /// <value>The schedule.</value>
    
    public virtual T_Schedule schedule
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
            var dispValue = this.Title + " : " + this.StartTime + "-" + this.EndTime;
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
            var dispValue = this.Title + " : " + this.StartTime + "-" + this.EndTime;
            this.m_DisplayValue = dispValue;
            return dispValue != null ? dispValue : "";
        }
        catch
        {
            return "";
        }
    }
    
}

}

