using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
namespace GeneratorBase.MVC.Models
{
/// <summary>A custom role provider (Role-Based Authentication Control).</summary>
public class CustomRoleProvider : RoleProvider
{
    /// <summary>Adds the specified user names to the specified roles for the configured
    /// applicationName.</summary>
    ///
    /// <param name="usernames">    A string array of user names to be added to the specified roles.</param>
    /// <param name="roleNames">    A string array of the role names to add the specified user names
    ///                             to.</param>
    
    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    { }
    
    /// <summary>Adds a new role to the data source for the configured applicationName.</summary>
    ///
    /// <param name="roleName">The name of the role to create.</param>
    
    public override void CreateRole(string roleName) { }
    
    /// <summary>Removes a role from the data source for the configured applicationName.</summary>
    ///
    /// <param name="roleName">            The name of the role to delete.</param>
    /// <param name="throwOnPopulatedRole"> If true, throw an exception if
    ///                                     <paramref name="roleName" /> has one or more members and
    ///                                     do not delete <paramref name="roleName" />.</param>
    ///
    /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
    
    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
        return true;
    }
    
    /// <summary>Gets an array of user names in a role where the user name contains the specified user
    /// name to match.</summary>
    ///
    /// <param name="roleName">       The role to search in.</param>
    /// <param name="usernameToMatch">The user name to search for.</param>
    ///
    /// <returns>A string array containing the names of all the users where the user name matches
    /// <paramref name="usernameToMatch" /> and the user is a member of the specified role.</returns>
    
    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
        return new string[] { "Admin" };
    }
    //public override string[] GetUsersInRole(string roleName)
    //{
    //    return new string[] { "Admin" };
    //}
    
    /// <summary>Gets a list of users in the specified role for the configured applicationName.</summary>
    ///
    /// <param name="roleId">Identifier for the role.</param>
    ///
    /// <returns>A string array containing the names of all the users who are members of the specified
    /// role for the configured applicationName.</returns>
    
    public override string[] GetUsersInRole(string roleId)
    {
        if(roleId == "")
            return new string[] { "" };
        using(var usersContext = new ApplicationDbContext(true))
        {
            if(roleId == "0")
            {
                var allUsers = usersContext.Users.Select(p => p.Id);
                return allUsers.ToArray();
            }
            var role = usersContext.Roles.FirstOrDefault(r => roleId == r.Id);
            var users = role.Users.Select(p => p.UserId);
            return users.ToArray();
        }
    }
    
    /// <summary>Removes the specified user names from the specified roles for the configured
    /// applicationName.</summary>
    ///
    /// <param name="usernames">    A string array of user names to be removed from the specified
    ///                             roles.</param>
    /// <param name="roleNames">    A string array of role names to remove the specified user names
    ///                             from.</param>
    
    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) { }
    
    /// <summary>Gets a value indicating whether the specified role name already exists in the role
    /// data source for the configured applicationName.</summary>
    ///
    /// <param name="roleName">The name of the role to search for in the data source.</param>
    ///
    /// <returns>true if the role name already exists in the data source for the configured
    /// applicationName; otherwise, false.</returns>
    
    public override bool RoleExists(string roleName)
    {
        return true;
    }
    
    /// <summary>Gets or sets the name of the application to store and retrieve role information for.</summary>
    ///
    /// <value>The name of the application to store and retrieve role information for.</value>
    
    public override string ApplicationName
    {
        get
        {
            return ApplicationName;
        }
        set { }
    }
    
    /// <summary>Gets a value indicating whether the specified user is in the specified role for the
    /// configured applicationName.</summary>
    ///
    /// <param name="username">The user name to search for.</param>
    /// <param name="roleName">The role to search in.</param>
    ///
    /// <returns>true if the specified user is in the specified role for the configured
    /// applicationName; otherwise, false.</returns>
    
    public override bool IsUserInRole(string username, string roleName)
    {
        using(var usersContext = new ApplicationDbContext(true))
        {
            var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
            if(user == null)
                return false;
            if(user.Roles == null)
                return false;
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id));
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"].Value;
            if(!string.IsNullOrEmpty(UserCurrentRole))
            {
                string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                roles = roles.Where(u => splittedarray.Contains(u.Name.Trim()));
            }
            AddDynamicRoles adr = new AddDynamicRoles();
            return roles.Any(r => r.Name == roleName) || adr.AddRolesDynamic(new string[] { }, user.Id).Contains(roleName);
        }
    }
    
    /// <summary>Gets a list of the roles that a specified user is in for the configured
    /// applicationName.</summary>
    ///
    /// <param name="username">The user to return a list of roles for.</param>
    ///
    /// <returns>A string array containing the names of all the roles that the specified user is in
    /// for the configured applicationName.</returns>
    
    public override string[] GetRolesForUser(string username)
    {
        using(var usersContext = new ApplicationDbContext(true))
        {
            var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
            if(user == null)
                return new string[] { };
            if(user.Roles == null)
                return new string[] { };
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();
            AddDynamicRoles adr = new AddDynamicRoles();
            roles = adr.AddRolesDynamic(roles, user.Id);
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"].Value;
            if(!string.IsNullOrEmpty(UserCurrentRole))
            {
                string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                roles = roles.Where(u => splittedarray.Contains(u.Trim())).ToArray();
            }
            return roles.OrderBy(p => p).ToArray();
        }
    }
    
    
    
    public string[] GetAllRolesForUser(string username)
    {
        using(var usersContext = new ApplicationDbContext(true))
        {
            var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
            if(user == null)
                return new string[] { };
            if(user.Roles == null)
                return new string[] { };
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();
            AddDynamicRoles adr = new AddDynamicRoles();
            roles = adr.AddRolesDynamic(roles, user.Id);
            return roles.OrderBy(p => p).ToArray();
        }
    }
    
    
    /// <summary>Gets a list of the roles that a specified user is in for the configured
    /// applicationName.</summary>
    ///
    /// <param name="username">The user to return a list of roles for.</param>
    /// <param name="nofilter">Application filter not apply.</param>
    ///
    /// <returns>A string array containing the names of all the roles that the specified user is in
    /// for the configured applicationName.</returns>
    
    public string[] GetRolesofUser(string username, bool nofilter = false)
    {
        using(var usersContext = new ApplicationDbContext(true))
        {
            var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
            if(user == null)
                return new string[] { };
            if(user.Roles == null)
                return new string[] { };
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();
            AddDynamicRoles adr = new AddDynamicRoles();
            roles = adr.AddRolesDynamic(roles, user.Id);
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl + "CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies[AppUrl + "CurrentRole"].Value;
            if(!string.IsNullOrEmpty(UserCurrentRole) && !nofilter)
            {
                string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                roles = roles.Where(u => splittedarray.Contains(u.Trim())).ToArray();
            }
            return roles.OrderBy(p => p).ToArray();
        }
    }
    // -- Snip --
    
    /// <summary>Gets a list of all the roles for the configured applicationName.</summary>
    ///
    /// <returns>A string array containing the names of all the roles stored in the data source for
    /// the configured applicationName.</returns>
    
    public override string[] GetAllRoles()
    {
        using(var usersContext = new ApplicationDbContext())
        {
            return usersContext.Roles.Select(r => r.Name).ToArray();
        }
    }
    
    /// <summary>Gets all roles report.</summary>
    ///
    /// <returns>all roles report.</returns>
    
    public Dictionary<string, string> GetAllRolesReport()
    {
        Dictionary<string, string> RolesList = new Dictionary<string, string>();
        using(var usersContext = new ApplicationDbContext())
        {
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
            var _rolelist = usersContext.Roles.OrderBy(p=>p.Name).ToList();
            RolesList.Add("All", "All");
            foreach(var item in _rolelist)
            {
                if(item.Name.Trim() == adminString.Trim()) continue;
                RolesList.Add(item.Id, item.Name);
            }
        }
        return RolesList;
    }
    
    /// <summary>Gets all roles notify role.</summary>
    ///
    /// <param name="adminString">The admin string.</param>
    ///
    /// <returns>all roles notify role.</returns>
    
    public Dictionary<string, string> GetAllRolesNotifyRole(string adminString)
    {
        Dictionary<string, string> RolesList = new Dictionary<string, string>();
        RolesList.Add("0", "All");
        using(var usersContext = new ApplicationDbContext())
        {
            var _rolelist = usersContext.Roles.ToList();
            foreach(var item in _rolelist)
            {
                if(item.Name == adminString)
                    continue;
                RolesList.Add(item.Id, item.Name);
            }
        }
        return RolesList;
    }
    
    /// <summary>Gets all roles line break.</summary>
    ///
    /// <returns>all roles line break.</returns>
    
    public Dictionary<string, string> GetAllRolesBR()
    {
        Dictionary<string, string> RolesList = new Dictionary<string, string>();
        RolesList.Add("0", "All");
        using(var usersContext = new ApplicationDbContext())
        {
            var _rolelist = usersContext.Roles.ToList();
            foreach(var item in _rolelist)
            {
                RolesList.Add(item.Id, item.Name);
            }
        }
        return RolesList;
    }
    // -- Snip --
}
}