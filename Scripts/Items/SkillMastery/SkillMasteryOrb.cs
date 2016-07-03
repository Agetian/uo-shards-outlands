using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class SkillMasteryOrb : Item
    {
        [Constructable]
        public SkillMasteryOrb(): base(22336)
        {
            Name = "a skill mastery orb";
            Hue = 2966;
            Weight = 1;

            Amount = 1;
            Stackable = true;
        }

        [Constructable]
        public SkillMasteryOrb(int amount): base(22336)
        {
            Name = "a skill mastery orb";
            Hue = 2966;
            Weight = 1;

            Amount = amount;
            Stackable = true;
        }

        public SkillMasteryOrb(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            //TEST: Create GUMP

            if (player.BonusSkillCap == PlayerMobile.MaxBonusSkillCap)
            {
                from.SendMessage("You are currently at the maximum skill cap and cannot use this.");
                return;
            }            

            int skillCapIncrease = 10;
            int amountIncreased = 0;

            if (player.BonusSkillCap + skillCapIncrease > PlayerMobile.MaxBonusSkillCap)
                amountIncreased = PlayerMobile.MaxBonusSkillCap - player.BonusSkillCap;

            player.BonusSkillCap += amountIncreased;

            string strAmountIncreased = ((double)amountIncreased / 10).ToString();
            string currentSkillCap = ((double)player.SkillsCap / 10).ToString();

            player.SendMessage("You increase your skillcap by " + strAmountIncreased + ". It is now " + currentSkillCap + ".");

            player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
            player.PlaySound(0x3BD);

            Consume();
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {

            if (target is PlayerMobile && ((PlayerMobile)target).Young)
            {
                from.SendMessage("Young players cannot may not acquire skill mastery orbs.");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire skill mastery orbs.");
                return false;
            }

            return base.DropToItem(from, target, p);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire skill mastery orbs.");
                return false;
            }

            return base.OnDragLift(from);
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