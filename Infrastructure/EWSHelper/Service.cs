using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EWSHelper
{
    public static class Service
    {
        // The following is a basic redirection validation callback method. It 
        // inspects the redirection URL and only allows the Service object to 
        // follow the redirection link if the URL is using HTTPS. 
        //
        // This redirection URL validation callback provides sufficient security
        // for development and testing of your application. However, it may not
        // provide sufficient security for your deployed application. You should
        // always make sure that the URL validation callback method that you use
        // meets the security requirements of your organization.
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

        public static ExchangeService ConnectToService(string emailAccount, string passWord,bool isAutodiscover)
        {
            return ConnectToService(emailAccount, passWord, null, isAutodiscover);
        }

        public static ExchangeService ConnectToService(string emailAccount, string passWord, ITraceListener listener, bool isAutodiscover)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            }

           // service.Credentials = new NetworkCredential(emailAccount, passWord);
            service.Credentials = new WebCredentials(emailAccount, passWord);
            if (isAutodiscover)
            {
                service.AutodiscoverUrl(emailAccount, RedirectionUrlValidationCallback);
            }
            else
            {
                service.Url = new Uri("https://outlook.office365.com/ews/exchange.asmx");
            }

            return service;
        }
    }
}
