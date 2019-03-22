using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EWSHelper
{
    public static class ItemManager
    {
        //延迟邮件
        public static EmailMessage BuildDelayedEmailMessage(ExchangeService service,DateTime sendTime ,string body, string to, string subject, string cc)
        {
            EmailMessage message = BuildEmailMessage(service,body,to,subject,cc);

            //设置延迟时间
            string _sendTime = sendTime.ToUniversalTime().ToString();
            CreateCustomExtendedProperties(message, Guid.NewGuid(), "delaysendmail", MapiPropertyType.SystemTime, _sendTime);
           
            return message;
        }

        public static EmailMessage BuildEmailMessage(ExchangeService service,string body, string to, string subject, string cc)
        {
            EmailMessage message = new EmailMessage(service);
            string[] arrTo = to.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var it in arrTo)
            {
                message.ToRecipients.Add(it);
            }
            message.Subject = subject;
            message.Body = new MessageBody(body);
            if (!string.IsNullOrEmpty(cc))
            {
                string[] arrCC = cc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var it in arrCC)
                {
                    message.CcRecipients.Add(it);
                }
            }

            return message;
        }

        public static void BuildEmailAttachments(EmailMessage message,string path)
        {
            // Add a file attachment by using a byte array.
            // In this example, theBytes is the byte array that represents the content of the image file to attach.
            string[] arrAtt = path.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var it in arrAtt)
            {
                message.Attachments.AddFileAttachment(path);
            }

        }

        public static void BuildEmailAttachments(EmailMessage message, string fileName, Stream sm)
        {
            message.Attachments.AddFileAttachment(fileName, sm);
        }

        public static void BuildEmailAttachments(EmailMessage message, string path, string alias)
        {
            byte[] theBytes = File.ReadAllBytes(path);

            // The byte array file attachment is named attNewName.
            message.Attachments.AddFileAttachment(alias, theBytes);
        }


        /// <summary>
        /// 创建与内嵌附件的电子邮件body，使用 EWS 托管 API
        /// </summary>
        /// <param name="path"></param>
        public static void CreateEmailBodyWithInlineAttachment(EmailMessage message,string path)
        {
            // Create the HTML body with the content identifier of the attachment.
            string html = @"<html>
                        <head>
                        </head>
                        <body>
                        <img width=100 height=100 id=""1"" src=""cid:" + Path.GetFileName(path) + "\">"
                        + "</body>"
                        + "</html>";
        }

        //public static Contact BuildContact(ExchangeService service)
        //{
        //    Contact contact = new Contact(service);

        //    // Specify the name and how the contact should be filed.
        //    contact.GivenName = "Brian";
        //    contact.MiddleName = "David";
        //    contact.Surname = "Johnson";
        //    contact.FileAsMapping = FileAsMapping.SurnameCommaGivenName;
        //    contact.DisplayName = "Brian Johnson";

        //    // Specify the company name.
        //    contact.CompanyName = "Contoso";

        //    // Specify the business, home, and car phone numbers.
        //    contact.PhoneNumbers[PhoneNumberKey.BusinessPhone] = "425-555-0110";
        //    contact.PhoneNumbers[PhoneNumberKey.HomePhone] = "425-555-0120";
        //    contact.PhoneNumbers[PhoneNumberKey.CarPhone] = "425-555-0130";

        //    // Specify two email addresses.
        //    contact.EmailAddresses[EmailAddressKey.EmailAddress1] = new EmailAddress("brian_1@contoso.com");
        //    contact.EmailAddresses[EmailAddressKey.EmailAddress2] = new EmailAddress("brian_2@contoso.com");

        //    // Specify two IM addresses.
        //    contact.ImAddresses[ImAddressKey.ImAddress1] = "brianIM1@contoso.com";
        //    contact.ImAddresses[ImAddressKey.ImAddress2] = "brianIM2@contoso.com";

        //    // Specify the home address.
        //    PhysicalAddressEntry paEntry1 = new PhysicalAddressEntry();
        //    paEntry1.Street = "123 Main Street";
        //    paEntry1.City = "Seattle";
        //    paEntry1.State = "WA";
        //    paEntry1.PostalCode = "11111";
        //    paEntry1.CountryOrRegion = "United States";
        //    contact.PhysicalAddresses[PhysicalAddressKey.Home] = paEntry1;

        //    // Specify the business address.
        //    PhysicalAddressEntry paEntry2 = new PhysicalAddressEntry();
        //    paEntry2.Street = "456 Corp Avenue";
        //    paEntry2.City = "Seattle";
        //    paEntry2.State = "WA";
        //    paEntry2.PostalCode = "11111";
        //    paEntry2.CountryOrRegion = "United States";
        //    contact.PhysicalAddresses[PhysicalAddressKey.Business] = paEntry2;

        //    contact.Save();
        //    Console.WriteLine("Contact created.");
        //}
        public static void CreateCustomExtendedProperties(EmailMessage message,Guid guid,string propertyName,MapiPropertyType mapiType,object propertyValue)
        {

            // Create a definition for the extended property.
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(guid,
                                                                                                   propertyName,
                                                                                                   mapiType);
            // Add the extended property to an email message object.
            message.SetExtendedProperty(extendedPropertyDefinition, propertyValue);
        }

        public static Appointment BuildMeeting(ExchangeService service,bool isRecurr ,string subject, string body, string location, DateTime start, DateTime end, string requiredAttendees, string optionalAttendees)
        {
            Appointment meeting = new Appointment(service);

            meeting.Subject = subject;
            meeting.Body = body;
            meeting.Start = start;
            meeting.End = end;
            meeting.Location = location;
            if (string.IsNullOrEmpty(requiredAttendees))
            {
                throw new ArgumentNullException("meeting attendees can not be null");
            }
            string [] arratt = requiredAttendees.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var attendee in arratt)
            {
                meeting.RequiredAttendees.Add(attendee);
            }
            if (!string.IsNullOrEmpty(optionalAttendees))
            {
                string[] arr = optionalAttendees.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var option in arr)
                {
                    meeting.OptionalAttendees.Add(option);
                }
            }
            if(isRecurr){
                DayOfTheWeek[] days = new DayOfTheWeek[] { DayOfTheWeek.Monday };
                meeting.Recurrence = new Recurrence.WeeklyPattern(meeting.Start.Date, 1, days);
                meeting.Recurrence.StartDate = meeting.Start.Date;
            }
            return meeting;
        }

        public static Microsoft.Exchange.WebServices.Data.Task BuildTask(ExchangeService service,string subject,string body,bool isRecurr)
        {
            Microsoft.Exchange.WebServices.Data.Task task = new Microsoft.Exchange.WebServices.Data.Task(service);

            // Specify the subject and body of the new task.
            task.Subject = subject;
            task.Body = new MessageBody(BodyType.HTML,body);

            if (isRecurr)
            {
                // Set the recurrance pattern for the new task.
                DayOfTheWeek[] days = new DayOfTheWeek[] { DayOfTheWeek.Friday };
                task.Recurrence = new Recurrence.WeeklyPattern(DateTime.Today, 1, days);
                task.Recurrence.StartDate = DateTime.Today;
                task.Recurrence.NeverEnds();
            }

            return task;
        }

       
    }
}
