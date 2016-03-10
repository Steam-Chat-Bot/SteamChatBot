using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

using SteamKit2;

namespace SteamChatBot.Triggers
{
    class WeatherTrigger : BaseTrigger
    {
        public WeatherTrigger(TriggerType type, string name, TriggerOptions options) : base(type, name, options)
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
            if (Options.ApiKey == null)
            {
                SendMessageAfterDelay(toID, "API Key from Wunderground is required.", room);
                return false;
            }
            else
            {
                string[] query = StripCommand(message, Options.Command);
                if (query != null && query[1] != null)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://api.wunderground.com/api/{0}/{1}/q/{2}.json", Options.ApiKey, "conditions", query[1]));
                    string body = "";
                    Weather weather = null;

                    using (var wundergroud = (HttpWebResponse)request.GetResponse())
                    {
                        using (var sr = new StreamReader(wundergroud.GetResponseStream()))
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            body = sr.ReadToEnd();
                            weather = (Weather)js.Deserialize(body, typeof(Weather));
                            SendMessageAfterDelay(toID, ParseResult(weather), room);
                        }
                    }
                    
                }
                return false;
            }
        }

        class Weather
        {
            public CurrentObservation current_observation { get; set; }
        }

        class CurrentObservation
        {
            public DisplayLocation display_location { get; set; }
            public string weather { get; set; }
            public string temperature_string { get; set; }
            public string feelslike_string { get; set; }
            public string wind_string { get; set; }
            public string relative_humidity { get; set; }
            public string precip_today_metric { get; set; }
            public string precip_today_string { get; set; }
            public string observation_time { get; set; }
            public string forecast_url { get; set; }
        }

        class DisplayLocation
        {
            public string full { get; set; }
            public string zip { get; set; }

        }

        private string ParseResult(Weather weather)
        {
            if(weather.current_observation != null)
            {
                CurrentObservation o = weather.current_observation;
                DisplayLocation d = o.display_location;

                string result = string.Format("Weather for {0}{1}: {2}, {3}", d.full, (d.zip != null ? " (" + d.zip + ")" : ""), o.weather, o.temperature_string);
                result += string.Format("; feels like {0}. {1} winds. {2} humidity", o.feelslike_string, o.wind_string, o.relative_humidity);
                if(o.precip_today_metric != null)
                {
                    result += string.Format("; {0} precipitation today", o.precip_today_string);
                }
                return string.Format("{0}. {1}\n{2}", result, o.observation_time, o.forecast_url);
            }
            return null;
        }
    }
}
