using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class ShipUpgrades
    {
        public enum UpgradeType
        {
            Theme,
            Paint,
            CannonMetal,

            Outfitting,
            Flag,
            Charm,            

            MinorAbility,
            MajorAbility,
            EpicAbility
        }

        public enum ThemeType
        {
            None,

            Navy,
            Merchant,
            Pirate,
            Orc,
            Undead
        }

        public enum PaintType
        {
            None,

            Blue,
            Tan,
            Black,
            Green,
            White
        }

        public enum CannonMetalType
        {
            None,

            Moonstone
        }

        public enum OutfittingType
        {
            None,

            Fishing,
            Merchant,
            Runner,
            Hunter,
            Destroyer,
            Dreadnought
        }

        public enum CharmType
        {
            None,

            LuckyRabbit
        }

        public enum FlagType
        {
            None,

            Pirate
        }

        public enum MinorAbilityType
        {
            None,

            Precision,
        }

        public enum MajorAbilityType
        {
            None,

            Smokescreen,
        }

        public enum EpicAbilityType
        {
            None,

            Hellfire
        }
    }
}