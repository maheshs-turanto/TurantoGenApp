using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace GeneratorBase.MVC.CurrencyHelper
{
public static class CurrencyHelper
{

    public static MvcHtmlString DisplayForCurrency(this HtmlHelper html, object val, string entity, string prop, Enum enumValue)
    {
        if(enumValue == null)
            enumValue = CurrencyList.dollar;
        string displayFormat = GetDisplayFormat(entity, prop);
        string symbol = GetCurrencySymbolByName(enumValue);
        if(val == null)
        {
            string DefaultResult = symbol + " " + string.Format(displayFormat, "0");
            return new MvcHtmlString(DefaultResult);
        }
        string result = symbol + " " + string.Format(displayFormat, val);
        return MvcHtmlString.Create(result);
    }
    
    public static string GetDisplayFormat(string entity, string Property)
    {
        var entityName = ModelReflector.Entities.FirstOrDefault(p => p.Name == entity);
        var property = entityName.Properties.FirstOrDefault(p => p.Name == Property);
        if(property != null)
            return property.DisplayFormat;
        else
            return string.Empty;
    }
    
    public static string GetCurrencySymbolByName(this Enum enumValue)
    {
        return enumValue.GetType()
               .GetMember(enumValue.ToString())
               .First()
               .GetCustomAttribute<DisplayAttribute>()
               .GetName();
    }
    
    
    public enum CurrencyList
    {
        [Display(Name = "$")]
        dollar,
        
        [Display(Name = "₹")]
        rupee,
        
        [Display(Name = "£")]
        pound,
        
        [Display(Name = "¥")]
        yen,
        
        [Display(Name = "€")]
        euro
    }
    
}
}