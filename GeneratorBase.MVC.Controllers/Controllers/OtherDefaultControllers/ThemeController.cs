using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling themes.</summary>
public class ThemeController : IdentityBaseController
{
    /// <summary>The application theme.</summary>
    private string AppTheme = GeneratorBase.MVC.Models.CommonFunction.Instance.getAppThemeNameAdmin();
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(UserManager != null) UserManager.Dispose();
            if(Identitydb != null) Identitydb.Dispose();
            if(RoleManager != null) RoleManager.Dispose();
            AppTheme = null;
        }
        base.Dispose(disposing);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) theme change.</summary>
    ///
    /// <returns>A response stream to send to the ThemeChange View.</returns>
    
    public ActionResult ThemeChange()
    {
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) theme change.</summary>
    ///
    /// <param name="themeName">Name of the theme.</param>
    ///
    /// <returns>A response stream to send to the ThemeChange View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ThemeChange(string themeName)
    {
        var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        var tag  = myConfiguration.AppSettings.Settings["AppTheme"];
        if(tag == null)
            myConfiguration.AppSettings.Settings.Add("AppTheme", themeName);
        else
            myConfiguration.AppSettings.Settings["AppTheme"].Value = themeName;
        myConfiguration.Save();
        return RedirectToAction("Index", "Home");
    }
    
    /// <summary>Indexes.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="sortBy">       Describes who sort this object.</param>
    /// <param name="isAsc">        The is ascending.</param>
    /// <param name="page">         The page.</param>
    /// <param name="itemsPerPage"> The items per page.</param>
    /// <param name="RenderPartial">The render partial.</param>
    /// <param name="BulkOperation">The bulk operation.</param>
    /// <param name="tenantId">     Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? RenderPartial, string BulkOperation,string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,tenantId);
        ViewData["TenantId"] = tenantId;
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        if(searchString != null)
            page = null;
        ViewBag.CurrentFilter = searchString;
        var lstThemeBase = from s in _themeSettingsRepository.GetThemeSettings()
                           select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstThemeBase = searchRecords(lstThemeBase.AsQueryable(), searchString.ToUpper());
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstThemeBase = sortRecords(lstThemeBase.AsQueryable(), sortBy, isAsc);
        }
        else lstThemeBase = lstThemeBase.OrderByDescending(c => c.IsActive);
        int pageSize = 12;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 120 ? 120 : pageSize;
        var _Theme = lstThemeBase;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_Theme.ToPagedList(pageNumber, pageSize));
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                return PartialView("BulkOperation", _Theme.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _Theme.ToPagedList(pageNumber, pageSize));
        }
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstTheme">    The list theme.</param>
    /// <param name="searchString">The search string.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<ThemeSettings> searchRecords(IQueryable<ThemeSettings> lstTheme, string searchString)
    {
        searchString = searchString.Trim();
        lstTheme = lstTheme.Where(s => (!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)));
        return lstTheme;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstTheme">The list theme.</param>
    /// <param name="sortBy">  Describes who sort this object.</param>
    /// <param name="isAsc">   The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<ThemeSettings> sortRecords(IQueryable<ThemeSettings> lstTheme, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        ParameterExpression paramExpression = Expression.Parameter(typeof(ThemeSettings), "themebase");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<ThemeSettings>)lstTheme.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstTheme.ElementType, lambda.Body.Type },
                       lstTheme.Expression,
                       lambda));
    }
    
    /// <summary>Creates a new ActionResult.</summary>
    ///
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string tenantId)
    {
        if(((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
        {
            Theme theme = new Theme();
            var list = CommonFunction.Instance.getTenantList(LogggedInUser);
            if(list != null)
            {
                if(string.IsNullOrEmpty(tenantId))
                    tenantId = list.FirstOrDefault().Key;
                else if(!list.ContainsKey(tenantId))
                {
                    return RedirectToAction("Index", "Error");
                }
                ViewBag.TenantList = new System.Web.Mvc.SelectList(list, "Key", "Value", tenantId);
                ViewData["TenantId"] = tenantId;
            }
            theme = GetCssVariable(theme);
            return View(theme);
        }
        else return RedirectToAction("Index", "Home");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a new ActionResult.</summary>
    ///
    /// <param name="theme">   The theme.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    public ActionResult Create(Theme theme,string tenantId)
    {
        long Id = 0;
        if(((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
        {
            ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser, tenantId);
            ThemeSettings themeSetting = null;
            if(ModelState.IsValid)
            {
                string cssStr = EditCssVariable(theme);
                if(theme.IsActive && _themeSettingsRepository.ThemeCount() > 0)
                {
                    _themeSettingsRepository.UpdateAll();
                }
                themeSetting = new ThemeSettings(0, theme.Name, cssStr, theme.IsActive, theme.IsDefault, theme.Name);
                Id = _themeSettingsRepository.InsertThemeModel(themeSetting);
            }
            if(theme.IsActive)
                return SetCustomThemeCreate(Id, themeSetting,  tenantId);
            else
                return RedirectToAction("Create", "Theme");
        }
        else return RedirectToAction("Index", "Home");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="RenderPartial">The render partial.</param>
    /// <param name="Id">           The identifier.</param>
    /// <param name="tenantId">     Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(bool? RenderPartial, long Id, string tenantId)
    {
        if(((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
        {
            Theme theme = new Theme();
            theme = GetCssVariable(theme, Id, tenantId);
            return View(theme);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="theme">        The theme.</param>
    /// <param name="RenderPartial">The render partial.</param>
    /// <param name="tenantId">     Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    public ActionResult Edit(Theme theme, bool? RenderPartial, string tenantId)
    {
        if(((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
        {
            ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,  tenantId);
            string eidtedTheme = EditCssVariable(theme);
            if(theme.IsActive)
                _themeSettingsRepository.UpdateAll();
            _themeSettingsRepository.EditThemesModel(theme, eidtedTheme);
            //if (theme.IsActive)
            //    SetStringCss(theme.Id);
            //else
            //{
            //    string temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css"));
            //    System.IO.File.WriteAllText(Server.MapPath("~/Content/Site"+AppTheme+".css"), temp);
            //}
            SetCustomThemeEdit(theme, tenantId);
            return RedirectToAction("Create", "Theme");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes this
    /// object.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    [HttpPost, ActionName("Delete")]
    public ActionResult Delete(long Id,string tenantId)
    {
        if(((CustomPrincipal)User).CanDeleteAdminFeature("UserInterfaceSetting"))
        {
            ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,  tenantId);
            _themeSettingsRepository.DeleteThemeModel(Id);
            SetStringCss(0,  tenantId);
            return RedirectToAction("Create", "Theme");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    
    /// <summary>Css get Set methods.</summary>
    ///
    /// <param name="theme">The theme.</param>
    ///
    /// <returns>The CSS variable.</returns>
    
    public Theme GetCssVariable(Theme theme)
    {
        string CssFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css"));
        theme.CssEditor = CssFile;
        Dictionary<string, string> varlist = new Dictionary<string, string>();
        if(System.IO.File.Exists(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css")))
        {
            using(System.IO.StreamReader sr = System.IO.File.OpenText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css")))
            {
                String input;
                while((input = sr.ReadLine()) != null)
                {
                    if(input.Contains("var"))
                        varlist.Add(input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[0], input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[1]);
                }
            }
        }
        theme = setThemeVariables(theme, varlist);
        return theme;
    }
    
    /// <summary>Gets CSS variable.</summary>
    ///
    /// <param name="theme">   The theme.</param>
    /// <param name="Id">      The identifier.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The CSS variable.</returns>
    
    public Theme GetCssVariable(Theme theme, long Id, string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,  tenantId);
        var lsttheme = (from s in _themeSettingsRepository.GetThemeSettings()
                        where(s.Id == Id)
                        select s).ToList();
        theme.Id = Id;
        theme.Name = lsttheme[0].Name;
        string CssFile = lsttheme[0].CssEditor;
        theme.CssEditor = lsttheme[0].CssEditor;
        theme.IsActive = lsttheme[0].IsActive;
        Dictionary<string, string> varlist = new Dictionary<string, string>();
        if(System.IO.File.Exists(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css")))
        {
            using(System.IO.StringReader sr = new StringReader(CssFile))
            {
                String input;
                while((input = sr.ReadLine()) != null)
                {
                    if(input.Contains("var"))
                        varlist.Add(input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[0].TrimStart(), input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[1].TrimStart());
                }
            }
        }
        theme.sidebarLinkColor = varlist.Where(p => p.Key.Trim() == "_sidebarLinkColor")
                                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.backgroundColorbody = varlist.Where(p => p.Key.Trim() == "_backgroundColorbody")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.navlistBackground = varlist.Where(p => p.Key.Trim() == "_navlistBackground")
                                  .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.footerbgcolor = varlist.Where(p => p.Key.Trim() == "_footerbgcolor")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderBackgroundColor = varlist.Where(p => p.Key.Trim() == "HeaderBackgroundColor")
                                      .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderAppLabel = varlist.Where(p => p.Key.Trim() == "HeaderAppLabel")
                               .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkReportBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportBG")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkReportText = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportText")
                                     .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkReportTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportTextHover")
                                          .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkAdminBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminBG")
                                  .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkAdminText = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminText")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkAdminTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminTextHover")
                                         .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkUserBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserBG")
                                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkUserText = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserText")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.HeaderLinkUserTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserTextHover")
                                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardBoxBG = varlist.Where(p => p.Key.Trim() == "DashboardBoxBG")
                               .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardIconBG = varlist.Where(p => p.Key.Trim() == "DashboardIconBG")
                                .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardBoxBorder = varlist.Where(p => p.Key.Trim() == "DashboardBoxBorder")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardBoxHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxHover")
                                  .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardBoxLink = varlist.Where(p => p.Key.Trim() == "DashboardBoxLink")
                                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.DashboardBoxLinkHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxLinkHover")
                                      .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TrackedBoxBorderColor = varlist.Where(p => p.Key.Trim() == "TrackedBoxBorderColor")
                                      .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.sidebarLinkHoverBG = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverBG")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.sidebarLinkHoverColor = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverColor")
                                      .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PageTitleBG = varlist.Where(p => p.Key.Trim() == "PageTitleBG")
                            .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PageTitleBorder1 = varlist.Where(p => p.Key.Trim() == "PageTitleBorder1")
                                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PanelHeadingBG = varlist.Where(p => p.Key.Trim() == "PanelHeadingBG")
                               .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PanelHeadingForeColor1 = varlist.Where(p => p.Key.Trim() == "PanelHeadingForeColor1")
                                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PanelHeadingBorder = varlist.Where(p => p.Key.Trim() == "PanelHeadingBorder")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.PageTitleForeColor = varlist.Where(p => p.Key.Trim() == "PageTitleForeColor")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.BtnDefaultBG1 = varlist.Where(p => p.Key.Trim() == "BtnDefaultBG1")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.BtnDefaultForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultForecolor")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.BtnDefaultHoverBG = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverBG")
                                  .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.BtnDefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverForecolor")
                                         .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Borderbutton = varlist.Where(p => p.Key.Trim() == "Borderbutton")
                             .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Borderbutton1 = varlist.Where(p => p.Key.Trim() == "Borderbutton1")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Btn2DefaultBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultBG")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Btn2DefaultForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultForecolor")
                                     .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Btn2DefaultHoverBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverBG")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Btn2DefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverForecolor")
                                          .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Borderbutton2 = varlist.Where(p => p.Key.Trim() == "Borderbutton2")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.Borderbutton3 = varlist.Where(p => p.Key.Trim() == "Borderbutton3")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TableHeaderBG = varlist.Where(p => p.Key.Trim() == "TableHeaderBG")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TableForecolor = varlist.Where(p => p.Key.Trim() == "TableForecolor")
                               .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TableBorder = varlist.Where(p => p.Key.Trim() == "TableBorder")
                            .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsActiveforecolor = varlist.Where(p => p.Key.Trim() == "TabsActiveforecolor")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsActiveBorder = varlist.Where(p => p.Key.Trim() == "TabsActiveBorder")
                                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsActiveTopborder = varlist.Where(p => p.Key.Trim() == "TabsActiveTopborder")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsActiveBackground = varlist.Where(p => p.Key.Trim() == "TabsActiveBackground")
                                     .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsForecolor = varlist.Where(p => p.Key.Trim() == "TabsForecolor")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsBackground = varlist.Where(p => p.Key.Trim() == "TabsBackground")
                               .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsBorder = varlist.Where(p => p.Key.Trim() == "TabsBorder")
                           .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsHeadingBG = varlist.Where(p => p.Key.Trim() == "TabsHeadingBG")
                              .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsHeadingBorder = varlist.Where(p => p.Key.Trim() == "TabsHeadingBorder")
                                  .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsForecolorHover = varlist.Where(p => p.Key.Trim() == "TabsForecolorHover")
                                   .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsBackgroundHover = varlist.Where(p => p.Key.Trim() == "TabsBackgroundHover")
                                    .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.TabsBorderHover = varlist.Where(p => p.Key.Trim() == "TabsBorderHover")
                                .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.SidebarActiveforcolor = varlist.Where(p => p.Key.Trim() == "SidebarActiveforcolor")
                                      .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.SidebarActiveBG = varlist.Where(p => p.Key.Trim() == "SidebarActiveBG")
                                .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.SidebarHoverBorderLeft = varlist.Where(p => p.Key.Trim() == "SidebarHoverBorderLeft")
                                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.FooterLinkcolor = varlist.Where(p => p.Key.Trim() == "FooterLinkcolor")
                                .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.FooterForecolor = varlist.Where(p => p.Key.Trim() == "FooterForecolor")
                                .First().Value.Replace("\"", "").Replace(";", "").Trim();
        theme.rightwrapper = varlist.Where(p => p.Key.Trim() == "rightwrapper")
                             .First().Value.Replace("\"", "").Replace(";", "").Trim();
        return theme;
    }
    
    /// <summary>Edit CSS variable.</summary>
    ///
    /// <param name="theme">The theme.</param>
    ///
    /// <returns>A string.</returns>
    
    public string EditCssVariable(Theme theme)
    {
        string temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/Sitetemp"+AppTheme+".css")); ;
        //temp = cssstring;
        temp = temp.Replace("_sidebarLinkColor = \"#ffffff\"", "_sidebarLinkColor = \"" + theme.sidebarLinkColor + "\"")
               .Replace("_backgroundColorbody = \"#62a8d1\"", "_backgroundColorbody = \"" + theme.backgroundColorbody + "\"")
               .Replace("_navlistBackground = \"#62a8d1\"", "_navlistBackground = \"" + theme.navlistBackground + "\"")
               .Replace("_footerbgcolor = \"#000000\"", "_footerbgcolor = \"" + theme.footerbgcolor + "\"")
               .Replace("HeaderBackgroundColor = \"#000000\"", "HeaderBackgroundColor = \"" + theme.HeaderBackgroundColor + "\"")
               .Replace("HeaderAppLabel = \"#ffffff\"", "HeaderAppLabel = \"" + theme.HeaderAppLabel + "\"")
               .Replace("HeaderLinkReportBG = \"#2e8965\"", "HeaderLinkReportBG = \"" + theme.HeaderLinkReportBG + "\"")
               .Replace("HeaderLinkReportText = \"#ffffff\"", "HeaderLinkReportText = \"" + theme.HeaderLinkReportText + "\"")
               .Replace("HeaderLinkReportTextHover = \"#2e8965\"", "HeaderLinkReportTextHover = \"" + theme.HeaderLinkReportTextHover + "\"")
               .Replace("HeaderLinkAdminBG = \"#892e65\"", "HeaderLinkAdminBG = \"" + theme.HeaderLinkAdminBG + "\"")
               .Replace("HeaderLinkAdminText = \"#ffffff\"", "HeaderLinkAdminText = \"" + theme.HeaderLinkAdminText + "\"")
               .Replace("HeaderLinkAdminTextHover = \"#892e65\"", "HeaderLinkAdminTextHover = \"" + theme.HeaderLinkAdminTextHover + "\"")
               .Replace("HeaderLinkUserBG = \"#62a8d1\"", "HeaderLinkUserBG = \"" + theme.HeaderLinkUserBG + "\"")
               .Replace("HeaderLinkUserText = \"#ffffff\"", "HeaderLinkUserText = \"" + theme.HeaderLinkUserText + "\"")
               .Replace("HeaderLinkUserTextHover = \"#62a8d1\"", "HeaderLinkUserTextHover = \"" + theme.HeaderLinkUserTextHover + "\"")
               .Replace("DashboardBoxBG = \"#eeeeee\"", "DashboardBoxBG = \"" + theme.DashboardBoxBG + "\"")
               .Replace("DashboardIconBG = \"#62a8d1\"", "DashboardIconBG = \"" + theme.DashboardIconBG + "\"")
               .Replace("DashboardBoxBorder = \"#d8d8d8\"", "DashboardBoxBorder = \"" + theme.DashboardBoxBorder + "\"")
               .Replace("DashboardBoxHover = \"#eeeeee\"", "DashboardBoxHover = \"" + theme.DashboardBoxHover + "\"")
               .Replace("DashboardBoxLink = \"#428bca\"", "DashboardBoxLink = \"" + theme.DashboardBoxLink + "\"")
               .Replace("DashboardBoxLinkHover = \"#777777\"", "DashboardBoxLinkHover = \"" + theme.DashboardBoxLinkHover + "\"")
               .Replace("TrackedBoxBorderColor = \"#dddddd\"", "TrackedBoxBorderColor = \"" + theme.TrackedBoxBorderColor + "\"")
               .Replace("sidebarLinkHoverBG = \"#ffffff\"", "sidebarLinkHoverBG = \"" + theme.sidebarLinkHoverBG + "\"")
               .Replace("sidebarLinkHoverColor = \"#1963aa\"", "sidebarLinkHoverColor = \"" + theme.sidebarLinkHoverColor + "\"")
               .Replace("PageTitleBG = \"#edf5fa\"", "PageTitleBG = \"" + theme.PageTitleBG + "\"")
               .Replace("PageTitleBorder1 = \"#c3ddec\"", "PageTitleBorder1 = \"" + theme.PageTitleBorder1 + "\"")
               .Replace("PanelHeadingBG = \"#edf5fa\"", "PanelHeadingBG = \"" + theme.PanelHeadingBG + "\"")
               .Replace("PanelHeadingForeColor1 = \"#3784b1\"", "PanelHeadingForeColor1 = \"" + theme.PanelHeadingForeColor1 + "\"")
               .Replace("PanelHeadingBorder = \"#c3ddec\"", "PanelHeadingBorder = \"" + theme.PanelHeadingBorder + "\"")
               .Replace("PageTitleForeColor = \"#333333\"", "PageTitleForeColor = \"" + theme.PageTitleForeColor + "\"")
               .Replace("BtnDefaultBG1 = \"#edf5fa\"", "BtnDefaultBG1 = \"" + theme.BtnDefaultBG1 + "\"")
               .Replace("BtnDefaultForecolor = \"#3784b1\"", "BtnDefaultForecolor = \"" + theme.BtnDefaultForecolor + "\"")
               .Replace("BtnDefaultHoverBG = \"#e6e6e6\"", "BtnDefaultHoverBG = \"" + theme.BtnDefaultHoverBG + "\"")
               .Replace("BtnDefaultHoverForecolor = \"#4d4d4d\"", "BtnDefaultHoverForecolor = \"" + theme.BtnDefaultHoverForecolor + "\"")
               .Replace("Borderbutton = \"#c3ddec\"", "Borderbutton = \"" + theme.Borderbutton + "\"")
               .Replace("Borderbutton1 = \"#dadada\"", "Borderbutton1 = \"" + theme.Borderbutton1 + "\"")
               .Replace("Btn2DefaultBG = \"#428bca\"", "Btn2DefaultBG = \"" + theme.Btn2DefaultBG + "\"")
               .Replace("Btn2DefaultForecolor = \"#ffffff\"", "Btn2DefaultForecolor = \"" + theme.Btn2DefaultForecolor + "\"")
               .Replace("Btn2DefaultHoverBG = \"#3276b1\"", "Btn2DefaultHoverBG = \"" + theme.Btn2DefaultHoverBG + "\"")
               .Replace("Btn2DefaultHoverForecolor = \"#ffffff\"", "Btn2DefaultHoverForecolor = \"" + theme.Btn2DefaultHoverForecolor + "\"")
               .Replace("Borderbutton2 = \"#357ebd\"", "Borderbutton2 = \"" + theme.Borderbutton2 + "\"")
               .Replace("Borderbutton3 = \"#285e8e\"", "Borderbutton3 = \"" + theme.Borderbutton3 + "\"")
               .Replace("TableHeaderBG = \"#edf5fa\"", "TableHeaderBG = \"" + theme.TableHeaderBG + "\"")
               .Replace("TableForecolor = \"#3784b1\"", "TableForecolor = \"" + theme.TableForecolor + "\"")
               .Replace("TableBorder = \"#c3ddec\"", "TableBorder = \"" + theme.TableBorder + "\"")
               .Replace("TabsActiveforecolor = \"#5b98c2\"", "TabsActiveforecolor = \"" + theme.TabsActiveforecolor + "\"")
               .Replace("TabsActiveBorder = \"#c5d0dc\"", "TabsActiveBorder = \"" + theme.TabsActiveBorder + "\"")
               .Replace("TabsActiveTopborder = \"#4c8fbd\"", "TabsActiveTopborder = \"" + theme.TabsActiveTopborder + "\"")
               .Replace("TabsActiveBackground = \"#ffffff\"", "TabsActiveBackground = \"" + theme.TabsActiveBackground + "\"")
               .Replace("TabsForecolor = \"#999999\"", "TabsForecolor = \"" + theme.TabsForecolor + "\"")
               .Replace("TabsBackground = \"#f9f9f9\"", "TabsBackground = \"" + theme.TabsBackground + "\"")
               .Replace("TabsBorder = \"#c5d0dc\"", "TabsBorder = \"" + theme.TabsBorder + "\"")
               .Replace("TabsHeadingBG = \"#edf5fa\"", "TabsHeadingBG = \"" + theme.TabsHeadingBG + "\"")
               .Replace("TabsHeadingBorder = \"#c5d0dc\"", "TabsHeadingBorder = \"" + theme.TabsHeadingBorder + "\"")
               .Replace("TabsForecolorHover = \"#4c8fbd\"", "TabsForecolorHover = \"" + theme.TabsForecolorHover + "\"")
               .Replace("TabsBackgroundHover = \"#ffffff\"", "TabsBackgroundHover = \"" + theme.TabsBackgroundHover + "\"")
               .Replace("TabsBorderHover = \"#c5d0dc\"", "TabsBorderHover = \"" + theme.TabsBorderHover + "\"")
               .Replace("SidebarActiveforcolor = \"#2b7dbc\"", "SidebarActiveforcolor = \"" + theme.SidebarActiveforcolor + "\"")
               .Replace("SidebarActiveBG = \"#ffffff\"", "SidebarActiveBG = \"" + theme.SidebarActiveBG + "\"")
               .Replace("SidebarHoverBorderLeft = \"#3382af\"", "SidebarHoverBorderLeft = \"" + theme.SidebarHoverBorderLeft + "\"")
               .Replace("FooterLinkcolor = \"#999999\"", "FooterLinkcolor = \"" + theme.FooterLinkcolor + "\"")
               .Replace("FooterForecolor = \"#2a6496\"", "FooterForecolor = \"" + theme.FooterForecolor + "\"")
               .Replace("rightwrapper = \"#ffffff\"", "rightwrapper = \"" + theme.rightwrapper + "\"");
        return temp;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) sets current theme.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the SetCurrentTheme View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SetCurrentTheme(long Id,string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,tenantId);
        _themeSettingsRepository.UpdateSingle(Id);
        SetStringCss(Id, tenantId);
        return Json("SUCCESS", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Sets custom theme create.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="theme">   The theme.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the SetCustomThemeCreate View.</returns>
    
    public ActionResult SetCustomThemeCreate(long Id, ThemeSettings theme, string tenantId)
    {
        if(((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
        {
            ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser, tenantId);
            string temp = "";
            temp = theme.CssEditor;
            //_themeSettingsRepository = new ThemeSettingRepository();
            _themeSettingsRepository.UpdateSingle(Id);
            string strVlaue = GetCssString(temp);
            if(theme.IsActive)
                System.IO.File.WriteAllText(Server.MapPath("~/Content/Site" + _themeSettingsRepository.tenantId + ""+AppTheme+".css"), strVlaue);
            return RedirectToAction("Create", "Theme");
        }
        else return RedirectToAction("Index", "Home");
    }
    
    /// <summary>Sets custom theme edit.</summary>
    ///
    /// <param name="theme">   The theme.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    
    public void SetCustomThemeEdit(Theme theme, string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser, tenantId);
        string temp = "";
        var lsttheme = (from s in _themeSettingsRepository.GetThemeSettings()
                        where(s.Id == theme.Id)
                        select s).ToList();
        if(theme.IsActive)
        {
            string strVlaue = GetCssString(lsttheme[0].CssEditor);
            System.IO.File.WriteAllText(Server.MapPath("~/Content/Site"+_themeSettingsRepository.tenantId+""+AppTheme+".css"), strVlaue);
        }
        else
        {
            temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css"));
            System.IO.File.WriteAllText(Server.MapPath("~/Content/Site" + _themeSettingsRepository.tenantId + ""+AppTheme+".css"), temp);
        }
    }
    
    /// <summary>Sets string CSS.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    
    public void SetStringCss(long Id, string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,  tenantId);
        string temp = "";
        string name = "";
        var lsttheme = (from s in _themeSettingsRepository.GetThemeSettings()
                        where(s.Id == Id)
                        select s).ToList();
        if(lsttheme.Count() == 0)
        {
            temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css"));
            System.IO.File.WriteAllText(Server.MapPath("~/Content/Site" + _themeSettingsRepository.tenantId + ""+AppTheme+".css"), temp);
            name = "Default";
        }
        else
        {
            temp = lsttheme.ToList()[0].CssEditor;
            name = lsttheme.ToList()[0].Name;
            string strVlaue = GetCssString(temp);
            if(lsttheme.ToList()[0].IsActive)
                System.IO.File.WriteAllText(Server.MapPath("~/Content/Site" + _themeSettingsRepository.tenantId + ""+AppTheme+".css"), strVlaue);
            else
            {
                //temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/SiteOriginal"+AppTheme+".css"));
                System.IO.File.WriteAllText(Server.MapPath("~/Content/Site" + _themeSettingsRepository.tenantId + ""+AppTheme+".css"), strVlaue);
            }
        }
    }
    
    /// <summary>Gets CSS string.</summary>
    ///
    /// <param name="themevalues">The themevalues.</param>
    ///
    /// <returns>The CSS string.</returns>
    
    public string GetCssString(string themevalues)
    {
        string CssFile = themevalues;
        Theme theme = new Theme();
        theme = GetCssVariableFromString(theme, themevalues);
        CssFile = CssFile.SafeReplace("@HeaderBackgroundColor", theme.HeaderBackgroundColor);
        CssFile = CssFile.SafeReplace("@HeaderAppLabel", theme.HeaderAppLabel);
        CssFile = CssFile.SafeReplace("@HeaderLinkReportBG", theme.HeaderLinkReportBG);
        CssFile = CssFile.SafeReplace("@HeaderLinkReportText", theme.HeaderLinkReportText);
        CssFile = CssFile.SafeReplace("@HeaderLinkReportTextHover", theme.HeaderLinkReportTextHover);
        CssFile = CssFile.SafeReplace("@HeaderLinkAdminBG", theme.HeaderLinkAdminBG);
        CssFile = CssFile.SafeReplace("@HeaderLinkAdminText", theme.HeaderLinkAdminText);
        CssFile = CssFile.SafeReplace("@HeaderLinkAdminTextHover", theme.HeaderLinkAdminTextHover);
        CssFile = CssFile.SafeReplace("@HeaderLinkUserBG", theme.HeaderLinkUserBG);
        CssFile = CssFile.SafeReplace("@HeaderLinkUserText", theme.HeaderLinkUserText);
        CssFile = CssFile.SafeReplace("@HeaderLinkUserTextHover", theme.HeaderLinkUserTextHover);
        CssFile = CssFile.SafeReplace("@_sidebarLinkColor", theme.sidebarLinkColor);
        CssFile = CssFile.SafeReplace("@sidebarLinkHoverColor", theme.sidebarLinkHoverColor);
        CssFile = CssFile.SafeReplace("@sidebarLinkHoverBG", theme.sidebarLinkHoverBG);
        CssFile = CssFile.SafeReplace("@_backgroundColorbody", theme.backgroundColorbody);
        CssFile = CssFile.SafeReplace("@_navlistBackground", theme.navlistBackground);
        CssFile = CssFile.SafeReplace("@_footerbgcolor", theme.footerbgcolor);
        CssFile = CssFile.SafeReplace("@DashboardBoxBG", theme.DashboardBoxBG);
        CssFile = CssFile.SafeReplace("@DashboardIconBG", theme.DashboardIconBG);
        CssFile = CssFile.SafeReplace("@DashboardBoxBorder", theme.DashboardBoxBorder);
        CssFile = CssFile.SafeReplace("@DashboardBoxHover", theme.DashboardBoxHover);
        CssFile = CssFile.SafeReplace("@DashboardBoxLink", theme.DashboardBoxLink);
        CssFile = CssFile.SafeReplace("@DashboardBoxLinkHover", theme.DashboardBoxLinkHover);
        CssFile = CssFile.SafeReplace("@TrackedBoxBorderColor", theme.TrackedBoxBorderColor);
        CssFile = CssFile.SafeReplace("@PageTitleBG", theme.PageTitleBG);
        CssFile = CssFile.SafeReplace("@PageTitleBorder1", theme.PageTitleBorder1);
        CssFile = CssFile.SafeReplace("@PanelHeadingBG", theme.PanelHeadingBG);
        CssFile = CssFile.SafeReplace("@PanelHeadingForeColor1", theme.PanelHeadingForeColor1);
        CssFile = CssFile.SafeReplace("@PanelHeadingBorder", theme.PanelHeadingBorder);
        CssFile = CssFile.SafeReplace("@PageTitleForeColor", theme.PageTitleForeColor);
        CssFile = CssFile.SafeReplace("@BtnDefaultBG1", theme.BtnDefaultBG1);
        CssFile = CssFile.SafeReplace("@BtnDefaultForecolor", theme.BtnDefaultForecolor);
        CssFile = CssFile.SafeReplace("@BtnDefaultHoverBG", theme.BtnDefaultHoverBG);
        CssFile = CssFile.SafeReplace("@BtnDefaultHoverForecolor", theme.BtnDefaultHoverForecolor);
        CssFile = CssFile.SafeReplace("@Borderbutton", theme.Borderbutton);
        CssFile = CssFile.SafeReplace("@Borderbutton1", theme.Borderbutton1);
        CssFile = CssFile.SafeReplace("@Btn2DefaultBG", theme.Btn2DefaultBG);
        CssFile = CssFile.SafeReplace("@Btn2DefaultForecolor", theme.Btn2DefaultForecolor);
        CssFile = CssFile.SafeReplace("@Btn2DefaultHoverBG", theme.Btn2DefaultHoverBG);
        CssFile = CssFile.SafeReplace("@Btn2DefaultHoverForecolor", theme.Btn2DefaultHoverForecolor);
        CssFile = CssFile.SafeReplace("@Borderbutton2", theme.Borderbutton2);
        CssFile = CssFile.SafeReplace("@Borderbutton3", theme.Borderbutton3);
        CssFile = CssFile.SafeReplace("@TableHeaderBG", theme.TableHeaderBG);
        CssFile = CssFile.SafeReplace("@TableForecolor", theme.TableForecolor);
        CssFile = CssFile.SafeReplace("@TableBorder", theme.TableBorder);
        CssFile = CssFile.SafeReplace("@TabsActiveforecolor", theme.TabsActiveforecolor);
        CssFile = CssFile.SafeReplace("@TabsActiveBorder", theme.TabsActiveBorder);
        CssFile = CssFile.SafeReplace("@TabsActiveTopborder", theme.TabsActiveTopborder);
        CssFile = CssFile.SafeReplace("@TabsActiveBackground", theme.TabsActiveBackground);
        CssFile = CssFile.SafeReplace("@TabsForecolor", theme.TabsForecolor);
        CssFile = CssFile.SafeReplace("@TabsBackground", theme.TabsBackground);
        CssFile = CssFile.SafeReplace("@TabsBorder", theme.TabsBorder);
        CssFile = CssFile.SafeReplace("@TabsHeadingBG", theme.TabsHeadingBG);
        CssFile = CssFile.SafeReplace("@TabsHeadingBorder", theme.TabsHeadingBorder);
        CssFile = CssFile.SafeReplace("@TabsForecolorHover", theme.TabsForecolorHover);
        CssFile = CssFile.SafeReplace("@TabsBackgroundHover", theme.TabsBackgroundHover);
        CssFile = CssFile.SafeReplace("@TabsBorderHover", theme.TabsBorderHover);
        CssFile = CssFile.SafeReplace("@SidebarActiveforcolor", theme.SidebarActiveforcolor);
        CssFile = CssFile.SafeReplace("@SidebarActiveBG", theme.SidebarActiveBG);
        CssFile = CssFile.SafeReplace("@SidebarHoverBorderLeft", theme.SidebarHoverBorderLeft);
        CssFile = CssFile.SafeReplace("@FooterForecolor", theme.FooterForecolor);
        CssFile = CssFile.SafeReplace("@FooterLinkcolor", theme.FooterLinkcolor);
        CssFile = CssFile.SafeReplace("@rightwrapper", theme.rightwrapper);
        return CssFile;
    }
    
    /// <summary>Sets CSS variable value.</summary>
    ///
    /// <param name="theme">        The theme.</param>
    /// <param name="propertyName"> Name of the property.</param>
    /// <param name="propertyValue">The property value.</param>
    /// <param name="varlist">      The varlist.</param>
    
    private void setCssVariableValue(Theme theme, string propertyName, string propertyValue, Dictionary<string, string> varlist)
    {
        var type = theme.GetType();
        var property = type.GetProperty(propertyName);
        if(property != null)
        {
            var newvalue = varlist.Where(p => p.Key.Trim() == propertyValue)
                           .FirstOrDefault();
            if(!string.IsNullOrEmpty(newvalue.Value))
                property.SetValue(theme, newvalue.Value.Replace("\"", "").Replace(";", "").Trim());
        }
    }
    
    /// <summary>Gets CSS variable from string.</summary>
    ///
    /// <param name="theme"> The theme.</param>
    /// <param name="strVar">The variable.</param>
    ///
    /// <returns>The CSS variable from string.</returns>
    
    public Theme GetCssVariableFromString(Theme theme, string strVar)
    {
        Dictionary<string, string> varlist = new Dictionary<string, string>();
        using(StringReader sr = new StringReader(strVar))
        {
            String input;
            while((input = sr.ReadLine()) != null)
            {
                if(input.Contains("var"))
                    varlist.Add(input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[0].TrimStart(), input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[1].TrimStart());
            }
        }
        theme = setThemeVariables(theme, varlist);
        return theme;
    }
    
    /// <summary>Sets theme variables.</summary>
    ///
    /// <param name="theme">  The theme.</param>
    /// <param name="varlist">The varlist.</param>
    ///
    /// <returns>A Theme.</returns>
    
    private Theme setThemeVariables(Theme theme,Dictionary<string,string> varlist)
    {
        setCssVariableValue(theme, "sidebarLinkColor", "_sidebarLinkColor", varlist);
        setCssVariableValue(theme, "backgroundColorbody", "_backgroundColorbody", varlist);
        setCssVariableValue(theme, "navlistBackground", "_navlistBackground", varlist);
        setCssVariableValue(theme, "footerbgcolor", "_footerbgcolor", varlist);
        setCssVariableValue(theme, "HeaderBackgroundColor", "HeaderBackgroundColor", varlist);
        setCssVariableValue(theme, "HeaderAppLabel", "HeaderAppLabel", varlist);
        setCssVariableValue(theme, "HeaderLinkReportBG", "HeaderLinkReportBG", varlist);
        setCssVariableValue(theme, "HeaderLinkReportText", "HeaderLinkReportText", varlist);
        setCssVariableValue(theme, "HeaderLinkReportTextHover", "HeaderLinkReportTextHover", varlist);
        setCssVariableValue(theme, "HeaderLinkAdminBG", "HeaderLinkAdminBG", varlist);
        setCssVariableValue(theme, "HeaderLinkAdminText", "HeaderLinkAdminText", varlist);
        setCssVariableValue(theme, "HeaderLinkAdminTextHover", "HeaderLinkAdminTextHover", varlist);
        setCssVariableValue(theme, "HeaderLinkUserBG", "HeaderLinkUserBG", varlist);
        setCssVariableValue(theme, "HeaderLinkUserText", "HeaderLinkUserText", varlist);
        setCssVariableValue(theme, "HeaderLinkUserTextHover", "HeaderLinkUserTextHover", varlist);
        setCssVariableValue(theme, "DashboardIconBG", "DashboardBoxBG", varlist);
        setCssVariableValue(theme, "DashboardBoxBorder", "DashboardBoxBorder", varlist);
        setCssVariableValue(theme, "DashboardBoxHover", "DashboardBoxHover", varlist);
        setCssVariableValue(theme, "DashboardBoxLink", "DashboardBoxLink", varlist);
        setCssVariableValue(theme, "DashboardBoxLinkHover", "DashboardBoxLinkHover", varlist);
        setCssVariableValue(theme, "TrackedBoxBorderColor", "TrackedBoxBorderColor", varlist);
        setCssVariableValue(theme, "sidebarLinkHoverBG", "sidebarLinkHoverBG", varlist);
        setCssVariableValue(theme, "sidebarLinkHoverColor", "sidebarLinkHoverColor", varlist);
        setCssVariableValue(theme, "PageTitleBG", "PageTitleBG", varlist);
        setCssVariableValue(theme, "PageTitleBorder1", "PageTitleBorder1", varlist);
        setCssVariableValue(theme, "PanelHeadingBG", "PanelHeadingBG", varlist);
        setCssVariableValue(theme, "PanelHeadingForeColor1", "PanelHeadingForeColor1", varlist);
        setCssVariableValue(theme, "PanelHeadingBorder", "PanelHeadingBorder", varlist);
        setCssVariableValue(theme, "PageTitleForeColor", "PageTitleForeColor", varlist);
        setCssVariableValue(theme, "BtnDefaultBG1", "BtnDefaultBG1", varlist);
        setCssVariableValue(theme, "BtnDefaultForecolor", "BtnDefaultForecolor", varlist);
        setCssVariableValue(theme, "BtnDefaultHoverBG", "BtnDefaultHoverBG", varlist);
        setCssVariableValue(theme, "BtnDefaultHoverForecolor", "BtnDefaultHoverForecolor", varlist);
        setCssVariableValue(theme, "Borderbutton", "Borderbutton", varlist);
        setCssVariableValue(theme, "Borderbutton1", "Borderbutton1", varlist);
        setCssVariableValue(theme, "Btn2DefaultBG", "Btn2DefaultBG", varlist);
        setCssVariableValue(theme, "Btn2DefaultForecolor", "Btn2DefaultForecolor", varlist);
        setCssVariableValue(theme, "Btn2DefaultHoverBG", "Btn2DefaultHoverBG", varlist);
        setCssVariableValue(theme, "Btn2DefaultHoverForecolor", "Btn2DefaultHoverForecolor", varlist);
        setCssVariableValue(theme, "Borderbutton2", "Borderbutton2", varlist);
        setCssVariableValue(theme, "Borderbutton3", "Borderbutton3", varlist);
        setCssVariableValue(theme, "TableHeaderBG", "TableHeaderBG", varlist);
        setCssVariableValue(theme, "TableForecolor", "TableForecolor", varlist);
        setCssVariableValue(theme, "TableBorder", "TableBorder", varlist);
        setCssVariableValue(theme, "TabsActiveforecolor", "TabsActiveforecolor", varlist);
        setCssVariableValue(theme, "TabsActiveBorder", "TabsActiveBorder", varlist);
        setCssVariableValue(theme, "TabsActiveTopborder", "TabsActiveTopborder", varlist);
        setCssVariableValue(theme, "TabsActiveBackground", "TabsActiveBackground", varlist);
        setCssVariableValue(theme, "TabsForecolor", "TabsForecolor", varlist);
        setCssVariableValue(theme, "TabsBackground", "TabsBackground", varlist);
        setCssVariableValue(theme, "TabsHeadingBG", "TabsHeadingBG", varlist);
        setCssVariableValue(theme, "TabsBorder", "TabsBorder", varlist);
        setCssVariableValue(theme, "TabsHeadingBorder", "TabsHeadingBorder", varlist);
        setCssVariableValue(theme, "TabsForecolorHover", "TabsForecolorHover", varlist);
        setCssVariableValue(theme, "TabsBackgroundHover", "TabsBackgroundHover", varlist);
        setCssVariableValue(theme, "TabsBorderHover", "TabsBorderHover", varlist);
        setCssVariableValue(theme, "SidebarActiveforcolor", "SidebarActiveforcolor", varlist);
        setCssVariableValue(theme, "SidebarActiveBG", "SidebarActiveBG", varlist);
        setCssVariableValue(theme, "SidebarHoverBorderLeft", "SidebarHoverBorderLeft", varlist);
        setCssVariableValue(theme, "FooterLinkcolor", "FooterLinkcolor", varlist);
        setCssVariableValue(theme, "FooterForecolor", "FooterForecolor", varlist);
        setCssVariableValue(theme, "rightwrapper", "rightwrapper", varlist);
        return theme;
    }
    
    /// <summary>Mobile Theme.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    
    public void EditMobile(long Id, string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser,  tenantId);
        _themeSettingsRepository.UpdateAllMobile(Id);
    }
    
    /// <summary>Gets theme CSS for mobile.</summary>
    ///
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The theme CSS for mobile.</returns>
    
    public string GetThemeCssForMobile(string tenantId)
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository(LogggedInUser, tenantId);
        string temp = "";
        // _themeSettingsRepository = new ThemeSettingRepository();
        var lstthemeMobile = from s in _themeSettingsRepository.GetThemeMobileSettings()
                             where(s.IsActive == true)
                             select s;
        if(lstthemeMobile.Count() == 0)
            temp = "skyblue";
        else
        {
            temp = lstthemeMobile.ToList()[0].CssName;
        }
        return temp;
        //Response.ContentType = "text/css";
        // return Razor.Parse(temp.Replace("@media", "@@media").Replace("@font", "@@font"));
    }
    //end Mobile Theme
}
/// <summary>A string extensions.</summary>
public static class StringExtensions
{
    /// <summary>A string extension method that safe replace.</summary>
    ///
    /// <param name="s">     The s to act on.</param>
    /// <param name="word">  The word.</param>
    /// <param name="bywhat">The bywhat.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string SafeReplace(this string s, string word, string bywhat)
    {
        char firstLetter = word[0];
        System.Text.StringBuilder  sb = new System.Text.StringBuilder();
        bool previousWasLetterOrDigit = false;
        int i = 0;
        while(i < s.Length - word.Length + 1)
        {
            bool wordFound = false;
            char c = s[i];
            if(c == firstLetter)
                if(!previousWasLetterOrDigit)
                    if(s.Substring(i, word.Length).Equals(word))
                    {
                        wordFound = true;
                        bool wholeWordFound = true;
                        if(s.Length > i + word.Length)
                        {
                            if(Char.IsLetterOrDigit(s[i + word.Length]))
                                wholeWordFound = false;
                        }
                        if(wholeWordFound)
                            sb.Append(bywhat);
                        else
                            sb.Append(word);
                        i += word.Length;
                    }
            if(!wordFound)
            {
                previousWasLetterOrDigit = Char.IsLetterOrDigit(c);
                sb.Append(c);
                i++;
            }
        }
        if(s.Length - i > 0)
            sb.Append(s.Substring(i));
        return sb.ToString();
    }
}

}