using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneratorBase.MVC.Models
{
/// <summary>The reports in role.</summary>
[Table("tbl_ReportsInRole")]
public class ReportsInRole : EntityDefault
{
    /// <summary>Gets or sets the identifier of the report.</summary>
    ///
    /// <value>The identifier of the report.</value>
    
    [DisplayName("Report")]
    public string ReportId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the role.</summary>
    ///
    /// <value>The identifier of the role.</value>
    
    [DisplayName("Roles")]
    public string RoleId
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
    
    /// <summary>Gets or sets the flag.</summary>
    ///
    /// <value>The flag.</value>
    
    [DisplayName("Flag")]
    public Nullable<bool> Flag
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the argument.</summary>
    ///
    /// <value>The argument.</value>
    
    [DisplayName("Argument")]
    public string Argument
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public string getDisplayValue()
    {
        try
        {
            var dispValue = (this.ReportId != null ? Convert.ToString(this.ReportId) + " " : Convert.ToString(this.ReportId));
            dispValue = dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
    
    /// <summary>Gets display value model.</summary>
    ///
    /// <returns>The display value model.</returns>
    
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.ReportId != null ? Convert.ToString(this.ReportId) + " " : Convert.ToString(this.ReportId));
            return dispValue != null ? dispValue.TrimEnd(" ".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
    }
    
    /// <summary>Gets or sets the selected role identifier.</summary>
    ///
    /// <value>The identifier of the selected role.</value>
    
    [NotMapped]
    public string[] SelectedRoleId
    {
        get;
        set;
    }
}
}