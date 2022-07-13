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
using System.IO;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling company profiles.</summary>
public class CompanyProfileController : Controller
{
    /// <summary>The repository.</summary>
    private ICompanyProfileRepository _repository;
    /// <summary>Default constructor.</summary>
    public CompanyProfileController()
        : this(new CompanyProfileRepository())
    {
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(_repository != null) _repository = null;
        }
        base.Dispose(disposing);
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="repository">The repository.</param>
    
    public CompanyProfileController(ICompanyProfileRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [Audit("Edit")]
    public ActionResult Edit(long? tenantId, bool footer=false)
    {
        if(((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
        {
            CompanyProfile cp = _repository.GetCompanyProfile(((CustomPrincipal)User),tenantId);
            if(cp == null)
                return RedirectToAction("Index");
            ViewBag.TenantList = _repository.SetViewBag((CustomPrincipal)User,tenantId); //new SelectList(list, "Key", "Value", tenantId);
            if(footer)
                ViewData["footer"] = true;
            return View(cp);
        }
        else return RedirectToAction("Index", "Home");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="cp">                     The cp.</param>
    /// <param name="Icon">                   The icon.</param>
    /// <param name="Logo">                   The logo.</param>
    /// <param name="LoginBg">                The login background.</param>
    /// <param name="LegalInformationAttach"> The legal information attach.</param>
    /// <param name="PrivacyPolicyAttach">    The privacy policy attach.</param>
    /// <param name="TermsOfServiceAttach">   The terms of service attach.</param>
    /// <param name="ThirdPartyAttach">       The third party attach.</param>
    /// <param name="CookieInformationAttach">The cookie information attach.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [Audit("Edit")]
    public ActionResult Edit(CompanyProfile cp, HttpPostedFileBase Icon, HttpPostedFileBase Logo, HttpPostedFileBase LoginBg, HttpPostedFileBase LegalInformationAttach, HttpPostedFileBase PrivacyPolicyAttach, HttpPostedFileBase TermsOfServiceAttach, HttpPostedFileBase ThirdPartyAttach, HttpPostedFileBase CookieInformationAttach)
    {
        if(((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
        {
            if(ModelState.IsValid)
            {
                try
                {
                    string command = Request.Form["hdncommand"];
                    string path = Server.MapPath("~/logo/");
                    string pathPolicyAndService = Server.MapPath("~/PolicyAndService/");
                    var tenantSuffix = "";
                    if(cp.TenantId.HasValue && cp.TenantId.Value > 0)
                    {
                        tenantSuffix =Convert.ToString(cp.TenantId.Value);
                    }
                    if(!cp.SSL.HasValue)
                        cp.SSL = false;
                    if(Icon != null)
                    {
                        cp.Icon = "logo" + tenantSuffix + ".gif";
                        Icon.SaveAs(path + "logo" + tenantSuffix + ".gif");
                    }
                    if(Logo != null)
                    {
                        cp.Logo = "logo_white" + tenantSuffix + ".gif";
                        Logo.SaveAs(path + "logo_white" + tenantSuffix + ".png");
                    }
                    if(LoginBg != null)
                    {
                        cp.LoginBg = "Loginbg" + tenantSuffix + ".gif";
                        LoginBg.SaveAs(path + "Loginbg" + tenantSuffix + ".jpg");
                        LoginBg.SaveAs(path + "Loginbg" + tenantSuffix + ".png");
                    }
                    if(LegalInformationAttach != null)
                    {
                        //cp.LegalInformation = "logo_white" + tenantSuffix + ".gif";
                        LegalInformationAttach.SaveAs(pathPolicyAndService + tenantSuffix + "Licensing.pdf");
                    }
                    if(PrivacyPolicyAttach != null)
                    {
                        //cp.LegalInformation = "logo_white" + tenantSuffix + ".gif";
                        PrivacyPolicyAttach.SaveAs(pathPolicyAndService + tenantSuffix + "PrivacyPolicy.pdf");
                    }
                    if(TermsOfServiceAttach != null)
                    {
                        //cp.LegalInformation = "logo_white" + tenantSuffix + ".gif";
                        TermsOfServiceAttach.SaveAs(pathPolicyAndService + tenantSuffix + "Terms_Of_Service.pdf");
                    }
                    if(ThirdPartyAttach != null)
                    {
                        //cp.LegalInformation = "logo_white" + tenantSuffix + ".gif";
                        ThirdPartyAttach.SaveAs(pathPolicyAndService + tenantSuffix + "Third_Party_Licenses.pdf");
                    }
                    if(CookieInformationAttach != null)
                    {
                        //cp.CookieInformation = "logo_white" + tenantSuffix + ".gif";
                        CookieInformationAttach.SaveAs(pathPolicyAndService + tenantSuffix + "CookiePolicy.pdf");
                    }
                    _repository.EditCompanyProfile(cp);
                    //journaling
                    DoAuditEntry.AddJournalEntryCommon(((CustomPrincipal)User),null,"Company Profile Data Modified","Company Profile");
                    if(command == "Refresh")
                        return RedirectToAction("Edit", "CompanyProfile", new { footer = "true" });
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
    
    /// <summary>Bulk email.</summary>
    ///
    /// <returns>A response stream to send to the BulkEmail View.</returns>
    
    public ActionResult BulkEmail()
    {
        if(((CustomPrincipal)User).CanAddAdminFeature("BulkEmail"))
        {
            ViewBag.TenantList = _repository.SetViewBagEmail((CustomPrincipal)User);
            var rolelist = new List<ApplicationRole>();
            var roleinfo = new ApplicationRole();
            roleinfo.Id = "All";
            roleinfo.Name = "All";
            rolelist.Add(roleinfo);
            rolelist.AddRange(new ApplicationDbContext((CustomPrincipal)User).Roles.OrderBy(p => p.Name).ToList());
            ViewBag.RoleList = new SelectList(rolelist.ToList(), "Id", "Name");
            return View();
        }
        else return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    [ValidateInput(false)]
    
    /// <summary>(An Action that handles HTTP POST requests) bulk email.</summary>
    ///
    /// <param name="TenantList">List of tenants.</param>
    /// <param name="subject">   The subject.</param>
    /// <param name="body">      The body.</param>
    ///
    /// <returns>A response stream to send to the BulkEmail View.</returns>
    [Audit("BulkEmail")]
    public ActionResult BulkEmail(long[] TenantList, string subject, string body, string RoleListValue)
    {
        if(((CustomPrincipal)User).CanAddAdminFeature("BulkEmail"))
        {
            var user = new ApplicationDbContext(true).Users.FirstOrDefault(p => p.UserName == ((CustomPrincipal)User).Name);
            var userid = "";
            var toUsers = "";
            if(user != null) userid = user.Id;
            if(TenantList != null && TenantList.Count() > 0)
            {
                var tenantusers = new List<string>();
                foreach(var tenant in TenantList)
                    tenantusers.AddRange(_repository.getEmailUsersByTenant(tenant));
                toUsers = String.Join(",", tenantusers.Where(wh => !string.IsNullOrEmpty(wh)).GroupBy(gr => gr).Select(s => s.First()).ToList());
            }
            else
            {
                using(ApplicationDbContext db = new ApplicationDbContext((CustomPrincipal)User))
                {
                    var users = (IQueryable<ApplicationUser>)db.Users;
                    if(!string.IsNullOrEmpty(RoleListValue) && !RoleListValue.Contains("All"))
                    {
                        var listrole  = RoleListValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        users =  users.Where(p => p.Roles.Any(a => listrole.Contains(a.RoleId)));
                    }
                    toUsers = String.Join(",", users.Where(wh => !(wh.LockoutEnabled)).Select(p => p.Email));
                }
            }
            SendEmail mail = new SendEmail();
            if(!string.IsNullOrEmpty(toUsers))
            {
                var emailstatus = mail.Notify(userid, toUsers, body, subject);
                if(emailstatus != "EmailSent")
                    ViewBag.Message = emailstatus;
                else
                    ViewBag.Message = "Email Sent to users.";
                //mail.Notify(userid, toUsers, body, subject);
                //ViewBag.Message = "Email Sent to users.";
            }
            else
            {
                ViewBag.Message = "Email list not found.";
            }
        }
        ViewBag.TenantList = _repository.SetViewBagEmail((CustomPrincipal)User);
        var rolelist = new List<ApplicationRole>();
        var roleinfo = new ApplicationRole();
        roleinfo.Id = "All";
        roleinfo.Name = "All";
        rolelist.Add(roleinfo);
        rolelist.AddRange(new ApplicationDbContext((CustomPrincipal)User).Roles.OrderBy(p => p.Name).ToList());
        ViewBag.RoleList = new SelectList(rolelist.ToList(), "Id", "Name");
        return View();
    }
    
}
}

