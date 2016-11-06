using Newtonsoft.Json;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;
using SteamChatBot.Triggers.TriggerOptions;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace SteamChatBot.Triggers
{
    public class NotificationTrigger : BaseTrigger
    {
        Timer saveTimer;

        public NotificationTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        {
            saveTimer = new Timer(Options.NotificationOptions.SaveTimer);
            saveTimer.Elapsed += SaveTimer_Elapsed;
        }

        private void SaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(Options.NotificationOptions.DB);
            File.WriteAllText(Options.NotificationOptions.DBFile, json);
            Log.Instance.Silly("{0}/{1}: Saved db file to {2}", Bot.username, Name, Options.NotificationOptions.DBFile);
        }

        public override bool onLoggedOn()
        {
            try
            {
                if (Options.NotificationOptions.DBFile == AppDomain.CurrentDomain.BaseDirectory + "/notification.json")
                {
                    Options.NotificationOptions.DBFile = Bot.username + "/notification.json";
                }
                Options.NotificationOptions.DB = JsonConvert.DeserializeObject<Dictionary<ulong, DB>>(File.ReadAllText(Options.NotificationOptions.DBFile));
                Log.Instance.Silly("{0}/{1}: Read db from {2}", Bot.username, Name, Options.NotificationOptions.DBFile);
            }
            catch(FileNotFoundException fnfe)
            {
                File.Create(Options.NotificationOptions.DBFile);
            }
            catch(Exception e)
            {
                Log.Instance.Error(e.StackTrace);
                return false;
            }

            if(Options.NotificationOptions.DB == null)
            {
                Options.NotificationOptions.DB = new Dictionary<ulong, DB>();
            }

            saveTimer.Start();
            return true;
        }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, userID, message, false);
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message, true);
        }

        private bool Respond(SteamID toID, SteamID userID, string message, bool room)
        {
            bool messageSent = false;
            CheckIfDBExists(userID);
            Dictionary<ulong, DB> db = Options.NotificationOptions.DB;

            db[userID].userID = userID.ConvertToUInt64();
            db[userID].seen = DateTime.Now;
            db[userID].name = Bot.steamFriends.GetFriendPersonaName(userID);

            string[] query = StripCommand(message, Options.NotificationOptions.SeenCommand);
            if (query != null && query.Length == 2)
            {
                if (!db.ContainsKey(Convert.ToUInt64(query[1])) || db[Convert.ToUInt64(query[1])] == null)
                {
                    SendMessageAfterDelay(toID, "The user " + query[1] + " was not found.", room);
                    messageSent = true;
                }
                else
                {
                    SendMessageAfterDelay(toID, string.Format("I last saw {0} on {1} at {2}", db[Convert.ToUInt64(query[1])].name, db[Convert.ToUInt64(query[1])].seen.ToShortDateString(), db[Convert.ToUInt64(query[1])].seen.ToShortTimeString()), room);
                    messageSent = true;
                }
            }

            query = StripCommand(message, Options.NotificationOptions.APICommand);
            if(query != null && userID != toID)
            {
                SendMessageAfterDelay(toID, "This command will only work in private to protect privacy.", room);
                messageSent = true;
            }
            else if(query != null && query.Length == 2 && userID == toID)
            {
                CheckIfDBExists(userID);
                string api = query[1];
                db[userID.ConvertToUInt64()].pb.apikey = api;
                PushbulletClient client = new PushbulletClient(api);
                PushNoteRequest note = new PushNoteRequest()
                {
                    Title = "Test note",
                    Body = "This is a test. If you receive this then you have successfully registered an API key with the bot."
                };

                PushResponse response = client.PushNote(note);
                if(response == null)
                {
                    SendMessageAfterDelay(toID, "Your push failed. Most likely your API key is incorrect.", room);
                    messageSent = true;
                }
                else
                {
                    SendMessageAfterDelay(toID, "Your push was a success.", room);
                    messageSent = true;
                }
            }

            query = StripCommand(message, Options.NotificationOptions.FilterCommand);
            if(query != null && query.Length == 1)
            {
                SendMessageAfterDelay(toID, "Available sub commands: add, list, clear, delete", room);
                messageSent = true;
            }
            else if(query != null && query.Length >= 2)
            {
                if(query[1] == "add" && query.Length >= 3)
                {

                    List<string> words = new List<string>();
                    words = db[userID].pb.filter;
                    // !filter add banshee test 1 2 3
                    for (int i = 2; i < query.Length; i++)
                    {
                        words.Add(query[i]);
                    }

                    db[userID].pb.filter = words;
                    SendMessageAfterDelay(toID, "Your filter has been successfully modified.", room);
                    messageSent = true;
                }
                else if(query[1] == "list" && query.Length == 2)
                {
                    if (db[userID].pb.filter.Count > 0)
                    {
                        string words = string.Join(", ", db[userID].pb.filter);
                        SendMessageAfterDelay(userID, "Your filter: " + words, false);
                        messageSent = true;
                    }
                    else
                    {
                        SendMessageAfterDelay(userID, "Your filter is empty. Use \"!filter add <words>\" to add to your filter", false);
                        messageSent = true;
                    }
                }
                else if(query[1] == "clear" && query.Length == 2)
                {
                    db[userID].pb.filter.Clear();
                    SendMessageAfterDelay(toID, "Your filter has been cleared", room);
                    messageSent = true;
                }
                else if(query[1] == "delete" && query.Length == 3)
                {
                    db[userID].pb.filter.Remove(query[2]);
                    SendMessageAfterDelay(toID, "The filter \"" + query[2] + "\" has been removed", room);
                    messageSent = true;
                }
            }

            query = StripCommand(message, Options.NotificationOptions.ClearCommand);
            if(query != null)
            {
                db[userID] = null;
                SendMessageAfterDelay(toID, "Your database file has been cleared.", room);
                messageSent = true;
            }


            foreach (DB d in db.Values)
            {
                if (d == null || d.pb == null || d.pb.filter == null || d.pb.filter.Count == 0)
                {
                    return false;
                }
                foreach (string word in d.pb.filter)
                {
                    if (message.ToLower().Contains(word.ToLower()) && userID.ConvertToUInt64() != d.userID)
                    {
                        if (d.pb.apikey != null && d.pb.apikey != "")
                        {
                            PushbulletClient client = new PushbulletClient(d.pb.apikey);
                            PushNoteRequest note = new PushNoteRequest();
                            note.Title = string.Format("Steam message from {0}/{1} in {2}", db[userID.ConvertToUInt64()].name, userID.ConvertToUInt64(), toID.ConvertToUInt64());
                            note.Body = message;
                            client.PushNote(note);
                            Log.Instance.Verbose("{0}/{1}: Sending pushbullet for {2}/{3}", Bot.username, Name, db[d.userID].name, db[d.userID].userID);
                            return true;
                        }
                    }
                }
            }

            return messageSent;
        }

        private void CheckIfDBExists(SteamID userID)
        {
            if (!Options.NotificationOptions.DB.ContainsKey(userID))
            {
                Options.NotificationOptions.DB.Add(userID.ConvertToUInt64(), new DB());
                Options.NotificationOptions.DB[userID.ConvertToUInt64()].pb = new PushBullet();
            }

            if (Options.NotificationOptions.DB[userID].pb.filter == null)
            {
                Options.NotificationOptions.DB[userID].pb.filter = new List<string>();
            }
        }
    }
}
