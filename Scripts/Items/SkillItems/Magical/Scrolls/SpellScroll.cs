using System;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.ContextMenus;
using Server.Engines.Craft;

namespace Server.Items
{
	public class SpellScroll : Item, ICommodity
    {
        private int m_SpellID;

        public int SpellID
        {
            get
            {
                return m_SpellID;
            }
        }

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return (Core.ML); } }
        
        public SpellScroll(Serial serial) : base(serial)
        {
        }

        [Constructable]
		public SpellScroll( int spellID, int itemID ) : this( spellID, itemID, 1 )
        {
        }

        [Constructable]
		public SpellScroll( int spellID, int itemID, int amount ) : base( itemID )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;

            m_SpellID = spellID;
        }

        public static SpellScroll GetRandomSpellScroll(int level)
        {
            SpellScroll spellScroll = null;

            List<Type> m_PotentialTypes = new List<Type>();

            #region Spell Levels

            if (level == 1)
            {
                m_PotentialTypes.Add(typeof(ReactiveArmorScroll));
                m_PotentialTypes.Add(typeof(ClumsyScroll));
                m_PotentialTypes.Add(typeof(CreateFoodScroll));
                m_PotentialTypes.Add(typeof(FeeblemindScroll));
                m_PotentialTypes.Add(typeof(HealScroll));
                m_PotentialTypes.Add(typeof(MagicArrowScroll));
                m_PotentialTypes.Add(typeof(NightSightScroll));
                m_PotentialTypes.Add(typeof(WeakenScroll));
            }

            if (level == 2)
            {
                m_PotentialTypes.Add(typeof(AgilityScroll));
                m_PotentialTypes.Add(typeof(CunningScroll));
                m_PotentialTypes.Add(typeof(CureScroll));
                m_PotentialTypes.Add(typeof(HarmScroll));
                m_PotentialTypes.Add(typeof(MagicTrapScroll));
                m_PotentialTypes.Add(typeof(MagicUnTrapScroll));
                m_PotentialTypes.Add(typeof(ProtectionScroll));
                m_PotentialTypes.Add(typeof(StrengthScroll));
            }

            if (level == 3)
            {
                m_PotentialTypes.Add(typeof(BlessScroll));
                m_PotentialTypes.Add(typeof(FireballScroll));
                m_PotentialTypes.Add(typeof(MagicLockScroll));
                m_PotentialTypes.Add(typeof(PoisonScroll));
                m_PotentialTypes.Add(typeof(TelekinisisScroll));
                m_PotentialTypes.Add(typeof(TeleportScroll));
                m_PotentialTypes.Add(typeof(UnlockScroll));
                m_PotentialTypes.Add(typeof(WallOfStoneScroll));
            }

            if (level == 4)
            {
                m_PotentialTypes.Add(typeof(ArchCureScroll));
                m_PotentialTypes.Add(typeof(ArchProtectionScroll));
                m_PotentialTypes.Add(typeof(CurseScroll));
                m_PotentialTypes.Add(typeof(FireFieldScroll));
                m_PotentialTypes.Add(typeof(GreaterHealScroll));
                m_PotentialTypes.Add(typeof(LightningScroll));
                m_PotentialTypes.Add(typeof(ManaDrainScroll));
                m_PotentialTypes.Add(typeof(RecallScroll));
            }

            if (level == 5)
            {
                m_PotentialTypes.Add(typeof(BladeSpiritsScroll));
                m_PotentialTypes.Add(typeof(DispelFieldScroll));
                m_PotentialTypes.Add(typeof(IncognitoScroll));
                m_PotentialTypes.Add(typeof(MagicReflectScroll));
                m_PotentialTypes.Add(typeof(MindBlastScroll));
                m_PotentialTypes.Add(typeof(ParalyzeScroll));
                m_PotentialTypes.Add(typeof(PoisonFieldScroll));
                m_PotentialTypes.Add(typeof(SummonCreatureScroll));
            }

            if (level == 6)
            {
                m_PotentialTypes.Add(typeof(DispelScroll));
                m_PotentialTypes.Add(typeof(EnergyBoltScroll));
                m_PotentialTypes.Add(typeof(ExplosionScroll));
                m_PotentialTypes.Add(typeof(InvisibilityScroll));
                m_PotentialTypes.Add(typeof(MarkScroll));
                m_PotentialTypes.Add(typeof(MassCurseScroll));
                m_PotentialTypes.Add(typeof(ParalyzeFieldScroll));
                m_PotentialTypes.Add(typeof(RevealScroll));
            }

            if (level == 7)
            {
                m_PotentialTypes.Add(typeof(ChainLightningScroll));
                m_PotentialTypes.Add(typeof(EnergyFieldScroll));
                m_PotentialTypes.Add(typeof(FlamestrikeScroll));
                m_PotentialTypes.Add(typeof(GateTravelScroll));
                m_PotentialTypes.Add(typeof(ManaVampireScroll));
                m_PotentialTypes.Add(typeof(MassDispelScroll));
                m_PotentialTypes.Add(typeof(MeteorSwarmScroll));
                m_PotentialTypes.Add(typeof(PolymorphScroll));
            }

            if (level == 8)
            {
                m_PotentialTypes.Add(typeof(EarthquakeScroll));
                m_PotentialTypes.Add(typeof(EnergyVortexScroll));
                m_PotentialTypes.Add(typeof(ResurrectionScroll));
                m_PotentialTypes.Add(typeof(SummonAirElementalScroll));
                m_PotentialTypes.Add(typeof(SummonDaemonScroll));
                m_PotentialTypes.Add(typeof(SummonEarthElementalScroll));
                m_PotentialTypes.Add(typeof(SummonFireElementalScroll));
                m_PotentialTypes.Add(typeof(SummonWaterElementalScroll));
            }

            #endregion

            if (m_PotentialTypes.Count == 0)
                return null;

            spellScroll = (SpellScroll)Activator.CreateInstance(m_PotentialTypes[Utility.RandomMinMax(0, m_PotentialTypes.Count - 1)]);

            return spellScroll;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            //Version 0
            writer.Write((int)m_SpellID);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_SpellID = reader.ReadInt();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.Movable)
                list.Add(new ContextMenus.AddToSpellbookEntry());
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Multis.DesignContext.Check(from))
                return; // They are customizing

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }
                
            CastScroll(from);
        }

        private void CastScroll(Mobile from)
        {
            Spell spell = SpellRegistry.NewSpell(m_SpellID, from, this);

            if (spell != null)
                spell.Cast();

            else
                from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
        }
    }
}