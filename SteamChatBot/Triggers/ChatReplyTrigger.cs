using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class ChatReplyTrigger : BaseTrigger
    {
        public ChatReplyTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, userID, message, false);
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterID, string message)
        {
            return Respond(roomID, chatterID, message, true);
        }

        private bool Respond(SteamID toID, SteamID userID, string message, bool room)
        {
            if(CheckMessage(message) != false)
            {
                string response = PickResponse();
                SendMessageAfterDelay(toID, response, room);
                return true;
            }
            return false;
        }

        private bool CheckMessage(string message)
        {
            if(Options.ChatReply.Matches != null && Options.ChatReply.Matches.Count > 0)
            {
                for (int i = 0; i < Options.ChatReply.Matches.Count; i++)
                {
                    string match = Options.ChatReply.Matches[i];
                    if (message.ToLower() == match.ToLower())
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        private string PickResponse()
        {
            if(Options.ChatReply.Responses != null && Options.ChatReply.Responses.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, Options.ChatReply.Responses.Count);
                return Options.ChatReply.Responses[index];
            }
            return "";
        }
    }
}
