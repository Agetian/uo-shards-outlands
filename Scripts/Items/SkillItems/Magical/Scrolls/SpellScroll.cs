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