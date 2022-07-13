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
/// <summary>A controller for handling reports groups.</summary>
public partial class ReportsGroupController : BaseController
{
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool CheckBeforeDelete(ReportsGroup reportsgroup)
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
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="command">     (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    
    public string CheckBeforeSave(ReportsGroup reportsgroup, string command ="")
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
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="unitdb">      The unitdb.</param>
    
    public void OnDeleting(ReportsGroup reportsgroup, ApplicationContext unitdb)
    {
        // Write your logic here
    }
    
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="db">          The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(ReportsGroup reportsgroup, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        // Write your logic here
    }
    
    /// <summary>After save.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(ReportsGroup reportsgroup, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
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
    /// <param name="reportsgroup">The reportsgroup.</param>
    
    private void CustomLoadViewDataListAfterOnCreate(ReportsGroup reportsgroup)
    {
    }
    
    /// <summary>Custom load view data list before edit.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    
    private void CustomLoadViewDataListBeforeEdit(ReportsGroup reportsgroup)
    {
    }
    
    /// <summary>Custom load view data list after edit.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    
    private void CustomLoadViewDataListAfterEdit(ReportsGroup reportsgroup)
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
    /// <param name="reportsgroup">          The reportsgroup.</param>
    /// <param name="customcreate_hasissues">[out] True to customcreate hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnCreate(ReportsGroup reportsgroup, out bool customcreate_hasissues, string command="")
    {
        var result = false;
        customcreate_hasissues = false;
        return result;
    }
    
    /// <summary>Custom save on edit.</summary>
    ///
    /// <param name="reportsgroup">        The reportsgroup.</param>
    /// <param name="customsave_hasissues">[out] True to customsave hasissues.</param>
    /// <param name="command">             (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnEdit(ReportsGroup reportsgroup, out bool customsave_hasissues, string command="")
    {
        var result = false;
        customsave_hasissues = false;
        return result;
    }
    
    /// <summary>Custom save on import.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="customerror"> [out] The customerror.</param>
    /// <param name="i">           Zero-based index of the.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomSaveOnImport(ReportsGroup reportsgroup, out string customerror, int i)
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
    /// <param name="reportsgroup">          The reportsgroup.</param>
    /// <param name="customdelete_hasissues">[out] True to customdelete hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private bool CustomDelete(ReportsGroup reportsgroup, out bool customdelete_hasissues, string command="")
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
    /// <returns>An IQueryable&lt;ReportsGroup&gt;</returns>
    
    private IQueryable<ReportsGroup> CustomSorting(IQueryable<ReportsGroup> list, string HostingEntity, string AssociatedType, string sortBy, string isAsc)
    {
        IQueryable<ReportsGroup> orderedList = list;
        // Do custom sorting here, you can also use this method to do custom filter
        return orderedList;
    }
    
    /// <summary>Creates a result that redirects to the given route.</summary>
    ///
    /// <param name="reportsgroup">The reportsgroup.</param>
    /// <param name="action">      The action.</param>
    /// <param name="command">     (Optional) The command.</param>
    ///
    /// <returns>A RedirectToRouteResult.</returns>
    
    private RedirectToRouteResult CustomRedirectUrl(ReportsGroup reportsgroup,string action,string command="")
    {
        // Sample Custom implemention below
        // return RedirectToAction("Index", "ReportsGroup");
        return null;
    }
}
}
