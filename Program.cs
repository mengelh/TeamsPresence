using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamsPresence
{
    class Program
    {
        private static HomeAssistantService HomeAssistantService;
        private static TeamsLogService TeamsLogService;
        private static TeamsPresenceConfig Config;

        static void Main(string[] args)
        {
            var configFile = "config.json";

            if (File.Exists(configFile))
            {
                //Console.WriteLine("Config file found!");

                try
                {
                    Config = JsonConvert.DeserializeObject<TeamsPresenceConfig>(File.ReadAllText(configFile));
                }
                catch
                {
                    Console.WriteLine("Config file could not be used. Either fix it or delete it and restart this application to scaffold a new one.");
                }
            }
            else
            {
                Console.WriteLine("Config file doesn't exist. Creating...");

                Config = new TeamsPresenceConfig()
                {
                    HomeAssistantUrl = "https://yourha.duckdns.org",
                    HomeAssistantToken = "eyJ0eXAiOiJKV1...",
                    AppDataRoamingPath = "",
                    StatusEntity = "sensor.teams_status",
                    StatusEntityFriendlyName = "Teams Status",
                    ActivityEntity = "sensor.teams_activity",
                    ActivityEntityFriendlyName = "Teams Activity",
                    StatusNames = new Dictionary<TeamsStatus, string>()
                    {
                        { TeamsStatus.Available, "Available" },
                        { TeamsStatus.Busy, "Busy" },
                        { TeamsStatus.OnThePhone, "On the phone" },
                        { TeamsStatus.Away, "Away" },
                        { TeamsStatus.BeRightBack, "Be right back" },
                        { TeamsStatus.DoNotDisturb, "Do not disturb" },
                        { TeamsStatus.Presenting, "Presenting" },
                        { TeamsStatus.Focusing, "Focusing" },
                        { TeamsStatus.InAMeeting, "In a meeting" },
                        { TeamsStatus.Offline, "Offline" },
                        { TeamsStatus.Unknown, "Unknown" }
                    },
                    StatusIcons = new Dictionary<TeamsStatus, string>()
                    {
                        { TeamsStatus.Available, "mdi:checkbox-marked-circle" },
                        { TeamsStatus.Busy, "mdi:checkbox-blank-circle" },
                        { TeamsStatus.OnThePhone, "mdi:minus-circle" },
                        { TeamsStatus.Away, "mdi:checkbox-blank-circle-outline" },
                        { TeamsStatus.BeRightBack, "mdi:checkbox-blank-circle-outline" },
                        { TeamsStatus.DoNotDisturb, "mdi:minus-circle" },
                        { TeamsStatus.Presenting, "mdi:minus-circle" },
                        { TeamsStatus.Focusing, " mdi:checkbox-blank-circle" },
                        { TeamsStatus.InAMeeting, " mdi:checkbox-blank-circle" },
                        { TeamsStatus.Offline, "mdi:cloud-outline-off" },
                        { TeamsStatus.Unknown, "mdi:cloud-outline-off" }
                    },
                    ActivityNames = new Dictionary<TeamsActivity, string>()
                    {
                        { TeamsActivity.InACall, "In a call" },
                        { TeamsActivity.NotInACall, "Not in a call" },
                        { TeamsActivity.Unknown, "Unknown" }
                    },
                    ActivityIcons = new Dictionary<TeamsActivity, string>()
                    {
                        { TeamsActivity.InACall, "mdi:phone-in-talk-outline" },
                        { TeamsActivity.NotInACall, "mdi:phone-off" },
                        { TeamsActivity.Unknown, "mdi:phone-cancel" }
                    }
                };

                File.WriteAllText(configFile, JsonConvert.SerializeObject(Config, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                }));

                Console.WriteLine("Done. Fill out the config file and restart this application.");

                return;
            }

            if (!String.IsNullOrWhiteSpace(Config.AppDataRoamingPath))
            {
                TeamsLogService = new TeamsLogService(Config.AppDataRoamingPath);
            }
            else
            {
                TeamsLogService = new TeamsLogService();
            }

            HomeAssistantService = new HomeAssistantService(Config.HomeAssistantUrl, Config.HomeAssistantToken);

            TeamsLogService.StatusChanged += Service_StatusChanged;
            TeamsLogService.ActivityChanged += Service_ActivityChanged;

            Console.WriteLine("Service started. Waiting for Teams updates...");

            TeamsLogService.Start();
        }

        private static void Service_StatusChanged(object sender, TeamsStatus status)
        {
            HomeAssistantService.UpdateEntity(Config.StatusEntity, Config.StatusNames[status], Config.StatusIcons[status], Config.StatusEntityFriendlyName);
            Console.WriteLine($"Updated status to {Config.StatusNames[status]} ({status})");
        }

        private static void Service_ActivityChanged(object sender, TeamsActivity activity)
        {
            HomeAssistantService.UpdateEntity(Config.ActivityEntity, Config.ActivityNames[activity], Config.ActivityIcons[activity], Config.ActivityEntityFriendlyName);

            Console.WriteLine($"Updated activity to {Config.ActivityNames[activity]} ({activity})");
        }
    }
}
