using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class ArmorValues
    {
        public static int LeatherBaseArmorValue = 20;
        public static int StuddedLeatherBaseArmorValue = 25;
        public static int BoneBaseArmorValue = 30;
        public static int RingmailBaseArmorValue = 35;
        public static int ChainmailBaseArmorValue = 40;
        public static int PlatemailBaseArmorValue = 50;

        public static int LeatherDurability = 50;
        public static int StuddedLeatherDurability = 55;
        public static int BoneDurability = 60;
        public static int RingmailDurability = 65;
        public static int ChainDurability = 70;
        public static int PlatemailDurability = 75;

        public static ArmorMeditationAllowance LeatherMeditationAllowed = ArmorMeditationAllowance.OneHundredPercent;
        public static ArmorMeditationAllowance StuddedLeatherMeditationAllowed = ArmorMeditationAllowance.EightyPercent;
        public static ArmorMeditationAllowance BoneMeditationAllowed = ArmorMeditationAllowance.SixtyPercent;
        public static ArmorMeditationAllowance RingmailMeditationAllowed = ArmorMeditationAllowance.FourtyPercent;
        public static ArmorMeditationAllowance ChainmailMeditationAllowed = ArmorMeditationAllowance.TwentyPercent;
        public static ArmorMeditationAllowance PlatemailMeditationAllowed = ArmorMeditationAllowance.None;

        public static int BucklerArmorValue = 8;
        public static int WoodenShieldArmorValue = 8;
        public static int WoodenKiteShieldArmorValue = 10;
        public static int MetalShieldArmorValue = 12;
        public static int BronzeShieldArmorValue = 14;
        public static int MetalKiteShieldArmorValue = 16;
        public static int HeaterShieldArmorValue = 18;

        public static ArmorMeditationAllowance BucklerMeditationAllowed = ArmorMeditationAllowance.OneHundredPercent;
        public static ArmorMeditationAllowance WoodenShieldMeditationAllowed = ArmorMeditationAllowance.OneHundredPercent;
        public static ArmorMeditationAllowance WoodenKiteShieldMeditationAllowed = ArmorMeditationAllowance.EightyPercent;
        public static ArmorMeditationAllowance MetalShieldMeditationAllowed = ArmorMeditationAllowance.SixtyPercent;
        public static ArmorMeditationAllowance BronzeShieldMeditationAllowed = ArmorMeditationAllowance.FourtyPercent;
        public static ArmorMeditationAllowance MetalKiteShieldMeditationAllowed = ArmorMeditationAllowance.TwentyPercent;
        public static ArmorMeditationAllowance HeaterShieldMeditationAllowed = ArmorMeditationAllowance.None;

        public static int BucklerDurability = 100;
        public static int WoodenShieldDurability = 100;
        public static int WoodenKiteShieldDurability = 105;
        public static int MetalShieldDurability = 110;
        public static int BronzeShieldDurability = 115;
        public static int MetalKiteShieldDurability = 120;
        public static int HeaterShieldDurability = 125;
    }
}