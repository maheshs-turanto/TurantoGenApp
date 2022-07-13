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
/// <summary>A company profile.</summary>
public class CompanyProfile
{
    /// <summary>Default constructor.</summary>
    public CompanyProfile()
    {
        this.TenantId = 0;
        this.Type = "Global";
        this.Name = "Turanto";
        this.Email = "Contact@turanto.com";
        this.Address = "";
        this.Country = "USA";
        this.State = "VA";
        this.City = "";
        this.Zip = "";
        this.ContactNumber1 = "";
        this.ContactNumber2 = "";
        this.SMTPServer = "";
        this.SMTPPassword = "";
        this.SMTPPort = 786;
        this.SSL = false;
        //master page icon
        this.Icon = "logo.gif";
        this.IconHeight = "28px";
        this.IconWidth = "28px";
        //
        //login logo page
        this.Logo = "logo_white.png";
        this.LogoWidth = "155px";
        this.LogoHeight = "29px";
        //
        this.LoginBg = "Loginbg.png";
        this.AboutCompany = "About Company";
        //LegalInformation
        this.LegalInformation = "Legal Information";
        this.LegalInformationLink = "/PolicyAndService/Licensing.pdf";
        this.LegalInformationAttach = "Licensing.pdf";
        this.LegalInformationDisplay = "1";
        //end
        //PrivacyPolicy
        this.PrivacyPolicy = "Privacy Policy";
        this.PrivacyPolicyLink = "/PolicyAndService/PrivacyPolicy.pdf";
        this.PrivacyPolicyAttach = "PrivacyPolicy.pdf";
        this.PrivacyPolicyDisplay = "1";
        //End
        //Terms Of Service
        this.TermsOfService = "Terms Of Service";
        this.TermsOfServiceLink = "/PolicyAndService/Terms_Of_Service.pdf";
        this.TermsOfServiceAttach = "Terms_Of_Service.pdf";
        this.TermsOfServiceDisplay = "1";
        //End
        //Third-Party Licenses
        this.ThirdParty = "Third-Party Licenses";
        this.ThirdPartyLink = "/PolicyAndService/Third_Party_Licenses.pdf";
        this.ThirdPartyAttach = "Third_Party_Licenses.pdf";
        this.ThirdPartyDisplay = "1";
        //End
        //Cookie Information
        this.CookieInformation = "Cookie Policy";
        this.CookieInformationLink = "/PolicyAndService/CookiePolicy.pdf";
        this.CookieInformationAttach = "CookiePolicy.pdf";
        this.CookieInformationDisplay = "1";
        //end
        //Emailto
        this.Emailto = "Email To";
        this.EmailtoAddress = "contact@turanto.com";
        //End
        //Create By
        this.CreatedBy = "Created With";
        this.CreatedByName = "Turanto";
        this.CreatedByLink = "http://www.turanto.com/";
        this.SMTPUser = "";
        this.UseAnonymous = false;
        this.Disclaimer = "Disclaimer : This computer system is the property of Etelic and is intended for authorized users only.";
    }
    /// <summary>.</summary>
    public CompanyProfile(long? tenantid, string type, string name, string email, string address, string country, string state, string city,
                          string zip, string contact1, string contact2, string smtpserver,
                          string smtppassword, int smtpport, bool ssl,
                          string aboutcompany,
                          string _Iconwidth, string _Iconheight,
                          string logowidth, string logoheight,
                          string loginbg,
                          string _Icon, string logo,
                          string legalInformation,
                          string legalInformationLink,
                          string legalInformationAttach,
                          string legalInformationDisplay,
                          string privacyPolicy,
                          string privacyPolicyLink,
                          string privacyPolicyAttach,
                          string privacyPolicyDisplay,
                          string termsOfService,
                          string termsOfServiceLink,
                          string termsOfServiceAttach,
                          string termsOfServiceDisplay,
                          string thirdparty,
                          string thirdpartyLink,
                          string thirdpartyAttach,
                          string thirdpartyDisplay,
                          string cookieInformation,
                          string cookieInformationLink,
                          string cookieInformationAttach,
                          string cookieInformationDisplay,
                          string emailto,
                          string emailtoAddress,
                          string createdBy,
                          string createdByName,
                          string createdByLink,
                          string disclaimer, string smtpuser, bool useAnonymous, string OneDriveClientId, string OneDriveSecret, string OneDriveTenantId, string OneDriveUserName, string OneDrivePassword, string OneDriveFolderName)
    {
        this.TenantId = tenantid;
        this.Type = type;
        this.Name = name;
        this.Email = email;
        this.Address = address;
        this.Country = country;
        this.State = state;
        this.City = city;
        this.Zip = zip;
        this.ContactNumber1 = contact1;
        this.ContactNumber2 = contact2;
        this.SMTPServer = smtpserver;
        this.SMTPPassword = smtppassword;
        this.SMTPPort = smtpport;
        this.SSL = ssl;
        this.SMTPUser = smtpuser;
        this.UseAnonymous = useAnonymous;
        //master page icon
        this.Icon = _Icon;
        this.IconWidth = _Iconwidth;
        this.IconHeight = _Iconheight;
        //login page logo
        this.Logo = logo;
        this.LogoWidth = logowidth;
        this.LogoHeight = logoheight;
        //
        this.LoginBg = loginbg;
        this.AboutCompany = aboutcompany;
        //Legal Information
        this.LegalInformation = legalInformation;
        this.LegalInformationLink = legalInformationLink;
        this.LegalInformationAttach = legalInformationAttach;
        this.LegalInformationDisplay = legalInformationDisplay;
        //end
        //PrivacyPolicy
        this.PrivacyPolicy = privacyPolicy;
        this.PrivacyPolicyLink = privacyPolicyLink;
        this.PrivacyPolicyAttach = privacyPolicyAttach;
        this.PrivacyPolicyDisplay = privacyPolicyDisplay;
        //End
        //Terms Of Service
        this.TermsOfService = termsOfService;
        this.TermsOfServiceLink = termsOfServiceLink;
        this.TermsOfServiceAttach = termsOfServiceAttach;
        this.TermsOfServiceDisplay = termsOfServiceDisplay;
        //End
        //Third-Party Licenses
        this.ThirdParty = thirdparty;
        this.ThirdPartyLink = thirdpartyLink;
        this.ThirdPartyAttach = thirdpartyAttach;
        this.ThirdPartyDisplay = thirdpartyDisplay;
        //End
        //Legal Information
        this.CookieInformation = cookieInformation;
        this.CookieInformationLink = cookieInformationLink;
        this.CookieInformationAttach = cookieInformationAttach;
        this.CookieInformationDisplay = cookieInformationDisplay;
        //end
        //Emailto
        this.Emailto = emailto;
        this.EmailtoAddress = emailtoAddress;
        //End
        //Create By
        this.CreatedBy = createdBy;
        this.CreatedByLink = createdByLink;
        this.CreatedByName = createdByName;
        this.Disclaimer = disclaimer;
        this.OneDriveClientId = OneDriveClientId;
        this.OneDriveSecret = OneDriveSecret;
        this.OneDriveTenantId = OneDriveTenantId;
        this.OneDriveUserName = OneDriveUserName;
        this.OneDrivePassword = OneDrivePassword;
        this.OneDriveFolderName = OneDriveFolderName;
    }
    
    /// <summary>Gets or sets the identifier of the tenant.</summary>
    ///
    /// <value>The identifier of the tenant.</value>
    
    public long? TenantId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type.</summary>
    ///
    /// <value>The type.</value>
    
    public string Type
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Company Name")]
    [Required]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the email.</summary>
    ///
    /// <value>The email.</value>
    
    [DisplayName("Company Email")]
    [Required]
    public string Email
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the address.</summary>
    ///
    /// <value>The address.</value>
    
    [DisplayName("Company Address")]
    public string Address
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the country.</summary>
    ///
    /// <value>The country.</value>
    
    [DisplayName("Company Country")]
    public string Country
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the state.</summary>
    ///
    /// <value>The state.</value>
    
    [DisplayName("Company State")]
    public string State
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the city.</summary>
    ///
    /// <value>The city.</value>
    
    [DisplayName("Company City")]
    public string City
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the zip.</summary>
    ///
    /// <value>The zip.</value>
    
    [DisplayName("Company Zip-Code")]
    public string Zip
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the contact number 1.</summary>
    ///
    /// <value>The contact number 1.</value>
    
    [DisplayName("Contact Number 1")]
    public string ContactNumber1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the contact number 2.</summary>
    ///
    /// <value>The contact number 2.</value>
    
    [DisplayName("Contact Number 2")]
    public string ContactNumber2
    {
        get;
        set;
    }
    
    /// <summary>Icon Master Page.</summary>
    ///
    /// <value>The icon.</value>
    
    [DisplayName("Company Icon")]
    public string Icon
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the width of the icon.</summary>
    ///
    /// <value>The width of the icon.</value>
    
    [DisplayName("Icon Width")]
    public string IconWidth
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the height of the icon.</summary>
    ///
    /// <value>The height of the icon.</value>
    
    [DisplayName("Icon Height")]
    public string IconHeight
    {
        get;
        set;
    }
    
    /// <summary>login page logo.</summary>
    ///
    /// <value>The logo.</value>
    
    [DisplayName("Company Logo")]
    public string Logo
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the width of the logo.</summary>
    ///
    /// <value>The width of the logo.</value>
    
    [DisplayName("Logo Width")]
    public string LogoWidth
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the height of the logo.</summary>
    ///
    /// <value>The height of the logo.</value>
    
    [DisplayName("Logo Height")]
    public string LogoHeight
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the login background.</summary>
    ///
    /// <value>The login background.</value>
    
    [DisplayName("Login Background")]
    public string LoginBg
    {
        get;
        set;
    }
    
    /// <summary>SMTP SERVER DETAILS.</summary>
    ///
    /// <value>The SMTP server.</value>
    
    [DisplayName("SMTP Server")]
    [Required]
    public string SMTPServer
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the SMTP password.</summary>
    ///
    /// <value>The SMTP password.</value>
    
    [DisplayName("SMTP Password")]
    [Required]
    public string SMTPPassword
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the SMTP port.</summary>
    ///
    /// <value>The SMTP port.</value>
    
    [DisplayName("SMTP Port")]
    [Required]
    public int SMTPPort
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the ssl.</summary>
    ///
    /// <value>The ssl.</value>
    
    [DisplayName("SSL Authentication")]
    public bool? SSL
    {
        get;
        set;
    }
    
    /// <summary>About Company.</summary>
    ///
    /// <value>The about company.</value>
    
    [DisplayName("About Company")]
    [System.Web.Mvc.AllowHtml]
    public string AboutCompany
    {
        get;
        set;
    }
    
    /// <summary>Legal Information.</summary>
    ///
    /// <value>Information describing the legal.</value>
    
    [DisplayName("Legal Information")]
    public string LegalInformation
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the legal information link.</summary>
    ///
    /// <value>The legal information link.</value>
    
    [DisplayName("Legal Information Link")]
    public string LegalInformationLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the legal information attach.</summary>
    ///
    /// <value>The legal information attach.</value>
    
    [DisplayName("Legal Information Attach")]
    public string LegalInformationAttach
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the legal information display.</summary>
    ///
    /// <value>The legal information display.</value>
    
    [DisplayName("Legal Information Display")]
    public string LegalInformationDisplay
    {
        get;
        set;
    }
    
    /// <summary>end Cookie Information.</summary>
    ///
    /// <value>Information describing the cookie.</value>
    
    [DisplayName("Cookie Information")]
    public string CookieInformation
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the cookie information link.</summary>
    ///
    /// <value>The cookie information link.</value>
    
    [DisplayName("Cookie Information Link")]
    public string CookieInformationLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the cookie information attach.</summary>
    ///
    /// <value>The cookie information attach.</value>
    
    [DisplayName("Cookie Information Attach")]
    public string CookieInformationAttach
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the cookie information display.</summary>
    ///
    /// <value>The cookie information display.</value>
    
    [DisplayName("Cookie Information Display")]
    public string CookieInformationDisplay
    {
        get;
        set;
    }
    
    /// <summary>end PrivacyPolicy.</summary>
    ///
    /// <value>The privacy policy.</value>
    
    [DisplayName("Privacy Policy")]
    public string PrivacyPolicy
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the privacy policy link.</summary>
    ///
    /// <value>The privacy policy link.</value>
    
    [DisplayName("Privacy Policy Link")]
    public string PrivacyPolicyLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the privacy policy attach.</summary>
    ///
    /// <value>The privacy policy attach.</value>
    
    [DisplayName("Privacy Policy Attach")]
    public string PrivacyPolicyAttach
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the privacy policy display.</summary>
    ///
    /// <value>The privacy policy display.</value>
    
    [DisplayName("Privacy Policy Display")]
    public string PrivacyPolicyDisplay
    {
        get;
        set;
    }
    
    /// <summary>End Terms Of Service.</summary>
    ///
    /// <value>The terms of service.</value>
    
    [DisplayName("Terms Of Service")]
    public string TermsOfService
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the terms of service link.</summary>
    ///
    /// <value>The terms of service link.</value>
    
    [DisplayName("Terms Of Service Link")]
    public string TermsOfServiceLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the terms of service attach.</summary>
    ///
    /// <value>The terms of service attach.</value>
    
    [DisplayName("Terms Of Service Attach")]
    public string TermsOfServiceAttach
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the terms of service display.</summary>
    ///
    /// <value>The terms of service display.</value>
    
    [DisplayName("Terms Of Service Display")]
    public string TermsOfServiceDisplay
    {
        get;
        set;
    }
    
    /// <summary>End Terms Of Service.</summary>
    ///
    /// <value>The third party.</value>
    
    [DisplayName("Third-Party Licenses")]
    public string ThirdParty
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the third party link.</summary>
    ///
    /// <value>The third party link.</value>
    
    [DisplayName("Third-Party Licenses Link")]
    public string ThirdPartyLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the third party attach.</summary>
    ///
    /// <value>The third party attach.</value>
    
    [DisplayName("Third-Party Licenses Attach")]
    public string ThirdPartyAttach
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the third party display.</summary>
    ///
    /// <value>The third party display.</value>
    
    [DisplayName("Third-Party Licenses Display")]
    public string ThirdPartyDisplay
    {
        get;
        set;
    }
    
    /// <summary>End Emailto.</summary>
    ///
    /// <value>The emailto.</value>
    
    [DisplayName("Email to")]
    public string Emailto
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the emailto address.</summary>
    ///
    /// <value>The emailto address.</value>
    
    [DisplayName("Email to Address")]
    public string EmailtoAddress
    {
        get;
        set;
    }
    
    /// <summary>End Create By.</summary>
    ///
    /// <value>Describes who created this object.</value>
    
    [DisplayName("Created With")]
    public string CreatedBy
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the created by link.</summary>
    ///
    /// <value>The created by link.</value>
    
    [DisplayName("Created By Link")]
    public string CreatedByLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name of the created by.</summary>
    ///
    /// <value>The name of the created by.</value>
    
    [DisplayName("Created By Name")]
    public string CreatedByName
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the disclaimer.</summary>
    ///
    /// <value>The disclaimer.</value>
    
    [DisplayName("Disclaimer")]
    [System.Web.Mvc.AllowHtml]
    public string Disclaimer
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the SMTP user.</summary>
    ///
    /// <value>The SMTP user.</value>
    
    [DisplayName("SMTP User")]
    public string SMTPUser
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object use anonymous.</summary>
    ///
    /// <value>True if use anonymous, false if not.</value>
    
    [DisplayName("Use Anonymous")]
    public bool UseAnonymous
    {
        get;
        set;
    }
    [DisplayName("OneDriveClientId")]
    public string OneDriveClientId
    {
        get;
        set;
    }
    [DisplayName("OneDriveSecret")]
    public string OneDriveSecret
    {
        get;
        set;
    }
    [DisplayName("OneDriveTenantId")]
    public string OneDriveTenantId
    {
        get;
        set;
    }
    [DisplayName("OneDriveUserName")]
    public string OneDriveUserName
    {
        get;
        set;
    }
    [DisplayName("OneDrivePassword")]
    public string OneDrivePassword
    {
        get;
        set;
    }
    [DisplayName("OneDriveFolderName")]
    public string OneDriveFolderName
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

/// <summary>Interface for company profile repository.</summary>
public interface ICompanyProfileRepository
{
    /// <summary>Edit company profile.</summary>
    ///
    /// <param name="cp">The cp.</param>
    
    void EditCompanyProfile(CompanyProfile cp);
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="user">The user.</param>
    ///
    /// <returns>The company profile.</returns>
    
    CompanyProfile GetCompanyProfile(IUser user);
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The company profile.</returns>
    
    CompanyProfile GetCompanyProfile(IUser user, long? tenantId);
    
    /// <summary>Sets view bag.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A System.Web.Mvc.SelectList.</returns>
    
    System.Web.Mvc.SelectList SetViewBag(IUser user, long? tenantId);
    
    /// <summary>Sets view bag email.</summary>
    ///
    /// <param name="user">The user.</param>
    ///
    /// <returns>A System.Web.Mvc.SelectList.</returns>
    
    System.Web.Mvc.SelectList SetViewBagEmail(IUser user);
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="userID">Identifier for the user.</param>
    ///
    /// <returns>The company profile.</returns>
    
    CompanyProfile GetCompanyProfile(string userID);
    
    /// <summary>Gets email users by tenant.</summary>
    ///
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The email users by tenant.</returns>
    
    List<string> getEmailUsersByTenant(long tenantId);
}
}