using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using steam_chat_bot_net;
using SteamKit2;

namespace steam_chat_bot_net.src.triggers
{
    public class Options
    {
        public string Type { get; set; }
        public SteamChatBot ChatBot { get; set; }
        public string Name { get; set; }
        public List<string> Admins { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Command { get; set; }
    }

    abstract class BaseTrigger
    {
        private Options _options;
        public Options options { get { return _options; } set { _options = value; } }

        public virtual bool OnLoad()
        {
            try
            {
                return true;
            }
            catch(Exception e)
            {
                Log.Instance.Error(e.StackTrace);
                return false;
            }
        }

        public abstract bool RespondToChatMessage(SteamID roomID, SteamID chatterID, string message);

        public abstract bool ResponseToFriendMessage(SteamID userID, string message);

        public abstract bool Respond(SteamID toID, SteamID userID, string message);

        public abstract string[] StripCommand(string message, string command);

        public virtual void SendMessageAfterDelay(SteamID steamID, string message)
        {
            Options options = new Options();
            Log.Instance.Silly(options.ChatBot.Name + "/" + options.Name + ": Sending nondelayed message to " + steamID + ": " + message);
            return true;
        }
    }
}
