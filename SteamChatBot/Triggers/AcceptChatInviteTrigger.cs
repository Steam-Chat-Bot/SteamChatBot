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
        public AcceptChatInviteTrigger(TriggerType type, string name, TriggerLists tl) : base(type, name, tl)
        { }

        public override bool respondToChatInvite(SteamID roomID, string roomName, SteamID inviterId)
        {
            if(Options.TriggerLists.Rooms == null)
            {
                Bot.steamFriends.JoinChat(roomID);
                return true;
            }
            else
            {
                if (Options.TriggerLists.Rooms.Contains(roomID))
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
}
