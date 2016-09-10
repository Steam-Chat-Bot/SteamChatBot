using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Xaml;

using Microsoft.VisualBasic;

using Newtonsoft.Json;
using SteamAuth;
using SteamChatBot.Triggers;
using SteamKit2;

namespace SteamChatBot
{
    public class Bot
    {
        #region steam instances

        public static SteamClient steamClient = new SteamClient();
        public static CallbackManager manager = new CallbackManager(steamClient);
        public static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        public static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();
        public static SteamGameCoordinator steamGC = steamClient.GetHandler<SteamGameCoordinator>();

        public static SteamGuardAccount steamGuardAccount = new SteamGuardAccount();

        #endregion

        #region login variables

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

        public static List<BaseTrigger> triggers = new List<BaseTrigger>();

        private bool disposed = false;

        #region login data read/write

        /// <summary>
        /// Reads login data from username/login.json
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static UserInfo ReadData(string _username)
        {
            username = _username;
            string file = File.ReadAllText(username + "/login.json");
            UserInfo info = JsonConvert.DeserializeObject<UserInfo>(file);
            return info;
        }

        /// <summary>
        /// Writes login data to username/login.json
        /// </summary>
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
            File.AppendAllText("chatbots.txt", username + "\n");
            File.SetAttributes("chatbots.txt", File.GetAttributes("chatbots.txt") | FileAttributes.Hidden);
        }

        #endregion

        /// <summary>
        /// Starts the bot
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_password"></param>
        /// <param name="cll"></param>
        /// <param name="fll"></param>
        /// <param name="_logFile"></param>
        /// <param name="_displayName"></param>
        /// <param name="_sentryFile"></param>
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

            if(sharedSecret != "")
            {
                steamGuardAccount.SharedSecret = sharedSecret;
            }

            SubForCB();

            if (Directory.Exists(username + "/triggers/") && Directory.GetFiles(username + "/triggers/").Length > 0)
            {
                if (triggers.Count > 0)
                {
                    List<BaseTrigger> oldTriggers = BaseTrigger.ReadTriggers();
                    List<BaseTrigger> newTriggers = triggers;

                    Log.Instance.Verbose("Saving triggers...");
                    int count = triggers.Count;
                    foreach (BaseTrigger trigger in newTriggers)
                    {
                        Log.Instance.Debug("Saving triggers, " + count + " left");
                        trigger.SaveTrigger();
                        Log.Instance.Silly("Trigger {0}/{1} saved", trigger.Name, trigger.Type.ToString());
                        count--;
                    }
                    Log.Instance.Verbose("Successfully read trigger data from " + username + "/triggers/ and from triggers window");

                    triggers = oldTriggers.Concat(newTriggers).ToList();
                }
                else
                {
                    Log.Instance.Verbose("Loading triggers...");
                    triggers = BaseTrigger.ReadTriggers();
                    Log.Instance.Verbose("Successfully read trigger data from " + username + "/triggers/");
                }
            }
            else
            {
                if (triggers.Count > 0)
                {
                    Log.Instance.Verbose("Saving triggers...");
                    int count = triggers.Count;
                    foreach (BaseTrigger trigger in triggers)
                    {
                        Log.Instance.Debug("Saving triggers, " + count + " left");
                        trigger.SaveTrigger(); // save new triggers to file
                        Log.Instance.Silly("Trigger {0}/{1} saved", trigger.Name, trigger.Type.ToString());
                        count--;
                    }
                    Log.Instance.Verbose("Successfully read trigger data from triggers window");
                }
            }

            isRunning = true;

            Log.Instance.Silly("Updating SteamKit2 servers...");
            SteamDirectory.Initialize().Wait();

            Log.Instance.Verbose("Connecting to Steam...");
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
            steamClient.Connect();
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
            if (callback.EntryType == EChatEntryType.ChatMsg)
            {
                Log.Instance.Info("Friend Msg " + callback.EntryType + " " + callback.Sender + ": " + callback.Message);
                foreach (BaseTrigger trigger in triggers)
                {
                    trigger.OnFriendMessage(callback.Sender, callback.Message, true);
                }
            }
            else
            {
                Log.Instance.Silly("Friend Msg " + callback.EntryType + " " + callback.Sender + ": " + callback.Message);
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
            Log.Instance.Warn("Logged off from Steam for reason: " + callback.Result + ", logging in again...");
            foreach(var trigger in triggers)
            {
                trigger.OnLoggedOff();
            }
            LogOn();
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Log.Instance.Warn("Disconnected from Steam, reconnecting in 5 seconds...");
            System.Timers.Timer t = new System.Timers.Timer(5000);
            t.Elapsed += T_Elapsed;
            t.AutoReset = false;
            t.Start();
            
        }

        private static void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
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
                        string _tfc = Interaction.InputBox("Two factor code (sent via sms): ");
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
                    string _sgc = Interaction.InputBox("Steam guard code (sent to your email at " + callback.EmailDomain + "): ");
                    authCode = _sgc;
                }
            }
            else if (callback.Result == EResult.InvalidPassword)
            {
                Log.Instance.Error("Invalid username/password combination!");
                string _u = Interaction.InputBox("Username: ");
                username = _u;
                string _p = Interaction.InputBox("Password: ");
                password = _p;
                WriteData();
            }
            else
            {
                Log.Instance.Warn("EResult for logon: " + callback.Result + "/" + callback.ExtendedResult);
            }
        }

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
                else if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Left)
                {
                    foreach (BaseTrigger trigger in triggers)
                    {
                        trigger.OnLeftChat(callback.ChatRoomID, callback.StateChangeInfo.ChatterActedOn);
                    }
                }
                else if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Kicked)
                {
                    foreach(BaseTrigger trigger in triggers)
                    {
                        trigger.OnKickedChat(callback.ChatRoomID, callback.StateChangeInfo.ChatterActedOn, callback.StateChangeInfo.ChatterActedBy, true);
                    }
                }
                else if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Banned)
                {
                    foreach (BaseTrigger trigger in triggers)
                    {
                        trigger.OnBannedChat(callback.ChatRoomID, callback.StateChangeInfo.ChatterActedOn, callback.StateChangeInfo.ChatterActedBy, true);
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