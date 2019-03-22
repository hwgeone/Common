using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailHelper
{
    public class Email
    {
        public string username;
        public string password;
        public string body;
        public string subject;
        public DateTime dueDate;
        public OlImportance importance;
        public string from;
        public string to;
        public bool isHtml;
        public List<System.Net.Mail.Attachment> mailAttach = new List<System.Net.Mail.Attachment>();
        public string server;
        public string CC;

        public Email()
        { }

        public void AddAttachment(string path)
        {
            System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(path, MediaTypeNames.Application.Zip);
            // Add time stamp information for the file.
            ContentDisposition disposition = att.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(path);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(path);

            mailAttach.Add(att);
        }
    }
}
