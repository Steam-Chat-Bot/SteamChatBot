using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Xaml;
using System.Windows.Controls;
using System.ComponentModel;

using SteamKit2;
using SteamKit2.Internal;
using Newtonsoft.Json;
using SteamChatBot.Triggers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

using Microsoft.VisualBasic;

namespace SteamChatBot
{
    public class Bot
    {
        #region steam instances

        public static SteamClient steamClient = new SteamClient();
        public static CallbackManager manager = new CallbackManager(steamClient);
        public static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        public static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();

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
        public static List<CheckBox> checkBoxes = new List<CheckBox>();
        public static List<CheckBox> activeCheckBoxes = new List<CheckBox>();
        
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
                fll = FLL
            };

            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            if(!Directory.Exists(username + "/"))
            {
                Directory.CreateDirectory(username + "/");
            }
            File.WriteAllText(username + "/login.json", json);
        }

        #endregion

        public static void Start(string _username, string _password, string cll, string fll, string _logFile, string _displayName, string _sentryFile)
        {
            username = _username;
            logFile = _logFile;
            password = _password;
            displayName = _displayName;
            sentryFile = _sentryFile;
            CLL = cll;
            FLL = fll;

            if (!File.Exists(username + "/login.json"))
            {
                MessageBoxResult save = MessageBox.Show("Save login information to file?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (save == MessageBoxResult.Yes)
                {
                    WriteData();
                }
            }

            foreach (CheckBox box in checkBoxes)
            {
                if (box.IsChecked == true)
                {
                    activeCheckBoxes.Add(box);
                }

            }

            SubForCB();

            if (Directory.Exists(username + "/triggers/") && Directory.GetFiles(username + "/triggers/").Length > 0)
            {
                triggers = BaseTrigger.ReadTriggers();
                Log.Instance.Silly("Successfully read trigger data from triggers/");
            }
            foreach (CheckBox box in activeCheckBoxes)
            {
                switch (box.Name)
                {
                    case "isUpTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.IsUpTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.IsUpTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new IsUpTrigger(TriggerType.IsUpTrigger, "IsUpTrigger", options));
                        }
                        break;
                    case "chatReplyTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.ChatReplyTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.ChatReplyTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new ChatReplyTrigger(TriggerType.ChatReplyTrigger, "ChatReplyTrigger", options));
                        }
                        break;
                    case "acceptFriendRequestTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.AcceptFriendRequestTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AcceptFriendRequestTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AcceptFriendRequestTrigger(TriggerType.AcceptFriendRequestTrigger, "AcceptFriendRequestTrigger"));
                        }
                        break;
                    case "autojoinChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.AutojoinChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AutojoinChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AutojoinChatTrigger(TriggerType.AutojoinChatTrigger, "AutojoinChatTrigger", options));
                        }
                        break;
                    case "banTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.BanTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.BanTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new BanTrigger(TriggerType.BanTrigger, "BanTrigger", options));
                        }
                        break;
                    case "kickTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.KickTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.KickTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new KickTrigger(TriggerType.KickTrigger, "KickTrigger", options));
                        }
                        break;
                    case "acceptChatInviteTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.AcceptChatInviteTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AcceptChatInviteTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AcceptChatInviteTrigger(TriggerType.AcceptChatInviteTrigger, "AcceptChatInviteTrigger", options));
                        }
                        break;
                    case "leaveChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.LeaveChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LeaveChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LeaveChatTrigger(TriggerType.LeaveChatTrigger, "LeaveChatTrigger", options));
                        }
                        break;
                    case "linkNameTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.LinkNameTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LinkNameTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LinkNameTrigger(TriggerType.LinkNameTrigger, "LinkNameTrigger", options));
                        }
                        break;
                    case "doormatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.DoormatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.DoormatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new DoormatTrigger(TriggerType.DoormatTrigger, "DoormatTrigger", options));
                        }
                        break;
                    case "lockChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.LockChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LockChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LockChatTrigger(TriggerType.LockChatTrigger, "LockChatTrigger", options));
                        }
                        break;
                    case "unlockChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.UnlockChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnlockChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnlockChatTrigger(TriggerType.UnlockChatTrigger, "UnlockChatTrigger", options));
                        }
                        break;
                    case "moderateChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.ModerateChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.ModerateChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new ModerateChatTrigger(TriggerType.ModerateChatTrigger, "ModerateChatTrigger", options));
                        }
                        break;
                    case "unmoderateChatTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.UnmoderateChatTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnmoderateChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnmoderateChatTrigger(TriggerType.UnmoderateChatTrigger, "UnmoderateChatTrigger", options));
                        }
                        break;
                    case "unbanTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.UnbanTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnbanTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnbanTrigger(TriggerType.UnbanTrigger, "UnbanTrigger", options));
                        }
                        break;
                    case "banCheckTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.BanCheckTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.BanCheckTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new BanCheckTrigger(TriggerType.BanCheckTrigger, "BanCheckTrigger", options));
                        }
                        break;
                    case "weatherTriggerBox":
                        {
                            TriggerOptions options = GetTriggerOptions(TriggerType.WeatherTrigger);
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.WeatherTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new WeatherTrigger(TriggerType.WeatherTrigger, "WeatherTrigger", options));
                        }
                        break;
                }
            }

            Log.Instance.Verbose("Saving triggers...");
            foreach (BaseTrigger trigger in triggers)
            {
                trigger.SaveTrigger();
                Log.Instance.Silly("Trigger {0} saved", trigger.Name);
            }

            isRunning = true;

            Connect();

            while (isRunning)
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
            isRunning = true;
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
                isRunning = false;
                Log.Instance.Warn("EResult for connection: " + callback.Result);
            }
        }

        private static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Log.Instance.Error("Logged off from Steam for reason: " + callback.Result + ", logging in again...");
            isRunning = false;
            LogOn();
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Log.Instance.Error("Disconnected from Steam, reconnecting...");
            isRunning = false;
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
                    string _tfc = Interaction.InputBox("Two factor code (sent via sms): ");
                    twoFactorAuth = _tfc;
                }
                else if (callback.Result == EResult.AccountLogonDenied)
                {
                    string _sgc = Interaction.InputBox("Steam guard code (sent to your email at " + callback.EmailDomain + "): ");
                    authCode = _sgc;
                }
            }
            else if (callback.Result == EResult.InvalidPassword)
            {
                Log.Instance.Error("Invalid password! Delete login.json and manually enter your details in the GUI application to change them for future runs!");
                string _u = Interaction.InputBox("Username: ");
                username = _u;
                string _p = Interaction.InputBox("Password: ");
                password = _p;
            }
            else
            {
                isRunning = false;
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