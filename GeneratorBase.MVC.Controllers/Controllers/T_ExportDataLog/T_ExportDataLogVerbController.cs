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
using System.Linq.Dynamic;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Controllers
{
public partial class T_ExportDataLogController : BaseController
{
    [Audit("Purge")]
    public ActionResult T_Purge(int? id)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataLog t_exportdatalog = null;
        if(db != null)
        {
            t_exportdatalog = db.T_ExportDataLogs.Find(id);
            if(t_exportdatalog == null)
            {
                return HttpNotFound();
            }
            t_exportdatalog.setDateTimeToClientTime();
        }
        var IsPurge = false;
        var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == t_exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID).OrderByDescending(o => o.Id).ToList();
        foreach(var item in exportdetails)
        {
            Type controller = new CreateControllerType(item.T_ChildEntity).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("PurgeExportData");
                object[] MethodParams = new object[] { t_exportdatalog.Id };
                var objresult = string.Empty;
                if(mc != null)
                {
                    objresult = mc.Invoke(objController, MethodParams).ToString();
                    if(!IsPurge)
                        IsPurge = true;
                }
            }
        }
        if(IsPurge)
        {
            t_exportdatalog.T_AssociatedExportDataLogStatusID = db.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Purged").Id;
            db.Entry(t_exportdatalog).State = EntityState.Modified;
            db.SaveChanges();
        }
        return Json(new { result = "Success", message = "Data permanently deleted." }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [Audit("Restore")]
    public ActionResult T_Restore(int? id)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        T_ExportDataLog t_exportdatalog = null;
        if(db != null)
        {
            t_exportdatalog = db.T_ExportDataLogs.Find(id);
            if(t_exportdatalog == null)
            {
                return HttpNotFound();
            }
            t_exportdatalog.setDateTimeToClientTime();
        }
        var IsRestore = false;
        var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == t_exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID).ToList();
        var dbcontxt = new ApplicationContext(new SystemUser(), true);
        foreach(var item in exportdetails)
        {
            Type controller = new CreateControllerType(item.T_ChildEntity).controllerType;
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("RestoreExportData");
                object[] MethodParams = new object[] { t_exportdatalog.Id };
                var objresult = string.Empty;
                if(mc != null)
                {
                    objresult = mc.Invoke(objController, MethodParams).ToString();
                    if(!IsRestore)
                        IsRestore = true;
                }
            }
        }
        if(IsRestore)
        {
            t_exportdatalog.T_AssociatedExportDataLogStatusID = db.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Restored").Id;
            db.Entry(t_exportdatalog).State = EntityState.Modified;
            db.SaveChanges();
        }
        return Json(new { result = "Success", message = "Data Restored sucessfully." }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [Audit("Restore Data")]
    public ActionResult T_RestoreData(long[] ids, string UrlReferrer)
    {
        foreach(var id in ids.Where(p => p > 0))
        {
            T_ExportDataLog t_exportdatalog = null;
            if(db != null)
            {
                t_exportdatalog = db.T_ExportDataLogs.Find(id);
                if(t_exportdatalog == null)
                {
                    return HttpNotFound();
                }
                t_exportdatalog.setDateTimeToClientTime();
            }
            var IsRestore = false;
            var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == t_exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID).ToList();
            foreach(var item in exportdetails)
            {
                Type controller = new CreateControllerType(item.T_ChildEntity).controllerType;
                using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                {
                    System.Reflection.MethodInfo mc = controller.GetMethod("RestoreExportData");
                    object[] MethodParams = new object[] { t_exportdatalog.Id };
                    var objresult = string.Empty;
                    if(mc != null)
                    {
                        objresult = mc.Invoke(objController, MethodParams).ToString();
                        if(!IsRestore)
                            IsRestore = true;
                    }
                }
            }
            if(IsRestore)
            {
                t_exportdatalog.T_AssociatedExportDataLogStatusID = db.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Restored").Id;
                db.Entry(t_exportdatalog).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        return Json(new { result = "Success", message = "Data Restored sucessfully.", isRedirect = true, redirectUrl = UrlReferrer }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    [Audit("Purge Data")]
    public ActionResult T_PurgeData(long[] ids, string UrlReferrer)
    {
        foreach(var id in ids.Where(p => p > 0))
        {
            T_ExportDataLog t_exportdatalog = null;
            if(db != null)
            {
                t_exportdatalog = db.T_ExportDataLogs.Find(id);
                if(t_exportdatalog == null)
                {
                    return HttpNotFound();
                }
                t_exportdatalog.setDateTimeToClientTime();
            }
            var IsPurge = false;
            var exportdetails = db.T_ExportDataDetailss.Where(a => a.T_ExportDataConfigurationExportDataDetailsAssociationID == t_exportdatalog.T_ExportDataConfigurationExportDataLogAssociationID).OrderByDescending(o => o.Id).ToList();
            foreach(var item in exportdetails)
            {
                Type controller = new CreateControllerType(item.T_ChildEntity).controllerType;
                using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                {
                    System.Reflection.MethodInfo mc = controller.GetMethod("PurgeExportData");
                    object[] MethodParams = new object[] { t_exportdatalog.Id };
                    var objresult = string.Empty;
                    if(mc != null)
                    {
                        objresult = mc.Invoke(objController, MethodParams).ToString();
                        if(!IsPurge)
                            IsPurge = true;
                    }
                }
            }
            if(IsPurge)
            {
                t_exportdatalog.T_AssociatedExportDataLogStatusID = db.T_ExportDataLogstatuss.FirstOrDefault(fd => fd.T_Name == "Purged").Id;
                db.Entry(t_exportdatalog).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        return Json(new { result = "Success", message = "Action executed sucessfully.", isRedirect = true, redirectUrl = UrlReferrer }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
}
}
