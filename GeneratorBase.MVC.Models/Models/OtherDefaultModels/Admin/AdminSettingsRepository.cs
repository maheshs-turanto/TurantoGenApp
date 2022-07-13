using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
namespace GeneratorBase.MVC.Models
{
/// <summary>An admin settings repository.</summary>
public class AdminSettingsRepository : IAdminSettingsRepository
{
    /// <summary>Information describing the admin settings.</summary>
    private XDocument adminSettingsData;
    /// <summary>constructor.</summary>
    public AdminSettingsRepository()
    {
        adminSettingsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
    
    /// <summary>Gets admin settings.</summary>
    ///
    /// <returns>The admin settings.</returns>
    
    public AdminSettings GetAdminSettings()
    {
        IEnumerable<AdminSettings> adminsettings = from admset in adminSettingsData.Descendants("Settings")
                select new AdminSettings((string)admset.Element("DefaultRoleForNewUser"));
        return adminsettings.ToList()[0];
    }
    
    /// <summary>Gets default role for new user.</summary>
    ///
    /// <returns>The default role for new user.</returns>
    
    public string GetDefaultRoleForNewUser()
    {
        IEnumerable<AdminSettings> adminsettings = from admset in adminSettingsData.Descendants("Settings")
                select new AdminSettings((string)admset.Element("DefaultRoleForNewUser"));
        return adminsettings.ToList()[0].DefaultRoleForNewUser;
    }
    
    /// <summary>Edit admin settings.</summary>
    ///
    /// <param name="admSet">The admin settings.</param>
    
    public void EditAdminSettings(AdminSettings admSet)
    {
        XElement node = adminSettingsData.Root.Elements("Settings").FirstOrDefault();
        node.SetElementValue("DefaultRoleForNewUser", admSet.DefaultRoleForNewUser);
        adminSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
}
}