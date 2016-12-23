using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using SteamChatBot.Triggers.TriggerOptions;
using SteamKit2;

namespace SteamChatBot.Triggers
{
    class YoutubeTrigger : BaseTrigger
    {
        public YoutubeTrigger(TriggerType type, string name, TriggerOptionsBase options) : base(type, name, options)
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
            string[] query = StripCommand(message, Options.ChatCommandApi.ChatCommand.Command);
            if (query != null && query.Length == 1)
            {
                SendMessageAfterDelay(toID, "Usage: " + Options.ChatCommandApi.ChatCommand.Command + " <query>", room);
                return true;
            }
            else if (query != null && query.Length >= 2)
            {
                try
                {
                    Task.Run(() => Run(toID, query, room));
                    return true;
                }
                catch (AggregateException e)
                {
                    foreach (var ex in e.InnerExceptions)
                    {
                        Log.Instance.Error(e.StackTrace);
                        SendMessageAfterDelay(toID, e.Message, room);
                    }
                    return true;
                }
            }
            return false;
        }

        private async Task Run(SteamID toID, string[] query, bool room)
        {
            YouTubeService ys = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Options.ChatCommandApi.ApiKey,
                ApplicationName = "Steam Chat Bot"
            });

            SearchResource.ListRequest search = ys.Search.List("snippet");
            string q = "";
            for (int i = 1; i < query.Length; i++)
            {
                q += query[i] + " ";
            }
            q = q.Trim();
            search.Q = q;
            search.MaxResults = 1;
            
            SearchListResponse response = await search.ExecuteAsync();
            
            foreach (SearchResult result in response.Items)
            {
                if (result.Id.Kind == "youtube#video")
                {
                    SendMessageAfterDelay(toID, "https://youtube.com/watch?v=" + result.Id.VideoId, room);
                }
                else if(result.Id.Kind == "youtube#channel")
                {
                    SendMessageAfterDelay(toID, "https://youtube.com/channel/" + result.Id.ChannelId, room);
                }
            }
        }
    }
}
