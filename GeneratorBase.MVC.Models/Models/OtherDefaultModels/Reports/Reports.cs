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
/// <summary>A reports.</summary>
[Table("tbl_Reports")]
public class Reports
{
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
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
    
    /// <summary>Gets or sets the Date/Time of the date created.</summary>
    ///
    /// <value>The date created.</value>
    
    [DisplayName("Date Created")]
    public DateTime? DateCreated
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the created by.</summary>
    ///
    /// <value>The identifier of the created by.</value>
    
    [DisplayName("CreatedBy")]
    public int CreatedById
    {
        get;
        set;
    }
    
    /// <summary>Gets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [NotMapped]
    public string DisplayValue
    {
        get
        {
            return  Convert.ToString(Name);
        }
    }
}
}

