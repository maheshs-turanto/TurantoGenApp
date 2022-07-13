using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
/// <summary>A ViewModel for the manage user.</summary>
public class ManageUserViewModel
{
    /// <summary>Gets or sets the current password.</summary>
    ///
    /// <value>The old password.</value>
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    [System.Web.Mvc.Remote("CheckPasswordLengthOldPassword", "Account", ErrorMessage = "Invalid password.")]
    public string OldPassword
    {
        get;
        set;
    }
    [Required]
    //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    
    /// <summary>Gets or sets the new password.</summary>
    ///
    /// <value>The new password.</value>
    
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    [System.Web.Mvc.Remote("CheckPasswordLengthNewPassword", "Account", ErrorMessage = "Invalid password.")]
    public string NewPassword
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the confirm password.</summary>
    ///
    /// <value>The confirm password.</value>
    
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage =
                 "The new password and confirmation password do not match.")]
    public string ConfirmPassword
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the token.</summary>
    ///
    /// <value>The token.</value>
    
    public string token
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the provider.</summary>
    ///
    /// <value>The provider.</value>
    
    public string provider
    {
        get;
        set;
    }
}
//Two factor change
/// <summary>A ViewModel for the resend confirmed for user.</summary>
public class ResendViewModel
{
    public string UserId
    {
        get;
        set;
    }
    public string ReturnUrl
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the send code for user.</summary>
public class SendCodeViewModel
{
    public string UserId
    {
        get;
        set;
    }
    public string SelectedProvider
    {
        get;
        set;
    }
    public ICollection<System.Web.Mvc.SelectListItem> Providers
    {
        get;
        set;
    }
    public string ReturnUrl
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the verify code for user.</summary>
public class VerifyCodeViewModel
{
    public string UserId
    {
        get;
        set;
    }
    [Required]
    public string Provider
    {
        get;
        set;
    }
    
    [Required]
    [Display(Name = "Verification Code")]
    [StringLength(6, ErrorMessage = "Verification Code cannot be longer than 6 characters.")]
    public string Code
    {
        get;
        set;
    }
    public string ReturnUrl
    {
        get;
        set;
    }
    
    [Display(Name = "Remember this browser?")]
    public bool RememberBrowser
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the forgot password.</summary>
public class ForgotPasswordViewModel
{
    /// <summary>Gets or sets the username.</summary>
    ///
    /// <value>The username.</value>
    
    [Display(Name = "User name")]
    [StringLength(65, ErrorMessage = "Less than 65 characters")]
    [RegularExpression(@"^[ A-Za-z0-9_@.#&+-?=]*$", ErrorMessage = "No special characters other than period (.),plus(+),equal to(=) question mark(?),dash(-),underscore (_) and at sign (@) are allowed in User Name. No spaces.")]
    public string Username
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
    public string Email
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the login.</summary>
public class LoginViewModel
{
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    [Required]
    [Display(Name = "User name")]
    [StringLength(65, ErrorMessage = "Less than 65 characters")]
    [RegularExpression(@"^[ A-Za-z0-9_@.#&+-?=]*$", ErrorMessage = "No special characters other than period (.),plus(+),equal to(=) question mark(?),dash(-),underscore (_) and at sign (@) are allowed in User Name. No spaces.")]
    public string UserName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the password.</summary>
    ///
    /// <value>The password.</value>
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [System.Web.Mvc.Remote("CheckPasswordLength", "Account", ErrorMessage = "Invalid password.")]
    public string Password
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the remember me.</summary>
    ///
    /// <value>True if remember me, false if not.</value>
    
    [Display(Name = "Remember me?")]
    public bool RememberMe
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the create role.</summary>
public class CreateRoleViewModel
{
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [Required]
    [Display(Name = "Role name")]
    [StringLength(256, ErrorMessage = "Name cannot be longer than 256 characters.")]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the role.</summary>
    ///
    /// <value>The type of the role.</value>
    [StringLength(256, ErrorMessage = "RoleType cannot be longer than 256 characters.")]
    public string RoleType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the tenant.</summary>
    ///
    /// <value>The identifier of the tenant.</value>
    
    public long? TenantId
    {
        get;
        set;
    }
    
    /// <summary>Return a pre-poulated instance of AppliationUser:</summary>
    ///
    /// <returns>The role.</returns>
    
    public ApplicationRole GetRole()
    {
        var role = new ApplicationRole()
        {
            Name = this.Name,
            Description = this.Description,
            RoleType = this.RoleType,
            TenantId = this.TenantId,
        };
        return role;
    }
}
/// <summary>A ViewModel for the register.</summary>
public class RegisterViewModel : ApplicationUserExtra
{
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    [Required]
    [Display(Name = "User name")]
    [StringLength(65, ErrorMessage = "Less than 65 characters")]
    [RegularExpression(@"^[ A-Za-z0-9_@.#&+-?=]*$", ErrorMessage = "No special characters other than period (.),plus(+),equal to(=) question mark(?),dash(-),underscore (_) and at sign (@) are allowed in User Name. No spaces.")]
    public string UserName
    {
        get;
        set;
    }
    [RequiredIf("NotifyForEmail", false, "The Password field is required.")]
    //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    
    /// <summary>Gets or sets the password.</summary>
    ///
    /// <value>The password.</value>
    
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [System.Web.Mvc.Remote("CheckPasswordLength", "Account", ErrorMessage = "Invalid password.")]
    public string Password
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the confirm password.</summary>
    ///
    /// <value>The confirm password.</value>
    
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage =
                 "The password and confirmation password do not match.")]
    public string ConfirmPassword
    {
        get;
        set;
    }
    
    /// <summary>New Fields added to extend Application User class:</summary>
    ///
    /// <value>The name of the first.</value>
    
    [Required]
    [Display(Name = "First Name")]
    [StringLength(256, ErrorMessage = "First Name cannot be longer than 256 characters.")]
    public string FirstName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's last name.</summary>
    ///
    /// <value>The name of the last.</value>
    
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(256, ErrorMessage = "Last Name cannot be longer than 256 characters.")]
    public string LastName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [Required]
    [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
    public string Email
    {
        get;
        set;
    }
    //Two factor change
    /// <summary>Gets or sets the phone number.</summary>
    ///
    /// <value>The Phone Number.</value>
    
    
    [RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid Phone Number")]
    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Phone Number Confirmed.</summary>
    ///
    /// <value>Boolean value to Phone Number Confirmed.</value>
    public Boolean PhoneNumberConfirmed
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Email Confirmed.</summary>
    ///
    /// <value>Boolean value to Email Confirmed.</value>
    public Boolean EmailConfirmed
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Notify For Email Activation.</summary>
    ///
    /// <value>Boolean value to Send Email Notification to Create Password.</value>
    
    [Display(Name = "Send Email Notification to Create Password")]
    public Boolean NotifyForEmail
    {
        get;
        set;
    }
    
    /// <summary>Return a pre-poulated instance of AppliationUser:</summary>
    ///
    /// <returns>The user.</returns>
    
    public ApplicationUser GetUser()
    {
        var user = new ApplicationUser()
        {
            UserName = this.UserName,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            NotifyForEmail = this.NotifyForEmail,
            //Two factor change
            PhoneNumber = this.PhoneNumber,
            PhoneNumberConfirmed = true,
        };
        return user;
    }
}
/// <summary>A ViewModel for the edit role.</summary>
public class EditRoleViewModel
{
    /// <summary>Default constructor.</summary>
    public EditRoleViewModel() { }
    
    /// <summary>Allow Initialization with an instance of ApplicationUser:</summary>
    ///
    /// <param name="Role">The role.</param>
    
    public EditRoleViewModel(ApplicationRole Role)
    {
        this.Name = Role.Name;
        this.OriginalName = Role.Name;
        this.id = Role.Id;
        this.Description = Role.Description;
        this.RoleType = Role.RoleType;
        this.TenantId = Role.TenantId;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Required]
    [Display(Name = "Role Name")]
    public string id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    [StringLength(256, ErrorMessage = "Name cannot be longer than 256 characters.")]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the original.</summary>
    ///
    /// <value>The name of the original.</value>
    
    public string OriginalName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the role.</summary>
    ///
    /// <value>The type of the role.</value>
    [StringLength(256, ErrorMessage = "Role Type cannot be longer than 256 characters.")]
    public string RoleType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the tenant.</summary>
    ///
    /// <value>The identifier of the tenant.</value>
    
    public long? TenantId
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the edit user.</summary>
public class EditUserViewModel : ApplicationUserExtra
{
    /// <summary>Default constructor.</summary>
    public EditUserViewModel() { }
    
    /// <summary>Allow Initialization with an instance of ApplicationUser:</summary>
    ///
    /// <param name="user">The user.</param>
    
    public EditUserViewModel(ApplicationUser user)
    {
        this.UserName = user.UserName;
        this.FirstName = user.FirstName;
        this.LastName = user.LastName;
        this.Email = user.Email;
        this.NotifyForEmail = user.NotifyForEmail.HasValue ? user.NotifyForEmail.Value : false;
        this.Id = user.Id;
        this.LockoutEndDateUtc = user.LockoutEndDateUtc;
        //Two factor change
        this.PhoneNumber = user.PhoneNumber;
        this.PhoneNumberConfirmed = true;
        this.EmailConfirmed = user.EmailConfirmed;
    }
    
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    [Required]
    [Display(Name = "User Name")]
    [StringLength(65, ErrorMessage = "Less than 65 characters")]
    [RegularExpression(@"^[ A-Za-z0-9_@.#&+-?=]*$", ErrorMessage = "No special characters other than period (.),plus(+),equal to(=) question mark(?),dash(-),underscore (_) and at sign (@) are allowed in User Name. No spaces.")]
    public string UserName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's first name.</summary>
    ///
    /// <value>The name of the first.</value>
    
    [Required]
    [Display(Name = "First Name")]
    [StringLength(256, ErrorMessage = "First Name cannot be longer than 256 characters.")]
    public string FirstName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's last name.</summary>
    ///
    /// <value>The name of the last.</value>
    
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(256, ErrorMessage = "Last Name cannot be longer than 256 characters.")]
    public string LastName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [Required]
    [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
    public string Email
    {
        get;
        set;
    }
    
    //Two factor change
    /// <summary>Gets or sets the phone number.</summary>
    ///
    /// <value>The Phone Number.</value>
    [RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid Phone Number")]
    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Phone Number Confirmed.</summary>
    ///
    /// <value>Boolean value to Phone Number Confirmed.</value>
    public Boolean PhoneNumberConfirmed
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Email Confirmed.</summary>
    ///
    /// <value>Boolean value to Email Confirmed.</value>
    public Boolean EmailConfirmed
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Notify For Email Activation.</summary>
    ///
    /// <value>Boolean value to Send Email Notification to Create Password.</value>
    
    [Display(Name = "Send Email Notification to Create Password")]
    public Boolean NotifyForEmail
    {
        get;
        set;
    }
    
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    public string Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Date/Time of the lockout end date UTC.</summary>
    ///
    /// <value>The lockout end date UTC.</value>
    
    public DateTime? LockoutEndDateUtc
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the select user roles.</summary>
public class SelectUserRolesViewModel
{
    /// <summary>Default constructor.</summary>
    public SelectUserRolesViewModel()
    {
        this.Roles = new List<SelectRoleEditorViewModel>();
    }
    
    /// <summary>Enable initialization with an instance of ApplicationUser:</summary>
    ///
    /// <param name="user">        The user.</param>
    /// <param name="loggedinuser">The loggedinuser.</param>
    
    public SelectUserRolesViewModel(ApplicationUser user, IUser loggedinuser)
        : this()
    {
        this.UserName = user.UserName;
        this.FirstName = user.FirstName;
        this.LastName = user.LastName;
        this.Id = user.Id;
        using(var Db = new ApplicationDbContext(loggedinuser))
        {
            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach(var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }
            // Set the Selected property to true for those roles for
            // which the current user is a member:
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = Db.Roles.Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name);
            foreach(var role in roles)
            {
                var checkUserRole =
                    this.Roles.Find(r => r.RoleName == role);
                checkUserRole.Selected = true;
            }
        }
    }
    
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    public string UserName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's first name.</summary>
    ///
    /// <value>The name of the first.</value>
    
    public string FirstName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's last name.</summary>
    ///
    /// <value>The name of the last.</value>
    
    public string LastName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    public string Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The roles.</value>
    
    public List<SelectRoleEditorViewModel> Roles
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the select users in role.</summary>
public class SelectUsersInRoleViewModel
{
    /// <summary>Default constructor.</summary>
    public SelectUsersInRoleViewModel()
    {
        this.Users = new List<SelectUserEditorViewModel>();
    }
    
    /// <summary>Enable initialization with an instance of ApplicationUser:</summary>
    ///
    /// <param name="loggedinuser">The loggedinuser.</param>
    /// <param name="role">        The role.</param>
    /// <param name="searchkey">   The searchkey.</param>
    
    public SelectUsersInRoleViewModel(IUser loggedinuser, ApplicationRole role, string searchkey)
        : this()
    {
        this.RoleName = role.Name;
        using(var Db = new ApplicationDbContext(loggedinuser))
        {
            // Add all available users to the list of EditorViewModels:
            this.UserCount = Db.Users.Count();
            var allUsers = Db.Users.OrderBy(p => p.UserName).Take(50).ToList();//search
            if(!string.IsNullOrEmpty(searchkey))
            {
                this.UserCount = Db.Users.Where(p => p.UserName.ToUpper().Contains(searchkey.ToUpper()) || p.FirstName.ToUpper().Contains(searchkey.ToUpper()) || p.LastName.ToUpper().Contains(searchkey.ToUpper())).Count();
                allUsers = Db.Users.Where(p => p.UserName.ToUpper().Contains(searchkey.ToUpper()) || p.FirstName.ToUpper().Contains(searchkey.ToUpper()) || p.LastName.ToUpper().Contains(searchkey.ToUpper())).OrderBy(p => p.UserName).Take(50).ToList();
            }
            foreach(var user in allUsers)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectUserEditorViewModel(user);
                this.Users.Add(rvm);
            }
            // Set the Selected property to true for those roles for
            // which the current user is a member:
            foreach(var user in allUsers)
            {
                var checkUsersInRole = this.Users.Find(r => r.UserName == user.UserName);
                var roleIds = user.Roles.Select(r => r.RoleId);
                checkUsersInRole.Selected = Db.Roles.Where(r => roleIds.Contains(r.Id))
                                            .Any(r => r.Name == RoleName);
            }
        }
    }
    
    /// <summary>Enable initialization with an instance of ApplicationUser:</summary>
    ///
    /// <param name="loggedinuser">The loggedinuser.</param>
    /// <param name="role">        The role.</param>
    
    public SelectUsersInRoleViewModel(IUser loggedinuser, ApplicationRole role)
        : this()
    {
        this.RoleName = role.Name;
        using(var Db = new ApplicationDbContext(loggedinuser))
        {
            // Add all available users to the list of EditorViewModels:
            var allUsers = Db.Users.ToList();
            foreach(var user in allUsers)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectUserEditorViewModel(user);
                this.Users.Add(rvm);
            }
            // Set the Selected property to true for those roles for
            // which the current user is a member:
            foreach(var user in allUsers)
            {
                var checkUsersInRole = this.Users.Find(r => r.UserName == user.UserName);
                var roleIds = user.Roles.Select(r => r.RoleId);
                checkUsersInRole.Selected = Db.Roles.Where(r => roleIds.Contains(r.Id))
                                            .Any(r => r.Name == RoleName);
            }
        }
    }
    
    /// <summary>Gets or sets the name of the role.</summary>
    ///
    /// <value>The name of the role.</value>
    
    public string RoleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the number of users.</summary>
    ///
    /// <value>The number of users.</value>
    
    public int UserCount
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the users.</summary>
    ///
    /// <value>The users.</value>
    
    public List<SelectUserEditorViewModel> Users
    {
        get;
        set;
    }
}
/// <summary>Used to display a single role with a checkbox, within a list structure:</summary>
public class SelectRoleEditorViewModel
{
    /// <summary>Default constructor.</summary>
    public SelectRoleEditorViewModel() { }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="role">The role.</param>
    
    public SelectRoleEditorViewModel(ApplicationRole role)
    {
        this.RoleName = role.Name;
        this.DisplayValue = role.DisplayValue;
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
    
    [Required]
    public string RoleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    public string DisplayValue
    {
        get;
        set;
    }
}
/// <summary>Used to display a single user with a checkbox, within a list structure:</summary>
public class SelectUserEditorViewModel
{
    /// <summary>Default constructor.</summary>
    public SelectUserEditorViewModel() { }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">The user.</param>
    
    public SelectUserEditorViewModel(IdentityUser user)
    {
        this.UserName = user.UserName;
    }
    
    /// <summary>Gets or sets a value indicating whether the selected.</summary>
    ///
    /// <value>True if selected, false if not.</value>
    
    public bool Selected
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the user.</summary>
    ///
    /// <value>The name of the user.</value>
    
    [Required]
    public string UserName
    {
        get;
        set;
    }
}
/// <summary>third party.</summary>
public class ExternalLoginListViewModel
{
    /// <summary>Gets or sets the action.</summary>
    ///
    /// <value>The action.</value>
    
    public string Action
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets URL of the return.</summary>
    ///
    /// <value>The return URL.</value>
    
    public string ReturnUrl
    {
        get;
        set;
    }
}
/// <summary>A ViewModel for the external login confirmation.</summary>
public class ExternalLoginConfirmationViewModel
{
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email
    {
        get;
        set;
    }
}
//

/// <summary>Conditional Required Field.</summary>
public class RequiredIfAttribute : ValidationAttribute
{
    private String PropertyName
    {
        get;
        set;
    }
    private String ErrorMessage
    {
        get;
        set;
    }
    private Object PropertyValue
    {
        get;
        set;
    }
    
    public RequiredIfAttribute(String propertyName, Object propertyvalue, String errormessage)
    {
        this.PropertyName = propertyName;
        this.PropertyValue = propertyvalue;
        this.ErrorMessage = errormessage;
    }
    
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        Object instance = context.ObjectInstance;
        Type type = instance.GetType();
        Object objproprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
        if(objproprtyvalue.ToString() == PropertyValue.ToString() && value == null)
        {
            return new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }
}
}
