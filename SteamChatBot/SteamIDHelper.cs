using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamKit2;

namespace SteamChatBot
{
    class SteamIDHelper
    {
        private static SteamID Decode(SteamID _steamID)
        {
            uint[] highLow = LongToDoubleInt((long)_steamID.ConvertToUInt64());

            return new SteamID()
            {
                AccountID = highLow[0],
                AccountInstance = highLow[1] & 0xFFFFF,
                AccountType = (EAccountType)(highLow[1] >> 20 & 0xF),
                AccountUniverse = (EUniverse)(highLow[1] >> 24 & 0xFF)
            };
        }

        private static uint[] LongToDoubleInt(long a)
        {
            uint a1 = (uint)(a & uint.MaxValue);
            uint a2 = (uint)(a >> 32);
            return new uint[]
            {
                a1,
                a2
            };
        }

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
