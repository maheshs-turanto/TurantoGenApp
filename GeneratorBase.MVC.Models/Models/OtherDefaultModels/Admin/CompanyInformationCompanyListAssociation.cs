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
/// <summary>CompanyInformation CompanyList Association model class: Bridge Entity for M:M association between CompanyInformation and CompanyList..</summary>
[Table("tbl_CompanyInformationCompanyListAssociation"), CustomDisplayName("CompanyInformationCompanyListAssociation", "CompanyInformationCompanyListAssociation.resx", "CompanyInformation CompanyList Association")]
public partial class CompanyInformationCompanyListAssociation : Entity
{
    /// <summary>Gets or sets the TenantId.</summary>
    ///
    /// <value>The TenantId.</value>
    [CustomDisplayName("TenantId", "CompanyInformationCompanyListAssociation.resx", "CompanyList"), Column("TenantId")]
    [PropertyInfoForEntity("CompanyList", "CompanyList", "TenantId")]
    public Nullable<long> TenantId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the CompanyInformation.</summary>
    ///
    /// <value>The CompanyInformation.</value>
    
    
    
    [CustomDisplayName("CompanyInformationID", "CompanyInformationCompanyListAssociation.resx", "CompanyInformation"), Column("CompanyInformationID")]
    public Nullable<long> CompanyInformationID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyInformation navigation property.</summary>
    ///
    /// <value>The companyinformation object.</value>
    
    public virtual CompanyInformation companyinformation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyList.</summary>
    ///
    /// <value>The CompanyList.</value>
    
    
    
    [CustomDisplayName("CompanyListID", "CompanyInformationCompanyListAssociation.resx", "CompanyList"), Column("CompanyListID")]
    public Nullable<long> CompanyListID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyList navigation property.</summary>
    ///
    /// <value>The companylist object.</value>
    
    //public virtual CompanyList companylist
    //{
    //    get;
    //    set;
    //}
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public string getDisplayValue()
    {
        try
        {
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var dispValue = (this.CompanyInformationID != null ? context.CompanyInformations.FirstOrDefault(p => p.Id == this.CompanyInformationID).DisplayValue : "");
                //this.m_DisplayValue = dispValue;
                this.m_DisplayValue = dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
                return dispValue;
            }
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Gets display value model (Actual value to render on UI).</summary>
    ///
    /// <returns>The display value model.</returns>
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.companyinformation != null ? companyinformation.DisplayValue : "");
            return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
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

