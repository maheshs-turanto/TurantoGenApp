using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace GeneratorBase.MVC.Models
{
/// <summary>A rule action.</summary>
[Table("tbl_RuleAction")]
public class RuleAction
{
    /// <summary>Default constructor.</summary>
    public RuleAction()
    {
        this.actionarguments = new HashSet<ActionArgs>();
    }
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [Key]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the action.</summary>
    ///
    /// <value>The name of the action.</value>
    
    [DisplayName("Action Name")]
    [Required]
    public string ActionName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a message describing the error.</summary>
    ///
    /// <value>A message describing the error.</value>
    
    [DisplayName("Send Email To")]
    [AllowHtml]
    public string ErrorMessage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is else action.</summary>
    ///
    /// <value>True if this object is else action, false if not.</value>
    
    [DisplayName("Is Else Action")]
    public Boolean IsElseAction
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the rule action.</summary>
    ///
    /// <value>The identifier of the rule action.</value>
    
    [DisplayName("Business Rule")]
    public Nullable<long> RuleActionID
    {
        get;
        set;
    }
    //  [ForeignKey("RuleActionID")]
    [JsonIgnore]
    //[ScriptIgnore]
    
    /// <summary>Gets or sets the ruleaction.</summary>
    ///
    /// <value>The ruleaction.</value>
    
    public virtual BusinessRule ruleaction
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value Document Temaplte.</summary>
    ///
    /// <value>Template Id</value>
    
    [DisplayName("Document Template")]
    public Nullable<long> TemplateId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the associated action type.</summary>
    ///
    /// <value>The identifier of the associated action type.</value>
    
    [DisplayName("Action Type")]
    public Nullable<long> AssociatedActionTypeID
    {
        get;
        set;
    }
    //  [ForeignKey("AssociatedActionTypeID")]
    
    /// <summary>Gets or sets the associatedactiontype.</summary>
    ///
    /// <value>The associatedactiontype.</value>
    
    [JsonIgnore]
    public virtual ActionType associatedactiontype
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the actionarguments.</summary>
    ///
    /// <value>The actionarguments.</value>
    
    public virtual ICollection<ActionArgs> actionarguments
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the display value.</summary>
    ///
    /// <value>The display value.</value>
    
    [DisplayName("DisplayValue")]
    public string DisplayValue
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        return Convert.ToString(this.ActionName);
    }
    
    /// <summary>Gets roles name by identifier.</summary>
    ///
    /// <param name="RolesIds">List of identifiers for the roles.</param>
    ///
    /// <returns>The roles name by identifier.</returns>
    
    public string getRolesNameById(string RolesIds)
    {
        if(RolesIds == "0")
            return "All";
        string result = "";
        string[] RolesIdstr = RolesIds.Split(",".ToCharArray());
        foreach(var item in RolesIdstr)
        {
            using(var usersContext = new ApplicationDbContext(true))
            {
                string rolename = usersContext.Roles.FirstOrDefault(r => item == r.Id).Name;
                result += rolename + ",";
            }
        }
        return result;
    }
    public string getIsEmailNotification(string IsEmailNotification)
    {
        var result = "";
        if(IsEmailNotification == "1")
            result = "<br />Is Email Notification :-<input class='check-box' disabled='disabled' checked='checked' type='checkbox'>";
        if(IsEmailNotification == "0")
            result = "<br />Is Email Notification :-<input class='check-box' disabled='disabled'  type='checkbox'>";
        return result;
    }
    public string getIsWebNotification(string IsWebNotification)
    {
        var result = "";
        if(IsWebNotification == "1")
            result = "<br />Is Web Notification :-<input class='check-box' disabled='disabled' checked='checked' type='checkbox'>";
        if(IsWebNotification == "0")
            result = "<br />Is Web Notification :-<input class='check-box' disabled='disabled' type='checkbox'>";
        return result;
    }
}
}

