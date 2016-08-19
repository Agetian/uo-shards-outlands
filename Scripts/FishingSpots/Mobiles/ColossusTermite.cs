﻿using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a colossus termite corpse")]
    public class ColossusTermite : BaseCreature
    {
        public DateTime m_NextShipChewAllowed;
        public TimeSpan NextShipChewDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        [Constructable]
        public ColossusTermite(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a colossus termite";
            Body = 738;
            Hue = 1853;
            BaseSoundID = 0x3BF;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;             

            Tameable = false;

            Fame = 500;
            Karma = -500;
        }
        
        public override bool IsHighSeasBodyType { get { return true; } }
        public override bool HasAlternateHighSeasHurtAnimation { get { return true; } }


        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.5;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < .05 && DateTime.UtcNow > m_NextShipChewAllowed && ShipOccupied != null)
            {
                if (ShipOccupied.Deleted) return;
                if (ShipOccupied.m_SinkTimer != null) return;

                ShipOccupied.ReceiveDamage(this, null, Utility.RandomMinMax(10, 20), DamageType.Hull);

                Say("*chews on the ship*");
                SpecialAbilities.HinderSpecialAbility(1.0, this, this, 1.0, 1, true, Utility.RandomList(0x134, 0x133), false, "", "", "-1");

                m_NextShipChewAllowed = DateTime.UtcNow + NextShipChewDelay;
            }
        }

        public override int GetDeathSound() { return 0x386;}

        public ColossusTermite(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}