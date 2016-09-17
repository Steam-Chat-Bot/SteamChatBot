using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class BanTrigger : BaseTrigger
    {
        public BanTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
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
                Bot.steamFriends.BanChatMember(roomID, new SteamID(Convert.ToUInt64(query[1])));
                return true;
            }
            return false;
        }
    }
}
