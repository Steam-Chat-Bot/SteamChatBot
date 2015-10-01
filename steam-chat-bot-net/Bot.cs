using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using SteamKit2;
using Newtonsoft.Json;

namespace steam_chat_bot_net
{
    class Bot
    {
        static SteamClient steamClient = new SteamClient();
        static CallbackManager manager = new CallbackManager(steamClient);
        static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();
        static bool isRunning;
        static string username;
        static string password;
        static string authCode;
        static string twoFactorAuth;
        static byte[] sentryHash;
        static string displayName;
        static string autoJoinFile;
        static string sentryFile;
        static string logFile;

        private bool disposed = false;

        public class UserInfo
        {
            public string username { get; set; }
            public string password { get; set; }
            public string logFile { get; set; }
            public string displayName { get; set; }
            public string autoJoinFile { get; set; }
            public string sentryFile { get; set; }
        }

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
                autoJoinFile = autoJoinFile,
                sentryFile = sentryFile
            };

            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            File.WriteAllText("login.json", json);
        }
        public static void Start(string _username, string _password, string CLL, string FLL, string _logFile, string _displayName, string _autojoinFile, string _sentryFile)
        {
            username = _username;
            logFile = _logFile;
            password = _password;
            displayName = _displayName;
            autoJoinFile = _autojoinFile;
            sentryFile = _sentryFile;
            if (!File.Exists("login.json"))
            {
                Console.WriteLine("Save steam info to file? y//n");
                string save = Console.ReadLine();
                if (save == "y")
                {
                    WriteData();
                }
                else if (save == "n")
                {
                    Console.WriteLine("Not saving data.");
                }
                else
                {
                    Console.WriteLine("Unknown response, defaulting to YES");
                    WriteData();
                }
            }
            SubForCB();

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
            if(disposed)
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
