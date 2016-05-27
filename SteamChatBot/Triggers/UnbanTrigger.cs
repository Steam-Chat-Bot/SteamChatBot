using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class UnbanTrigger : BaseTrigger
    {
        public UnbanTrigger(TriggerType type, string name, ChatCommand options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message);
        }

        private bool Respond(SteamID roomID, SteamID userID, string message)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if (query != null && query[1] != null)
            {
                Bot.steamFriends.UnbanChatMember(roomID, new SteamID(Convert.ToUInt64(query[1])));
                return true;
            }
            return false;
        }
    }
}
