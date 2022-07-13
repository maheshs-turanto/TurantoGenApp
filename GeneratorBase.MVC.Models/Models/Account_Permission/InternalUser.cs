using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>An internal user.</summary>
public class InternalUser : IUser
{
    /// <summary>Gets the name.</summary>
    ///
    /// <value>The name.</value>
    
    public string Name
    {
        get
        {
            return "System";
        }
    }
    
    /// <summary>Gets the name of the java script encoded.</summary>
    ///
    /// <value>The name of the java script encoded.</value>
    
    public string JavaScriptEncodedName
    {
        get
        {
            return "System";
        }
    }
    
    /// <summary>Gets a value indicating whether this object is admin.</summary>
    ///
    /// <value>True if this object is admin, false if not.</value>
    
    public bool IsAdmin
    {
        get
        {
            return true;
        }
    }
    
    /// <summary>Query if this object is admin user.</summary>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    public bool IsAdminUser()
    {
        return true;
    }
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="role">The role.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(string role)
    {
        return true;
    }
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="role">The role.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(string[] role)
    {
        return true;
    }
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="userroles">The userroles.</param>
    /// <param name="roles">    The roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    public bool IsInRole(List<string> userroles, string[] roles)
    {
        return true;
    }
    
    /// <summary>Gets the roles in this collection.</summary>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the roles in this collection.</returns>
    
    public IEnumerable<string> GetRoles()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>Determine if we can add.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can add, false if not.</returns>
    
    public bool CanAdd(string resource)
    {
        return true;
    }
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public bool CanView(string resource)
    {
        return true;
    }
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    public bool CanView(string resource, string property)
    {
        return true;
    }
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    public bool CanEdit(string resource)
    {
        return true;
    }
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    public bool CanEdit(string resource, string property)
    {
        return true;
    }
    
    /// <summary>Determine if we can delete.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can delete, false if not.</returns>
    
    public bool CanDelete(string resource)
    {
        return true;
    }
    
    /// <summary>Impose owner permission.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ImposeOwnerPermission(string resource)
    {
        return false;
    }
    
    /// <summary>Owner association.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    public string OwnerAssociation(string resource)
    {
        return string.Empty;
    }
    public string ViewRestrict(string resource)
    {
        return string.Empty;
    }
    public string EditRestrict(string resource)
    {
        return string.Empty;
    }
    public string DeleteRestrict(string resource)
    {
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
        return true;
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
        return true;
    }
    public bool CanDeleteItemInHierarchy(string resource, object obj, IUser User)
    {
        return true;
    }
    public bool CanEditItemInHierarchy(string resource, object obj, IUser User)
    {
        return true;
    }
    /// <summary>Fls applied on properties.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    public string FLSAppliedOnProperties(string resource)
    {
        return String.Empty;
    }
    /// <summary>Gets the non-viewable properties.</summary>
    ///
    /// <value>The non-viewable properties.</value>
    
    public List<string> CanNotView(string input)
    {
        return new List<string>();
    }
    /// <summary>Gets or sets the businessrules.</summary>
    ///
    /// <value>The businessrules.</value>
    
    public List<BusinessRule> businessrules
    {
        get;
        set;
    }
    
    /// <summary>Gets the permissions.</summary>
    ///
    /// <value>The permissions.</value>
    
    public List<Permission> permissions
    {
        get
        {
            return new List<Permission>();
        }
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
    
    /// <summary>Gets the userroles.</summary>
    ///
    /// <value>The userroles.</value>
    
    public List<string> userroles
    {
        get
        {
            return new List<string>(new string[] { "InternalUser" });
        }
    }
}
}