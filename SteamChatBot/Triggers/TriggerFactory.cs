using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamChatBot.Triggers
{
    public class TriggerFactory
    {
        public Dictionary<string, TriggerType> TriggerTypes { get; set; }

        public TriggerFactory(Dictionary<string, TriggerType> triggerTypes)
        {
            TriggerTypes = triggerTypes;
        }

        public BaseTrigger CreateTrigger(TriggerType type, string name)
        {
            if(TriggerTypes.ContainsKey(type.ToString()))
            {
                return new BaseTrigger(type, name);
            }
            return null;
        }
    }
}
