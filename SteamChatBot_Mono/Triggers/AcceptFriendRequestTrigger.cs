using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot_Mono.Triggers
{
    class AcceptFriendRequestTrigger : BaseTrigger
    {
        public AcceptFriendRequestTrigger(TriggerType type, string name) : base(type, name)
        { }

        public override bool respondToFriendRequest(SteamID userID)
        {
            Bot.steamFriends.AddFriend(userID);
            return true;
        }
    }
}
