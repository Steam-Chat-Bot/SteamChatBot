using System.Collections.Generic;
using SteamKit2;

namespace SteamChatBot.Triggers.TriggerOptions
{
    public class NoteTriggerOptions
    {
        public string Name { get; set; }
        public string NoteCommand { get; set; }
        public string InfoCommand { get; set; }
        public string DeleteCommand { get; set; }
        public NoCommand NoCommand { get; set; }
        public Dictionary<SteamID, Dictionary<string, Note>> Notes { get; set; }
    }

    public class Note
    {
        public string ModifiedWhen { get; set; }
        public string ModifiedBy { get; set; }
        public string Definition { get; set; }
    }
}
