using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguine assassin corpse")]
    public class SanguinAssassin : BaseSanguin
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);     

        [Constructable]
        public SanguinAssassin(): base()
        {           
            Name = "a sanguine assassin";

            SetStr(75);
            SetDex(75);
            SetInt(50);

            SetHits(550);

            SetDamage(16, 24);

            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100); 

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Poisoning, 25);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new Bandana() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new LeatherArms() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Cloak() { Movable = false, Hue = itemHue });

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new Kryss() { Movable = false, Hue = weaponHue, }); break;
                case 2: AddItem(new Katana() { Movable = false, Hue = weaponHue, }); break;
            }

            PackItem(new PoisonPotion());
            PackItem(new PoisonPotion());
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
                {
                    if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                    {
                        if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))
                        {
                            PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                            switch (Utility.RandomMinMax(1, 4))
                            {
                                case 1:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 2:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 3:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 4:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 10;
                                    break;
                            }
                        }

                        m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                    }
                }
            }
        }

        public SanguinAssassin(Serial serial): base(serial)
        {
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
        }
    }
}