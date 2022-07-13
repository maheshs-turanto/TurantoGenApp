using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.AspNet.Identity
{
public class EmailService : IIdentityMessageService
{
    public Task SendAsync(IdentityMessage message)
    {
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        CompanyProfile cp = commonObj.getCompanyProfile(new SystemUser());
        if(cp != null)
        {
            //added for enable email notification
            if(commonObj.EnableNotification().ToLower() == "true")
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(cp.Email, cp.Name);// changed from SMTPUser to Name on 9/24/2020
                string client = cp.SMTPServer;
                SmtpClient smtp = new SmtpClient(client);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = cp.SSL ?? false;
                smtp.Port = cp.SMTPPort; // 25 is default for sendgrid
                smtp.Timeout = 3000; //TO DO - read value from app settings
                if(!cp.UseAnonymous)
                {
                    smtp.Credentials = new NetworkCredential(cp.SMTPUser, cp.SMTPPassword);
                }
                try
                {
                    smtp.Send(mail);
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }
        return Task.FromResult(0);
    }
}
}