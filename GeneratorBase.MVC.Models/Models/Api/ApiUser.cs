using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>An API user.</summary>
public class ApiUser : IUser
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="username">The username.</param>
    
    public ApiUser(string username)
    {
        this.Name = username;
        this.JavaScriptEncodedName = username;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the java script encoded.</summary>
    ///
    /// <value>The name of the java script encoded.</value>
    
    public string JavaScriptEncodedName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the multi tenant.</summary>
    ///
    /// <value>The multi tenant.</value>
    
    public string MultiTenant
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is admin.</summary>
    ///
    /// <value>True if this object is admin, false if not.</value>
    
    public bool IsAdmin
    {
        get;
        set;
    }
    
    /// <summary>Query if this object is admin user.</summary>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    public bool IsAdminUser()
    {
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        CustomRoleProvider RoleProvider = new CustomRoleProvider();
        return RoleProvider.IsUserInRole(this.Name, adminString);
    }
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="role">The role.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(string role)
    {
        /// This is a magic string. We should call IsAdmin instead of IsInRole("Admin")
        if("Admin".Equals(role, System.StringComparison.Ordinal))
        {
            return IsAdminUser();
        }
        return this.GetRoles().Contains(role);
    }
    
    /// <summary>Query if 'userroles' is in role.</summary>
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
    
    /// <summary>Gets the non-viewable properties.</summary>
    ///
    /// <value>The non-viewable properties.</value>
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
    /// <summary>Query if 'userroles' is in role.</summary>
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
    
    /// <summary>Gets the roles in this collection.</summary>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the roles in this collection.</returns>
    
    public IEnumerable<string> GetRoles()
    {
        CustomRoleProvider RoleProvider = new CustomRoleProvider();
        return RoleProvider.GetRolesForUser(this.Name);
    }
    
    /// <summary>Determine if we can add.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can add, false if not.</returns>
    
    public bool CanAdd(string resource)
    {
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
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanView);
    }
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public bool CanView(string resource, string property)
    {
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoView == null || !p.NoView.Contains(property + ",")));
    }
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    public bool CanEdit(string resource)
    {
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
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        return this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoEdit == null || !p.NoEdit.Contains(property + ",")));
    }
    
    /// <summary>Determine if we can delete.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can delete, false if not.</returns>
    
    public bool CanDelete(string resource)
    {
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
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
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
            return this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && p.IsOwner.Value && !string.IsNullOrEmpty(p.UserAssociation)).UserAssociation;
        }
        return string.Empty;
    }
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
        return CanDelete(resource) && (!ImposeOwnerPermission(resource) || (ImposeOwnerPermission(resource) && (!(new CheckPermissionForOwner()).CheckOwnerPermission(resource, obj, User, false)))) && !((new CheckPermissionForOwner()).CheckLockCondition(resource, obj, User, false));
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
        if(Enum.IsDefined(typeof(PermissionFreeContext), resource))
            return true;
        if(User.IsAdminUser())
            return true;
        return this.permissions.Any(p => p.Verbs != null && p.Verbs.Contains(resource) && p.EntityName == entityname);
    }
    
    /// <summary>Gets or sets the businessrules.</summary>
    ///
    /// <value>The businessrules.</value>
    
    public List<BusinessRule> businessrules
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the permissions.</summary>
    ///
    /// <value>The permissions.</value>
    
    public List<Permission> permissions
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
    
    /// <summary>Gets or sets the adminprivileges.</summary>
    ///
    /// <value>The adminprivileges.</value>
    
    public List<PermissionAdminPrivilege> adminprivileges
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
    
    /// <summary>Gets or sets the userroles.</summary>
    ///
    /// <value>The userroles.</value>
    
    public List<string> userroles
    {
        get;
        set;
    }
}
}