using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ActiveDirectory
{
    public class ActiveDirectoryConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("enabled", DefaultValue = "true", IsRequired = true)]
        public Boolean Enabled
        {
            get
            {
                return (Boolean)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("server", DefaultValue = "", IsRequired = true)]
        public string Server
        {
            get
            {
                return this["server"].ToString();
            }

            set
            {
                this["server"] = value;
            }
        }

        [ConfigurationProperty("domain", DefaultValue = "", IsRequired = true)]
        public string Domain
        {
            get
            {
                return this["domain"].ToString();
            }

            set
            {
                this["domain"] = value;
            }
        }

        [ConfigurationProperty("directoryPath", DefaultValue = "", IsRequired = true)]
        public string DirectoryPath
        {
            get
            {
                return this["directoryPath"].ToString();
            }

            set
            {
                this["directoryPath"] = value;
            }
        }

        [ConfigurationProperty("groupName", DefaultValue = "", IsRequired = true)]
        public string GroupName
        {
            get
            {
                return this["groupName"].ToString();
            }

            set
            {
                this["groupName"] = value;
            }
        }

        [ConfigurationProperty("filter", DefaultValue = "", IsRequired = true)]
        public string Filter
        {
            get
            {
                return this["filter"].ToString();
            }

            set
            {
                this["filter"] = value;
            }
        }

        [ConfigurationProperty("filterReplace", DefaultValue = "", IsRequired = true)]
        public string FilterReplace
        {
            get
            {
                return this["filterReplace"].ToString();
            }

            set
            {
                this["filterReplace"] = value;
            }
        }

        [ConfigurationProperty("pageLevelSecurityCheck", DefaultValue = "false", IsRequired = true)]
        public Boolean PageLevelSecurityCheck
        {
            get
            {
                return (Boolean)this["pageLevelSecurityCheck"];
            }
            set
            {
                this["pageLevelSecurityCheck"] = value;
            }
        }
    }
}
