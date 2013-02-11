using System;
using System.IO;
using System.Net;
using Loki.Data;
using Loki.IO;
using Loki.Maple;
using MySql.Data.MySqlClient;
using Loki.Net;

namespace Loki
{
    public static class LoginServerSetup
    {
        public static void Run()
        {
            Log.Entitle("Login Server Setup");

            Log.Inform("If you do not know a value, leave the field blank to apply default.");

            Log.Entitle("Database Setup");

            string databaseHost = string.Empty;
            string databaseSchema = string.Empty;
            string databaseUsername = string.Empty;
            string databasePassword = string.Empty;

        databaseConfiguration:

            Log.Inform("Please enter your database credentials: ");

            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("Host: ", "localhost");
                databaseSchema = Log.Input("Database: ", "login");
                databaseUsername = Log.Input("Username: ", "root");
                databasePassword = Log.Input("Password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();
                    Database.Fetch("accounts", "Username", "ID = '{0}'", 1);
                }
            }
            catch (MySqlException e)
            {
                Log.SkipLine();

                Log.Error(e);

                Log.SkipLine();

                if ((e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the " + databaseSchema + " database? ", true)) || (e.Message.Contains("Table") && Log.YesNo("Populate the " + databaseSchema + " database? ", true)))
                {
                    Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE IF NOT EXISTS {0};
							USE {0};

							SET FOREIGN_KEY_CHECKS=0;

							DROP TABLE IF EXISTS `accounts`;
							CREATE TABLE `accounts` (
							  `ID` int(10) NOT NULL AUTO_INCREMENT,
							  `Username` varchar(12) NOT NULL,
							  `Password` varchar(128) NOT NULL,
							  `Salt` varchar(32) NOT NULL,
							  `Pin` varchar(64) NOT NULL DEFAULT '',
							  `Pic` varchar(26) DEFAULT NULL,
							  `IsLoggedIn` tinyint(1) unsigned NOT NULL,
							  `IsBanned` tinyint(1) unsigned NOT NULL,
							  `IsMaster` tinyint(1) unsigned NOT NULL,
							  `Birthday` date NOT NULL,
							  `Creation` datetime NOT NULL,
							  `MaplePoints` int(10) NOT NULL DEFAULT '0',
							  `PaypalNX` int(10) NOT NULL DEFAULT '0',
							  `CardNX` int(10) NOT NULL DEFAULT '0',
							  PRIMARY KEY (`ID`),
							  KEY `username` (`Username`) USING BTREE
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `banned_ip`;
							CREATE TABLE `banned_ip` (
							  `Address` varchar(15) NOT NULL,
							  PRIMARY KEY (`Address`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `banned_mac`;
							CREATE TABLE `banned_mac` (
							  `Address` varchar(17) NOT NULL,
							  PRIMARY KEY (`Address`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `master_ip`;
							CREATE TABLE `master_ip` (
							  `IP` varchar(15) NOT NULL,
							  PRIMARY KEY (`IP`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							INSERT INTO master_ip VALUES ('127.0.0.1');
						", databaseSchema);

                    Log.Inform("Database '{0}' created.", databaseSchema);
                }
                else
                {
                    goto databaseConfiguration;
                }
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Row"))
                {
                    Log.SkipLine();

                    goto databaseConfiguration;
                }
            }

            Log.SkipLine();

            Log.Success("Database configured!");

            Log.Entitle("Server Configuration");

            string securityCode = Log.Input("Assign a security code between servers: ", "");
            bool requireStaffIP = Log.YesNo("Require staff to connect through specific IPs? ", true);
            bool autoRegister = Log.YesNo("Allow players to register in-game? ", true);
            bool requestPin = Log.YesNo("Require players to enter PIN on login? ", false);
            bool requestPic = Log.YesNo("Require players to enter PIC on character selection? ", false);
            int maxCharacters = Log.Input("Maximum characters per account: ", 3);

            Log.SkipLine();

            Log.Success("Server configured!");

            Log.Entitle("World Configuration");

            bool configuredWorld = true;

            int WorldExperienceRate = 1;
            int WorldQuestExperienceRate = 1;
            int WorldPartyQuestExperienceRate = 1;
            int WorldMesoDropRate = 1;
            int WorldItemDropRate = 1;
            string WorldName = string.Empty;
            string WorldRecommendedMessage = string.Empty;
            ServerFlag WorldFlag = ServerFlag.None;
            IPAddress WorldIP = IPAddress.Loopback;

            if (Log.YesNo("Skip World configuration (not recommended)? ", false))
            {
                configuredWorld = false;
                goto userProfile;
            }

            Log.SkipLine();
            Log.Inform("Please enter the basic details: ");

            WorldName = string.Empty;

            do
            {
                WorldName = Log.Input("World name (examples: Bera, Khaini): ", "Scania");
            }
            while (!WorldNameResolver.IsValid(WorldName));

            WorldIP = Log.Input("Host IP (external for remote only): ", IPAddress.Loopback);
            WorldRecommendedMessage = Log.Input("World recommended message (leave blank if you don't want): ", "");

            Log.SkipLine();
            Log.Inform("Please specify the World rates: ");

            WorldExperienceRate = Log.Input("Normal experience: ", 1);
            WorldQuestExperienceRate = Log.Input("Quest experience: ", 1);
            WorldPartyQuestExperienceRate = Log.Input("Party quest experience: ", 1);
            WorldMesoDropRate = Log.Input("Meso drop: ", 1);
            WorldItemDropRate = Log.Input("Item drop: ", 1);

            Log.SkipLine();

            Log.Inform("Which flag should be shown with this World?\n  None\n  New\n  Hot\n  Event");

        inputFlag:
            Log.SkipLine();
            try
            {
                WorldFlag = (ServerFlag)Enum.Parse(typeof(ServerFlag), Log.Input("World flag: ", "None"));
            }
            catch
            {
                goto inputFlag;
            }

            Log.SkipLine();

            Log.Success("World '{0}' configured!", WorldName);

        userProfile:
            Log.Inform("Please choose what information to display.\n  A. Hide packets (recommended)\n  B. Show names\n  C. Show content");
            Log.SkipLine();

            LogLevel logLevel;

        multipleChoice:
            switch (Log.Input("Please enter yours choice: ", "Hide").ToLower())
            {
                case "a":
                case "hide":
                    logLevel = LogLevel.None;
                    break;

                case "b":
                case "names":
                    logLevel = LogLevel.Name;
                    break;

                case "c":
                case "content":
                    logLevel = LogLevel.Full;
                    break;

                default:
                    goto multipleChoice;
            }

            Log.Entitle("Please wait...");

            Log.Inform("Applying settings to 'Configuration.ini'...");

            string lines = string.Format(
                @"[Log]
				Packets={0}
				StackTrace=False
				LoadTime=False
				PacketRecord=[recive/send, ExactPacketName]
				
				[Server]
				Port=8484
				AutoRegister={1}
				RequestPin={2}
				RequestPic={3}
				MaxCharacters={4}
				RequireStaffIP={5}
				
				[Channels]
				Port=8540
				SecurityCode={6}
				
				[Database]
				Host={7}
				Schema={8}
				Username={9}
				Password={10}",
                logLevel, autoRegister, requestPin, requestPic, 
                maxCharacters, requireStaffIP, securityCode, databaseHost, databaseSchema,
                databaseUsername, databasePassword).Replace("	", "");

            if (configuredWorld)
            {
                lines += string.Format(@"
				
				[{0}]
				HostIP={1}
				RecommendedMessage={2}
				StaffOnly=False
				Flag={3}
				ExperienceRate={4}
				QuestExperienceRate={5}
				PartyQuestExperienceRate={6}
				MesoDropRate={7}
				ItemDropRate={8}",
                WorldName, WorldIP, WorldRecommendedMessage, WorldFlag, WorldExperienceRate, WorldQuestExperienceRate,
                WorldPartyQuestExperienceRate, WorldMesoDropRate, WorldItemDropRate).Replace("	", "");
            }

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "Configuration.ini"))
            {
                file.WriteLine(lines);
            }

            Log.Success("Configuration done!");
        }
    }
}
