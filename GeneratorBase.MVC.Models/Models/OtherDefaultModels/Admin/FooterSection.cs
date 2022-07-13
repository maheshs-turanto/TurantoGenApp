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
/// <summary>Footer Section  model class: Footer Section.</summary>
[Table("tbl_FooterSection"),CustomDisplayName("FooterSection", "FooterSection.resx", "Footer Section ")]
public partial class FooterSection : Entity
{
    /// <summary>Gets or sets the TenantId.</summary>
    ///
    /// <value>The TenantId.</value>
    [CustomDisplayName("TenantId", "FooterSection.resx", "CompanyList"), Column("TenantId")]
    [PropertyInfoForEntity("CompanyList", "CompanyList", "TenantId")]
    public Nullable<long> TenantId
    {
        get;
        set;
    }
    
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The Name.</value>
    [CustomDisplayName("Name","FooterSection.resx","Name"), Column("Name")] [Required]
    public string Name
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Web Link Title cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Web Link Title.</summary>
    ///
    /// <value>The WebLinkTitle.</value>
    [CustomDisplayName("WebLinkTitle","FooterSection.resx","Web Link Title"), Column("WebLinkTitle")]
    public string WebLinkTitle
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Web Link cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Web Link.</summary>
    ///
    /// <value>The WebLink.</value>
    [CustomDisplayName("WebLink","FooterSection.resx","Web Link"), Column("WebLink")]
    public string WebLink
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Document Upload.</summary>
    ///
    /// <value>The DocumentUpload.</value>
    [CustomDisplayName("DocumentUpload","FooterSection.resx","Document Upload"), Column("DocumentUpload")]
    public Nullable<long> DocumentUpload
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyInformation.</summary>
    ///
    /// <value>The CompanyInformationFooterSectionAssociation.</value>
    
    
    
    [CustomDisplayName("CompanyInformationFooterSectionAssociationID","FooterSection.resx","CompanyInformation"), Column("CompanyInformationFooterSectionAssociationID")]
    public Nullable<long> CompanyInformationFooterSectionAssociationID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyInformation navigation property.</summary>
    ///
    /// <value>The companyinformationfootersectionassociation object.</value>
    
    public virtual CompanyInformation companyinformationfootersectionassociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Document Display.</summary>
    ///
    /// <value>The AssociatedFooterSectionType.</value>
    
    [CustomValidation(typeof(MandatoryDropdown), "ValidateDropdown")]
    
    [CustomDisplayName("AssociatedFooterSectionTypeID","FooterSection.resx","Document Display"), Column("AssociatedFooterSectionTypeID")]
    public Nullable<long> AssociatedFooterSectionTypeID
    {
        get;
        set;
    }
    
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = (this.Name != null ?Convert.ToString(this.Name)+" " : Convert.ToString(this.Name));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
            return dispValue;
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
            var dispValue = (this.Name != null ?Convert.ToString(this.Name)+" " : Convert.ToString(this.Name));
            return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
        try {  }
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

