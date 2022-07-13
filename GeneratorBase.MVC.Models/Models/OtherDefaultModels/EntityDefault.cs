using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
namespace GeneratorBase.MVC.Models
{

/// <summary>An entity default.</summary>
public abstract class EntityDefault : IEntity
{
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the concurrency key.</summary>
    ///
    /// <value>The concurrency key.</value>
    
    [Timestamp]
    [ConcurrencyCheck]
    public byte[] ConcurrencyKey
    {
        get;
        set;
    }
    /// <summary>The display value.</summary>
    [NotMapped]
    public string m_DisplayValue = "";
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [DisplayName("DisplayValue")]
    public string DisplayValue
    {
        get
        {
            return getDisplayValueModel();
        }
        set
        {
            value = m_DisplayValue;
        }
    }
    
    /// <summary>Gets or sets the timezone.</summary>
    ///
    /// <value>The m timezone.</value>
    
    [NotMapped]
    [Newtonsoft.Json.JsonIgnore]
    public TimeZoneInfo m_Timezone
    {
        get
        {
            var result = "Eastern Standard Time";
            if(HttpContext.Current != null)
            {
                var timeZoneCookie = HttpContext.Current.Request.Cookies["_timezone"];
                if(timeZoneCookie != null)
                {
                    result = Convert.ToString(timeZoneCookie.Value);
                }
            }
            return  TimeZoneInfo.FindSystemTimeZoneById(result);
        }
        set { }
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public virtual string getDisplayValueModel()
    {
        return m_DisplayValue;
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