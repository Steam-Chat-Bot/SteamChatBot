using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamChatBot.Triggers.TriggerOptions;
using System.Net;
using System.IO;

namespace SteamChatBot.Triggers
{
    public class TranslateTrigger : BaseTrigger
    {
        class TranslateResult
        {
            public string text { get; set; }
            public Pos pos { get; set; }
            public string source { get; set; }

           public class Pos
            {
                public object code { get; set; }
                public object title { get; set; }
            }
        }

        public TranslateTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
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
            if (query != null && query.Length == 4)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://hablaa.com/hs/translation/" + query[1] + "/" + query[2] + "-" + query[3] + "/"));
                try {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return SendNotFound(toID, query, room);
                        }
                        else if (response.StatusCode != HttpStatusCode.OK)
                        {
                            SendMessageAfterDelay(toID, "Status code from hablaa.com: " + response.StatusCode + "/" + response.StatusDescription, room);
                            return true;
                        }
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            string text = sr.ReadToEnd();
                            TranslateResult[] result = text.ParseJSON<TranslateResult[]>();
                            if (result == null)
                            {
                                return SendNotFound(toID, query, room);
                            }
                            string trans = result[0].text;
                            if (trans == null || trans == "")
                            {
                                return SendNotFound(toID, query, room);
                            }
                            else
                            {
                                SendMessageAfterDelay(toID, trans, room);
                                return true;
                            }
                        }
                    }
                }
                catch(WebException we)
                {
                    return SendNotFound(toID, query, room);
                }
            }
            return false;
        }

        private bool SendNotFound(SteamID toID, string[] query, bool room)
        {
            SendMessageAfterDelay(toID, "Either " + query[1] + " is not defined in that language or one of the languages you specified is incorrect. Please visit http://hablaa.com/hs/translation/" + 
                query[1] + "/" + query[2] + "-" + query[3] + " for more information.", room);
            return true;
        }
    }
}
