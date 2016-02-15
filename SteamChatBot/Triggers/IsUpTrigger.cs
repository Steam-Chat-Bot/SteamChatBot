using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SteamKit2;

namespace SteamChatBot.Triggers
{
    class IsUpTrigger : BaseTrigger
    {
        public IsUpTrigger(TriggerType type, string name, TriggerOptions options) : base(type, name, options)
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
            string[] query = StripCommand(message, Options.Command);
            if (query != null)
            {
                HttpWebResponse response;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query[1]);
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException e)
                {
                    response = ((HttpWebResponse)e.Response);
                }
                SendMessageAfterDelay(toID, response.StatusCode.ToString(), room);
                return true;
            }
            return false;
        }

    }
}
