using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
/// <summary>A OData services repository.</summary>
public class ODataServicesRepository : IODataServicesRepository
{
    /// <summary>Information describing the data services.</summary>
    private XDocument ODataServicesData;
    /// <summary>constructor.</summary>
    public ODataServicesRepository()
    {
        ODataServicesData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
    
    /// <summary>Gets o data services.</summary>
    ///
    /// <returns>The o data services.</returns>
    
    public ODataServices GetODataServices()
    {
        IEnumerable<ODataServices> ODataServices = from cp in ODataServicesData.Descendants("Settings")
                select new ODataServices((bool)cp.Element("EnableODataServices"));
        return ODataServices.ToList()[0];
    }
    
    /// <summary>Edit Record.</summary>
    ///
    /// <param name="cp">The cp.</param>
    
    public void EditODataServices(ODataServices cp)
    {
        XElement node = ODataServicesData.Root.Elements("Settings").FirstOrDefault();
        node.SetElementValue("EnableODataServices", cp.EnableODataServices == false ? false : cp.EnableODataServices);
        ODataServicesData.Save(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
}