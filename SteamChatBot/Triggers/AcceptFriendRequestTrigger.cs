using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class AcceptFriendRequestTrigger : BaseTrigger
    {
        public AcceptFriendRequestTrigger(TriggerType type, string name, TriggerOptionsBase tl) : base(type, name, tl)
        { }

        public override bool respondToFriendRequest(SteamID userID)
        {
            Bot.steamFriends.AddFriend(userID);
            return true;
        }
    }
}
