using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A user define pages context.</summary>
public class UserDefinePagesContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public UserDefinePagesContext()
        : base("DefaultConnection")
    {
    }
    
    /// <summary>Gets or sets the user define pagess.</summary>
    ///
    /// <value>The user define pagess.</value>
    
    public DbSet<UserDefinePages> UserDefinePagess
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
        DoBusinessRules();
        CacheHelper.RemoveCache("UserDefinePages");
        return base.SaveChanges();
    }
    /// <summary>Executes the business rules operation.</summary>
    private void DoBusinessRules()
    {
        //Business Rules goes here.....
    }
}
}
