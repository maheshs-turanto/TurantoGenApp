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
[Table("tbl_MultiTenantLoginSelected"), CustomDisplayName("MultiTenantLoginSelected", "MultiTenantLoginSelected.resx", "Application Security Access")]
public class MultiTenantLoginSelected : EntityDefault
{
    [CustomDisplayName("T_AccessNo", "MultiTenantLoginSelected.resx", "AccessNo"), Column("AccessNo")]
    public Nullable<long> T_AccessNo
    {
        get;
        set;
    }
    
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    [CustomDisplayName("T_AccessDateTime", "MultiTenantLoginSelected.resx", "AccessDateTime"), Column("AccessDateTime")]
    [Required]
    public DateTime T_AccessDateTime
    {
        get;
        set;
    }
    
    [CustomDisplayName("T_User", "MultiTenantLoginSelected.resx", "User"), Column("User")]
    [Required]
    public string T_User
    {
        get;
        set;
    }
    
    [CustomDisplayName("T_MainEntity", "MultiTenantLoginSelected.resx", "MainEntity"), Column("MainEntity")]
    [Required]
    public long? T_MainEntity
    {
        get;
        set;
    }
    
    [CustomDisplayName("T_MainEntityValue", "MultiTenantLoginSelected.resx", "MainEntityValue"), Column("MainEntityValue")]
    public string T_MainEntityValue
    {
        get;
        set;
    }
    
    public string getDisplayValue()
    {
        try
        {
            var dispValue = (this.T_AccessNo != null ? Convert.ToString(this.T_AccessNo) + " " : Convert.ToString(this.T_AccessNo));
            dispValue = dispValue != null ? dispValue.TrimEnd("   ".ToCharArray()) : "";
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.T_AccessNo != null ? Convert.ToString(this.T_AccessNo) + " " : Convert.ToString(this.T_AccessNo));
            return dispValue != null ? dispValue.TrimEnd("   ".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
    }
    public void setCalculation()
    {
    }
}
[Table("tbl_LoginSelectedRoles"), CustomDisplayName("LoginSelectedRoles", "LoginSelectedRoles.resx", "LoginSelectedRoles")]
public class LoginSelectedRoles : EntityDefault
{
    [CustomDisplayName("User", "LoginSelectedRoles.resx", "User"), Column("T_User")]
    [Required]
    public string User
    {
        get;
        set;
    }
    [CustomDisplayName("Roles", "LoginSelectedRoles.resx", "Roles"), Column("T_Roles")]
    public string Roles
    {
        get;
        set;
    }
    public string getDisplayValue()
    {
        try
        {
            var dispValue = (this.User != null ? Convert.ToString(this.User) + " " : Convert.ToString(this.User));
            dispValue = dispValue != null ? dispValue.TrimEnd("   ".ToCharArray()) : "";
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.User != null ? Convert.ToString(this.User) + " " : Convert.ToString(this.User));
            return dispValue != null ? dispValue.TrimEnd("   ".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
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
