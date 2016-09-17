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
            return Respond(userID, message);
        }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, message);
        }

        private bool Respond(SteamID toID, string message)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if(query != null)
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
