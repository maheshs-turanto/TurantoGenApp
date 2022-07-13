using GeneratorBase.MVC.Models;
using Microsoft.Web.Http.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using System.Web.Http.Routing;
namespace GeneratorBase.MVC
{
/// <summary>A web API configuration.</summary>
public static class WebApiConfig
{
    /// <summary>Registers this object.</summary>
    ///
    /// <param name="config">The configuration.</param>
    
    public static void Register(HttpConfiguration config)
    {
        //https://github.com/Microsoft/aspnet-api-versioning/wiki/Versioning-via-the-URL-Path
        var constraintResolver = new DefaultInlineConstraintResolver()
        {
            //  ConstraintMap = { ["apiVersion"] = typeof(ApiVersionRouteConstraint) } //member initializer error in VS2013
        };
        if(!constraintResolver.ConstraintMap.Keys.Contains("apiVersion"))
            constraintResolver.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint)); //check if this can be used
        config.MapHttpAttributeRoutes(constraintResolver);
        config.AddApiVersioning(o => o.ReportApiVersions = true);
        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );
        //config.Routes.MapHttpRoute(
        //    name: "v2.0",
        //    routeTemplate: "api/v2/{controller}/{id}",
        //    defaults: new { id = RouteParameter.Optional }
        //);
    }
}
/// <summary>A namespace HTTP controller selector.</summary>
public class NamespaceHttpControllerSelector : IHttpControllerSelector
{
    /// <summary>The configuration.</summary>
    private readonly HttpConfiguration _configuration;
    /// <summary>The controllers.</summary>
    private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="config">The configuration.</param>
    
    public NamespaceHttpControllerSelector(HttpConfiguration config)
    {
        _configuration = config;
        _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
    }
    
    /// <summary>Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for
    /// the given <see cref="T:System.Net.Http.HttpRequestMessage" />.</summary>
    ///
    /// <exception cref="HttpResponseException">    Thrown when a HTTP Response error condition
    ///                                             occurs.</exception>
    ///
    /// <param name="request">The request message.</param>
    ///
    /// <returns>An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.</returns>
    
    public HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
        var routeData = request.GetRouteData();
        if(routeData == null)
        {
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        var controllerName = GetControllerName(routeData);
        if(controllerName == null)
        {
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        var namespaceName = GetVersion(routeData);
        if(namespaceName == null)
        {
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        var controllerKey = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", namespaceName, controllerName);
        HttpControllerDescriptor controllerDescriptor;
        if(_controllers.Value.TryGetValue(controllerKey, out controllerDescriptor))
        {
            return controllerDescriptor;
        }
        throw new HttpResponseException(HttpStatusCode.NotFound);
    }
    
    /// <summary>Returns a map, keyed by controller string, of all
    /// <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can
    /// select.  This is primarily called by
    /// <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible
    /// controllers in the system.</summary>
    ///
    /// <returns>A map of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />
    /// that the selector can select, or null if the selector does not have a well-defined mapping of
    /// <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />.</returns>
    
    public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
    {
        return _controllers.Value;
    }
    
    /// <summary>Initializes the controller dictionary.</summary>
    ///
    /// <returns>A Dictionary&lt;string,HttpControllerDescriptor&gt;</returns>
    
    private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
    {
        var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
        var assembliesResolver = _configuration.Services.GetAssembliesResolver();
        var controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();
        var controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
        foreach(var controllerType in controllerTypes)
        {
            var segments = controllerType.Namespace.Split(Type.Delimiter);
            var controllerName = controllerType.Name.Remove(controllerType.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);
            var controllerKey = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", segments[segments.Length - 1], controllerName);
            if(!dictionary.Keys.Contains(controllerKey))
            {
                dictionary[controllerKey] = new HttpControllerDescriptor(_configuration,
                        controllerType.Name,
                        controllerType);
            }
        }
        return dictionary;
    }
    
    /// <summary>Gets route variable.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="routeData">Information describing the route.</param>
    /// <param name="name">     The name.</param>
    ///
    /// <returns>The route variable.</returns>
    
    private T GetRouteVariable<T>(IHttpRouteData routeData, string name)
    {
        object result;
        if(routeData.Values.TryGetValue(name, out result))
        {
            return (T)result;
        }
        return default(T);
    }
    
    /// <summary>Gets controller name.</summary>
    ///
    /// <param name="routeData">Information describing the route.</param>
    ///
    /// <returns>The controller name.</returns>
    
    private string GetControllerName(IHttpRouteData routeData)
    {
        var subroute = routeData.GetSubRoutes().FirstOrDefault();
        if(subroute == null) return null;
        //((HttpActionDescriptor[])subroute.Route.DataTokens["actions"]).First()
        var dataTokenValue = subroute.Route.DataTokens["actions"];
        if(dataTokenValue == null)
            return null;
        var controllerName = ((HttpActionDescriptor[])dataTokenValue).First().ControllerDescriptor.ControllerName.Replace("Controller", string.Empty);
        return controllerName;
    }
    
    /// <summary>Gets a version.</summary>
    ///
    /// <param name="routeData">Information describing the route.</param>
    ///
    /// <returns>The version.</returns>
    
    public string GetVersion(IHttpRouteData routeData)
    {
        var subRouteData = routeData.GetSubRoutes().FirstOrDefault();
        if(subRouteData == null) return null;
        return GetRouteVariable<string>(subRouteData, "apiVersion");
    }
}
}
