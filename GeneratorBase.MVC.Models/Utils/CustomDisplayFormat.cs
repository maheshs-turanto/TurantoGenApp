using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GeneratorBase.MVC
{

/// <summary>Attribute for custom display name (used in model class for dynamic label change).</summary>
public class CustomDisplayFormat : DisplayFormatAttribute
{
    private static PropertyValidationandFormat result;
    public string uidisplayformat
    {
        get;
        set;
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    public CustomDisplayFormat(string key, string resourcename, string defaultvalue, string uidefaultvalue, bool applyineditmode, string type = "")
        : base()
    {
        Lookup(key, resourcename, defaultvalue, uidefaultvalue, applyineditmode, type);
        this.DataFormatString = result.DisplayFormat;
        this.ApplyFormatInEditMode = applyineditmode;
        this.uidisplayformat = result.Other1;
    }
    /// <summary>Looks up a given key to find its associated value.</summary>
    ///
    /// <param name="key">         The key.</param>
    /// <param name="resourcename">The resourcename.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    ///
    /// <returns>A string.</returns>
    static PropertyValidationandFormat Lookup(string key, string resourcename, string defaultvalue, string uidefaultvalue, bool applyineditmode, string type)
    {
        result = new PropertyValidationandFormat();
        result.PropertyName = key;
        result.DisplayFormat = defaultvalue;
        result.Other1 = uidefaultvalue;
        try
        {
            using(var db = new PropertyValidationandFormatContext())
            {
                var validationObj = db.PropertyValidationandFormats.FirstOrDefault(p => p.IsEnabled.HasValue && p.IsEnabled.Value && p.EntityName == resourcename && p.PropertyName == key);
                if(validationObj != null && !string.IsNullOrEmpty(validationObj.DisplayFormat))
                {
                    result.DisplayFormat = validationObj.DisplayFormat;
                    result.Other1 = ConvertToMomentFormat(result.DisplayFormat);
                }
                else if(validationObj == null)
                {
                    var dateformat = ConfigurationManager.AppSettings["DateFormat"];
                    var timeformat = ConfigurationManager.AppSettings["TimeFormat"];
                    if(type == "DateOnly" && dateformat != null && dateformat != "Default")
                    {
                        result.DisplayFormat = "{0:" + dateformat + "}";
                        result.Other1 = ConvertToMomentFormat(result.DisplayFormat);
                    }
                    else if(type == "TimeOnly" && timeformat != null && timeformat != "Default")
                    {
                        result.DisplayFormat = "{0:" + timeformat + "}";
                        result.Other1 = ConvertToMomentFormat(result.DisplayFormat);
                    }
                    else
                    {
                        if(type != "DateOnly" && type != "TimeOnly")
                            if((dateformat != null && dateformat != "Default") || (timeformat != null && timeformat != "Default"))
                            {
                                dateformat = dateformat != null && dateformat != "Default" ? dateformat : defaultvalue.Substring(3, defaultvalue.IndexOf(" "));
                                timeformat = timeformat != null && timeformat != "Default" ? timeformat : defaultvalue.TrimEnd('}').Substring(defaultvalue.IndexOf(" ") + 1);
                                if(dateformat != "Default" && timeformat != "Default")
                                {
                                    result.DisplayFormat = "{0:" + dateformat + " " + timeformat + "}"; ;
                                    result.Other1 = ConvertToMomentFormat(result.DisplayFormat);
                                }
                            }
                    }
                }
            }
        }
        catch
        {
            return result; // fallback
        }
        return result;
    }
    public static string ConvertToMomentFormat(string s0)
    {
        var a = "";
        //s0 = System.Text.RegularExpressions.Regex.Replace(s0, @"{\d+:(.*)}", "$1");
        if(s0.StartsWith("{0:") && s0.EndsWith("}"))
        {
            s0 = s0.TrimStart("{0:".ToCharArray());
            s0 = s0.TrimEnd("}:".ToCharArray());
        }
        List<string> arr = new List<string>();
        string sb = "";
        var count = 0;
        if(!string.IsNullOrEmpty(s0))
        {
            char previousLetter = s0[0];
            sb = previousLetter.ToString();
            for(int i = 1; i < s0.Count(); i++)
            {
                if(s0[i] == previousLetter)
                {
                    sb += s0[i].ToString();
                }
                else
                {
                    arr.Add(sb);
                    sb = s0[i].ToString();
                    previousLetter = s0[i];
                    count++;
                }
            }
            arr.Add(sb);
        }
        //string[] m = Regex.Split(s0, @"/((.)\2*)/g");
        foreach(var t in arr)
        {
            if(string.IsNullOrEmpty(t)) continue;
            var s = t;
            switch(s)
            {
            case "d":
                s = "MM/DD/YYYY";
                break;
            case "dd":
                s = "DD";
                break;
            case "ddd":
                s = "ddd";
                break;
            case "dddd":
                s = "dddd";
                break;
            case "D":
                s = "DD MMMM YYYY";
                break;
            case "f":
                s = "DD MMMM YYYY HH:mm";
                break;
            case "fff":
                s = "SSS";
                break;
            case "F":
                s = "DD MMMM YYYY HH:mm:ss";
                break;
            case "FFF":
                s = "SSS";
                break; // no trailing 0s
            case "g":
                s = "DD/MM/YYYY HH:mm";
                break;
            case "G":
                s = "DD/MM/YYYY HH:mm:ss";
                break;
            case "hh":
                s = "hh";
                break;
            case "HH":
                s = "HH";
                break;
            case "m":
                s = "MMMM DD";
                break;
            case "mm":
                s = "mm";
                break;
            case "M":
                s = "MMMM DD";
                break;
            case "MM":
                s = "MM";
                break;
            case "MMM":
                s = "MMM";
                break;
            case "MMMM":
                s = "MMMM";
                break;
            case "o":
                s = "YYYY-MM-DD HH:mm:ssZ";
                break;
            case "O":
                s = "YYYY-MM-DD HH:mm:ssZ";
                break;
            case "r":
                s = "ddd, DD MMM YYYY, H:mm:ss z";
                break;
            case "R":
                s = "ddd, DD MMM YYYY, H:mm:ss z";
                break;
            case "s":
                s = "YYYY-MM-DDTHH:mm:ss";
                break;
            case "ss":
                s = "ss";
                break;
            case "t":
                s = "HH:mm";
                break;
            case "tt":
                s = "A";
                break;
            case "T":
                s = "HH:mm:ss";
                break;
            case "u":
                s = "YYYY-MM-DD HH:mm:ssZ";
                break;
            case "y":
                s = "MMMM, YYYY";
                break;
            case "yy":
                s = "YY";
                break;
            case "yyyy":
                s = "YYYY";
                break;
            case "Y":
                s = "MMMM, YYYY";
                break;
            }
            a = a + s;
        }
        return a;
    }
}

}