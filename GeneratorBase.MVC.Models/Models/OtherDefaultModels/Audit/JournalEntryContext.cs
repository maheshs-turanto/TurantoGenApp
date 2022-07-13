using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A journal entry context.</summary>
public class JournalEntryContext : ApplicationContext
{
    /// <summary>The user.</summary>
    IUser user;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">The user.</param>
    
    public JournalEntryContext(IUser user)
        : base(user)
    {
        this.user = user;
        this.Configuration.LazyLoadingEnabled = false;
        if(this.user != null && !this.user.IsAdmin)
            this.ApplyFilters(new List<IFilter<JournalEntryContext>>()
        {
            new JournalEntrySecurityFilter(user)
        });
    }
    /// <summary>Default constructor.</summary>
    public JournalEntryContext():base(new SystemUser())
    {
    }
    
    /// <summary>Sets date time to UTC.</summary>
    ///
    /// <param name="dbEntityEntry">The database entity entry.</param>
    
    private void SetDateTimeToUTC(DbEntityEntry dbEntityEntry)//mahesh
    {
        Type EntityType = dbEntityEntry.Entity.GetType();
        dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
        try
        {
            EntityObj.setDateTimeToUTC();
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
    /// <summary>Saves all changes made in this context to the underlying database.</summary>
    ///
    /// <returns>The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property included
    /// in the entity class (often referred to as independent associations).</returns>
    
    public override int SaveChanges()
    {
        var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                      e.State.HasFlag(EntityState.Modified) ||
                      e.State.HasFlag(EntityState.Deleted));
        foreach(var entry in entries)
        {
            SetDateTimeToUTC(entry);
        }
        var result = base.DirectSaveChanges();
        return result;
    }
    
    /// <summary>This method is called when the model for a derived context has been initialized, but
    /// before the model has been locked down and used to initialize the context.  The default
    /// implementation of this method does nothing, but it can be overridden in a derived class such
    /// that the model can be further configured before it is locked down.</summary>
    ///
    /// <remarks>Typically, this method is called only once when the first instance of a derived
    /// context is created.  The model for that context is then cached and is for all further
    /// instances of the context in the app domain.  This caching can be disabled by setting the
    /// ModelCaching property on the given ModelBuidler, but note that this can seriously degrade
    /// performance. More control over caching is provided through use of the DbModelBuilder and
    /// DbContextFactory classes directly.</remarks>
    ///
    /// <param name="modelBuilder"> The builder that defines the model for the context being created.</param>
    
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
        var role = modelBuilder.Entity<IdentityRole>()
                   .ToTable("AspNetRoles");
        role.Property(r => r.Name).IsRequired();
        role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
    }
    
    /// <summary>Applies the filters described by filters.</summary>
    ///
    /// <param name="filters">The filters.</param>
    
    public void ApplyFilters(IList<IFilter<JournalEntryContext>> filters)
    {
        foreach(var filter in filters)
        {
            filter.DbContext = this;
            if(user != null && (!string.IsNullOrEmpty(user.Name)))
            {
                filter.ApplyBasicSecurity();
                filter.ApplyUserBasedSecurity();
                filter.ApplyMainSecurity();
                filter.CustomFilter();
            }
        }
    }
}
}
