using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Linq.Dynamic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Web.UI;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using UAParser;
namespace GeneratorBase.MVC.Models
{
/// <summary>Attribute for custom display name (used in model class for dynamic label change).</summary>
public class CustomDisplayNameAttribute : DisplayNameAttribute
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    public CustomDisplayNameAttribute(string key, string resourcename, string defaultvalue) : base(Lookup(key, resourcename, defaultvalue)) { }
    /// <summary>Looks up a given key to find its associated value.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    ///
    /// <returns>A string.</returns>
    static string Lookup(string key,string resourcename,string defaultvalue)
    {
        var result = defaultvalue;
        try
        {
            var filepath = "";
            var fileexist = false;
            if(System.Web.HttpContext.Current != null)
            {
                filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
                fileexist = System.IO.File.Exists(filepath + resourcename);
            }
            if(fileexist)
            {
                using(System.Resources.ResourceSet resxSet = new System.Resources.ResourceSet(filepath + resourcename))
                {
                    result = resxSet.GetString(key);
                    if(string.IsNullOrEmpty(result))
                        result = defaultvalue;
                }
            }
        }
        catch
        {
            return defaultvalue; // fallback
        }
        return result;
    }
}
/// <summary>Attribute for custom date.</summary>
public class CustomDateAttribute : RangeAttribute
{
    /// <summary>Constructor.</summary>
    ///
    /// <param name="fromDate">from date.</param>
    /// <param name="toDate">  to date.</param>
    public CustomDateAttribute(string fromDate, string toDate)
        : base(typeof(DateTime), fromDate, toDate)
    { }
}


///<summary>A property Type for entity.</summary>
[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
public class PropertyTypeForEntity : System.Attribute
{

    string check;
    bool isdisplaypropert;
    /// <summary>Constructor.</summary>
    ///        /// <param name="ptype">The pcheck.</param>
    public PropertyTypeForEntity(string pcheck = null, bool pisdisplayproperty = false)
    {
        check = pcheck;
        isdisplaypropert = pisdisplayproperty;
    }
    /// <summary>Gets the proptype.</summary>
    ///
    /// <value>The propcheck.</value>
    public string PropCheck
    {
        get
        {
            return check;
        }
    }
    public bool IsDisplayProperty
    {
        get
        {
            return isdisplaypropert;
        }
    }
}


/// <summary>A property information for entity.</summary>
[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
public class PropertyInfoForEntity : System.Attribute
{
    /// <summary>The name.</summary>
    string name;
    /// <summary>The text.</summary>
    string text;
    /// <summary>The type.</summary>
    string type;
    string groupinternalname;
    /// <summary>Constructor.</summary>
    ///
    /// <param name="pname">The pname.</param>
    /// <param name="ptext">The ptext.</param>
    /// <param name="ptype">The ptype.</param>
    public PropertyInfoForEntity(string pname, string ptext, string ptype)
    {
        name = pname;
        text = ptext;
        type = ptype;
        if(ptype =="Group")
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[;\\\\/:*?#$%!~^,@&()\"<>|&'\\[\\]]");
            var sourcenameOutput = r.Replace(ptext, " ");
            sourcenameOutput = sourcenameOutput.Replace(" ", "");
            sourcenameOutput = sourcenameOutput.Replace(".", "");
            sourcenameOutput = sourcenameOutput.Replace("+", "");
            sourcenameOutput = sourcenameOutput.Replace("-", "");
            groupinternalname = sourcenameOutput;
        }
    }
    /// <summary>Gets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    public string PropName
    {
        get
        {
            return name;
        }
    }
    
    /// <summary>Gets the property text.</summary>
    ///
    /// <value>The property text.</value>
    public string PropText
    {
        get
        {
            return text;
        }
    }
    
    /// <summary>Gets the proptype.</summary>
    ///
    /// <value>The proptype.</value>
    public string Proptype
    {
        get
        {
            return type;
        }
    }
    public string GroupInternalName
    {
        get
        {
            return groupinternalname;
        }
    }
    
}
/// <summary>Sorting IQueryable through reflection.</summary>
public static class Sorting
{
    /// <summary>An IQueryable<T> extension method that sorts.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">  The source to act on.</param>
    /// <param name="property">The property.</param>
    ///
    /// <returns>The sorted values.</returns>
    public static IQueryable<T> Sort<T>(this IQueryable<T> source, string property)
    {
        if(string.IsNullOrEmpty(property)) return source;
        string[] props = property.Split(',');
        string lamba = "";
        Type type = typeof(T);
        var EntityName = type.Name;
        var entityModel  = ModelReflector.Entities;
        foreach(string prop in props)
        {
            if(string.IsNullOrEmpty(prop)) continue;
            if(prop.Contains("."))
            {
                lamba += prop + ",";
                continue;
            }
            var asso = entityModel.FirstOrDefault(p => p.Name == EntityName).Associations.FirstOrDefault(p => p.AssociationProperty == prop);
            if(asso != null)
            {
                lamba += asso.Name.ToLower()+"."+"DisplayValue"+",";
            }
            else
            {
                lamba += prop+",";
            }
        }
        lamba = lamba.TrimEnd(",".ToCharArray());
        return source.AsQueryable().OrderBy(lamba);
    }
    /// <summary>An IQueryable<T> extension method that sorts.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">  The source to act on.</param>
    /// <param name="property">The property.</param>
    /// <param name="isasc">   True to isasc.</param>
    ///
    /// <returns>The sorted values.</returns>
    public static IQueryable<T> Sort<T>(this IQueryable<T> source, string property, bool isasc)
    {
        if(string.IsNullOrEmpty(property)) return source;
        string[] props = property.Split(',');
        string lamba = "";
        Type type = typeof(T);
        var EntityName = type.Name;
        var entityModel  = ModelReflector.Entities;
        foreach(string prop in props)
        {
            if(string.IsNullOrEmpty(prop)) continue;
            if(prop.Contains("."))
            {
                lamba += prop + ",";
                continue;
            }
            var asso = entityModel.FirstOrDefault(p => p.Name == EntityName).Associations.FirstOrDefault(p => p.AssociationProperty == prop);
            if(asso != null)
            {
                lamba += asso.Name.ToLower() + "." + "DisplayValue" + ",";
            }
            else
            {
                lamba += prop + ",";
            }
        }
        lamba = lamba.TrimEnd(",".ToCharArray());
        if(isasc)
            return source.AsQueryable().OrderBy(lamba);
        else
            return source.AsQueryable().OrderBy(lamba + " descending");
    }
}
/// <summary>A common function singleton class (mainly used for application configuration).</summary>
public partial class CommonFunction
{
    /// <summary>The instance.</summary>
    private static CommonFunction instance = null;
    
    /// <summary>Gets or sets the full pathname of the report file.</summary>
    ///
    /// <value>The full pathname of the report file.</value>
    
    private string reportPath
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the report user.</summary>
    ///
    /// <value>The report user.</value>
    
    private string reportUser
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the report pass.</summary>
    ///
    /// <value>The report pass.</value>
    
    private string reportPass
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the pathname of the report folder.</summary>
    ///
    /// <value>The pathname of the report folder.</value>
    
    private string reportFolder
    {
        get;
        set;
    }
    /// <summary>Gets or sets the date format.</summary>
    ///
    /// <value>The name of the date format.</value>
    private string dateformat
    {
        get;
        set;
    }
    /// <summary>Gets or sets the time format.</summary>
    ///
    /// <value>The name of the time format.</value>
    private string timeformat
    {
        get;
        set;
    }
    /// <summary>Gets or sets the time zone.</summary>
    ///
    /// <value>The name of the time zone.</value>
    private string timezone
    {
        get;
        set;
    }
    /// <summary>Gets or sets the administrator roles.</summary>
    ///
    /// <value>The administrator roles.</value>
    
    private string administratorRoles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the pathname of the use active directory.</summary>
    ///
    /// <value>The pathname of the use active directory.</value>
    
    private string useActiveDirectory
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the use active directory role.</summary>
    ///
    /// <value>The use active directory role.</value>
    
    private string useActiveDirectoryRole
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the domain.</summary>
    ///
    /// <value>The name of the domain.</value>
    
    private string domainName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the need shared user system.</summary>
    ///
    /// <value>The need shared user system.</value>
    
    private string needSharedUserSystem
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the server.</summary>
    ///
    /// <value>The server.</value>
    
    private string server
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets URL of the application.</summary>
    ///
    /// <value>The application URL.</value>
    
    private string appURL
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the application.</summary>
    ///
    /// <value>The name of the application.</value>
    
    private string appName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the multiple role selection.</summary>
    ///
    /// <value>The multiple role selection.</value>
    
    private string multipleRoleSelection
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the enable prototyping tool.</summary>
    ///
    /// <value>The enable prototyping tool.</value>
    
    private string enablePrototypingTool
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the maintenance mode.</summary>
    ///
    /// <value>The maintenance mode.</value>
    
    private string maintenanceMode
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the maintenance mode roles.</summary>
    ///
    /// <value>The maintenance mode roles.</value>
    
    private string maintenanceModeRoles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a message describing the maintenance mode alert.</summary>
    ///
    /// <value>A message describing the maintenance mode alert.</value>
    
    private string maintenanceModeAlertMessage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the gpsenabled.</summary>
    ///
    /// <value>The gpsenabled.</value>
    
    private string gpsenabled
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the application sessiontimeout.</summary>
    ///
    /// <value>The application sessiontimeout.</value>
    
    private string applicationSessiontimeout
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the application sessiontimeout alert.</summary>
    ///
    /// <value>The application sessiontimeout alert.</value>
    
    private string applicationSessiontimeoutAlert
    {
        get;
        set;
    }
    
    
    /// <summary>Gets or sets the createan account.</summary>
    ///
    /// <value>The createan account.</value>
    
    private string createanAccount
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the enable ga.</summary>
    ///
    /// <value>The enable ga.</value>
    
    private string enableGA
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the tracking.</summary>
    ///
    /// <value>The identifier of the tracking.</value>
    
    private string trackingID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the customdimensionname.</summary>
    ///
    /// <value>The customdimensionname.</value>
    
    private string customdimensionname
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the soft delete enabled.</summary>
    ///
    /// <value>The soft delete enabled.</value>
    
    private string softDeleteEnabled
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the promptforroleselection.</summary>
    ///
    /// <value>The promptforroleselection.</value>
    
    private string promptforroleselection
    {
        get;
        set;
    }
    /// <summary>Gets or sets the external validation jwt key.</summary>
    ///
    /// <value>The promptforroleselection.</value>
    
    private string externalvalidationjwtkey
    {
        get;
        set;
    }
    /// <summary>Gets or sets the external validation jwt keysize.</summary>
    ///
    /// <value>The promptforroleselection.</value>
    
    private string externalvalidationjwtkeysize
    {
        get;
        set;
    }
    /// <summary>Gets or sets the external jwt issuer name.</summary>
    ///
    /// <value>The promptforroleselection.</value>
    private string externaljwtissuername
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the external jwt algorithm.</summary>
    ///
    /// <value>The promptforroleselection.</value>
    private string externaljwtalgorithm
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the external cache timeout.</summary>
    ///
    /// <value>The cache timeout.</value>
    private string querycachetimeout
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the prevent multiple login.</summary>
    ///
    /// <value>The preventmultiplelogin.</value>
    private string preventmultiplelogin
    {
        get;
        set;
    }
    /// <summary>>Gets or sets the googlemapapikey </summary>
    ///
    /// <value>The googlemapapikey.</value>
    private string googlemapapikey
    {
        get;
        set;
    }
    /// <summary>>Gets or sets the units </summary>
    ///
    /// <value>The units.</value>
    private string units
    {
        get;
        set;
    }
    /// <summary>Gets or sets the applysecuritypolicy.</summary>
    ///
    /// <value>The applysecuritypolicy.</value>
    private string applysecuritypolicy
    {
        get;
        set;
    }
    /// <summary>Gets or sets the enforcechangepassword.</summary>
    ///
    /// <value>The enforcechangepassword.</value>
    private string enforcechangepassword
    {
        get;
        set;
    }
    /// <summary>Gets or sets the journal everything.</summary>
    ///
    /// <value>The journal everything.</value>
    private string journaleverything
    {
        get;
        set;
    }
    /// <summary>Gets or sets the enable query caching.</summary>
    ///
    /// <value>The enablequerycaching.</value>
    private string enablequerycaching
    {
        get;
        set;
    }
    /// <summary>Gets or sets the scheduledtaskcallbacktime.</summary>
    ///
    /// <value>The scheduledtaskcallbacktime.</value>
    private string scheduledtaskcallbacktime
    {
        get;
        set;
    }
    /// <summary>Gets or sets the use revalee.</summary>
    ///
    /// <value>The useravalee.</value>
    private string userevalee
    {
        get;
        set;
    }
    /// <summary>Gets or sets the passwordexpirationindays.</summary>
    ///
    /// <value>The passwordexpirationindays.</value>
    private string passwordexpirationindays
    {
        get;
        set;
    }
    //Two fctor change
    /// <summary>Gets or sets the twofactorauthenticationenable.</summary>
    ///
    /// <value>The twofactorauthenticationenable.</value>
    private string twofactorauthenticationenable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the twilio account SID.</summary>
    ///
    /// <value>The twilio account SID.</value>
    private string twilioaccountsid
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the twilio auth token.</summary>
    ///
    /// <value>The twilio ith token.</value>
    private string twilioauthtoken
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the twilio from number.</summary>
    ///
    /// <value>The twilio from number.</value>
    private string twiliofromnumber
    {
        get;
        set;
    }
    /// <summary>Gets or sets the twilio default country code.</summary>
    ///
    /// <value>The twilio country code.</value>
    private string twiliocountrycode
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Always Use Email As Username.</summary>
    ///
    /// <value>The AlwaysUseEmailAsUsername.</value>
    private string _AlwaysUseEmailAsUsername
    {
        get;
        set;
    }
    /// <summary>
    /// frontdoorurl
    /// </summary>
    private string frontdoorurl
    {
        get;
        set;
    }
    
    /// <summary>
    /// frontdoorenable
    /// </summary>
    private string frontdoorenable
    {
        get;
        set;
    }
    
    /// <summary>
    /// frontdoorid
    /// </summary>
    private string frontdoorid
    {
        get;
        set;
    }
    /// <summary>
    /// showmenuontop
    /// </summary>
    private string showmenuontop
    {
        get;
        set;
    }
    
    private string enablenotification //added by rachana
    {
        get;
        set;
    }
    
    
    /// <summary>Constructor that prevents a default instance of this class from being created.</summary>
    private CommonFunction()
    {
        using(var appsettingdb = (new ApplicationContext(new SystemUser())))
        {
            var appsettinglist = appsettingdb.AppSettings;
            reportPath = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportPath".ToLower()).Value;
            reportUser = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportUser".ToLower()).Value;
            reportPass = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportPass".ToLower()).Value;
            reportFolder = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportFolder".ToLower()).Value;
            administratorRoles = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AdministratorRoles".ToLower()).Value;
            useActiveDirectory = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "UseActiveDirectory".ToLower()).Value;
            useActiveDirectoryRole = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "UseActiveDirectoryRole".ToLower()).Value;
            domainName = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "DomainName".ToLower()).Value;
            needSharedUserSystem = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "NeedSharedUserSystem".ToLower()).Value;
            server = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "server".ToLower()).Value;
            appURL = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AppURL".ToLower()).Value;
            appName = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AppName".ToLower()).Value;
            multipleRoleSelection = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "MultipleRoleSelection".ToLower()).Value;
            enablePrototypingTool = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "EnablePrototypingTool".ToLower()).Value;
            var objmaintenanceMode  = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "MaintenanceMode".ToLower());
            maintenanceMode = objmaintenanceMode != null ? objmaintenanceMode.Value : "false";
            var objmaintenanceModeRoles = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "MaintenanceModeRoles");
            maintenanceModeRoles = objmaintenanceModeRoles != null ? objmaintenanceModeRoles.Value : "Admin";
            var objmaintenanceModeAlertMessage = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "MaintenanceModeAlertMessage");
            maintenanceModeAlertMessage = objmaintenanceModeAlertMessage != null ? objmaintenanceModeAlertMessage.Value : "The application is undergoing maintenance. Please try again later.";
            gpsenabled = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "GPSEnabled".ToLower()).Value;
            applicationSessiontimeout = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ApplicationSessionTimeOut".ToLower()).Value;
            applicationSessiontimeoutAlert = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ApplicationSessionTimeOutAlert".ToLower()).Value;
            createanAccount = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "CreateAnAccount".ToLower()).Value;
            //GA Seetings
            enableGA = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "Enable google analytics".ToLower()).Value;
            trackingID = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "Tracking ID".ToLower()).Value;
            customdimensionname = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "Custom Dimension Name".ToLower()).Value;
            //
            softDeleteEnabled ="false";
            var objPromptForRoleSelection = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "PromptForRoleSelection".ToLower());
            promptforroleselection = objPromptForRoleSelection != null ? objPromptForRoleSelection.Value : "true";
            dateformat = ConfigurationManager.AppSettings["DateFormat"];
            timeformat = ConfigurationManager.AppSettings["TimeFormat"];
            timezone = ConfigurationManager.AppSettings["TimeZone"];
            externalvalidationjwtkey = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ExternalValidationKey".ToLower()).Value;
            externalvalidationjwtkeysize = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ExternalValidationKeySize".ToLower()).Value;
            externaljwtissuername = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ExternalIssuerName".ToLower()).Value;
            externaljwtalgorithm = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ExternalSecurityAlgorithm".ToLower()).Value;
            var objquerycachetimeout = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "QueryCacheTimeOut".ToLower());
            querycachetimeout = objquerycachetimeout != null ? objquerycachetimeout.Value : "2";
            var objPreventMultipleLogin = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "PreventMultipleLogin".ToLower());
            preventmultiplelogin = objPreventMultipleLogin != null ? objPreventMultipleLogin.Value : "no";
            var objapplySecurityPolicy = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "applysecuritypolicy");
            applysecuritypolicy = objapplySecurityPolicy != null ? objapplySecurityPolicy.Value : "no";
            var objenforcechangepassword = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "enforcechangepassword ");
            enforcechangepassword = objenforcechangepassword != null ? objenforcechangepassword.Value : "no";
            var objjournaleverything = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "journaleverything");
            journaleverything = objjournaleverything != null ? objjournaleverything.Value : "no";
            var objenablequerycaching = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "enablequerycache");
            enablequerycaching = objenablequerycaching != null ? objenablequerycaching.Value : "no";
            var objAlwaysUseEmailAsUsername = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "alwaysuseemailasusername");
            _AlwaysUseEmailAsUsername = objAlwaysUseEmailAsUsername != null ? objAlwaysUseEmailAsUsername.Value : "no";
            var objuserevalee = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "userevalee");
            userevalee = objuserevalee != null ? objuserevalee.Value : "no";
            var objpasswordexpirationindays = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "passwordexpirationindays ");
            passwordexpirationindays = objpasswordexpirationindays != null ? objpasswordexpirationindays.Value : "0";
            var objscheduledtaskcallbacktime = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "scheduledtaskcallbacktime");
            scheduledtaskcallbacktime = objscheduledtaskcallbacktime != null ? objscheduledtaskcallbacktime.Value : "30";
            //Two factor change
            var objtwofactorauthenticationenable = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twofactorauthenticationenable ");
            twofactorauthenticationenable = objtwofactorauthenticationenable != null ? objtwofactorauthenticationenable.Value : "no";
            var objtwilioaccountsid = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twilioaccountsid");
            twilioaccountsid = objtwilioaccountsid != null ? objtwilioaccountsid.Value : "";
            var objtwilioauthtoken = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twilioauthtoken");
            twilioauthtoken = objtwilioauthtoken != null ? objtwilioauthtoken.Value : "";
            var objtwiliofromnumber = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twiliofromnumber");
            twiliofromnumber = objtwiliofromnumber != null ? objtwiliofromnumber.Value : "";
            var objtwiliocountrycode = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "twiliodefaultcountrycode");
            twiliocountrycode = objtwiliocountrycode != null ? objtwiliocountrycode.Value : "+1";
            //front door
            var objfrontdoorurl = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "frontdoorurl");
            frontdoorurl = objfrontdoorurl != null ? objfrontdoorurl.Value : "";
            var objfrontdoorenable = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "frontdoorenable");
            frontdoorenable = objfrontdoorenable != null ? objfrontdoorenable.Value : "No";
            var objfrontdoorid = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "frontdoorid");
            frontdoorid = objfrontdoorid != null ? objfrontdoorid.Value : "";
            //
            //google API key
            var objgooglemapapikey = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "googlemapapikey".ToLower());
            googlemapapikey = objgooglemapapikey != null ? Decryptdata(objgooglemapapikey.Value) : "";
            var objunits = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "unit".ToLower());
            units = objunits != null ? objunits.Value : "KM";
            //
            var objshowmenuontop = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ShowMenuOnTop".ToLower());
            showmenuontop = objshowmenuontop != null ? objshowmenuontop.Value : "false";
            var objEnableNotification = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "enablenotification"); //added by RP
            enablenotification = objEnableNotification != null ? objEnableNotification.Value : "no";
        }
        string key  = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(@"~/ProductKeys/" + "GemBox.key"));
        GemBox.Document.ComponentInfo.SetLicense((new EncryptDecrypt()).DecryptString(key));
        using(var roleContext = new ApplicationDbContext(true))
        {
            if(roleContext.Roles.FirstOrDefault(p => p.Name == "ReadOnly") != null)
            {
                using(var permissionContext = new PermissionContext())
                {
                    if(permissionContext.Permissions.Where(p => p.RoleName == "ReadOnly").Count() == 0)
                    {
                        foreach(var ent in ModelReflector.Entities)
                        {
                            Permission permission = new Permission();
                            permission.CanAdd = false;
                            permission.CanDelete = false;
                            permission.CanView = true;
                            permission.CanEdit = false;
                            permission.IsOwner = false;
                            permission.SelfRegistration = false;
                            permission.EntityName = ent.Name;
                            permission.RoleName = "ReadOnly";
                            permissionContext.Permissions.Add(permission);
                        }
                        permissionContext.SaveChanges();
                    }
                }
            }
            //
            if(roleContext.Roles.FirstOrDefault(p => p.Name == "CanEdit") != null)
            {
                using(var permissionContext1 = new PermissionContext())
                {
                    if(permissionContext1.Permissions.Where(p => p.RoleName == "CanEdit").Count() == 0)
                    {
                        foreach(var ent in ModelReflector.Entities)
                        {
                            Permission permission = new Permission();
                            permission.CanAdd = true;
                            permission.CanDelete = false;
                            permission.CanView = true;
                            permission.CanEdit = true;
                            permission.IsOwner = false;
                            permission.SelfRegistration = false;
                            permission.EntityName = ent.Name;
                            permission.RoleName = "CanEdit";
                            permissionContext1.Permissions.Add(permission);
                        }
                        permissionContext1.SaveChanges();
                    }
                }
            }
        }
    }
    /// <summary>Gets or sets the instance (creates only when it is null).</summary>
    ///
    /// <value>The instance.</value>
    public static CommonFunction Instance
    {
        get
        {
            if(CommonFunction.instance == null)
                CommonFunction.instance = new CommonFunction();
            return CommonFunction.instance;
        }
        set { }
    }
    /// <summary>Resets the instance (makes instance null).</summary>
    public static void ResetInstance()
    {
        CommonFunction.instance = null;
    }
    /// <summary>Report path for SSRS.</summary>
    ///
    /// <returns>A string.</returns>
    public string ReportPath()
    {
        return reportPath;
    }
    /// <summary>SSRS Report user.</summary>
    ///
    /// <returns>A string.</returns>
    public string ReportUser()
    {
        return reportUser;
    }
    /// <summary>SSRS Report user dcrypted password.</summary>
    ///
    /// <returns>A string.</returns>
    public string ReportPass()
    {
        return (new EncryptDecrypt()).DecryptString(reportPass);
    }
    /// <summary>SSRS Report folder.</summary>
    ///
    /// <returns>A string.</returns>
    public string ReportFolder()
    {
        if(string.IsNullOrEmpty(reportFolder) || reportFolder!= ConfigurationManager.AppSettings["ReportFolder"])
            reportFolder = ConfigurationManager.AppSettings["ReportFolder"];
        return reportFolder;
    }
    /// <summary>Date Format.</summary>
    ///
    /// <returns>A string.</returns>
    public string DateFormat()
    {
        if(string.IsNullOrEmpty(dateformat))
            dateformat = "Default";
        return dateformat;
    }
    /// <summary>Time Format.</summary>
    ///
    /// <returns>A string.</returns>
    public string TimeFormat()
    {
        if(string.IsNullOrEmpty(timeformat))
            timeformat = "Default";
        return timeformat;
    }
    /// <summary>Date Format.</summary>
    ///
    /// <returns>A string.</returns>
    public string TimeZone()
    {
        if(string.IsNullOrEmpty(timezone))
            timezone = "Default";
        return timezone;
    }
    /// <summary>Gets Administrator roles.</summary>
    ///
    /// <returns>A string.</returns>
    public string AdministratorRoles()
    {
        return administratorRoles;
    }
    /// <summary>Checks if application uses Windows AD authentication.</summary>
    ///
    /// <returns>A string.</returns>
    public string UseActiveDirectory()
    {
        return useActiveDirectory;
    }
    /// <summary>Use Windows AD authentication role.</summary>
    ///
    /// <returns>A string.</returns>
    public string UseActiveDirectoryRole()
    {
        return useActiveDirectoryRole;
    }
    /// <summary>Use Windows AD authentication domain name.</summary>
    ///
    /// <returns>A string.</returns>
    public string DomainName()
    {
        return domainName;
    }
    /// <summary>Use shared user system for login (feature removed).</summary>
    ///
    /// <returns>A string.</returns>
    public string NeedSharedUserSystem()
    {
        if(string.IsNullOrEmpty(needSharedUserSystem))
            needSharedUserSystem ="no";
        return needSharedUserSystem;
    }
    /// <summary>Gets the hosting server name.</summary>
    ///
    /// <returns>A string.</returns>
    public string Server()
    {
        if(string.IsNullOrEmpty(server))
        {
            server = ConfigurationManager.AppSettings["server"];
        }
        else if(server != ConfigurationManager.AppSettings["server"])
        {
            server = ConfigurationManager.AppSettings["server"];
        }
        return server;
    }
    /// <summary>Gets the hosted url.</summary>
    ///
    /// <returns>A string.</returns>
    public string AppURL()
    {
        if(string.IsNullOrEmpty(appURL))
        {
            appURL = ConfigurationManager.AppSettings["AppURL"];
        }
        else if(appURL != ConfigurationManager.AppSettings["AppURL"])
        {
            appURL = ConfigurationManager.AppSettings["AppURL"];
        }
        return appURL;
    }
    /// <summary>Gets the application name (used on log and layout master screens).</summary>
    ///
    /// <returns>A string.</returns>
    public string AppName()
    {
        if(string.IsNullOrEmpty(appName))
            appName = ConfigurationManager.AppSettings["appName"];
        return appName;
    }
    /// <summary>Is multiple role selection enabled.</summary>
    ///
    /// <returns>A string.</returns>
    public string MultipleRoleSelection()
    {
        return multipleRoleSelection;
    }
    /// <summary>Enable prototyping tool.</summary>
    ///
    /// <returns>A string.</returns>
    public string EnablePrototypingTool()
    {
        return enablePrototypingTool;
    }
    /// <summary>Get or Set maintenance mode for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string MaintenanceMode()
    {
        if(string.IsNullOrEmpty(maintenanceMode))
            maintenanceMode = "false";
        return maintenanceMode;
    }
    /// <summary>Maintenance mode roles (users in maintenance mode can use app).</summary>
    ///
    /// <returns>A string.</returns>
    public string MaintenanceModeRoles()
    {
        if(string.IsNullOrEmpty(maintenanceModeRoles))
            maintenanceModeRoles = "Admin";
        return maintenanceModeRoles;
    }
    /// <summary>Maintenance mode alert message for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string MaintenanceModeAlertMessage()
    {
        if(string.IsNullOrEmpty(maintenanceModeAlertMessage))
            maintenanceModeAlertMessage = "The application is undergoing maintenance. Please try again later.";
        return maintenanceModeAlertMessage;
    }
    /// <summary>GPS enabled (mobile web application).</summary>
    ///
    /// <returns>A string.</returns>
    public string GPSEnabled()
    {
        if(string.IsNullOrEmpty(gpsenabled))
            gpsenabled = "false";
        return gpsenabled;
    }
    /// <summary>Enable create an account.</summary>
    ///
    /// <returns>A string.</returns>
    public string EnableCreateAnAccount()
    {
        if(string.IsNullOrEmpty(createanAccount))
            createanAccount = "false";
        return createanAccount;
    }
    /// <summary>Application Session TimeOut interval.</summary>
    ///
    /// <returns>A string.</returns>
    public string ApplicationSessionTimeOut()
    {
        return applicationSessiontimeout;
    }
    /// <summary>Application Session TimeOut alert message.</summary>
    ///
    /// <returns>A string.</returns>
    public string ApplicationSessionTimeOutAlert()
    {
        if(string.IsNullOrEmpty(applicationSessiontimeoutAlert))
            applicationSessiontimeoutAlert = "false";
        return applicationSessiontimeoutAlert;
    }
    /// <summary>External Validation Key.</summary>
    ///
    /// <returns>A string.</returns>
    public string ExternalValidationKey()
    {
        return externalvalidationjwtkey;
    }
    /// <summary>External Validation Key Size.</summary>
    ///
    /// <returns>A string.</returns>
    public string ExternalValidationKeySize()
    {
        return externalvalidationjwtkeysize;
    }
    /// <summary>External Issuer Name.</summary>
    ///
    /// <returns>A string.</returns>
    public string ExternalIssuerName()
    {
        return externaljwtissuername;
    }
    /// <summary>External External Security Algorithm.</summary>
    ///
    /// <returns>A string.</returns>
    public string ExternalSecurityAlgorithm()
    {
        return externaljwtalgorithm;
    }
    /// <summary>QueryCacheTimeOut.</summary>
    ///
    /// <returns>A string.</returns>
    public string QueryCacheTimeOut()
    {
        if(string.IsNullOrEmpty(querycachetimeout))
            return  "1";
        return querycachetimeout;
    }
    /// <summary>PreventMultipleLogin.</summary>
    ///
    /// <returns>A string.</returns>
    public string PreventMultipleLogin()
    {
        if(string.IsNullOrEmpty(preventmultiplelogin))
            return  "no";
        return preventmultiplelogin;
    }
    /// <summary>JournalEverything.</summary>
    ///
    /// <returns>A string.</returns>
    public bool JournalEverything()
    {
        if(string.IsNullOrEmpty(journaleverything))
            return  false;
        return journaleverything.ToLower()=="yes"?true:false;
    }
    /// <summary>EnableQueryCache.</summary>
    ///
    /// <returns>A boolean.</returns>
    public bool EnableQueryCache()
    {
        if(string.IsNullOrEmpty(enablequerycaching))
            return false;
        return enablequerycaching.ToLower() == "yes" ? true : false;
    }
    
    /// <summary>UseRevalee.</summary>
    ///
    /// <returns>A boolean.</returns>
    public bool UseRevalee()
    {
        if(string.IsNullOrEmpty(userevalee))
            return false;
        return userevalee.ToLower() == "yes" ? true : false;
    }
    /// <summary>ScheduledTaskCallbackTime.</summary>
    ///
    /// <returns>A dboule.</returns>
    public double ScheduledTaskCallbackTime()
    {
        if(string.IsNullOrEmpty(scheduledtaskcallbacktime))
            return 30;
        double result = 30;
        if(double.TryParse(scheduledtaskcallbacktime, out result))
            return result;
        return 30;
    }
    
    
    /// <summary>AlwaysUseEmailAsUsername.</summary>
    ///
    /// <returns>A string.</returns>
    public bool AlwaysUseEmailAsUsername()
    {
        if(string.IsNullOrEmpty(_AlwaysUseEmailAsUsername))
            return  false;
        return _AlwaysUseEmailAsUsername.ToLower()=="yes"?true:false;
    }
    
    /// <summary>Get or Set applysecuritypolicy for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string ApplySecurityPolicy()
    {
        if(string.IsNullOrEmpty(applysecuritypolicy))
            applysecuritypolicy = "false";
        return applysecuritypolicy.ToLower() == "yes"?"true":"false";
    }
    /// <summary>Get or Set enforcechangepassword for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string EnforceChangePassword()
    {
        if(string.IsNullOrEmpty(enforcechangepassword))
            enforcechangepassword = "false";
        return enforcechangepassword.ToLower() == "yes" ? "true" : "false"; ;
    }
    public int PasswordExpirationInDays()
    {
        if(string.IsNullOrEmpty(passwordexpirationindays))
            return 0;
        int result = 0;
        if(int.TryParse(passwordexpirationindays, out result))
            return result;
        return 0;
    }
    
    //Two factor change
    /// <summary>Get or Set TwoFactorAuthenticationEnabled for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string TwoFactorAuthenticationEnabled()
    {
        if(string.IsNullOrEmpty(twofactorauthenticationenable))
            twofactorauthenticationenable = "false";
        return twofactorauthenticationenable.ToLower() == "yes" ? "true" : "false";
    }
    // <summary>Get or Set Twilio Account SID for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string TwilioAccountSID()
    {
        if(string.IsNullOrEmpty(twilioaccountsid))
            twilioaccountsid = ConfigurationManager.AppSettings["TwilioAccountSID"];
        return twilioaccountsid;
    }
    
    /// <summary>Get or Set Twilio Auth Token for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string TwilioAuthToken()
    {
        if(string.IsNullOrEmpty(twilioauthtoken))
            twilioauthtoken = ConfigurationManager.AppSettings["TwilioAuthToken"];
        return twilioauthtoken;
    }
    
    /// <summary>Get or Set Twilio From Number for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string TwilioFromNumber()
    {
        if(string.IsNullOrEmpty(twiliofromnumber))
            twiliofromnumber = ConfigurationManager.AppSettings["TwilioFromNumber"];
        return twiliofromnumber;
    }
    /// <summary>Get or Set Twilio Country Code for application.</summary>
    ///
    /// <returns>A string.</returns>
    public string TwilioCountryCode()
    {
        if(string.IsNullOrEmpty(twiliocountrycode))
            twiliocountrycode = ConfigurationManager.AppSettings["TwilioCountryCode"];
        return twiliocountrycode;
    }
    
    public string EnableNotification()
    {
        if(string.IsNullOrEmpty(enablenotification))
            enablenotification = "false";
        return enablenotification.ToLower() == "yes" ? "true" : "false";
    }
    
    
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string FrontDoorUrl()
    {
        if(string.IsNullOrEmpty(frontdoorurl))
            frontdoorurl = "<Your Front Door Url>";
        return frontdoorurl;
    }
    
    public string FrontDoorEnable()
    {
        if(string.IsNullOrEmpty(frontdoorenable))
            frontdoorenable = "No";
        return frontdoorenable;
    }
    public string FrontDoorId()
    {
        if(string.IsNullOrEmpty(frontdoorid))
            frontdoorid = "<Your Front Door Id>";
        return frontdoorid;
    }
    public bool ShowMenuOnTop()
    {
        if(string.IsNullOrEmpty(showmenuontop))
            return false;
        return showmenuontop.ToLower() == "true" ? true : false;
    }
    
    public CompanyProfile GetCompanyProfile(string UserId)
    {
        if(string.IsNullOrEmpty(UserId))
            return getCompanyProfile(new SystemUser());
        return getCompanyProfile(new SystemUser());
    }
    public CompanyProfile getCompanyProfile(IUser user)
    {
        CompanyProfile company = CompanyInformationValue(user);
        return company;
    }
    
    public OneDriveHelper OneDrive(IUser user)
    {
        var companyprofile = getCompanyProfile(user);
        return new OneDriveHelper(companyprofile);
    }
    /// <summary>Get default entity page.</summary>
    ///
    /// <returns>A string.</returns>
    public bool getDefaultEntityPage(string entityname)
    {
        bool flag = false;
        bool isdeleted = false;
        using(var entitydb = (new ApplicationContext(new SystemUser())))
        {
            var objDefName = entitydb.DefaultEntityPages;
            if(objDefName.Count() > 0)
            {
                if(entityname == "Home")
                {
                    foreach(var item in objDefName)
                    {
                        var isDisable = item.Flag == null ? false : item.Flag.Value;
                        if(item.PageType != "Home" && (item.HomePage != null && !isDisable))
                        {
                            flag = isDisable;
                            break;
                        }
                    }
                }
                else
                {
                    var objNameEntityHome = objDefName.Where(p => p.EntityName == entityname);
                    if(objNameEntityHome.Count() > 0)
                        flag = objNameEntityHome.FirstOrDefault().Flag == null ? false : objNameEntityHome.FirstOrDefault().Flag.Value; ;
                }
            }
            else
                return true;
        }
        return flag;
    }
    /// <summary>Get Base Uri for email and other feature.</summary>
    ///
    /// <returns>A string.</returns>
    public string getBaseUri()
    {
        string baseUri = "";
        Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
        string pathQuery = uri.PathAndQuery;
        string hostName = uri.ToString().Replace(pathQuery, "");
        var virtualurl = VirtualPathUtility.ToAbsolute("~/");
        if(virtualurl == "/" || string.IsNullOrEmpty(virtualurl))
            baseUri = hostName;
        else
            baseUri = virtualurl;
        var fdenable = CommonFunction.Instance.FrontDoorEnable();
        if(fdenable.ToLower() == "yes")
        {
            var fdurl = CommonFunction.Instance.FrontDoorUrl();
            baseUri = fdurl;
        }
        return baseUri;
    }
    /// <summary>Application theme.</summary>
    ///
    /// <returns>A string.</returns>
    public string getAppThemeName()
    {
        var theme = ConfigurationManager.AppSettings["AppTheme"];
        if(theme == null || theme == "DefaultCompress")
        {
            theme = "Default";    //Change By Ashok 3/31/2020
        }
        return theme;
    }
    /// <summary>Application theme name for super admin.</summary>
    ///
    /// <returns>A string.</returns>
    public string getAppThemeNameAdmin()
    {
        var theme = ConfigurationManager.AppSettings["AppTheme"];
        if(theme == null) theme = "Default";
        if(theme == "Angular") theme = "";
        if(theme == "DefaultCompress") theme = "DefaultCompress";  //Change By Ashok 3/31/2020
        return theme;
    }
    /// <summary>Uri for saving favorite items.</summary>
    ///
    /// <returns>A string.</returns>
    public string getBaseForFavoriteUri()
    {
        //string baseUri = "";
        //Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
        //string pathQuery = uri.PathAndQuery;
        //string hostName = uri.ToString().Replace(pathQuery, "");
        //var virtualurl = VirtualPathUtility.ToAbsolute("~/");
        //if (virtualurl == "/" || string.IsNullOrEmpty(virtualurl))
        //    baseUri = pathQuery.ToString().Replace(hostName, "/");
        //else
        //    baseUri = pathQuery.ToString().Replace(virtualurl, ""); ;
        //return baseUri;
        return HttpContext.Current.Request.Url.PathAndQuery;
    }
    /// <summary>Tenant related site.css script name.</summary>
    ///
    /// <returns>A string.</returns>
    public string getTenantSiteScript(IUser user)
    {
        var result = "Site" + getAppThemeNameAdmin() + ".css";
        return result;
    }
    /// <summary>Get tenantId of user.</summary>
    ///
    /// <returns>A string.</returns>
    public string getTenantId(IUser user)
    {
        var result = string.Empty;
        return result;
    }
    
    /// <summary>Get tenant list.</summary>
    ///
    /// <returns>A string.</returns>
    public Dictionary<string, string> getTenantList(IUser user)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        return null;
    }
    
    /// <summary>googlemapapikey.</summary>
    ///
    /// <returns>A string.</returns>
    public string GetGoogleMapApiKey()
    {
        if(string.IsNullOrEmpty(googlemapapikey))
            return "";
        return googlemapapikey;
    }
    /// <summary>GetUnit.</summary>
    ///
    /// <returns>A string.</returns>
    public string GetUnit()
    {
        if(string.IsNullOrEmpty(units))
            return "KM";
        return units;
    }
    /// <summary>Enable google analytics.</summary>
    ///
    /// <returns>A string.</returns>
    public string EnableGoogleAnalytics()
    {
        if(string.IsNullOrEmpty(enableGA) || enableGA != ConfigurationManager.AppSettings["enableGA"])
            enableGA = ConfigurationManager.AppSettings["enableGA"];
        return enableGA;
    }
    /// <summary>Tracking ID for google analytics.</summary>
    ///
    /// <returns>A string.</returns>
    public string TrackingID()
    {
        if(string.IsNullOrEmpty(trackingID) || enableGA != ConfigurationManager.AppSettings["trackingID"])
            trackingID = ConfigurationManager.AppSettings["trackingID"];
        return trackingID;
    }
    /// <summary>Get CustomDimensionName for google analytics.</summary>
    ///
    /// <returns>A string.</returns>
    public string CustomDimensionName()
    {
        if(string.IsNullOrEmpty(customdimensionname) || enableGA != ConfigurationManager.AppSettings["customdimensionname"])
            customdimensionname = ConfigurationManager.AppSettings["customdimensionname"];
        return customdimensionname;
    }
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public bool HasThirdParty()
    {
        return false;
    }
    /// <summary>Enable Soft Delete feature.</summary>
    ///
    /// <returns>A string.</returns>
    public string SoftDeleteEnabled()
    {
        if(string.IsNullOrEmpty(softDeleteEnabled))
            softDeleteEnabled = "false";
        return softDeleteEnabled;
    }
    /// <summary>Enable role selection prompt for multiple roles.</summary>
    ///
    /// <returns>A string.</returns>
    public string PromptForRoleSelection()
    {
        if(string.IsNullOrEmpty(promptforroleselection))
            promptforroleselection = "true";
        return promptforroleselection;
    }
    /// <summary>Legal document file name.</summary>
    ///
    /// <returns>A string.</returns>
    public string Legal(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.LegalInformation))
            return profile.LegalInformation;
        return string.Empty;
    }
    /// <summary>Legal document file name link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string LegalLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.LegalInformationLink))
            return profile.LegalInformationLink;
        return string.Empty;
    }
    /// <summary>Attach document file name.</summary>
    ///
    /// <returns>A string.</returns>
    public string LegalAttach(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.LegalInformationAttach))
            return profile.LegalInformationAttach;
        return string.Empty;
    }
    /// <summary>Show legal footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string LegalDisplay(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.LegalInformationDisplay))
            return profile.LegalInformationDisplay;
        return string.Empty;
    }
    /// <summary>Policy document file name in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string Policy(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.PrivacyPolicy))
            return profile.PrivacyPolicy;
        return string.Empty;
    }
    /// <summary>Policy document file link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string PolicyLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.PrivacyPolicyLink))
            return profile.PrivacyPolicyLink;
        return string.Empty;
    }
    /// <summary>Attach Policy document file name in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string PolicyAttach(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.PrivacyPolicyAttach))
            return profile.PrivacyPolicyAttach;
        return string.Empty;
    }
    /// <summary>Display Policy document file in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string PolicyDisplay(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.PrivacyPolicyDisplay))
            return profile.PrivacyPolicyDisplay;
        return string.Empty;
    }
    /// <summary>Terms and conditions in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string Terms(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.TermsOfService))
            return profile.TermsOfService;
        return string.Empty;
    }
    /// <summary>Terms and conditions link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string TermsLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.TermsOfServiceLink))
            return profile.TermsOfServiceLink;
        return string.Empty;
    }
    /// <summary>Terms and conditions in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string TermsAttach(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.TermsOfServiceAttach))
            return profile.TermsOfServiceAttach;
        return string.Empty;
    }
    /// <summary>Show Terms and conditions in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string TermsDisplay(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.TermsOfServiceDisplay))
            return profile.TermsOfServiceDisplay;
        return string.Empty;
    }
    /// <summary>Third party in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string ThirdParty(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.ThirdParty))
            return profile.ThirdParty;
        return string.Empty;
    }
    /// <summary>Third party link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string ThirdPartyLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.ThirdPartyLink))
            return profile.ThirdPartyLink;
        return string.Empty;
    }
    /// <summary>Attach Third party in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string ThirdPartyAttach(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.ThirdPartyAttach))
            return profile.ThirdPartyAttach;
        return string.Empty;
    }
    /// <summary>Attach Third party in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string ThirdPartyDisplay(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.ThirdPartyDisplay))
            return profile.ThirdPartyDisplay;
        return string.Empty;
    }
    /// <summary>Cookie policy document in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string Cookie(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CookieInformation))
            return profile.CookieInformation;
        return string.Empty;
    }
    /// <summary>Cookie policy document link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CookieLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CookieInformationLink))
            return profile.CookieInformationLink;
        return string.Empty;
    }
    /// <summary>Cookie policy document link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CookieAttach(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CookieInformationAttach))
            return profile.CookieInformationAttach;
        return string.Empty;
    }
    /// <summary>Show Cookie policy document link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CookieDisplay(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CookieInformationDisplay))
            return profile.CookieInformationDisplay;
        return string.Empty;
    }
    /// <summary>Email to link in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string Emailto(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.Emailto))
            return profile.Emailto;
        return string.Empty;
    }
    /// <summary>Email-to email address in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string EmailtoAddress(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.EmailtoAddress))
            return profile.EmailtoAddress;
        return string.Empty;
    }
    /// <summary>created by in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CreatedBy(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CreatedBy))
            return profile.CreatedBy;
        return string.Empty;
    }
    /// <summary>created by name in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CreatedByName(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CreatedByName))
            return profile.CreatedByName;
        return string.Empty;
    }
    /// <summary>created by url in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public string CreatedByLink(IUser user)
    {
        var profile = getCompanyProfile(user);
        if(!string.IsNullOrEmpty(profile.CreatedByLink))
            return profile.CreatedByLink;
        return string.Empty;
    }
    /// <summary>privacy policy in footer.</summary>
    ///
    /// <returns>A string.</returns>
    public bool IsPrivacyPolicy(IUser user)
    {
        bool haspolicy = false;
        var profile = getCompanyProfile(user);
        if(profile.LegalInformation != null || profile.PrivacyPolicy != null
                || profile.TermsOfService != null || profile.Emailto != null
                || profile.CreatedBy != null)
        {
            haspolicy = true;
        }
        return haspolicy;
    }
    /// <summary>Set Tenant ViewBag.</summary>
    ///
    /// <returns>A SelectList.</returns>
    public System.Web.Mvc.SelectList SetTenantViewBag(GeneratorBase.MVC.Models.IUser user, long? tenantId)
    {
        return null;
    }
    
    /// <summary>Set Tenant ViewBag.</summary>
    ///
    /// <returns>A SelectList.</returns>
    public System.Web.Mvc.SelectList SetSelectListTenantViewBag(GeneratorBase.MVC.Models.IUser user, long? tenantId)
    {
        return null;
    }
    
    
    
    /// <summary>
    ///
    /// </summary>
    /// <param name="compInfo"></param>
    /// <returns>For Compay Profile New</returns>
    public CompanyProfile CompanyInformationValue(IUser user)
    {
        CompanyProfile comp = new CompanyProfile();
        long? TenantId = 0;
        string Type = "Global";
        string Name = "Turanto";
        string Email = "Contact@turanto.com";
        string Address = "5388 Twin Hickory Rd";
        string Country = "USA";
        string State = "VA";
        string City = "Glen Allen";
        string Zip = "23059";
        string ContactNumber1 = "1.866.591.5906";
        string ContactNumber2 = "1.866.591.5906";
        //smtp setting
        int SMTPPort = 465;
        string SMTPServer = "smtp.sendgrid.net";
        string SMTPPassword = Decryptdata("VHVyYW50bzIxNiM=");
        string SMTPUser = "azure_15eb8f52958d116ced4774522747a486@azure.com";
        bool SSL = true, Use, UseAnonymous = false;
        //master page icon
        string Icon = "logo.gif";
        string IconHeight = "28px";
        string IconWidth = "28px";
        //
        //login logo page
        string Logo = "logo_white.png";
        string LogoWidth = "155px";
        string LogoHeight = "29px";
        //
        string LoginBg = "Loginbg.png";
        string AboutCompany = "About Company";
        //LegalInformation
        string LegalInformation = "Legal Information";
        string LegalInformationLink = "/PolicyAndService/Licensing.pdf";
        string LegalInformationAttach = "Licensing.pdf";
        string LegalInformationDisplay = "1";
        //end
        //PrivacyPolicy
        string PrivacyPolicy = "Privacy Policy";
        string PrivacyPolicyLink = "/PolicyAndService/PrivacyPolicy.pdf";
        string PrivacyPolicyAttach = "PrivacyPolicy.pdf";
        string PrivacyPolicyDisplay = "1";
        //End
        //Terms Of Service
        string TermsOfService = "Terms Of Service";
        string TermsOfServiceLink = "/PolicyAndService/Terms_Of_Service.pdf";
        string TermsOfServiceAttach = "Terms_Of_Service.pdf";
        string TermsOfServiceDisplay = "1";
        //End
        //Third-Party Licenses
        string ThirdParty = "Third-Party Licenses";
        string ThirdPartyLink = "/PolicyAndService/Third_Party_Licenses.pdf";
        string ThirdPartyAttach = "Third_Party_Licenses.pdf";
        string ThirdPartyDisplay = "1";
        //End
        //Cookie Information
        string CookieInformation = "Cookie Policy";
        string CookieInformationLink = "/PolicyAndService/CookiePolicy.pdf";
        string CookieInformationAttach = "CookiePolicy.pdf";
        string CookieInformationDisplay = "1";
        //end
        //Emailto
        string Emailto = "Email To";
        string EmailtoAddress = "contact@turanto.com";
        //End
        //Create By
        string CreatedBy = "Created With";
        string CreatedByName = "Turanto";
        string CreatedByLink = "http://www.turanto.com/";
        string Disclaimer = "Disclaimer : This computer system is the property of Etelic and is intended for authorized users only.";
        string OneDriveClientId = null;
        string OneDriveSecret = null;
        string OneDriveTenantId = null;
        string OneDriveUserName = null;
        string OneDrivePassword = null;
        string OneDriveFolderName = null;
        IUser newuser = user;
        if(string.IsNullOrEmpty(user.Name))
            newuser = new SystemUser();
        using(var dbContext = new ApplicationContext(newuser))
        {
            List<CompanyInformation> compInfo = dbContext.CompanyInformations.Include(u => u.companyinformationfootersectionassociation).Include(u => u.CompanyInformationCompanyListAssociation_companyinformation).GetFromCache<IQueryable<CompanyInformation>, CompanyInformation>().ToList();
            if(compInfo.Count() == 0)
                compInfo = new ApplicationContext(new SystemUser()).CompanyInformations.Include(u => u.companyinformationfootersectionassociation).Include(u => u.CompanyInformationCompanyListAssociation_companyinformation).GetFromCache<IQueryable<CompanyInformation>, CompanyInformation>().ToList();
            comp = GetCompanyProfileForAll(TenantId, Type, Name, Email, Address, Country, State, City, Zip, ContactNumber1, ContactNumber2, SMTPServer, SMTPPassword,
                                           SMTPPort, SSL, AboutCompany, IconWidth, IconHeight, LogoWidth, LogoHeight, LoginBg, Icon, Logo, LegalInformation, LegalInformationLink, LegalInformationAttach,
                                           LegalInformationDisplay, PrivacyPolicy, PrivacyPolicyLink, PrivacyPolicyAttach,
                                           PrivacyPolicyDisplay, TermsOfService, TermsOfServiceLink, TermsOfServiceAttach, TermsOfServiceDisplay, ThirdParty, ThirdPartyLink, ThirdPartyAttach, ThirdPartyDisplay,
                                           CookieInformation, CookieInformationLink, CookieInformationAttach, CookieInformationDisplay, Emailto, EmailtoAddress, CreatedBy, CreatedByName,
                                           CreatedByLink, Disclaimer, SMTPUser,
                                           UseAnonymous, compInfo, OneDriveClientId, OneDriveSecret, OneDriveTenantId, OneDriveUserName, OneDrivePassword, OneDriveFolderName);
            return comp;
        }
    }
    public CompanyProfile GetCompanyProfileForAll(long? TenantId, string Type, string Name, string Email, string Address, string Country, string State, string City, string Zip, string ContactNumber1, string ContactNumber2, string SMTPServer, string SMTPPassword, int SMTPPort, bool SSL, string
            AboutCompany, string IconWidth, string IconHeight, string LogoWidth, string LogoHeight, string LoginBg, string Icon, string Logo, string LegalInformation, string LegalInformationLink, string LegalInformationAttach, string
            LegalInformationDisplay, string PrivacyPolicy, string PrivacyPolicyLink, string PrivacyPolicyAttach, string
            PrivacyPolicyDisplay, string TermsOfService, string TermsOfServiceLink, string TermsOfServiceAttach, string TermsOfServiceDisplay, string ThirdParty, string ThirdPartyLink, string ThirdPartyAttach, string ThirdPartyDisplay, string
            CookieInformation, string CookieInformationLink, string CookieInformationAttach, string CookieInformationDisplay, string Emailto, string EmailtoAddress, string CreatedBy, string CreatedByName, string
            CreatedByLink, string Disclaimer, string SMTPUser, bool UseAnonymous, List<CompanyInformation> compInfolist, string OneDriveClientId, string OneDriveSecret, string OneDriveTenantId, string OneDriveUserName, string OneDrivePassword, string OneDriveFolderName)
    {
        if(compInfolist.Count() > 0 && compInfolist != null)
        {
            CompanyInformation compInfo = compInfolist.FirstOrDefault();
            TenantId = compInfo.CompanyInformationCompanyListAssociation_companyinformation.FirstOrDefault().TenantId > 0 ? compInfo.CompanyInformationCompanyListAssociation_companyinformation.FirstOrDefault().TenantId : TenantId;
            SMTPPort = compInfo.SMTPPort != null ? Convert.ToInt32(compInfo.SMTPPort) : SMTPPort;
            SSL = compInfo.SMTPPort != null ? compInfo.SSL.Value : SSL;
            SMTPUser = !string.IsNullOrEmpty(compInfo.SMTPUser) ? compInfo.SMTPUser : SMTPUser;
            UseAnonymous = compInfo.UseAnonymous != null ? compInfo.UseAnonymous.Value : UseAnonymous;
            Type = compInfo.CompanyInformationCompanyListAssociation_companyinformation.FirstOrDefault().TenantId != 0 ? compInfo.CompanyInformationCompanyListAssociation_companyinformation.FirstOrDefault().DisplayValue : Type;
            Name = !string.IsNullOrEmpty(compInfo.CompanyName) ? compInfo.CompanyName : Name;
            Email = !string.IsNullOrEmpty(compInfo.CompanyEmail) ? compInfo.CompanyEmail : Email;
            Address = !string.IsNullOrEmpty(compInfo.CompanyAddress) ? compInfo.CompanyAddress : Address;
            Country = !string.IsNullOrEmpty(compInfo.CompanyCountry) ? compInfo.CompanyCountry : Country;
            State = !string.IsNullOrEmpty(compInfo.CompanyState) ? compInfo.CompanyState : State;
            City = !string.IsNullOrEmpty(compInfo.CompanyCity) ? compInfo.CompanyCity : City;
            Zip = !string.IsNullOrEmpty(compInfo.CompanyZipCode) ? compInfo.CompanyZipCode : Zip;
            ContactNumber1 = !string.IsNullOrEmpty(compInfo.ContactNumber1) ? compInfo.ContactNumber1 : ContactNumber1;
            ContactNumber2 = !string.IsNullOrEmpty(compInfo.ContactNumber2) ? compInfo.ContactNumber1 : ContactNumber1;
            SMTPServer = !string.IsNullOrEmpty(compInfo.SMTPServer) ? compInfo.SMTPServer : SMTPServer;
            SMTPPassword = !string.IsNullOrEmpty(compInfo.SMTPPassword) ? Decryptdata(compInfo.SMTPPassword) : SMTPPassword;
            Icon = compInfo.Icon != null ? compInfo.Icon.Value.ToString() : Icon;
            IconHeight = !string.IsNullOrEmpty(compInfo.IconHeight) ? compInfo.IconHeight : IconHeight;
            IconWidth = !string.IsNullOrEmpty(compInfo.IconWidth) ? compInfo.IconWidth : IconWidth;
            //login logo page
            Logo = compInfo.Logo != null ? compInfo.Logo.Value.ToString() : Logo;
            LogoWidth = !string.IsNullOrEmpty(compInfo.LogoWidth) ? compInfo.LogoWidth : LogoWidth;
            LogoHeight = !string.IsNullOrEmpty(compInfo.LogoHeight) ? compInfo.LogoHeight : LogoHeight;
            LoginBg = compInfo.LoginBg != null ? compInfo.LoginBg.Value.ToString() : LoginBg;
            AboutCompany = !string.IsNullOrEmpty(compInfo.AboutCompany) ? compInfo.AboutCompany : AboutCompany;
            OneDriveClientId = compInfo.OneDriveClientId;
            OneDriveSecret = compInfo.OneDriveSecret;
            OneDriveTenantId = compInfo.OneDriveTenantId;
            OneDriveUserName = compInfo.OneDriveUserName;
            OneDrivePassword = compInfo.OneDrivePassword;
            OneDriveFolderName = compInfo.OneDriveFolderName;
            if(compInfo.companyinformationfootersectionassociation != null)
            {
                var legal = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "legal information").FirstOrDefault();
                if(legal != null)
                {
                    LegalInformation = legal.Name;
                    LegalInformationLink = !string.IsNullOrEmpty(legal.WebLink) ? LegalInformationLink : LegalInformationLink;
                    LegalInformationLink = legal.DocumentUpload != null ? "/Document/Download/" + legal.DocumentUpload : LegalInformationLink;
                    LegalInformationDisplay = legal.AssociatedFooterSectionTypeID != null ? legal.AssociatedFooterSectionTypeID.ToString() : LegalInformationDisplay;
                    //LegalInformationAttach=
                }
                var privacypolicy = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "privacy policy").FirstOrDefault();
                if(privacypolicy != null)
                {
                    PrivacyPolicy = privacypolicy.Name;
                    PrivacyPolicyLink = !string.IsNullOrEmpty(privacypolicy.WebLink) ? PrivacyPolicyLink : PrivacyPolicyLink;
                    PrivacyPolicyLink = privacypolicy.DocumentUpload != null ? "/Document/Download/" + privacypolicy.DocumentUpload : PrivacyPolicyLink;
                    PrivacyPolicyDisplay = privacypolicy.AssociatedFooterSectionTypeID != null ? privacypolicy.AssociatedFooterSectionTypeID.ToString() : PrivacyPolicyDisplay;
                    //PrivacyPolicyAttach=
                }
                var termsOfservice = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "terms of service").FirstOrDefault();
                if(termsOfservice != null)
                {
                    TermsOfService = termsOfservice.Name;
                    TermsOfServiceLink = !string.IsNullOrEmpty(termsOfservice.WebLink) ? TermsOfServiceLink : TermsOfServiceLink;
                    TermsOfServiceLink = termsOfservice.DocumentUpload != null ? "/Document/Download/" + termsOfservice.DocumentUpload : TermsOfServiceLink;
                    TermsOfServiceDisplay = termsOfservice.AssociatedFooterSectionTypeID != null ? termsOfservice.AssociatedFooterSectionTypeID.ToString() : TermsOfServiceDisplay;
                    //TermsOfServiceLinkAttach=
                }
                var thirdParty = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "third-party licenses").FirstOrDefault();
                if(thirdParty != null)
                {
                    ThirdParty = thirdParty.Name;
                    ThirdPartyLink = !string.IsNullOrEmpty(thirdParty.WebLink) ? ThirdPartyLink : ThirdPartyLink;
                    ThirdPartyLink = thirdParty.DocumentUpload != null ? "/Document/Download/" + thirdParty.DocumentUpload : ThirdPartyLink;
                    ThirdPartyDisplay = thirdParty.AssociatedFooterSectionTypeID != null ? thirdParty.AssociatedFooterSectionTypeID.ToString() : ThirdPartyDisplay;
                    //ThirdPartyLinkAttach=
                }
                var cookieInformation = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "cookie policy").FirstOrDefault();
                if(cookieInformation != null)
                {
                    CookieInformation = cookieInformation.Name;
                    CookieInformationLink = !string.IsNullOrEmpty(cookieInformation.WebLink) ? CookieInformationLink : CookieInformationLink;
                    CookieInformationLink = cookieInformation.DocumentUpload != null ? "/Document/Download/" + cookieInformation.DocumentUpload : CookieInformationLink;
                    CookieInformationDisplay = cookieInformation.AssociatedFooterSectionTypeID != null ? cookieInformation.AssociatedFooterSectionTypeID.ToString() : CookieInformationDisplay;
                    //CookieInformationLinkAttach=
                }
                var emailto = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "email to").FirstOrDefault();
                if(emailto != null)
                {
                    Emailto = !string.IsNullOrEmpty(emailto.WebLinkTitle) ? emailto.WebLinkTitle : Emailto;
                    EmailtoAddress = !string.IsNullOrEmpty(emailto.WebLink) ? emailto.WebLink : EmailtoAddress;
                }
                var createdWith = compInfo.companyinformationfootersectionassociation.Where(p => p.Name.ToLower() == "created with").FirstOrDefault();
                if(createdWith != null)
                {
                    CreatedBy = !string.IsNullOrEmpty(createdWith.Name) ? createdWith.Name : CreatedBy;
                    CreatedByName = !string.IsNullOrEmpty(createdWith.WebLinkTitle) ? createdWith.WebLinkTitle : CreatedByName;
                    CreatedByLink = !string.IsNullOrEmpty(createdWith.WebLink) ? createdWith.WebLink : CreatedByLink;
                }
            }
            Disclaimer = !string.IsNullOrEmpty(compInfo.Disclaimer) ? compInfo.Disclaimer : Disclaimer;
        }
        CompanyProfile comp = new CompanyProfile(TenantId, Type, Name, Email, Address, Country, State, City, Zip, ContactNumber1, ContactNumber2, SMTPServer, SMTPPassword, SMTPPort, SSL,
                AboutCompany, IconWidth, IconHeight, LogoWidth, LogoHeight, LoginBg, Icon, Logo, LegalInformation, LegalInformationLink, LegalInformationAttach,
                LegalInformationDisplay, PrivacyPolicy, PrivacyPolicyLink, PrivacyPolicyAttach,
                PrivacyPolicyDisplay, TermsOfService, TermsOfServiceLink, TermsOfServiceAttach, TermsOfServiceDisplay, ThirdParty, ThirdPartyLink, ThirdPartyAttach, ThirdPartyDisplay,
                CookieInformation, CookieInformationLink, CookieInformationAttach, CookieInformationDisplay, Emailto, EmailtoAddress, CreatedBy, CreatedByName,
                CreatedByLink, Disclaimer, SMTPUser, UseAnonymous, OneDriveClientId, OneDriveSecret, OneDriveTenantId, OneDriveUserName, OneDrivePassword, OneDriveFolderName);
        return comp;
    }
    /// <summary>Decryptdata.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>A string.</returns>
    private string Decryptdata(string password)
    {
        string decryptpwd = string.Empty;
        System.Text.UTF8Encoding encodepwd = new System.Text.UTF8Encoding();
        System.Text.Decoder Decode = encodepwd.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(password);
        int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        decryptpwd = new String(decoded_char);
        return decryptpwd;
    }
    /// <summary>Get list of entities having homepage.</summary>
    ///
    /// <returns>A Dictionary.</returns>
    public Dictionary<string,string> GetEntityHomePages()
    {
        Dictionary<string,string> result = new Dictionary<string,string>();
        return result;
    }
    //
}
/// <summary>Display name of model class.</summary>
public class ModelConversion
{
    /// <summary>Gets display name of entity.</summary>
    ///
    /// <param name="InternalName">Name of the internal.</param>
    ///
    /// <returns>The display name of entity.</returns>
    public static string GetDisplayNameOfEntity(string InternalName)
    {
        string result = InternalName;
        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == InternalName);
        if(entity != null && !string.IsNullOrEmpty(entity.DisplayName))
            result = entity.DisplayName;
        return result;
    }
}
/// <summary>A file zipper.</summary>
public class FileZipper
{
    /// <summary>The base dir.</summary>
    string strBaseDir = "";
    /// <summary>The zos.</summary>
    ZipOutputStream zos = null;
    /// <summary>Starts a zip.</summary>
    ///
    /// <param name="ZipDirectory">Pathname of the zip directory.</param>
    /// <param name="ZipFile">     The zip file.</param>
    /// <param name="sender">      The sender.</param>
    public void StartZip(string ZipDirectory, string ZipFile, object sender)
    {
        MemoryStream ms = null;
        System.Web.UI.Page pgSender = (System.Web.UI.Page)sender;
        pgSender.Response.ContentType = "application/octet-stream";
        ZipFile = HttpUtility.UrlEncode(ZipFile).Replace('+', ' ');
        pgSender.Response.AddHeader("Content-Disposition", "attachment; filename=" + ZipFile + ".rar");
        ms = new MemoryStream();
        zos = new ZipOutputStream(ms);
        strBaseDir = ZipDirectory + "\\";
        zos.Finish();
        zos.Close();
        pgSender.Response.Clear();
        pgSender.Response.BinaryWrite(ms.ToArray());
        pgSender.Response.End();
    }
    /// <summary>Copies the zip.</summary>
    ///
    /// <param name="ZipDirectory">Pathname of the zip directory.</param>
    /// <param name="ZipFile">     The zip file.</param>
    ///
    /// <returns>An asynchronous result.</returns>
    public async Task CopyZip(string ZipDirectory, string ZipFile)
    {
        string exportcodefile = string.Empty;
        exportcodefile = ZipDirectory + "\\" + ZipFile;
        if(File.Exists(exportcodefile))
        {
            while(File.Exists(exportcodefile))
            {
                try
                {
                    File.Delete(exportcodefile);
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }
        MemoryStream ms = null;
        ms = new MemoryStream();
        zos = new ZipOutputStream(ms);
        strBaseDir = ZipDirectory + "\\";
        addZipEntry(strBaseDir);
        zos.Finish();
        zos.Close();
        System.IO.FileStream destfilefs = File.Create(exportcodefile);
        foreach(byte bt in ms.ToArray())
            destfilefs.WriteByte(bt);
        destfilefs.Close();
    }
    /// <summary>Adds a zip entry.</summary>
    ///
    /// <param name="PathStr">The path string.</param>
    protected void addZipEntry(string PathStr)
    {
        DirectoryInfo di = new DirectoryInfo(PathStr);
        foreach(DirectoryInfo item in di.GetDirectories())
        {
            addZipEntry(item.FullName);
        }
        foreach(FileInfo item in di.GetFiles())
        {
            FileStream fs = File.OpenRead(item.FullName);
            if(fs.Length > 0)
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string strEntryName = item.FullName.Replace(strBaseDir, "");
                ZipEntry entry = new ZipEntry(strEntryName);
                zos.PutNextEntry(entry);
                zos.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }
    }
    /// <summary>Zips.</summary>
    ///
    /// <param name="name">  The name.</param>
    /// <param name="array"> The array.</param>
    /// <param name="sender">The sender.</param>
    protected static void Zip(string name, byte[] array, System.Web.UI.Page sender)
    {
        MemoryStream raw = new MemoryStream();
        using(ZipOutputStream zStream = new ZipOutputStream(raw))
        {
            zStream.Write(array, 0, array.Length);
        }
        var zipped = raw.ToArray();
        sender.Response.ContentType = "application/octet-stream";
        sender.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(name) + ".zip");
        sender.Response.Clear();
        sender.Response.BinaryWrite(zipped);
        sender.Response.End();
    }
}
/// <summary>Grid Grouping.</summary>
public static class MyEnumerableExtensions
{
    /// <summary>Enumerates group by many in this collection.</summary>
    ///
    /// <typeparam name="TElement">Type of the element.</typeparam>
    /// <param name="elements">      The elements to act on.</param>
    /// <param name="groupSelectors">   A variable-length parameters list containing group selectors.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process group by many in this
    /// collection.</returns>
    public static IEnumerable<GroupResult> GroupByMany<TElement>(
        this IEnumerable<TElement> elements, params string[] groupSelectors)
    {
        var selectors = new List<Func<TElement, object>>(groupSelectors.Length);
        foreach(var selector in groupSelectors)
        {
            LambdaExpression l = System.Linq.Dynamic.DynamicExpression.ParseLambda(typeof(TElement), typeof(object), selector);
            selectors.Add((Func<TElement, object>)l.Compile());
        }
        return elements.GroupByMany(selectors.ToArray());
    }
    /// <summary>Enumerates group by many in this collection.</summary>
    ///
    /// <typeparam name="TElement">Type of the element.</typeparam>
    /// <param name="elements">      The elements to act on.</param>
    /// <param name="groupSelectors">   A variable-length parameters list containing group selectors.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process group by many in this
    /// collection.</returns>
    public static IEnumerable<GroupResult> GroupByMany<TElement>(
        this IEnumerable<TElement> elements, params Func<TElement, object>[] groupSelectors)
    {
        if(groupSelectors.Length > 0)
        {
            var selector = groupSelectors.First();
            var nextSelectors = groupSelectors.Skip(1).ToArray(); //reduce the list recursively until zero
            return
                elements.GroupBy(selector).Select(
                    g => new GroupResult
            {
                Key = Convert.ToString(g.Key) == null ? "None" : Convert.ToString(g.Key),
                Count = g.Count(),
                Items = g,
                SubGroups = g.GroupByMany(nextSelectors)
            });
        }
        else
            return null;
    }
    /// <summary>Encapsulates the result of a group.</summary>
    public class GroupResult
    {
        public string Key
        {
            get;
            set;
        }
        public int Count
        {
            get;
            set;
        }
        public IEnumerable Items
        {
            get;
            set;
        }
        public IEnumerable<GroupResult> SubGroups
        {
            get;
            set;
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Key, Count);
        }
    }
}
/// <summary>A generate token (used for web-api calls).</summary>
public static class GenerateToken
{
    /// <summary>Gets a token.</summary>
    ///
    /// <param name="userId">Identifier for the user.</param>
    ///
    /// <returns>The token.</returns>
    public static ApiToken GetToken(string userId)
    {
        ApplicationContext db = new ApplicationContext(new SystemUser());
        string token = Guid.NewGuid().ToString();
        DateTime issuedOn = DateTime.UtcNow;
        DateTime expiredOn =DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
        bool neverExpireToken = ConfigurationManager.AppSettings["NeverExpireToken"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["NeverExpireToken"]);
        if(neverExpireToken)
        {
            var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userId);
            if(obj != null)
            {
                token = obj.T_AuthToken;
                var tokenModelexisting = new ApiToken
                {
                    T_UsersID = userId,
                    T_AuthToken = token,
                    T_IssuedOn = issuedOn,
                    T_ExpiresOn = expiredOn
                };
                return tokenModelexisting;
            }
        }
        var tokendomain = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        db.ApiTokens.Add(tokendomain);
        db.SaveChanges();
        var tokenModel = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        return tokenModel;
    }
}
public enum AdminFeaturesOld
{
    Role,
    User,
    AssignUserRole,
    RoleEntityPermission,
    FieldLevelSecurity,
    BusinessRule,
    ApplicationConfiguration,
    UserBasedSecurity,
    DynamicRoles,
    MultiTenantExtraPrivileges,
    UserInterfaceSetting,
    ApplicationDocuments,
    
}
public class AdminFeaturesDictionary
{
    Dictionary<string, string> dictionary;
    public  AdminFeaturesDictionary()
    {
        dictionary = new Dictionary<string, string>();
        dictionary.Add("Role", "Manage Application Roles");
        dictionary.Add("User", "Manage Application Users");
        dictionary.Add("AssignUserRole", "Manage Relation between User and Roles");
        dictionary.Add("RoleEntityPermission", "Manage Entity Level Permission for Roles");
        dictionary.Add("FieldLevelSecurity", "Manage Field Level Security for Role and Entity");
        dictionary.Add("BusinessRule", "Manage Business Rule for Application");
        dictionary.Add("UserBasedSecurity", "Manage User Based Security");
        dictionary.Add("ApplicationConfiguration", "Configuration Settings for Application");
        dictionary.Add("DynamicRoles", "Create Dynamic Role for Application");
        dictionary.Add("MultiTenantExtraPrivileges", "Manage extra privilege for user in case of multitenant security");
        dictionary.Add("UserInterfaceSetting", "Manage User Interface Setting e.g. default page, theme etc.");
        dictionary.Add("ApplicationDocuments", "Access to Application Documents");
        dictionary.Add("ReportRoles", "Manage roles and report");
        dictionary.Add("BulkEmail", "Send email to application users.");
        dictionary.Add("EntityHelpPage", "Set Entity Home Page And Property Helps.");
    }
    public Dictionary<string, string> getDictionary()
    {
        return this.dictionary;
    }
}
/// <summary>Needed for change data feature.</summary>
public class JsonHelper
{
    public static IEntity DeserializeJsonObject(string JsonData, string entityname)
    {
        var type = Type.GetType("GeneratorBase.MVC.Models." + entityname);
        return (IEntity)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, type);
    }
    public static dynamic ChangeType(IEntity newobj, string entityname)
    {
        var entityType = Type.GetType("GeneratorBase.MVC.Models." + entityname);
        return Convert.ChangeType(newobj, entityType);
    }
    public static dynamic GetRecordbyId(int? id, string entityname)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityname + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(entityname).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("GetRecordById");
        object[] MethodParams = new object[] { Convert.ToString(id) };
        dynamic oldentity = mc.Invoke(objController, MethodParams);
        return oldentity;
    }
    public static object GetIdFromPropertyValue(string entityname, string propertyname, string propertyvalue)
    {
        //Type assocontroller = Type.GetType("GeneratorBase.MVC.Controllers." + entityname + "Controller, GeneratorBase.MVC.Controllers");
        Type assocontroller = new CreateControllerType(entityname).controllerType;
        object objAssoController = Activator.CreateInstance(assocontroller, null);
        System.Reflection.MethodInfo assomc = assocontroller.GetMethod("GetIdFromPropertyValue");
        object[] assoMethodParams = new object[] { propertyname, propertyvalue };
        object objId = assomc.Invoke(objAssoController, assoMethodParams);
        return objId;
    }
    
}
/// <summary>A property information for entity.</summary>
[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
public class PropertyUIInfoType : System.Attribute
{
    /// <summary>The IsUIInfo.</summary>
    string IsUIInfo;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="pIsUIInfo">The pIsUIInfo.</param>
    public PropertyUIInfoType(string pIsUIInfo)
    {
        IsUIInfo = pIsUIInfo;
    }
    /// <summary>Gets the name of the property.</summary>
    ///
    /// <value>The name of the property.</value>
    public string PropUIInfo
    {
        get
        {
            return IsUIInfo;
        }
    }
}
public class DoAuditEntry
{
    public static string GetEntityNameForDocument(long id, IUser user)
    {
        var result = "";
        var baseuri =CommonFunction.Instance.getBaseUri();
        baseuri = baseuri.EndsWith("/") ? baseuri : baseuri + "/";
        using(var db = new ApplicationContext(user))
        {
            var FileDocument_AttachDocument = db.FileDocuments.FirstOrDefault(p=>p.AttachDocument == id);
            if(FileDocument_AttachDocument!= null)
                return "<a href='" + baseuri + "FileDocument/Edit/" + FileDocument_AttachDocument.Id + "'>"+ ModelReflector.Entities.FirstOrDefault(fd => fd.Name == "FileDocument").DisplayName +"</a>";
        }
        return result;
    }
    
    public static string EntityNameForDocument(long id)
    {
        var result = "";
        using(var db = new ApplicationContext(new SystemUser(), true))
        {
        }
        return result;
    }
    
    public static void AddJournalEntryCommon(IUser User, ApplicationDbContext identitydb, string role, string EntityName)
    {
        using(JournalEntryContext db = new JournalEntryContext())
        {
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.UtcNow;
            Je.EntityName = EntityName;
            Je.UserName = User.Name;
            Je.Type = "Added";
            Je.RecordInfo = role;
            Je.BrowserInfo = GetBrowserDetails();
            Je.RecordId = 0;
            db.JournalEntries.Add(Je);
            db.SaveChanges();
        }
    }
    public static void AddJournalEntry(string entityname, string username, string type, string recordinfo, string source="")
    {
        using(JournalEntryContext db = new JournalEntryContext())
        {
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.UtcNow;
            Je.EntityName = entityname;
            Je.UserName = username;
            Je.Type = type;
            Je.RecordInfo = recordinfo;
            Je.BrowserInfo = GetBrowserDetails();
            Je.RecordId = 0;
            Je.Source = source;
            db.JournalEntries.Add(Je);
            db.SaveChanges();
        }
    }
    public static void AddJournalEntryRecordId(string entityname, string username, string type,  string recordinfo, string recordid, string source = "")
    {
        using(JournalEntryContext db = new JournalEntryContext())
        {
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.UtcNow;
            Je.EntityName = entityname;
            Je.UserName = username;
            Je.Type = type;
            Je.RecordInfo = recordinfo;
            Je.BrowserInfo = GetBrowserDetails();
            Je.RecordId = long.Parse(recordid);
            Je.Source = source;
            db.JournalEntries.Add(Je);
            db.SaveChanges();
        }
    }
    public static void MakeAddJournalEntry(IUser User, ApplicationDbContext identitydb, System.Data.Entity.Infrastructure.DbEntityEntry entry)
    {
        var entityType = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entry.Entity.GetType());
        var EntityName = entityType.Name;
        var userName = User != null ? User.Name : "AppAdmin";
        JournalEntry Je = new JournalEntry();
        Je.DateTimeOfEntry = DateTime.UtcNow;
        Je.EntityName = EntityName;
        Je.UserName = userName;
        Je.Type = entry.State.ToString();
        Je.RecordId = 0;
        Type EntityType = entry.Entity.GetType();
        var displayValue = "";
        dynamic EntityObj = Convert.ChangeType(entry.Entity, EntityType);
        try
        {
            displayValue = EntityObj.DisplayValue;
        }
        catch
        {
            displayValue = "";
        }
        if(EntityName == "ApplicationUser")
            displayValue = EntityObj.UserName;
        if(EntityName == "IdentityUserRole")
        {
            var roleid = EntityObj.RoleId;
            var userid = EntityObj.UserId;
            displayValue = identitydb.Users.Find(userid).UserName + " - " + identitydb.Roles.Find(roleid).Name;
        }
        Je.RecordInfo = displayValue;
        Je.BrowserInfo = GetBrowserDetails();
        using(JournalEntryContext db = new JournalEntryContext())
        {
            db.JournalEntries.Add(Je);
            db.SaveChanges();
        }
    }
    public static void MakeUpdateJournalEntry(IUser User, System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry)
    {
        var entityType = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
        var EntityName = entityType.Name;
        var OriginalObj = dbEntityEntry.GetDatabaseValues();
        var CurrentObj = dbEntityEntry.CurrentValues;
        var userName = User != null ? User.Name : "AppAdmin";
        string dispValue = "";
        try
        {
            Type EntityType = dbEntityEntry.Entity.GetType();
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
            dispValue = EntityObj.DisplayValue;
        }
        catch
        {
            dispValue ="";
        }
        if(string.IsNullOrEmpty(dispValue))
        {
            if(EntityName == "ApplicationUser")
                dispValue = Convert.ToString(CurrentObj.GetValue<object>("UserName"));
            if(EntityName == "IdentityRole")
                dispValue = Convert.ToString(CurrentObj.GetValue<object>("Name"));
        }
        using(var db = new JournalEntryContext())
        {
            foreach(var property in dbEntityEntry.OriginalValues.PropertyNames)
            {
                if(property == "DisplayValue" || property == "TenantId" || property == "ConcurrencyKey" || property.ToLower().Contains("password") || property.ToLower().Contains("security")) continue;
                var original = OriginalObj.GetValue<object>(property);
                var current = CurrentObj.GetValue<object>(property);
                if(original != current && (original == null || !original.Equals(current)))
                {
                    JournalEntry Je = new JournalEntry();
                    Je.DateTimeOfEntry = DateTime.UtcNow;
                    Je.EntityName = EntityName;
                    Je.UserName = userName;
                    Je.Type = dbEntityEntry.State.ToString();
                    var displayValue = dispValue;
                    Je.RecordInfo = displayValue;
                    Je.BrowserInfo = GetBrowserDetails();
                    Je.PropertyName = property;
                    Je.OldValue = Convert.ToString(original);
                    Je.NewValue = Convert.ToString(current);
                    //Je.RecordId = Convert.ToInt64(id);
                    db.JournalEntries.Add(Je);
                }
            }
            db.SaveChanges();
        }
    }
    
    public static string GetBrowserDetails()
    {
        var uaParser = Parser.GetDefault();
        string uaString = ((HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Headers != null && HttpContext.Current.Request.Headers["User-Agent"] != null) ? HttpContext.Current.Request.Headers["User-Agent"].ToString() : "");
        ClientInfo c = uaParser.Parse(uaString);
        //Console.WriteLine(c.UserAgent.Family); // => "Mobile Safari"
        //Console.WriteLine(c.UserAgent.Major);  // => "5"
        //Console.WriteLine(c.UserAgent.Minor);  // => "1"
        //Console.WriteLine(c.OS.Family);        // => "iOS"
        //Console.WriteLine(c.OS.Major);         // => "5"
        //Console.WriteLine(c.OS.Minor);         // => "1"
        //Console.WriteLine(c.Device.Family);    // => "iPhone"
        string browserDetails = string.Empty;
        browserDetails = c.UserAgent.Family; //(HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Browser != null) ? HttpContext.Current.Request.Browser.Browser : "";
        try
        {
            browserDetails += "-" + uaString;
        }
        catch
        {
            return browserDetails;
        }
        return browserDetails;
    }
    
}
public static class Extensions
{
    public static string Replace(this string source, string oldString, string newString, StringComparison comp)
    {
        int index = source.IndexOf(oldString, comp);
        // Determine if we found a match
        bool MatchFound = index >= 0;
        if(MatchFound)
        {
            // Remove the old text
            source = source.Remove(index, oldString.Length);
            // Add the replacemenet text
            source = source.Insert(index, newString);
        }
        // recurse for multiple instances of the name
        if(source.IndexOf(oldString, comp) != -1)
        {
            source = Replace(source, oldString, newString, comp);
        }
        return source;
    }
}
}


