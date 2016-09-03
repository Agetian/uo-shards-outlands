using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;

namespace Server
{
    public class Enhancements
    {
        public enum CustomizationType
        {
            BenchPlayer,
            Carnage,
            ShapeShifter,
            Hoarder,           
            Venomous,           
            SomethingFishy
        }

        public enum SpellType
        {
            MagicArrow,
            Clumsy,
            Feeblemind,
            Weaken,
            ReactiveArmor,

            Harm,
            Cure,
            Protection,

            Fireball,
            Bless,
            WallOfStone,
            Teleport,

            ArchCure,
            ArchProtection,
            Curse,

            BladeSpirits,
            MagicReflect,
            MindBlast,
            SummonCreature,

            Dispel,
            EnergyBolt,
            Explosion,
            MassCurse,
            ParalyzeField,

            EnergyField,
            Flamestrike,
            MassDispel,
            MeteorSwarm,

            AirElemental,
            EarthElemental,
            FireElemental,
            WaterElemental,
            SummonDaemon,
            EnergyVortex,
        }

        public enum SpellHueType
        {
            Standard,
            Charcoal,
            Earthen,
            Rose,
            Otherworldly
        }

        public enum EmoteType
        {
            Anger,
            Belch,
            Clap,
            Confused,
            Cough,
            Cry,
            Fart,
            Greet,
            Groan,
            Hiccup,
            Hurt,
            Kiss,
            Laugh,
            No,
            Oops,
            Puke,
            Shush,
            Sick,
            Sleep,
            Spit,
            Surprise,
            Whistle,
            Yell,
            Yes
        }

        public static bool HasCustomizationActive(PlayerMobile player, CustomizationType customization)
        {
            if (player == null)
                return false;

            CustomizationEntry entry = GetCustomizationEntry(player, customization);

            if (entry == null)
                return false;

            if (entry.m_Unlocked && entry.m_Active)
                return true;

            return false;
        }

        public static CustomizationEntry GetCustomizationEntry(PlayerMobile player, CustomizationType customization)
        {
            CustomizationEntry entry = null;

            if (player == null)
                return entry;

            EnhancementsPersistance.CheckAndCreateEnhancementsAccountEntry(player);

            foreach (CustomizationEntry playerEntry in player.m_EnhancementsAccountEntry.m_Customizations)
            {
                if (playerEntry.m_Customization == customization)
                    return playerEntry;
            }

            return entry;
        }

        public static CustomizationDetail GetCustomizationDetail(CustomizationType customizationType)
        {
            CustomizationDetail detail = new CustomizationDetail();

            detail.m_CustomizationType = customizationType;

            #region Customization Details

            switch (customizationType)
            {
                case CustomizationType.BenchPlayer:
                    detail.m_CustomizationName = "Bench Player";
                    detail.m_Description = "Unlocks a 6th character slot on your account.";
                    detail.m_AlwaysActive = true;
                    detail.GumpCollectionId = "BenchPlayerCustomization";
                break;

                case CustomizationType.Carnage:
                    detail.m_CustomizationName = "Carnage";
                    detail.m_Description = "Has a 25% chance upon killing another player that they will explode in a shower of gore.";
                    detail.GumpCollectionId = "CarnageCustomization";
                break;

                case CustomizationType.ShapeShifter:
                    detail.m_CustomizationName = "Shape Shifter";
                    detail.m_Description = "Add high-level creature types to the Polymorph spell.";
                    detail.GumpCollectionId = "ShapeShifterCustomization";
                break;

                case CustomizationType.Hoarder:
                    detail.m_CustomizationName = "Hoarder";
                    detail.m_Description = "Increases your bank's item limit to 150.";
                    detail.m_AlwaysActive = true;
                    detail.GumpCollectionId = "HoarderCustomization";
                break;

                case CustomizationType.Venomous:
                    detail.m_CustomizationName = "Venomous";
                    detail.m_Description = "Applying poison to another target will create additional visual and sound effects.";
                    detail.GumpCollectionId = "VenomousCustomization";
                break;

                case CustomizationType.SomethingFishy:
                    detail.m_CustomizationName = "Something Fishy";
                    detail.m_Description = "Adds occasional, random visual and sound effects while making fishing attempts.";
                    detail.GumpCollectionId = "SomethingFishyCustomization";
                break;
            }

            #endregion

            return detail;
        }

        public static SpellHueEntry GetSpellHueEntry(PlayerMobile player, SpellType spellType)
        {
            SpellHueEntry entry = null;

            if (player == null)
                return entry;

            EnhancementsPersistance.CheckAndCreateEnhancementsAccountEntry(player);

            foreach (SpellHueEntry playerEntry in player.m_EnhancementsAccountEntry.m_SpellHues)
            {
                if (playerEntry.m_SpellType == spellType)
                    return playerEntry;
            }

            return entry;
        }

        public static SpellHueDetail GetSpellHueDetail(SpellType spellType)
        {
            SpellHueDetail detail = new SpellHueDetail();

            detail.m_SpellType = spellType;

            #region Spell Hue Details

            switch (spellType)
            {
                case SpellType.MagicArrow:
                    detail.m_SpellName = "Magic Arrow"; 
                    detail.m_ItemID = 8324;
                break;

                case SpellType.Clumsy:
                    detail.m_SpellName = "Clumsy";
                    detail.m_ItemID = 8320;
                break;

                case SpellType.Feeblemind:
                    detail.m_SpellName = "Feeblemind";
                    detail.m_ItemID = 8322;
                break;

                case SpellType.Weaken:
                    detail.m_SpellName = "Weaken";
                    detail.m_ItemID = 8327;
                break;

                case SpellType.ReactiveArmor:
                    detail.m_SpellName = "Reactive Armor";
                    detail.m_ItemID = 8326;
                break;

                case SpellType.Harm:
                    detail.m_SpellName = "Harm";
                    detail.m_ItemID = 8331;
                break;

                case SpellType.Cure:
                    detail.m_SpellName = "Cure";
                    detail.m_ItemID = 8330;
                break;

                case SpellType.Protection:
                    detail.m_SpellName = "Protection";
                    detail.m_ItemID = 8334;
                break;

                case SpellType.Fireball:
                    detail.m_SpellName = "Fireball";
                    detail.m_ItemID = 8337;
                break;

                case SpellType.Bless:
                    detail.m_SpellName = "Bless";
                    detail.m_ItemID = 8336;
                break;

                case SpellType.WallOfStone:
                    detail.m_SpellName = "Wall of Stone";
                    detail.m_ItemID = 8343;
                break;

                case SpellType.Teleport:
                    detail.m_SpellName = "Teleport";
                    detail.m_ItemID = 8341;
                break;

                case SpellType.ArchCure:
                    detail.m_SpellName = "Arch Cure";
                    detail.m_ItemID = 8344;
                break;

                case SpellType.ArchProtection:
                    detail.m_SpellName = "Arch Protection";
                    detail.m_ItemID = 8345;
                break;

                case SpellType.Curse:
                    detail.m_SpellName = "Curse";
                    detail.m_ItemID = 8346;
                break;

                case SpellType.BladeSpirits:
                    detail.m_SpellName = "Blade Spirits";
                    detail.m_ItemID = 8352;
                break;

                case SpellType.MagicReflect:
                    detail.m_SpellName = "Magic Reflect";
                    detail.m_ItemID = 8355;
                break;

                case SpellType.MindBlast:
                    detail.m_SpellName = "Mind Blast";
                    detail.m_ItemID = 8356;
                break;

                case SpellType.SummonCreature:
                    detail.m_SpellName = "Summon Creature";
                    detail.m_ItemID = 8359;
                break;

                case SpellType.Dispel:
                    detail.m_SpellName = "Dispel";
                    detail.m_ItemID = 8360;
                break;

                case SpellType.EnergyBolt:
                    detail.m_SpellName = "Energy Bolt";
                    detail.m_ItemID = 8361;
                break;

                case SpellType.Explosion:
                    detail.m_SpellName = "Explosion";
                    detail.m_ItemID = 8362;
                break;

                case SpellType.MassCurse:
                    detail.m_SpellName = "Mass Curse";
                    detail.m_ItemID = 8365;
                break;

                case SpellType.ParalyzeField:
                    detail.m_SpellName = "Paralyze Field";
                    detail.m_ItemID = 8366;
                break;

                case SpellType.EnergyField:
                    detail.m_SpellName = "Energy Field";
                    detail.m_ItemID = 8369;
                break;

                case SpellType.Flamestrike:
                    detail.m_SpellName = "Flamestrike";
                    detail.m_ItemID = 8370;
                break;

                case SpellType.MassDispel:
                    detail.m_SpellName = "Mass Dispel";
                    detail.m_ItemID = 8373;
                break;

                case SpellType.MeteorSwarm:
                    detail.m_SpellName = "Meteor Swarm";
                    detail.m_ItemID = 8374;
                break;

                case SpellType.AirElemental:
                    detail.m_SpellName = "Air Elemental";
                    detail.m_ItemID = 8379;
                break;

                case SpellType.EarthElemental:
                    detail.m_SpellName = "Earth Elemental";
                    detail.m_ItemID = 8381;
                break;

                case SpellType.FireElemental:
                    detail.m_SpellName = "Fire Elemental";
                    detail.m_ItemID = 8382;
                break;

                case SpellType.WaterElemental:
                    detail.m_SpellName = "Water Elemental";
                    detail.m_ItemID = 8383;
                break;

                case SpellType.SummonDaemon:
                    detail.m_SpellName = "Summon Daemon";
                    detail.m_ItemID = 8380;
                break;

                case SpellType.EnergyVortex:
                    detail.m_SpellName = "Energy Vortex";
                    detail.m_ItemID = 8377;
                break;
            }

            #endregion

            return detail;
        }

        public static SpellHueTypeDetail GetSpellHueTypeDetail(SpellHueType spellHueType)
        {
            SpellHueTypeDetail detail = new SpellHueTypeDetail();

            detail.m_SpellHueType = spellHueType;

            #region Spell Hue Type Details

            switch (spellHueType)
            {
                case SpellHueType.Standard:
                    detail.m_SpellHueTypeName = "Standard";
                    detail.m_Hue = 0;
                break;

                case SpellHueType.Charcoal:
                    detail.m_SpellHueTypeName = "Fire";
                    detail.m_Hue = 2117;
                break;

                case SpellHueType.Earthen:
                    detail.m_SpellHueTypeName = "Earthen";
                    detail.m_Hue = 2550;
                break;

                case SpellHueType.Rose:
                    detail.m_SpellHueTypeName = "Rose";
                    detail.m_Hue = 2660;
                break;

                case SpellHueType.Otherworldly:
                    detail.m_SpellHueTypeName = "Otherworldly";
                    detail.m_Hue = 2962;
                break;
            }

            #endregion

            return detail;
        }

        public static bool HasEmoteActive(PlayerMobile player, EmoteType emote)
        {
            if (player == null)
                return false;

            EmoteEntry entry = GetEmoteEntry(player, emote);

            if (entry == null)
                return false;

            if (entry.m_Unlocked)
                return true;

            return false;
        }

        public static EmoteEntry GetEmoteEntry(PlayerMobile player, EmoteType emoteType)
        {
            EmoteEntry entry = null;

            if (player == null)
                return entry;

            EnhancementsPersistance.CheckAndCreateEnhancementsAccountEntry(player);

            foreach (EmoteEntry playerEntry in player.m_EnhancementsAccountEntry.m_Emotes)
            {
                if (playerEntry.m_Emote == emoteType)
                    return playerEntry;
            }

            return entry;
        }

        public static EmoteDetail GetEmoteDetail(EmoteType emoteType)
        {
            EmoteDetail detail = new EmoteDetail();

            detail.m_EmoteType = emoteType;

            #region Emote Details

            switch (emoteType)
            {
                case EmoteType.Anger:
                    detail.m_EmoteName = "Anger";
                    detail.m_Hue = 2116;
                break;

                case EmoteType.Belch:
                    detail.m_EmoteName = "Belch";
                    detail.m_Hue = 2206;
                break;

                case EmoteType.Clap:
                    detail.m_EmoteName = "Clap";
                    detail.m_Hue = 2212;
                break;

                case EmoteType.Confused:
                    detail.m_EmoteName = "Confused";
                    detail.m_Hue = 2592;
                break;

                case EmoteType.Cough:
                    detail.m_EmoteName = "Cough";
                    detail.m_Hue = 2415;
                break;

                case EmoteType.Cry:
                    detail.m_EmoteName = "Cry";
                    detail.m_Hue = 2603;
                break;

                case EmoteType.Fart:
                    detail.m_EmoteName = "Fart";
                    detail.m_Hue = 2208;
                break;

                case EmoteType.Greet:
                    detail.m_EmoteName = "Greet";
                    detail.m_Hue = 2606;
                break;

                case EmoteType.Groan:
                    detail.m_EmoteName = "Groan";
                    detail.m_Hue = 2415;
                break;

                case EmoteType.Hiccup:
                    detail.m_EmoteName = "Hiccup";
                    detail.m_Hue = 53;
                break;

                case EmoteType.Hurt:
                    detail.m_EmoteName = "Hurt";
                    detail.m_Hue = 1256;
                break;

                case EmoteType.Kiss:
                    detail.m_EmoteName = "Kiss";
                    detail.m_Hue = 2606;
                break;

                case EmoteType.Laugh:
                    detail.m_EmoteName = "Laugh";
                    detail.m_Hue = 2603;
                break;

                case EmoteType.No:
                    detail.m_EmoteName = "No";
                    detail.m_Hue = 2116;
                break;

                case EmoteType.Oops:
                    detail.m_EmoteName = "Oops";
                    detail.m_Hue = 2553;
                break;

                case EmoteType.Puke:
                    detail.m_EmoteName = "Puke";
                    detail.m_Hue = 2208;
                break;

                case EmoteType.Shush:
                    detail.m_EmoteName = "Shush";
                    detail.m_Hue = 2530;
                break;

                case EmoteType.Sick:
                    detail.m_EmoteName = "Sick";
                    detail.m_Hue = 2208;
                break;

                case EmoteType.Sleep:
                    detail.m_EmoteName = "Sleep";
                    detail.m_Hue = 2535;
                break;

                case EmoteType.Spit:
                    detail.m_EmoteName = "Spit";
                    detail.m_Hue = 2603;
                break;

                case EmoteType.Surprise:
                    detail.m_EmoteName = "Surprise";
                    detail.m_Hue = 2592;
                break;

                case EmoteType.Whistle:
                    detail.m_EmoteName = "Whistle";
                    detail.m_Hue = 2660;
                break;

                case EmoteType.Yell:
                    detail.m_EmoteName = "Yell";
                    detail.m_Hue = 53;
                break;

                case EmoteType.Yes:
                    detail.m_EmoteName = "Yes";
                    detail.m_Hue = 2599;
                break;
            }

            #endregion

            return detail;
        }

        public class CustomizationEntry
        {
            public CustomizationType m_Customization = CustomizationType.BenchPlayer;
            public bool m_Unlocked = false;
            public bool m_Active = false;

            public CustomizationEntry(CustomizationType customization, bool unlocked, bool active)
            {
                m_Customization = customization;
                m_Unlocked = unlocked;
                m_Active = active;
            }
        }

        public class SpellHueEntry
        {
            public SpellType m_SpellType;
            public List<SpellHueType> m_UnlockedHues = new List<SpellHueType>();
            public SpellHueType m_SelectedHue = SpellHueType.Standard;

            public SpellHueEntry(SpellType spellType)
            {
                m_SpellType = spellType;
            }
        }

        public class EmoteEntry
        {
            public EmoteType m_Emote = EmoteType.Anger;
            public bool m_Unlocked = false;

            public EmoteEntry(EmoteType emote, bool unlocked)
            {
                m_Emote = emote;
                m_Unlocked = unlocked;
            }
        }

        public class CustomizationDetail
        {
            public CustomizationType m_CustomizationType;
            public string m_CustomizationName = "";
            public string m_Description = "";
            public bool m_AlwaysActive = false;
            public string GumpCollectionId = "";

            public CustomizationDetail()
            {
            }
        }

        public class SpellHueDetail
        {
            public SpellType m_SpellType;
            public string m_SpellName = "";
            public int m_ItemID = 0;

            public SpellHueDetail()
            {
            }
        }

        public class SpellHueTypeDetail
        {
            public SpellHueType m_SpellHueType;
            public string m_SpellHueTypeName = "";
            public int m_Hue = 0;

            public SpellHueTypeDetail()
            {
            }
        }

        public class EmoteDetail
        {
            public EmoteType m_EmoteType;
            public string m_EmoteName = "";
            public int m_Hue = 0;

            public EmoteDetail()
            {
            }
        }
    }
}