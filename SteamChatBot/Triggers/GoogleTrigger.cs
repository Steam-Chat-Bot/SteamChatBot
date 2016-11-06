using SteamChatBot.Triggers.TriggerOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace SteamChatBot.Triggers
{
    class GoogleTrigger : BaseTrigger
    {
        public GoogleTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message, true);
        }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, userID, message, false);
        }

        private bool Respond(SteamID toID, SteamID userID, string message, bool room)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if(query != null && query.Length >= 2)
            {
                string q = "";
                for (int i = 1; i < query.Length; i++)
                {
                    q += query[i] + " ";
                }
                q = q.Trim();

                StringBuilder sb = new StringBuilder();
                byte[] ResultsBuffer = new byte[8192];
                string SearchResults = "http://google.com/search?q=" + q;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SearchResults);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(ResultsBuffer, 0, ResultsBuffer.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(ResultsBuffer, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);

                string sbb = sb.ToString();

                HtmlDocument html = new HtmlDocument();
                html.OptionOutputAsXml = true;
                html.LoadHtml(sbb);
                HtmlNode doc = html.DocumentNode;

                foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                {
                    string hrefValue = link.GetAttributeValue("href", "");
                    if (!hrefValue.ToString().ToUpper().Contains("GOOGLE") && hrefValue.ToString().Contains("/url?q=") && hrefValue.ToString().ToUpper().Contains("HTTP://"))
                    {
                        int index = hrefValue.IndexOf("&");
                        if (index > 0)
                        {
                            hrefValue = hrefValue.Substring(0, index);
                            SendMessageAfterDelay(toID, hrefValue.Replace("/url?q=", ""), room);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
