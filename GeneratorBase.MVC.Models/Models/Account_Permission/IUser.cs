using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>Interface for user.</summary>
public interface IUser
{
    /// <summary>Gets the name.</summary>
    ///
    /// <value>The name.</value>
    
    string Name
    {
        get;
    }
    
    /// <summary>Gets the name of the java script encoded.</summary>
    ///
    /// <value>The name of the java script encoded.</value>
    
    string JavaScriptEncodedName
    {
        get;
    }
    
    /// <summary>Gets a value indicating whether this object is admin.</summary>
    ///
    /// <value>True if this object is admin, false if not.</value>
    
    bool IsAdmin
    {
        get;
    }
    
    /// <summary>Query if this object is admin user.</summary>
    ///
    /// <returns>True if admin user, false if not.</returns>
    
    bool IsAdminUser();
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="role">The role.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    bool IsInRole(string role);
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="roles">The roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    bool IsInRole(string[] roles);
    
    /// <summary>Query if 'userroles' is in role.</summary>
    ///
    /// <param name="userroles">The userroles.</param>
    /// <param name="roles">    The roles.</param>
    ///
    /// <returns>True if in role, false if not.</returns>
    
    bool IsInRole(List<string> userroles, string[] roles);
    
    /// <summary>Gets the roles in this collection.</summary>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the roles in this collection.</returns>
    
    IEnumerable<string> GetRoles();
    
    /// <summary>Determine if we can add.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can add, false if not.</returns>
    
    bool CanAdd(string resource);
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    bool CanView(string resource);
    
    /// <summary>Determine if we can view.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can view, false if not.</returns>
    
    bool CanView(string resource, string property);
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    bool CanEdit(string resource);
    
    /// <summary>Determine if we can edit.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>True if we can edit, false if not.</returns>
    
    bool CanEdit(string resource, string property);
    
    /// <summary>Determine if we can delete.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if we can delete, false if not.</returns>
    
    bool CanDelete(string resource);
    
    /// <summary>Impose owner permission.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    bool ImposeOwnerPermission(string resource);
    
    /// <summary>Owner association.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    string OwnerAssociation(string resource);
    
    /// <summary>View Restrict.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    string ViewRestrict(string resource);
    
    /// <summary>Edit Restrict.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    string EditRestrict(string resource);
    /// <summary>Delete Restrict.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    string DeleteRestrict(string resource);
    /// <summary>Determine if we can edit item.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="obj">     The object.</param>
    /// <param name="user">    The user.</param>
    ///
    /// <returns>True if we can edit item, false if not.</returns>
    
    bool CanEditItem(string resource,Object obj, IUser user);
    
    /// <summary>Determine if we can delete item.</summary>
    ///
    /// <param name="resource">The resource.</param>
    /// <param name="obj">     The object.</param>
    /// <param name="user">    The user.</param>
    ///
    /// <returns>True if we can delete item, false if not.</returns>
    
    bool CanDeleteItem(string resource, Object obj, IUser user);
    bool CanDeleteItemInHierarchy(string resource, Object obj, IUser user);
    bool CanEditItemInHierarchy(string resource, Object obj, IUser user);
    /// <summary>Returns list of non-viewable properties.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>List of non-viewable properties.</returns>
    
    List<string> CanNotView(string resource);
    
    /// <summary>Fls applied on properties.</summary>
    ///
    /// <param name="resource">The resource.</param>
    ///
    /// <returns>A string.</returns>
    
    string FLSAppliedOnProperties(string resource);
    
    /// <summary>Gets the businessrules.</summary>
    ///
    /// <value>The businessrules.</value>
    
    List<BusinessRule> businessrules
    {
        get;
    }
    
    /// <summary>Gets the permissions.</summary>
    ///
    /// <value>The permissions.</value>
    
    List<Permission> permissions
    {
        get;
    }
    
    /// <summary>Gets the multi tenant login selected.</summary>
    ///
    /// <value>The multi tenant login selected.</value>
    
    List<MultiTenantLoginSelected> MultiTenantLoginSelected
    {
        get;
    }
    
    /// <summary>Gets the adminprivileges.</summary>
    ///
    /// <value>The adminprivileges.</value>
    
    List<PermissionAdminPrivilege> adminprivileges
    {
        get;
    }
    
    /// <summary>Gets or sets the extra multitenant priviledges.</summary>
    ///
    /// <value>The extra multitenant priviledges.</value>
    
    List<long> extraMultitenantPriviledges
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the userbasedsecurity.</summary>
    ///
    /// <value>The userbasedsecurity.</value>
    
    List<UserBasedSecurity> userbasedsecurity
    {
        get;
        set;
    }
    
    /// <summary>Gets the userroles.</summary>
    ///
    /// <value>The userroles.</value>
    
    List<string> userroles
    {
        get;
    }
}
}