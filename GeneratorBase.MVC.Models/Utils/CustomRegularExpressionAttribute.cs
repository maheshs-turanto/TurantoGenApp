using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GeneratorBase.MVC
{
public class CustomRangeAttribute:RangeAttribute
{
    private static PropertyValidationandFormat result;
    /// <summary>Constructor.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    public CustomRangeAttribute(string key, string resourcename, Type type, string lower, string upper, string errormsg)
        : base(type, Convert.ToString(Lookup(key, resourcename, lower, upper, errormsg).LowerBound), Convert.ToString(Lookup(key, resourcename, lower, upper, errormsg).UpperBound.ToString()))
    {
        //this.ErrorMessage = result.ErrorMessage;
    }
    /// <summary>Looks up a given key to find its associated value.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    ///
    /// <returns>A string.</returns>
    static PropertyValidationandFormat Lookup(string key, string resourcename, string lower, string upper, string errormsg)
    {
        result = new PropertyValidationandFormat();
        result.PropertyName = key;
        result.LowerBound = lower;
        result.UpperBound = upper;
        //result.ErrorMessage = errormsg;
        try
        {
            using(var db = new PropertyValidationandFormatContext())
            {
                var validationObj = db.PropertyValidationandFormats.FirstOrDefault(p => p.EntityName == resourcename && p.PropertyName == key);
                if(validationObj != null && !string.IsNullOrEmpty(validationObj.LowerBound) && !string.IsNullOrEmpty(validationObj.UpperBound))
                {
                    result.LowerBound = validationObj.LowerBound;
                    result.UpperBound = validationObj.UpperBound;
                }
            }
        }
        catch
        {
            return result; // fallback
        }
        return result;
    }
}
/// <summary>Attribute for custom display name (used in model class for dynamic label change).</summary>
public class CustomRegularExpressionAttribute : RegularExpressionAttribute
{
    private static PropertyValidationandFormat result;
    public string maskpattern
    {
        get;
        set;
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    public CustomRegularExpressionAttribute(string key, string resourcename, string defaultvalue, string defaultmask, string errormsg)
        : base(Lookup(key, resourcename, defaultvalue,defaultmask, errormsg).RegExPattern)
    {
        this.ErrorMessage = result.ErrorMessage;
        this.maskpattern = result.MaskPattern;
    }
    /// <summary>Looks up a given key to find its associated value.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    ///
    /// <returns>A string.</returns>
    static PropertyValidationandFormat Lookup(string key, string resourcename, string defaultvalue,string defaultmask, string errormsg)
    {
        result = new PropertyValidationandFormat();
        result.PropertyName = key;
        result.RegExPattern = defaultvalue;
        result.MaskPattern = defaultmask;
        result.ErrorMessage = errormsg;
        try
        {
            using(var db = new PropertyValidationandFormatContext())
            {
                var validationObj = db.PropertyValidationandFormats.FirstOrDefault(p => p.EntityName == resourcename && p.PropertyName == key);
                if(validationObj != null && !string.IsNullOrEmpty(validationObj.RegExPattern))
                {
                    result.RegExPattern = validationObj.RegExPattern;
                    result.ErrorMessage = string.IsNullOrEmpty(validationObj.ErrorMessage) ? errormsg : validationObj.ErrorMessage;
                    result.MaskPattern = validationObj.MaskPattern;
                }
            }
        }
        catch
        {
            return result; // fallback
        }
        return result;
    }
}

}