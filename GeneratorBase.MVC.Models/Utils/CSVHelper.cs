using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GeneratorBase.MVC
{
/// <summary>Encapsulates the result of a csv.</summary>
///
/// <typeparam name="T">Generic type parameter.</typeparam>

public class CsvResult<T> : FileResult
{
    /// <summary>The list.</summary>
    private readonly IList<T> _list;
    /// <summary>List of names of the properties.</summary>
    private readonly string[] _propertyNames;
    /// <summary>Application User.</summary>
    private readonly IUser _user;
    /// <summary> Not include in import</summary>
    private readonly string[] _excludedproperties;
    /// <summary>The separator.</summary>
    private readonly char _separator;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="list">            The list.</param>
    /// <param name="fileDownloadName">Name of the file download.</param>
    /// <param name="propertyNames">   List of names of the properties.</param>
    /// <param name="user">   Application User.</param>
    /// <param name="excludedproperties">  Not include in import</param>
    /// <param name="separator">       (Optional) The separator.</param>
    ///
    public CsvResult(IList<T> list,
                     string fileDownloadName,
                     string[] propertyNames,
                     IUser user = null,
                     string[] excludedproperties = null,
                     char separator = ',')
        : base("text/csv")
    {
        _list = list;
        FileDownloadName = fileDownloadName;
        _propertyNames = propertyNames;
        _excludedproperties = excludedproperties;
        _user = user;
        _separator = separator;
    }
    
    /// <summary>Writes the file to the response.</summary>
    ///
    /// <param name="response">The response.</param>
    
    protected override void WriteFile(HttpResponseBase response)
    {
        var outputStream = response.OutputStream;
        using(var memoryStream = new MemoryStream())
        {
            WriteList(memoryStream);
            outputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
    }
    /// <summary>Writes a list.</summary>
    ///
    /// <param name="stream">The stream.</param>
    private void WriteList(Stream stream)
    {
        var streamWriter = new StreamWriter(stream, Encoding.Default);
        WriteHeaderLine(streamWriter);
        streamWriter.WriteLine();
        WriteDataLines(streamWriter);
        streamWriter.Flush();
    }
    /// <summary>Member list.</summary>
    ///
    /// <returns>A List&lt;MemberInfo&gt;</returns>
    private List<PropertyInfo> MemberList()
    {
        var members = new List<PropertyInfo>();
        var association = ModelReflector.Entities.FirstOrDefault(p => p.Name == typeof(T).Name).Associations;
        //Get all the properties
        var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => _user.CanView(typeof(T).Name, prop.Name) && prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(false) != null && prop.PropertyType.Name != "ICollection`1" && prop.Name != "ExportDataLogId" && prop.Name != "IsDeleted" && prop.Name != "DeleteDateTime" && prop.Name != "TenantId" && prop.Name != "ConcurrencyKey" && prop.Name != "Id" && prop.Name != "m_Timezone" && prop.Name != "m_Timezonecustom" && prop.Name != "DisplayValue").ToArray().ToList();
        if(_excludedproperties != null)
        {
            foreach(var columnName in _excludedproperties)
            {
                var column = Props.FirstOrDefault(fd => fd.Name == columnName);
                if(column != null)
                    Props.Remove(column);
            }
        }
        foreach(var columnName in _propertyNames)
        {
            var column = Props.FirstOrDefault(fd => fd.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName == columnName);
            if(column != null)
            {
                members.Add(column);
                Props.Remove(column);
            }
        }
        foreach(var column in Props)
        {
            members.Add(column);
        }
        return members;
    }
    /// <summary>Writes a header line.</summary>
    ///
    /// <param name="streamWriter">The stream writer.</param>
    
    private void WriteHeaderLine(StreamWriter streamWriter)
    {
        var members = MemberList();
        foreach(MemberInfo member in members)
        {
            WriteValue(streamWriter, member.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName);
        }
    }
    
    /// <summary>Writes a data lines.</summary>
    ///
    /// <param name="streamWriter">The stream writer.</param>
    
    private void WriteDataLines(StreamWriter streamWriter)
    {
        var entityfields = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == typeof(T).Name);
        var members = MemberList();
        foreach(T line in _list)
        {
            foreach(MemberInfo member in members)
            {
                var assoInfo = entityfields.Associations.FirstOrDefault(fd => fd.AssociationProperty == member.Name);
                if(assoInfo != null)
                {
                    WriteValue(streamWriter, EntityComparer.GetDisplayValueForAssociation(assoInfo.Target, GetPropertyValue(line, member.Name)));
                }
                else
                {
                    var attr = member.CustomAttributes.Where(a => a.AttributeType.Name == "PropertyTypeForEntity").FirstOrDefault();
                    if(attr != null)
                    {
                        var tempCsd = attr.ConstructorArguments.Where(arg => arg.Value != null);
                        var tempcata = tempCsd.Where(arg => arg.Value.ToString() == "Encrypted");
                        if(tempcata.Count() > 0)
                        {
                            WriteValue(streamWriter, "***");
                        }
                        else
                        {
                            WriteValue(streamWriter, GetPropertyValue(line, member.Name));
                        }
                    }
                    else
                    {
                        WriteValue(streamWriter, GetPropertyValue(line, member.Name));
                    }
                }
            }
            streamWriter.WriteLine();
        }
    }
    
    /// <summary>Writes a value.</summary>
    ///
    /// <param name="writer">The writer.</param>
    /// <param name="value"> The value.</param>
    
    private void WriteValue(StreamWriter writer, String value)
    {
        writer.Write("\"");
        writer.Write(value.Replace("\"", "\"\""));
        writer.Write("\"" + _separator);
    }
    
    /// <summary>Gets property value.</summary>
    ///
    /// <param name="src">     Source for the.</param>
    /// <param name="propName">Name of the property.</param>
    ///
    /// <returns>The property value.</returns>
    
    public static string GetPropertyValue(object src, string propName)
    {
        var type = src.GetType();
        var property = type.GetProperty(propName);
        var value = property.GetValue(src, null);
        var propdatatype = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.DataTypeAttribute>(false);
        if(value != null && propdatatype != null && propdatatype.DataType == System.ComponentModel.DataAnnotations.DataType.Date)
            value = Convert.ToDateTime(value).ToShortDateString();
        if(value != null && propdatatype != null && propdatatype.DataType == System.ComponentModel.DataAnnotations.DataType.Time)
            value = Convert.ToDateTime(value).ToShortTimeString();
        return value == null ? "" : value.ToString();
    }
}
}