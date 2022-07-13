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
using System.Collections;
using System.Reflection.Emit;
using System.Reflection;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling custom reports.</summary>
public partial class CustomReportsController : BaseController
{
    /// <summary>Gets generic data for reports.</summary>
    ///
    /// <param name="context">The context.</param>
    /// <param name="_type">  The type.</param>
    ///
    /// <returns>The generic data for reports.</returns>
    
    public DbSet GetGenericDataForReports(ApplicationContext context, Type _type)
    {
        return context.Set(_type);
    }
    
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="customreports">The customreports.</param>
    
    private void LoadViewDataAfterOnEdit(CustomReports customreports)
    {
        ViewBag.CreatedByUserID = new SelectList(UserContext.Users, "Id", "UserName", customreports.CreatedByUserID);
    }
    
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="customreports">The customreports.</param>
    
    private void LoadViewDataBeforeOnEdit(CustomReports customreports)
    {
        ViewBag.CreatedByUserID = new SelectList(UserContext.Users, "Id", "UserName", customreports.CreatedByUserID);
    }
    
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="customreports">The customreports.</param>
    
    private void LoadViewDataAfterOnCreate(CustomReports customreports)
    {
        ViewBag.CreatedByUserID = new SelectList(UserContext.Users, "Id", "UserName", customreports.CreatedByUserID);
    }
    
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        //Add object for UserDropDown
        Dictionary<string, string> CreatedByUserDict = new Dictionary<string, string>();
        ViewBag.CreatedByUserID = new SelectList(CreatedByUserDict, "Id", "UserName");
        //
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstCustomReports">The list custom reports.</param>
    /// <param name="searchString">    The search string.</param>
    /// <param name="IsDeepSearch">    The is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<CustomReports> searchRecords(IQueryable<CustomReports> lstCustomReports, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstCustomReports = lstCustomReports.Where(s => (!String.IsNullOrEmpty(s.ReportName) && s.ReportName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ReportType) && s.ReportType.ToUpper().Contains(searchString)) || (s.createdbyuser != null && (s.createdbyuser.UserName.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ResultProperty) && s.ResultProperty.ToUpper().Contains(searchString)) || (s.ColumnOrder != null && SqlFunctions.StringConvert((double)s.ColumnOrder).Contains(searchString)) || (!String.IsNullOrEmpty(s.OrderBy) && s.OrderBy.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CrossTabRow) && s.CrossTabRow.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CrossTabColumn) && s.CrossTabColumn.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.AggregateEntity) && s.AggregateEntity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.AggregateProperty) && s.AggregateProperty.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.AggregateFunction) && s.AggregateFunction.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FilterProperty) && s.FilterProperty.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FilterCondition) && s.FilterCondition.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FilterType) && s.FilterType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FilterValue) && s.FilterValue.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SelectValueFromList) && s.SelectValueFromList.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SelectProperty) && s.SelectProperty.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.RelatedEntity) && s.RelatedEntity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ForeignKeyEntity) && s.ForeignKeyEntity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.RelationName) && s.RelationName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityValues) && s.EntityValues.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CrossTabPropertyValues) && s.CrossTabPropertyValues.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.QueryConditionValues) && s.QueryConditionValues.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.RelationsValues) && s.RelationsValues.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.OtherValues) && s.OtherValues.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstCustomReports = lstCustomReports.Where(s => (!String.IsNullOrEmpty(s.ReportName) && s.ReportName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ReportType) && s.ReportType.ToUpper().Contains(searchString)) || (s.createdbyuser != null && (s.createdbyuser.UserName.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        bool boolvalue;
        if(Boolean.TryParse(searchString, out boolvalue))
            lstCustomReports = lstCustomReports.Union(db.CustomReportss.Where(s => (s.GroupBy == boolvalue)));
        DateTime datevalue;
        if(DateTime.TryParse(searchString, out datevalue))
            lstCustomReports = lstCustomReports.Union(db.CustomReportss.Where(s => (s.CreatedOn == datevalue)));
        return lstCustomReports;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstCustomReports">The list custom reports.</param>
    /// <param name="sortBy">          Describes who sort this object.</param>
    /// <param name="isAsc">           The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<CustomReports> sortRecords(IQueryable<CustomReports> lstCustomReports, string sortBy, string isAsc)
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
        if(sortBy == "CreatedByUserID")
            return isAsc.ToLower() == "asc" ? lstCustomReports.OrderBy(p => p.createdbyuser.UserName) : lstCustomReports.OrderByDescending(p => p.createdbyuser.UserName);
        ParameterExpression paramExpression = Expression.Parameter(typeof(CustomReports), "customreports");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<CustomReports>)lstCustomReports.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstCustomReports.ElementType, lambda.Body.Type },
                       lstCustomReports.Expression,
                       lambda));
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
        var PrpertyType = GetDataTypeOfProperty("CustomReports", property);
        var dataType = PrpertyType[0];
        property = PrpertyType[1];
        if(value.StartsWith("[") && value.EndsWith("]"))
        {
            var ValueType = GetDataTypeOfProperty("CustomReports", value, true);
            if(ValueType != null && ValueType[0] == "dynamic")
            {
                dataType = ValueType[0];
                value = ValueType[1];
            }
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
            DateTime val = Convert.ToDateTime(value);
            expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
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
                    else
                    {
                        var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == targetProperties[0]);
                        PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        propertyName = targetProperties[1];
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
        var _Obj = db1.CustomReportss.FirstOrDefault(p => p.Id == idvalue);
        return _Obj == null ? "" : _Obj.DisplayValue;
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
        var _Obj = db1.CustomReportss.Find(Convert.ToInt64(id));
        return _Obj;
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
        var _Obj = db1.CustomReportss.Find(Convert.ToInt64(id));
        return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<CustomReports>(_Obj, "CustomReports", new string[] { "" }); ;
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
        IQueryable<CustomReports> list = db.CustomReportss;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.CustomReportss;
                string lambda = "";
                foreach(var asso in AssoNameWithParent.Split("?".ToCharArray()))
                {
                    lambda += asso + "=" + AssociationID + " OR ";
                }
                lambda = lambda.TrimEnd(" OR ".ToCharArray());
                query = query.Where(lambda);
                //Type[] exprArgTypes = { query.ElementType };
                // string propToWhere = AssoNameWithParent;
                // ParameterExpression p = Expression.Parameter(typeof(CustomReports), "p");
                // MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                // LambdaExpression lambda = Expression.Lambda<Func<CustomReports, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                // MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                // IQueryable q = query.Provider.CreateQuery(methodCall);
                //list = ((IQueryable<CustomReports>)q);
                list = ((IQueryable<CustomReports>)query);
            }
            else if(AssoID == 0)
            {
                IQueryable query = db.CustomReportss;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(CustomReports), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<CustomReports, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<CustomReports>)q);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<CustomReports>(User, list, "CustomReports", caller);
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(q => q.DisplayValue).Take(10).ToList()
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
        IQueryable<CustomReports> list = db.CustomReportss;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.CustomReportss;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(CustomReports), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<CustomReports, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<CustomReports>)q);
            }
        }
        var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                   select new { Id = x.Id, Name = x.DisplayValue };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="propNameBR">The property name line break.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValueForBR action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
    {
        IQueryable<CustomReports> list = db.CustomReportss;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            //added new code (Remove old code if everything works)
            var result = list.Select("new(Id," + propNameBR + " as value)");
            return Json(result, JsonRequestBehavior.AllowGet);
            //
            ParameterExpression param = Expression.Parameter(typeof(CustomReports), "d");
            //bulid expression tree:data.Field1
            var Property = typeof(CustomReports).GetProperty(propNameBR);
            Expression selector = Expression.Property(param, Property);
            Expression pred = Expression.Lambda(selector, param);
            //bulid expression tree:Select(d=>d.Field1)
            var targetType = Property.PropertyType;
            if(targetType.GetGenericArguments().Count() > 0)
            {
                if(targetType.GetGenericArguments()[0].Name == "DateTime")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(DateTime?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<DateTime?> query = list.Provider.CreateQuery<DateTime?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
            }
            if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if(targetType.GetGenericArguments()[0].Name == "Decimal")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(decimal?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<decimal?> query = list.Provider.CreateQuery<decimal?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Boolean")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(bool?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<bool?> query = list.Provider.CreateQuery<bool?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Int32")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(Int32?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<Int32?> query = list.Provider.CreateQuery<Int32?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Int64")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(Int64?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<Int64?> query = list.Provider.CreateQuery<Int64?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                if(targetType.GetGenericArguments()[0].Name == "Double")
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                      new Type[] { typeof(CustomReports), typeof(double?) }, Expression.Constant(list.AsQueryable()), pred);
                    IQueryable<double?> query = list.Provider.CreateQuery<double?>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Expression expr = Expression.Call(typeof(Queryable), "Select",
                                                  new Type[] { typeof(CustomReports), typeof(string) }, Expression.Constant(list.AsQueryable()), pred);
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
        IQueryable<CustomReports> list = db.CustomReportss;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            IQueryable query = db.CustomReportss;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = AssoNameWithParent;
            var AssoIDs = AssociationID.Split(',').ToList();
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Parameter(typeof(CustomReports), "p"));
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
            list = ((IQueryable<CustomReports>)q);
        }
        if(key != null && key.Length > 0)
        {
            var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.Take(10).ToList()
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
    
    public bool ValidateModel(CustomReports validate)
    {
        return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetReadOnlyProperties(CustomReports OModel)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "CustomReports").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "CustomReports");
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
                        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "CustomReports");
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
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetMandatoryProperties(CustomReports OModel, string ruleType)
    {
        Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "CustomReports").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                if(ruleType == "OnCreate")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                else if(ruleType == "OnEdit")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "CustomReports");
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
                        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "CustomReports");
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
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetLockBusinessRules(CustomReports OModel)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //string RulesApplied = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "CustomReports").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "CustomReports");
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
    
    private List<string> CheckMandatoryProperties(CustomReports OModel)
    {
        List<string> result = new List<string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "CustomReports").ToList();
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "CustomReports");
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
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "CustomReports").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
        ApplicationContext db1 = new ApplicationContext(User);
        if(string.IsNullOrEmpty(displayvalue)) return 0;
        var _Obj = db1.CustomReportss.FirstOrDefault(p => p.DisplayValue == displayvalue);
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
        IQueryable query = db1.CustomReportss;
        Type[] exprArgTypes = { query.ElementType };
        string propToWhere = PropName;
        ParameterExpression p = Expression.Parameter(typeof(CustomReports), "p");
        MemberExpression member = Expression.PropertyOrField(p, propToWhere);
        LambdaExpression lambda = null;
        if(PropValue.ToLower().Trim() != "null")
            lambda = Expression.Lambda<Func<CustomReports, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
        else
            lambda = Expression.Lambda<Func<CustomReports, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
        IQueryable q = query.Provider.CreateQuery(methodCall);
        long outValue;
        var list1 = ((IQueryable<CustomReports>)q);
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
        var obj1 = db1.CustomReportss.Find(Id);
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
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="brType">    Type of the line break.</param>
    ///
    /// <returns>A string.</returns>
    
    public string checkHidden(string entityName, string brType)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
        System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
        RuleActionContext objRuleAction = new RuleActionContext();
        ConditionContext objCondition = new ConditionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string[] rbList = null;
        if(businessRules != null && businessRules.Count > 0)
        {
            foreach(BusinessRule objBR in businessRules)
            {
                long ActionTypeId = 6;
                var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                if(objRuleActionList.Count() > 0)
                {
                    if(objBR.AssociatedBusinessRuleTypeID == 1 && brType != "OnCreate")
                        continue;
                    else if(objBR.AssociatedBusinessRuleTypeID == 2 && brType != "OnEdit")
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
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                else
                                    condValue = objCon.Value;
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                chkHidden.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                if(datatype == "Association")
                                {
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                    }
                                }
                                else
                                {
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnEdit")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val() " + operand + " '" + objCon.Value + "')";
                                            else if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                fnConditionValue = "('true')";
                                            else
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                        }
                                        else
                                        {
                                            fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
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
                                        fnProp += "$('#dv" + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
                                    }
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "hiddenProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        chkHidden.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) {" + fnName + "('none'); } else { " + fnName + "('block');  } }");
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
                    }
                }
            }
        }
        return chkHidden.ToString();
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
    
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="customreports">The customreports.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CheckBeforeDelete(CustomReports customreports)
    {
        var result = true;
        // Write your logic here
        if(!result)
        {
            var AlertMsg = "Validation Alert - Before Delete !! Information not deleted.";
            ModelState.AddModelError("CustomError", AlertMsg);
            ViewBag.ApplicationError = AlertMsg;
        }
        return result;
    }
    
    /// <summary>code for verb action security.</summary>
    ///
    /// <returns>An array of string.</returns>
    
    public string[] getVerbsName()
    {
        string[] Verbsarr = new string[] { "BulkDelete" };
        return Verbsarr;
    }
    
    /// <summary>Entity List.</summary>
    ///
    /// <returns>A Dictionary&lt;string,string&gt;</returns>
    
    public Dictionary<string, string> EnityDictionary()
    {
        Dictionary<string, string> entList = new Dictionary<string, string>();
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name != "MultiTenantExtraAccess" && p.Name != "FileDocument" && p.Name != "MultiTenantLoginSelected" && p.Name != "CustomReports" && p.Name != "CustomReport" && p.Name != "ApiToken");
        foreach(var item in entitys)
        {
            var lstitem = item.Associations;
            var getDateType = item.Properties;
            int nonstringcount = 0;
            foreach(var proItem in getDateType)
                if(proItem.DataType.ToUpper() != "STRING")
                    nonstringcount = nonstringcount + 1;
            entList.Add(item.Name, item.DisplayName);
        }
        return entList;
    }
    
    /// <summary>Property Dropown.</summary>
    ///
    /// <param name="UserId">Identifier for the user.</param>
    /// <param name="db">    The database.</param>
    ///
    /// <returns>True if already registred, false if not.</returns>
    
    public bool IsAlreadyRegistred(string UserId, ApplicationContext db)
    {
        var result = false;
        var obj = db.CustomReportss.FirstOrDefault(p => p.CreatedByUserID == UserId);
        if(obj != null)
            result = true;
        return result;
    }
    
    /// <summary>Automatic registration.</summary>
    ///
    /// <param name="userid">The userid.</param>
    ///
    /// <returns>A response stream to send to the AutoRegistration View.</returns>
    
    [AllowAnonymous]
    public ActionResult AutoRegistration(string userid)
    {
        CustomReports newregistration = new CustomReports();
        newregistration.CreatedByUserID = userid;
        ApplicationContext dbregistration = new ApplicationContext(new SystemUser());
        //set required properties
        var properties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "CustomReports").Properties;
        foreach(var prop in properties)
        {
            if(!prop.IsRequired) continue;
            var targetProp = newregistration.GetType().GetProperty(prop.Name);
            if(prop.DataType.ToUpper() == "STRING")
            {
                if(targetProp != null)
                    targetProp.SetValue(newregistration, User.Name);
            }
            else
                targetProp.SetValue(newregistration, 0);
        }
        dbregistration.CustomReportss.Add(newregistration);
        dbregistration.SaveChanges();
        return RedirectToAction("Edit", new { id = newregistration.Id });
    }
    
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="customreports">The customreports.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetCalculationValues(CustomReports customreports)
    {
        customreports.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (customreports.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>UserLogin Dropown.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueUserLogin action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueUserLogin(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var list = UserContext.Users;
        var data = from x in list.OrderBy(q => q.UserName).ToList()
                   select new { Id = x.Id, Name = x.UserName };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <returns>A JSON response stream to send to the GetDisplayValueByUserName action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetDisplayValueByUserName()
    {
        ApplicationDbContext usercontext = new ApplicationDbContext();
        var userid = usercontext.Users.FirstOrDefault(p => p.UserName == User.Name).Id;
        IQueryable<CustomReports> list = db.CustomReportss;
        var data = (from x in list.Where(p => p.CreatedByUserID == userid).ToList()
                    select new { Id = x.Id, Name = x.DisplayValue }).FirstOrDefault();
        return Json(data, JsonRequestBehavior.AllowGet);
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
    
    /// <summary>report section.</summary>
    ///
    /// <param name="entityName">      Name of the entity.</param>
    /// <param name="entityDispayName">Name of the entity dispay.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllProperties action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllProperties(string entityName, string entityDispayName)
    {
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == entityName).ToList();
        var data = from x in entitys.FirstOrDefault().Properties
                   select new { Key = entityName + "." + x.Name, Value = entityDispayName + "." + x.DisplayName };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllAssotions action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllAssotions(string entityName)
    {
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == entityName).ToList();
        var data = from x in entitys.FirstOrDefault().Associations
                   select new { SourceEntity = entityName, AssocName = x.Name, TargetEntity = x.Target, TargetEntityDisp = getEntityDisplayName(x.Target) };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets property display name reports.</summary>
    ///
    /// <param name="Property">  The property.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>The property display name reports.</returns>
    
    public string getPropertyDisplayNameReports(string Property, string EntityName)
    {
        string EntName = "";
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            EntName = EntityReflector.DisplayName;
            prop = EntityReflector.Properties.FirstOrDefault(q => q.Name == Property).DisplayName;
        }
        catch(Exception ex)
        {
            EntName = EntityName;
            prop = Property;
        }
        return prop;
    }
    
    /// <summary>Gets property display name.</summary>
    ///
    /// <param name="Property">  The property.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>The property display name.</returns>
    
    public string getPropertyDisplayName(string Property, string EntityName)
    {
        string EntName = "";
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            EntName = EntityReflector.DisplayName;
            prop = EntityReflector.Properties.FirstOrDefault(q => q.Name == Property).DisplayName;
        }
        catch(Exception ex)
        {
            EntName = EntityName;
            prop = Property;
        }
        return EntName + " " + prop;
    }
    
    /// <summary>Gets entity display name.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>The entity display name.</returns>
    
    public string getEntityDisplayName(string EntityName)
    {
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            prop = EntityReflector.DisplayName;
        }
        catch(Exception ex)
        {
            prop = EntityName;
        }
        return prop;
    }
    
    /// <summary>Reports result.</summary>
    ///
    /// <param name="entityVal">          The entity value.</param>
    /// <param name="properties">         The properties.</param>
    /// <param name="FilterCondition">    The filter condition.</param>
    /// <param name="relationsValues">    The relations values.</param>
    /// <param name="reportsType">        Type of the reports.</param>
    /// <param name="rowProp">            The row property.</param>
    /// <param name="ColProp">            The col property.</param>
    /// <param name="CrossTabPropertyVal">The cross tab property value.</param>
    /// <param name="ReportName">         Name of the report.</param>
    ///
    /// <returns>A response stream to send to the ReportsResult View.</returns>
    
    public ActionResult ReportsResult(string entityVal, string properties, string FilterCondition, string relationsValues, string reportsType, string rowProp, string ColProp, string CrossTabPropertyVal, string ReportName)
    {
        ViewBag.ReportName = ReportName;
        List<object> list = null;
        if(reportsType == "Simple")
        {
            ViewBag.ReportType = reportsType;
            list = SimpleReports(entityVal, properties, FilterCondition, relationsValues);
            return PartialView("ReportsResultPartial", list);
        }
        if(reportsType == "CrossTab")
        {
            ViewBag.ReportType = reportsType;
            list = CrossTabReports(entityVal, properties, FilterCondition, relationsValues, rowProp, ColProp, CrossTabPropertyVal, ReportName);
            return PartialView("ReportsResultPartial", list);
        }
        return PartialView("ReportsResultPartial", list);
    }
    
    /// <summary>Cross tab reports.</summary>
    ///
    /// <param name="entityVal">          The entity value.</param>
    /// <param name="properties">         The properties.</param>
    /// <param name="FilterCondition">    The filter condition.</param>
    /// <param name="relationsValues">    The relations values.</param>
    /// <param name="rowProp">            The row property.</param>
    /// <param name="ColProp">            The col property.</param>
    /// <param name="CrossTabPropertyVal">The cross tab property value.</param>
    /// <param name="ReportName">         Name of the report.</param>
    ///
    /// <returns>A List&lt;object&gt;</returns>
    
    public List<object> CrossTabReports(string entityVal, string properties, string FilterCondition, string relationsValues, string rowProp, string ColProp, string CrossTabPropertyVal, string ReportName)
    {
        List<object> list = null;
        StringBuilder whereCondition = null;
        IQueryable<dynamic> selectQuery = null;
        var entityValArray = entityVal.Split(',');
        var relArry = relationsValues.Split(',');
        var crossTabCol = CrossTabPropertyVal.Split(',');
        List<IEnumerable<dynamic>> qureyArr = new List<IEnumerable<dynamic>>();
        if(relArry != null && !string.IsNullOrEmpty(relationsValues))
        {
            bool hasrel = false;
            foreach(var item in entityValArray)
            {
                foreach(var relItem in relArry)
                {
                    var relitm = relItem.Split('-');
                    if(relitm[0] + "s" == item + "s")
                    {
                        IEnumerable<dynamic> qurey = typeof(ApplicationContext).GetProperty(relitm[0] + "s").GetValue(db, null) as IEnumerable<dynamic>;
                        qureyArr.Add(qurey);
                        hasrel = true;
                        break;
                    }
                }
                if(hasrel)
                    break;
            }
        }
        else
        {
            foreach(var item in entityValArray)
            {
                IEnumerable<dynamic> qurey = typeof(ApplicationContext).GetProperty(item + "s").GetValue(db, null) as IEnumerable<dynamic>;
                qureyArr.Add(qurey);
                break;
            }
        }
        bool first = true;
        foreach(var qurey in qureyArr)
        {
            if(first)
            {
                selectQuery = (IQueryable<dynamic>)qurey;
            }
            else
            {
                //selectQuery += selectQuery.Join();
                //joinStff
            }
        }
        if(!string.IsNullOrEmpty(FilterCondition))
        {
            whereCondition = GetFilterConditions(FilterCondition, entityVal, relationsValues);
            if(whereCondition.ToString().StartsWith(" AND "))
            {
                whereCondition = whereCondition.Remove(0, 4);
            }
            if(whereCondition.ToString().StartsWith(" OR "))
            {
                whereCondition = whereCondition.Remove(0, 3);
            }
        }
        string propsSel = "";
        int ZmatCount = 0;
        foreach(var Col in crossTabCol)
        {
            var colitm = Col.Split('-');
            var crossent = colitm[0];
            var crossProps = colitm[1];
            var crossFun = colitm[2];
            var crossFunCount = crossFun.Split(';');
            foreach(var item in crossFunCount)
            {
                ZmatCount++;
            }
            propsSel += crossProps + ",";
        }
        var fldSelect = propsSel + rowProp + "," + ColProp;
        string fldStr = "";
        var fldArr = fldSelect.Split(',');
        foreach(var flds in fldArr)
        {
            var fldEnt = flds.Split('.')[0];
            var fld = flds.Split('.')[1];
            if(relArry != null && !string.IsNullOrEmpty(relationsValues))
            {
                foreach(var rel in relArry)
                {
                    var AssoStr = rel.Split('-');
                    var MainEnt = AssoStr[0];
                    var AssoEnt = AssoStr[1];
                    var AssoName = AssoStr[2];
                    if(fld.Substring(fld.Length - 2) == "ID" && fld.Substring(0, fld.Length - 2) == AssoName)
                    {
                        string dispName = getPropertyDisplayNameReports(fld, MainEnt);
                        string colname = GetInternalName(dispName);
                        fldStr += AssoName.ToLower() + ".DisplayValue" + " as " + colname + " ,";
                        break;
                    }
                    else if(fld.Substring(fld.Length - 2) != "ID")
                    {
                        string dispName = getPropertyDisplayNameReports(fld, fldEnt);
                        string colname = GetInternalName(dispName);
                        fldStr += fld + " as " + colname + ",";
                        break;
                    }
                }
            }
            else
            {
                string dispName = getPropertyDisplayNameReports(fld, fldEnt);
                string colname = GetInternalName(dispName);
                fldStr += fld + " as " + colname + ",";
                break;
            }
        }
        if(whereCondition != null)
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.Where(whereCondition.ToString()).Select(" new (" + fldStr.TrimEnd(',') + ")");
        }
        else
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.Select(" new (" + fldStr.TrimEnd(',') + ")");
        }
        list = (from a in selectQuery select a).ToList() as List<object>;
        DataTable dataTable = new DataTable("CrossTabReport");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        dataTable = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json);
        string[,] zMatrix = new string[ZmatCount, 2];
        int ZmatrixCount = 0;
        foreach(var Col in crossTabCol)
        {
            var colitm = Col.Split('-');
            var crossent = colitm[0];
            var crossProps = colitm[1];
            var crossFun = colitm[2];
            var crossFunCount = crossFun.Split(';');
            foreach(var item in crossFunCount)
            {
                string dispName = getPropertyDisplayNameReports(crossProps.Split('.')[1], crossProps.Split('.')[0]);
                string colname = GetInternalName(dispName);
                zMatrix[ZmatrixCount, 0] = colname;
                zMatrix[ZmatrixCount, 1] = item;
                ZmatrixCount++;
            }
        }
        string ColField = "";
        string RowField = "";
        ColField = GetRowColField(ColProp, relationsValues);
        RowField = GetRowColField(rowProp, relationsValues);
        Pivot pivot = new Pivot(dataTable);
        StringBuilder tableBuilder = new StringBuilder();
        pivot.PivotTable(ColField, RowField, zMatrix, tableBuilder, ReportName);
        ViewBag.TabResult = tableBuilder.ToString();
        //Session["ResultTable"] = htmlTable;
        return list;
    }
    
    /// <summary>Simple reports.</summary>
    ///
    /// <param name="entityVal">      The entity value.</param>
    /// <param name="properties">     The properties.</param>
    /// <param name="FilterCondition">The filter condition.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>A List&lt;object&gt;</returns>
    
    public List<object> SimpleReports(string entityVal, string properties, string FilterCondition, string relationsValues)
    {
        // string entityVal, string properties, string FilterCondition, string relationsValues
        StringBuilder whereCondition = null;
        string OrderByCond = string.Empty;
        string GroupByCond = string.Empty;
        string Aggstr = string.Empty;
        IQueryable<dynamic> selectQuery = null;
        var entityValArray = entityVal.Split(',');
        var relArry = relationsValues.Split(',');
        ViewBag.Relations = relationsValues;
        string propsArg = getPrpoertyForSelect(properties);
        List<IEnumerable<dynamic>> qureyArr = new List<IEnumerable<dynamic>>();
        if(relArry != null && !string.IsNullOrEmpty(relationsValues))
        {
            bool hasrel = false;
            foreach(var item in entityValArray)
            {
                foreach(var relItem in relArry)
                {
                    var relitm = relItem.Split('-');
                    if(relitm[0] + "s" == item + "s")
                    {
                        IEnumerable<dynamic> qurey = typeof(ApplicationContext).GetProperty(relitm[0] + "s").GetValue(db, null) as IEnumerable<dynamic>;
                        qureyArr.Add(qurey);
                        hasrel = true;
                        break;
                    }
                }
                if(hasrel)
                    break;
            }
        }
        else
        {
            foreach(var item in entityValArray)
            {
                IEnumerable<dynamic> qurey = typeof(ApplicationContext).GetProperty(item + "s").GetValue(db, null) as IEnumerable<dynamic>;
                qureyArr.Add(qurey);
                break;
            }
        }
        bool first = true;
        foreach(var qurey in qureyArr)
        {
            if(first)
            {
                selectQuery = (IQueryable<dynamic>)qurey;
            }
            else
            {
                //selectQuery += selectQuery.Join();
                //joinStff
            }
        }
        //
        if(!string.IsNullOrEmpty(properties))
        {
            OrderByCond = GetOrderByStr(properties, relationsValues);
        }
        if(!string.IsNullOrEmpty(properties))
        {
            GroupByCond = GetGroupByStr(properties, relationsValues);
        }
        if(!string.IsNullOrEmpty(FilterCondition))
        {
            whereCondition = GetFilterConditions(FilterCondition, entityVal, relationsValues);
            if(whereCondition.ToString().StartsWith(" AND "))
            {
                whereCondition = whereCondition.Remove(0, 4);
            }
            if(whereCondition.ToString().StartsWith(" OR "))
            {
                whereCondition = whereCondition.Remove(0, 3);
            }
        }
        if(whereCondition != null && string.IsNullOrEmpty(GroupByCond) && string.IsNullOrEmpty(OrderByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition.ToString());//.Select("new(" + propsArg + " )");
        }
        if(whereCondition != null && !string.IsNullOrEmpty(GroupByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition.ToString()).GroupBy("new(" + GroupByCond + " )", "it");
        }
        else if(whereCondition != null && string.IsNullOrEmpty(GroupByCond) && !string.IsNullOrEmpty(OrderByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whereCondition.ToString()).OrderBy(OrderByCond);
        }
        else if(whereCondition == null && !string.IsNullOrEmpty(GroupByCond) && !string.IsNullOrEmpty(OrderByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().GroupBy("new(" + GroupByCond + " )", "it");
        }
        else if(whereCondition == null && !string.IsNullOrEmpty(GroupByCond) && string.IsNullOrEmpty(OrderByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().GroupBy("new(" + GroupByCond + " )", "it");
        }
        else if(whereCondition == null && string.IsNullOrEmpty(GroupByCond) && !string.IsNullOrEmpty(OrderByCond))
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().OrderBy(OrderByCond);
        }
        else
        {
            selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable();
        }
        Aggstr = GetAggExp(properties, relationsValues);
        if(Aggstr != "")
        {
            string AggrationResult = GetAggergationResult(Aggstr, selectQuery);
            ViewBag.AggrationResult = AggrationResult;
        }
        //
        var list = (from a in selectQuery select a).ToList() as List<object>;
        if(!string.IsNullOrEmpty(GroupByCond))
        {
            ViewBag.GroupResult = list;
        }
        object[] props = getProertyHeader(properties); //list.FirstOrDefault().GetType().GetProperties();
        ViewBag.columns = getProertyHeader(properties);
        ViewBag.RowValue = PropertyValues(properties, relationsValues);
        return list;
    }
    
    /// <summary>Gets row col field.</summary>
    ///
    /// <param name="FieldName">      Name of the field.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The row col field.</returns>
    
    public string GetRowColField(string FieldName, string relationsValues)
    {
        var relArry = relationsValues.Split(',');
        string fld = "";
        if(relationsValues != "")
        {
            foreach(var rel in relArry)
            {
                var crossEnt = FieldName.Split('.')[0];
                var crossCol = FieldName.Split('.')[1];
                var AssoStr = rel.Split('-');
                var MainEnt = AssoStr[0];
                var AssoEnt = AssoStr[1];
                var AssoName = AssoStr[2];
                if(crossCol.Substring(crossCol.Length - 2) == "ID" && crossCol.Substring(0, crossCol.Length - 2) == AssoName)
                {
                    string dispName = getEntityDisplayName(AssoEnt);
                    string colname = GetInternalName(dispName);
                    fld = colname;
                    break;
                }
                else
                {
                    string dispName = getPropertyDisplayNameReports(crossCol, crossEnt);
                    string colname = GetInternalName(dispName);
                    fld = colname;
                    break;
                }
            }
        }
        else
        {
            var crossEnt = FieldName.Split('.')[0];
            var crossCol = FieldName.Split('.')[1];
            string dispName = getPropertyDisplayNameReports(crossCol, crossEnt);
            string colname = GetInternalName(dispName);
            fld = colname;
        }
        return fld;
    }
    
    /// <summary>Gets internal name.</summary>
    ///
    /// <param name="DisplayName">Name of the display.</param>
    ///
    /// <returns>The internal name.</returns>
    
    public string GetInternalName(string DisplayName)
    {
        var result = DisplayName;
        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[;\\\\/:*?#$%!~^,@&()\"<>|&'\\[\\]]");
        var sourcenameOutput = r.Replace(result, " ");
        sourcenameOutput = sourcenameOutput.Replace(" ", "");
        sourcenameOutput = sourcenameOutput.Replace(".", "");
        sourcenameOutput = sourcenameOutput.Replace("+", "");
        sourcenameOutput = sourcenameOutput.Replace("-", "");
        return sourcenameOutput;
    }
    
    /// <summary>Gets aggergation result.</summary>
    ///
    /// <param name="aggStr">The aggregate string.</param>
    /// <param name="SqlStr">The SQL string.</param>
    ///
    /// <returns>The aggergation result.</returns>
    
    public string GetAggergationResult(string aggStr, IQueryable<dynamic> SqlStr)
    {
        var aggtot = aggStr.Split(',');
        object aggResult = 0;
        string result = "";
        int entcount = 0;
        foreach(var propAgg in aggtot)
        {
            var prop = propAgg.Split('-');
            var proptot = prop[0].ToString().Split('.');
            var entName = proptot[0];
            var propName = proptot[1];
            var AggOp = prop[1].Split(';');
            foreach(var AggName in AggOp)
            {
                if(AggName == "Sum")
                {
                    aggResult = SqlStr.Aggration(propName, "Sum");
                }
                if(AggName == "Count")
                {
                    aggResult = SqlStr.Aggration(propName, "Count");
                }
                if(AggName == "Average")
                {
                    aggResult = SqlStr.Aggration(propName, "Average");
                }
                if(AggName == "Min")
                {
                    aggResult = SqlStr.Aggration(propName, "Min");
                }
                if(AggName == "Max")
                {
                    aggResult = SqlStr.Aggration(propName, "Max");
                }
                result += entName + "." + propName + "-" + AggName + "-" + aggResult + ",";
            }
        }
        return result.TrimEnd(',');
    }
    
    /// <summary>Gets aggregate exponent.</summary>
    ///
    /// <param name="properties">     The properties.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The aggregate exponent.</returns>
    
    public string GetAggExp(string properties, string relationsValues)
    {
        StringBuilder SumCond = new StringBuilder();
        var rel = relationsValues.Split(',');
        var propsStr = properties.Split(',');
        //ViewBag["Order"] = null;
        foreach(var props in propsStr)
        {
            var prop = props.Split('-');
            var propEntProps = prop[0].Split('.');
            var ent = propEntProps[0];
            var _prop = propEntProps[1];
            var AggregateOp = prop[5];
            if(string.IsNullOrEmpty(relationsValues))
            {
                if(AggregateOp != "")
                {
                    SumCond.Append(ent + "." + _prop + "-" + AggregateOp + ",");
                    break;
                }
            }
            else
            {
                foreach(var rels in rel)
                {
                    var AssoStr = rels.Split('-');
                    var MainEnt = AssoStr[0];
                    var AssoEnt = AssoStr[1];
                    var AssoName = AssoStr[2];
                    if(AggregateOp != "")
                    {
                        if(ent == MainEnt)
                        {
                            if(_prop.Substring(_prop.Length - 2) == "ID" && _prop.Substring(0, _prop.Length - 2) == AssoName)
                            {
                                SumCond.Append(ent + "." + AssoName.ToLower() + "." + "DisplayValue" + "-" + AggregateOp + ",");
                                break;
                            }
                            else
                            {
                                SumCond.Append(ent + "." + _prop + "-" + AggregateOp + ",");
                                break;
                            }
                        }
                        else if(ent == AssoEnt)
                        {
                            SumCond.Append(ent + "." + AssoName.ToLower() + "." + _prop + "-" + AggregateOp + ",");
                            break;
                        }
                    }
                }
            }
        }
        return (SumCond.ToString()).TrimEnd(',');
    }
    
    /// <summary>Gets order by string.</summary>
    ///
    /// <param name="properties">     The properties.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The order by string.</returns>
    
    public string GetOrderByStr(string properties, string relationsValues)
    {
        StringBuilder OrderByCond = new StringBuilder();
        var rel = relationsValues.Split(',');
        var propsStr = properties.Split(',');
        //ViewBag["Order"] = null;
        foreach(var props in propsStr)
        {
            var prop = props.Split('-');
            var propEntProps = prop[0].Split('.');
            var ent = propEntProps[0];
            var _prop = propEntProps[1];
            var _Order = prop[2];
            if(string.IsNullOrEmpty(relationsValues))
            {
                if(_Order != "")
                {
                    OrderByCond.Append(_prop + " " + _Order + ",");
                    break;
                }
            }
            else
            {
                foreach(var rels in rel)
                {
                    var AssoStr = rels.Split('-');
                    var MainEnt = AssoStr[0];
                    var AssoEnt = AssoStr[1];
                    var AssoName = AssoStr[2];
                    if(_Order != "")
                    {
                        //if (ViewBag["Order"] == null)
                        //    ViewBag["Order"] = _Order;
                        if(ent == MainEnt)
                        {
                            if(_prop.Substring(_prop.Length - 2) == "ID" && _prop.Substring(0, _prop.Length - 2) == AssoName)
                            {
                                OrderByCond.Append(AssoName.ToLower() + "." + "DisplayValue " + _Order + ",");
                                break;
                            }
                            else
                            {
                                OrderByCond.Append(_prop + " " + _Order + ",");
                                break;
                            }
                        }
                        else if(ent == AssoEnt)
                        {
                            OrderByCond.Append(AssoName.ToLower() + "." + _prop + " " + _Order + ",");
                            break;
                        }
                    }
                }
            }
        }
        return (OrderByCond.ToString()).TrimEnd(',');
    }
    
    /// <summary>Gets group by string.</summary>
    ///
    /// <param name="properties">     The properties.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The group by string.</returns>
    
    public string GetGroupByStr(string properties, string relationsValues)
    {
        StringBuilder GroupByCond = new StringBuilder();
        var rel = relationsValues.Split(',');
        var propsStr = properties.Split(',');
        var GroupByCols = "";
        foreach(var props in propsStr)
        {
            var prop = props.Split('-');
            var propEntProps = prop[0].Split('.');
            var ent = propEntProps[0];
            var _prop = propEntProps[1];
            var _Order = prop[2];
            var _Group = prop[3];
            if(string.IsNullOrEmpty(relationsValues))
            {
                if(Convert.ToBoolean(_Group))
                {
                    GroupByCond.Append(_prop + ",");
                    GroupByCols += prop[0] + ",";
                    break;
                }
            }
            else
            {
                foreach(var rels in rel)
                {
                    var AssoStr = rels.Split('-');
                    var MainEnt = AssoStr[0];
                    var AssoEnt = AssoStr[1];
                    var AssoName = AssoStr[2];
                    if(Convert.ToBoolean(_Group))
                    {
                        if(ent == MainEnt)
                        {
                            if(_prop.Substring(_prop.Length - 2) == "ID" && _prop.Substring(0, _prop.Length - 2) == AssoName)
                            {
                                GroupByCond.Append(AssoName.ToLower() + "." + "DisplayValue" + ",");
                                GroupByCols += prop[0] + ",";
                                break;
                            }
                            else
                            {
                                GroupByCond.Append(_prop + ",");
                                GroupByCols += prop[0] + ",";
                                break;
                            }
                        }
                        else if(ent == AssoEnt)
                        {
                            GroupByCond.Append(AssoName.ToLower() + "." + _prop + ",");
                            GroupByCols += prop[0] + ",";
                            break;
                        }
                    }
                }
            }
        }
        if(GroupByCond.ToString() != "")
            ViewBag.GroupCol = GroupByCols.TrimEnd(',');
        return (GroupByCond.ToString()).TrimEnd(',');
    }
    
    /// <summary>Gets proerty header.</summary>
    ///
    /// <param name="properties">The properties.</param>
    ///
    /// <returns>An array of object.</returns>
    
    public object[] getProertyHeader(string properties)
    {
        List<object> propHeader = new List<object>();
        var propArr = properties.Split(',');
        foreach(var propItem in propArr)
        {
            var prop = propItem.Split('-')[0];
            string Entityitm = prop.Split('.')[0];
            string propitm = prop.Split('.')[1];
            string propHead = getPropertyDisplayName(propitm, Entityitm);
            propHeader.Add(propHead);
        }
        return propHeader.ToArray();
    }
    
    /// <summary>Gets group proerty header.</summary>
    ///
    /// <param name="GroupProperty">The group property.</param>
    ///
    /// <returns>An array of object.</returns>
    
    public object[] getGroupProertyHeader(string GroupProperty)
    {
        List<object> propHeader = new List<object>();
        var propArr = GroupProperty.Split(',');
        foreach(var propItem in propArr)
        {
            string Entityitm = propItem.Split('.')[0];
            string propitm = propItem.Split('.')[1];
            string propHead = getPropertyDisplayName(propitm, Entityitm);
            propHeader.Add(propHead);
        }
        return propHeader.ToArray();
    }
    
    /// <summary>Gets filter conditions.</summary>
    ///
    /// <param name="FilterCondition">The filter condition.</param>
    /// <param name="EntityName">     Name of the entity.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The filter conditions.</returns>
    
    public StringBuilder GetFilterConditions(string FilterCondition, string EntityName, string relationsValues)
    {
        StringBuilder whereCondition = new StringBuilder();
        var Params = FilterCondition.Split(",".ToCharArray());
        var entityName = EntityName.Split(',');
        foreach(var param in Params)
        {
            string[] coud = param.Split('-');
            var PropertyName = coud[0].Split('.')[1];
            string RelEntStr = "";
            string MasterEnt = "";
            foreach(var item in entityName)
            {
                MasterEnt = item;
                if(item == coud[0].Split('.')[0])
                {
                    if(string.IsNullOrEmpty(relationsValues))
                    {
                        PropertyName = "[" + MasterEnt + "." + PropertyName + "]";
                    }
                    else
                    {
                        RelEntStr = item;
                        var reltStr = relationsValues.Split(',');
                        foreach(var relitem in reltStr)
                        {
                            var rel = relitem.Split('-');
                            if(RelEntStr == rel[1])
                            {
                                if(RelEntStr == rel[0])
                                {
                                    PropertyName = "[" + rel[2] + "ID." + PropertyName + "]";
                                    MasterEnt = rel[0];
                                }
                                break;
                            }
                            else if(MasterEnt == rel[0] && PropertyName == rel[2] + "ID")
                            {
                                PropertyName = "[" + rel[2] + "ID.DisplayValue]";
                                MasterEnt = rel[0];
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            var Operator = getOperatorAndValue(coud[1]);
            var Value = string.Empty;
            var LogicalConnector = string.Empty;
            Value = coud[3];
            LogicalConnector = (coud[4] == "0" ? " " : " " + coud[4]);
            whereCondition.Append(LogicalConnector + conditionReports(PropertyName, Operator, Value, MasterEnt));
        }
        return whereCondition;
    }
    
    /// <summary>Condition reports.</summary>
    ///
    /// <param name="property">  The property.</param>
    /// <param name="operators"> The operators.</param>
    /// <param name="value">     The value.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A string.</returns>
    
    public string conditionReports(string property, string operators, string value, string EntityName)
    {
        string expression = string.Empty;
        var PrpertyType = GetDataTypeOfProperty(EntityName, property);
        var dataType = PrpertyType[0];
        property = PrpertyType[1];
        if(value.StartsWith("[") && value.EndsWith("]"))
        {
            var ValueType = GetDataTypeOfProperty(EntityName, value, true);
            if(ValueType != null && ValueType[0] == "dynamic")
            {
                dataType = ValueType[0];
                value = ValueType[1];
            }
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
            DateTime val = Convert.ToDateTime(value);
            expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
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
    
    /// <summary>Gets prpoerty for select.</summary>
    ///
    /// <param name="properties">The properties.</param>
    ///
    /// <returns>The prpoerty for select.</returns>
    
    public string getPrpoertyForSelect(string properties)
    {
        bool first = true;
        string propsArg = "";
        string[] propStrs = properties.Split(',');
        foreach(var item in propStrs)
        {
            string[] propentity = item.Split('-');
            if(first)
            {
                propsArg = propentity[0].Split('.')[1];
            }
            else
                propsArg += "," + propentity[0].Split('.')[1];
            first = false;
        }
        return propsArg;
    }
    
    /// <summary>Property values.</summary>
    ///
    /// <param name="properties">     The properties.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>An object[].</returns>
    
    public object[] PropertyValues(string properties, string relationsValues)
    {
        List<object> rows = new List<object>();
        var props = properties.Split(',');
        foreach(var item in props)
        {
            var prop = item.Split('-')[0];
            var propEntId = item.Split('-')[4];
            string Entityitm = prop.Split('.')[0];
            string propitm = prop.Split('.')[1];
            string propHead = getPropertyFoReports(propitm, Entityitm);
            rows.Add(propHead);
            string EntityitmId = propEntId.Split('.')[0];
            string propId = propEntId.Split('.')[1];
            if(propitm.EndsWith("ID"))
            {
                EntityitmId = GetEntityForId(propitm, relationsValues);
            }
            string propHeadId = getPropertyFoReports(propId, EntityitmId);
            rows.Add(propHeadId);
        }
        return rows.ToArray();
    }
    
    /// <summary>Gets property fo reports.</summary>
    ///
    /// <param name="Property">  The property.</param>
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>The property fo reports.</returns>
    
    public string getPropertyFoReports(string Property, string EntityName)
    {
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            prop = EntityReflector.Properties.FirstOrDefault(q => q.Name == Property).Name;
        }
        catch(Exception ex)
        {
            prop = Property;
        }
        return EntityName + "." + prop;
    }
    
    /// <summary>Gets entity for identifier.</summary>
    ///
    /// <param name="propitm">        The propitm.</param>
    /// <param name="relationsValues">The relations values.</param>
    ///
    /// <returns>The entity for identifier.</returns>
    
    public string GetEntityForId(string propitm, string relationsValues)
    {
        string AssoEntForId = "";
        var rel = relationsValues.Split(',');
        foreach(var rels in rel)
        {
            var AssoStr = rels.Split('-');
            var MainEnt = AssoStr[0];
            var AssoEnt = AssoStr[1];
            var AssoName = AssoStr[2];
            if(propitm.Substring(propitm.Length - 2) == "ID" && propitm.Substring(0, propitm.Length - 2) == AssoName)
            {
                AssoEntForId = AssoEnt;
                break;
            }
        }
        return AssoEntForId;
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="FilterPropertyName">Name of the filter property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyDataType action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetPropertyDataType(string FilterPropertyName)
    {
        string[] FilterName = FilterPropertyName.Split('.');
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == FilterName[0]).ToList();
        string type = "";
        string data = "";
        foreach(GeneratorBase.MVC.ModelReflector.Property p in entitys.FirstOrDefault().Properties)
        {
            if(p.Name == FilterName[1] && !FilterName[1].EndsWith("ID"))
            {
                type = p.DataType;
            }
            if(type != "")
                break;
        }
        if(type.ToLower() != "string" && type.ToLower() != "datatime")
        {
            data = "nonstring";
        }
        else if(type == "")
        {
            data = "";
        }
        else
            data = "string";
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyAggDataType action.</returns>
    
    public JsonResult GetPropertyAggDataType(string PropName)
    {
        string[] FilterName = PropName.Split('.');
        var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == FilterName[0]).ToList();
        string type = "";
        string data = "";
        foreach(GeneratorBase.MVC.ModelReflector.Property p in entitys.FirstOrDefault().Properties)
        {
            if(p.Name == FilterName[1] && !FilterName[1].EndsWith("ID"))
            {
                type = p.DataType;
            }
            if(type != "")
                break;
        }
        if(type.ToLower() == "datetime")
        {
            data = "datetime";
        }
        if(type.ToLower() == "decimal")
        {
            data = "decimal";
        }
        if(type.ToLower() == "int32")
        {
            data = "int32";
        }
        if(type.ToLower() == "int64")
        {
            data = "double";
        }
        if(type == "")
        {
            data = "string";
        }
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Gets operator and value.</summary>
    ///
    /// <param name="opValue">The operation value.</param>
    ///
    /// <returns>The operator and value.</returns>
    
    public string getOperatorAndValue(string opValue)
    {
        string opval = "";
        switch(opValue)
        {
        //numeric oprators
        case "1":
            opval = "> ";
            break;
        case "2":
            opval = ">= ";
            break;
        case "3":
            opval = "= ";
            break;
        case "4":
            opval = "<>";
            break;
        case "5":
            opval = "< ";
            break;
        case "6":
            opval = "<=";
            break;
        //string oprators
        case "7":
            //Contains
            opval = "contains";
            break;
        case "8":
            //Begins With
            opval = "contains";
            break;
        case "9":
            //Ends With
            opval = "contains";
            break;
        case "10":
            opval = "=";
            break;
        case "11":
            opval = "<> ";
            break;
        default:
            break;
        }
        return opval;
    }
    
    /// <summary>Gets record value.</summary>
    ///
    /// <param name="relation">The relation.</param>
    /// <param name="row">     The row.</param>
    /// <param name="column">  The column.</param>
    ///
    /// <returns>The record value.</returns>
    
    public object getRecordValue(string relation, object row, object column)
    {
        object res = null;
        if(!string.IsNullOrEmpty(relation))
        {
            var relArry = relation.Split(',');
            bool isColumnAsso = false;
            bool isColumnAssoEntity = false;
            string assconame = "";
            string userEntity = "";
            foreach(var relItem in relArry)
            {
                var relItemArry = relItem.Split('-');
                var actualCol = column.ToString().Split('.');
                if(actualCol[1].Substring(0, actualCol[1].Length - 2).ToLower() == relItemArry[2].ToLower() && (actualCol[0] == relItemArry[0] || actualCol[1] == relItemArry[2] + "ID"))
                {
                    isColumnAsso = true;
                    assconame = relItemArry[2].ToLower();
                    userEntity = relItemArry[1];
                    break;
                }
                else if(actualCol[0] == relItemArry[1])
                {
                    if(actualCol[0] == relItemArry[1])
                    {
                        isColumnAssoEntity = true;
                        assconame = relItemArry[2].ToLower();
                    }
                    break;
                }
            }
            if(isColumnAsso)
            {
                res = row.GetType().GetProperty(assconame).GetValue(row);
                if(res != null)
                {
                    if(userEntity.ToLower() == "identityuser")
                        res = res == null ? null : res.GetType().GetProperty("UserName").GetValue(res);
                    else
                        res = res == null ? null : res.GetType().GetProperty("DisplayValue").GetValue(res);
                }
                else
                {
                    res = null;
                }
            }
            else if(isColumnAssoEntity)
            {
                res = row.GetType().GetProperty(assconame).GetValue(row);
                if(res != null)
                {
                    res = res == null ? null : res.GetType().GetProperty(column.ToString().Split('.')[1]).GetValue(res);
                }
                else
                    res = null;
            }
            else
            {
                res = row.GetType().GetProperty(column.ToString().Split('.')[1]).GetValue(row);
            }
        }
        else
        {
            res = row.GetType().GetProperty(column.ToString().Split('.')[1]).GetValue(row);
        }
        return res;
    }
    
    /// <summary>Gets data format string.</summary>
    ///
    /// <param name="row">   The row.</param>
    /// <param name="column">The column.</param>
    ///
    /// <returns>The data format string.</returns>
    
    public object getDataFormatString(object row, object column)
    {
        Type currentType = Type.GetType("GeneratorBase.MVC.Models." + column.ToString().Split('.')[0] + ", GeneratorBase.MVC.Models");
        //row.GetType().GetProperty(column.ToString().Split('.')[0]).GetType();
        //row.GetType();
        DisplayFormatAttribute currentDisplayFormatAttribute;
        string currentDataFormatString = "{0}";
        PropertyInfo property = currentType.GetProperty(column.ToString().Split('.')[1]);
        currentDisplayFormatAttribute = (DisplayFormatAttribute)property.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault();
        if(currentDisplayFormatAttribute != null)
        {
            currentDataFormatString = currentDisplayFormatAttribute.DataFormatString;
        }
        return currentDataFormatString;
        //return string.Format(currentDataFormatString, value);
    }
    
}
}


