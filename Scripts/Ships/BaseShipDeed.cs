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
using Server.Multis;
using Server.Engines.Craft;

namespace Server
{
    public abstract class BaseShipDeed : Item, ICraftable
    {
        public virtual Type ShipType { get { return typeof(SmallShip); } }

        private int m_MultiID;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID { get { return m_MultiID; } set { m_MultiID = value; } }

        private Point3D m_Offset;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get { return m_Offset; } set { m_Offset = value; } }
        
        public bool m_Registered = false;

        public string m_ShipName;
        public PlayerMobile m_Owner;

        public int HitPoints = 0;
        public int MaxHitPoints = 0;
        public int SailPoints = 0;
        public int MaxSailPoints = 0;
        public int GunPoints = 0;
        public int MaxGunPoints = 0;

        public double ForwardSpeed = 1.0;
        public double DriftSpeed = 1.0;
        public double SlowdownModePenalty = 1.0;

        public double CannonAccuracy = 1.0;
        public double CannonMinDamage = 1.0;
        public double CannonMaxDamage = 1.0;
        public double CannonRange = 1.0;
        public double CannonReloadDuration = 1.0;

        public double MinorAbilityCooldownDuration = 1.0;
        public double MajorAbilityCooldownDuration = 1.0;
        public double EpicAbilityCooldownDuration = 1.0;

        public double RepairCooldownDuration = 1.0;
        public double BoardingChance = 1.0;

        public double HitPointsCreationScalar = 1.0;
        public double SailPointsCreationScalar = 1.0;
        public double GunPointsCreationScalar = 1.0;

        public double ForwardSpeedCreationScalar = 1.0;
        public double DriftSpeedCreationScalar = 1.0;
        public double SlowdownModePenaltyCreationScalar = 1.0;

        public double CannonAccuracyCreationScalar = 1.0;
        public double CannonDamageCreationScalar = 1.0;
        public double CannonRangeCreationScalar = 1.0;
        public double CannonReloadDurationCreationScalar = 1.0;

        public double MinorAbilityCooldownDurationCreationScalar = 1.0;
        public double MajorAbilityCooldownDurationCreationScalar = 1.0;
        public double EpicAbilityCooldownDurationCreationScalar = 1.0;

        public double RepairCooldownDurationCreationScalar = 1.0;
        public double BoardingChanceCreationScalar = 1.0;

        public TargetingMode m_TargetingMode = TargetingMode.Hull;        

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
        public ShipUpgrades.MinorAbilityType m_MinorAbilityUpgrade;
        public ShipUpgrades.MajorAbilityType m_MajorAbilityUpgrade;
        public ShipUpgrades.EpicAbilityType m_EpicAbilityUpgrade;

        public List<Mobile> m_CoOwners = new List<Mobile>();
        public List<Mobile> m_Friends = new List<Mobile>();

        //-----

        [Constructable]
        public BaseShipDeed(int id, Point3D offset): base(5363)
        {
            Weight = 1.0;

            //-----

            m_MultiID = id;
            m_Offset = offset;
        }

        public BaseShipDeed(Serial serial): base(serial)
        {
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (Quality)quality;
            
            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(ShipType); 

            return quality;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (!m_Registered)
            {
                ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(ShipType);

                int doubloonsRequired = shipStatsProfile.RegistrationDoubloonCost;

                LabelTo(from, "(must be registered before usable)");
            }

            else            
                LabelTo(from, "(double click to view ship details)");            
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;            

            if (!m_Registered)
            {
                player.CloseGump(typeof(ShipRegistrationGump));
                player.SendGump(new ShipRegistrationGump(player, this));
            }

            else
            {
                ShipGumpObject shipGumpObject = new ShipGumpObject(player, null, this);

                player.CloseGump(typeof(ShipGump));
                player.SendGump(new ShipGump(player, shipGumpObject));
            }           
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

                BaseShip ship = (BaseShip)Activator.CreateInstance(ShipType);

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
                    ship.m_ShipRune = shipRune;

                    ShipRune shipBankRune = new ShipRune(ship, player);
                    ship.m_ShipBankRune = shipBankRune;                        

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

            writer.Write(m_Registered);

            writer.Write(m_ShipName);
            writer.Write(m_Owner);

            writer.Write(HitPoints);
            writer.Write(MaxHitPoints);
            writer.Write(SailPoints);
            writer.Write(MaxSailPoints);
            writer.Write(GunPoints);
            writer.Write(MaxGunPoints);

            writer.Write(ForwardSpeed);
            writer.Write(DriftSpeed);
            writer.Write(SlowdownModePenalty);

            writer.Write(CannonAccuracy);
            writer.Write(CannonMinDamage);
            writer.Write(CannonMaxDamage);
            writer.Write(CannonRange);
            writer.Write(CannonReloadDuration);

            writer.Write(MinorAbilityCooldownDuration);
            writer.Write(MajorAbilityCooldownDuration);
            writer.Write(EpicAbilityCooldownDuration);

            writer.Write(RepairCooldownDuration);
            writer.Write(BoardingChance);

            writer.Write(HitPointsCreationScalar);
            writer.Write(SailPointsCreationScalar);
            writer.Write(GunPointsCreationScalar);

            writer.Write(ForwardSpeedCreationScalar);
            writer.Write(DriftSpeedCreationScalar);
            writer.Write(SlowdownModePenaltyCreationScalar);

            writer.Write(CannonAccuracyCreationScalar);
            writer.Write(CannonDamageCreationScalar);
            writer.Write(CannonRangeCreationScalar);
            writer.Write(CannonReloadDurationCreationScalar);

            writer.Write(MinorAbilityCooldownDurationCreationScalar);
            writer.Write(MajorAbilityCooldownDurationCreationScalar);
            writer.Write(EpicAbilityCooldownDurationCreationScalar);

            writer.Write(RepairCooldownDurationCreationScalar);
            writer.Write(BoardingChanceCreationScalar);

            writer.Write((int)m_TargetingMode);

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
            writer.Write((int)m_MinorAbilityUpgrade);
            writer.Write((int)m_MajorAbilityUpgrade);
            writer.Write((int)m_EpicAbilityUpgrade);

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

                m_Registered = reader.ReadBool();

                m_ShipName = reader.ReadString();
                m_Owner = (PlayerMobile)reader.ReadMobile();

                HitPoints = reader.ReadInt();
                MaxHitPoints = reader.ReadInt();
                SailPoints = reader.ReadInt();
                MaxSailPoints = reader.ReadInt();
                GunPoints = reader.ReadInt();
                MaxGunPoints = reader.ReadInt();

                ForwardSpeed = reader.ReadDouble();
                DriftSpeed = reader.ReadDouble();
                SlowdownModePenalty = reader.ReadDouble();

                CannonAccuracy = reader.ReadDouble();
                CannonMinDamage = reader.ReadDouble();
                CannonMaxDamage = reader.ReadDouble();
                CannonRange = reader.ReadDouble();
                CannonReloadDuration = reader.ReadDouble();

                MinorAbilityCooldownDuration = reader.ReadDouble();
                MajorAbilityCooldownDuration = reader.ReadDouble();
                EpicAbilityCooldownDuration = reader.ReadDouble();

                RepairCooldownDuration = reader.ReadDouble();
                BoardingChance = reader.ReadDouble();

                HitPointsCreationScalar = reader.ReadDouble();
                SailPointsCreationScalar = reader.ReadDouble();
                GunPointsCreationScalar = reader.ReadDouble();

                ForwardSpeedCreationScalar = reader.ReadDouble();
                DriftSpeedCreationScalar = reader.ReadDouble();
                SlowdownModePenaltyCreationScalar = reader.ReadDouble();

                CannonAccuracyCreationScalar = reader.ReadDouble();
                CannonDamageCreationScalar = reader.ReadDouble();
                CannonRangeCreationScalar = reader.ReadDouble();
                CannonReloadDurationCreationScalar = reader.ReadDouble();

                MinorAbilityCooldownDurationCreationScalar = reader.ReadDouble();
                MajorAbilityCooldownDurationCreationScalar = reader.ReadDouble();
                EpicAbilityCooldownDurationCreationScalar = reader.ReadDouble();

                RepairCooldownDurationCreationScalar = reader.ReadDouble();
                BoardingChanceCreationScalar = reader.ReadDouble();

                m_TargetingMode = (TargetingMode)reader.ReadInt();

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
                m_MinorAbilityUpgrade = (ShipUpgrades.MinorAbilityType)reader.ReadInt();
                m_MajorAbilityUpgrade = (ShipUpgrades.MajorAbilityType)reader.ReadInt();
                m_EpicAbilityUpgrade = (ShipUpgrades.EpicAbilityType)reader.ReadInt();

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
            }
        }
    }
}