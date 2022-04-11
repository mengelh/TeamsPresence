using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsPresence
{
    public class TeamsPresenceConfig
    {
        public string HomeAssistantUrl { get; set; }
        public string HomeAssistantToken { get; set; }
        public string AppDataRoamingPath { get; set; }

        public string StatusEntity { get; set; }
        public string StatusEntityFriendlyName { get; set; }
        public string ActivityEntity { get; set; }
        public string ActivityEntityFriendlyName { get; set; }

        public Dictionary<TeamsStatus, string> StatusNames { get; set; }
        public Dictionary<TeamsStatus, string> StatusIcons { get; set; }
        public Dictionary<TeamsActivity, string> ActivityNames { get; set; }
        public Dictionary<TeamsActivity, string> ActivityIcons { get; set; }
    }
}
