using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Net;
using System.IO;
using System.Data;
using System.Xml;
namespace GeneratorBase.MVC.Models
{
/// <summary>A mandatory dropdown (Custom Validation).</summary>
public class MandatoryDropdown
{
    /// <summary>Validates the dropdown.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="validationContext">Context for the validation.</param>
    ///
    /// <returns>A ValidationResult.</returns>
    
    public static ValidationResult ValidateDropdown(long? id, ValidationContext validationContext)
    {
        if(!id.HasValue && !HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsValue("SyncData"))
        {
            var displayName = "";
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName).GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if(attributes != null)
                displayName = (attributes[0] as DisplayNameAttribute).DisplayName;
            else
                displayName = validationContext.DisplayName;
            return new ValidationResult(
                       displayName + " Field is required.");
        }
        else
            return ValidationResult.Success;
    }
}
/// <summary>An entity.</summary>
public abstract class Entity : IEntity, ISoftDelete
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
    [Newtonsoft.Json.JsonIgnore]
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
            if(System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != null && System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != "Default")
            {
                result = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
                var timezone  = TimeZoneInfo.FindSystemTimeZoneById(result);
                if(timezone != null)
                    return timezone;
            }
            if(HttpContext.Current != null)
            {
                var timeZoneCookie = HttpContext.Current.Request.Cookies["_timezone"];
                if(timeZoneCookie != null)
                {
                    result = Convert.ToString(timeZoneCookie.Value);
                }
            }
            return TimeZoneInfo.FindSystemTimeZoneById(result);
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
    
    /// <summary>Gets or sets the timezonecustom.</summary>
    ///
    /// <value>The m timezonecustom.</value>
    
    [NotMapped]
    [Newtonsoft.Json.JsonIgnore]
    public TimeZoneInfo m_Timezonecustom
    {
        get
        {
            var result = "Eastern Standard Time";
            if(HttpContext.Current != null)
            {
                var timeZoneCookie = HttpContext.Current.Request.Cookies["_timezonecustom"];
                if(timeZoneCookie != null)
                {
                    result = Convert.ToString(timeZoneCookie.Value);
                }
            }
            return TimeZoneInfo.FindSystemTimeZoneById(result);
        }
        set { }
    }
    
    /// <summary>Gets time zone custom.</summary>
    ///
    /// <param name="timezonecustomid">The timezonecustomid.</param>
    ///
    /// <returns>The time zone custom.</returns>
    
    public TimeZoneInfo getTimeZoneCustom(Int64 timezonecustomid)
    {
        var result = "Eastern Standard Time";
        using(var timedb = (new ApplicationContext(new SystemUser())))
        {
            var timezonecustominfo = timedb.T_TimeZones.Find(timezonecustomid);
            if(timezonecustominfo != null)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timezonecustominfo.T_FullName);
            }
        }
        return TimeZoneInfo.FindSystemTimeZoneById(result);
    }
    
    /// <summary>Applies the hidden business rule (make values empty).</summary>
    ///
    /// <param name="businessrule">The businessrule.</param>
    /// <param name="entityName">  Name of the entity.</param>
    ///
    /// <returns>A list of.</returns>
    
    public System.Collections.Generic.List<string> ApplyHiddenRule(System.Collections.Generic.List<BusinessRule> businessrule, string entityName, bool IsEdit = false)
    {
        return BusinessRuleHelper.HiddenPropertiesRule(this, businessrule, entityName,  IsEdit);
    }
    public System.Collections.Generic.List<string> ApplyHiddenGroupRule(System.Collections.Generic.List<BusinessRule> businessrule, string entityName)
    {
        return BusinessRuleHelper.HiddenGroupsRule(this, businessrule, entityName);
    }
    public System.Collections.Generic.List<string> ApplyHiddenVerbRule(System.Collections.Generic.List<BusinessRule> businessrule, string entityName)
    {
        return BusinessRuleHelper.HiddenVerbOnGridRule(this, businessrule, entityName);
    }
    
    /// <summary>Applies the lock business rule.</summary>
    ///
    /// <param name="obj">Entity Record</param>
    /// <param name="User">Application User.</param>
    /// <param name="businessrule">The businessrule.</param>
    /// <param name="entityName">  Name of the entity.</param>
    ///
    /// <returns>A list of.</returns>
    
    public System.Boolean ApplyLockRecordRule(object obj, IUser User, System.Collections.Generic.List<BusinessRule> businessrule, string entityName)
    {
        return BusinessRuleHelper.GetLockBusinessRules(obj, User, businessrule, entityName);
    }
    /// <summary>Call When Calculation of distance// </summary>
    /// <param name="origin">Start Point</param>
    /// <param name="destination">End Point</param>
    /// <returns></returns>
    public Nullable<Decimal> GetDistance(string origin, string destination)
    {
        decimal? distance = null;
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        string Apikey = commonObj.GetGoogleMapApiKey();
        if(string.IsNullOrEmpty(Apikey))
            return null;
        if(!(string.IsNullOrEmpty(origin) && string.IsNullOrEmpty(destination)))
        {
            var url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&key=" + Apikey + "&mode=driving&language=en-EN&sensor=false&libraries=places";
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responsereader = reader.ReadToEnd();
            response.Close();
            DataSet ds = new DataSet();
            ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
            if(ds.Tables.Count > 0)
            {
                if(ds.Tables[0].Rows[0]["status"].ToString() == "OK" && ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
                {
                    //duration = ds.Tables["duration"].Rows[0]["text"].ToString();
                    var _distance = ds.Tables["distance"].Rows[0]["value"].ToString();
                    if(!string.IsNullOrEmpty(_distance) && _distance != "0")
                    {
                        if(_distance == "0")
                            return 0;
                        if(commonObj.GetUnit().ToLower() == "miles" || commonObj.GetUnit().ToLower() == "mi")
                        {
                            distance = Convert.ToDecimal(_distance) / 1609;
                        }
                        else
                            distance = Convert.ToDecimal(_distance) /1000;
                    }
                }
            }
        }
        return Convert.ToDecimal(distance);
    }
}
}
