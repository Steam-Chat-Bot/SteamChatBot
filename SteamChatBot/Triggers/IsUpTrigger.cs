using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class IsUpTrigger : BaseTrigger
    {
        public IsUpTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
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
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if (query != null)
            {
                HttpWebResponse response;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query[1]);
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (UriFormatException e)
                {
                    Log.Instance.Error(Bot.username + "/" + Name + ": " + e.StackTrace);
                    SendMessageAfterDelay(toID, "Uri was not in the correct format (missing http:// probably).", true);
                    response = null;
                    return false;
                }
                catch (WebException e)
                {
                    response = ((HttpWebResponse)e.Response);
                }
                SendMessageAfterDelay(toID, response.StatusCode.ToString() + " (" + (int)response.StatusCode + ")", room);
                return true;
            }
            return false;
        }

    }
}
