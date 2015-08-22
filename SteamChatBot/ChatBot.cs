using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using SteamKit2;

namespace SteamChatBot
{
    class ChatBot
    {
        static SteamClient steamClient = new SteamClient();
        static CallbackManager manager = new CallbackManager(steamClient);
        static SteamUser steamUser = steamClient.GetHandler<SteamUser>();
        static SteamFriends steamFriends = steamClient.GetHandler<SteamFriends>();

        static SteamID chatRoom = 103582791438731217; //chatbot dev group

        static bool isRunning;

        static string username;
        static string password;
        static string authCode;
        static string twoFactorAuth;
        static byte[] sentryHash;

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Username: ");
                string _u = Console.ReadLine();
                username = _u;
                Console.WriteLine("Password: ");
                string _p = Console.ReadLine();
                password = _p;
            }
            else
            {
                username = args[0];
                password = args[1];
            }
            SubForCB();

            isRunning = true;

            Connect();

            while(isRunning)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
            
            Console.ReadLine();
        }

        /// <summary>
        /// Connects the bot to steam
        /// </summary>
        static void Connect()
        {
            steamClient.Connect();
            Console.WriteLine("Connecting to Steam...");
            isRunning = true;
        }

        /// <summary>
        /// Logs the bot onto steam
        /// </summary>
        static void LogOn()
        {
            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = username,
                Password = password,
                AuthCode = authCode,
                TwoFactorCode = twoFactorAuth,
                SentryFileHash = sentryHash
            });
        }

        /// <summary>
        /// Subscribes for events
        /// </summary>
        static void SubForCB()
        {
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuth);
            manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);
            manager.Subscribe<SteamFriends.ChatMsgCallback>(OnChatMsg);
            Console.WriteLine("Callback managers subscribed");
        }
        
        /// <summary>
        /// onAccountInfo event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            Console.WriteLine("Got user info");
            steamFriends.SetPersonaState(EPersonaState.Online);
            steamFriends.SetPersonaName("C# > Node.js");
            steamFriends.JoinChat(chatRoom);
            steamFriends.SendChatRoomMessage(chatRoom, EChatEntryType.ChatMsg, "C# port of SteamChatBot has arrived!");
        }

        /// <summary>
        /// onChatMsg (group chat) event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnChatMsg(SteamFriends.ChatMsgCallback callback)
        {
            Console.WriteLine("Message in " + callback.ChatRoomID + ": " + callback.Message);
            if(callback.Message == "!friends")
            {
                Console.WriteLine("friends: " + steamFriends.GetFriendCount());
                steamFriends.SendChatRoomMessage(chatRoom, EChatEntryType.ChatMsg, "I currently have " + steamFriends.GetFriendCount().ToString());
            }
        }

        /// <summary>
        /// connected event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if(callback.Result == EResult.OK)
            {
                Console.WriteLine("Connected, logging in...");
                sentryHash = null;
                if(File.Exists(username + ".sentry"))
                {
                    byte[] sentryFile = File.ReadAllBytes(username + ".sentry");
                    sentryHash = CryptoHelper.SHAHash(sentryFile);
                }

                LogOn();
            }
            else
            {
                isRunning = false;
                Console.WriteLine("EResult for connection: " + callback.Result);
            }
        }

        /// <summary>
        /// loggedOff event handler
        /// </summary>
        /// <param name="callback"></param>

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam for reason: " + callback.Result);
            isRunning = false;
            LogOn();
        }

        /// <summary>
        /// onDisconnected event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected!");
            isRunning = false;
            Connect();
        }

        /// <summary>
        /// onLoggedOn event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Console.WriteLine("Logged in!");
            }
            else if(callback.Result == EResult.AccountLogonDenied || callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                if(callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
                {
                    Console.WriteLine("Two factor code: ");
                    string _tfc = Console.ReadLine();
                    twoFactorAuth = _tfc;
                }
                else if(callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine("Steam guard code (" + callback.EmailDomain + "): ");
                    string _sgc = Console.ReadLine();
                    authCode = _sgc;
                }
            }
            else if(callback.Result == EResult.InvalidPassword)
            {
                Console.WriteLine("Invalid details!");
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
                Console.WriteLine("EResult for logon: " + callback.Result + "/" + callback.ExtendedResult);
            }
        }

        /// <summary>
        /// onUpdateMachineAuth event handler
        /// </summary>
        /// <param name="callback"></param>
        static void OnUpdateMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Console.WriteLine("New sentry: " + callback.FileName + ". Writing file...");

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

            Console.WriteLine("Sent sentry response!");
        }
    }
}
