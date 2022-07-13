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
/// <summary>An admin settings.</summary>
public class AdminSettings
{
    /// <summary>Default constructor.</summary>
    public AdminSettings()
    {
        this.DefaultRoleForNewUser = "None";
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="defaultrolefornewuser">The default role for new user.</param>
    
    public AdminSettings(string defaultrolefornewuser)
    {
        this.DefaultRoleForNewUser = defaultrolefornewuser;
    }
    
    /// <summary>Gets or sets the default role for new user.</summary>
    ///
    /// <value>The default role for new user.</value>
    
    [DisplayName("Set Default Role For New User")]
    public string DefaultRoleForNewUser
    {
        get;
        set;
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
/// <summary>Interface for admin settings repository.</summary>
public interface IAdminSettingsRepository
{
    /// <summary>Edit admin settings.</summary>
    ///
    /// <param name="adminSettings">The admin settings.</param>
    
    void EditAdminSettings(AdminSettings adminSettings);
    
    /// <summary>Gets admin settings.</summary>
    ///
    /// <returns>The admin settings.</returns>
    
    AdminSettings GetAdminSettings();
}
}