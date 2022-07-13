using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.Description;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GeneratorBase.MVC.Models
{
/// <summary>This class will generate the samples for the help page.</summary>
public class HelpPageSampleGenerator
{
    /// <summary>Initializes a new instance of the <see cref="HelpPageSampleGenerator"/> class.</summary>
    public HelpPageSampleGenerator()
    {
        ActualHttpMessageTypes = new Dictionary<HelpPageSampleKey, Type>();
        ActionSamples = new Dictionary<HelpPageSampleKey, object>();
        SampleObjects = new Dictionary<Type, object>();
    }
    
    /// <summary>Gets or sets CLR types that are used as the content of
    /// <see cref="HttpRequestMessage"/> or <see cref="HttpResponseMessage"/>.</summary>
    ///
    /// <value>A list of types of the actual HTTP messages.</value>
    
    public IDictionary<HelpPageSampleKey, Type> ActualHttpMessageTypes
    {
        get;
        internal set;
    }
    
    /// <summary>Gets or sets the objects that are used directly as samples for certain actions.</summary>
    ///
    /// <value>The action samples.</value>
    
    public IDictionary<HelpPageSampleKey, object> ActionSamples
    {
        get;
        internal set;
    }
    
    /// <summary>Gets or sets the objects that are serialized as samples by the supported formatters.</summary>
    ///
    /// <value>The sample objects.</value>
    
    public IDictionary<Type, object> SampleObjects
    {
        get;
        internal set;
    }
    
    /// <summary>Gets the request body samples for a given <see cref="ApiDescription"/>.</summary>
    ///
    /// <param name="api">The <see cref="ApiDescription"/>.</param>
    ///
    /// <returns>The samples keyed by media type.</returns>
    
    public IDictionary<MediaTypeHeaderValue, object> GetSampleRequests(ApiDescription api)
    {
        return GetSample(api, SampleDirection.Request);
    }
    
    /// <summary>Gets the response body samples for a given <see cref="ApiDescription"/>.</summary>
    ///
    /// <param name="api">The <see cref="ApiDescription"/>.</param>
    ///
    /// <returns>The samples keyed by media type.</returns>
    
    public IDictionary<MediaTypeHeaderValue, object> GetSampleResponses(ApiDescription api)
    {
        return GetSample(api, SampleDirection.Response);
    }
    
    /// <summary>Gets the request or response body samples.</summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null.</exception>
    ///
    /// <param name="api">            The <see cref="ApiDescription"/>.</param>
    /// <param name="sampleDirection">  The value indicating whether the sample is for a request or
    ///                                 for a response.</param>
    ///
    /// <returns>The samples keyed by media type.</returns>
    
    public virtual IDictionary<MediaTypeHeaderValue, object> GetSample(ApiDescription api, SampleDirection sampleDirection)
    {
        if(api == null)
        {
            throw new ArgumentNullException("api");
        }
        string controllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
        string actionName = api.ActionDescriptor.ActionName;
        IEnumerable<string> parameterNames = api.ParameterDescriptions.Select(p => p.Name);
        Collection<MediaTypeFormatter> formatters;
        Type type = ResolveType(api, controllerName, actionName, parameterNames, sampleDirection, out formatters);
        var samples = new Dictionary<MediaTypeHeaderValue, object>();
        // Use the samples provided directly for actions
        var actionSamples = GetAllActionSamples(controllerName, actionName, parameterNames, sampleDirection);
        foreach(var actionSample in actionSamples)
        {
            samples.Add(actionSample.Key.MediaType, WrapSampleIfString(actionSample.Value));
        }
        // Do the sample generation based on formatters only if an action doesn't return an HttpResponseMessage.
        // Here we cannot rely on formatters because we don't know what's in the HttpResponseMessage, it might not even use formatters.
        if(type != null && !typeof(HttpResponseMessage).IsAssignableFrom(type))
        {
            object sampleObject = GetSampleObject(type);
            foreach(var formatter in formatters)
            {
                foreach(MediaTypeHeaderValue mediaType in formatter.SupportedMediaTypes)
                {
                    if(!samples.ContainsKey(mediaType))
                    {
                        object sample = GetActionSample(controllerName, actionName, parameterNames, type, formatter, mediaType, sampleDirection);
                        // If no sample found, try generate sample using formatter and sample object
                        if(sample == null && sampleObject != null)
                        {
                            sample = WriteSampleObjectUsingFormatter(formatter, sampleObject, type, mediaType);
                        }
                        samples.Add(mediaType, WrapSampleIfString(sample));
                    }
                }
            }
        }
        return samples;
    }
    
    /// <summary>Search for samples that are provided directly through <see cref="ActionSamples"/>.</summary>
    ///
    /// <param name="controllerName"> Name of the controller.</param>
    /// <param name="actionName">     Name of the action.</param>
    /// <param name="parameterNames"> The parameter names.</param>
    /// <param name="type">           The CLR type.</param>
    /// <param name="formatter">      The formatter.</param>
    /// <param name="mediaType">      The media type.</param>
    /// <param name="sampleDirection">  The value indicating whether the sample is for a request or
    ///                                 for a response.</param>
    ///
    /// <returns>The sample that matches the parameters.</returns>
    
    public virtual object GetActionSample(string controllerName, string actionName, IEnumerable<string> parameterNames, Type type, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, SampleDirection sampleDirection)
    {
        object sample;
        // First, try get sample provided for a specific mediaType, controllerName, actionName and parameterNames.
        // If not found, try get the sample provided for a specific mediaType, controllerName and actionName regardless of the parameterNames
        // If still not found, try get the sample provided for a specific type and mediaType
        if(ActionSamples.TryGetValue(new HelpPageSampleKey(mediaType, sampleDirection, controllerName, actionName, parameterNames), out sample) ||
                ActionSamples.TryGetValue(new HelpPageSampleKey(mediaType, sampleDirection, controllerName, actionName, new[] { "*" }), out sample) ||
                ActionSamples.TryGetValue(new HelpPageSampleKey(mediaType, type), out sample))
        {
            return sample;
        }
        return null;
    }
    
    /// <summary>Gets the sample object that will be serialized by the formatters. First, it will look
    /// at the <see cref="SampleObjects"/>. If no sample object is found, it will try to create one
    /// using <see cref="ObjectGenerator"/>.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>The sample object.</returns>
    
    public virtual object GetSampleObject(Type type)
    {
        object sampleObject;
        if(!SampleObjects.TryGetValue(type, out sampleObject))
        {
            // Try create a default sample object
            ObjectGenerator objectGenerator = new ObjectGenerator();
            sampleObject = objectGenerator.GenerateObject(type);
        }
        return sampleObject;
    }
    
    /// <summary>Resolves the type of the action parameter or return value when
    /// <see cref="HttpRequestMessage"/> or <see cref="HttpResponseMessage"/> is used.</summary>
    ///
    /// <exception cref="InvalidEnumArgumentException"> Thrown when an Invalid Enum Argument error
    ///                                                 condition occurs.</exception>
    /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
    ///                                                 are null.</exception>
    ///
    /// <param name="api">            The <see cref="ApiDescription"/>.</param>
    /// <param name="controllerName"> Name of the controller.</param>
    /// <param name="actionName">     Name of the action.</param>
    /// <param name="parameterNames"> The parameter names.</param>
    /// <param name="sampleDirection">  The value indicating whether the sample is for a request or a
    ///                                 response.</param>
    /// <param name="formatters">     [out] The formatters.</param>
    ///
    /// <returns>A Type.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is only used in advanced scenarios.")]
    public virtual Type ResolveType(ApiDescription api, string controllerName, string actionName, IEnumerable<string> parameterNames, SampleDirection sampleDirection, out Collection<MediaTypeFormatter> formatters)
    {
        if(!Enum.IsDefined(typeof(SampleDirection), sampleDirection))
        {
            throw new InvalidEnumArgumentException("sampleDirection", (int)sampleDirection, typeof(SampleDirection));
        }
        if(api == null)
        {
            throw new ArgumentNullException("api");
        }
        Type type;
        if(ActualHttpMessageTypes.TryGetValue(new HelpPageSampleKey(sampleDirection, controllerName, actionName, parameterNames), out type) ||
                ActualHttpMessageTypes.TryGetValue(new HelpPageSampleKey(sampleDirection, controllerName, actionName, new[] { "*" }), out type))
        {
            // Re-compute the supported formatters based on type
            Collection<MediaTypeFormatter> newFormatters = new Collection<MediaTypeFormatter>();
            foreach(var formatter in api.ActionDescriptor.Configuration.Formatters)
            {
                if(IsFormatSupported(sampleDirection, formatter, type))
                {
                    newFormatters.Add(formatter);
                }
            }
            formatters = newFormatters;
        }
        else
        {
            switch(sampleDirection)
            {
            case SampleDirection.Request:
                ApiParameterDescription requestBodyParameter = api.ParameterDescriptions.FirstOrDefault(p => p.Source == ApiParameterSource.FromBody);
                type = requestBodyParameter == null ? null : requestBodyParameter.ParameterDescriptor.ParameterType;
                formatters = api.SupportedRequestBodyFormatters;
                break;
            case SampleDirection.Response:
            default:
                type = api.ActionDescriptor.ReturnType;
                formatters = api.SupportedResponseFormatters;
                break;
            }
        }
        return type;
    }
    
    /// <summary>Writes the sample object using formatter.</summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null.</exception>
    ///
    /// <param name="formatter">The formatter.</param>
    /// <param name="value">    The value.</param>
    /// <param name="type">     The type.</param>
    /// <param name="mediaType">Type of the media.</param>
    ///
    /// <returns>An object.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is recorded as InvalidSample.")]
    public virtual object WriteSampleObjectUsingFormatter(MediaTypeFormatter formatter, object value, Type type, MediaTypeHeaderValue mediaType)
    {
        if(formatter == null)
        {
            throw new ArgumentNullException("formatter");
        }
        if(mediaType == null)
        {
            throw new ArgumentNullException("mediaType");
        }
        object sample = String.Empty;
        MemoryStream ms = null;
        HttpContent content = null;
        try
        {
            if(formatter.CanWriteType(type))
            {
                ms = new MemoryStream();
                content = new ObjectContent(type, value, formatter, mediaType);
                formatter.WriteToStreamAsync(type, value, ms, content, null).Wait();
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms);
                string serializedSampleString = reader.ReadToEnd();
                if(mediaType.MediaType.ToUpperInvariant().Contains("XML"))
                {
                    serializedSampleString = TryFormatXml(serializedSampleString);
                }
                else if(mediaType.MediaType.ToUpperInvariant().Contains("JSON"))
                {
                    serializedSampleString = TryFormatJson(serializedSampleString);
                }
                sample = new TextSample(serializedSampleString);
            }
            else
            {
                sample = new InvalidSample(String.Format(
                                               CultureInfo.CurrentCulture,
                                               "Failed to generate the sample for media type '{0}'. Cannot use formatter '{1}' to write type '{2}'.",
                                               mediaType,
                                               formatter.GetType().Name,
                                               type.Name));
            }
        }
        catch(Exception e)
        {
            sample = new InvalidSample(String.Format(
                                           CultureInfo.CurrentCulture,
                                           "An exception has occurred while using the formatter '{0}' to generate sample for media type '{1}'. Exception message: {2}",
                                           formatter.GetType().Name,
                                           mediaType.MediaType,
                                           e.Message));
        }
        finally
        {
            if(ms != null)
            {
                ms.Dispose();
            }
            if(content != null)
            {
                content.Dispose();
            }
        }
        return sample;
    }
    
    /// <summary>Try format JSON.</summary>
    ///
    /// <param name="str">The string.</param>
    ///
    /// <returns>A string.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Handling the failure by returning the original string.")]
    private static string TryFormatJson(string str)
    {
        try
        {
            object parsedJson = JsonConvert.DeserializeObject(str);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
        catch
        {
            // can't parse JSON, return the original string
            return str;
        }
    }
    
    /// <summary>Try format XML.</summary>
    ///
    /// <param name="str">The string.</param>
    ///
    /// <returns>A string.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Handling the failure by returning the original string.")]
    private static string TryFormatXml(string str)
    {
        try
        {
            XDocument xml = XDocument.Parse(str);
            return xml.ToString();
        }
        catch
        {
            // can't parse XML, return the original string
            return str;
        }
    }
    
    /// <summary>Query if 'sampleDirection' is format supported.</summary>
    ///
    /// <param name="sampleDirection">  The value indicating whether the sample is for a request or
    ///                                 for a response.</param>
    /// <param name="formatter">      The formatter.</param>
    /// <param name="type">           The CLR type.</param>
    ///
    /// <returns>True if format supported, false if not.</returns>
    
    private static bool IsFormatSupported(SampleDirection sampleDirection, MediaTypeFormatter formatter, Type type)
    {
        switch(sampleDirection)
        {
        case SampleDirection.Request:
            return formatter.CanReadType(type);
        case SampleDirection.Response:
            return formatter.CanWriteType(type);
        }
        return false;
    }
    
    /// <summary>Gets all action samples in this collection.</summary>
    ///
    /// <param name="controllerName"> Name of the controller.</param>
    /// <param name="actionName">     Name of the action.</param>
    /// <param name="parameterNames"> The parameter names.</param>
    /// <param name="sampleDirection">  The value indicating whether the sample is for a request or
    ///                                 for a response.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process all action samples in this
    /// collection.</returns>
    
    private IEnumerable<KeyValuePair<HelpPageSampleKey, object>> GetAllActionSamples(string controllerName, string actionName, IEnumerable<string> parameterNames, SampleDirection sampleDirection)
    {
        HashSet<string> parameterNamesSet = new HashSet<string>(parameterNames, StringComparer.OrdinalIgnoreCase);
        foreach(var sample in ActionSamples)
        {
            HelpPageSampleKey sampleKey = sample.Key;
            if(String.Equals(controllerName, sampleKey.ControllerName, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(actionName, sampleKey.ActionName, StringComparison.OrdinalIgnoreCase) &&
                    (sampleKey.ParameterNames.SetEquals(new[] { "*" }) || parameterNamesSet.SetEquals(sampleKey.ParameterNames)) &&
                    sampleDirection == sampleKey.SampleDirection)
            {
                yield return sample;
            }
        }
    }
    
    /// <summary>Wrap sample if string.</summary>
    ///
    /// <param name="sample">The sample.</param>
    ///
    /// <returns>An object.</returns>
    
    private static object WrapSampleIfString(object sample)
    {
        string stringSample = sample as string;
        if(stringSample != null)
        {
            return new TextSample(stringSample);
        }
        return sample;
    }
}
}