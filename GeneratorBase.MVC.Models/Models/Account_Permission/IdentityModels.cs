using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Security;
using System.Data.Entity;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace GeneratorBase.MVC.Models
{
/// <summary>An application user derived from standard dot net identity user for authorization.</summary>
[CustomDisplayName("ApplicationUser", "User.resx", "ApplicationUser")]
public class ApplicationUser : IdentityUser
{
    /// <summary>Gets or sets the person's first name.</summary>
    ///
    /// <value>The name of the first.</value>
    
    [Required]
    [CustomDisplayName("FirstName", "User.resx", "First Name")]
    public string FirstName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the person's last name.</summary>
    ///
    /// <value>The name of the last.</value>
    
    [Required]
    [CustomDisplayName("LastName", "User.resx", "Last Name")]
    public string LastName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Notify For Email Activation.</summary>
    ///
    /// <value>Boolean value to send email for Email Activation.</value>
    
    [Display(Name = "Notify For Email Activation")]
    public System.Boolean? NotifyForEmail
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string DisplayValue
    {
        get
        {
            return this.UserName + "-" + this.FirstName + "-" + this.LastName;
        }
        set { }
    }
}


public class ApplicationUserExtra
{
}


/// <summary>An application role derived from standard dot net identity role for authorization.</summary>
public class ApplicationRole : IdentityRole
{
    /// <summary>Default constructor.</summary>
    public ApplicationRole() : base() { }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="name">The name.</param>
    
    public ApplicationRole(string name)
        : base(name)
    {
        this.RoleType = "Global";
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    
    public ApplicationRole(string name, string description)
        : base(name)
    {
        this.Description = description;
        this.RoleType = "Global";
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="roletype">   The type of the role.</param>
    
    public ApplicationRole(string name, string description,string roletype)
        : base(name)
    {
        this.Description = description;
        this.RoleType = roletype;
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="roletype">   The type of the role.</param>
    /// <param name="tenantid">   The identifier of the tenant.</param>
    
    public ApplicationRole(string name, string description, string roletype,long? tenantid)
    : base(name)
    {
        this.Description = description;
        this.RoleType = roletype;
        this.TenantId = tenantid;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    public virtual string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the role.</summary>
    ///
    /// <value>The type of the role.</value>
    
    public virtual string RoleType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the tenant.</summary>
    ///
    /// <value>The identifier of the tenant.</value>
    
    public virtual long? TenantId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string DisplayValue
    {
        get
        {
            return this.Name + (string.IsNullOrEmpty(this.Description) ? "" : " | " + this.Description) + (string.IsNullOrEmpty(this.RoleType) ? "": " | " + this.RoleType);
        }
        set { }
    }
}
/// <summary>An authentication database context.</summary>
public class AuthenticationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>Default constructor.</summary>
    public AuthenticationDbContext()
        : base("AuthenticationConnection", throwIfV1Schema: false)
    {
    }
    /// <summary>Maps table names, and sets up relationships between the various user entities.</summary>
    ///
    /// <param name="modelBuilder">.</param>
    //       protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //     var user = modelBuilder.Entity<IdentityUser>()
    // .ToTable("AspNetUsers");
    //    user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
    //    user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
    //    user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
    //    user.Property(u => u.UserName).IsRequired();
    //    modelBuilder.Entity<IdentityUserRole>()
    //        .HasKey(r => new { r.UserId, r.RoleId })
    //        .ToTable("AspNetUserRoles");
    //    modelBuilder.Entity<IdentityUserLogin>()
    //        .HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey })
    //        .ToTable("AspNetUserLogins");
    //    modelBuilder.Entity<IdentityUserClaim>()
    //        .ToTable("AspNetUserClaims");
    //    var role = modelBuilder.Entity<ApplicationRole>()
    //        .ToTable("AspNetRoles");
    //    role.Property(r => r.Name).IsRequired();
    //    role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
    //}
}
/// <summary>An application database context.</summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>The user.</summary>
    IUser user;
    /// <summary>Default constructor.</summary>
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
        this.ApplyFilters(new List<IFilter<ApplicationDbContext>>()
        {
            new UserFilter(user)
        });
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="nofilter">True to nofilter.</param>
    public ApplicationDbContext(bool nofilter)
        : base("DefaultConnection", throwIfV1Schema: false)
    {
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">The user.</param>
    public ApplicationDbContext(IUser user)
        : base("DefaultConnection", throwIfV1Schema: false)
    {
        this.user = user;
        this.ApplyFilters(new List<IFilter<ApplicationDbContext>>()
        {
            new UserFilter(user)
        });
    }
    /// <summary>Gets or sets the login attempts.</summary>
    ///
    /// <value>The login attempts.</value>
    
    public IDbSet<LoginAttempts> LoginAttempts
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the password historys.</summary>
    ///
    /// <value>The password historys.</value>
    
    public IDbSet<PasswordHistory> PasswordHistorys
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the multi tenant extra access.</summary>
    ///
    /// <value>The multi tenant extra access.</value>
    
    public IDbSet<MultiTenantExtraAccess> MultiTenantExtraAccess
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the multi tenant login selected.</summary>
    ///
    /// <value>The multi tenant login selected.</value>
    
    public IDbSet<MultiTenantLoginSelected> MultiTenantLoginSelected
    {
        get;
        set;
    }
    /// <summary>Gets or sets the roles select on login.</summary>
    ///
    /// <value>The login selected roles.</value>
    
    public IDbSet<LoginSelectedRoles> LoginSelectedRoles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The roles.</value>
    
    public IDbSet<ApplicationRole> Roles
    {
        get;
        set;
    }
    
    /// <summary>Asynchronously saves all changes made in this context to the underlying database.</summary>
    ///
    /// <remarks>Multiple active operations on the same context instance are not supported.  Use
    /// 'await' to ensure that any asynchronous operations have completed before calling another
    /// method on this context.</remarks>
    ///
    /// <returns>1 if succeded else 0</returns>
    public override async Task<int> SaveChangesAsync()
    {
        var test  = System.Threading.SynchronizationContext.Current;
        var result = 0;
        var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                      e.State.HasFlag(EntityState.Modified) ||
                      e.State.HasFlag(EntityState.Deleted));
        var originalStates = new Dictionary<DbEntityEntry, EntityState>();
        foreach(var entry in entries)
        {
            try
            {
                if(entry.State == EntityState.Modified)
                    DoAuditEntry.MakeUpdateJournalEntry(user,entry);
                if(entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                    DoAuditEntry.MakeAddJournalEntry(user,this,entry);
                CacheHelper.RemoveCache(System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
            }
            catch
            {
                continue;
            }
        }
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
        CacheHelper.RemoveCache("ApplicationUser");
        CacheHelper.RemoveCache("IdentityUser");
        CacheHelper.RemoveCache("IdentityUserRole");
        CacheHelper.RemoveCache("ApplicationRole");
        CacheHelper.RemoveCache("JournalEntry");
        result = await base.SaveChangesAsync();
        return result;
    }
    /// <summary>Saves all changes made in this context to the underlying database.</summary>
    ///
    /// <returns>The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property included
    /// in the entity class (often referred to as independent associations).</returns>
    public override int SaveChanges()
    {
        var result = 0;
        var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                      e.State.HasFlag(EntityState.Modified) ||
                      e.State.HasFlag(EntityState.Deleted));
        var originalStates = new Dictionary<DbEntityEntry, EntityState>();
        foreach(var entry in entries)
        {
            if(entry.Entity is GeneratorBase.MVC.Models.LoginAttempts || entry.Entity is GeneratorBase.MVC.Models.LoginSelectedRoles) continue;
            if(entry.State == EntityState.Modified)
                DoAuditEntry.MakeUpdateJournalEntry(user, entry);
            if(entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                DoAuditEntry.MakeAddJournalEntry(user, this, entry);
            CacheHelper.RemoveCache(System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
        }
        CacheHelper.RemoveCache("MultiTenantLoginSelected");
        CacheHelper.RemoveCache("LoginSelectedRoles");
        CacheHelper.RemoveCache("ApplicationUser");
        CacheHelper.RemoveCache("IdentityUser");
        CacheHelper.RemoveCache("IdentityUserRole");
        CacheHelper.RemoveCache("ApplicationRole");
        CacheHelper.RemoveCache("JournalEntry");
        result = base.SaveChanges();
        return result;
    }
    /// <summary>Maps table names, and sets up relationships between the various user entities.</summary>
    ///
    /// <param name="modelBuilder">.</param>
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<JournalEntry>().HasKey(au => au.Id).ToTable("tbl_JournalEntry");
        var user = modelBuilder.Entity<IdentityUser>()
                   .ToTable("AspNetUsers");
        user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
        user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
        user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
        user.Property(u => u.UserName).IsRequired();
        modelBuilder.Entity<IdentityUserRole>()
        .HasKey(r => new
        {
            r.UserId, r.RoleId
        })
        .ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserLogin>()
        .HasKey(l => new
        {
            l.UserId, l.LoginProvider, l.ProviderKey
        })
        .ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityUserClaim>()
        .ToTable("AspNetUserClaims");
        var role = modelBuilder.Entity<ApplicationRole>()
                   .ToTable("AspNetRoles");
        role.Property(r => r.Name).IsRequired();
        role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
    }
    /// <summary>Applies the security filter (basic, tenant and user-based security filters).</summary>
    ///
    /// <param name="filters">The filters.</param>
    public void ApplyFilters(IList<IFilter<ApplicationDbContext>> filters)
    {
        foreach(var filter in filters)
        {
            filter.DbContext = this;
            filter.ApplyMainSecurity();
            filter.ApplyHierarchicalSecurity();
            filter.ApplyUserBasedSecurity();
            filter.CustomFilter();
        }
    }
    /// <summary>A user context filter.</summary>
    public class UserFilter : IFilter<ApplicationDbContext>
    {
        /// <summary>Gets or sets a context for the database.</summary>
        ///
        /// <value>The database context.</value>
        public ApplicationDbContext DbContext
        {
            get;
            set;
        }
        /// <summary>The user.</summary>
        IUser user;
        /// <summary>Constructor.</summary>
        ///
        /// <param name="user">The user.</param>
        public UserFilter(IUser user)
        {
            this.user = user;
        }
        /// <summary>Applies the basic security.</summary>
        public void ApplyBasicSecurity()
        {
        }
        /// <summary>Applies the HierarchicalSecurity.</summary>
        public void ApplyHierarchicalSecurity()
        {
        }
        /// <summary>Applies the tenant based security.</summary>
        public void ApplyMainSecurity()
        {
        }
        /// <summary>Applies the user based security.</summary>
        public void ApplyUserBasedSecurity()
        {
            if(HttpContext.Current == null || HttpContext.Current.User == null)
                return;
            if((((CustomPrincipal)HttpContext.Current.User).IsAdmin || string.IsNullOrEmpty(((CustomPrincipal)HttpContext.Current.User).Identity.Name)))
                return;
            var User = ((CustomPrincipal)HttpContext.Current.User);
            if(User.userbasedsecurity != null && User.userbasedsecurity.Where(p => p.IsMainEntity).Count() > 0)
            {
                var _ubs = User.userbasedsecurity.FirstOrDefault(p => p.IsMainEntity);
                if(_ubs.RolesToIgnore != null && User.IsInRole(User.userroles,_ubs.RolesToIgnore.Split(",".ToCharArray())))
                    return;
                DbContext.Users = new FilteredDbSet<ApplicationUser>(DbContext, d => d.UserName == HttpContext.Current.User.Identity.Name);
            }
        }
        /// <summary>Applies the custom security defined by code hook.</summary>
        public void CustomFilter()
        {
        }
    }
}
/// <summary>Helper class to manages role and user related actions.</summary>
public partial class IdentityManager
{
    /// <summary>Queries if a given role already exists.</summary>
    ///
    /// <param name="name">The name.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool RoleExists(string name)
    {
        var rm = new RoleManager<ApplicationRole>(
            new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        return rm.RoleExists(name);
    }
    /// <summary>Creates a role.</summary>
    ///
    /// <param name="LogggedInUser">The loggged in user.</param>
    /// <param name="name">         The name.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CreateRole(IUser LogggedInUser, string name)
    {
        var rm = new RoleManager<ApplicationRole>(
            new RoleStore<ApplicationRole>(new ApplicationDbContext(LogggedInUser)));
        var idResult = rm.Create(new ApplicationRole(name));
        return idResult.Succeeded;
    }
    
    /// <summary>Creates a user.</summary>
    ///
    /// <param name="LogggedInUser">The loggged in user.</param>
    /// <param name="user">         The user.</param>
    /// <param name="password">     The password.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CreateUser(IUser LogggedInUser, ApplicationUser user, string password)
    {
        var um = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        um.UserValidator = new UserValidator<ApplicationUser>(um)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        var idResult = um.Create(user, password);
        return idResult.Succeeded;
    }
    
    /// <summary>Adds a user to role.</summary>
    ///
    /// <param name="LogggedInUser">The loggged in user.</param>
    /// <param name="userId">       Identifier for the user.</param>
    /// <param name="roleName">     Name of the role.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool AddUserToRole(IUser LogggedInUser, string userId, string roleName)
    {
        var um = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        roleName = (new ApplicationDbContext()).Roles.Find(roleName) == null ? roleName : (new ApplicationDbContext()).Roles.Find(roleName).Name;
        um.UserValidator = new UserValidator<ApplicationUser>(um)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        var idResult = um.AddToRole(userId, roleName);
        return idResult.Succeeded;
    }
    
    /// <summary>Remove roles of the user.</summary>
    ///
    /// <param name="LogggedInUser">The loggged in user.</param>
    /// <param name="userId">       Identifier for the user.</param>
    /// <param name="rolesToIgnore">(Optional) The roles to ignore.</param>
    
    public void ClearUserRoles(IUser LogggedInUser, string userId, List<string> rolesToIgnore = null)
    {
        if(rolesToIgnore == null)
            rolesToIgnore = new List<string>();
        var um = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        um.UserValidator = new UserValidator<ApplicationUser>(um)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        var user = um.FindById(userId);
        using(var ctx = new ApplicationDbContext())
        {
            var roleIds = user.Roles.Where(p=> !rolesToIgnore.Contains(p.RoleId)).Select(r => r.RoleId);
            var roles = ctx.Roles.Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name);
            um.RemoveFromRoles(userId, roles.ToArray());
        }
    }
    
    /// <summary>Remove the users from role.</summary>
    ///
    /// <param name="LogggedInUser">The loggged in user.</param>
    /// <param name="userId">       Identifier for the user.</param>
    /// <param name="roleName">     Name of the role.</param>
    
    public void ClearUsersFromRole(IUser LogggedInUser, string userId, string roleName)
    {
        var um = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        roleName = (new ApplicationDbContext()).Roles.Find(roleName) == null ? roleName : (new ApplicationDbContext()).Roles.Find(roleName).Name;
        um.UserValidator = new UserValidator<ApplicationUser>(um)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        um.RemoveFromRole(userId, roleName);
    }
}
}

