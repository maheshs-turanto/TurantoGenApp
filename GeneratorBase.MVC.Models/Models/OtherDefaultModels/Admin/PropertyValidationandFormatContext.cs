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
/// <summary>An action arguments context.</summary>
public class PropertyValidationandFormatContext : DbContext
{
    /// <summary>Default constructor.</summary>
    public PropertyValidationandFormatContext()
        : base("DefaultConnection")
    {
    }
    
    /// <summary>Gets or sets the action argss.</summary>
    ///
    /// <value>The action argss.</value>
    
    public IDbSet<PropertyValidationandFormat> PropertyValidationandFormats
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
            objectStateEntry = objectStateEntryList.Where(p => p.Entity is PropertyValidationandFormat).ToList()[0];
            state = objectStateEntry.State;
        }
        SetDisplayValue(objectStateEntryList, state);
        var result = base.SaveChanges();
        if(result > 0)
            if(state != null && (state == EntityState.Modified || state == EntityState.Added))
            {
                AfterSave(objectStateEntry);
                CacheHelper.RemoveCache("PropertyValidationandFormat");
            }
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
            if(objectStateEntry.Entity is PropertyValidationandFormat)
            {
                var entity = ((GeneratorBase.MVC.Models.PropertyValidationandFormat)(objectStateEntry.Entity));
                ((GeneratorBase.MVC.Models.PropertyValidationandFormat)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
            }
        }
    }
    /// <summary>Apply logic after save.</summary>
    ///
    /// <param name="originals">The originals.</param>
    private void AfterSave(ObjectStateEntry originals)
    {
        var entityType = ObjectContext.GetObjectType(originals.Entity.GetType());
        var EntityName = entityType.Name;
        //dynamic EntityObj = Convert.ChangeType(entry.Entity, entityType);
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("AfterSave");
                object[] MethodParams = new object[] { originals.Entity, new SystemUser() };
                mc.Invoke(objController, MethodParams);
            }
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    
}
}


