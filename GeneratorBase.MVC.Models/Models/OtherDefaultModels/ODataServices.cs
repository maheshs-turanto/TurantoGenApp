using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>A Odata services.</summary>
public class ODataServices
{
    /// <summary>Default constructor.</summary>
    public ODataServices()
    {
        this.EnableODataServices = false;
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="enableodataservices">True if enable o data services, false if not.</param>
    
    public ODataServices(bool enableodataservices)
    {
        this.EnableODataServices = enableodataservices;
    }
    
    /// <summary>Gets or sets a value indicating whether the o data services is enabled.</summary>
    ///
    /// <value>True if enable o data services, false if not.</value>
    
    [DisplayName("Enable OData Services")]
    public bool EnableODataServices
    {
        get;
        set;
    }
}
/// <summary>Interface for i/o data services repository.</summary>
public interface IODataServicesRepository
{
    /// <summary>Edit o data services.</summary>
    ///
    /// <param name="cp">The cp.</param>
    
    void EditODataServices(ODataServices cp);
    
    /// <summary>Gets o data services.</summary>
    ///
    /// <returns>The o data services.</returns>
    
    ODataServices GetODataServices();
}
}