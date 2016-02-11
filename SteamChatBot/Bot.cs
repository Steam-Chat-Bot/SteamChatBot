using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Xaml;
using System.Windows.Controls;

using SteamKit2;
using Newtonsoft.Json;
using SteamChatBot.Triggers;

namespace SteamChatBot
{
    public class Bot
    {
        public static SteamClient steamClient = new SteamClient();
        public static CallbackManager manager = new CallbackManager(steamClient);
        public static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        public static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();
        public static bool isRunning;
        public static string username;
        public static string password;
        public static string authCode;
        public static string twoFactorAuth;
        public static byte[] sentryHash;
        public static string displayName;
        public static string sentryFile;
        public static string logFile;
        public static string FLL;
        public static string CLL;
        public static List<BaseTrigger> triggers = new List<BaseTrigger>();
        public static List<CheckBox> checkBoxes = new List<CheckBox>();
        public static List<CheckBox> activeCheckBoxes = new List<CheckBox>();

        private bool disposed = false;

        public static UserInfo ReadData()
        {
            string file = File.ReadAllText("login.json");
            UserInfo info = JsonConvert.DeserializeObject<UserInfo>(file);
            return info;
        }

        public static void WriteData()
        {
            UserInfo info = new UserInfo
            {
                username = username,
                password = password,
                logFile = logFile,
                displayName = displayName,
                sentryFile = sentryFile,
                cll = CLL,
                fll = FLL
            };

            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            File.WriteAllText("login.json", json);
        }
        public static void Start(string _username, string _password, string cll, string fll, string _logFile, string _displayName, string _sentryFile)
        {
            username = _username;
            logFile = _logFile;
            password = _password;
            displayName = _displayName;
            sentryFile = _sentryFile;
            CLL = cll;
            FLL = fll;
            
            if (!File.Exists("login.json"))
            {
                Console.WriteLine("Save steam info to file? y//n");
                Console.CursorVisible = false;
                ConsoleKeyInfo save = Console.ReadKey(true);
                if (save.Key == ConsoleKey.Y)
                {
                    WriteData();
                }
                else if (save.Key == ConsoleKey.N)
                {
                    Console.WriteLine("Not saving data.");
                }
                else
                {
                    Console.WriteLine("Unknown response, defaulting to YES");
                    WriteData();
                }
            }
            foreach(CheckBox box in checkBoxes)
            {
                if(box.IsChecked == true)
                {
                    activeCheckBoxes.Add(box);
                }
                    
            }

            SubForCB();

            foreach(CheckBox box in activeCheckBoxes)
            {
                if(box.Name == "isUpTriggerBox")
                {
                    triggers.Add(new IsUpTrigger(TriggerType.IsUpTrigger, "IsUpTriggerTest"));
                   
                }
            }

            foreach(BaseTrigger trigger in triggers)
            {
                trigger.SaveTrigger();
            }

            isRunning = true;

            Connect();

            while (isRunning)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                Log.Instance.Dispose();
            }
            disposed = true;
        }
        static void Connect()
        {
            isRunning = true;
            steamClient.Connect();
            Log.Instance.Verbose("Connecting to Steam...");
        }

        static void LogOn()
        {
            Log.Instance.Verbose("Logging in...");
            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = username,
                Password = password,
                AuthCode = authCode,
                TwoFactorCode = twoFactorAuth,
                SentryFileHash = sentryHash
            });
        }

        static void SubForCB()
        {
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuth);
            manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);
            manager.Subscribe<SteamFriends.ChatMsgCallback>(OnChatMsg);
            manager.Subscribe<SteamFriends.FriendMsgCallback>(OnFriendMsg);
            Log.Instance.Silly("Callback managers subscribed");
        }

        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            Log.Instance.Debug("Got user info");
            steamFriends.SetPersonaState(EPersonaState.Online);
            steamFriends.SetPersonaName(displayName);
        }

        static void OnFriendMsg(SteamFriends.FriendMsgCallback callback)
        {
            Log.Instance.Info("Friend Msg " + callback.EntryType + " " + callback.Sender + ": " + callback.Message);
            if (callback.EntryType == EChatEntryType.ChatMsg)
            {
                foreach (BaseTrigger trigger in triggers)
                {
                    trigger.OnFriendMessage(callback.Sender, callback.Message, true);
                }
            }
        }

        static void OnChatMsg(SteamFriends.ChatMsgCallback callback)
        {
            Log.Instance.Info("Chat Msg " + callback.ChatMsgType + " " + callback.ChatRoomID + ": " + callback.Message);
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Log.Instance.Info("Connected, logging in...");
                sentryHash = null;
                if (File.Exists(username + ".sentry"))
                {
                    byte[] _sentryFile = File.ReadAllBytes(sentryFile);
                    sentryHash = CryptoHelper.SHAHash(_sentryFile);
                }

                LogOn();
            }
            else
            {
                isRunning = false;
                Log.Instance.Warn("EResult for connection: " + callback.Result);
            }
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Log.Instance.Error("Logged off from Steam for reason: " + callback.Result + ", logging in again...");
            isRunning = false;
            LogOn();
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Log.Instance.Error("Disconnected from Steam, reconnecting...");
            isRunning = false;
            Connect();
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Log.Instance.Info("Logged in!");
            }
            else if (callback.Result == EResult.AccountLogonDenied || callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                if (callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
                {
                    Console.WriteLine("Two factor code: ");
                    string _tfc = Console.ReadLine();
                    twoFactorAuth = _tfc;
                }
                else if (callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine("Steam guard code (sent to your email at " + callback.EmailDomain + "): ");
                    string _sgc = Console.ReadLine();
                    authCode = _sgc;
                }
            }
            else if (callback.Result == EResult.InvalidPassword)
            {
                Log.Instance.Error("Invalid password! Delete login.json and manually enter your details in the GUI application to change them for future runs!");
                Console.WriteLine("Username: ");
                string _u = Console.ReadLine();
                username = _u;
                Console.WriteLine("Password: ");
                string _p = Console.ReadLine();
                password = _p;
            }
            else
            {
                isRunning = false;
                Log.Instance.Warn("EResult for logon: " + callback.Result + "/" + callback.ExtendedResult);
            }
        }

        static void OnUpdateMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Log.Instance.Debug("New sentry: " + callback.FileName + ". Writing file...");

            int fileSize;
            using (var fs = File.Open(username + ".sentry", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = new SHA1CryptoServiceProvider())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,
                FileName = callback.FileName,
                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,
                Result = EResult.OK,
                LastError = 0,
                OneTimePassword = callback.OneTimePassword,
                SentryFileHash = sentryHash
            });

            Log.Instance.Verbose("Sent sentry response!");
        }
    }
}
