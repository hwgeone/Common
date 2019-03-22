using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailHelper
{
    public static class MailToHelper
    {
        public static string ToUrl(string to, string subject, string body)
        {
            string url = HttpUtility.HtmlAttributeEncode(
  String.Format("mailto:{0}?subject={1}&body={2}", to,
  Uri.EscapeDataString(subject),
  Uri.EscapeDataString(body)));
            return url;
        }

        public static string GetFormattedBodyForMailTo(IDictionary<KeyObject, string> keyValues)
        {
            string formattedbody;
            using (var writer = new StringWriter())
            {
                foreach (var keyValue in keyValues)
                {
                    writer.WriteLine(keyValue.Key.ToString() + ":"+keyValue.Value);
                    writer.WriteLine();
                }
                formattedbody = writer.ToString();
            }

            return formattedbody;
        }

        public static string GetBodyTextValueByKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
