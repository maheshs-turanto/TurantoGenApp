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
/// <summary>CompanyInformation model class: Company Information.</summary>
[Table("tbl_CompanyInformation"),CustomDisplayName("CompanyInformation", "CompanyInformation.resx", "CompanyInformation")]
public partial class CompanyInformation : Entity
{
    /// <summary>Default constructor for CompanyInformation.</summary>
    public CompanyInformation()
    {
        if(!this.SSL.HasValue)
            this.SSL = false;
        if(!this.UseAnonymous.HasValue)
            this.UseAnonymous = false;
        this.CompanyInformationCompanyListAssociation_companyinformation = new HashSet<CompanyInformationCompanyListAssociation>();
        this.companyinformationfootersectionassociation = new HashSet<FooterSection>();
    }
    [StringLength(255, ErrorMessage = "Company Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Company Name.</summary>
    ///
    /// <value>The CompanyName.</value>
    [CustomDisplayName("CompanyName","CompanyInformation.resx","Company Name"), Column("CompanyName")] [Required]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyName
    {
        get;
        set;
    }
    [EmailAddress]
    /// <summary>Gets or sets the Company Email.</summary>
    ///
    /// <value>The CompanyEmail.</value>
    [CustomDisplayName("CompanyEmail","CompanyInformation.resx","Company Email"), Column("CompanyEmail")] [Required]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyEmail
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Company Address.</summary>
    ///
    /// <value>The CompanyAddress.</value>
    [CustomDisplayName("CompanyAddress","CompanyInformation.resx","Company Address"), Column("CompanyAddress")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyAddress
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = " Company Country cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the  Company Country.</summary>
    ///
    /// <value>The CompanyCountry.</value>
    [CustomDisplayName("CompanyCountry","CompanyInformation.resx"," Company Country"), Column("CompanyCountry")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyCountry
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Company State cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Company State.</summary>
    ///
    /// <value>The CompanyState.</value>
    [CustomDisplayName("CompanyState","CompanyInformation.resx","Company State"), Column("CompanyState")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyState
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Company City cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Company City.</summary>
    ///
    /// <value>The CompanyCity.</value>
    [CustomDisplayName("CompanyCity","CompanyInformation.resx","Company City"), Column("CompanyCity")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyCity
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Company Zip-Code cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Company Zip-Code.</summary>
    ///
    /// <value>The CompanyZipCode.</value>
    [CustomDisplayName("CompanyZipCode","CompanyInformation.resx","Company Zip-Code"), Column("CompanyZipCode")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string CompanyZipCode
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = " Contact Number 1 cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the  Contact Number 1.</summary>
    ///
    /// <value>The ContactNumber1.</value>
    [CustomDisplayName("ContactNumber1","CompanyInformation.resx"," Contact Number 1"), Column("ContactNumber1")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string ContactNumber1
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = " Contact Number 2 cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the  Contact Number 2.</summary>
    ///
    /// <value>The ContactNumber2.</value>
    [CustomDisplayName("ContactNumber2","CompanyInformation.resx"," Contact Number 2"), Column("ContactNumber2")]
    [PropertyInfoForEntity("98363","Company Information","Group")]
    public string ContactNumber2
    {
        get;
        set;
    }
    /// <summary>Gets or sets the LoginBg.</summary>
    ///
    /// <value>The LoginBg.</value>
    [CustomDisplayName("LoginBg","CompanyInformation.resx","LoginBg"), Column("LoginBg")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public Nullable<long> LoginBg
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Login  Background Width cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Login  Background Width.</summary>
    ///
    /// <value>The LoginBackgroundWidth.</value>
    [CustomDisplayName("LoginBackgroundWidth","CompanyInformation.resx","Login  Background Width"), Column("LoginBackgroundWidth")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string LoginBackgroundWidth
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Login  Background Height cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Login  Background Height.</summary>
    ///
    /// <value>The LoginBackgroundHeight.</value>
    [CustomDisplayName("LoginBackgroundHeight","CompanyInformation.resx","Login  Background Height"), Column("LoginBackgroundHeight")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string LoginBackgroundHeight
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Logo.</summary>
    ///
    /// <value>The Logo.</value>
    [CustomDisplayName("Logo","CompanyInformation.resx","Logo"), Column("Logo")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public Nullable<long> Logo
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Long In Width cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Long In Width.</summary>
    ///
    /// <value>The LogoWidth.</value>
    [CustomDisplayName("LogoWidth","CompanyInformation.resx","Long In Width"), Column("LogoWidth")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string LogoWidth
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Long In Height cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Long In Height.</summary>
    ///
    /// <value>The LogoHeight.</value>
    [CustomDisplayName("LogoHeight","CompanyInformation.resx","Long In Height"), Column("LogoHeight")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string LogoHeight
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Icon.</summary>
    ///
    /// <value>The Icon.</value>
    [CustomDisplayName("Icon","CompanyInformation.resx","Icon"), Column("Icon")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public Nullable<long> Icon
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Header Logo Width cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Header Logo Width.</summary>
    ///
    /// <value>The IconWidth.</value>
    [CustomDisplayName("IconWidth","CompanyInformation.resx","Header Logo Width"), Column("IconWidth")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string IconWidth
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Header Logo Height cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Header Logo Height.</summary>
    ///
    /// <value>The IconHeight.</value>
    [CustomDisplayName("IconHeight","CompanyInformation.resx","Header Logo Height"), Column("IconHeight")]
    [PropertyInfoForEntity("98365","Change Icons","Group")]
    public string IconHeight
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "SMTP User cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the SMTP User.</summary>
    ///
    /// <value>The SMTPUser.</value>
    [CustomDisplayName("SMTPUser","CompanyInformation.resx","SMTP User"), Column("SMTPUser")] [Required]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public string SMTPUser
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "SMTP Server cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the SMTP Server.</summary>
    ///
    /// <value>The SMTPServer.</value>
    [CustomDisplayName("SMTPServer","CompanyInformation.resx","SMTP Server"), Column("SMTPServer")] [Required]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public string SMTPServer
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "SMTP Password cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the SMTP Password.</summary>
    ///
    /// <value>The SMTPPassword.</value>
    [CustomDisplayName("SMTPPassword","CompanyInformation.resx","SMTP Password"), Column("SMTPPassword")] [Required]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public string SMTPPassword
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "SMTP Port cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the SMTP Port.</summary>
    ///
    /// <value>The SMTPPort.</value>
    [CustomDisplayName("SMTPPort","CompanyInformation.resx","SMTP Port"), Column("SMTPPort")] [Required]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public string SMTPPort
    {
        get;
        set;
    }
    /// <summary>Gets or sets the SSL Authentication.</summary>
    ///
    /// <value>The SSL.</value>
    [CustomDisplayName("SSL","CompanyInformation.resx","SSL Authentication"), Column("SSL")]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public Boolean? SSL
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Use Anonymous.</summary>
    ///
    /// <value>The UseAnonymous.</value>
    [CustomDisplayName("UseAnonymous","CompanyInformation.resx","Use Anonymous"), Column("UseAnonymous")]
    [PropertyInfoForEntity("98366","SMTP Details","Group")]
    public Boolean? UseAnonymous
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the About.</summary>
    ///
    /// <value>The AboutCompany.</value>
    [CustomDisplayName("AboutCompany","CompanyInformation.resx","About"), Column("AboutCompany")]
    [PropertyInfoForEntity("98371","About","Group")]
    [System.Web.Mvc.AllowHtml]
    public string AboutCompany
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Disclaimer.</summary>
    ///
    /// <value>The Disclaimer.</value>
    [CustomDisplayName("Disclaimer","CompanyInformation.resx","Disclaimer"), Column("Disclaimer")]
    [PropertyInfoForEntity("98372","Disclaimer","Group")]
    [System.Web.Mvc.AllowHtml]
    public string Disclaimer
    {
        get;
        set;
    }
    
    [CustomDisplayName("OneDriveClientId", "CompanyInformation.resx", "One Drive ClientId"), Column("OneDriveClientId")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDriveClientId
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("OneDriveSecret", "CompanyInformation.resx", "One Drive Secret"), Column("OneDriveSecret")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDriveSecret
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("OneDriveTenantId", "CompanyInformation.resx", "One Drive TenantId"), Column("OneDriveTenantId")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDriveTenantId
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("OneDriveUserName", "CompanyInformation.resx", "One Drive UserName"), Column("OneDriveUserName")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDriveUserName
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("OneDrivePassword", "CompanyInformation.resx", "One Drive Password"), Column("OneDrivePassword")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDrivePassword
    {
        get;
        set;
    }
    
    
    [CustomDisplayName("OneDriveFolderName", "CompanyInformation.resx", "One Drive Folder"), Column("OneDriveFolderName")]
    [PropertyInfoForEntity("98373", "OneDrive", "Group")]
    public string OneDriveFolderName
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyInformationCompanyListAssociation_companyinformation.</summary>
    ///
    /// <value>The ICollection<CompanyInformationCompanyListAssociation>.</value>
    
    public virtual ICollection<CompanyInformationCompanyListAssociation> CompanyInformationCompanyListAssociation_companyinformation
    {
        get;
        set;
    }
    //[NotMapped]
    //[JsonIgnore]
    //public virtual ICollection<CompanyList> CompanyList_CompanyInformationCompanyListAssociation
    //{
    //    get;
    //    set;
    //}
    /// <summary>Gets or sets the SelectedCompanyList_CompanyInformationCompanyListAssociation many to many association.</summary>
    ///
    /// <value>The ICollection<long?>.</value>
    [NotMapped]
    public virtual ICollection<long?> SelectedCompanyList_CompanyInformationCompanyListAssociation
    {
        get;
        set;
    }
    /// <summary>Gets or sets the companyinformationfootersectionassociation.</summary>
    ///
    /// <value>The ICollection<FooterSection>.</value>
    
    [JsonIgnore]
    public virtual ICollection<FooterSection> companyinformationfootersectionassociation
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
            var dispValue = (this.CompanyName != null ?Convert.ToString(this.CompanyName)+" " : Convert.ToString(this.CompanyName));
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
            var dispValue = (this.CompanyName != null ?Convert.ToString(this.CompanyName)+" " : Convert.ToString(this.CompanyName));
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

