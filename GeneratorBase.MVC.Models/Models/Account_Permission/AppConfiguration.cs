using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
/// <summary>The business rule helper.</summary>
public class AppRoles
{
    public ApplicationRole role
    {
        get;
        set;
    }
    public List<Permission> permissions
    {
        get;
        set;
    }
    public List<PermissionAdminPrivilege> adminPrivileges
    {
        get;
        set;
    }
    public bool isselected
    {
        get;
        set;
    }
    public string type
    {
        get;
        set;
    }
}
public class AppConfiguration
{
    public List<AppRoles> rolespermission
    {
        get;
        set;
    }
    public List<AppSetting> appSettings
    {
        get;
        set;
    }
    public List<CompanyInformation> companyProfiles
    {
        get;
        set;
    }
    public List<UserBasedSecurity> userBasedSecurities
    {
        get;
        set;
    }
    public bool isroleselected
    {
        get;
        set;
    }
    public bool isappsettingselected
    {
        get;
        set;
    }
    public bool iscompanyprofileselected
    {
        get;
        set;
    }
    public bool isuserbasedsecurityselected
    {
        get;
        set;
    }
    
    
    //            User, Role, Permission, FLS, Admin Privileges
    //App setting
    //Company Profile
    //Label settings
    //SMTP Settings
    //Business rules(already available) but need to merge into
    //User Based Security
    //Property validation and format
    //Set Default pages
    //Data Metric with Faceted Search
}
}


