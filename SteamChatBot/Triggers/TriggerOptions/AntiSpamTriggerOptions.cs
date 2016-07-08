using SteamKit2;
using System.Collections.Generic;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class AntiSpamTriggerOptions
    {
        public string Name { get; set; }
        public NoCommand NoCommand { get; set; }
        public List<SteamID> admins { get; set; }
        public Dictionary<SteamID, Dictionary<SteamID, int>> groups { get; set; }
        public Score score { get; set; }
        public Timers timers { get; set; }
        public PTimer ptimer { get; set; }
        public string warnMessage { get; set; }
        public int msgPenalty { get; set; }
        //public Dictionary<string, int> badWords { get; set; } // TODO: Add badwords
    }

    public class Score
    {
        public int warn { get; set; }
        public int warnMax { get; set; }
        public int kick { get; set; }
        public int ban { get; set; }
        public int tattle { get; set; }
        public int tattleMax { get; set; }
    }

    public class Timers
    {
        public int messages { get; set; }
        public int unban { get; set; }
    }

    public class PTimer
    {
        public int resolution { get; set; }
        public int amount { get; set; }
    }
}
