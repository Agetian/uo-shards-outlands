using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    public class ArcaneEssence : Item
    {
        [Constructable]
        public ArcaneEssence(): base(12686)
        {
            Name = "arcane essence";
            Hue = 2609;

            Stackable = true;
            Weight = .01;
        }

        [Constructable]
        public ArcaneEssence(int amount): base(12686)
        {
            Name = "arcane essence";
            Hue = 2609;

            Stackable = true;
            Weight = .01;

            Amount = amount;
        }        
        
        public ArcaneEssence(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("What do you wish to recharge with this?");
            from.Target = new ArcaneEssenceTarget(this);
        }

        public class ArcaneEssenceTarget : Target
        {
            public ArcaneEssence m_ArcaneEssence;
            public PlayerMobile m_Player; 

            public ArcaneEssenceTarget(ArcaneEssence arcaneEssence): base(18, false, TargetFlags.None)
            {
                m_ArcaneEssence = arcaneEssence;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                m_Player = from as PlayerMobile;

                if (m_Player == null) return;
                if (m_ArcaneEssence == null) 
                {
                    m_Player.SendMessage("That arcane essence is no longer available.");
                    return;
                }

                if (m_ArcaneEssence.Deleted) 
                {
                    m_Player.SendMessage("That arcane essence is no longer available.");
                    return;
                }

                if (!m_ArcaneEssence.IsChildOf(m_Player.Backpack))
                {
                    m_Player.SendMessage("The arcane essence you wish to use must be in your backpack.");
                    return;
                }

                Item targetItem = target as Item;

                if (targetItem == null)
                {
                    m_Player.SendMessage("That is not an item.");
                    return;
                }

                if (!(targetItem.IsChildOf(m_Player.Backpack) || targetItem.RootParent == m_Player))
                {
                    m_Player.SendMessage("The item you wish to recharge must be in your backpack or currently equipped.");
                    return;
                }

                if (targetItem.ArcaneChargesPerArcaneEssence == 0)
                {
                    m_Player.SendMessage("That item cannot be recharged with arcane essence.");
                    return;
                }

                if (!targetItem.ArcaneRechargeable)
                {
                    m_Player.SendMessage("That item cannot be recharged with arcane essence.");
                    return;
                }   


                if (targetItem.ArcaneCharges >= targetItem.ArcaneChargesMax)
                {
                    m_Player.SendMessage("That item is at it's maximum number of arcane charges.");
                    return;
                }                             

                if (targetItem.IsArcaneRechargeRestricted(m_Player))
                    return;                

                int arcaneChargesAvailable = targetItem.ArcaneChargesMax - targetItem.ArcaneCharges;
                
                double arcaneEssenceNeeded = 0;
                double arcaneChargesGrantedPerArcaneEssence = targetItem.ArcaneChargesPerArcaneEssence;

                if (((double)m_ArcaneEssence.Amount * arcaneChargesGrantedPerArcaneEssence) < 1)
                {
                    double arcaneEssenceNeededPerCharge = Math.Ceiling(1.0 / arcaneChargesGrantedPerArcaneEssence);
                    
                    if (arcaneEssenceNeededPerCharge > m_ArcaneEssence.Amount)
                    {
                        m_Player.SendMessage("You do not have sufficient arcane essence to provide any charges to that item.");
                        return;
                    }
                }

                if (((double)m_ArcaneEssence.Amount * arcaneChargesGrantedPerArcaneEssence) >= (double)arcaneChargesAvailable)
                {
                    int arcaneEssenceToUse = (int)(Math.Ceiling((double)arcaneChargesAvailable / arcaneChargesGrantedPerArcaneEssence));

                    targetItem.ArcaneCharges = targetItem.ArcaneChargesMax;
                    m_ArcaneEssence.Amount -= arcaneEssenceToUse;

                    if (m_ArcaneEssence.Amount == 0)
                        m_ArcaneEssence.Delete();

                    m_Player.PlaySound(0x5AA);
                    m_Player.SendMessage("You use " + arcaneEssenceToUse.ToString() + " arcane essence to fully recharge the item.");

                    return;
                }

                else
                {
                    int initialAmount = m_ArcaneEssence.Amount;
                    int rechargesPossible = (int)(Math.Floor((double)m_ArcaneEssence.Amount * arcaneChargesGrantedPerArcaneEssence));
                   
                    targetItem.ArcaneCharges += rechargesPossible;
                    m_ArcaneEssence.Delete();

                    m_Player.PlaySound(0x5AA);
                    m_Player.SendMessage("You use " + initialAmount + " arcane essence to add " + rechargesPossible.ToString() + " charges to the item.");

                    return;
                }
            }
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
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
    }
}