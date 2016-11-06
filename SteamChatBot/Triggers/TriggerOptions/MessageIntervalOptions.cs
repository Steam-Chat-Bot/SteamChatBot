using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class MessageIntervalOptions
    {
        public string Name { get; set; }
        public int Interval { get; set; }
        public List<SteamID> Destinations { get; set; }
        public string Message { get; set; }
    }
}
