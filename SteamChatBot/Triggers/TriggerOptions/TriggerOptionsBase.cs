namespace SteamChatBot.Triggers.TriggerOptions
{
    public class TriggerOptionsBase
    {
        public TriggerType Type { get; set; }
        public string Name { get; set; }
        public ChatCommand ChatCommand { get; set; }
        public ChatCommandApi ChatCommandApi { get; set; }
        public ChatReply ChatReply { get; set; }
        public NoCommand NoCommand { get; set; }
        public TriggerLists TriggerLists { get; set; }
        public TriggerNumbers TriggerNumbers { get; set; }
        public AntiSpamTriggerOptions AntiSpamTriggerOptions { get; set; }
        public DiscordOptions DiscordOptions { get; set; }
        public NoteTriggerOptions NoteTriggerOptions { get; set; }
        public NotificationOptions NotificationOptions { get; set; }
    }
}
