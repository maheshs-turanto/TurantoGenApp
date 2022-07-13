using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using GeneratorBase.MVC.Models;

namespace Microsoft.AspNet.Identity
{
public class SmsService : IIdentityMessageService
{
    public Task SendAsync(IdentityMessage message)
    {
        try
        {
            // Twilio Begin
            var accountSid = CommonFunction.Instance.TwilioAccountSID();
            var authToken = CommonFunction.Instance.TwilioAuthToken();
            var fromNumber = CommonFunction.Instance.TwilioFromNumber();
            var countryCode = CommonFunction.Instance.TwilioCountryCode();
            TwilioClient.Init(accountSid, authToken);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;
            MessageResource result = MessageResource.Create(
                                         new PhoneNumber(countryCode + message.Destination),
                                         from: new PhoneNumber(fromNumber),
                                         body: message.Body
                                     );
            //Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status.ToString());
            //Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
            //Twilio End
            // ASPSMS Begin
            // var soapSms = new MvcPWx.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            // soapSms.SendSimpleTextSMS(
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"],
            //   message.Destination,
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"],
            //   message.Body);
            // soapSms.Close();
            // return Task.FromResult(0);
            // ASPSMS End
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            return null;
        }
    }
}
}