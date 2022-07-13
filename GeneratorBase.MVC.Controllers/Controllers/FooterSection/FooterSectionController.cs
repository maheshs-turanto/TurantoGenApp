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
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Footer Section  actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class FooterSectionController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="footersection">The Footer Section .</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(FooterSection footersection, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="footersection">The Footer Section .</param>
    private void LoadViewDataAfterOnEdit(FooterSection footersection)
    {
        LoadViewDataBeforeOnEdit(footersection, false);
        CustomLoadViewDataListAfterEdit(footersection);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="footersection">         The Footer Section .</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(FooterSection footersection, bool loadCustomViewData = true)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        var _objCompanyInformationFooterSectionAssociation = new List<CompanyInformation>();
        _objCompanyInformationFooterSectionAssociation.Add(footersection.companyinformationfootersectionassociation);
        ViewBag.CompanyInformationFooterSectionAssociationID = new SelectList(_objCompanyInformationFooterSectionAssociation, "ID", "DisplayValue", footersection.CompanyInformationFooterSectionAssociationID);
        Dictionary<long?, string> _objAssociatedFooterSectionType = new Dictionary<long?, string>();
        _objAssociatedFooterSectionType.Add(1, "Login and Master");
        _objAssociatedFooterSectionType.Add(2, "Login Only");
        _objAssociatedFooterSectionType.Add(3, "Master Only");
        _objAssociatedFooterSectionType.Add(4, "Never");
        ViewBag.AssociatedFooterSectionTypeID = new SelectList(_objAssociatedFooterSectionType.Select(f => new
        {
            ID = f.Key, DisplayValue = f.Value
        }), "ID", "DisplayValue", footersection.AssociatedFooterSectionTypeID);
        ViewBag.FooterSectionIsHiddenRule = checkHidden(User, "FooterSection", "OnEdit", false, null);
        ViewBag.FooterSectionIsGroupsHiddenRule = checkHidden(User, "FooterSection", "OnEdit", true, null);
        ViewBag.FooterSectionIsSetValueUIRule = checkSetValueUIRule(User, "FooterSection", "OnEdit", new long[] { 6, 8 }, null, null);
        ViewBag.FooterSectionRestrictDropdownValueRule = RestrictDropdownValueRule(User, "FooterSection", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(footersection);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="footersection">The Footer Section .</param>
    private void LoadViewDataAfterOnCreate(FooterSection footersection)
    {
        ViewBag.CompanyInformationFooterSectionAssociationID = new SelectList(db.CompanyInformations.Where(p => p.Id == footersection.CompanyInformationFooterSectionAssociationID), "ID", "DisplayValue", footersection.CompanyInformationFooterSectionAssociationID);
        Dictionary<long?, string> _objAssociatedFooterSectionType = new Dictionary<long?, string>();
        _objAssociatedFooterSectionType.Add(1, "Login and Master");
        _objAssociatedFooterSectionType.Add(2, "Login Only");
        _objAssociatedFooterSectionType.Add(3, "Master Only");
        _objAssociatedFooterSectionType.Add(4, "Never");
        ViewBag.AssociatedFooterSectionTypeID = new SelectList(_objAssociatedFooterSectionType.Where(p => p.Key == footersection.AssociatedFooterSectionTypeID).Select(f => new
        {
            ID = f.Key, DisplayValue = f.Value
        }), "ID", "DisplayValue", footersection.AssociatedFooterSectionTypeID);
        CustomLoadViewDataListAfterOnCreate(footersection);
        ViewBag.FooterSectionIsHiddenRule = checkHidden(User, "FooterSection", "OnCreate", false, null);
        ViewBag.FooterSectionIsGroupsHiddenRule = checkHidden(User, "FooterSection", "OnCreate", true, null);
        ViewBag.FooterSectionIsSetValueUIRule = checkSetValueUIRule(User, "FooterSection", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.FooterSectionRestrictDropdownValueRule = RestrictDropdownValueRule(User, "FooterSection", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(HostingEntityName == "CompanyInformation" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "CompanyInformationFooterSectionAssociation")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var objList = db.CompanyInformations.Where(p => p.Id == hostid).ToList();
            ViewBag.CompanyInformationFooterSectionAssociationID = new SelectList(objList, "ID", "DisplayValue", HostingEntityID);
            ViewBag.DisplayVal = objList.FirstOrDefault().DisplayValue;
        }
        else
        {
            var objCompanyInformationFooterSectionAssociation = new List<CompanyInformation>();
            ViewBag.CompanyInformationFooterSectionAssociationID = new SelectList(objCompanyInformationFooterSectionAssociation, "ID", "DisplayValue");
            //
        }
        Dictionary<long?, string> _objAssociatedFooterSectionType = new Dictionary<long?, string>();
        _objAssociatedFooterSectionType.Add(1, "Login and Master");
        _objAssociatedFooterSectionType.Add(2, "Login Only");
        _objAssociatedFooterSectionType.Add(3, "Master Only");
        _objAssociatedFooterSectionType.Add(4, "Never");
        ViewBag.AssociatedFooterSectionTypeID = new SelectList(_objAssociatedFooterSectionType.Select(f => new
        {
            ID = f.Key, DisplayValue = f.Value
        }), "ID", "DisplayValue", HostingEntityID);
        ViewBag.FooterSectionIsHiddenRule = checkHidden(User, "FooterSection", "OnCreate", false, null);
        ViewBag.FooterSectionIsGroupsHiddenRule = checkHidden(User, "FooterSection", "OnCreate", true, null);
        ViewBag.FooterSectionIsSetValueUIRule = checkSetValueUIRule(User, "FooterSection", "OnCreate", new long[] { 6, 7 }, null, null);
        ViewBag.FooterSectionRestrictDropdownValueRule = RestrictDropdownValueRule(User, "FooterSection", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    /// <summary>Export Excel.</summary>
    ///
    /// <param name="query">List of objects.</param>
    ///
    /// <returns>xlsx File.</returns>
    public FileResult DownloadExcel(List<GeneratorBase.MVC.Models.FooterSection> query)
    {
        foreach(var item in query)
        {
            item.setDateTimeToClientTime();
            item.ApplyHiddenRule(User.businessrules, "FooterSection");
        }
        DataTable dt = ExcelExportHelper.ToDataTable<FooterSection>(User, query, "FooterSection");
        dt.AcceptChanges();
        string fileName = "Footer Section .xlsx";
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            var columns = EntityColumns().Select(p => p.Value).ToArray();
            dt.SetColumnsOrder(columns);
            dt.RemoveColumns(new string[] { "Document Upload" });
            wb.Properties.Author = "KKCompanyProfile3";
            wb.Properties.Title = "Footer Section ";
            wb.Properties.Subject = "Footer Section  data";
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
        var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FooterSection").Properties;
        columns.Add("2", modelproperties.FirstOrDefault(q => q.Name == "CompanyInformationFooterSectionAssociationID").DisplayName);
        columns.Add("3", modelproperties.FirstOrDefault(q => q.Name == "Name").DisplayName);
        columns.Add("4", modelproperties.FirstOrDefault(q => q.Name == "AssociatedFooterSectionTypeID").DisplayName);
        columns.Add("5", modelproperties.FirstOrDefault(q => q.Name == "WebLinkTitle").DisplayName);
        columns.Add("6", modelproperties.FirstOrDefault(q => q.Name == "WebLink").DisplayName);
        columns.Add("7", modelproperties.FirstOrDefault(q => q.Name == "DocumentUpload").DisplayName);
        return columns;
    }
    
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstFooterSection">The list Footer Section .</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<FooterSection> searchRecords(IQueryable<FooterSection> lstFooterSection, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstFooterSection = lstFooterSection.Where(s => (s.companyinformationfootersectionassociation != null && (s.companyinformationfootersectionassociation.DisplayValue.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.WebLinkTitle) && s.WebLinkTitle.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.WebLink) && s.WebLink.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstFooterSection = lstFooterSection.Where(s => (s.companyinformationfootersectionassociation != null && (s.companyinformationfootersectionassociation.DisplayValue.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.WebLinkTitle) && s.WebLinkTitle.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.WebLink) && s.WebLink.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstFooterSection;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstFooterSection">The IQueryable list Footer Section .</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<FooterSection> sortRecords(IQueryable<FooterSection> lstFooterSection, string sortBy, string isAsc)
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
        if(sortBy == "CompanyInformationFooterSectionAssociationID")
            return isAsc.ToLower() == "asc" ? lstFooterSection.OrderBy(p => p.companyinformationfootersectionassociation.DisplayValue) : lstFooterSection.OrderByDescending(p => p.companyinformationfootersectionassociation.DisplayValue);
        ParameterExpression paramExpression = Expression.Parameter(typeof(FooterSection), "footersection");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<FooterSection>)lstFooterSection.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstFooterSection.ElementType, lambda.Body.Type },
                       lstFooterSection.Expression,
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
        if(sourceEntity == "CompanyInformationCompanyListAssociation")
        {
            var url = (Url.Action("FSearch"));
            var _Object = db.CompanyInformationCompanyListAssociations.Find(Convert.ToInt64(id));
            url += "?search=";
            var CompanyInformationFooterSectionAssociation = _Object.CompanyInformationID;
            url += "&companyinformationfootersectionassociation=" + CompanyInformationFooterSectionAssociation;
            return Redirect(url.ToString());
        }
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
    public ActionResult SetFSearch(string searchString, string HostingEntity, string companyinformationfootersectionassociation, string associatedfootersectiontype, bool? RenderPartial, bool ShowDeleted = false)
    {
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        SetFSearchViewBag("FooterSection");
        ViewBag.HideColumns = new MultiSelectList(EntityColumns(), "Key", "Value");
        return View(new FooterSection());
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
    public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string companyinformationfootersectionassociation, string associatedfootersectiontype, string FilterCondition, string HostingEntity, string AssociatedType, string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn, bool? IsReports, bool? IsdrivedTab, bool? IsFilter = false, bool ShowDeleted = false)
    {
        FSearchViewBag(currentFilter, searchString, FSFilter, sortBy, isAsc, page, itemsPerPage, search, FilterCondition, HostingEntity, AssociatedType, HostingEntityID, viewtype, SortOrder, HideColumns, GroupByColumn, IsReports, IsdrivedTab, IsFilter, "FooterSection");
        CustomLoadViewDataListOnIndex(HostingEntity, Convert.ToInt32(HostingEntityID), AssociatedType);
        var lstFooterSection = from s in db.FooterSections
                               select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstFooterSection = searchRecords(lstFooterSection, searchString.ToUpper(), true);
        }
        if(!string.IsNullOrEmpty(search))
            search = search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(search))
        {
            ViewBag.SearchResult += "\r\n General Criterial= " + search + ",";
            lstFooterSection = searchRecords(lstFooterSection, search, true);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstFooterSection = sortRecords(lstFooterSection, sortBy, isAsc);
        }
        else lstFooterSection = lstFooterSection.OrderByDescending(c => c.Id);
        lstFooterSection = CustomSorting(lstFooterSection, HostingEntity, AssociatedType, sortBy, isAsc);
        //lstFooterSection = lstFooterSection.IncludeOptimized(t=>t.companyinformationfootersectionassociation).IncludeOptimized(t=>t.associatedfootersectiontype);
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
                ViewBag.SearchResult += " " + GetPropertyDP("FooterSection", PropertyName) + " " + Operator + " " + Value + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch("FooterSection", PropertyName, Operator, Value) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstFooterSection = lstFooterSection.Where(whereCondition.ToString());
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
            lstFooterSection = Sorting.Sort<FooterSection>(lstFooterSection, DataOrdering);
        var _FooterSection = lstFooterSection;
        //if (lstFooterSection.Where(p => p.companyinformationfootersectionassociation != null).Count() <= 50)
        //ViewBag.companyinformationfootersectionassociation = new SelectList(lstFooterSection.Where(p => p.companyinformationfootersectionassociation != null).Select(P => P.companyinformationfootersectionassociation).Distinct(), "ID", "DisplayValue");
        if(companyinformationfootersectionassociation != null)
        {
            var ids = companyinformationfootersectionassociation.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            ViewBag.SearchResult += "\r\n CompanyInformation= ";
            foreach(var str in ids)
            {
                //Null Search
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        ViewBag.SearchResult += "";
                    }
                    else
                    {
                        ids1.Add(Convert.ToInt64(str));
                        var obj = db.CompanyInformations.Find(Convert.ToInt64(str));
                        ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            _FooterSection = _FooterSection.Where(p => ids1.Contains(p.CompanyInformationFooterSectionAssociationID));
        }
        //if (lstFooterSection.Where(p => p.associatedfootersectiontype != null).Count() <= 50)
        //ViewBag.associatedfootersectiontype = new SelectList(lstFooterSection.Where(p => p.associatedfootersectiontype != null).Select(P => P.associatedfootersectiontype).Distinct(), "ID", "DisplayValue");
        _FooterSection = FilterByHostingEntity(_FooterSection, HostingEntity, AssociatedType, Convert.ToInt32(HostingEntityID));
        ViewBag.SearchResult = ((string)ViewBag.SearchResult).TrimStart("\r\n".ToCharArray()).TrimEnd(", ".ToCharArray()).TrimEnd(",".ToCharArray());
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "FooterSection"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "FooterSection"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "FooterSection"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        ViewBag.PageSize = pageSize;
        //
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_FooterSection.Count() > 0)
                pageSize = _FooterSection.Count();
            //return View("ExcelExport", _FooterSection.ToCachedPagedList(pageNumber, pageSize));
            return DownloadExcel(_FooterSection.ToCachedPagedList(pageNumber, pageSize).ToList());
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _FooterSection.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        ViewBag.Pages = pageNumber;
        if(!Request.IsAjaxRequest())
        {
            if(string.IsNullOrEmpty(viewtype))
                viewtype = "IndexPartial";
            ViewBag.TemplatesName = GetTemplatesForList(User, "FooterSection", viewtype);
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
                ViewBag.TemplatesName = viewtype;
            var list = _FooterSection.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityFooterSectionDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["FooterSectionlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "FooterSection", tagsSplit.ToArray());
                }
            return View("Index", list);
        }
        else
        {
            var list = _FooterSection.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityFooterSectionDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["FooterSectionlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "FooterSection", tagsSplit.ToArray());
                }
            if(ViewBag.TemplatesName == null)
                return PartialView("IndexPartial", list);
            else
                return PartialView(ViewBag.TemplatesName, list);
        }
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_FooterSection">IQueryable<FooterSection>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<FooterSection>.</returns>
    private IQueryable<FooterSection> FilterByHostingEntity(IQueryable<FooterSection> _FooterSection, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        if(HostingEntity == "CompanyInformation" && AssociatedType == "CompanyInformationFooterSectionAssociation")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _FooterSection = _FooterSection.Where(p => p.CompanyInformationFooterSectionAssociationID == hostid);
                ViewBag.HostingEntityIDData = db.CompanyInformations.FirstOrDefault(fd => fd.Id == hostid);
            }
            else
                _FooterSection = _FooterSection.Where(p => p.CompanyInformationFooterSectionAssociationID == null);
        }
        return _FooterSection;
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
            var _Obj = appcontext.FooterSections.FirstOrDefault(p => p.Id == idvalue);
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
    }
    /// <summary>Gets IQueryable.</summary>
    ///
    /// <param name="appdb">The applicationContext.</param>
    ///
    /// <returns>The IQueryable.</returns>
    public IQueryable<FooterSection> GetIQueryable(ApplicationContext appdb)
    {
        return appdb.FooterSections.AsNoTracking();
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
    public object GetRecordById(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.FooterSections.Find(Convert.ToInt64(id));
            return _Obj;
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
        if(string.IsNullOrEmpty(id)) return Json(new FooterSection(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.FooterSections.Find(Convert.ToInt64(id));
            return Json(_Obj, JsonRequestBehavior.AllowGet);
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
            var _Obj = context.FooterSections.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<FooterSection>(_Obj, "FooterSection", new string[] { "" });
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
        IQueryable<FooterSection> list = db.FooterSections;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<FooterSection>(User, list, "FooterSection", caller);
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
    [Audit(0)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string RestrictDropdownVal = "")
    {
        var result = DropdownHelper.GetAllValue<FooterSection>(User, db.FooterSections, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "FooterSection", RestrictDropdownVal);
        return Json(result.Select(x => new
        {
            Id = x.Id,
            Name = x.DisplayValue
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
        var result = DropdownHelper.GetAllValueForRB<FooterSection>(User, db.FooterSections, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "FooterSection");
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
        IQueryable<FooterSection> list = db.FooterSections;
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
    public JsonResult GetAllMultiSelectValue(string caller, string key, string AssoNameWithParent, string AssociationID)
    {
        caller = caller.Replace("?", "");
        var result = DropdownHelper.GetAllMultiSelectValue(User, db.FooterSections, key, AssoNameWithParent, AssociationID, "FooterSection", caller);
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
        case "CompanyInformationFooterSectionAssociationID":
            columnValue = db.CompanyInformations.FirstOrDefault(p => p.Id == id).DisplayValue;
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
    public ActionResult Upload(string FileType, string AssociatedType, string HostingEntityName, string HostingEntityID, string UrlReferrer)
    {
        if(FileType.ToLower() == "csv")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportCSV", "FooterSection", User) || !User.CanAdd("FooterSection"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else if(FileType.ToLower() == "xls" || FileType.ToLower() == "xlsx")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "FooterSection", User) || !User.CanAdd("FooterSection"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "FooterSection")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "FooterSection").ToList();
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
        if(FileUpload != null)
        {
            var AssociatedType = collection["hdnAssociatedType"];
            var HostingEntityName = collection["hdnHostingEntityName"];
            var HostingEntityID = collection["hdnHostingEntityID"];
            var UrlReferrer = collection["hdnUrlReferrer"];
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "FooterSection");
                if(entList != null)
                {
                    var entityprop = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime" && p.Name != "DocumentUpload");
                    string[] allcolumns = new string[] { "CompanyInformationFooterSectionAssociationID", "Name", "AssociatedFooterSectionTypeID", "WebLinkTitle", "WebLink" };
                    var prorlist = new List<ModelReflector.Property>();
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
                                if(!lstProperty.ContainsKey(prop.DisplayName))
                                    lstProperty.Add(prop.DisplayName, prop.DisplayName + "-" + assoc.AssociationProperty);
                            }
                            //var dispValue = lstProperty.Keys.FirstOrDefault();
                            objColMapAssocProperties.Add(assoc, new SelectList(lstProperty.AsEnumerable(), "Value", "Key", "Key"));
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
                if(!string.IsNullOrEmpty(collection["ListOfMappings"]))
                {
                    string typeName = "";
                    string colKey = "";
                    string colDisKey = "";
                    string colListInx = "";
                    typeName = "FooterSection";
                    //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                    string idMapping = collection["ListOfMappings"];
                    string ExistsColumnMappingName = string.Empty;
                    string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    if(collection["DefaultMapping"] == "on")
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == "FooterSection" && p.IsDefaultMapping);
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
                            if(AssociatedType != null && colExist == 0)
                                if(defcol.Key.Name == AssociatedType + "ID")
                                    colExist = 1;
                            if(colExist == 0)
                                ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
                            if(AssociatedType != null && colExist == 1 && defcol.Key.Name == AssociatedType + "ID")
                                colListInx = colListInx + colKey.Split(',').ToList().IndexOf(AssociatedType + "ID") + 1 + ",";
                            else
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
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Footer Section </b>";
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
                                case "CompanyInformationFooterSectionAssociationID":
                                    var companyinformationfootersectionassociationId = db.CompanyInformations.FirstOrDefault(p => p.DisplayValue == assovalue);
                                    if(companyinformationfootersectionassociationId == null)
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
        string DetailMessage = "";
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string typename = "FooterSection";
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
                    lstEntityProp.Add(lst[1], lst[0]);
                }
            }
            //var tenantDb = new ApplicationContext(User, "", Tenant);
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
                    var assentName = ModelReflector.Entities.FirstOrDefault(p => p.DisplayName == tblcols[i]);
                    if(assentName != null)
                    {
                        var mapingProplst = lstEntityProp.Where(p => p.Value == assentName.Properties.FirstOrDefault().DisplayName && p.Value.ToLower() != "displayvalue");
                        if(mapingProplst != null && mapingProplst.Count() > 0)
                            shtcolName = mapingProplst.FirstOrDefault().Value;
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
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Footer Section </b>";
            Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "FooterSection");
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
                        case "CompanyInformationFooterSectionAssociationID":
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
            ViewBag.Tenant = Tenant;
            ViewBag.AssociatedType = AssociatedType;
            ViewBag.HostingEntityName = HostingEntityName;
            ViewBag.HostingEntityID = HostingEntityID;
            ViewBag.UrlReferrer = UrlReferrer;
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
        //var tenantDb = new ApplicationContext(User, "", Tenant);
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
                    FooterSection model = new FooterSection();
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
                        case "Name":
                            model.Name = columnValue;
                            break;
                        case "WebLinkTitle":
                            model.WebLinkTitle = columnValue;
                            break;
                        case "WebLink":
                            model.WebLink = columnValue;
                            break;
                        case "CompanyInformationFooterSectionAssociationID":
                            dynamic companyinformationfootersectionassociationId = null;
                            if(AssociatedType != null && AssociatedType == "CompanyInformationFooterSectionAssociation" && HostingEntityID != null)
                            {
                                long id = Convert.ToInt64(HostingEntityID);
                                model.CompanyInformationFooterSectionAssociationID = id;
                            }
                            else
                            {
                                if(lstEntityProp.Count > 0)
                                {
                                }
                                else
                                { }
                            }
                            if(companyinformationfootersectionassociationId != null)
                                model.CompanyInformationFooterSectionAssociationID = companyinformationfootersectionassociationId.Id;
                            else
                            {
                                if((collection["CompanyInformationFooterSectionAssociationID"] != null) && (collection["CompanyInformationFooterSectionAssociationID"].ToString() == "true,false"))
                                {
                                    try
                                    {
                                        CompanyInformation objCompanyInformation = new CompanyInformation();
                                        objCompanyInformation.CompanyName = (columnValue);
                                        try
                                        {
                                            objCompanyInformation.CompanyEmail = (columnValue);
                                        }
                                        catch
                                        {
                                            objCompanyInformation.CompanyEmail = default(string);
                                        }
                                        try
                                        {
                                            objCompanyInformation.SMTPUser = (columnValue);
                                        }
                                        catch
                                        {
                                            objCompanyInformation.SMTPUser = default(string);
                                        }
                                        try
                                        {
                                            objCompanyInformation.SMTPServer = (columnValue);
                                        }
                                        catch
                                        {
                                            objCompanyInformation.SMTPServer = default(string);
                                        }
                                        try
                                        {
                                            objCompanyInformation.SMTPPassword = (columnValue);
                                        }
                                        catch
                                        {
                                            objCompanyInformation.SMTPPassword = default(string);
                                        }
                                        try
                                        {
                                            objCompanyInformation.SMTPPort = (columnValue);
                                        }
                                        catch
                                        {
                                            objCompanyInformation.SMTPPort = default(string);
                                        }
                                        db.CompanyInformations.Add(objCompanyInformation);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                            db.Entry(objCompanyInformation).State = EntityState.Detached;
                                            continue;
                                        }
                                        model.CompanyInformationFooterSectionAssociationID = objCompanyInformation.Id;
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
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, "FooterSection", mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User, db, model, "FooterSection");
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", "FooterSection");
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                                //if (result == null || result.Count == 0)
                            {
                                var customerror = "";
                                if(!CustomSaveOnImport(model, out customerror, i))
                                {
                                    if(!flagUpdate)
                                    {
                                        db.FooterSections.Add(model);
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
                var obj1 = appcontext.FooterSections.Where(p => p.Id == Id).FirstOrDefault();
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
    /// <param name="footersection">The Footer Section .</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleOnSaving action.</returns>
    private JsonResult ApplyBusinessRuleOnSaving(FooterSection footersection)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "FooterSection").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            //var ruleids = businessrule.Select(q => q.Id).ToList();
            //var typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(User, db, footersection, "OnEdit", "FooterSection");
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
                var resultBR = GetMandatoryPropertiesDictionary(User, db, footersection, "OnEdit", "FooterSection");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(!dic.Key.Contains("FailureMessage"))
                        {
                            var type = footersection.GetType();
                            if(type.GetProperty(dic.Key) != null)
                            {
                                var propertyvalue = type.GetProperty(dic.Key).GetValue(footersection, null);
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
                var resultBR = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, footersection, "OnEdit", "FooterSection");
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
        var businessrule = User.businessrules.Where(p => p.EntityName == "FooterSection").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var footersection = db.FooterSections.Find(objectId);
            if(footersection != null)
            {
                var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
                if(typelist.Contains(1) || typelist.Contains(11))
                {
                    var validateLockResult = GetLockBusinessRulesDictionary(User, db, footersection, "FooterSection").Where(p => !p.Key.Contains("InformationMessage"));
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
                    var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User, db, footersection, "FooterSection");
                    if(validateMandatorypropertyResult.Count() > 0)
                    {
                        string stringResult = "";
                        string BRID = "";
                        foreach(var dic in validateMandatorypropertyResult)
                        {
                            if(!dic.Key.Contains("FailureMessage"))
                            {
                                var type = footersection.GetType();
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
    /// <param name="footersection">       The Footer Section .</param>
    /// <param name="IsReadOnlyIgnore">(Optional) True if is read only ignored, false if not.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBefore action.</returns>
    private JsonResult ApplyBusinessRuleBefore(FooterSection footersection, bool IsReadOnlyIgnore = false)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "FooterSection").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(User, db, footersection, "FooterSection").Where(p => p.Key.Contains("FailureMessage"));
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
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User, db, footersection, "FooterSection").Where(p => p.Key.Contains("FailureMessage"));
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
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetReadOnlyProperties(FooterSection OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
        return Json(GetReadOnlyPropertiesDictionary(User, db, OModel, "FooterSection"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON (Mandatory business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetMandatoryProperties(FooterSection OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
        return Json(GetMandatoryPropertiesDictionary(User, db, OModel, ruleType, "FooterSection"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetUIAlertBusinessRules(FooterSection OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "FooterSection").ToList();
            var BRAll = BR;
            if(ruleType == "OnCreate")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
            else if(ruleType == "OnEdit")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "FooterSection");
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
    /// <returns>FooterSection object.</returns>
    public FooterSection UpdateHiddenProperties(FooterSection OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.FooterSections.Find(OModel.Id);
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
    public JsonResult GetHiddenVerb(FooterSection OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "FooterSection").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
                OModel.setCalculation();
                var ResultOfBusinessRules  =new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
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
    
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSaveProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSaveProperties(FooterSection OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
        return Json(GetValidateBeforeSavePropertiesDictionary(User, db, OModel, ruleType, "FooterSection"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSavePropertiesForPopupConfirm action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSavePropertiesForPopupConfirm(FooterSection OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
        return Json(GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "FooterSection"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Lock record business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    [ValidateAntiForgeryToken]
    public JsonResult GetLockBusinessRules(FooterSection OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("FooterSection")));
        return Json(GetLockBusinessRulesDictionary(User, db, OModel, "FooterSection"), JsonRequestBehavior.AllowGet);
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
        return 0;
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
            IQueryable query = context.FooterSections;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(FooterSection), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if(PropValue.ToLower().Trim() != "null")
                lambda = Expression.Lambda<Func<FooterSection, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
            else
                lambda = Expression.Lambda<Func<FooterSection, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<FooterSection>)q);
            if(list1 != null && list1.Count() > 0)
                return Int64.TryParse(list1.FirstOrDefault().Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Id">      The identifier.</param>
    /// <param name="PropName">Name of the property.</param>
    ///
    /// <returns>A JSON response stream to send to the GetPropertyValueByEntityId action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
    {
        using(var context = (new ApplicationContext(new SystemUser(), true)))
        {
            var obj1 = context.FooterSections.Find(Id);
            if(obj1 == null)
                return Json("0", JsonRequestBehavior.AllowGet);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            PropValue = PropValue == null ? 0 : PropValue;
            return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
        }
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
            FooterSection obj1 = context.FooterSections.Find(Id);
            if(obj1 != null)
            {
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(q => q.PropertyType.FullName.StartsWith("System")).ToArray();
                //
                string propToWhere = PropName;
                ParameterExpression p = Expression.Parameter(typeof(FooterSection), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                //LambdaExpression lambda = null;
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                object propValue = typeConverter.ConvertFromString(value);
                System.Reflection.PropertyInfo Property1 = properties.FirstOrDefault(r => r.Name == PropName);
                Property1.SetValue(obj1, propValue, null);
                //
                //context.Entry(obj1).State = EntityState.Modified; //removed due to concurrency error
                //context.SaveChanges();
            }
        }
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (1M threshold value).</summary>
    ///
    /// <param name="footersection">The Footer Section .</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    [HttpPost]
    public JsonResult Check1MThresholdValue(FooterSection footersection)
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
        string[] Verbsarr = new string[] { "BulkUpdate", "BulkDelete", "ImportExcel", "ImportCSV", "ExportExcel", "ExportCSV" };
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
        string[][] groupsarr = new string[][] { };
        return groupsarr;
    }
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="footersection">The Footer Section .</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetCalculationValues(FooterSection footersection)
    {
        footersection.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (footersection.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="footersection">     The Footer Section .</param>
    /// <param name="IgnoreEditable">(Optional) The ignore editable.</param>
    /// <param name="source">        (Optional) Source.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetails(FooterSection footersection, string IgnoreEditable = null, string source = null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (footersection.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">          The host.</param>
    /// <param name="value">         The value.</param>
    /// <param name="footersection">     The Footer Section .</param>
    /// <param name="IgnoreEditable">The ignored editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetailsInline(string host, string value, FooterSection footersection, string IgnoreEditable)
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
    
}
}

