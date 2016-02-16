using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SteamKit2;
using SteamChatBot;
using System.IO;
using Newtonsoft.Json;

namespace SteamChatBotUnitTest
{
    [TestClass]
    public class BotTest
    {
        [TestMethod]
        public void Test_Start()
        {
            string username = "username";
            string password = "password";
            string cll = "silly";
            string fll = "silly";
            string log = "username.log";
            string sentry = "username.sentry";
            string displayname = "Chat Bot";

            Bot.Start(username, password, cll, fll, log, displayname, sentry);

            Assert.AreEqual(username, Bot.username, "Username not set properly");
            Assert.AreEqual(password, Bot.password, "Password not set properly");
            Assert.AreEqual(cll, Bot.CLL, "CLL not set properly");
            Assert.AreEqual(fll, Bot.FLL, "FLL not set properly");
            Assert.AreEqual(log, Bot.logFile, "Log file not set properly");
            Assert.AreEqual(sentry, Bot.sentryFile, "Sentry file not set properly");
            Assert.AreEqual(displayname, Bot.displayName, "Display name not set properly");

        }

        [TestMethod]
        public void Test_Connect()
        {
            bool isRunning = false;
            Log.CreateInstance("log.log", "Bot test", Log.LogLevel.Silly, Log.LogLevel.Silly);

            Bot.Connect();

            Assert.AreNotEqual(isRunning, Bot.isRunning, "Bot is not running");
        }

        [TestMethod]
        public void Test_ReadData()
        {
            string username = "username";
            string password = "password";
            string cll = "silly";
            string fll = "silly";
            string log = "username.log";
            string sentry = "username.sentry";
            string displayname = "Chat Bot";

            UserInfo toWrite = new UserInfo
            {
                username = username,
                password = password,
                logFile = log,
                displayName = displayname,
                sentryFile = sentry,
                cll = cll,
                fll = fll
            };
            string jsonToWrite = JsonConvert.SerializeObject(toWrite, Formatting.Indented);
            File.WriteAllText("testfile.json", jsonToWrite);

            string jsonToRead = File.ReadAllText("testfile.json");
            UserInfo info = JsonConvert.DeserializeObject<UserInfo>(jsonToRead);

            Assert.AreEqual(username, info.username, "Username not read correctly");
            Assert.AreEqual(password, info.password, "Password not read correctly");
            Assert.AreEqual(cll, info.cll, "CLL not read correctly");
            Assert.AreEqual(fll, info.fll, "FLL not read correctly");
            Assert.AreEqual(log, info.logFile, "Log file not read correctly");
            Assert.AreEqual(sentry, info.sentryFile, "Sentry file not read correctly");
            Assert.AreEqual(displayname, info.displayName, "Display name not read correctly");

            File.Delete("testfile.json");
        }
    }

    [TestClass] 
    public class BaseTriggerTest
    {

    }
}
