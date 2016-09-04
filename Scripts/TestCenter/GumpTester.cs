using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class GumpTester : Item
    {
        [Constructable]
        public GumpTester(): base(0x26BC)
        {
            Name = "a gump tester"; 
        }

        public GumpTester(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                EnhancementsGumpObject enhancementGumpObject = new EnhancementsGumpObject(player);

                //player.CloseGump(typeof(EnhancementsGump));
                //player.SendGump(new EnhancementsGump(player, enhancementGumpObject));

                player.CloseGump(typeof(AchievementsGump));
                player.SendGump(new AchievementsGump(player, AchievementsGump.PageType.Main, 0, AchievementCategory.Adventuring, 0, 0));

                //player.CloseGump(typeof(TestGump));
                //player.SendGump(new TestGump(player));
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
   
    public class TestGump : Gump
    {
        public TestGump(Mobile from) : base(10, 10)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int WhiteTextHue = 2655;

            AddImage(120, 219, 103);
            AddImage(197, 310, 103);
            AddImage(197, 329, 103);
            AddButton(44, 135, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(3, 114, 2599, "Expedite Repairs");
            AddLabel(76, 138, WhiteTextHue, "1m 59s");
            AddButton(168, 135, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(142, 114, 2603, "Smokescreen");
            AddLabel(201, 138, WhiteTextHue, "4m 59s");
            AddButton(287, 135, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(277, 114, 2606, "Hellfire");
            AddLabel(321, 138, WhiteTextHue, "9m 59s");
            AddAlphaRegion(12, 12, 30, 86);
            AddItem(4, 41, 5363);
            AddButton(19, 74, 1210, 248, 0, GumpButtonType.Reply, 0);
            AddImage(15, 12, 9900);
            AddButton(226, 189, 4029, 248, 0, GumpButtonType.Reply, 0);
            AddButton(195, 194, 2223, 248, 0, GumpButtonType.Reply, 0);
            AddButton(266, 194, 2224, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(185, 169, 169, "Disembark Followers");
            AddButton(51, 194, 2223, 248, 0, GumpButtonType.Reply, 0);
            AddButton(127, 194, 2224, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(88, 189, WhiteTextHue, "Full");
            AddLabel(52, 169, 187, "Movement Mode");
            AddImage(33, 219, 103);
            AddImage(196, 219, 103);
            AddImage(175, 329, 103);
            AddImage(33, 305, 103);
            AddImage(33, 329, 103);
            AddImageTiled(46, 230, 280, 190, 2624);
            AddImage(48, 12, 103);
            AddImage(185, 12, 103);
            AddImageTiled(62, 26, 257, 77, 2624);
            AddLabel(71, 43, 149, "Hull");
            AddImage(102, 47, 2057);
            AddLabel(219, 43, WhiteTextHue, "2925/3000");
            AddLabel(66, 62, 187, "Sails");
            AddImage(102, 67, 2054);
            AddLabel(219, 62, WhiteTextHue, "125/1500");
            AddLabel(66, 82, WhiteTextHue, "Guns");
            AddImage(102, 87, 2057, 2499);
            AddLabel(219, 82, WhiteTextHue, "1250/1500");
            AddImageTiled(187, 90, 21, 7, 2488);
            AddImageTiled(127, 69, 80, 7, 2488);
            AddImageTiled(196, 50, 12, 7, 2488);
            AddImage(52, 15, 1141);
            AddLabel(150, 17, 149, "The Rebellion");
            AddButton(114, 336, 4014, 248, 0, GumpButtonType.Reply, 0);
            AddButton(227, 336, 4007, 248, 0, GumpButtonType.Reply, 0);
            AddButton(160, 278, 4500, 248, 0, GumpButtonType.Reply, 0);
            AddButton(220, 278, 4501, 248, 0, GumpButtonType.Reply, 0);
            AddButton(267, 321, 4502, 248, 0, GumpButtonType.Reply, 0);
            AddButton(220, 366, 4503, 248, 0, GumpButtonType.Reply, 0);
            AddButton(160, 365, 4504, 248, 0, GumpButtonType.Reply, 0);
            AddButton(100, 365, 4505, 248, 0, GumpButtonType.Reply, 0);
            AddButton(55, 321, 4506, 248, 0, GumpButtonType.Reply, 0);
            AddButton(100, 278, 4507, 248, 0, GumpButtonType.Reply, 0);
            AddButton(171, 336, 4017, 248, 0, GumpButtonType.Reply, 0);
            AddItem(33, 215, 733);
            AddButton(59, 282, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(96, 243, WhiteTextHue, "10");
            AddItem(284, 218, 709);
            AddButton(287, 284, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddButton(127, 257, 2223, 248, 0, GumpButtonType.Reply, 0);
            AddButton(221, 257, 2224, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(171, 253, WhiteTextHue, "Hull");
            AddLabel(136, 234, 2115, "Targeting Mode");
            AddItem(88, 261, 3700);
            AddLabel(263, 243, WhiteTextHue, "8");
            AddItem(255, 261, 3700);
        }
    }     
}