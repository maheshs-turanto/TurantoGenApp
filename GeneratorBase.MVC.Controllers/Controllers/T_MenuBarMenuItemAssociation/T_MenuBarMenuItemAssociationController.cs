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
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Z.EntityFramework.Plus;
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Menu Bar Menu Item Association actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class T_MenuBarMenuItemAssociationController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    private void LoadViewDataAfterOnEdit(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation)
    {
        LoadViewDataBeforeOnEdit(t_menubarmenuitemassociation, false);
        CustomLoadViewDataListAfterEdit(t_menubarmenuitemassociation);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="t_menubarmenuitemassociation">         The Menu Bar Menu Item Association.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, bool loadCustomViewData = true)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        var _objT_MenuBar = new List<T_MenuBar>();
        _objT_MenuBar.Add(t_menubarmenuitemassociation.t_menubar);
        ViewBag.T_MenuBarID = new SelectList(_objT_MenuBar, "ID", "DisplayValue", t_menubarmenuitemassociation.T_MenuBarID);
        var _objT_MenuItem = new List<T_MenuItem>();
        _objT_MenuItem.Add(t_menubarmenuitemassociation.t_menuitem);
        ViewBag.T_MenuItemID = new SelectList(_objT_MenuItem, "ID", "DisplayValue", t_menubarmenuitemassociation.T_MenuItemID);
        ViewBag.T_MenuBarMenuItemAssociationIsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnEdit", false,null);
        ViewBag.T_MenuBarMenuItemAssociationIsGroupsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnEdit", true,null);
        ViewBag.T_MenuBarMenuItemAssociationIsSetValueUIRule = checkSetValueUIRule(User,"T_MenuBarMenuItemAssociation", "OnEdit",new long[] { 6, 8 },null,null);
        ViewBag.T_MenuBarMenuItemAssociationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_MenuBarMenuItemAssociation", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        var DocumentTemplates = db.T_DocumentTemplates.Where(p => p.T_EntityName == "T_MenuBarMenuItemAssociation" && !p.T_Disable.Value).OrderBy(p=>p.T_DisplayOrder).ToList();
        ViewBag.DocumentTemplates = DocumentTemplates.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        var ExportData = db.T_ExportDataConfigurations.Where(p => p.T_EntityName == "T_MenuBarMenuItemAssociation" && !p.T_Disable.Value).OrderBy(p => p.Id).ToList();
        ViewBag.ExportDataTemplates = ExportData.Where(p => string.IsNullOrEmpty(p.T_AllowedRoles) || User.IsInRole(User.userroles, p.T_AllowedRoles.Split(",".ToCharArray()))).ToList();
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(t_menubarmenuitemassociation);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    private void LoadViewDataAfterOnCreate(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation)
    {
        ViewBag.T_MenuBarID = new SelectList(db.T_MenuBars.Where(p => p.Id == t_menubarmenuitemassociation.T_MenuBarID), "ID", "DisplayValue", t_menubarmenuitemassociation.T_MenuBarID);
        ViewBag.T_MenuItemID = new SelectList(db.T_MenuItems.Where(p => p.Id == t_menubarmenuitemassociation.T_MenuItemID), "ID", "DisplayValue", t_menubarmenuitemassociation.T_MenuItemID);
        CustomLoadViewDataListAfterOnCreate(t_menubarmenuitemassociation);
        ViewBag.T_MenuBarMenuItemAssociationIsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnCreate", false,null);
        ViewBag.T_MenuBarMenuItemAssociationIsGroupsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnCreate", true,null);
        ViewBag.T_MenuBarMenuItemAssociationIsSetValueUIRule = checkSetValueUIRule(User,"T_MenuBarMenuItemAssociation", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.T_MenuBarMenuItemAssociationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_MenuBarMenuItemAssociation", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(HostingEntityName == "T_MenuBar" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_MenuBarMenuItemAssociation_T_MenuBar")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var objList = db.T_MenuBars.Where(p=>p.Id==hostid).ToList();
            ViewBag.T_MenuBarID = new SelectList(objList, "ID", "DisplayValue",HostingEntityID);
            ViewBag.DisplayVal = objList.FirstOrDefault().DisplayValue;
        }
        else
        {
            //Manytomany associations
            var T_MenuBarMenuItemAssociationlist = db.T_MenuItems.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
            ViewBag.t_menubarmenuitemassociation = new SelectList(T_MenuBarMenuItemAssociationlist, "ID", "DisplayValue");
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuItemID == hostid).Select(q => q.T_MenuBarID);
            var AvailableList = db.T_MenuBars.Where(p => !Selectedlist.Contains(p.Id));
            AvailableList = _fad.FilterDropdown<T_MenuBar>(User, AvailableList, "T_MenuBar", "T_MenuBarMenuItemAssociation.T_MenuBarID");
            ViewBag.IsTop50 = "none";
            if(AvailableList.Count() > 50)
                ViewBag.IsTop50 = "block";
            ViewBag.T_MenuBarIDAvailable  = new SelectList(AvailableList.OrderBy(p => p.DisplayValue).Take(50).ToList(), "ID", "DisplayValue");
            var SelectedListComplete = db.T_MenuBars.Where(p => Selectedlist.Contains(p.Id));
            var SelectedListFiltered = db.T_MenuBars.Where(p => Selectedlist.Contains(p.Id));
            SelectedListFiltered = _fad.FilterDropdown<T_MenuBar>(User, SelectedListFiltered, "T_MenuBar", "T_MenuBarMenuItemAssociation.T_MenuBarID");
            ViewBag.SelectedListRemoved= string.Join(",", SelectedListComplete.Except(SelectedListFiltered).Select(p=>p.Id).ToList());
            ViewBag.T_MenuBarIDSelected = new MultiSelectList(SelectedListComplete.OrderBy(p => p.DisplayValue).ToList(), "ID", "DisplayValue", Selectedlist);
        }
        if(HostingEntityName == "T_MenuItem" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_MenuBarMenuItemAssociation_T_MenuItem")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var objList = db.T_MenuItems.Where(p=>p.Id==hostid).ToList();
            ViewBag.T_MenuItemID = new SelectList(objList, "ID", "DisplayValue",HostingEntityID);
            ViewBag.DisplayVal = objList.FirstOrDefault().DisplayValue;
        }
        else
        {
            //ManytoOne association
            var T_MenuItemMenuItemAssociationlist = db.T_MenuItems.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
            ViewBag.t_menuitemmenuitemassociation = new SelectList(T_MenuItemMenuItemAssociationlist, "ID", "DisplayValue");
            //Manytomany associations
            var T_MenuBarMenuItemAssociationlist = db.T_MenuBars.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
            ViewBag.t_menubarmenuitemassociation = new SelectList(T_MenuBarMenuItemAssociationlist, "ID", "DisplayValue");
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuBarID == hostid).Select(q => q.T_MenuItemID);
            var AvailableList = db.T_MenuItems.Where(p => !Selectedlist.Contains(p.Id));
            AvailableList = _fad.FilterDropdown<T_MenuItem>(User, AvailableList, "T_MenuItem", "T_MenuBarMenuItemAssociation.T_MenuItemID");
            ViewBag.IsTop50 = "none";
            if(AvailableList.Count() > 50)
                ViewBag.IsTop50 = "block";
            ViewBag.T_MenuItemIDAvailable  = new SelectList(AvailableList.OrderBy(p => p.DisplayValue).Take(50).ToList(), "ID", "DisplayValue");
            var SelectedListComplete = db.T_MenuItems.Where(p => Selectedlist.Contains(p.Id));
            var SelectedListFiltered = db.T_MenuItems.Where(p => Selectedlist.Contains(p.Id));
            SelectedListFiltered = _fad.FilterDropdown<T_MenuItem>(User, SelectedListFiltered, "T_MenuItem", "T_MenuBarMenuItemAssociation.T_MenuItemID");
            ViewBag.SelectedListRemoved= string.Join(",", SelectedListComplete.Except(SelectedListFiltered).Select(p=>p.Id).ToList());
            ViewBag.T_MenuItemIDSelected = new MultiSelectList(SelectedListComplete.OrderBy(p => p.DisplayValue).ToList(), "ID", "DisplayValue", Selectedlist);
        }
        ViewBag.T_MenuBarMenuItemAssociationIsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnCreate", false,null);
        ViewBag.T_MenuBarMenuItemAssociationIsGroupsHiddenRule = checkHidden(User,"T_MenuBarMenuItemAssociation", "OnCreate", true,null);
        ViewBag.T_MenuBarMenuItemAssociationIsSetValueUIRule = checkSetValueUIRule(User,"T_MenuBarMenuItemAssociation", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.T_MenuBarMenuItemAssociationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_MenuBarMenuItemAssociation", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    public ActionResult DoExport(T_MenuBarMenuItemAssociationIndexViewModel model, IQueryable<T_MenuBarMenuItemAssociation> _T_MenuBarMenuItemAssociation)
    {
        if(!((CustomPrincipal)User).CanUseVerb("ExportExcel", "T_MenuBarMenuItemAssociation", User) || !User.CanView("T_MenuBarMenuItemAssociation"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(model.ExportType == "csv")
        {
            model.pageNumber = 1;
            if(_T_MenuBarMenuItemAssociation.Count() > 0)
                model.PageSize = _T_MenuBarMenuItemAssociation.Count();
            var csvdata = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
            csvdata.ToList().ForEach(fr => fr.setDateTimeToClientTime());
            csvdata.ToList().ForEach(fr => fr.ApplyHiddenRule(User.businessrules, "T_MenuBarMenuItemAssociation"));
            return new CsvResult<T_MenuBarMenuItemAssociation>(csvdata.ToList(), "Menu Bar Menu Item Association.csv", EntityColumns().Select(s => s.Value).ToArray(), User, new string[] {  });
        }
        else
        {
            model.pageNumber = 1;
            if(_T_MenuBarMenuItemAssociation.Count() > 0)
                model.PageSize = _T_MenuBarMenuItemAssociation.Count();
            return DownloadExcel(_T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize).ToList());
        }
    }
    public ActionResult DoBulkOperations(T_MenuBarMenuItemAssociationIndexViewModel model, IQueryable<T_MenuBarMenuItemAssociation> _T_MenuBarMenuItemAssociation, IQueryable<T_MenuBarMenuItemAssociation> lstT_MenuBarMenuItemAssociation)
    {
        if(model.BulkOperation != null && (model.BulkOperation == "single" || model.BulkOperation == "multiple"))
        {
            model.TemplatesName = "IndexPartial";
            ViewData["BulkAssociate"] = model.BulkAssociate;
            if(!string.IsNullOrEmpty(model.caller))
            {
                FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                _T_MenuBarMenuItemAssociation = _fad.FilterDropdown<T_MenuBarMenuItemAssociation>(User,  _T_MenuBarMenuItemAssociation, "T_MenuBarMenuItemAssociation", model.caller);
            }
            if(Convert.ToBoolean(model.BulkAssociate))
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = sortRecords(lstT_MenuBarMenuItemAssociation.Except(_T_MenuBarMenuItemAssociation),model.sortBy,model.IsAsc).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
                else
                {
                    model.list = lstT_MenuBarMenuItemAssociation.Except(_T_MenuBarMenuItemAssociation).OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation", model);
                }
            }
            else
            {
                if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
                {
                    model.list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
                    return PartialView("BulkOperation",model);
                }
                else
                {
                    model.list =_T_MenuBarMenuItemAssociation.OrderByDescending(c => c.Id).ToCachedPagedList(model.pageNumber, model.PageSize);
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
    public FileResult DownloadExcel(List<GeneratorBase.MVC.Models.T_MenuBarMenuItemAssociation> query)
    {
        List<string> lstHiddenProp = new List<string>();
        foreach(var item in query)
        {
            item.setDateTimeToClientTime();
            //item.ApplyHiddenRule(User.businessrules, "T_MenuBarMenuItemAssociation");
            lstHiddenProp = item.ApplyHiddenRule(User.businessrules, "T_MenuBarMenuItemAssociation");
        }
        if(lstHiddenProp.Count > 0)
        {
            var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation").Properties;
            for(int i = 0; i < lstHiddenProp.Count; i++)
            {
                lstHiddenProp[i] = modelproperties.FirstOrDefault(q => q.Name == lstHiddenProp[i]).DisplayName;
            }
        }
        DataTable dt = ExcelExportHelper.ToDataTable<T_MenuBarMenuItemAssociation>(User, query,"T_MenuBarMenuItemAssociation");
        dt.AcceptChanges();
        string fileName = "Menu Bar Menu Item Association.xlsx";
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            var columns = EntityColumns().Select(p=>p.Value).ToArray();
            dt.SetColumnsOrder(columns);
            //dt.RemoveColumns(new string[] { });
            dt.RemoveColumns((lstHiddenProp.Count > 0) ? lstHiddenProp.ToArray() : new string[] {   });
            wb.Properties.Author = "Menu Bar Designer V3";
            wb.Properties.Title = "Menu Bar Menu Item Association";
            wb.Properties.Subject = "Menu Bar Menu Item Association data";
            wb.Properties.Comments = "Export Excel";
            wb.Properties.LastModifiedBy = User.Name;
            //Add DataTable in worksheet
            wb.Worksheets.Add(dt,"List");
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
    private Dictionary<string,string> EntityColumns()
    {
        Dictionary<string, string> columns = new Dictionary<string, string>();
        var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation").Properties;
        columns.Add("2", modelproperties.FirstOrDefault(q => q.Name == "T_MenuBarID").DisplayName);
        columns.Add("3", modelproperties.FirstOrDefault(q => q.Name == "T_MenuItemID").DisplayName);
        columns.Add("4", modelproperties.FirstOrDefault(q => q.Name == "T_OrderNumber").DisplayName);
        return columns;
    }
    
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstT_MenuBarMenuItemAssociation">The list Menu Bar Menu Item Association.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<T_MenuBarMenuItemAssociation> searchRecords(IQueryable<T_MenuBarMenuItemAssociation> lstT_MenuBarMenuItemAssociation, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
        {
            lstT_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation.Where(s => (s.t_menubar!= null && (s.t_menubar.DisplayValue.ToUpper().Contains(searchString))) ||(s.t_menuitem!= null && (s.t_menuitem.DisplayValue.ToUpper().Contains(searchString))) ||(s.T_OrderNumber != null && SqlFunctions.StringConvert((double)s.T_OrderNumber).Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        }
        else
            lstT_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation.Where(s => (s.t_menubar!= null && (s.t_menubar.DisplayValue.ToUpper().Contains(searchString))) ||(s.t_menuitem!= null && (s.t_menuitem.DisplayValue.ToUpper().Contains(searchString))) ||(s.T_OrderNumber != null && SqlFunctions.StringConvert((double)s.T_OrderNumber).Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstT_MenuBarMenuItemAssociation;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstT_MenuBarMenuItemAssociation">The IQueryable list Menu Bar Menu Item Association.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<T_MenuBarMenuItemAssociation> sortRecords(IQueryable<T_MenuBarMenuItemAssociation> lstT_MenuBarMenuItemAssociation, string sortBy, string isAsc)
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
        if(sortBy == "T_MenuBarID")
            return isAsc.ToLower() == "asc" ? lstT_MenuBarMenuItemAssociation.OrderBy(p => p.t_menubar.DisplayValue) : lstT_MenuBarMenuItemAssociation.OrderByDescending(p => p.t_menubar.DisplayValue);
        if(sortBy == "T_MenuItemID")
            return isAsc.ToLower() == "asc" ? lstT_MenuBarMenuItemAssociation.OrderBy(p => p.t_menuitem.DisplayValue) : lstT_MenuBarMenuItemAssociation.OrderByDescending(p => p.t_menuitem.DisplayValue);
        if(sortBy == "DisplayValue")
            return isAsc.ToLower() == "asc" ? lstT_MenuBarMenuItemAssociation.OrderBy(p => p.DisplayValue) : lstT_MenuBarMenuItemAssociation.OrderByDescending(p => p.DisplayValue);
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstT_MenuBarMenuItemAssociation.Sort(sortBy,true) : lstT_MenuBarMenuItemAssociation.Sort(sortBy,false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(T_MenuBarMenuItemAssociation), "t_menubarmenuitemassociation");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T_MenuBarMenuItemAssociation>)lstT_MenuBarMenuItemAssociation.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstT_MenuBarMenuItemAssociation.ElementType, lambda.Body.Type },
                       lstT_MenuBarMenuItemAssociation.Expression,
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
    public ActionResult SetFSearch(string searchString, string HostingEntity,string t_menubar,string t_menuitem, bool? RenderPartial,  string FsearchId = "", bool ShowDeleted = false)
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
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            var T_MenuBarlist = _fad.FilterDropdown<T_MenuBar>(User, db.T_MenuBars, "T_MenuBar", null);
            SetFSearchDropdownViewBag(T_MenuBarlist, t_menubar, "T_MenuBar", "Menu Bar", "t_menubar");
            var T_MenuItemlist = _fad.FilterDropdown<T_MenuItem>(User, db.T_MenuItems, "T_MenuItem", null);
            SetFSearchDropdownViewBag(T_MenuItemlist, t_menuitem, "T_MenuItem", "Menu Item", "t_menuitem");
        }
        else
        {
            var objT_MenuBar = new List<T_MenuBar>();
            ViewBag.t_menubar = new SelectList(objT_MenuBar, "ID", "DisplayValue");
            var objT_MenuItem = new List<T_MenuItem>();
            ViewBag.t_menuitem = new SelectList(objT_MenuItem, "ID", "DisplayValue");
        }
        SetFSearchViewBag("T_MenuBarMenuItemAssociation");
        ViewBag.HideColumns = new MultiSelectList(EntityColumns(), "Key", "Value");
        ViewBag.FsearchId = FsearchId;
        return View(new T_MenuBarMenuItemAssociation());
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
//public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport ,string t_menubar,string t_menuitem ,string T_OrderNumberFrom,string T_OrderNumberTo,string FilterCondition, string HostingEntity, string AssociatedType,string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn,bool? IsReports, bool? IsdrivedTab, bool? IsFilter=false, bool ShowDeleted = false, string ExportType = null)
    public ActionResult FSearch(T_MenuBarMenuItemAssociationIndexArgsOption args)
    {
        //FSearchViewBag(currentFilter, searchString, FSFilter, sortBy, isAsc, page, itemsPerPage, search, FilterCondition, HostingEntity, AssociatedType, HostingEntityID, viewtype, SortOrder, HideColumns, GroupByColumn, IsReports, IsdrivedTab, IsFilter,"T_MenuBarMenuItemAssociation");
        IndexViewBag(args);
        T_MenuBarMenuItemAssociationIndexViewModel model = new T_MenuBarMenuItemAssociationIndexViewModel(User, args);
        CustomLoadViewDataListOnIndex(model.HostingEntity, Convert.ToInt32(model.HostingEntityID), model.AssociatedType);
        var lstT_MenuBarMenuItemAssociation  = from s in db.T_MenuBarMenuItemAssociations
                                               select s;
        // for Restrict Dropdown
        ViewBag.T_MenuBarMenuItemAssociationRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "T_MenuBarMenuItemAssociation", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        if(!String.IsNullOrEmpty(model.searchString))
        {
            lstT_MenuBarMenuItemAssociation  = searchRecords(lstT_MenuBarMenuItemAssociation, model.searchString.ToUpper(),true);
        }
        if(!string.IsNullOrEmpty(model.search))
            model.search=model.search.Replace("?IsAddPop=true", "");
        if(!string.IsNullOrEmpty(model.search))
        {
            model.SearchResult += "\r\n General Criterial= " + model.search + ",";
            lstT_MenuBarMenuItemAssociation = searchRecords(lstT_MenuBarMenuItemAssociation, model.search,true);
        }
        if(!String.IsNullOrEmpty(model.sortBy) && !String.IsNullOrEmpty(model.IsAsc))
        {
            lstT_MenuBarMenuItemAssociation  = sortRecords(lstT_MenuBarMenuItemAssociation, model.sortBy, model.IsAsc);
        }
        else   lstT_MenuBarMenuItemAssociation  = lstT_MenuBarMenuItemAssociation.OrderByDescending(c => c.Id);
        lstT_MenuBarMenuItemAssociation = CustomSorting(lstT_MenuBarMenuItemAssociation,model.HostingEntity,model.AssociatedType,model.sortBy,model.IsAsc);
        //lstT_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation.IncludeOptimized(t=>t.t_menubar).IncludeOptimized(t=>t.t_menuitem);
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
                model.SearchResult += " " + GetPropertyDP("T_MenuBarMenuItemAssociation", PropertyName) + " " + Operator + " " + Value.Trim() + " " + LogicalConnector;
                whereCondition.Append(conditionFSearch("T_MenuBarMenuItemAssociation",PropertyName, Operator, Value.Trim(),type) + LogicalConnector);
                iCnt++;
            }
            if(!string.IsNullOrEmpty(whereCondition.ToString()))
                lstT_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation.Where(whereCondition.ToString());
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
            lstT_MenuBarMenuItemAssociation = Sorting.Sort<T_MenuBarMenuItemAssociation>(lstT_MenuBarMenuItemAssociation, DataOrdering);
        var _T_MenuBarMenuItemAssociation = lstT_MenuBarMenuItemAssociation;
        if(model.T_OrderNumberFrom!=null || model.T_OrderNumberTo !=null)
        {
            try
            {
                int from = model.T_OrderNumberFrom == null ? 0 : Convert.ToInt32(model.T_OrderNumberFrom);
                int to = model.T_OrderNumberTo == null ? int.MaxValue : Convert.ToInt32(model.T_OrderNumberTo);
                _T_MenuBarMenuItemAssociation =  _T_MenuBarMenuItemAssociation.Where(o => o.T_OrderNumber >= from && o.T_OrderNumber <= to);
                model.SearchResult += "\r\n Order Number= "+from+"-"+to;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        //if (lstT_MenuBarMenuItemAssociation.Where(p => p.t_menubar != null).Count() <= 50)
        //ViewBag.t_menubar = new SelectList(lstT_MenuBarMenuItemAssociation.Where(p => p.t_menubar != null).Select(P => P.t_menubar).Distinct(), "ID", "DisplayValue");
        if(model.t_menubar != null)
        {
            var ids = model.t_menubar.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            model.SearchResult += "\r\n Menu Bar= ";
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
                        var obj = db.T_MenuBars.Find(Convert.ToInt64(str));
                        model.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => ids1.Contains(p.T_MenuBarID));
        }
        //if (lstT_MenuBarMenuItemAssociation.Where(p => p.t_menuitem != null).Count() <= 50)
        //ViewBag.t_menuitem = new SelectList(lstT_MenuBarMenuItemAssociation.Where(p => p.t_menuitem != null).Select(P => P.t_menuitem).Distinct(), "ID", "DisplayValue");
        if(model.t_menuitem != null)
        {
            var ids = model.t_menuitem.Split(",".ToCharArray());
            List<long?> ids1 = new List<long?>();
            model.SearchResult += "\r\n Menu Item= ";
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
                        var obj = db.T_MenuItems.Find(Convert.ToInt64(str));
                        model.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => ids1.Contains(p.T_MenuItemID));
        }
        _T_MenuBarMenuItemAssociation = FilterByHostingEntity(_T_MenuBarMenuItemAssociation, model.HostingEntity, model.AssociatedType, Convert.ToInt32(model.HostingEntityID));
        if(model.viewtype == "Metric")
        {
            var querystring = HttpUtility.ParseQueryString(Request.Url.Query);
            var aggregate = querystring["aggregate"];
            var aggregateproperty = querystring["aggregateproperty"];
            if(!string.IsNullOrEmpty(model.GroupByColumn))
            {
                model.GroupByColumn = model.GroupByColumn.TrimEnd(",".ToCharArray());
                if(model.GroupByColumn.ToLower().EndsWith("id"))
                    model.GroupByColumn = model.GroupByColumn.ToLower().Substring(0,model.GroupByColumn.Length-2)+".DisplayValue";
                if(string.IsNullOrEmpty(aggregateproperty)) aggregateproperty = "Id";
                var entityModel = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation");
                var property = entityModel.Properties.FirstOrDefault(p => p.Name == model.GroupByColumn);
                string dataType = string.Empty;
                string DisplayFormat = string.Empty;
                if(property != null && property.DataTypeAttribute != null)
                {
                    dataType = property.DataTypeAttribute;
                    DisplayFormat = (dataType.ToLower()=="date") ? CustomDisplayFormat.ConvertToMomentFormat(property.DisplayFormat):property.DisplayFormat;
                }
                IQueryable result = null;
                if(model.GroupByColumn.ToLower().Contains("displayvalue") == true)
                    if(aggregate != "Count")
                        result = _T_MenuBarMenuItemAssociation.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it."+aggregate+"(Value) as Y)").OrderBy("X");
                    else
                        result = _T_MenuBarMenuItemAssociation.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key.DisplayValue as X, it."+aggregate+"() as Y)").OrderBy("X");
                else if(aggregate != "Count")
                    result = _T_MenuBarMenuItemAssociation.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "(Value) as Y)").OrderBy("X");
                else
                    result = _T_MenuBarMenuItemAssociation.GroupBy("new(" + model.GroupByColumn + " )", "it." + aggregateproperty).Select("new (it.Key." + model.GroupByColumn + " as X, it." + aggregate + "() as Y)").OrderBy("X");
                var groupbyresult = result.ToListAsync().Result;
                return Json(new { type = "groupby", result = groupbyresult, dataType = dataType, displayFormat = DisplayFormat }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            if(aggregate!= "Count")
            {
                var result = _T_MenuBarMenuItemAssociation.Aggration(aggregateproperty, aggregate);
                var entityName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation");
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
            return Json(_T_MenuBarMenuItemAssociation.Count(), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if((string)model.SearchResult != null)
            model.SearchResult = ((string)model.SearchResult).TrimStart("\r\n".ToCharArray()).TrimEnd(", ".ToCharArray()).TrimEnd(",".ToCharArray());
        model = (T_MenuBarMenuItemAssociationIndexViewModel)SetPagination(model, "T_MenuBarMenuItemAssociation");
        model.PageSize = model.PageSize > 100 ? 100 : model.PageSize;
        if(model.PageSize == -1)
        {
            model.pageNumber = 1;
            var totalcount = _T_MenuBarMenuItemAssociation.Count();
            model.PageSize = totalcount <= 10 ? 10 : totalcount;
        }
        //
        if(Convert.ToBoolean(model.IsExport))
        {
            return DoExport(model, _T_MenuBarMenuItemAssociation);
        }
        else
        {
            if(model.pageNumber > 1)
            {
                var totalListCount = _T_MenuBarMenuItemAssociation.Count();
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
            var list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_MenuBarMenuItemAssociationlist"] = list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_MenuBarMenuItemAssociation", tagsSplit.ToArray());
                }
            model.list = list;
            return View("Index",model);
        }
        else
        {
            var list = _T_MenuBarMenuItemAssociation.ToCachedPagedList(model.pageNumber, model.PageSize);
            ViewBag.EntityT_MenuBarMenuItemAssociationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["T_MenuBarMenuItemAssociationlist"] = list.Select(z => new
            {
                ID = z.Id, DisplayValue = z.DisplayValue
            });
            if(!string.IsNullOrEmpty(model.GroupByColumn))
                foreach(var item in list)
                {
                    var tagsSplit = model.GroupByColumn.Split(',').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));
                    item.m_DisplayValue = EntityComparer.GetGroupByDisplayValue(item, "T_MenuBarMenuItemAssociation", tagsSplit.ToArray());
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
    /// <summary>Renders Graph.</summary>
    ///
    /// <param name="type">Type of Graph.</param>
    /// <param name="inlarge">Inlarge value.</param>
    ///
    /// <returns>html string.</returns>
    public string ShowGraph(string type, int? inlarge)
    {
        return GraphHelper.ShowGraph(db, "T_MenuBarMenuItemAssociation", type, inlarge);
    }
    /// <summary>Renders Graph on Home.</summary>
    ///
    /// <param name="type">Type of Graph.</param>
    /// <param name="inlarge">Inlarge value.</param>
    /// <param name="XAxis">XAxis value.</param>
    ///
    /// <returns>html string.</returns>
    public string ShowGraphEntityHome(string type, int? inlarge, string XAxis)
    {
        return GraphHelper.ShowGraphEntityHome(db, "T_MenuBarMenuItemAssociation", type, inlarge, XAxis);
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_T_MenuBarMenuItemAssociation">IQueryable<T_MenuBarMenuItemAssociation>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<T_MenuBarMenuItemAssociation>.</returns>
    private IQueryable<T_MenuBarMenuItemAssociation> FilterByHostingEntity(IQueryable<T_MenuBarMenuItemAssociation> _T_MenuBarMenuItemAssociation, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        if(HostingEntity == "T_MenuBar" && AssociatedType == "T_MenuBarMenuItemAssociation_T_MenuBar")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => p.T_MenuBarID == hostid);
                ViewBag.HostingEntityIDData = db.T_MenuBars.FirstOrDefault(fd => fd.Id == hostid);
            }
            else
                _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => p.T_MenuBarID == null);
        }
        if(HostingEntity == "T_MenuItem" && AssociatedType == "T_MenuBarMenuItemAssociation_T_MenuItem")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => p.T_MenuItemID == hostid);
                ViewBag.HostingEntityIDData = db.T_MenuItems.FirstOrDefault(fd => fd.Id == hostid);
            }
            else
                _T_MenuBarMenuItemAssociation = _T_MenuBarMenuItemAssociation.Where(p => p.T_MenuItemID == null);
        }
        return _T_MenuBarMenuItemAssociation;
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
        using(var appcontext = (new ApplicationContext(new SystemUser(),true)))
        {
            var _Obj = appcontext.T_MenuBarMenuItemAssociations.FirstOrDefault(p => p.Id == idvalue);
            return  _Obj==null?"":_Obj.DisplayValue;
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
            IQueryable<T_MenuBarMenuItemAssociation> list = appcontext.T_MenuBarMenuItemAssociations;
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_MenuBarMenuItemAssociation>(new SystemUser(), list, "T_MenuBarMenuItemAssociation", caller);
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
    public IQueryable<T_MenuBarMenuItemAssociation> GetIQueryable(ApplicationContext appdb)
    {
        return appdb.T_MenuBarMenuItemAssociations.AsNoTracking();
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
        using(var appcontext = (new ApplicationContext(new SystemUser(),true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(id));
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
            var list = appcontext.T_MenuBarMenuItemAssociations.Where(p => RecordIds.Contains(p.Id));
            var result = searchRecords(list, searchkey, true);
            if(result != null)
                iqueryable = result.Select(p =>p.Id).ToList().AsQueryable();
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
        if(string.IsNullOrEmpty(id)) return Json(new T_MenuBarMenuItemAssociation(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(id));
            long? tenantId = null;
            var _updatedAssocObj = UpdateAssociationValue("T_MenuBarMenuItemAssociation", _Obj, tenantId);
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
        using(var context = (new ApplicationContext(new SystemUser(),true)))
        {
            context.Configuration.LazyLoadingEnabled = false;
            var _Obj = context.T_MenuBarMenuItemAssociations.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_MenuBarMenuItemAssociation>(_Obj, "T_MenuBarMenuItemAssociation", new string[] { ""  });
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
        IQueryable<T_MenuBarMenuItemAssociation> list = db.T_MenuBarMenuItemAssociations;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_MenuBarMenuItemAssociation>(User, list, "T_MenuBarMenuItemAssociation", caller);
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
    public JsonResult GetAllValueDropMenu(string associationName,string RestrictDropdownVal="")
    {
        var list = db.T_MenuBarMenuItemAssociations.OrderBy(p => p.DisplayValue).AsQueryable();//;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_MenuBarMenuItemAssociation>(User, list, "T_MenuBarMenuItemAssociation", associationName+"ID",RestrictDropdownVal);
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
        var list = CustomDropdownFilter(db.T_MenuBarMenuItemAssociations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, ExtraVal, RestrictDropdownVal, CustomParameter, "GetAllValue");
        var result = DropdownHelper.GetAllValue<T_MenuBarMenuItemAssociation>(User, list, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_MenuBarMenuItemAssociation", RestrictDropdownVal);
        return Json(result.Select(x=> new
        {
            Id = x.Id, Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    public ActionResult EditBridgeQuick(string otherId, string UrlReferrer, string HostingEntityName, string AssociatedType, string HostingEntityID, string viewtype)
    {
        if(otherId == null || HostingEntityID == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        if(HostingEntityName == "T_MenuBar")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var otherid = Convert.ToInt64(otherId);
            var obj = db.T_MenuBarMenuItemAssociations.FirstOrDefault(p => p.T_MenuBarID == hostid && p.T_MenuItemID == otherid);
            if(obj == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return RedirectToAction("EditQuick", new { id = obj.Id, UrlReferrer = UrlReferrer, HostingEntityName = HostingEntityName, AssociatedType = AssociatedType });
        }
        if(HostingEntityName == "T_MenuItem")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var otherid = Convert.ToInt64(otherId);
            var obj = db.T_MenuBarMenuItemAssociations.FirstOrDefault(p => p.T_MenuItemID == hostid && p.T_MenuBarID == otherid);
            if(obj == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return RedirectToAction("EditQuick", new { id = obj.Id, UrlReferrer = UrlReferrer, HostingEntityName = HostingEntityName, AssociatedType = AssociatedType });
        }
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
/// <summary>Creates a JSON result with the given data as its content (dropdown - chosen.jquery.js).</summary>
    ///
    /// <param name="SearchString">            The SearchString.</param>
    /// <param name="HostingEntityName">               The HostingEntityName.</param>
    /// <param name="HostingEntityID">HostingEntityID.</param>
    /// <param name="AssociatedType">     AssociatedType.</param>
    /// <param name="size">          size.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValues action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetAllValues(string SearchString, string HostingEntityName, string HostingEntityID, string AssociatedType, string size)
    {
        int pagesize = 50;
        if(HostingEntityName == "T_MenuBar")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuBarID == hostid).Select(q => q.T_MenuItemID);
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            var Finallist = _fad.FilterDropdown<T_MenuItem>(User, db.T_MenuItems.Where(p => !Selectedlist.Contains(p.Id)), "T_MenuItem", "T_MenuBarMenuItemAssociation.T_MenuItemID").ToList();
            if(!string.IsNullOrEmpty(SearchString))
                Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
            else
                Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
            var ListCount = Finallist.Count();
            var Results= false;
            if(!string.IsNullOrEmpty(size))
                if(size == "All")
                    pagesize = ListCount;
                else
                    pagesize = Convert.ToInt32(size);
            if(ListCount > pagesize)
                Results = true;
            var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
            var genericResult = new { data = data, Results = Results };
            return Json(genericResult, JsonRequestBehavior.AllowGet);
        }
        if(HostingEntityName == "T_MenuItem")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuItemID == hostid).Select(q => q.T_MenuBarID);
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            var Finallist = _fad.FilterDropdown<T_MenuBar>(User, db.T_MenuBars.Where(p => !Selectedlist.Contains(p.Id)), "T_MenuBar", "T_MenuBarMenuItemAssociation.T_MenuBarID").ToList();
            if(!string.IsNullOrEmpty(SearchString))
                Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
            else
                Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
            var ListCount = Finallist.Count();
            var Results= false;
            if(!string.IsNullOrEmpty(size))
                if(size == "All")
                    pagesize = ListCount;
                else
                    pagesize = Convert.ToInt32(size);
            if(ListCount > pagesize)
                Results = true;
            var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
            var genericResult = new { data = data, Results = Results };
            return Json(genericResult, JsonRequestBehavior.AllowGet);
        }
        return null;
    }
    /// <summary>Creates a JSON result with the given data as its content (dropdown - chosen.jquery.js).</summary>
    ///
    /// <param name="SearchString">            The SearchString.</param>
    /// <param name="HostingEntityName">               The HostingEntityName.</param>
    /// <param name="HostingEntityID">HostingEntityID.</param>
    /// <param name="AssociatedType">     AssociatedType.</param>
    /// <param name="size">          size.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValues1 action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetAllValues1(string SearchString, string HostingEntityName, string HostingEntityID, string AssociatedType, string size, string t_menubarmenuitemassociation,string t_menuitemmenuitemassociation,string T_Disabled,string T_Horizontal)
    {
        int pagesize = 50;
        if(HostingEntityName == "T_MenuBar")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuBarID == hostid).Select(q => q.T_MenuItemID);
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            var Finallist = _fad.FilterDropdown<T_MenuItem>(User, db.T_MenuItems.Where(p => !Selectedlist.Contains(p.Id)), "T_MenuItem", "T_MenuBarMenuItemAssociation.T_MenuItemID").ToList();
            if(!string.IsNullOrEmpty(SearchString))
                Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
            else
                Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
//ManyToMany search in m.m
            if(t_menubarmenuitemassociation != null)
            {
                var ids = t_menubarmenuitemassociation.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                foreach(var str in ids)
                {
                    //Null Search
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                        {
                            ids1.Add(null);
                        }
                        else
                        {
                            var idvalue= Convert.ToInt64(str);
                            ids1.AddRange(db.T_MenuBarMenuItemAssociations.Where(p=>p.T_MenuBarID ==idvalue).Select(p=>p.T_MenuItemID));
                        }
                    }
                    //
                }
                ids1 = ids1.ToList();
                Finallist = Finallist.Where(p => ids1.Contains(p.Id)).ToList();
            }
            if(t_menuitemmenuitemassociation != null)
            {
                var ids = t_menuitemmenuitemassociation.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                foreach(var str in ids)
                {
                    //Null Search
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                            ids1.Add(null);
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            //var obj = db.T_MenuItems.Find(Convert.ToInt64(str));
                        }
                    }
                    //
                }
                ids1 = ids1.ToList();
                Finallist = Finallist.Where(p => ids1.Contains(p.T_MenuItemMenuItemAssociationID)).ToList();
            }
            var ListCount = Finallist.Count();
            var Results= false;
            if(!string.IsNullOrEmpty(size))
                if(size == "All")
                    pagesize = ListCount;
                else
                    pagesize = Convert.ToInt32(size);
            if(ListCount > pagesize)
                Results = true;
            var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
            var genericResult = new { data = data, Results = Results };
            return Json(genericResult, JsonRequestBehavior.AllowGet);
        }
        if(HostingEntityName == "T_MenuItem")
        {
            var hostid = Convert.ToInt64(HostingEntityID);
            var Selectedlist = db.T_MenuBarMenuItemAssociations.Where(p => p.T_MenuItemID == hostid).Select(q => q.T_MenuBarID);
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            var Finallist = _fad.FilterDropdown<T_MenuBar>(User, db.T_MenuBars.Where(p => !Selectedlist.Contains(p.Id)), "T_MenuBar", "T_MenuBarMenuItemAssociation.T_MenuBarID").ToList();
            if(T_Disabled!=null)
            {
                try
                {
                    bool boolvalue = Convert.ToBoolean(T_Disabled);
                    Finallist =  Finallist.Where(o => o.T_Disabled == boolvalue).ToList();
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            if(T_Horizontal!=null)
            {
                try
                {
                    bool boolvalue = Convert.ToBoolean(T_Horizontal);
                    Finallist =  Finallist.Where(o => o.T_Horizontal == boolvalue).ToList();
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            if(!string.IsNullOrEmpty(SearchString))
                Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
            else
                Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
//ManyToMany search in m.m
            if(t_menubarmenuitemassociation != null)
            {
                var ids = t_menubarmenuitemassociation.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                foreach(var str in ids)
                {
                    //Null Search
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                        {
                            ids1.Add(null);
                        }
                        else
                        {
                            var idvalue= Convert.ToInt64(str);
                            ids1.AddRange(db.T_MenuBarMenuItemAssociations.Where(p=>p.T_MenuItemID ==idvalue).Select(p=>p.T_MenuBarID));
                        }
                    }
                    //
                }
                ids1 = ids1.ToList();
                Finallist = Finallist.Where(p => ids1.Contains(p.Id)).ToList();
            }
            var ListCount = Finallist.Count();
            var Results= false;
            if(!string.IsNullOrEmpty(size))
                if(size == "All")
                    pagesize = ListCount;
                else
                    pagesize = Convert.ToInt32(size);
            if(ListCount > pagesize)
                Results = true;
            var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
            var genericResult = new { data = data, Results = Results };
            return Json(genericResult, JsonRequestBehavior.AllowGet);
        }
        return null;
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
        var result = DropdownHelper.GetAllValueForRB<T_MenuBarMenuItemAssociation>(User, db.T_MenuBarMenuItemAssociations, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_MenuBarMenuItemAssociation");
        return Json(result.Select(x => new
        {
            Id = x.Id, Name = x.DisplayValue
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
        IQueryable<T_MenuBarMenuItemAssociation> list = db.T_MenuBarMenuItemAssociations;
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
    public JsonResult GetAllMultiSelectValue(string caller,string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string CustomParameter = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.T_MenuBarMenuItemAssociations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, null, null, CustomParameter, "GetAllMultiSelectValue");
        var result  = DropdownHelper.GetAllMultiSelectValue(User, list, key, AssoNameWithParent, AssociationID, "T_MenuBarMenuItemAssociation", caller);
        return Json(result.Select(x => new
        {
            Id = x.Id, Name = x.DisplayValue
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
    public JsonResult GetMultiSelectValueAllSelection(string caller,string key, string AssoNameWithParent, string AssociationID, string bulkAdd = null, string bulkSelection = null, string CustomParameter = null)
    {
        if(caller != null)
            caller = caller.Replace("?", "");
        var list = CustomDropdownFilter(db.T_MenuBarMenuItemAssociations.AsNoTracking(), caller, key, AssoNameWithParent, AssociationID, null, null, CustomParameter, "GetMultiSelectValueAllSelection");
        var result  = DropdownHelper.GetMultiSelectValueAllSelection(User, list, key, AssoNameWithParent, AssociationID, "T_MenuBarMenuItemAssociation", caller);
        return Json(result.Select(x => new
        {
            Id = x.Id, Name = x.DisplayValue
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
        case "T_MenuBarID":
            columnValue = db.T_MenuBars.FirstOrDefault(p => p.Id == id).DisplayValue;
            break;
        case "T_MenuItemID":
            columnValue = db.T_MenuItems.FirstOrDefault(p => p.Id == id).DisplayValue;
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
    public ActionResult Upload(string FileType, string AssociatedType, string HostingEntityName, string HostingEntityID, string UrlReferrer,string ImportType="")
    {
        if(FileType.ToLower() == "csv")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportCSV", "T_MenuBarMenuItemAssociation", User) || !User.CanAdd("T_MenuBarMenuItemAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else if(FileType.ToLower() == "xls" || FileType.ToLower() == "xlsx")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "T_MenuBarMenuItemAssociation", User) || !User.CanAdd("T_MenuBarMenuItemAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_MenuBarMenuItemAssociation")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_MenuBarMenuItemAssociation").ToList();
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
        string typeName = "T_MenuBarMenuItemAssociation";
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
                    allcolumns = new string[] { "T_MenuBarID","T_MenuItemID","T_OrderNumber" };
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
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Menu Bar Menu Item Association</b>";
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
                                case "T_MenuBarID":
                                    var t_menubarId = db.T_MenuBars.FirstOrDefault(p => p.DisplayValue == assovalue);
                                    if(t_menubarId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                case "T_MenuItemID":
                                    var t_menuitemId = db.T_MenuItems.FirstOrDefault(p => p.DisplayValue == assovalue);
                                    if(t_menuitemId == null)
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
        string typename = "T_MenuBarMenuItemAssociation";
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
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>Menu Bar Menu Item Association</b>";
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
                        case "T_MenuBarID":
                            var strPropertyT_MenuBar = lstEntityProp.FirstOrDefault(p => p.Key == "T_MenuBarID").Value;
                            ModelReflector.Property propT_MenuBar = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBar").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_MenuBar);
                            var t_menubarId = db.T_MenuBars.Where(propT_MenuBar.Name + "=(@0)", assovalue).FirstOrDefault();
                            //var t_menubarId = db.T_MenuBars.Where(propT_MenuBar.Name+"=\""+assovalue+"\"").FirstOrDefault();
                            if(t_menubarId == null)
                                uniqueassoValues.Add(assovalue);
                            break;
                        case "T_MenuItemID":
                            var strPropertyT_MenuItem = lstEntityProp.FirstOrDefault(p => p.Key == "T_MenuItemID").Value;
                            ModelReflector.Property propT_MenuItem = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuItem").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_MenuItem);
                            var t_menuitemId = db.T_MenuItems.Where(propT_MenuItem.Name + "=(@0)", assovalue).FirstOrDefault();
                            //var t_menuitemId = db.T_MenuItems.Where(propT_MenuItem.Name+"=\""+assovalue+"\"").FirstOrDefault();
                            if(t_menuitemId == null)
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
                    T_MenuBarMenuItemAssociation model = new T_MenuBarMenuItemAssociation();
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
                        case "T_OrderNumber":
                            model.T_OrderNumber = Int32.Parse(columnValue);
                            break;
                        case "T_MenuBarID":
                            dynamic t_menubarId = null;
                            if(AssociatedType != null && AssociatedType == "T_MenuBar" && HostingEntityID != null)
                            {
                                long id = Convert.ToInt64(HostingEntityID);
                                model.T_MenuBarID = id;
                            }
                            else
                            {
                                if(lstEntityProp.Count > 0)
                                {
                                    var strPropertyT_MenuBar = lstEntityProp.FirstOrDefault(p => p.Key == "T_MenuBarID").Value;
                                    ModelReflector.Property propT_MenuBar = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBar").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_MenuBar);
                                    if(propT_MenuBar.Name == "DisplayValue")
                                        t_menubarId = db.T_MenuBars.AsNoTracking().FirstOrDefault(p=>p.DisplayValue == columnValue);
                                    else
                                        t_menubarId = db.T_MenuBars.AsNoTracking().Where(propT_MenuBar.Name + "=\"" + columnValue + "\"").FirstOrDefault();
                                }
                                else
                                    t_menubarId = db.T_MenuBars.AsNoTracking().FirstOrDefault(p=>p.DisplayValue == columnValue);
                            }
                            if(t_menubarId != null)
                                model.T_MenuBarID = t_menubarId.Id;
                            else
                            {
                                if((collection["T_MenuBarID"] != null) && (collection["T_MenuBarID"].ToString() == "true,false"))
                                {
                                    try
                                    {
                                        T_MenuBar objT_MenuBar = new T_MenuBar();
                                        objT_MenuBar.T_Name  = (columnValue);
                                        db.T_MenuBars.Add(objT_MenuBar);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                            db.Entry(objT_MenuBar).State = EntityState.Detached;
                                            continue;
                                        }
                                        model.T_MenuBarID = objT_MenuBar.Id;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                            break;
                        case "T_MenuItemID":
                            dynamic t_menuitemId = null;
                            if(AssociatedType != null && AssociatedType == "T_MenuItem" && HostingEntityID != null)
                            {
                                long id = Convert.ToInt64(HostingEntityID);
                                model.T_MenuItemID = id;
                            }
                            else
                            {
                                if(lstEntityProp.Count > 0)
                                {
                                    var strPropertyT_MenuItem = lstEntityProp.FirstOrDefault(p => p.Key == "T_MenuItemID").Value;
                                    ModelReflector.Property propT_MenuItem = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuItem").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_MenuItem);
                                    if(propT_MenuItem.Name == "DisplayValue")
                                        t_menuitemId = db.T_MenuItems.AsNoTracking().FirstOrDefault(p=>p.DisplayValue == columnValue);
                                    else
                                        t_menuitemId = db.T_MenuItems.AsNoTracking().Where(propT_MenuItem.Name + "=\"" + columnValue + "\"").FirstOrDefault();
                                }
                                else
                                    t_menuitemId = db.T_MenuItems.AsNoTracking().FirstOrDefault(p=>p.DisplayValue == columnValue);
                            }
                            if(t_menuitemId != null)
                                model.T_MenuItemID = t_menuitemId.Id;
                            else
                            {
                                if((collection["T_MenuItemID"] != null) && (collection["T_MenuItemID"].ToString() == "true,false"))
                                {
                                    try
                                    {
                                        T_MenuItem objT_MenuItem = new T_MenuItem();
                                        objT_MenuItem.T_Name  = (columnValue);
                                        db.T_MenuItems.Add(objT_MenuItem);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                            db.Entry(objT_MenuItem).State = EntityState.Detached;
                                            continue;
                                        }
                                        model.T_MenuItemID = objT_MenuItem.Id;
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
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, "T_MenuBarMenuItemAssociation", mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User,db,model,"T_MenuBarMenuItemAssociation");
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", "T_MenuBarMenuItemAssociation");
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                                //if (result == null || result.Count == 0)
                            {
                                var customerror = "";
                                if(!CustomSaveOnImport(model, out customerror,i))
                                {
                                    if(!flagUpdate)
                                    {
                                        db.T_MenuBarMenuItemAssociations.Add(model);
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
                            ViewBag.ImportError = "Row No : " + (i + 1) + " "+ AlrtMsg;
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
                        ViewBag.ImportError += "<br/><br/> <b>Import error message:</b> Imported only " + (countTotalRows - countErrorRows) + " of "+ countTotalRows +" rows.";
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
        string MainEntityName = "T_MenuBarMenuItemAssociation";
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
        var Db=db;
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
                    T_MenuBarMenuItemAssociation model = new T_MenuBarMenuItemAssociation();
                    var tblColumns = lstMaped;
                    model = LoadObjectFromSheet(model, sheetColumns, tblColumns, i, objDataSet, AssociatedType, HostingEntityID, lstEntityProp, mappedColumns, MainEntityName,Db) as T_MenuBarMenuItemAssociation;
                    var MatchColumns = collection["hdnListChkUpdate"];
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, MainEntityName, mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model as T_MenuBarMenuItemAssociation, "ImportData");
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
                                if(!CustomSaveOnImport(model as T_MenuBarMenuItemAssociation, out customerror, i))
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
            using(var appcontext  = (new ApplicationContext(new SystemUser(),true)))
            {
                var obj1 = appcontext.T_MenuBarMenuItemAssociations.Where(p=>p.Id == Id).FirstOrDefault();
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
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleOnSaving action.</returns>
    private JsonResult ApplyBusinessRuleOnSaving(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_MenuBarMenuItemAssociation").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            //var ruleids = businessrule.Select(q => q.Id).ToList();
            //var typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(User,db,t_menubarmenuitemassociation, "OnEdit","T_MenuBarMenuItemAssociation");
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
                var resultBR = GetMandatoryPropertiesDictionary(User,db,t_menubarmenuitemassociation, "OnEdit","T_MenuBarMenuItemAssociation");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(!dic.Key.Contains("FailureMessage"))
                        {
                            var type = t_menubarmenuitemassociation.GetType();
                            if(type.GetProperty(dic.Key) != null)
                            {
                                var propertyvalue = type.GetProperty(dic.Key).GetValue(t_menubarmenuitemassociation, null);
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
                var resultBR = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User,db,t_menubarmenuitemassociation, "OnEdit","T_MenuBarMenuItemAssociation");
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
    /// <param name="t_menubarmenuitemassociation">       The Menu Bar Menu Item Association.</param>
    /// <param name="IsReadOnlyIgnore">(Optional) True if is read only ignored, false if not.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBefore action.</returns>
    private JsonResult ApplyBusinessRuleBefore(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, bool IsReadOnlyIgnore=false)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_MenuBarMenuItemAssociation").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(User, db,t_menubarmenuitemassociation,"T_MenuBarMenuItemAssociation").Where(p => p.Key.Contains("FailureMessage"));
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
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User,db,t_menubarmenuitemassociation,"T_MenuBarMenuItemAssociation").Where(p => p.Key.Contains("FailureMessage"));
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
    public JsonResult GetReadOnlyProperties(T_MenuBarMenuItemAssociation OModel,string ruleType=null)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
        return Json(GetReadOnlyPropertiesDictionary(User, db, OModel, "T_MenuBarMenuItemAssociation",ruleType), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON (Mandatory business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetMandatoryProperties(T_MenuBarMenuItemAssociation OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
        return Json(GetMandatoryPropertiesDictionary(User,db,OModel,ruleType,"T_MenuBarMenuItemAssociation"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetUIAlertBusinessRules(T_MenuBarMenuItemAssociation OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MenuBarMenuItemAssociation").ToList();
            var BRAll = BR;
            if(ruleType == "OnCreate")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
            else if(ruleType == "OnEdit")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "T_MenuBarMenuItemAssociation");
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
    /// <returns>T_MenuBarMenuItemAssociation object.</returns>
    public T_MenuBarMenuItemAssociation UpdateHiddenProperties(T_MenuBarMenuItemAssociation OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.T_MenuBarMenuItemAssociations.Find(OModel.Id);
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
    public JsonResult GetHiddenVerb(T_MenuBarMenuItemAssociation OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MenuBarMenuItemAssociation").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.GetHiddenVerb(OModel, BR, "T_MenuBarMenuItemAssociation");
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
    public Dictionary<string, string> GetHiddenVerbDetails(T_MenuBarMenuItemAssociation OModel, string ruleType, string pagetype = null)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        //var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MenuBarMenuItemAssociation").ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.GetHiddenVerb(OModel, BR, "T_MenuBarMenuItemAssociation");
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
    public JsonResult GetValidateBeforeSaveProperties(T_MenuBarMenuItemAssociation OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
        return Json(GetValidateBeforeSavePropertiesDictionary(User,db,OModel,ruleType,"T_MenuBarMenuItemAssociation"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSavePropertiesForPopupConfirm action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSavePropertiesForPopupConfirm(T_MenuBarMenuItemAssociation  OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
        return Json(GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "T_MenuBarMenuItemAssociation"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Lock record business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    [ValidateAntiForgeryToken]
    public JsonResult GetLockBusinessRules(T_MenuBarMenuItemAssociation OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_MenuBarMenuItemAssociation")));
        return Json(GetLockBusinessRulesDictionary(User,db,OModel,"T_MenuBarMenuItemAssociation"), JsonRequestBehavior.AllowGet);
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
            var _Obj = db1.T_MenuBarMenuItemAssociations.FirstOrDefault(p => p.DisplayValue == displayvalue);
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
            IQueryable query = context.T_MenuBarMenuItemAssociations;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(T_MenuBarMenuItemAssociation), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if(PropValue.ToLower().Trim() != "null")
            {
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    PropValue = GetIdofDisplayValueforSetIfZero(PropName, "T_MenuBarMenuItemAssociation", PropValue);
                    System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                    object PropValue1 = typeConverter.ConvertFromString(PropValue);
                    lambda = Expression.Lambda<Func<T_MenuBarMenuItemAssociation, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue1), member.Type)), p);
                }
                else
                {
                    lambda = Expression.Lambda<Func<T_MenuBarMenuItemAssociation, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
                }
            }
            else
                lambda = Expression.Lambda<Func<T_MenuBarMenuItemAssociation, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<T_MenuBarMenuItemAssociation>)q);
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
        using(var context = (new ApplicationContext(new SystemUser(),true)))
        {
            if(!string.IsNullOrEmpty(propertyofAssociation))
            {
                var AssocpropValue = GetFieldValueByAssocationId(Id, PropName, propertyofAssociation);
                return Json(AssocpropValue, JsonRequestBehavior.AllowGet);
            }
            var obj1 = context.T_MenuBarMenuItemAssociations.Find(Id);
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
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation");
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
            T_MenuBarMenuItemAssociation obj1 = context.T_MenuBarMenuItemAssociations.Find(Id);
            if(obj1 != null)
            {
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(q => q.PropertyType.FullName.StartsWith("System")).ToArray();
                //
                string propToWhere = PropName;
                ParameterExpression p = Expression.Parameter(typeof(T_MenuBarMenuItemAssociation), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                //LambdaExpression lambda = null;
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(member.Type);
                if(PropName.Substring(PropName.Length - 2) == "ID")
                {
                    value = GetIdofDisplayValueforSet(PropName, "T_MenuBarMenuItemAssociation", value);
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
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    [HttpPost]
    public JsonResult Check1MThresholdValue(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation)
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
        string[] Verbsarr = new string[] { "BulkUpdate","BulkDelete","ImportExcel","ImportCSV","ExportExcel","ExportCSV","ImportExcelAdvanced","ImportCSVAdvanced" };
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
        string[][] labelarr = new string[][] {  };
        return labelarr;
    }
    
    /// <summary>code for list of groups.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <returns>An array of string[].</returns>
    public string[][] getGroupsName()
    {
        string[][] groupsarr = new string[][] {  };
        return groupsarr;
    }
/// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="t_menubarmenuitemassociation">The Menu Bar Menu Item Association.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetCalculationValues(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation)
    {
        t_menubarmenuitemassociation.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_menubarmenuitemassociation.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MenuBarMenuItemAssociation");
        string DataType = string.Empty;
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_menubarmenuitemassociation">     The Menu Bar Menu Item Association.</param>
    /// <param name="IgnoreEditable">(Optional) The ignore editable.</param>
    /// <param name="source">        (Optional) Source.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetails(T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, string IgnoreEditable=null, string source=null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_menubarmenuitemassociation.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">          The host.</param>
    /// <param name="value">         The value.</param>
    /// <param name="t_menubarmenuitemassociation">     The Menu Bar Menu Item Association.</param>
    /// <param name="IgnoreEditable">The ignored editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetailsInline(string host, string value, T_MenuBarMenuItemAssociation t_menubarmenuitemassociation, string IgnoreEditable)
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
        List<string> list = new List<string> {  };
        return Json(list, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Gets inline grid associations of entity.</summary>
    ///
    /// <returns>A response stream to send to the getInlineGridAssociationsOfEntity View.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public ActionResult getInlineGridAssociationsOfEntity()
    {
        List<string> list = new List<string> {  };
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
    public async Task<ActionResult> BulkDocumentDownload(string Ids,bool Isflat = false)
    {
        var entityName = "T_MenuBarMenuItemAssociation";
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
        var t_menubarmenuitemassociations = await db.T_MenuBarMenuItemAssociations.Where(s => eIds.Contains(s.Id)).ToListAsync();
        foreach(var t_menubarmenuitemassociation in t_menubarmenuitemassociations)
        {
            await AddDownloadDirectoryForEntity(dir, entityName, t_menubarmenuitemassociation);
        }
        dir = dir.Children.Count() == 1 ? dir.Children[0] : dir;
        //if (dir.Children.Count == 0 && dir.Parent == null && dir.Files.Count == 0)
        // return Json(new { path = "", result = "fail" }, JsonRequestBehavior.AllowGet);
        var bytes = await ZipDirectory(dir, Isflat);
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.zip", GetProperFileDirName(dir.Name) + "_" + String.Format("{0:MM-dd-yyyy HH-mm-ss-ffff}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), t_menubarmenuitemassociations.FirstOrDefault().m_Timezone))));
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
        if(!User.CanView("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
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
    public ActionResult ShowHideColumns(string HostingEntityName, string HostingEntityID, string AssociatedType,string viewtype)
    {
        if(!User.CanAdd("T_MenuBarMenuItemAssociation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
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
        ViewBag.EntityName = "T_MenuBarMenuItemAssociation";
        ViewBag.EntityId = id;
        ViewBag.T_Document = DocumentId;
        ViewBag.T_Name = documentName;
        ViewBag.T_DefaultOutputFormat = outputFormat;
        ViewBag.isdownload = false;
        ViewBag.ispreview = true;
        return View("~/Views/Shared/_ViewPDF.cshtml");
    }
    
    [Audit("Generate Document")]
    public ActionResult GenerateDocument(int? id, int? DocumentId, string documentName, string outputFormat, bool? isdownload, bool? ispreview, string forcedOutput="")
    {
        T_MenuBarMenuItemAssociation t_menubarmenuitemassociation = null;
        if(db != null)
        {
            t_menubarmenuitemassociation = db.T_MenuBarMenuItemAssociations.AsNoTracking().FirstOrDefault(r => r.Id == id);
            t_menubarmenuitemassociation.setDateTimeToClientTime();
        }
        documentName = documentName + "_" + t_menubarmenuitemassociation.DisplayValue + " " + String.Format("{0:MM-dd-yyyy HH-mm}", TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), t_menubarmenuitemassociation.m_Timezone));
        var bytedata =  new ApplicationContext(new SystemUser()).Documents.AsNoTracking().Where(p => p.Id == DocumentId).Select(p => p.Byte).FirstOrDefault();
        //Custom Validate Before Generate
        var result =  ValidateBeforeDocGenerated(t_menubarmenuitemassociation, documentName, outputFormat, isdownload);
        if(!string.IsNullOrEmpty(result)) return Json(result, JsonRequestBehavior.AllowGet);
        using(Stream stream = new MemoryStream(bytedata))
        {
            if(!string.IsNullOrEmpty(forcedOutput))
                outputFormat = forcedOutput;
            var document = GemBox.Document.DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);
            //OnGenerating - Modify object before generating document
            OnDocGenerating(document, t_menubarmenuitemassociation, documentName, outputFormat, isdownload);
            document.MailMerge.ClearOptions = GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyRanges | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveUnusedFields | GemBox.Document.MailMerging.MailMergeClearOptions.RemoveEmptyParagraphs;
            document.MailMerge.Execute(t_menubarmenuitemassociation);
            Stream outstream = new MemoryStream();
            if(outputFormat.ToLower() == "docx")
                document.Save(outstream, GemBox.Document.SaveOptions.DocxDefault);
            else document.Save(outstream, GemBox.Document.SaveOptions.PdfDefault);
            //AfterGenerate - Do some stuff after doc generated e.g. attach in an entity or send email
            AfterDocGenerated(t_menubarmenuitemassociation, documentName, outputFormat, isdownload, outstream);
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
    
    public ActionResult ExportDataNote(int btnid, string Ids)
    {
        var model = db.T_ExportDataConfigurations.Find(btnid);
        ViewBag.Ids = Ids;
        ViewBag.EntityName = "T_MenuBarMenuItemAssociation";
        return View("~/Views/Shared/_ExportDataNote.cshtml", model);
    }
    
    public List<ExportDataKey> visitedList = new List<ExportDataKey>();
    public List<ExportDataKey> KeysData = new List<ExportDataKey>();
    public List<ExportDataDeleteSet> DeleteSets = new List<ExportDataDeleteSet>();
    public ActionResult ExportData(string Ids, int btnid, string Notes)
    {
        if(Ids == null)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var lstIds = Ids.Split(',').Where(wh => !string.IsNullOrEmpty(wh)).Select(int.Parse).ToList();
        var IsBulk = lstIds.Count > 1 ? true : false;
        var entities = ModelReflector.Entities;
        var BulkfolderPath = string.Empty;
        var zipvalue = string.Empty;
        var Bulkzipvalue = string.Empty;
        if(IsBulk)
        {
            BulkfolderPath = Path.Combine(Server.MapPath("~/Files/" + entities.FirstOrDefault(fd => fd.Name == "T_MenuBarMenuItemAssociation").DisplayName + "BulkExport_" + DateTime.UtcNow.Ticks));
            Bulkzipvalue = entities.FirstOrDefault(fd => fd.Name == "T_MenuBarMenuItemAssociation").DisplayName + "-BulkExport-" + DateTime.UtcNow.ToString("dd-MM-yyyy hh-mm-ss") + " UTC";
        }
        foreach(var id in lstIds)
        {
            var zipdata = db.T_MenuBarMenuItemAssociations.Find(id);
            zipvalue = entities.FirstOrDefault(fd => fd.Name == "T_MenuBarMenuItemAssociation").DisplayName + "-" + zipdata.DisplayValue + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy hh-mm-ss") + " UTC";
            var objExportDataConfiguration = db.T_ExportDataConfigurations.FirstOrDefault(fd => fd.Id == btnid);
            var IsSoftDeleteEnabled = false;
            var datacntlog = new StringBuilder();
            KeysData = KeysList(objExportDataConfiguration.t_exportdataconfigurationexportdatadetailsassociation.ToList());
            string folderPath = string.Empty;
            if(IsBulk)
            {
                folderPath = Path.Combine(BulkfolderPath, zipdata.DisplayValue);
            }
            else
            {
                folderPath = Path.Combine(Server.MapPath("~/Files/" + entities.FirstOrDefault(fd => fd.Name == "T_MenuBarMenuItemAssociation").DisplayName + "_" + DateTime.UtcNow.Ticks));
                BulkfolderPath = folderPath;
            }
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            foreach(var item in KeysData)
            {
                if(visitedList.Contains(item)) continue;
                var rootlist = db.T_MenuBarMenuItemAssociations.Where(wh => wh.Id == id);
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
                IsSoftDeleteEnabled = bool.Parse(CommonFunction.Instance.SoftDeleteEnabled()) && bool.Parse(IsEnableDelete.ToString());
                if(IsSoftDeleteEnabled)
                    if(datalist.Count() > 0)
                    {
                        DeleteSets.Add(new ExportDataDeleteSet()
                        {
                            Id = item.Id, EntityName = item.ChildEntity, Query = datalist
                        });
                    }
                DataTable dt = ExcelExportHelper.ToDataTable<object>(User, datalist.ToListAsync().Result.ToList(), item.ChildEntity, item.ParentEntity, item.AssociationName, folderPath, childfolderPath, IsSelf);
                lstcondition.Add(dt.TableName, conditioncriteria(item.Id, item.ChildEntity));
                ds.Tables.Add(dt);
                visitedList.Add(item);
                var childlist = IsSelf ? new List<ExportDataKey>() : KeysData.Where(wh => wh.ParentEntity == item.ChildEntity);
                foreach(var citem in childlist)
                {
                    recursivemethod(citem, datalist, visitedList, ds, KeysData, IsSoftDeleteEnabled, folderPath, childfolderPath, lstcondition);
                }
                string fileName = entitydp + ".xlsx";
                datacntlog.Append(ReadmeLog(ds, fileName));
                using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                {
                    wb.Properties.Author = "Menu Bar Designer V3";
                    wb.Properties.Title = entitydp;
                    wb.Properties.Subject = entitydp + " data";
                    wb.Properties.Comments = "Export Excel";
                    wb.Properties.LastModifiedBy = User.Name;
                    wb.Worksheets.Add(ds);
                    foreach(var wsitem in wb.Worksheets)
                    {
                        foreach(var dtrow in wsitem.Cells().Where(s => s.Value.ToString().StartsWith("=HYPERLINK")))
                        {
                            var data = dtrow.Value.ToString().Replace("=HYPERLINK", "").Split(',');
                            dtrow.Value = data[1].ToString();
                            dtrow.Hyperlink = new ClosedXML.Excel.XLHyperlink(@"../"+data[0].ToString());
                        }
                    }
                    var wrksht = wb.Worksheets.Add("Summary", 0);
                    wrksht.Cell("A1").Value = "EXPORT DATA SUMMARY";
                    wrksht.Range("A1:B1").Merge().Style.Font.SetBold().Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    wrksht.Columns("A").Style.Font.SetBold();
                    wrksht.Cell("A2").Value = "Action";
                    wrksht.Cell("B2").Value = objExportDataConfiguration.T_Name;
                    wrksht.Cell("A3").Value = "Is Deleted";
                    wrksht.Cell("B3").Value = (objExportDataConfiguration.T_EnableDelete.HasValue && objExportDataConfiguration.T_EnableDelete.Value) ? "Yes" : "No";
                    wrksht.Cell("A4").Value = "Exported By";
                    wrksht.Cell("B4").Value = User.Name;
                    wrksht.Cell("A5").Value = "Exported On";
                    wrksht.Cell("B5").Value = DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss") + " UTC";
                    wrksht.Cell("A7").Value = "Record Information";
                    wrksht.Cell("B7").Value = zipdata.Id.ToString() + " - " + zipdata.DisplayValue;
                    wrksht.Cell("A8").Value = "Note";
                    wrksht.Cell("B8").Value = Notes;
                    int clmcnt = 10;
                    foreach(DataTable tbl in ds.Tables)
                    {
                        var condition = lstcondition.Count > 0 ? lstcondition.Where(wh => wh.Key == tbl.TableName).FirstOrDefault().Value : string.Empty;
                        wrksht.Cell("A" + clmcnt).Value = tbl.TableName;
                        wrksht.Cell("B" + clmcnt).Value = tbl.Rows.Count.ToString() + (!string.IsNullOrEmpty(condition) ? " (" + condition + ")" : string.Empty);
                        clmcnt++;
                    }
                    wrksht.Rows().AdjustToContents();
                    wrksht.Columns().AdjustToContents();
                    wrksht.CellsUsed().Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    wrksht.CellsUsed().Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    wrksht.CellsUsed().Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    wrksht.CellsUsed().Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    wb.SaveAs(Path.Combine(childfolderPath, fileName));
                }
            }
            var exportdatalog = new T_ExportDataLog();
            using(var dbnew = new ApplicationContext(User))
            {
                exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID = btnid;
                exportdatalog.T_AssociatedExportDataLogStatusID = dbnew.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Open").Id;;
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
                DoAuditEntry.AddJournalEntryRecordId("T_MenuBarMenuItemAssociation", User.Name, objExportDataConfiguration.T_Name, zipdata.DisplayValue, Convert.ToString(id));
                var readmelog = new StringBuilder();
                readmelog.AppendLine("EXPORT DATA SUMMARY");
                readmelog.AppendLine("------------------------------------------------------------------------------------------");
                readmelog.AppendLine("Action - " + objExportDataConfiguration.T_Name);
                readmelog.AppendLine("Is Deleted - " + ((objExportDataConfiguration.T_EnableDelete.HasValue && objExportDataConfiguration.T_EnableDelete.Value) ? "Yes" : "No"));
                readmelog.AppendLine("Exported By - " + User.Name);
                readmelog.AppendLine("Exported On - " + DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss")+ " UTC");
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
                }
            }
        }
        if(IsBulk)
            zipvalue = Bulkzipvalue;
        return DownloadZipFile(BulkfolderPath, zipvalue);
    }
    public StringBuilder ReadmeLog(DataSet ds, string FileName)
    {
        var log = new StringBuilder();
        log.AppendLine(FileName);
        foreach(DataTable tbl in ds.Tables)
        {
            log.AppendLine(tbl.TableName + " - " + tbl.Rows.Count.ToString());
        }
        log.AppendLine("------------------------------------------------------------------------------------------");
        return log;
    }
    public string conditioncriteria(long Id, string entity)
    {
        StringBuilder whereCondition = new StringBuilder();
        var conditions = (new ConditionContext()).Conditions.Where(p => p.ExportDetailConditionsID == Id).ToList();
        int iCnt = 1;
        foreach(var cond in conditions)
        {
            var PropertyName = SearchHelper.GetPropertyDP(entity, cond.PropertyName);
            var Operator = cond.Operator;
            var Value = cond.Value;
            var LogicalConnector = cond.LogicalConnector;
            var type = string.Empty;
            if(iCnt == conditions.Count())
                LogicalConnector = string.Empty;
            whereCondition.Append(PropertyName + " " + Operator + " " + Value + (!string.IsNullOrEmpty(LogicalConnector) ? " " + LogicalConnector +" " : string.Empty));
            whereCondition.Replace("[", "").Replace("]", "");
            iCnt++;
        }
        return whereCondition.ToString();
    }
    public IQueryable filtercondtion(IQueryable datalist, long Id, string entity)
    {
        StringBuilder whereCondition = new StringBuilder();
        var conditions = (new ConditionContext()).Conditions.Where(p => p.ExportDetailConditionsID == Id).ToList();
        int iCnt = 1;
        foreach(var cond in conditions)
        {
            var PropertyName = cond.PropertyName;
            var Operator = cond.Operator;
            var Value = cond.Value;
            var LogicalConnector = cond.LogicalConnector;
            var type = string.Empty;
            if(iCnt == conditions.Count())
                LogicalConnector = "";
            whereCondition.Append(conditionFSearch(entity, PropertyName, Operator, Value.Trim(), type) + LogicalConnector);
            iCnt++;
        }
        if(!string.IsNullOrEmpty(whereCondition.ToString()))
            return datalist.Where(whereCondition.ToString());
        else
            return datalist;
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
                    Id = citem.Id, EntityName = citem.ChildEntity, Query = childdatalist
                });
            }
        DataTable cdt = ExcelExportHelper.ToDataTable<object>(User, childdatalist.ToListAsync().Result.ToList(), citem.ChildEntity, citem.ParentEntity, citem.AssociationName, BasePath, folderPath, IsSelf);
        lstcondition.Add(cdt.TableName, conditioncriteria(citem.Id, citem.ChildEntity));
        ds.Tables.Add(cdt);
        visitedList.Add(citem);
        var childlist = IsSelf ? new List<ExportDataKey>() : KeysData.Where(wh => wh.ParentEntity == citem.ChildEntity && wh.Hierarchy[0] == citem.Hierarchy[0]);
        foreach(var ccitem in childlist)
        {
            recursivemethod(ccitem, childdatalist, visitedList, ds, KeysData, IsSoftDeleteEnabled, BasePath, folderPath, lstcondition);
        }
    }
    public List<ExportDataKey> KeysList(List<T_ExportDataDetails> exportDataDetails)
    {
        var KeyData = new List<ExportDataKey>();
        foreach(var item in exportDataDetails)
        {
            var objdata = new ExportDataKey();
            objdata.Id = item.Id;
            objdata.ParentEntity = item.T_ParentEntity.Trim();
            objdata.ChildEntity = item.T_ChildEntity.Trim();
            objdata.AssociationName = item.T_AssociationName.Trim();
            objdata.IsNested = item.T_IsNested.Value;
            objdata.Hierarchy = item.T_Hierarchy.Split('?').Where(wh => !(string.IsNullOrEmpty(wh))).ToList();
            KeyData.Add(objdata);
        }
        return KeyData;
    }
    public FileResult DownloadZipFile(string directoryToZip, string ZipName)
    {
        var fileName = string.Format("{0}.zip", ZipName);
        var temppath = Server.MapPath("~/Files/");
        if(!Directory.Exists(temppath))
        {
            Directory.CreateDirectory(temppath);
        }
        var tempOutPutPath = Path.Combine(temppath, fileName);
        var filenames = Directory.GetFiles(directoryToZip, "*.*", SearchOption.AllDirectories);
        using(var s = new ZipOutputStream(System.IO.File.Create(tempOutPutPath)))
        {
            s.SetLevel(9);
            var buffer = new byte[4096];
            foreach(var file in filenames)
            {
                var relativePath = file.Substring(directoryToZip.Length).TrimStart('\\');
                var entry = new ZipEntry(relativePath);
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);
                using(var fs = System.IO.File.OpenRead(file))
                {
                    int sourceBytes;
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, sourceBytes);
                    }
                    while(sourceBytes > 0);
                }
            }
            s.Finish();
            s.Flush();
            s.Close();
        }
        byte[] finalResult = System.IO.File.ReadAllBytes(tempOutPutPath);
        if(System.IO.File.Exists(tempOutPutPath))
            System.IO.File.Delete(tempOutPutPath);
        if(Directory.Exists(directoryToZip))
            Directory.Delete(directoryToZip, true);
        if(finalResult == null || !finalResult.Any())
            throw new Exception(String.Format("No Files found with Image"));
        return File(finalResult, "application/zip", fileName);
    }
    
    
    
    
}
}

