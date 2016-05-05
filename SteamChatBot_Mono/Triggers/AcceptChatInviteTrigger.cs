using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot_Mono.Triggers
{
    class AcceptChatInviteTrigger : BaseTrigger
    {
        public AcceptChatInviteTrigger(TriggerType type, string name, TriggerOptions options) : base(type, name, options)
        { }

        public override bool respondToChatInvite(SteamID roomID, string roomName, SteamID inviterId)
        {
            if(Options.Rooms == null)
            {
                Bot.steamFriends.JoinChat(roomID);
                return true;
            }
            else
            {
                if (Options.Rooms.Contains(roomID))
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
