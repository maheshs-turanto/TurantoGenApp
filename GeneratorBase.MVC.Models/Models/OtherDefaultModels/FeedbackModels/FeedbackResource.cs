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
/// <summary>A feedback resource.</summary>
[Table("tbl_FeedbackResource")]
public class FeedbackResource : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public FeedbackResource()
    {
        this.applicationfeedbackresource = new HashSet<ApplicationFeedback>();
    }
    
    /// <summary>Gets or sets the identifier of the resource.</summary>
    ///
    /// <value>The identifier of the resource.</value>
    
    [DisplayName("Resource Id")]
    public Nullable<long> ResourceId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's first name.</summary>
    ///
    /// <value>The name of the first.</value>
    
    [DisplayName("First Name")] [Required]
    public string FirstName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's last name.</summary>
    ///
    /// <value>The name of the last.</value>
    
    [DisplayName("Last Name")]
    public string LastName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [RegularExpression(@"^[\w\.-]{1,}\@([\da-zA-Z-]{1,}\.){1,}[\da-zA-Z-]+$", ErrorMessage = "Invalid Email")]
    [DisplayName("Email")]
    public string Email
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the phone no.</summary>
    ///
    /// <value>The phone no.</value>
    
    [RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid Phone No")]
    [DisplayName("Phone No")]
    public string PhoneNo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the applicationfeedbackresource.</summary>
    ///
    /// <value>The applicationfeedbackresource.</value>
    
    public virtual ICollection<ApplicationFeedback> applicationfeedbackresource
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.ResourceId)+" - "+Convert.ToString(this.FirstName);
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
        var dispValue = Convert.ToString(this.ResourceId) + " - " + Convert.ToString(this.FirstName);
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

