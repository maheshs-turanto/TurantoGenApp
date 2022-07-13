using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.ComponentModel.DataAnnotations;
using GeneratorBase.MVC.DynamicQueryable;
using System.Web.UI.DataVisualization.Charting;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling faceted searches.</summary>
public partial class T_FacetedSearchController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="AssocType">      Type of the associated.</param>
    
    private void LoadViewDataForCount(T_FacetedSearch t_facetedsearch, string AssocType)
    {
    }
    
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    
    private void LoadViewDataAfterOnEdit(T_FacetedSearch t_facetedsearch)
    {
        CustomLoadViewDataListAfterEdit(t_facetedsearch);
    }
    
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    
    private void LoadViewDataBeforeOnEdit(T_FacetedSearch t_facetedsearch)
    {
        CustomLoadViewDataListBeforeEdit(t_facetedsearch);
    }
    
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    
    private void LoadViewDataAfterOnCreate(T_FacetedSearch t_facetedsearch)
    {
        CustomLoadViewDataListAfterOnCreate(t_facetedsearch);
    }
    
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstT_FacetedSearch">The list faceted search.</param>
    /// <param name="searchString">      The search string.</param>
    /// <param name="IsDeepSearch">      The is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<T_FacetedSearch> searchRecords(IQueryable<T_FacetedSearch> lstT_FacetedSearch, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstT_FacetedSearch = lstT_FacetedSearch.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_EntityName) && s.T_EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_LinkAddress) && s.T_LinkAddress.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_TargetEntity) && s.T_TargetEntity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_AssociationName) && s.T_AssociationName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_OtherInfo) && s.T_OtherInfo.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstT_FacetedSearch = lstT_FacetedSearch.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_EntityName) && s.T_EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_LinkAddress) && s.T_LinkAddress.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_TargetEntity) && s.T_TargetEntity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_AssociationName) && s.T_AssociationName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_OtherInfo) && s.T_OtherInfo.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstT_FacetedSearch;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstT_FacetedSearch">The list faceted search.</param>
    /// <param name="sortBy">            Describes who sort this object.</param>
    /// <param name="isAsc">             The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<T_FacetedSearch> sortRecords(IQueryable<T_FacetedSearch> lstT_FacetedSearch, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstT_FacetedSearch.Sort(sortBy, true) : lstT_FacetedSearch.Sort(sortBy, false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(T_FacetedSearch), "t_facetedsearch");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T_FacetedSearch>)lstT_FacetedSearch.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstT_FacetedSearch.ElementType, lambda.Body.Type },
                       lstT_FacetedSearch.Expression,
                       lambda));
    }
    
    /// <summary>Searches for the first f search.</summary>
    ///
    /// <param name="id">          The identifier.</param>
    /// <param name="sourceEntity">Source entity.</param>
    ///
    /// <returns>A response stream to send to the FindFSearch View.</returns>
    
    public ActionResult FindFSearch(string id, string sourceEntity)
    {
        return null;
    }
    
    /// <summary>Sets f search.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="HostingEntity">The hosting entity.</param>
    /// <param name="RenderPartial">The render partial.</param>
    ///
    /// <returns>A response stream to send to the SetFSearch View.</returns>
    
    public ActionResult SetFSearch(string searchString, string HostingEntity, bool? RenderPartial)
    {
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        if(Qcount > 0)
        {
        }
        else
        {
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
            ViewBag.FavoriteItem = lstFavoriteItem;
        ViewBag.EntityName = "T_FacetedSearch";
        var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == ViewBag.EntityName);
        var Prop = Entity.Properties.Select(z => new
        {
            z.DisplayName,
            z.Name
        });
        var sortlist = Prop;
        ViewBag.PropertyList = new SelectList(Prop, "Name", "DisplayName");
        ViewBag.SuggestedDynamicValueInCondition7 = new SelectList(Prop, "Name", "DisplayName");
        Dictionary<string, string> Dumplist = new Dictionary<string, string>();
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                          = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(Dumplist, "key", "value");
        ViewBag.SortOrder1 = new SelectList(sortlist, "Name", "DisplayName");
        ViewBag.GroupByColumn = new SelectList(sortlist, "Name", "DisplayName");
        Dictionary<string, string> columns = new Dictionary<string, string>();
        columns.Add("2", "Name");
        columns.Add("3", "Description");
        columns.Add("4", "EntityName");
        columns.Add("5", "Show To All");
        columns.Add("6", "Disable");
        columns.Add("7", "LinkAddress");
        columns.Add("8", "TargetEntity");
        columns.Add("9", "Association Name");
        columns.Add("10", "OtherInfo");
        columns.Add("11", "Flag");
        ViewBag.HideColumns = new MultiSelectList(columns, "Key", "Value");
        return View(new T_FacetedSearch());
    }
    
    /// <summary>Condition f search.</summary>
    ///
    /// <param name="property"> The property.</param>
    /// <param name="operators">The operators.</param>
    /// <param name="value">    The value.</param>
    ///
    /// <returns>A string.</returns>
    
    public string conditionFSearch(string property, string operators, string value)
    {
        string expression = string.Empty;
        var PrpertyType = GetDataTypeOfProperty("T_FacetedSearch", property);
        var dataType = PrpertyType[0];
        property = PrpertyType[1];
        if(value.StartsWith("[") && value.EndsWith("]"))
        {
            var ValueType = GetDataTypeOfProperty("T_FacetedSearch", value, true);
            if(ValueType != null && ValueType[0] == "dynamic")
            {
                dataType = ValueType[0];
                value = ValueType[1];
            }
        }
        if(value.ToLower().Trim() == "null")
        {
            expression = string.Format(" " + property + " " + operators + " {0}", "null");
            return expression;
        }
        switch(dataType)
        {
        case "Int32":
        case "Int64":
        case "Double":
        case "Boolean":
        case "Decimal":
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        case "DateTime":
        {
            if(value.Trim().ToLower() == "today")
            {
                expression = string.Format(" DbFunctions.TruncateTime(" + property + ") " + operators + " (\"{0}\")", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, (new T_FacetedSearch()).m_Timezone)).Date);
            }
            else
            {
                DateTime val = Convert.ToDateTime(value);
                expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
            }
            break;
        }
        case "Text":
        case "String":
        {
            if(operators.ToLower() == "contains")
                expression = string.Format(" " + property + "." + operators + "(\"{0}\")", value);
            else
                expression = string.Format(" " + property + " " + operators + " \"{0}\"", value);
            break;
        }
        default:
        {
            expression = string.Format(" " + property + " " + operators + " {0}", value);
            break;
        }
        }
        return expression;
    }
    
    /// <summary>Gets data type of property.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="valueType">   (Optional) True to value type.</param>
    ///
    /// <returns>The data type of property.</returns>
    
    public List<string> GetDataTypeOfProperty(string entityName, string propertyName, bool valueType = false)
    {
        var listString = new List<string>();
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        ModelReflector.Property targetPropInfo = null;
        var associatedprop = string.Empty;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        associatedprop = AssociationInfo.Name.ToLower() + "." + PropInfo.Name;
                    }
                }
            }
        }
        string DataType = string.Empty;
        if(valueType)
            DataType = "dynamic";
        else
            DataType = PropInfo.DataType;
        listString.Add(DataType);
        if(AssociationInfo != null)
            listString.Add(associatedprop);
        else
            listString.Add(propertyName);
        return listString;
    }
    
    /// <summary>Gets property dp.</summary>
    ///
    /// <param name="entityName">  Name of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    ///
    /// <returns>The property dp.</returns>
    
    public string GetPropertyDP(string entityName, string propertyName)
    {
        System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == propertyName);
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
        ModelReflector.Property targetPropInfo = null;
        var associatedprop = string.Empty;
        if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
        {
            propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(propertyName.Contains("."))
            {
                var targetProperties = propertyName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        associatedprop = "[" + AssociationInfo.DisplayName + "." + PropInfo.DisplayName + "]";
                    }
                }
            }
        }
        string PropertyName = string.Empty;
        if(AssociationInfo != null)
            PropertyName = associatedprop;
        else
            PropertyName = PropInfo.DisplayName;
        return PropertyName;
    }
    
    /// <summary>GET: /T_FacetedSearch/FSearch/.</summary>
    ///
    /// <param name="currentFilter">  A filter specifying the current.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="FSFilter">       A filter specifying the file system.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="search">         The search.</param>
    /// <param name="IsExport">       The is export.</param>
    /// <param name="T_Roles">        The roles.</param>
    /// <param name="T_Disable">      The disable.</param>
    /// <param name="T_Flag">         The flag.</param>
    /// <param name="FilterCondition">The filter condition.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="SortOrder">      The sort order.</param>
    /// <param name="HideColumns">    The hide columns.</param>
    /// <param name="GroupByColumn">  The group by column.</param>
    /// <param name="IsReports">      The is reports.</param>
    /// <param name="IsdrivedTab">    The isdrived tab.</param>
    /// <param name="IsFilter">       (Optional) A filter specifying the is.</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string T_Roles, string T_Disable, string T_Flag, string FilterCondition, string HostingEntity, string AssociatedType, string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn, bool? IsReports, bool? IsdrivedTab, bool? IsFilter = false)
    {
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewBag.SearchResult = "";
        ViewData["HideColumns"] = HideColumns;
        ViewBag.GroupByColumn = GroupByColumn;
        ViewBag.IsdrivedTab = IsdrivedTab;
        ViewData["IsFilter"] = IsFilter;
        if(!string.IsNullOrEmpty(viewtype))
            ViewBag.TemplatesName = viewtype.Replace("?IsAddPop=true", "");
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        CustomLoadViewDataListOnIndex(HostingEntity, Convert.ToInt32(HostingEntityID), AssociatedType);
        if(!string.IsNullOrEmpty(searchString) && FSFilter == null)
            ViewBag.FSFilter = "Fsearch";
        var lstT_FacetedSearch = from s in db.T_FacetedSearchs
                                 select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstT_FacetedSearch = searchRecords(lstT_FacetedSearch, searchString.ToUpper(), true);
        }
        if(!string.IsNullOrEmpty(search))
            search = search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search;
            lstT_FacetedSearch = searchRecords(lstT_FacetedSearch, search, true);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstT_FacetedSearch = sortRecords(lstT_FacetedSearch, sortBy, isAsc);
        }
        else lstT_FacetedSearch = lstT_FacetedSearch.OrderByDescending(c => c.Id);
        lstT_FacetedSearch = CustomSorting(lstT_FacetedSearch, HostingEntity, AssociatedType, sortBy, isAsc);
        lstT_FacetedSearch = lstT_FacetedSearch;
        if(!string.IsNullOrEmpty(FilterCondition))
        {
            StringBuilder whereCondition = new StringBuilder();
            var conditions = FilterCondition.Split("?".ToCharArray()).Where(lrc => lrc != "");
            int iCnt = 1;
            foreach(var cond in conditions)
            {
                if(string.IsNullOrEmpty(cond)) continue;
                var param = cond.Split(",".ToCharArray());
                var PropertyName = param[0];
                var Operator = param[1];
                var Value = string.Empty;
                var LogicalConnector = string.Empty;
                Value = param[2];
                LogicalConnector = (param[3] == "AND" ? " And" : " Or");
                if(iCnt == conditions.Count())
                    LogicalConnector = "";
                ViewBag.SearchResult += " " + GetPropertyDP("T_FacetedSearch", PropertyName) + " " + Operator + " " + Value + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch(PropertyName, Operator, Value) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstT_FacetedSearch = lstT_FacetedSearch.Where(whereCondition.ToString());
            ViewBag.FilterCondition = FilterCondition;
        }
        var DataOrdering = string.Empty;
        if(!string.IsNullOrEmpty(GroupByColumn))
        {
            DataOrdering = GroupByColumn;
            ViewBag.IsGroupBy = true;
        }
        if(!string.IsNullOrEmpty(SortOrder))
            DataOrdering += SortOrder;
        if(!string.IsNullOrEmpty(DataOrdering))
            lstT_FacetedSearch = Sorting.Sort<T_FacetedSearch>(lstT_FacetedSearch, DataOrdering);
        var _T_FacetedSearch = lstT_FacetedSearch;
        if(T_Disable != null)
        {
            try
            {
                bool boolvalue = Convert.ToBoolean(T_Disable);
                _T_FacetedSearch = _T_FacetedSearch.Where(o => o.T_Disable == boolvalue);
                ViewBag.SearchResult += "\r\n Disable= " + T_Disable;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(T_Flag != null)
        {
            try
            {
                bool boolvalue = Convert.ToBoolean(T_Flag);
                _T_FacetedSearch = _T_FacetedSearch.Where(o => o.T_Flag == boolvalue);
                ViewBag.SearchResult += "\r\n Flag= " + T_Flag;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(!string.IsNullOrEmpty(SortOrder))
        {
            ViewBag.SearchResult += " \r\n Sort Order = ";
            var sortString = "";
            var sortProps = SortOrder.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch");
            foreach(var prop in sortProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                if(prop.Contains("."))
                {
                    sortString += prop + ",";
                    continue;
                }
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    sortString += asso.DisplayName + ">";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    sortString += propInfo.DisplayName + ">";
                }
            }
            ViewBag.SearchResult += sortString.TrimEnd(">".ToCharArray());
        }
        if(!string.IsNullOrEmpty(GroupByColumn))
        {
            ViewBag.SearchResult += " \r\n Group By = ";
            var groupbyString = "";
            var GroupProps = GroupByColumn.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch");
            foreach(var prop in GroupProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    groupbyString += asso.DisplayName + " > ";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    groupbyString += propInfo.DisplayName + " > ";
                }
            }
            ViewBag.SearchResult += groupbyString.TrimEnd(" > ".ToCharArray());
        }
        ViewBag.SearchResult = ((string)ViewBag.SearchResult).TrimStart("\r\n".ToCharArray());
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_FacetedSearch"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_T_FacetedSearch.Count() > 0)
                pageSize = _T_FacetedSearch.Count();
            return View("ExcelExport", _T_FacetedSearch.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _T_FacetedSearch.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        if(!Request.IsAjaxRequest())
        {
            if(string.IsNullOrEmpty(viewtype))
                viewtype = "IndexPartial";
            ViewBag.TemplatesName = "IndexPartial";
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
                ViewBag.TemplatesName = viewtype;
            var list = _T_FacetedSearch.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityT_FacetedSearchDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_FacetedSearchlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_FacetedSearch", tagsSplit.ToArray());
                }
            return View("Index", list);
        }
        else
        {
            var list = _T_FacetedSearch.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityT_FacetedSearchDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_FacetedSearchlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_FacetedSearch", tagsSplit.ToArray());
                }
            if(ViewBag.TemplatesName == null)
                return PartialView("IndexPartial", list);
            else
                return PartialView(ViewBag.TemplatesName, list);
        }
    }
    #region Chart Methods
    
    /// <summary>Creates a title.</summary>
    ///
    /// <param name="charttitle">The charttitle.</param>
    ///
    /// <returns>The new title.</returns>
    
    public Title CreateTitle(string charttitle)
    {
        Title title = new Title();
        title.Text = charttitle;
        title.Font = new System.Drawing.Font("Helvetica", 10F, System.Drawing.FontStyle.Regular);
        title.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        return title;
    }
    
    /// <summary>Creates a legend.</summary>
    ///
    /// <param name="chartlegend">The chartlegend.</param>
    ///
    /// <returns>The new legend.</returns>
    
    public Legend CreateLegend(string chartlegend)
    {
        Legend legend = new Legend();
        legend.Title = chartlegend;
        legend.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        legend.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        legend.Docking = Docking.Bottom;
        legend.Alignment = System.Drawing.StringAlignment.Center;
        return legend;
    }
    
    /// <summary>Creates the series.</summary>
    ///
    /// <param name="chartType">  Type of the chart.</param>
    /// <param name="chartseries">The chartseries.</param>
    ///
    /// <returns>The new series.</returns>
    
    public Series CreateSeries(SeriesChartType chartType, string chartseries)
    {
        Series seriesDetail = new Series();
        seriesDetail.Name = chartseries;
        seriesDetail.IsValueShownAsLabel = false;
        if(chartType == SeriesChartType.Column)
            seriesDetail.IsVisibleInLegend = false;
        seriesDetail.ChartType = chartType;
        seriesDetail.BorderWidth = 2;
        seriesDetail.ChartArea = chartseries;
        return seriesDetail;
    }
    
    /// <summary>Creates chart area.</summary>
    ///
    /// <param name="chartType">Type of the chart.</param>
    /// <param name="chartarea">The chartarea.</param>
    /// <param name="xtitle">   The xtitle.</param>
    /// <param name="ytitle">   The ytitle.</param>
    ///
    /// <returns>The new chart area.</returns>
    
    public ChartArea CreateChartArea(SeriesChartType chartType, string chartarea, string xtitle, string ytitle)
    {
        ChartArea chartArea = new ChartArea();
        chartArea.Name = chartarea;
        chartArea.BackColor = System.Drawing.Color.Transparent;
        chartArea.AxisX.IsLabelAutoFit = false;
        chartArea.AxisY.IsLabelAutoFit = false;
        chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.Interval = 1;
        if(chartType == SeriesChartType.Column)
            chartArea.AxisX.LabelStyle.Angle = -90;
        chartArea.AxisX.Title = xtitle;
        chartArea.AxisY.Title = ytitle;
        return chartArea;
    }
    #endregion
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        long idvalue = Convert.ToInt64(id);
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var _Obj = db1.T_FacetedSearchs.FirstOrDefault(p => p.Id == idvalue);
        return _Obj == null ? "" : _Obj.DisplayValue;
    }
    
    /// <summary>Gets extra journal entry.</summary>
    ///
    /// <param name="id">  The identifier.</param>
    /// <param name="user">The user.</param>
    /// <param name="jedb">The jedb.</param>
    ///
    /// <returns>The extra journal entry.</returns>
    
    public IQueryable<JournalEntry> GetExtraJournalEntry(int? id, Models.IUser user, JournalEntryContext jedb)
    {
        var listjournaliquery = jedb.JournalEntries.Where(p => p.Id == 0);
        Expression<Func<JournalEntry, bool>> predicateJournalEntry = n => false;
        listjournaliquery = new FilteredDbSet<JournalEntry>(jedb, predicateJournalEntry);
        return listjournaliquery;
    }
    
    /// <summary>Gets record by identifier.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier.</returns>
    
    public object GetRecordById(string id)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        if(string.IsNullOrEmpty(id)) return "";
        var _Obj = db1.T_FacetedSearchs.Find(Convert.ToInt64(id));
        return _Obj;
    }
    
    /// <summary>
    /// Gets JsonResult By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Json Data</returns>
    public JsonResult GetJsonResultById(string id)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var data = db1.T_FacetedSearchs.Find(Convert.ToInt64(id));
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets record by identifier reflection.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier reflection.</returns>
    
    public string GetRecordById_Reflection(string id)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        if(string.IsNullOrEmpty(id)) return "";
        var _Obj = db1.T_FacetedSearchs.Find(Convert.ToInt64(id));
        return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_FacetedSearch>(_Obj, "T_FacetedSearch", new string[] { "" }); ;
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                   select new { Id = x.Id, Name = x.DisplayValue };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.T_FacetedSearchs;
                string lambda = "";
                foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                {
                    lambda += asso + "=" + AssociationID + " OR ";
                }
                lambda = lambda.TrimEnd(" OR ".ToCharArray());
                query = query.Where(lambda);
                list = ((IQueryable<T_FacetedSearch>)query);
            }
            else if(AssoID == 0)
            {
                IQueryable query = db.T_FacetedSearchs;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T_FacetedSearch), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T_FacetedSearch, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_FacetedSearch>)q);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_FacetedSearch>(User, list, "T_FacetedSearch", caller);
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForRB action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.T_FacetedSearchs;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T_FacetedSearch), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T_FacetedSearch, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_FacetedSearch>)q);
            }
        }
        var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                   select new { Id = x.Id, Name = x.DisplayValue };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueMobile action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            try
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if(AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_FacetedSearchs;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_FacetedSearch), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_FacetedSearch, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_FacetedSearch>)q).Take(20);
                }
            }
            catch
            {
                var data = from x in list.OrderBy(q => q.DisplayValue).Take(20).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_FacetedSearch>(User, list, "T_FacetedSearch", caller);
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(20).Union(list.Where(p => p.Id == val)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(20).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.Id != val).Take(20).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.OrderBy(q => q.DisplayValue).Take(20).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="propNameBR">The property name line break.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValueForBR action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            //added new code (Remove old code if everything works)
            var result = list.Select("new(Id," + propNameBR + " as value)");
            return Json(result, JsonRequestBehavior.AllowGet);
            //
            ParameterExpression param = Expression.Parameter(typeof(T_FacetedSearch), "d");
            //bulid expression tree:data.Field1
            var Property = typeof(T_FacetedSearch).GetProperty(propNameBR);
            Expression selector = Expression.Property(param, Property);
            Expression pred = Expression.Lambda(selector, param);
            //bulid expression tree:Select(d=>d.Field1)
            var targetType = Property.PropertyType;
            if(targetType.GetGenericArguments().Count() > 0)
            {
                if(targetType.GetGenericArguments()[0].Name == "DateTime")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(DateTime?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<DateTime?> query = list.Provider.CreateQuery<DateTime?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
            }
            if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if(targetType.GetGenericArguments()[0].Name == "Decimal")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(decimal?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<decimal?> query = list.Provider.CreateQuery<decimal?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Boolean")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(bool?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<bool?> query = list.Provider.CreateQuery<bool?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Int32")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(Int32?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<Int32?> query = list.Provider.CreateQuery<Int32?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Int64")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(Int64?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<Int64?> query = list.Provider.CreateQuery<Int64?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Double")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(T_FacetedSearch), typeof(double?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<double?> query = list.Provider.CreateQuery<double?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                  new Type[] { typeof(T_FacetedSearch), typeof(string) }, Expression.Constant(list.AsQueryable()), pred);
                //var result = query.AsEnumerable().Take(10);
                IQueryable<string> query = list.Provider.CreateQuery<string>(expr).Distinct();
                return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
            }
            return Json(list.AsEnumerable(), JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValue(string key, string AssoNameWithParent, string AssociationID)
    {
        IQueryable<T_FacetedSearch> list = db.T_FacetedSearchs;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            IQueryable query = db.T_FacetedSearchs;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = AssoNameWithParent;
            var AssoIDs = AssociationID.Split(',').ToList();
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Parameter(typeof(T_FacetedSearch), "p"));
            MemberExpression member = Expression.PropertyOrField(paramList[0], propToWhere);
            List<LambdaExpression> lexList = new List<LambdaExpression>();
            Expression ex = null;
            for(int i = 0; i < AssoIDs.Count; i++)
            {
                if(string.IsNullOrEmpty(AssoIDs[i].ToString()))
                    continue;
                Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i].ToString());
                if(i == 0)
                {
                    Expression bodyInner = Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type));
                    lexList.Add(Expression.Lambda(bodyInner, paramList[0]));
                }
                else
                {
                    ex = Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type));
                    Expression bodyOuter = Expression.Or(lexList[lexList.Count - 1].Body, ex);
                    lexList.Add(Expression.Lambda(bodyOuter, paramList[0]));
                    ex = null;
                }
            }
            LambdaExpression lambda = (lexList[lexList.Count - 1]);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            list = ((IQueryable<T_FacetedSearch>)q);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_FacetedSearch>(User, list, "T_FacetedSearch", null);
        if(key != null && key.Length > 0)
        {
            var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(s => s.DisplayValue.IndexOf(key)).ThenBy(p => p.DisplayValue.Length).ThenBy(p => p.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    
    /// <summary>Data import.</summary>
    ///
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="fileLocation"> The file location.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    private DataSet DataImport(string fileExtension, string fileLocation)
    {
        string excelConnectionString = string.Empty;
        if(fileExtension == ".xls")
        {
            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
        }
        else if(fileExtension == ".csv")
        {
            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(fileLocation) + ";Extended Properties=\"Text;HDR=YES;FMT=Delimited\"";
        }
        else if(fileExtension == ".xlsx")
        {
            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        }
        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
        excelConnection.Open();
        DataSet objDataSet = new DataSet();
        DataTable dt = null;
        string query = string.Empty;
        String[] excelSheets = null;
        if(fileExtension == ".csv")
        {
            query = "SELECT * FROM [" + System.IO.Path.GetFileName(fileLocation) + "]";
        }
        else
        {
            dt = new DataTable();
            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if(dt == null)
            {
                return null;
            }
            excelSheets = new String[dt.Rows.Count];
            int t = 0;
            foreach(DataRow row in dt.Rows)
            {
                excelSheets[t] = row["TABLE_NAME"].ToString();
                t++;
            }
            query = string.Format("Select * from [{0}]", excelSheets[0]);
        }
        excelConnection.Close();
        using(OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        {
            dataAdapter.Fill(objDataSet);
        }
        excelConnection1.Close();
        return WithoutBlankRow(objDataSet);
    }
    
    /// <summary>Without blank row.</summary>
    ///
    /// <param name="ds">The ds.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    private DataSet WithoutBlankRow(DataSet ds)
    {
        DataSet dsnew = new DataSet();
        DataTable dt = ds.Tables[0].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field).ToString().Trim(), string.Empty) == 0)).CopyToDataTable();
        dsnew.Tables.Add(dt);
        return dsnew;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [HttpGet]
    public ActionResult Upload()
    {
        if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "T_FacetedSearch", User) || !User.CanAdd("T_FacetedSearch"))
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_FacetedSearch")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_FacetedSearch").ToList();
        var distinctMapping = lstMappings.GroupBy(p => p.MappingName).Distinct();
        List<ImportConfiguration> ddlMappingList = new List<ImportConfiguration>();
        foreach(var elem in distinctMapping)
        {
            ddlMappingList.Add(elem.FirstOrDefault());
        }
        var DefaultMapping = lstMappings.Where(p => p.IsDefaultMapping).FirstOrDefault();
        var mappingID = DefaultMapping == null ? "" : DefaultMapping.MappingName;
        ViewBag.IsDefaultMapping = DefaultMapping != null ? true : false;
        //ViewBag.ListOfMappings = new SelectList(ddlMappingList, "ID", "MappingName", mappingID);
        ViewBag.ListOfMappings = new SelectList(ddlMappingList, "MappingName", "MappingName", mappingID);
        ViewBag.Title = "Upload File";
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <param name="FileUpload">The file upload.</param>
    /// <param name="collection">The collection.</param>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Upload([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection)
    {
        if(FileUpload != null)
        {
            string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();
            if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv" || fileExtension == ".all")
            {
                string rename = string.Empty;
                if(fileExtension == ".all")
                {
                    rename = System.IO.Path.GetFileName(FileUpload.FileName.ToLower().Replace(fileExtension, ".csv"));
                    fileExtension = ".csv";
                }
                else
                    rename = System.IO.Path.GetFileName(FileUpload.FileName);
                string fileLocation = string.Format("{0}\\{1}", Server.MapPath("~/ExcelFiles"), rename);
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                FileUpload.SaveAs(fileLocation);
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
                var col = new List<SelectListItem>();
                if(objDataSet.Tables.Count > 0)
                {
                    int iCols = objDataSet.Tables[0].Columns.Count;
                    if(iCols > 0)
                    {
                        for(int i = 0; i < iCols; i++)
                        {
                            col.Add(new SelectListItem { Value = (i + 1).ToString(), Text = objDataSet.Tables[0].Columns[i].Caption });
                        }
                    }
                }
                col.Insert(0, new SelectListItem { Value = "", Text = "Select Column" });
                Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList> objColMapAssocProperties = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList>();
                Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>();
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_FacetedSearch");
                if(entList != null)
                {
                    foreach(var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                    {
                        long selectedVal = 0;
                        var colSelected = col.FirstOrDefault(p => p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                        if(colSelected != null)
                            selectedVal = long.Parse(colSelected.Value);
                        objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                    }
                    List<GeneratorBase.MVC.ModelReflector.Association> assocList = entList.Associations;
                    if(assocList != null)
                    {
                        foreach(var assoc in assocList)
                        {
                            if(assoc.Target == "IdentityUser")
                                continue;
                            Dictionary<string, string> lstProperty = new Dictionary<string, string>();
                            var assocEntity = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == assoc.Target);
                            var assocProperties = assocEntity.Properties.Where(p => p.Name != "DisplayValue");
                            lstProperty.Add("DisplayValue", "DisplayValue-" + assoc.AssociationProperty);
                            foreach(var prop in assocProperties)
                            {
                                lstProperty.Add(prop.DisplayName, prop.DisplayName + "-" + assoc.AssociationProperty);
                            }
                            // var dispValue = lstProperty.Keys.FirstOrDefault();
                            objColMapAssocProperties.Add(assoc, new SelectList(lstProperty.AsEnumerable(), "Value", "Key", "Key"));
                        }
                    }
                }
                ViewBag.AssociatedProperties = objColMapAssocProperties;
                ViewBag.ColumnMapping = objColMap;
                ViewBag.FilePath = fileLocation;
                if(!string.IsNullOrEmpty(collection["ListOfMappings"]))
                {
                    string typeName = "";
                    string colKey = "";
                    string colDisKey = "";
                    string colListInx = "";
                    typeName = "T_FacetedSearch";
                    //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                    string idMapping = collection["ListOfMappings"];
                    string ExistsColumnMappingName = string.Empty;
                    string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    if(collection["DefaultMapping"] == "on")
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_FacetedSearch" && p.IsDefaultMapping);
                        if(lstMapping.Count() > 0)
                        {
                            foreach(var mapping in lstMapping)
                            {
                                mapping.IsDefaultMapping = false;
                                db.Entry(mapping).State = EntityState.Modified;
                            }
                        }
                        foreach(var defaultMapping in DefaultMapping)
                        {
                            defaultMapping.IsDefaultMapping = true;
                            db.Entry(defaultMapping).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                    foreach(var defcol in ViewBag.ColumnMapping as Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>)
                    {
                        colDisKey = colDisKey + defcol.Key.DisplayName + ",";
                        colKey = colKey + defcol.Key.Name + ",";
                        string colSelected = (DefaultMapping.ToList().Where(p => p.TableColumn == defcol.Key.DisplayName).Count() > 0 ? DefaultMapping.ToList().Where(p => p.TableColumn == defcol.Key.DisplayName).FirstOrDefault().SheetColumn : null);
                        int colExist = 0;
                        if(!string.IsNullOrEmpty(colSelected))
                        {
                            colExist = defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).Count();
                            if(colExist == 0)
                                ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
                            colListInx = colListInx + (colExist > 0 ? defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).First().Value.ToString() : "") + ",";
                        }
                        else
                            colListInx = colListInx + "" + ",";
                    }
                    if(colKey != "")
                        colKey = colKey.Substring(0, colKey.Length - 1);
                    if(colDisKey != "")
                        colDisKey = colDisKey.Substring(0, colDisKey.Length - 1);
                    if(colListInx != "")
                        colListInx = colListInx.Substring(0, colListInx.Length - 1);
                    if(!string.IsNullOrEmpty(ExistsColumnMappingName))
                        ExistsColumnMappingName = ExistsColumnMappingName.Trim().Substring(0, ExistsColumnMappingName.Trim().Length - 1);
                    string FilePath = ViewBag.FilePath;
                    var columnlist = colKey;
                    var columndisplaynamelist = colDisKey;
                    var selectedlist = colListInx;
                    string DefaultColumnMappingName = string.Empty;
                    if(DefaultMapping.Count > 0)
                        DefaultColumnMappingName = String.Join(", ", DefaultMapping.OrderByDescending(p => p.Id).Select(p => p.TableColumn));
                    ViewBag.DefaultMappingMsg = null;
                    if(DefaultMapping.Count() != colListInx.Split(',').Where(p => p.Trim() != string.Empty).Count())
                    {
                        ViewBag.DefaultMappingMsg += "There was an ERROR in file being uploaded: It does not contain all the required columns.";
                        ViewBag.DefaultMappingMsg += "<br /><br /> Error Details: <br /> The following columns are missing : " + ExistsColumnMappingName;
                        ViewBag.DefaultMappingMsg += "<br /><br /> Please verify the file and upload again. No data has currently been imported and NO change has been made.";
                    }
                    string DetailMessage = "";
                    string excelConnectionString = string.Empty;
                    DataTable tempdt = new DataTable();
                    if(selectedlist != null && columnlist != null)
                    {
                        var dtsheetColumns = selectedlist.Split(',').ToList();
                        var dttblColumns = columndisplaynamelist.Split(',').ToList();
                        for(int j = 0; j < dtsheetColumns.Count; j++)
                        {
                            string columntable = dttblColumns[j];
                            int columnSheet = 0;
                            if(string.IsNullOrEmpty(dtsheetColumns[j]))
                                continue;
                            else
                                columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                            tempdt.Columns.Add(columntable);
                        }
                        for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            var sheetColumns = selectedlist.Split(',').ToList();
                            if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                                continue;
                            var tblColumns = columndisplaynamelist.Split(',').ToList();
                            DataRow objdr = tempdt.NewRow();
                            for(int j = 0; j < sheetColumns.Count; j++)
                            {
                                string columntable = tblColumns[j];
                                int columnSheet = 0;
                                if(string.IsNullOrEmpty(sheetColumns[j]))
                                    continue;
                                else
                                    columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                                string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                                if(string.IsNullOrEmpty(columnValue))
                                    continue;
                                objdr[columntable] = columnValue;
                            }
                            tempdt.Rows.Add(objdr);
                        }
                    }
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new FacetedSearchs";
                    Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
                    if(entList != null)
                    {
                        DataTable uniqueCols = new DataTable();
                        foreach(var association in entList.Associations)
                        {
                            if(!tempdt.Columns.Contains(association.DisplayName))
                                continue;
                            uniqueCols = tempdt.DefaultView.ToTable(true, association.DisplayName);
                            List<String> uniqueassoValues = new List<String>();
                            for(int i = 0; i < uniqueCols.Rows.Count; i++)
                            {
                                string assovalue = "";
                                if(string.IsNullOrEmpty(uniqueCols.Rows[i][0].ToString().Trim()))
                                    continue;
                                else
                                    assovalue = uniqueCols.Rows[i][0].ToString();
                                #region Association Values
                                switch(association.AssociationProperty)
                                {
                                default:
                                    break;
                                }
                                #endregion
                            }
                            if(uniqueassoValues.Count > 0)
                            {
                                DetailMessage += ", " + uniqueassoValues.Count() + " <b>new " + (association.DisplayName.EndsWith("s") ? association.DisplayName + "</b>" : association.DisplayName + "s</b>");
                                objAssoUnique.Add(association, uniqueassoValues.ToList());
                                if(!User.CanAdd(association.Target) && ViewBag.Confirm == null)
                                    ViewBag.Confirm = true;
                            }
                        }
                        if(objAssoUnique.Count > 0)
                            ViewBag.AssoUnique = objAssoUnique;
                        if(!string.IsNullOrEmpty(DetailMessage))
                            ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
                        ViewBag.ColumnMapping = null;
                        ViewBag.FilePath = FilePath;
                        ViewBag.ColumnList = columnlist;
                        ViewBag.SelectedList = selectedlist;
                        ViewBag.ConfirmImportData = tempdt;
                        if(ViewBag.ConfirmImportData != null)
                        {
                            ViewBag.Title = "Data Preview";
                            return View("Upload");
                        }
                        else
                            return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Plese select Excel File.");
            }
        }
        ViewBag.Title = "Column Mapping";
        return View("Upload");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) confirm import data.</summary>
    ///
    /// <param name="collection">The collection.</param>
    ///
    /// <returns>A response stream to send to the ConfirmImportData View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmImportData(FormCollection collection)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["lblColumn"];
        var columndisplaynamelist = collection["lblColumnDisplayName"];
        var selectedlist = collection["colList"];
        var selectedAssocPropList = collection["colAssocPropList"];
        bool SaveMapping = collection["SaveMapping"] == "on" ? true : false;
        string mappingName = collection["MappingName"];
        string DetailMessage = "";
        string fileLocation = FilePath;
        string excelConnectionString = string.Empty;
        string typename = "T_FacetedSearch";
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            if(!String.IsNullOrEmpty(mappingName))
            {
                if(SaveMapping)
                {
                    var lstMapping = db.ImportConfigurations.Where(p => p.Name == typename && p.IsDefaultMapping);
                    if(lstMapping.Count() > 0)
                    {
                        foreach(var mapping in lstMapping)
                        {
                            mapping.IsDefaultMapping = false;
                            db.Entry(mapping).State = EntityState.Modified;
                        }
                    }
                }
                var tblcols = columndisplaynamelist.Split(',').ToList();
                var shtcols = selectedlist.Split(',').ToList();
                //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typename).ToList();
                for(int i = 0; i < tblcols.Count(); i++)
                {
                    ImportConfiguration objImtConfig = null;
                    string shtcolName = string.IsNullOrEmpty(shtcols[i]) ? "" : objDataSet.Tables[0].Columns[int.Parse(shtcols[i]) - 1].Caption;
                    objImtConfig = new ImportConfiguration();
                    objImtConfig.Name = typename;
                    objImtConfig.MappingName = mappingName;
                    objImtConfig.IsDefaultMapping = SaveMapping;
                    objImtConfig.TableColumn = tblcols[i];
                    objImtConfig.SheetColumn = shtcolName;
                    db.ImportConfigurations.Add(objImtConfig);
                }
                db.SaveChanges();
            }
            DataTable tempdt = new DataTable();
            if(selectedlist != null && columnlist != null)
            {
                var dtsheetColumns = selectedlist.Split(',').ToList();
                var dttblColumns = columndisplaynamelist.Split(',').ToList();
                for(int j = 0; j < dtsheetColumns.Count; j++)
                {
                    string columntable = dttblColumns[j];
                    int columnSheet = 0;
                    if(string.IsNullOrEmpty(dtsheetColumns[j]))
                        continue;
                    else
                        columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                    tempdt.Columns.Add(columntable);
                }
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    var sheetColumns = selectedlist.Split(',').ToList();
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    var tblColumns = columndisplaynamelist.Split(',').ToList();
                    DataRow objdr = tempdt.NewRow();
                    for(int j = 0; j < sheetColumns.Count; j++)
                    {
                        string columntable = tblColumns[j];
                        int columnSheet = 0;
                        if(string.IsNullOrEmpty(sheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                        string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                        if(string.IsNullOrEmpty(columnValue))
                            continue;
                        if(CustomImportDataValidate(objDataSet, objDataSet.Tables[0].Columns[columnSheet].ColumnName, columnValue))
                            objdr[columntable] = columnValue;
                    }
                    tempdt.Rows.Add(objdr);
                }
            }
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new FacetedSearchs";
            Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
            if(!string.IsNullOrEmpty(selectedAssocPropList))
            {
                var entitypropList = selectedAssocPropList.Split(',');
                foreach(var prop in entitypropList)
                {
                    var lst = prop.Split('-');
                    lstEntityProp.Add(lst[1], lst[0]);
                }
            }
            Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_FacetedSearch");
            if(entList != null)
            {
                DataTable uniqueCols = new DataTable();
                foreach(var association in entList.Associations)
                {
                    if(!tempdt.Columns.Contains(association.DisplayName))
                        continue;
                    uniqueCols = tempdt.DefaultView.ToTable(true, association.DisplayName);
                    List<String> uniqueassoValues = new List<String>();
                    for(int i = 0; i < uniqueCols.Rows.Count; i++)
                    {
                        string assovalue = "";
                        if(string.IsNullOrEmpty(uniqueCols.Rows[i][0].ToString().Trim()))
                            continue;
                        else
                            assovalue = uniqueCols.Rows[i][0].ToString();
                        #region Association Values
                        switch(association.AssociationProperty)
                        {
                        default:
                            break;
                        }
                        #endregion
                    }
                    if(uniqueassoValues.Count > 0)
                    {
                        DetailMessage += ", " + uniqueassoValues.Count() + " <b>new " + (association.DisplayName.EndsWith("s") ? association.DisplayName + "</b>" : association.DisplayName + "s</b>");
                        objAssoUnique.Add(association, uniqueassoValues.ToList());
                        if(!User.CanAdd(association.Target) && ViewBag.Confirm == null)
                            ViewBag.Confirm = true;
                    }
                }
            }
            if(objAssoUnique.Count > 0)
                ViewBag.AssoUnique = objAssoUnique;
            if(!string.IsNullOrEmpty(DetailMessage))
                ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
            ViewBag.FilePath = FilePath;
            ViewBag.ColumnList = columnlist;
            ViewBag.SelectedList = selectedlist;
            ViewBag.ConfirmImportData = tempdt;
            ViewBag.colAssocPropList = selectedAssocPropList;
            if(ViewBag.ConfirmImportData != null)
            {
                ViewBag.Title = "Data Preview";
                return View("Upload");
            }
            else
                return RedirectToAction("Index");
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) import data.</summary>
    ///
    /// <param name="collection">The collection.</param>
    ///
    /// <returns>A response stream to send to the ImportData View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ImportData(FormCollection collection)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnColumnList"];
        var selectedlist = collection["hdnSelectedList"];
        string fileLocation = FilePath;
        string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        var selectedAssocPropList = collection["hdnSelectedAssocPropList"];
        Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(selectedAssocPropList))
        {
            var entitypropList = selectedAssocPropList.Split(',');
            foreach(var prop in entitypropList)
            {
                var lst = prop.Split('-');
                lstEntityProp.Add(lst[1], lst[0]);
            }
        }
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            string error = string.Empty;
            if(selectedlist != null && columnlist != null)
            {
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    var sheetColumns = selectedlist.Split(',').ToList();
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    T_FacetedSearch model = new T_FacetedSearch();
                    var tblColumns = columnlist.Split(',').ToList();
                    for(int j = 0; j < sheetColumns.Count; j++)
                    {
                        string columntable = tblColumns[j];
                        int columnSheet = 0;
                        if(string.IsNullOrEmpty(sheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                        string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                        if(string.IsNullOrEmpty(columnValue))
                            continue;
                        switch(columntable)
                        {
                        case "T_Name":
                            model.T_Name = columnValue;
                            break;
                        case "T_Description":
                            model.T_Description = columnValue;
                            break;
                        case "T_EntityName":
                            model.T_EntityName = columnValue;
                            break;
                        case "T_Disable":
                            model.T_Disable = Boolean.Parse(columnValue);
                            break;
                        case "T_LinkAddress":
                            model.T_LinkAddress = columnValue;
                            break;
                        case "T_TargetEntity":
                            model.T_TargetEntity = columnValue;
                            break;
                        case "T_AssociationName":
                            model.T_AssociationName = columnValue;
                            break;
                        case "T_OtherInfo":
                            model.T_OtherInfo = columnValue;
                            break;
                        case "T_Flag":
                            model.T_Flag = Boolean.Parse(columnValue);
                            break;
                        default:
                            break;
                        }
                    }
                    CheckBeforeSave(model, "ImportData");
                    var customimport_hasissues = false;
                    if(ValidateModel(model))
                    {
                        var result = CheckMandatoryProperties(model);
                        if(result == null || result.Count == 0)
                        {
                            var customerror = "";
                            if(!CustomSaveOnImport(model, out customerror, i))
                            {
                                db.T_FacetedSearchs.Add(model);
                                db.SaveChanges();
                            }
                            error += customerror;
                        }
                        else
                        {
                            if(ViewBag.ImportError == null)
                                ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                            else
                                ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                            error += ((i + 1).ToString()) + ",";
                        }
                    }
                    else
                    {
                        if(ViewBag.ImportError == null)
                            ViewBag.ImportError = "Row No : " + (i + 1) + " Value is Blank or Duplicate or Validation failed.";
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " Value is Blank or Duplicate or Validation failed.";
                        error += ((i + 1).ToString()) + ",";
                    }
                }
            }
            if(ViewBag.ImportError != null)
            {
                ViewBag.FilePath = FilePath;
                ViewBag.ErrorList = error.Substring(0, error.Length - 1);
                ViewBag.Title = "Error List";
                return View("Upload");
            }
            else
            {
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                if(ViewBag.ImportError == null)
                {
                    ViewBag.ImportError = "success";
                    ViewBag.Title = "Upload List";
                    return View("Upload");
                }
                return RedirectToAction("Index");
            }
        }
        return View();
    }
    
    /// <summary>Downloads the sheet described by collection.</summary>
    ///
    /// <param name="collection">The collection.</param>
    ///
    /// <returns>A response stream to send to the DownloadSheet View.</returns>
    
    public ActionResult DownloadSheet(FormCollection collection)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnErrorList"];
        string fileLocation = FilePath;
        string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            if(System.IO.File.Exists(fileLocation))
                System.IO.File.Delete(fileLocation);
            (new DataToExcel()).ExportDetails(objDataSet.Tables[0], fileExtension == ".csv" ? "CSV" : "Excel", "DownloadError" + (fileExtension == ".csv" ? ".csv" : ".xls"), columnlist.Split(',').ToList());
        }
        return View();
    }
    
    /// <summary>Validates the model described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ValidateModel(T_FacetedSearch validate)
    {
        return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null, true);
    }
    
    /// <summary>Determine if we are all columns empty.</summary>
    ///
    /// <param name="dr">          The dr.</param>
    /// <param name="sheetColumns">The sheet columns.</param>
    ///
    /// <returns>True if all columns empty, false if not.</returns>
    
    bool AreAllColumnsEmpty(DataRow dr, List<string> sheetColumns)
    {
        if(dr == null)
        {
            return true;
        }
        else
        {
            for(int i = 0; i < sheetColumns.Count(); i++)
            {
                if(string.IsNullOrEmpty(sheetColumns[i]))
                    continue;
                if(dr[Convert.ToInt32(sheetColumns[i]) - 1] != null && dr[Convert.ToInt32(sheetColumns[i]) - 1].ToString() != "")
                {
                    return false;
                }
            }
            return true;
        }
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="typename">The typename.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMapping action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetMapping(string typename)
    {
        bool isMapping = (db.ImportConfigurations.Where(p => p.LastUpdateUser == User.Name && p.Name == typename)).Count() > 0 ? true : false;
        return Json(isMapping, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets field value by entity identifier.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>The field value by entity identifier.</returns>
    
    public object GetFieldValueByEntityId(long Id, string PropName)
    {
        try
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var obj1 = db1.T_FacetedSearchs.Find(Id);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            return PropValue;
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    
    [HttpPost]
    public JsonResult GetReadOnlyProperties(T_FacetedSearch OModel)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel.setCalculation();
                var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_FacetedSearch");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 4);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                    var listArgs = Args;
                    foreach(var parametersArgs in Args)
                    {
                        var dispName = "";
                        var paramName = parametersArgs.ParameterName;
                        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch");
                        var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                        if(property != null)
                            dispName = property.DisplayName;
                        else
                        {
                            if(paramName.Contains("."))
                            {
                                var arrparamName = paramName.Split('.');
                                var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);
                                var targetPropName = ModelReflector.Entities.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                            }
                        }
                        if(!RulesApplied.ContainsKey(paramName))
                        {
                            RulesApplied.Add(paramName, dispName);
                            var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
                            if(!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
                                RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    
    [HttpPost]
    public JsonResult GetMandatoryProperties(T_FacetedSearch OModel, string ruleType)
    {
        Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                if(ruleType == "OnCreate")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                else if(ruleType == "OnEdit")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                OModel.setCalculation();
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_FacetedSearch");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var ruleActions = new RuleActionContext().RuleActions.Where(p => p.AssociatedActionTypeID == 2).Select(p => p.RuleActionID).ToList();
                var BRFail = BRAll.Except(BR);
                BRFail = BRFail.Where(p => ruleActions.Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    var listArgs = Args;
                    foreach(var parametersArgs in Args)
                    {
                        var dispName = "";
                        var paramName = parametersArgs.ParameterName;
                        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch");
                        var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                        if(property != null)
                            dispName = property.DisplayName;
                        else
                        {
                            if(paramName.Contains("."))
                            {
                                var arrparamName = paramName.Split('.');
                                var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);
                                var targetPropName = ModelReflector.Entities.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                            }
                        }
                        if(!RequiredProperties.ContainsKey(paramName))
                        {
                            RequiredProperties.Add(paramName, dispName);
                            var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
                            if(!RequiredProperties.ContainsKey("FailureMessage-" + objBR.Id))
                                RequiredProperties.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RequiredProperties.ContainsKey("InformationMessage-" + objBR.Id))
                            RequiredProperties.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    
    [HttpPost]
    public JsonResult GetUIAlertBusinessRules(T_FacetedSearch OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "T_FacetedSearch");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 13);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(13))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        RulesApplied.Add("<span style=\"font-weight:bold\">Warning(#" + rules.Value.BRID + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                        foreach(var objBR in BRList)
                        {
                            if(!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
                                RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSaveProperties action.</returns>
    
    [HttpPost]
    public JsonResult GetValidateBeforeSaveProperties(T_FacetedSearch OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            EntityState state = EntityState.Added;
            if(ruleType == "OnEdit")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                state = EntityState.Modified;
            }
            if(ruleType == "OnCreate")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                state = EntityState.Added;
            }
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel.setCalculation();
                var ResultOfBusinessRules = db.ValidateBeforeSavePropertiesRule(OModel, BR, "T_FacetedSearch", state);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 10);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(10))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        if(rules.Key.TypeNo == 10)
                        {
                            var ruleconditionsdb = new ConditionContext().Conditions.Where(p => p.RuleConditionsID == rules.Value.BRID);
                            foreach(var condition in ruleconditionsdb)
                            {
                                string conditionPropertyName = condition.PropertyName;
                                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch");
                                var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName);
                                var AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == conditionPropertyName);
                                var propDispName = "";
                                if(conditionPropertyName.StartsWith("[") && conditionPropertyName.EndsWith("]"))
                                {
                                    conditionPropertyName = conditionPropertyName.TrimStart('[').TrimEnd(']').Trim();
                                    if(conditionPropertyName.Contains("."))
                                    {
                                        var targetProperties = conditionPropertyName.Split(".".ToCharArray());
                                        if(targetProperties.Length > 1)
                                        {
                                            AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                            if(AssociationInfo != null)
                                            {
                                                var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                                conditionPropertyName = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]).DisplayName;
                                            }
                                        }
                                        propDispName = AssociationInfo.DisplayName + "." + conditionPropertyName;
                                    }
                                }
                                else
                                {
                                    propDispName = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName).DisplayName;
                                }
                                conditions += propDispName + " " + condition.Operator + " " + condition.Value + ", ";
                            }
                        }
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(','));
                        //RulesApplied.Add("<span style=\"font-weight:bold\">Warning(#" + rules.Value.BRID + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                        foreach(var objBR in BRList)
                        {
                            if(!RulesApplied.ContainsKey("<span style=\"font-weight:bold\">Warning(#" + objBR.Id + ") :</span> "))
                            {
                                if(!string.IsNullOrEmpty(objBR.FailureMessage))
                                    RulesApplied.Add("<span style=\"font-weight:bold\">Warning(#" + objBR.Id + ") :</span> ", objBR.FailureMessage);
                                else
                                    RulesApplied.Add("<span style=\"font-weight:bold\">Warning(#" + objBR.Id + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
                            }
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult GetLockBusinessRules(T_FacetedSearch OModel)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //string RulesApplied = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel.setCalculation();
                var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "T_FacetedSearch");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 2);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1) || ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                {
                    var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                    foreach(var objBR in BRList)
                    {
                        RulesApplied.Add("Rule #" + objBR.Id + " is Applied", objBR.RuleName);
                        if(!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
                            RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Check mandatory properties.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    
    private List<string> CheckMandatoryProperties(T_FacetedSearch OModel)
    {
        List<string> result = new List<string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_FacetedSearch").ToList();
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_FacetedSearch");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach(string paramName in Args.Select(p => p.ParameterName))
                    {
                        var type = OModel.GetType();
                        if(type.GetProperty(paramName) == null) continue;
                        var propertyvalue = type.GetProperty(paramName).GetValue(OModel, null);
                        if(propertyvalue == null)
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_FacetedSearch").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            result.Add(dispName);
                        }
                    }
                }
            }
        }
        return result;
    }
    
    /// <summary>Gets identifier from display value.</summary>
    ///
    /// <param name="displayvalue">The displayvalue.</param>
    ///
    /// <returns>The identifier from display value.</returns>
    
    public long? GetIdFromDisplayValue(string displayvalue)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        if(string.IsNullOrEmpty(displayvalue)) return 0;
        var _Obj = db1.T_FacetedSearchs.FirstOrDefault(p => p.DisplayValue == displayvalue);
        long outValue;
        if(_Obj != null)
            return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
        else return 0;
    }
    
    /// <summary>Gets identifier from property value.</summary>
    ///
    /// <param name="PropName"> Name of the property.</param>
    /// <param name="PropValue">The property value.</param>
    ///
    /// <returns>The identifier from property value.</returns>
    
    public long? GetIdFromPropertyValue(string PropName, string PropValue)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        IQueryable query = db1.T_FacetedSearchs;
        Type[] exprArgTypes = { query.ElementType };
        string propToWhere = PropName;
        ParameterExpression p = Expression.Parameter(typeof(T_FacetedSearch), "p");
        MemberExpression member = Expression.PropertyOrField(p, propToWhere);
        LambdaExpression lambda = null;
        if(PropValue.ToLower().Trim() != "null")
            lambda = Expression.Lambda<Func<T_FacetedSearch, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
        else
            lambda = Expression.Lambda<Func<T_FacetedSearch, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
        IQueryable q = query.Provider.CreateQuery(methodCall);
        long outValue;
        var list1 = ((IQueryable<T_FacetedSearch>)q);
        if(list1 != null && list1.Count() > 0)
            return Int64.TryParse(list1.FirstOrDefault().Id.ToString(), out outValue) ? (long?)outValue : null;
        else return 0;
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyValueByEntityId action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        var obj1 = db1.T_FacetedSearchs.Find(Id);
        if(obj1 == null)
            return Json("0", JsonRequestBehavior.AllowGet);
        System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var Property = properties.FirstOrDefault(p => p.Name == PropName);
        object PropValue = Property.GetValue(obj1, null);
        PropValue = PropValue == null ? 0 : PropValue;
        return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Check hidden.</summary>
    ///
    /// <param name="entityName">   Name of the entity.</param>
    /// <param name="brType">       Type of the line break.</param>
    /// <param name="isHiddenGroup">True if is hidden group, false if not.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkHidden(string entityName, string brType, bool isHiddenGroup)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder hiddenBRString = new System.Text.StringBuilder();
        System.Text.StringBuilder chkHiddenGroup = new System.Text.StringBuilder();
        RuleActionContext objRuleAction = new RuleActionContext();
        ConditionContext objCondition = new ConditionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string selectCondition = "";
        string selectval = "";
        string propChangeEvnetdd = "";
        string AssociationName = "";
        string[] rbList = null;
        {
            foreach(BusinessRule objBR in businessRules)
            {
                long ActionTypeId = 6;
                if(isHiddenGroup)
                    ActionTypeId = 12;
                var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                if(objRuleActionList.Count() > 0)
                {
                    System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
                    if(objBR.AssociatedBusinessRuleTypeID == 1 && brType != "OnCreate")
                        continue;
                    else if(objBR.AssociatedBusinessRuleTypeID == 2 && (brType != "OnEdit" && brType != "OnDetails"))
                        continue;
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                if(string.IsNullOrEmpty(chkHidden.ToString()))
                                {
                                    chkHidden.Append("<script type='text/javascript'>$(document).ready(function () {");
                                    fnCondition = "hiddenCondition" + objCon.Id.ToString() + "()";
                                    chkHidden.Append(fnCondition + ";");
                                }
                                string datatype = checkPropType(entityName, objCon.PropertyName);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                else
                                    condValue = objCon.Value;
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                if(datatype == "Association")
                                {
                                    var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    var strText = ".text()";
                                    var strOptionSelected = "$('option:selected', '#" + propertyName + "')";
                                    if(brType != "OnDetails")
                                        chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                    else
                                    {
                                        propertyName = "lbl" + propertyName;
                                        strText = "[0].innerText";
                                        strOptionSelected = "$('#" + propertyName + "')";
                                    }
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "(" + strOptionSelected) + strText + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " " + logicalconnector + " (" + strOptionSelected) + strText + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "(" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " " + logicalconnector + " (" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                    }
                                    if(!rbcheck)
                                    {
                                        if(isHiddenGroup)
                                        {
                                            if(AssociationName != propertyName)
                                            {
                                                propChangeEvnetdd += " $('#" + propertyName + "').change(function() { CONDITION ";
                                                selectval += " var selected" + propertyName + " =  $('option:selected', '#" + propertyName + "') " + ".text().toLowerCase(); ";
                                                AssociationName = propertyName;
                                            }
                                            selectCondition += " selected" + propertyName + operand + " '" + condValue + "'.toLowerCase() ||";
                                        }
                                    }
                                }
                                else
                                {
                                    string propertyName = objCon.PropertyName;
                                    string strVal = ".val()";
                                    string eventName = ".change";
                                    if(propertyAttribute.ToLower() == "currency")
                                        eventName = ".blur";
                                    if(brType != "OnDetails")
                                        chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + eventName + "(function() { " + fnCondition + "; });");
                                    else
                                    {
                                        if(propertyName != "Id")
                                        {
                                            propertyName = "lbl" + propertyName;
                                            strVal = "[0].innerText";
                                        }
                                    }
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = "($('#" + propertyName + "')" + strVal + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " ($('#" + propertyName + "')" + strVal + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        string strLowerCase = ".toLowerCase() ";
                                        string strCondValue = " '" + condValue + "'";
                                        if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                        {
                                            strLowerCase = "";
                                            strCondValue = condValue;
                                        }
                                        if(datatype.ToLower() == "boolean")
                                        {
                                            strVal = ".prop('checked')";
                                            strCondValue = condValue.ToLower();
                                            strLowerCase = "";
                                        }
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(propertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && (brType == "OnEdit" || brType == "OnDetails"))
                                                fnConditionValue = "($('#" + propertyName + "')" + strVal + " " + operand + " '" + objCon.Value + "')";
                                            else if(propertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                fnConditionValue = "('true')";
                                            else
                                            {
                                                if(strCondValue.ToLower().Trim() == "'null'")
                                                    fnConditionValue = "($('#" + propertyName + "')" + strVal + strLowerCase + operand + "''" + strLowerCase + ")";
                                                else
                                                    fnConditionValue = "($('#" + propertyName + "')" + strVal + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                            }
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " ($('#" + propertyName + "')" + strVal + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkHidden.ToString()))
                            {
                                chkHidden.Append(" });");
                                var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    string fn = string.Empty;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        fn += objaa.Id.ToString();
                                        //change for inline association
                                        if(isHiddenGroup)
                                            fnProp += "$('#dvGroup" + objaa.ParameterName.Remove(objaa.ParameterName.IndexOf('|')) + "').css('display', type);";
                                        else
                                            fnProp += "$('#dv" + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
                                    }
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "hiddenProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        string hdnElse = "";
                                        string showHideAllGroup = "";
                                        //if (!isHiddenGroup)
                                        {
                                            hdnElse = "else { " + fnName + "('block'); }";
                                        }
                                        if(isHiddenGroup)
                                        {
                                            showHideAllGroup = "showalldivs();";
                                        }
                                        chkHidden.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + showHideAllGroup + " " + fnName + "('none'); } " + hdnElse + " }");
                                        chkFnHidden.Append("function " + fnName + "(type) { " + fnProp + " }");
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkFnHidden.ToString()))
                            {
                                chkHidden.Append(chkFnHidden.ToString());
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(chkHidden.ToString()))
                    {
                        chkHidden.Append("</script> ");
                        hiddenBRString.Append(chkHidden);
                    }
                }
            }
        }
        //
        if(selectCondition != "" && selectval != "")
        {
            chkHiddenGroup.Append("<script type='text/javascript'> $(document).ready(function () {");
            selectCondition = selectCondition.Remove(selectCondition.Length - 2);
            string finalStr = selectval + "if (!(" + selectCondition + ")){showalldivs();}});";
            chkHiddenGroup.Append(propChangeEvnetdd.Replace("CONDITION", finalStr));
            chkHiddenGroup.Append("}); ");
            chkHiddenGroup.Append("</script> ");
            //chkHiddenGroup = "";
            selectval = "";
            propChangeEvnetdd = "";
            selectCondition = "";
            AssociationName = "";
            hiddenBRString.Append(chkHiddenGroup);
        }
        //
        return hiddenBRString.ToString();
    }
    
    /// <summary>Check set value user interface rule.</summary>
    ///
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="brType">    Type of the line break.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkSetValueUIRule(string entityName, string brType)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder SetValueBRString = new System.Text.StringBuilder();
        ConditionContext objCondition = new ConditionContext();
        RuleActionContext objRuleAction = new RuleActionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string[] rbList = null;
        if(businessRules != null && businessRules.Count > 0)
        {
            foreach(BusinessRule objBR in businessRules)
            {
                var ActionCount = 1;
                long ActionTypeId = 7;
                var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id).ToList();
                if(objRuleActionList.Count() > 0)
                {
                    System.Text.StringBuilder chkSetValue = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkFnSetValue = new System.Text.StringBuilder();
                    if(objBR.AssociatedBusinessRuleTypeID != 6)
                        continue;
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        if(ActionCount > 1)
                            continue;
                        var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                ActionCount++;
                                if(string.IsNullOrEmpty(chkSetValue.ToString()))
                                {
                                    chkSetValue.Append("<script type='text/javascript'>$(document).ready(function () {");
                                    fnCondition = "setvalueUIRule" + objCon.Id.ToString() + "()";
                                    chkSetValue.Append(fnCondition + ";");
                                }
                                string datatype = checkPropType(entityName, objCon.PropertyName);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                else
                                    condValue = objCon.Value;
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                if(datatype == "Association")
                                {
                                    //var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    var propertyName = objCon.PropertyName;
                                    if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
                                    {
                                        var parameterSplit = propertyName.Substring(1, propertyName.Length - 2).Split('.');
                                        string[] inlineAssoList = { };
                                        if(inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                            propertyName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                        else
                                            propertyName = parameterSplit[0];
                                    }
                                    if(fnCondition == null)
                                        continue;
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase()" + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".val().toLowerCase() " + operand + "''.toLowerCase())";
                                            else
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                    }
                                }
                                else
                                {
                                    string eventName = ".on('keyup keypress blur change',";
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + eventName + " function(event) { " + fnCondition + "; });");
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        string strLowerCase = ".toLowerCase() ";
                                        string strCondValue = " '" + condValue + "'";
                                        if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                        {
                                            strLowerCase = "";
                                            strCondValue = condValue;
                                        }
                                        if(datatype.ToLower() == "boolean")
                                        {
                                            fnConditionValue = "($('#" + objCon.PropertyName + "').is(':checked').toString()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                        else if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnEdit")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val() " + operand + " '" + objCon.Value + "')";
                                            else if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                fnConditionValue = "('true')";
                                            else
                                            {
                                                if(strCondValue.ToLower().Trim() == "'null'")
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + "''" + strLowerCase + ")";
                                                else
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                            }
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkSetValue.ToString()))
                            {
                                chkSetValue.Append(" });");
                                long[] ids = objRuleActionList.Select(item => item.Id).ToArray();
                                var objActionArgsList = objActionArgs.ActionArgss.Where(aa => ids.Any(x => x == aa.ActionArgumentsID));
                                //var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    string fn = string.Empty;
                                    bool IsNotElseAssoc = true;
                                    bool IsNotElseValue = true;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        string paramValue = objaa.ParameterValue;
                                        string paramType = checkPropType(entityName, objaa.ParameterName);
                                        if(paramValue.ToLower().Trim().Contains("today"))
                                        {
                                            Type type = Type.GetType("GeneratorBase.MVC.Models." + entityName + ", GeneratorBase.MVC.Models");
                                            var displayformatattributes = type.GetProperty(objaa.ParameterName).CustomAttributes.Where(a => a.NamedArguments.Count() > 0);
                                            if(displayformatattributes.Count() > 0)
                                            {
                                                var formatstring = displayformatattributes.FirstOrDefault().NamedArguments.FirstOrDefault(fd => fd.MemberName == "DataFormatString").TypedValue.Value.ToString();
                                                paramValue = ApplyRule.EvaluateDateForActionInTarget(paramValue, formatstring.Substring((formatstring.LastIndexOf("0")) + 2, formatstring.Length - 4), true);
                                            }
                                            else
                                                paramValue = ApplyRule.EvaluateDateForActionInTarget(paramValue);
                                            fnProp += "$('#" + objaa.ParameterName + "').change();$('#" + objaa.ParameterName + "').val('" + paramValue + "');";
                                        }
                                        else if(paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                                        {
                                            paramValue = paramValue.TrimStart('[').TrimEnd(']').Trim();
                                            paramValue = "$('#" + paramValue + "').val()";
                                            fnProp += "$('#" + objaa.ParameterName + "').change();$('#" + objaa.ParameterName + "').val('" + paramValue + "');";
                                        }
                                        if(paramType == "Association")
                                        {
                                            if(IsNotElseAssoc)
                                            {
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    string[] inlineAssoList = { };
                                                    if(inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                }
                                                if(paramValue.ToLower().Trim() == "null")
                                                {
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                    fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).val() == '') return this; }).attr('selected', 'selected');";
                                                }
                                                else
                                                {
                                                    fnProp += "$('#" + parameterName + "_chosen').find('input').val('" + paramValue + "');";
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                    fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).text() == '" + paramValue + "') return this; }).attr('selected', 'selected');";
                                                }
                                                fnProp += "$('#" + parameterName + "').trigger('click.chosen');";
                                                fnProp += "$('#" + parameterName + "').trigger('chosen:updated');";
                                                fnProp += "$('#" + parameterName + "').trigger('change');";
                                                IsNotElseAssoc = false;
                                            }
                                            else
                                            {
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    string[] inlineAssoList = { };
                                                    if(inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                }
                                                fnProp += "}else {";
                                                if(paramValue.ToLower().Trim() == "null")
                                                {
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                    fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).val() == '') return this; }).attr('selected', 'selected');";
                                                }
                                                else
                                                {
                                                    fnProp += "$('#" + parameterName + "_chosen').find('input').val('" + paramValue + "');";
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                    fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).text() == '" + paramValue + "') return this; }).attr('selected', 'selected');";
                                                }
                                                fnProp += "$('#" + parameterName + "').trigger('click.chosen');";
                                                fnProp += "$('#" + parameterName + "').trigger('chosen:updated');";
                                                fnProp += "$('#" + parameterName + "').trigger('change');";
                                            }
                                        }
                                        else
                                        {
                                            if(IsNotElseValue)
                                            {
                                                if(paramValue.ToLower().Trim() == "null")
                                                    paramValue = "";
                                                fnProp += "$('#" + objaa.ParameterName + "').change();  $('#" + objaa.ParameterName + "').val('" + paramValue + "');";
                                                IsNotElseValue = false;
                                            }
                                            else
                                            {
                                                if(paramValue.ToLower().Trim() == "null")
                                                    paramValue = "";
                                                fnProp += "}else {";
                                                fnProp += " $('#" + objaa.ParameterName + "').change(); $('#" + objaa.ParameterName + "').val('" + paramValue + "');";
                                            }
                                        }
                                        fn += objaa.Id.ToString();
                                    }
                                    //if (fnCondition == "")
                                    //    continue;
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "setvalueUIRuleProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        chkSetValue.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + fnProp + " } }");
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkFnSetValue.ToString()))
                            {
                                chkSetValue.Append(chkFnSetValue.ToString());
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(chkSetValue.ToString()))
                    {
                        chkSetValue.Append("</script> ");
                        SetValueBRString.Append(chkSetValue);
                    }
                }
            }
        }
        return SetValueBRString.ToString();
    }
    
    /// <summary>Check operator.</summary>
    ///
    /// <param name="condition">The condition.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkOperator(string condition)
    {
        string opr = string.Empty;
        switch(condition)
        {
        case "=":
            opr = "==";
            break;
        case "Contains":
            opr = "Contains";
            break;
        default:
            opr = condition;
            break;
        }
        return opr;
    }
    
    /// <summary>Gets property attribute.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="PropName">  Name of the property.</param>
    ///
    /// <returns>The property attribute.</returns>
    
    public string getPropertyAttribute(string EntityName, string PropName)
    {
        if(PropName == "Id")
            return "long";
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(PropName.StartsWith("[") && PropName.EndsWith("]"))
        {
            PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                    }
                }
            }
        }
        string DataTypeAttribute = PropInfo.DataTypeAttribute;
        if(AssociationInfo != null)
        {
            DataTypeAttribute = "Association";
        }
        return DataTypeAttribute;
    }
    
    /// <summary>Check property type.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="PropName">  Name of the property.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkPropType(string EntityName, string PropName)
    {
        if(PropName == "Id")
            return "long";
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(PropName.StartsWith("[") && PropName.EndsWith("]"))
        {
            PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                    }
                }
            }
        }
        string DataType = PropInfo.DataType;
        if(AssociationInfo != null)
        {
            DataType = "Association";
        }
        return DataType;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result with the given data
    /// as its content.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    
    [HttpPost]
    public JsonResult Check1MThresholdValue(T_FacetedSearch t_facetedsearch)
    {
        Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
        return Json(msgThreshold, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>code for verb action security.</summary>
    ///
    /// <returns>An array of string.</returns>
    
    public string[] getVerbsName()
    {
        string[] Verbsarr = new string[] { "BulkUpdate", "BulkDelete", "ImportExcel", "ExportExcel" };
        return Verbsarr;
    }
    
    /// <summary>code for list of groups.</summary>
    ///
    /// <returns>An array of string[].</returns>
    
    public string[][] getGroupsName()
    {
        string[][] groupsarr = new string[][] { };
        return groupsarr;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetCalculationValues(T_FacetedSearch t_facetedsearch)
    {
        t_facetedsearch.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_facetedsearch.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    //get Templates
    // select templates
    
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="IgnoreEditable"> (Optional) The ignore editable.</param>
    /// <param name="source">         (Optional) Source for the.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetDerivedDetails(T_FacetedSearch t_facetedsearch, string IgnoreEditable = null, string source = null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_facetedsearch.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">           The host.</param>
    /// <param name="value">          The value.</param>
    /// <param name="t_facetedsearch">The facetedsearch.</param>
    /// <param name="IgnoreEditable"> The ignore editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetDerivedDetailsInline(string host, string value, T_FacetedSearch t_facetedsearch, string IgnoreEditable)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets inline associations of entity.</summary>
    ///
    /// <returns>A response stream to send to the getInlineAssociationsOfEntity View.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult getInlineAssociationsOfEntity()
    {
        List<string> list = new List<string> { };
        return Json(list, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Downloads the given FileName.</summary>
    ///
    /// <param name="FileName">Filename of the file.</param>
    ///
    /// <returns>A response stream to send to the Download View.</returns>
    
    public ActionResult Download(string FileName)
    {
        string filename = FileName;
        string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
        byte[] filedata = System.IO.File.ReadAllBytes(filepath);
        string contentType = MimeMapping.GetMimeMapping(filepath);
        var cd = new System.Net.Mime.ContentDisposition
        {
            FileName = filename,
            Inline = true,
        };
        return File(filedata, "application/force-download", Path.GetFileName(FileName));
    }
}
}

