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
/// <summary> User login attempts.</summary>
[Table("LoginAttempts")]
public class LoginAttempts
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
    
    /// <summary>Gets or sets the identifier of the user.</summary>
    ///
    /// <value>The identifier of the user.</value>
    
    public string UserId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the date.</summary>
    ///
    /// <value>The date.</value>
    
    public System.DateTime Date
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the IP address.</summary>
    ///
    /// <value>The IP address.</value>
    
    public string IPAddress
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is successfull.</summary>
    ///
    /// <value>True if this object is successfull, false if not.</value>
    
    public bool IsSuccessfull
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
/// <summary>A password history.</summary>
[Table("PasswordHistory")]
public class PasswordHistory
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
    
    /// <summary>Gets or sets the identifier of the user.</summary>
    ///
    /// <value>The identifier of the user.</value>
    
    public string UserId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the date.</summary>
    ///
    /// <value>The date.</value>
    
    public System.DateTime Date
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the hashed password.</summary>
    ///
    /// <value>The hashed password.</value>
    
    public string HashedPassword
    {
        get;
        set;
    }
    
}
}

