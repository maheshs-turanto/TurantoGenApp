using GeneratorBase.MVC.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeneratorBase.MVC;
using System.Collections;
namespace GeneratorBase.MVC.Report
{
/// <summary>A SSRS report viewer.</summary>
public partial class ReportViewer : System.Web.UI.Page
{
    /// <summary>Event handler. Called by Page for load events.</summary>
    ///
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">     Event information.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            string reportName = Request.QueryString["Report"];
            string ID = Request.QueryString["ID"];
            LoadReport(reportName, ID);
        }
    }
    /// <summary>Loads a report.</summary>
    ///
    /// <param name="reportName">Name of the report.</param>
    /// <param name="ID">        The identifier.</param>
    private void LoadReport(string reportName, string ID)
    {
        if(!string.IsNullOrEmpty(reportName))
        {
            MyReportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            MyReportViewer.ServerReport.ReportServerCredentials = new ReportServerCredentials(CommonFunction.Instance.ReportUser(), CommonFunction.Instance.ReportPass(), CommonFunction.Instance.DomainName());
            MyReportViewer.ServerReport.ReportServerUrl = new Uri(CommonFunction.Instance.ReportPath());
            MyReportViewer.ServerReport.ReportPath = reportName;
            JournalEntry je = new JournalEntry();
            var timezone = je.m_Timezone;
            var datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
            var rptName = reportName.Substring(reportName.LastIndexOf('/') + 1);
            MyReportViewer.ServerReport.DisplayName = rptName + "_" + datetime.ToString("MM-dd-yyyy hh-mm tt");
            if(!string.IsNullOrEmpty(ID))
            {
                var reportParameter = new ReportParameter("empId");
                reportParameter.Values.Add(ID);
                MyReportViewer.ServerReport.SetParameters(new[] { reportParameter });
            }
            MyReportViewer.ServerReport.Refresh();
        }
    }
}
/// <summary>A SSRS server credentials.</summary>
public class ReportServerCredentials : IReportServerCredentials
{
    /// <summary>Name of the report server user.</summary>
    private string reportServerUserName;
    /// <summary>The report server password.</summary>
    private string reportServerPassword;
    /// <summary>The report server domain.</summary>
    private string reportServerDomain;
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">[out] The password of the user.</param>
    /// <param name="domain">  The domain.</param>
    public ReportServerCredentials(string userName, string password, string domain)
    {
        reportServerUserName = userName;
        reportServerPassword = password;
        reportServerDomain = domain;
    }
    /// <summary>Gets the <see cref="T:System.Security.Principal.WindowsIdentity" /> of the user to
    /// impersonate when the <see cref="T:Microsoft.Reporting.WebForms.ReportViewer" /> control
    /// connects to a report server.</summary>
    ///
    /// <value>A <see cref="T:System.Security.Principal.WindowsIdentity" /> object that represents
    /// the user to impersonate.</value>
    public System.Security.Principal.WindowsIdentity ImpersonationUser
    {
        get
        {
            // Use default identity.
            return null;
        }
    }
    /// <summary>Gets the network credentials that are used for authentication with the report server.</summary>
    ///
    /// <value>An implementation of <see cref="T:System.Net.ICredentials" /> that contains the
    /// credential information for connecting to a report server.</value>
    public ICredentials NetworkCredentials
    {
        get
        {
            // Use default identity.
            return new NetworkCredential(reportServerUserName, reportServerPassword, reportServerDomain);
        }
    }
    /// <summary>Create new instance.</summary>
    ///
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">[out] The password of the user.</param>
    /// <param name="domain">  The domain.</param>
    public void New(string userName, string password, string domain)
    {
        reportServerUserName = userName;
        reportServerPassword = password;
        reportServerDomain = domain;
    }
    /// <summary>Provides information that will be used to connect to the report server that is
    /// configured for forms authentication.</summary>
    ///
    /// <param name="authCookie">[out] A report server authentication cookie.</param>
    /// <param name="user">      [out] The name of the user.</param>
    /// <param name="password">  [out] The password of the user.</param>
    /// <param name="authority">    [out] The authority to use when authenticating the user, such as
    ///                             a Microsoft Windows domain.</param>
    ///
    /// <returns>true if forms authentication should be used; otherwise, false.</returns>
    public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
    {
        // Do not use forms credentials to authenticate.
        authCookie = null;
        user = null;
        password = null;
        authority = null;
        return false;
    }
}
}

