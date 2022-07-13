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
using ICSharpCode.SharpZipLib.Zip;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Verbs Name actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class VerbsNameController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="verbsname">The Verbs Name.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(VerbsName verbsname, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="verbsname">The Verbs Name.</param>
    private void LoadViewDataAfterOnEdit(VerbsName verbsname)
    {
        LoadViewDataBeforeOnEdit(verbsname, false);
        CustomLoadViewDataListAfterEdit(verbsname);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="verbsname">         The Verbs Name.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(VerbsName verbsname, bool loadCustomViewData = true)
    {
        ViewBag.VerbEnityName = verbsname.verbnameselect.EntityInternalName;
        ViewBag.VerbTypeId = verbsname.verbnameselect.GroupId;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        var _objVerbNameSelect = new List<VerbGroup>();
        _objVerbNameSelect.Add(verbsname.verbnameselect);
        ViewBag.VerbNameSelectID = new SelectList(_objVerbNameSelect, "ID", "DisplayValue", verbsname.VerbNameSelectID);
        if(_objVerbNameSelect.Count == 1)
            ViewBag.VerbNameSelectUIProp = UIPropertyHtmlHelper.UIPropertyHtmlHelper.UIPropertyStyleDropDown(_objVerbNameSelect[0], "Lable");
        var entityName = verbsname.verbnameselect.EntityInternalName;
        var _getVerbListForAll = GetVerbListForAll(entityName).Where(p => p.VerbTypeID == verbsname.verbnameselect.GroupId).ToList();
        var verbObj = new List<VerbInformationDetails>();
        VerbInformationDetails _verbobj = new VerbInformationDetails();
        _verbobj = _getVerbListForAll.Where(v => v.VerbName == verbsname.VerbId && v.VerbTypeID == verbsname.verbnameselect.GroupId).FirstOrDefault();
        verbObj.Add(_verbobj);
        ViewBag.VerbId = new SelectList(verbObj, "VerbName", "Name", verbsname.VerbId);
        ViewBag.VerbsNameIsHiddenRule = checkHidden(User, "VerbsName", "OnEdit", false, null);
        ViewBag.VerbsNameIsGroupsHiddenRule = checkHidden(User, "VerbsName", "OnEdit", true, null);
        ViewBag.VerbsNameIsSetValueUIRule = checkSetValueUIRule(User, "VerbsName", "OnEdit", new long[] { 6, 8 }, null, null);
        ViewBag.VerbsNameRestrictDropdownValueRule = RestrictDropdownValueRule(User, "VerbsName", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var DocumentTemplates = db.T_DocumentTemplates.Where(p => p.T_EntityName == "VerbsName" && !p.T_Disable.Value).OrderBy(p => p.T_DisplayOrder).ToList();
        ViewBag.DocumentTemplates = DocumentTemplates.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        var ExportData = db.T_ExportDataConfigurations.Where(p => p.T_EntityName == "VerbsName" && !p.T_Disable.Value).OrderBy(p => p.Id).ToList();
        ViewBag.ExportDataTemplates = ExportData.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        ViewBag.EntityHelp = db.EntityPages.GetFromCache<IQueryable<EntityPage>, EntityPage>().ToList().Where(p => p.EntityName == "VerbsName" && p.Disable.Value == false).Count() > 0;
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(verbsname);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="verbsname">The Verbs Name.</param>
    private void LoadViewDataAfterOnCreate(VerbsName verbsname)
    {
        ViewBag.VerbNameSelectID = new SelectList(db.VerbGroups.Where(p => p.Id == verbsname.VerbNameSelectID), "ID", "DisplayValue", verbsname.VerbNameSelectID);
        //ViewBag.VerbId = new SelectList(db.VerbNames.Where(p => p.Id == verbsname.VerbId), "ID", "DisplayValue", verbsname.VerbId);
        CustomLoadViewDataListAfterOnCreate(verbsname);
        ViewBag.VerbsNameIsHiddenRule = checkHidden(User, "VerbsName", "OnCreate", false, null);
        ViewBag.VerbsNameIsGroupsHiddenRule = checkHidden(User, "VerbsName", "OnCreate", true, null);
        ViewBag.VerbsNameIsSetValueUIRule = checkSetValueUIRule(User, "VerbsName", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.VerbsNameRestrictDropdownValueRule = RestrictDropdownValueRule(User, "VerbsName", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        ViewBag.VerbEnityName = verbsname.verbnameselect.EntityInternalName;
        ViewBag.VerbTypeId = verbsname.verbnameselect.GroupId;
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType, bool IsBulkUpdate = false)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(HostingEntityName == "VerbGroup" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "VerbNameSelect")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var objList = db.VerbGroups.Where(p => p.Id == hostid).ToList();
            ViewBag.VerbNameSelectID = new SelectList(objList, "ID", "DisplayValue", HostingEntityID);
            ViewBag.VerbEnityName = objList.FirstOrDefault().EntityInternalName;
            ViewBag.VerbTypeId = objList.FirstOrDefault().GroupId;
            ViewBag.DisplayVal = objList.FirstOrDefault().DisplayValue;
        }
        else
        {
            var objVerbNameSelect = new List<VerbGroup>();
            ViewBag.VerbNameSelectID = new SelectList(objVerbNameSelect, "ID", "DisplayValue");
            //
            var filtered_VerbGroup = _fad.FilterDropdown<VerbGroup>(User, db.VerbGroups, "VerbGroup", "VerbNameSelectID");
            if(filtered_VerbGroup != null && filtered_VerbGroup.Count() == 1)
            {
                objVerbNameSelect = filtered_VerbGroup.ToList();
                ViewBag.VerbNameSelectID = new SelectList(objVerbNameSelect, "ID", "DisplayValue", objVerbNameSelect.FirstOrDefault().Id);
            }
        }
        List<VerbInformationDetails> _getVerbListForAll = new List<VerbInformationDetails>();
        ViewBag.VerbId = new SelectList(_getVerbListForAll, "VerbName", "Name");
        if(IsBulkUpdate)
            ViewBag.VerbsNameIsHiddenRule = checkHidden(User, "VerbsName", "OnEdit", false, null);
        else
            ViewBag.VerbsNameIsHiddenRule = checkHidden(User, "VerbsName", "OnCreate", false, null);
        ViewBag.VerbsNameIsGroupsHiddenRule = checkHidden(User, "VerbsName", "OnCreate", true, null);
        ViewBag.VerbsNameIsSetValueUIRule = checkSetValueUIRule(User, "VerbsName", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.VerbsNameRestrictDropdownValueRule = RestrictDropdownValueRule(User, "VerbsName", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        ViewBag.EntityHelp = db.EntityPages.GetFromCache<IQueryable<EntityPage>, EntityPage>().ToList().Where(p => p.EntityName == "VerbsName" && p.Disable.Value == false).Count() > 0;
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    public ActionResult DoExport(VerbsNameIndexViewModel model, IQueryable<VerbsName> _VerbsName)
    {
        if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "VerbsName", User) || !User.CanView("VerbsName"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(model.ExportType == "csv")
        {
            model.pageNumber = 1;
            if(_VerbsName.Count() > 0)
                model.PageSize = _VerbsName.Count();
            var csvdata = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
            csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
            csvdata.ToList().ForEach(fr => fr.ApplyHiddenRule(User.businessrules, "VerbsName"));
            csvdata.ToList().ForEach(fr => fr.ApplyHiddenGroupRule(User.businessrules, "VerbsName"));
            return new CsvResult<VerbsName>(csvdata.ToList(), "Verbs Name.csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] { });
        }
        else
        {
            model.pageNumber = 1;
            if(_VerbsName.Count() > 0)
                model.PageSize = _VerbsName.Count();
            return DownloadExcel(_VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize).ToList());
        }
    }
    public ActionResult DoBulkOperations(VerbsNameIndexViewModel model, IQueryable<VerbsName> _VerbsName, IQueryable<VerbsName> lstVerbsName)
    {
        if(model.BulkOperation != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
        {
            model.TemplatesName = "IndexPartial";
            ViewData["BulkAssociate"] = model.BulkAssociate;
            if(!string.IsNullOrEmpty(model.caller))
            {
                FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                _VerbsName = _fad.FilterDropdown<VerbsName>(User, _VerbsName, "VerbsName", model.caller);
            }
            if(Convert.ToBoolean(model.BulkAssociate))
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = sortRecords(lstVerbsName.Except(_VerbsName), model.sortBy, model.IsAsc).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
                else
                {
                    model.list = lstVerbsName.Except(_VerbsName).OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
            }
            else
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
                else
                {
                    model.list = _VerbsName.OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
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
    public FileResult DownloadExcel(List<GeneratorBase.MVC.Models.VerbsName> query)
    {
        List<string> lstHiddenProp = new List<string>();
        List<string> lstHiddenGroupProp = new List<string>();
        foreach(var item in query)
        {
            item.setDateTimeToClientTime();
            //item.ApplyHiddenRule(User.businessrules, "VerbsName");
            lstHiddenProp = item.ApplyHiddenRule(User.businessrules, "VerbsName");
            lstHiddenGroupProp = item.ApplyHiddenGroupRule(User.businessrules, "VerbsName");
        }
        if(lstHiddenProp.Count > 0)
        {
            var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName").Properties;
            for(int i = 0; i < lstHiddenProp.Count; i++)
            {
                lstHiddenProp[i] = modelproperties.FirstOrDefault(q => q.Name == lstHiddenProp[i]).DisplayName;
            }
        }
        DataTable dt = ExcelExportHelper.ToDataTable<VerbsName>(User, query, "VerbsName");
        dt.AcceptChanges();
        string fileName = "Verbs Name.xlsx";
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            var columns = EntityColumns().Select(p => p.Value).ToArray();
            dt.SetColumnsOrder(columns);
            //dt.RemoveColumns(new string[] { });
            dt.RemoveColumns((lstHiddenProp.Count > 0) ? lstHiddenProp.ToArray() : new string[] { });
            dt.RemoveColumns((lstHiddenGroupProp.Count > 0) ? lstHiddenGroupProp.ToArray() : new string[] { });
            wb.Properties.Author = "KKVerbGroupV3";
            wb.Properties.Title = "Verbs Name";
            wb.Properties.Subject = "Verbs Name data";
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
        var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName").Properties;
        columns.Add("2", modelproperties.FirstOrDefault(q => q.Name == "VerbNameSelectID").DisplayName);
        columns.Add("3", modelproperties.FirstOrDefault(q => q.Name == "VerbId").DisplayName);
        columns.Add("4", modelproperties.FirstOrDefault(q => q.Name == "DisplayOrder").DisplayName);
        columns.Add("5", modelproperties.FirstOrDefault(q => q.Name == "VerbIcon").DisplayName);
        columns.Add("6", modelproperties.FirstOrDefault(q => q.Name == "BackGroundColor").DisplayName);
        columns.Add("7", modelproperties.FirstOrDefault(q => q.Name == "FontColor").DisplayName);
        columns.Add("8", modelproperties.FirstOrDefault(q => q.Name == "VerbId").DisplayName);
        columns.Add("9", modelproperties.FirstOrDefault(q => q.Name == "VerbTypeID").DisplayName);
        columns.Add("10", modelproperties.FirstOrDefault(q => q.Name == "VerbName").DisplayName);
        return columns;
    }
    
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstVerbsName">The list Verbs Name.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<VerbsName> searchRecords(IQueryable<VerbsName> lstVerbsName, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
        {
            lstVerbsName = lstVerbsName.Where(s => (s.verbnameselect != null && (s.verbnameselect.DisplayValue.ToUpper().Contains(searchString))) || (s.DisplayOrder != null && SqlFunctions.StringConvert((double)s.DisplayOrder).Contains(searchString)) || (!String.IsNullOrEmpty(s.VerbIcon) && s.VerbIcon.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.BackGroundColor) && s.BackGroundColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FontColor) && s.FontColor.ToUpper().Contains(searchString)) || (s.VerbTypeID != null && SqlFunctions.StringConvert((double)s.VerbTypeID).Contains(searchString)) || (!String.IsNullOrEmpty(s.VerbName) && s.VerbName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        }
        else
            lstVerbsName = lstVerbsName.Where(s => (s.verbnameselect != null && (s.verbnameselect.DisplayValue.ToUpper().Contains(searchString))) || (s.DisplayOrder != null && SqlFunctions.StringConvert((double)s.DisplayOrder).Contains(searchString)) || (!String.IsNullOrEmpty(s.VerbIcon) && s.VerbIcon.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.BackGroundColor) && s.BackGroundColor.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FontColor) && s.FontColor.ToUpper().Contains(searchString)) || (s.VerbTypeID != null && SqlFunctions.StringConvert((double)s.VerbTypeID).Contains(searchString)) || (!String.IsNullOrEmpty(s.VerbName) && s.VerbName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstVerbsName;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstVerbsName">The IQueryable list Verbs Name.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<VerbsName> sortRecords(IQueryable<VerbsName> lstVerbsName, string sortBy, string isAsc)
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
        if(sortBy == "VerbNameSelectID")
            return isAsc.ToLower() == "asc" ? lstVerbsName.OrderBy(p => p.verbnameselect.DisplayValue) : lstVerbsName.OrderByDescending(p => p.verbnameselect.DisplayValue);
        if(sortBy == "DisplayValue")
            return isAsc.ToLower() == "asc" ? lstVerbsName.OrderBy(p => p.DisplayValue) : lstVerbsName.OrderByDescending(p => p.DisplayValue);
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstVerbsName.Sort(sortBy, true) : lstVerbsName.Sort(sortBy, false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(VerbsName), "verbsname");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<VerbsName>)lstVerbsName.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstVerbsName.ElementType, lambda.Body.Type },
                       lstVerbsName.Expression,
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
    public ActionResult SetFSearch(string searchString, string HostingEntity, string verbnameselect, string verbsnameverbsnametypeassociation, string VerbId, bool? RenderPartial, string FsearchId = "", bool ShowDeleted = false)
    {
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        SetFSearchViewBag("VerbsName");
        ViewBag.HideColumns = new MultiSelectList(EntityColumns(), "Key", "Value");
        ViewBag.FsearchId = FsearchId;
        return View(new VerbsName());
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
    public ActionResult FSearch(VerbsNameIndexArgsOption args)
    {
        args.EntityName = "VerbsName";
        //FSearchViewBag(currentFilter, searchString, FSFilter, sortBy, isAsc, page, itemsPerPage, search, FilterCondition, HostingEntity, AssociatedType, HostingEntityID, viewtype, SortOrder, HideColumns, GroupByColumn, IsReports, IsdrivedTab, IsFilter,"VerbsName");
        IndexViewBag(args);
        VerbsNameIndexViewModel model = new VerbsNameIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, Convert.ToInt32(model.HostingEntityID), model.AssociatedType);
        var lstVerbsName = from s in db.VerbsNames
                           select s;
        // for Restrict Dropdown
        ViewBag.VerbsNameRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "VerbsName", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstVerbsName = searchRecords(lstVerbsName, model.searchString.ToUpper(), true);
        }
        if(!string.IsNullOrEmpty(model.search))
            model.search = model.search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(model.search))
        {
            model.SearchResult += "\r\n General Criterial= " + model.search + ",";
            lstVerbsName = searchRecords(lstVerbsName, model.search, true);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstVerbsName = sortRecords(lstVerbsName, model.sortBy, model.IsAsc);
        }
        else lstVerbsName = lstVerbsName.OrderByDescending(c => c.Id);
        lstVerbsName = CustomSorting(lstVerbsName, model.HostingEntity, model.AssociatedType, model.sortBy, model.IsAsc);
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
                model.SearchResult += " " + GetPropertyDP("VerbsName", PropertyName) + " " + Operator + " " + Value.Trim() + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch("VerbsName", PropertyName, Operator, Value.Trim(), type) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstVerbsName = lstVerbsName.Where(whereCondition.ToString());
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
            lstVerbsName = Sorting.Sort<VerbsName>(lstVerbsName, DataOrdering);
        var _VerbsName = lstVerbsName;
        if(model.DisplayOrderFrom != null || model.DisplayOrderTo != null)
        {
            try
            {
                int from = model.DisplayOrderFrom == null ? 0 : Convert.ToInt32(model.DisplayOrderFrom);
                int to = model.DisplayOrderTo == null ? int.MaxValue : Convert.ToInt32(model.DisplayOrderTo);
                _VerbsName = _VerbsName.Where(o => o.DisplayOrder >= from && o.DisplayOrder <= to);
                model.SearchResult += "\r\n Display Order= " + from + "-" + to;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(model.VerbTypeIDFrom != null || model.VerbTypeIDTo != null)
        {
            try
            {
                int from = model.VerbTypeIDFrom == null ? 0 : Convert.ToInt32(model.VerbTypeIDFrom);
                int to = model.VerbTypeIDTo == null ? int.MaxValue : Convert.ToInt32(model.VerbTypeIDTo);
                _VerbsName = _VerbsName.Where(o => o.VerbTypeID >= from && o.VerbTypeID <= to);
                model.SearchResult += "\r\n VerbTypeID= " + from + "-" + to;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        //if (lstVerbsName.Where(p => p.verbnameselect != null).Count() <= 50)
        //ViewBag.verbnameselect = new SelectList(lstVerbsName.Where(p => p.verbnameselect != null).Select(P => P.verbnameselect).Distinct(), "ID", "DisplayValue");
        if(model.verbnameselect != null)
        {
            var ids = model.verbnameselect.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            model.SearchResult += "\r\n Group Name= ";
            foreach(var str in ids)
            {
                //Null Search
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        model.SearchResult += "";
                    }
                    else if(str == "0")
                    {
                        ids1.Add(Convert.ToInt64(str));
                        model.SearchResult += "LoggedInUser, ";
                    }
                    else
                    {
                        ids1.Add(Convert.ToInt64(str));
                        var obj = db.VerbGroups.Find(Convert.ToInt64(str));
                        model.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            _VerbsName = _VerbsName.Where(p => ids1.Contains(p.VerbNameSelectID));
        }
        _VerbsName = FilterByHostingEntity(_VerbsName, model.HostingEntity, model.AssociatedType, Convert.ToInt32(model.HostingEntityID));
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
                var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName");
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
                        result = _VerbsName.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it." + aggregate + "(Value) as Y)").OrderBy("X");
                    else
                        result = _VerbsName.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it." + aggregate + "() as Y)").OrderBy("X");
                else if(aggregate != "Count")
                    result = _VerbsName.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "(Value) as Y)").OrderBy("X");
                else
                    result = _VerbsName.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "() as Y)").OrderBy("X");
                var groupbyresult = result.ToListAsync().Result;
                return Json(new { type = "groupby", result = groupbyresult, dataType = dataType, displayFormat = DisplayFormat }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            if(aggregate != "Count")
            {
                var result = _VerbsName.Aggration(aggregateproperty, aggregate);
                var entityName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName");
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
            return Json(_VerbsName.Count(), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if((string)model.SearchResult != null)
            model.SearchResult = ((string)model.SearchResult).TrimStart("\r\n".ToCharArray()).TrimEnd(", ".ToCharArray()).TrimEnd(",".ToCharArray());
        model = (VerbsNameIndexViewModel)SetPagination(model, "VerbsName");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _VerbsName.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        //
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _VerbsName);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _VerbsName.Count();
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
            var list = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityVerbsNameDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["VerbsNamelist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "VerbsName", tagsSplit.ToArray());
                }
            model.list = list;
            return View("Index", model);
        }
        else
        {
            var list = _VerbsName.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityVerbsNameDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["VerbsNamelist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "VerbsName", tagsSplit.ToArray());
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
    /// <param name="_VerbsName">IQueryable<VerbsName>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<VerbsName>.</returns>
    private IQueryable<VerbsName> FilterByHostingEntity(IQueryable<VerbsName> _VerbsName, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        if(HostingEntity == "VerbGroup" && AssociatedType == "VerbNameSelect")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _VerbsName = _VerbsName.Where(p => p.VerbNameSelectID == hostid);
                ViewBag.HostingEntityIDData = db.VerbGroups.FirstOrDefault(fd => fd.Id == hostid);
            }
            else
                _VerbsName = _VerbsName.Where(p => p.VerbNameSelectID == null);
        }
        return _VerbsName;
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
            var _Obj = appcontext.VerbsNames.FirstOrDefault(p => p.Id == idvalue);
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
            IQueryable<VerbsName> list = appcontext.VerbsNames;
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<VerbsName>(new SystemUser(), list, "VerbsName", caller);
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
    public IQueryable<VerbsName> GetIQueryable(ApplicationContext appdb)
    {
        return appdb.VerbsNames.AsNoTracking();
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
        listjournaliquery = new FilteredDbSet<JournalEntry>(jedb, predicateJournalEntry);
        return listjournaliquery;
    }
    
    /// <summary>Gets record by identifier.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The record by identifier.</returns>
    public object GetDetailsRecordById(string id, ApplicationContext appcontext)
    {
        if(string.IsNullOrEmpty(id)) return "";
        var dataid = Convert.ToInt64(id);
        var _Obj = appcontext.VerbsNames.Where(fd => fd.Id == dataid).ToList().FirstOrDefault();
        return _Obj;
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
            var _Obj = appcontext.VerbsNames.Find(Convert.ToInt64(id));
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
            var list = appcontext.VerbsNames.Where(p => RecordIds.Contains(p.Id));
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
        if(string.IsNullOrEmpty(id)) return Json(new VerbsName(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.VerbsNames.Find(Convert.ToInt64(id));
            long? tenantId = null;
            var _updatedAssocObj = UpdateAssociationValue("VerbsName", _Obj, tenantId);
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
            var _Obj = context.VerbsNames.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<VerbsName>(_Obj, "VerbsName", new string[] { "" });
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
        IQueryable<VerbsName> list = db.VerbsNames;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<VerbsName>(User, list, "VerbsName", caller);
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
        var list = db.VerbsNames.OrderBy(p => p.DisplayValue).AsQueryable();//;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<VerbsName>(User, list, "VerbsName", associationName + "ID", RestrictDropdownVal);
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
        var list = CustomDropdownFilter(db.VerbsNames.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, ExtraVal, RestrictDropdownVal, CustomParameter, "GetAllValue");
        var result = DropdownHelper.GetAllValue<VerbsName>(User, list, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "VerbsName", RestrictDropdownVal);
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
        var result = DropdownHelper.GetAllValueForRB<VerbsName>(User, db.VerbsNames, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "VerbsName");
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
        IQueryable<VerbsName> list = db.VerbsNames;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            var result = list.Select("new(Id," + propNameBR + " as value)");
            if(propNameBR.ToLower() != "displayvalue")
            {
                var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName");
                var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propNameBR);
                if(AssociationInfo != null && AssociationInfo.Target == "IdentityUser")
                {
                    result = list.Select("new(Id," + propNameBR.ToLower().Remove(propNameBR.Length - 2) + ".UserName as value)");
                    return Json(new { result = result, IsUserDropDown = true }, JsonRequestBehavior.AllowGet);
                }
            }
            if(propNameBR.ToLower() == "displayvalue")
            {
                result = list.Select("new(Id,DisplayValue as value)");
            }
            return Json(new { result = result, IsUserDropDown = false }, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(new { result = data, IsUserDropDown = false }, JsonRequestBehavior.AllowGet);
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
    public JsonResult GetAllMultiSelectValue(string caller, string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string CustomParameter = null, string ExtraVal = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.VerbsNames.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, ExtraVal, null, CustomParameter, "GetAllMultiSelectValue");
        var result = DropdownHelper.GetAllMultiSelectValue(User, list, key, AssoNameWithParent, AssociationID, "VerbsName", caller, ExtraVal);
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
    [Audit(0)]
    public JsonResult GetAllValueForVerb(string caller, string key, string ExtraVal = "", string VerbEntityName = "", string VerbTypeID = "")
    {
        if(!(string.IsNullOrEmpty(VerbEntityName) && string.IsNullOrEmpty(VerbTypeID)))
        {
            long typeId = Convert.ToInt64(VerbTypeID);
            var result = GetVerbListForAll(VerbEntityName).Where(p => p.VerbTypeID == typeId);
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                var val = ExtraVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(wh => string.IsNullOrWhiteSpace(wh)).ToList();
                result = result.Where(p => !(val.Contains(p.VerbName))).Union(result.Where(p => val.Contains(p.VerbName))).Distinct();
            }
            return Json(result.Select(x => new
            {
                Id = x.VerbName,
                Name = x.Name
            }), JsonRequestBehavior.AllowGet);
        }
        return null;
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
    public JsonResult GetMultiSelectValueAllSelection(string caller, string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string bulkSelection = null, string CustomParameter = null, string ExtraVal = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.VerbsNames.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, ExtraVal, null, CustomParameter, "GetMultiSelectValueAllSelection");
        var result = DropdownHelper.GetMultiSelectValueAllSelection(User, list, key, AssoNameWithParent, AssociationID, "VerbsName", caller, ExtraVal);
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
        case "VerbNameSelectID":
            columnValue = db.VerbGroups.FirstOrDefault(p => p.Id == id).DisplayValue;
            break;
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
            if(!((CustomPrincipal)User).CanUseVerb("ImportCSV", "VerbsName", User) || !User.CanAdd("VerbsName"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else if(FileType.ToLower() == "xls" || FileType.ToLower() == "xlsx")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "VerbsName", User) || !User.CanAdd("VerbsName"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "VerbsName")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "VerbsName").ToList();
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
        string typeName = "VerbsName";
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
                    allcolumns = new string[] { "VerbNameSelectID", "VerbId", "DisplayOrder", "VerbIcon", "BackGroundColor", "FontColor", "VerbId", "VerbTypeID", "VerbName" };
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
                        var defkeyDispName = defcol.Key.DisplayName.Trim().ToLower();
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
                                colSelected = defRec.Count() > 0 ? defRec.FirstOrDefault().SheetColumn.ToLower() : null;
                                defkeyDispName = colSelected;
                                colDisKey = colDisKey + entName + "$" + dispName + ",";
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
                                    propDetail = entityModel.Properties.FirstOrDefault(p => p.Name.ToLower() == adventName[0].ToLower());
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
                                        defkeyDispName = propDetail.DisplayName.ToLower();
                                    colExist = 1;
                                }
                            if(colExist == 0)
                                ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
                            if(AssociatedType != null && colExist == 1 && defcol.Key.Name == propDetail.Name)
                            {
                                var defval = "";
                                var defcolint = defcol.Value.Where(s => s.Text.ToLower().Trim().Equals(defkeyDispName)).FirstOrDefault();
                                if(defcolint != null)
                                    defval = defcolint.Value.ToString();
                                else
                                    ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
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
                        ViewBag.DefaultMappingMsg += "<br /><br /> Error Details: <br /> The following columns are missing : Table Column - SheetColumn " + ExistsColumnMappingName;
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
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Verbs Name</b>";
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
                                case "VerbNameSelectID":
                                    var verbnameselectId = db.VerbGroups.FirstOrDefault(p => p.DisplayValue == assovalue);
                                    if(verbnameselectId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
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
                            ViewBag.chkUpdateList = string.Join(",", DefaultMapping.ToList().Where(q => !string.IsNullOrEmpty(q.UpdateColumn)).Select(p => p.UpdateColumn).ToList());
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
    public ActionResult ConfirmImportData(FormCollection collection)
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
        string typename = "VerbsName";
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        var Updatefields = chkUpdateList.Split(',');
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
                    if(Updatefields.Contains(tblInternalNames.Split(',')[i]))
                    {
                        objImtConfig.UpdateColumn = tblInternalNames.Split(',')[i];
                    }
                    if(!string.IsNullOrEmpty(ImportType))
                    {
                        var splitedcol = tblInternalNames.Split(',');
                        var ischildentity = splitedcol[i].Split('$').Length > 1 ? true : false;
                        if(ischildentity)
                            objImtConfig.TableColumn = splitedcol[i];
                        if(Updatefields.ToList().Any(p => "VerbsName$" + p == objImtConfig.TableColumn))
                            objImtConfig.UpdateColumn = Updatefields.ToList().FirstOrDefault(p => "VerbsName$" + p == objImtConfig.TableColumn);
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
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Verbs Name</b>";
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
                        case "VerbNameSelectID":
                            var strPropertyVerbNameSelect = lstEntityProp.FirstOrDefault(p => p.Key == "VerbNameSelectID").Value;
                            ModelReflector.Property propVerbNameSelect = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbGroup").Properties.FirstOrDefault(p => p.DisplayName == strPropertyVerbNameSelect);
                            var verbnameselectId = db.VerbGroups.Where(propVerbNameSelect.Name + "=(@0)", assovalue).FirstOrDefault();
                            //var verbnameselectId = db.VerbGroups.Where(propVerbNameSelect.Name+"=\""+assovalue+"\"").FirstOrDefault();
                            if(verbnameselectId == null)
                                uniqueassoValues.Add(assovalue);
                            break;
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
    public ActionResult ImportData(FormCollection collection)
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
                    VerbsName model = new VerbsName();
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
                        case "DisplayOrder":
                            model.DisplayOrder = Int32.Parse(columnValue);
                            break;
                        case "VerbIcon":
                            model.VerbIcon = columnValue;
                            break;
                        case "BackGroundColor":
                            model.BackGroundColor = columnValue;
                            break;
                        case "FontColor":
                            model.FontColor = columnValue;
                            break;
                        case "VerbId":
                            model.VerbId = columnValue;
                            break;
                        case "VerbTypeID":
                            model.VerbTypeID = Int32.Parse(columnValue);
                            break;
                        case "VerbName":
                            model.VerbName = columnValue;
                            break;
                        case "VerbNameSelectID":
                            dynamic verbnameselectId = null;
                            if(AssociatedType != null && AssociatedType == "VerbNameSelect" && HostingEntityID != null)
                            {
                                long id = Convert.ToInt64(HostingEntityID);
                                model.VerbNameSelectID = id;
                            }
                            else
                            {
                                if(lstEntityProp.Count > 0)
                                {
                                    var strPropertyVerbNameSelect = lstEntityProp.FirstOrDefault(p => p.Key == "VerbNameSelectID").Value;
                                    ModelReflector.Property propVerbNameSelect = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbGroup").Properties.FirstOrDefault(p => p.DisplayName == strPropertyVerbNameSelect);
                                    if(propVerbNameSelect.Name == "DisplayValue")
                                        verbnameselectId = db.VerbGroups.AsNoTracking().FirstOrDefault(p => p.DisplayValue == columnValue);
                                    else
                                        verbnameselectId = db.VerbGroups.AsNoTracking().Where(propVerbNameSelect.Name + "=\"" + columnValue + "\"").FirstOrDefault();
                                }
                                else
                                    verbnameselectId = db.VerbGroups.AsNoTracking().FirstOrDefault(p => p.DisplayValue == columnValue);
                            }
                            if(verbnameselectId != null)
                                model.VerbNameSelectID = verbnameselectId.Id;
                            else
                            {
                                if((collection["VerbNameSelectID"] != null) && (collection["VerbNameSelectID"].ToString() == "true,false"))
                                {
                                    try
                                    {
                                        VerbGroup objVerbGroup = new VerbGroup();
                                        objVerbGroup.Name = (columnValue);
                                        db.VerbGroups.Add(objVerbGroup);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                            db.Entry(objVerbGroup).State = EntityState.Detached;
                                            continue;
                                        }
                                        model.VerbNameSelectID = objVerbGroup.Id;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                        }
                    }
                    // Columns to update
                    var MatchColumns = collection["hdnListChkUpdate"];
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, "VerbsName", mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User, db, model, "VerbsName");
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", "VerbsName");
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                                //if (result == null || result.Count == 0)
                            {
                                var customerror = "";
                                if(!CustomSaveOnImport(model, out customerror, i))
                                {
                                    if(!flagUpdate)
                                    {
                                        db.VerbsNames.Add(model);
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
    public ActionResult ImportDataAdvanced(FormCollection collection)
    {
        string MainEntityName = "VerbsName";
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
        var dbnew = new ApplicationContext(User, 0);
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
                    VerbsName model = new VerbsName();
                    var tblColumns = lstMaped;
                    //
                    var MatchColumns = collection["hdnListChkUpdate"];
                    int mapCount = 0;
                    mapCount = MatchColumns.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().Count();
                    model = LoadObjectFromSheet(model, sheetColumns, tblColumns, i, objDataSet, AssociatedType, HostingEntityID, lstEntityProp, mappedColumns, MainEntityName, dbnew, mapCount) as VerbsName;
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, dbnew, MainEntityName, mappedColumns, true);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model as VerbsName, "ImportData");
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
                                if(!CustomSaveOnImport(model as VerbsName, out customerror, i))
                                {
                                    if(!flagUpdate)
                                    {
                                        dbnew.VerbsNames.Add(model);
                                        try
                                        {
                                            dbnew.SaveChanges();
                                        }
                                        catch(System.Data.Entity.Validation.DbEntityValidationException e)
                                        {
                                            dbnew.Entry(model).State = EntityState.Detached;
                                            StringBuilder sb = new StringBuilder();
                                            foreach(var eve in e.EntityValidationErrors)
                                            {
                                                sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                                            eve.Entry.Entity.GetType().Name,
                                                                            eve.Entry.State));
                                                foreach(var ve in eve.ValidationErrors)
                                                {
                                                    sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                                                ve.PropertyName,
                                                                                ve.ErrorMessage));
                                                }
                                            }
                                            if(ViewBag.ImportError == null)
                                                ViewBag.ImportError += sb.ToString();
                                            else
                                                ViewBag.ImportError += sb.ToString();
                                            continue;
                                        }
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
                var obj1 = appcontext.VerbsNames.Where(p => p.Id == Id).FirstOrDefault();
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
    /// <param name="verbsname">The Verbs Name.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleOnSaving action.</returns>
    private JsonResult ApplyBusinessRuleOnSaving(VerbsName verbsname)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "VerbsName").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            //var ruleids = businessrule.Select(q => q.Id).ToList();
            //var typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(User, db, verbsname, "OnEdit", "VerbsName");
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
                var resultBR = GetMandatoryPropertiesDictionary(User, db, verbsname, "OnEdit", "VerbsName");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(!dic.Key.Contains("FailureMessage"))
                        {
                            var type = verbsname.GetType();
                            if(type.GetProperty(dic.Key) != null)
                            {
                                var propertyvalue = type.GetProperty(dic.Key).GetValue(verbsname, null);
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
                var resultBR = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, verbsname, "OnEdit", "VerbsName");
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
    /// <param name="verbsname">       The Verbs Name.</param>
    /// <param name="IsReadOnlyIgnore">(Optional) True if is read only ignored, false if not.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBefore action.</returns>
    private JsonResult ApplyBusinessRuleBefore(VerbsName verbsname, bool IsReadOnlyIgnore = false)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "VerbsName").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(User, db, verbsname, "VerbsName").Where(p => p.Key.Contains("FailureMessage"));
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
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User, db, verbsname, "VerbsName").Where(p => p.Key.Contains("FailureMessage"));
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
    [HttpPost]
    [Audit(0)]
    public JsonResult ResetToDefaultField(long id, string fieldName)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(id > 0)
        {
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.VerbsNames.Find(id);
                if(obj1 != null)
                {
                    System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == fieldName);
                    var propertyInfo = obj1.GetType().GetProperty(fieldName);
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
                            hiddenProperties.Add(fieldName, Convert.ToString(safeValue));
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        else
        {
            hiddenProperties.Add(fieldName, Convert.ToString(""));
        }
        return Json(hiddenProperties, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Audit(0)]
    public JsonResult ResetToDefault(long id, string groupName)
    {
        var proplstgroupby = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName").Properties.Where(p => p.Proptype.ToLower() == "group").Where(p => ("VerbsName" + p.GroupInternalName) == groupName);
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(id > 0)
        {
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.VerbsNames.Find(id);
                if(obj1 != null)
                {
                    foreach(var item in proplstgroupby)
                    {
                        System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == item.Name);
                        var propertyInfo = obj1.GetType().GetProperty(item.Name);
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
                                hiddenProperties.Add(item.Name, Convert.ToString(safeValue));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach(var item in proplstgroupby)
            {
                hiddenProperties.Add(item.Name, Convert.ToString(""));
            }
        }
        return Json(hiddenProperties, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Readonly business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    /// <param name="ruleType">page from.</param>
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetReadOnlyProperties(VerbsName OModel, string ruleType = null)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
        return Json(GetReadOnlyPropertiesDictionary(User, db, OModel, "VerbsName", ruleType), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON (Mandatory business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetMandatoryProperties(VerbsName OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
        return Json(new { dict = GetMandatoryPropertiesDictionary(User, db, OModel, ruleType, "VerbsName"), template = getBRTemplate(2) }, JsonRequestBehavior.AllowGet);
        //return Json(GetMandatoryPropertiesDictionary(User,db,OModel,ruleType,"VerbsName"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetUIAlertBusinessRules(VerbsName OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "VerbsName").ToList();
            var BRAll = BR;
            if(ruleType == "OnCreate")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
            else if(ruleType == "OnEdit")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "VerbsName");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 13);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(13))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        // RulesApplied.Add("<span style=\"color:grey;font-size:11px;\">Warning(#" + rules.Value.BRID + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        RulesApplied.Add(rules.Value.BRID.ToString(), conditions.Trim().TrimEnd(",".ToCharArray()));
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
        return Json(new { dict = RulesApplied, JsonRequestBehavior.AllowGet, template = getBRTemplate(13) });
        //return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    /// <summary>UpdateHiddenProperties.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="cannotview">Type of the cannotview.</param>
    ///
    /// <returns>VerbsName object.</returns>
    public VerbsName UpdateHiddenProperties(VerbsName OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.VerbsNames.Find(OModel.Id);
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
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetHiddenVerb action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetHiddenVerb(VerbsName OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "VerbsName").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.GetHiddenVerb(OModel, BR, "VerbsName");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                //var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 16);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(16))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        using(var ruleactionArgdb = new ActionArgsContext())
                        {
                            var args = ruleactionArgdb.ActionArgss.Where(p => p.ActionArgumentsID == rules.Value.ActionID).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                            foreach(var arg in args)
                            {
                                RulesApplied.Add(arg.Id.ToString(), arg.ParameterValue.ToString());
                            }
                        }
                    }
                }
            }
        }
        return Json(RulesApplied, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetHiddenVerb action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public Dictionary<string, string> GetHiddenVerbDetails(VerbsName OModel, string ruleType, string pagetype = null)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "VerbsName").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.GetHiddenVerb(OModel, BR, "VerbsName");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                //var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 16);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(16))
                {
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        using(var ruleactionArgdb = new ActionArgsContext())
                        {
                            var args = ruleactionArgdb.ActionArgss.Where(p => p.ActionArgumentsID == rules.Value.ActionID).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                            foreach(var arg in args)
                            {
                                RulesApplied.Add(arg.Id.ToString(), arg.ParameterValue.ToString());
                            }
                        }
                    }
                }
            }
        }
        return RulesApplied;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSaveProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetValidateBeforeSaveProperties(VerbsName OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
        return Json(new { dict = GetValidateBeforeSavePropertiesDictionary(User, db, OModel, ruleType, "VerbsName"), template = getBRTemplate(10) }, JsonRequestBehavior.AllowGet);
        //return Json(GetValidateBeforeSavePropertiesDictionary(User,db,OModel,ruleType,"VerbsName"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSavePropertiesForPopupConfirm action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetValidateBeforeSavePropertiesForPopupConfirm(VerbsName OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
        return Json(new { dict = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "VerbsName"), template = getBRTemplate(15) }, JsonRequestBehavior.AllowGet);
        //return Json(GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "VerbsName"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Lock record business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    
    public JsonResult GetLockBusinessRules(VerbsName OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("VerbsName")));
        return Json(new { dict = GetLockBusinessRulesDictionary(User, db, OModel, "VerbsName"), template = getBRTemplate(1) }, JsonRequestBehavior.AllowGet);
        //return Json(GetLockBusinessRulesDictionary(User,db,OModel,"VerbsName"), JsonRequestBehavior.AllowGet);
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
    public long? GetIdFromDisplayValue(string displayvalue)
    {
        if(string.IsNullOrEmpty(displayvalue)) return 0;
        using(ApplicationContext db1 = new ApplicationContext(new SystemUser()))
        {
            db1.Configuration.LazyLoadingEnabled = false;
            var _Obj = db1.VerbsNames.FirstOrDefault(p => p.DisplayValue == displayvalue);
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
            IQueryable query = context.VerbsNames;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(VerbsName), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if(PropValue.ToLower().Trim() != "null")
            {
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    PropValue = GetIdofDisplayValueforSetIfZero(PropName, "VerbsName", PropValue);
                    System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                    object PropValue1 = typeConverter.ConvertFromString(PropValue);
                    lambda = Expression.Lambda<Func<VerbsName, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue1), member.Type)), p);
                }
                else
                {
                    lambda = Expression.Lambda<Func<VerbsName, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
                }
            }
            else
                lambda = Expression.Lambda<Func<VerbsName, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<VerbsName>)q);
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
            var obj1 = context.VerbsNames.Find(Id);
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
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName");
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
            VerbsName obj1 = context.VerbsNames.Find(Id);
            if(obj1 != null)
            {
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(q => q.PropertyType.FullName.StartsWith("System")).ToArray();
                //
                string propToWhere = PropName;
                ParameterExpression p = Expression.Parameter(typeof(VerbsName), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                //LambdaExpression lambda = null;
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    value = GetIdofDisplayValueforSet(PropName, "VerbsName", value);
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
    /// <param name="verbsname">The Verbs Name.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult Check1MThresholdValue(VerbsName verbsname)
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
        string[] Verbsarr = new string[] { "BulkUpdate", "BulkDelete", "ImportExcel", "ImportCSV", "ExportExcel", "ExportCSV", "ImportExcelAdvanced", "ImportCSVAdvanced" };
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
        string[][] groupsarr = new string[][] { new string[] { "324066", "Basic Information" }, new string[] { "324067", "UI Information" }, new string[] { "324068", "Internal Use Only" } };
        return groupsarr;
    }
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="verbsname">The Verbs Name.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetCalculationValues(VerbsName verbsname)
    {
        verbsname.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (verbsname.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "VerbsName");
        string DataType = string.Empty;
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="verbsname">     The Verbs Name.</param>
    /// <param name="IgnoreEditable">(Optional) The ignore editable.</param>
    /// <param name="source">        (Optional) Source.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetails(VerbsName verbsname, string IgnoreEditable = null, string source = null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (verbsname.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">          The host.</param>
    /// <param name="value">         The value.</param>
    /// <param name="verbsname">     The Verbs Name.</param>
    /// <param name="IgnoreEditable">The ignored editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetailsInline(string host, string value, VerbsName verbsname, string IgnoreEditable)
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
        var entityName = "VerbsName";
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
        var verbsnames = await db.VerbsNames.Where(s => eIds.Contains(s.Id)).ToListAsync();
        var derivedProp = new Dictionary<string, string>();
        foreach(var verbsname in verbsnames)
        {
            await AddDownloadDirectoryForEntity(dir, entityName, verbsname, derivedProp);
        }
        dir = dir.Children.Count() == 1 ? dir.Children[0] : dir;
        //if (dir.Children.Count == 0 && dir.Parent == null && dir.Files.Count == 0)
        // return Json(new { path = "", result = "fail" }, JsonRequestBehavior.AllowGet);
        var bytes = await ZipDirectory(dir, Isflat);
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.zip", GetProperFileDirName(dir.Name) + "_" + String.Format("{0:MM-dd-yyyy HH-mm-ss-ffff}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), verbsnames.FirstOrDefault().m_Timezone))));
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
        if(!User.CanView("VerbsName") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
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
        if(!User.CanView("VerbsName") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
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
        ViewBag.EntityName = "VerbsName";
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
        VerbsName verbsname = null;
        if(db != null)
        {
            verbsname = db.VerbsNames.AsNoTracking().FirstOrDefault(r => r.Id == id);
            verbsname.setDateTimeToClientTime();
        }
        documentName = documentName + "_" + verbsname.DisplayValue + " " + String.Format("{0:MM-dd-yyyy HH-mm}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), verbsname.m_Timezone));
        var bytedata = new ApplicationContext(new SystemUser()).Documents.AsNoTracking().Where(p => p.Id == DocumentId).Select(p => p.Byte).FirstOrDefault();
        //Custom Validate Before Generate
        var result = ValidateBeforeDocGenerated(verbsname, documentName, outputFormat, isdownload);
        if(!string.IsNullOrEmpty(result)) return Json(result, JsonRequestBehavior.AllowGet);
        using(Stream stream = new MemoryStream(bytedata))
        {
            if(!string.IsNullOrEmpty(forcedOutput))
                outputFormat = forcedOutput;
            var document = GemBox.Document.DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);
            //OnGenerating - Modify object before generating document
            OnDocGenerating(document, verbsname, documentName, outputFormat, isdownload);
            document.MailMerge.ClearOptions = GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyRanges | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveUnusedFields | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyParagraphs;
            document.MailMerge.Execute(verbsname);
            Stream outstream = new MemoryStream();
            bool preview = ispreview.HasValue ? ispreview.Value : false;
            if(outputFormat.ToLower() == "docx")
            {
                document.Save(outstream, GemBox.Document.SaveOptions.DocxDefault);
                preview = false;
                isdownload = true;
            }
            else document.Save(outstream, GemBox.Document.SaveOptions.PdfDefault);
            //AfterGenerate - Do some stuff after doc generated e.g. attach in an entity or send email
            AfterDocGenerated(verbsname, documentName, outputFormat, isdownload, outstream);
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
    
    public ActionResult ExportDataNote(int btnid, string Ids)
    {
        var model = db.T_ExportDataConfigurations.Find(btnid);
        ViewBag.Ids = Ids;
        ViewBag.EntityName = "VerbsName";
        return View("~/Views/Shared/_ExportDataNote.cshtml", model);
    }
    
    public List<ExportDataKey> visitedList = new List<ExportDataKey>();
    public List<ExportDataKey> KeysData = new List<ExportDataKey>();
    public List<ExportDataDeleteSet> DeleteSets = new List<ExportDataDeleteSet>();
    public async Task<ActionResult> ExportData(string Ids, int btnid, string Notes)
    {
        var lstIds = Ids.Split(',').Where(wh => !string.IsNullOrEmpty(wh)).Select(int.Parse).ToList();
        var IsBulk = lstIds.Count > 1 ? true : false;
        var entities = ModelReflector.Entities;
        var BulkfolderPath = string.Empty;
        var zipvalue = string.Empty;
        var Bulkzipvalue = string.Empty;
        var datalogList = new List<T_ExportDataLog>();
        var objExportDataConfiguration = db.T_ExportDataConfigurations.FirstOrDefault(fd => fd.Id == btnid);
        var logstatusId = db.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Open").Id;
        if(IsBulk)
        {
            BulkfolderPath = Path.Combine(Server.MapPath("~/Files/" + entities.FirstOrDefault(fd => fd.Name == "VerbsName").DisplayName + "BulkExport_" + DateTime.UtcNow.Ticks));
            Bulkzipvalue = entities.FirstOrDefault(fd => fd.Name == "VerbsName").DisplayName + "-BulkExport-" + DateTime.UtcNow.ToString("dd-MM-yyyy hh-mm-ss") + " UTC";
        }
        foreach(var id in lstIds)
        {
            var zipdata = db.VerbsNames.Find(id);
            zipvalue = entities.FirstOrDefault(fd => fd.Name == "VerbsName").DisplayName + "-" + zipdata.DisplayValue + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy hh-mm-ss") + " UTC";
            var IsSoftDeleteEnabled = false;
            var IsRootItem = false;
            var datacntlog = new StringBuilder();
            KeysData = ExportDataHelper.KeysList(objExportDataConfiguration.t_exportdataconfigurationexportdatadetailsassociation.ToList());
            string folderPath = string.Empty;
            if(IsBulk)
            {
                folderPath = Path.Combine(BulkfolderPath, zipdata.DisplayValue);
            }
            else
            {
                folderPath = Path.Combine(Server.MapPath("~/Files/" + entities.FirstOrDefault(fd => fd.Name == "VerbsName").DisplayName + "_" + DateTime.UtcNow.Ticks));
                BulkfolderPath = folderPath;
            }
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            foreach(var item in KeysData)
            {
                if(visitedList.Contains(item)) continue;
                var rootlist = db.VerbsNames.Where(wh => wh.Id == id);
                var ds = new DataSet();
                var lstcondition = new Dictionary<string, string>();
                IQueryable datalist = null;
                if(item.Hierarchy.Count > 1)
                {
                    int icnt = 0;
                    foreach(var asso in item.Hierarchy)
                    {
                        if(icnt == 0)
                            datalist = rootlist.SelectMany(asso);
                        else
                            datalist = datalist.SelectMany(asso);
                        icnt++;
                    }
                }
                else
                    datalist = rootlist.SelectMany(item.AssociationName);
                datalist = filtercondtion(datalist, item.Id, item.ChildEntity);
                var entitydp = entities.FirstOrDefault(fd => fd.Name == item.ChildEntity).DisplayName;
                var childfolderPath = Path.Combine(folderPath, entitydp);
                if(!Directory.Exists(childfolderPath))
                {
                    Directory.CreateDirectory(childfolderPath);
                }
                var IsSelf = item.AssociationName.StartsWith("Self");
                var IsEnableDelete = objExportDataConfiguration.T_EnableDelete.Value;
                IsRootItem = objExportDataConfiguration.T_IsRootDeleted.HasValue ? objExportDataConfiguration.T_IsRootDeleted.Value : false;
                IsSoftDeleteEnabled = bool.Parse(CommonFunction.Instance.SoftDeleteEnabled()) && bool.Parse(IsEnableDelete.ToString());
                if(IsSoftDeleteEnabled)
                    if(datalist.Count() > 0)
                    {
                        DeleteSets.Add(new ExportDataDeleteSet()
                        {
                            Id = item.Id,
                            EntityName = item.ChildEntity,
                            Query = datalist
                        });
                    }
                DataTable dt = ExcelExportHelper.ToDataTable<object>(User, datalist.ToListAsync().Result.ToList(), item.ChildEntity, item.ParentEntity, item.AssociationName, folderPath, childfolderPath, IsSelf, IsSoftDeleteEnabled);
                lstcondition.Add(dt.TableName, ExportDataHelper.conditioncriteria(item.Id, item.ChildEntity));
                ds.Tables.Add(dt);
                visitedList.Add(item);
                var childlist = IsSelf ? new List<ExportDataKey>() : KeysData.Where(wh => wh.ParentEntity == item.ChildEntity);
                foreach(var citem in childlist)
                {
                    recursivemethod(citem, datalist, visitedList, ds, KeysData, IsSoftDeleteEnabled, folderPath, childfolderPath, lstcondition);
                }
                string fileName = entitydp + ".xlsx";
                datacntlog.Append(ExportDataHelper.ReadmeLog(ds, fileName));
                ExportDataHelper.SaveExcelFile(ds, objExportDataConfiguration, lstcondition, User.Name, childfolderPath, fileName, entitydp, zipdata.Id.ToString(), zipdata.DisplayValue, Notes, "KKVerbGroupV3");
            }
            if(IsRootItem)
            {
                var rootdp = entities.FirstOrDefault(fd => fd.Name == "VerbsName").DisplayName;
                var rootlist = db.VerbsNames.Where(wh => wh.Id == id);
                var ds = new DataSet();
                var lstcondition = new Dictionary<string, string>();
                var childfolderPath = Path.Combine(folderPath, rootdp);
                IQueryable datalist = null;
                datalist = rootlist;
                if(!Directory.Exists(childfolderPath))
                {
                    Directory.CreateDirectory(childfolderPath);
                }
                if(IsSoftDeleteEnabled)
                    if(datalist.Count() > 0)
                    {
                        DeleteSets.Add(new ExportDataDeleteSet()
                        {
                            Id = 0,
                            EntityName = "VerbsName",
                            Query = datalist
                        });
                    }
                DataTable dt = ExcelExportHelper.ToDataTable<object>(User, datalist.ToListAsync().Result.ToList(), "VerbsName", "VerbsName", null, folderPath, childfolderPath, false, IsSoftDeleteEnabled);
                ds.Tables.Add(dt);
                string fileName = rootdp + ".xlsx";
                datacntlog.Append(ExportDataHelper.ReadmeLog(ds, fileName));
                ExportDataHelper.SaveExcelFile(ds, objExportDataConfiguration, lstcondition, User.Name, childfolderPath, fileName, rootdp, zipdata.Id.ToString(), zipdata.DisplayValue, Notes, "KKVerbGroupV3");
            }
            if(DeleteSets.Count > 0 && IsSoftDeleteEnabled)
                DeleteSets.Add(new ExportDataDeleteSet()
            {
                Id = DeleteSets.Max(m => m.Id) + 1,
                EntityName = "Document",
                Query = db.Documents.Where(wh => wh.IsDeleted.HasValue && wh.IsDeleted.Value)
            });
            var exportdatalog = new T_ExportDataLog();
            using(var dbnew = new ApplicationContext(User))
            {
                exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID = btnid;
                exportdatalog.T_AssociatedExportDataLogStatusID = logstatusId;
                exportdatalog.T_Tag = zipvalue;
                exportdatalog.T_Notes = Notes;
                dbnew.T_ExportDataLogs.Add(exportdatalog);
                dbnew.SaveChanges();
                if(IsSoftDeleteEnabled)
                    foreach(var itemset in DeleteSets.OrderByDescending(o => o.Id))
                    {
                        Type controller = new CreateControllerType(itemset.EntityName).controllerType;
                        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                        {
                            System.Reflection.MethodInfo mc = controller.GetMethod("DeleteExportData");
                            object[] MethodParams = new object[] { exportdatalog.Id, itemset.Query };
                            var objresult = string.Empty;
                            if(mc != null)
                            {
                                objresult = mc.Invoke(objController, MethodParams).ToString();
                            }
                        }
                    }
                DoAuditEntry.AddJournalEntryRecordId("VerbsName", User.Name, objExportDataConfiguration.T_Name, zipdata.DisplayValue, Convert.ToString(id));
                var readmelog = new StringBuilder();
                readmelog.AppendLine("EXPORT DATA SUMMARY");
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.AppendLine("Action - " + objExportDataConfiguration.T_Name);
                readmelog.AppendLine("Is Deleted - " + ((objExportDataConfiguration.T_EnableDelete.HasValue && objExportDataConfiguration.T_EnableDelete.Value) ? "Yes" : "No"));
                readmelog.AppendLine("Exported By - " + User.Name);
                readmelog.AppendLine("Exported On - " + DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss") + " UTC");
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.AppendLine("Record Information - " + zipdata.Id.ToString() + " - " + zipdata.DisplayValue);
                readmelog.AppendLine("Export Data Log Create - " + exportdatalog.DisplayValue);
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.AppendLine("------------------------------------------------------------------------------------------" + Environment.NewLine);
                readmelog.AppendLine("EXPORTED DATA COUNT");
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.Append(datacntlog.ToString());
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.AppendLine(Environment.NewLine + "NOTE : Use the above details to restore/purge. In order to work with zipped file, it must be unzipped or extracted first.");
                System.IO.File.WriteAllText(folderPath + "/README.txt", readmelog.ToString());
                if(readmelog.Length > 0)
                {
                    exportdatalog.T_Summary = readmelog.ToString();
                    dbnew.Entry(exportdatalog).State = EntityState.Modified;
                    dbnew.SaveChanges();
                    datalogList.Add(exportdatalog);
                }
            }
        }
        if(IsBulk)
            zipvalue = Bulkzipvalue;
        //return DownloadZipFile(BulkfolderPath, zipvalue);
        var genfile = DownloadZipFile(BulkfolderPath, zipvalue);
        if(objExportDataConfiguration.T_UploadOneDrive.HasValue && objExportDataConfiguration.T_UploadOneDrive.Value)
        {
            var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
            string AccessToken = string.Empty;
            if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
            {
                AccessToken = await OfficeAccessSession.GetOneDriveToken();
            }
            if(AccessToken != null)
            {
                //upload the file to oneDrive and get a file id
                string oneDrivePath = genfile.FileDownloadName;
                string result = await OfficeAccessSession.GetFilesList(genfile, oneDrivePath);
                if(!string.IsNullOrEmpty(result))
                    using(var dburl = new ApplicationContext(User))
                    {
                        foreach(var item in datalogList)
                        {
                            item.T_OneDriveUrl = result;
                            dburl.Entry(item).State = EntityState.Modified;
                            dburl.SaveChanges();
                        }
                        dburl.SaveChanges();
                    }
            }
        }
        return genfile;
    }
    
    public void recursivemethod(ExportDataKey citem, IQueryable datalist, List<ExportDataKey> visitedList, DataSet ds, List<ExportDataKey> KeysData, bool IsSoftDeleteEnabled, string BasePath = null, string folderPath = null, Dictionary<string, string> lstcondition = null)
    {
        var childdatalist = datalist.SelectMany(citem.AssociationName);
        childdatalist = filtercondtion(childdatalist, citem.Id, citem.ChildEntity);
        var IsSelf = citem.AssociationName.StartsWith("Self");
        if(IsSoftDeleteEnabled)
            if(childdatalist.Count() > 0)
            {
                DeleteSets.Add(new ExportDataDeleteSet()
                {
                    Id = citem.Id,
                    EntityName = citem.ChildEntity,
                    Query = childdatalist
                });
            }
        DataTable cdt = ExcelExportHelper.ToDataTable<object>(User, childdatalist.ToListAsync().Result.ToList(), citem.ChildEntity, citem.ParentEntity, citem.AssociationName, BasePath, folderPath, IsSelf, IsSoftDeleteEnabled);
        lstcondition.Add(cdt.TableName, ExportDataHelper.conditioncriteria(citem.Id, citem.ChildEntity));
        ds.Tables.Add(cdt);
        visitedList.Add(citem);
        var childlist = IsSelf ? new List<ExportDataKey>() : KeysData.Where(wh => wh.ParentEntity == citem.ChildEntity && wh.Hierarchy[0] == citem.Hierarchy[0]);
        foreach(var ccitem in childlist)
        {
            recursivemethod(ccitem, childdatalist, visitedList, ds, KeysData, IsSoftDeleteEnabled, BasePath, folderPath, lstcondition);
        }
    }
    
    
    
}
}

