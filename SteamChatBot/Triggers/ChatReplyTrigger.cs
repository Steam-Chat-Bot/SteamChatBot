using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    class ChatReplyTrigger : BaseTrigger
    {
        public ChatReplyTrigger(TriggerType type, string name, List<string> matches, List<string> responses) : base(type, name, matches, responses)
        { }

        public override bool OnFriendMessage(SteamID userID, string message, bool haveSentMessage)
        {
            return Respond(userID, userID, message, false);
        }

        public override bool OnChatMessage(SteamID roomID, SteamID chatterID, string message, bool haveSentMessage)
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
            if(Matches == null || Matches.Count == 0)
            {
                return true;
            }
            for (int i = 0; i < Matches.Count; i++)
            {
                string match = Matches[i];
                if(message.ToLower() == match.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private string PickResponse()
        {
            if(Responses != null && Responses.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, Responses.Count);
                Console.WriteLine(index);
                return Responses[index];
            }
            return "";
        }
    }
}
