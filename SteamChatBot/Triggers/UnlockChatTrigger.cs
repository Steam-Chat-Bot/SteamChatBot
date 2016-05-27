using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;
using SteamKit2.Internal;

using SteamChatBot.Triggers.TriggerOptions;

namespace SteamChatBot.Triggers
{
    class UnlockChatTrigger : BaseTrigger
    {
        public UnlockChatTrigger(TriggerType type, string name, ChatCommand options) : base(type, name, options)
        { }

        public override bool respondToChatMessage(SteamID roomID, SteamID chatterId, string message)
        {
            return Respond(roomID, message);
        }

        private bool Respond(SteamID roomID, string message)
        {
            string[] query = StripCommand(message, Options.ChatCommand.Command);
            if (query != null)
            {
                var msg = new ClientMsg<MsgClientChatAction>();
                msg.Body.SteamIdChat = SteamHelper.ToChatID(roomID);
                msg.Body.SteamIdUserToActOn = SteamHelper.ToChatID(roomID);
                msg.Body.ChatAction = EChatAction.UnlockChat;

                Bot.steamClient.Send(msg);
                return true;
            }
            return false;
        }
    }
}
