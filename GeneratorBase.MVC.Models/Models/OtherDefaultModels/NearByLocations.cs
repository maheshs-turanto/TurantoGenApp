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
/// <summary>A near by locations.</summary>
public class NearByLocations
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="latitude">     The latitude.</param>
    /// <param name="longitude">    The longitude.</param>
    /// <param name="address">      The address.</param>
    /// <param name="displayValue"> The display value.</param>
    /// <param name="ImageFileType">The type of the image file.</param>
    
    public NearByLocations(double latitude, double longitude, string address, string displayValue, string ImageFileType)
    {
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
        DisplayValue = displayValue;
        Distance = 0;
        Picture = "";
        ID = 0;
        ImageFileType = "File";
    }
    
    /// <summary>Gets or sets the latitude.</summary>
    ///
    /// <value>The latitude.</value>
    
    public double Latitude
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the longitude.</summary>
    ///
    /// <value>The longitude.</value>
    
    public double Longitude
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the address.</summary>
    ///
    /// <value>The address.</value>
    
    public string Address
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    public string DisplayValue
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the distance.</summary>
    ///
    /// <value>The distance.</value>
    
    public double Distance
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the picture.</summary>
    ///
    /// <value>The picture.</value>
    
    public string Picture
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    public long ID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the image file.</summary>
    ///
    /// <value>The type of the image file.</value>
    
    public string ImageFileType
    {
        get;
        set;
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