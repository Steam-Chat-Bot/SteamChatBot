using SteamChatBot.Triggers.TriggerOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    class ChangeNameTrigger : BaseTrigger
    {
        public ChangeNameTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
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
            if (query != null && query.Length == 1)
            {
                SendMessageAfterDelay(toID, "Usage: " + Options.ChatCommand.Command + " <new name>", room);
                return true;
            }
            else if (query != null && query.Length >= 2)
            {
                string name = "";
                for (int i = 1; i < query.Length; i++)
                {
                    name += query[i] + " ";
                }
                name = name.Substring(0, name.Length - 1);

                Bot.steamFriends.SetPersonaName(name);
                Bot.displayName = name;
                Bot.WriteData();
                return true;
            }
            return false;
        }
    }
}
