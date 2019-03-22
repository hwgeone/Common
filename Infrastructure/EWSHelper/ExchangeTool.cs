using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EWSHelper
{
    public static class ExchangeTool
    {

        #region 邮件
        public static void SendEmail(EmailMessage message)
        {
            if (message == null)
            {
                throw new Exception("Set email object first.");
            }
            else if (message.ToRecipients.Count == 0)
            {
                throw new Exception("Please set toRecipients.");
            }
            else
            {
                message.Send();
            }
        }

        /// <summary>
        /// 使用 EWS 托管 API 发送延迟的电子邮件
        /// </summary>
        /// <returns></returns>
        public static void SendEmailDelay(EmailMessage message, DateTime sendTime)
        {
            // Identify the extended property that can be used to specify when to send the email. 
            ExtendedPropertyDefinition PidTagDeferredSendTime = new ExtendedPropertyDefinition(16367, MapiPropertyType.SystemTime);

            // Set the time that will be used to specify when the email is sent. 
            // In this example, the email will be sent one minute after the next line executes, 
            // provided that the message.SendAndSaveCopy request is processed by the server within one minute. 
            string _sendTime = sendTime.ToUniversalTime().ToString();

            // Specify when to send the email by setting the value of the extended property. 
            message.SetExtendedProperty(PidTagDeferredSendTime, _sendTime);

            // Submit the request to send the email message. 
            message.SendAndSaveCopy();
        }

        /// <summary>
        /// 批量发送邮件
        /// </summary>
        /// <param name="service"></param>
        public static void SendBatchEmails(ExchangeService service, Collection<EmailMessage> messages)
        {

            try
            {
                // Send the batch of email messages. This results in a call to EWS. The response contains the results of the batched request to send email messages.
                ServiceResponseCollection<ServiceResponse> response = service.CreateItems(messages, WellKnownFolderName.Inbox, MessageDisposition.SendOnly, null);

                // Check the response to determine whether the email messages were successfully submitted.
                if (response.OverallResult == ServiceResult.Success)
                {

                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 附件
        public static void GetAttachments(ExchangeService service, ItemView view, string querystring)
        {
            // Find the first email message in the Inbox that has attachments. This results in a FindItem operation call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystring, view);
            Collection<Attachment> attachments = new Collection<Attachment>();
            if (results.TotalCount > 0)
            {
                EmailMessage email = results.Items[0] as EmailMessage;

                // Request all the attachments on the email message. This results in a GetItem operation call to EWS.
                email.Load(new PropertySet(EmailMessageSchema.Attachments));

                foreach (Attachment attachment in email.Attachments)
                {
                    if (attachment is FileAttachment)
                    {
                        FileAttachment fileAttachment = attachment as FileAttachment;

                        // Load the file attachment into memory. This gives you access to the attachment content, which 
                        // is a byte array that you can use to attach this file to another item. This results in a GetAttachment operation
                        // call to EWS.
                        fileAttachment.Load();

                        // Load attachment contents into a file. This results in a GetAttachment operation call to EWS.
                        fileAttachment.Load("C:\\temp\\" + fileAttachment.Name);

                        // Put attachment contents into a stream.
                        using (FileStream theStream = new FileStream("C:\\temp\\Stream_" + fileAttachment.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            //This results in a GetAttachment operation call to EWS.
                            fileAttachment.Load(theStream);
                        }
                    }
                    else // Attachment is an item attachment.
                    {
                        ItemAttachment itemAttachment = attachment as ItemAttachment;

                        // Load the item attachment properties. This results in a GetAttachment operation call to EWS.
                        itemAttachment.Load();
                    }
                }
            }
        }

        public static void GetAttachmentsFromEmail(ExchangeService service, ItemId itemId)
        {
            // Bind to an existing message item and retrieve the attachments collection.
            // This method results in an GetItem call to EWS.
            EmailMessage message = EmailMessage.Bind(service, itemId, new PropertySet(ItemSchema.Attachments));

            // Iterate through the attachments collection and load each attachment.
            foreach (Attachment attachment in message.Attachments)
            {
                if (attachment is FileAttachment)
                {
                    FileAttachment fileAttachment = attachment as FileAttachment;

                    // Load the attachment into a file.
                    // This call results in a GetAttachment call to EWS.
                    fileAttachment.Load("C:\\temp2017\\" + fileAttachment.Name);
                }
                else // Attachment is an item attachment.
                {
                    ItemAttachment itemAttachment = attachment as ItemAttachment;

                    // Load attachment into memory and write out the subject.
                    // This does not save the file like it does with a file attachment.
                    // This call results in a GetAttachment call to EWS.
                    itemAttachment.Load();
                }
            }
        }

        public static void SaveEmailAttachment(ExchangeService service, ItemId itemId)
        {
            // Bind to an existing message item and retrieve the attachments collection.
            // This method results in an GetItem call to EWS.
            EmailMessage message = EmailMessage.Bind(service, itemId, new PropertySet(ItemSchema.Attachments));

            foreach (Attachment attachment in message.Attachments)
            {
                if (attachment is ItemAttachment)
                {
                    ItemAttachment itemAttachment = attachment as ItemAttachment;
                    itemAttachment.Load(ItemSchema.MimeContent);
                    string fileName = "C:\\Temp\\" + itemAttachment.Item.Subject + ".eml";

                    // Write the bytes of the attachment into a file.
                    File.WriteAllBytes(fileName, itemAttachment.Item.MimeContent.Content);

                    Console.WriteLine("Email attachment name: " + itemAttachment.Item.Subject + ".eml");
                }
            }
        }

        public static void DeleteAllAttachments(ExchangeService service, ItemId itemId)
        {
            // Bind to an existing message by using its item ID and requesting its attachments collection.
            // This method results in a GetItem call to EWS.

            EmailMessage message = EmailMessage.Bind(service, itemId, new PropertySet(ItemSchema.Attachments));
            // Delete all attachments from the message.
            message.Attachments.Clear();

            // Save the updated message.
            // This method results in an DeleteAttachment call to EWS.
            message.Update(ConflictResolutionMode.AlwaysOverwrite);
        }

        private static void DeleteAttachments(ExchangeService service, ItemView view, string querystring)
        {
            // Find the first email message in the Inbox that has attachments and a subject that contains 'Message with Attachments'. 
            // This results in a FindItem call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystring, view);

            if (results.TotalCount > 0)
            {
                EmailMessage email = results.Items[0] as EmailMessage;

                // Get all the attachments on the email message. This results in a GetAttachment call to EWS.
                email.Load(new PropertySet(EmailMessageSchema.Attachments));

                // Remove all attachments from an item. 
                email.Attachments.Clear();

                // Save the updated message. This results in a DeleteAttachment operation call to EWS.
                // If any other properties are updated in addition to the attachments,
                // an UpdateItem operation call to EWS will occur.
                email.Update(ConflictResolutionMode.AlwaysOverwrite);
            }
        }

        private static void DeleteAttachments(ExchangeService service, ItemView view, SearchFilter filter)
        {
            // Find the first email message in the Inbox that has attachments and a subject that contains 'Message with Attachments'. 
            // This results in a FindItem call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, filter, view);

            if (results.TotalCount > 0)
            {
                EmailMessage email = results.Items[0] as EmailMessage;

                // Get all the attachments on the email message. This results in a GetAttachment call to EWS.
                email.Load(new PropertySet(EmailMessageSchema.Attachments));

                // Remove all attachments from an item. 
                email.Attachments.Clear();

                // Save the updated message. This results in a DeleteAttachment operation call to EWS.
                // If any other properties are updated in addition to the attachments,
                // an UpdateItem operation call to EWS will occur.
                email.Update(ConflictResolutionMode.AlwaysOverwrite);
            }
        }
        #endregion

        #region 文件夹
        public static void CreateTasksForder(ExchangeService service, string forderName)
        {
            // Create a custom Tasks folder.
            TasksFolder folder = new TasksFolder(service);
            folder.DisplayName = forderName;

            // Save the folder as a child folder in the Inbox folder.
            // This method call results in a CreateFolder call to EWS.
            folder.Save(WellKnownFolderName.Inbox);
        }

        public static void CopyFolder(ExchangeService service, string DisplayName, WellKnownFolderName SourceFolder, WellKnownFolderName DestinationFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (DisplayName) within the specified folder (SourceFolder).
            FolderId folderId = FindFolderIdByDisplayName(service, DisplayName, SourceFolder);

            if (folderId != null)
            {
                // Bind to the source folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Create a copy of the source folder in the specified folder (DestinationFolder).
                Folder newFolder = folder.Copy(DestinationFolder);
            }
            else
            {
                throw new Exception("Not find folder");
            }
        }

        public static void EmptyFolder(ExchangeService service, string DisplayName, DeleteMode deleteMode, bool DeleteSubFolders, WellKnownFolderName ParentFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (DisplayName) within the specified folder (ParentFolder).
            FolderId folderId = FindFolderIdByDisplayName(service, DisplayName, ParentFolder);

            if (folderId != null)
            {
                // Bind to the folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Delete all contents in the folder.
                folder.Empty(deleteMode, DeleteSubFolders);
            }
            else
            {
                throw new Exception("Folder not find");
            }
        }
        #endregion

        #region Item
        public static void DeleteItem(Item item)
        {
            item.Delete(DeleteMode.MoveToDeletedItems, false);
        }
        #endregion

        #region 联系人
        public static void CreateContact(Contact contact)
        {
            contact.Save();
        }

        public static FolderId FindFolderIdByDisplayName(ExchangeService service, string DisplayName, WellKnownFolderName SearchFolder)
        {
            // Specify the root folder to be searched.
            Folder rootFolder = Folder.Bind(service, SearchFolder);

            // Loop through the child folders of the folder being searched.
            foreach (Folder folder in rootFolder.FindFolders(new FolderView(100)))
            {
                // If the display name of the current folder matches the specified display name, return the folder's unique identifier.
                if (folder.DisplayName == DisplayName)
                {
                    return folder.Id;
                }
            }

            // If no folders have a display name that matches the specified display name, return null.
            return null;
        }

        public static void DeleteFolder(ExchangeService service, string DisplayName, DeleteMode deleteMode, WellKnownFolderName ParentFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (DisplayName) within the specified folder (ParentFolder).
            FolderId folderId = FindFolderIdByDisplayName(service, DisplayName, ParentFolder);

            if (folderId != null)
            {
                // Bind to the folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Delete the folder.
                folder.Delete(deleteMode);

            }
            else
            {
                throw new Exception("Not find folder");
            }
        }

        public static Collection<Contact> BatchGetContactItems(ExchangeService service, Collection<ItemId> itemIds)
        {
            // Create a property set that limits the properties returned by the Bind method to only those that are required.
            PropertySet propSet = new PropertySet(BasePropertySet.IdOnly, ContactSchema.DisplayName);

            // Get the items from the server.
            // This method call results in a GetItem call to EWS.
            ServiceResponseCollection<GetItemResponse> response = service.BindToItems(itemIds, propSet);

            // Instantiate a collection of Contact objects to populate from the values that are returned by the Exchange server.
            Collection<Contact> contactItems = new Collection<Contact>();

            foreach (GetItemResponse getItemResponse in response)
            {
                try
                {
                    Item item = getItemResponse.Item;
                    Contact contact = (Contact)item;
                    contactItems.Add(contact);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return contactItems;
        }
        /// <summary>
        /// 根据显示名获取联系人
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <returns></returns>
        public static Contact FindContactByDisplayName(ExchangeService service, string DisplayName)
        {
            // Get the number of items in the Contacts folder. To keep the response smaller, request only the TotalCount property.
            ContactsFolder contactsfolder = ContactsFolder.Bind(service,
                                                                WellKnownFolderName.Contacts,
                                                                new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));

            // Set the number of items to the smaller of the number of items in the Contacts folder or 1000.
            int numItems = contactsfolder.TotalCount < 1000 ? contactsfolder.TotalCount : 1000;

            // Instantiate the item view with the number of items to retrieve from the Contacts folder.
            ItemView view = new ItemView(numItems);

            // To keep the request smaller, send only the display name.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ContactSchema.DisplayName);

            //Create a searchfilter.
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(ContactSchema.DisplayName, DisplayName);

            // Retrieve the items in the Contacts folder that have the properties you've selected.
            FindItemsResults<Item> contactItems = service.FindItems(WellKnownFolderName.Contacts, filter, view);
            if (contactItems.Count() == 1) //Only one contact was found
            {
                return (Contact)contactItems.Items[0];
            }
            else //No contacts, or more than one with the same DisplayName, were found.
            {
                return null;
            }
        }

        public static void DeleteContact(ExchangeService service, string displayName, DeleteMode deleteMode)
        {
            // Look for the contact to delete. If it doesn't exist, create it.
            Contact contact = FindContactByDisplayName(service, displayName);

            if (contact == null)
            {
                throw new Exception("Not find Contact to delete.");
            }

            // Delete the contact.
            contact.Delete(deleteMode);
        }
        #endregion

        #region 约会或会议
        public static void CreateMeeting(ExchangeService service, Collection<Appointment> meetings, bool isBatchCreateMeeting)
        {
            try
            {
                if (isBatchCreateMeeting) // Show batch.
                {
                    // Create the batch of meetings. This results in a CreateItem operation call to EWS.
                    ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(meetings,
                                                                                              WellKnownFolderName.Calendar,
                                                                                              MessageDisposition.SendOnly,
                                                                                              SendInvitationsMode.SendToAllAndSaveCopy);

                    if (responses.OverallResult == ServiceResult.Success)
                    {
                        Console.WriteLine("You've successfully created a couple of meetings in a single call.");
                    }
                    else if (responses.OverallResult == ServiceResult.Warning)
                    {
                        Console.WriteLine("There are some issues with your batch request.");

                        foreach (ServiceResponse response in responses)
                        {
                            if (response.Result == ServiceResult.Error)
                            {
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString());
                                Console.WriteLine("Error message: " + response.ErrorMessage);
                            }
                        }
                    }
                    else // responses.OverallResult == ServiceResult.Error
                    {
                        Console.WriteLine("There are errors with your batch request.");

                        foreach (ServiceResponse response in responses)
                        {
                            if (response.Result == ServiceResult.Error)
                            {
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString());
                                Console.WriteLine("Error message: " + response.ErrorMessage);
                            }
                        }
                    }
                }
                else // Show creation of a single meeting.
                {
                    // Create a single meeting. This results in a CreateItem operation call to EWS.
                    meetings.FirstOrDefault().Save(SendInvitationsMode.SendToAllAndSaveCopy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateMeetingByPrinciple(ExchangeService service, Folder principleCalendar, bool isBatchCreateMeeting, Collection<Appointment> meetingsCollection)
        {
            try
            {
                if (isBatchCreateMeeting) // Show batch
                {
                    Collection<Appointment> meetings = meetingsCollection;
                    // Create the batch of meetings. This results in a call to EWS by using the CreateItem operation.
                    ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(meetings,
                                                                                              principleCalendar.Id,
                                                                                              MessageDisposition.SendOnly,
                                                                                              SendInvitationsMode.SendToAllAndSaveCopy);

                }
                else // Show creation of a single meeting.
                {
                    // Create a single meeting. This results in a call to EWS by using the CreateItem operation.
                    meetingsCollection.FirstOrDefault().Save(principleCalendar.Id, SendInvitationsMode.SendToAllAndSaveCopy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CancelMeeting(ExchangeService service, string querystring, string organizer)
        {
            // Specify a view that returns a single item.
            ItemView view = new ItemView(1);

            try
            {
                // Find the first appointment in the calendar with "Lunch" set for the subject property.
                // This results in a FindItem operation call to EWS.
                // SearchFilter orgfilter = new SearchFilter.IsEqualTo(AppointmentSchema.Organizer,organizer);
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Calendar, querystring, view);

                if (results.TotalCount > 0)
                {
                    if (results.Items[0] is Appointment)
                    {
                        Appointment meeting = results.Items[0] as Appointment;

                        // Determine whether the caller is the organizer. Only organizers can cancel meetings.
                        if (meeting.Organizer.Equals(new EmailAddress(organizer)))
                        {
                            // Cancels the meeting and sends cancellation messages to the attendees.
                            // This results in a call to EWS by means of the CreateItem operation and
                            // a MeetingCancellation response object. Do not delete meetings because
                            // cancellation messages are not sent to attendees. You can also use 
                            // Appointment.Delete(DeleteMode, SendCancellationsMode) to cancel a meeting
                            // but this does not give you an option to send a cancellation message.
                            var cancelResults = meeting.CancelMeeting("This meeting has been canceled");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Collection<EmailAddress> GetRooms(ExchangeService service, string strRoom)
        {
            // Return all the room lists in the organization.
            EmailAddressCollection roomLists = service.GetRoomLists();

            // Retrieve the room list that matches your criteria.
            // Replace "ConfRoomsIn31@contoso.com" with the room list you are looking for.
            EmailAddress roomAddress = new EmailAddress(strRoom);
            foreach (EmailAddress address in roomLists)
            {
                if (address == roomAddress)
                {
                }
                else
                {
                    throw new Exception("Not find room");
                }
            }

            // Expand the selected collection to get a list of rooms.
            System.Collections.ObjectModel.Collection<EmailAddress> roomAddresses = service.GetRooms(roomAddress);

            return roomAddresses;
        }
        #endregion

        #region 日历
        public static Collection<Appointment> AccessCalendarItemsInARecurringSeries(ExchangeService service, SearchFilter filter, ItemView view)
        {

            Collection<Appointment> collApp = new Collection<Appointment>();

            try
            {
                // Find up to the first five recurring master appointments in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return the recurring master
                // appointment.
                FindItemsResults<Item> masterResults = service.FindItems(WellKnownFolderName.Calendar, filter, view);

                foreach (Item item in masterResults.Items)
                {
                    Appointment appointment = item as Appointment;

                    collApp.Add(appointment);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return collApp;
        }

        // DateTime startDate = new DateTime(2017, 8, 30);
        //DateTime endDate = new DateTime(2017, 9, 25);
        //CalendarView calView = new CalendarView(startDate, endDate);
        public static Collection<Appointment> AccessCalendarItemsInARecurringSeries(ExchangeService service, CalendarView calView)
        {

            // Find all the appointments in the calendar based on the dates set in the CalendarView.
            // This results in a FindItem call to EWS. This will return the occurrences and exceptions
            // to a recurring series and will return appointments that are not part of a recurring series. This will not return 
            // recurring master items. Note that a search restriction or querystring cannot be used with a CalendarView.
            FindItemsResults<Item> instanceResults = service.FindItems(WellKnownFolderName.Calendar, calView);
            Collection<Appointment> collApp = new Collection<Appointment>();
            foreach (Item item in instanceResults.Items)
            {
                Appointment appointment = item as Appointment;

                collApp.Add(appointment);

            }
            return collApp;

        }
        #endregion

        /// <summary>
        /// 获取用户配置信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="emailAddress"></param>
        /// <param name="maxHops"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static GetUserSettingsResponse GetUserSettings(AutodiscoverService service, string emailAddress, int maxHops, params UserSettingName[] settings)
        {
            Uri url = null;
            GetUserSettingsResponse response = null;

            for (int attempt = 0; attempt < maxHops; attempt++)
            {
                service.Url = url;
                service.EnableScpLookup = (attempt < 2);

                response = service.GetUserSettings(emailAddress, settings);

                if (response.ErrorCode == AutodiscoverErrorCode.RedirectAddress)
                {
                    url = new Uri(response.RedirectTarget);
                }
                else if (response.ErrorCode == AutodiscoverErrorCode.RedirectUrl)
                {
                    url = new Uri(response.RedirectTarget);
                }
                else
                {
                    return response;
                }
            }

            throw new Exception("No suitable Autodiscover endpoint was found.");
        }

        public static void CreateTask(Microsoft.Exchange.WebServices.Data.Task task, WellKnownFolderName folderName)
        {
            if (folderName == null)
            {
                task.Save();
            }
            else
            {
                task.Save(WellKnownFolderName.Tasks);
            }
        }

        #region 规定
        /// <summary>
        /// Deletes all Inbox rules named MoveInterestingToJunk. Deletion to the Inbox rules 
        /// collection can be batched into a single EWS call.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void DeleteInboxRule(ExchangeService service, string displayName)
        {
            // Get all the Inbox rules in the user's mailbox. This results in a GetInboxRules operation
            // call to EWS.
            RuleCollection ruleCollection = service.GetInboxRules();

            // Inbox rule updates, including deletions, can be batched into a single call to EWS.
            Collection<RuleOperation> ruleOperations = new Collection<RuleOperation>();

            foreach (Rule rule in ruleCollection)
            {
                if (rule.DisplayName == displayName)
                {

                    DeleteRuleOperation deleteRule = new DeleteRuleOperation(rule.Id);

                    // Add each rule to deletion into a RuleOperation collection. Update operations
                    // can also be added to batch up changes to Inbox rules.
                    ruleOperations.Add(deleteRule);

                }
            }

            // The inbox rules are deleted here. This results in an UpdateInboxrules operaion call to EWS. 
            service.UpdateInboxRules(ruleOperations, true);
        }
        #endregion

        #region 遍历

        public static IList<EmailMessage> GetEamilMessages(ExchangeService service, string subSubject, int size = 50, WellKnownFolderName folername = WellKnownFolderName.Inbox)
        {
            IList<EmailMessage> messages = new List<EmailMessage>();
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, subSubject));

            // Create the search filter.
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.Or, searchFilterCollection.ToArray());

            // Create a view with a page size of 50 default.
            ItemView view = new ItemView(size);

            // Order the search results by the DateTimeReceived in descending order.
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Set the traversal to shallow. (Shallow is the default option; other options are Associated and SoftDeleted.)
            view.Traversal = ItemTraversal.Shallow;

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(folername, searchFilter, view);
            foreach (Item myItem in findResults.Items)
            {
                if (myItem is EmailMessage)
                {
                    EmailMessage message = myItem as EmailMessage;
                    message.Load(new PropertySet(
                        EmailMessageSchema.TextBody,
                        EmailMessageSchema.Body,
                        EmailMessageSchema.NormalizedBody,
                        EmailMessageSchema.UniqueBody
                        ));
                    messages.Add(message);
                }
                else
                {
                    // Else handle other item types.
                }
            }

            return messages;
        }

        #endregion
    }
}
