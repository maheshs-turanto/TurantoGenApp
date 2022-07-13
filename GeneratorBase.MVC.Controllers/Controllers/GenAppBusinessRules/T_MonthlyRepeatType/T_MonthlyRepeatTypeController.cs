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
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling monthly repeat types.</summary>
public partial class T_MonthlyRepeatTypeController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="AssocType">          Type of the associated.</param>
    
    private void LoadViewDataForCount(T_MonthlyRepeatType t_monthlyrepeattype, string AssocType)
    {
    }
    
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    
    private void LoadViewDataAfterOnEdit(T_MonthlyRepeatType t_monthlyrepeattype)
    {
    }
    
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    
    private void LoadViewDataBeforeOnEdit(T_MonthlyRepeatType t_monthlyrepeattype)
    {
        ViewBag.T_RepeatByCount = t_monthlyrepeattype.t_repeatby.Count();
    }
    
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    
    private void LoadViewDataAfterOnCreate(T_MonthlyRepeatType t_monthlyrepeattype)
    {
    }
    
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstT_MonthlyRepeatType">Type of the list monthly repeat.</param>
    /// <param name="searchString">          The search string.</param>
    /// <param name="IsDeepSearch">          The is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<T_MonthlyRepeatType> searchRecords(IQueryable<T_MonthlyRepeatType> lstT_MonthlyRepeatType, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstT_MonthlyRepeatType = lstT_MonthlyRepeatType.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstT_MonthlyRepeatType = lstT_MonthlyRepeatType.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        return lstT_MonthlyRepeatType;
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstT_MonthlyRepeatType">Type of the list monthly repeat.</param>
    /// <param name="sortBy">                Describes who sort this object.</param>
    /// <param name="isAsc">                 The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<T_MonthlyRepeatType> sortRecords(IQueryable<T_MonthlyRepeatType> lstT_MonthlyRepeatType, string sortBy, string isAsc)
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
        ParameterExpression paramExpression = Expression.Parameter(typeof(T_MonthlyRepeatType), "t_monthlyrepeattype");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<T_MonthlyRepeatType>)lstT_MonthlyRepeatType.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstT_MonthlyRepeatType.ElementType, lambda.Body.Type },
                       lstT_MonthlyRepeatType.Expression,
                       lambda));
    }
    
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
        var _Obj = db1.T_MonthlyRepeatTypes.FirstOrDefault(p => p.Id == idvalue);
        return  _Obj==null?"":_Obj.DisplayValue;
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
        var _Obj = db1.T_MonthlyRepeatTypes.Find(Convert.ToInt64(id));
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
        var _Obj = db1.T_MonthlyRepeatTypes.Find(Convert.ToInt64(id));
        return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_MonthlyRepeatType>(_Obj, "T_MonthlyRepeatType", new string[] { ""  }); ;
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
        IQueryable<T_MonthlyRepeatType> list = db.T_MonthlyRepeatTypes;
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
        IQueryable<T_MonthlyRepeatType> list = db.T_MonthlyRepeatTypes;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.T_MonthlyRepeatTypes;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T_MonthlyRepeatType), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T_MonthlyRepeatType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_MonthlyRepeatType>)q);
            }
            else if(AssoID == 0)
            {
                IQueryable query = db.T_MonthlyRepeatTypes;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T_MonthlyRepeatType), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T_MonthlyRepeatType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_MonthlyRepeatType>)q);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_MonthlyRepeatType>(User,list, "T_MonthlyRepeatType",caller);
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p=>p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p=>p.Id != val).Take(9).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
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
        IQueryable<T_MonthlyRepeatType> list = db.T_MonthlyRepeatTypes;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.T_MonthlyRepeatTypes;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(T_MonthlyRepeatType), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<T_MonthlyRepeatType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_MonthlyRepeatType>)q);
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
        IQueryable<T_MonthlyRepeatType> list = db.T_MonthlyRepeatTypes;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            try
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if(AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_MonthlyRepeatTypes;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_MonthlyRepeatType), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_MonthlyRepeatType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_MonthlyRepeatType>)q).Take(20);
                }
            }
            catch
            {
                var data = from x in list.Take(20).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        list = _fad.FilterDropdown<T_MonthlyRepeatType>(User,list, "T_MonthlyRepeatType",caller);
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(20).Union(list.Where(p=>p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(20).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p=>p.Id != val).Take(20).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Take(20).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
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
        IQueryable<T_MonthlyRepeatType> list = db.T_MonthlyRepeatTypes;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            IQueryable query = db.T_MonthlyRepeatTypes;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = AssoNameWithParent;
            var AssoIDs = AssociationID.Split(',').ToList();
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Parameter(typeof(T_MonthlyRepeatType), "p"));
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
            list = ((IQueryable<T_MonthlyRepeatType>)q);
        }
        if(key != null && key.Length > 0)
        {
            var data = from x in list.Where(p=>p.DisplayValue.Contains(key)).Take(10).OrderBy(q=>q.DisplayValue).ToList()
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
    
    /// <summary>(An Action that handles HTTP POST requests) uploads the given file.</summary>
    ///
    /// <returns>A response stream to send to the Upload View.</returns>
    
    [HttpGet]
    public ActionResult Upload()
    {
        //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_MonthlyRepeatType")).Count() > 0 ? true : false;
        var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_MonthlyRepeatType").ToList();
        var distinctMapping = lstMappings.GroupBy(p => p.MappingName).Distinct();
        List<ImportConfiguration> ddlMappingList = new List<ImportConfiguration>();
        foreach(var elem in distinctMapping)
        {
            ddlMappingList.Add(elem.FirstOrDefault());
        }
        var DefaultMapping = lstMappings.Where(p => p.IsDefaultMapping).FirstOrDefault();
        var mappingID = DefaultMapping == null ? 0 : DefaultMapping.Id;
        ViewBag.IsDefaultMapping = DefaultMapping != null ? true : false;
        ViewBag.ListOfMappings = new SelectList(ddlMappingList, "ID", "MappingName", mappingID);
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_MonthlyRepeatType");
                if(entList != null)
                {
                    foreach(var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
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
                    typeName = "T_MonthlyRepeatType";
                    //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                    string ExistsColumnMappingName = string.Empty;
                    string mappingName = db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                    if(collection["DefaultMapping"] == "on")
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_MonthlyRepeatType" && p.IsDefaultMapping);
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
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Monthly Repeat Types";
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
        string typename = "T_MonthlyRepeatType";
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
                        objdr[columntable] = columnValue;
                    }
                    tempdt.Rows.Add(objdr);
                }
            }
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Monthly Repeat Types";
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
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_MonthlyRepeatType");
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
                    T_MonthlyRepeatType model = new T_MonthlyRepeatType();
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
                        default:
                            break;
                        }
                    }
                    if(ValidateModel(model) && string.IsNullOrEmpty(CheckBeforeSave(model)))
                    {
                        var result = CheckMandatoryProperties(model);
                        if(result == null || result.Count == 0)
                        {
                            db.T_MonthlyRepeatTypes.Add(model);
                            db.SaveChanges();
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
                            ViewBag.ImportError = "Row No : " + (i + 1) + " Some Required Value Missing or Before save validation failed.";
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " Some Required Value Missing or Before save validation failed";
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
    
    public bool ValidateModel(T_MonthlyRepeatType validate)
    {
        return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null);
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
                if(dr[ Convert.ToInt32(sheetColumns[i]) - 1] != null && dr[ Convert.ToInt32(sheetColumns[i]) - 1].ToString() != "")
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
            var obj1 = db1.T_MonthlyRepeatTypes.Find(Id);
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
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="OModel">The model.</param>
    ///
    /// <returns>A JSON response stream to send to the GetReadOnlyProperties action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetReadOnlyProperties(T_MonthlyRepeatType OModel)
    {
        Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MonthlyRepeatType").ToList();
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_MonthlyRepeatType");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                    foreach(string paramName in Args.Select(p => p.ParameterName))
                    {
                        var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MonthlyRepeatType").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                        if(!RequiredProperties.ContainsKey(paramName))
                            RequiredProperties.Add(paramName, dispName);
                    }
                }
            }
        }
        return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="ruleType">Type of the rule.</param>
    ///
    /// <returns>A JSON response stream to send to the GetMandatoryProperties action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetMandatoryProperties(T_MonthlyRepeatType OModel,string ruleType)
    {
        Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MonthlyRepeatType").ToList();
            if(BR != null && BR.Count > 0)
            {
                if(ruleType == "OnCreate")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                else if(ruleType == "OnEdit")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR,"T_MonthlyRepeatType");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach(string paramName in Args.Select(p => p.ParameterName))
                    {
                        var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MonthlyRepeatType").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                        if(!RequiredProperties.ContainsKey(paramName))
                            RequiredProperties.Add(paramName, dispName);
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
    public JsonResult GetLockBusinessRules(T_MonthlyRepeatType OModel)
    {
        string RulesApplied = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MonthlyRepeatType").ToList();
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.LockEntityRule(OModel, BR,"T_MonthlyRepeatType");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
                {
                    RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
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
    
    private List<string> CheckMandatoryProperties(T_MonthlyRepeatType OModel)
    {
        List<string> result = new List<string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == "T_MonthlyRepeatType").ToList();
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if(BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_MonthlyRepeatType");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach(string paramName in Args.Select(p => p.ParameterName))
                    {
                        var type = OModel.GetType();
                        if(type.GetProperty(paramName) == null) continue;
                        var propertyvalue = type.GetProperty(paramName).GetValue(OModel, null);
                        if(propertyvalue == null)
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MonthlyRepeatType").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
        var _Obj = db1.T_MonthlyRepeatTypes.FirstOrDefault(p => p.DisplayValue == displayvalue);
        long outValue;
        if(_Obj != null)
            return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
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
        var obj1 = db1.T_MonthlyRepeatTypes.Find(Id);
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
        System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
        System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
        RuleActionContext objRuleAction = new RuleActionContext();
        ConditionContext objCondition = new ConditionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
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
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? "&& ($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
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
                                            fnConditionValue += (rbcheck ? "&& ($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
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
                                        fnProp += "$('#dv" + objaa.ParameterName + "').css('display', type);";
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
        string DataType = PropInfo.DataType;
        if(AssociationInfo != null)
        {
            DataType = "Association";
        }
        return DataType;
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    ///
    /// <returns>A JSON response stream to send to the Check1MThresholdValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult Check1MThresholdValue(T_MonthlyRepeatType t_monthlyrepeattype)
    {
        Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
        return Json(msgThreshold, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CheckBeforeDelete(T_MonthlyRepeatType t_monthlyrepeattype)
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
    
    /// <summary>Check before save.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    ///
    /// <returns>A string.</returns>
    
    public string CheckBeforeSave(T_MonthlyRepeatType t_monthlyrepeattype)
    {
        var result = true;
        var AlertMsg = "";
        // Write your logic here
        if(!result)
        {
            AlertMsg = "Validation Alert - Before Save !! Information not saved.";
            ModelState.AddModelError("CustomError", AlertMsg);
            ViewBag.ApplicationError = AlertMsg;
        }
        return AlertMsg;
    }
    
    /// <summary>Executes the deleting action.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    
    public void OnDeleting(T_MonthlyRepeatType t_monthlyrepeattype)
    {
        // Write your logic here
    }
    
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="db">                 The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(T_MonthlyRepeatType t_monthlyrepeattype, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        // Write your logic here
    }
    
    /// <summary>After save.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(T_MonthlyRepeatType t_monthlyrepeattype, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    /// <summary>code for verb action security.</summary>
    ///
    /// <returns>An array of string.</returns>
    
    public string[] getVerbsName()
    {
        string[] Verbsarr = new string[] { "BulkUpdate","BulkDelete"   };
        return Verbsarr;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) gets calculation values.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    ///
    /// <returns>A response stream to send to the GetCalculationValues View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetCalculationValues(T_MonthlyRepeatType t_monthlyrepeattype)
    {
        t_monthlyrepeattype.setCalculation();
        Dictionary<string, string> Calculations = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_monthlyrepeattype.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    //get Templates
    // select templates
    
    /// <summary>(An Action that handles HTTP POST requests) gets derived details.</summary>
    ///
    /// <param name="t_monthlyrepeattype">The monthlyrepeattype.</param>
    /// <param name="IgnoreEditable">     The ignore editable.</param>
    ///
    /// <returns>A response stream to send to the GetDerivedDetails View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetDerivedDetails(T_MonthlyRepeatType t_monthlyrepeattype, string IgnoreEditable)
    {
        Dictionary<string, string> derivedlist = new Dictionary<string, string>();
        System.Reflection.PropertyInfo[] properties = (t_monthlyrepeattype.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
}
}

