﻿using System;
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
using System.Threading.Tasks;
using EntityFramework.DynamicFilters;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Export Data Configuration actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class T_ExportDataConfigurationController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(T_ExportDataConfiguration t_exportdataconfiguration, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    private void LoadViewDataAfterOnEdit(T_ExportDataConfiguration t_exportdataconfiguration)
    {
        LoadViewDataBeforeOnEdit(t_exportdataconfiguration, false);
        CustomLoadViewDataListAfterEdit(t_exportdataconfiguration);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="t_exportdataconfiguration">         The Export Data Configuration.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(T_ExportDataConfiguration t_exportdataconfiguration, bool loadCustomViewData = true)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        ViewBag.T_ExportDataConfigurationExportDataDetailsAssociationCount = db.T_ExportDataDetailss.Count(p => p.T_ExportDataConfigurationExportDataDetailsAssociationID == t_exportdataconfiguration.Id);
        ViewBag.T_ExportDataConfigurationExportDataLogAssociationCount = db.T_ExportDataLogs.Count(p => p.T_ExportDataConfigurationExportDataLogAssociationID == t_exportdataconfiguration.Id);
        ViewBag.T_ExportDataConfigurationIsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnEdit", false, null);
        ViewBag.T_ExportDataConfigurationIsGroupsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnEdit", true, null);
        ViewBag.T_ExportDataConfigurationIsSetValueUIRule = checkSetValueUIRule(User, "T_ExportDataConfiguration", "OnEdit", new long[] { 6, 8 }, null, null);
        ViewBag.T_ExportDataConfigurationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_ExportDataConfiguration", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var DocumentTemplates = db.T_DocumentTemplates.Where(p => p.T_EntityName == "T_ExportDataConfiguration" && !p.T_Disable.Value).OrderBy(p => p.T_DisplayOrder).ToList();
        DocumentTemplates = FilterDocumentTemplateForSelectedTenant(DocumentTemplates);
        ViewBag.DocumentTemplates = DocumentTemplates.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        if(string.IsNullOrEmpty(t_exportdataconfiguration.T_AllowedRoles))
            ViewBag.T_AllowedRoles = new SelectList(RoleList, "", "");
        else
            ViewBag.T_AllowedRoles = new SelectList(RoleList, "", "", t_exportdataconfiguration.T_AllowedRoles);
        ViewBag.T_EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName", t_exportdataconfiguration.T_EntityName);
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(t_exportdataconfiguration);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    private void LoadViewDataAfterOnCreate(T_ExportDataConfiguration t_exportdataconfiguration)
    {
        CustomLoadViewDataListAfterOnCreate(t_exportdataconfiguration);
        ViewBag.T_ExportDataConfigurationIsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnCreate", false, null);
        ViewBag.T_ExportDataConfigurationIsGroupsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnCreate", true, null);
        ViewBag.T_ExportDataConfigurationIsSetValueUIRule = checkSetValueUIRule(User, "T_ExportDataConfiguration", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.T_ExportDataConfigurationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_ExportDataConfiguration", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        ViewBag.T_ExportDataConfigurationIsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnCreate", false, null);
        ViewBag.T_ExportDataConfigurationIsGroupsHiddenRule = checkHidden(User, "T_ExportDataConfiguration", "OnCreate", true, null);
        ViewBag.T_ExportDataConfigurationIsSetValueUIRule = checkSetValueUIRule(User, "T_ExportDataConfiguration", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.T_ExportDataConfigurationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_ExportDataConfiguration", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        ViewBag.T_AllowedRoles = new SelectList(RoleList, "", "");
        ViewBag.T_EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsnotificationEntity && !p.IsDefault), "Name", "DisplayName");
        //ViewBag.TenantId = CommonFunction.Instance.SetSelectListTenantViewBag((CustomPrincipal)User, null);
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    public ActionResult DoExport(T_ExportDataConfigurationIndexViewModel model, IQueryable<T_ExportDataConfiguration> _T_ExportDataConfiguration)
    {
        if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "T_ExportDataConfiguration", User) || !User.CanView("T_ExportDataConfiguration"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(model.ExportType == "csv")
        {
            model.pageNumber = 1;
            if(_T_ExportDataConfiguration.Count() > 0)
                model.PageSize = _T_ExportDataConfiguration.Count();
            var csvdata = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
            csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
            return new CsvResult<T_ExportDataConfiguration>(csvdata.ToList(), "Export Data Configuration.csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] { });
        }
        else
        {
            model.pageNumber = 1;
            if(_T_ExportDataConfiguration.Count() > 0)
                model.PageSize = _T_ExportDataConfiguration.Count();
            return DownloadExcel(_T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize).ToList());
        }
    }
    public ActionResult DoBulkOperations(T_ExportDataConfigurationIndexViewModel model, IQueryable<T_ExportDataConfiguration> _T_ExportDataConfiguration, IQueryable<T_ExportDataConfiguration> lstT_ExportDataConfiguration)
    {
        if(model.BulkOperation != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
        {
            model.TemplatesName = "IndexPartial";
            ViewData["BulkAssociate"] = model.BulkAssociate;
            if(!string.IsNullOrEmpty(model.caller))
            {
                FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                _T_ExportDataConfiguration = _fad.FilterDropdown<T_ExportDataConfiguration>(User, _T_ExportDataConfiguration, "T_ExportDataConfiguration", model.caller);
            }
            if(Convert.ToBoolean(model.BulkAssociate))
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = sortRecords(lstT_ExportDataConfiguration.Except(_T_ExportDataConfiguration), model.sortBy, model.IsAsc).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
                else
                {
                    model.list = lstT_ExportDataConfiguration.Except(_T_ExportDataConfiguration).OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
            }
            else
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
                else
                {
                    model.list = _T_ExportDataConfiguration.OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
            }
        }
        return View();
    }
    
    /// <summary>Export Excel.</summary>
    ///
    /// <param name="query">List of objects.</param>
    ///
    /// <returns>xlsx File.</returns>
    public FileResult DownloadExcel(List<GeneratorBase.MVC.Models.T_ExportDataConfiguration> query)
    {
        List<string> lstHiddenProp = new List<string>();
        foreach(var item in query)
        {
            item.setDateTimeToClientTime();
            //item.ApplyHiddenRule(User.businessrules, "T_ExportDataConfiguration");
            lstHiddenProp = item.ApplyHiddenRule(User.businessrules, "T_ExportDataConfiguration");
        }
        if(lstHiddenProp.Count > 0)
        {
            var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration").Properties;
            for(int i = 0; i < lstHiddenProp.Count; i++)
            {
                lstHiddenProp[i] = modelproperties.FirstOrDefault(q => q.Name == lstHiddenProp[i]).DisplayName;
            }
        }
        DataTable dt = ExcelExportHelper.ToDataTable<T_ExportDataConfiguration>(User, query, "T_ExportDataConfiguration");
        dt.AcceptChanges();
        string fileName = "Export Data Configuration.xlsx";
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            var columns = EntityColumns().Select(p => p.Value).ToArray();
            dt.SetColumnsOrder(columns);
            //dt.RemoveColumns(new string[] { });
            dt.RemoveColumns((lstHiddenProp.Count > 0) ? lstHiddenProp.ToArray() : new string[] { });
            wb.Properties.Author = "ExportAndPurgeFeatureV2";
            wb.Properties.Title = "Export Data Configuration";
            wb.Properties.Subject = "Export Data Configuration data";
            wb.Properties.Comments = "Export Excel";
            wb.Properties.LastModifiedBy = User.Name;
            //Add DataTable in worksheet
            wb.Worksheets.Add(dt, "List");
            var range = wb.Worksheets.FirstOrDefault(p => p.Name == "List").RangeUsed();
            range.SetAutoFilter(false).Enabled = false;
            range.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.Transparent;
            range.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            range.FirstRow().CellsUsed().Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.Black;
            using(MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
    private Dictionary<string, string> EntityColumns()
    {
        Dictionary<string, string> columns = new Dictionary<string, string>();
        var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration").Properties;
        columns.Add("2", modelproperties.FirstOrDefault(q => q.Name == "T_AutoNo").DisplayName);
        columns.Add("3", modelproperties.FirstOrDefault(q => q.Name == "T_Name").DisplayName);
        columns.Add("4", modelproperties.FirstOrDefault(q => q.Name == "T_EntityName").DisplayName);
        columns.Add("5", modelproperties.FirstOrDefault(q => q.Name == "T_AllowedRoles").DisplayName);
        columns.Add("6", modelproperties.FirstOrDefault(q => q.Name == "T_Description").DisplayName);
        columns.Add("7", modelproperties.FirstOrDefault(q => q.Name == "T_Disable").DisplayName);
        columns.Add("8", modelproperties.FirstOrDefault(q => q.Name == "T_BackGroundColor").DisplayName);
        columns.Add("9", modelproperties.FirstOrDefault(q => q.Name == "T_FontColor").DisplayName);
        columns.Add("10", modelproperties.FirstOrDefault(q => q.Name == "T_ToolTip").DisplayName);
        return columns;
    }
    
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstT_ExportDataConfiguration">The list Export Data Configuration.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<T_ExportDataConfiguration> searchRecords(IQueryable<T_ExportDataConfiguration> lstT_ExportDataConfiguration, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
        {
            lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.Where(s => (s.T_AutoNo != null && SqlFunctions.StringConvert((double)s.T_AutoNo).Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_EntityName) && s.T_EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_AllowedRoles) && s.T_AllowedRoles.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_BackGroundColor) && s.T_BackGroundColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_FontColor) && s.T_FontColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_ToolTip) && s.T_ToolTip.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        }
        else
            lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.Where(s => (s.T_AutoNo != null && SqlFunctions.StringConvert((double)s.T_AutoNo).Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_EntityName) && s.T_EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_AllowedRoles) && s.T_AllowedRoles.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_BackGroundColor) && s.T_BackGroundColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_FontColor) && s.T_FontColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_ToolTip) && s.T_ToolTip.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        bool boolvalue;
        if(Boolean.TryParse(searchString, out boolvalue))
            lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.Union(db.T_ExportDataConfigurations.Where(s => (s.T_Disable == boolvalue)));
        return lstT_ExportDataConfiguration;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstT_ExportDataConfiguration">The IQueryable list Export Data Configuration.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<T_ExportDataConfiguration> sortRecords(IQueryable<T_ExportDataConfiguration> lstT_ExportDataConfiguration, string sortBy, string isAsc)
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
        if(sortBy == "DisplayValue")
            return isAsc.ToLower() == "asc" ? lstT_ExportDataConfiguration.OrderBy(p => p.DisplayValue) : lstT_ExportDataConfiguration.OrderByDescending(p => p.DisplayValue);
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstT_ExportDataConfiguration.Sort(sortBy, true) : lstT_ExportDataConfiguration.Sort(sortBy, false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(T_ExportDataConfiguration), "t_exportdataconfiguration");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T_ExportDataConfiguration>)lstT_ExportDataConfiguration.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstT_ExportDataConfiguration.ElementType, lambda.Body.Type },
                       lstT_ExportDataConfiguration.Expression,
                       lambda));
    }
    /// <summary>Searches for the similar records (Match Making).</summary>
    ///
    /// <param name="id">          The identifier.</param>
    /// <param name="sourceEntity">Source entity.</param>
    ///
    /// <returns>A response stream to send to the FindFSearch View.</returns>
    public ActionResult FindFSearch(string id, string sourceEntity)
    {
        return null;
    }
    
    /// <summary>Renders UI to define faceted search.</summary>
    ///
    /// <param name="searchString"> The search string.</param>
    /// <param name="HostingEntity">The hosting entity.</param>
    /// <param name="RenderPartial">The render partial.</param>
    /// <param name="ShowDeleted">  (Optional) True to show, false to hide the deleted (when soft-delete is on).</param>
    ///
    /// <returns>A response stream to send to the SetFSearch View.</returns>
    public ActionResult SetFSearch(string searchString, string HostingEntity, bool? RenderPartial, string FsearchId = "", bool ShowDeleted = false)
    {
        if(ShowDeleted)
        {
            db.DisableFilter("IsDeleted");
            ViewData["ShowDeleted"] = ShowDeleted;
        }
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        if(Qcount > 0)
        {
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        }
        else
        {
        }
        SetFSearchViewBag("T_ExportDataConfiguration");
        ViewBag.HideColumns = new MultiSelectList(EntityColumns(), "Key", "Value");
        ViewBag.FsearchId = FsearchId;
        return View(new T_ExportDataConfiguration());
    }
    
    
    /// <summary>Renders result of faceted search.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="currentFilter">  Specifying the current filter.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="FSFilter">       A filter specifying the file system.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="search">         The search.</param>
    /// <param name="IsExport">       Is export excel.</param>
    /// <param name="FilterCondition">The filter condition.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="AssociatedType"> Association name.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="SortOrder">      The sort order.</param>
    /// <param name="HideColumns">    The hide columns.</param>
    /// <param name="GroupByColumn">  The group by column.</param>
    /// <param name="IsReports">      Is reports.</param>
    /// <param name="IsdrivedTab">    Isdrived tab.</param>
    /// <param name="IsFilter">       (Optional) IsFilter.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted (when soft-delete is on).</param>
    ///
    /// <returns>A response stream to send to the FSearch View.</returns>
    [Audit("FacetedSearch")]
    //public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport  ,string T_AutoNoFrom,string T_AutoNoTo,string T_Disable,string FilterCondition, string HostingEntity, string AssociatedType,string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn,bool? IsReports, bool? IsdrivedTab, bool? IsFilter=false, bool ShowDeleted = false, string ExportType = null)
    public ActionResult FSearch(T_ExportDataConfigurationIndexArgsOption args)
    {
        //FSearchViewBag(currentFilter, searchString, FSFilter, sortBy, isAsc, page, itemsPerPage, search, FilterCondition, HostingEntity, AssociatedType, HostingEntityID, viewtype, SortOrder, HideColumns, GroupByColumn, IsReports, IsdrivedTab, IsFilter,"T_ExportDataConfiguration");
        IndexViewBag(args);
        T_ExportDataConfigurationIndexViewModel model = new T_ExportDataConfigurationIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, Convert.ToInt32(model.HostingEntityID), model.AssociatedType);
        var lstT_ExportDataConfiguration = from s in db.T_ExportDataConfigurations
                                           select s;
        // for Restrict Dropdown
        ViewBag.T_ExportDataConfigurationRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_ExportDataConfiguration", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_ExportDataConfiguration = searchRecords(lstT_ExportDataConfiguration, model.searchString.ToUpper(), true);
        }
        if(!string.IsNullOrEmpty(model.search))
            model.search = model.search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(model.search))
        {
            model.SearchResult += "\r\n General Criterial= " + model.search + ",";
            lstT_ExportDataConfiguration = searchRecords(lstT_ExportDataConfiguration, model.search, true);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_ExportDataConfiguration = sortRecords(lstT_ExportDataConfiguration, model.sortBy, model.IsAsc);
        }
        else lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.OrderByDescending(c => c.Id);
        lstT_ExportDataConfiguration = CustomSorting(lstT_ExportDataConfiguration, model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc);
        //lstT_ExportDataConfiguration = lstT_ExportDataConfiguration;
        if(!string.IsNullOrEmpty(model.FilterCondition))
        {
            StringBuilder whereCondition = new StringBuilder();
            var conditions = Server.UrlDecode(model.FilterCondition).Split("?".ToCharArray()).Where(lrc => lrc != "");
            int iCnt = 1;
            foreach(var cond in conditions)
            {
                if(string.IsNullOrEmpty(cond)) continue;
                var param = cond.Split(",".ToCharArray());
                var PropertyName = param[0];
                var Operator = param[1];
                var Value = string.Empty;
                var LogicalConnector = string.Empty;
                var type = string.Empty;
                Value = param[2];
                LogicalConnector = (param[3] == "AND" ? " And" : " Or");
                if(param.Count() > 4)
                    type = param[4];
                if(iCnt == conditions.Count())
                    LogicalConnector = "";
                model.SearchResult += " " + GetPropertyDP("T_ExportDataConfiguration", PropertyName) + " " + Operator + " " + Value.Trim() + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch("T_ExportDataConfiguration", PropertyName, Operator, Value.Trim(), type) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstT_ExportDataConfiguration = lstT_ExportDataConfiguration.Where(whereCondition.ToString());
            model.FilterCondition = model.FilterCondition;
        }
        var DataOrdering = string.Empty;
        if(!string.IsNullOrEmpty(model.GroupByColumn))
        {
            DataOrdering = model.GroupByColumn;
            model.IsGroupBy = true;
        }
        if(!string.IsNullOrEmpty(model.SortOrder))
            DataOrdering += model.SortOrder;
        if(!string.IsNullOrEmpty(DataOrdering))
            lstT_ExportDataConfiguration = Sorting.Sort<T_ExportDataConfiguration>(lstT_ExportDataConfiguration, DataOrdering);
        var _T_ExportDataConfiguration = lstT_ExportDataConfiguration;
        if(model.T_AutoNoFrom != null || model.T_AutoNoTo != null)
        {
            try
            {
                long from = model.T_AutoNoFrom == null ? 0 : Convert.ToInt64(model.T_AutoNoFrom);
                long to = model.T_AutoNoTo == null ? long.MaxValue : Convert.ToInt64(model.T_AutoNoTo);
                _T_ExportDataConfiguration = _T_ExportDataConfiguration.Where(o => o.T_AutoNo >= from && o.T_AutoNo <= to);
                model.SearchResult += "\r\n Auto No.= " + from + "-" + to;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(model.T_Disable != null)
        {
            try
            {
                bool boolvalue = Convert.ToBoolean(model.T_Disable);
                _T_ExportDataConfiguration = _T_ExportDataConfiguration.Where(o => o.T_Disable == boolvalue);
                model.SearchResult += "\r\n Disable= " + model.T_Disable;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        _T_ExportDataConfiguration = FilterByHostingEntity(_T_ExportDataConfiguration, model.HostingEntity, model.AssociatedType, Convert.ToInt32(model.HostingEntityID));
        if(model.viewtype == "Metric")
        {
            var querystring = HttpUtility.ParseQueryString(Request.Url.Query);
            var aggregate = querystring["aggregate"];
            var aggregateproperty = querystring["aggregateproperty"];
            if(!string.IsNullOrEmpty(model.GroupByColumn))
            {
                model.GroupByColumn = model.GroupByColumn.TrimEnd(",".ToCharArray());
                if(model.GroupByColumn.ToLower().EndsWith("id"))
                    model.GroupByColumn = model.GroupByColumn.ToLower().Substring(0, model.GroupByColumn.Length - 2) + ".DisplayValue";
                if(string.IsNullOrEmpty(aggregateproperty)) aggregateproperty = "Id";
                var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration");
                var property = entityModel.Properties.FirstOrDefault(p => p.Name == model.GroupByColumn);
                string dataType = string.Empty;
                string DisplayFormat = string.Empty;
                if(property != null && property.DataTypeAttribute != null)
                {
                    dataType = property.DataTypeAttribute;
                    DisplayFormat = (dataType.ToLower() == "date") ? CustomDisplayFormat.ConvertToMomentFormat(property.DisplayFormat) : property.DisplayFormat;
                }
                IQueryable result = null;
                if(model.GroupByColumn.ToLower().Contains("displayvalue") == true)
                    if(aggregate != "Count")
                        result = _T_ExportDataConfiguration.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it." + aggregate + "(Value) as Y)").OrderBy("X");
                    else
                        result = _T_ExportDataConfiguration.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it." + aggregate + "() as Y)").OrderBy("X");
                else if(aggregate != "Count")
                    result = _T_ExportDataConfiguration.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "(Value) as Y)").OrderBy("X");
                else
                    result = _T_ExportDataConfiguration.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "() as Y)").OrderBy("X");
                var groupbyresult = result.ToListAsync().Result;
                return Json(new { type = "groupby", result = groupbyresult, dataType = dataType, displayFormat = DisplayFormat }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            if(aggregate != "Count")
            {
                var result = _T_ExportDataConfiguration.Aggration(aggregateproperty, aggregate);
                var entityName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration");
                var property = entityName.Properties.FirstOrDefault(p => p.Name == aggregateproperty);
                if(property != null && property.DataTypeAttribute != null)
                {
                    if(result == null)
                        result = string.Format(property.DisplayFormat, "0.00");
                    else
                        result = string.Format(property.DisplayFormat, result);
                }
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return Json(_T_ExportDataConfiguration.Count(), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if((string)model.SearchResult != null)
            model.SearchResult = ((string)model.SearchResult).TrimStart("\r\n".ToCharArray()).TrimEnd(", ".ToCharArray()).TrimEnd(",".ToCharArray());
        model = (T_ExportDataConfigurationIndexViewModel)SetPagination(model, "T_ExportDataConfiguration");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_ExportDataConfiguration.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        //
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_ExportDataConfiguration);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_ExportDataConfiguration.Count();
                int quotient = totalListCount / model.PageSize;
                var remainder = totalListCount % model.PageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(model.pageNumber > maxpagenumber)
                {
                    model.pageNumber = 1;
                }
            }
        }
        model.Pages = model.pageNumber;
        if(!Request.IsAjaxRequest())
        {
            var list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_ExportDataConfigurationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_ExportDataConfigurationlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_ExportDataConfiguration", tagsSplit.ToArray());
                }
            model.list = list;
            return View("Index", model);
        }
        else
        {
            var list = _T_ExportDataConfiguration.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_ExportDataConfigurationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_ExportDataConfigurationlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_ExportDataConfiguration", tagsSplit.ToArray());
                }
            if(model.TemplatesName == null)
            {
                model.list = list;
                return PartialView("IndexPartial", model);
            }
            else
            {
                model.list = list;
                return PartialView(model.TemplatesName, model);
            }
        }
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_T_ExportDataConfiguration">IQueryable<T_ExportDataConfiguration>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<T_ExportDataConfiguration>.</returns>
    private IQueryable<T_ExportDataConfiguration> FilterByHostingEntity(IQueryable<T_ExportDataConfiguration> _T_ExportDataConfiguration, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        return _T_ExportDataConfiguration;
    }
    /// <summary>Gets display value by record id.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    public string GetDisplayValue(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        long idvalue = Convert.ToInt64(id);
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            var _Obj = appcontext.T_ExportDataConfigurations.FirstOrDefault(p => p.Id == idvalue);
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="EntityName"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public Dictionary<long, string> GetDisplayValuePivotMatrix(string EntityName, string caller)
    {
        Dictionary<long, string> dict = new Dictionary<long, string>();
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            IQueryable<T_ExportDataConfiguration> list = appcontext.T_ExportDataConfigurations;
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_ExportDataConfiguration>(new SystemUser(), list, "T_ExportDataConfiguration", caller);
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            foreach(var item in data)
            {
                dict.Add(item.Id, item.Name);
            }
            return dict;
        }
    }
    /// <summary>Gets IQueryable.</summary>
    ///
    /// <param name="appdb">The applicationContext.</param>
    ///
    /// <returns>The IQueryable.</returns>
    public IQueryable<T_ExportDataConfiguration> GetIQueryable(ApplicationContext appdb)
    {
        return appdb.T_ExportDataConfigurations.AsNoTracking();
    }
    /// <summary>Gets extra journal entry - journal entries for tabs(1.m,m.m) associations.</summary>
    ///
    /// <param name="id">  The identifier.</param>
    /// <param name="user">The logged-in user.</param>
    /// <param name="jedb">The JournalEntryContext.</param>
    ///
    /// <returns>The extra journal entry.</returns>
    public IQueryable<JournalEntry> GetExtraJournalEntry(int? id, Models.IUser user, JournalEntryContext jedb, string RelatedEntityRecords)
    {
        var listjournaliquery = jedb.JournalEntries.AsNoTracking().Where(p => p.Id == 0);
        Expression<Func<JournalEntry, bool>> predicateJournalEntry = n => false;
        if(RelatedEntityRecords == "T_ExportDataDetails" || string.IsNullOrEmpty(RelatedEntityRecords))
        {
            var T_ExportDataConfigurationExportDataDetailsAssociationIds = jedb.T_ExportDataDetailss.AsNoTracking().Where(p => p.T_ExportDataConfigurationExportDataDetailsAssociationID == id).Select(p => p.Id);
            predicateJournalEntry = predicateJournalEntry.Or(d => (d.EntityName == "T_ExportDataDetails" && T_ExportDataConfigurationExportDataDetailsAssociationIds.Contains(d.RecordId) && d.Type != "View" && d.Type != "ViewList"));
        }
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
        if(string.IsNullOrEmpty(id)) return "";
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.T_ExportDataConfigurations.Find(Convert.ToInt64(id));
            return _Obj;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    /// <param name="searchkey"></param>
    /// <param name="RecordIds"></param>
    /// <returns></returns>
    public IQueryable<long> SearchRecords(string searchkey, List<long> RecordIds)
    {
        IQueryable<long> iqueryable = Enumerable.Empty<long>().AsQueryable();
        if(string.IsNullOrEmpty(searchkey) || RecordIds.Count() == 0) return iqueryable;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var list = appcontext.T_ExportDataConfigurations.Where(p => RecordIds.Contains(p.Id));
            var result = searchRecords(list, searchkey, true);
            if(result != null)
                iqueryable = result.Select(p => p.Id).ToList().AsQueryable();
            return iqueryable;
        }
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A JSON response stream to send to the GetJsonRecordById action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetJsonRecordById(string id)
    {
        if(string.IsNullOrEmpty(id)) return Json(new T_ExportDataConfiguration(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.T_ExportDataConfigurations.Find(Convert.ToInt64(id));
            long? tenantId = null;
            var _updatedAssocObj = UpdateAssociationValue("T_ExportDataConfiguration", _Obj, tenantId);
            return Json(_updatedAssocObj, JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>Gets record by identifier reflection.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier reflection.</returns>
    public string GetRecordById_Reflection(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            context.Configuration.LazyLoadingEnabled = false;
            var _Obj = context.T_ExportDataConfigurations.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_ExportDataConfiguration>(_Obj, "T_ExportDataConfiguration", new string[] { "" });
        }
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller to filter items.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetAllValueForFilter(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<T_ExportDataConfiguration> list = db.T_ExportDataConfigurations;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_ExportDataConfiguration>(User, list, "T_ExportDataConfiguration", caller);
        var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                   select new { Id = x.Id, Name = x.DisplayValue };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="associationName">The associationName.</param>
    /// <param name="RestrictDropdownVal">The RestrictDropdownVal.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetAllValueDropMenu(string associationName, string RestrictDropdownVal = "")
    {
        var list = db.T_ExportDataConfigurations.OrderBy(p => p.DisplayValue).AsQueryable();//;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_ExportDataConfiguration>(User, list, "T_ExportDataConfiguration", associationName + "ID", RestrictDropdownVal);
        return Json(list.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
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
    [Audit(0)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string RestrictDropdownVal = "", string CustomParameter = "")
    {
        var list = CustomDropdownFilter(db.T_ExportDataConfigurations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, ExtraVal, RestrictDropdownVal, CustomParameter, "GetAllValue");
        var result = DropdownHelper.GetAllValue<T_ExportDataConfiguration>(User, list, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_ExportDataConfiguration", RestrictDropdownVal);
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue,
            style = UIPropertyHtmlHelper.UIPropertyHtmlHelper.UIPropertyStyleDropDown(x, "Lable")
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (used in business rule).</summary>
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
        var result = DropdownHelper.GetAllValueForRB<T_ExportDataConfiguration>(User, db.T_ExportDataConfigurations, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_ExportDataConfiguration");
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown for businessrule).</summary>
    ///
    /// <param name="propNameBR">The property name line break.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValueForBR action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
    {
        IQueryable<T_ExportDataConfiguration> list = db.T_ExportDataConfigurations;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            var result = list.Select("new(Id," + propNameBR + " as value)");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown on UI).</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="caller">            The caller to filter items.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValue(string caller, string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string CustomParameter = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.T_ExportDataConfigurations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, null, null, CustomParameter, "GetAllMultiSelectValue");
        var result = DropdownHelper.GetAllMultiSelectValue(User, list, key, AssoNameWithParent, AssociationID, "T_ExportDataConfiguration", caller);
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown on UI).</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="caller">            The caller to filter items.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetMultiSelectValueAllSelection(string caller, string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string bulkSelection = null, string CustomParameter = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.T_ExportDataConfigurations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, null, null, CustomParameter, "GetMultiSelectValueAllSelection");
        var result = DropdownHelper.GetMultiSelectValueAllSelection(User, list, key, AssoNameWithParent, AssociationID, "T_ExportDataConfiguration", caller);
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Get Display Value for Import</summary>
    ///
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    ///
    private string GetDisplayValueForImport(string AssociatedType, string HostingEntityID, string columnValue)
    {
        #region Get Display Value
        long id = Convert.ToInt64(HostingEntityID);
        switch(AssociatedType + "ID")
        {
        default:
            break;
        }
        #endregion
        return columnValue;
    }
    
    /// <summary>(An Action that handles HTTP GET requests) uploads the given excel/csv file.</summary>
    ///
    /// <param name="FileType">The file uploaded.</param>
    /// <returns>A response stream to send to the Upload View.</returns>
    [HttpGet]
    public ActionResult Upload(string FileType, string AssociatedType, string HostingEntityName, string HostingEntityID, string UrlReferrer, string ImportType = "")
    {
        if(FileType.ToLower() == "csv")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportCSV", "T_ExportDataConfiguration", User) || !User.CanAdd("T_ExportDataConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else if(FileType.ToLower() == "xls" || FileType.ToLower() == "xlsx")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "T_ExportDataConfiguration", User) || !User.CanAdd("T_ExportDataConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_ExportDataConfiguration")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_ExportDataConfiguration").ToList();
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
        ViewBag.FileType = FileType.ToLower();
        ViewBag.ImportType = ImportType;
        ViewBag.AssociatedType = AssociatedType;
        ViewBag.HostingEntityName = HostingEntityName;
        ViewBag.HostingEntityID = HostingEntityID;
        ViewBag.UrlReferrer = UrlReferrer;
        ViewBag.Title = "Upload File";
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <param name="FileUpload">The file uploaded.</param>
    /// <param name="collection">The form collection.</param>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Upload([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection)
    {
        string typeName = "T_ExportDataConfiguration";
        if(FileUpload != null)
        {
            var AssociatedType = collection["hdnAssociatedType"];
            var HostingEntityName = collection["hdnHostingEntityName"];
            var HostingEntityID = collection["hdnHostingEntityID"];
            var UrlReferrer = collection["hdnUrlReferrer"];
            var ImportType = collection["hdnImportType"];
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == typeName);
                string[] allcolumns = null;
                if(entList != null)
                {
                    var entityprop = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime");
                    allcolumns = new string[] { "T_AutoNo", "T_Name", "T_EntityName", "T_AllowedRoles", "T_Description", "T_Disable", "T_BackGroundColor", "T_FontColor", "T_ToolTip" };
                    var prorlist = new List<ModelReflector.Property>();
                    if(!string.IsNullOrEmpty(collection["ListOfMappings"]) && !string.IsNullOrEmpty(ImportType))
                    {
                        var mapName = collection["ListOfMappings"];
                        allcolumns = GetAllColumnForAdvanceImport(typeName, mapName, prorlist, col, objColMap, objColMapAssocProperties);
                    }
                    else
                    {
                        foreach(var prorp in allcolumns)
                        {
                            if(entityprop.Select(s => s.Name).Contains(prorp))
                            {
                                prorlist.Add(entityprop.FirstOrDefault(fd => fd.Name == prorp));
                            }
                        }
                        foreach(var prop in prorlist)
                        {
                            long selectedVal = 0;
                            bool isReq = false;
                            var colSelected = col.FirstOrDefault(p => p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                            var propAssoc = entList.Associations.FirstOrDefault(ae => ae.AssociationProperty == prop.Name);
                            if(propAssoc != null)
                            {
                                isReq = propAssoc.IsRequired;
                                if(isReq && colSelected != null)
                                    selectedVal = long.Parse(colSelected.Value);
                            }
                            else
                            {
                                if(colSelected != null)
                                    selectedVal = long.Parse(colSelected.Value);
                            }
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
                                    if(!lstProperty.ContainsKey(prop.DisplayName))
                                        lstProperty.Add(prop.DisplayName, prop.DisplayName + "-" + assoc.AssociationProperty);
                                }
                                //var dispValue = lstProperty.Keys.FirstOrDefault();
                                objColMapAssocProperties.Add(assoc, new SelectList(lstProperty.AsEnumerable(), "Value", "Key", "Key"));
                            }
                        }
                    }
                }
                ViewBag.AssociatedProperties = objColMapAssocProperties;
                ViewBag.ColumnMapping = objColMap;
                ViewBag.FilePath = fileLocation;
                ViewBag.AssociatedType = AssociatedType;
                ViewBag.HostingEntityName = HostingEntityName;
                ViewBag.HostingEntityID = HostingEntityID;
                ViewBag.UrlReferrer = UrlReferrer;
                ViewBag.ImportType = ImportType;
                if(!string.IsNullOrEmpty(collection["ListOfMappings"]))
                {
                    string colKey = "";
                    string colDisKey = "";
                    string colListInx = "";
                    //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                    string idMapping = collection["ListOfMappings"];
                    string ExistsColumnMappingName = string.Empty;
                    string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    if(collection["DefaultMapping"] == "on")
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.IsDefaultMapping);
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
                    var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == typeName);
                    int mapcnt = 0;
                    foreach(var defcol in ViewBag.ColumnMapping as Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>)
                    {
                        var propDetail = entityModel.Properties.FirstOrDefault(p => p.DisplayName == defcol.Key.DisplayName);
                        var defcolmaped = DefaultMapping.ToList().Where(p => p.TableColumn.Split('$')[p.TableColumn.Split('$').Length - 1] == defcol.Key.Name || p.TableColumn == defcol.Key.DisplayName && !(string.IsNullOrEmpty(p.SheetColumn)));
                        string colSelected = defcolmaped.Count() > 0 ? defcolmaped.FirstOrDefault().SheetColumn : null;
                        var defkeyDispName = defcol.Key.DisplayName.Trim();
                        bool hassameName = false;
                        if(allcolumns != null)
                        {
                            var adventName = allcolumns[mapcnt].Split('$');
                            if(adventName.Length > 1)
                            {
                                var entName = adventName[0];
                                var propName = adventName[1];
                                entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == entName);
                                propDetail = entityModel.Properties.FirstOrDefault(p => p.Name == propName);
                                var dispName = entityModel.Properties.FirstOrDefault(p => p.Name == propName).DisplayName;
                                var defRec = DefaultMapping.ToList().Where(p => p.TableColumn.Split('$')[0] == entName && p.TableColumn.Split('$')[p.TableColumn.Split('$').Length - 1] == propName);
                                colSelected = defRec.Count() > 0 ? defRec.FirstOrDefault().SheetColumn : null;
                                defkeyDispName = colSelected;
                                colDisKey = colDisKey + entName + "." + dispName + ",";
                                colKey = colKey + (defRec.Count() > 0 ? defRec.FirstOrDefault().TableColumn : "") + ",";
                                hassameName = true;
                            }
                            else
                            {
                                colDisKey = colDisKey + defcol.Key.DisplayName + ",";
                                colKey = colKey + defcol.Key.Name + ",";
                                if(propDetail == null)
                                {
                                    entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == typeName);
                                    propDetail = entityModel.Properties.FirstOrDefault(p => p.Name == adventName[0]);
                                }
                            }
                        }
                        int colExist = 0;
                        if(!string.IsNullOrEmpty(colSelected))
                        {
                            colExist = defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).Count();
                            if(AssociatedType != null && colExist == 0)
                                if(propDetail != null && defcol.Key.Name == propDetail.Name)
                                {
                                    if(hassameName)
                                        defkeyDispName = propDetail.DisplayName;
                                    colExist = 1;
                                }
                            if(colExist == 0)
                                ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
                            if(AssociatedType != null && colExist == 1 && defcol.Key.Name == propDetail.Name)
                            {
                                var defval = "";
                                var defcolint = defcol.Value.Where(s => s.Text.Trim().Equals(defkeyDispName)).FirstOrDefault();
                                if(defcolint != null)
                                    defval = defcolint.Value.ToString();
                                colListInx = colListInx + defval + ",";
                            }
                            else
                                colListInx = colListInx + (colExist > 0 ? defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).FirstOrDefault().Value.ToString() : "") + ",";
                        }
                        else
                            colListInx = colListInx + "" + ",";
                        mapcnt++;
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
                    int mapcount = colListInx.Split(',').Where(p => p.Trim() != string.Empty).Count();
                    if(!string.IsNullOrEmpty(ImportType))
                        mapcount = DefaultMapping.Count();
                    if(DefaultMapping.Count() != mapcount)
                    {
                        ViewBag.DefaultMappingMsg += "There was an ERROR in file being uploaded: It does not contain all the required columns.";
                        ViewBag.DefaultMappingMsg += "<br /><br /> Error Details: <br /> The following columns are missing : " + ExistsColumnMappingName;
                        ViewBag.DefaultMappingMsg += "<br /><br /> Please verify the file and upload again. No data has currently been imported and NO change has been made.";
                    }
                    string DetailMessage = "";
                    //string excelConnectionString = string.Empty;
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
                                if(AssociatedType != null && AssociatedType + "ID" == columnlist.Split(',')[j] && HostingEntityID != null)
                                {
                                    columnValue = GetDisplayValueForImport(AssociatedType, HostingEntityID, columnValue);
                                }
                                if(string.IsNullOrEmpty(columnValue))
                                    continue;
                                objdr[columntable] = columnValue;
                            }
                            tempdt.Rows.Add(objdr);
                        }
                    }
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Export Data Configuration</b>";
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
                                if(AssociatedType != null && AssociatedType + "ID" == association.AssociationProperty && HostingEntityID != null)
                                {
                                    assovalue = GetDisplayValueForImport(AssociatedType, HostingEntityID, assovalue);
                                }
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
                            ViewBag.DetailMessage = DetailMessage + " in the " + fileExtension.ToUpper().Replace(".", "") + " file. Please review the data below, before we import it into the system.";
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
    /// <param name="collection">The form collection.</param>
    ///
    /// <returns>A response stream to send to the ConfirmImportData View.</returns>
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmImportData(FormCollection collection, long Tenant)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["lblColumn"];
        //var columndisplaynamelist = collection["lblColumnDisplayName"];
        var columndisplaynamelist = Request.Form.GetValues("lblColumnDisplayName");
        var selectedlist = collection["colList"];
        var selectedAssocPropList = collection["colAssocPropList"];
        var chkUpdateList = collection["ListChkUpdate"];
        bool SaveMapping = collection["SaveMapping"] == "on" ? true : false;
        string mappingName = collection["MappingName"];
        var AssociatedType = collection["hdnAssociatedType"];
        var HostingEntityName = collection["hdnHostingEntityName"];
        var HostingEntityID = collection["hdnHostingEntityID"];
        var UrlReferrer = collection["hdnUrlReferrer"];
        var ImportType = collection["hdnImportType"];
        string DetailMessage = "";
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string typename = "T_ExportDataConfiguration";
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
            if(!string.IsNullOrEmpty(selectedAssocPropList))
            {
                var entitypropList = selectedAssocPropList.Split(',');
                foreach(var prop in entitypropList)
                {
                    var lst = prop.Split('-');
                    if(!lstEntityProp.ContainsKey(lst[1]))
                        lstEntityProp.Add(lst[1], lst[0]);
                }
            }
            var tenantDb = new ApplicationContext(User, 0);
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
                var tblcols = columndisplaynamelist.ToList();
                var shtcols = selectedlist.Split(',').ToList();
                var tblInternalNames = columnlist;
                //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typename).ToList();
                for(int i = 0; i < tblcols.Count(); i++)
                {
                    ImportConfiguration objImtConfig = null;
                    string shtcolName = string.IsNullOrEmpty(shtcols[i]) ? "" : objDataSet.Tables[0].Columns[int.Parse(shtcols[i]) - 1].Caption;
                    if(string.IsNullOrEmpty(shtcolName))
                        continue;
                    objImtConfig = new ImportConfiguration();
                    objImtConfig.Name = typename;
                    objImtConfig.MappingName = mappingName;
                    objImtConfig.IsDefaultMapping = SaveMapping;
                    objImtConfig.TableColumn = tblcols[i];
                    if(!string.IsNullOrEmpty(ImportType))
                    {
                        var splitedcol = tblInternalNames.Split(',');
                        var ischildentity = splitedcol[i].Split('$').Length > 1 ? true : false;
                        if(ischildentity)
                            objImtConfig.TableColumn = splitedcol[i];
                    }
                    var tblcolarr = tblcols[i].Split('$');
                    if(tblcolarr.Length > 1)
                    {
                        string colname = "";
                        for(int j = 1; j < tblcolarr.Length; j++)
                        {
                            colname += tblcolarr[j] + " ";
                        }
                        shtcolName = colname.TrimEnd(' ');
                    }
                    var assentName = ModelReflector.Entities.FirstOrDefault(p => p.DisplayName == tblcols[i]);
                    if(assentName != null)
                    {
                        var mapingProplst = lstEntityProp.Where(p => p.Value == assentName.Properties.FirstOrDefault().DisplayName && p.Value.ToLower() != "displayvalue");
                        if(mapingProplst != null && mapingProplst.Count() > 0)
                            shtcolName = mapingProplst.FirstOrDefault().Value;
                        else
                        {
                            var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == typename);
                            var propDetail = entityModel.Properties.FirstOrDefault(p => p.DisplayName == tblcols[i] && p.Name.ToLower() != "TenantId".ToLower());
                            if(!string.IsNullOrEmpty(ImportType))
                                shtcolName = propDetail.DisplayName;
                            else
                                shtcolName = lstEntityProp.FirstOrDefault(p => p.Key == propDetail.Name).Value;
                        }
                    }
                    objImtConfig.SheetColumn = shtcolName;
                    db.ImportConfigurations.Add(objImtConfig);
                }
                db.SaveChanges();
            }
            DataTable tempdt = new DataTable();
            if(selectedlist != null && columnlist != null)
            {
                var dtsheetColumns = selectedlist.Split(',').ToList();
                var dttblColumns = columndisplaynamelist.ToList();
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
                    var tblColumns = columndisplaynamelist.ToList();
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
                        if(AssociatedType != null && AssociatedType + "ID" == columnlist.Split(',')[j] && HostingEntityID != null)
                        {
                            columnValue = GetDisplayValueForImport(AssociatedType, HostingEntityID, columnValue);
                        }
                        if(CustomImportDataValidate(objDataSet, objDataSet.Tables[0].Columns[columnSheet].ColumnName, columnValue))
                            objdr[columntable] = columnValue;
                    }
                    tempdt.Rows.Add(objdr);
                }
            }
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Export Data Configuration</b>";
            Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == typename);
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
                        if(AssociatedType != null && AssociatedType + "ID" == association.AssociationProperty && HostingEntityID != null)
                        {
                            assovalue = GetDisplayValueForImport(AssociatedType, HostingEntityID, assovalue);
                        }
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
                ViewBag.DetailMessage = DetailMessage + " in the " + fileExtension.ToUpper().Replace(".", "") + " file. Please review the data below, before we import it into the system.";
            ViewBag.FilePath = FilePath;
            ViewBag.ColumnList = columnlist;
            ViewBag.SelectedList = selectedlist;
            ViewBag.ConfirmImportData = tempdt;
            ViewBag.chkUpdateList = chkUpdateList;
            ViewBag.colAssocPropList = selectedAssocPropList;
            ViewBag.Tenant = Tenant;
            ViewBag.AssociatedType = AssociatedType;
            ViewBag.HostingEntityName = HostingEntityName;
            ViewBag.HostingEntityID = HostingEntityID;
            ViewBag.UrlReferrer = UrlReferrer;
            ViewBag.ImportType = ImportType;
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
    /// <param name="collection">The form collection.</param>
    ///
    /// <returns>A response stream to send to the ImportData View.</returns>
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ImportData(FormCollection collection, long Tenant = 0)
    {
        db.Configuration.AutoDetectChangesEnabled = false;
        db.Configuration.LazyLoadingEnabled = false;
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnColumnList"];
        var selectedlist = collection["hdnSelectedList"];
        var AssociatedType = collection["hdnAssociatedType"];
        var HostingEntityName = collection["hdnHostingEntityName"];
        var HostingEntityID = collection["hdnHostingEntityID"];
        var UrlReferrer = collection["hdnUrlReferrer"];
        var ImportType = collection["hdnImportType"];
        var tenantDb = new ApplicationContext(User, 0);
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        var selectedAssocPropList = collection["hdnSelectedAssocPropList"];
        Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(selectedAssocPropList))
        {
            var entitypropList = selectedAssocPropList.Split(',');
            foreach(var prop in entitypropList)
            {
                var lst = prop.Split('-');
                if(!lstEntityProp.ContainsKey(lst[1]))
                    lstEntityProp.Add(lst[1], lst[0]);
            }
        }
        int countTotalRows = 0;
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            string error = string.Empty;
            if(objDataSet != null && objDataSet.Tables.Count > 0 && selectedlist != null && columnlist != null)
            {
                var sheetColumns = selectedlist.Split(',').ToList();
                var mappedColumns = new List<string>();
                countTotalRows = objDataSet.Tables[0].Rows.Count;
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    T_ExportDataConfiguration model = new T_ExportDataConfiguration();
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
                        mappedColumns.Add(columntable);
                        switch(columntable)
                        {
                        case "T_AutoNo":
                            model.T_AutoNo = Int64.Parse(columnValue);
                            break;
                        case "T_Name":
                            model.T_Name = columnValue;
                            break;
                        case "T_EntityName":
                            model.T_EntityName = columnValue;
                            break;
                        case "T_AllowedRoles":
                            model.T_AllowedRoles = columnValue;
                            break;
                        case "T_Description":
                            model.T_Description = columnValue;
                            break;
                        case "T_Disable":
                            model.T_Disable = Boolean.Parse(columnValue);
                            break;
                        case "T_BackGroundColor":
                            model.T_BackGroundColor = columnValue;
                            break;
                        case "T_FontColor":
                            model.T_FontColor = columnValue;
                            break;
                        case "T_ToolTip":
                            model.T_ToolTip = columnValue;
                            break;
                        default:
                            break;
                        }
                    }
                    // Columns to update
                    var MatchColumns = collection["hdnListChkUpdate"];
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, "T_ExportDataConfiguration", mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User, db, model, "T_ExportDataConfiguration");
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", "T_ExportDataConfiguration");
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                                //if (result == null || result.Count == 0)
                            {
                                var customerror = "";
                                if(!CustomSaveOnImport(model, out customerror, i))
                                {
                                    if(!flagUpdate)
                                    {
                                        db.T_ExportDataConfigurations.Add(model);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                            db.Entry(model).State = EntityState.Detached;
                                            continue;
                                        }
                                    }
                                }
                                error += customerror;
                            }
                            else
                            {
                                if(result != null && result.Count > 0)
                                {
                                    if(ViewBag.ImportError == null)
                                        ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                    else
                                        ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                    error += ((i + 1).ToString()) + ",";
                                }
                                if(validatebr_result != null && validatebr_result.Count > 0)
                                {
                                    foreach(var warningmsg in validatebr_result)
                                    {
                                        if(ViewBag.ImportError == null)
                                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " " + warningmsg.Value;
                                        else
                                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " " + warningmsg.Value;
                                        error += ((i + 1).ToString()) + ",";
                                    }
                                }
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
                    else
                    {
                        if(ViewBag.ImportError == null)
                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + AlrtMsg;
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + AlrtMsg;
                        error += ((i + 1).ToString()) + ",";
                    }
                }
            }
            else
            {
                ViewBag.ImportError = "You have already downloaded error file.";
            }
            if(ViewBag.ImportError != null)
            {
                ViewBag.FilePath = FilePath;
                if(error.Length > 0)
                    ViewBag.ErrorList = error.Substring(0, error.Length - 1);
                ViewBag.Title = "Error List";
                if(!string.IsNullOrEmpty(HostingEntityName))
                {
                    ViewBag.HostingEntityName = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == HostingEntityName).DisplayName;
                    ViewBag.UrlReferrer = UrlReferrer;
                    ViewBag.ImportType = ImportType;
                }
                int countErrorRows = System.Text.RegularExpressions.Regex.Matches(ViewBag.ImportError, "Row No").Count;
                if(countErrorRows > 0)
                {
                    if((countTotalRows - countErrorRows) > 0)
                        ViewBag.ImportError += "<br/><br/> <b>Import error message:</b> Imported only " + (countTotalRows - countErrorRows) + " of " + countTotalRows + " rows.";
                    else
                        ViewBag.ImportError += "<br/><br/> <b>Import error message:</b> None of the rows imported.";
                }
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
                    if(!string.IsNullOrEmpty(HostingEntityName))
                    {
                        ViewBag.HostingEntityName = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == HostingEntityName).DisplayName;
                        ViewBag.UrlReferrer = UrlReferrer;
                        ViewBag.ImportType = ImportType;
                    }
                    return View("Upload");
                }
                return RedirectToAction("Index");
            }
        }
        return View();
    }
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ImportDataAdvanced(FormCollection collection, long Tenant = 0)
    {
        string MainEntityName = "T_ExportDataConfiguration";
        db.Configuration.AutoDetectChangesEnabled = false;
        db.Configuration.LazyLoadingEnabled = false;
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnColumnList"];
        var selectedlist = collection["hdnSelectedList"];
        var AssociatedType = collection["hdnAssociatedType"];
        var HostingEntityName = collection["hdnHostingEntityName"];
        var HostingEntityID = collection["hdnHostingEntityID"];
        var UrlReferrer = collection["hdnUrlReferrer"];
        var ImportType = collection["hdnImportType"];
        var Db = db;
        Db = new ApplicationContext(User, 0);
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        var selectedAssocPropList = collection["hdnSelectedAssocPropList"];
        Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
        Dictionary<string, string> keyvalueForentityprop = new Dictionary<string, string>();
        var typeMain = ModelReflector.Entities.Where(e => e.Name == MainEntityName);
        var lstMaped = GetInternalColumnOfSheet(columnlist, MainEntityName, typeMain);
        //ChangeDataController cd = new ChangeDataController();
        if(!string.IsNullOrEmpty(selectedAssocPropList))
        {
            var entitypropList = selectedAssocPropList.Split(',');
            foreach(var prop in entitypropList)
            {
                var lst = prop.Split('-');
                if(!lstEntityProp.ContainsKey(lst[1]))
                    lstEntityProp.Add(lst[1], lst[0]);
            }
        }
        int countTotalRows = 0;
        var mainEnt = Type.GetType("GeneratorBase.MVC.Models." + MainEntityName + ", GeneratorBase.MVC.Models");
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = DataImport(fileExtension, fileLocation);
            string error = string.Empty;
            if(objDataSet != null && objDataSet.Tables.Count > 0 && selectedlist != null && columnlist != null)
            {
                var sheetColumns = selectedlist.Split(',').ToList();
                var mappedColumns = new List<string>();
                countTotalRows = objDataSet.Tables[0].Rows.Count;
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    T_ExportDataConfiguration model = new T_ExportDataConfiguration();
                    var tblColumns = lstMaped;
                    model = LoadObjectFromSheet(model, sheetColumns, tblColumns, i, objDataSet, AssociatedType, HostingEntityID, lstEntityProp, mappedColumns, MainEntityName, Db) as T_ExportDataConfiguration;
                    var MatchColumns = collection["hdnListChkUpdate"];
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, MainEntityName, mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model as T_ExportDataConfiguration, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User, db, model, MainEntityName);
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", MainEntityName);
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                            {
                                var customerror = "";
                                var errors = new Dictionary<string, string>();
                                if(!CustomSaveOnImport(model as T_ExportDataConfiguration, out customerror, i))
                                {
                                    if(!flagUpdate)
                                    {
                                        IEntity obj = null;
                                        var change = obj;
                                        var values = new Dictionary<string, object>();
                                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                                        if(!string.IsNullOrEmpty(json))
                                        {
                                            result = SaveMapedObjectOfSheet(json, MainEntityName, model.Id);
                                            if(result != null && result.Count > 0)
                                            {
                                                if(ViewBag.ImportError == null)
                                                    ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                                else
                                                    ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                                error += ((i + 1).ToString()) + ",";
                                            }
                                        }
                                    }
                                }
                                error += customerror;
                            }
                            else
                            {
                                if(result != null && result.Count > 0)
                                {
                                    if(ViewBag.ImportError == null)
                                        ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                    else
                                        ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                    error += ((i + 1).ToString()) + ",";
                                }
                                if(validatebr_result != null && validatebr_result.Count > 0)
                                {
                                    foreach(var warningmsg in validatebr_result)
                                    {
                                        if(ViewBag.ImportError == null)
                                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " " + warningmsg.Value;
                                        else
                                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " " + warningmsg.Value;
                                        error += ((i + 1).ToString()) + ",";
                                    }
                                }
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
                    else
                    {
                        if(ViewBag.ImportError == null)
                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + AlrtMsg;
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + AlrtMsg;
                        error += ((i + 1).ToString()) + ",";
                    }
                }
            }
            else
            {
                ViewBag.ImportError = "You have already downloaded error file.";
            }
            if(ViewBag.ImportError != null)
            {
                ViewBag.FilePath = FilePath;
                if(error.Length > 0)
                    ViewBag.ErrorList = error.Substring(0, error.Length - 1);
                ViewBag.Title = "Error List";
                if(!string.IsNullOrEmpty(HostingEntityName))
                {
                    ViewBag.HostingEntityName = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == HostingEntityName).DisplayName;
                    ViewBag.UrlReferrer = UrlReferrer;
                    ViewBag.ImportType = ImportType;
                }
                int countErrorRows = System.Text.RegularExpressions.Regex.Matches(ViewBag.ImportError, "Row No").Count;
                if(countErrorRows > 0)
                {
                    if((countTotalRows - countErrorRows) > 0)
                        ViewBag.ImportError += "<br/><br/> <b>Import error message:</b> Imported only " + (countTotalRows - countErrorRows) + " of " + countTotalRows + " rows.";
                    else
                        ViewBag.ImportError += "<br/><br/> <b>Import error message:</b> None of the rows imported.";
                }
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
                    if(!string.IsNullOrEmpty(HostingEntityName))
                    {
                        ViewBag.HostingEntityName = ModelReflector.Entities.FirstOrDefault(fd => fd.Name == HostingEntityName).DisplayName;
                        ViewBag.UrlReferrer = UrlReferrer;
                        ViewBag.ImportType = ImportType;
                    }
                    return View("Upload");
                }
                return RedirectToAction("Index");
            }
        }
        return View();
    }
    /// <summary>Downloads the sheet described by collection (error sheet - excel import).</summary>
    ///
    /// <param name="collection">The form collection.</param>
    ///
    /// <returns>A response stream to send to the DownloadSheet View.</returns>
    public ActionResult DownloadSheet(FormCollection collection)
    {
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnErrorList"];
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
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
            using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = appcontext.T_ExportDataConfigurations.Where(p => p.Id == Id).FirstOrDefault();
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                return PropValue;
            }
        }
        catch
        {
            return null;
        }
    }
    /// <summary>Creates a JSON result with the given data as its content (Business Rule).</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleOnSaving action.</returns>
    private JsonResult ApplyBusinessRuleOnSaving(T_ExportDataConfiguration t_exportdataconfiguration)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_ExportDataConfiguration").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            //var ruleids = businessrule.Select(q => q.Id).ToList();
            //var typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(User, db, t_exportdataconfiguration, "OnEdit", "T_ExportDataConfiguration");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in resultBR)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + "  ";
                    }
                    return Json(stringResult, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            if(typelist.Contains(2))
            {
                var resultBR = GetMandatoryPropertiesDictionary(User, db, t_exportdataconfiguration, "OnEdit", "T_ExportDataConfiguration");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(!dic.Key.Contains("FailureMessage"))
                        {
                            var type = t_exportdataconfiguration.GetType();
                            if(type.GetProperty(dic.Key) != null)
                            {
                                var propertyvalue = type.GetProperty(dic.Key).GetValue(t_exportdataconfiguration, null);
                                if(propertyvalue == null || string.IsNullOrEmpty(Convert.ToString(propertyvalue)))
                                {
                                    stringResult += dic.Key + " is Required,";
                                }
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(BRID) || !string.IsNullOrEmpty(stringResult))
                        return Json(BRID + " : " + stringResult, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            if(typelist.Contains(15))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, t_exportdataconfiguration, "OnEdit", "T_ExportDataConfiguration");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in resultBR)
                    {
                        stringResult += "Type15:" + dic.Value + "  ";
                    }
                    return Json(stringResult, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
        }
        return Json(null);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="objectId">Identifier for the object.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBeforeById action.</returns>
    [Audit(0)]
    public JsonResult ApplyBusinessRuleBeforeById(long objectId)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_ExportDataConfiguration").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var t_exportdataconfiguration = db.T_ExportDataConfigurations.Find(objectId);
            if(t_exportdataconfiguration != null)
            {
                var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
                if(typelist.Contains(1) || typelist.Contains(11))
                {
                    var validateLockResult = GetLockBusinessRulesDictionary(User, db, t_exportdataconfiguration, "T_ExportDataConfiguration").Where(p => !p.Key.Contains("InformationMessage"));
                    if(validateLockResult.Count() > 0)
                    {
                        string stringResult = "";
                        foreach(var dic in validateLockResult)
                        {
                            stringResult += dic.Key.Replace("FailureMessage", "BR") + dic.Value + " | ";
                        }
                        return Json(new { Result = "lock", data = stringResult + "" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
                if(typelist.Contains(4))
                {
                    var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User, db, t_exportdataconfiguration, "T_ExportDataConfiguration");
                    if(validateMandatorypropertyResult.Count() > 0)
                    {
                        string stringResult = "";
                        string BRID = "";
                        foreach(var dic in validateMandatorypropertyResult)
                        {
                            if(!dic.Key.Contains("FailureMessage"))
                            {
                                var type = t_exportdataconfiguration.GetType();
                                if(type.GetProperty(dic.Key) != null)
                                {
                                    stringResult += dic.Key + ",";
                                }
                            }
                        }
                        if(!string.IsNullOrEmpty(BRID) || !string.IsNullOrEmpty(stringResult))
                            return Json(new { Result = "readonlyproperty", data = stringResult }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        return Json(new { Result = "Success", data = "" }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="t_exportdataconfiguration">       The Export Data Configuration.</param>
    /// <param name="IsReadOnlyIgnore">(Optional) True if is read only ignored, false if not.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBefore action.</returns>
    private JsonResult ApplyBusinessRuleBefore(T_ExportDataConfiguration t_exportdataconfiguration, bool IsReadOnlyIgnore = false)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_ExportDataConfiguration").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(User, db, t_exportdataconfiguration, "T_ExportDataConfiguration").Where(p => p.Key.Contains("FailureMessage"));
                if(validateLockResult.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in validateLockResult)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + " | ";
                    }
                    return Json(stringResult, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            if(typelist.Contains(4) && !IsReadOnlyIgnore)
            {
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User, db, t_exportdataconfiguration, "T_ExportDataConfiguration").Where(p => p.Key.Contains("FailureMessage"));
                if(validateMandatorypropertyResult.Count() > 0)
                {
                    string stringResult = "";
                    foreach(var dic in validateMandatorypropertyResult)
                    {
                        stringResult += dic.Key.Replace("FailureMessage", "BR") + ":" + dic.Value + " | ";
                    }
                    return Json(stringResult, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
        }
        return Json(null);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Readonly business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    /// <param name="ruleType">page from.</param>
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetReadOnlyProperties(T_ExportDataConfiguration OModel, string ruleType = null)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
        return Json(GetReadOnlyPropertiesDictionary(User, db, OModel, "T_ExportDataConfiguration", ruleType), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON (Mandatory business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetMandatoryProperties(T_ExportDataConfiguration OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
        return Json(GetMandatoryPropertiesDictionary(User, db, OModel, ruleType, "T_ExportDataConfiguration"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetUIAlertBusinessRules(T_ExportDataConfiguration OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_ExportDataConfiguration").ToList();
            var BRAll = BR;
            if(ruleType == "OnCreate")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
            else if(ruleType == "OnEdit")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "T_ExportDataConfiguration");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 13);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(13))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        RulesApplied.Add("<span style=\"color:grey;font-size:11px;\">Warning(#" + rules.Value.BRID + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
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
    /// <summary>UpdateHiddenProperties.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="cannotview">Type of the cannotview.</param>
    ///
    /// <returns>T_ExportDataConfiguration object.</returns>
    public T_ExportDataConfiguration UpdateHiddenProperties(T_ExportDataConfiguration OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.T_ExportDataConfigurations.Find(OModel.Id);
                if(obj1 != null)
                {
                    foreach(var item in cannotview.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == item);
                        var propertyInfo = OModel.GetType().GetProperty(item);
                        if(Property != null && propertyInfo != null)
                        {
                            object PropValue = Property.GetValue(obj1, null);
                            PropValue = PropValue == null ? 0 : PropValue;
                            Type targetType = propertyInfo.PropertyType;
                            if(propertyInfo.PropertyType.IsGenericType)
                                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                            try
                            {
                                object safeValue = (PropValue == null) ? null : Convert.ChangeType(PropValue, targetType);
                                propertyInfo.SetValue(OModel, safeValue, null);
                            }
                            catch
                            {
                                propertyInfo.SetValue(OModel, null, null);
                            }
                        }
                    }
                }
            }
        return OModel;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSaveProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSaveProperties(T_ExportDataConfiguration OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
        return Json(GetValidateBeforeSavePropertiesDictionary(User, db, OModel, ruleType, "T_ExportDataConfiguration"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSavePropertiesForPopupConfirm action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSavePropertiesForPopupConfirm(T_ExportDataConfiguration OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
        return Json(GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "T_ExportDataConfiguration"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Lock record business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    [ValidateAntiForgeryToken]
    public JsonResult GetLockBusinessRules(T_ExportDataConfiguration OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_ExportDataConfiguration")));
        return Json(GetLockBusinessRulesDictionary(User, db, OModel, "T_ExportDataConfiguration"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Inline association list (Business rule helper).</summary>
    ///
    /// <returns>A string[].</returns>
    
    public string[] InlineAssociationList()
    {
        string[] inlineassoclist = null;
        return inlineassoclist;
    }
    /// <summary>Gets identifier from display value of an object.</summary>
    ///
    /// <param name="displayvalue">The displayvalue.</param>
    ///
    /// <returns>The Id from display value.</returns>
    [Audit(0)]
    public long? GetIdFromDisplayValue(string displayvalue, long tenantId = 0)
    {
        if(string.IsNullOrEmpty(displayvalue)) return 0;
        using(ApplicationContext db1 = new ApplicationContext(new SystemUser(), 0))
        {
            db1.Configuration.LazyLoadingEnabled = false;
            var _Obj = db1.T_ExportDataConfigurations.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if(_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
    }
    /// <summary>Gets identifier from property value.</summary>
    ///
    /// <param name="PropName"> Name of the property.</param>
    /// <param name="PropValue">The property value.</param>
    ///
    /// <returns>The Id from property value.</returns>
    [Audit(0)]
    public long? GetIdFromPropertyValue(string PropName, string PropValue)
    {
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            IQueryable query = context.T_ExportDataConfigurations;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(T_ExportDataConfiguration), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if(PropValue.ToLower().Trim() != "null")
            {
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    PropValue = GetIdofDisplayValueforSetIfZero(PropName, "T_ExportDataConfiguration", PropValue);
                    System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                    object PropValue1 = typeConverter.ConvertFromString(PropValue);
                    lambda = Expression.Lambda<Func<T_ExportDataConfiguration, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue1), member.Type)), p);
                }
                else
                {
                    lambda = Expression.Lambda<Func<T_ExportDataConfiguration, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
                }
            }
            else
                lambda = Expression.Lambda<Func<T_ExportDataConfiguration, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<T_ExportDataConfiguration>)q);
            if(list1 != null && list1.Count() > 0)
                return Int64.TryParse(list1.FirstOrDefault().Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="PropName"></param>
    /// <param name="EntityName"></param>
    /// <param name="parmValue"></param>
    /// <returns></returns>
    public string GetIdofDisplayValueforSetIfZero(string PropName, string EntityName, string parmValue)
    {
        string PropValue = "0";
        string propertyName = PropName.Replace("ID", "").ToLower().Trim();
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(AssociationInfo != null)
        {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller");
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromDisplayValue");
                object[] MethodParams = new object[] { parmValue.ToString() };
                var obj = mc.Invoke(objController, MethodParams);
                PropValue = obj.ToString();
            }
            return PropValue.ToString();
        }
        return PropValue.ToString();
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyValueByEntityId action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetPropertyValueByEntityId(long Id, string PropName, string propertyofAssociation = "")
    {
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            if(!string.IsNullOrEmpty(propertyofAssociation))
            {
                var AssocpropValue = GetFieldValueByAssocationId(Id, PropName, propertyofAssociation);
                return Json(AssocpropValue, JsonRequestBehavior.AllowGet);
            }
            var obj1 = context.T_ExportDataConfigurations.Find(Id);
            if(obj1 == null)
                return Json("0", JsonRequestBehavior.AllowGet);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            PropValue = PropValue == null ? 0 : PropValue;
            return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="PropName"></param>
    /// <param name="propertyofAssociation"></param>
    /// <returns></returns>
    [Audit(0)]
    public string GetFieldValueByAssocationId(long Id, string PropName, string propertyofAssociation)
    {
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration");
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        string AssocpropValue = "";
        if(AssociationInfo != null)
        {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller");
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                ModelReflector.Property propinfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target).Properties.FirstOrDefault(p => p.Name == propertyofAssociation);
                System.Reflection.MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                object[] MethodParams = new object[] { Convert.ToInt64(Id), propertyofAssociation };
                var obj = mc.Invoke(objController, MethodParams);
                if(propinfo.DataType.ToLower() == "datetime")
                    AssocpropValue = Convert.ToDateTime(obj).ToString(propinfo.DisplayFormat.Substring(3).TrimEnd('}'));
                else
                    AssocpropValue = obj.ToString();
            }
        }
        return AssocpropValue;
    }
    /// <summary>Sets property value by entity identifier.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    /// <param name="value">   The value.</param>
    public void SetPropertyValueByEntityId(long Id, string PropName, string value)
    {
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            T_ExportDataConfiguration obj1 = context.T_ExportDataConfigurations.Find(Id);
            if(obj1 != null)
            {
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(q => q.PropertyType.FullName.StartsWith("System")).ToArray();
                //
                string propToWhere = PropName;
                ParameterExpression p = Expression.Parameter(typeof(T_ExportDataConfiguration), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                //LambdaExpression lambda = null;
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    value = GetIdofDisplayValueforSet(PropName, "T_ExportDataConfiguration", value);
                    object propValue = typeConverter.ConvertFromString(value);
                    System.Reflection.PropertyInfo Property1 = properties.FirstOrDefault(r => r.Name == PropName);
                    Property1.SetValue(obj1, propValue, null);
                    context.Entry(obj1).State = EntityState.Modified; //removed due to concurrency error
                    context.SaveChanges();
                }
                else
                {
                    object propValue = typeConverter.ConvertFromString(value);
                    System.Reflection.PropertyInfo Property1 = properties.FirstOrDefault(r => r.Name == PropName);
                    Property1.SetValue(obj1, propValue, null);
                }
                //
                //context.Entry(obj1).State = EntityState.Modified; //removed due to concurrency error
                //context.SaveChanges();
            }
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="PropName"></param>
    /// <param name="EntityName"></param>
    /// <param name="parmValue"></param>
    /// <returns></returns>
    public string GetIdofDisplayValueforSet(string PropName, string EntityName, string parmValue)
    {
        string PropValue = "0";
        string propertyName = PropName.Replace("ID", "").ToLower().Trim();
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(AssociationInfo != null)
        {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller");
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromPropertyValue");
                object[] MethodParams = new object[] { "DisplayValue", parmValue.ToString() };
                var obj = mc.Invoke(objController, MethodParams);
                PropValue = obj.ToString();
            }
            return PropValue.ToString();
        }
        return PropValue.ToString();
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (1M threshold value).</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    [HttpPost]
    public JsonResult Check1MThresholdValue(T_ExportDataConfiguration t_exportdataconfiguration)
    {
        Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
        return Json(msgThreshold, JsonRequestBehavior.AllowGet);
    }
    /// <summary>code for verb action security.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <returns>An array of string.</returns>
    public string[] getVerbsName()
    {
        string[] Verbsarr = new string[] { "BulkUpdate", "BulkDelete", "ImportExcel", "ImportCSV", "ExportExcel", "ExportCSV", "ImportExcelAdvanced", "ImportCSVAdvanced", "Recycle" };
        return Verbsarr;
    }
    
    //instruction feature
    /// <summary>code for list of Instruction label.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <returns>An array of string[].</returns>
    public string[][] getInstructionLabelInfo()
    {
        string[][] labelarr = new string[][] { };
        return labelarr;
    }
    
    /// <summary>code for list of groups.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <returns>An array of string[].</returns>
    public string[][] getGroupsName()
    {
        string[][] groupsarr = new string[][] { new string[] { "186418", "Basic Information" }, new string[] { "186419", "UI Information" } };
        return groupsarr;
    }
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="t_exportdataconfiguration">The Export Data Configuration.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetCalculationValues(T_ExportDataConfiguration t_exportdataconfiguration)
    {
        t_exportdataconfiguration.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_exportdataconfiguration.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_ExportDataConfiguration");
        string DataType = string.Empty;
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_exportdataconfiguration">     The Export Data Configuration.</param>
    /// <param name="IgnoreEditable">(Optional) The ignore editable.</param>
    /// <param name="source">        (Optional) Source.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetails(T_ExportDataConfiguration t_exportdataconfiguration, string IgnoreEditable = null, string source = null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_exportdataconfiguration.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">          The host.</param>
    /// <param name="value">         The value.</param>
    /// <param name="t_exportdataconfiguration">     The Export Data Configuration.</param>
    /// <param name="IgnoreEditable">The ignored editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetailsInline(string host, string value, T_ExportDataConfiguration t_exportdataconfiguration, string IgnoreEditable)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Gets inline associations of entity.</summary>
    ///
    /// <returns>A response stream to send to the getInlineAssociationsOfEntity View.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public ActionResult getInlineAssociationsOfEntity()
    {
        List<string> list = new List<string> { };
        return Json(list, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Gets inline grid associations of entity.</summary>
    ///
    /// <returns>A response stream to send to the getInlineGridAssociationsOfEntity View.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public ActionResult getInlineGridAssociationsOfEntity()
    {
        List<string> list = new List<string> { };
        return Json(list, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Downloads the given FileName (file stored in directory).</summary>
    ///
    /// <param name="FileName">Filename of the file.</param>
    ///
    /// <returns>File.</returns>
    public ActionResult Download(string FileName)
    {
        string filename = FileName;
        string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
        byte[] filedata = System.IO.File.ReadAllBytes(filepath);
        //string contentType = MimeMapping.GetMimeMapping(filepath);
        //var cd = new System.Net.Mime.ContentDisposition
        //{
        //    FileName = filename,
        //    Inline = true,
        //};
        return File(filedata, "application/force-download", Path.GetFileName(FileName));
    }
    
    /// <summary>Bulk Document Download(for download all document of entity and its assocation in zip file).
    ///
    /// </summary>
    /// <param name="Ids"></param>
    [Audit("Bulk Document Download")]
    public async Task<ActionResult> BulkDocumentDownload(string Ids, bool Isflat = false)
    {
        var entityName = "T_ExportDataConfiguration";
        var entityNameDisplay = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName).DisplayName;
        string[] IdsArr = Ids.Split(',');
        if(IdsArr == null)
            return Json(new { path = "", result = "fail" }, JsonRequestBehavior.AllowGet);
        var eIds = IdsArr.Select(s => TryGetInt64(s))
                   .Where(id => id.HasValue)
                   .Select(id => id.Value);
        if(!eIds.Any())
            return Json(new { path = "", result = "fail" }, JsonRequestBehavior.AllowGet);
        var dir = new DownloadDirectory()
        {
            Name = entityNameDisplay
        };
        var t_exportdataconfigurations = await db.T_ExportDataConfigurations.Where(s => eIds.Contains(s.Id)).ToListAsync();
        foreach(var t_exportdataconfiguration in t_exportdataconfigurations)
        {
            await AddDownloadDirectoryForEntity(dir, entityName, t_exportdataconfiguration);
        }
        dir = dir.Children.Count() == 1 ? dir.Children[0] : dir;
        //if (dir.Children.Count == 0 && dir.Parent == null && dir.Files.Count == 0)
        // return Json(new { path = "", result = "fail" }, JsonRequestBehavior.AllowGet);
        var bytes = await ZipDirectory(dir, Isflat);
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.zip", GetProperFileDirName(dir.Name) + "_" + String.Format("{0:MM-dd-yyyy HH-mm-ss-ffff}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), t_exportdataconfigurations.FirstOrDefault().m_Timezone))));
        return File(bytes, "application/zip");
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="HostingEntityName"></param>
    /// <param name="HostingEntityID"></param>
    /// <param name="AssociatedType"></param>
    /// <returns></returns>
    [Audit(0)]
    public ActionResult SetFSearchGrid(string HostingEntityName, string HostingEntityID, string AssociatedType, string viewtype)
    {
        if(!User.CanView("T_ExportDataConfiguration") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewBag.TemplatesName = viewtype;
        //return PartialView("SetFSearchGrid");
        return View();
    }
    
    /// <summary>
    ///
    /// </summary>
    /// <param name="HostingEntityName"></param>
    /// <param name="HostingEntityID"></param>
    /// <param name="AssociatedType"></param>
    /// <param name="viewtype"></param>
    /// <returns></returns>
    [Audit(0)]
    public ActionResult ShowHideColumns(string HostingEntityName, string HostingEntityID, string AssociatedType, string viewtype)
    {
        if(!User.CanAdd("T_ExportDataConfiguration") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["viewtype"] = viewtype;
        return View();
    }
    [Audit(0)]
    public ActionResult ViewPDF(int? id, int? DocumentId, string documentName, string outputFormat, bool? isdownload, bool? ispreview, string forcedOutput = "")
    {
        ViewBag.EntityName = "T_ExportDataConfiguration";
        ViewBag.EntityId = id;
        ViewBag.T_Document = DocumentId;
        ViewBag.T_Name = documentName;
        ViewBag.T_DefaultOutputFormat = outputFormat;
        ViewBag.isdownload = false;
        ViewBag.ispreview = true;
        return View("~/Views/Shared/_ViewPDF.cshtml");
    }
    
    [Audit("Generate Document")]
    public ActionResult GenerateDocument(int? id, int? DocumentId, string documentName, string outputFormat, bool? isdownload, bool? ispreview, string forcedOutput = "")
    {
        T_ExportDataConfiguration t_exportdataconfiguration = null;
        if(db != null)
        {
            t_exportdataconfiguration = db.T_ExportDataConfigurations.AsNoTracking().FirstOrDefault(r => r.Id == id);
            t_exportdataconfiguration.setDateTimeToClientTime();
        }
        documentName = documentName + "_" + t_exportdataconfiguration.DisplayValue + " " + String.Format("{0:MM-dd-yyyy HH-mm}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), t_exportdataconfiguration.m_Timezone));
        var bytedata = new ApplicationContext(new SystemUser()).Documents.AsNoTracking().Where(p => p.Id == DocumentId).Select(p => p.Byte).FirstOrDefault();
        //Custom Validate Before Generate
        var result = ValidateBeforeDocGenerated(t_exportdataconfiguration, documentName, outputFormat, isdownload);
        if(!string.IsNullOrEmpty(result)) return Json(result, JsonRequestBehavior.AllowGet);
        using(Stream stream = new MemoryStream(bytedata))
        {
            if(!string.IsNullOrEmpty(forcedOutput))
                outputFormat = forcedOutput;
            var document = GemBox.Document.DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);
            //OnGenerating - Modify object before generating document
            OnDocGenerating(document, t_exportdataconfiguration, documentName, outputFormat, isdownload);
            document.MailMerge.ClearOptions = GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyRanges | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveUnusedFields | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyParagraphs;
            document.MailMerge.Execute(t_exportdataconfiguration);
            Stream outstream = new MemoryStream();
            if(outputFormat.ToLower() == "docx")
                document.Save(outstream, GemBox.Document.SaveOptions.DocxDefault);
            else document.Save(outstream, GemBox.Document.SaveOptions.PdfDefault);
            //AfterGenerate - Do some stuff after doc generated e.g. attach in an entity or send email
            AfterDocGenerated(t_exportdataconfiguration, documentName, outputFormat, isdownload, outstream);
            bool preview = ispreview.HasValue ? ispreview.Value : false;
            if((isdownload.HasValue && isdownload.Value) && (!preview))
                return File(outstream, System.Net.Mime.MediaTypeNames.Application.Octet, documentName + "." + outputFormat.ToLower());
            else if((isdownload.HasValue && !isdownload.Value) && (preview && outputFormat.ToLower() == "pdf"))
            {
                Response.AddHeader("content-disposition", "inline; filename=" + documentName + "." + outputFormat.ToLower());
                return new FileStreamResult(outstream, "application/pdf");
            }
            else
                return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
    
    
    
}
}

