using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class NotificationOptions
    {
        public string Name { get; set; }
        public NoCommand NoCommand { get; set; }
        public string APICommand { get; set; }
        public string SeenCommand { get; set; }
        public string FilterCommand { get; set; }
        public string ClearCommand { get; set; }
        public string DBFile { get; set; }
        public int SaveTimer { get; set; }
        public Dictionary<ulong, DB> DB { get; set; }
    }

    public class DB
    {
        public ulong userID { get; set; }
        public string name { get; set; }
        public DateTime seen { get; set; }
        public PushBullet pb { get; set; }
    }

    public class PushBullet
    {
        public string apikey { get; set; }
        public List<string> filter { get; set; }
    }
}
