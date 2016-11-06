using SteamChatBot.Triggers.TriggerOptions;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.ComponentModel;

namespace SteamChatBot.Triggers
{
    class MessageIntervalTrigger : BaseTrigger
    {
        public MessageIntervalTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool onLoad()
        {
            Timer t = new Timer(Options.MessageIntervalOptions.Interval);
            t.Elapsed += T_Elapsed;
            t.Enabled = true;
            return true;
        }
       
        private void T_Elapsed(object sender, EventArgs e)
        {
            try
            {
                foreach(SteamID destination in Options.MessageIntervalOptions.Destinations)
                {
                    SendMessageAfterDelay(destination, Options.MessageIntervalOptions.Message, true);
                }
            }
            catch(Exception err)
            {
                Log.Instance.Error(err.StackTrace);
            }
        }
    }
}
