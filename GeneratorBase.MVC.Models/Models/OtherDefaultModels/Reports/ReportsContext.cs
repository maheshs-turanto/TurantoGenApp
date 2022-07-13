﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>The reports context.</summary>
public class ReportsContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public ReportsContext()
        : base("DefaultConnection")
    {
    }
    
    /// <summary>Gets or sets the reportss.</summary>
    ///
    /// <value>The reportss.</value>
    
    public DbSet<Reports> Reportss
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
        var result = base.SaveChanges();
        return result;
    }
    /// <summary>Executes the business rules operation.</summary>
    private void DoBusinessRules()
    {
        //Business Rules goes here.....
    }
}
}


