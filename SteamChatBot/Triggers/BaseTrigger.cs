using System;
using System.Collections.Generic;
using SteamKit2;
using SteamChatBot;

namespace SteamChatBot.Triggers
{
    public class BaseTrigger
    {
        public TriggerType Type { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string UserString { get; set; }

        public BaseTrigger(TriggerType type, string name)
        {
            Type = type;
            Name = name;
        }
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
            try
            {
                bool messageSent = respondToFriendMessage(userID, message);
                if (messageSent)
                {
                    Log.Instance.Silly("{0}/{1}: Sent RespondToFriendMessage - {2} - {3}", Bot.username, Name, userID, message);
                }
                return messageSent;
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
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
            try
            {
                bool messageSent = respondToChatMessage(roomID, chatterID, message);
                if (messageSent)
                {
                    Log.Instance.Silly("{0}/{1}: Sent RespondToChatMessage - {2} - {3} - {4}", Bot.username, Name, chatterID, roomID, message);
                }
                return messageSent;
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
        /// <param name="userID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnEnteredChat(SteamID roomID, SteamID userID, bool haveSentMessage)
        {
            try
            {
                return respondToEnteredMessage(roomID, userID);
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
        /// <param name="kickedID"></param>
        /// <param name="kickerID"></param>
        /// <param name="haveSentMessage"></param>
        /// <returns></returns>
        public virtual bool OnKickedChat(SteamID roomID, SteamID kickedID, SteamID kickerID, bool haveSentMessage)
        {
            try
            {
                return respondToKick(roomID, kickedID, kickerID);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
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
            try
            {
                return respondToBan(roomID, bannedID, bannerID);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
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
            try
            {
                return respondToDisconnect(roomID, userID);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
        }

        public virtual bool OnLeftChat(SteamID roomID, SteamID userID)
        {
            try
            {
                return respondToEnteredMessage(roomID, userID);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
                return false;
            }
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

        protected void SendMessageAfterDelay(SteamID steamID, string message)
        {
            try
            {
                Log.Instance.Silly("{0}/{1}: Sending message to {2}: {3}", Bot.username, Name, steamID, message);
                Bot.steamFriends.SendChatRoomMessage(steamID, EChatEntryType.ChatMsg, message);
            }
            catch (Exception e)
            {
                Log.Instance.Error(IfError(Bot.username, Name, e.StackTrace));
            }
        }

        /*
        protected bool checkIgnores(SteamID toID, SteamID fromID)
        {
            if (Options.Ignore != null && Options.Ignore.Count > 0)
            {
                for (int i = 0; i < Options.Ignore.Count; i++)
                {
                    SteamID ignored = Options.Ignore[i];
                    if (toID == ignored || fromID == ignored)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        protected bool checkRoom(SteamID toID)
        {
            if (Options.Room != null && Options.Room.Count > 0)
            {
                for (int i = 0; i < Options.Room.Count; i++)
                {
                    SteamID room = Options.Room[i];
                    if (toID == room)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        protected bool checkUser(SteamID fromID)
        {
            if (Options.User != null && Options.User.Count > 0)
            {
                for (int i = 0; i < Options.User.Count; i++)
                {
                    SteamID user = Options.User[i];
                    if (fromID == user)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        */

        #endregion

    }
}
