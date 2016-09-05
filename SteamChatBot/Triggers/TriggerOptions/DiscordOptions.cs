using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class DiscordOptions
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public SteamID SteamChat { get; set; }
        public ulong DiscordServerID { get; set; }
        public NoCommand NoCommand { get; set; }
    }
}
