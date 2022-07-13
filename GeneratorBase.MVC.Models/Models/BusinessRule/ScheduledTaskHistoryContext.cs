using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
namespace GeneratorBase.MVC.Models
{
/// <summary>A scheduled task history context.</summary>
public class ScheduledTaskHistoryContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public ScheduledTaskHistoryContext()
        : base("DefaultConnection")
    {
        this.ApplyFilters(new List<IFilter<ScheduledTaskHistoryContext>>()
        {
            new ScheduledTaskHistorysFilter()
        });
    }
    
    /// <summary>Gets or sets the scheduled task historys.</summary>
    ///
    /// <value>The scheduled task historys.</value>
    
    public IDbSet<ScheduledTaskHistory> ScheduledTaskHistorys
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
            objectStateEntry = objectStateEntryList.Where(p => p.Entity is ScheduledTaskHistory).ToList()[0];
            state = objectStateEntry.State;
        }
        SetDisplayValue(objectStateEntryList,state);
        var result = base.SaveChanges();
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
            if(objectStateEntry.Entity is ScheduledTaskHistory)
            {
                var entity = ((GeneratorBase.MVC.Models.ScheduledTaskHistory)(objectStateEntry.Entity));
                ((GeneratorBase.MVC.Models.ScheduledTaskHistory)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
            }
        }
    }
    
    /// <summary>Applies the filters described by filters.</summary>
    ///
    /// <param name="filters">The filters.</param>
    
    public void ApplyFilters(IList<IFilter<ScheduledTaskHistoryContext>> filters)
    {
        foreach(var filter in filters)
        {
            filter.DbContext = this;
            filter.ApplyBasicSecurity();
            filter.ApplyMainSecurity();
            filter.ApplyUserBasedSecurity();
        }
    }
    /// <summary>A scheduled task historys filter.</summary>
    public class ScheduledTaskHistorysFilter : IFilter<ScheduledTaskHistoryContext>
    {
        /// <summary>Gets or sets a context for the database.</summary>
        ///
        /// <value>The database context.</value>
        
        public ScheduledTaskHistoryContext DbContext
        {
            get;
            set;
        }
        /// <summary>Applies the basic security.</summary>
        public void ApplyBasicSecurity()
        {
        }
        /// <summary>Custom filter.</summary>
        public void CustomFilter()
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
        /// <summary>Applies the HierarchicalSecurity.</summary>
        public void ApplyHierarchicalSecurity()
        {
        }
    }
}
}


