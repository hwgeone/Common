using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailHelper
{
    public static class Emailhelper
    {
        private static Email email;
        static Emailhelper()
        {

        }
        public static Email GetEmailObj()
        {
            return email;
        }

        public static void SetEmailObj(Email emailpram)
        {
            email = emailpram;
        }
        /// <summary>
        /// 给outlook添加一个新的任务
        /// </summary>
        /// <param name="subject">新任务标题</param>
        /// <param name="body">新任务正文</param>
        /// <param name="dueDate">新任务到期时间</param>
        /// <param name="importance">新任务优先级</param>
        public static void AddNewTask(Email email)
        {
            try
            {
                Application outLookApp = new Application();
                TaskItem newTask = (TaskItem)outLookApp.CreateItem(OlItemType.olTaskItem);
                newTask.Body = email.body;
                newTask.Subject = email.subject;
                newTask.Importance = email.importance;
                newTask.DueDate = email.dueDate;
                newTask.Save();
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="emailAddr"></param>
        public static void GetContactorInfo(string emailAddr)
        {
            Application OL = new Application();
            //NameSpace OLNameS;
            //MAPIFolder OLFolder;
            //DistListItem OLDistListItem;//用于接收组信息 
            //OLNameS = OL.GetNamespace("MAPI");
            //OLFolder = OLNameS.GetDefaultFolder(OlDefaultFolders.olFolderContacts);
            //int contactItemCout = OLFolder.Items.Count; 
            Accounts accounts = OL.Session.Accounts;
        }

        /// <summary>
        /// 一个最简单的发送邮件的方式。同步方式。只支持发送到一个地址，而且没有附件。
        /// </summary>
        /// <param name="server">smtp服务器地址</param>
        /// <param name="from">发送者邮箱</param>
        /// <param name="to">接收者邮箱</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文</param>
        /// <param name="isHtml">正文是否以html形式展现</param>
        public static void SimpleSendMail(Email email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = email.isHtml;
                mail.From = new MailAddress(email.from);
                mail.To.Add(email.to);
                mail.Subject = email.subject;
                mail.Body = email.body;
                if (email.mailAttach != null && email.mailAttach.Count != 0)
                {
                    foreach (var it in email.mailAttach)
                    {
                        mail.Attachments.Add(it);
                    }
                }
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(email.username, email.password);
                smtp.Port = 587;
                smtp.Host = email.server;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 支持向多人发邮件，并支持多个附件的一个发送邮件的例子。
        /// </summary>
        /// <param name="server">smtp服务器地址</param>
        /// <param name="from">发送者邮箱</param>
        /// <param name="to">接收者邮箱，多个接收者以;隔开</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="mailAttach">附件</param>
        /// <param name="isHtml">邮件正文是否需要以html的方式展现</param>
        public static void MultiSendEmail(Email email)
        {
            MailMessage eMail = new MailMessage();
            eMail.Subject = email.subject;
            eMail.SubjectEncoding = Encoding.UTF8;
            eMail.Body = email.body;
            eMail.BodyEncoding = Encoding.UTF8;
            eMail.From = new MailAddress(email.from);
            if (!string.IsNullOrEmpty(email.CC))
            {
                eMail.CC.Add(email.CC);
            }
           
            string[] arrMailAddr;

            try
            {
                #region 添加多个收件人
                eMail.To.Clear();
                if (!string.IsNullOrEmpty(email.to))
                {
                    arrMailAddr = email.to.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
                    foreach (string strTo in arrMailAddr)
                    {
                        if (!string.IsNullOrEmpty(strTo))
                        {
                            eMail.To.Add(strTo);
                        }
                    }
                }
                #endregion
                #region 添加多个附件
                eMail.Attachments.Clear();
                if (email.mailAttach != null)
                {
                    for (int i = 0; i < email.mailAttach.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(email.mailAttach[i].ToString()))
                        {
                            eMail.Attachments.Add(new System.Net.Mail.Attachment(email.mailAttach[i].ToString()));
                        }
                    }
                }
                #endregion
                #region 发送邮件
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.username, email.password);
                smtp.Port = 587;
                smtp.Host = email.server;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(eMail);
                #endregion
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

    }
}
