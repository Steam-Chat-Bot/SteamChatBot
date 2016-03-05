using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Xaml;
using System.Windows.Controls;

using SteamKit2;
using SteamKit2.Internal;
using Newtonsoft.Json;
using SteamChatBot.Triggers;

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
        public static List<SteamID> friends;
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

        public string GetUserName(SteamID steamID)
        {
            return "";
        }

        private bool disposed = false;

        #region login data read/write

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
            foreach (CheckBox box in checkBoxes)
            {
                if (box.IsChecked == true)
                {
                    activeCheckBoxes.Add(box);
                }

            }

            SubForCB();

            if (Directory.Exists("triggers/") && Directory.GetFiles("triggers/").Length > 0)
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
                            string command;
                            int timeout;
                            int delay;
                            int probability;
                            delays.TryGetValue(TriggerType.IsUpTrigger, out delay);
                            timeouts.TryGetValue(TriggerType.IsUpTrigger, out timeout);
                            commandList.TryGetValue(TriggerType.IsUpTrigger, out command);
                            probs.TryGetValue(TriggerType.IsUpTrigger, out probability);
                            TriggerOptions options = new TriggerOptions
                            {
                                Command = command,
                                Delay = delay,
                                Timeout = timeout,
                                Probability = probability != 0 ? probability / 100 : 1
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.IsUpTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new IsUpTrigger(TriggerType.IsUpTrigger, "IsUpTrigger", options));
                        }
                        break;
                    case "chatReplyTriggerBox":
                        {
                            List<string> matches;
                            List<string> responses;
                            int timeout;
                            int delay;
                            int probability;
                            delays.TryGetValue(TriggerType.ChatReplyTrigger, out delay);
                            timeouts.TryGetValue(TriggerType.ChatReplyTrigger, out timeout);
                            matchesList.TryGetValue(TriggerType.ChatReplyTrigger, out matches);
                            responsesList.TryGetValue(TriggerType.ChatReplyTrigger, out responses);
                            probs.TryGetValue(TriggerType.ChatReplyTrigger, out probability);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Delay = delay,
                                Timeout = timeout,
                                Matches = matches,
                                Responses = responses,
                                Probability = probability != 0 ? probability / 100 : 1
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.ChatReplyTrigger);
                            triggers.Remove(removed);

                            triggers.Add(new ChatReplyTrigger(TriggerType.ChatReplyTrigger, "ChatReplyTrigger", options));
                        }
                        break;
                    case "acceptFriendRequestTriggerBox":
                        {
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AcceptFriendRequestTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AcceptFriendRequestTrigger(TriggerType.AcceptFriendRequestTrigger, "AcceptFriendRequestTrigger"));
                        }
                        break;
                    case "autojoinChatTriggerBox":
                        {
                            List<SteamID> _rooms = new List<SteamID>();
                            rooms.TryGetValue(TriggerType.AutojoinChatTrigger, out _rooms);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Rooms = _rooms
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AutojoinChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AutojoinChatTrigger(TriggerType.AutojoinChatTrigger, "AutojoinChatTrigger", options));
                        }
                        break;
                    case "banTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.BanTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.BanTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new BanTrigger(TriggerType.BanTrigger, "BanTrigger", options));
                        }
                        break;
                    case "kickTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.KickTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.KickTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new KickTrigger(TriggerType.KickTrigger, "KickTrigger", options));
                        }
                        break;
                    case "acceptChatInviteTriggerBox":
                        {
                            List<SteamID> rooms_;
                            rooms.TryGetValue(TriggerType.AcceptChatInviteTrigger, out rooms_);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Rooms = rooms_
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.AcceptChatInviteTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new AcceptChatInviteTrigger(TriggerType.AcceptChatInviteTrigger, "AcceptChatInviteTrigger", options));
                        }
                        break;
                    case "leaveChatTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.LeaveChatTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LeaveChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LeaveChatTrigger(TriggerType.LeaveChatTrigger, "LeaveChatTrigger", options));
                        }
                        break;
                    case "linkNameTriggerBox":
                        {
                            TriggerOptions options = new TriggerOptions();
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LinkNameTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LinkNameTrigger(TriggerType.LinkNameTrigger, "LinkNameTrigger", options));
                        }
                        break;
                    case "doormatTriggerBox":
                        {
                            TriggerOptions options = new TriggerOptions();
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.DoormatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new DoormatTrigger(TriggerType.DoormatTrigger, "DoormatTrigger", options));
                        }
                        break;
                    case "lockChatTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.LockChatTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.LockChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new LockChatTrigger(TriggerType.LockChatTrigger, "LockChatTrigger", options));
                        }
                        break;
                    case "unlockChatTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.UnlockChatTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnlockChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnlockChatTrigger(TriggerType.UnlockChatTrigger, "UnlockChatTrigger", options));
                        }
                        break;
                    case "moderateChatTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.ModerateChatTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.ModerateChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new ModerateChatTrigger(TriggerType.ModerateChatTrigger, "ModerateChatTrigger", options));
                        }
                        break;
                    case "unmoderateChatTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.UnmoderateChatTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnmoderateChatTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnmoderateChatTrigger(TriggerType.UnmoderateChatTrigger, "UnmoderateChatTrigger", options));
                        }
                        break;
                    case "unbanTriggerBox":
                        {
                            string command;
                            commandList.TryGetValue(TriggerType.UnbanTrigger, out command);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.UnbanTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new UnbanTrigger(TriggerType.UnbanTrigger, "UnbanTrigger", options));
                        }
                        break;
                    case "banCheckTriggerBox":
                        {
                            string command;
                            string api;
                            commandList.TryGetValue(TriggerType.BanCheckTrigger, out command);
                            apiKeys.TryGetValue(TriggerType.BanCheckTrigger, out api);
                            TriggerOptions options = new TriggerOptions()
                            {
                                Command = command,
                                ApiKey = api
                            };
                            var removed = triggers.SingleOrDefault(r => r.Type == TriggerType.BanCheckTrigger);
                            triggers.Remove(removed);
                            triggers.Add(new BanCheckTrigger(TriggerType.BanCheckTrigger, "BanCheckTrigger", options));
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
                    Console.WriteLine("Two factor code (sent via sms): ");
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

        #endregion
    }
}