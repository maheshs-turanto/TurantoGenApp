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
/// <summary>A controller for handling report lists.</summary>
public partial class ReportListController : BaseController
{
    /// <summary>Synchronises this object.</summary>
    ///
    /// <returns>A response stream to send to the Sync View.</returns>
    
    public ActionResult Sync()
    {
        var allReportLists = ReportHelper.GetAllReport();
        if(!string.IsNullOrEmpty(ReportHelper.Error))
        {
            TempData["ErrorInfo"] = ReportHelper.Error;
            return RedirectToAction("Index");
        }
        var count = 1;
        using(ApplicationContext dbReport = new ApplicationContext(new SystemUser(), 1))
        {
            var allCustomReports = dbReport.CustomReportss.OrderBy(p => p.ReportName).ToList();
            var otherCustomReports = allCustomReports.Select(p => p.ReportName).ToList();
            var otherReports = allReportLists.Select(q => q.Name);
            if(allReportLists.Count() > 0)
                foreach(var item in dbReport.ReportLists.Where(p => !otherReports.Contains(p.Name) && p.Type == "SSRS").ToList())
                {
                    dbReport.ReportLists.Remove(item);
                    dbReport.SaveChanges();
                }
            foreach(var item in dbReport.ReportLists.Where(p => !otherCustomReports.Contains(p.Name) && p.Type == "Custom").ToList())
            {
                dbReport.ReportLists.Remove(item);
                dbReport.SaveChanges();
            }
            foreach(var item in allReportLists.OrderBy(p => p.Name))
            {
                var obj = dbReport.ReportLists.FirstOrDefault(p => p.Name == item.Name && p.Type == "SSRS");
                if(obj != null)
                {
                    var ischange = false;
                    if(obj.ReportID != item.ID)
                    {
                        obj.ReportID = item.ID;
                        ischange = true;
                    }
                    if(obj.ReportPath != item.Path)
                    {
                        obj.ReportPath = item.Path;
                        ischange = true;
                    }
                    if(ischange)
                    {
                        dbReport.Entry(obj).State = EntityState.Modified;
                        dbReport.SaveChanges();
                    }
                }
                else
                {
                    var newobj = new ReportList();
                    newobj.ReportID = item.ID;
                    newobj.Name = item.Name;
                    newobj.Description = item.Description;
                    newobj.DisplayName = item.Name;
                    newobj.ReportNo = count;
                    newobj.ReportPath = item.Path;
                    newobj.Type = "SSRS";
                    dbReport.ReportLists.Add(newobj);
                    dbReport.SaveChanges();
                    count++;
                }
            }
            foreach(var item in allCustomReports)
            {
                var obj = dbReport.ReportLists.FirstOrDefault(p => p.Name == item.ReportName && p.Type == "Custom");
                if(obj != null)
                {
                    var ischange = false;
                    if(obj.ReportID != Convert.ToString(item.Id))
                    {
                        obj.ReportID = Convert.ToString(item.Id);
                        ischange = true;
                    }
                    if(obj.ReportPath != Convert.ToString(item.Id))
                    {
                        obj.ReportPath = Convert.ToString(item.Id);
                        ischange = true;
                    }
                    if(ischange)
                    {
                        dbReport.Entry(obj).State = EntityState.Modified;
                        dbReport.SaveChanges();
                    }
                }
                else
                {
                    var newobj = new ReportList();
                    newobj.ReportID = Convert.ToString(item.Id);
                    newobj.Name = item.ReportName;
                    newobj.Description = item.Description;
                    newobj.DisplayName = item.ReportName;
                    newobj.ReportNo = count;
                    newobj.ReportPath = Convert.ToString(item.Id);
                    newobj.Type = "Custom";
                    dbReport.ReportLists.Add(newobj);
                    dbReport.SaveChanges();
                    count++;
                }
            }
        }
        return RedirectToAction("Index");
    }
    
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CheckBeforeDelete(ReportList ssrsreport)
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
    /// <param name="ssrsreport">The ssrsreport.</param>
    /// <param name="command">   (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    
    public string CheckBeforeSave(ReportList ssrsreport, string command = "")
    {
        var AlertMsg = "";
        // Write your logic here
        //Make sure to assign AlertMsg with proper message
        //AlertMsg = "Validation Alert - Before Save !! Information not saved.";
        //ModelState.AddModelError("CustomError", AlertMsg);
        //ViewBag.ApplicationError = AlertMsg;
        return AlertMsg;
    }
    
    /// <summary>Executes the deleting action.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    /// <param name="unitdb">    The unitdb.</param>
    
    public void OnDeleting(ReportList ssrsreport, ApplicationContext unitdb)
    {
        // Write your logic here
    }
    
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    /// <param name="db">        The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(ReportList ssrsreport, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        // Write your logic here
    }
    
    /// <summary>After save.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(ReportList ssrsreport, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    /// <summary>Custom authorization before create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomAuthorizationBeforeCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        var result = true;
        return result;
    }
    
    /// <summary>Custom authorization before edit.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomAuthorizationBeforeEdit(int? id, string HostingEntityName, string AssociatedType)
    {
        var result = true;
        return result;
    }
    
    /// <summary>Custom authorization before details.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomAuthorizationBeforeDetails(int? id, string HostingEntityName, string AssociatedType)
    {
        var result = true;
        return result;
    }
    
    /// <summary>Custom authorization before delete.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomAuthorizationBeforeDelete(int? id)
    {
        var result = true;
        return result;
    }
    
    /// <summary>Custom authorization before bulk update.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomAuthorizationBeforeBulkUpdate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        var result = true;
        return result;
    }
    
    /// <summary>Custom load view data list before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    
    private void CustomLoadViewDataListBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
    }
    
    /// <summary>Custom load view data list after on create.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    
    private void CustomLoadViewDataListAfterOnCreate(ReportList ssrsreport)
    {
    }
    
    /// <summary>Custom load view data list before edit.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    
    private void CustomLoadViewDataListBeforeEdit(ReportList ssrsreport)
    {
    }
    
    /// <summary>Custom load view data list after edit.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    
    private void CustomLoadViewDataListAfterEdit(ReportList ssrsreport)
    {
    }
    
    /// <summary>Custom load view data list on index.</summary>
    ///
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    
    private void CustomLoadViewDataListOnIndex(string HostingEntity, int? HostingEntityID, string AssociatedType)
    {
    }
    
    /// <summary>Custom save on create.</summary>
    ///
    /// <param name="ssrsreport">            The ssrsreport.</param>
    /// <param name="customcreate_hasissues">[out] True to customcreate hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnCreate(ReportList ssrsreport, out bool customcreate_hasissues, string command = "")
    {
        var result = false;
        customcreate_hasissues = false;
        return result;
    }
    
    /// <summary>Custom save on edit.</summary>
    ///
    /// <param name="ssrsreport">          The ssrsreport.</param>
    /// <param name="customsave_hasissues">[out] True to customsave hasissues.</param>
    /// <param name="command">             (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnEdit(ReportList ssrsreport, out bool customsave_hasissues, string command = "")
    {
        var result = false;
        customsave_hasissues = false;
        return result;
    }
    
    /// <summary>Custom save on import.</summary>
    ///
    /// <param name="ssrsreport"> The ssrsreport.</param>
    /// <param name="customerror">[out] The customerror.</param>
    /// <param name="i">          Zero-based index of the.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnImport(ReportList ssrsreport, out string customerror, int i)
    {
        var result = false;
        customerror = "";
        return result;
    }
    
    /// <summary>Custom import data validate.</summary>
    ///
    /// <param name="objDataSet"> Set the object data belongs to.</param>
    /// <param name="columnName"> Name of the column.</param>
    /// <param name="columnValue">The column value.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomImportDataValidate(DataSet objDataSet, string columnName, string columnValue)
    {
        var result = true;
        //create ViewBag.CustomErrorsConfirmImportData for extra validation msg here
        return result;
    }
    
    /// <summary>Custom delete.</summary>
    ///
    /// <param name="ssrsreport">            The ssrsreport.</param>
    /// <param name="customdelete_hasissues">[out] True to customdelete hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomDelete(ReportList ssrsreport, out bool customdelete_hasissues, string command = "")
    {
        var result = false;
        customdelete_hasissues = false;
        return result;
    }
    
    /// <summary>Custom sorting.</summary>
    ///
    /// <param name="list">          The list.</param>
    /// <param name="HostingEntity"> The hosting entity.</param>
    /// <param name="AssociatedType">Type of the associated.</param>
    /// <param name="sortBy">        Describes who sort this object.</param>
    /// <param name="isAsc">         The is ascending.</param>
    ///
    /// <returns>A list of.</returns>
    
    private IQueryable<ReportList> CustomSorting(IQueryable<ReportList> list, string HostingEntity, string AssociatedType, string sortBy, string isAsc)
    {
        IQueryable<ReportList> orderedList = list;
        if(!this.User.IsAdmin)
        {
            List<string> reportIDs = new List<string>();
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesReport();
            var roles = this.User.GetRoles();
            foreach(var role in roles)
            {
                var RoleName = RoleList.FirstOrDefault(r => r.Value == role);
                if(RoleName.Value != null)
                {
                    reportIDs.AddRange(db.ReportsInRoles.Where(r => r.RoleId == RoleName.Key).Select(p => p.ReportId).ToList());
                }
            }
            reportIDs.AddRange(db.ReportsInRoles.Where(r => r.RoleId == "All").Select(p => p.ReportId).ToList());
            //List<long> SeelectedreportID = reportIDs.Select(long.Parse).ToList();
            long x = 0;
            List<long> SelectedreportID = reportIDs.Distinct().Where(str => long.TryParse(str, out x))
                                          .Select(str => x)
                                          .ToList();
            orderedList = orderedList.Where(p => SelectedreportID.Contains(p.Id) && !p.IsHidden.Value);
        }
        // Do custom sorting here, you can also use this method to do custom filter
        return orderedList;
    }
    
    /// <summary>Creates a result that redirects to the given route.</summary>
    ///
    /// <param name="ssrsreport">The ssrsreport.</param>
    /// <param name="action">    The action.</param>
    /// <param name="command">   (Optional) The command.</param>
    ///
    /// <returns>A RedirectToRouteResult.</returns>
    
    private RedirectToRouteResult CustomRedirectUrl(ReportList ssrsreport, string action, string command = "")
    {
        // Sample Custom implemention below
        // return RedirectToAction("Index", "ReportList");
        return null;
    }
}
}
