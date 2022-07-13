using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
/// <summary>A user define pages.</summary>
[Table("tbl_UserDefinePages")]
public class UserDefinePages
{
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the page.</summary>
    ///
    /// <value>The name of the page.</value>
    
    [DisplayName("Page Name")]
    [Required]
    public string PageName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page content.</summary>
    ///
    /// <value>The page content.</value>
    
    [DisplayName("Page Content")]
    [System.Web.Mvc.AllowHtml]
    public string PageContent
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the create user define page.</summary>
public class CreateUserDefinePageViewModel
{
    /// <summary>Default constructor.</summary>
    public CreateUserDefinePageViewModel()
    {
        //this.Roles = new List<SelectUserRoleEditorViewModel>();
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="userpages">The userpages.</param>
    
    public CreateUserDefinePageViewModel(UserDefinePages userpages)
        : this()
    {
        this.PageName = userpages.PageName;
        this.PageContent = userpages.PageContent;
        var Db = new ApplicationDbContext();
        var allRoles = Db.Roles;
        //foreach (var role in allRoles)
        //{
        //    var rvm = new SelectUserRoleEditorViewModel(role);
        //    this.Roles.Add(rvm);
        //}
        var userdefinepagesrole = new UserDefinePagesRoleContext();
        var disableroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId != userpages.Id);
        //foreach (var userRole in disableroles)
        //{
        //    var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
        //    checkUserRole.isdisabled = true;
        //}
        var roleslist = disableroles.Select(a => a.RoleName).ToList();
        this.RoleList = new SelectList(allRoles.Where(r => !roleslist.Any(dr => dr == r.Name)).ToList(), "Name", "Name", null);
    }
    
    /// <summary>Gets or sets the name of the page.</summary>
    ///
    /// <value>The name of the page.</value>
    
    [DisplayName("Page Name")]
    [Required]
    public string PageName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page content.</summary>
    ///
    /// <value>The page content.</value>
    
    public string PageContent
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a list of roles.</summary>
    ///
    /// <value>A list of roles.</value>
    
    public SelectList RoleList
    {
        get;
        set;
    }
    //public List<SelectUserRoleEditorViewModel> Roles { get; set; }
}
/// <summary>A ViewModel for the edit user define page.</summary>
public class EditUserDefinePageViewModel
{
    /// <summary>Default constructor.</summary>
    public EditUserDefinePageViewModel()
    {
        this.Roles = new List<SelectUserRoleEditorViewModel>();
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="Id">The identifier.</param>
    
    public EditUserDefinePageViewModel(Int64 Id)
        : this()
    {
        var Db = new ApplicationDbContext();
        var allRoles = Db.Roles;
        foreach(var role in allRoles)
        {
            var rvm = new SelectUserRoleEditorViewModel(role);
            this.Roles.Add(rvm);
        }
        if(Id == 0)
            return;
        var db = new UserDefinePagesContext();
        var userpages = db.UserDefinePagess.Where(u => u.Id == Id).ToList()[0];
        this.Id = userpages.Id;
        this.PageName = userpages.PageName;
        this.PageContent = userpages.PageContent;
        var userdefinepagesrole = new UserDefinePagesRoleContext();
        var pagesroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId == userpages.Id);
        var disableroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId != userpages.Id);
        foreach(var userRole in pagesroles)
        {
            var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
            if(checkUserRole != null)
                checkUserRole.Selected = true;
        }
        foreach(var userRole in disableroles)
        {
            var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
            if(checkUserRole != null)
                checkUserRole.isdisabled = true;
        }
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    public Int64 Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the page.</summary>
    ///
    /// <value>The name of the page.</value>
    
    [DisplayName("Page Name")]
    [Required]
    public string PageName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page content.</summary>
    ///
    /// <value>The page content.</value>
    
    public string PageContent
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The roles.</value>
    
    public List<SelectUserRoleEditorViewModel> Roles
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the select user role editor.</summary>
public class SelectUserRoleEditorViewModel
{
    /// <summary>Default constructor.</summary>
    public SelectUserRoleEditorViewModel() { }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="role">The role.</param>
    
    public SelectUserRoleEditorViewModel(IdentityRole role)
    {
        this.Id = role.Id;
        this.RoleName = role.Name;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    public string Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the selected.</summary>
    ///
    /// <value>True if selected, false if not.</value>
    
    public bool Selected
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the role.</summary>
    ///
    /// <value>The name of the role.</value>
    
    public string RoleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the isdisabled.</summary>
    ///
    /// <value>True if isdisabled, false if not.</value>
    
    public bool isdisabled
    {
        get;
        set;
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}

