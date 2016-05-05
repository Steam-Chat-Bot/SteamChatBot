using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using SteamAuth;
using SteamChatBot_Mono.Triggers;
using SteamKit2;

namespace SteamChatBot_Mono
{
    public class Bot
    {
        #region steam instances

        public static SteamClient steamClient = new SteamClient();
        public static CallbackManager manager = new CallbackManager(steamClient);
        public static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        public static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();
        public static SteamGuardAccount steamGuardAccount = new SteamGuardAccount();
        #endregion

        #region public static login variables

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
        public static string sharedSecret;

        #endregion

        #region trigger options

        public static Dictionary<TriggerType, string> commandList = new Dictionary<TriggerType, string>();
        public static Dictionary<TriggerType, List<string>> matchesList = new Dictionary<TriggerType, List<string>>();
        public static Dictionary<TriggerType, List<string>> responsesList = new Dictionary<TriggerType, List<string>>();
        public static Dictionary<TriggerType, int> delays = new Dictionary<TriggerType, int>();
        public static Dictionary<TriggerType, int> timeouts = new Dictionary<TriggerType, int>();
        public static Dictionary<TriggerType, int> probs = new Dictionary<TriggerType, int>();
        public static Dictionary<TriggerType, List<SteamID>> rooms = new Dictionary<TriggerType, List<SteamID>>();
        public static Dictionary<TriggerType, List<SteamID>> users = new Dictionary<TriggerType, List<SteamID>>();
        public static Dictionary<TriggerType, List<SteamID>> ignores = new Dictionary<TriggerType, List<SteamID>>();
        public static Dictionary<TriggerType, string> apiKeys = new Dictionary<TriggerType, string>();

        #endregion

        public static List<BaseTrigger> triggers = new List<BaseTrigger>();
        
        public static BackgroundWorker worker;
        private bool disposed = false;

        /*
        private static async void RunCallbacks()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                });
            }
        }
        */

        #region login data read/write

        public static UserInfo ReadData(string _username)
        {
            username = _username;
            string file = File.ReadAllText(username + "/login.json");
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
                fll = FLL,
                sharedSecret = sharedSecret
            };

            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            if(!Directory.Exists(username + "/"))
            {
                Directory.CreateDirectory(username + "/");
            }
            File.WriteAllText(username + "/login.json", json);
        }

        #endregion

        public static void Start(string _username, string _password, string cll, string fll, string _logFile, string _displayName, string _sentryFile, string _sharedSecret)
        {
            username = _username;
            logFile = _logFile;
            password = _password;
            displayName = _displayName;
            sentryFile = _sentryFile;
            CLL = cll;
            FLL = fll;
            sharedSecret = _sharedSecret;

            if (!File.Exists(username + "/login.json"))
            {
                Console.WriteLine("Save login data to file? Y/N");
                ConsoleKeyInfo save = Console.ReadKey();
                if (save.Key == ConsoleKey.Y)
                {
                    WriteData();
                }
            }

            if(sharedSecret != "")
            {
                steamGuardAccount.SharedSecret = sharedSecret;
            }

            SubForCB();

            if (Directory.Exists(username + "/triggers/") && Directory.GetFiles(username + "/triggers/").Length > 0)
            {
                triggers = BaseTrigger.ReadTriggers();
                Log.Instance.Silly("Successfully read trigger data from triggers/");
            }

            Connect();

            while (true)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private static TriggerOptions GetTriggerOptions(TriggerType type)
        {
            string command;
            List<string> matches;
            List<string> responses;
            int timeout;
            int delay;
            int probability;
            List<SteamID> room;
            List<SteamID> user;
            List<SteamID> ignore;
            string api;
            commandList.TryGetValue(type, out command);
            matchesList.TryGetValue(type, out matches);
            responsesList.TryGetValue(type, out responses);
            delays.TryGetValue(type, out delay);
            timeouts.TryGetValue(type, out timeout);
            probs.TryGetValue(type, out probability);
            rooms.TryGetValue(type, out room);
            users.TryGetValue(type, out user);
            ignores.TryGetValue(type, out ignore);
            apiKeys.TryGetValue(type, out api);
            TriggerOptions options = new TriggerOptions
            {
                Command = command,
                Delay = delay,
                Matches = matches,
                Responses = responses,
                Timeout = timeout,
                Probability = probability != 0 ? probability / 100 : 1,
                Rooms = room,
                Ignore = ignore,
                User = user,
                ApiKey = api
            };

            return options;
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

        #region steam methods

        public static void Connect()
        {
            steamClient.Connect();
            Log.Instance.Verbose("Connecting to Steam...");
        }

        public static void LogOn()
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

        #endregion

        #region steam callback manager

        private static void SubForCB()
        {
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuth);
            manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);

            manager.Subscribe<SteamFriends.ChatMsgCallback>(OnChatMsg);
            manager.Subscribe<SteamFriends.FriendMsgCallback>(OnFriendMsg);
            manager.Subscribe<SteamFriends.FriendsListCallback>(OnFriendList);
            manager.Subscribe<SteamFriends.ChatInviteCallback>(OnChatInvite);
            manager.Subscribe<SteamFriends.ChatMemberInfoCallback>(OnChatMemberInfo);
            Log.Instance.Silly("Callback managers subscribed");
        }

        private static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            steamFriends.SetPersonaState(EPersonaState.Online);
            steamFriends.SetPersonaName(displayName);
        }

        private static void OnFriendMsg(SteamFriends.FriendMsgCallback callback)
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

        private static void OnChatMsg(SteamFriends.ChatMsgCallback callback)
        {
            Log.Instance.Info("Chat Msg " + callback.ChatMsgType + " " + callback.ChatRoomID + ": " + callback.Message);
            if (callback.ChatMsgType == EChatEntryType.ChatMsg)
            {
                foreach (BaseTrigger trigger in triggers)
                {
                    trigger.OnChatMessage(callback.ChatRoomID, callback.ChatterID, callback.Message, true);
                }
            }
        }

        private static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Log.Instance.Info("Connected, logging in...");
                sentryHash = null;
                if (File.Exists(username + "/" + username + ".sentry"))
                {
                    byte[] _sentryFile = File.ReadAllBytes(username + "/" + sentryFile);
                    sentryHash = CryptoHelper.SHAHash(_sentryFile);
                }

                LogOn();
            }
            else
            {
                Log.Instance.Warn("EResult for connection: " + callback.Result);
            }
        }

        private static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Log.Instance.Error("Logged off from Steam for reason: " + callback.Result + ", logging in again...");;
            LogOn();
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Log.Instance.Error("Disconnected from Steam, reconnecting...");
            Connect();
        }

        private static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Log.Instance.Info("Logged in!");
                foreach (BaseTrigger trigger in triggers)
                {
                    trigger.OnLoggedOn();
                }
            }
            else if (callback.Result == EResult.AccountLogonDenied || callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                if (callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
                {
                    if (sharedSecret == "")
                    {
                        Console.WriteLine("Two Factor Code (sent via mobile Steam app): ");
                        string _tfc = Console.ReadLine();
                        twoFactorAuth = _tfc;
                    }
                    else
                    {
                        Log.Instance.Debug("Automatically getting 2FA code with Shared Secret");
                        string _tfc = steamGuardAccount.GenerateSteamGuardCode();
                        twoFactorAuth = _tfc;
                    }
                        
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
                Log.Instance.Error("Invalid username/password!");
                Console.WriteLine("Username: ");
                username = Console.ReadLine();
                Console.WriteLine("Password: ");
                password = Console.ReadLine();
            }
            else
            {
                Log.Instance.Warn("EResult for logon: " + callback.Result + "/" + callback.ExtendedResult);
            }
        }

        /*
        private static void OnFriendAdded(SteamFriends.FriendAddedCallback callback)
        {
            Log.Instance.Debug("{0} proposed friend add.", callback.SteamID);
            foreach (BaseTrigger trigger in triggers)
            {
                trigger.OnFriendRequest(callback.SteamID);
            }
        }
        */

        private static void OnFriendList(SteamFriends.FriendsListCallback callback)
        {
            foreach (var friend in callback.FriendList)
            {
                if (friend.Relationship == EFriendRelationship.RequestRecipient)
                {
                    Log.Instance.Info("Received friend request from " + friend.SteamID);
                    foreach (BaseTrigger trigger in triggers)
                    {
                        trigger.OnFriendRequest(friend.SteamID);
                    }
                }
            }
        }

        private static void OnChatInvite(SteamFriends.ChatInviteCallback callback)
        {
            Log.Instance.Verbose("Received invite from {0} to chat {1}/{2}", callback.PatronID, callback.ChatRoomID, callback.ChatRoomName);
            foreach (BaseTrigger trigger in triggers)
            {
                trigger.OnChatInvite(SteamHelper.ToClanID(callback.ChatRoomID), callback.ChatRoomName, callback.PatronID);
            }
        }

        static void OnChatMemberInfo(SteamFriends.ChatMemberInfoCallback callback)
        {
            Log.Instance.Info("ChatStateChange " + callback.StateChangeInfo.StateChange + " in " + callback.ChatRoomID + " acted on " + callback.StateChangeInfo.ChatterActedBy + " by " + callback.StateChangeInfo.ChatterActedBy);
            if(callback.Type == EChatInfoType.StateChange)
            {
                if (callback.StateChangeInfo.StateChange == EChatMemberStateChange.Entered) {
                    foreach (BaseTrigger trigger in triggers)
                    {
                        trigger.OnEnteredChat(callback.ChatRoomID, callback.StateChangeInfo.ChatterActedOn, true);
                    }
                }
            }
        }

        private static void OnUpdateMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Log.Instance.Debug("New sentry: " + callback.FileName + ". Writing file...");

            int fileSize;
            using (var fs = File.Open(username + "/" + username + ".sentry", FileMode.OpenOrCreate, FileAccess.ReadWrite))
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

        #endregion
    }
}