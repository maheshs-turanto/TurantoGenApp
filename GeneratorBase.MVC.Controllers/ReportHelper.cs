//using GeneratorBase.MVC.WebReference;
using GeneratorBase.MVC.Controllers.WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
namespace GeneratorBase.MVC.Models
{
/// <summary>Testing Handler to restrict file downloading based on if user session doesn't exist.
/// for instance my User session will be Session["User"].</summary>

public class ReportHelper
{

    public static string Error
    {
        get;
        set;
    }
    /// <summary>Gets all report.</summary>
    ///
    /// <returns>all report.</returns>
    
    public static List<CatalogItem> GetAllReport()
    {
        Error = string.Empty;
        var items = new List<CatalogItem>();
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
                items = children.ToList();
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
            Error = "Please check report server configuration...!";
            items = new List<CatalogItem>();
        }
        log.AppendLine(String.Format("Found {0} reports.", items.Count));
        return items;
    }
    
}
}


