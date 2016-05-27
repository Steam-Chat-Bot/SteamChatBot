using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class LeaveChatTrigger : BaseTrigger
    {
        public LeaveChatTrigger(TriggerType type, string name, ChatCommand options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message);
        }

        private bool Respond(SteamID roomID, SteamID userID, string message)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if(query != null)
            {
                Bot.steamFriends.LeaveChat(roomID);
                return true;
            }
            return false;
        }
    }
}
