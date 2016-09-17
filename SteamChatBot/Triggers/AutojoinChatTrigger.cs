using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class AutojoinChatTrigger : BaseTrigger
    {
        public AutojoinChatTrigger(TriggerType type, string name, TriggerOptionsBase tl) : base(type, name, tl)
        { }

        public override bool OnLoggedOn()
        {
            if(Options.TriggerLists.Rooms.Count == 0 || Options.TriggerLists.Rooms == null)
            {
                throw new InvalidOperationException("Must include rooms!");
            }
            else
            {
                foreach (SteamID roomID in Options.TriggerLists.Rooms)
                {
                    Log.Instance.Verbose("Joining chat room " + roomID.ToString());
                    Bot.steamFriends.JoinChat(roomID);
                }
                return true;
            }
        }


    }
}
