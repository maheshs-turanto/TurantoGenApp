using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Models
{
/// <summary>An entity default page helper.</summary>
public class EntityDefaultPageHelper
{
    /// <summary>get Templates
    ///  select templates.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for list.</returns>
    
    public static string GetTemplatesForList(IUser User, string entityName, string defaultview)
    {
        string pageNameList = "";
        string result = defaultview;
        //ViewBag.BuiltInPage = false;
        bool IsInRoles = false;
        using(var context = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in context.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.ListEntityPage!= "IndexPartial"
                                       select s;
            var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All");
            //var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault();
            if(DefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, DefaultEntityPage.ListEntityPage);
                if(!hasView)
                    DefaultEntityPage = null;
            }
            if(DefaultEntityPage != null)
            {
                string role = DefaultEntityPage.Roles;
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
                var defaultEntityPage = DefaultEntityPage.ListEntityPage;
                if(!string.IsNullOrEmpty(defaultEntityPage))
                {
                    pageNameList = defaultEntityPage;
                    if(DefaultEntityPage.PageType == "Default")
                    {
                        //ViewBag.BuiltInPage = true;
                    }
                }
            }
        }
        if(!String.IsNullOrEmpty(pageNameList) && IsInRoles)
            result = pageNameList;
        return result;
    }
    
    /// <summary>Gets templates for details.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for details.</returns>
    
    public static string GetTemplatesForDetails(IUser User, string entityName, string defaultview)
    {
        string pageNameList = "";
        string result = defaultview;
        //ViewBag.BuiltInPage = false;
        bool IsInRoles = false;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.ViewEntityPage!= "Details"
                                       select s;
            var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All");
            //var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault();
            if(DefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, DefaultEntityPage.ViewEntityPage);
                if(!hasView)
                    DefaultEntityPage = null;
            }
            if(DefaultEntityPage != null)
            {
                string role = DefaultEntityPage.Roles;
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
                var defaultEntityPage = DefaultEntityPage.ViewEntityPage;
                if(!string.IsNullOrEmpty(defaultEntityPage) && defaultEntityPage != "Details")
                {
                    pageNameList = defaultEntityPage;
                    if(DefaultEntityPage.PageType == "Default")
                    {
                        //ViewBag.BuiltInPage = true;
                    }
                }
            }
        }
        if(!String.IsNullOrEmpty(pageNameList) && IsInRoles)
            result = pageNameList;
        return result;
    }
    
    /// <summary>Gets templates for edit.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit.</returns>
    
    public static string GetTemplatesForEdit(IUser User, string entityName, string defaultview)
    {
        string pageNameList = "";
        string result = defaultview;
        //ViewBag.BuiltInPage = false;
        bool IsInRoles = false;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.EditEntityPage!= "Edit"
                                       select s;
            var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All");
            //var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault();
            if(DefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, DefaultEntityPage.EditEntityPage);
                if(!hasView)
                    DefaultEntityPage = null;
            }
            if(DefaultEntityPage != null)
            {
                string role = DefaultEntityPage.Roles;
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
                var defaultEntityPage = DefaultEntityPage.EditEntityPage;
                if(!string.IsNullOrEmpty(defaultEntityPage) && defaultEntityPage != "Edit")
                {
                    pageNameList = defaultEntityPage;
                    if(DefaultEntityPage.PageType == "Default")
                    {
                        // ViewBag.BuiltInPage = true;
                    }
                }
            }
        }
        if(!String.IsNullOrEmpty(pageNameList) && IsInRoles)
            result = pageNameList;
        return result;
    }
    
    /// <summary>Gets templates for create quick.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create quick.</returns>
    
    public static string GetTemplatesForCreateQuick(IUser User, string entityName, string defaultview)
    {
        string pageDetails = "";
        bool IsInRoles = false;
        string result = defaultview;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.CreateEntityPage != "Create"
                                       select s;
            if(lstDefaultEntityPage != null)
            {
                string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").Roles;
                //string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").CreateEntityPage;
                //var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.CreateEntityPage).FirstOrDefault();
                if(defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
        }
        if(!String.IsNullOrEmpty(pageDetails) && IsInRoles)
            result = pageDetails;
        return result;
    }
    
    /// <summary>Gets templates for create.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create.</returns>
    
    public static string GetTemplatesForCreate(IUser User, string entityName, string defaultview)
    {
        string pageNameList = "";
        string result = defaultview;
        bool IsInRoles = false;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.CreateEntityPage!= "Create"
                                       select s;
            var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All");
            //var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault();
            if(DefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, DefaultEntityPage.CreateEntityPage);
                if(!hasView)
                    DefaultEntityPage = null;
            }
            if(DefaultEntityPage != null)
            {
                string role = DefaultEntityPage.Roles;
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
                var defaultEntityPage = DefaultEntityPage.CreateEntityPage;
                if(!string.IsNullOrEmpty(defaultEntityPage) && defaultEntityPage != "Create")
                {
                    pageNameList = defaultEntityPage;
                    if(DefaultEntityPage.PageType == "Default")
                    {
                        //ViewBag.BuiltInPage = true;
                    }
                }
            }
        }
        if(!String.IsNullOrEmpty(pageNameList) && IsInRoles)
            result = pageNameList;
        return result;
    }
    
    /// <summary>Gets templates for edit quick.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit quick.</returns>
    
    public static string GetTemplatesForEditQuick(IUser User, string entityName, string defaultview)
    {
        string pageDetails = "";
        bool IsInRoles = false;
        string result = defaultview;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false
                                       select s;
            if(lstDefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").EditEntityPage);
                //bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.EditEntityPage).FirstOrDefault());
                if(!hasView)
                    lstDefaultEntityPage = null;
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").Roles;
                //string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All"> p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.EditEntityPage).FirstOrDefault();
                if(defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
        }
        if(!String.IsNullOrEmpty(pageDetails) && IsInRoles)
            result = pageDetails;
        return result;
    }
    
    /// <summary>Gets templates for create wizard.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for create wizard.</returns>
    
    public static string GetTemplatesForCreateWizard(IUser User, string entityName, string defaultview)
    {
        string pageDetails = "";
        bool IsInRoles = false;
        string result = defaultview;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false && s.CreateEntityPage != "Create"
                                       select s;
            if(lstDefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").CreateWizardEntityPage);
                //bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.CreateWizardEntityPage).FirstOrDefault());
                if(!hasView)
                    lstDefaultEntityPage = null;
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").Roles;
                //string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").CreateWizardEntityPage;
                //var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.CreateWizardEntityPage).FirstOrDefault();
                if(defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
        }
        if(!String.IsNullOrEmpty(pageDetails) && IsInRoles)
            result = pageDetails;
        return result;
    }
    
    /// <summary>Gets templates for edit wizard.</summary>
    ///
    /// <param name="User">       The user.</param>
    /// <param name="entityName"> Name of the entity.</param>
    /// <param name="defaultview">The defaultview.</param>
    ///
    /// <returns>The templates for edit wizard.</returns>
    
    public static string GetTemplatesForEditWizard(IUser User, string entityName, string defaultview)
    {
        string pageDetails = "";
        bool IsInRoles = false;
        string result = defaultview;
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false
                                       select s;
            if(lstDefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").EditWizardEntityPage);
                //bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.EditWizardEntityPage).FirstOrDefault());
                if(!hasView)
                    lstDefaultEntityPage = null;
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").Roles;
                //string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").EditWizardEntityPage;
                //var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.EditWizardEntityPage).FirstOrDefault();
                if(defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
        }
        if(!String.IsNullOrEmpty(pageDetails) && IsInRoles)
            result = pageDetails;
        return result;
    }
    
    /// <summary>Gets templates for search.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The templates for search.</returns>
    
    public static string GetTemplatesForSearch(IUser User, string entityName)
    {
        string pageSearch = "";
        bool IsInRoles = false;
        string result = "SetFSearch";
        using(var appcontext = (new ApplicationContext(new SystemUser())))
        {
            var lstDefaultEntityPage = from s in appcontext.DefaultEntityPages.AsNoTracking()
                                       where s.EntityName == entityName && s.Flag == false
                                       select s;
            if(lstDefaultEntityPage != null)
            {
                bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").SearchEntityPage);
                //bool hasView = HaveCustumTamplates(entityName, lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.SearchEntityPage).FirstOrDefault());
                if(!hasView)
                    lstDefaultEntityPage = null;
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").Roles;
                //string role = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
            }
            if(lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Count() > 0)
            {
                var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All").SearchEntityPage;
                //var defaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().Select(p => p.SearchEntityPage).FirstOrDefault();
                if(defaultEntityPage != null)
                    pageSearch = defaultEntityPage.ToString();
            }
        }
        if(!String.IsNullOrEmpty(pageSearch) && IsInRoles)
            result = pageSearch;
        return result;
    }
    public static string GetLayout(IUser User, string theme, string pagename)
    {
        string pageNameList = "";
        var result = "";
        bool IsInRoles = false;
        using(var context = (new ApplicationContext(new SystemUser())))
        {
            string themeName = theme;
            if(theme == "")
                themeName = "angular";
            var lstDefaultEntityPage = from s in context.DefaultEntityPages.AsNoTracking()
                                       where s.PageType.ToLower() == "layouts" && s.Other.ToLower() == themeName && s.Flag == false && s.EntityName.ToLower().Trim() == pagename.ToLower().Trim()
                                       select s;
            var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault(p => User.IsInRole(p.Roles.Split(',').ToArray()) == true || p.Roles == "All");
            //var DefaultEntityPage = lstDefaultEntityPage.GetFromCache<IQueryable<DefaultEntityPage>, DefaultEntityPage>().ToList().FirstOrDefault();
            if(DefaultEntityPage != null)
            {
                var angularlayouts = DefaultEntityPage.Other;
                if(DefaultEntityPage != null && angularlayouts.ToLower() == "angular")
                {
                    angularlayouts = "";
                }
                bool hasView = HaveCustumLayOuts(angularlayouts, DefaultEntityPage.PageUrl);
                if(!hasView)
                    DefaultEntityPage = null;
            }
            if(DefaultEntityPage != null)
            {
                string role = DefaultEntityPage.Roles;
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());
                }
                var defaultEntityPage = DefaultEntityPage.PageUrl;
                if(!string.IsNullOrEmpty(defaultEntityPage))
                {
                    pageNameList = defaultEntityPage;
                }
            }
        }
        if(!String.IsNullOrEmpty(pageNameList) && IsInRoles)
            result = pageNameList;
        return result;
    }
    
    public static bool HaveCustumTamplates(string entityName, string pagename)
    {
        bool hasView = false;
        if(pagename.ToLower() == "indexpartiallist")
            return true;
        var tamplateName = System.IO.Directory.EnumerateFiles(System.Web.HttpContext.Current.Server.MapPath(@"~/Views/" + entityName)).Where(f => f.Contains(pagename)).FirstOrDefault();
        if(!string.IsNullOrEmpty(tamplateName))
            hasView = true;
        return hasView;
    }
    public static bool HaveCustumLayOuts(string layoutname, string pagename)
    {
        bool hasView = false;
        var tamplateName = System.IO.Directory.EnumerateFiles(System.Web.HttpContext.Current.Server.MapPath(@"~/Views" + layoutname.Trim() + "/Shared")).Where(f => f.Contains(pagename.Trim() + ".cshtml")).FirstOrDefault();
        if(!string.IsNullOrEmpty(tamplateName))
            hasView = true;
        return hasView;
    }
    //
}
}