using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace steam_chat_bot_net.Triggers
{
    public abstract class BaseTrigger
    {
        public SteamChatBot ChatBot { get; private set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public bool RespectsMute { get; set; }
        public Log Log { get; set; }
        public List<SteamID> Admins { get; set; }
        public string UserName { get; set; }
        public string UserString { get; set; }

        /// <summary>
        /// If there is an error, log it easily
        /// </summary>
        /// <param name="cbn"></param>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected string IfError(string cbn, string name, string error)
        {
            return string.Format("{0}/{1}: Error: {2}", cbn, name, error);
        }

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
                    Log.Instance.Error("{0}/{1}: Error loading trigger {2}: OnLoad returned {3}", ChatBot.Name, Name, Name, ret);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if (checkUser(inviterID) && checkRoom(roomID) && !checkIgnores(roomID, inviterID))
            {
                try
                {
                    return respondToChatInvite(roomID, roomName, inviterID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
                    return false;
                }
            }
            else
            {
                return false;
            }
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
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if(checkUser(userID) && !checkIgnores(userID))
            {
                try
                {
                    bool messageSent = respondToFriendMessage(userID, message);
                    if (messageSent)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToFriendMessage - {2} - {3}", ChatBot.Name, Name, userID, message);
                    }
                    return messageSent;
                }
                catch(Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
                    Log.Instance.Silly("{0}/{1}: Sent RespondToTradeOffer: {2}", ChatBot.Name, Name, number);
                }
                return eventEaten;
            }
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            try {
                if (checkUser(userID) && !checkIgnores(userID))
                {
                    bool eventEaten = respondToTradeProposal(tradeID, userID);
                    if (eventEaten)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToTradeProposal: {2}", ChatBot.Name, Name, tradeID);
                    }
                    return eventEaten;
                }
            }
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
                return false;
            }
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
                if (checkUser(userID) && !checkIgnores(userID))
                {
                    bool eventEaten = respondToTradeSession(userID);
                    if (eventEaten)
                    {
                        Log.Instance.Silly("{0}/{1}: Opened trade with: {2}", ChatBot.Name, Name, userID);
                    }
                    return eventEaten;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            try
            {
                if (!checkIgnores(groupID))
                {
                    bool eventEaten = respondToAnnouncement(groupID, headline);
                    if (eventEaten)
                    {
                        Log.Instance.Silly("{0}/{1}: responded to {1}'s announcement: {2}", ChatBot.Name, Name, groupID, headline);
                    }
                    return eventEaten;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
                return false;
            }
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
                bool messageSeen = respondToSendMessage(toID, message);
                if (messageSeen)
                {
                    return true;
                }
                return messageSeen;
            }
            catch(Exception e)
            {
                Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if(checkUser(chatterID) && checkRoom(roomID) && !checkIgnores(chatterID, roomID))
            {
                try
                {
                    bool messageSent = respondToChatMessage(roomID, chatterID, message);
                    if (messageSent)
                    {
                        Log.Instance.Silly("{0}/{1}: Sent RespondToChatMessage - {2} - {3} - {4}", ChatBot.Name, Name, chatterID, roomID, message);
                    }
                    return messageSent;
                }
                catch(Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if (checkUser(chatterID) && checkRoom(roomID) && !checkIgnores(userID, roomID))
            {
                try
                {
                    return respondToEnteredMessage(roomID, userID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if (checkRoom(roomID))
            {
                try
                {
                    return respondToKick(roomID, kickedID, kickerID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if (checkRoom(roomID))
            {
                try
                {
                    return respondToBan(roomID, bannedID, bannerID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
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
            if (checkRoom(roomID) && !checkIgnores(userID, roomID))
            {
                try
                {
                    return respondToDisconnect(roomID, userID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }

        public virtual bool OnLeftChat(SteamID roomID, SteamID userID)
        {
            if (checkUser(userID) && checkRoom(roomID) && !checkIgnores(roomID, userID))
            {
                try
                {
                    return respondToEnteredMessage(roomID, userID);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(IfError(ChatBot.Name, Name, e.StackTrace));
                    return false;
                }
            }
            return false;
        }


    }
}
