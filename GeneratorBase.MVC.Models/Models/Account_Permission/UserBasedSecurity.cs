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
/// <summary>A user based security.</summary>
[Table("tbl_UserBasedSecurity")]
public class UserBasedSecurity
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
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("EntityName")] [Required]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the target entity.</summary>
    ///
    /// <value>The name of the target entity.</value>
    
    [DisplayName("TargetEntityName")]
    [Required]
    public string TargetEntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the association.</summary>
    ///
    /// <value>The name of the association.</value>
    
    [DisplayName("AssociationName")]
    [Required]
    public string AssociationName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is main entity.</summary>
    ///
    /// <value>True if this object is main entity, false if not.</value>
    
    [DisplayName("IsMainEntity")]
    public bool IsMainEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles to ignore.</summary>
    ///
    /// <value>The roles to ignore.</value>
    
    [DisplayName("RolesToIgnore")]
    public string RolesToIgnore
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the other 1.</summary>
    ///
    /// <value>The other 1.</value>
    
    [DisplayName("Other1")]
    public string Other1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the other 2.</summary>
    ///
    /// <value>The other 2.</value>
    
    [DisplayName("Other2")]
    public string Other2
    {
        get;
        set;
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
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

