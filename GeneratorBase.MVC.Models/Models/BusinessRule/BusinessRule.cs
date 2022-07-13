using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
namespace GeneratorBase.MVC.Models
{
/// <summary>The business rule.</summary>
[Table("tbl_BusinessRule")]
public class BusinessRule
{
    /// <summary>Default constructor.</summary>
    public BusinessRule()
    {
        this.ruleconditions = new HashSet<Condition>();
        this.ruleaction = new HashSet<RuleAction>();
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
    
    /// <summary>Gets or sets the name of the rule.</summary>
    ///
    /// <value>The name of the rule.</value>
    
    [DisplayName("Rule Name")]
    [Required]
    public string RuleName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the entity.</summary>
    ///
    /// <value>The name of the entity.</value>
    
    [DisplayName("Entity Name")]
    public string EntityName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the description.</summary>
    ///
    /// <value>The description.</value>
    
    [DisplayName("Description")]
    public string Description
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the roles.</summary>
    ///
    /// <value>The roles.</value>
    
    [DisplayName("Roles")]
    public string Roles
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the date created 1.</summary>
    ///
    /// <value>The date created 1.</value>
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    [DisplayName("Date Created")]
    public Nullable<DateTime> DateCreated1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the entity subscribe.</summary>
    ///
    /// <value>True if entity subscribe, false if not.</value>
    
    [DisplayName("Notify me")]
    public Boolean EntitySubscribe
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is disabled.</summary>
    ///
    /// <value>True if disable, false if not.</value>
    
    [DisplayName("Is Disabled?")]
    public Boolean Disable
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether the freeze.</summary>
    ///
    /// <value>True if freeze, false if not.</value>
    
    [DisplayName("Is Freezed?")]
    public Boolean Freeze
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a message describing the information.</summary>
    ///
    /// <value>A message describing the information.</value>
    
    [DisplayName("Information Message")]
    public string InformationMessage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a message describing the failure.</summary>
    ///
    /// <value>A message describing the failure.</value>
    
    [DisplayName("Failure Message")]
    public string FailureMessage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the associated business rule type.</summary>
    ///
    /// <value>The identifier of the associated business rule type.</value>
    
    [DisplayName("BusinessRule Type")]
    public Nullable<long> AssociatedBusinessRuleTypeID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the associatedbusinessruletype.</summary>
    ///
    /// <value>The associatedbusinessruletype.</value>
    
    [JsonIgnore]
    public virtual BusinessRuleType associatedbusinessruletype
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the ruleconditions.</summary>
    ///
    /// <value>The ruleconditions.</value>
    
    public virtual ICollection<Condition> ruleconditions
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the ruleaction.</summary>
    ///
    /// <value>The ruleaction.</value>
    
    public virtual ICollection<RuleAction> ruleaction
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
        return this.EntityName + "-" + Convert.ToString(this.RuleName);
    }
    
    /// <summary>Gets or sets the identifier of the scheduler task.</summary>
    ///
    /// <value>The identifier of the scheduler task.</value>
    
    [DisplayName("Scheduler Task"), Column("SchedulerTaskID")]
    public Nullable<long> T_SchedulerTaskID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the schedulertask.</summary>
    ///
    /// <value>The t schedulertask.</value>
    
    public virtual T_Schedule t_schedulertask
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the action type.</summary>
    ///
    /// <value>The identifier of the action type.</value>
    
    [NotMapped]
    public List<int> ActionTypeID
    {
        get;
        set;
    }
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}

