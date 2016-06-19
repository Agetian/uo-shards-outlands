﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
    public enum GuildSymbolType
    {
        Barrel,
        SilverSeal,
        MetalChest
    }

    public static class GuildSymbols
    {
        public static List<GuildSymbolType> GetDefaultGuildSymbols()
        {
            return new List<GuildSymbolType>()
            {
                #region Symbols

                GuildSymbolType.Barrel,
                GuildSymbolType.SilverSeal,
                GuildSymbolType.MetalChest

                #endregion
            };
        }

        public static GuildSymbolDetail GetGuildSymbolDetail(GuildSymbolType guildSymbolType)
        {
            GuildSymbolDetail detail = new GuildSymbolDetail(guildSymbolType);

            switch (guildSymbolType)
            {
                #region Symbols

                case GuildSymbolType.Barrel:
                    detail.m_SymbolName = "Barrel";
                    detail.SymbolIconItemId = 4014;
                    detail.SymbolIconHue = 0;
                    detail.SymbolIconOffsetX = 16;
                    detail.SymbolIconOffsetY = 8;
                break;

                case GuildSymbolType.SilverSeal:
                    detail.m_SymbolName = "Silver Seal";
                    detail.SymbolIconItemId = 571;
                    detail.SymbolIconHue = 0;
                    detail.SymbolIconOffsetX = 21;
                    detail.SymbolIconOffsetY = 23;
                break;

                case GuildSymbolType.MetalChest:
                detail.m_SymbolName = "Metal Chest";
                detail.SymbolIconItemId = 2475;
                detail.SymbolIconHue = 0;
                detail.SymbolIconOffsetX = 20;
                detail.SymbolIconOffsetY = 7;
                break;

                #endregion
            }

            return detail;
        }
    }

    public class GuildSymbolDetail
    {
        public GuildSymbolType m_Symbol;
        public string m_SymbolName;

        public int SymbolIconItemId;
        public int SymbolIconHue;
        public int SymbolIconOffsetX;
        public int SymbolIconOffsetY;

        public GuildSymbolDetail(GuildSymbolType symbol)
        {            
        }
    }
}
