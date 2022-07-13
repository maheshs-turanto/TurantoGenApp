using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace GeneratorBase.MVC.Controllers
{
public static class UserDataProtectionProvider
{
    public static Microsoft.Owin.Security.DataProtection.IDataProtectionProvider dataProtectionProvider;
}
/// <summary>A controller for handling identity bases.</summary>
[NoCache]
[Audit(true)]
public class IdentityBaseController : Controller
{
    /// <summary>Gets or sets the identitydb.</summary>
    ///
    /// <value>The identitydb.</value>
    
    public ApplicationDbContext Identitydb
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Gets or sets the loggged in user.</summary>
    ///
    /// <value>The loggged in user.</value>
    
    public new GeneratorBase.MVC.Models.IUser LogggedInUser
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Gets or sets the manager for user.</summary>
    ///
    /// <value>The user manager.</value>
    
    public UserManager<ApplicationUser> UserManager
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the manager for role.</summary>
    ///
    /// <value>The role manager.</value>
    
    public RoleManager<ApplicationRole> RoleManager
    {
        get;
        private set;
    }
    
    /// <summary>Called when authorization occurs.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnAuthorization(AuthorizationContext filterContext)
    {
        LogggedInUser = base.User as GeneratorBase.MVC.Models.IUser;
        Identitydb = new ApplicationDbContext(base.User as GeneratorBase.MVC.Models.IUser);
        //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        //var dataProtectionProvider = Startup.DataProtectionProvider;
        //this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
    }
    
    /// <summary>Called before the action method is invoked.</summary>
    ///
    /// <param name="filterContext">Information about the current request and action.</param>
    
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        this.RoleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext(LogggedInUser)));
        this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
        //var dataProtectionProvider = Startup.DataProtectionProvider;
        //this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
        //var dataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("AppName");
        //this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
        this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(GeneratorBase.MVC.Controllers.UserDataProtectionProvider.dataProtectionProvider.Create("AppName"));
        UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        UserManager.UserLockoutEnabledByDefault = true;
        using(ApplicationContext db = new ApplicationContext(new SystemUser()))
        {
            var appSettings = db.AppSettings;
            string applySecurityPolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
            if(applySecurityPolicy.ToLower() == "yes")
            {
                UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(Double.Parse(appSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                UserManager.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(appSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value);
                UserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = Convert.ToInt32(appSettings.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value),
                    RequireNonLetterOrDigit = appSettings.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireDigit = appSettings.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireLowercase = appSettings.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireUppercase = appSettings.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                };
            }
            //Two factor change
            string twofactorauthenticationenable = appSettings.Where(p => p.Key == "TwoFactorAuthenticationEnable").FirstOrDefault().Value;
            if(twofactorauthenticationenable.ToLower() == "yes")
            {
                UserManager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is: {0}"
                });
                UserManager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
                {
                    MessageFormat = "Your security code is: {0}"
                });
                UserManager.EmailService = new EmailService();
                UserManager.SmsService = new SmsService();
            }
        }
        base.OnActionExecuting(filterContext);
    }
    
    /// <summary>Query if 'file' is file type and size allowed.</summary>
    ///
    /// <param name="file">The file.</param>
    ///
    /// <returns>True if file type and size allowed, false if not.</returns>
    protected bool IsFileTypeAndSizeAllowed(HttpPostedFileBase file)
    {
        using(var db = new ApplicationContext(new SystemUser()))
        {
            var fileType = db.AppSettings.Where(p => p.Key == "FileTypes").FirstOrDefault();
            var fileSize = db.AppSettings.Where(p => p.Key == "FileSize").FirstOrDefault();
            var fileExt = System.IO.Path.GetExtension(file.FileName).Replace(".", "").Trim();
            bool isFileValid = true;
            string alertMsg = "Validation Alert - ";
            if(fileType != null)
                if(!(fileType.Value.ToLower().Contains(fileExt.ToLower())))
                {
                    alertMsg = alertMsg + "File type is not proper. Accepts only [" + fileType.Value + "] file types)";
                    isFileValid = false;
                }
            if(fileSize != null)
                if(!(Convert.ToInt32(fileSize.Value) * 1024 * 1024 >= file.ContentLength))
                {
                    alertMsg = alertMsg + " File size is not proper. Accepts file of size less than [ " + fileSize.Value + " MB ])";
                    isFileValid = false;
                }
            if(!isFileValid)
            {
                ModelState.AddModelError("CustomError", alertMsg);
                ViewBag.ApplicationError = alertMsg;
            }
            return isFileValid;
        }
    }
    /// <summary>Saves a document.</summary>
    ///
    /// <param name="file">The file.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long SaveDocument(HttpPostedFileBase file)
    {
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = file.FileName;
        filename = System.IO.Path.GetFileName(file.FileName);
        fileExt = System.IO.Path.GetExtension(file.FileName);
        fileSize = file.ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(file.InputStream))
        {
            fileData = binaryReader.ReadBytes(file.ContentLength);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
        document.FileName = System.IO.Path.GetFileName(file.FileName);
        document.FileSize = fileSize;
        document.MIMEType = file.ContentType;
        document.Byte = fileData;
        using(var db = new ApplicationContext(new SystemUser()))
        {
            db.Documents.Add(document);
            db.SaveChanges();
        }
        return document.Id;
    }
    protected string SaveAnyDocument(HttpPostedFileBase file, string filetype, long? Id = null)
    {
        string result = string.Empty;
        string path = Server.MapPath("~/Files/");
        string ticks = DateTime.UtcNow.Ticks.ToString();
        if(filetype.ToLower() == "file")
        {
            file.SaveAs(path + ticks + System.IO.Path.GetFileName(file.FileName.Trim().Replace(" ", "")));
            result = ticks + System.IO.Path.GetFileName(file.FileName.Trim().Replace(" ", ""));
        }
        else if(filetype.ToLower() == "byte")
        {
            using(var db = new ApplicationContext(new SystemUser()))
            {
                var document = db.Documents.Find(Id);
                if(document == null)
                    document = new Document();
                string fileExt = "";
                string filename = "";
                long fileSize = 0;
                document.DocumentName = file.FileName;
                filename = System.IO.Path.GetFileName(file.FileName);
                fileExt = System.IO.Path.GetExtension(file.FileName);
                fileSize = file.ContentLength;
                byte[] fileData = null;
                using(var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                document.DocumentName = filename;
                document.DateCreated = System.DateTime.UtcNow.Date;
                document.DateLastUpdated = System.DateTime.UtcNow.Date;
                document.FileExtension = fileExt;
                document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
                document.FileName = System.IO.Path.GetFileName(file.FileName);
                document.FileSize = fileSize;
                document.MIMEType = file.ContentType;
                document.Byte = fileData;
                if(document != null && Id > 0)
                    db.Entry(document).State = EntityState.Modified;
                else
                    db.Documents.Add(document);
                db.SaveChanges();
                result = document.Id.ToString();
            }
        }
        return result;
    }
    protected string SaveAnyCameraFile(HttpRequestBase formRequest, string fieldname, string filetype, long? Id = null)
    {
        string result = string.Empty;
        string path = Server.MapPath("~/Files/");
        string ticks = DateTime.UtcNow.Ticks.ToString();
        if(filetype.ToLower() == "file")
        {
            using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(formRequest.Form[fieldname]))))
            {
                image.Save(path + ticks + "Camera-" + ticks + "-" + LogggedInUser.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                result = ticks + "Camera-" + ticks + "-" + LogggedInUser.Name + ".jpg";
            }
        }
        else if(filetype.ToLower() == "byte")
        {
            using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(formRequest.Form[fieldname]))))
            {
                image.Save(path + ticks + "Camera-" + ticks + "-" + LogggedInUser.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                string fileext = ".jpeg";
                string fileName = ticks + "Camera-" + ticks + "-" + LogggedInUser.Name;
                byte[] bytes = Convert.FromBase64String(formRequest.Form[fieldname]);
                long _contentLength = bytes.Length;
                int Imglen = bytes.Length;
                using(var db = new ApplicationContext(new SystemUser()))
                {
                    var document = db.Documents.Find(Id);
                    if(document == null)
                        document = new Document();
                    long fileSize = 0;
                    byte[] fileData = null;
                    using(var binaryReader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        fileData = binaryReader.ReadBytes(Imglen);
                    }
                    document.DocumentName = fileName;
                    document.DateCreated = System.DateTime.UtcNow.Date;
                    document.DateLastUpdated = System.DateTime.UtcNow.Date;
                    document.FileExtension = fileext;
                    document.DisplayValue = fileName;
                    document.FileName = fileName;
                    document.FileSize = fileSize;
                    document.MIMEType = "image/png";
                    document.Byte = fileData;
                    if(document != null && Id > 0)
                        db.Entry(document).State = EntityState.Modified;
                    else
                        db.Documents.Add(document);
                    db.SaveChanges();
                    result = document.Id.ToString();
                }
            }
        }
        return result;
    }
    /// <summary>Set Cookie.</summary>
    ///
    /// <param name="cookiename"> The cookiename.</param>
    /// <param name="value">value.</param>
    public void SetCookie(string cookiename, string value)
    {
        cookieParameter param = new cookieParameter(1);
        HttpCookie cookie = new HttpCookie(cookiename);
        cookie.HttpOnly = param.httponly;
        //cookie.Secure = param.secure;
        //browsers which support the secure flag will only send cookies with the secure flag when the request is going to a HTTPS page
        //web.config has default global setting for secured application cookies, removing web.config httpcookie tag will insecure cookies.
        cookie.Expires = param.expiration;
        cookie.Value = value;
        Response.Cookies.Add(cookie);
    }
    /// <summary>Remove Cookie.</summary>
    ///
    /// <param name="cookiename"> The cookiename.</param>
    public void RemoveCookie(string cookiename)
    {
        cookieParameter param = new cookieParameter(-1);
        HttpCookie cookie = new HttpCookie(cookiename);
        cookie.HttpOnly = param.httponly;
        //browsers which support the secure flag will only send cookies with the secure flag when the request is going to a HTTPS page
        //web.config has default global setting for secured application cookies, removing web.config httpcookie tag will insecure cookies.
        //cookie.Secure = param.secure;
        cookie.Value = null;
        cookie.Expires = param.expiration;
        Response.Cookies.Add(cookie);
    }
    
    /// <summary>Updates the document.</summary>
    ///
    /// <param name="file"> The file.</param>
    /// <param name="docId">Identifier for the document.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long UpdateDocument(HttpPostedFileBase file, long? docId)
    {
        using(var db = new ApplicationContext(new SystemUser()))
        {
            var document = db.Documents.Find(docId);
            if(document == null)
                return SaveDocument(file);
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            //Document document = new Document();
            document.DocumentName = file.FileName;
            filename = System.IO.Path.GetFileName(file.FileName);
            fileExt = System.IO.Path.GetExtension(file.FileName);
            fileSize = file.ContentLength;
            byte[] fileData = null;
            using(var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.UtcNow.Date;
            document.DateLastUpdated = System.DateTime.UtcNow.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = filename;
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = file.ContentType;
            document.Byte = fileData;
            //db.Documents.Add(document);
            db.Entry(document).State = EntityState.Modified;
            db.SaveChanges();
            return document.Id;
        }
    }
    /// <summary>add/upadte for camera.</summary>
    ///
    /// <param name="ext">          The extent.</param>
    /// <param name="camfilename">  The camfilename.</param>
    /// <param name="filebyte">     The filebyte.</param>
    /// <param name="ContentLength">Length of the content.</param>
    /// <param name="len">          The length.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long SaveDocumentCameraImage(string ext, string camfilename, byte[] filebyte, long ContentLength, int len)
    {
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = filename;
        filename = camfilename;
        fileExt = ext;
        fileSize = ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
        {
            fileData = binaryReader.ReadBytes(len);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = "image/png";
        document.Byte = fileData;
        using(var db = new ApplicationContext(new SystemUser()))
        {
            db.Documents.Add(document);
            db.SaveChanges();
        }
        return document.Id;
    }
    
    /// <summary>Updates the document camera.</summary>
    ///
    /// <param name="ext">          The extent.</param>
    /// <param name="camfilename">  The camfilename.</param>
    /// <param name="filebyte">     The filebyte.</param>
    /// <param name="ContentLength">Length of the content.</param>
    /// <param name="len">          The length.</param>
    /// <param name="docId">        Identifier for the document.</param>
    ///
    /// <returns>A long.</returns>
    
    protected long UpdateDocumentCamera(string ext, string camfilename, byte[] filebyte, long ContentLength, int len, long? docId)
    {
        using(var db = new ApplicationContext(new SystemUser()))
        {
            var document = db.Documents.Find(docId);
            if(document == null)
                return SaveDocumentCameraImage(ext, camfilename, filebyte, ContentLength, len);
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            document.DocumentName = camfilename;
            filename = camfilename;
            fileExt = ext;
            fileSize = ContentLength;
            byte[] fileData = null;
            using(var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
            {
                fileData = binaryReader.ReadBytes(len);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.UtcNow.Date;
            document.DateLastUpdated = System.DateTime.UtcNow.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = filename;
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = "image/png";
            document.Byte = fileData;
            db.Entry(document).State = EntityState.Modified;
            db.SaveChanges();
            return document.Id;
        }
    }
    
}

}