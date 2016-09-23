using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteamChatBot.Triggers.TriggerOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    public class NoteTrigger : BaseTrigger
    {
        private Timer saveNoteTimer;

        public NoteTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
        {
            Options.NoteTriggerOptions.Notes = new Dictionary<SteamID, Dictionary<string, Note>>();
            saveNoteTimer = new Timer(1000 * 60 * 5);
            saveNoteTimer.Elapsed += SaveNoteTimer_Elapsed;
        }

        private void SaveNoteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(Options.NoteTriggerOptions.Notes);
            File.WriteAllText(Bot.username + "/notes.json", json);
        }

        public override bool OnLoggedOn()
        {
            try
            {
                string notes = File.ReadAllText(Bot.username + "/notes.json");
                Options.NoteTriggerOptions.Notes = (Dictionary<SteamID, Dictionary<string, Note>>)JsonConvert.DeserializeObject(notes);
            }
            catch(FileNotFoundException fnfe)
            {
                File.Create(Bot.username + "/notes.json");

            }
            catch(Exception e)
            {
                Log.Instance.Error(e.StackTrace);
            }
            

            saveNoteTimer.Start();

            return true;
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, chatterId, message);
        }

        private bool Respond(SteamID roomID, SteamID userID, string message)
        {
            string[] query = StripCommand(message, Options.NoteTriggerOptions.NoteCommand);
            if (query != null && query.Length == 2)
            {
                string name = query[1];
                Note note = Options.NoteTriggerOptions.Notes[roomID][name];
                if(note == null)
                {
                    SendMessageAfterDelay(roomID, string.Format("The note \"{0}\" does not exist. Use \"{1}\" to create it.", name, Options.NoteTriggerOptions.NoteCommand + " " + name + " <definition>"), true);
                    return true;
                }
                else
                {
                    SendMessageAfterDelay(roomID, "\"" + note.Definition + "\"", true);
                    return true;
                }
            }
            else if (query != null && query.Length >= 3)
            {
                Note note = new Note();

                string name = query[1];
                List<string> def = new List<string>();

                for (int i = 2; i < query.Length; i++)
                {
                    def.Add(query[i]);
                }

                string definition = string.Join(" ", def);

                note.Definition = definition;
                note.ModifiedBy = Bot.steamFriends.GetFriendPersonaName(userID) + " <" + userID.ConvertToUInt64().ToString() + ">";
                note.ModifiedWhen = DateTime.Now.ToString();

                Dictionary<string, Note> innerNote = new Dictionary<string, Note>();
                innerNote.Add(name, note);

                if (Options.NoteTriggerOptions.Notes == null || Options.NoteTriggerOptions.Notes[roomID] == null || Options.NoteTriggerOptions.Notes[roomID][name] == null)
                {
                    Options.NoteTriggerOptions.Notes = new Dictionary<SteamID, Dictionary<string, Note>>();
                    Options.NoteTriggerOptions.Notes.Add(roomID, innerNote);
                }
                else
                {
                    Options.NoteTriggerOptions.Notes.Remove(roomID);
                    Options.NoteTriggerOptions.Notes.Add(roomID, innerNote);
                }

                SendMessageAfterDelay(roomID, string.Format("Note \"{0}\" saved", name), true);
                return true;
            }

            query = StripCommand(message, Options.NoteTriggerOptions.DeleteCommand);
            if(query != null && query.Length == 2)
            {
                string name = query[1];
                Options.NoteTriggerOptions.Notes[roomID].Remove(name);
                SendMessageAfterDelay(roomID, string.Format("Note \"{0}\" deleted", name), true);
                return true;
            }
            return false;


        }
    }
}
