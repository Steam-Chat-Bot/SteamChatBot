using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class DoormatTrigger : BaseTrigger
    {
        public DoormatTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool respondToEnteredMessage(SteamID roomID, SteamID userID)
        {
            return SendGreeting(roomID, userID);
        }

        private bool SendGreeting(SteamID groupID, SteamID userID)
        {
            SendMessageAfterDelay(groupID, "Hello " + Bot.steamFriends.GetFriendPersonaName(userID), true);
            return true;
        }
    }
}
