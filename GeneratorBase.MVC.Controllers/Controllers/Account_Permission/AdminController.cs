using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling admins.</summary>
public class AdminController : BaseController
{
    /// <summary>The repository.</summary>
    private IAdminSettingsRepository _repository;
    /// <summary>Default constructor.</summary>
    public AdminController()
        : this(new AdminSettingsRepository())
    {
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="repository">The repository.</param>
    
    public AdminController(IAdminSettingsRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    [Audit("View")]
    public ActionResult Index()
    {
        if(!((CustomPrincipal)User).HasAdminPrivileges())
            return RedirectToAction("Index", "Home");
        var fileList = Directory.GetFiles(Server.MapPath(@"~/Templates/"), "*.docx").Where(file => Regex.IsMatch(Path.GetFileName(file), "^[0-9]+"))
                       .Select(f =>
        {
            int id = Int32.Parse(Path.GetFileNameWithoutExtension(f.ToLower()).Replace(".docx", ""));
            string Name = documentName(id).ToLower().Replace(".docx", "");
            return new { id, Name };
        }).ToList();
        if(fileList.Count > 0)
            ViewBag.FileList = fileList;
        return View();
    }
    
    /// <summary>Document name.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A string.</returns>
    
    private string documentName(int id)
    {
        string docName = string.Empty;
        switch(id)
        {
        case 1:
            docName = "Common User's Guide.docx";
            break;
        case 2:
            docName = "General Deployment Guide.docx";
            break;
        case 3:
            docName = "Security Guide.docx";
            break;
        case 4:
            docName = "Configuring Business Rules.docx";
            break;
        //case 5:
        //    docName = "Database Design Document.docx";
        //    break;
        case 9:
            docName = "Troubleshooting Guide.docx";
            break;
        //case 10:
        //    docName = "Turanto User's Guide.docx";
        //    break;
        case 6:
            docName = ".NET Documentation Instructions.docx";
            break;
        case 7:
            docName = "Current Security Settings.docx";
            break;
        case 8:
            docName = "Current Business Rules.docx";
            break;
        default:
            break;
        }
        return docName;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="adminSettings">The admin settings.</param>
    /// <param name="logo">         The logo.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    public ActionResult Edit(AdminSettings adminSettings, HttpPostedFileBase logo)
    {
        if(((CustomPrincipal)User).IsAdmin)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _repository.EditAdminSettings(adminSettings);
                    return RedirectToAction("Index", "Admin");
                }
                catch(Exception ex)
                {
                    //error msg for failed edit in XML file
                    ModelState.AddModelError("", "Error editing record. " + ex.Message);
                }
            }
        }
        return RedirectToAction("Index", "Home");
    }
    
    /// <summary>Sets default role.</summary>
    ///
    /// <param name="RoleName">Name of the role.</param>
    ///
    /// <returns>A response stream to send to the SetDefaultRole View.</returns>
    
    public ActionResult SetDefaultRole(string RoleName)
    {
        if(((CustomPrincipal)User).IsAdmin)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    AdminSettings adminSettings = new AdminSettings();
                    adminSettings.DefaultRoleForNewUser = RoleName;
                    _repository.EditAdminSettings(adminSettings);
                    return RedirectToAction("RoleList", "Account");
                }
                catch(Exception ex)
                {
                    //error msg for failed edit in XML file
                    ModelState.AddModelError("", "Error editing record. " + ex.Message);
                }
            }
        }
        return RedirectToAction("Index", "Home");
    }
    
    /// <summary>Downloads the test script.</summary>
    ///
    /// <returns>A FileResult.</returns>
    
    public FileResult DownloadTestScript()
    {
        string zipsolutionPath = Server.MapPath("~/SeleniumTestScript");
        FileZipper fz;
        fz = new FileZipper();
        fz.CopyZip(zipsolutionPath, "TestScript.rar");
        string solutionPath = zipsolutionPath;
        solutionPath = zipsolutionPath + "\\TestScript.rar";
        if(System.IO.File.Exists(solutionPath))
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(solutionPath);
            string fileName = "TestScript.rar";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        return null;
    }
    
    /// <summary>Downloads the SQL script.</summary>
    ///
    /// <returns>A FileResult.</returns>
    
    public FileResult DownloadSQLScript()
    {
        if(!User.IsAdmin) return null;
        string zipsolutionPath = Server.MapPath("~/App_Data/SQLScript");
        FileZipper fz;
        var dateTime = "";
        var name = "SQLScript" + dateTime + ".rar";
        fz = new FileZipper();
        fz.CopyZip(zipsolutionPath, name);
        string solutionPath = zipsolutionPath;
        solutionPath = zipsolutionPath + "\\" + name;
        if(System.IO.File.Exists(solutionPath))
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(solutionPath);
            string fileName = name;
            System.IO.File.Delete(solutionPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        return null;
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
            if(_repository != null) _repository = null;
        }
        base.Dispose(disposing);
    }
}
}