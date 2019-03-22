using ADExtended;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Infrastructure.ActiveDirectory
{
    public static class ActiveDirectoryConnector
    {
        #region Member Variables

        private static ActiveDirectoryConfiguration _currentActiveDirectoryConfiguration = null;

        #endregion

        #region Properties

        private static ActiveDirectoryConfiguration activeDirectorySettings = null;
        public static ActiveDirectoryConfiguration ActiveDirectorySettings
        {
            get
            {
                try
                {
                    if (activeDirectorySettings == null)
                    {
                        activeDirectorySettings = (ActiveDirectoryConfiguration)ConfigurationManager.GetSection("ldapConfiguration");
                    }
                }
                catch (Exception ex)
                {
                }
                return activeDirectorySettings;
            }
        }

        #endregion

        #region Methods

        public static bool IsUserLoggedIn(string userName, string password)
        {
            try
            {
                if (ActiveDirectorySettings.Enabled)
                {
                    int startIndex = userName.IndexOf("@");
                    if (startIndex >= 0)
                    {
                        userName = userName.Substring(0, startIndex);
                    }
                    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ActiveDirectorySettings.Server + "/" + ActiveDirectorySettings.DirectoryPath, userName, password);
                    DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
                    searcher.Filter = ActiveDirectorySettings.Filter.Replace("and", "&");
                    searcher.Filter = searcher.Filter.Replace(ActiveDirectorySettings.FilterReplace, userName);
                    searcher.PropertiesToLoad.Add("memberOf");
                    searcher.PropertiesToLoad.Add("userAccountControl");

                    SearchResult directoryUser = searcher.FindOne();
                    if (directoryUser != null)
                    {
                        int flags = Convert.ToInt32(directoryUser.Properties["userAccountControl"][0].ToString());
                        if (!Convert.ToBoolean(flags & 0x0002))
                        {
                            string desiredGroupName = ActiveDirectorySettings.GroupName.ToLower();
                            if (desiredGroupName != string.Empty)
                            {
                                desiredGroupName = "cn=" + desiredGroupName + ",";
                                int numberOfGroups = directoryUser.Properties["memberOf"].Count;
                                bool isWithinGroup = false;
                                for (int i = 0; i < numberOfGroups; i++)
                                {
                                    string groupName = directoryUser.Properties["memberOf"][i].ToString().ToLower();
                                    if (groupName.Contains(desiredGroupName))
                                    {
                                        isWithinGroup = true;
                                        break;
                                    }
                                }
                                if (!isWithinGroup)
                                {
                                    throw new Exception("User [" + userName + "] is not a member of the desired group.");
                                }
                            }
                            return true;
                        }
                        else
                        {
                            throw new Exception("User [" + userName + "] is inactive.");
                        }
                    }
                    else
                    {
                        throw new Exception("User [" + userName + "] not found in the specified active directory path.");
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (LdapException ex)
            {
                if (ex.ErrorCode == 49)
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
                else
                {
                    throw new Exception("Active directory server not found.", ex);
                }
            }
            catch (DirectoryOperationException ex)
            {
                throw new Exception("Invalid active directory path.", ex);
            }
            catch (DirectoryServicesCOMException ex)
            {
                if (ex.ExtendedError == 8333)
                {
                    throw new Exception("Invalid active directory path.", ex);
                }
                else
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                throw new Exception("Active directory server not found.", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.Source == "System.DirectoryServices")
                {
                    throw new Exception("Invalid search filter expression.", ex);
                }
                else
                {
                    throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
            }
        }

        //public static void UserAuthenticationCheck()
        //{
        //    try
        //    {
        //        if (ActiveDirectorySettings.Enabled)
        //        {
        //            if ((ActiveDirectorySettings.PageLevelSecurityCheck) && !HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("login.aspx"))
        //            {
        //                if (HttpContext.Current.User != null)
        //                {
        //                    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //                    {
        //                        if (HttpContext.Current.User.Identity is FormsIdentity)
        //                        {
        //                            FormsIdentity formIdentity = (FormsIdentity)HttpContext.Current.User.Identity;
        //                            FormsAuthenticationTicket userAuthTicket = formIdentity.Ticket;
        //                            if (!IsUserLoggedIn(userAuthTicket.Name, userAuthTicket.UserData))
        //                            {
        //                                FormsAuthentication.SignOut();
        //                                FormsAuthentication.RedirectToLoginPage();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        FormsAuthentication.SignOut();
        //        FormsAuthentication.RedirectToLoginPage();
        //    }
        //}

        public static string GetDepartmentByUser(string userName, string password)
        {
            string department = string.Empty;
            try
            {
                if (ActiveDirectorySettings.Enabled)
                {

                    int startIndex = userName.IndexOf("@");
                    if (startIndex >= 0)
                    {
                        userName = userName.Substring(0, startIndex);
                    }
                    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ActiveDirectorySettings.Server + "/" + ActiveDirectorySettings.DirectoryPath, userName, password);
                    DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
                    searcher.Filter = ActiveDirectorySettings.Filter.Replace("and", "&");
                    searcher.Filter = searcher.Filter.Replace(ActiveDirectorySettings.FilterReplace, userName);
                    searcher.PropertiesToLoad.Add("department");
                    searcher.PropertiesToLoad.Add("userAccountControl");

                    SearchResult directoryUser = searcher.FindOne();
                    if (directoryUser != null)
                    {
                        int flags = Convert.ToInt32(directoryUser.Properties["userAccountControl"][0].ToString());
                        if (!Convert.ToBoolean(flags & 0x0002))
                        {
                            department = directoryUser.Properties["department"][0].ToString();

                            //string desiredGroupName = ActiveDirectorySettings.GroupName.ToLower();
                            //if (desiredGroupName != string.Empty)
                            //{
                            ////    desiredGroupName = "cn=" + desiredGroupName + ",";
                            //    int numberOfGroups = directoryUser.Properties["memberOf"].Count;
                            ////    bool isWithinGroup = false;
                            //    for (int i = 0; i < numberOfGroups; i++)
                            //    {
                            //        department = directoryUser.Properties["memberOf"][i].ToString().ToLower() + ",";
                            ////        if (groupName.Contains(desiredGroupName))
                            ////        {
                            ////            isWithinGroup = true;
                            ////            break;
                            ////        }
                            //    }
                            ////    if (!isWithinGroup)
                            ////    {
                            //        throw new Exception("User [" + userName + "] is not a member of the desired group.");
                            //    }
                            //}
                            return department;
                        }
                        else
                        {
                            throw new Exception("User [" + userName + "] is inactive.");
                        }
                    }
                    else
                    {
                        throw new Exception("User [" + userName + "] not found in the specified active directory path.");
                    }
                }
                else
                {
                    return department;
                }
            }
            catch (LdapException ex)
            {
                if (ex.ErrorCode == 49)
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
                else
                {
                    throw new Exception("Active directory server not found.", ex);
                }
            }
            catch (DirectoryOperationException ex)
            {
                throw new Exception("Invalid active directory path.", ex);
            }
            catch (DirectoryServicesCOMException ex)
            {
                if (ex.ExtendedError == 8333)
                {
                    throw new Exception("Invalid active directory path.", ex);
                }
                else
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                throw new Exception("Active directory server not found.", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.Source == "System.DirectoryServices")
                {
                    throw new Exception("Invalid search filter expression.", ex);
                }
                else
                {
                    throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
            }
        }
        public static string GetEmailByUser(string userName, string password)
        {
            string email = string.Empty;
            try
            {
                if (ActiveDirectorySettings.Enabled)
                {

                    int startIndex = userName.IndexOf("@");
                    if (startIndex >= 0)
                    {
                        userName = userName.Substring(0, startIndex);
                    }
                    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ActiveDirectorySettings.Server + "/" + ActiveDirectorySettings.DirectoryPath, userName, password);
                    DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
                    searcher.Filter = ActiveDirectorySettings.Filter.Replace("and", "&");
                    searcher.Filter = searcher.Filter.Replace(ActiveDirectorySettings.FilterReplace, userName);
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("userAccountControl");

                    SearchResult directoryUser = searcher.FindOne();
                    if (directoryUser != null)
                    {
                        int flags = Convert.ToInt32(directoryUser.Properties["userAccountControl"][0].ToString());
                        if (!Convert.ToBoolean(flags & 0x0002))
                        {
                            email = directoryUser.Properties["mail"][0].ToString();

                            //string desiredGroupName = ActiveDirectorySettings.GroupName.ToLower();
                            //if (desiredGroupName != string.Empty)
                            //{
                            ////    desiredGroupName = "cn=" + desiredGroupName + ",";
                            //    int numberOfGroups = directoryUser.Properties["memberOf"].Count;
                            ////    bool isWithinGroup = false;
                            //    for (int i = 0; i < numberOfGroups; i++)
                            //    {
                            //        department = directoryUser.Properties["memberOf"][i].ToString().ToLower() + ",";
                            ////        if (groupName.Contains(desiredGroupName))
                            ////        {
                            ////            isWithinGroup = true;
                            ////            break;
                            ////        }
                            //    }
                            ////    if (!isWithinGroup)
                            ////    {
                            //        throw new Exception("User [" + userName + "] is not a member of the desired group.");
                            //    }
                            //}
                            return email;
                        }
                        else
                        {
                            throw new Exception("User [" + userName + "] is inactive.");
                        }
                    }
                    else
                    {
                        throw new Exception("User [" + userName + "] not found in the specified active directory path.");
                    }
                }
                else
                {
                    return email;
                }
            }
            catch (LdapException ex)
            {
                if (ex.ErrorCode == 49)
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
                else
                {
                    throw new Exception("Active directory server not found.", ex);
                }
            }
            catch (DirectoryOperationException ex)
            {
                throw new Exception("Invalid active directory path.", ex);
            }
            catch (DirectoryServicesCOMException ex)
            {
                if (ex.ExtendedError == 8333)
                {
                    throw new Exception("Invalid active directory path.", ex);
                }
                else
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                throw new Exception("Active directory server not found.", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.Source == "System.DirectoryServices")
                {
                    throw new Exception("Invalid search filter expression.", ex);
                }
                else
                {
                    throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
            }
        }
        public static string GetGroupByUser(string userName, string password)
        {
            StringBuilder group = new StringBuilder();
            try
            {
                if (ActiveDirectorySettings.Enabled)
                {

                    int startIndex = userName.IndexOf("@");
                    if (startIndex >= 0)
                    {
                        userName = userName.Substring(0, startIndex);
                    }
                    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ActiveDirectorySettings.Server + "/" + ActiveDirectorySettings.DirectoryPath, userName, password);
                    DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
                    searcher.Filter = ActiveDirectorySettings.Filter.Replace("and", "&");
                    searcher.Filter = searcher.Filter.Replace(ActiveDirectorySettings.FilterReplace, userName);
                    searcher.PropertiesToLoad.Add("memberof");
                    searcher.PropertiesToLoad.Add("userAccountControl");

                    SearchResult directoryUser = searcher.FindOne();
                    if (directoryUser != null)
                    {
                        int flags = Convert.ToInt32(directoryUser.Properties["userAccountControl"][0].ToString());
                        if (!Convert.ToBoolean(flags & 0x0002))
                        {
                            int propertyCount = directoryUser.Properties["memberOf"].Count;
                            string dn;
                            int equalsIndex, commaIndex;
                            for (int i = 0; i < propertyCount; i++)
                            {
                                dn = (string)directoryUser.Properties["memberOf"][i];
                                equalsIndex = dn.IndexOf("=", 1);
                                commaIndex = dn.IndexOf(",", 1);
                                if (-1 == equalsIndex)
                                {
                                    return null;
                                }
                                group.Append("</br>" + dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                            }
                            return group.ToString();
                        }
                        else
                        {
                            throw new Exception("User [" + userName + "] is inactive.");
                        }
                    }
                    else
                    {
                        throw new Exception("User [" + userName + "] not found in the specified active directory path.");
                    }
                }
                else
                {
                    return group.ToString();
                }
            }
            catch (LdapException ex)
            {
                if (ex.ErrorCode == 49)
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
                else
                {
                    throw new Exception("Active directory server not found.", ex);
                }
            }
            catch (DirectoryOperationException ex)
            {
                throw new Exception("Invalid active directory path.", ex);
            }
            catch (DirectoryServicesCOMException ex)
            {
                if (ex.ExtendedError == 8333)
                {
                    throw new Exception("Invalid active directory path.", ex);
                }
                else
                {
                    throw new Exception("Invalid user authentication. Please input a valid user name & pasword and try again.", ex);
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                throw new Exception("Active directory server not found.", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.Source == "System.DirectoryServices")
                {
                    throw new Exception("Invalid search filter expression.", ex);
                }
                else
                {
                    throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandeled exception occured while authenticating user using active directory.", ex);
            }
        }
        public static string GetUserTitle(string userName)
        {
            int startIndex = userName.IndexOf("@");
            if (startIndex >= 0)
            {
                userName = userName.Substring(0, startIndex);
            }
            string title = string.Empty;
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "crownbio.com"))
            {
                // Search the directory for the new object. 
                UserPrincipalEx myUser = UserPrincipalEx.FindByIdentity(ctx, userName);

                if (myUser != null)
                {
                    // get the title which is now available on your "myUser" object!
                    title = myUser.Title;
                    if (!string.IsNullOrEmpty(title))
                        title = title.ToLower().Trim();
                }
            }
            return title;
        }

        public static string GetUserCompany(string userName)
        {
            int startIndex = userName.IndexOf("@");
            if (startIndex >= 0)
            {
                userName = userName.Substring(0, startIndex);
            }
            string company = string.Empty;
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "crownbio.com"))
            {
                // Search the directory for the new object. 
                UserPrincipalEx myUser = UserPrincipalEx.FindByIdentity(ctx, userName);

                if (myUser != null)
                {
                    // get the title which is now available on your "myUser" object!
                    company = myUser.Company;
                    if (!string.IsNullOrEmpty(company))
                        company = company.ToLower().Trim();
                }
            }
            return company;
        }

        public static List<string> GetUsersByGroup(string groupname)
        {
            List<string> list = new List<string>();
            using (var context = new PrincipalContext(ContextType.Domain, "crownbio.com"))
            {
                using (var group = GroupPrincipal.FindByIdentity(context, groupname))
                {
                    if (group == null)
                    {

                    }
                    else
                    {
                        var users = group.GetMembers(true);
                        foreach (UserPrincipal user in users)
                        {
                            list.Add(user.EmailAddress);
                        }
                    }
                }
            }

            return list;
        }

        public static string GetOU(string username)
        {
            string result = string.Empty;
            using (HostingEnvironment.Impersonate())
            {
                //Getting the domain
                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

                //Finding the user
                UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, username);

                //If the user found
                if (user != null)
                {
                    // Getting the DirectoryEntry
                    DirectoryEntry directoryEntry = (user.GetUnderlyingObject() as DirectoryEntry);
                    //if the directoryEntry is not null
                    if (directoryEntry != null)
                    {
                        //Getting the directoryEntry's path and spliting with the "," character
                        string[] directoryEntryPath = directoryEntry.Path.Split(',');
                        //Getting the each items of the array and spliting again with the "=" character
                        foreach (var splitedPath in directoryEntryPath)
                        {
                            string[] eleiments = splitedPath.Split('=');
                            //If the 1st element of the array is "OU" string then get the 2dn element
                            if (eleiments[0].Trim() == "OU")
                            {
                                result = username + "-" + eleiments[1].Trim();
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static List<string> GetUsersByDepartment(string department)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("OU=IT,OU=CBTC Users,DC=crownbio,DC=com", department);
            string ou = sb.ToString();
            List<string> list = new List<string>();
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "crownbio.com", ou))
            {
                // define the "query-by-example" user (or group, or computer) for your search
                UserPrincipal qbeUser = new UserPrincipal(ctx);

                // define a searcher for that context and that query-by-example 
                using (PrincipalSearcher searcher = new PrincipalSearcher(qbeUser))
                {
                    foreach (Principal p in searcher.FindAll())
                    {
                        // Convert the "generic" Principal to a UserPrincipal
                        UserPrincipal user = p as UserPrincipal;

                        if (user != null)
                        {
                            // do something with your found user....
                        }
                    }
                }
            }
            return list;
        }

        public static List<string> GetAllActiveDirectoryGroups()
        {
            List<string> groups = new List<string>();
            using (var context = new PrincipalContext(ContextType.Domain, "crownbio.com"))
            {
                using (var group = new GroupPrincipal(context))
                {
                    if (group == null)
                    {

                    }
                    else
                    {
                        group.IsSecurityGroup = false;
                        PrincipalSearcher srch = new PrincipalSearcher(group);

                        // find all matches
                        foreach (var found in srch.FindAll())
                        {
                            if (!string.IsNullOrEmpty(found.DisplayName))
                            {
                                groups.Add(found.DisplayName);
                            }
                        }
                    }
                }
            }

            return groups;
        }

        public static List<string> GetAllDepartments(string name, string password)
        {
            List<string> dict = new List<string>();
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ActiveDirectorySettings.Server + "/" + ActiveDirectorySettings.DirectoryPath, name, password);
            DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
            searcher.Filter = "(objectClass=user)";
            searcher.PropertiesToLoad.Add("department");
            SearchResultCollection results = searcher.FindAll();
            foreach (SearchResult result in results)
            {
                string dept = String.Empty;
                DirectoryEntry de = result.GetDirectoryEntry();
                if (de.Properties.Contains("department"))
                {
                    dept = de.Properties["department"][0].ToString();
                    if (!dict.Contains(dept))
                    {
                        dict.Add(result.Properties["department"][0].ToString());
                    }
                }
            }
            return dict;
        }

        #endregion

        #region helper method

        public static string GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
