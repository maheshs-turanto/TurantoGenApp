using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A permission context.</summary>
public class PermissionContext : DbContext
{
    /// <summary>The user.</summary>
    IUser user;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">The user.</param>
    
    public PermissionContext(IUser user)
        : base("DefaultConnection")
    {
        this.user = user;
    }
    /// <summary>Default constructor.</summary>
    public PermissionContext()
        : base("DefaultConnection")
    {
    }
    
    /// <summary>Gets or sets the permissions.</summary>
    ///
    /// <value>The permissions.</value>
    
    public DbSet<Permission> Permissions
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the admin privileges.</summary>
    ///
    /// <value>The admin privileges.</value>
    
    public DbSet<PermissionAdminPrivilege> AdminPrivileges
    {
        get;
        set;
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
        //var entry = entries.FirstOrDefault();
        //if (entry != null)
        //{
        //    if (entry.State == EntityState.Modified)
        //        DoAuditEntry.MakeUpdateJournalEntry(user, entry);
        //    if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
        //        DoAuditEntry.MakeAddJournalEntry(user, null, entry);
        //}
        foreach(var entry in entries)
        {
            try
            {
                if(entry.State == EntityState.Modified)
                    DoAuditEntry.MakeUpdateJournalEntry(user, entry);
                if(entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                    DoAuditEntry.MakeAddJournalEntry(user, null, entry);
            }
            catch
            {
                continue;
            }
            CacheHelper.RemoveCache(ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
        }
        result = base.SaveChanges();
        return result;
    }
    /// <summary>Executes the business rules operation.</summary>
    private void DoBusinessRules()
    {
        //Business Rules goes here.....
    }
}
}
