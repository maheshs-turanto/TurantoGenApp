using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>An API token.</summary>
[Table("tbl_ApiToken"), CustomDisplayName("T_ApiToken", "T_ApiToken.resx", "Api Client")]
public class ApiToken : EntityDefault
{



    [CustomDisplayName("T_AuthToken", "ApiToken.resx", "AuthToken"), Column("AuthToken")]
    [Required]
    
    /// <summary>Gets or sets the authentication token.</summary>
    ///
    /// <value>The t authentication token.</value>
    
    public string T_AuthToken
    {
        get;
        set;
    }
    
    [DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    
    [CustomDisplayName("T_IssuedOn", "ApiToken.resx", "IssuedOn"), Column("IssuedOn")]
    [Required]
    
    /// <summary>Gets or sets the Date/Time of the issued on.</summary>
    ///
    /// <value>The t issued on.</value>
    
    public DateTime T_IssuedOn
    {
        get;
        set;
    }
    
    [DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    
    [CustomDisplayName("T_ExpiresOn", "ApiToken.resx", "ExpiresOn"), Column("ExpiresOn")]
    [Required]
    
    /// <summary>Gets or sets the Date/Time of the expires on.</summary>
    ///
    /// <value>The t expires on.</value>
    
    public DateTime T_ExpiresOn
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the users.</summary>
    ///
    /// <value>The identifier of the users.</value>
    
    [CustomDisplayName("T_UsersID", "ApiToken.resx", "Users"), Column("UsersID")]
    public string T_UsersID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the users.</summary>
    ///
    /// <value>The t users.</value>
    
    public virtual IdentityUser t_users
    {
        get;
        set;
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <returns>The display value.</returns>
    
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = "";
            dispValue = dispValue!=null?dispValue.TrimEnd("".ToCharArray()):"";
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
            var dispValue = "";
            return dispValue!=null?dispValue.TrimEnd("".ToCharArray()):"";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets the calculation.</summary>
    public void setCalculation()
    {
    }
}
}

