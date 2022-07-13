using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace GeneratorBase.MVC.Models
{
public class CustomRemoteValidation : RemoteAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if(value == null || (value is string && string.IsNullOrWhiteSpace((string)value)) || (value is long && (long)value == 0)) return ValidationResult.Success;
        var type = validationContext.ObjectType;
        var property = type.GetProperty(validationContext.MemberName);
        type = property.DeclaringType;
        using(var dbcontext = (IDisposable)Activator.CreateInstance(typeof(ApplicationContext), new SystemUser(), null, 0))
        {
            var context = dbcontext is DbContext ? ((IObjectContextAdapter)dbcontext).ObjectContext : (ObjectContext)dbcontext;
            context.ContextOptions.LazyLoadingEnabled = false;
            var md = context.MetadataWorkspace;
            var entityType = md.GetItems<EntityType>(DataSpace.CSpace).SingleOrDefault(et => et.Name == type.Name);
            while(entityType.BaseType != null)
                entityType = (EntityType)entityType.BaseType;
            var objectType = typeof(object);
            var isInherited = false;
            var baseType = type;
            while(baseType.Name != entityType.Name && baseType.BaseType != objectType)
            {
                baseType = baseType.BaseType;
                isInherited = true;
            }
            var methodCreateObjectSet = typeof(ObjectContext).GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(baseType);
            var baseObjectSet = (ObjectQuery)methodCreateObjectSet.Invoke(context, new object[] { });
            var objectSet = baseObjectSet;
            var setType = baseObjectSet.GetType();
            if(isInherited)
            {
                var ofType = setType.GetMethod("OfType");
                ofType = ofType.MakeGenericMethod(type);
                objectSet = (ObjectQuery)ofType.Invoke(baseObjectSet, null);
                setType = objectSet.GetType();
            }
            var methodWhere = setType.GetMethod("Where");
            var eSql = string.Format("it.{0} = @{0}", validationContext.MemberName);
            var query = (ObjectQuery)methodWhere.Invoke(objectSet,
            new object[] { eSql, new[] { new ObjectParameter(validationContext.MemberName, value) } });
            var result = query.Execute(MergeOption.NoTracking).Cast<object>();
            bool isValid = true;
            using(var enumerator = result.GetEnumerator())
            {
                if(enumerator.MoveNext())
                {
                    var nameProperty = typeof(ObjectSet<>).MakeGenericType(baseType).GetProperty("EntitySet");
                    var entitySet = (EntitySet)nameProperty.GetValue(baseObjectSet, null);
                    var entitySetName = entitySet.Name;
                    do
                    {
                        var current = enumerator.Current;
                        var curKey = context.CreateEntityKey(entitySetName, current);
                        var validatingKey = context.CreateEntityKey(entitySetName, validationContext.ObjectInstance);
                        if(curKey != validatingKey)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    while(enumerator.MoveNext());
                }
            }
            dbcontext.Dispose();
            if(!string.IsNullOrEmpty(this.ErrorMessage))
            {
                // if (!isValid) this.error = Convert.ToString(value);
                return isValid ?
                       ValidationResult.Success :
                       new ValidationResult(
                           this.ErrorMessage,
                           new[] { validationContext.MemberName });
            }
            else
            {
                return isValid ?
                       ValidationResult.Success :
                       new ValidationResult(
                           string.Format("A record with its '{0}' set to '{1}' already exists.",
                                         validationContext.DisplayName,
                                         value),
                           new[] { validationContext.MemberName });
            }
        }
    }
    
    public CustomRemoteValidation(string routeName)
        : base(routeName)
    {
    }
    
    public CustomRemoteValidation(string action, string controller)
        : base(action, controller)
    {
    }
    
    public CustomRemoteValidation(string action, string controller,
                                  string areaName) : base(action, controller, areaName)
    {
    }
    
}
}
