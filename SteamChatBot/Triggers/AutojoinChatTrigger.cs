using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

namespace SteamChatBot.Triggers
{
    class AutojoinChatTrigger : BaseTrigger
    {
        public AutojoinChatTrigger(TriggerType type, string name, TriggerOptions options) : base(type, name, options)
        { }

        public override bool OnLoggedOn()
        {
            if(Options.Rooms.Count == 0 || Options.Rooms == null)
            {
                Log.Instance.Error("Must have rooms!");
                return false;
            }
            else
            {
                foreach (SteamID roomID in Options.Rooms)
                {
                    Log.Instance.Verbose("Joining chat room " + roomID.ToString());
                    Bot.steamFriends.JoinChat(roomID);
                }
                return true;
            }
        }


    }
}
