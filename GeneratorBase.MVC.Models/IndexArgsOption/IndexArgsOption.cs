using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GeneratorBase.MVC.Models
{
public class IndexArgsOption
{
    public string currentFilter
    {
        get;
        set;
    }
    public string searchString
    {
        get;
        set;
    }
    public string sortBy
    {
        get;
        set;
    }
    public string IsAsc
    {
        get;
        set;
    }
    public int? page
    {
        get;
        set;
    }
    public int? Pages
    {
        get;
        set;
    }
    public int? itemsPerPage
    {
        get;
        set;
    }
    public int? CurrentItemsPerPage
    {
        get;
        set;
    }
    public int pageNumber
    {
        get;
        set;
    }
    public int PageSize
    {
        get;
        set;
    }
    public string HostingEntity
    {
        get;
        set;
    }
    public int? HostingEntityID
    {
        get;
        set;
    }
    public string AssociatedType
    {
        get;
        set;
    }
    public bool? IsExport
    {
        get;
        set;
    }
    public bool? IsDeepSearch
    {
        get;
        set;
    }
    public bool? IsFilter
    {
        get;
        set;
    }
    public bool? RenderPartial
    {
        get;
        set;
    }
    public string BulkOperation
    {
        get;
        set;
    }
    public string parent
    {
        get;
        set;
    }
    public string Wfsearch
    {
        get;
        set;
    }
    public string caller
    {
        get;
        set;
    }
    public bool? BulkAssociate
    {
        get;
        set;
    }
    public string viewtype
    {
        get;
        set;
    }
    public string isMobileHome
    {
        get;
        set;
    }
    public bool? isList
    {
        get;
        set;
    }
    public bool? IsHomeList
    {
        get;
        set;
    }
    public bool IsDivRender
    {
        get;
        set;
    }
    public bool ShowDeleted
    {
        get;
        set;
    }
    public string ExportType
    {
        get;
        set;
    }
    public string FilterCondition
    {
        get;
        set;
    }
    public string FSFilter
    {
        get;
        set;
    }
    public string search
    {
        get;
        set;
    }
    public string SortOrder
    {
        get;
        set;
    }
    public string HideColumns
    {
        get;
        set;
    }
    public string GroupByColumn
    {
        get;
        set;
    }
    public bool? IsReports
    {
        get;
        set;
    }
    public bool? IsGroupBy
    {
        get;
        set;
    }
    public bool? IsdrivedTab
    {
        get;
        set;
    }
    public bool BuiltInPage
    {
        get;
        set;
    }
    public string TemplatesName
    {
        get;
        set;
    }
    public string CurrentSort
    {
        get;
        set;
    }
    public string EntityName
    {
        get;
        set;
    }
    public string HomeVal
    {
        get;
        set;
    }
    public string SearchResult
    {
        get;
        set;
    }
    
    public string CustomParameter
    {
        get;
        set;
    }
    protected void SetProperties(object target, object source)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var mainClassType = source.GetType();
        MemberInfo[] members = mainClassType.GetFields(bindingFlags).Cast<MemberInfo>()
                               .Concat(mainClassType.GetProperties(bindingFlags)).ToArray();
        foreach(var memberInfo in members)
        {
            if(memberInfo.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberInfo as PropertyInfo;
                object value = propertyInfo.GetValue(source, null);
                if(null != value)
                {
                    propertyInfo.SetValue(target, value, null);
                }
            }
            else
            {
                var fieldInfo = memberInfo as FieldInfo;
                object value = fieldInfo.GetValue(source);
                if(null != value)
                    fieldInfo.SetValue(target, value);
            }
        }
    }
}
}