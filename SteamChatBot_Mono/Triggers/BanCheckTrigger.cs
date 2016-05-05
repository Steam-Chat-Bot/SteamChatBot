using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;

using SteamKit2;

namespace SteamChatBot_Mono.Triggers
{
    class BanCheckTrigger : BaseTrigger
    {
        public BanCheckTrigger(TriggerType type, string name, TriggerOptions options) : base(type, name, options)
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
            string[] query = StripCommand(message, Options.Command);

            if(Options.ApiKey == null || Options.ApiKey == "")
            {
                SendMessageAfterDelay(toID, "You must declare a Steam API key to use this trigger.", room);
                return false;
            }
            if(query != null && query[1] != null)
            {
                if (query[1] != null)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={0}&steamids={1}", Options.ApiKey, query[1]));

                    string text = "";
                    BanCheckResponse bans = null;

                    try {
                        using (var steamwebresponse = (HttpWebResponse)request.GetResponse())
                        {
                            using (var sr = new StreamReader(steamwebresponse.GetResponseStream()))
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                text = sr.ReadToEnd();
                                bans = (BanCheckResponse)js.Deserialize(text, typeof(BanCheckResponse));
                            }
                        }
                    }
                    catch(ArgumentException e)
                    {
                        Log.Instance.Error(e.StackTrace);
                    }

                    bool communitybanned = false;
                    bool vacced = false;
                    int vac_number = 0;
                    bool econban = false;
                    int bancount = 0;

                    if (bans.CommunityBanned == true)
                    {
                        communitybanned = true;
                        bancount++;
                    }
                    if (bans.VACBanned)
                    {
                        vacced = true;
                        vac_number = bans.NumberOfVACBans;
                        bancount++;
                    }
                    if (bans.EconomyBan != "none" && bans.EconomyBan != null)
                    {
                        econban = true;
                        bancount++;
                    }

                    string msg;
                    int commas = bancount - 1;

                    if (bancount > 0)
                    {
                        msg = "WARNING: " + Bot.steamFriends.GetFriendPersonaName(new SteamID(Convert.ToUInt64(query[1]))) + " has the following bans: ";
                        if (vacced)
                        {
                            msg += bans.NumberOfVACBans + " VAC bans" + (commas > 0 ? ", " : ".");
                        }
                        if (communitybanned)
                        {
                            msg += "a community ban" + (commas > 0 ? ", " : ".");
                        }
                        if (econban)
                        {
                            msg += "an economy ban (" + bans.EconomyBan + ").";
                        }
                        SendMessageAfterDelay(toID, msg, room);
                        return true;
                    }
                    else
                    {
                        SendMessageAfterDelay(toID, Bot.steamFriends.GetFriendPersonaName(userID) + " has no bans.", room);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class BanCheckResponse
    {
        public bool CommunityBanned { get; set; }
        public bool VACBanned { get; set; }
        public int NumberOfVACBans { get; set; }
        public string EconomyBan { get; set; }
    }
}
