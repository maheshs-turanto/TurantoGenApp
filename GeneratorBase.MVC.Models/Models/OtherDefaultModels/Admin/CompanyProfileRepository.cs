using GeneratorBase.MVC;
using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
/// <summary>A company profile repository.</summary>
public class CompanyProfileRepository : ICompanyProfileRepository, IDisposable
{
    /// <summary>Information describing the company.</summary>
    private XDocument companyData;
    /// <summary>constructor.</summary>
    public CompanyProfileRepository()
    {
        companyData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
    }
    
    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.</summary>
    
    public void Dispose()
    {
        companyData = null;
    }
    
    /// <summary>Encryptdata.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>A string.</returns>
    
    private string Encryptdata(string password)
    {
        string strmsg = string.Empty;
        byte[] encode = new byte[password.Length];
        encode = Encoding.UTF8.GetBytes(password);
        strmsg = Convert.ToBase64String(encode);
        return strmsg;
    }
    
    /// <summary>Decryptdata.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>A string.</returns>
    
    private string Decryptdata(string password)
    {
        string decryptpwd = string.Empty;
        UTF8Encoding encodepwd = new UTF8Encoding();
        Decoder Decode = encodepwd.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(password);
        int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        decryptpwd = new String(decoded_char);
        return decryptpwd;
    }
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="user">The user.</param>
    ///
    /// <returns>The company profile.</returns>
    
    public CompanyProfile GetCompanyProfile(IUser user)
    {
        long tenantid = 0;
        IEnumerable<CompanyProfile> companyprofile = from cp in companyData.Descendants("item")
                select new CompanyProfile((long?)cp.Element("TenantId"), (string)cp.Element("Type"), (string)cp.Element("Name"), (string)cp.Element("Email").Value,
                                          (string)cp.Element("Address").Value,
                                          (string)cp.Element("Country"),
                                          (string)cp.Element("State").Value,
                                          (string)cp.Element("City"),
                                          (string)cp.Element("Zip"),
                                          (string)cp.Element("ContactNumber1"),
                                          (string)cp.Element("ContactNumber2"),
                                          (string)cp.Element("SMTPServer"),
                                          Decryptdata((string)cp.Element("SMTPPassword")),
                                          (int)cp.Element("SMTPPort"),
                                          (bool)cp.Element("SSL"),
                                          (string)cp.Element("AboutCompany"),
                                          (string)cp.Element("IconWidth"),
                                          (string)cp.Element("IconHeight"),
                                          (string)cp.Element("LogoWidth"),
                                          (string)cp.Element("LogoHeight"),
                                          (string)cp.Element("LoginBg"),
                                          (string)cp.Element("Icon"),
                                          (string)cp.Element("Logo"),
                                          (string)cp.Element("LegalInformation"),
                                          (string)cp.Element("LegalInformationLink"),
                                          (string)cp.Element("LegalInformationAttach"),
                                          (string)cp.Element("LegalInformationDisplay"),
                                          (string)cp.Element("PrivacyPolicy"),
                                          (string)cp.Element("PrivacyPolicyLink"),
                                          (string)cp.Element("PrivacyPolicyAttach"),
                                          (string)cp.Element("PrivacyPolicyDisplay"),
                                          (string)cp.Element("TermsOfService"),
                                          (string)cp.Element("TermsOfServiceLink"),
                                          (string)cp.Element("TermsOfServiceAttach"),
                                          (string)cp.Element("TermsOfServiceDisplay"),
                                          (string)cp.Element("ThirdParty"),
                                          (string)cp.Element("ThirdPartyLink"),
                                          (string)cp.Element("ThirdPartyAttach"),
                                          (string)cp.Element("ThirdPartyDisplay"),
                                          (string)cp.Element("CookieInformation"),
                                          (string)cp.Element("CookieInformationLink"),
                                          (string)cp.Element("CookieInformationAttach"),
                                          (string)cp.Element("CookieInformationDisplay"),
                                          (string)cp.Element("Emailto"),
                                          (string)cp.Element("EmailtoAddress"),
                                          (string)cp.Element("CreatedBy"),
                                          (string)cp.Element("CreatedByLink"),
                                          (string)cp.Element("CreatedByName"),
                                          (string)cp.Element("Disclaimer"),
                                          cp.Element("SMTPUser") == null ? (string)cp.Element("Email").Value : (string)cp.Element("SMTPUser"),
                                          cp.Element("UseAnonymous") == null ? false : (bool)cp.Element("UseAnonymous"),
                                          (string)cp.Element("OneDriveClientId"),
                                          (string)cp.Element("OneDriveSecret"),
                                          (string)cp.Element("OneDriveTenantId"),
                                          (string)cp.Element("OneDriveUserName"),
                                          (string)cp.Element("OneDrivePassword"),
                                          (string)cp.Element("OneDriveFolderName"));
        if(tenantid > 0)
        {
            var cProfile = companyprofile.FirstOrDefault(p => p.TenantId.HasValue && p.TenantId.Value == tenantid);
            if(cProfile != null)
                return cProfile;
            else
            {
                var firstObj = companyprofile.FirstOrDefault();
                firstObj.TenantId = tenantid;
                firstObj.Type = "Tenant";
                return firstObj;
            }
        }
        else return companyprofile.ToList()[0];
    }
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="UserId">Identifier for the user.</param>
    ///
    /// <returns>The company profile.</returns>
    
    public CompanyProfile GetCompanyProfile(string UserId)
    {
        if(string.IsNullOrEmpty(UserId))
            return GetCompanyProfile(new SystemUser());
        return GetCompanyProfile(new SystemUser());
    }
    
    /// <summary>Gets company profile.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The company profile.</returns>
    
    public CompanyProfile GetCompanyProfile(IUser user, long? tenantId)
    {
        if(!tenantId.HasValue) return GetCompanyProfile(user);
        long tenantid = tenantId.HasValue ? tenantId.Value : 0;
        IEnumerable<CompanyProfile> companyprofile = from cp in companyData.Descendants("item")
                select new CompanyProfile((long?)cp.Element("TenantId"), (string)cp.Element("Type"), (string)cp.Element("Name"), (string)cp.Element("Email").Value,
                                          (string)cp.Element("Address").Value,
                                          (string)cp.Element("Country"),
                                          (string)cp.Element("State").Value,
                                          (string)cp.Element("City"),
                                          (string)cp.Element("Zip"),
                                          (string)cp.Element("ContactNumber1"),
                                          (string)cp.Element("ContactNumber2"),
                                          (string)cp.Element("SMTPServer"),
                                          Decryptdata((string)cp.Element("SMTPPassword")),
                                          (int)cp.Element("SMTPPort"),
                                          (bool)cp.Element("SSL"),
                                          (string)cp.Element("AboutCompany"),
                                          (string)cp.Element("IconWidth"),
                                          (string)cp.Element("IconHeight"),
                                          (string)cp.Element("LogoWidth"),
                                          (string)cp.Element("LogoHeight"),
                                          (string)cp.Element("LoginBg"),
                                          (string)cp.Element("Icon"),
                                          (string)cp.Element("Logo"),
                                          (string)cp.Element("LegalInformation"),
                                          (string)cp.Element("LegalInformationLink"),
                                          (string)cp.Element("LegalInformationAttach"),
                                          (string)cp.Element("LegalInformationDisplay"),
                                          (string)cp.Element("PrivacyPolicy"),
                                          (string)cp.Element("PrivacyPolicyLink"),
                                          (string)cp.Element("PrivacyPolicyAttach"),
                                          (string)cp.Element("PrivacyPolicyDisplay"),
                                          (string)cp.Element("TermsOfService"),
                                          (string)cp.Element("TermsOfServiceLink"),
                                          (string)cp.Element("TermsOfServiceAttach"),
                                          (string)cp.Element("TermsOfServiceDisplay"),
                                          (string)cp.Element("ThirdParty"),
                                          (string)cp.Element("ThirdPartyLink"),
                                          (string)cp.Element("ThirdPartyAttach"),
                                          (string)cp.Element("ThirdPartyDisplay"),
                                          (string)cp.Element("CookieInformation"),
                                          (string)cp.Element("CookieInformationLink"),
                                          (string)cp.Element("CookieInformationAttach"),
                                          (string)cp.Element("CookieInformationDisplay"),
                                          (string)cp.Element("Emailto"),
                                          (string)cp.Element("EmailtoAddress"),
                                          (string)cp.Element("CreatedBy"),
                                          (string)cp.Element("CreatedByLink"),
                                          (string)cp.Element("CreatedByName"),
                                          (string)cp.Element("Disclaimer"),
                                          (cp.Element("SMTPUser") == null ? (string)cp.Element("Email").Value : (string)cp.Element("SMTPUser")),
                                          (cp.Element("UseAnonymous") == null ? false : (bool)cp.Element("UseAnonymous")),
                                          (string)cp.Element("OneDriveClientId"),
                                          (string)cp.Element("OneDriveSecret"),
                                          (string)cp.Element("OneDriveTenantId"),
                                          (string)cp.Element("OneDriveUserName"),
                                          (string)cp.Element("OneDrivePassword"),
                                          (string)cp.Element("OneDriveFolderName"));
        if(tenantid > 0)
        {
            var cProfile = companyprofile.FirstOrDefault(p => p.TenantId.HasValue && p.TenantId.Value == tenantid);
            if(cProfile != null)
                return cProfile;
            else
            {
                var firstObj = companyprofile.FirstOrDefault();
                firstObj.TenantId = tenantid;
                firstObj.Type = "Tenant";
                return firstObj;
            }
        }
        else return companyprofile.ToList()[0];
    }
    
    /// <summary>Edit company profile.</summary>
    ///
    /// <param name="cp">The cp.</param>
    
    public void EditCompanyProfile(CompanyProfile cp)
    {
        XElement node = companyData.Root.Elements("item").FirstOrDefault(p => (long?)p.Element("TenantId") == cp.TenantId && (string)p.Element("Type") == cp.Type);
        if(cp.TenantId.HasValue && cp.TenantId.Value > 0 && node == null)
        {
            node = companyData.Root.Elements("item").FirstOrDefault();
            XElement nodechild = new XElement(node);
            nodechild.Elements("Name").FirstOrDefault().AddBeforeSelf(new XElement("TenantId", cp.TenantId));
            nodechild.Elements("Name").FirstOrDefault().AddBeforeSelf(new XElement("Type", cp.Type));
            nodechild.SetElementValue("Name", string.IsNullOrEmpty(cp.Name) ? "" : cp.Name);
            nodechild.SetElementValue("Email", string.IsNullOrEmpty(cp.Email) ? "" : cp.Email);
            nodechild.SetElementValue("Address", string.IsNullOrEmpty(cp.Address) ? "" : cp.Address);
            nodechild.SetElementValue("Country", string.IsNullOrEmpty(cp.Country) ? "" : cp.Country);
            nodechild.SetElementValue("State", string.IsNullOrEmpty(cp.State) ? "" : cp.State);
            nodechild.SetElementValue("City", string.IsNullOrEmpty(cp.City) ? "" : cp.City);
            nodechild.SetElementValue("Zip", string.IsNullOrEmpty(cp.Zip) ? "" : cp.Zip);
            nodechild.SetElementValue("ContactNumber1", string.IsNullOrEmpty(cp.ContactNumber1) ? "" : cp.ContactNumber1);
            nodechild.SetElementValue("ContactNumber2", string.IsNullOrEmpty(cp.ContactNumber2) ? "" : cp.ContactNumber2);
            nodechild.SetElementValue("SMTPServer", string.IsNullOrEmpty(cp.SMTPServer) ? "" : cp.SMTPServer);
            nodechild.SetElementValue("SMTPPassword", string.IsNullOrEmpty(cp.SMTPPassword) ? "" : Encryptdata(cp.SMTPPassword));
            nodechild.SetElementValue("SMTPPort", string.IsNullOrEmpty(Convert.ToString(cp.SMTPPort)) ? "" : Convert.ToString(cp.SMTPPort));
            nodechild.SetElementValue("SSL", string.IsNullOrEmpty(Convert.ToString(cp.SSL)) ? "" : Convert.ToString(cp.SSL));
            //master page icon
            nodechild.SetElementValue("IconWidth", string.IsNullOrEmpty(cp.IconWidth) ? "28px" : cp.IconWidth);
            nodechild.SetElementValue("IconHeight", string.IsNullOrEmpty(cp.IconHeight) ? "28px" : cp.IconHeight);
            nodechild.SetElementValue("Icon", string.IsNullOrEmpty(cp.Icon) ? "logo.gif" : cp.Icon);
            //login logo page
            nodechild.SetElementValue("LogoWidth", string.IsNullOrEmpty(cp.LogoWidth) ? "155px" : cp.LogoWidth);
            nodechild.SetElementValue("LogoHeight", string.IsNullOrEmpty(cp.LogoHeight) ? "29px" : cp.LogoHeight);
            nodechild.SetElementValue("Logo", string.IsNullOrEmpty(cp.Logo) ? "logo_white.png" : cp.Logo);
            nodechild.SetElementValue("LoginBg", string.IsNullOrEmpty(cp.LoginBg) ? "Loginbg.png" : cp.LoginBg);
            //
            nodechild.SetElementValue("AboutCompany", string.IsNullOrEmpty(cp.AboutCompany) ? "" : cp.AboutCompany);
            //Legal Information
            nodechild.SetElementValue("LegalInformation", string.IsNullOrEmpty(cp.LegalInformation) ? "Legal Information" : cp.LegalInformation);
            nodechild.SetElementValue("LegalInformationLink", string.IsNullOrEmpty(cp.LegalInformationLink) ? "/PolicyAndService/Licensing.pdf" : cp.LegalInformationLink);
            nodechild.SetElementValue("LegalInformationAttach", string.IsNullOrEmpty(cp.LegalInformationAttach) ? "Licensing.pdf" : cp.LegalInformationAttach);
            nodechild.SetElementValue("LegalInformationDisplay", string.IsNullOrEmpty(cp.LegalInformationDisplay) ? "1" : cp.LegalInformationDisplay);
            nodechild.SetElementValue("PrivacyPolicy", string.IsNullOrEmpty(cp.PrivacyPolicy) ? "Privacy Policy" : cp.PrivacyPolicy);
            nodechild.SetElementValue("PrivacyPolicyLink", string.IsNullOrEmpty(cp.PrivacyPolicyLink) ? "/PolicyAndService/PrivacyPolicy.pdf" : cp.PrivacyPolicyLink);
            nodechild.SetElementValue("PrivacyPolicyAttach", string.IsNullOrEmpty(cp.PrivacyPolicyAttach) ? "PrivacyPolicy.pdf" : cp.PrivacyPolicyAttach);
            nodechild.SetElementValue("PrivacyPolicyDisplay", string.IsNullOrEmpty(cp.PrivacyPolicyDisplay) ? "1" : cp.PrivacyPolicyDisplay);
            nodechild.SetElementValue("TermsOfService", string.IsNullOrEmpty(cp.TermsOfService) ? "Terms Of Service" : cp.TermsOfService);
            nodechild.SetElementValue("TermsOfServiceLink", string.IsNullOrEmpty(cp.TermsOfServiceLink) ? "/PolicyAndService/Terms_Of_Service.pdf" : cp.TermsOfServiceLink);
            nodechild.SetElementValue("TermsOfServiceAttach", string.IsNullOrEmpty(cp.TermsOfServiceAttach) ? "Terms_Of_Service.pdf" : cp.TermsOfServiceAttach);
            nodechild.SetElementValue("TermsOfServiceDisplay", string.IsNullOrEmpty(cp.TermsOfServiceDisplay) ? "1" : cp.TermsOfServiceDisplay);
            nodechild.SetElementValue("ThirdParty", string.IsNullOrEmpty(cp.ThirdParty) ? "Third-Party Licenses" : cp.ThirdParty);
            nodechild.SetElementValue("ThirdPartyLink", string.IsNullOrEmpty(cp.ThirdPartyLink) ? "/PolicyAndService/Third_Party_Licenses.pdf" : cp.ThirdPartyLink);
            nodechild.SetElementValue("ThirdPartyAttach", string.IsNullOrEmpty(cp.ThirdPartyAttach) ? "Third_Party_Licenses.pdf" : cp.ThirdPartyAttach);
            nodechild.SetElementValue("ThirdPartyDisplay", string.IsNullOrEmpty(cp.ThirdPartyDisplay) ? "1" : cp.ThirdPartyDisplay);
            //Cookie Information
            nodechild.SetElementValue("CookieInformation", string.IsNullOrEmpty(cp.CookieInformation) ? "Cookie Information" : cp.CookieInformation);
            nodechild.SetElementValue("CookieInformationLink", string.IsNullOrEmpty(cp.CookieInformationLink) ? "/PolicyAndService/CookiePolicy.pdf" : cp.CookieInformationLink);
            nodechild.SetElementValue("CookieInformationAttach", string.IsNullOrEmpty(cp.CookieInformationAttach) ? "CookiePolicy.pdf" : cp.CookieInformationAttach);
            nodechild.SetElementValue("CookieInformationDisplay", string.IsNullOrEmpty(cp.CookieInformationDisplay) ? "1" : cp.CookieInformationDisplay);
            nodechild.SetElementValue("Emailto", string.IsNullOrEmpty(cp.Emailto) ? "Email To" : cp.Emailto);
            nodechild.SetElementValue("EmailtoAddress", string.IsNullOrEmpty(cp.EmailtoAddress) ? "contact@turanto.com" : cp.EmailtoAddress);
            nodechild.SetElementValue("CreatedBy", string.IsNullOrEmpty(cp.CreatedBy) ? "Created By" : cp.CreatedBy);
            nodechild.SetElementValue("CreatedByLink", string.IsNullOrEmpty(cp.CreatedByLink) ? "http://www.turanto.com/" : cp.CreatedByLink);
            nodechild.SetElementValue("CreatedByName", string.IsNullOrEmpty(cp.CreatedByName) ? "Turanto" : cp.CreatedByName);
            //
            nodechild.SetElementValue("Disclaimer", string.IsNullOrEmpty(cp.Disclaimer) ? "" : cp.Disclaimer);
            nodechild.SetElementValue("SMTPUser", string.IsNullOrEmpty(Convert.ToString(cp.SMTPUser)) ? cp.Email : Convert.ToString(cp.SMTPUser));
            nodechild.SetElementValue("UseAnonymous", string.IsNullOrEmpty(Convert.ToString(cp.UseAnonymous)) ? "false" : Convert.ToString(cp.UseAnonymous));
            companyData.Root.Add(nodechild);
            companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
            return;
        }
        else if(node == null)
        {
            node = companyData.Root.Elements("item").FirstOrDefault();
        }
        //node.SetElementValue("TenantId", cp.TenantId);
        //node.SetElementValue("Type", string.IsNullOrEmpty(cp.Type) ? "" : cp.Type);
        node.SetElementValue("Name", string.IsNullOrEmpty(cp.Name) ? "" : cp.Name);
        node.SetElementValue("Email", string.IsNullOrEmpty(cp.Email) ? "" : cp.Email);
        node.SetElementValue("Address", string.IsNullOrEmpty(cp.Address) ? "" : cp.Address);
        node.SetElementValue("Country", string.IsNullOrEmpty(cp.Country) ? "" : cp.Country);
        node.SetElementValue("State", string.IsNullOrEmpty(cp.State) ? "" : cp.State);
        node.SetElementValue("City", string.IsNullOrEmpty(cp.City) ? "" : cp.City);
        node.SetElementValue("Zip", string.IsNullOrEmpty(cp.Zip) ? "" : cp.Zip);
        node.SetElementValue("ContactNumber1", string.IsNullOrEmpty(cp.ContactNumber1) ? "" : cp.ContactNumber1);
        node.SetElementValue("ContactNumber2", string.IsNullOrEmpty(cp.ContactNumber2) ? "" : cp.ContactNumber2);
        node.SetElementValue("SMTPServer", string.IsNullOrEmpty(cp.SMTPServer) ? "" : cp.SMTPServer);
        node.SetElementValue("SMTPPassword", string.IsNullOrEmpty(cp.SMTPPassword) ? "" : Encryptdata(cp.SMTPPassword));
        node.SetElementValue("SMTPPort", string.IsNullOrEmpty(Convert.ToString(cp.SMTPPort)) ? "" : Convert.ToString(cp.SMTPPort));
        node.SetElementValue("SSL", string.IsNullOrEmpty(Convert.ToString(cp.SSL)) ? "" : Convert.ToString(cp.SSL));
        //master page icon
        node.SetElementValue("IconWidth", string.IsNullOrEmpty(cp.IconWidth) ? "28px" : cp.IconWidth);
        node.SetElementValue("IconHeight", string.IsNullOrEmpty(cp.IconHeight) ? "28px" : cp.IconHeight);
        node.SetElementValue("Icon", string.IsNullOrEmpty(cp.Icon) ? "logo.gif" : cp.Icon);
        //login logo page
        node.SetElementValue("LogoWidth", string.IsNullOrEmpty(cp.LogoWidth) ? "155px" : cp.LogoWidth);
        node.SetElementValue("LogoHeight", string.IsNullOrEmpty(cp.LogoHeight) ? "29px" : cp.LogoHeight);
        node.SetElementValue("Logo", string.IsNullOrEmpty(cp.Logo) ? "logo_white.png" : cp.Logo);
        node.SetElementValue("LoginBg", string.IsNullOrEmpty(cp.LoginBg) ? "Loginbg.png" : cp.LoginBg);
        //
        node.SetElementValue("AboutCompany", string.IsNullOrEmpty(cp.AboutCompany) ? "" : cp.AboutCompany);
        //Legal Information
        node.SetElementValue("LegalInformation", string.IsNullOrEmpty(cp.LegalInformation) ? "Legal Information" : cp.LegalInformation);
        node.SetElementValue("LegalInformationLink", string.IsNullOrEmpty(cp.LegalInformationLink) ? "/PolicyAndService/Licensing.pdf" : cp.LegalInformationLink);
        node.SetElementValue("LegalInformationAttach", string.IsNullOrEmpty(cp.LegalInformationAttach) ? "Licensing.pdf" : cp.LegalInformationAttach);
        node.SetElementValue("LegalInformationDisplay", string.IsNullOrEmpty(cp.LegalInformationDisplay) ? "1" : cp.LegalInformationDisplay);
        node.SetElementValue("PrivacyPolicy", string.IsNullOrEmpty(cp.PrivacyPolicy) ? "Privacy Policy" : cp.PrivacyPolicy);
        node.SetElementValue("PrivacyPolicyLink", string.IsNullOrEmpty(cp.PrivacyPolicyLink) ? "/PolicyAndService/PrivacyPolicy.pdf" : cp.PrivacyPolicyLink);
        node.SetElementValue("PrivacyPolicyAttach", string.IsNullOrEmpty(cp.PrivacyPolicyAttach) ? "PrivacyPolicy.pdf" : cp.PrivacyPolicyAttach);
        node.SetElementValue("PrivacyPolicyDisplay", string.IsNullOrEmpty(cp.PrivacyPolicyDisplay) ? "1" : cp.PrivacyPolicyDisplay);
        node.SetElementValue("TermsOfService", string.IsNullOrEmpty(cp.TermsOfService) ? "Terms Of Service" : cp.TermsOfService);
        node.SetElementValue("TermsOfServiceLink", string.IsNullOrEmpty(cp.TermsOfServiceLink) ? "/PolicyAndService/Terms_Of_Service.pdf" : cp.TermsOfServiceLink);
        node.SetElementValue("TermsOfServiceAttach", string.IsNullOrEmpty(cp.TermsOfServiceAttach) ? "Terms_Of_Service.pdf" : cp.TermsOfServiceAttach);
        node.SetElementValue("TermsOfServiceDisplay", string.IsNullOrEmpty(cp.TermsOfServiceDisplay) ? "1" : cp.TermsOfServiceDisplay);
        node.SetElementValue("ThirdParty", string.IsNullOrEmpty(cp.ThirdParty) ? "Third-Party Licenses" : cp.ThirdParty);
        node.SetElementValue("ThirdPartyLink", string.IsNullOrEmpty(cp.ThirdPartyLink) ? "/PolicyAndService/Third_Party_Licenses.pdf" : cp.ThirdPartyLink);
        node.SetElementValue("ThirdPartyAttach", string.IsNullOrEmpty(cp.ThirdPartyAttach) ? "Third_Party_Licenses.pdf" : cp.ThirdPartyAttach);
        node.SetElementValue("ThirdPartyDisplay", string.IsNullOrEmpty(cp.ThirdPartyDisplay) ? "1" : cp.ThirdPartyDisplay);
        //Cookie Information
        node.SetElementValue("CookieInformation", string.IsNullOrEmpty(cp.CookieInformation) ? "Cookie Information" : cp.CookieInformation);
        node.SetElementValue("CookieInformationLink", string.IsNullOrEmpty(cp.CookieInformationLink) ? "/PolicyAndService/CookiePolicy.pdf" : cp.CookieInformationLink);
        node.SetElementValue("CookieInformationAttach", string.IsNullOrEmpty(cp.CookieInformationAttach) ? "CookiePolicy.pdf" : cp.CookieInformationAttach);
        node.SetElementValue("CookieInformationDisplay", string.IsNullOrEmpty(cp.CookieInformationDisplay) ? "1" : cp.CookieInformationDisplay);
        //
        node.SetElementValue("Emailto", string.IsNullOrEmpty(cp.Emailto) ? "Email To" : cp.Emailto);
        node.SetElementValue("EmailtoAddress", string.IsNullOrEmpty(cp.EmailtoAddress) ? "contact@turanto.com" : cp.EmailtoAddress);
        node.SetElementValue("CreatedBy", string.IsNullOrEmpty(cp.CreatedBy) ? "Created By" : cp.CreatedBy);
        node.SetElementValue("CreatedByLink", string.IsNullOrEmpty(cp.CreatedByLink) ? "http://www.turanto.com/" : cp.CreatedByLink);
        node.SetElementValue("CreatedByName", string.IsNullOrEmpty(cp.CreatedByName) ? "Turanto" : cp.CreatedByName);
        //
        node.SetElementValue("Disclaimer", string.IsNullOrEmpty(cp.Disclaimer) ? "" : cp.Disclaimer);
        node.SetElementValue("SMTPUser", string.IsNullOrEmpty(Convert.ToString(cp.SMTPUser)) ? cp.Email : Convert.ToString(cp.SMTPUser));
        node.SetElementValue("UseAnonymous", string.IsNullOrEmpty(Convert.ToString(cp.UseAnonymous)) ? "false" : Convert.ToString(cp.UseAnonymous));
        companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
    }
    
    /// <summary>Gets email users by tenant.</summary>
    ///
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>The email users by tenant.</returns>
    
    public List<string> getEmailUsersByTenant(long tenantId)
    {
        List<string> emails = new List<string>();
        return emails;
    }
    
    /// <summary>Sets view bag email.</summary>
    ///
    /// <param name="user">The user.</param>
    ///
    /// <returns>A System.Web.Mvc.SelectList.</returns>
    
    public System.Web.Mvc.SelectList SetViewBagEmail(IUser user)
    {
        return null;
    }
    
    /// <summary>Sets view bag.</summary>
    ///
    /// <param name="user">    The user.</param>
    /// <param name="tenantId">Identifier for the tenant.</param>
    ///
    /// <returns>A System.Web.Mvc.SelectList.</returns>
    
    public System.Web.Mvc.SelectList SetViewBag(IUser user, long? tenantId)
    {
        return null;
    }
}
