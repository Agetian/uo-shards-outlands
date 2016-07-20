using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;


namespace Server.Items
{	
	public class ClothingBlessDeed : Item 
	{
		[Constructable]
		public ClothingBlessDeed() : base( 5360 )
		{
            Name = "clothing bless deed";
            Hue = 2958;

			Weight = 1.0;

            
		}

		public ClothingBlessDeed( Serial serial ) : base( serial )
		{
		}		
        
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack))			
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.			

			else
			{
                from.SendMessage("What item of clothing or mask do you wish to bless?");
				from.Target = new ClothingBlessTarget( this ); // Call our target
			 }
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }

        public class ClothingBlessTarget : Target
        {
            private ClothingBlessDeed m_Deed;

            public ClothingBlessTarget(ClothingBlessDeed deed): base(1, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Deed.Deleted || m_Deed.RootParent != from)
                    return;

                Item item = target as Item;

                if (item == null)
                {
                    from.SendMessage("That cannot be blessed");
                    return;
                }

                bool isMask = false;

                if (item is BaseArmor)
                {
                    BaseArmor armor = item as BaseArmor;

                    if (armor.IsMask())
                        isMask = true;
                }

                if (item is BaseClothing || isMask || item.DecorativeEquipment)
                {
                    if (item.LootType == LootType.Blessed || item.BlessedFor == from )
                        from.SendLocalizedMessage(1045113); // That item is already blessed

                    else if (item.LootType != LootType.Regular)
                        from.SendLocalizedMessage(1045114); // You can not bless that item				

                    else if (item.RootParent != from)
                        from.SendLocalizedMessage(500509); // You cannot bless that object				

                    else
                    {
                        item.LootType = LootType.Blessed;

                        from.SendMessage("You bless the item.");
                        from.PlaySound(0x1F7);

                        m_Deed.Delete(); // Delete the bless deed
                    }
                }

                else
                    from.SendLocalizedMessage(500509); // You cannot bless that object            
            }
        }

	}
}