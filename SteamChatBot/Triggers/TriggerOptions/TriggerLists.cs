using System.Collections.Generic;
using SteamKit2;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class TriggerLists
    {
        public string Name { get; set; }
        public List<SteamID> Ignore { get; set; }
        public List<SteamID> User { get; set; }
        public List<SteamID> Rooms { get; set; }
    }
}
