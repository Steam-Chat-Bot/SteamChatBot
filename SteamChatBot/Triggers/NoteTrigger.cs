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
            saveNoteTimer = new Timer(options.NoteTriggerOptions.SaveTimer);
            saveNoteTimer.Elapsed += SaveNoteTimer_Elapsed;
        }

        private void SaveNoteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(Options.NoteTriggerOptions.Notes);
            File.WriteAllText(Options.NoteTriggerOptions.NoteFile, json);
            Log.Instance.Silly("{0}/{1}: Wrote notes to {0}/notes.json", Bot.username, Name);
        }

        public override bool OnLoggedOn()
        {
            try
            {
                Options.NoteTriggerOptions.Notes = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, Note>>>(File.ReadAllText(Options.NoteTriggerOptions.NoteFile));
                Log.Instance.Silly(Bot.username + "/" + Name + ": Loaded notes from " + Options.NoteTriggerOptions.NoteFile);
            }
            catch (FileNotFoundException fnfe)
            {
                File.Create(Options.NoteTriggerOptions.NoteFile);

            }
            catch (Exception e)
            {
                Log.Instance.Error(e.Message + ": " + e.StackTrace);
            }

            if (Options.NoteTriggerOptions.Notes == null)
            {
                Options.NoteTriggerOptions.Notes = new Dictionary<ulong, Dictionary<string, Note>>();
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
            Dictionary<string, Note> db;
            ulong room = roomID.ConvertToUInt64();

            if (!Options.NoteTriggerOptions.Notes.ContainsKey(room))
            {
                Options.NoteTriggerOptions.Notes[room] = new Dictionary<string, Note>();
                db = Options.NoteTriggerOptions.Notes[room];
            }
            else
            {
                db = Options.NoteTriggerOptions.Notes[room];
            }

            string[] query = StripCommand(message, Options.NoteTriggerOptions.NotesCommand);
            if(query != null && query.Length == 1)
            {
                List<string> _notes = new List<string>();
                string notes = "";
                foreach(string note in db.Keys)
                {
                    _notes.Add(note);
                }
                notes = string.Join(", ", _notes.ToArray());
                SendMessageAfterDelay(roomID, "Please see your personal chat for a list of all the notes in this chat room", true);
                SendMessageAfterDelay(userID, "Notes in " + roomID.ConvertToUInt64() + ": " + notes, false);
                return true;
            }

            query = StripCommand(message, Options.NoteTriggerOptions.DeleteCommand);
            if (query != null && query.Length == 2)
            {
                string name = query[1];
                if (!db.ContainsKey(name))
                {
                    SendMessageAfterDelay(roomID, string.Format("The note \"{0}\" does not exist. Use \"{1}\" to create it.", name, Options.NoteTriggerOptions.NoteCommand + " " + name + " <definition>"), true);
                    return true;
                }
                else
                {
                    db.Remove(name);
                    SendMessageAfterDelay(roomID, string.Format("Note \"{0}\" deleted", name), true);
                    return true;
                }
            }

            query = StripCommand(message, Options.NoteTriggerOptions.InfoCommand);
            if(query != null && query.Length == 2)
            {
                string name = query[1];
                if (!db.ContainsKey(name))
                {
                    SendMessageAfterDelay(roomID, string.Format("The note \"{0}\" does not exist. Use \"{1}\" to create it.", name, Options.NoteTriggerOptions.NoteCommand + " " + name + " <definition>"), true);
                    return true;
                }
                else
                {
                    Note note = db[name];
                    SendMessageAfterDelay(roomID, string.Format("The note \"{0}\" with definition \"{1}\" was last modified by {2} on {3}", name, note.Definition, note.ModifiedBy, note.ModifiedWhen), true);
                    return true;
                }
            }

            query = StripCommand(message, Options.NoteTriggerOptions.NoteCommand);
            if (query != null && query.Length == 2)
            {
                string name = query[1];
                if (!db.ContainsKey(name))
                {
                    SendMessageAfterDelay(roomID, string.Format("The note \"{0}\" does not exist. Use \"{1}\" to create it.", name, Options.NoteTriggerOptions.NoteCommand + " " + name + " <definition>"), true);
                    return true;
                }
                else
                {
                    Note note = db[name];
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
                note.ModifiedBy = Bot.steamFriends.GetFriendPersonaName(userID) + " (" + userID.ConvertToUInt64().ToString() + ")";
                note.ModifiedWhen = DateTime.Now.ToString();

                if (!db.ContainsKey(name))
                {
                    db.Add(name, note);
                }
                else
                {
                    db.Remove(name);
                    db.Add(name, note);
                }

                SendMessageAfterDelay(roomID, string.Format("Note \"{0}\" saved", name), true);
                return true;
            }
            return false;
        }
    }
}