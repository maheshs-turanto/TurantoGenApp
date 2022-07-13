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
/// <summary>An application feedback.</summary>
[Table("tbl_ApplicationFeedback")]
public class ApplicationFeedback : EntityDefault
{
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Entity Name")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    
    [DisplayName("Property Name")]
    public string PropertyName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the page.</summary>
    ///
    /// <value>The name of the page.</value>
    
    [DisplayName("Page Name")]
    public string PageName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page URL title.</summary>
    ///
    /// <value>The page URL title.</value>
    
    [DisplayName("Page Url Title")]
    public string PageUrlTitle
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the control.</summary>
    ///
    /// <value>The name of the control.</value>
    
    [DisplayName("UI Control Name")]
    public string UIControlName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets URL of the page.</summary>
    ///
    /// <value>The page URL.</value>
    
    [DataType(DataType.Url)]
    
    [DisplayName("Page Url")]
    public string PageUrl
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the comment.</summary>
    ///
    /// <value>The identifier of the comment.</value>
    
    [DisplayName("Comment Id")]
    public Nullable<long> CommentId
    {
        get;
        set;
    }
    /// <summary>Amount to reported by.</summary>
    DateTime? m_ReportedBy = DateTime.UtcNow;
    
    /// <summary>Gets or sets the amount to reported by.</summary>
    ///
    /// <value>Amount to reported by.</value>
    
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public Nullable<DateTime> ReportedBy
    {
        get
        {
            return m_ReportedBy;
        }
        set
        {
            m_ReportedBy = value;
        }
    }
    /// <summary>The reported by user.</summary>
    string m_ReportedByUser = HttpContext.Current.User.Identity.Name;
    
    /// <summary>Gets or sets the reported by user.</summary>
    ///
    /// <value>The reported by user.</value>
    
    [DisplayName("ReportedByUser")]
    public string ReportedByUser
    {
        get
        {
            return m_ReportedByUser;
        }
        set
        {
            m_ReportedByUser = value;
        }
    }
    
    /// <summary>Gets or sets the summary.</summary>
    ///
    /// <value>The summary.</value>
    
    [DisplayName("Summary")]
    public string Summary
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [DisplayName("Description")]
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the attach image.</summary>
    ///
    /// <value>The attach image.</value>
    
    [DisplayName("Attach Image")]
    public string AttachImage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the attach document.</summary>
    ///
    /// <value>The attach document.</value>
    
    [DisplayName("Attach Document")]
    public string AttachDocument
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the associated application feedback type.</summary>
    ///
    /// <value>The identifier of the associated application feedback type.</value>
    
    [DisplayName("Type")]
    public Nullable<long> AssociatedApplicationFeedbackTypeID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedapplicationfeedbacktype.</summary>
    ///
    /// <value>The associatedapplicationfeedbacktype.</value>
    
    public virtual ApplicationFeedbackType associatedapplicationfeedbacktype
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the associated application feedback status.</summary>
    ///
    /// <value>The identifier of the associated application feedback status.</value>
    
    [DisplayName("Status")]
    public Nullable<long> AssociatedApplicationFeedbackStatusID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedapplicationfeedbackstatus.</summary>
    ///
    /// <value>The associatedapplicationfeedbackstatus.</value>
    
    public virtual ApplicationFeedbackStatus associatedapplicationfeedbackstatus
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the application feedback priority.</summary>
    ///
    /// <value>The identifier of the application feedback priority.</value>
    
    [DisplayName("Priority")]
    public Nullable<long> ApplicationFeedbackPriorityID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the applicationfeedbackpriority.</summary>
    ///
    /// <value>The applicationfeedbackpriority.</value>
    
    public virtual FeedbackPriority applicationfeedbackpriority
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the application feedback severity.</summary>
    ///
    /// <value>The identifier of the application feedback severity.</value>
    
    [DisplayName("Severity")]
    public Nullable<long> ApplicationFeedbackSeverityID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the applicationfeedbackseverity.</summary>
    ///
    /// <value>The applicationfeedbackseverity.</value>
    
    public virtual FeedbackSeverity applicationfeedbackseverity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the application feedback resource.</summary>
    ///
    /// <value>The identifier of the application feedback resource.</value>
    
    [DisplayName("Assigned To")]
    public Nullable<long> ApplicationFeedbackResourceID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the applicationfeedbackresource.</summary>
    ///
    /// <value>The applicationfeedbackresource.</value>
    
    public virtual FeedbackResource applicationfeedbackresource
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.CommentId) + " - " + Convert.ToString(this.EntityName) + " - " + Convert.ToString(this.PropertyName) + " - " + Convert.ToString(this.Summary);
        dispValue = dispValue.TrimEnd(" - ".ToCharArray());
        this.m_DisplayValue = dispValue;
        return dispValue;
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        if(!string.IsNullOrEmpty(m_DisplayValue))
            return m_DisplayValue;
        var dispValue = Convert.ToString(this.CommentId) + " - " + Convert.ToString(this.EntityName) + " - " + Convert.ToString(this.PropertyName) + " - " + Convert.ToString(this.Summary);
        return dispValue.TrimEnd(" - ".ToCharArray());
    }
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

