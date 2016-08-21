using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server
{
    public abstract class BaseShipDeed : Item
    {
        private int m_MultiID;
        private Point3D m_Offset;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID { get { return m_MultiID; } set { m_MultiID = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get { return m_Offset; } set { m_Offset = value; } }

        public abstract BaseShip Ship { get; }

        public virtual int DoubloonCost { get { return 0; } }
        public virtual double DoubloonMultiplier { get { return 1.0; } }

        //-----Player Persistant Properties (Pushed to Ship)

        public string m_ShipName;
        public PlayerMobile m_Owner;

        public int m_HitPoints = 1000;
        public int m_SailPoints = 500;
        public int m_GunPoints = 500;

        public TargetingMode m_TargetingMode = TargetingMode.Hull;

        public List<Mobile> m_CoOwners = new List<Mobile>();
        public List<Mobile> m_Friends = new List<Mobile>();

        public bool m_IPAsCoOwners = false;
        public bool m_GuildAsCoOwners = false;
        public bool m_IPAsFriends = false;
        public bool m_GuildAsFriends = false;

        public ShipUpgrades.ThemeType m_ThemeUpgrade;
        public ShipUpgrades.PaintType m_PaintUpgrade;
        public ShipUpgrades.CannonMetalType m_CannonMetalUpgrade;
        public ShipUpgrades.OutfittingType m_OutfittingUpgrade;
        public ShipUpgrades.BannerType m_BannerUpgrade;
        public ShipUpgrades.CharmType m_CharmUpgrade;
        public ShipUpgrades.MinorAbilityType m_MinorAbility;
        public ShipUpgrades.MajorAbilityType m_MajorAbility;
        public ShipUpgrades.EpicAbilityType m_EpicAbility;

        //-----

        [Constructable]
        public BaseShipDeed(int id, Point3D offset): base(0x14F2)
        {
            Weight = 1.0;

            //-----

            m_HitPoints = Ship.MaxHitPoints;
            m_SailPoints = Ship.MaxSailPoints;
            m_GunPoints = Ship.MaxGunPoints;

            m_MultiID = id;
            m_Offset = offset;
        }

        public BaseShipDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", Name));
                
                if (DoubloonCost > 0)
                    ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Requires " + DoubloonCost.ToString() + " doubloons]"));                  
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(player.Backpack))
            {
                player.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.	
                return;
            }

            ShipGumpObject shipGumpObject = new ShipGumpObject(player, null, this);

            player.CloseGump(typeof(ShipGump));
            player.SendGump(new ShipGump(player, shipGumpObject));

            //from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
            //from.Target = new InternalTarget(this);
        }        

        public void OnPlacement(PlayerMobile player, Point3D location)
        {
            if (player == null) return;
            if (!player.Alive) return;
            if (Deleted) return;

            else if (!IsChildOf(player.Backpack))
                player.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.			

            else
            {
                Map map = player.Map;

                if (map == null)
                    return;

                if (player.AccessLevel < AccessLevel.GameMaster && map != Map.Felucca)
                {
                    player.SendLocalizedMessage(1043284); // A ship can not be created here.
                    return;
                }

                if (player.Region.IsPartOf(typeof(HouseRegion)) || BaseShip.FindShipAt(player, player.Map) != null)
                {
                    player.SendLocalizedMessage(1010568, null, 0x25); // You may not place a ship while on another ship or inside a house.
                    return;
                }

                Region region = Region.Find(location, map);

                if (region.IsPartOf(typeof(DungeonRegion)))
                {
                    player.SendLocalizedMessage(502488); // You can not place a ship inside a dungeon.
                    return;
                }

                if (region.IsPartOf(typeof(HouseRegion)))
                {
                    player.SendLocalizedMessage(1042549); // A ship may not be placed in this area.
                    return;
                }

                if (player.GetDistanceToSqrt(location) > 10)
                {
                    player.SendMessage("You cannot place a ship that far away from land.");
                    return;
                }

                foreach(BaseShip shipInstance in BaseShip.m_Instances)
                {
                    if (shipInstance.Owner == player)
                    {
                        player.SendMessage("You already have a ship at sea.");
                        return;
                    }
                }

                BaseShip ship = Ship;

                if (ship == null)
                    return;

                location = new Point3D(location.X - m_Offset.X, location.Y - m_Offset.Y, location.Z - m_Offset.Z);

                Direction newDirection = Direction.North;
                int shipFacingItemID = -1;

                switch (player.Direction)
                {
                    case Direction.North:
                        newDirection = Direction.North;
                        shipFacingItemID = ship.NorthID;
                    break;

                    case Direction.Up:
                        newDirection = Direction.North;
                        shipFacingItemID = ship.NorthID;
                    break;

                    case Direction.East:
                        newDirection = Direction.East;
                        shipFacingItemID = ship.EastID;
                    break;

                    case Direction.Right:
                        newDirection = Direction.East;
                        shipFacingItemID = ship.EastID;
                    break;

                    case Direction.South:
                        newDirection = Direction.South;
                        shipFacingItemID = ship.SouthID;
                    break;

                    case Direction.Down:
                        newDirection = Direction.South;
                        shipFacingItemID = ship.SouthID;
                    break;

                    case Direction.West:
                        newDirection = Direction.West;
                        shipFacingItemID = ship.WestID;
                    break;

                    case Direction.Left:
                        newDirection = Direction.West;
                        shipFacingItemID = ship.WestID;
                    break;

                    default:
                        newDirection = Direction.North;
                        shipFacingItemID = ship.NorthID;
                    break; 
                }

                if (BaseShip.IsValidLocation(location, map) && ship.CanFit(location, map, shipFacingItemID))
                {
                    ship.Owner = player;

                    ShipUniqueness.GenerateShipUniqueness(ship);

                    BaseShip.PushDeedStoredPropertiesToShip(this, ship);                                

                    ship.DecayTime = DateTime.UtcNow + ship.ShipDecayDelay;    
                    ship.Anchored = true;
                    ship.SetFacing(newDirection);

                    ship.MoveToWorld(location, map);                       

                    Delete();

                    ShipRune shipRune = new ShipRune(ship, player);
                    ship.ShipRune = shipRune;

                    ShipRune shipBankRune = new ShipRune(ship, player);
                    ship.ShipBankRune = shipBankRune;                        

                    bool addedToPack = false;
                    bool addedToBank = false;

                    if (player.AddToBackpack(shipRune))
                        addedToPack = true;

                    BankBox bankBox = player.FindBankNoCreate();

                    if (bankBox != null)
                    {
                        if (bankBox.Items.Count < bankBox.MaxItems)
                        {
                            bankBox.AddItem(shipBankRune);
                            addedToBank = true;
                        }
                    }

                    string message = "You place the ship at sea. A ship rune has been placed both in your bankbox and your backpack.";

                    if (!addedToPack && !addedToBank)
                        message = "You place the ship at sea. However, there was no room in neither your bankbox nor your backpack to place ship runes.";

                    else if (!addedToPack)
                        message = "You place the ship at sea. A ship rune was placed in your bankbox, however, there was no room in your backpack to place a ship rune.";

                    else if (!addedToBank)
                        message = "You place the ship at sea. A ship rune was placed in your backpack, however, there was no room in your bankbox to place a ship rune.";

                    player.SendMessage(message);
                }

                else
                {
                    ship.Delete();
                    player.SendMessage("A ship cannot be placed there. You may change your facing to change the direction of the ship placement.");
                }
            }
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_MultiID);
            writer.Write(m_Offset);

            //-----Player Persistant Properties (Pushed to Ship)

            writer.Write(m_ShipName);
            writer.Write(m_Owner);

            writer.Write(m_HitPoints);
            writer.Write(m_SailPoints);
            writer.Write(m_GunPoints);

            writer.Write((int)m_TargetingMode);

            writer.Write(m_CoOwners.Count);
            for (int a = 0; a < m_CoOwners.Count; a++)
            {
                writer.Write(m_CoOwners[a]);
            }

            writer.Write(m_Friends.Count);
            for (int a = 0; a < m_Friends.Count; a++)
            {
                writer.Write(m_Friends[a]);
            }

            writer.Write(m_IPAsCoOwners);
            writer.Write(m_GuildAsCoOwners);
            writer.Write(m_IPAsFriends);
            writer.Write(m_GuildAsFriends);

            writer.Write((int)m_ThemeUpgrade);
            writer.Write((int)m_PaintUpgrade);
            writer.Write((int)m_CannonMetalUpgrade);
            writer.Write((int)m_OutfittingUpgrade);
            writer.Write((int)m_BannerUpgrade);
            writer.Write((int)m_CharmUpgrade);
            writer.Write((int)m_MinorAbility);
            writer.Write((int)m_MajorAbility);
            writer.Write((int)m_EpicAbility);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            //Version 0
            if (version >= 0)
            {
                m_MultiID = reader.ReadInt();
                m_Offset = reader.ReadPoint3D();

                //-----

                m_ShipName = reader.ReadString();
                m_Owner = (PlayerMobile)reader.ReadMobile();

                m_HitPoints = reader.ReadInt();
                m_SailPoints = reader.ReadInt();
                m_GunPoints = reader.ReadInt();

                m_TargetingMode = (TargetingMode)reader.ReadInt();

                int coOwnerCount = reader.ReadInt();
                for (int a = 0; a < coOwnerCount; a++)
                {
                    m_CoOwners.Add(reader.ReadMobile());
                }

                int friendCount = reader.ReadInt();
                for (int a = 0; a < friendCount; a++)
                {
                    m_Friends.Add(reader.ReadMobile());
                }

                m_IPAsCoOwners = reader.ReadBool();
                m_GuildAsCoOwners = reader.ReadBool();
                m_IPAsFriends = reader.ReadBool();
                m_GuildAsFriends = reader.ReadBool();

                m_ThemeUpgrade = (ShipUpgrades.ThemeType)reader.ReadInt();
                m_PaintUpgrade = (ShipUpgrades.PaintType)reader.ReadInt();
                m_CannonMetalUpgrade = (ShipUpgrades.CannonMetalType)reader.ReadInt();
                m_OutfittingUpgrade = (ShipUpgrades.OutfittingType)reader.ReadInt();
                m_BannerUpgrade = (ShipUpgrades.BannerType)reader.ReadInt();
                m_CharmUpgrade = (ShipUpgrades.CharmType)reader.ReadInt();
                m_MinorAbility = (ShipUpgrades.MinorAbilityType)reader.ReadInt();
                m_MajorAbility = (ShipUpgrades.MajorAbilityType)reader.ReadInt();
                m_EpicAbility = (ShipUpgrades.EpicAbilityType)reader.ReadInt();
            }
        }
    }

    public class BindBaseShipDeedGump : Gump
    {
        BaseShipDeed m_BaseShipDeed;
        PlayerMobile m_Player;

        public BindBaseShipDeedGump(BaseShipDeed baseShipDeed, PlayerMobile player): base(50, 50)
        {
            m_BaseShipDeed = baseShipDeed;
            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddBackground(90, 150, 380, 185, 5054);
            AddBackground(100, 160, 360, 165, 3000);

            AddLabel(110, 175, 0, @"The following number of doubloons will be removed");
            AddLabel(110, 197, 0, @"from your bank box and the ship will be bound to you");
            AddHtml(118, 230, 320, 17, @"<center>" + m_BaseShipDeed.DoubloonCost.ToString() + "</center>", (bool)false, (bool)false);
            AddItem(255, 255, 2539);

            AddButton(170, 285, 247, 248, 1, GumpButtonType.Reply, 0); //Okay
            AddButton(310, 285, 243, 248, 2, GumpButtonType.Reply, 0); //Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1:
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= m_BaseShipDeed.DoubloonCost)
                    {
                        if (Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), m_BaseShipDeed.DoubloonCost))
                        {
                            Doubloon doubloonPile = new Doubloon(m_BaseShipDeed.DoubloonCost);
                            from.SendSound(doubloonPile.GetDropSound());
                            doubloonPile.Delete();                            
                           
                            m_Player.SendMessage("You claim the ship as your own.");
                        }

                        else
                            m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this ship.");
                    }

                    else
                        m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this ship.");
                    break;
            }
        }
    }
}