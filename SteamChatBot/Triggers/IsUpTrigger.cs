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
        public string Command { get; set; }

        public IsUpTrigger(TriggerType type, string name) : base(type, name)
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
            string[] query = StripCommand(message, Command);
            if(query != null)
            {
                HttpWebResponse response;
                try {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query[1]);
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch(WebException e)
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
