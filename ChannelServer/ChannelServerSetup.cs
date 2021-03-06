﻿using System;
using System.IO;
using System.Net;
using Loki.Data;
using Loki.IO;
using Loki.Maple;
using MySql.Data.MySqlClient;

namespace Loki
{
    public static class ChannelServerSetup
    {
        private const string McdbFileName = "mcdb-4.3-111.sql";

        public static void Run()
        {
            Log.Entitle("Channel Server Setup");

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
                databaseSchema = Log.Input("Schema: ", "channel");
                databaseUsername = Log.Input("Username: ", "root");
                databasePassword = Log.Input("Password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();
                    Database.Fetch("characters", "AccountID", "ID = '{0}'", 1);
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

							DROP TABLE IF EXISTS `characters`;
							CREATE TABLE `characters` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `AccountID` int(11) NOT NULL,
							  `Name` varchar(13) NOT NULL,
							  `Level` tinyint(3) unsigned NOT NULL,
							  `Experience` int(11) NOT NULL DEFAULT '0',
							  `Job` smallint(6) NOT NULL DEFAULT '0',
							  `Strength` smallint(6) NOT NULL,
							  `Dexterity` smallint(6) NOT NULL,
							  `Luck` smallint(6) NOT NULL,
							  `Intelligence` smallint(6) NOT NULL,
							  `CurrentHP` smallint(6) NOT NULL,
							  `MaxHP` smallint(6) NOT NULL,
							  `CurrentMP` smallint(6) NOT NULL,
							  `MaxMP` smallint(6) NOT NULL,
							  `Meso` int(10) NOT NULL DEFAULT '0',
							  `Fame` smallint(6) NOT NULL DEFAULT '0',
							  `Gender` tinyint(3) unsigned NOT NULL DEFAULT '0',
							  `Hair` int(11) NOT NULL,
							  `Skin` tinyint(3) unsigned NOT NULL DEFAULT '0',
							  `Face` int(11) NOT NULL,
							  `AvailableAP` smallint(6) NOT NULL DEFAULT '0',
							  `AvailableSP` smallint(6) NOT NULL DEFAULT '0',
							  `MapID` int(11) NOT NULL DEFAULT '0',
							  `SpawnPoint` tinyint(3) unsigned NOT NULL DEFAULT '0',
							  `MaxBuddies` tinyint(3) unsigned NOT NULL DEFAULT '20',
							  `SpecialJob` enum('0', '1', '2') NOT NULL,
							  `DemonMark` int(11) NOT NULL DEFAULT '0',
							  `EquipmentSlots` tinyint(3) unsigned NOT NULL DEFAULT '24',
							  `UsableSlots` tinyint(3) unsigned NOT NULL DEFAULT '48',
							  `SetupSlots` tinyint(3) unsigned NOT NULL DEFAULT '24',
							  `EtceteraSlots` tinyint(3) unsigned NOT NULL DEFAULT '24',
							  PRIMARY KEY (`ID`),
							  KEY `account_id` (`AccountID`),
							  KEY `name` (`Name`) USING BTREE
							) ENGINE=InnoDB AUTO_INCREMENT=100000 DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `items`;
							CREATE TABLE `items` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(10) NOT NULL,
							  `MapleID` int(11) NOT NULL,
							  `Slot` tinyint(4) NOT NULL,
							  `Creator` varchar(13) NOT NULL,
							  `UpgradesAvailable` tinyint(3) unsigned NOT NULL,
							  `UpgradesApplied` tinyint(3) unsigned NOT NULL,
							  `Strength` smallint(6) NOT NULL,
							  `Dexterity` smallint(6) NOT NULL,
							  `Intelligence` smallint(6) NOT NULL,
							  `Luck` smallint(6) NOT NULL,
							  `HP` smallint(6) NOT NULL,
							  `MP` smallint(6) NOT NULL,
							  `WeaponAttack` smallint(6) NOT NULL,
							  `MagicAttack` smallint(6) NOT NULL,
							  `WeaponDefense` smallint(6) NOT NULL,
							  `MagicDefense` smallint(6) NOT NULL,
							  `Accuracy` smallint(6) NOT NULL,
							  `Avoidability` smallint(6) NOT NULL,
							  `Agility` smallint(6) NOT NULL,
							  `Speed` smallint(6) NOT NULL,
							  `Jump` smallint(6) NOT NULL,
							  `IsScisored` tinyint(1) unsigned NOT NULL,
							  `PreventsSlipping` tinyint(1) unsigned NOT NULL,
							  `PreventsColdness` tinyint(1) unsigned NOT NULL,
							  `IsStored` tinyint(1) unsigned NOT NULL,
							  `Quantity` smallint(6) NOT NULL,
							  `ViciousHammerApplied` tinyint(3) unsigned NOT NULL,
							  `Potential` tinyint(3) unsigned NOT NULL,
							  `Stars` tinyint(3) unsigned NOT NULL,
							  `Potential1` smallint(6) NOT NULL,
							  `Potential2` smallint(6) NOT NULL,
							  `Potential3` smallint(6) NOT NULL,
							  `PotentialLines` tinyint(3) unsigned NOT NULL,
							  `SerialNumber` int(11) NOT NULL,
							  `UniqueID` int(11) NOT NULL,
							  PRIMARY KEY (`ID`),
							  KEY `character_id` (`CharacterID`) USING BTREE
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `quests_completed`;
							CREATE TABLE `quests_completed` (
							  `CharacterID` int(11) NOT NULL,
							  `QuestID` smallint(6) unsigned NOT NULL,
							  `CompletionTime` datetime NOT NULL,
							  UNIQUE KEY `Quest` (`CharacterID`,`QuestID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `quests_started`;
							CREATE TABLE `quests_started` (
							  `CharacterID` int(11) NOT NULL,
							  `QuestID` smallint(6) unsigned NOT NULL,
							  `MobID` int(11) DEFAULT NULL,
							  `Killed` smallint(6) DEFAULT NULL,
							  UNIQUE KEY `QuestRequirement` (`CharacterID`,`QuestID`,`MobID`) USING BTREE
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `skills`;
							CREATE TABLE  `skills` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `MapleID` int(11) NOT NULL,
							  `CurrentLevel` tinyint(3) unsigned NOT NULL,
							  `MaxLevel` tinyint(3) unsigned NOT NULL,
							  `CooldownEnd` datetime NOT NULL,
							  PRIMARY KEY (`ID`),
							  KEY `character_id` (`CharacterID`) USING BTREE
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `buffs`;
							CREATE TABLE  `buffs` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `Type` tinyint(3) unsigned NOT NULL,
							  `MapleID` int(11) NOT NULL,
							  `SkillLevel` int(11) NOT NULL,
							  `Value` int(11) NOT NULL,
							  `End` datetime NOT NULL,
							  PRIMARY KEY (`id`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `keymaps`;
							CREATE TABLE  `keymaps` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `KeyID` int(11) NOT NULL,
							  `Type` tinyint(3) unsigned NOT NULL,
							  `Action` int(11) NOT NULL,
							  PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `buddies`;
							CREATE TABLE `buddies` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `BuddyID` int(11) NOT NULL,
							  `Pending` tinyint(4) NOT NULL DEFAULT '0',
							  `GroupName` varchar(13) NOT NULL DEFAULT 'Default Group',
							  PRIMARY KEY (`ID`),
							KEY `groupname` (`GroupName`) USING BTREE
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `sptables`;
							CREATE TABLE `sptables` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `Advancement` tinyint(3) unsigned NOT NULL,
							  `AvailableSP` tinyint(3) unsigned NOT NULL DEFAULT '0',
							  PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `wishlists`;
							CREATE TABLE  `wishlists` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `CharacterID` int(11) NOT NULL,
							  `SerialNumber` int(11) NOT NULL,
							  PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `cash_inventory`;
							CREATE TABLE  `cash_inventory` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `AccountID` int(11) NOT NULL,
							  `SerialNumber` int(11) NOT NULL,
							  `UniqueID` int(11) NOT NULL,
							  `Quantity` smallint(6) NOT NULL,
							  PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `uniqueids`;
							CREATE TABLE `uniqueids` (
							  `ID` int(11) NOT NULL DEFAULT '0'
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;
							INSERT INTO `uniqueids` VALUES (0);

							DROP TABLE IF EXISTS `storages`;
							CREATE TABLE  `storages` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `AccountID` int(11) NOT NULL,
 							 `Slots` tinyint(3) unsigned NOT NULL DEFAULT '4',
 							 `Meso` int(11) NOT NULL DEFAULT '0',
 							 PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							DROP TABLE IF EXISTS `storage_items`;
							CREATE TABLE  `storage_items` (
							  `ID` int(11) NOT NULL AUTO_INCREMENT,
							  `AccountID` int(11) NOT NULL,
							  `MapleID` int(11) NOT NULL,
							  `Slot` tinyint(4) NOT NULL,
							  `Creator` varchar(13) NOT NULL,
							  `UpgradesAvailable` tinyint(3) unsigned NOT NULL,
							  `UpgradesApplied` tinyint(3) unsigned NOT NULL,
							  `Strength` smallint(6) NOT NULL,
							  `Dexterity` smallint(6) NOT NULL,
							  `Intelligence` smallint(6) NOT NULL,
							  `Luck` smallint(6) NOT NULL,
							  `HP` smallint(6) NOT NULL,
							  `MP` smallint(6) NOT NULL,
							  `WeaponAttack` smallint(6) NOT NULL,
							  `MagicAttack` smallint(6) NOT NULL,
							  `WeaponDefense` smallint(6) NOT NULL,
							  `MagicDefense` smallint(6) NOT NULL,
							  `Accuracy` smallint(6) NOT NULL,
							  `Avoidability` smallint(6) NOT NULL,
							  `Agility` smallint(6) NOT NULL,
							  `Speed` smallint(6) NOT NULL,
							  `Jump` smallint(6) NOT NULL,
							  `IsScisored` tinyint(1) unsigned NOT NULL,
							  `PreventsSlipping` tinyint(1) unsigned NOT NULL,
							  `PreventsColdness` tinyint(1) unsigned NOT NULL,
							  `IsStored` tinyint(1) unsigned NOT NULL,
							  `Quantity` smallint(6) NOT NULL,
							  `ViciousHammerApplied` tinyint(3) unsigned NOT NULL,
							  `Potential` tinyint(3) unsigned NOT NULL,
							  `Stars` tinyint(3) unsigned NOT NULL,
							  `Potential1` smallint(6) NOT NULL,
							  `Potential2` smallint(6) NOT NULL,
							  `Potential3` smallint(6) NOT NULL,
							  `PotentialLines` tinyint(3) unsigned NOT NULL,
							  `SerialNumber` int(11) NOT NULL,
							  `UniqueID` int(11) NOT NULL,
							  PRIMARY KEY (`ID`)
							) ENGINE=InnoDB DEFAULT CHARSET=latin1;
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

        mcdbConfiguration:
            Log.Inform("The setup will now check for a MapleStory database.");

            try
            {
                using (Database.TemporaryConnection(databaseHost, "mcdb", databaseUsername, databasePassword))
                {
                    Database.Test();
                }
            }
            catch (MySqlException e)
            {
                Log.Error(e);

                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the MCDB database? ", true))
                {
                    try
                    {
                        Log.Inform("Please wait...");

                        Database.ExecuteFile(databaseHost, databaseUsername, databasePassword, Application.ExecutablePath + ChannelServerSetup.McdbFileName);

                        Log.Inform("Database 'mcdb' created.", databaseSchema);
                    }
                    catch (Exception mcdbE)
                    {
                        Log.Error("Error while creating 'mcdb': ", mcdbE);
                        goto mcdbConfiguration;
                    }
                }
                else
                {
                    Log.SkipLine();

                    goto mcdbConfiguration;
                }
            }

            Log.SkipLine();

            Log.Success("Database configured!");

            Log.Entitle("Server Configuration");

            string WorldName = string.Empty;

            do
            {
                WorldName = Log.Input("World name (examples: Bera, Khaini): ", "Scania");
            }
            while (!WorldNameResolver.IsValid(WorldName));

            IPAddress loginIP = Log.Input("Enter the IP of the login server: ", IPAddress.Loopback);
            string securityCode = Log.Input("Assign the security code between servers: ", "");
            IPAddress externalIP = Log.Input("Enter the public channel server IP: ", IPAddress.Loopback);
            int maxUsers = Log.Input("Maximum amount of users for this channel: ", 80);

            Log.SkipLine();

            Log.Success("Server configured!");

            Log.Entitle("User Profile");

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
				JumpLists=3
				PacketRecord=[receive/send, ExactPacketName]
				
				[Server]
				World={1}
				ExternalIP={2}
				AutoRestartTime=30
				MaxUsers={3}
				
				[Login]
				IP={4}
				Port=8540
				SecurityCode={5}
				
				[Database]
				Host={6}
				Schema={7}
				Username={8}
				Password={9}",
                logLevel, WorldName, externalIP, maxUsers, loginIP,
                securityCode, databaseHost, databaseSchema,
                databaseUsername, databasePassword).Replace("	", "");

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "Configuration.ini"))
            {
                file.WriteLine(lines);
            }

            Log.Success("Configuration done!");
        }
    }
}
