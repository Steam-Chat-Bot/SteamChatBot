namespace SteamChatBot.Triggers.TriggerOptions
{
    public class ChatCommand
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public TriggerNumbers TriggerNumbers { get; set; }
        public TriggerLists TriggerLists { get; set; }
    }
}
