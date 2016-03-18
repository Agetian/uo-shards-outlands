using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class Drake : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public Drake(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a drake";
            Body = Utility.RandomList(60, 61);
            BaseSoundID = 362;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;            

            Fame = 5500;
            Karma = -5500;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85;
        }

        public override int Meat { get { return 10; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override bool CanFly { get { return true; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8406; } }
        public override int TamedItemHue { get { return 1205; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 16; } }
        public override int TamedBaseMaxDamage { get { return 18; } }
        public override double TamedBaseWrestling { get { return 80; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void SetTamedAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            
            if (!IsBonded)
            {
                switch (Utility.Random(500))
                {
                    case 0: { c.AddItem(SpellScroll.MakeMaster(new GateTravelScroll())); } break;
                }
            }
        }

        public Drake(Serial serial): base(serial)
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
