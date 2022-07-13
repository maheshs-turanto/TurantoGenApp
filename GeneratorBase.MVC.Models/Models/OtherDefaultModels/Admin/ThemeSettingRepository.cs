using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
using System.IO;
using System.Configuration;
namespace GeneratorBase.MVC.Models
{
/// <summary>A theme setting repository.</summary>
public class ThemeSettingRepository
{
    /// <summary>all themes.</summary>
    private List<ThemeSettings> allThemes;
    /// <summary>Information describing the theme settings.</summary>
    private XDocument themeSettingsData;
    /// <summary>The user.</summary>
    private IUser  User;
    /// <summary>Identifier for the tenant.</summary>
    public string tenantId = "";
    /// <summary>The application theme.</summary>
    private string AppTheme = GeneratorBase.MVC.Models.CommonFunction.Instance.getAppThemeNameAdmin();
    
    /// <summary>constructor.</summary>
    ///
    /// <param name="User">    The user.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    
    public ThemeSettingRepository(IUser User, string tenantId)
    {
        this.User = User;
        this.tenantId = string.IsNullOrEmpty(tenantId)?"":tenantId;
        try
        {
            if(!File.Exists(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml")))
            {
                File.Copy(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"), HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
                File.Copy(HttpContext.Current.Server.MapPath("~/Content/Site.css"), HttpContext.Current.Server.MapPath("~/Content/Site" + this.tenantId + ".css"));
            }
            themeSettingsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
            allThemes = new List<ThemeSettings>();
            var Themes = from c in themeSettingsData.Descendants("Settings")
                         select new ThemeSettings(
                             (long)c.Element("Id"),
                             c.Element("Name").Value,
                             c.Element("CssEditor").Value,
                             (bool)c.Element("IsActive"),
                             (bool)c.Element("IsDefault"),
                             c.Element("DisplayValue").Value
                         );
            allThemes.AddRange(Themes.ToList());
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    //public ThemeSettingRepository(IUser User)
    //{
    //    this.User = User;
    //    var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
    //    var TenantId = commonObj.getTenantId(User);
    //    this.tenantId = TenantId;
    //    try
    //    {
    //        if (!File.Exists(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + TenantId + ".xml")))
    //        {
    //            File.Copy(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"), HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + TenantId + ".xml"));
    //            File.Copy(HttpContext.Current.Server.MapPath("~/Content/Site.css"), HttpContext.Current.Server.MapPath("~/Content/Site" + TenantId + ".css"));
    //        }
    
    //        themeSettingsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + TenantId + ".xml"));
    //        allThemes = new List<ThemeSettings>();
    //        var Themes = from c in themeSettingsData.Descendants("Settings")
    //                     select new ThemeSettings(
    //                          (long)c.Element("Id"),
    //                          c.Element("Name").Value,
    //                          c.Element("CssEditor").Value,
    //                          (bool)c.Element("IsActive"),
    //                          (bool)c.Element("IsDefault"),
    //                          c.Element("DisplayValue").Value
    //                     );
    //        allThemes.AddRange(Themes.ToList());
    
    
    
    //    }
    //    catch
    //    {
    //    }
    //}
    
    /// <summary>Theme count.</summary>
    ///
    /// <returns>An int.</returns>
    
    public int ThemeCount()
    {
        return allThemes.Count();
    }
    
    /// <summary>Gets theme settings.</summary>
    ///
    /// <returns>The theme settings.</returns>
    
    public List<ThemeSettings> GetThemeSettings()
    {
        return allThemes;
    }
    
    /// <summary>Gets theme settings by identifier.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The theme settings by identifier.</returns>
    
    public ThemeSettings GetThemeSettingsById(long id)
    {
        return allThemes.Find(t => t.Id == id);
    }
    
    /// <summary>Inserts a theme model described by Themes.</summary>
    ///
    /// <param name="Themes">The themes.</param>
    ///
    /// <returns>A long.</returns>
    
    public long InsertThemeModel(ThemeSettings Themes)
    {
        if(ThemeCount() > 0)
            Themes.Id = (long)(from T in themeSettingsData.Descendants("Settings") orderby(long)T.Element("Id") descending select(long)T.Element("Id")).FirstOrDefault() + 1;
        else
            Themes.Id = 1;
        themeSettingsData.Root.Add(new XElement("Settings", new XElement("Id", Themes.Id),
                                                new XElement("Name", Themes.Name),
                                                new XElement("CssEditor", Themes.CssEditor),
                                                new XElement("IsActive", Themes.IsActive),
                                                new XElement("IsDefault", Themes.IsDefault),
                                                new XElement("DisplayValue", Themes.DisplayValue)));
        themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme+this.tenantId+".xml"));
        return Themes.Id;
    }
    
    /// <summary>Edit themes model.</summary>
    ///
    /// <param name="Themes">The themes.</param>
    /// <param name="strCss">The CSS.</param>
    
    public void EditThemesModel(Theme Themes, string strCss)
    {
        try
        {
            XElement node = themeSettingsData.Root.Elements("Settings").Where(i => (long)i.Element("Id") == Themes.Id).FirstOrDefault();
            node.SetElementValue("Name", Themes.Name);
            node.SetElementValue("CssEditor", strCss);
            node.SetElementValue("IsActive", Themes.IsActive);
            node.SetElementValue("IsDefault", Themes.IsDefault);
            node.SetElementValue("DisplayValue", Themes.Name);
            themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Updates all.</summary>
    public void UpdateAll()
    {
        try
        {
            foreach(var element in themeSettingsData.Descendants("Settings"))
            {
                element.SetElementValue("IsActive", false);
            }
            themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSetting.xml"));
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
    /// <summary>Updates the single described by ID.</summary>
    ///
    /// <param name="Id">The identifier.</param>
    
    public void UpdateSingle(long Id)
    {
        try
        {
            //themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true)
            if(Id != 0)
            {
                var ActiveElement = themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true).FirstOrDefault();
                if(ActiveElement != null)
                {
                    ActiveElement.SetElementValue("IsActive", false);
                    themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
                }
                var CurrentElement = themeSettingsData.Descendants("Settings").Where(t => (long)t.Element("Id") == Id).FirstOrDefault();
                CurrentElement.SetElementValue("IsActive", true);
                themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
            }
            else
            {
                if(ThemeCount() > 0)
                {
                    var _defalutElement = themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true).FirstOrDefault();
                    _defalutElement.SetElementValue("IsActive", false);
                    themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
                }
            }
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
    /// <summary>Deletes the theme model described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    
    public void DeleteThemeModel(long id)
    {
        try
        {
            themeSettingsData.Root.Elements("Settings").Where(i => (long)i.Element("Id") == id).Remove();
            themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings"+AppTheme + this.tenantId + ".xml"));
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
    /// <summary>Mobile Theme.</summary>
    ///
    /// <returns>The theme mobile settings.</returns>
    
    public List<ThemeSetingMobile> GetThemeMobileSettings()
    {
        //mobile
        List<ThemeSetingMobile> allThemesMobile;
        XDocument themeSettingsMobileData;
        themeSettingsMobileData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
        allThemesMobile = new List<ThemeSetingMobile>();
        var ThemesMobile = from c in themeSettingsMobileData.Descendants("Settings")
                           select new ThemeSetingMobile(
                               (long)c.Element("Id"),
                               c.Element("Name").Value,
                               c.Element("CssName").Value,
                               (bool)c.Element("IsActive")
                           );
        allThemesMobile.AddRange(ThemesMobile.ToList());
        return allThemesMobile;
    }
    
    /// <summary>Updates all mobile described by ID.</summary>
    ///
    /// <param name="Id">The identifier.</param>
    
    public void UpdateAllMobile(long Id)
    {
        try
        {
            XDocument MobileDataTheme = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
            XElement node = MobileDataTheme.Root.Elements("Settings").Where(i => (long)i.Element("Id") == Id).FirstOrDefault();
            foreach(var element in MobileDataTheme.Descendants("Settings"))
            {
                element.SetElementValue("IsActive", false);
            }
            node.SetElementValue("IsActive", true);
            MobileDataTheme.Save(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    //End Mobile Theme
}
}