using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using SteamKit2;
using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class DiscordTrigger : BaseTrigger
    {
        private DiscordClient client;
        
        public DiscordTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool onLoggedOn()
        {
            client = new DiscordClient();
            client.Connect(Options.DiscordOptions.Token);

            client.Ready += Client_Ready;
            client.MessageReceived += Client_MessageReceived;
            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
            client.ChannelUpdated += Client_ChannelUpdated;
            client.UserUpdated += Client_UserUpdated;
            client.UserBanned += Client_UserBanned;
            
            return true;
        }

        private void Client_UserBanned(object sender, UserEventArgs e)
        {
            if(e.Server.Id == Options.DiscordOptions.DiscordServerID)
            {
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, string.Format("{0} was banned from Discord", e.User.Name), true);
            }
        }

        /**
         * Discord Section
         */

        private void Client_UserLeft(object sender, UserEventArgs e)
        {
            if(e.Server.Id == Options.DiscordOptions.DiscordServerID)
            {
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, string.Format("{0} has left Discord", e.User.Name), true);
            }
        }

        private void Client_UserJoined(object sender, UserEventArgs e)
        {
            if(e.Server.Id == Options.DiscordOptions.DiscordServerID)
            {
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, string.Format("{0} has joined Discord", e.User.Name), true);
            }
        }

        private void Client_ChannelUpdated(object sender, ChannelUpdatedEventArgs e)
        {
            if(e.Server.Id == Options.DiscordOptions.DiscordServerID && e.After.Topic != "" && e.After.Topic != e.Before.Topic)
            {
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, "The topic in Discord has been changed to:\n" + e.After.Topic, true);
            }
        }

        private void Client_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if(e.After.Name != e.Before.Name)
            { 
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, string.Format("{0} has changed their name in Discord to {1}", e.Before.Name, e.After.Name), true);
            }
        }

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Channel.Id == Options.DiscordOptions.DiscordServerID && !e.Message.IsAuthor)
            {
                SendMessageAfterDelay(Options.DiscordOptions.SteamChat, string.Format("{0} sent a message in Discord: {1}", e.Message.User.Name, e.Message.Text), true);
            }
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.Instance.Verbose(Bot.username + "/" + Name + ": Connected to Discord with sessionID " + client.SessionId);
            SendMessageAfterDelay(Options.DiscordOptions.SteamChat, "Connected to Discord as " + client.CurrentUser.Name, true);
        }

        /**
         * Steam Section
         */

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            SendSteamMessage(message, chatterId);
            return true;
        }

        public override bool respondToEnteredMessage(SteamID roomID, SteamID userID)
        {
            SendSteamAction("joined Steam", userID);
            return true;
        }

        public override bool respondToLeftMessage(SteamID roomID, SteamID userID)
        {
            SendSteamAction("left Steam", userID);
            return true;
        }

        public override bool respondToKick(SteamID roomID, SteamID kickedId, SteamID kickerId)
        {
            SendSteamAction("was kicked from Steam", kickedId, kickerId);
            return true;
        }

        public override bool respondToBan(SteamID roomID, SteamID bannedId, SteamID bannerId)
        {
            SendSteamAction("was banned from Steam", bannedId, bannerId);
            return true;
        }
        
        private void SendSteamMessage(string message, SteamID userID)
        {
            Channel channel = client.GetChannel(Options.DiscordOptions.DiscordServerID);
            channel.SendMessage(string.Format("** {0}: {1} **", Bot.steamFriends.GetFriendPersonaName(userID), message));
        }

        private void SendSteamAction(string action, SteamID userID)
        {
            Channel channel = client.GetChannel(Options.DiscordOptions.DiscordServerID);
            channel.SendMessage(string.Format("** {0} {1} **", Bot.steamFriends.GetFriendPersonaName(userID), action));
        }

        private void SendSteamAction(string action, SteamID userID, SteamID fromID)
        {
            Channel channel = client.GetChannel(Options.DiscordOptions.DiscordServerID);
            channel.SendMessage(string.Format("** {0} {1} by {2} **", Bot.steamFriends.GetFriendPersonaName(userID), action, Bot.steamFriends.GetFriendPersonaName(fromID)));
        }
    }
}
