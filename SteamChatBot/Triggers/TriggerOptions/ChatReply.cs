using System.Collections.Generic;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class ChatReply
    {
        public string Name { get; set; }
        public List<string> Matches { get; set; }
        public List<string> Responses { get; set; }
        public TriggerNumbers TriggerNumbers { get; set; }
        public TriggerLists TriggerLists { get; set; }
    }
}
