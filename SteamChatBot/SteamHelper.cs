using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

namespace SteamChatBot
{
    class SteamHelper
    {
        public static SteamID ToClanID(SteamID steamID)
        {
            if(steamID.AccountInstance == (uint)SteamID.ChatInstanceFlags.Clan)
            {
                steamID.AccountType = EAccountType.Clan;
                steamID.AccountInstance = 0;
            }

            return steamID;
        }

        public static SteamID ToChatID(SteamID steamID)
        {
            if(steamID.AccountType == EAccountType.Clan)
            {
                steamID.AccountInstance = (uint)SteamID.ChatInstanceFlags.Clan;
                steamID.AccountType = EAccountType.Chat;
            }

            return steamID;
        }
    }
}
