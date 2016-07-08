using SteamChatBot.Triggers.TriggerOptions;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SteamChatBot.Triggers
{
    class AntispamTrigger : BaseTrigger
    {
        public AntispamTrigger(TriggerType type, string name, AntiSpamTriggerOptions options) : base(type, name, options)
        {
            options.groups = new Dictionary<SteamID, Dictionary<SteamID, int>>();
            Timer reducePenT = new Timer(options.ptimer.resolution);
            reducePenT.Elapsed += ReducePenT_Tick;
            reducePenT.Start();
        }

        private void ReducePenT_Tick(object sender, EventArgs e)
        {
            ReducePenalties();
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message);
        }

        private bool Respond(SteamID toID, SteamID userID, string message)
        {
            Dictionary<SteamID, Dictionary<SteamID, int>> groups = Options.AntiSpamTriggerOptions.groups;
            AntiSpamTriggerOptions options = Options.AntiSpamTriggerOptions;

            if(!groups.ContainsKey(toID))
            {
                groups[toID] = new Dictionary<SteamID, int>();
            }
            if(!groups[toID].ContainsKey(userID))
            {
                groups[toID][userID] = 0;
            }

            groups[toID][userID] += options.msgPenalty;
            
            if(groups[toID][userID] >= options.score.warn && groups[toID][userID] <= options.score.warnMax)
            {
                Log("warning", userID, toID);
                SendMessageAfterDelay(userID, options.warnMessage, false);
                return true;
            }
            else if(groups[toID][userID] >= options.score.kick)
            {
                Log("kicking", userID, toID);
                Bot.steamFriends.KickChatMember(toID, userID);
                return true;
            }
            else if (groups[toID][userID] >= options.score.ban)
            {
                Log("banning", userID, toID);
                Timer unban = new Timer(options.timers.unban);
                unban.Elapsed += new ElapsedEventHandler((sender, e) => Unban_Elapsed(sender, e, toID, userID));
                return true;
            }
            else if (groups[toID][userID] >= options.score.tattle && groups[toID][userID] <= options.score.tattleMax)
            {
                Log("tattling on", userID, toID);
                foreach(SteamID admin in options.admins)
                {
                    SendMessageAfterDelay(admin, Bot.steamFriends.GetFriendPersonaName(userID) + " is spamming in https://steamcommunity.com/gid/" + toID.ToString(), false);
                }
                return true;
            }
            return false;
        }

        #region timers

        private void Unban_Elapsed(object sender, ElapsedEventArgs e, SteamID toID, SteamID userID)
        {
            Log("**UNbanning**", userID, toID, "timeout");
            Bot.steamFriends.UnbanChatMember(toID, userID);
        }

        /// <summary>
        /// Reduces the score of every user by (options.ptimer.amount) every (options.ptimer.resolution) ms
        /// </summary>
        private void ReducePenalties()
        {
            foreach(Dictionary<SteamID, int> users in Options.AntiSpamTriggerOptions.groups.Values)
            {
                try
                {
                    foreach (SteamID user in users.Keys)
                    {
                        users[user] -= Options.AntiSpamTriggerOptions.ptimer.amount;
                        if (users[user] <= 0)
                        {
                            users.Remove(user);
                            return;
                        }
                    }
                }
                catch (Exception e) { return; }
            }
        }

        #endregion

        /// <summary>
        /// Logs bot actions to the console
        /// </summary>
        /// <param name="prefix">Action that was taken</param>
        /// <param name="userID">The user to act upon</param>
        /// <param name="groupID">The group the action is taking place in</param>
        /// <param name="reason">Reason they had action taken upon them</param>
        private void Log(string prefix, SteamID userID, SteamID groupID)
        {
            string message = string.Format("{0}/{1}: {2} {3} for spamming in https://steamcommunity.com/gid/{4}", Bot.username, Name, prefix, Bot.steamFriends.GetFriendPersonaName(userID), groupID.ToString());
            SteamChatBot.Log.Instance.Info(message);
        }

        /// <summary>
        /// Logs bot actions to the console
        /// </summary>
        /// <param name="prefix">Action that was taken</param>
        /// <param name="userID">The user to act upon</param>
        /// <param name="groupID">The group the action is taking place in</param>
        /// <param name="reason">Reason they had action taken upon them</param>
        private void Log(string prefix, SteamID userID, SteamID groupID, string reason)
        {
            string message = string.Format("{0}/{1}: {2} {3} for spamming in https://steamcommunity.com/gid/{4} to prevent spam ({5})", Bot.username, Name, prefix, Bot.steamFriends.GetFriendPersonaName(userID), groupID.ToString(), reason);
            SteamChatBot.Log.Instance.Info(message);
        }
    }
}
