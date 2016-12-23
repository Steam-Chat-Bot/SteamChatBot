using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamChatBot.Triggers.TriggerOptions;
using SteamKit2;
using SteamKit2.Internal;
using SteamKit2.GC;

namespace SteamChatBot.Triggers
{
    class PlayGameTrigger : BaseTrigger
    {
        public PlayGameTrigger(TriggerType type, string name, TriggerOptionsBase cc) : base(type, name, cc)
        { }

        public override bool respondToFriendMessage(SteamID userID, string message)
        {
            return Respond(userID, message, false);
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, message, true);
        }

        private bool Respond(SteamID toID, string message, bool room)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if (query != null && query.Length == 1)
            {
                SendMessageAfterDelay(toID, "Usage: " + Options.ChatCommand.Command + " <appid OR \"clear\">", room);
                return true;
            }
            else if (query != null && query.Length == 2)
            {
                var gamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
                if(query[1] == "clear")
                {
                    gamesPlayed.Body.games_played.Clear();
                    Bot.steamClient.Send(gamesPlayed);
                    return true;
                }
                else
                {
                    gamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
                    {
                        game_id = new GameID(Convert.ToUInt64(query[1]))
                    });
                    Bot.steamClient.Send(gamesPlayed);
                    return true;
                }
            }
            return false;
        }
    }
}
