using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.UIPropertyHtmlHelper
{
public static class UIPropertyHtmlHelper
{
    public static MvcHtmlString LabelForUIProperty(this HtmlHelper Html, object PropForColor, string type)
    {
        if(PropForColor == null)
            return new MvcHtmlString(string.Empty);
        TagBuilder label = new TagBuilder("label");
        TagBuilder anchor = new TagBuilder("a");
        string backcolorprop = "";
        string fgcolorprop = "";
        object backcolor = null;
        object fgcolor = null;
        PropertyInfo[] props = PropForColor.GetType().GetProperties();
        foreach(PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true);
            foreach(object attr in attrs)
            {
                PropertyUIInfoType ColorAttr = attr as PropertyUIInfoType;
                if(ColorAttr != null)
                {
                    string colorAttVal = ColorAttr.PropUIInfo;
                    if(colorAttVal == "bgcolor")
                        backcolorprop = prop.Name;
                    if(colorAttVal == "fgcolor")
                        fgcolorprop = prop.Name;
                }
            }
            if(!string.IsNullOrEmpty(backcolorprop) && !string.IsNullOrEmpty(fgcolorprop))
                break;
        }
        if(!string.IsNullOrEmpty(backcolorprop))
            backcolor = PropForColor.GetType().GetProperty(backcolorprop).GetValue(PropForColor, null);
        if(!string.IsNullOrEmpty(fgcolorprop))
            fgcolor = PropForColor.GetType().GetProperty(fgcolorprop).GetValue(PropForColor, null);
        var displayValue = PropForColor.GetType().GetProperty("DisplayValue").GetValue(PropForColor, null);
        if(backcolor == null && fgcolor == null)
        {
            if(displayValue == null)
                return new MvcHtmlString(string.Empty);
            if(displayValue != null)
                return new MvcHtmlString(displayValue.ToString());
        }
        label.SetInnerText(displayValue.ToString());
        label.AddCssClass("badge");
        label.MergeAttribute("style", "background-color:" + backcolor + ";color:" + fgcolor + ";" + "font-size:0.9rem;padding:5px 12px");
        return new MvcHtmlString(label.ToString());
    }
    public static string UIPropertyStyleDropDown(object PropForColor, string type)
    {
        if(PropForColor == null)
            return string.Empty;
        string backcolorprop = "";
        string fgcolorprop = "";
        object backcolor = null;
        object fgcolor = null;
        //PropertyInfo[] props = PropForColor.GetType().GetProperties();
        //foreach (PropertyInfo prop in props)
        //{
        //    object[] attrs = prop.GetCustomAttributes(true);
        //    foreach (object attr in attrs)
        //    {
        //        PropertyUIInfoType ColorAttr = attr as PropertyUIInfoType;
        //        if (ColorAttr != null)
        //        {
        //            string colorAttVal = ColorAttr.PropUIInfo;
        //            if (colorAttVal == "bgcolor")
        //                backcolorprop = prop.Name;
        //            if (colorAttVal == "fgcolor")
        //                fgcolorprop = prop.Name;
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(backcolorprop) && !string.IsNullOrEmpty(fgcolorprop))
        //        break;
        //}
        foreach(PropertyInfo prop in PropForColor.GetType().GetProperties().Where(x => x.GetCustomAttribute<PropertyUIInfoType>() != null && (x.GetCustomAttribute<PropertyUIInfoType>().PropUIInfo.Equals("bgcolor") || x.GetCustomAttribute<PropertyUIInfoType>().PropUIInfo.Equals("fgcolor"))))
        {
            var attr = prop.GetCustomAttributes(typeof(PropertyUIInfoType), true);
            string colorAttVal = (((PropertyUIInfoType[])attr)[0]).PropUIInfo;
            if(colorAttVal == "bgcolor")
                backcolorprop = prop.Name;
            if(colorAttVal == "fgcolor")
                fgcolorprop = prop.Name;
            if(!string.IsNullOrEmpty(backcolorprop) && !string.IsNullOrEmpty(fgcolorprop))
                break;
        }
        if(!string.IsNullOrEmpty(backcolorprop))
            backcolor = PropForColor.GetType().GetProperty(backcolorprop).GetValue(PropForColor, null);
        if(!string.IsNullOrEmpty(fgcolorprop))
            fgcolor = PropForColor.GetType().GetProperty(fgcolorprop).GetValue(PropForColor, null);
        var displayValue = PropForColor.GetType().GetProperty("DisplayValue").GetValue(PropForColor, null);
        if(backcolor == null && fgcolor == null)
        {
            if(displayValue == null)
                return string.Empty;
            if(displayValue != null)
                return displayValue.ToString();
        }
        string style = "background-color:" + backcolor + ";color:" + fgcolor;
        return style;
    }
}
}