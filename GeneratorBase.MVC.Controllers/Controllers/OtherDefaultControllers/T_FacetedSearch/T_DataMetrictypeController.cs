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
/// <summary>A partial controller class for DataMetric Type actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class T_DataMetrictypeController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(T_DataMetrictype t_datametrictype, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    private void LoadViewDataAfterOnEdit(T_DataMetrictype t_datametrictype)
    {
        LoadViewDataBeforeOnEdit(t_datametrictype, false);
        CustomLoadViewDataListAfterEdit(t_datametrictype);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="t_datametrictype">         The DataMetric Type.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(T_DataMetrictype t_datametrictype, bool loadCustomViewData = true)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        ViewBag.T_AssociatedDataMetricTypeCount = db.T_DataMetrics.Count(p => p.T_AssociatedDataMetricTypeID == t_datametrictype.Id);
        ViewBag.T_DataMetrictypeIsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnEdit", false,null);
        ViewBag.T_DataMetrictypeIsGroupsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnEdit", true,null);
        ViewBag.T_DataMetrictypeIsSetValueUIRule = checkSetValueUIRule(User,"T_DataMetrictype", "OnEdit",new long[] { 6, 8 },null,null);
        ViewBag.T_DataMetrictypeRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_DataMetrictype", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(t_datametrictype);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    private void LoadViewDataAfterOnCreate(T_DataMetrictype t_datametrictype)
    {
        CustomLoadViewDataListAfterOnCreate(t_datametrictype);
        ViewBag.T_DataMetrictypeIsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnCreate", false,null);
        ViewBag.T_DataMetrictypeIsGroupsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnCreate", true,null);
        ViewBag.T_DataMetrictypeIsSetValueUIRule = checkSetValueUIRule(User,"T_DataMetrictype", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.T_DataMetrictypeRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_DataMetrictype", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        ViewBag.T_DataMetrictypeIsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnCreate", false,null);
        ViewBag.T_DataMetrictypeIsGroupsHiddenRule = checkHidden(User,"T_DataMetrictype", "OnCreate", true,null);
        ViewBag.T_DataMetrictypeIsSetValueUIRule = checkSetValueUIRule(User,"T_DataMetrictype", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.T_DataMetrictypeRestrictDropdownValueRule = RestrictDropdownValueRule(User, "T_DataMetrictype", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    /// <summary>Export Excel.</summary>
    ///
    /// <param name="query">List of objects.</param>
    ///
    /// <returns>xlsx File.</returns>
    public FileResult DownloadExcel(List<GeneratorBase.MVC.Models.T_DataMetrictype> query)
    {
        foreach(var item in query)
        {
            item.setDateTimeToClientTime();
            item.ApplyHiddenRule(User.businessrules, "T_DataMetrictype");
        }
        DataTable dt = ExcelExportHelper.ToDataTable<T_DataMetrictype>(User, query,"T_DataMetrictype");
        dt.AcceptChanges();
        string fileName = "DataMetric Type.xlsx";
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            var columns = EntityColumns().Select(p=>p.Value).ToArray();
            dt.SetColumnsOrder(columns);
            dt.RemoveColumns(new string[] { });
            wb.Properties.Author = "DashboardDemoV2";
            wb.Properties.Title = "DataMetric Type";
            wb.Properties.Subject = "DataMetric Type data";
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
        var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_DataMetrictype").Properties;
        columns.Add("2", modelproperties.FirstOrDefault(q => q.Name == "T_Name").DisplayName);
        columns.Add("3", modelproperties.FirstOrDefault(q => q.Name == "T_Description").DisplayName);
        return columns;
    }
    
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstT_DataMetrictype">The list DataMetric Type.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<T_DataMetrictype> searchRecords(IQueryable<T_DataMetrictype> lstT_DataMetrictype, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstT_DataMetrictype = lstT_DataMetrictype.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstT_DataMetrictype = lstT_DataMetrictype.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstT_DataMetrictype;
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstT_DataMetrictype">The IQueryable list DataMetric Type.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<T_DataMetrictype> sortRecords(IQueryable<T_DataMetrictype> lstT_DataMetrictype, string sortBy, string isAsc)
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
            return isAsc.ToLower() == "asc" ? lstT_DataMetrictype.Sort(sortBy,true) : lstT_DataMetrictype.Sort(sortBy,false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(T_DataMetrictype), "t_datametrictype");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T_DataMetrictype>)lstT_DataMetrictype.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstT_DataMetrictype.ElementType, lambda.Body.Type },
                       lstT_DataMetrictype.Expression,
                       lambda));
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_T_DataMetrictype">IQueryable<T_DataMetrictype>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<T_DataMetrictype>.</returns>
    private IQueryable<T_DataMetrictype> FilterByHostingEntity(IQueryable<T_DataMetrictype> _T_DataMetrictype, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        return _T_DataMetrictype;
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
            var _Obj = appcontext.T_DataMetrictypes.FirstOrDefault(p => p.Id == idvalue);
            return  _Obj==null?"":_Obj.DisplayValue;
        }
    }
    /// <summary>Gets IQueryable.</summary>
    ///
    /// <param name="appdb">The applicationContext.</param>
    ///
    /// <returns>The IQueryable.</returns>
    public IQueryable<T_DataMetrictype> GetIQueryable(ApplicationContext appdb)
    {
        return appdb.T_DataMetrictypes.AsNoTracking();
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
            var _Obj = appcontext.T_DataMetrictypes.Find(Convert.ToInt64(id));
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
        if(string.IsNullOrEmpty(id)) return Json(new T_DataMetrictype(), JsonRequestBehavior.AllowGet); ;
        using(var appcontext = (new ApplicationContext(new SystemUser(), true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.T_DataMetrictypes.Find(Convert.ToInt64(id));
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
        using(var context = (new ApplicationContext(new SystemUser(),true)))
        {
            context.Configuration.LazyLoadingEnabled = false;
            var _Obj = context.T_DataMetrictypes.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_DataMetrictype>(_Obj, "T_DataMetrictype", new string[] { ""  });
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
        IQueryable<T_DataMetrictype> list = db.T_DataMetrictypes;
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_DataMetrictype>(User, list, "T_DataMetrictype", caller);
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
        var result = DropdownHelper.GetAllValue<T_DataMetrictype>(User, db.T_DataMetrictypes, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_DataMetrictype", RestrictDropdownVal);
        return Json(result.Select(x=> new
        {
            Id = x.Id, Name = x.DisplayValue
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
        var result = DropdownHelper.GetAllValueForRB<T_DataMetrictype>(User, db.T_DataMetrictypes, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "T_DataMetrictype");
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
        IQueryable<T_DataMetrictype> list = db.T_DataMetrictypes;
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
    public JsonResult GetAllMultiSelectValue(string caller,string key, string AssoNameWithParent, string AssociationID)
    {
        caller = caller.Replace("?", "");
        var result  = DropdownHelper.GetAllMultiSelectValue(User, db.T_DataMetrictypes, key, AssoNameWithParent, AssociationID, "T_DataMetrictype", caller);
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
            if(!((CustomPrincipal)User).CanUseVerb("ImportCSV", "T_DataMetrictype", User) || !User.CanAdd("T_DataMetrictype"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else if(FileType.ToLower() == "xls" || FileType.ToLower() == "xlsx")
        {
            if(!((CustomPrincipal)User).CanUseVerb("ImportExcel", "T_DataMetrictype", User) || !User.CanAdd("T_DataMetrictype"))
            {
                return RedirectToAction("Index", "Error");
            }
        }
        else
        {
            return RedirectToAction("Index", "Error");
        }
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_DataMetrictype")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_DataMetrictype").ToList();
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_DataMetrictype");
                if(entList != null)
                {
                    var entityprop = entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "TenantId" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime");
                    string[] allcolumns = new string[] { "T_Name","T_Description" };
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
                        var colSelected = col.FirstOrDefault(p=> p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                        if(colSelected != null)
                            selectedVal = long.Parse(colSelected.Value);
                        objColMap.Add(prop, new SelectList(col,"Value", "Text", selectedVal));
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
                    typeName = "T_DataMetrictype";
                    //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                    string idMapping = collection["ListOfMappings"];
                    string ExistsColumnMappingName = string.Empty;
                    string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    if(collection["DefaultMapping"] == "on")
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_DataMetrictype" && p.IsDefaultMapping);
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
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>DataMetric Type</b>";
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
        string DetailMessage = "";
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string typename = "T_DataMetrictype";
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
                        if(mapingProplst != null && mapingProplst.Count()>0)
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
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new <b>DataMetric Type</b>";
            Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_DataMetrictype");
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
                    T_DataMetrictype model = new T_DataMetrictype();
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
                        case "T_Name":
                            model.T_Name = columnValue;
                            break;
                        case "T_Description":
                            model.T_Description = columnValue;
                            break;
                        default:
                            break;
                        }
                    }
                    // Columns to update
                    var MatchColumns = collection["hdnListChkUpdate"];
                    var flagUpdate = MatchUpdate.Update(model, MatchColumns, db, "T_DataMetrictype", mappedColumns);
                    if(flagUpdate) continue;
                    //
                    var AlrtMsg = CheckBeforeSave(model, "ImportData");
                    if(AlrtMsg == "")
                    {
                        var customimport_hasissues = false;
                        if(ValidateModel(model))
                        {
                            var result = CheckMandatoryProperties(User,db,model,"T_DataMetrictype");
                            var validatebr_result = GetValidateBeforeSavePropertiesDictionary(User, db, model, "OnCreate", "T_DataMetrictype");
                            if((result == null || result.Count == 0) && (validatebr_result == null || validatebr_result.Count == 0))
                                //if (result == null || result.Count == 0)
                            {
                                var customerror = "";
                                if(!CustomSaveOnImport(model, out customerror,i))
                                {
                                    if(!flagUpdate)
                                    {
                                        db.T_DataMetrictypes.Add(model);
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
                var obj1 = appcontext.T_DataMetrictypes.Where(p=>p.Id == Id).FirstOrDefault();
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
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleOnSaving action.</returns>
    private JsonResult ApplyBusinessRuleOnSaving(T_DataMetrictype t_datametrictype)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_DataMetrictype").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            //var ruleids = businessrule.Select(q => q.Id).ToList();
            //var typelist = (new GeneratorBase.MVC.Models.RuleActionContext()).RuleActions.Where(p => ruleids.Contains(p.RuleActionID.Value) && p.associatedactiontype.TypeNo.HasValue).Select(p => p.associatedactiontype.TypeNo.Value).Distinct().ToList();
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(10))
            {
                var resultBR = GetValidateBeforeSavePropertiesDictionary(User,db,t_datametrictype, "OnEdit","T_DataMetrictype");
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
                var resultBR = GetMandatoryPropertiesDictionary(User,db,t_datametrictype, "OnEdit","T_DataMetrictype");
                if(resultBR.Count() > 0)
                {
                    string stringResult = "";
                    string BRID = "";
                    foreach(var dic in resultBR)
                    {
                        if(!dic.Key.Contains("FailureMessage"))
                        {
                            var type = t_datametrictype.GetType();
                            if(type.GetProperty(dic.Key) != null)
                            {
                                var propertyvalue = type.GetProperty(dic.Key).GetValue(t_datametrictype, null);
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
                var resultBR = GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User,db,t_datametrictype, "OnEdit","T_DataMetrictype");
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
    /// <param name="t_datametrictype">       The DataMetric Type.</param>
    /// <param name="IsReadOnlyIgnore">(Optional) True if is read only ignored, false if not.</param>
    ///
    /// <returns>A JSON response stream to send to the ApplyBusinessRuleBefore action.</returns>
    private JsonResult ApplyBusinessRuleBefore(T_DataMetrictype t_datametrictype, bool IsReadOnlyIgnore=false)
    {
        var businessrule = User.businessrules.Where(p => p.EntityName == "T_DataMetrictype").ToList();
        if((businessrule != null && businessrule.Count > 0))
        {
            var typelist = businessrule.SelectMany(p => p.ActionTypeID).Distinct().ToList();
            if(typelist.Contains(1) || typelist.Contains(11))
            {
                var validateLockResult = GetLockBusinessRulesDictionary(User, db,t_datametrictype,"T_DataMetrictype").Where(p => p.Key.Contains("FailureMessage"));
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
                var validateMandatorypropertyResult = GetReadOnlyPropertiesDictionary(User,db,t_datametrictype,"T_DataMetrictype").Where(p => p.Key.Contains("FailureMessage"));
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
    public JsonResult GetReadOnlyProperties(T_DataMetrictype OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
        return Json(GetReadOnlyPropertiesDictionary(User, db, OModel, "T_DataMetrictype"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON (Mandatory business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetMandatoryProperties(T_DataMetrictype OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
        return Json(GetMandatoryPropertiesDictionary(User,db,OModel,ruleType,"T_DataMetrictype"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (UI alert business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetUIAlertBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetUIAlertBusinessRules(T_DataMetrictype OModel, string ruleType)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_DataMetrictype").ToList();
            var BRAll = BR;
            if(ruleType == "OnCreate")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
            else if(ruleType == "OnEdit")
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
            if(BR != null && BR.Count > 0)
            {
                OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
                OModel.setCalculation();
                var ResultOfBusinessRules = db.UIAlertRule(OModel, BR, "T_DataMetrictype");
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
    /// <returns>T_DataMetrictype object.</returns>
    public T_DataMetrictype UpdateHiddenProperties(T_DataMetrictype OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.T_DataMetrictypes.Find(OModel.Id);
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
    public JsonResult GetValidateBeforeSaveProperties(T_DataMetrictype OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
        return Json(GetValidateBeforeSavePropertiesDictionary(User,db,OModel,ruleType,"T_DataMetrictype"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Before save business rule).</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetValidateBeforeSavePropertiesForPopupConfirm action.</returns>
    [HttpPost]
    [Audit(0)]
    public JsonResult GetValidateBeforeSavePropertiesForPopupConfirm(T_DataMetrictype  OModel, string ruleType)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
        return Json(GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(User, db, OModel, ruleType, "T_DataMetrictype"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) creates a JSON result (Lock record business rule).</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetLockBusinessRules action.</returns>
    [HttpPost]
    [Audit(0)]
    [ValidateAntiForgeryToken]
    public JsonResult GetLockBusinessRules(T_DataMetrictype OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("T_DataMetrictype")));
        return Json(GetLockBusinessRulesDictionary(User,db,OModel,"T_DataMetrictype"), JsonRequestBehavior.AllowGet);
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
            var _Obj = db1.T_DataMetrictypes.FirstOrDefault(p => p.DisplayValue == displayvalue);
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
            IQueryable query = context.T_DataMetrictypes;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(T_DataMetrictype), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if(PropValue.ToLower().Trim() != "null")
                lambda = Expression.Lambda<Func<T_DataMetrictype, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
            else
                lambda = Expression.Lambda<Func<T_DataMetrictype, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<T_DataMetrictype>)q);
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
        using(var context = (new ApplicationContext(new SystemUser(),true)))
        {
            var obj1 = context.T_DataMetrictypes.Find(Id);
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
            T_DataMetrictype obj1 = context.T_DataMetrictypes.Find(Id);
            if(obj1 != null)
            {
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(q => q.PropertyType.FullName.StartsWith("System")).ToArray();
                //
                string propToWhere = PropName;
                ParameterExpression p = Expression.Parameter(typeof(T_DataMetrictype), "p");
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
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    [HttpPost]
    public JsonResult Check1MThresholdValue(T_DataMetrictype t_datametrictype)
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
        string[] Verbsarr = new string[] { "BulkUpdate","BulkDelete","ImportExcel","ImportCSV","ExportExcel","ExportCSV"  };
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
        string[][] groupsarr = new string[][] { new string[] {"55864","Basic Information"} };
        return groupsarr;
    }
/// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="t_datametrictype">The DataMetric Type.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetCalculationValues(T_DataMetrictype t_datametrictype)
    {
        t_datametrictype.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_datametrictype.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="t_datametrictype">     The DataMetric Type.</param>
    /// <param name="IgnoreEditable">(Optional) The ignore editable.</param>
    /// <param name="source">        (Optional) Source.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetails(T_DataMetrictype t_datametrictype, string IgnoreEditable=null, string source=null)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_datametrictype.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>(An Action that handles HTTP POST requests) gets derived details inline.</summary>
    ///
    /// <param name="host">          The host.</param>
    /// <param name="value">         The value.</param>
    /// <param name="t_datametrictype">     The DataMetric Type.</param>
    /// <param name="IgnoreEditable">The ignored editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetailsInline View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Audit(0)]
    public ActionResult GetDerivedDetailsInline(string host, string value, T_DataMetrictype t_datametrictype, string IgnoreEditable)
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
}
}

