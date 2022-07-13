using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using GeneratorBase.MVC.DynamicQueryable;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace GeneratorBase.MVC.Models
{
/// <summary>Creates controller type with controller name.</summary>
public class CreateControllerType
{
    public Type controllerType;
    public CreateControllerType(string name)
    {
        Assembly ControllerAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => !p.GlobalAssemblyCache && p.FullName.Contains("GeneratorBase.MVC.Controllers"));
        if(ControllerAssembly != null)
            this.controllerType = Type.GetType("GeneratorBase.MVC.Controllers." + name + "Controller, " + ControllerAssembly.FullName);
        else this.controllerType = Type.GetType("GeneratorBase.MVC.Controllers." + name + "Controller");
    }
}
/// <summary>Update object with match columns information.</summary>
public static class MatchUpdate
{
    /// <summary>Updates this object.</summary>
    ///
    /// <param name="model">           The model.</param>
    /// <param name="MatchColumns">    The match columns.</param>
    /// <param name="db">              The database.</param>
    /// <param name="EntityName">      Name of the entity.</param>
    /// <param name="UpdateProperties">(Optional) The update properties.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool Update(object model, string MatchColumns, ApplicationContext db, string EntityName, List<string> UpdateProperties = null, bool isAdv = false)
    {
        var listMatchColumns = MatchColumns.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        var flagUpdate = false;
        db.Configuration.LazyLoadingEnabled = false;
        Type controller = new CreateControllerType(EntityName).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("GetIQueryable");
        object[] MethodParams = new object[] { db };
        var dataToUpdate = (IQueryable<IEntity>)mc.Invoke(objController, MethodParams);
        foreach(var item in listMatchColumns)
        {
            var value = GetValueFromObject(model, item);
            if(value != null)
            {
                var dataType = GetDataTypeObject(model, item, EntityName);
                var expression = Expression(dataType, item, "=", Convert.ToString(value));
                if(!string.IsNullOrEmpty(expression))
                {
                    dataToUpdate = dataToUpdate.Where(expression);
                    flagUpdate = true;
                }
                else
                {
                    flagUpdate = false;
                }
            }
        }
        var countvalue = dataToUpdate.AsNoTracking().Select(p => p.Id).Count();
        if(flagUpdate && countvalue > 0)
        {
            flagUpdate = true;
            if(countvalue == 1)
            {
                var listoupdate = dataToUpdate.AsNoTracking().ToList();
                object inlineAssociationObj = null;
                if(isAdv)
                {
                    Type VerbTypeCls = new GeneratorBase.MVC.Models.CreateControllerType(EntityName).controllerType;
                    if(VerbTypeCls != null)
                    {
                        MethodInfo method = VerbTypeCls.GetMethod("InlineAssociationList");
                        if(method != null)
                        {
                            using(var clsInstance = (IDisposable)Activator.CreateInstance(VerbTypeCls))
                            {
                                var inlineassociations = (string[])method.Invoke(clsInstance, null);
                                inlineAssociationObj = inlineassociations;
                                if(inlineassociations != null)
                                {
                                    foreach(var item in inlineassociations)
                                    {
                                        dataToUpdate = dataToUpdate.Include(item.Substring(0, item.Length - 2).ToLower());
                                    }
                                    listoupdate = dataToUpdate.AsNoTracking().ToList();
                                }
                            }
                        }
                    }
                }
                if(listoupdate != null && listoupdate.Count > 0)
                {
                    foreach(var obj in listoupdate)
                    {
                        try
                        {
                            if(!isAdv)
                                CopyValues(model, obj, listMatchColumns, UpdateProperties);
                            else
                                CopyValuesAdv(model, obj, listMatchColumns, db, UpdateProperties, EntityName, inlineAssociationObj);
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch(Exception x)
                        {
                            db.Entry(obj).State = EntityState.Detached;
                            db.Entry(model).State = EntityState.Detached;
                            continue;
                        }
                    }
                }
            }
        }
        else
            flagUpdate = false;
        //
        return flagUpdate;
    }
    
    /// <summary>Copies the values.</summary>
    ///
    /// <param name="source">              Source for the.</param>
    /// <param name="destination">         Destination for the.</param>
    /// <param name="excludeProp">         The exclude property.</param>
    /// <param name="onlyMappedProperties">(Optional) The only mapped properties.</param>
    
    public static void CopyValues(object source, object destination, List<string> excludeProp, List<string> onlyMappedProperties = null)
    {
        foreach(var item1 in source.GetType().GetProperties())
        {
            var item = item1.Name;
            if(item == "Id" || item == "ConcurrencyKey" || item == "TenantId" || item == "DisplayValue" || excludeProp.Contains(item) || item == "m_Timezone" || item == "ICollection`1") continue;
            if(onlyMappedProperties != null && !onlyMappedProperties.Where(p => !string.IsNullOrEmpty(p)).Contains(item)) continue;
            var target = item;
            var src = item;
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    targetProp.SetValue(destination, sourceProp.GetValue(
                                            source, new object[] { }), new object[] { });
                }
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="excludeProp"></param>
    /// <param name="db"></param>
    /// <param name="onlyMappedProperties"></param>
    /// <param name="EntityName"></param>
    public static void CopyValuesAdv(object source, object destination, List<string> excludeProp, ApplicationContext db, List<string> onlyMappedProperties = null, string EntityName = null, object inlineAssociationObj = null)
    {
        List<string> onlyMappedPropertiesNew = new List<string>();
        if(onlyMappedProperties != null)
        {
            foreach(var onlymapprop in onlyMappedProperties)
            {
                var navigation = onlymapprop.Split('$');
                string props = navigation[navigation.Length - 1];
                if(navigation.Length > 2)
                {
                    onlyMappedPropertiesNew.Add(navigation[navigation.Length - 2] + "ID");
                    props = navigation[navigation.Length - 2].ToLower();
                }
                if(onlyMappedPropertiesNew != null && onlyMappedPropertiesNew.Where(p => !string.IsNullOrEmpty(p)).Contains(props)) continue;
                onlyMappedPropertiesNew.Add(props);
            }
        }
        foreach(var item1 in source.GetType().GetProperties())
        {
            var item = item1.Name;
            if(item == "Id" || item == "ConcurrencyKey" || item == "TenantId" || item == "DisplayValue" || excludeProp.Contains(item) || item == "m_Timezone" || item == "ICollection`1") continue;
            if(onlyMappedProperties != null && !onlyMappedPropertiesNew.Where(p => !string.IsNullOrEmpty(p)).Contains(item)) continue;
            var target = item;
            var src = item;
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    var newobj = sourceProp.GetValue(
                                     source, new object[] { });
                    if(newobj is IEntity)
                    {
                        if(inlineAssociationObj != null)
                        {
                            var inlineAssociation = (string[])inlineAssociationObj;
                            if(inlineAssociation.Contains(target + "id", StringComparer.CurrentCultureIgnoreCase) || inlineAssociation.Contains(target, StringComparer.CurrentCultureIgnoreCase))
                            {
                                targetProp = destination.GetType().GetProperty(target);
                                var targetval = targetProp.GetValue(destination);
                                if(targetval != null)
                                {
                                    sourceProp = targetval.GetType().GetProperty("Id");
                                    var value = sourceProp.GetValue(
                                                    targetval, new object[] { });
                                    long existingid = 0;
                                    Int64.TryParse(Convert.ToString(value), out existingid);
                                    if(existingid == 0)
                                    {
                                        newobj.GetType().GetProperty("Id").SetValue(newobj, 0, new object[] { });
                                        db.Entry(newobj).State = EntityState.Added;
                                    }
                                    else
                                    {
                                        newobj.GetType().GetProperty("Id").SetValue(newobj, value, new object[] { });
                                        newobj.GetType().GetProperty("ConcurrencyKey").SetValue(newobj, targetval.GetType().GetProperty("ConcurrencyKey").GetValue(targetval, new object[] { }), new object[] { });
                                        db.Entry(newobj).State = EntityState.Modified;
                                    }
                                }
                                //else
                                //{
                                //    if (inlineAssociation.Contains(target, StringComparer.CurrentCultureIgnoreCase))
                                //    {
                                //    }
                                //}
                            }
                        }
                        long Id = ((IEntity)newobj).Id;
                        if(Id == 0)
                            db.Entry(newobj).State = EntityState.Added;
                        else if(Id > 0)
                        {
                            var Assocation = ModelReflector.Entities.Single(e => e.Name == EntityName).Associations.Where(p => p.Name.ToLower() != "tenantid" && p.Name.ToLower() == item).FirstOrDefault();
                            if(Assocation != null)
                                destination.GetType().GetProperty(Assocation.AssociationProperty).SetValue(destination, Id);
                        }
                    }
                    targetProp.SetValue(destination, newobj, new object[] { });
                }
        }
    }
    
    
    /// <summary>Expressions.</summary>
    ///
    /// <param name="dataType"> Type of the data.</param>
    /// <param name="property"> The property.</param>
    /// <param name="operators">The operators.</param>
    /// <param name="value">    The value.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string Expression(string dataType, string property, string operators, string value)
    {
        var expression = "";
        switch(dataType)
        {
        case "Int32":
        case "Int64":
        case "Double":
        case "Boolean":
        case "Decimal":
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        case "DateTime":
        {
            if(value.Trim().ToLower() == "today")
            {
                //expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new T_Employee()).m_Timezone)).Date);
            }
            else
            {
                DateTime val = Convert.ToDateTime(value);
                expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
            }
            break;
        }
        case "Text":
        case "String":
        {
            if(operators.ToLower() == "contains" || operators.ToLower() == "!contains")
                expression = string.Format(" " + property + "." + operators + "(\"{0}\")", value);
            else
                expression = string.Format(" " + property + " " + operators + " \"{0}\"", value);
            break;
        }
        default:
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        }
        return expression;
    }
    
    /// <summary>Gets value from object.</summary>
    ///
    /// <param name="source">Source for the.</param>
    /// <param name="prop">  The property.</param>
    ///
    /// <returns>The value from object.</returns>
    
    public static object GetValueFromObject(object source, string prop)
    {
        var property = source.GetType().GetProperty(prop);
        if(property != null)
        {
            object value1 = property.GetValue(source, null);
            return value1;
        }
        return null;
    }
    
    /// <summary>Gets data type object.</summary>
    ///
    /// <param name="source">    Source for the.</param>
    /// <param name="prop">      The property.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The data type object.</returns>
    
    public static string GetDataTypeObject(object source, string prop, string entityName)
    {
        var property = source.GetType().GetProperty(prop);
        if(property != null)
        {
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == prop);
            string value1 = PropInfo.DataType;
            return value1;
        }
        return "";
    }
}
/// <summary>An entity copy.</summary>
public static class EntityCopy
{
    /// <summary>Copies the values.</summary>
    ///
    /// <param name="source">       Another instance to copy.</param>
    /// <param name="destination">  Destination for the.</param>
    /// <param name="ColumnMapping">The column mapping.</param>
    /// <param name="CodePrefix">   The code prefix.</param>
    
    public static void CopyValues(object source, object destination, string ColumnMapping, string CodePrefix)
    {
        var mapping = ColumnMapping.Split(",".ToCharArray());
        foreach(var item in mapping)
        {
            var map = item.Split("-".ToCharArray());
            var target = CodePrefix + map[0];
            var src = CodePrefix + map[1];
            //code for document
            var doc = map[1].Split("#".ToCharArray());
            long documentID = 0;
            if(doc.Length > 1)
            {
                if(doc[1] == "Doc")
                {
                    src = CodePrefix + doc[0];
                    ApplicationContext db1 = new ApplicationContext(new SystemUser());
                    var docIdpro = source.GetType().GetProperty(src);
                    var docId = docIdpro.GetValue(
                                    source, new object[] { });
                    if(docId != null)
                    {
                        Document document = db1.Documents.Find(docId);
                        db1.Documents.Add(document);
                        db1.SaveChanges();
                        documentID = document.Id;
                    }
                }
            }
            //
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    //changes for document
                    if(documentID == 0)
                    {
                        targetProp.SetValue(destination, sourceProp.GetValue(
                                                source, new object[] { }), new object[] { });
                    }
                    else
                        targetProp.SetValue(destination, documentID);
                }
        }
    }
    
    /// <summary>Copies the values for same object type.</summary>
    ///
    /// <param name="source">       Another instance to copy.</param>
    /// <param name="destination">  Destination for the.</param>
    /// <param name="ColumnMapping">The column mapping.</param>
    
    public static void CopyValuesForSameObjectType(object source, object destination, string ColumnMapping)
    {
        var mapping = ColumnMapping.Split(",".ToCharArray());
        foreach(var item in mapping)
        {
            var target = item;
            var src = item;
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    targetProp.SetValue(destination, sourceProp.GetValue(
                                            source, new object[] { }), new object[] { });
                }
        }
    }
    
    public static void CopyValuesForSameObjectType1(object source, object destination)
    {
        foreach(var item1 in source.GetType().GetProperties())
        {
            var item = item1.Name;
            if(item == "Id" || item == "ConcurrencyKey" || item == "TenantId" || item == "DisplayValue" || item == "m_Timezone" || item1.PropertyType.Name == "ICollection`1" || item1.PropertyType.BaseType == typeof(Entity))
                continue;
            var target = item;
            var src = item;
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    targetProp.SetValue(destination, sourceProp.GetValue(
                                            source, new object[] { }), new object[] { });
                }
        }
    }
    /// <summary>Copies the values for same object type.</summary>
    ///
    /// <param name="source">     Another instance to copy.</param>
    /// <param name="destination">Destination for the.</param>
    
    public static void CopyValuesForSameObjectType(object source, object destination)
    {
        foreach(var item1 in source.GetType().GetProperties())
        {
            var item = item1.Name;
            if(item == "Id" || item == "ConcurrencyKey" || item == "TenantId" || item == "DisplayValue" || item == "m_Timezone" || item1.PropertyType.Name == "ICollection`1")
                continue;
            var target = item;
            var src = item;
            var targetProp = destination.GetType().GetProperty(target);
            var sourceProp = source.GetType().GetProperty(src);
            if(sourceProp == null && targetProp == null)
            {
                targetProp = destination.GetType().GetProperty(target + "ID");
                sourceProp = source.GetType().GetProperty(src + "ID");
            }
            if(sourceProp != null && targetProp != null)
                if(targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    targetProp.SetValue(destination, sourceProp.GetValue(
                                            source, new object[] { }), new object[] { });
                }
        }
    }
}
/// <summary>An entity comparer.</summary>
public static class EntityComparer
{
    /// <summary>Gets group by display value.</summary>
    ///
    /// <param name="source">    Source for the.</param>
    /// <param name="entityname">The entityname.</param>
    /// <param name="props">     The properties.</param>
    ///
    /// <returns>The group by display value.</returns>
    
    public static string GetGroupByDisplayValue(object source, string entityname, string[] props)
    {
        var result = string.Empty;
        var modelprop = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityname);
        foreach(string prop in props)
        {
            if(string.IsNullOrEmpty(prop)) continue;
            var asso = modelprop.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
            string value1 = Convert.ToString((source.GetType()).GetProperty(prop).GetValue(source, null));
            if(asso != null)
            {
                if(value1 != null)
                {
                    if(result.Length > 0)
                        result += " - ";
                    result += EntityComparer.GetDisplayValueForAssociation(asso.Target, value1);
                }
                else
                {
                    if(result.Length > 0)
                        result += " - ";
                    result += "None";
                }
            }
            else
            {
                var proptype = modelprop.Properties.FirstOrDefault(p => p.Name == prop);
                if(proptype.DataType == "DateTime")
                {
                    if(proptype.DataTypeAttribute == "Date")
                        value1 = Convert.ToDateTime(value1).ToShortDateString();
                    if(proptype.DataTypeAttribute == "Time")
                        value1 = Convert.ToDateTime(value1).ToShortTimeString();
                }
                if(result.Length > 0)
                    result += " - ";
                result += value1;
            }
        }
        return result;
    }
    
    /// <summary>Enumerates the property differences in this collection.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="obj1">      The obj1 to act on.</param>
    /// <param name="obj2">      The second object.</param>
    /// <param name="state">     The state.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the property differences in
    /// this collection.</returns>
    
    public static IEnumerable<string> EnumeratePropertyDifferences<T>(this T obj1, T obj2, string state, string EntityName)
    {
        PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        List<string> changes = new List<string>();
        string id = Convert.ToString(typeof(T).GetProperty("Id").GetValue(obj2, null));
        string dispValue = Convert.ToString(typeof(T).GetProperty("DisplayValue").GetValue(obj2, null));
        var entityModel = ModelReflector.Entities;
        using(var db = new JournalEntryContext())
        {
            foreach(PropertyInfo pi in properties)
            {
                if(pi.Name == "DisplayValue" || pi.Name == "TenantId" || pi.Name == "ConcurrencyKey" || typeof(T).GetProperty(pi.Name).PropertyType.Name == "ICollection`1") continue;
                object value1 = typeof(T).GetProperty(pi.Name).GetValue(obj1, null);
                object value2 = typeof(T).GetProperty(pi.Name).GetValue(obj2, null);
                if(value1 != value2 && (value1 == null || !value1.Equals(value2)))
                {
                    JournalEntry Je = new JournalEntry();
                    Je.DateTimeOfEntry = DateTime.UtcNow;
                    Je.EntityName = EntityName;
                    Je.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    Je.Type = state;
                    var displayValue = dispValue;
                    Je.RecordInfo = displayValue;
                    Je.PropertyName = pi.Name;
                    var assolist = entityModel.Where(p => p.Name == EntityName).ToList()[0].Associations.Where(p => p.AssociationProperty == pi.Name).ToList();
                    if(assolist.Count() > 0)
                    {
                        Je.PropertyName = assolist[0].DisplayName;
                        if(value1 != null)
                            Je.OldValue = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value1));
                        if(value2 != null)
                            Je.NewValue = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value2));
                    }
                    else
                    {
                        Je.OldValue = Convert.ToString(value1);
                        Je.NewValue = Convert.ToString(value2);
                    }
                    Je.RecordId = Convert.ToInt64(id);
                    db.JournalEntries.Add(Je);
                }
            }
            db.SaveChanges();
        }
        return changes;
    }
    /// <summary>Gets username value for association.</summary>
    ///
    /// <param name="id">        The identifier.</param>
    ///
    /// <returns>The user name for association.</returns>
    
    public static string GetUserNameForAssociation(string id)
    {
        try
        {
            string idvalue = Convert.ToString(id);
            using(var context = (new ApplicationDbContext()))
            {
                var objuser = context.Users.First(p => p.Id == idvalue);
                if(objuser != null)
                    return objuser.UserName;
                else
                    return "";
            }
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Gets display value for association.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="id">        The identifier.</param>
    ///
    /// <returns>The display value for association.</returns>
    
    public static string GetDisplayValueForAssociation(string EntityName, string id)
    {
        try
        {
            // Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                MethodInfo mc = controller.GetMethod("GetDisplayValue");
                if(mc != null)
                {
                    object[] MethodParams = new object[] { id };
                    var obj = mc.Invoke(objController, MethodParams);
                    return Convert.ToString(obj == null ? "" : obj);
                }
                else
                    return "";
            }
        }
        catch
        {
            return id;
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="EntityName"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public static object GetDisplayValuesForPivotMatrix(string EntityName, string caller)
    {
        try
        {
            // Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                MethodInfo mc = controller.GetMethod("GetDisplayValuePivotMatrix");
                object[] MethodParams = new object[] { EntityName, caller };
                var obj = mc.Invoke(objController, MethodParams);
                return obj;
            }
        }
        catch
        {
            return null;
        }
    }
    /// <summary>A T extension method that enumerate property values.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="obj1">        The obj1 to act on.</param>
    /// <param name="EntityName">  Name of the entity.</param>
    /// <param name="excludeProps">The exclude properties.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string EnumeratePropertyValues<T>(this T obj1, string EntityName, string[] excludeProps)
    {
        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        string result = "";
        var style = "background-color: #CC9999; color: black;";
        var entityModel = ModelReflector.Entities;
        foreach(PropertyInfo pi in properties)
        {
            // if (pi.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() > 0) continue;
            if(pi.Name == "Id" || pi.Name == "TenantId" || pi.Name == "ConcurrencyKey" || pi.Name == "DisplayValue" || pi.Name == "m_Timezone" || pi.Name == "m_Timezonecustom" || (obj1.GetType()).GetProperty(pi.Name).PropertyType.Name == "ICollection`1") continue;
            if(pi.Name == "WFeMailNotification" || pi.Name.EndsWith("WorkFlowInstanceId")) continue;
            if(excludeProps != null && excludeProps.Contains(pi.Name)) continue;
            var DisplayName = pi.Name;
            var prefix = "";
            object value1 = (obj1.GetType()).GetProperty(pi.Name).GetValue(obj1, null);
            var entity = entityModel.Where(p => p.Name == EntityName).ToList()[0];
            var assolist = entity.Associations.Where(p => p.AssociationProperty == pi.Name).ToList();
            if(assolist.Count() > 0)
            {
                if(assolist[0].Target == "IdentityUser")
                {
                    try
                    {
                        string idvalue = Convert.ToString(value1);
                        using(var context = (new ApplicationDbContext()))
                        {
                            var objuser = context.Users.First(p => p.Id == idvalue);
                            if(objuser != null)
                                value1 = objuser.UserName;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    value1 = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value1));
                }
                DisplayName = assolist[0].DisplayName;
            }
            else
            {
                var proplist = entity.Properties;
                var prop = proplist.FirstOrDefault(p => p.Name == DisplayName);
                if(prop != null)
                {
                    if(prop.DataTypeAttribute == "Currency")
                        prefix = "$";
                    if(prop.DataTypeAttribute == "DateTime")
                    {
                        //value1 = value1 != null ? ((DateTime)value1).ToShortDateString() : value1;
                        value1 = value1 == null ? value1 : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(value1), (new JournalEntry().m_Timezone));
                    }
                }
                DisplayName = (prop == null ? DisplayName : prop.DisplayName);
            }
            if(value1 != null && !string.IsNullOrEmpty(value1.ToString()))
            {
                if(style == "background-color: #ffffff; color: black;")
                    style = "background-color: #eeeeee; color: black;";
                else style = "background-color: #ffffff; color: black;";
                result += "<tr style=\"" + style + "\"><td width=200>" + (DisplayName + " </td><td> " + prefix + value1.ToString() + "</td></tr>");
            }
        }
        if(!string.IsNullOrEmpty(result))
        {
            var classvalue = " <style>.MailTable {margin: 0px 0px 20px 0px;padding: 0;width: 97%;"
                             + "}.MailTable table { border-collapse: collapse;border-spacing: 0;width: 100%;height: 100%;margin: 0px;padding: 0px;}"
                             + ".MailTable td {vertical-align: middle;border: 1px solid #c1c1c1;border-width: 1px 1px 1px 1px;text-align: left;padding: 5px;font-size: 12px;font-family: Arial;font-weight: normal;color: #333333;}"
                             + ".MailTable td:first-child {font-weight: bold;}</style>";
            return classvalue + " <div class=\"MailTable\">" + "<table>" + result + "</table></div>";
        }
        else
            return result;
    }
}
/// <summary>An apply rule.</summary>
public static class ApplyRule
{
    /// <summary>A T extension method that check rule.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="obj1">      The first object.</param>
    /// <param name="br">        The line break.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool CheckRule<T>(this T obj1, BusinessRule br, string EntityName)
    {
        if(EntityName != br.EntityName) return false;
        var modelentities = ModelReflector.Entities;
        var EntityInfo = modelentities.FirstOrDefault(p => p.Name == EntityName);
        bool result = true;
        bool containsCondition = false;
        string lstConditions = "";
        var ruleconditionsdb = br.ruleconditions.ToList();
        var ruleactions = br.ruleaction.Where(p => p.AssociatedActionTypeID == 1).ToList();
        foreach(var condition in ruleconditionsdb)
        {
            containsCondition = true;
            var PropName = condition.PropertyName;
            var PropNameAssoco = "";
            var secondAssocation = "";
            var ConditionOperator = condition.Operator;
            var ConditionValue = condition.Value;
            var ConditionValueId = condition.Value;
            object oldPropValue = null;
            bool isPropertyValueChanged = true;
            if(br.AssociatedBusinessRuleTypeID == 4)
                isPropertyValueChanged = IsPropertyValueChanged(obj1, EntityName, PropName);
            if(PropName == "Id" && ConditionOperator == ">" && ConditionValue == "0")
            {
                result = true && isPropertyValueChanged;
            }
            else
            {
                if(br.AssociatedBusinessRuleTypeID == 4 && ConditionOperator == "Changes to anything")
                    result = isPropertyValueChanged;
                else
                {
                    string propDataType = "String";
                    var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
                    if(PropInfo != null)
                        propDataType = PropInfo.DataType;
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                    bool? IsAssociation = false;
                    var isUserAssocation = false;
                    if(PropName.StartsWith("[") && PropName.EndsWith("]"))
                    {
                        PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                        if(PropName.Contains("."))
                        {
                            var targetProperties = PropName.Split(".".ToCharArray());
                            if(targetProperties.Length > 1)
                            {
                                AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                if(AssociationInfo != null)
                                {
                                    var targetEntityInfo = modelentities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                    PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                                    propDataType = PropInfo.DataType;
                                    var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                                    PropName = targetProperties[0];
                                    if(assocInfo != null)
                                    {
                                        IsAssociation = true;
                                        propDataType = "String";
                                        PropNameAssoco = targetProperties[1];
                                        secondAssocation = assocInfo.Target;
                                        if(condition.Value.ToLower() == "loggedinuser" && obj1 != null)
                                        {
                                            string EntityNameMain = EntityName;
                                            EntityName = targetEntityInfo.Name;
                                            obj1 = (T)GetOldValueOfUserEntity(obj1, EntityName, PropName, "Association", PropNameAssoco, EntityName);
                                            isUserAssocation = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string targetValue = GetTargetConditionValue(obj1, ConditionValue, EntityName);
                    if(!string.IsNullOrEmpty(targetValue))
                        ConditionValue = targetValue;
                    string DataType = propDataType;
                    PropertyInfo Property = null;
                    if(obj1 != null)
                    {
                        Property = (obj1.GetType()).GetProperty(PropName);
                    }
                    if(condition !=null && condition.Value.ToLower() == "loggedinuser" && obj1 != null)
                    {
                        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        if(!isUserAssocation)
                            PropNameAssoco = PropName;
                        var prop1 = properties.FirstOrDefault(p => p.Name == PropNameAssoco);
                        object PropValue = prop1.GetValue(obj1, null);
                        if(PropValue == null)
                            return false;
                        var itemUserId = PropValue.ToString();
                        using(var context = (new ApplicationDbContext()))
                        {
                            var objuser = context.Users.First(p => p.Id == itemUserId);
                            if(objuser != null)
                            {
                                if(targetValue != null)
                                {
                                    return ValidateValueAgainstRule(objuser.UserName, DataType, ConditionOperator, targetValue, prop1, propDataType);
                                }
                            }
                        }
                        return result;
                    }
                    bool oldvalueflag = true;
                    if(AssociationInfo != null)
                    {
                        if(ConditionValue != null)
                        {
                            long propid;
                            if(Int64.TryParse(ConditionValue, out propid))
                                ConditionValue = GetValueForAssociationProperty(AssociationInfo.Target, Convert.ToString(ConditionValue), PropInfo.Name, IsAssociation);
                        }
                        if(ruleactions != null && ruleactions.Count() > 0)
                        {
                            if(IsAssociation != null && IsAssociation != false)
                            {
                                //obj2 = getrecord(AssociationInfo.Target, Convert.ToString(oldPropValue));
                                object obj2 = GetOldValueOfEntity(obj1, AssociationInfo.Target, PropName, "Association");
                                oldPropValue = GetOldValueOfEntity(obj2, AssociationInfo.Target, PropNameAssoco);
                                oldPropValue = GetDisplayValueForAssociation(secondAssocation, Convert.ToString(oldPropValue));
                            }
                            else
                            {
                                oldPropValue = GetOldValueOfEntity(obj1, EntityName, PropName);
                                oldPropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(oldPropValue));
                            }
                            oldvalueflag = false;
                        }
                        DataType = "Association";
                    }
                    if(ruleactions != null && ruleactions.Count() > 0)
                    {
                        if(isPropertyValueChanged)
                        {
                            if(oldvalueflag)
                                oldPropValue = GetOldValueOfEntity(obj1, EntityName, PropName);
                            bool isRuleValid = ValidateValueAgainstRule(oldPropValue, DataType, ConditionOperator, ConditionValue, Property, propDataType);
                            result = isRuleValid;
                        }
                        else result = false;
                    }
                    else
                    {
                        if(isPropertyValueChanged)
                        {
                            object PropValue = (Property != null) ? Property.GetValue(obj1, null) : null;
                            if(AssociationInfo != null && PropValue != null)
                            {
                                if(IsAssociation != null && IsAssociation != false)
                                {
                                    if(condition.Value.ToLower() == "loggedinuser")
                                    {
                                        object obj2 = GetOldValueOfUserEntity(obj1, AssociationInfo.Target, PropName, "Association", PropNameAssoco);
                                        var oldPropValueforReadOnly = GetOldValueOfEntity(obj2, AssociationInfo.Target, PropNameAssoco);
                                        PropValue = GetDisplayValueForAssociation(secondAssocation, Convert.ToString(oldPropValueforReadOnly));
                                    }
                                    else
                                    {
                                        object obj2 = GetOldValueOfEntity(obj1, AssociationInfo.Target, PropName, "Association");
                                        oldPropValue = GetOldValueOfEntity(obj2, AssociationInfo.Target, PropNameAssoco);
                                        PropValue = GetDisplayValueForAssociation(secondAssocation, Convert.ToString(oldPropValue));
                                    }
                                }
                                else
                                    PropValue = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name); //GetValueForAssociationProperty(AssociationInfo.Target, Convert.ToString(PropValue), PropInfo.Name);
                            }
                            bool isRuleValid = ValidateValueAgainstRule(PropValue, DataType, ConditionOperator, ConditionValue, Property, propDataType);
                            result = isRuleValid;
                        }
                        else result = false;
                    }
                }
                lstConditions += Convert.ToString(result) + " " + condition.LogicalConnector + " ";
            }
        }
        result = CheckRuleWithLogicalConnectors(lstConditions);
        if(!containsCondition)
            return false;
        return result;
    }
    public static bool CheckRule<T>(this T obj1, List<Condition> ruleconditionsdb, string EntityName)
    {
        var modelentities = ModelReflector.Entities;
        var EntityInfo = modelentities.FirstOrDefault(p => p.Name == EntityName);
        bool result = true;
        bool containsCondition = false;
        string lstConditions = "";
        foreach(var condition in ruleconditionsdb)
        {
            containsCondition = true;
            var PropName = condition.PropertyName;
            var ConditionOperator = condition.Operator;
            var ConditionValue = condition.Value;
            object oldPropValue = null;
            bool isPropertyValueChanged = true;
            string propDataType = "String";
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
            if(PropInfo != null)
                propDataType = PropInfo.DataType;
            var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
            bool? IsAssociation = false;
            if(PropName.StartsWith("[") && PropName.EndsWith("]"))
            {
                PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if(PropName.Contains("."))
                {
                    var targetProperties = PropName.Split(".".ToCharArray());
                    if(targetProperties.Length > 1)
                    {
                        AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if(AssociationInfo != null)
                        {
                            var targetEntityInfo = modelentities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            propDataType = PropInfo.DataType;
                            var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                            if(assocInfo != null)
                            {
                                IsAssociation = true;
                                propDataType = "String";
                            }
                            PropName = targetProperties[0];
                        }
                    }
                }
            }
            string targetValue = GetTargetConditionValue(obj1, ConditionValue, EntityName);
            if(!string.IsNullOrEmpty(targetValue))
                ConditionValue = targetValue;
            string DataType = propDataType;
            //PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            //var Property = properties.FirstOrDefault(p => p.Name == PropName);
            var Property = (obj1.GetType()).GetProperty(PropName);
            bool oldvalueflag = true;
            if(AssociationInfo != null)
            {
                if(ConditionValue != null)
                {
                    long propid;
                    if(Int64.TryParse(ConditionValue, out propid))
                        ConditionValue = GetValueForAssociationProperty(AssociationInfo.Target, Convert.ToString(ConditionValue), PropInfo.Name, IsAssociation);
                }
                DataType = "Association";
            }
            object PropValue = (Property != null) ? Property.GetValue(obj1, null) : null;
            if(AssociationInfo != null && PropValue != null)
                PropValue = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name); //GetValueForAssociationProperty(AssociationInfo.Target, Convert.ToString(PropValue), PropInfo.Name);
            bool isRuleValid = ValidateValueAgainstRule(PropValue, DataType, ConditionOperator, ConditionValue, Property, propDataType);
            result = isRuleValid;
            lstConditions += Convert.ToString(result) + " " + condition.LogicalConnector + " ";
        }
        result = CheckRuleWithLogicalConnectors(lstConditions);
        if(!containsCondition)
            return true;
        return result;
    }
    /// <summary>Check rule with logical connectors.</summary>
    ///
    /// <param name="lstConditions">The list conditions.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool CheckRuleWithLogicalConnectors(string lstConditions)
    {
        if(string.IsNullOrEmpty(lstConditions))
            return true;
        System.Data.DataTable table = new System.Data.DataTable();
        table.Columns.Add("", typeof(Boolean));
        lstConditions = lstConditions.Trim();
        if(lstConditions.EndsWith("AND"))
            lstConditions = lstConditions.Remove(lstConditions.LastIndexOf("AND"));
        else if(lstConditions.EndsWith("OR"))
            lstConditions = lstConditions.Remove(lstConditions.LastIndexOf("OR"));
        table.Columns[0].Expression = lstConditions;
        System.Data.DataRow r = table.NewRow();
        table.Rows.Add(r);
        return (Boolean)r[0];
    }
    
    /// <summary>Gets target condition value.</summary>
    ///
    /// <param name="obj1">          The first object.</param>
    /// <param name="targetProperty">Target property.</param>
    /// <param name="EntityName">    Name of the entity.</param>
    ///
    /// <returns>The target condition value.</returns>
    
    public static string GetTargetConditionValue(object obj1, string targetProperty, string EntityName)
    {
        string result = "";
        ModelReflector.Property PropInfo = null;
        string PropName = "";
        string PropName1 = "";
        string targetPropertyName = "";
        var entityModel = ModelReflector.Entities;
        var EntityInfo = entityModel.FirstOrDefault(p => p.Name == EntityName);
        if(targetProperty.StartsWith("[") && targetProperty.EndsWith("]"))
        {
            PropName = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var targetEntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        //var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                        targetPropertyName = targetProperties[0];
                        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                        object PropValue = Property.GetValue(obj1, null);
                        result = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name);// GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                    }
                }
            }
            else
            {
                targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                object PropValue = Property.GetValue(obj1, null);
                result = Convert.ToString(PropValue);
            }
        }
        else if(targetProperty.StartsWith("[") && (targetProperty.ToLower().EndsWith("days") || targetProperty.ToLower().EndsWith("months") || targetProperty.ToLower().EndsWith("weeks") || targetProperty.ToLower().EndsWith("years")))
        {
            var dateValueDMY = "";
            string dateValueDMY1 = "";
            var propNameArr = targetProperty.Split(" ".ToArray());
            PropName = propNameArr[0].TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            PropName1 = propNameArr[2].TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var targetEntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        //var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                        targetPropertyName = targetProperties[0];
                        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                        object PropValue = Property.GetValue(obj1, null);
                        result = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name);// GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                        if(!string.IsNullOrEmpty(result))
                        {
                            DateTime dateValue = Convert.ToDateTime(result);
                            result = dateValue.ToString("MM/dd/yyyy");
                        }
                    }
                }
                if(PropName1.Contains("."))
                {
                    dateValueDMY = result;
                    var targetProperties1 = PropName1.Split(".".ToCharArray());
                    if(targetProperties1.Length > 1)
                    {
                        var AssociationInfo1 = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties1[0]);
                        if(AssociationInfo1 != null)
                        {
                            var targetEntityInfo1 = entityModel.FirstOrDefault(p => p.Name == AssociationInfo1.Target);
                            PropInfo = targetEntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties1[1]);
                            //var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                            targetPropertyName = targetProperties1[0];
                            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                            var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                            object PropValue = Property.GetValue(obj1, null);
                            result = GetValueForAssociationPropertyNew(obj1, AssociationInfo1, Convert.ToString(PropValue), PropInfo.Name);// GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                            if(!string.IsNullOrEmpty(result))
                            {
                                DateTime dateValue = Convert.ToDateTime(result);
                                dateValueDMY1 = dateValue.ToString("MM/dd/yyyy");
                                if(targetProperty.ToLower().EndsWith("days"))
                                {
                                    //result = dateValue.ToString("MM/dd/yyyy");
                                    result = dateValue.Day.ToString();
                                }
                                else if(targetProperty.ToLower().EndsWith("months"))
                                {
                                    result = dateValue.Month.ToString();
                                }
                                else if(targetProperty.ToLower().EndsWith("years"))
                                {
                                    result = dateValue.Year.ToString();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                object PropValue = Property.GetValue(obj1, null);
                DateTime dateValue = Convert.ToDateTime(PropValue);
                result = dateValue.ToString("MM/dd/yyyy");
            }
            var propTarget = targetProperty.Replace("[", "").Replace("]", "").Trim();
            if(!string.IsNullOrEmpty(dateValueDMY1))
            {
                propTarget = propTarget.Replace(PropName, dateValueDMY);
                result = EvaluateDateTime(dateValueDMY1, propTarget.Replace(PropName1, result));
            }
            else
            {
                result = EvaluateDateTime(result, propTarget.Replace(PropName, result));
            }
        }
        else if(targetProperty.Contains("[") && (targetProperty.ToLower().EndsWith("days") || targetProperty.ToLower().EndsWith("months") || targetProperty.ToLower().EndsWith("weeks") || targetProperty.ToLower().EndsWith("years")))
        {
            var dateValueDMY = "";
            var propNameArr = targetProperty.Split(" ".ToArray());
            PropName = propNameArr[2].TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var targetEntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        //var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                        targetPropertyName = targetProperties[0];
                        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                        object PropValue = Property.GetValue(obj1, null);
                        result = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name);// GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                        if(!string.IsNullOrEmpty(result))
                        {
                            DateTime dateValue = Convert.ToDateTime(result);
                            dateValueDMY = dateValue.ToString("MM/dd/yyyy");
                            if(targetProperty.ToLower().EndsWith("days"))
                            {
                                //result = dateValue.ToString("MM/dd/yyyy");
                                result = dateValue.Day.ToString();
                            }
                            else if(targetProperty.ToLower().EndsWith("months"))
                            {
                                result = dateValue.Month.ToString();
                            }
                            else if(targetProperty.ToLower().EndsWith("years"))
                            {
                                result = dateValue.Year.ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                object PropValue = Property.GetValue(obj1, null);
                DateTime dateValue = Convert.ToDateTime(PropValue);
                result = dateValue.ToString("MM/dd/yyyy");
            }
            var propTarget = targetProperty.Replace("[", "").Replace("]", "").Trim();
            result = EvaluateDateTime(dateValueDMY, propTarget.Replace(PropName, result));
        }
        else
        {
            if(targetProperty.ToLower() == "loggedinuser")
            {
                result = System.Web.HttpContext.Current.User.Identity.Name;
            }
        }
        return result;
    }
    
    /// <summary>Gets property change value.</summary>
    ///
    /// <param name="obj1">          The first object.</param>
    /// <param name="targetProperty">Target property.</param>
    /// <param name="EntityName">    Name of the entity.</param>
    ///
    /// <returns>The property change value.</returns>
    
    public static string GetPropertyChangeValue(object obj1, string targetProperty, string EntityName)
    {
        string result = "";
        ModelReflector.Property PropInfo = null;
        string PropName = "";
        string targetPropertyName = "";
        if(targetProperty.StartsWith("[") && targetProperty.EndsWith("]"))
        {
            var entityModel = ModelReflector.Entities;
            var EntityInfo = entityModel.FirstOrDefault(p => p.Name == EntityName);
            PropName = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var targetEntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        //var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                        targetPropertyName = targetProperties[0];
                        PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                        object PropValue = Property.GetValue(obj1, null);
                        result = GetValueForAssociationPropertyNew(obj1, AssociationInfo, Convert.ToString(PropValue), PropInfo.Name);// GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                    }
                }
            }
            else
            {
                targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                object PropValue = Property.GetValue(obj1, null);
                result = Convert.ToString(PropValue);
            }
        }
        else
        {
            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var property = properties.FirstOrDefault(p => p.Name == targetProperty);
            object Value = property.GetValue(obj1, null);
            result = Convert.ToString(Value);
        }
        return result;
    }
    
    /// <summary>Query if 'newentity' is property value changed.</summary>
    ///
    /// <param name="newentity"> The newentity.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="propName">  Name of the property.</param>
    ///
    /// <returns>True if property value changed, false if not.</returns>
    
    public static bool IsPropertyValueChanged(object newentity, string entityName, string propName)
    {
        //old value
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(entityName).controllerType;
        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
        {
            MethodInfo mc = controller.GetMethod("GetRecordById");
            PropertyInfo[] newproperties = (newentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var idProperty = newproperties.FirstOrDefault(p => p.Name == "Id");
            object idPropValue = idProperty.GetValue(newentity, null);
            if(Convert.ToInt64(idPropValue) != 0)
            {
                object newValue = GetPropertyChangeValue(newentity, propName, entityName);
                //var newproperty = newproperties.FirstOrDefault(p => p.Name == propName);
                //object newValue = newproperty.GetValue(newentity, null);
                object[] MethodParams = new object[] { Convert.ToString(idPropValue) };
                var oldentity = mc.Invoke(objController, MethodParams);
                object oldValue = GetPropertyChangeValue(oldentity, propName, entityName);
                //PropertyInfo[] oldproperties = (oldentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                //var oldproperty = oldproperties.FirstOrDefault(p => p.Name == propName);
                //object oldValue = oldproperty.GetValue(oldentity, null);
                if(Convert.ToString(oldValue) == Convert.ToString(newValue))
                    return false;
                else
                    return true;
            }
        }
        return true;
    }
    
    
    /// <summary>Gets old value of entity.</summary>
    ///
    /// <param name="newentity"> The newentity.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="propName">  Name of the property.</param>
    ///
    /// <returns>The old value of entity.</returns>
    
    public static object GetOldValueOfEntity(object newentity, string entityName, string propName, string isAssoc = null)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller, GeneratorBase.MVC.Controllers");
        if(newentity == null)
            return null;
        Type controller = new CreateControllerType(entityName).controllerType;
        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
        {
            MethodInfo mc = controller.GetMethod("GetRecordById");
            PropertyInfo[] newproperties = (newentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var idProperty = newproperties.FirstOrDefault(p => p.Name == "Id");
            if(idProperty == null)
                return null;
            object idPropValue = idProperty.GetValue(newentity, null);
            if(!string.IsNullOrEmpty(isAssoc))
            {
                var newpropertyId = newproperties.FirstOrDefault(p => p.Name == propName);
                var newValueId = newpropertyId.GetValue(newentity, null);
                idPropValue = newValueId;
            }
            if(Convert.ToInt64(idPropValue) != 0)
            {
                var newproperty = newproperties.FirstOrDefault(p => p.Name == propName);
                object newValue = newproperty.GetValue(newentity, null);
                object[] MethodParams = new object[] { Convert.ToString(idPropValue) };
                var oldentity = mc.Invoke(objController, MethodParams);
                if(oldentity == null)
                    return null;
                PropertyInfo[] oldproperties = (oldentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var oldproperty = oldproperties.FirstOrDefault(p => p.Name == propName);
                if(!string.IsNullOrEmpty(isAssoc))
                    return oldentity;
                object oldValue = oldproperty.GetValue(oldentity, null);
                return oldValue;
            }
        }
        return null;
    }
    /// <summary>Gets old value of entity.</summary>
    ///
    /// <param name="newentity"> The newentity.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="propName">  Name of the property.</param>
    ///
    /// <returns>The old value of entity.</returns>
    
    public static object GetOldValueOfUserEntity(object newentity, string entityName, string propName, string isAssoc = null, string propAssco = null, string AsscoEntity = null)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(entityName).controllerType;
        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
        {
            MethodInfo mc = controller.GetMethod("GetRecordById");
            PropertyInfo[] newproperties = (newentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var idProperty = newproperties.FirstOrDefault(p => p.Name == "Id");
            object idPropValue = idProperty.GetValue(newentity, null);
            if(!string.IsNullOrEmpty(isAssoc))
            {
                var newpropertyId = newproperties.FirstOrDefault(p => p.Name == propName);
                if(newpropertyId != null)
                {
                    var newValueId = newpropertyId.GetValue(newentity, null);
                    idPropValue = newValueId;
                }
            }
            if(Convert.ToInt64(idPropValue) != 0)
            {
                var newproperty = newproperties.FirstOrDefault(p => p.Name == propName);
                object newValue = newproperty.GetValue(newentity, null);
                object[] MethodParams = new object[] { Convert.ToString(idPropValue) };
                var oldentity = mc.Invoke(objController, MethodParams);
                if(oldentity != null && AsscoEntity != null)
                {
                    Type controllerAss = new CreateControllerType(AsscoEntity).controllerType;
                    using(var ObjcontrollerAss = (IDisposable)Activator.CreateInstance(controllerAss, null))
                    {
                        MethodInfo mcAss = controllerAss.GetMethod("GetRecordById");
                        object[] MethodParamsAss = new object[] { Convert.ToString(idPropValue) };
                        object result1 = mcAss.Invoke(ObjcontrollerAss, MethodParams);
                        if(!string.IsNullOrEmpty(isAssoc) && result1 != null)
                            oldentity = result1;
                        if(oldentity == null)
                            return null;
                    }
                    PropertyInfo[] oldproperties = (oldentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var oldproperty = oldproperties.FirstOrDefault(p => p.Name == propName);
                    if(!string.IsNullOrEmpty(isAssoc))
                        return oldentity;
                    object oldValue = oldproperty.GetValue(oldentity, null);
                    return oldValue;
                }
            }
        }
        return null;
    }
    /// <summary>Gets display value for association.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="id">        The identifier.</param>
    ///
    /// <returns>The display value for association.</returns>
    
    private static string GetDisplayValueForAssociation(string EntityName, string id)
    {
        try
        {
            //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
            Type controller = new CreateControllerType(EntityName).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                MethodInfo mc = controller.GetMethod("GetDisplayValue");
                object[] MethodParams = new object[] { id };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToString(obj == null ? "" : obj);
            }
        }
        catch
        {
            return id;
        }
    }
    
    /// <summary>Validates the value against rule.</summary>
    ///
    /// <param name="PropValue">    The property value.</param>
    /// <param name="DataType">     Type of the data.</param>
    /// <param name="condition">    The condition.</param>
    /// <param name="value">        The value.</param>
    /// <param name="property">     The property.</param>
    /// <param name="AssocDataType">Type of the associated data.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private static bool ValidateValueAgainstRule(object PropValue, string DataType, string condition, string value, PropertyInfo property, string AssocDataType)
    {
        if(PropValue == null && value.ToLower().Trim() != "null")
            return false;
        if(PropValue != null && string.IsNullOrEmpty(value) && condition == "Changes to anything")
            return false;
        if(value.ToLower().Trim() == "null")
        {
            if(CheckNullCondition(PropValue, value.ToLower().Trim(), condition))
                return true;
            else
                return false;
        }
        bool result = false;
        if(property == null)
            return result;
        Type targetType = property.PropertyType;
        if(property.PropertyType.IsGenericType)
            targetType = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
        if(DataType == "Association")
        {
            if(ValidateValueForAssociation(PropValue, condition, value, AssocDataType))
                return true;
            else
                return false;
        }
        if(DataType == "DateTime")
        {
            if(value.Trim().ToLower() == "today")
                value = Convert.ToString(DateTime.UtcNow.Date);
            else if(value.Trim().ToLower().Contains("days") || value.Trim().ToLower().Contains("weeks")
                    || value.Trim().ToLower().Contains("months") || value.Trim().ToLower().Contains("years"))
            {
                value = EvaluateDateTime(Convert.ToString(PropValue).Trim(), value.Trim());
            }
        }
        try
        {
            PropValue = Convert.ToString(PropValue).Trim().ToLower();
            dynamic value1 = Convert.ChangeType(PropValue, targetType);
            dynamic value2 = Convert.ChangeType(value.Trim().ToLower(), targetType);
            switch(condition)
            {
            case "=":
                if(value1 == value2) return true;
                break;
            case ">":
                if(value1 > value2) return true;
                break;
            case "<":
                if(value1 < value2) return true;
                break;
            case "<=":
                if(value1 <= value2) return true;
                break;
            case ">=":
                if(value1 >= value2) return true;
                break;
            case "!=":
                if(value1 != value2) return true;
                break;
            case "Contains":
                if((Convert.ToString(value1)).Contains(value2.ToString())) return true;
                break;
            case "!Contains":
                if(!(Convert.ToString(value1)).Contains(value2.ToString())) return true;
                break;
            case "Regular Expression":
                if(IsRegularExpressionMatch(value1, value2)) return true;
                break;
            case "Match":
                if(IsRegularExpressionMatch(value1, value2)) return true;
                break;
            }
        }
        catch
        {
            return false;
        }
        return result;
    }
    
    /// <summary>Validates the value for association.</summary>
    ///
    /// <param name="PropValue">    The property value.</param>
    /// <param name="condition">    The condition.</param>
    /// <param name="value">        The value.</param>
    /// <param name="AssocDataType">Type of the associated data.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool ValidateValueForAssociation(object PropValue, string condition, string value, string AssocDataType)
    {
        if(PropValue == null && value.ToLower().Trim() != "null")
            return false;
        if(Convert.ToString(PropValue) == "")
            return false;
        bool flag = false;
        var lstconditions = "";
        //Type targetType = typeof(System.String);
        Type targetType = getTypeForAssocProperty(AssocDataType);
        PropValue = Convert.ToString(PropValue).Trim().ToLower();
        dynamic value1 = Convert.ChangeType(PropValue, targetType);
        dynamic value2 = Convert.ChangeType(value.Trim().ToLower(), targetType);
        //  var arrValue = Convert.ToString(value2).Split("OR".ToCharArray());
        var arrValue = Convert.ToString(value2).Split(new string[] { "OR" }, StringSplitOptions.None);
        foreach(var item in arrValue)
        {
            dynamic itemVal = Convert.ChangeType(item.Trim(), targetType);
            if(!string.IsNullOrEmpty(item))
            {
                switch(condition)
                {
                case "=":
                    if(value1 == itemVal) flag = true;
                    else flag = false;
                    break;
                case ">":
                    if(value1 > itemVal) flag = true;
                    else flag = false;
                    break;
                case "<":
                    if(value1 < itemVal) flag = true;
                    else flag = false;
                    break;
                case "<=":
                    if(value1 <= itemVal) flag = true;
                    else flag = false;
                    break;
                case ">=":
                    if(value1 >= itemVal) flag = true;
                    else flag = false;
                    break;
                case "!=":
                    if(value1 != itemVal) flag = true;
                    else flag = false;
                    break;
                case "Contains":
                    if((Convert.ToString(value1)).Contains(itemVal.ToString())) return true;
                    break;
                case "!Contains":
                    if(!(Convert.ToString(value1)).Contains(value2.ToString())) return true;
                    break;
                case "Regular Expression":
                    if(IsRegularExpressionMatch(value1, itemVal)) return true;
                    break;
                case "Match":
                    if(IsRegularExpressionMatch(value1, value2)) return true;
                    break;
                }
                lstconditions += Convert.ToString(flag) + " OR ";
            }
        }
        if(CheckRuleWithLogicalConnectors(lstconditions))
            return true;
        else
            return false;
    }
    
    /// <summary>Gets type for associated property.</summary>
    ///
    /// <param name="AssocDataType">Type of the associated data.</param>
    ///
    /// <returns>The type for associated property.</returns>
    
    public static Type getTypeForAssocProperty(string AssocDataType)
    {
        if(AssocDataType == "Int32")
            return typeof(System.Int32);
        else if(AssocDataType == "Int64")
            return typeof(System.Int64);
        else if(AssocDataType == "Boolean")
            return typeof(System.Boolean);
        else if(AssocDataType == "Decimal")
            return typeof(System.Decimal);
        else if(AssocDataType == "Double")
            return typeof(System.Double);
        else if(AssocDataType == "DateTime")
            return typeof(System.DateTime);
        return typeof(System.String);
    }
    
    /// <summary>Query if 'value' is regular expression match.</summary>
    ///
    /// <param name="value">     The value.</param>
    /// <param name="expression">The expression.</param>
    ///
    /// <returns>True if regular expression match, false if not.</returns>
    
    public static bool IsRegularExpressionMatch(string value, string expression)
    {
        Regex regex = new Regex(expression.Replace("&#63;", "?").Replace("&#644;", ","));
        var match = regex.Match(value);
        return !(match.Success);
    }
    
    /// <summary>Check null condition.</summary>
    ///
    /// <param name="PropValue">The property value.</param>
    /// <param name="value">    The value.</param>
    /// <param name="condition">The condition.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool CheckNullCondition(object PropValue, string value, string condition)
    {
        if(condition == "=")
        {
            if(PropValue == null || PropValue == "")
                return true;
            else
                return false;
        }
        else if(condition == "!=")
        {
            if(PropValue != null && PropValue != "")
                return true;
            else
                return false;
        }
        else
            return false;
    }
    
    /// <summary>Evaluate date time.</summary>
    ///
    /// <param name="propValue">     The property value.</param>
    /// <param name="conditionValue">The condition value.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string EvaluateDateTime(string propValue, string conditionValue)
    {
        string targetDate = "";
        if(string.IsNullOrEmpty(conditionValue))
        {
            targetDate = propValue;
        }
        else
        {
            var valuearr = conditionValue.Split(' ');
            int count = Convert.ToInt32(valuearr[2]);
            string interval = valuearr[3].ToLower().Trim();
            double noOfDays = count;
            DateTime pValue = new DateTime();
            if(!string.IsNullOrEmpty(propValue))
                pValue = Convert.ToDateTime(propValue);
            DateTime conditionDateValue = DateTime.UtcNow.Date;
            if(valuearr[0].ToLower() != "today")
                if(!string.IsNullOrEmpty(valuearr[0]))
                    conditionDateValue = Convert.ToDateTime(valuearr[0]);
                else
                    conditionDateValue = pValue;
            if(interval == "days")
            {
                if(valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddDays(noOfDays));
                else if(valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.Subtract(TimeSpan.FromDays(noOfDays)));
            }
            else if(interval == "weeks")
            {
                if(valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddDays(noOfDays * 7));
                else if(valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.Subtract(TimeSpan.FromDays(noOfDays * 7)));
            }
            else if(interval == "months")
            {
                if(valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddMonths(Convert.ToInt32(noOfDays)));
                else if(valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.AddMonths(-Convert.ToInt32(noOfDays)));
            }
            else if(interval == "years")
            {
                if(valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddYears(Convert.ToInt32(noOfDays)));
                else if(valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.AddYears(-Convert.ToInt32(noOfDays)));
            }
        }
        return targetDate;
    }
    /// <summary>Evaluate date for action in target On Loading event.</summary>
    ///
    /// <param name="finalvalue">The finalvalue.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string EvaluateDateForActionInTargetOnLoading(string format, string finalvalue, string brType, string parameterName = "")
    {
        string targetDate = "moment(new Date()).format(" + format + ")";
        string calcOperator = " + ";
        if(finalvalue.Contains("-"))
            calcOperator = " - ";
        if((brType == "OnCreate" || brType == "OnEdit") && (finalvalue.ToLower().Contains("days") || finalvalue.ToLower().Contains("weeks") || finalvalue.ToLower().Contains("months") || finalvalue.ToLower().Contains("years")))
        {
            string datecalcvalue = finalvalue.ToLower().Replace("today", "").Replace("days", "").Replace("weeks", "").Replace("months", "").Replace("years", "");
            if(finalvalue.ToLower().Trim().Contains("today"))
            {
                if(finalvalue.ToLower().Contains("days"))
                    targetDate = "moment(new Date().setDate(new Date().getDate()" + datecalcvalue + ")).format(" + format + ")";
                else if(finalvalue.ToLower().Contains("weeks"))
                {
                    datecalcvalue = datecalcvalue.Replace("+", "").Replace("-", "").Trim();
                    var weekdays = Convert.ToInt32(datecalcvalue) * 7;
                    targetDate = "moment(new Date().setDate(new Date().getDate()" + calcOperator + weekdays.ToString() + ")).format(" + format + ")";
                }
                else if(finalvalue.ToLower().Contains("months"))
                    targetDate = "moment(new Date().setMonth(new Date().getMonth()" + datecalcvalue + ")).format(" + format + ")";
                else if(finalvalue.ToLower().Contains("years"))
                    targetDate = "moment(new Date().setYear(new Date().getFullYear()" + datecalcvalue + ")).format(" + format + ")";
            }
            else
            {
                string datecalcvaluenonToday = finalvalue.Replace("[" + parameterName + "]", "").Replace(parameterName, "").ToLower().Replace("days", "").Replace("weeks", "").Replace("months", "").Replace("years", "");
                if(finalvalue.ToLower().Contains("days"))
                    targetDate = "moment(new Date().setDate(new Date(new Date($('#" + parameterName + "').val()).getDate()" + datecalcvaluenonToday + "))).format(" + format + ")";
                else if(finalvalue.ToLower().Contains("weeks"))
                {
                    datecalcvaluenonToday = datecalcvaluenonToday.Replace("+", "").Replace("-", "").Trim();
                    var weekdays = Convert.ToInt32(datecalcvaluenonToday) * 7;
                    targetDate = "moment(new Date().setDate(new Date(new Date($('#" + parameterName + "').val()).getDate()" + calcOperator + weekdays.ToString() + "))).format(" + format + ")";
                }
                else if(finalvalue.ToLower().Contains("months"))
                    targetDate = "moment(new Date().setMonth(new Date(new Date($('#" + parameterName + "').val()).getMonth()" + datecalcvaluenonToday + "))).format(" + format + ")";
                else if(finalvalue.ToLower().Contains("years"))
                    targetDate = "moment(new Date().setYear(new Date(new Date($('#" + parameterName + "').val()).getFullYear()" + datecalcvaluenonToday + "))).format(" + format + ")";
            }
            return targetDate;
        }
        else
            return targetDate;
    }
    /// <summary>Evaluate date for action in target.</summary>
    ///
    /// <param name="finalvalue">The finalvalue.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string EvaluateDateForActionInTarget(string finalvalue)
    {
        string targetDate = "";
        //string currentDate = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
        string currentDate = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).ToString("MM/dd/yyyy");
        string strFinalValue = Convert.ToString(finalvalue).ToLower();
        if(Convert.ToString(finalvalue).ToLower() == "today")
            targetDate = currentDate;
        else if(Convert.ToString(finalvalue).ToLower().Contains("today"))
        {
            string strDuration = strFinalValue.Substring("today".Length);
            targetDate = ApplyRule.EvaluateDateTime(currentDate, strDuration);
        }
        else
        {
            DateTime dateTimeTemp;
            if(DateTime.TryParseExact(strFinalValue, "hh:mm tt", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out dateTimeTemp))
            {
                targetDate = strFinalValue;
            }
            else
            {
                string[] strarr = strFinalValue.Split(' ');
                string strDuration = strFinalValue.Substring(strarr[0].Length);
                targetDate = ApplyRule.EvaluateDateTime(strarr[0], strDuration);
            }
        }
        return targetDate;
    }
    
    /// <summary>Evaluate date for action in target.</summary>
    ///
    /// <param name="finalvalue">The finalvalue.</param>
    /// <param name="Format">    Describes the format to use.</param>
    /// <param name="isutc">     (Optional) True to isutc.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string EvaluateDateForActionInTarget(string finalvalue, string Format, bool isutc = false)
    {
        string targetDate = "";
        string currentDate = DateTime.UtcNow.ToString(Format);
        if(isutc)
            currentDate = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).ToString(Format);
        string strFinalValue = Convert.ToString(finalvalue).ToLower();
        if(Convert.ToString(finalvalue).ToLower() == "today")
            targetDate = currentDate;
        else if(Convert.ToString(finalvalue).ToLower().Contains("today"))
        {
            string strDuration = strFinalValue.Substring("today".Length);
            targetDate = ApplyRule.EvaluateDateTime(currentDate, strDuration);
        }
        else
        {
            DateTime dateTimeTemp;
            if(DateTime.TryParseExact(strFinalValue, "hh:mm tt", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out dateTimeTemp))
            {
                targetDate = strFinalValue;
            }
            else
            {
                string[] strarr = strFinalValue.Split(' ');
                string strDuration = strFinalValue.Substring(strarr[0].Length);
                targetDate = ApplyRule.EvaluateDateTime(strarr[0], strDuration);
            }
        }
        return targetDate;
    }
    
    /// <summary>Evaluate date for action in target.</summary>
    ///
    /// <param name="finalvalue">The finalvalue.</param>
    /// <param name="Format">    Describes the format to use.</param>
    /// <param name="isutc">     (Optional) True to isutc.</param>
    /// <param name="isonsaving">     (Optional) false onsaving event.</param>
    /// <returns>A string.</returns>
    
    public static string EvaluateDateForActionInTarget(string finalvalue, string Format, bool isutc = false, bool isonsaving = false)
    {
        string targetDate = "";
        string currentDate = DateTime.UtcNow.ToString(Format);
        //ldt customization - to fix dateconversion to utc
        if(Format == "MM/dd/yyyy")
            currentDate = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).ToString(Format);
        DateTime localDateTime = new DateTime();
        if(isutc)
        {
            localDateTime = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone));
            currentDate = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).ToString(Format);
        }
        if(isonsaving)
            currentDate = (TimeZoneInfo.ConvertTimeToUtc(localDateTime, (new JournalEntry()).m_Timezone)).ToString(Format);
        string strFinalValue = Convert.ToString(finalvalue).ToLower();
        if(Convert.ToString(finalvalue).ToLower() == "today")
            targetDate = currentDate;
        else if(Convert.ToString(finalvalue).ToLower().Contains("today"))
        {
            string strDuration = strFinalValue.Substring("today".Length);
            targetDate = ApplyRule.EvaluateDateTime(currentDate, strDuration);
        }
        else
        {
            DateTime dateTimeTemp;
            if(DateTime.TryParseExact(strFinalValue, "hh:mm tt", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out dateTimeTemp))
            {
                targetDate = strFinalValue;
            }
            else
            {
                string[] strarr = strFinalValue.Split(' ');
                string strDuration = strFinalValue.Substring(strarr[0].Length);
                targetDate = ApplyRule.EvaluateDateTime(strarr[0], strDuration);
            }
        }
        return targetDate;
    }
    
    /// <summary>Gets value for association property.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="id">        The identifier.</param>
    /// <param name="propName">  Name of the property.</param>
    /// <param name="isassociation"> Is association property.</param>
    /// <returns>The value for association property.</returns>
    
    private static string GetValueForAssociationProperty(string EntityName, string id, string propName, bool? isassociation = null)
    {
        try
        {
            object obj = null;
            if(isassociation == null || isassociation.Value == false)
            {
                // Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
                Type controller = new CreateControllerType(EntityName).controllerType;
                using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                {
                    MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                    object[] MethodParams = new object[] { Convert.ToInt64(id), propName };
                    obj = mc.Invoke(objController, MethodParams);
                }
            }
            var entityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            var assocInfo = entityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propName);
            if(assocInfo != null)
            {
                EntityName = assocInfo.Target;
                propName = "DisplayValue";
                //Type controllerAssoc = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
                Type controllerAssoc = new CreateControllerType(EntityName).controllerType;
                using(var objControllerAssoc = (IDisposable)Activator.CreateInstance(controllerAssoc, null))
                {
                    MethodInfo mcAssoc = controllerAssoc.GetMethod("GetFieldValueByEntityId");
                    object[] MethodParamsAssoc = new object[] { isassociation != null && isassociation.Value ? Convert.ToInt64(id) : Convert.ToInt64(obj), propName };
                    obj = mcAssoc.Invoke(objControllerAssoc, MethodParamsAssoc);
                }
            }
            return Convert.ToString(obj == null ? id : obj);
        }
        catch
        {
            return id;
        }
    }
    private static string GetValueForAssociationPropertyNew(object obj1, ModelReflector.Association AssociationInfo, string id, string propName, bool? isassociation = null)
    {
        try
        {
            string EntityName = AssociationInfo.Target;
            object obj = null;
            if(isassociation == null || isassociation.Value == false)
            {
                var flagfromDbContext = true;
                var navigationproperty = (obj1.GetType()).GetProperty(AssociationInfo.Name.ToLower());
                if(navigationproperty != null)
                {
                    var navigationobject = navigationproperty.GetValue(obj1, null);
                    if(navigationobject != null)
                    {
                        obj = navigationobject.GetType().GetProperty(propName).GetValue(navigationobject, null);
                        flagfromDbContext = false;
                    }
                }
                if(flagfromDbContext)
                {
                    //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
                    Type controller = new CreateControllerType(EntityName).controllerType;
                    using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                    {
                        MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                        object[] MethodParams = new object[] { Convert.ToInt64(id), propName };
                        obj = mc.Invoke(objController, MethodParams);
                        if(navigationproperty != null)
                            return Convert.ToString(obj == null ? "" : obj);
                    }
                }
            }
            var entityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            var assocInfo = entityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propName);
            if(assocInfo != null)
            {
                EntityName = assocInfo.Target;
                propName = "DisplayValue";
                var flagfromDbContext = true;
                var navigationproperty = (obj1.GetType()).GetProperty(AssociationInfo.Name.ToLower());
                if(navigationproperty != null)
                {
                    var navigationobject = navigationproperty.GetValue(obj1, null);
                    if(navigationobject != null)
                    {
                        obj = navigationobject.GetType().GetProperty(propName).GetValue(navigationobject, null);
                        flagfromDbContext = false;
                    }
                }
                if(flagfromDbContext)
                {
                    //Type controllerAssoc = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller, GeneratorBase.MVC.Controllers");
                    Type controllerAssoc = new CreateControllerType(EntityName).controllerType;
                    using(var objControllerAssoc = (IDisposable)Activator.CreateInstance(controllerAssoc, null))
                    {
                        MethodInfo mcAssoc = controllerAssoc.GetMethod("GetFieldValueByEntityId");
                        object[] MethodParamsAssoc = new object[] { isassociation != null && isassociation.Value ? Convert.ToInt64(id) : Convert.ToInt64(obj), propName };
                        obj = mcAssoc.Invoke(objControllerAssoc, MethodParamsAssoc);
                    }
                }
            }
            return Convert.ToString(obj == null ? id : obj);
        }
        catch
        {
            return id;
        }
    }
}
public class VerbInformationDetails
{
    public VerbInformationDetails()  // Instance constructor.
    {
    }
    public VerbInformationDetails(long id, string verbname, string name, long typeId)  // Instance constructor.
    {
        this.Id = id;
        this.VerbName = verbname;
        this.Name = name;
        this.VerbTypeID = typeId;
    }
    public Nullable<Int64> Id
    {
        get;
        set;
    }
    public string VerbName
    {
        get;
        set;
    }
    public string Name
    {
        get;
        set;
    }
    public Nullable<Int64> VerbTypeID
    {
        get;
        set;
    }
    
}
}