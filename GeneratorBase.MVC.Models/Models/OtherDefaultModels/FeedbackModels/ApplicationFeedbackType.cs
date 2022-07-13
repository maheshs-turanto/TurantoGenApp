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
/// <summary>An application feedback type.</summary>
[Table("tbl_ApplicationFeedbackType")]
public class ApplicationFeedbackType : EntityDefault
{
    /// <summary>Default constructor.</summary>
    public ApplicationFeedbackType()
    {
        this.associatedapplicationfeedbacktype = new HashSet<ApplicationFeedback>();
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Name")] [Required]
    public string Name
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
    
    /// <summary>Gets or sets the associatedapplicationfeedbacktype.</summary>
    ///
    /// <value>The associatedapplicationfeedbacktype.</value>
    
    public virtual ICollection<ApplicationFeedback> associatedapplicationfeedbacktype
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        var dispValue = Convert.ToString(this.Name);
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
        var dispValue = Convert.ToString(this.Name);
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

