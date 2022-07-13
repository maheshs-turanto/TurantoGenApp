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
/// <summary>The business rule type context.</summary>
public class BusinessRuleTypeContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public BusinessRuleTypeContext()
        : base("DefaultConnection")
    {
        this.ApplyFilters(new List<IFilter<BusinessRuleTypeContext>>()
        {
            new BusinessRuleTypesFilter()
        });
    }
    
    /// <summary>Gets or sets a list of types of the business rules.</summary>
    ///
    /// <value>A list of types of the business rules.</value>
    
    public IDbSet<BusinessRuleType> BusinessRuleTypes
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
            objectStateEntry = objectStateEntryList.Where(p => p.Entity is BusinessRuleType).ToList()[0];
            state = objectStateEntry.State;
        }
        SetDisplayValue(objectStateEntryList, state);
        var result = base.SaveChanges();
        if(result > 0)
            CacheHelper.RemoveCache("BusinessRuleType");
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
    
    private void SetDisplayValue(List<ObjectStateEntry> objectStateEntryList, EntityState state)
    {
        if(state == EntityState.Deleted) return;
        foreach(var objectStateEntry in objectStateEntryList)
        {
            if(objectStateEntry.Entity is BusinessRuleType)
            {
                var entity = ((GeneratorBase.MVC.Models.BusinessRuleType)(objectStateEntry.Entity));
                ((GeneratorBase.MVC.Models.BusinessRuleType)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
            }
        }
    }
    
    /// <summary>Applies the filters described by filters.</summary>
    ///
    /// <param name="filters">The filters.</param>
    
    public void ApplyFilters(IList<IFilter<BusinessRuleTypeContext>> filters)
    {
        foreach(var filter in filters)
        {
            filter.DbContext = this;
            filter.ApplyBasicSecurity();
            filter.ApplyMainSecurity();
            filter.ApplyUserBasedSecurity();
        }
    }
    /// <summary>The business rule types filter.</summary>
    public class BusinessRuleTypesFilter : IFilter<BusinessRuleTypeContext>
    {
        /// <summary>Gets or sets a context for the database.</summary>
        ///
        /// <value>The database context.</value>
        
        public BusinessRuleTypeContext DbContext
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
        /// <summary>Applies the HierarchicalSecurity.</summary>
        public void ApplyHierarchicalSecurity()
        {
        }
        /// <summary>Custom filter.</summary>
        public void CustomFilter()
        {
        }
    }
}
}


