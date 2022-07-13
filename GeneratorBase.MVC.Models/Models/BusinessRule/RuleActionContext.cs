using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Models
{
/// <summary>A rule action context.</summary>
public class RuleActionContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public RuleActionContext()
        : base("DefaultConnection")
    {
        this.ApplyFilters(new List<IFilter<RuleActionContext>>()
        {
            new RuleActionsFilter()
        });
    }
    
    /// <summary>Gets or sets the rule actions.</summary>
    ///
    /// <value>The rule actions.</value>
    
    public IDbSet<RuleAction> RuleActions
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
        ObjectContext ctx = ((IObjectContextAdapter)this).ObjectContext;
        List<ObjectStateEntry> objectStateEntryList =
            ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added
                    | EntityState.Modified
                    | EntityState.Deleted)
            .ToList();
        EntityState state = new EntityState();
        ObjectStateEntry objectStateEntry = null;
        if(objectStateEntryList.Count > 0)
        {
            objectStateEntry = objectStateEntryList.Where(p => p.Entity is RuleAction).ToList()[0];
            state = objectStateEntry.State;
        }
        SetDisplayValue(objectStateEntryList,state);
        var result = base.SaveChanges();
        if(result > 0)
            CacheHelper.RemoveCache("RuleAction");
        return result;
    }
    /// <summary>Executes the business rules operation.</summary>
    private void DoBusinessRules()
    {
        //Business Rules goes here.....
    }
    
    /// <summary>Sets display value.</summary>
    ///
    /// <param name="objectStateEntryList">List of object state entries.</param>
    /// <param name="state">               The state.</param>
    
    private void SetDisplayValue(List<ObjectStateEntry> objectStateEntryList,EntityState state)
    {
        if(state == EntityState.Deleted) return;
        foreach(var objectStateEntry in objectStateEntryList)
        {
            if(objectStateEntry.Entity is RuleAction)
            {
                var entity = ((GeneratorBase.MVC.Models.RuleAction)(objectStateEntry.Entity));
                ((GeneratorBase.MVC.Models.RuleAction)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
            }
        }
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
        modelBuilder.Entity<RuleAction>().HasOptional<BusinessRule>(p => p.ruleaction).WithMany(s => s.ruleaction).HasForeignKey(f => f.RuleActionID).WillCascadeOnDelete(false);
        modelBuilder.Entity<BusinessRule>().HasOptional<BusinessRuleType>(p => p.associatedbusinessruletype).WithMany(s => s.associatedbusinessruletype).HasForeignKey(f => f.AssociatedBusinessRuleTypeID).WillCascadeOnDelete(false);
        modelBuilder.Entity<RuleAction>().HasOptional<ActionType>(p => p.associatedactiontype).WithMany(s => s.associatedactiontype).HasForeignKey(f => f.AssociatedActionTypeID).WillCascadeOnDelete(false);
        base.OnModelCreating(modelBuilder);
    }
    
    /// <summary>Applies the filters described by filters.</summary>
    ///
    /// <param name="filters">The filters.</param>
    
    public void ApplyFilters(IList<IFilter<RuleActionContext>> filters)
    {
    }
    /// <summary>A rule actions filter.</summary>
    public class RuleActionsFilter : IFilter<RuleActionContext>
    {
        /// <summary>Gets or sets a context for the database.</summary>
        ///
        /// <value>The database context.</value>
        
        public RuleActionContext DbContext
        {
            get;
            set;
        }
        /// <summary>Applies the basic security.</summary>
        public void ApplyBasicSecurity()
        {
        }
        /// <summary>Applies the main security.</summary>
        public void ApplyMainSecurity()
        {
        }
        /// <summary>Applies the user based security.</summary>
        public void ApplyUserBasedSecurity()
        {
        }
        /// <summary>Custom filter.</summary>
        public void CustomFilter()
        {
        }
        /// <summary>Applies the HierarchicalSecurity.</summary>
        public void ApplyHierarchicalSecurity()
        {
        }
    }
}
}


