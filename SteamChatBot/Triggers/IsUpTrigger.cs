using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    class IsUpTrigger : BaseTrigger
    {
        public IsUpTrigger(TriggerType type, string name) : base(type, name)
        { }

        public override bool OnFriendMessage(SteamID userID, string message, bool haveSentMessage)
        {
            SendMessageAfterDelay(userID, message);
            return true;
        }

    }
}
