using GeneratorBase.MVC.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>Attribute for audit.</summary>
public class AuditAttribute : ActionFilterAttribute
{
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    private string name
    {
        get;
        set;
    }
    private bool? journaleverything
    {
        get;
        set;
    }
    private int mode
    {
        get;
        set;
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="name">The name.</param>
    
    public AuditAttribute(string name)
    {
        this.mode = 1;
        this.name = name;
        this.journaleverything = null;
    }
    /// <summary>Default constructor.</summary>
    public AuditAttribute()
    {
        this.mode = 1;
        this.name = "";
        this.journaleverything = null;
    }
    public AuditAttribute(bool frombase)
    {
        this.mode = 1;
        this.name = "";
        this.journaleverything = GeneratorBase.MVC.Models.CommonFunction.Instance.JournalEverything();
    }
    public AuditAttribute(int mode) //0 to ignore
    {
        this.mode = mode;
        this.name = "";
        this.journaleverything = null;
    }
    /// <summary>Called by the ASP.NET MVC framework before the action method executes.</summary>
    ///
    /// <param name="filterContext">The filter context.</param>
    
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if(this.mode == 0) return;
        //Stores the Request in an Accessible object
        if(string.IsNullOrEmpty(name) && !this.journaleverything.Value) return;
        if(string.IsNullOrEmpty(name)) name = filterContext.ActionDescriptor.ActionName;
        if(Enum.IsDefined(typeof(IgnoreActions), name)) return;
        var request = filterContext.HttpContext.Request;
        string entity = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        int RecdId = 0;
        if(filterContext.ActionParameters.Count()>0 && filterContext.ActionParameters.First().Key.ToUpper() == "ID")
            Int32.TryParse(Convert.ToString(filterContext.ActionParameters.First().Value), out RecdId);
        //RecdId = Convert.ToInt32(filterContext.ActionParameters.First().Value);
        var relativeUrl = VirtualPathUtility.MakeRelative("~", request.Url.AbsolutePath);
        JournalEntry audit = new JournalEntry()
        {
            UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
            RoleName = (request.IsAuthenticated) ?string.Join(",",((CustomPrincipal)filterContext.HttpContext.User).userroles) : "Anonymous",
            EntityName = entity,
            RecordInfo = "<a href=\"" + relativeUrl.Replace("RenderPartial=True&","").Replace("EditQuick","Edit") + "\">" + (RecdId > 0 ? EntityComparer.GetDisplayValueForAssociation(entity, Convert.ToString(RecdId)) : "Click to view") + "</a>",
            DateTimeOfEntry = DateTime.UtcNow,
            RecordId = RecdId,
            BrowserInfo = DoAuditEntry.GetBrowserDetails(),
            Type =  name
        };
        //Stores the Audit in the Database
        using(JournalEntryContext context = new JournalEntryContext(new SystemUser()))
        {
            context.JournalEntries.Add(audit);
            context.DirectSaveChanges();
        }
        //Finishes executing the Action as normal
        base.OnActionExecuting(filterContext);
    }
    private enum IgnoreActions
    {
        ShowHelpIcon,
        NotFound404,
        GetAllValue,
        GetAllValueForFilter,
        GetAllValueForRB,
        GetAllMultiSelectValueForBR,
        GetAllMultiSelectValue
    }
}
}