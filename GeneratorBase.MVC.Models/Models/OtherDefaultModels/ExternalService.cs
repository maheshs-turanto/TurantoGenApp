using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
namespace GeneratorBase.MVC.Models
{
/// <summary>A schema generator for json (external web-api and internal api helper).</summary>
public class SchemaGeneratorForJson
{
    /// <summary>Generates.</summary>
    ///
    /// <param name="jsonStr">  The JSON string.</param>
    /// <param name="roots">    The roots.</param>
    /// <param name="tablename">The tablename.</param>
    
    public void Generate(string jsonStr, List<string> roots, string tablename)
    {
        JsonValue parsedJsonObject = JsonObject.Parse(jsonStr);
        switch(parsedJsonObject.JsonType)
        {
        case JsonType.String:
        case JsonType.Number:
        case JsonType.Boolean:
            //JSon properties, get the value by converting it to string
            string value = Convert.ToString(parsedJsonObject.ToString());
            break;
        case JsonType.Array:
            JsonArray jArray = parsedJsonObject as JsonArray;
            // DataTable dt = new DataTable();
            for(int index = 0; index < jArray.Count; ++index)
            {
                JsonValue jArrayItem = jArray[index];
                Generate(jArrayItem.ToString(), roots, tablename);
                break;
            } // ds.Tables.Add(dt);
            break;
        case JsonType.Object:
            JsonObject jObject = parsedJsonObject as JsonObject;
            foreach(string key in jObject.Keys)
            {
                if(roots.Contains(key))
                {
                    JsonValue jSubObject = jObject[key];
                    // ds.Tables.Add(dt);
                    // entities.Add(new SchemaEntity { Name = key });
                    Generate(jSubObject.ToString(), roots, key);
                }
                else
                {
                    if(!string.IsNullOrEmpty(tablename)) //&& entities.FirstOrDefault(p => p.Name == tablename) == null)
                    {
                        // DataTable dt1 = new DataTable(tablename);
                        // ds.Tables.Add(dt1);
                        //  entities.Add(new SchemaEntity { Name = tablename });
                    }
                    else if(string.IsNullOrEmpty(tablename))
                    {
                        //  DataTable dt1 = new DataTable("External Integration Service");
                        //ds.Tables.Add(dt1);
                        tablename = "External Integration Service";
                        //entities.Add(new SchemaEntity { Name = "External Integration Service" });
                    }
                    //  var ent = entities.FirstOrDefault(p => p.Name == tablename);
                    // dt.Columns.Add(key);
                    //Now recursively parse, each usb item. i.e jSubObject
                }
            }
            break;
        }
    }
}
/// <summary>A service for accessing externals information.</summary>
public static class ExternalService
{
    private static readonly TaskFactory _taskFactory = new
    TaskFactory(System.Threading.CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
    public async static Task<T> ExecuteAPIGetRequestAsync<T>(string EntityName, Dictionary<string, string> parameters) where T : class
    {
        var context = System.Web.HttpContext.Current;
        try
        {
            return getEntityData<T>(EntityName, "JSON", parameters);
        }
        catch(Exception ex)
        {
            if(context != null) Elmah.ErrorSignal.FromContext(context).Raise(ex);
            else Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            return default(T);
        }
    }
    /// <summary>Executes a pi get request operation.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="parameters">Options for controlling the operation.</param>
    ///
    /// <returns>An asynchronous result that yields a T.</returns>
    public static T ExecuteAPIGetRequest<T>(string EntityName, Dictionary<string, string> parameters) where T : class
    {
        Func<Task<T>> func = () => ExecuteAPIGetRequestAsync<T>(EntityName, parameters);
        var rawTask = _taskFactory.StartNew(func).Unwrap();
        return rawTask.ConfigureAwait(false).GetAwaiter().GetResult();
    }
    
    /// <summary>Executes a pi delete request operation.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="url">       URL of the resource.</param>
    /// <param name="obj">       The object.</param>
    /// <param name="parameters">Options for controlling the operation.</param>
    ///
    /// <returns>An asynchronous result that yields true if it succeeds, false if it fails.</returns>
    
    public async static Task<bool> ExecuteAPIDeleteRequest<T>(string url, object obj, Dictionary<string, string> parameters)
    {
        //Todo: delete item
        return false;
    }
    
    /// <summary>Executes a pi post request operation.</summary>
    ///
    /// <param name="EntityName">  Name of the entity.</param>
    /// <param name="obj">         The object.</param>
    /// <param name="parameters">  Options for controlling the operation.</param>
    /// <param name="dataSourceId">Identifier for the data source.</param>
    ///
    /// <returns>An asynchronous result that yields true if it succeeds, false if it fails.</returns>
    
    public async static Task<bool> ExecuteAPIPostRequest(string EntityName, object obj, Dictionary<string, string> parameters, long dataSourceId)
    {
        var url = getEntityDataSource(EntityName, "JSON", "POST");
        var EntityDatasource = (new ApplicationContext(new SystemUser())).EntityDataSources.FirstOrDefault(p => p.Id == dataSourceId);
        if(string.IsNullOrEmpty(url))
            return false;
        ServicePointManager.ServerCertificateValidationCallback = delegate
        {
            return true;
        };
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
        IDictionary<string, string> values = obj.ToDictionary(EntityName, "JSON");
        foreach(var param in EntityDatasource.entitydatasourceparameters.Where(p => !p.flag.HasValue || !p.flag.Value).ToList())
            client.DefaultRequestHeaders.Add(param.ArgumentName, param.ArgumentValue);
        var content = new FormUrlEncodedContent(values);
        var response = client.PostAsync(url, content).Result;
        response.EnsureSuccessStatusCode();
        return false;
    }
    
    /// <summary>Executes a pi post request operation.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="obj">       The object.</param>
    /// <param name="parameters">Options for controlling the operation.</param>
    ///
    /// <returns>An asynchronous result that yields true if it succeeds, false if it fails.</returns>
    
    public async static Task<bool> ExecuteAPIPostRequest<T>(string EntityName, object obj, Dictionary<string, string> parameters)
    {
        var url = getEntityDataSource(EntityName, "JSON", "POST");
        var EntityDatasource = (new ApplicationContext(new SystemUser())).EntityDataSources.FirstOrDefault(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == "JSON" && p.MethodType.ToUpper() == "POST");
        if(string.IsNullOrEmpty(url))
            return false;
        ServicePointManager.ServerCertificateValidationCallback = delegate
        {
            return true;
        };
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
        IDictionary<string, string> values = obj.ToDictionary(EntityName, "JSON");
        foreach(var param in EntityDatasource.entitydatasourceparameters.Where(p => !p.flag.HasValue || !p.flag.Value).ToList())
            client.DefaultRequestHeaders.Add(param.ArgumentName, param.ArgumentValue);
        var content = new FormUrlEncodedContent(values);
        var response = client.PostAsync(url, content).Result;
        response.EnsureSuccessStatusCode();
        return false;
    }
    
    /// <summary>JSON value after mapping.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="data">      The data.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="methodType">Type of the method.</param>
    ///
    /// <returns>An object.</returns>
    
    private static object JsonValueAfterMapping<T>(string EntityName, string data, string sourceType, string methodType)
    {
        Dictionary<string, string> mapping = new Dictionary<string, string>();
        var result = string.Empty;
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == sourceType);
        if(EntityDatasource != null)
        {
            var propMapping  = EntityDatasource.entitypropertymapping.Where(p=>!string.IsNullOrEmpty(p.PropertyName) && !string.IsNullOrEmpty(p.DataName));
            var IdProp = propMapping.FirstOrDefault(p=>p.PropertyName.ToUpper() == "ID");
            foreach(var map in propMapping)
            {
                if(map.PropertyName.ToUpper() != "ID" && (IdProp != null && map.DataName == IdProp.DataName)) continue;
                mapping.Add(map.PropertyName, map.DataName);
            }
        }
        var settings = new JsonSerializerSettings();
        settings.ContractResolver = new CustomContractResolver(mapping);
        return JsonConvert.DeserializeObject<T>(data, settings);
    }
    
    /// <summary>JSON property mapping.</summary>
    ///
    /// <param name="JsonData">  Information describing the JSON.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="methodType">Type of the method.</param>
    ///
    /// <returns>A string.</returns>
    
    private static string JsonPropertyMapping(string JsonData, string EntityName, string sourceType, string methodType)
    {
        var result = string.Empty;
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == sourceType);
        if(EntityDatasource == null)
            return string.Empty;
        else
        {
            foreach(var map in EntityDatasource.entitypropertymapping)
            {
                if(methodType == "GET")
                    JsonData = JsonData.Replace("\"" + map.DataName + "\"", "\"" + map.PropertyName + "\"");
                else
                    JsonData = JsonData.Replace("\"" + map.PropertyName + "\"", "\"" + map.DataName + "\"");
            }
        }
        result = JsonData;
        return result;
    }
    
    /// <summary>Gets entity data.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="parameters">(Optional) Options for controlling the operation.</param>
    ///
    /// <returns>The entity data.</returns>
    
    private static T getEntityData<T>(string EntityName, string sourceType, Dictionary<string, string> parameters = null)
    {
        using(ApplicationContext db = new ApplicationContext(new SystemUser()))
        {
            var EntityDatasourceList = db.EntityDataSources.Where(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == sourceType && p.MethodType.ToUpper() == "GET").ToList();
            EntityDataSource datasource = null;
            bool flag = false;
            string itemvalue = "";
            string url = string.Empty;
            if(parameters != null && parameters.Count() > 0)
            {
                foreach(var item in parameters)
                {
                    itemvalue = item.Value;
                }
                datasource = EntityDatasourceList.FirstOrDefault(p => p.Action.ToUpper() == "GETITEM");
                if(datasource != null)
                {
                    url = datasource.DataSource;
                    foreach(var item in parameters)
                    {
                        url += "/" + item.Value;// +")";
                    }
                }
                else
                {
                    flag = true;
                    datasource = EntityDatasourceList.FirstOrDefault(p => p.Action.ToUpper() == "GETLIST");
                    if(datasource == null) url = string.Empty;
                    else url = datasource.DataSource;
                }
            }
            else
            {
                datasource = EntityDatasourceList.FirstOrDefault(p => p.Action.ToUpper() == "GETLIST");
                if(datasource == null) url = string.Empty;
                else url = datasource.DataSource;
            }
            if(datasource == null || string.IsNullOrEmpty(url))
                return default(T);
            else
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.UseDefaultCredentials = true;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; // to work with https request
                var paramList = datasource.entitydatasourceparameters.Where(p => !p.flag.HasValue || !p.flag.Value).ToList();
                foreach(var param in paramList)
                    req.Headers.Add(param.ArgumentName + ":" + param.ArgumentValue);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var result = sr.ReadToEnd().Trim();
                JsonValue parsedJsonObject = JsonObject.Parse(result);
                if(!string.IsNullOrEmpty(datasource.RootNode))
                    parsedJsonObject = parsedJsonObject[datasource.RootNode];
                if(flag)
                {
                    List<T> obj1 = (List<T>)JsonValueAfterMapping<List<T>>(EntityName, parsedJsonObject.ToString(), "JSON", "GET");
                    return obj1.Where("Id=" + itemvalue).FirstOrDefault();
                }
                else
                {
                    var obj = JsonValueAfterMapping<T>(EntityName, parsedJsonObject.ToString(), "JSON", "GET");
                    return (T)obj;
                }
            }
        }
        return default(T);
    }
    
    /// <summary>Gets entity data source.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="methodType">Type of the method.</param>
    ///
    /// <returns>The entity data source.</returns>
    
    private static string getEntityDataSource(string EntityName, string sourceType, string methodType)
    {
        var result = string.Empty;
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == sourceType && p.MethodType == methodType);
        if(EntityDatasource == null)
            return string.Empty;
        else
            result = EntityDatasource.DataSource;
        return result;
    }
}
/// <summary>An object to dictionary helper.</summary>
public static class ObjectToDictionaryHelper
{
    /// <summary>An object extension method that converts this object to a dictionary.</summary>
    ///
    /// <param name="source">    Source for the.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="sourceType">Type of the source.</param>
    ///
    /// <returns>The given data converted to an IDictionary&lt;string,string&gt;</returns>
    
    public static IDictionary<string, string> ToDictionary(this object source, string EntityName, string sourceType)
    {
        return source.ToDictionary<string>(EntityName, sourceType);
    }
    
    /// <summary>An object extension method that converts this object to a dictionary.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">    Source for the.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="sourceType">Type of the source.</param>
    ///
    /// <returns>The given data converted to an IDictionary&lt;string,string&gt;</returns>
    
    public static IDictionary<string, string> ToDictionary<T>(this object source, string EntityName, string sourceType)
    {
        if(source == null)
            ThrowExceptionWhenSourceArgumentIsNull();
        var dictionary = new Dictionary<string, string>();
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => !p.flag.Value && p.EntityName == EntityName && p.SourceType == sourceType);
        foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            AddPropertyToDictionary<string>(property, source, dictionary, EntityDatasource);
        return dictionary;
    }
    
    /// <summary>Adds a property to dictionary.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="property">        The property.</param>
    /// <param name="source">          Source for the.</param>
    /// <param name="dictionary">      The dictionary.</param>
    /// <param name="EntityDatasource">The entity datasource.</param>
    
    private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, string> dictionary, EntityDataSource EntityDatasource)
    {
        if(property.PropertyType.Name == "ICollection`1") return;
        object value = property.GetValue(source);
        if(EntityDatasource != null)
        {
            foreach(var mapping in EntityDatasource.entitypropertymapping.Where(p => p.PropertyName == property.Name))
            {
                if(!dictionary.ContainsKey(mapping.DataName))
                    dictionary.Add(mapping.DataName, Convert.ToString(value));
            }
            return;
        }
        // if (IsOfType<T>(value))
        dictionary.Add(property.Name, Convert.ToString(value));
    }
    
    /// <summary>Query if 'value' is of type.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="value">The value.</param>
    ///
    /// <returns>True if of type, false if not.</returns>
    
    private static bool IsOfType<T>(object value)
    {
        return value is T;
    }
    
    /// <summary>Throw exception when source argument is null.</summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null.</exception>
    
    private static void ThrowExceptionWhenSourceArgumentIsNull()
    {
        throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
    }
}
/// <summary>A custom contract resolver.</summary>
public class CustomContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
{
    /// <summary>Gets or sets the property mappings.</summary>
    ///
    /// <value>The property mappings.</value>
    
    private Dictionary<string, string> PropertyMappings
    {
        get;
        set;
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="mapping">The mapping.</param>
    
    public CustomContractResolver(Dictionary<string, string> mapping)
    {
        this.PropertyMappings = mapping;
    }
    
    /// <summary>Resolves the name of the property.</summary>
    ///
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>Resolved name of the property.</returns>
    
    protected override string ResolvePropertyName(string propertyName)
    {
        string resolvedName = null;
        var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
        return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
    }
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}