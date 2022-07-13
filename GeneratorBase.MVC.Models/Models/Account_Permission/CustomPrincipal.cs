using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace GeneratorBase.MVC.Models
{
/// <summary>A custom principal (Role-Based Authentication Control).</summary>
public class CustomPrincipal : IPrincipal, IUser
{
    /// <summary>Gets or sets the permissions.</summary>
    ///
    /// <value>The permissions.</value>
    
    public List<Permission> permissions
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the businessrules.</summary>
    ///
    /// <value>The businessrules.</value>
    
    public List<BusinessRule> businessrules
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the appsettings.</summary>
    ///
    /// <value>The appsettings.</value>
    
    public List<AppSetting> appsettings
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the multi tenant login selected.</summary>
    ///
    /// <value>The multi tenant login selected.</value>
    
    public List<MultiTenantLoginSelected> MultiTenantLoginSelected
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the original.</summary>
    ///
    /// <value>The original.</value>
    
    public IPrincipal Original
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    public string Name
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the name of the java script encoded.</summary>
    ///
    /// <value>The name of the java script encoded.</value>
    
    public string JavaScriptEncodedName
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is admin.</summary>
    ///
    /// <value>True if this object is admin, false if not.</value>
    
    public bool IsAdmin
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the adminprivileges.</summary>
    ///
    /// <value>The adminprivileges.</value>
    
    public List<PermissionAdminPrivilege> adminprivileges
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the userroles.</summary>
    ///
    /// <value>The userroles.</value>
    
    public List<string> userroles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the extra multitenant priviledges.</summary>
    ///
    /// <value>The extra multitenant priviledges.</value>
    
    public List<long> extraMultitenantPriviledges
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the userbasedsecurity.</summary>
    ///
    /// <value>The userbasedsecurity.</value>
    
    public List<UserBasedSecurity> userbasedsecurity
    {
        get;
        set;
    }
    
    /// <summary>public WindowsIdentity Identity { get; private set; } //FOR WINDOWS AUTHENTICATION....</summary>
    ///
    /// <value>The identity.</value>
    
    public dynamic Identity
    {
        get;
        private set;
    }
    
    /// <summary>Gets the identity of the current principal.</summary>
    ///
    /// <value>The <see cref="T:System.Security.Principal.IIdentity" /> object associated with the
    /// current principal.</value>
    
    IIdentity IPrincipal.Identity
    {
        get
        {
            var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
            if(Convert.ToBoolean(UseActiveDirectory))
                return (WindowsIdentity)this.Identity;
            else
                return (IIdentity)this.Identity;
        }
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="original">The original.</param>
    
    public CustomPrincipal(IPrincipal original)
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            if(!(original.Identity is WindowsIdentity)) throw new NotImplementedException();
            this.Identity = (WindowsIdentity)original.Identity;
        }
        else
        {
            this.Identity = (IIdentity)original.Identity;
        }
        this.Original = original;
        //this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name);
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            //this.Name = original.Identity.Name;
            //changed for active directory
            if(original.Identity != null && original.Identity.Name.Split('\\').Length > 1)
            {
                this.Name = original.Identity.Name.Split('\\')[1];
                this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name.Split('\\')[1]);
            }
        }
        else
        {
            this.Name = original.Identity.Name;
            this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name);
        }
    }
    
    /// <summary>Gets the roles in this collection.</summary>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the roles in this collection.</returns>
    //public IEnumerable<string> GetRoles()
    //{
    //    //FOR WINDOWS AUTHENTICATION....
    //    var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
    //    if (Convert.ToBoolean(UseActiveDirectory))
    //    {
    //        //var adl = new ActiveDirectoryLookup(this.Identity);
    //        //return adl.GetRoles();
    //        var UseActiveDirectoryRole = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
    //        if (Convert.ToBoolean(UseActiveDirectoryRole))
    //        {
    //            var adl = new ActiveDirectoryLookup(this.Identity);
    //            return adl.GetRoles();
    //        }
    //        else
    //        {
    //            var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
    //            CustomRoleProvider RoleProvider = new CustomRoleProvider();
    //            string usrname = this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim();
    //            return RoleProvider.GetRolesForUser(usrname);
    //        }
    //    }
    //    else
    //    {
    //        CustomRoleProvider RoleProvider = new CustomRoleProvider();
    //        return RoleProvider.GetRolesForUser(((IIdentity)this.Identity).Name);
    //    }
    //}
    public IEnumerable<string> GetRoles()
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            var UseActiveDirectoryRole = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            if(Convert.ToBoolean(UseActiveDirectoryRole))
            {
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                var applicationRoles = RoleProvider.GetAllRoles();
                var adl = new ActiveDirectoryLookup(this.Identity);
                var adRoles = adl.GetRoles();
                var commonRoles = adRoles.Intersect(applicationRoles).ToArray();
                return commonRoles;
            }
            else
            {
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                string usrname = this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim();
                return RoleProvider.GetRolesForUser(usrname);
            }
        }
        else
        {
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.GetRolesForUser(((IIdentity)this.Identity).Name);
        }
    }
    
    public IEnumerable<string> GetAllRoles()
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            var UseActiveDirectoryRole = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            if(Convert.ToBoolean(UseActiveDirectoryRole))
            {
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                var applicationRoles = RoleProvider.GetAllRoles();
                var adl = new ActiveDirectoryLookup(this.Identity);
                var adRoles = adl.GetRoles();
                var commonRoles = adRoles.Intersect(applicationRoles).ToArray();
                return commonRoles;
            }
            else
            {
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                string usrname = this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim();
                return RoleProvider.GetAllRolesForUser(usrname);
            }
        }
        else
        {
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.GetAllRolesForUser(((IIdentity)this.Identity).Name);
        }
    }
    
    /// <summary>Determines whether the current principal belongs to the specified role.</summary>
    ///
    /// <param name="role">The name of the role for which to check membership.</param>
    ///
    /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
    
    public bool IsInRole(string role)
    {
        /// This is a magic string. We should call IsAdmin instead of IsInRole("Admin")
        if("Admin".Equals(role, System.StringComparison.Ordinal))
        {
            return IsAdminUser();
        }
        return this.GetRoles().Contains(role);
    }
    
    /// <summary>Query if 'roles' is in role.</summary>
    ///
    /// <param name="roles">The roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(string[] roles)
    {
        var rolelist = roles.ToList().ConvertAll(d => d.ToUpper().Trim());
        if(rolelist.Contains("ALL")) return true;
        foreach(var str in this.GetRoles())
        {
            if(rolelist.Contains(str.Trim().ToUpper())) return true;
        }
        return false;
    }
    
    /// <summary>Query if 'roles' is in role.</summary>
    ///
    /// <param name="userroles">The userroles.</param>
    /// <param name="roles">    The roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(List<string> userroles, string[] roles)
    {
        var rolelist = roles.ToList().ConvertAll(d => d.ToUpper().Trim());
        if(rolelist.Contains("ALL")) return true;
        foreach(var str in userroles)
        {
            if(rolelist.Contains(str.Trim().ToUpper())) return true;
        }
        return false;
    }
    
    /// <summary>Query if 'roles' is in role.</summary>
    ///
    /// <param name="roles">   The roles.</param>
    /// <param name="AllRoles">all roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(string[] roles, IEnumerable<string> AllRoles)
    {
        var rolelist = roles.ToList().ConvertAll(d => d.ToUpper().Trim());
        if(rolelist.Contains("ALL")) return true;
        foreach(var str in AllRoles)
        {
            if(rolelist.Contains(str.Trim().ToUpper())) return true;
        }
        return false;
    }
    
    /// <summary>Query if 'username' is admin user.</summary>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    public bool IsAdminUser()
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            //var adl = new ActiveDirectoryLookup(this.Identity);
            //var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            //var admins = adminString.Split(',', ';');
            //return adl.GetRoles().Any(r => admins.Contains(r));
            var UseActiveDirectoryRole = ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            if(Convert.ToBoolean(UseActiveDirectoryRole))
            {
                var adl = new ActiveDirectoryLookup(this.Identity);
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                var admins = adminString.Split(',', ';');
                return adl.GetRoles().Any(r => admins.Contains(r));
            }
            else
            {
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.IsUserInRole(this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim(), adminString);
            }
        }
        else
        {
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.IsUserInRole(((IIdentity)this.Identity).Name, adminString);
        }
    }
    
    /// <summary>Gets admin roles.</summary>
    ///
    /// <returns>The admin roles.</returns>
    
    public string GetAdminRoles()
    {
        return System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
    }
    
    /// <summary>Query if 'username' is admin user.</summary>
    ///
    /// <param name="username">The username.</param>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    public bool IsAdminUser(string username)
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            //var adl = new ActiveDirectoryLookup(this.Identity);
            //var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            //var admins = adminString.Split(',', ';');
            //return adl.GetRoles().Any(r => admins.Contains(r));
            var UseActiveDirectoryRole = ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            if(Convert.ToBoolean(UseActiveDirectoryRole))
            {
                var adl = new ActiveDirectoryLookup(this.Identity);
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                var admins = adminString.Split(',', ';');
                return adl.GetRoles().Any(r => admins.Contains(r));
            }
            else
            {
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.IsUserInRole(username, adminString);
            }
        }
        else
        {
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.IsUserInRole(username, adminString);
        }
    }
    
    /// <summary>Query if 'username' is admin user.</summary>
    ///
    /// <param name="username">The username.</param>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    public bool IsAdminUserAD(string username)
    {
        //FOR WINDOWS AUTHENTICATION....
        var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if(Convert.ToBoolean(UseActiveDirectory))
        {
            var UseActiveDirectoryRole = ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
            if(Convert.ToBoolean(UseActiveDirectoryRole))
            {
                var adl = new ActiveDirectoryLookup(this.Identity);
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                var admins = adminString.Split(',', ';');
                bool flag = adl.GetGroupsByUserIdentity(username).Any(r => admins.Contains(r));
                return flag;
            }
            else
            {
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.IsUserInRole(username, adminString);
            }
        }
        else
        {
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.IsUserInRole(username, adminString);
        }
    }
    
    /// <summary>Gets faceted search.</summary>
    ///
    /// <param name="entityname">The entityname.</param>
    /// <param name="user">      The user.</param>
    ///
    /// <returns>The faceted search.</returns>
    
    public List<T_FacetedSearch> GetFacetedSearch(string entityname, IUser user)
    {
        using(ApplicationContext db = new ApplicationContext(new SystemUser()))
        {
            var list = db.T_FacetedSearchs.Where(p => p.T_EntityName == entityname && !p.T_Disable.Value).GetFromCache<IQueryable<T_FacetedSearch>, T_FacetedSearch>().ToList();
            if(!user.IsAdmin)
                list = list.Where(p => string.IsNullOrEmpty(p.T_Roles) || user.IsInRole(user.userroles, p.T_Roles.Split(",".ToCharArray()))).ToList();
            return list;
        }
    }
    
    /// <summary>Determine if we can add.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can add, false if not.</returns>
    
    public bool CanAdd(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanAdd);
    }
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public bool CanView(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanView);
    }
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public string ViewRestrict(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return "";
        if(this.IsAdmin) return "";
        if(this.permissions.Any(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.ViewR)))
        {
            var obj = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.ViewR));
            return obj != null ? obj.ViewR : string.Empty;
        }
        return string.Empty;
    }
    public string EditRestrict(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return "";
        if(this.IsAdmin) return "";
        if(this.permissions.Any(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.EditR)))
        {
            var obj = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.EditR));
            return obj != null ? obj.EditR : string.Empty;
        }
        return string.Empty;
    }
    public string DeleteRestrict(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return "";
        if(this.IsAdmin) return "";
        if(this.permissions.Any(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.DeleteR)))
        {
            var obj = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.DeleteR));
            return obj != null ? obj.DeleteR : string.Empty;
        }
        return string.Empty;
    }
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public bool CanView(string resource, string property)
    {
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return CanView(resource) && this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoView == null || !p.NoView.Contains(property + ",")));
    }
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    public bool CanEdit(string resource)
    {
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanEdit);
    }
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    public bool CanEdit(string resource, string property)
    {
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return CanEdit(resource) && this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoEdit == null || !p.NoEdit.Contains(property + ",")));
    }
    
    /// <summary>Determine if we can delete.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can delete, false if not.</returns>
    
    public bool CanDelete(string resource)
    {
        if(this.IsAdmin) return true;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanDelete);
    }
    
    /// <summary>Impose owner permission.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ImposeOwnerPermission(string resource)
    {
        if(this.IsAdmin) return false;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return false;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.IsOwner.Value);
    }
    
    /// <summary>Owner association.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    public string OwnerAssociation(string resource)
    {
        if(this.permissions.Any(p => p.EntityName.Equals(resource) && p.IsOwner.Value))
        {
            var obj = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && p.IsOwner.Value && !string.IsNullOrEmpty(p.UserAssociation));
            return obj != null ? obj.UserAssociation : string.Empty;
        }
        return string.Empty;
    }
    
    /// <summary>Determine if we can edit item.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="obj">     The object.</param>
    /// <param name="User">    The user.</param>
    ///
    /// <returns>True if we can edit item, false if not.</returns>
    
    public bool CanEditItem(string resource, object obj, IUser User)
    {
        bool result = CanEdit(resource) && (!ImposeOwnerPermission(resource) || (ImposeOwnerPermission(resource) && (!(new CheckPermissionForOwner()).CheckOwnerPermission(resource, obj, User, false)))) && !((new CheckPermissionForOwner()).CheckLockCondition(resource, obj, User, false));
        if(result && !string.IsNullOrEmpty(User.EditRestrict(resource)))
            result = result && HiearchicalSecurityHelper.HierarchicalSecurityEditCheck(resource, User, (new ApplicationContext(new SystemUser())), User.EditRestrict(resource), obj);
        return result;
    }
    
    /// <summary>Determine if we can delete item.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="obj">     The object.</param>
    /// <param name="User">    The user.</param>
    ///
    /// <returns>True if we can delete item, false if not.</returns>
    
    public bool CanDeleteItem(string resource, object obj, IUser User)
    {
        bool result = CanDelete(resource) && (!ImposeOwnerPermission(resource) || (ImposeOwnerPermission(resource) && (!(new CheckPermissionForOwner()).CheckOwnerPermission(resource, obj, User, false)))) && !((new CheckPermissionForOwner()).CheckLockCondition(resource, obj, User, false));
        if(result && !string.IsNullOrEmpty(User.DeleteRestrict(resource)))
            result = result && HiearchicalSecurityHelper.HierarchicalSecurityDeleteCheck(resource, User, (new ApplicationContext(new SystemUser())), User.DeleteRestrict(resource), obj);
        return result;
    }
    
    public bool CanDeleteItemInHierarchy(string resource, object obj, IUser User)
    {
        bool result = true;
        if(!string.IsNullOrEmpty(User.DeleteRestrict(resource)))
            result = HiearchicalSecurityHelper.HierarchicalSecurityDeleteCheck(resource, User, (new ApplicationContext(new SystemUser())), User.DeleteRestrict(resource), obj);
        return result;
    }
    public bool CanEditItemInHierarchy(string resource, object obj, IUser User)
    {
        bool result = true;
        if(!string.IsNullOrEmpty(User.EditRestrict(resource)))
            result = HiearchicalSecurityHelper.HierarchicalSecurityEditCheck(resource, User, (new ApplicationContext(new SystemUser())), User.EditRestrict(resource), obj);
        return result;
    }
    
    /// <summary>Returns list of non-viewable properties.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>List of non-viewable properties.</returns>
    public List<string> CanNotView(string resource)
    {
        List<string> list = new List<string>();
        if(this.IsAdmin) return list;
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return list;
        var canview = CanView(resource);
        if(canview && this.permissions.Any(p => p.EntityName.Equals(resource) && (!string.IsNullOrEmpty(p.NoView))))
        {
            var permission = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && (!string.IsNullOrEmpty(p.NoView))).NoView;
            return permission.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        else if(canview) return list;
        return null;
    }
    /// <summary>Fls applied on properties.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    public string FLSAppliedOnProperties(string resource)
    {
        var permission = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource));
        if(permission != null)
            return permission.NoEdit + permission.NoView;
        return string.Empty;
    }
    
    /// <summary>code for verb action security.</summary>
    ///
    /// <param name="resource">  The resource.</param>
    /// <param name="entityname">The entityname.</param>
    /// <param name="User">      The user.</param>
    ///
    /// <returns>True if we can use verb, false if not.</returns>
    
    public bool CanUseVerb(string resource, string entityname, IUser User)
    {
        if(resource == "Recycle")
        {
            bool SoftDeleteEnabled = GeneratorBase.MVC.Models.CommonFunction.Instance.SoftDeleteEnabled().ToLower() == "true" ? true : false;
            if(!SoftDeleteEnabled)
                return false;
        }
        if(User.IsAdmin || Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.Verbs != null && p.Verbs.Contains(resource) && p.EntityName == entityname);
    }
    
    /// <summary>Determine if we can add admin feature.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can add admin feature, false if not.</returns>
    
    public bool CanAddAdminFeature(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsAdd);
    }
    
    /// <summary>Determine if we can view admin feature.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can view admin feature, false if not.</returns>
    
    public bool CanViewAdminFeature(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsAllow);
    }
    
    /// <summary>Determine if we can edit admin feature.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can edit admin feature, false if not.</returns>
    
    public bool CanEditAdminFeature(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsEdit);
    }
    
    /// <summary>Determine if we can delete admin feature.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can delete admin feature, false if not.</returns>
    
    public bool CanDeleteAdminFeature(string resource)
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsDelete);
    }
    
    /// <summary>Query if this object has admin privileges.</summary>
    ///
    /// <returns>True if admin privileges, false if not.</returns>
    
    public bool HasAdminPrivileges()
    {
        if(string.IsNullOrEmpty(this.Name))
            return false;
        if(this.IsAdmin) return true;
        return this.adminprivileges.Any(p => p.IsAllow);
    }
}
}

