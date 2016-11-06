using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class AcceptChatInviteTrigger : BaseTrigger
    {
        public AcceptChatInviteTrigger(TriggerType type, string name, TriggerOptionsBase tl) : base(type, name, tl)
        { }

        public override bool respondToChatInvite(SteamID roomID, string roomName, SteamID inviterId)
        {
            if (Options.TriggerLists.Rooms.Contains(roomID) || Options.TriggerLists.Rooms == null || Options.TriggerLists.Rooms.Count == 0)
            {
                Bot.steamFriends.JoinChat(roomID);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
