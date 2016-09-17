using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;

using SteamKit2;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class LinkNameTrigger : BaseTrigger
    {
        public LinkNameTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, message, true);
        }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, message, false);
        }

        private bool Respond(SteamID toID, string message, bool room)
        {
            string pattern = @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>";
            Uri uriResult;
            string[] splitmes = message.Split(' ');
            for (int i = 0; i < splitmes.Length; i++)
            {
                try
                {
                    bool result = Uri.TryCreate(splitmes[i], UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (result)
                    {
                        using (WebClient client = new WebClient())
                        {
                            string body = client.DownloadString(splitmes[i]);
                            string title = Regex.Match(body, pattern, RegexOptions.IgnoreCase).Groups["Title"].Value;
                            if (title != null)
                            {
                                SendMessageAfterDelay(toID, title.ToString(), room);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (WebException e)
                {
                    Log.Instance.Error(e.StackTrace);
                }
            }
            return false;
        }
    }
}
