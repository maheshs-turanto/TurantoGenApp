using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
/// <summary>A search helper.</summary>
public class SearchHelper
{
    /// <summary>Gets data type of property.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="valueType">   (Optional) True to value type.</param>
    ///
    /// <returns>The data type of property.</returns>
    
    public static List<string> GetDataTypeOfProperty(string entityName, string propertyName, bool valueType = false)
    {
        var listString = new List<string>();
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var entityModel = ModelReflector.Entities;
        var EntityInfo = entityModel.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        ModelReflector.Property targetPropInfo = null;
        var associatedprop = string.Empty;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        associatedprop = AssociationInfo.Name.ToLower() + "." + PropInfo.Name;
                    }
                }
            }
        }
        string DataType = string.Empty;
        if(valueType)
            DataType = "dynamic";
        else
            DataType = PropInfo.DataType;
        listString.Add(DataType);
        if(AssociationInfo != null)
            listString.Add(associatedprop);
        else
            listString.Add(propertyName);
        return listString;
    }
    
    /// <summary>Gets property dp.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>The property dp.</returns>
    
    public static string GetPropertyDP(string entityName, string propertyName)
    {
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        ModelReflector.Property targetPropInfo = null;
        var associatedprop = string.Empty;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        associatedprop = "[" + AssociationInfo.DisplayName + "." + PropInfo.DisplayName + "]";
                    }
                }
            }
        }
        string PropertyName = string.Empty;
        if(AssociationInfo != null)
            PropertyName = associatedprop;
        else
            PropertyName = PropInfo.DisplayName;
        return PropertyName;
    }
    
    /// <summary>Condition f search.</summary>
    ///
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="property">  The property.</param>
    /// <param name="operators"> The operators.</param>
    /// <param name="value">     The value.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string conditionFSearch(string entityName, string property, string operators, string value,string type=null)
    {
        string expression = string.Empty;
        var PrpertyType = GetDataTypeOfProperty(entityName, property);
        var dataType = PrpertyType[0];
        property = PrpertyType[1];
        if(value.StartsWith("[") && value.EndsWith("]"))
        {
            var ValueType = GetDataTypeOfProperty(entityName, value, true);
            if(ValueType != null && ValueType[0] == "dynamic")
            {
                dataType = ValueType[0];
                value = ValueType[1];
            }
        }
        if(value.ToLower().Trim() == "null")
        {
            expression = string.Format(" " + property + " " + operators + " {0}", "null");
            return expression;
        }
        switch(dataType)
        {
        case "Int32":
        case "Int64":
        case "Double":
        case "Boolean":
        case "Decimal":
        {
            if(type != null && dataType == "Int64" && property.Contains(".") && type.ToLower() == "Constant".ToLower())
            {
                var parm = property.Split(".".ToCharArray());
                var p1 = parm[0];
                var p2 = parm[1].Remove(parm[1].Length - 2).ToLower();
                if(operators == "=")
                {
                    expression = string.Format("(" + p1 + "." + p2 + ".DisplayValue =" + operators + " {0}", "\"" + value + "\")");
                }
                else
                    expression = string.Format("(" + p1 + "." + p2 + ".DisplayValue" + " " + operators + " {0}", "\"" + value + "\")");
            }
            else
                expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        case "DateTime":
        {
            if(value.Trim().ToLower() == "today" || value.Trim().ToLower().Contains("today"))
            {
                if(value.Trim().ToLower() == "today")
                    expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new JournalEntry()).m_Timezone)).Date);
                else
                {
                    var param = value.Split(" ".ToCharArray());
                    var op = param[1];
                    var noOfDays = Convert.ToInt32(param[2]);
                    var interval = param[3].ToLower();
                    if(interval == "days")
                    {
                        if(op == "+")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(noOfDays), (new JournalEntry()).m_Timezone)).Date);
                        else if(op == "-")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Subtract(TimeSpan.FromDays(noOfDays)), (new JournalEntry()).m_Timezone)).Date);
                    }
                    else if(op == "weeks")
                    {
                        if(op == "+")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(noOfDays * 7), (new JournalEntry()).m_Timezone)).Date);
                        else if(op == "-")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Subtract(TimeSpan.FromDays(noOfDays * 7)), (new JournalEntry()).m_Timezone)).Date);
                    }
                    else if(interval == "months")
                    {
                        if(op == "+")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(noOfDays), (new JournalEntry()).m_Timezone)).Date);
                        else if(op == "-")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(-noOfDays), (new JournalEntry()).m_Timezone)).Date);
                    }
                    else if(interval == "years")
                    {
                        if(op == "+")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(noOfDays), (new JournalEntry()).m_Timezone)).Date);
                        else if(op == "-")
                            expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(-noOfDays), (new JournalEntry()).m_Timezone)).Date);
                    }
                }
            }
            else
            {
                DateTime val = (TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(value), (new JournalEntry()).m_Timezone));
                expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
            }
            break;
        }
        case "Text":
        case "String":
        {
            if(operators.ToLower() == "contains")
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
}
}