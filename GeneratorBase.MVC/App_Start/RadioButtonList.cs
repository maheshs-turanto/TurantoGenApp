using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
namespace GeneratorBase.MVC
{
/// <summary>A HTML extensions.</summary>
public static class HtmlExtensions
{
    #region RadioButtonList
    
    /// <summary>A HtmlHelper extension method that radio button list.</summary>
    ///
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="name">      The name.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name)
    {
        return RadioButtonList(htmlHelper, name, null /* inputList */, null /* htmlAttributes */);
    }
    
    /// <summary>A HtmlHelper extension method that radio button list.</summary>
    ///
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="name">      The name.</param>
    /// <param name="inputList"> List of inputs.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList)
    {
        return RadioButtonList(htmlHelper, name, inputList, null /* htmlAttributes */);
    }
    
    /// <summary>A HtmlHelper extension method that radio button list.</summary>
    ///
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="name">          The name.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, object htmlAttributes)
    {
        return RadioButtonList(htmlHelper, name, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }
    
    /// <summary>A HtmlHelper extension method that radio button list.</summary>
    ///
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="name">          The name.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
    {
        return RadioButtonListHelper(htmlHelper, name, inputList, htmlAttributes);
    }
    
    /// <summary>A HtmlHelper&lt;TModel&gt; extension method that radio button list for.</summary>
    ///
    /// <typeparam name="TModel">   Type of the model.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="inputList"> List of inputs.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList)
    {
        return RadioButtonListFor(htmlHelper, expression, inputList, null /* htmlAttributes */);
    }
    
    /// <summary>A HtmlHelper&lt;TModel&gt; extension method that radio button list for.</summary>
    ///
    /// <typeparam name="TModel">   Type of the model.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="expression">    The expression.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, object htmlAttributes)
    {
        return RadioButtonListFor(htmlHelper, expression, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }
    
    /// <summary>A HtmlHelper&lt;TModel&gt; extension method that radio button list for.</summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null.</exception>
    ///
    /// <typeparam name="TModel">   Type of the model.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="expression">    The expression.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
    {
        if(expression == null)
        {
            throw new ArgumentNullException("expression");
        }
        return RadioButtonListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), inputList, htmlAttributes);
    }
    
    /// <summary>Helper method that radio button list.</summary>
    ///
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="expression">    The expression.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    private static MvcHtmlString RadioButtonListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
    {
        return InputInternal(htmlHelper, expression, inputList, false /* allowMultiple */, htmlAttributes);
    }
    #endregion
    #region Helper methods
    
    /// <summary>A HtmlHelper extension method that input internal.</summary>
    ///
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///                                         illegal values.</exception>
    ///
    /// <param name="htmlHelper">    The HTML helper.</param>
    /// <param name="name">          The name.</param>
    /// <param name="inputList">     List of inputs.</param>
    /// <param name="allowMultiple"> True to allow, false to suppress the multiple.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    ///
    /// <returns>A MvcHtmlString.</returns>
    
    private static MvcHtmlString InputInternal(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
    {
        var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        if(String.IsNullOrEmpty(fullName))
        {
            throw new ArgumentException("Null or Empty", "name");
        }
        var usedViewData = false;
        var IsRequired = false;
        if(htmlAttributes != null)
        {
            IsRequired = htmlAttributes.Keys.Any(k => k.ToLower().Trim() == "required");
            //htmlAttributes.Remove("required");
        }
        if(inputList == null)
        {
            inputList = htmlHelper.GetInputData(fullName);
            usedViewData = true;
        }
        var defaultValue = GetDefaultValue(htmlHelper, fullName, allowMultiple, usedViewData);
        if(defaultValue != null)
        {
            inputList = GetListWithDefaultValue(inputList, defaultValue, allowMultiple);
        }
        var listItemBuilder = new StringBuilder();
        if(inputList != null)
        {
            if(!IsRequired)
                listItemBuilder.AppendLine(InputItemToHtml(new SelectListItem { Value = "0", Text = "None", Selected = !inputList.Any(p => p.Selected) }, allowMultiple, fullName, IsRequired));
            else if(inputList.Count() > 0 && !inputList.Any(p => p.Selected))
                inputList.FirstOrDefault().Selected = true;
            foreach(var item in inputList)
                listItemBuilder.AppendLine(InputItemToHtml(item, allowMultiple, fullName, IsRequired));
        }
        if(inputList == null || inputList.Count() == 0)
        {
            var builder = new TagBuilder("input");
            builder.Attributes["id"] = name;
            builder.Attributes["name"] = name;
            builder.Attributes["type"] = "text";
            builder.Attributes.Add("style", "width:0px; height:0px; border:0px solid #fff !important;");
            if(IsRequired)
                builder.Attributes["required"] = "required";
            listItemBuilder.AppendLine(builder.ToString(TagRenderMode.Normal));
        }
        var tagBuilder = new TagBuilder("ul")
        {
            InnerHtml = listItemBuilder.ToString()
        };
        tagBuilder.Attributes.Add("style", "padding:0");
        tagBuilder.Attributes.Add("name", "ul" + fullName);
        if(htmlAttributes != null)
            tagBuilder.MergeAttributes(htmlAttributes, true /* replaceExisting */);
        tagBuilder.GenerateId("ul" + fullName);
        ModelState modelState;
        if(htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
        {
            if(modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
        }
        tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));
        return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
    }
    
    /// <summary>Gets the input data in this collection.</summary>
    ///
    /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
    ///                                                 invalid.</exception>
    ///
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="name">      The name.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the input data in this
    /// collection.</returns>
    
    private static IEnumerable<SelectListItem> GetInputData(this HtmlHelper htmlHelper, string name)
    {
        object inputObject = null;
        if(htmlHelper.ViewData != null)
        {
            inputObject = htmlHelper.ViewData.Eval(name);
        }
        if(inputObject != null)
        {
            var inputList = inputObject as IEnumerable<SelectListItem>;
            if(inputList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.",
                        name,
                        inputObject.GetType().FullName,
                        "IEnumerable<SelectListItem>"));
            }
            return inputList;
        }
        return null;
    }
    
    /// <summary>Gets default value.</summary>
    ///
    /// <param name="htmlHelper">   The HTML helper.</param>
    /// <param name="fullName">     Name of the full.</param>
    /// <param name="allowMultiple">True to allow, false to suppress the multiple.</param>
    /// <param name="usedViewData"> True to used view data.</param>
    ///
    /// <returns>The default value.</returns>
    
    private static object GetDefaultValue(HtmlHelper htmlHelper, string fullName, bool allowMultiple, bool usedViewData)
    {
        var defaultValue = (allowMultiple) ?
                           GetModelStateValue(htmlHelper, fullName, typeof(string[])) :
                           GetModelStateValue(htmlHelper, fullName, typeof(string));
        if(defaultValue == null && !usedViewData)
        {
            defaultValue = htmlHelper.ViewData.Eval(fullName);
        }
        return defaultValue;
    }
    
    /// <summary>Gets model state value.</summary>
    ///
    /// <param name="htmlHelper">     The HTML helper.</param>
    /// <param name="key">            The key.</param>
    /// <param name="destinationType">Type of the destination.</param>
    ///
    /// <returns>The model state value.</returns>
    
    private static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
    {
        ModelState modelState;
        if(htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            if(modelState.Value != null)
                return modelState.Value.ConvertTo(destinationType, null /* culture */);
        return null;
    }
    
    /// <summary>Gets the list with default values in this collection.</summary>
    ///
    /// <param name="inputList">    List of inputs.</param>
    /// <param name="defaultValue"> The default value.</param>
    /// <param name="allowMultiple">True to allow, false to suppress the multiple.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the list with default values
    /// in this collection.</returns>
    
    private static IEnumerable<SelectListItem> GetListWithDefaultValue(IEnumerable<SelectListItem> inputList, object defaultValue, bool allowMultiple)
    {
        //var defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
        //IEnumerable<string> values = from object value in defaultValues
        //                             select Convert.ToString(value, CultureInfo.CurrentCulture);
        //var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        var newInputList = new List<SelectListItem>();
        foreach(var item in inputList)
        {
            // item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
            newInputList.Add(item);
        }
        return newInputList;
    }
    
    /// <summary>Input item to HTML.</summary>
    ///
    /// <param name="item">         The item.</param>
    /// <param name="allowMultiple">True to allow, false to suppress the multiple.</param>
    /// <param name="name">         The name.</param>
    /// <param name="IsRequired">   True if is required, false if not.</param>
    ///
    /// <returns>A string.</returns>
    
    internal static string InputItemToHtml(SelectListItem item, bool allowMultiple, string name, bool IsRequired)
    {
        var listItemBuilder = new StringBuilder();
        listItemBuilder.AppendLine(GetInputItem(item, allowMultiple, name, IsRequired));
        listItemBuilder.AppendLine(CoverWithTag("span", HttpUtility.HtmlEncode(item.Text)));
        var label = CoverWithTag("label", listItemBuilder.ToString());
        return CoverWithLi(label);
    }
    
    /// <summary>Gets input item.</summary>
    ///
    /// <param name="item">         The item.</param>
    /// <param name="allowMultiple">True to allow, false to suppress the multiple.</param>
    /// <param name="name">         The name.</param>
    /// <param name="IsRequired">   True if is required, false if not.</param>
    ///
    /// <returns>The input item.</returns>
    
    internal static string GetInputItem(SelectListItem item, bool allowMultiple, string name, bool IsRequired)
    {
        var builder = new TagBuilder("input");
        builder.Attributes["name"] = name;
        if(allowMultiple)
            builder.Attributes["type"] = "checkbox";
        else
            builder.Attributes["type"] = "radio";
        if(item.Value != null)
            builder.Attributes["value"] = item.Value;
        else
            builder.Attributes["value"] = item.Text;
        if(item.Selected)
            builder.Attributes["checked"] = "checked";
        if(IsRequired)
            builder.Attributes["required"] = "required";
        return builder.ToString(TagRenderMode.Normal);
    }
    
    /// <summary>Cover with tag.</summary>
    ///
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="html">   The HTML.</param>
    ///
    /// <returns>A string.</returns>
    
    private static string CoverWithTag(string tagName, string html)
    {
        var builder = new TagBuilder(tagName)
        {
            InnerHtml = html
        };
        return builder.ToString(TagRenderMode.Normal);
    }
    
    /// <summary>Cover with li.</summary>
    ///
    /// <param name="html">The HTML.</param>
    ///
    /// <returns>A string.</returns>
    
    internal static string CoverWithLi(string html)
    {
        var builder = new TagBuilder("li")
        {
            InnerHtml = html
        };
        builder.Attributes.Add("style", "list-style-type:none;");
        return builder.ToString(TagRenderMode.Normal);
    }
    
    /// <summary>A TagBuilder extension method that converts this object to a MVC HTML string.</summary>
    ///
    /// <param name="tagBuilder">The tagBuilder to act on.</param>
    /// <param name="renderMode">The render mode.</param>
    ///
    /// <returns>The given data converted to a MvcHtmlString.</returns>
    
    private static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
    {
        Debug.Assert(tagBuilder != null);
        return new MvcHtmlString(tagBuilder.ToString(renderMode));
    }
    #endregion
    
    public static MvcHtmlString CheckBoxNull<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool?>> expression)
    {
        return htmlHelper.CheckBoxNull<TModel>(expression, null);
    }
    public static MvcHtmlString CheckBoxNull<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool?>> expression, object htmlAttributes)
    {
        ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        bool? isChecked = null;
        if(metadata.Model != null)
        {
            bool modelChecked;
            if(Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
            {
                isChecked = modelChecked;
            }
        }
        else
        {
            if(htmlAttributes != null)
            {
                var htmlattr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                if(htmlattr.Keys.Any(k => k.ToLower().Trim() == "checked"))
                {
                    var checkedval = htmlattr.Keys.FirstOrDefault(k => k.ToLower().Trim() == "checked");
                    if(checkedval.ToLower() == "checked")
                        isChecked = true;
                }
            }
        }
        return htmlHelper.CheckBox(ExpressionHelper.GetExpressionText(expression), isChecked ?? false, htmlAttributes);
    }
}
}