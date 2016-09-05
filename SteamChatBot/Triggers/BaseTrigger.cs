using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using SteamKit2;
using Newtonsoft.Json;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    public class BaseTrigger
    {
        public TriggerType Type { get; set; }
        public OptionType OptionsType { get; set; }
        public string Name { get; set; }
        public TriggerOptionsBase Options { get; set; }

        public bool ReplyEnabled = true;

        #region constructors

        /// <summary>
        /// Constructor for ChatCommand triggers
        /// </summary>
        /// <param name="type">TriggerType</param>
        /// <param name="name">Name of the trigger</param>
        /// <param name="chatCommand">ChatCommand object with options</param>
        public BaseTrigger(TriggerType type, string name, ChatCommand chatCommand)
        {
            OptionsType = OptionType.ChatCommand;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                ChatCommand = chatCommand
            };
        }

        /// <summary>
        /// Constructor for ChatReply triggers
        /// </summary>
        /// <param name="type">TriggerType</param>
        /// <param name="name">Name of the trigger</param>
        /// <param name="chatReply">ChatReply object with options</param>
        public BaseTrigger(TriggerType type, string name, ChatReply chatReply)
        {
            OptionsType = OptionType.ChatReply;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                ChatReply = chatReply
            };
        }

        /// <summary>
        /// Constructor for NoCommand triggers
        /// </summary>
        /// <param name="type">TriggerType</param>
        /// <param name="name">Name of teh trigger</param>
        /// <param name="noCommand">NoCommand object with options</param>
        public BaseTrigger(TriggerType type, string name, NoCommand noCommand)
        {
            OptionsType = OptionType.NoCommand;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                NoCommand = noCommand
            };
        }

        /// <summary>
        /// Constructor for ChatCommandApi triggers
        /// </summary>
        /// <param name="type">TriggerType</param>
        /// <param name="name">Name of the trigger</param>
        /// <param name="chatCommandApi">ChatCommandApi object with options</param>
        public BaseTrigger(TriggerType type, string name, ChatCommandApi chatCommandApi)
        {
            OptionsType = OptionType.ChatCommandAPI;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                ChatCommandApi = chatCommandApi
            };
        }

        /// <summary>
        /// Constructor for TriggerLists triggers
        /// </summary>
        /// <param name="type">TriggerType</param>
        /// <param name="name">Name of the trigger</param>
        /// <param name="tl">TriggerLists object with options</param>
        public BaseTrigger(TriggerType type, string name, TriggerLists tl)
        {
            OptionsType = OptionType.JustLists;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                TriggerLists = tl
            };
        }

        /// <summary>
        /// Constructor for AntiSpamTrigger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public BaseTrigger(TriggerType type, string name, AntiSpamTriggerOptions options)
        {
            OptionsType = OptionType.AntiSpamTrigger;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                AntiSpamTriggerOptions = options
            };
        }

        /// <summary>
        /// Constructor for DiscordTrigger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public BaseTrigger(TriggerType type, string name, DiscordOptions options)
        {
            OptionsType = OptionType.DiscordTrigger;
            Type = type;
            Name = name;
            Options = new TriggerOptionsBase
            {
                Type = type,
                Name = name,
                DiscordOptions = options
            };
        }
        #endregion


        /// <summary>
        /// If there is an error, log it easily
        /// </summary>
        /// <param name="cbn"></param>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <returns>error string</returns>
        protected string IfError(string cbn, string name, string error)
        {
            return string.Format("{0}/{1}: Error: {2}", cbn, name, error);
        }

        #region trigger read-write

        /// <summary>
        /// Save current trigger to file
        /// </summary>
        public void SaveTrigger()
        {
            if (!Directory.Exists(Bot.username + "/triggers/"))
            {
                Directory.CreateDirectory(Bot.username + "/triggers/");
            }

            if (Options != null)
            {
                TriggerOptionsBase options = new TriggerOptionsBase
                {
                    Name = Options.Name,
                    Type = Options.Type,
                    ChatCommand = Options.ChatCommand,
                    ChatCommandApi = Options.ChatCommandApi,
                    ChatReply = Options.ChatReply,
                    NoCommand = Options.NoCommand,
                    TriggerLists = Options.TriggerLists,
                    TriggerNumbers = Options.TriggerNumbers,
                    AntiSpamTriggerOptions = Options.AntiSpamTriggerOptions,
                    DiscordOptions = Options.DiscordOptions
                };
                string json = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(Bot.username + "/triggers/" + Name + ".json", json);
            }
            else if (Options == null)
            {
                TriggerOptionsBase options = new TriggerOptionsBase();
                string json = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(Bot.username + "/triggers/" + Name + ".json", json);
            }
        }
        
        /// <summary>
        /// Read triggers from username/triggers/
        /// </summary>
        /// <returns>A list of BaseTrigger objects</returns>
        public static List<BaseTrigger> ReadTriggers()
        {
            List<BaseTrigger> temp = new List<BaseTrigger>();
            IEnumerable<string> files = Directory.EnumerateFiles(Bot.username + "/triggers/");
            foreach (string file in files)
            {
                /*
                int start = file.IndexOf("triggers/") + "triggers/".Length;
                int end = file.IndexOf(".", start);
                string _file = file.Substring(start, end - start);
                */

                TriggerOptionsBase options = JsonConvert.DeserializeObject<TriggerOptionsBase>(File.ReadAllText(file));
                TriggerType type = options.Type;
                string name = options.Name;

                switch (type)
                {
                    case TriggerType.AcceptChatInviteTrigger:
                        temp.Add(new AcceptChatInviteTrigger(type, name, options.TriggerLists));
                        break;
                    case TriggerType.AcceptFriendRequestTrigger:
                        temp.Add(new AcceptFriendRequestTrigger(type, name, options.TriggerLists));
                        break;
                    case TriggerType.AntispamTrigger:
                        temp.Add(new AntispamTrigger(type, name, options.AntiSpamTriggerOptions));
                        break;
                    case TriggerType.AutojoinChatTrigger:
                        temp.Add(new AutojoinChatTrigger(type, name, options.TriggerLists));
                        break;
                    case TriggerType.BanCheckTrigger:
                        temp.Add(new BanCheckTrigger(type, name, options.ChatCommandApi));
                        break;
                    case TriggerType.BanTrigger:
                        temp.Add(new BanTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.ChatReplyTrigger:
                        temp.Add(new ChatReplyTrigger(type, name, options.ChatReply));
                        break;
                    case TriggerType.DiscordTrigger:
                        temp.Add(new DiscordTrigger(type, name, options.DiscordOptions));
                        break;
                    case TriggerType.DoormatTrigger:
                        temp.Add(new DoormatTrigger(type, name, options.NoCommand));
                        break;
                    case TriggerType.IsUpTrigger:
                        temp.Add(new IsUpTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.KickTrigger:
                        temp.Add(new KickTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.LeaveChatTrigger:
                        temp.Add(new LeaveChatTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.LinkNameTrigger:
                        temp.Add(new LinkNameTrigger(type, name, options.NoCommand));
                        break;
                    case TriggerType.LockChatTrigger:
                        temp.Add(new LockChatTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.ModerateChatTrigger:
                        temp.Add(new ModerateChatTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.PlayGameTrigger:
                        temp.Add(new PlayGameTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.UnbanTrigger:
                        temp.Add(new UnbanTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.UnlockChatTrigger:
                        temp.Add(new UnlockChatTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.UnmoderateChatTrigger:
                        temp.Add(new UnmoderateChatTrigger(type, name, options.ChatCommand));
                        break;
                    case TriggerType.WeatherTrigger:
                        temp.Add(new WeatherTrigger(type, name, options.ChatCommandApi));
                        break;
                    default:
                        break;
                }
            }
            return temp;
        }

        #endregion

        #region overriden methods
        /// <summary>
        /// Return true if trigger loads properly
        /// </summary>
        /// <returns></returns>
        public virtual bool OnLoad()
        {
            try
            {
                bool ret = onLoad();
                if (!ret)
                {
                    Log.Instance.Error("{0}/{1}: Error loading trigger {2}: OnLoad returned {3}", Bot.username, Name, Name, ret);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Reacts to bot being logged on
        /// </summary>
        /// <returns></returns>
        public virtual bool OnLoggedOn()
        {
            try
            {
                return onLoggedOn();
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Reacts to bot being logged off
        /// </summary>
        /// <returns></returns>
        public virtual bool OnLoggedOff()
        {
            try
            {
                return onLoggedOff();
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Return true if the invite is accepted
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="roomName"></param>
        /// <param name="inviterID"></param>
        /// <returns></returns>
        public virtual bool OnChatInvite(SteamID roomID, string roomName, SteamID inviterID)
        {
            if (CheckUser(inviterID) && CheckRoom(roomID) && !CheckIgnores(roomID, inviterID))
            {
                try
                {
                    return respondToChatInvite(roomID, roomName, inviterID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if the request is accepted
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public virtual bool OnFriendRequest(SteamID userID)
        {
            try
            {
                return respondToFriendRequest(userID);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Return true if a message was sent
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="message"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnFriendMessage(SteamID userID, string message, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckUser(userID) && !CheckIgnores(userID, null))
            {
                try
                {
                    bool messageSent = respondToFriendMessage(userID, message);
                    if (messageSent)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToFriendMessage - {2} - {3}", Bot.username, Name, userID, message);
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// When someone sends a trade offer
        /// </summary>
        /// <param name="number"></param>
        /// <param name="haveEatenEvent"></param>
        /// <returns></returns>
        public virtual bool OnTradeOffer(int number, bool haveEatenEvent)
        {
            try
            {
                var eventEaten = respondToTradeOffer(number);
                if (eventEaten)
                {
                    Log.Instance.Silly("{0}/{1}: Sent RespondToTradeOffer: {2}", Bot.username, Name, number);
                }
                return eventEaten;
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// When someone sends a trade invite
        /// </summary>
        /// <param name="tradeID"></param>
        /// <param name="userID"></param>
        /// <param name="haveEatenEvent"></param>
        /// <returns></returns>
        public virtual bool OnTradeProposed(SteamID tradeID, SteamID userID, bool haveEatenEvent)
        {
            if (CheckUser(userID) && CheckIgnores(userID, null))
            {
                try
                {
                    bool eventEaten = respondToTradeProposal(tradeID, userID);
                    if (eventEaten)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToTradeProposal: {2}", Bot.username, Name, tradeID);
                    }
                    return eventEaten;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// When someone a trade is opened
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="haveEatenEvent"></param>
        /// <returns></returns>
        public virtual bool OnTradeSession(SteamID userID, bool haveEatenEvent)
        {
            try
            {
                bool eventEaten = respondToTradeSession(userID);
                if (eventEaten)
                {
                    Log.Instance.Silly("{0}/{1}: Opened trade with: {2}", Bot.username, Name, userID);
                }
                return eventEaten;
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// When a group makes an announcement
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="headline"></param>
        /// <param name="haveEatenEvent"></param>
        /// <returns></returns>
        public virtual bool OnAnnouncement(SteamID groupID, string headline, bool haveEatenEvent)
        {
            if (CheckIgnores(groupID, null))
            {
                try
                {
                    bool eventEaten = respondToAnnouncement(groupID, headline);
                    if (eventEaten)
                    {
                        Log.Instance.Silly("{0}/{1}: responded to {1}'s announcement: {2}", Bot.username, Name, groupID, headline);
                    }
                    return eventEaten;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if message was seen but don't want other triggers to see
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="message"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnSentMessage(SteamID toID, string message, bool haveSentMessage)
        {
            try
            {
                bool messageSeen = respondToSentMessage(toID, message);
                if (messageSeen)
                {
                    return true;
                }
                return messageSeen;
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Return true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="chatterID"></param>
        /// <param name="message"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnChatMessage(SteamID roomID, SteamID chatterID, string message, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckUser(chatterID) && CheckRoom(roomID) && !CheckIgnores(chatterID, roomID))
            {
                try
                {
                    bool messageSent = respondToChatMessage(roomID, chatterID, message);
                    if (messageSent)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToChatMessage - {2} - {3} - {4}", Bot.username, Name, chatterID, roomID, message);
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="userID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnEnteredChat(SteamID roomID, SteamID userID, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckRoom(roomID) && CheckUser(userID) && !CheckIgnores(userID, roomID))
            {
                try
                {
                    bool messageSent = respondToEnteredMessage(roomID, userID);
                    if(messageSent)
                    {
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="kickedID"></param>
        /// <param name="kickerID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnKickedChat(SteamID roomID, SteamID kickedID, SteamID kickerID, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckRoom(roomID)) {
                try
                {
                    bool messageSent = respondToKick(roomID, kickedID, kickerID);
                    if(messageSent)
                    {
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="bannedID"></param>
        /// <param name="bannerID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnBannedChat(SteamID roomID, SteamID bannedID, SteamID bannerID, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckRoom(roomID))
            {
                try
                {
                    bool messageSent = respondToBan(roomID, bannedID, bannerID);
                    if(messageSent)
                    {
                        DisableForTimeout();
                    }
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="userID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnDisconnected(SteamID roomID, SteamID userID, bool haveSentMessage)
        {
            if (ReplyEnabled && RandomRoll() && CheckRoom(roomID) && CheckUser(userID) && !CheckIgnores(userID, roomID))
            {
                try
                {
                    bool messageSent = respondToDisconnect(roomID, userID);
                    if(messageSent)
                    {
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a message was sent
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public virtual bool OnLeftChat(SteamID roomID, SteamID userID)
        {
            if (ReplyEnabled && RandomRoll() && CheckRoom(roomID) && CheckUser(userID) && !CheckIgnores(roomID, userID))
            {
                try
                {
                    bool messageSent = respondToEnteredMessage(roomID, userID);
                    if(messageSent)
                    {
                        DisableForTimeout();
                    }
                    return messageSent;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region subclass methods

        public virtual bool onLoad()
        {
            return true;
        }

        public virtual bool onLoggedOn()
        {
            return true;
        }

        public virtual bool onLoggedOff()
        {
            return true;
        }

        public virtual bool respondToChatInvite(SteamID roomID, string roomName, SteamID inviterId)
        {
            return false;
        }

        // Returns true if the request is accepted
        public virtual bool respondToFriendRequest(SteamID userID)
        {
            return false;
        }

        // Return true if a message was sent
        public virtual bool respondToFriendMessage(SteamID userID, string message)
        {
            return false;
        }

        // Return true if a sent message has been used and shouldn't be seen again.
        public virtual bool respondToSentMessage(SteamID toID, string message)
        {
            return false;
        }

        // Return true if a message was sent
        public virtual bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return false;
        }

        // Return true if the event was eaten
        public virtual bool respondToEnteredMessage(SteamID roomID, SteamID userID)
        {
            return false;
        }

        // Return true if the event was eaten
        public virtual bool respondToBan(SteamID roomID, SteamID bannedId, SteamID bannerId)
        {
            return false;
        }

        // Return true if the event was eaten
        public virtual bool respondToDisconnect(SteamID roomID, SteamID userID)
        {
            return false;
        }

        // Return true if the event was eaten
        public virtual bool respondToLeftMessage(SteamID roomID, SteamID userID)
        {
            return false;
        }

        // Return true if the event was eaten
        public virtual bool respondToKick(SteamID roomID, SteamID kickedId, SteamID kickerId)
        {
            return false;
        }

        public virtual bool respondToAnnouncement(SteamID groupID, string headline)
        {
            return false;
        }

        public virtual bool respondToTradeSession(SteamID userID)
        {
            return false;
        }

        public virtual bool respondToTradeProposal(SteamID tradeId, SteamID steamId)
        {
            return false;
        }

        public virtual bool respondToTradeOffer(int number)
        {
            return false;
        }
        #endregion

        #region helper methods


        /// <summary>
        /// Sends a message to the specified SteamID
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="message"></param>
        /// <param name="room"></param>
        protected void SendMessageAfterDelay(SteamID steamID, string message, bool room)
        {
            int delay = 0;

            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        delay = Options.ChatCommand.TriggerNumbers.Delay == null ? 0 : Options.ChatCommand.TriggerNumbers.Delay.Value;
                        break;
                    case OptionType.ChatReply:
                        delay = Options.ChatReply.TriggerNumbers.Delay == null ? 0 : Options.ChatReply.TriggerNumbers.Delay.Value;
                        break;
                    case OptionType.NoCommand:
                        delay = Options.NoCommand.TriggerNumbers.Delay == null ? 0 : Options.NoCommand.TriggerNumbers.Delay.Value;
                        break;
                    case OptionType.ChatCommandAPI:
                        delay = Options.ChatCommandApi.ChatCommand.TriggerNumbers.Delay == null ? 0 : Options.ChatCommandApi.ChatCommand.TriggerNumbers.Delay.Value;
                        break;
                    case OptionType.JustLists:
                        delay = 0;
                        break;
                    case OptionType.AntiSpamTrigger:
                        delay = Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Delay == null ? 0 : Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Delay.Value;
                        break;
                    case OptionType.DiscordTrigger:
                        delay = Options.DiscordOptions.NoCommand.TriggerNumbers.Delay == null ? 0 : Options.DiscordOptions.NoCommand.TriggerNumbers.Delay.Value;
                        break;
                }
            }
            catch (NullReferenceException nfe) { }
            catch (Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (delay == 0)
            {
                Log.Instance.Silly("{0}/{1}: Sending nondelayed message to {2}: {3}", Bot.username, Name, steamID, message);
                if (room)
                {
                    Bot.steamFriends.SendChatRoomMessage(steamID, EChatEntryType.ChatMsg, message);
                }
                else
                {
                    Bot.steamFriends.SendChatMessage(steamID, EChatEntryType.ChatMsg, message);
                }
            }
            else
            {
                Log.Instance.Warn(delay.ToString());
                Log.Instance.Silly("{0}/{1}: Sending delayed message to {2}: {3}", Bot.username, Name, steamID, message);
                System.Timers.Timer timer = new System.Timers.Timer(delay);

                timer.Elapsed += (sender, e) => TimerElapsed_Message(sender, e, steamID, message, room);
            }
        }

        private void TimerElapsed_Message(object sender, ElapsedEventArgs e, SteamID steamID, string message, bool room)
        {
            if (room)
            {
                Bot.steamFriends.SendChatRoomMessage(steamID, EChatEntryType.ChatMsg, message);
            }
            else
            {
                Bot.steamFriends.SendChatMessage(steamID, EChatEntryType.ChatMsg, message);
            }
        }

        /// <summary>
        /// Splits the message and returns an array of words
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected string[] StripCommand(string message, string command)
        {
            if (message != null && command != null && message.ToLower().IndexOf(command.ToLower()) == 0)
            {
                return message.Split(' ');
            }
            return null;
        }

        /// <summary>
        /// Check to see if the user or room is on the ignore list
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="fromID"></param>
        /// <returns></returns>
        protected bool CheckIgnores(SteamID toID, SteamID fromID)
        {
            List<SteamID> ignore = new List<SteamID>();

            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        ignore = Options.ChatCommand.TriggerLists.Ignore;
                        break;
                    case OptionType.ChatReply:
                        ignore = Options.ChatReply.TriggerLists.Ignore;
                        break;
                    case OptionType.NoCommand:
                        ignore = Options.NoCommand.TriggerLists.Ignore;
                        break;
                    case OptionType.ChatCommandAPI:
                        ignore = Options.ChatCommandApi.ChatCommand.TriggerLists.Ignore;
                        break;
                    case OptionType.JustLists:
                        ignore = Options.TriggerLists.Ignore;
                        break;
                    case OptionType.ListsAndNumbers:
                        ignore = Options.TriggerLists.Ignore;
                        break;
                    case OptionType.AntiSpamTrigger:
                        ignore = Options.AntiSpamTriggerOptions.NoCommand.TriggerLists.Ignore;
                        break;
                    case OptionType.DiscordTrigger:
                        ignore = Options.DiscordOptions.NoCommand.TriggerLists.Ignore;
                        break;
                }
            }
            catch (NullReferenceException nfe) { }
            catch (Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (ignore != null && ignore.Count > 0)
            {
                for (int i = 0; i < ignore.Count; i++)
                {
                    SteamID ignored = ignore[i];
                    if (toID == ignored || fromID == ignored)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the room is on the whitelist
        /// </summary>
        /// <param name="toID"></param>
        /// <returns></returns>
        protected bool CheckRoom(SteamID toID)
        {
            List<SteamID> rooms = new List<SteamID>();

            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        rooms = Options.ChatCommand.TriggerLists.Rooms;
                        break;
                    case OptionType.ChatReply:
                        rooms = Options.ChatReply.TriggerLists.Rooms;
                        break;
                    case OptionType.NoCommand:
                        rooms = Options.NoCommand.TriggerLists.Rooms;
                        break;
                    case OptionType.ChatCommandAPI:
                        rooms = Options.ChatCommandApi.ChatCommand.TriggerLists.Rooms;
                        break;
                    case OptionType.JustLists:
                        rooms = Options.TriggerLists.Rooms;
                        break;
                    case OptionType.ListsAndNumbers:
                        rooms = Options.TriggerLists.Rooms;
                        break;
                    case OptionType.AntiSpamTrigger:
                        rooms = Options.AntiSpamTriggerOptions.NoCommand.TriggerLists.Rooms;
                        break;
                    case OptionType.DiscordTrigger:
                        rooms = Options.DiscordOptions.NoCommand.TriggerLists.Rooms;
                        break;
                }
            }
            catch (NullReferenceException nfe) { }
            catch (Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (rooms == null || rooms.Count == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    SteamID room = rooms[i];
                    if (toID == room)
                    {
                        return true;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Check to see if the user is on the whitelist
        /// </summary>
        /// <param name="fromID"></param>
        /// <returns></returns>
        protected bool CheckUser(SteamID fromID)
        {
            List<SteamID> users = new List<SteamID>();
            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        users = Options.ChatCommand.TriggerLists.User;
                        break;
                    case OptionType.ChatReply:
                        users = Options.ChatReply.TriggerLists.User;
                        break;
                    case OptionType.NoCommand:
                        users = Options.NoCommand.TriggerLists.User;
                        break;
                    case OptionType.ChatCommandAPI:
                        users = Options.ChatCommandApi.ChatCommand.TriggerLists.User;
                        break;
                    case OptionType.JustLists:
                        users = Options.TriggerLists.User;
                        break;
                    case OptionType.ListsAndNumbers:
                        users = Options.TriggerLists.User;
                        break;
                    case OptionType.AntiSpamTrigger:
                        users = Options.AntiSpamTriggerOptions.NoCommand.TriggerLists.User;
                        break;
                    case OptionType.DiscordTrigger:
                        users = Options.DiscordOptions.NoCommand.TriggerLists.User;
                        break;
                }
            }
            catch (NullReferenceException nfe) { }
            catch (Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (users != null && users.Count > 0)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    SteamID user = users[i];
                    if (fromID == user)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Randomly decides if a message will be sent
        /// </summary>
        /// <returns></returns>
        protected bool RandomRoll()
        {
            float prob = 1;
            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        prob = Options.ChatCommand.TriggerNumbers.Probability == null ? 1 : Options.ChatCommand.TriggerNumbers.Probability.Value;
                        break;
                    case OptionType.ChatReply:
                        prob = Options.ChatReply.TriggerNumbers.Probability == null ? 1 : Options.ChatReply.TriggerNumbers.Probability.Value;
                        break;
                    case OptionType.NoCommand:
                        prob = Options.NoCommand.TriggerNumbers.Probability == null ? 1 : Options.NoCommand.TriggerNumbers.Probability.Value;
                        break;
                    case OptionType.ChatCommandAPI:
                        prob = Options.ChatCommandApi.ChatCommand.TriggerNumbers.Probability == null ? 1 : Options.ChatCommandApi.ChatCommand.TriggerNumbers.Probability.Value;
                        break;
                    case OptionType.JustLists:
                        prob = 1;
                        break;
                    case OptionType.AntiSpamTrigger:
                        prob = Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Probability == null ? 1 : Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Probability.Value;
                        break;
                    case OptionType.DiscordTrigger:
                        prob = Options.DiscordOptions.NoCommand.TriggerNumbers.Probability == null ? 1 : Options.DiscordOptions.NoCommand.TriggerNumbers.Probability.Value;
                        break;

                }
            }
            catch (NullReferenceException nfe) { }
            catch(Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (prob != 1)
            {
                double rng = new Random().NextDouble();
                if (rng > prob)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Disables bot reply for the duration of the timeout
        /// </summary>
        protected void DisableForTimeout()
        {
            int to = 0;

            try
            {
                switch (OptionsType)
                {
                    case OptionType.ChatCommand:
                        to = Options.ChatCommand.TriggerNumbers.Timeout == null ? 0 : Options.ChatCommand.TriggerNumbers.Timeout.Value;
                        break;
                    case OptionType.ChatReply:
                        to = Options.ChatReply.TriggerNumbers.Timeout == null ? 0 : Options.ChatReply.TriggerNumbers.Timeout.Value;
                        break;
                    case OptionType.NoCommand:
                        to = Options.NoCommand.TriggerNumbers.Timeout == null ? 0 : Options.NoCommand.TriggerNumbers.Timeout.Value;
                        break;
                    case OptionType.ChatCommandAPI:
                        to = Options.ChatCommandApi.ChatCommand.TriggerNumbers.Timeout == null ? 0 : Options.ChatCommandApi.ChatCommand.TriggerNumbers.Timeout.Value;
                        break;
                    case OptionType.JustLists:
                        to = 0;
                        break;
                    case OptionType.AntiSpamTrigger:
                        to = Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Timeout == null ? 0 : Options.AntiSpamTriggerOptions.NoCommand.TriggerNumbers.Timeout.Value;
                        break;
                    case OptionType.DiscordTrigger:
                        to = Options.DiscordOptions.NoCommand.TriggerNumbers.Timeout == null ? 0 : Options.DiscordOptions.NoCommand.TriggerNumbers.Timeout.Value;
                        break;
                }
            }
            catch (NullReferenceException nfe) { }
            catch (Exception e)
            {
                Log.Instance.Error("{0}/{1}: {2}", Bot.username, Name, e.StackTrace);
            }

            if (to > 0)
            {
                ReplyEnabled = false;
                Log.Instance.Silly("{0}/{1}: Setting timeout ({2} ms)", Bot.username, Name, to);
                System.Timers.Timer timer = new System.Timers.Timer(to);
                timer.Elapsed += (sender, e) => AfterTimer_Timeout(sender, e);
            }
        }

        private void AfterTimer_Timeout(object sender, ElapsedEventArgs e)
        {
            Log.Instance.Silly("{0}/{1}: Timeout expired", Bot.username, Name);
            ReplyEnabled = true;
        }
        #endregion

    }
}
