using System.Collections.Generic;

using SteamKit2;

namespace SteamChatBot.Triggers
{
    public class TriggerOptions
    {
        public int? Delay { get; set; }
        public float? Probability { get; set; }
        public int? Timeout { get; set; }
        public List<SteamID> Ignore { get; set; }
        public List<SteamID> User { get; set; }
        public List<SteamID> Rooms { get; set; }
        public string Command { get; set; }
        public List<string> Matches { get; set; }
        public List<string> Responses { get; set; }
    }
}
