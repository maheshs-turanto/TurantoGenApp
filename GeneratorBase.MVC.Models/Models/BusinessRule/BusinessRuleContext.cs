using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Models
{
/// <summary>The business rule context.</summary>
public class BusinessRuleContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public BusinessRuleContext()
        : base("DefaultConnection")
    {
        //this.BusinessRules.FromCache("businessrule");
    }
    
    /// <summary>Gets or sets the business rules.</summary>
    ///
    /// <value>The business rules.</value>
    
    public IDbSet<BusinessRule> BusinessRules
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
            objectStateEntry = objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0];
            state = objectStateEntry.State;
        }
        SetDisplayValue(objectStateEntryList, state);
        if(objectStateEntry != null && state == EntityState.Modified)
        {
            var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
            MakeUpdateJournalEntry(id, objectStateEntry.State.ToString(), objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0]);
        }
        if(objectStateEntry != null && state == EntityState.Deleted)
        {
            var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
            ConditionContext brconditions = new ConditionContext();
            long? longid = Convert.ToInt64(id);
            var condlist = brconditions.Conditions.Where(p => p.RuleConditionsID.Value == longid);
            foreach(var cond in condlist)
                brconditions.Conditions.Remove(cond);
            brconditions.SaveChanges();
            RuleActionContext brActions = new RuleActionContext();
            ActionArgsContext brArgs = new ActionArgsContext();
            var actionlist = brActions.RuleActions.Where(p => p.RuleActionID.Value == longid);
            foreach(var action in actionlist)
            {
                brActions.RuleActions.Remove(action);
                var argslist = brArgs.ActionArgss.Where(p => p.ActionArgumentsID == action.Id);
                foreach(var args in argslist)
                    brArgs.ActionArgss.Remove(args);
                brArgs.SaveChanges();
            }
            brActions.SaveChanges();
        }
        var result = base.SaveChanges();
        if(result > 0)
        {
            CacheHelper.RemoveCache("businessrule");
            CacheHelper.RemoveCache("actionargs");
            CacheHelper.RemoveCache("ruleAction");
            CacheHelper.RemoveCache("condition");
            CacheHelper.RemoveCache("ActionType");
        }
        if(objectStateEntryList.Count > 0)
        {
            if(objectStateEntry != null && state == EntityState.Added)
            {
                var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
                MakeAddJournalEntry(id, state.ToString(), objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0]);
            }
        }
        return result;
    }
    
    /// <summary>Gets action arguments.</summary>
    ///
    /// <param name="ActionIds">List of identifiers for the actions.</param>
    ///
    /// <returns>The action arguments.</returns>
    
    public static List<ActionArgs> GetActionArguments(List<long> ActionIds)
    {
        return new ActionArgsContext().ActionArgss.Where(p => ActionIds.Contains(p.ActionArgumentsID.Value)).GetFromCache<IQueryable<ActionArgs>, ActionArgs>().ToList();
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
            if(objectStateEntry.Entity is BusinessRule)
            {
                var entity = ((GeneratorBase.MVC.Models.BusinessRule)(objectStateEntry.Entity));
                ((GeneratorBase.MVC.Models.BusinessRule)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
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
        modelBuilder.Entity<BusinessRule>().HasOptional<BusinessRuleType>(p => p.associatedbusinessruletype).WithMany(s => s.associatedbusinessruletype).HasForeignKey(f => f.AssociatedBusinessRuleTypeID).WillCascadeOnDelete(false);
        base.OnModelCreating(modelBuilder);
    }
    
    /// <summary>Makes add journal entry.</summary>
    ///
    /// <param name="id">   The identifier.</param>
    /// <param name="state">The state.</param>
    /// <param name="entry">The entry.</param>
    
    private void MakeAddJournalEntry(string id, string state, ObjectStateEntry entry)
    {
        using(JournalEntryContext db = new JournalEntryContext())
        {
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.UtcNow;
            Je.EntityName = "BusinessRule";
            Je.UserName = System.Web.HttpContext.Current.User.Identity.Name;
            Je.Type = state;
            if(entry.Entity is BusinessRule)
            {
                var displayValue = ((GeneratorBase.MVC.Models.BusinessRule)(entry.Entity)).DisplayValue;
                Je.RecordInfo = displayValue;
                Je.RecordId = Convert.ToInt64(id);
                Je.BrowserInfo = DoAuditEntry.GetBrowserDetails();
                db.JournalEntries.Add(Je);
                db.SaveChanges();
            }
        }
    }
    
    /// <summary>Makes update journal entry.</summary>
    ///
    /// <param name="id">   The identifier.</param>
    /// <param name="state">The state.</param>
    /// <param name="entry">The entry.</param>
    
    private void MakeUpdateJournalEntry(string id, string state, ObjectStateEntry entry)
    {
        List<string> result = new List<string>();
        using(var context = new BusinessRuleContext())
        {
            var _BusinessRule = context.BusinessRules.Find(Convert.ToInt64(id));
            result = EntityComparer.EnumeratePropertyDifferences<BusinessRule>(_BusinessRule, ((GeneratorBase.MVC.Models.BusinessRule)(entry.Entity)), state, "BusinessRule").ToList();
        }
    }
    /// <summary>A mapped value.</summary>
    public class MappedValue
    {
        /// <summary>Gets or sets the identifier of the action.</summary>
        ///
        /// <value>The identifier of the action.</value>
        
        public long ActionID
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the brid.</summary>
        ///
        /// <value>The brid.</value>
        
        public long BRID
        {
            get;
            set;
        }
        public long? BRTID
        {
            get;
            set;
        }
    }
}
}


