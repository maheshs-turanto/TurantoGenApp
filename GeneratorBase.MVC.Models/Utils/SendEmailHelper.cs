using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A send email.</summary>
public class SendEmail
{
    /// <summary>Instant notification.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="Id">        The identifier.</param>
    /// <param name="actionid">  The actionid.</param>
    /// <param name="userName">  Name of the user.</param>
    
    public void InstantNotification(string EntityName, long Id, long actionid, string userName, IUser user, object entity)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers.TimeBasedAlertController, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType("TimeBasedAlert").controllerType;
        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
        {
            System.Reflection.MethodInfo mc = controller.GetMethod("NotifyOneTime");
            object[] MethodParams = new object[] { EntityName, Id, actionid, userName, user, entity };
            var entry = mc.Invoke(objController, MethodParams);
        }
    }
    /// <summary>Instant web notification.</summary>
    /// <param name="EntityName"></param>
    /// <param name="Id"></param>
    /// <param name="actionid"></param>
    /// <param name="userName"></param>
    /// <param name="entity"></param>
    public void InstantWebNotification(string EntityName, long Id, long actionid, string userName, object entity)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers.NotificationController, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType("Notification").controllerType;
        if(controller != null)
        {
            using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
            {
                System.Reflection.MethodInfo mc = controller.GetMethod("CreateNotificationBR");
                object[] MethodParams = new object[] { Id, actionid, userName, EntityName, entity };
                var entry = mc.Invoke(objController, MethodParams);
            }
        }
    }
    
    /// <summary>Notifies.</summary>
    ///
    /// <param name="userID"> Identifier for the user.</param>
    /// <param name="ToID">   Identifier for to.</param>
    /// <param name="Body">   The body.</param>
    /// <param name="Subject">The subject.</param>
    /// <returns>string</returns>
    public string Notify(string userID, String ToID, String Body, String Subject)
    {
        var _cp = GeneratorBase.MVC.Models.CommonFunction.Instance;
        CompanyProfile cp = _cp.GetCompanyProfile(userID);
        if(cp != null)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(ToID.TrimEnd(",".ToCharArray()).TrimEnd(";".ToCharArray()));
            mail.Subject = Subject;
            if(cp.Address != null)
                Body = Body.Replace("###CompanyProfileAddress###", cp.Address);
            byte[] logoBytes = null;
            AlternateView alterView = null;
            if(!cp.Icon.Contains("logo"))
            {
                var logo = new ApplicationContext(new SystemUser()).Documents.Find(Convert.ToInt64(cp.Icon));
                if(logo != null && logo.Byte != null)
                {
                    logoBytes = logo.Byte;
                    string base64String = Convert.ToBase64String(logoBytes, 0, logoBytes.Length);
                    var imagedata = "data:image/" + logo.FileExtension.Replace(".", "").Trim() + ";base64," + base64String;
                    Body = Body.Replace("###CompanyProfileLogo###", "<img src=\"" + imagedata + "\" style=\"width:100px; height:100px;\">");
                }
            }
            alterView = ContentToAlternateView(Body);
            //added for enable email notification
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            if(commonObj.EnableNotification().ToLower() == "true")
            {
                mail.AlternateViews.Add(alterView);
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(cp.Email, cp.Name);// changed from SMTPUser to Name on 9/24/2020
                string client = cp.SMTPServer;
                SmtpClient smtp = new SmtpClient(client);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = cp.SSL ?? false;
                smtp.Port = cp.SMTPPort; // 25 is default for sendgrid
                smtp.Timeout = 4000; //TO DO - read value from app settings
                if(!cp.UseAnonymous)
                {
                    smtp.Credentials = new NetworkCredential(cp.SMTPUser, cp.SMTPPassword);
                }
                try
                {
                    smtp.Send(mail);
                    return "EmailSent";
                }
                catch(SmtpException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return "SMTP Issue: " + ex.Message;
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return ex.Message;
                }
            }
            else
            {
                if(Subject.ToLower().Contains("reset your password") || Subject.Contains("Create Password for New Account"))
                    return "Unable to send Email as Email notification is not set to Yes.";
                else if(Subject.Contains("Your registration is successful") || Subject.Contains("You have been registered successfully"))
                    return "User has been created but unable to send Email as Email notification is not set to Yes.";
                else
                    return "Unable to send Email as Email notification is not set to Yes.";
            }
        }
        else
            return "CompanyProfileNotSet";
    }
    public string NotifyWithAttachment(String ToID, String Body, String Subject, Attachment attachment)
    {
        var _cp = GeneratorBase.MVC.Models.CommonFunction.Instance;
        CompanyProfile cp = _cp.getCompanyProfile(new SystemUser());
        if(cp != null)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(ToID.TrimEnd(",".ToCharArray()).TrimEnd(";".ToCharArray()));
            mail.Subject = Subject;
            //mail.Body = Body;
            AlternateView alterView = ContentToAlternateView(Body);
            //added for enable email notification
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            if(commonObj.EnableNotification().ToLower() == "true")
            {
                mail.AlternateViews.Add(alterView);
                mail.IsBodyHtml = true;
                mail.Attachments.Add(attachment);
                mail.From = new MailAddress(cp.Email, cp.Name);// changed from SMTPUser to Name on 9/24/2020
                string client = cp.SMTPServer;
                SmtpClient smtp = new SmtpClient(client);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = cp.SSL ?? false;
                smtp.Port = cp.SMTPPort; // 25 is default for sendgrid
                smtp.Timeout = 4000; //TO DO - read value from app settings
                if(!cp.UseAnonymous)
                {
                    smtp.Credentials = new NetworkCredential(cp.SMTPUser, cp.SMTPPassword);
                }
                try
                {
                    smtp.Send(mail);
                    return "EmailSent";
                }
                catch(SmtpException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return "SMTP Issue: " + ex.Message;
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return ex.Message;
                }
            }
            else
            {
                if(Subject.ToLower().Contains("reset your password") || Subject.Contains("Create Password for New Account"))
                    return "Unable to send Email as Email notification is not set to Yes.";
                else if(Subject.Contains("Your registration is successful") || Subject.Contains("You have been registered successfully"))
                    return "User has been created but unable to send Email as Email notification is not set to Yes.";
                else
                    return "Unable to send Email as Email notification is not set to Yes.";
            }
        }
        else
            return "CompanyProfileNotSet";
    }
    public string NotifyWithAttachment(String ToID, String Body, String Subject, List<Attachment> attachments)
    {
        var _cp = GeneratorBase.MVC.Models.CommonFunction.Instance;
        CompanyProfile cp = _cp.getCompanyProfile(new SystemUser());
        if(cp != null)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(ToID.TrimEnd(",".ToCharArray()).TrimEnd(";".ToCharArray()));
            mail.Subject = Subject;
            //mail.Body = Body;
            AlternateView alterView = ContentToAlternateView(Body);
            //added for enable email notification
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            if(commonObj.EnableNotification().ToLower() == "true")
            {
                if(attachments != null && attachments.Count > 0)
                {
                    foreach(var attachment in attachments)
                    {
                        mail.Attachments.Add(attachment);
                    }
                }
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(alterView);
                mail.From = new MailAddress(cp.Email, cp.Name);// changed from SMTPUser to Name on 9/24/2020
                string client = cp.SMTPServer;
                SmtpClient smtp = new SmtpClient(client);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = cp.SSL ?? false;
                smtp.Port = cp.SMTPPort; // 25 is default for sendgrid
                smtp.Timeout = 4000; //TO DO - read value from app settings
                if(!cp.UseAnonymous)
                {
                    smtp.Credentials = new NetworkCredential(cp.SMTPUser, cp.SMTPPassword);
                }
                try
                {
                    smtp.Send(mail);
                    return "EmailSent";
                }
                catch(SmtpException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return "SMTP Issue: " + ex.Message;
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return ex.Message;
                }
            }
            else
            {
                if(Subject.ToLower().Contains("reset your password") || Subject.Contains("Create Password for New Account"))
                    return "Unable to send Email as Email notification is not set to Yes.";
                else if(Subject.Contains("Your registration is successful") || Subject.Contains("You have been registered successfully"))
                    return "User has been created but unable to send Email as Email notification is not set to Yes.";
                else
                    return "Unable to send Email as Email notification is not set to Yes.";
            }
        }
        else
            return "CompanyProfileNotSet";
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static AlternateView ContentToAlternateView(string content)
    {
        var imgCount = 0;
        List<LinkedResource> resourceCollection = new List<LinkedResource>();
        foreach(Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
        {
            imgCount++;
            var imgContent = m.Groups["value"].Value;
            string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
            string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
            if(String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
            {
                //ignore replacement when match normal <img> tag
                continue;
            }
            var replacement = " src=\"cid:" + imgCount + "\"";
            content = content.Replace(imgContent, replacement);
            var tempResource = new LinkedResource(Base64ToImageStream(base64), new System.Net.Mime.ContentType(type))
            {
                ContentId = imgCount.ToString()
            };
            resourceCollection.Add(tempResource);
        }
        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, System.Net.Mime.MediaTypeNames.Text.Html);
        foreach(var item in resourceCollection)
        {
            alternateView.LinkedResources.Add(item);
        }
        return alternateView;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static System.IO.Stream Base64ToImageStream(string base64String)
    {
        byte[] imageBytes = Convert.FromBase64String(base64String);
        System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);
        return ms;
    }
    /// <summary>Decryptdata.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>A string.</returns>
    
    public string Decryptdata(string password)
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
    
    
    public string getEmailBodyFromTemplate(string templatetype, Dictionary<string, string> dictionary)
    {
        string mailbody = string.Empty;
        string mailsubject = string.Empty;
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == templatetype);
        if(EmailTemplate != null)
        {
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                foreach(var item in dictionary)
                {
                    try
                    {
                        // mailbody = mailbody.Replace(item.Key, item.Value);
                        mailbody = Regex.Replace(mailbody, item.Key, item.Value, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
        return mailbody;
    }
    
    
    public List<string> getEmailBodyAndSubjectFromTemplate(string templatetype, Dictionary<string, string> dictionary)
    {
        string mailbody = string.Empty;
        string mailsubject = string.Empty;
        var result = new List<string>();
        var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == templatetype);
        if(EmailTemplate != null)
        {
            mailsubject = EmailTemplate.EmailSubject;
            if(!string.IsNullOrEmpty(EmailTemplate.EmailContent))
            {
                mailbody = EmailTemplate.EmailContent;
                foreach(var item in dictionary)
                {
                    try
                    {
                        // mailbody = mailbody.Replace(item.Key, item.Value);
                        mailbody = Regex.Replace(mailbody, item.Key, item.Value, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
        result.Add(mailsubject);
        result.Add(mailbody);
        return result;
    }
}
}