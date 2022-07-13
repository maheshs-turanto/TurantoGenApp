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
using System.Configuration;
//using GeneratorBase.MVC.WebReference;
using GeneratorBase.MVC.Controllers.WebReference;
using System.Text;
using System.Security.Principal;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling reports.</summary>
public class ReportsController : BaseController
{
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
    
    /// <summary>Result show.</summary>
    ///
    /// <param name="ReportName"> Name of the report.</param>
    /// <param name="id">         The identifier.</param>
    /// <param name="DisplayName">Name of the display.</param>
    ///
    /// <returns>A response stream to send to the ResultShow View.</returns>
    
    public ActionResult ResultShow(string ReportName, string id, string DisplayName)
    {
        var rptName = ReportName.Substring(ReportName.LastIndexOf('/') + 1);
        ViewBag.DisplayName = DisplayName;
        ViewBag.Name = ReportName.Split('&')[0]; ;
        int param = ReportName.Split('&').Length;
        ViewBag.IsAuthenticated = false;
        if(param == 1)
        {
            ViewBag.ReportName = ReportName;
            if(!User.IsAdmin)
                ViewBag.IsAuthenticated = GetAllReport(rptName, true).Count() > 0 ? true : false;
        }
        else
            ViewBag.ReportName = ConfigurationManager.AppSettings["ReportFolder"] + "/" + ReportName;
        var relativeUrl = VirtualPathUtility.MakeRelative("~", Request.Url.AbsolutePath);
        JournalEntry audit = new JournalEntry()
        {
            UserName = User.Name,
            RoleName = string.Join(",", (User).userroles),
            EntityName = "Reports",
            RecordInfo = "<a href=\"" + relativeUrl.Replace("RenderPartial=True&", "").Replace("EditQuick", "Edit") + "?ReportName=" + ReportName + "\">" + "Click to view" + "</a>",
            DateTimeOfEntry = DateTime.UtcNow,
            PropertyName = rptName,
            BrowserInfo = DoAuditEntry.GetBrowserDetails(),
            RecordId = 0,
            Type = "ReportResult"
        };
        using(JournalEntryContext context = new JournalEntryContext())
        {
            context.JournalEntries.Add(audit);
            context.SaveChanges();
        }
        return View();
    }
    
    /// <summary>Indexes.</summary>
    ///
    /// <param name="page">        The page.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="itemsPerPage">The items per page.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    [Audit("ViewList")]
    public ActionResult Index(int? page, string searchString, int? itemsPerPage)
    {
        var items = GetAllReport(searchString);
        int pageSize = 30;
        int pageNumber = (page ?? 1);
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        var _Reports = items.OrderBy(o => o.Name);
        if(Request.IsAjaxRequest())
        {
            return PartialView("IndexPartial", _Reports.ToPagedList(pageNumber, pageSize));
        }
        return View(_Reports.ToPagedList(pageNumber, pageSize));
    }
    
    /// <summary>Gets all reports in this collection.</summary>
    ///
    /// <param name="searchString">The search string.</param>
    /// <param name="IsReportName">(Optional) True if is report name, false if not.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process all reports in this
    /// collection.</returns>
    
    public IEnumerable<CatalogItem> GetAllReport(string searchString, bool IsReportName = false)
    {
        var items = new HashSet<CatalogItem>();
        var reportUser = CommonFunction.Instance.ReportUser();
        var reportPass = CommonFunction.Instance.ReportPass();
        var root = CommonFunction.Instance.ReportFolder();
        var reportPath = CommonFunction.Instance.ReportPath();
        var log = new StringBuilder();
        log.AppendLine("Report Log:");
        try
        {
            using(var rs = new ReportingService2005())
            {
                rs.Url = reportPath + @"/ReportService2005.asmx";
                log.AppendLine(String.Format("\tTarget: {0}", rs.Url));
                rs.Credentials = new System.Net.NetworkCredential(
                    reportUser,
                    reportPass
                );
                log.AppendLine("Credentials set.");
                log.AppendLine(String.Format("Listing children under: {0}", root));
                var children = rs.ListChildren(root, true)
                               .Where(c => c.Type == ItemTypeEnum.Report)
                               .Where(c => !c.Hidden);
                log.AppendLine(String.Format("Retrieved {0} items.", children.Count()));
                var filtered = FilterItemsByString(children, log, searchString, IsReportName);
                items = FilterItemsByRole(filtered, log);
            }
        }
        catch(Exception ex)
        {
            var exLog = new StringBuilder(log.ToString());
            exLog.AppendLine("Credentials:");
            exLog.AppendLine(String.Format("\tUser: {0}", reportUser));
            //exLog.AppendLine(String.Format("\tPassword: {0}", CommonFunction.Instance.ReportPass())); //We should not log passwords to ELMAH.
            exLog.AppendLine(String.Format("\tDomain: {0}", CommonFunction.Instance.DomainName()));
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(exLog.ToString(), ex));
            items = new HashSet<CatalogItem>();
        }
        log.AppendLine(String.Format("Found {0} reports.", items.Count));
        ViewBag.Log = log.ToString();
        return items;
    }
    
    /// <summary>Enumerates filter items by string in this collection.</summary>
    ///
    /// <param name="children">    The children.</param>
    /// <param name="log">         The log.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsReportName">(Optional) True if is report name, false if not.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process filter items by string in
    /// this collection.</returns>
    
    private IEnumerable<CatalogItem> FilterItemsByString(IEnumerable<CatalogItem> children, StringBuilder log, string searchString, bool IsReportName = false)
    {
        if(!String.IsNullOrEmpty(searchString))
        {
            ViewBag.CurrentFilter = searchString;
            if(IsReportName)
            {
                children = children.Where(c => (!String.IsNullOrEmpty(c.Name) && c.Name == searchString));
                return children;
            }
            children = children.Where(c => (!String.IsNullOrEmpty(c.Name) && c.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                                      || (!String.IsNullOrEmpty(c.Description) && c.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0));
        }
        return children;
    }
    
    /// <summary>Filter items by role.</summary>
    ///
    /// <param name="children">The children.</param>
    /// <param name="log">     The log.</param>
    ///
    /// <returns>A HashSet&lt;CatalogItem&gt;</returns>
    
    private HashSet<CatalogItem> FilterItemsByRole(IEnumerable<CatalogItem> children, StringBuilder log)
    {
        var items = new HashSet<CatalogItem>();
        if(!this.User.IsAdmin)
        {
            log.AppendLine("Not an admin.");
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
            long x = 0;
            List<long> SelectedreportID = reportIDs.Distinct().Where(str => long.TryParse(str, out x))
                                          .Select(str => x)
                                          .ToList();
            var reportGUID = db.ReportLists.Where(p => SelectedreportID.Contains(p.Id)).Select(p => p.ReportID).ToList();
            foreach(var child in children.Where(c => reportGUID.Contains(c.ID)))
            {
                log.AppendLine(String.Format("\tMatch found: {0}", child.Name));
                items.Add(child);
            }
        }
        else
        {
            log.AppendLine("Admin!");
            items = new HashSet<CatalogItem>(children);
        }
        return items;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [Audit("Edit")]
    public ActionResult Edit(string id, string UrlReferrer, bool? IsAddPop)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ReportsInRole objreportsinrole = new ReportsInRole();
        var reportsinrole = db.ReportsInRoles.Where(p => p.ReportId == id);
        objreportsinrole.ReportId = id;
        if(reportsinrole.Count() > 0)
            objreportsinrole.SelectedRoleId = reportsinrole.Select(r => r.RoleId).ToArray();
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesReport();
        ViewBag.RoleId = RoleList;
        ViewBag.IsAddPop = IsAddPop;
        ViewData["ReportsInRoleParentUrl"] = UrlReferrer;
        return View(objreportsinrole);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="reportsinrole">The reportsinrole.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="IsAddPop">     The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    public ActionResult Edit([Bind(Include = "Id,ReportId,RoleId,SelectedRoleId")] ReportsInRole reportsinrole, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid || reportsinrole.Id == 0)
        {
            var objreportinrole = db.ReportsInRoles.Where(r => r.ReportId == reportsinrole.ReportId).ToList();
            foreach(var obj in objreportinrole)
            {
                db.ReportsInRoles.Remove(obj);
                db.SaveChanges();
            }
            if(reportsinrole.SelectedRoleId != null)
            {
                foreach(var role in reportsinrole.SelectedRoleId)
                {
                    ReportsInRole objreportinroleadd = new ReportsInRole();
                    objreportinroleadd.ReportId = reportsinrole.ReportId;
                    objreportinroleadd.RoleId = role;
                    db.Entry(objreportinroleadd).State = EntityState.Added;
                    db.ReportsInRoles.Add(objreportinroleadd);
                }
                db.SaveChanges();
            }
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
}
}

