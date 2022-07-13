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
using Z.EntityFramework.Plus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using AttributeRouting.Helpers;
using System.Linq.Dynamic;
using System.Reflection;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for ImportConfiguration.</summary>
///
/// <remarks></remarks>
public partial class ImportConfigurationController : BaseController
{
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool CheckBeforeDelete(ImportConfiguration importconfiguration)
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
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public string CheckBeforeSave(ImportConfiguration importconfiguration, string command ="")
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
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="importconfigurationdb">   The importconfigurationdb.</param>
    /// <param name="ondeletinguser">The application user.</param>
    public void OnDeleting(ImportConfiguration importconfiguration, ApplicationContext importconfigurationdb, GeneratorBase.MVC.Models.IUser ondeletinguser)
    {
        // Write your logic here
    }
    /// <summary>Executes the saving action.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="db">       The database.</param>
    /// <param name="onsavinguser">The application user.</param>
    public void OnSaving(ImportConfiguration importconfiguration, ApplicationContext db, GeneratorBase.MVC.Models.IUser onsavinguser)
    {
        //if(importconfiguration.Id > 0)
        //{
        //    //importconfiguration.T_RecordAdded = DateTime.UtcNow;
        //    //importconfiguration.T_RecordAddedUser = onsavinguser.Name;
        //    //db.Entry(importconfiguration).Property(x => x.T_RecordAddedInsertDate).IsModified = false;
        //    //db.Entry(importconfiguration).Property(x => x.T_RecordAddedInsertBy).IsModified = false;
        //}
        //else
        //{
        //    //importconfiguration.T_RecordAddedInsertDate = DateTime.UtcNow;
        //    //importconfiguration.T_RecordAddedInsertBy = onsavinguser.Name;
        //}
        // Write your logic here
    }
    /// <summary>After save.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="aftersaveuser">The application user.</param>
    public void AfterSave(ImportConfiguration importconfiguration, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    /// <summary>ValidateBeforeDocGenerated.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="documentName">       The string.</param>
    /// <param name="outputFormat">The string.</param>
    /// <param name="isdownload">The bool.</param>
    ///
    /// <returns>A string.</returns>
    public string ValidateBeforeDocGenerated(ImportConfiguration importconfiguration, string documentName, string outputFormat, bool? isdownload)
    {
        var AlertMsg = "";
        // Write your logic here
        //Make sure to assign AlertMsg with proper message
        //AlertMsg = "Validation Alert - Before Save !! Information not saved.";
        return AlertMsg;
    }
    /// <summary>Executes the OnDocGenerating.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="documentName">       The string.</param>
    /// <param name="outputFormat">The string.</param>
    /// <param name="isdownload">The bool.</param>
    public void OnDocGenerating(GemBox.Document.DocumentModel document, ImportConfiguration importconfiguration, string documentName, string outputFormat, bool? isdownload)
    {
        // Modify importconfiguration before generating document
        // e.g. trim some values, add/calculate some in other fields and use in template
        // call execute merge for custom/extra fields
        //e.g.
        //Stream stream = new MemoryStream(db.Documents.FirstOrDefault(p => p.Id == importconfiguration.T_Picture).Byte);
        //document.MailMerge.Execute(new { ImageField = stream });
        ////To resize and insert use following code
        //var wi = new WebImage(db.Documents.FirstOrDefault(p => p.Id == importconfiguration.T_Picture).Byte);
        //wi.Resize(30, 30, preventEnlarge: true);
        //document.MailMerge.Execute(new { ImageField = wi.GetBytes() });
    }
    /// <summary>After save.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="documentName">       The string.</param>
    /// <param name="outputFormat">The string.</param>
    /// <param name="isdownload">The bool.</param>
    /// <param name="stream">The Stream.</param>
    public void AfterDocGenerated(ImportConfiguration importconfiguration, string documentName, string outputFormat, bool? isdownload, Stream stream)
    {
        // Write your logic here
        //e.g. save stream in db.Document, or send an email to User.Name/email. Attach document to an entity etc.
    }
    /// <summary>Custom authorization before create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
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
    /// <param name="AssociatedType">   Type of the associated entity.</param>
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
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    
    private void CustomLoadViewDataListAfterOnCreate(ImportConfiguration importconfiguration)
    {
    }
/// <summary>Custom load view data list before edit.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    private void CustomLoadViewDataListBeforeEdit(ImportConfiguration importconfiguration)
    {
    }
/// <summary>Custom load view data list after edit.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    private void CustomLoadViewDataListAfterEdit(ImportConfiguration importconfiguration)
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
    /// <param name="importconfiguration">             The ImportConfiguration.</param>
    /// <param name="customcreate_hasissues">[out] True to customcreate hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnCreate(ImportConfiguration importconfiguration, out bool customcreate_hasissues, string command="")
    {
        var result = false;
        customcreate_hasissues = false;
        return result;
    }
/// <summary>Custom save on edit.</summary>
    ///
    /// <param name="importconfiguration">           The ImportConfiguration.</param>
    /// <param name="customsave_hasissues">[out] True to customsave hasissues.</param>
    /// <param name="command">             (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnEdit(ImportConfiguration importconfiguration, out bool customsave_hasissues, string command="")
    {
        var result = false;
        customsave_hasissues = false;
        return result;
    }
/// <summary>Custom save on import.</summary>
    ///
    /// <param name="importconfiguration">  The ImportConfiguration.</param>
    /// <param name="customerror">[out] The customerror.</param>
    /// <param name="i">          Zero-based index of the.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomSaveOnImport(ImportConfiguration importconfiguration, out string customerror, int i)
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
    /// <param name="importconfiguration">             The ImportConfiguration.</param>
    /// <param name="customdelete_hasissues">[out] True to customdelete hasissues.</param>
    /// <param name="command">               (Optional) The command.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CustomDelete(ImportConfiguration importconfiguration, out bool customdelete_hasissues, string command="")
    {
        var result = false;
        customdelete_hasissues = false;
        return result;
    }
    /// <summary>Custom sorting.</summary>
    ///
    /// <param name="list">          The IQueryable<ImportConfiguration> list.</param>
    /// <param name="HostingEntity"> The hosting entity.</param>
    /// <param name="AssociatedType">Type of the associated.</param>
    /// <param name="sortBy">        Describes who sort this object.</param>
    /// <param name="isAsc">         The is ascending.</param>
    ///
    /// <returns>An IQueryable<ImportConfiguration></returns>
    private IQueryable<ImportConfiguration> CustomSorting(IQueryable<ImportConfiguration> list, string HostingEntity, string AssociatedType, string sortBy, string isAsc)
    {
        IQueryable<ImportConfiguration> orderedList = list;
        // Do custom sorting here, you can also use this method to do custom filter
        return orderedList;
    }
/// <summary>Creates a result that redirects to the given route.</summary>
    ///
    /// <param name="importconfiguration">The ImportConfiguration.</param>
    /// <param name="action">   The action.</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A RedirectToRouteResult.</returns>
    private RedirectToRouteResult CustomRedirectUrl(ImportConfiguration importconfiguration,string action,string command="", string viewmode="")
    {
        // Sample Custom implemention below
        // return RedirectToAction("Index", "ImportConfiguration");
        return null;
    }
    
    private IQueryable<ImportConfiguration> CustomDropdownFilter(IQueryable<ImportConfiguration> list, string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string RestrictDropdownVal, string CustomParameter)
    {
        return list;
    }
}
}
