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
/// <summary>An email template.</summary>
[Table("tbl_EmailTemplate")]
public class EmailTemplate : EntityDefault
{
    /// <summary>Gets or sets the identifier of the associated email template type.</summary>
    ///
    /// <value>The identifier of the associated email template type.</value>
    
    [DisplayName("Email Template Type")]
    public Nullable<long> AssociatedEmailTemplateTypeID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedemailtemplatetype.</summary>
    ///
    /// <value>The associatedemailtemplatetype.</value>
    
    public virtual EmailTemplateType associatedemailtemplatetype
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email content.</summary>
    ///
    /// <value>The email content.</value>
    
    [DisplayName("Email Content")]
    [System.Web.Mvc.AllowHtml]
    public string EmailContent
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email subject.</summary>
    ///
    /// <value>The email subject.</value>
    
    [DisplayName("Email Subject")]
    public string EmailSubject
    {
        get;
        set;
    }
    /// <summary>Amount to last updated by.</summary>
    DateTime? m_LastUpdatedBy = DateTime.UtcNow;
    
    /// <summary>Gets or sets the amount to last updated by.</summary>
    ///
    /// <value>Amount to last updated by.</value>
    
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public Nullable<DateTime> LastUpdatedBy
    {
        get
        {
            return m_LastUpdatedBy;
        }
        set
        {
            m_LastUpdatedBy = value;
        }
    }
    /// <summary>.</summary>
    string m_LastUpdatedByUser = HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : "";
    
    /// <summary>Gets or sets the last updated by user.</summary>
    ///
    /// <value>The last updated by user.</value>
    
    [DisplayName("LastUpdatedByUser")]
    public string LastUpdatedByUser
    {
        get
        {
            return m_LastUpdatedByUser;
        }
        set
        {
            m_LastUpdatedByUser = value;
        }
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        var dispValue = (this.AssociatedEmailTemplateTypeID != null ? (new ApplicationContext(new SystemUser())).EmailTemplateTypes.Find(this.AssociatedEmailTemplateTypeID).DisplayValue + "  " : "");
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
        return (this.AssociatedEmailTemplateTypeID != null ? (new ApplicationContext(new SystemUser())).EmailTemplateTypes.Find(this.AssociatedEmailTemplateTypeID).DisplayValue + "  " : "");
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