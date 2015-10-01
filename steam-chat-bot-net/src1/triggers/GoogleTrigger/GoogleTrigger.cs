using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.API.Search;
using SteamKit2;

namespace steam_chat_bot_net.src.GoogleTrigger
{
    class GoogleTrigger : triggers.BaseTrigger
    {
        public override bool RespondToChatMessage(SteamID roomID, SteamID chatterID, string message)
        {
            return Respond(roomID, chatterID, message);
        }

        public override bool ResponseToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, userID, message);
        }

        public override bool Respond(SteamID toID, SteamID userID, string message)
        {
            string[] _stripped = StripCommand(message, options.Command);
            GwebSearchClient client = new GwebSearchClient("");
            IList<IWebResult> results = client.Search(_stripped[1], 1);
            return SendMessageAfterDelay(toID, results[0].ToString());

        }

        public override string[] StripCommand(string message, string command)
        {
            if(command != "" && message != "" && message.ToLower().IndexOf(command.ToLower()) == 0)
            {
                return message.Split(' ');
            }
            else
            {
                return null;
            }
        }

    }
}
