using SteamChatBot.Triggers.TriggerOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    public class ChooseTrigger : BaseTrigger
    {
        public ChooseTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message, true);
        }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, userID, message, false);
        }

        private bool Respond(SteamID toID, SteamID userID, string message, bool room)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if(query != null && query.Length > 2)
            {
                List<string> removed = new List<string>();
                for (int i = 1; i < query.Length; i++)
                {
                    removed.Add(query[i]);
                }
                Random rng = new Random();
                string choice = removed[rng.Next(0, removed.Count)];
                SendMessageAfterDelay(toID, "I have chosen " + choice, room);
                return true;
            }
            return false;
        }
    }
}
