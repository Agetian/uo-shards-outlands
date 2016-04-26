using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Movement;
using Server.Network;
using Server.Custom;
using Server.Multis;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using Server.Commands;
using Server.Guilds;

namespace Server
{
    public enum DamageType
    {
        Hull,
        Sails,
        Guns
    }

    public enum TargetingMode
    {
        Random,
        Hull,
        Sails,
        Guns
    }

    public enum DryDockResult 
    { 
        Valid,
        Dead, 
        NoKey, 
        NotAnchored, 
        Mobiles,
        Items, 
        Hold, 
        Decaying, 
        TooFar
    }

    public enum MobileControlType
    {
        Player,
        Good,
        Innocent,
        Neutral,
        Evil,
        Null
    }

    public enum MobileFactionType
    {
        None,
        Britain,
        Fishing,
        Pirate,
        Undead,
        Orghereim,
        Orc,
        Null
    }

    public abstract class BaseBoat : BaseMulti, ILogoutRetain
    {   
        private static void EventSink_WorldSave(WorldSaveEventArgs e)
        {
            new UpdateAllTimer().Start();
        }

        public static void Initialize()
        {
            new UpdateAllTimer().Start();

            EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);

            CommandSystem.Register("ShipHotbar", AccessLevel.Player, new CommandEventHandler(ShipHotbarCommand));

            CommandSystem.Register("Ship", AccessLevel.Player, new CommandEventHandler(ShipSelectionCommand));
            CommandSystem.Register("Boat", AccessLevel.Player, new CommandEventHandler(ShipSelectionCommand));
            CommandSystem.Register("Tillerman", AccessLevel.Player, new CommandEventHandler(ShipSelectionCommand));

            CommandSystem.Register("FireLeftCannons", AccessLevel.Player, new CommandEventHandler(FireLeftCannonsCommand));
            CommandSystem.Register("FireRightCannons", AccessLevel.Player, new CommandEventHandler(FireRightCannonsCommand));            

            CommandSystem.Register("RaiseAnchor", AccessLevel.Player, new CommandEventHandler(RaiseAnchorCommand));
            CommandSystem.Register("LowerAnchor", AccessLevel.Player, new CommandEventHandler(LowerAnchorCommand));

            CommandSystem.Register("Stop", AccessLevel.Player, new CommandEventHandler(StopCommand));

            CommandSystem.Register("Forward", AccessLevel.Player, new CommandEventHandler(ForwardCommand));
            CommandSystem.Register("ForwardLeft", AccessLevel.Player, new CommandEventHandler(ForwardLeftCommand));
            CommandSystem.Register("ForwardRight", AccessLevel.Player, new CommandEventHandler(ForwardRightCommand));
            CommandSystem.Register("Left", AccessLevel.Player, new CommandEventHandler(LeftCommand));
            CommandSystem.Register("Right", AccessLevel.Player, new CommandEventHandler(RightCommand));
            CommandSystem.Register("Backward", AccessLevel.Player, new CommandEventHandler(BackwardCommand));
            CommandSystem.Register("BackwardLeft", AccessLevel.Player, new CommandEventHandler(BackwardLeftCommand));
            CommandSystem.Register("BackwardRight", AccessLevel.Player, new CommandEventHandler(BackwardRightCommand));
            CommandSystem.Register("TurnLeft", AccessLevel.Player, new CommandEventHandler(TurnLeftCommand));
            CommandSystem.Register("TurnRight", AccessLevel.Player, new CommandEventHandler(TurnRightCommand));

            CommandSystem.Register("ForwardOne", AccessLevel.Player, new CommandEventHandler(ForwardOneCommand));
            CommandSystem.Register("ForwardLeftOne", AccessLevel.Player, new CommandEventHandler(ForwardLeftOneCommand));
            CommandSystem.Register("ForwardRightOne", AccessLevel.Player, new CommandEventHandler(ForwardRightOneCommand));
            CommandSystem.Register("LeftOne", AccessLevel.Player, new CommandEventHandler(LeftOneCommand));
            CommandSystem.Register("RightOne", AccessLevel.Player, new CommandEventHandler(RightOneCommand));
            CommandSystem.Register("BackwardOne", AccessLevel.Player, new CommandEventHandler(BackwardOne_OnCommand));
            CommandSystem.Register("BackwardOneLeft", AccessLevel.Player, new CommandEventHandler(BackwardOneLeft_OnCommand));
            CommandSystem.Register("BackwardOneRight", AccessLevel.Player, new CommandEventHandler(BackwardOneRight_OnCommand));
            
            CommandSystem.Register("ShipLocationOffsets", AccessLevel.GameMaster, new CommandEventHandler(ShipLocationOffsets));
            CommandSystem.Register("DeleteAllNPCShips", AccessLevel.GameMaster, new CommandEventHandler(DeleteAllNPCShips_OnCommand));
        }

        #region Boat Construction

        public BaseBoat(): base(0x0)
        {
            m_TillerMan = new TillerMan(this);
            m_Hold = new Hold(this);
            m_ShipTrashBarrel = new ShipTrashBarrel();

            m_PPlank = new Plank(this, PlankSide.Port, 0);
            m_SPlank = new Plank(this, PlankSide.Starboard, 0);

            m_PPlank.MoveToWorld(new Point3D(X + PortOffset.X, Y + PortOffset.Y, Z), Map);
            m_SPlank.MoveToWorld(new Point3D(X + StarboardOffset.X, Y + StarboardOffset.Y, Z), Map);

            Facing = Direction.North;

            Movable = false;

            HitPoints = MaxHitPoints;
            SailPoints = MaxSailPoints;
            GunPoints = MaxGunPoints;

            m_LastCombatTime = DateTime.UtcNow - TimeSpan.FromMinutes(5);
            m_TimeLastMoved = DateTime.UtcNow;
            m_TimeLastRepaired = DateTime.UtcNow;
            m_NextTimeRepairable = DateTime.UtcNow;

            ShipUniqueness.GenerateShipUniqueness(this);

            m_ConfigureShipTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(100), delegate { ConfigureShip(); });

            m_Instances.Add(this);
        }

        public BaseBoat(Serial serial): base(serial)
        {
        }

        public override bool AllowsRelativeDrop {  get { return true; } }

        public virtual void ConfigureShip()
        {
            if (Deleted)
                return;

            m_ConfigureShipTimer = null;

            GenerateShipCannons();
            ShipDeckItems.GenerateShipDeckItems(this);
            ShipCrew.GenerateShipCrew(this);
            ShipLoot.GenerateShipLoot(this);

            if (MobileControlType == MobileControlType.Player)
            {
                m_DecayTimer = new DecayTimer(this);
                m_DecayTimer.Start();
            }

            bool activate = false;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 50);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile is PlayerMobile)
                {
                    activate = true;
                    break;
                }
            }

            nearbyMobiles.Free();

            if (activate && (MobileControlType != MobileControlType.Player && MobileControlType != MobileControlType.Null))
            {
                m_BoatAITimer = new BoatAITimer(this);
                m_BoatAITimer.Start();
            }

            Refresh();
        }

        #endregion

        #region Boat Update Components Timer

        public class UpdateAllTimer : Timer
        {
            public UpdateAllTimer(): base(TimeSpan.FromSeconds(1.0))
            {
            }

            protected override void OnTick()
            {
                UpdateAllComponents();
            }
        }

        #endregion

        #region Commands

        [Usage("[ShipHotbar")]
        [Description("Displays the ship hotbar gump")]
        public static void ShipHotbarCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(BoatHotbarGump));
            player.SendGump(new BoatHotbarGump(player));
        }

        [Usage("[Ship or [Boat or [Tillerman")]
        [Description("Player targets a ship which open's that ship's gump")]
        public static void ShipSelectionCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            ShipSelection(player);
        }

        [Usage("FireLeftCannons")]
        [Description("Acts as if player double-clicked any cannon on the left side of the ship")]
        public static void FireLeftCannonsCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            FireCannons(player, true);
        }

        [Usage("FireRightCannons")]
        [Description("Acts as if player double-clicked any cannon on the right side of the ship")]
        public static void FireRightCannonsCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            FireCannons(player, false);
        }

        [Usage("RaiseAnchor")]
        [Description("Raises the ship's Anchor")]
        private static void RaiseAnchorCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            RaiseAnchor(player);           
        }

        public static void RaiseAnchor(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.RaiseAnchor(true);
        }

        [Usage("LowerAnchor")]
        [Description("Lowers the ship's Anchor")]
        private static void LowerAnchorCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            LowerAnchor(player);     
        }

        public static void LowerAnchor(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.LowerAnchor(true);
        }

        [Usage("Stop")]
        [Description("Stops the player's ship")]
        private static void StopCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            Stop(player);            
        }

        public static void Stop(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StopMove(false);
        }

        [Usage("Forward")]
        [Description("Moves the player's ship forward")]
        private static void ForwardCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveForward(player);     
        }

        public static void StartMoveForward(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(Forward, true, false);
        }

        [Usage("ForwardLeft")]
        [Description("Moves the player's ship forward left")]
        private static void ForwardLeftCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveForwardLeft(player);
        }

        public static void StartMoveForwardLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(ForwardLeft, true, false);
        }

        [Usage("ForwardRight")]
        [Description("Moves the player's ship forward right")]
        private static void ForwardRightCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveForwardRight(player);
        }

        public static void StartMoveForwardRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(ForwardRight, true, false);
        }

        [Usage("Left")]
        [Description("Moves the player's ship left")]
        private static void LeftCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveLeft(player);
        }

        public static void StartMoveLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(Left, true, false);
        }

        [Usage("Right")]
        [Description("Moves the player's ship right")]
        private static void RightCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveRight(player);
        }

        public static void StartMoveRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(Right, true, false);
        }

        [Usage("Backwards")]
        [Description("Moves the player's ship backward")]
        private static void BackwardCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveBackward(player);
        }

        public static void StartMoveBackward(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(Backward, true, false);
        }

        [Usage("BackwardLeft")]
        [Description("Moves the player's ship backward left")]
        private static void BackwardLeftCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveBackwardLeft(player);
        }

        public static void StartMoveBackwardLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(BackwardLeft, true, false);
        }

        [Usage("BackwardRight")]
        [Description("Moves the player's ship backward right")]
        private static void BackwardRightCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartMoveBackwardRight(player);
        }

        public static void StartMoveBackwardRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartMove(BackwardRight, true, false);
        }

        [Usage("TurnLeft")]
        [Description("Turns the player's ship left")]
        private static void TurnLeftCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartTurnLeft(player);
        }

        public static void StartTurnLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartTurn(-2, false);
        }

        [Usage("TurnRight")]
        [Description("Turns the player's ship right")]
        private static void TurnRightCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            StartTurnRight(player);
        }

        public static void StartTurnRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.StartTurn(2, false);
        }

        [Usage("ForwardOne")]
        [Description("Moves the player's ship forward one space")]
        private static void ForwardOneCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveForward(player);
        }

        public static void OneMoveForward(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(Forward);
        }

        [Usage("ForwardLeftOne")]
        [Description("Moves the player's ship forward left one space")]
        private static void ForwardLeftOneCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveForwardLeft(player);
        }

        public static void OneMoveForwardLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(ForwardLeft);
        }

        [Usage("ForwardRightOne")]
        [Description("Moves the player's ship forward right one space")]
        private static void ForwardRightOneCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveForwardRight(player);
        }

        public static void OneMoveForwardRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(ForwardRight);
        }

        [Usage("LeftOne")]
        [Description("Moves the player's ship left one space")]
        private static void LeftOneCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveLeft(player);
        }

        public static void OneMoveLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(Left);
        }

        [Usage("RightOne")]
        [Description("Moves the player's ship right one space")]
        private static void RightOneCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveRight(player);
        }

        public static void OneMoveRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(Right);
        }

        [Usage("BackwardOne")]
        [Description("Moves the player's ship backward one space")]
        private static void BackwardOne_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveBackward(player);
        }

        public static void OneMoveBackward(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(Backward);
        }

        [Usage("BackwardOneLeft")]
        [Description("Moves the player's ship backward left one space")]
        private static void BackwardOneLeft_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveBackwardLeft(player);
        }

        public static void OneMoveBackwardLeft(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(BackwardLeft);
        }

        [Usage("BackwardOneRight")]
        [Description("Moves the player's ship backward right one space")]
        private static void BackwardOneRight_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            OneMoveBackwardRight(player);
        }

        public static void OneMoveBackwardRight(PlayerMobile player)
        {
            if (player == null) return;
            if (player.BoatOccupied == null) return;

            if (player.BoatOccupied.IsOwner(player) || player.BoatOccupied.IsCoOwner(player))
                player.BoatOccupied.OneMove(BackwardRight);
        }

        [Usage("[ShipLocationOffsets")]
        [Description("Gets the players location offsets relative to the ship location")]
        public static void ShipLocationOffsets(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player != null)
            {
                BaseBoat playerBoat = BaseBoat.FindBoatAt(player.Location, player.Map);

                if (playerBoat != null)
                {
                    int x = player.Location.X - playerBoat.Location.X;
                    int y = player.Location.Y - playerBoat.Location.Y;
                    int z = player.Location.Z - playerBoat.Location.Z;

                    player.SendMessage("Offsets: " + x + "," + y + "," + z);
                }
            }
        }

        [Usage("DeleteAllNPCShips")]
        [Description("Deletes All NPC Ships Currently Spawned")]
        private static void DeleteAllNPCShips_OnCommand(CommandEventArgs e)
        {
            Mobile mobile = e.Mobile;

            PlayerMobile player = mobile as PlayerMobile;

            if (player == null)
                return;

            int boatCount = m_Instances.Count;
            int deleteCount = 0;
            {
                for (int a = 0; a < boatCount; a++)
                {
                    int index = a - deleteCount;

                    BaseBoat boat = m_Instances[index];

                    if (boat.MobileControlType != MobileControlType.Player)
                    {
                        m_Instances.RemoveAt(index);
                        boat.Delete();
                        deleteCount++;
                    }
                }
            }

            player.SendMessage("All NPC Ships deleted.");
        }
       
        public static void FireCannons(Mobile from, bool leftSide)
        {
             /*
            BaseBoat boat = BaseBoat.FindBoatAt(from.Location, from.Map);
            Server.Custom.Pirates.BaseCannon cannonToUse = null;

            if (boat == null)
            {
                from.SendMessage("You are not currently on a boat.");
                return;
            }

            if (boat.m_Cannons.Count == 0)
            {
                from.SendMessage("That ship does not have any cannons.");
                return;
            }

            if (!(boat.IsOwner(from) || boat.IsCoOwner(from)))
            {
                from.SendMessage("You do not have permission to fire cannons on that boat.");
                return;
            }

            foreach (Server.Custom.Pirates.BaseCannon cannon in boat.Cannons)
            {
                //Ship North
                if (boat.Facing == Direction.North)
                {
                    if (leftSide && cannon.Facing == Direction.West)
                    {
                        cannonToUse = cannon;
                        break;
                    }

                    if (!leftSide && cannon.Facing == Direction.East)
                    {
                        cannonToUse = cannon;
                        break;
                    }
                }

                //Ship East
                if (boat.Facing == Direction.East)
                {
                    if (leftSide && cannon.Facing == Direction.North)
                    {
                        cannonToUse = cannon;
                        break;
                    }

                    if (!leftSide && cannon.Facing == Direction.South)
                    {
                        cannonToUse = cannon;
                        break;
                    }
                }

                //Ship South
                if (boat.Facing == Direction.South)
                {
                    if (leftSide && cannon.Facing == Direction.East)
                    {
                        cannonToUse = cannon;
                        break;
                    }

                    if (!leftSide && cannon.Facing == Direction.West)
                    {
                        cannonToUse = cannon;
                        break;
                    }
                }

                //Ship West
                if (boat.Facing == Direction.West)
                {
                    if (leftSide && cannon.Facing == Direction.South)
                    {
                        cannonToUse = cannon;
                        break;
                    }

                    if (!leftSide && cannon.Facing == Direction.North)
                    {
                        cannonToUse = cannon;
                        break;
                    }
                }
            }

            if (cannonToUse != null)
                cannonToUse.OnDoubleClick(from);
            */
        }        

        public static void ShipSelection(PlayerMobile player)
        {
            if (player == null)
                return;

            player.SendMessage("Which ship would you like to select?");
            player.Target = new ShipTarget(player);
        }

        private class ShipTarget : Target
        {
            private Mobile m_Mobile;

            public ShipTarget(Mobile mobile): base(25, true, TargetFlags.None)
            {
                m_Mobile = mobile;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                        p = ((Item)p).GetWorldTop();

                    else if (p is Mobile)
                        p = ((Mobile)p).Location;

                    BaseBoat boatAtLocation = BaseBoat.FindBoatAt(p, from.Map);

                    if (boatAtLocation != null)
                    {
                        if (boatAtLocation.Owner != null)
                        {
                            //if (!boatAtLocation.m_ScuttleInProgress)
                                //from.SendGump(new BoatGump(from, boatAtLocation));
                        }
                    }

                    else
                        from.SendMessage("That is not a targetable ship.");
                }
            }
        }

        #endregion               

        #region Properties

        Point3D ILogoutRetain.LoginLocation { get { return GetMarkedLocation(); } }
        Map ILogoutRetain.LoginMap { get { return Map; } }

        private static Rectangle2D[] m_BritWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 5120 - 32, 4096 - 32), new Rectangle2D(5136, 2320, 992, 1760) };
        private static Rectangle2D[] m_IlshWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 2304 - 32, 1600 - 32) };
        private static Rectangle2D[] m_TokunoWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 1448 - 32, 1448 - 32) };

        public const int MaxFriends = 100;
        public const int MaxCoOwners = 100;

        public static double shipBasedDamageToPlayerScalar = 0.5;
        public static double shipBasedDamageToCreatureScalar = 0.5;

        public TimeSpan TimeNeededToBeOutOfCombat = TimeSpan.FromSeconds(60); //Time Needed to Be Out of Combat For Fast Ship Repair and Dry Docking
        public TimeSpan DryDockMinimumLastMovement = TimeSpan.FromSeconds(10); //Minimum Time Needed Since Last Movement for Ship Docking

        public TimeSpan DamageEntryDuration = TimeSpan.FromMinutes(60);
        public TimeSpan BoatDecayDelay = TimeSpan.FromHours(72);
        public TimeSpan DeactivateDelay = TimeSpan.FromMinutes(2);

        private double m_AcquireTargetDelayAmount = Utility.RandomMinMax(3, 5);
        private double m_AcquireNewTargetDelayAmount = Utility.RandomMinMax(8, 10);

        public int BaseMaxHitPoints = 1000;
        public int BaseMaxSailPoints = 500;
        public int BaseMaxGunPoints = 500;

        public MobileControlType BaseMobileControlType = MobileControlType.Player;
        public MobileFactionType BaseMobileFactionType = MobileFactionType.None;

        public int BasePerceptionRange = 24;

        public double BaseCannonAccuracyModifer = 1.0;
        public double BaseCannonRangeScalar = 1.0;
        public double BaseCannonDamageScalar = 1.0;
        public double BaseCannonReloadTimeScalar = 1.0;
        public double BaseDamageFromPlayerBoatScalar = 1.0;

        public static int CannonMaxAmmunition = 10;
        public static int CannonMaxRange = 12;

        public static int CannonFiringLoops = 3;
        public static double CannonLoopDelay = .5;

        public static int CannonExplosionRange = 1; //How large is the cannon blast radius

        public static double CannonOceanCreatureDamageMultiplier = .75; //Damage scalar for creatures (on Water)
        public static double CannonMobileDamageMultiplier = .5; //Damage scalar for creatures & NPCs (on Land or Ships)
        public static double CannonPlayerDamageMultiplier = .5; //Damage scalar for players (on Land or Ships)
        public static double CannonIndirectHitDamageMultiplier = .25; //If explosion range is larger than 1, damage modifier for Mobiles outside of target location
        public static double CannonShipIndirectHitDamageMultiplier = .75; //Ship damage modifier if player directly hit instead of the ship

        public static double CannonMovementMaxAccuracyPenalty = 0.2; //Maximum Accuracy Penalty for Ship Moving or Having Recently Moved
        public static double CannonTargetMovementMaxAccuracyPenalty = 0.2; //Maximum Accuracy Penalty for Opponent's Moving or Having Recently Moved
        public static double CannonMovementAccuracyCooldown = 10.0; //Seconds after stopping ship movement before no penalty to accuracy exists: scales from 0 to this number of seconds

        public static double CannonMaxMisfireChance = 0.40;

        public static double CannonAccuracy = 0.8;
        public static int CannonDamageMin = 20;
        public static int CannonDamageMax = 30;
        public static double CannonCooldownTime = 10.0;
        public static double CannonReloadTime = 2.0;

        public double BaseFastInterval = 0.20;
        public double BaseFastDriftInterval = 0.40;

        public double BaseSlowInterval = 0.40;
        public double BaseSlowDriftInterval = 1.0;

        public int BaseDoubloonValue = 0;

        public virtual int ReducedSpeedModeMinDuration { get { return 10; } }
        public virtual int ReducedSpeedModeMaxDuration { get { return 20; } }
        public virtual int ReducedSpeedModeCooldown { get { return 10; } }

        public static TimeSpan ScuttleInterval = TimeSpan.FromSeconds(10);
        public static TimeSpan PlayerShipDecayDamageDelay = TimeSpan.FromSeconds(10);
        public static TimeSpan NPCShipUncrewedDamageDelay = TimeSpan.FromSeconds(10);
        
        public List<ShipCannon> m_Cannons = new List<ShipCannon>();
        public List<ShipCannon> m_LeftCannons = new List<ShipCannon>();
        public List<ShipCannon> m_RightCannons = new List<ShipCannon>();
        public List<ShipCannon> m_FrontCannons = new List<ShipCannon>();
        public List<ShipCannon> m_RearCannons = new List<ShipCannon>();  
        
        public abstract List<Point3D> m_EmbarkLocations();
        public abstract List<Point3D> m_MastLocations();        
        public abstract List<Point3D> m_BoatFireLocations();

        private static readonly TimeSpan m_TillermanHoldTime = TimeSpan.FromSeconds(15);

        private DateTime m_LastActivated;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastActivated
        {
            get { return m_LastActivated; } 
            set { m_LastActivated = value; }
        }        

        private DateTime m_NextSinkDamageAllowed;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSinkDamageAllowed 
        {
            get { return m_NextSinkDamageAllowed; }
            set { m_NextSinkDamageAllowed = value; } 
        }    

        public List<Mobile> m_CoOwners = new List<Mobile>();
        public List<Mobile> CoOwners
        {
            get { return m_CoOwners; }
            set { m_CoOwners = value; }
        }

        private List<Mobile> m_Friends = new List<Mobile>();
        public List<Mobile> Friends
        {
            get { return m_Friends; }
            set { m_Friends = value; }
        }

        private List<Mobile> m_ParticipatingMobiles = new List<Mobile>();
        public List<Mobile> ParticipatingMobiles
        { 
            get { return m_ParticipatingMobiles; } 
            set { m_ParticipatingMobiles = value; }
        }

        private List<Mobile> m_EmbarkedMobiles = new List<Mobile>();
        public List<Mobile> EmbarkedMobiles
        { 
            get { return m_EmbarkedMobiles; } 
            set { m_EmbarkedMobiles = value; } 
        }

        private List<Mobile> m_Crew = new List<Mobile>();
        public List<Mobile> Crew
        {
            get { return m_Crew; }
            set { m_Crew = value; } 
        }   

        private double m_TempSpeedModifier = 1.0;
        [CommandProperty(AccessLevel.Counselor)]
        public double TempSpeedModifier
        {
            get { return m_TempSpeedModifier; }
            set
            {
                m_TempSpeedModifier = value;
            }
        }

        private DateTime m_TempSpeedModifierExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.Counselor)]
        public DateTime TempSpeedModifierExpiration
        {
            get { return m_TempSpeedModifierExpiration; }
            set { m_TempSpeedModifierExpiration = value; }
        }        

        private bool m_AdminControlled = false;
        [CommandProperty(AccessLevel.Counselor)]
        public bool AdminControlled
        {
            get { return m_AdminControlled; }
            set { m_AdminControlled = value; }
        }

        private BaseBoat m_BoatCombatant = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatCombatant
        {
            get { return m_BoatCombatant; }
            set
            {
                if (value != this)
                    m_BoatCombatant = value;
            }
        }

        private bool m_GuildAsFriends = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuildAsFriends
        {
            get { return m_GuildAsFriends; }
            set { m_GuildAsFriends = value; }
        } 
       
        private int m_ClientSpeed;  
      
        private string m_ShipName;

        public Timer m_TurnTimer;
        public Timer m_ShipDamageEntryTimer;
        public Timer m_CannonCooldownTimer;
        public Timer m_DecayTimer;
        public Timer m_SinkTimer;
        public Timer m_ScuttleTimer;
        public Timer m_ConfigureShipTimer;
        public Timer m_BoatAITimer;

        public bool m_ScuttleInProgress = false;

        public ShipSpawner m_ShipSpawner;
       
        private DateTime m_LastAcquireTarget;

        public List<Item> m_ShipItems = new List<Item>();     
        public List<Item> m_ItemsToSink = new List<Item>();
        public List<Mobile> m_MobilesToSink = new List<Mobile>();

        private bool m_Destroyed = false;

        private int m_HitPoints;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set
            {
                m_HitPoints = value;

                if (m_HitPoints > MaxHitPoints)
                    m_HitPoints = MaxHitPoints;

                if (m_HitPoints < 0)
                    m_HitPoints = 0;

                //Determine Boat Fires
                int maxBoatFires = (int)((double)m_EmbarkLocations().Count / 3);
                int currentBoatFires = m_BoatFires.Count;

                double missingHitPointsPercent = (double)(1 - (float)HitPoints / (float)MaxHitPoints);
                double percentBufferBeforeFires = .05;
                double fireInterval = (1 - percentBufferBeforeFires) / (double)maxBoatFires;

                double adjustedMissingHitPointsPercent = missingHitPointsPercent - percentBufferBeforeFires;

                int desiredBoatFires = 0;

                if (adjustedMissingHitPointsPercent > 0)
                    desiredBoatFires = (int)(Math.Ceiling(adjustedMissingHitPointsPercent / fireInterval));

                if (desiredBoatFires > maxBoatFires)
                    desiredBoatFires = maxBoatFires;

                int newBoatFires = desiredBoatFires - currentBoatFires;
                int boatFiresToRemove = 0;

                if (newBoatFires < 0)
                    boatFiresToRemove = Math.Abs(newBoatFires);

                //Add New Fires
                if (newBoatFires > 0)
                {
                    int startingBoatFires = m_BoatFires.Count;

                    for (int a = 0; a < newBoatFires; a++)
                    {
                        List<Point3D> m_PotentialBoatFireLocations = new List<Point3D>();

                        foreach (Point3D embarkLocation in m_EmbarkLocations())
                        {
                            bool foundActiveBoatFire = false;

                            foreach (BoatFireItem activeBoatFire in m_BoatFires)
                            {
                                Point3D activeBoatFireLocation = new Point3D(activeBoatFire.xOffset, activeBoatFire.yOffset, activeBoatFire.zOffset);

                                if (embarkLocation == activeBoatFireLocation)
                                {
                                    foundActiveBoatFire = true;
                                    break;
                                }
                            }

                            if (foundActiveBoatFire == false)
                                m_PotentialBoatFireLocations.Add(embarkLocation);
                        }

                        Point3D boatFireLocation = m_PotentialBoatFireLocations[Utility.RandomMinMax(0, m_PotentialBoatFireLocations.Count - 1)];
                        Point3D rotatedBoatFireLocation = GetRotatedLocation(boatFireLocation.X, boatFireLocation.Y, boatFireLocation.Z);

                        if (!(rotatedBoatFireLocation is Point3D))
                            continue;

                        Point3D point = new Point3D(rotatedBoatFireLocation.X + this.X, rotatedBoatFireLocation.Y + this.Y, rotatedBoatFireLocation.Z + this.Z);

                        if (Facing == Direction.West || Facing == Direction.East)
                        {
                            BoatFireItem b = new BoatFireItem(0x398C, point, this.Map, TimeSpan.FromMinutes(60), boatFireLocation.X, boatFireLocation.Y, boatFireLocation.Z);
                            m_BoatFires.Add(b);
                        }

                        else
                        {
                            BoatFireItem b = new BoatFireItem(0x3996, point, this.Map, TimeSpan.FromMinutes(60), boatFireLocation.X, boatFireLocation.Y, boatFireLocation.Z);
                            m_BoatFires.Add(b);
                        }
                    }
                }

                //Remove Some Existing Fires
                else if (boatFiresToRemove > 0)
                {
                    for (int a = 0; a < boatFiresToRemove; a++)
                    {
                        BoatFireItem boatFire = m_BoatFires[m_BoatFires.Count - 1];
                        m_BoatFires.RemoveAt(m_BoatFires.Count - 1);
                        boatFire.Delete();
                    }
                }
            }
        }

        private int m_MaxHitPoints = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                if (m_MaxHitPoints == -1)
                    return BaseMaxHitPoints;

                return m_MaxHitPoints;
            }

            set
            {
                m_MaxHitPoints = value;

                if (HitPoints > m_MaxHitPoints)
                    HitPoints = m_MaxHitPoints;

                if (m_MaxHitPoints < 0)
                    m_MaxHitPoints = 0;
            }
        }

        private int m_SailPoints;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SailPoints
        {
            get { return m_SailPoints; }
            set
            {
                m_SailPoints = value;

                if (m_SailPoints > MaxSailPoints)
                    m_SailPoints = MaxSailPoints;

                else if (m_SailPoints < 0)
                    m_SailPoints = 0;
            }
        }

        private int m_MaxSailPoints = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSailPoints
        {
            get
            {
                if (m_MaxSailPoints == -1)
                    return BaseMaxSailPoints;

                return m_MaxSailPoints;
            }

            set
            {
                m_MaxSailPoints = value;

                if (SailPoints > m_MaxSailPoints)
                    SailPoints = m_MaxSailPoints;

                if (m_MaxSailPoints < 0)
                    m_MaxSailPoints = 0;
            }
        }

        private int m_GunPoints;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GunPoints
        {
            get { return m_GunPoints; }
            set
            {
                m_GunPoints = value;

                if (m_GunPoints > MaxGunPoints)
                    m_GunPoints = MaxGunPoints;

                else if (m_GunPoints < 0)
                    m_GunPoints = 0;
            }
        }

        private int m_MaxGunPoints = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxGunPoints
        {
            get
            {
                if (m_MaxGunPoints == -1)
                    return BaseMaxGunPoints;

                return m_MaxGunPoints;
            }

            set
            {
                m_MaxGunPoints = value;

                if (GunPoints > m_MaxGunPoints)
                    GunPoints = m_MaxGunPoints;

                if (m_MaxGunPoints < 0)
                    m_MaxGunPoints = 0;
            }
        }

        private MobileControlType m_MobileControlType = MobileControlType.Null;
        [CommandProperty(AccessLevel.GameMaster)]
        public MobileControlType MobileControlType
        {
            get
            {
                if (m_MobileControlType == MobileControlType.Null)
                    return BaseMobileControlType;

                return m_MobileControlType;
            }

            set
            {
                m_MobileControlType = value;
            }
        }

        private MobileFactionType m_MobileFactionType = MobileFactionType.Null;
        [CommandProperty(AccessLevel.GameMaster)]
        public MobileFactionType MobileFactionType
        {
            get
            {
                if (m_MobileFactionType == MobileFactionType.Null)
                    return BaseMobileFactionType;

                return m_MobileFactionType;
            }

            set
            {
                m_MobileFactionType = value;
            }
        }

        private int m_DoubloonValue = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DoubloonValue
        {
            get
            {
                if (m_DoubloonValue == -1)
                    return BaseDoubloonValue;

                return m_DoubloonValue;
            }

            set
            {
                m_DoubloonValue = value;

                if (m_DoubloonValue < 0)
                    m_DoubloonValue = 0;
            }
        }

        private int m_PerceptionRange = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PerceptionRange
        {
            get
            {
                if (m_PerceptionRange == -1)
                    return BasePerceptionRange;

                return m_PerceptionRange;
            }

            set
            {
                m_PerceptionRange = value;

                if (m_PerceptionRange < 0)
                    m_PerceptionRange = 0;
            }
        }

        private double m_CannonAccuracyModifer = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CannonAccuracyModifer
        {
            get
            {
                if (m_CannonAccuracyModifer == -1)
                    return BaseCannonAccuracyModifer;

                return m_CannonAccuracyModifer;
            }

            set
            {
                m_CannonAccuracyModifer = value;

                if (m_CannonAccuracyModifer < 0)
                    m_CannonAccuracyModifer = 0;
            }
        }

        private double m_CannonRangeScalar = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CannonRangeScalar
        {
            get
            {
                if (m_CannonRangeScalar == -1)
                    return BaseCannonRangeScalar;

                return m_CannonRangeScalar;
            }

            set
            {
                m_CannonRangeScalar = value;

                if (m_CannonRangeScalar < 0)
                    m_CannonRangeScalar = 0;
            }
        }

        private double m_CannonDamageScalar = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CannonDamageScalar
        {
            get
            {
                if (m_CannonDamageScalar == -1)
                    return BaseCannonDamageScalar;

                return m_CannonDamageScalar;
            }

            set
            {
                m_CannonDamageScalar = value;

                if (m_CannonDamageScalar < 0)
                    m_CannonDamageScalar = 0;
            }
        }

        private double m_CannonReloadTimeScalar = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CannonReloadTimeScalar
        {
            get
            {
                if (m_CannonReloadTimeScalar == -1)
                    return BaseCannonReloadTimeScalar;

                return m_CannonReloadTimeScalar;
            }

            set
            {
                m_CannonReloadTimeScalar = value;

                if (m_CannonReloadTimeScalar < 0)
                    m_CannonReloadTimeScalar = 0;
            }
        }

        private double m_DamageFromPlayerBoatScalar = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double DamageFromPlayerBoatScalar
        {
            get
            {
                if (m_DamageFromPlayerBoatScalar == -1)
                    return BaseDamageFromPlayerBoatScalar;

                return m_DamageFromPlayerBoatScalar;
            }

            set
            {
                m_DamageFromPlayerBoatScalar = value;

                if (m_DamageFromPlayerBoatScalar < 0)
                    m_DamageFromPlayerBoatScalar = 0;
            }
        }

        private double m_FastInterval = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double FastInterval
        {
            get
            {
                if (m_FastInterval == -1)
                    return BaseFastInterval;

                return m_FastInterval;
            }

            set
            {
                m_FastInterval = value;

                if (m_FastInterval < 0)
                    m_FastInterval = 0;
            }
        }

        private double m_FastDriftInterval = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double FastDriftInterval
        {
            get
            {
                if (m_FastDriftInterval == -1)
                    return BaseFastDriftInterval;

                return m_FastDriftInterval;
            }

            set
            {
                m_FastDriftInterval = value;

                if (m_FastDriftInterval < 0)
                    m_FastDriftInterval = 0;
            }
        }

        private double m_SlowInterval = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double SlowInterval
        {
            get
            {
                if (m_SlowInterval == -1)
                    return BaseSlowInterval;

                return m_SlowInterval;
            }

            set
            {
                m_SlowInterval = value;

                if (m_SlowInterval < 0)
                    m_SlowInterval = 0;
            }
        }

        private double m_SlowDriftInterval = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double SlowDriftInterval
        {
            get
            {
                if (m_SlowDriftInterval == -1)
                    return BaseSlowDriftInterval;

                return m_SlowDriftInterval;
            }

            set
            {
                m_SlowDriftInterval = value;

                if (m_SlowDriftInterval < 0)
                    m_SlowDriftInterval = 0;
            }
        }

        private DateTime m_CannonCooldown = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CannonCooldown
        {
            get { return m_CannonCooldown; }
            set { m_CannonCooldown = value; }
        }

        private TargetingMode m_TargetingMode = TargetingMode.Hull;
        [CommandProperty(AccessLevel.GameMaster)]
        public TargetingMode TargetingMode
        {
            get { return m_TargetingMode; }
            set { m_TargetingMode = value; }
        }
               

        private List<BoatFireItem> m_BoatFires = new List<BoatFireItem>();
        public List<BoatFireItem> BoatFires
        {
            get { return m_BoatFires; }
            set { m_BoatFires = value; }
        }

        private List<ShipDamageEntry> m_ShipDamageEntries = new List<ShipDamageEntry>();
        public List<ShipDamageEntry> ShipDamageEntries
        {
            get { return m_ShipDamageEntries; }
            set { m_ShipDamageEntries = value; }
        }

        private DateTime m_TimeLastMoved;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeLastMoved
        {
            get { return m_TimeLastMoved; }
            set { m_TimeLastMoved = value; }
        }

        private DateTime m_LastCombatTime;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastCombatTime
        {
            get { return m_LastCombatTime; }
            set { m_LastCombatTime = value; }
        }

        private DateTime m_TimeLastRepaired;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeLastRepaired
        {
            get { return m_TimeLastRepaired; }
            set { m_TimeLastRepaired = value; }
        }

        private DateTime m_NextTimeRepairable;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextTimeRepairable
        {
            get { return m_NextTimeRepairable; }
            set { m_NextTimeRepairable = value; }
        }

        private DateTime m_DecayTime;
        public DateTime DecayTime
        {
            get { return m_DecayTime; }
            set { m_DecayTime = value; }
        }
        
        private Boolean m_ReducedSpeedMode;
        [CommandProperty(AccessLevel.GameMaster)]
        public Boolean ReducedSpeedMode
        {
            get { return m_ReducedSpeedMode; }
            set { m_ReducedSpeedMode = value; }
        }

        private DateTime m_ReducedSpeedModeTime;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ReducedSpeedModeTime
        {
            get { return m_ReducedSpeedModeTime; }
            set { m_ReducedSpeedModeTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }

            set
            {
                if (Hue == value)
                    return;

                if (m_TillerMan != null)
                    m_TillerMan.Hue = value;

                if (m_Hold != null)
                    m_Hold.Hue = value;

                if (m_PPlank != null)
                    m_PPlank.Hue = value;

                if (m_SPlank != null)
                    m_SPlank.Hue = value;

                base.Hue = value;
            }
        }

        private int m_CannonHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CannonHue
        {
            get { return m_CannonHue; }
            set { m_CannonHue = value; }
        }

        private Hold m_Hold;
        [CommandProperty(AccessLevel.GameMaster)]
        public Hold Hold 
        { 
            get { return m_Hold; } 
            set { m_Hold = value; } 
        }

        private TillerMan m_TillerMan;
        [CommandProperty(AccessLevel.GameMaster)]
        public TillerMan TillerMan
        {
            get { return m_TillerMan; } 
            set { m_TillerMan = value; } 
        }

        private ShipTrashBarrel m_ShipTrashBarrel;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipTrashBarrel ShipTrashBarrel
        {
            get { return m_ShipTrashBarrel; } 
            set { m_ShipTrashBarrel = value; } 
        }

        private Plank m_PPlank;
        [CommandProperty(AccessLevel.GameMaster)]
        public Plank PPlank 
        { 
            get { return m_PPlank; }
            set { m_PPlank = value; } 
        }

        private Plank m_SPlank;
        [CommandProperty(AccessLevel.GameMaster)]
        public Plank SPlank
        {
            get { return m_SPlank; } 
            set { m_SPlank = value; }
        }

        private Mobile m_Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set  { m_Owner = value; }
        }

        private Direction m_Facing;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing 
        { 
            get { return m_Facing; } 
            set { SetFacing(value); } 
        }

        private Direction m_Moving;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving 
        { 
            get { return m_Moving; } 
            set { m_Moving = value; } 
        }

        private Timer m_MoveTimer;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving 
        {
            get { return (m_MoveTimer != null); }
        }

        private int m_Speed;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        { 
            get { return m_Speed; }
            set { m_Speed = value; } 
        }

        private bool m_Anchored;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored 
        { 
            get { return m_Anchored; } 
            set { m_Anchored = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName 
        { 
            get { return m_ShipName; }
            set
            { 
                m_ShipName = value; 

                if (m_TillerMan != null) 
                    m_TillerMan.InvalidateProperties();
            } 
        }

        public virtual BaseBoatDeed BoatDeed { get { return null; } }

        public BoatRune BoatRune = null;
        public BoatRune BoatBankRune = null;

        public static List<BaseBoat> m_Instances = new List<BaseBoat>();

        private static bool NewBoatMovement { get { return true; } }

        private static int SlowSpeed = 1;
        private static int FastSpeed = 1;

        private static int SlowDriftSpeed = 1;
        private static int FastDriftSpeed = 1;

        private static Direction Forward = Direction.North;
        private static Direction ForwardLeft = Direction.Up;
        private static Direction ForwardRight = Direction.Right;
        private static Direction Backward = Direction.South;
        private static Direction BackwardLeft = Direction.Left;
        private static Direction BackwardRight = Direction.Down;
        private static Direction Left = Direction.West;
        private static Direction Right = Direction.East;
        private static Direction Port = Left;
        private static Direction Starboard = Right;

        public virtual int NorthID { get { return 0; } }
        public virtual int EastID { get { return 0; } }
        public virtual int SouthID { get { return 0; } }
        public virtual int WestID { get { return 0; } }

        public virtual int HoldDistance { get { return 0; } }
        public virtual int TillerManDistance { get { return 0; } }
        public virtual int Width { get { return 2; } }

        public virtual Point2D StarboardOffset { get { return Point2D.Zero; } }
        public virtual Point2D PortOffset { get { return Point2D.Zero; } }
        public virtual Point3D MarkOffset { get { return Point3D.Zero; } }

        public override bool HandlesOnSpeech { get { return true; } }

        private DateTime m_TillermanRelease;       
        
        #endregion       

        #region Embark / Disembark

        public bool AddEmbarkedMobile(Mobile mobile)
        {
            if (!EmbarkedMobiles.Contains(mobile))
                EmbarkedMobiles.Add(mobile);

            return true;
        }

        public bool RemoveEmbarkedMobile(Mobile mobile)
        {
            if (EmbarkedMobiles.Contains(mobile))
                EmbarkedMobiles.Remove(mobile);

            return true;
        }

        public bool TransferEmbarkedMobile(Mobile mobile)
        {
            foreach (BaseBoat targetBoat in m_Instances)
            {
                if (targetBoat == this) continue;

                if (targetBoat.EmbarkedMobiles.Contains(mobile))
                    targetBoat.EmbarkedMobiles.Remove(mobile);
            }

            if (!EmbarkedMobiles.Contains(mobile))
                EmbarkedMobiles.Add(mobile);

            return true;
        }

        public class EmbarkTarget : Target
        {
            private Mobile m_From;
            private bool m_Followers;

            public EmbarkTarget(Mobile from, bool followers): base(20, true, TargetFlags.None)
            {
                if (from == null)
                    return;

                m_From = from;
                m_Followers = followers;

                CheckLOS = true;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_From == null) 
                    return;

                IPoint3D location = target as IPoint3D;

                if (location != null)
                {
                    if (location is Item)
                        location = ((Item)location).GetWorldTop();

                    else if (location is Mobile)
                        location = ((Mobile)location).Location;

                    BaseBoat boatAtLocation = BaseBoat.FindBoatAt(location, from.Map);

                    if (boatAtLocation != null)
                    {
                        if (boatAtLocation.Deleted || boatAtLocation.m_SinkTimer != null)
                            boatAtLocation = null;                        
                    }

                    if (boatAtLocation != null)
                    {
                        if (boatAtLocation.IsFriend(m_From) || boatAtLocation.IsCoOwner(m_From) || boatAtLocation.IsOwner(m_From))
                        {
                            if (m_Followers)
                                boatAtLocation.EmbarkFollowers(m_From);

                            else
                                boatAtLocation.Embark(m_From, false);
                        }

                        else
                            from.SendMessage("You must use a boarding rope to access that ship.");                        
                    }                        

                    else                    
                        from.SendMessage("That is not a targetable ship.");
                }
            }
        }

        public class DisembarkTarget : Target
        {
            private BaseBoat m_Boat;
            private Mobile m_From;
            private Boolean m_Followers;

            public DisembarkTarget(BaseBoat boat, Mobile from, bool followers) : base(20, true, TargetFlags.None)
            {
                CheckLOS = true;

                m_Followers = followers;
                m_Boat = boat;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    Target(from, p);
            }

            public void Target(Mobile from, IPoint3D p)
            {
                IPoint3D orig = p;
                Map map = from.Map;

                SpellHelper.GetSurfaceTop(ref p);
                Point3D pP = new Point3D(p);

                bool multiLocation = false;
                bool boatLocation = false;
                bool guildDockLocation = false;

                if (SpellHelper.CheckMulti(pP, map))
                    multiLocation = true;

                if (BaseBoat.FindBoatAt(pP, map) != null)
                    boatLocation = true;
                
                bool foundOceanStatic = false;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(pP, 0);

                foreach (Item item in nearbyItems)
                {
                    if (item.OceanStatic)
                    {
                        foundOceanStatic = true;
                        break;
                    }
                }

                nearbyItems.Free();

                if (foundOceanStatic)
                {
                    from.SendMessage("That is not a valid location to disembark to.");
                    return;
                }

                if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
                    from.SendLocalizedMessage(501942); //That location is blocked.  

                else if (m_Boat.GetBoatToLocationDistance(m_Boat, pP) > 8)
                    from.SendMessage("That location is too far away.");

                else if (!SpellHelper.CheckTravel(from, TravelCheckType.TeleportFrom))
                    from.SendLocalizedMessage(501942); // That location is blocked.                

                else if (!SpellHelper.CheckTravel(from, map, pP, TravelCheckType.TeleportTo))
                    from.SendLocalizedMessage(501942); // That location is blocked.                

                else if ((multiLocation && !guildDockLocation) || boatLocation)
                    from.SendLocalizedMessage(501942); // That location is blocked.    

                else
                {
                    //Disembark Followers
                    if (m_Followers)
                    {
                        PlayerMobile pm = from as PlayerMobile;

                        if (pm == null)
                            return;

                        bool followersFailed = false;

                        foreach (Mobile follower in pm.AllFollowers)
                        {
                            if (follower == null)
                                continue;

                            if (follower.Map != pm.Map)
                                continue;

                            if (m_Boat.Contains(follower) == false)
                                continue;

                            Point3D pFrom = follower.Location;
                            Point3D pTo = pP;

                            follower.Location = pTo;
                            follower.ProcessDelta();

                            m_Boat.RemoveEmbarkedMobile(follower);

                            IPooledEnumerable eable = follower.GetItemsInRange(0);

                            foreach (Item item in eable)
                            {
                                if (item is Server.Spells.Sixth.ParalyzeFieldSpell.InternalItem || item is Server.Spells.Fifth.PoisonFieldSpell.InternalItem || item is Server.Spells.Fourth.FireFieldSpell.FireFieldItem)
                                    item.OnMoveOver(follower);
                            }

                            eable.Free();
                        }
                    }

                    //Disembark Player
                    else
                    {
                        Point3D pFrom = from.Location;
                        Point3D pTo = pP;

                        //Half-Bow Animation
                        if (from.Body.IsHuman && !from.Mounted)
                            from.Animate(32, 3, 1, true, false, 0);

                        from.Location = pTo;
                        from.ProcessDelta();

                        m_Boat.RemoveEmbarkedMobile(from);

                        IPooledEnumerable eable = from.GetItemsInRange(0);

                        foreach (Item item in eable)
                        {
                            if (item is Server.Spells.Sixth.ParalyzeFieldSpell.InternalItem || item is Server.Spells.Fifth.PoisonFieldSpell.InternalItem || item is Server.Spells.Fourth.FireFieldSpell.FireFieldItem)
                                item.OnMoveOver(from);
                        }

                        eable.Free();
                    }
                }
            }
        }

        public static void TargetedEmbark(Mobile from)
        {
            from.SendMessage("Where do you wish to embark?");
            from.Target = new BaseBoat.EmbarkTarget(from, false);
        }

        public static void TargetedEmbarkFollowers(Mobile from)
        {
            from.SendMessage("Where do you wish to your followers to embark?");
            from.Target = new BaseBoat.EmbarkTarget(from, true);
        }

        public bool Embark(Mobile from, bool boarding)
        {
            if (this == null || Deleted)
                return false;

            if (!from.Alive)
            {
                from.SendMessage("You cannot embark onto a ship as a ghost.");
                return false;
            }

            if (Contains(from) == true)
            {
                from.SendMessage("You are already onboard this ship.");
                return false;
            }

            int distance = GetBoatToLocationDistance(this, from.Location);

            if (distance > 8 && boarding == false)
            {
                from.SendMessage("You are too far away to embark on this ship.");

                return false;
            }

            bool multiLocation = false;
            bool boatLocation = false;
            bool guildDockLocation = false;

            if (SpellHelper.CheckMulti(from.Location, from.Map))
                multiLocation = true;

            List<BaseMulti> m_Multis = BaseMulti.GetMultisAt(from.Location, from.Map);

            foreach (BaseMulti multi in m_Multis)
            {
                if (multi is BaseBoat)
                {
                    boatLocation = true;
                    break;
                }

                /*
                if (multi is BaseGuildDock)
                {
                    guildDockLocation = true;
                    break;
                }
                */
            }

            if (multiLocation && !boatLocation && !guildDockLocation)
            {
                from.SendMessage("You cannot board a ship from that location.");
                return false;
            }

            //Half-Bow Animation
            if (from.Body.IsHuman && !from.Mounted)
                from.Animate(32, 3, 1, true, false, 0);

            Point3D location = GetRandomEmbarkLocation(true);

            from.Location = location;

            TransferEmbarkedMobile(from);

            BaseCreature bc_Creature = from as BaseCreature;

            if (bc_Creature != null)
                bc_Creature.BoatOccupied = this;

            if (IsOwner(from) || IsCoOwner(from) || IsFriend(from))
                Refresh();

            return true;
        }

        public bool EmbarkFollowers(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm != null)
            {
                if (!from.Alive)
                {
                    from.SendMessage("You cannot command your followers to embark while you are a ghost.");
                    return false;
                }

                int distance = GetBoatToLocationDistance(this, from.Location);

                if (distance > 8)
                {
                    from.SendMessage("You are too far away from the ship to command your followers to embark on it.");
                    return false;
                }

                else
                {
                    bool followersLeftBehind = false;

                    int embarkedCreatures = 0;

                    foreach (Mobile follower in pm.AllFollowers)
                    {
                        int followerDistanceToController = (int)(Math.Floor(follower.GetDistanceToSqrt(from)));
                        int followerDistanceToBoat = GetBoatToLocationDistance(this, follower.Location);

                        BaseBoat followerBoat = BaseBoat.FindBoatAt(follower.Location, follower.Map);

                        if (followerBoat != null)
                        {
                            int originBoatDistance = this.GetBoatToBoatDistance(followerBoat, this);

                            if (originBoatDistance < followerDistanceToBoat)
                                followerDistanceToBoat = originBoatDistance;
                        }

                        bool inRange = false;

                        if (Contains(from) && followerDistanceToBoat <= 8)
                            inRange = true;

                        if (followerDistanceToController <= 8 && followerDistanceToBoat <= 8)
                            inRange = true;

                        if (inRange)
                        {
                            if (Contains(follower) == false)
                            {
                                Point3D location = GetRandomEmbarkLocation(true);

                                follower.Location = location;
                                TransferEmbarkedMobile(follower);

                                BaseCreature bc_Creature = follower as BaseCreature;

                                if (bc_Creature != null)
                                    bc_Creature.BoatOccupied = this;

                                embarkedCreatures++;

                                if (this.IsOwner(from) || this.IsCoOwner(from) || this.IsFriend(from))
                                    this.Refresh();
                            }
                        }

                        else
                            followersLeftBehind = true;
                    }

                    if (pm.AllFollowers.Count > 0)
                    {
                        if (embarkedCreatures > 0 && !followersLeftBehind)
                            from.SendMessage("You embark your followers onto the ship.");

                        else if (followersLeftBehind)
                            from.SendMessage("At least one of your followers was too far away to embark onto the ship.");
                    }
                }
            }

            return false;
        }

        public bool Disembark(Mobile from)
        {
            if (Contains(from) == false)
                from.SendMessage("You are not onboard this ship");

            else
            {
                from.SendMessage("Where do you wish to disembark?");
                from.Target = new BaseBoat.DisembarkTarget(this, from, false);
            }

            return true;
        }

        public bool DisembarkFollowers(Mobile from)
        {
            from.SendMessage("Where do you wish to disembark your followers?");
            from.Target = new BaseBoat.DisembarkTarget(this, from, true);

            return true;
        }

        public Point3D GetRandomEmbarkLocation(bool allowHold)
        {
            int firstItem = 0;

            if (!allowHold)
                firstItem = 1;

            Point3D embarkSpot = m_EmbarkLocations()[Utility.RandomMinMax(firstItem, m_EmbarkLocations().Count - 1)];
            Point3D rotatedLocation = GetRotatedLocation(embarkSpot.X, embarkSpot.Y, 0);
            Point3D newLocation = new Point3D(X + rotatedLocation.X, Y + rotatedLocation.Y, Z + 3);

            return newLocation;
        }

        #endregion

        #region Friends / Co-Owners / Ownership / Throw Overboard

        public class BoatCoOwnerTarget : Target
        {
            private BaseBoat m_Boat;
            private bool m_Add;

            public BoatCoOwnerTarget(bool add, BaseBoat boat): base(15, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Boat = boat;
                m_Add = add;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive || m_Boat.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    if (m_Add)
                        m_Boat.AddCoOwner(from, (Mobile)targeted);

                    else
                        m_Boat.RemoveCoOwner(from, (Mobile)targeted);
                }

                else
                    from.SendLocalizedMessage(501362); //That can't be a coowner                
            }
        }

        public class BoatFriendTarget : Target
        {
            private BaseBoat m_Boat;
            private bool m_Add;

            public BoatFriendTarget(bool add, BaseBoat boat): base(15, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Boat = boat;
                m_Add = add;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive || m_Boat.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    if (m_Add)
                        m_Boat.AddFriend(from, (Mobile)targeted);

                    else
                        m_Boat.RemoveFriend(from, (Mobile)targeted);
                }

                else
                    from.SendLocalizedMessage(501371); // That can't be a friend                
            }
        }        
        
        public void AddCoOwner(Mobile from, Mobile targ)
        {
            bool wasFriend = false;

            if (!IsOwner(from) || m_CoOwners == null)
                return;

            //Mobile Belongs to Friends List
            if (m_Friends != null)
            {
                if (m_Friends.Contains(targ))
                    wasFriend = true;
            }

            if (IsOwner(targ))
                from.SendMessage("This person is already the ship's owner.");

            else if (!targ.Player)
                from.SendMessage("That can't be a co-owner of the ship.");

            else if (m_CoOwners.Count >= MaxCoOwners)
                from.SendLocalizedMessage(501368); // Your co-owner list is full!            

            else if (m_CoOwners.Contains(targ))
                from.SendLocalizedMessage(501369); // This person is already on your co-owner list!            

            else
            {
                //Remove Mobile from Friends List: Will Upgrade to Co-owner
                if (wasFriend)
                    m_Friends.Remove(targ);

                //Add to Co-Owners List
                m_CoOwners.Add(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendMessage("You have been made a co-owner of the ship.");
            }
        }

        public void RemoveCoOwner(Mobile from, Mobile targ)
        {
            if (!IsOwner(from) || m_CoOwners == null)
                return;

            if (m_CoOwners.Contains(targ))
            {
                m_CoOwners.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                from.SendLocalizedMessage(501299); // Co-owner removed from list.
                targ.SendMessage("You have been removed as a co-owner of the ship.");
            }
        }

        public void AddFriend(Mobile from, Mobile targ)
        {
            if (!(IsOwner(from) || IsCoOwner(from)) || m_Friends == null)
                return;

            if (IsOwner(targ))
                from.SendMessage("This person is an owner of the ship.");

            else if (m_CoOwners.Contains(targ))
                from.SendMessage("This person is already a co-owner of the ship.");

            else if (!targ.Player)
                from.SendMessage("That can't be a friend of the ship.");

            else if (m_Friends.Count >= MaxFriends)
                from.SendLocalizedMessage(501375); // Your friends list is full!            

            else if (m_Friends.Contains(targ))
                from.SendLocalizedMessage(501376); // This person is already on your friends list!            

            else
            {
                m_Friends.Add(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendMessage("You have been made a friend of the ship.");
            }
        }

        public void RemoveFriend(Mobile from, Mobile targ)
        {
            if (!(IsCoOwner(from) || IsCoOwner(from)) || m_Friends == null)
                return;

            if (m_Friends.Contains(targ))
            {
                m_Friends.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                from.SendLocalizedMessage(501298); // Friend removed from list.
                targ.SendMessage("You are no longer a friend of the ship.");
            }
        }

        public bool IsOwner(Mobile m)
        {
            if (m == null)
                return false;

            if (m.AccessLevel > AccessLevel.Player)
                return true;

            if (m == m_Owner)
                return true;

            return false;
        }

        public bool IsCoOwner(Mobile m)
        {
            if (m == null || m_CoOwners == null)
                return false;

            if (m.AccessLevel > AccessLevel.Player)
                return true;

            if (m_CoOwners.Contains(m))
                return true;

            return false;
        }

        public bool IsFriend(Mobile m)
        {
            if (m == null || Owner == null)
                return false;

            if (m.AccessLevel > AccessLevel.Player)
                return true;

            //Same Guild And Auto-Guild Friendship Turned On
            if (GuildAsFriends)
            {
                if (m.Guild != null && Owner.Guild != null && m.Guild == Owner.Guild)
                    return true;
            }

            if (m_Friends == null)
                return false;

            return (m_Friends.Contains(m));
        }

        public void AddCoOwnerCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsOwner(from))
            {
                from.SendMessage("Target the person you wish to name a co-owner of this boat.");
                from.Target = new BaseBoat.BoatCoOwnerTarget(true, this);
            }
        }

        public void AddFriendCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsCoOwner(from) || IsOwner(from))
            {
                from.SendMessage("Target the person you wish to name a friend of this boat.");
                from.Target = new BaseBoat.BoatFriendTarget(true, this);
            }
        }

        public class ThrowOverboardTarget : Target
        {
            private PlayerMobile m_Player;
            private BaseBoat m_Boat;

            public ThrowOverboardTarget(Mobile from, BaseBoat boat)
                : base(15, false, TargetFlags.None)
            {
                PlayerMobile m_Player = from as PlayerMobile;

                if (m_Player == null || boat == null)
                    return;

                m_Boat = boat;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Player == null) return;
                if (m_Boat.Deleted) return;

                Mobile mobileTarget = targeted as Mobile;

                if (mobileTarget == null)
                {
                    m_Player.SendMessage("You may only target players and creatures with this.");
                    return;
                }

                bool playerIsOwner = m_Boat.IsOwner(m_Player);
                bool playerIsCoOwner = m_Boat.IsCoOwner(m_Player);
                bool playerIsFriend = m_Boat.IsFriend(m_Player);

                bool targetIsOwner = m_Boat.IsOwner(mobileTarget);
                bool targetIsCoOwner = m_Boat.IsCoOwner(mobileTarget);
                bool targetIsFriend = m_Boat.IsFriend(mobileTarget);
                bool targetIsUnfriendly = false;

                if (!targetIsOwner && !targetIsCoOwner && !targetIsFriend)
                    targetIsUnfriendly = true;

                bool controllerIsOwner = false;
                bool controllerIsCoOwner = false;
                bool controllerIsFriend = false;
                bool controllerIsUnfriendly = false;

                if (mobileTarget is BaseCreature)
                {
                    BaseCreature bc_Target = mobileTarget as BaseCreature;

                    if (bc_Target.ControlMaster is PlayerMobile)
                    {
                        controllerIsOwner = m_Boat.IsOwner(bc_Target.ControlMaster);
                        controllerIsCoOwner = m_Boat.IsCoOwner(bc_Target.ControlMaster);
                        controllerIsFriend = m_Boat.IsFriend(bc_Target.ControlMaster);

                        if (!controllerIsOwner && !controllerIsCoOwner && !controllerIsFriend)
                            controllerIsUnfriendly = true;
                    }
                }

                bool allowThrowOverboard = false;

                if (playerIsOwner)
                {
                    if (playerIsCoOwner || playerIsFriend || (targetIsUnfriendly && !mobileTarget.Alive))
                        allowThrowOverboard = true;

                    if (controllerIsCoOwner || controllerIsFriend)
                        allowThrowOverboard = true;
                }

                if (playerIsCoOwner)
                {
                    if (playerIsFriend || (targetIsUnfriendly && !mobileTarget.Alive))
                        allowThrowOverboard = true;

                    if (controllerIsFriend)
                        allowThrowOverboard = true;
                }

                if (playerIsFriend)
                {
                    if (targetIsUnfriendly && !mobileTarget.Alive)
                        allowThrowOverboard = true;
                }

                if (mobileTarget.AccessLevel > m_Player.AccessLevel)
                    allowThrowOverboard = false;

                if (allowThrowOverboard)
                {
                    List<Mobile> m_Mobiles = m_Boat.GetMobilesOnBoat(true, false);
                    List<Mobile> creaturesToKill = new List<Mobile>();

                    int followers = 0;

                    foreach (Mobile mobile in m_Mobiles)
                    {
                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature != null)
                        {
                            if (bc_Creature.Controlled && bc_Creature.ControlMaster == mobileTarget)
                            {
                                followers++;
                                creaturesToKill.Add(bc_Creature);
                            }
                        }
                    }

                    Custom.Pirates.PirateHelper.KillAtSea(mobileTarget);

                    foreach (BaseCreature creature in creaturesToKill)
                    {
                        creature.Kill();

                        if (creature.IsBonded)
                            creature.MoveToWorld(creature.ControlMaster.Location, creature.ControlMaster.Map);
                    }

                    if (followers == 0)
                        m_Player.SendMessage("You throw them overboard.");

                    else
                        m_Player.SendMessage("You throw them and their followers overboard.");
                }

                else
                {
                    m_Player.SendMessage("You do not have access privileges to throw that individual overboard.");
                    return;
                }
            }
        }

        public void ThrowOverboardCommand(Mobile from)
        {
            if (from == null)
                return;

            if (!(IsFriend(from) || IsCoOwner(from) || IsOwner(from)))
                return;

            from.SendMessage("Target the person or creature you wish to throw overboard.");
            from.Target = new BaseBoat.ThrowOverboardTarget(from, this);
        }

        #endregion        
        
        #region Movement / Facing

        public bool SetFacing(Direction facing)
        {
            if (Parent != null || this.Map == null)
                return false;

            if (m_ScuttleInProgress)
                return false;

            if (Map != Map.Internal)
            {
                switch (facing)
                {
                    case Direction.North: if (!CanFit(Location, Map, NorthID)) return false; break;
                    case Direction.East: if (!CanFit(Location, Map, EastID)) return false; break;
                    case Direction.South: if (!CanFit(Location, Map, SouthID)) return false; break;
                    case Direction.West: if (!CanFit(Location, Map, WestID)) return false; break;
                }
            }

            Direction old = m_Facing;

            m_Facing = facing;

            if (m_TillerMan != null)
                m_TillerMan.SetFacing(facing);

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon == null) continue;
                if (shipCannon.Deleted) continue;

                Point3D rotatedCannonLocation = GetRotatedLocation(shipCannon.m_xOffset, shipCannon.m_yOffset, shipCannon.m_zOffset);

                if (!(rotatedCannonLocation is Point3D))
                    continue;

                Point3D point = new Point3D(rotatedCannonLocation.X + X, rotatedCannonLocation.Y + Y, rotatedCannonLocation.Z + Z);

                shipCannon.Location = point;
                shipCannon.BoatFacingChange(m_Facing);
                shipCannon.Z += shipCannon.GetAdjustedCannonZOffset();                
            }

            if (m_Hold != null)
                m_Hold.SetFacing(facing);

            if (m_PPlank != null)
                m_PPlank.SetFacing(facing);

            if (m_SPlank != null)
                m_SPlank.SetFacing(facing);

            List<IEntity> toMove = GetMovingEntities(false);

            toMove.Add(m_PPlank);
            toMove.Add(m_SPlank);

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);

            if (m_TillerMan != null)
                m_TillerMan.Location = new Point3D(X + (xOffset * TillerManDistance) + (facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), m_TillerMan.Z);

            if (m_Hold != null)
                m_Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z);

            if (m_ShipTrashBarrel != null)
            {
                Point3D rotatedShipTrashBarrelLocation = GetRotatedLocation(0, Math.Abs(TillerManDistance), 10);
                Point3D point = new Point3D(rotatedShipTrashBarrelLocation.X + this.X, rotatedShipTrashBarrelLocation.Y + this.Y, rotatedShipTrashBarrelLocation.Z + this.Z);

                if (m_Facing == Direction.West)
                    point.Y++;

                if (m_Facing == Direction.South)
                    point.X++;

                if (m_Facing == Direction.East)
                    point.Z = -2;

                m_ShipTrashBarrel.Location = point;
            }

            foreach (BoatFireItem boatFire in m_BoatFires)
            {
                if (boatFire != null)
                {
                    Point3D rotatedBoatFireLocation = GetRotatedLocation(boatFire.xOffset, boatFire.yOffset, boatFire.zOffset);

                    if (!(rotatedBoatFireLocation is Point3D))
                        continue;

                    Point3D point = new Point3D(rotatedBoatFireLocation.X + this.X, rotatedBoatFireLocation.Y + this.Y, rotatedBoatFireLocation.Z + this.Z);

                    boatFire.Location = point;

                    if (Facing == Direction.West || Facing == Direction.East)
                        boatFire.ItemID = 0x398C;

                    else
                        boatFire.ItemID = boatFire.ItemID = 0x3996;
                }
            }

            int count = (int)(m_Facing - old) & 0x7;
            count /= 2;

            for (int i = 0; i < toMove.Count; ++i)
            {
                IEntity e = toMove[i];

                if (e is Item)
                {
                    Item item = (Item)e;
                    item.Location = Rotate(item.Location, count);
                }

                else if (e is Mobile)
                {
                    Mobile m = (Mobile)e;

                    m.Direction = (m.Direction - old + facing) & Direction.Mask;
                    m.Location = Rotate(m.Location, count);
                }
            }

            switch (facing)
            {
                case Direction.North: ItemID = NorthID; break;
                case Direction.East: ItemID = EastID; break;
                case Direction.South: ItemID = SouthID; break;
                case Direction.West: ItemID = WestID; break;
            }

            return true;
        } 

        public Direction GetMovementFor(int x, int y, out int maxSpeed)
        {
            int dx = x - this.X;
            int dy = y - this.Y;

            int adx = Math.Abs(dx);
            int ady = Math.Abs(dy);

            Direction dir = Utility.GetDirection(this, new Point2D(x, y));
            int iDir = (int)dir;

            if (iDir % 2 == 0) // North, East, South and West
                maxSpeed = Math.Abs(adx - ady);

            else // Right, Down, Left and Up
                maxSpeed = Math.Min(adx, ady);

            return (Direction)((iDir - (int)Facing) & 0x7);
        }

        public void Teleport(int xOffset, int yOffset, int zOffset)
        {
            List<IEntity> toMove = GetMovingEntities(false);

            for (int i = 0; i < toMove.Count; ++i)
            {
                IEntity e = toMove[i];

                //if (e is Custom.Pirates.BaseCannon && !m_Cannons.Contains((Custom.Pirates.BaseCannon)e))
                //    continue;

                if (e is Item)
                {
                    Item item = (Item)e;
                    item.Location = new Point3D(item.X + xOffset, item.Y + yOffset, item.Z + zOffset);
                }

                else if (e is Mobile)
                {
                    Mobile m = (Mobile)e;
                    m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z + zOffset);
                }
            }

            Location = new Point3D(X + xOffset, Y + yOffset, Z + zOffset);
        }

        private bool NearFacingCardinal(Direction d)
        {
            Direction cw = (Direction)((int)Facing + 1);
            Direction ccw = (int)Facing == 0 ? (Direction)7 : (Direction)((int)Facing - 1);

            return (d == Facing || d == cw || d == ccw);
        }

        private int FindTurnDirection(Direction d)
        {
            if (Facing == Direction.North)
            {
                if (d == Direction.East || d == Direction.Down || d == Direction.South)
                    return 2;

                else
                    return -2;
            }

            else if (Facing == Direction.East)
            {
                if (d == Direction.South || d == Direction.Left || d == Direction.Up)
                    return 2;

                else
                    return -2;
            }

            else if (Facing == Direction.South)
            {
                if (d == Direction.West || d == Direction.Up || d == Direction.North)
                    return 2;

                else
                    return -2;
            }

            else
            {
                if (d == Direction.North || d == Direction.Right || d == Direction.East)
                    return 2;

                else
                    return -2;
            }
        }  

        public bool StartMove(Direction dir, bool fast, bool message)
        {
            if (m_ScuttleInProgress)
                return false;
            
            if (HitPoints <= 0)
            {
                if (m_TillerMan != null)
                {
                    m_TillerMan.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "Ar, the ship is destroyed sir.");
                    return false;
                }
            }
            
            if (ReducedSpeedMode)
            {
                if (DateTime.UtcNow < ReducedSpeedModeTime)
                    fast = false;

                else
                    ReducedSpeedMode = false;
            }
            
            bool drift = (dir != Forward && dir != ForwardLeft && dir != ForwardRight);
            TimeSpan interval = (fast ? (drift ? TimeSpan.FromSeconds(FastDriftInterval) : TimeSpan.FromSeconds(FastInterval)) : (drift ? TimeSpan.FromSeconds(SlowDriftInterval) : TimeSpan.FromSeconds(SlowInterval)));
            int speed = (fast ? (drift ? FastDriftSpeed : FastSpeed) : (drift ? SlowDriftSpeed : SlowSpeed));
            int clientSpeed = fast ? 0x4 : 0x3;
            double intervalSeconds = interval.TotalSeconds;

            if (m_TempSpeedModifierExpiration >= DateTime.UtcNow)
                intervalSeconds *= m_TempSpeedModifier;

            if (intervalSeconds <= 0)
            {
                if (m_TillerMan != null)
                    m_TillerMan.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "Yar, we be held in place by something!");

                return false;
            }

            interval = TimeSpan.FromSeconds(intervalSeconds);

            if (StartMove(dir, speed, clientSpeed, interval, false, message))            
                return true;            

            return false;
        }

        public bool StartMove(Direction dir, int speed, int clientSpeed, TimeSpan interval, bool single, bool message)
        {
            if (m_ScuttleInProgress)
                return false;

            if (m_Anchored)
            {
                if (m_TillerMan != null && message)
                    m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            if (m_TempSpeedModifierExpiration > DateTime.UtcNow)
            {
                if (m_TempSpeedModifier <= 0)
                {
                    if (m_TillerMan != null)
                        m_TillerMan.Say("Yar, we be held in place by something!");

                    return false;
                }
            }

            m_Moving = dir;
            m_Speed = speed;
            m_ClientSpeed = clientSpeed;

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_MoveTimer = new MoveTimer(this, interval, single);
            m_MoveTimer.Start();

            return true;
        }

        public bool OneMove(Direction dir)
        {
            if (m_ScuttleInProgress)
                return false;

            bool drift = (dir != Forward);

            TimeSpan interval = drift ? TimeSpan.FromSeconds(FastDriftInterval) : TimeSpan.FromSeconds(FastInterval);
            int speed = drift ? FastDriftSpeed : FastSpeed;

            if (StartMove(dir, speed, 0x1, interval, true, true))            
                return true;            

            return false;
        }

        private class MoveTimer : Timer
        {
            private BaseBoat m_Boat;

            public MoveTimer(BaseBoat boat, TimeSpan interval, bool single) : base(interval, interval, single ? 1 : 0)
            {
                m_Boat = boat;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                bool drift = (m_Boat.Moving != Forward && m_Boat.Moving != ForwardLeft && m_Boat.Moving != ForwardRight);
                bool fast = !m_Boat.ReducedSpeedMode;

                TimeSpan interval = (fast ? (drift ? TimeSpan.FromSeconds(m_Boat.FastDriftInterval) : TimeSpan.FromSeconds(m_Boat.FastInterval)) : (drift ? TimeSpan.FromSeconds(m_Boat.SlowDriftInterval) : TimeSpan.FromSeconds(m_Boat.SlowInterval)));
                double intervalSeconds = interval.TotalSeconds;               
               
                if (m_Boat.m_TempSpeedModifierExpiration > DateTime.UtcNow)
                {
                    if (m_Boat.m_TempSpeedModifier <= 0)
                    {
                        m_Boat.m_TillerMan.Say("Yar, something be holding us in place!");
                        m_Boat.StopMove(false);
                    }

                    else
                        intervalSeconds *= m_Boat.m_TempSpeedModifier;
                }

                if (intervalSeconds <= .05)
                    intervalSeconds = .05;

                interval = TimeSpan.FromSeconds(intervalSeconds);

                bool foundBoatController = false;

                List<Mobile> boatOccupants = m_Boat.GetMobilesOnBoat(true, true);

                foreach (Mobile occupant in boatOccupants)
                {
                    if (m_Boat.IsOwner(occupant) || m_Boat.IsCoOwner(occupant))
                    {
                        if (occupant.Alive)
                            foundBoatController = true;
                    }
                }

                if (foundBoatController == false)
                    m_Boat.StopMove(false);

                else if (!m_Boat.DoMovement(true))
                    m_Boat.StopMove(false);

                else
                {
                    m_Boat.m_TimeLastMoved = DateTime.UtcNow;

                    PlayerMobile pm_BoatOwner = m_Boat.Owner as PlayerMobile;

                    if (!m_Boat.ReducedSpeedMode && pm_BoatOwner != null)
                    {
                        if (pm_BoatOwner.BoatOccupied == m_Boat)
                        {
                            bool smoothSailing = PlayerEnhancementPersistance.IsCustomizationEntryActive(pm_BoatOwner, CustomizationType.SmoothSailing);

                            if (smoothSailing)
                                CustomizationAbilities.SmoothSailing(pm_BoatOwner);
                        }
                    }

                    if (m_Boat.ReducedSpeedMode)
                    {
                        if (DateTime.UtcNow > m_Boat.ReducedSpeedModeTime)
                        {
                            m_Boat.StopMove(false);
                            m_Boat.ReducedSpeedMode = false;
                            m_Boat.StartMove(m_Boat.Moving, true, false);

                            m_Boat.m_TillerMan.Say("Yar, back to full speed!");
                        }
                    }

                    else if (DateTime.UtcNow > m_Boat.ReducedSpeedModeTime + TimeSpan.FromSeconds((double)m_Boat.ReducedSpeedModeCooldown))
                    {
                        double sailsPercent = (double)((float)m_Boat.SailPoints / (float)m_Boat.MaxSailPoints);

                        double slowDownChance = .10 * (1 - sailsPercent);
                        double chance = Utility.RandomDouble();

                        if (chance < slowDownChance)
                        {
                            m_Boat.m_TillerMan.Say("Arr, we've slowed down!");

                            m_Boat.ReducedSpeedModeTime = DateTime.UtcNow + TimeSpan.FromSeconds((double)(Utility.RandomMinMax(m_Boat.ReducedSpeedModeMinDuration, m_Boat.ReducedSpeedModeMaxDuration)));

                            m_Boat.StopMove(false);
                            m_Boat.ReducedSpeedMode = true;
                            m_Boat.StartMove(m_Boat.Moving, false, false);
                        }
                    }
                }
            }
        }

        public bool DoMovement(bool message)
        {
            Direction dir;
            int speed, clientSpeed;
            
            dir = m_Moving;
            speed = m_Speed;
            clientSpeed = m_ClientSpeed;

            return Move(dir, speed, clientSpeed, true);
        }

        public bool Move(Direction dir, int speed, int clientSpeed, bool message)
        {
            Map map = Map;

            if (map == null || Deleted || m_ScuttleInProgress)
                return false;

            if (m_Anchored)
            {
                if (message && m_TillerMan != null)
                    m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            int rx = 0, ry = 0;
            Direction d = (Direction)(((int)m_Facing + (int)dir) & 0x7);
            Movement.Movement.Offset(d, ref rx, ref ry);

            for (int i = 1; i <= speed; ++i)
            {
                if (!CanFit(new Point3D(X + (i * rx), Y + (i * ry), Z), Map, ItemID))
                {
                    if (i == 1)
                    {
                        if (message && m_TillerMan != null)
                            m_TillerMan.Say(501424); // Ar, we've stopped sir.

                        return false;
                    }

                    speed = i - 1;
                    break;
                }
            }

            int xOffset = speed * rx;
            int yOffset = speed * ry;

            int newX = X + xOffset;
            int newY = Y + yOffset;

            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                Rectangle2D rect = wrap[i];

                if (rect.Contains(new Point2D(X, Y)) && !rect.Contains(new Point2D(newX, newY)))
                {
                    if (newX < rect.X)
                        newX = rect.X + rect.Width - 1;

                    else if (newX >= rect.X + rect.Width)
                        newX = rect.X;

                    if (newY < rect.Y)
                        newY = rect.Y + rect.Height - 1;

                    else if (newY >= rect.Y + rect.Height)
                        newY = rect.Y;

                    for (int j = 1; j <= speed; ++j)
                    {
                        if (!CanFit(new Point3D(newX + (j * rx), newY + (j * ry), Z), Map, ItemID))
                        {
                            if (message && m_TillerMan != null)
                                m_TillerMan.Say(501424); // Ar, we've stopped sir.

                            return false;
                        }
                    }

                    xOffset = newX - X;
                    yOffset = newY - Y;
                }
            }

            if (!NewBoatMovement || Math.Abs(xOffset) > 1 || Math.Abs(yOffset) > 1)
                Teleport(xOffset, yOffset, 0);

            else
            {
                List<IEntity> toMove = GetMovingEntities(false);

                SafeAdd(m_TillerMan, toMove);
                SafeAdd(m_Hold, toMove);
                SafeAdd(m_ShipTrashBarrel, toMove);
                SafeAdd(m_PPlank, toMove);
                SafeAdd(m_SPlank, toMove);

                foreach (ShipCannon shipCannon in m_Cannons)
                {
                    if (shipCannon != null)
                        SafeAdd(shipCannon, toMove);
                }
                
                foreach (BoatFireItem boatFire in m_BoatFires)
                {
                    if (boatFire != null)
                        SafeAdd(boatFire, toMove);
                }

                foreach (NetState netstate in Map.GetClientsInRange(Location, GetMaxUpdateRange()))
                {
                    Mobile mobile = netstate.Mobile;

                    if (netstate.HighSeas && mobile.CanSee(this) && mobile.InRange(Location, GetUpdateRange(mobile)))
                        netstate.Send(new MoveBoatHS(mobile, this, d, clientSpeed, toMove, xOffset, yOffset));
                }

                foreach (IEntity entity in toMove)
                {
                    if (entity is Item)
                    {
                        Item item = (Item)entity;

                        item.NoMoveHS = true;

                        if (!(item is TillerMan || item is Hold || item is Plank || item is BoatFireItem || item is ShipCannon || item is ShipTrashBarrel))                        
                            item.Location = new Point3D(item.X + xOffset, item.Y + yOffset, item.Z);                        
                    }

                    else if (entity is Mobile)
                    {
                        Mobile m = (Mobile)entity;

                        m.NoMoveHS = true;
                        m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z);
                    }
                }

                NoMoveHS = true;
                Location = new Point3D(X + xOffset, Y + yOffset, Z);

                foreach (IEntity e in toMove)
                {
                    if (e is Item)
                        ((Item)e).NoMoveHS = false;

                    else if (e is Mobile)
                        ((Mobile)e).NoMoveHS = false;
                }

                NoMoveHS = false;
            }

            return true;
        }

        public bool StopMove(bool message)
        {
            if (m_ScuttleInProgress)
                return false;

            if (m_MoveTimer == null)
            {
                if (message && m_TillerMan != null)
                    m_TillerMan.Say(501443); // Er, the ship is not moving sir.

                return false;
            }
          
            m_Speed = 0;
            m_ClientSpeed = 0;
            m_MoveTimer.Stop();
            m_MoveTimer = null;

            if (message && m_TillerMan != null)
                m_TillerMan.Say(501429); // Aye aye sir.

            return true;
        }

        public bool StartTurn(int offset, bool message)
        {
            if (m_Anchored)
            {
                if (message)
                    m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            else
            {
                if (m_TurnTimer != null)
                    m_TurnTimer.Stop();

                m_TurnTimer = new TurnTimer(this, offset);
                m_TurnTimer.Start();

                if (message && TillerMan != null)
                    TillerMan.Say(501429); // Aye aye sir.

                return true;
            }
        }

        public bool Turn(int offset, bool message)
        {
            if (m_TurnTimer != null)
            {
                m_TurnTimer.Stop();
                m_TurnTimer = null;
            }

            if (m_Anchored)
            {
                if (message)
                    m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            else if (SetFacing((Direction)(((int)m_Facing + offset) & 0x7)))
                return true;

            else
            {
                if (message)
                    m_TillerMan.Say(501423); // Ar, can't turn sir.

                return false;
            }
        }

        private class TurnTimer : Timer
        {
            private BaseBoat m_Boat;
            private int m_Offset;

            public TurnTimer(BaseBoat boat, int offset): base(TimeSpan.FromSeconds(0.5))
            {
                m_Boat = boat;
                m_Offset = offset;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!m_Boat.Deleted)
                    m_Boat.Turn(m_Offset, true);
            }
        }

        #endregion        

        #region Location Change

        public static Rectangle2D[] GetWrapFor(Map m)
        {
            if (m == Map.Ilshenar)
                return m_IlshWrap;

            else if (m == Map.Tokuno)
                return m_TokunoWrap;

            else
                return m_BritWrap;
        }

        public override void OnLocationChange(Point3D old)
        {
            if (m_TillerMan != null)
                m_TillerMan.Location = new Point3D(X + (m_TillerMan.X - old.X), Y + (m_TillerMan.Y - old.Y), Z + (m_TillerMan.Z - old.Z));

            if (m_Hold != null)
                m_Hold.Location = new Point3D(X + (m_Hold.X - old.X), Y + (m_Hold.Y - old.Y), Z + (m_Hold.Z - old.Z));

            if (m_ShipTrashBarrel != null)
                m_ShipTrashBarrel.Location = new Point3D(X + (m_ShipTrashBarrel.X - old.X), Y + (m_ShipTrashBarrel.Y - old.Y), Z + (m_ShipTrashBarrel.Z - old.Z));

            if (m_PPlank != null)
                m_PPlank.Location = new Point3D(X + (m_PPlank.X - old.X), Y + (m_PPlank.Y - old.Y), Z + (m_PPlank.Z - old.Z));

            if (m_SPlank != null)
                m_SPlank.Location = new Point3D(X + (m_SPlank.X - old.X), Y + (m_SPlank.Y - old.Y), Z + (m_SPlank.Z - old.Z));

            foreach (BoatFireItem boatFire in m_BoatFires)
            {
                if (boatFire != null)
                    boatFire.Location = new Point3D(X + (boatFire.X - old.X), Y + (boatFire.Y - old.Y), Z + (boatFire.Z - old.Z));
            }

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon != null)
                    shipCannon.Location = new Point3D(X + (shipCannon.X - old.X), Y + (shipCannon.Y - old.Y), Z + (shipCannon.Z - old.Z));
            }
        }

        public override void OnMapChange()
        {
            if (m_TillerMan != null)
                m_TillerMan.Map = Map;

            if (m_Hold != null)
                m_Hold.Map = Map;

            if (m_ShipTrashBarrel != null)
                m_ShipTrashBarrel.Map = Map;

            if (m_PPlank != null)
                m_PPlank.Map = Map;

            if (m_SPlank != null)
                m_SPlank.Map = Map;
        }

        #endregion

        #region Anchor

        public bool RaiseAnchor(bool message)
        {
            if (m_ScuttleInProgress)
                return false;

            if (!m_Anchored)
            {
                if (message && m_TillerMan != null)
                    m_TillerMan.Say(501447); // Ar, the anchor has not been dropped sir.

                return false;
            }

            m_Anchored = false;

            if (message && m_TillerMan != null)
                m_TillerMan.Say(501446); // Ar, anchor raised sir.

            return true;
        }

        public bool LowerAnchor(bool message)
        {
            if (m_ScuttleInProgress)
                return false;

            if (HitPoints <= 0)
            {
                m_TillerMan.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "Ar, the boat is destroyed sir.");
                return false;
            }

            if (m_Anchored)
            {
                if (message && m_TillerMan != null)
                    m_TillerMan.Say(501445); // Ar, the anchor was already dropped sir.

                return false;
            }

            StopMove(false);

            m_Anchored = true;

            if (message && m_TillerMan != null)
                m_TillerMan.Say(501444); // Ar, anchor dropped sir.

            return true;
        }

        #endregion

        #region Find Boat At

        public static BaseBoat FindBoatAt(IPoint2D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseBoat boat = sector.Multis[i] as BaseBoat;

                if (boat != null && boat.Contains(loc.X, loc.Y))
                    return boat;
            }

            return null;
        }

        #endregion

        #region Is Water Tile

        public static bool IsWaterTile(Point3D point, Map map)
        {
            LandTile landTile = map.Tiles.GetLandTile(point.X, point.Y);
            StaticTile[] tiles = map.Tiles.GetStaticTiles(point.X, point.Y, true);

            bool hasWaterLandTile = false;
            bool hasWaterStaticTile = false;
            bool hasDockStaticTile = false;

            if (((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                hasWaterLandTile = true;

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];

                if (tile.ID >= 0x1796 && tile.ID <= 0x17B2)
                    hasWaterStaticTile = true;

                if (tile.ID >= 1993 && tile.ID <= 2000)
                    hasDockStaticTile = true;
            }

            if (hasDockStaticTile)
                return false;

            if (hasWaterLandTile || hasWaterStaticTile)
                return true;

            return false;
        }

        #endregion

        #region Get Mobiles on Boat

        public List<Mobile> GetMobilesOnBoat(bool considerGhosts, bool considerStaff)
        {
            var list = new List<Mobile>();

            if (this != null && Map != null && Map != Map.Internal)
            {
                MultiComponentList mcl = Components;
                IPooledEnumerable eable = Map.GetMobilesInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

                foreach (Mobile mobile in eable)
                {
                    if (mobile.CanSwim)
                        continue;

                    if (!considerGhosts && !mobile.Alive)
                        continue;

                    if (!considerStaff && mobile.AccessLevel > AccessLevel.Player)
                        continue;

                    list.Add(mobile);
                }

                eable.Free();
            }

            return list;
        }

        #endregion 

        #region Ship-Based Damage Modifiers

        public static bool UseShipBasedDamageModifer(Mobile from, Mobile target)
        {
            if (from == null || target == null)
                return false;

            bool useShipBasedDamagePenalty = false;

            BaseBoat fromBoat = null;
            BaseBoat toBoat = null;

            BaseCreature bc_From = from as BaseCreature;
            PlayerMobile pm_From = from as PlayerMobile;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            if (bc_From != null)
                fromBoat = bc_From.BoatOccupied;

            if (pm_From != null)
                fromBoat = pm_From.BoatOccupied;

            if (bc_Target != null)
            {
                if (bc_Target.IsOceanCreature)
                    return false;

                toBoat = bc_Target.BoatOccupied;
            }

            if (pm_Target != null)
                toBoat = pm_Target.BoatOccupied;

            bool fromBoatValid = false;
            bool toBoatValid = false;

            if (fromBoat != null)
            {
                if (!fromBoat.Deleted)
                {
                    fromBoatValid = true;
                    fromBoat.m_LastCombatTime = DateTime.UtcNow;
                }
            }

            if (toBoat != null)
            {
                if (!toBoat.Deleted)
                {
                    toBoatValid = true;
                    toBoat.m_LastCombatTime = DateTime.UtcNow;
                }
            }

            if (fromBoatValid && !toBoatValid)
                useShipBasedDamagePenalty = true;

            if (!fromBoatValid && toBoatValid)
                useShipBasedDamagePenalty = true;

            if (fromBoatValid && toBoatValid)
            {
                if (fromBoat != toBoat)
                    useShipBasedDamagePenalty = true;
            }

            return useShipBasedDamagePenalty;
        }

        #endregion

        #region Ship Damage

        public void ReceiveDamage(Mobile attacker, BaseBoat attackerBoat, int amount, DamageType damageType)
        {
            if (damageType == null)
                damageType = DamageType.Hull;

            if (amount <= 0)
                return;

            m_LastCombatTime = DateTime.UtcNow;

            int actualDamage = 0;

            if (attackerBoat != null)
            {
                if (attackerBoat.MobileControlType == MobileControlType.Player && MobileControlType != MobileControlType.Player)
                    amount = (int)((double)amount * DamageFromPlayerBoatScalar);
            }

            string damageMessage = "";

            switch (damageType)
            {
                case DamageType.Hull:

                    if (amount < 1)
                        amount = 1;

                    if (amount > HitPoints)
                        actualDamage += HitPoints;
                    else
                        actualDamage += amount;

                    HitPoints -= amount;

                    if (attacker != null && HitPoints > 0)
                        damageMessage = "Hit: Hull (" + HitPoints.ToString() + "/" + MaxHitPoints.ToString() + ")";
                break;

                case DamageType.Sails:
                    if (amount > SailPoints)
                        actualDamage += SailPoints;
                    else
                        actualDamage += amount;

                    SailPoints -= amount;

                    if (attacker != null)
                        damageMessage = "Hit: Sails (" + SailPoints.ToString() + "/" + MaxSailPoints.ToString() + ")";
                break;

                case DamageType.Guns:
                    if (amount > GunPoints)
                        actualDamage += GunPoints;

                    else
                        actualDamage += amount;

                    GunPoints -= amount;

                    if (attacker != null)
                        damageMessage = "Hit: Guns (" + GunPoints.ToString() + "/" + MaxGunPoints.ToString() + ")";

                break;
            }

            if (damageMessage != "" && attackerBoat != null)
            {
                List<Mobile> m_MobilesOnBoat = attackerBoat.GetMobilesOnBoat(true, true);

                foreach (Mobile mobile in m_MobilesOnBoat)
                {
                    if (attackerBoat.IsOwner(mobile) || attackerBoat.IsCoOwner(mobile) || attackerBoat.IsFriend(mobile))
                        mobile.SendMessage(damageMessage);
                }
            }

            int damageAmount = actualDamage;
            DateTime time = DateTime.UtcNow;

            bool foundExistingEntry = false;

            int shipDamageEntries = m_ShipDamageEntries.Count;

            for (int a = 0; a < shipDamageEntries; a++)
            {
                ShipDamageEntry entry = m_ShipDamageEntries[a];

                if (entry.m_Mobile == attacker && entry.m_Boat == attackerBoat)
                {
                    entry.m_TotalAmount += damageAmount;
                    entry.m_lastDamage = time;

                    foundExistingEntry = true;

                    break;
                }
            }

            if (!foundExistingEntry)
                m_ShipDamageEntries.Add(new ShipDamageEntry(attacker, attackerBoat, damageAmount, time));

            if (m_ShipDamageEntryTimer == null)
            {
                m_ShipDamageEntryTimer = new ShipDamageEntryTimer(this);
                m_ShipDamageEntryTimer.Start();
            }

            else
            {
                if (!m_ShipDamageEntryTimer.Running)
                    m_ShipDamageEntryTimer.Start();
            }

            //Check for Sinking
            if (HitPoints <= 0)
            {
                if (!m_Destroyed)
                {
                    if (m_ScuttleTimer != null)
                    {
                        m_ScuttleTimer.Stop();
                        m_ScuttleTimer = null;
                    }

                    if (m_ShipDamageEntryTimer != null)
                    {
                        m_ShipDamageEntryTimer.Stop();
                        m_ShipDamageEntryTimer = null;
                    }

                    foreach (BaseBoat boat in m_Instances)
                    {
                        if (boat != null)
                        {
                            if (boat.BoatCombatant == this)
                                boat.BoatCombatant = null;
                        }
                    }

                    StopMove(false);
                    RaiseAnchor(false);

                    Effects.PlaySound(Location, Map, 0x020);

                    m_SinkTimer = new SinkTimer(this);
                    m_SinkTimer.Start();

                    if (TillerMan != null)
                        TillerMan.Say(RandomShipSinkingTillermanSpeech());

                    Z = -6;

                    ResolveDamagers();

                    m_Destroyed = true;
                }
            }

            m_LastCombatTime = DateTime.UtcNow;
        }

        public void ResolveDamagers()
        {
            int totalDamageToShip = 0;

            Dictionary<BaseBoat, int> DictBoatDamagers = new Dictionary<BaseBoat, int>();

            PlayerMobile boatPlayer = Owner as PlayerMobile;

            foreach (ShipDamageEntry entry in m_ShipDamageEntries)
            {
                PlayerMobile attackerPlayer = entry.m_Mobile as PlayerMobile;
                BaseBoat attackerPlayerBoat = entry.m_Boat;

                bool addAttackingPlayer = true;

                totalDamageToShip += entry.m_TotalAmount;

                if (attackerPlayer == null || attackerPlayerBoat == null)
                    continue;

                if (DictBoatDamagers.ContainsKey(attackerPlayerBoat))
                {
                    int oldValue = 0;

                    DictBoatDamagers.TryGetValue(attackerPlayerBoat, out oldValue);
                    DictBoatDamagers.Remove(attackerPlayerBoat);
                    DictBoatDamagers.Add(attackerPlayerBoat, entry.m_TotalAmount + oldValue);
                }

                else
                    DictBoatDamagers.Add(attackerPlayerBoat, entry.m_TotalAmount);
            }

            BaseBoat highestDamagingBoat = null;
            double highestBoatDamagePercent = 0;

            int shipDoubloonValue = DoubloonValue;
            int shipPlayerHoldDoubloonAmount = GetHoldDoubloonTotal(this);

            foreach (KeyValuePair<BaseBoat, int> keyPair in DictBoatDamagers)
            {
                BaseBoat attackingBoat = keyPair.Key as BaseBoat;

                if (attackingBoat == null) continue;
                if (attackingBoat.Deleted) continue;
                if (attackingBoat.Owner == null) continue;

                PlayerMobile attackingBoatPlayerOwner = attackingBoat.Owner as PlayerMobile;

                if (attackingBoatPlayerOwner == null)
                    continue;

                double damagePercent = (double)keyPair.Value / (double)totalDamageToShip;

                if (damagePercent >= highestBoatDamagePercent)
                {
                    highestDamagingBoat = attackingBoat;
                    highestBoatDamagePercent = damagePercent;
                }

                int deposited = 0;

                if (attackingBoat.TillerMan != null)
                    attackingBoat.TillerMan.Say(RandomSunkOtherShipTillermanSpeech());

                if (shipDoubloonValue > 0 || shipPlayerHoldDoubloonAmount > 0)
                {
                    int shipDoubloonValueAdjusted = (int)((double)shipDoubloonValue * damagePercent);
                    
                    int finalDoubloonAmount = shipDoubloonValueAdjusted + shipPlayerHoldDoubloonAmount;

                    if (finalDoubloonAmount < 1)
                        finalDoubloonAmount = 1;

                    if (attackingBoat.DepositDoubloons(finalDoubloonAmount))
                    {
                        Doubloon doubloonPile = new Doubloon(finalDoubloonAmount);
                        attackingBoatPlayerOwner.SendSound(doubloonPile.GetDropSound());
                        doubloonPile.Delete();

                        attackingBoatPlayerOwner.SendMessage("You've received " + finalDoubloonAmount.ToString() + " doubloons for sinking their ship! The coins have been placed in your ship's hold.");
                    }

                    else
                        attackingBoatPlayerOwner.SendMessage("You've sunk a ship but alas there was no room in your ship's hold to place all the doubloons!");
                }
            }
        }

        private class ShipDamageEntryTimer : Timer
        {
            private BaseBoat m_Boat;

            public ShipDamageEntryTimer(BaseBoat boat)
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                m_Boat = boat;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                int damageEntries = m_Boat.m_ShipDamageEntries.Count;

                List<ShipDamageEntry> m_EntriesToRemove = new List<ShipDamageEntry>();

                for (int a = 0; a < damageEntries; a++)
                {
                    ShipDamageEntry entry = m_Boat.m_ShipDamageEntries[a];

                    if (entry.m_lastDamage + m_Boat.DamageEntryDuration < DateTime.UtcNow)
                        m_EntriesToRemove.Add(entry);
                }

                foreach (ShipDamageEntry entry in m_EntriesToRemove)
                {
                    m_Boat.m_ShipDamageEntries.Remove(entry);
                }

                if (m_Boat.m_ShipDamageEntries.Count == 0)
                    Stop();
            }
        }

        public DamageType GetDamageTypeByTargetingMode(TargetingMode targetingMode)
        {
            if (targetingMode == null)
                targetingMode = TargetingMode.Random;

            if (targetingMode == TargetingMode.Random)
                return GetRandomDamageType();

            List<DamageType> validDamageTypes = new List<DamageType>();

            validDamageTypes.Add(DamageType.Hull);

            if (SailPoints > 0)
                validDamageTypes.Add(DamageType.Sails);

            if (GunPoints > 0)
                validDamageTypes.Add(DamageType.Guns);

            double chance = .60;
            double offChance = .20;

            DamageType desiredDamageType = ConvertTargetingModeToDamageType(targetingMode);

            double randomResult = Utility.RandomDouble();
            double specificChance = chance + ((((double)(3 - (int)validDamageTypes.Count)) * offChance) / (double)validDamageTypes.Count);

            if (randomResult <= specificChance)
            {
                int damageTypeIndex = validDamageTypes.IndexOf(desiredDamageType);

                if (damageTypeIndex == -1)
                {
                    DamageType damageTypeResult = validDamageTypes[Utility.RandomMinMax(0, validDamageTypes.Count - 1)];
                    return damageTypeResult;
                }

                else
                    return desiredDamageType;
            }

            else
            {
                int damageTypeIndex = validDamageTypes.IndexOf(desiredDamageType);

                if (damageTypeIndex > -1)
                    validDamageTypes.RemoveAt(damageTypeIndex);

                if (validDamageTypes.Count > 0)
                {
                    DamageType damageTypeResult = validDamageTypes[Utility.RandomMinMax(0, validDamageTypes.Count - 1)];
                    return damageTypeResult;
                }
            }

            return desiredDamageType;
        }

        public DamageType GetRandomDamageType()
        {
            List<DamageType> validDamageTypes = new List<DamageType>();

            validDamageTypes.Add(DamageType.Hull);

            if (SailPoints > 0)
                validDamageTypes.Add(DamageType.Sails);

            if (GunPoints > 0)
                validDamageTypes.Add(DamageType.Guns);

            return validDamageTypes[Utility.RandomMinMax(0, validDamageTypes.Count - 1)];
        }

        public DamageType ConvertTargetingModeToDamageType(TargetingMode targetingMode)
        {
            if (targetingMode == TargetingMode.Sails)
                return DamageType.Sails;

            if (targetingMode == TargetingMode.Guns)
                return DamageType.Guns;

            return DamageType.Hull;
        } 

        #endregion

        #region Sinking

        private class SinkTimer : Timer
        {
            private BaseBoat m_Boat;
            private DateTime m_SinkTime;

            public SinkTimer(BaseBoat boat): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Boat = boat;
                m_SinkTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 3));
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow > m_SinkTime)
                {
                    if (m_Boat.Z == -8)
                        m_Boat.DestroyBoatMobiles(true, true);

                    if (m_Boat.Z == -13)
                    {
                        m_Boat.DestroyBoatItems(true);

                        if (m_Boat.m_ShipSpawner != null)
                            m_Boat.m_ShipSpawner.ShipSunk(m_Boat);

                        if (m_Boat.Owner != null)
                            m_Boat.Owner.SendMessage("Your boat has been sunk.");

                        m_Boat.Delete();

                        this.Stop();
                    }

                    if (m_Boat.Z > -70)
                    {
                        m_Boat.Z -= 1;
                        Effects.PlaySound(m_Boat.Location, m_Boat.Map, 0x020);
                    }
                }
            }
        }

        public void TakeSinkingDamage(BaseBoat boat, int damage)
        {
            if (boat == null) return;
            if (boat.Deleted) return;

            double damagePercent = 1 - ((double)boat.HitPoints / (double)boat.MaxHitPoints);

            int maxWater = (int)((double)boat.MaxHitPoints / 150);
            int currentWater = (int)(Math.Ceiling(damagePercent * (double)maxWater));

            for (int a = 0; a < currentWater; a++)
            {
                Blood water = new Blood();
                water.Hue = 2222;
                water.Name = "water";
                water.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                water.MoveToWorld(boat.GetRandomEmbarkLocation(false), boat.Map);
            }

            Effects.PlaySound(boat.Location, boat.Map, 0x020);

            string message = "";

            if (damagePercent < .33)
                message = "*water slowly fills the ship*";

            else if (damagePercent < .67)
                message = "*water rapidly pours into the ship*";

            else if (damagePercent < .95)
                message = "*water has almost flooded the ship*";

            else
                message = "*the ship is about to go under!*";

            boat.PublicOverheadMessage(MessageType.Regular, 0, false, message);

            boat.ReceiveDamage(null, null, 5, DamageType.Hull);
        }

        #endregion

        #region Refresh / Decay

        public void Refresh()
        {
            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;
        }

        private class DecayTimer : Timer
        {
            private BaseBoat m_Boat;
            private int m_Count;

            public DecayTimer(BaseBoat boat): base(TimeSpan.Zero, BaseBoat.PlayerShipDecayDamageDelay)
            {
                m_Boat = boat;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Boat == null) return;
                if (m_Boat.Deleted) return;

                List<Mobile> m_BoatMobiles = m_Boat.GetMobilesOnBoat(true, true);

                foreach (Mobile mobile in m_BoatMobiles)
                {
                    if (mobile is PlayerMobile && mobile.Alive)
                    {
                        m_Boat.Refresh();
                        return;
                    }
                }

                if (m_Boat.m_DecayTime < DateTime.UtcNow && m_Boat.MobileControlType == MobileControlType.Player && !m_Boat.IsAdminOnboard())
                    m_Boat.TakeSinkingDamage(m_Boat, 5);
            }
        }

        #endregion

        #region Tillerman Speech

        public string RandomShipSinkingTillermanSpeech()
        {
            List<string> m_Phrases = new List<string>();

            m_Phrases.Add("We're sinking! Gods have mercy on us!");
            m_Phrases.Add("Better a death at sea than life on land!");
            m_Phrases.Add("I only wish I had stolen a little more...");
            m_Phrases.Add("Curses and damnation!");
            m_Phrases.Add("To hell with thee!");
            m_Phrases.Add("Blast ye scruvy dogs!");
            m_Phrases.Add("I had a bad feeling about today...");

            return m_Phrases[Utility.RandomMinMax(0, m_Phrases.Count - 1)];
        }

        public string RandomSunkOtherShipTillermanSpeech()
        {
            List<string> m_Phrases = new List<string>();

            m_Phrases.Add("Yar! We've sent them to a watery grave!");
            m_Phrases.Add("Har! They'll soon be visiting Davy Jones' locker!");
            m_Phrases.Add("Yar! To the bottom of the sea with them!");
            m_Phrases.Add("Avast! They'll soon be feeding the fishes!");
            m_Phrases.Add("Ho! The scallywags were no match for us!");
            m_Phrases.Add("Har! Their ship looks better this way!");

            return m_Phrases[Utility.RandomMinMax(0, m_Phrases.Count - 1)];
        }

        #endregion        

        #region Cannons     
   
        public virtual void GenerateShipCannons()
        {
        }

        public void StartCannonCooldown()
        {
            if (m_CannonCooldownTimer != null)
            {
                m_CannonCooldownTimer.Stop();
                m_CannonCooldownTimer = null;
            }

            m_CannonCooldownTimer = new CannonCooldownTimer(this, m_CannonCooldown);
            m_CannonCooldownTimer.Start();
        }

        private class CannonCooldownTimer : Timer
        {
            private BaseBoat m_Boat;

            public CannonCooldownTimer(BaseBoat boat, DateTime cannonCooldown): base(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))
            {
                m_Boat = boat;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                /*
                if (m_Boat.CannonCooldown < DateTime.UtcNow)
                {
                    if (m_Boat.TillerMan != null)
                    {
                        if (m_Boat.m_SinkTimer == null)
                        {
                            int emptyCannons = 0;

                            for (int a = 0; a < m_Boat.Cannons.Count; a++)
                            {
                                if (m_Boat.Cannons[a].CurrentCharges == 0)
                                    emptyCannons++;
                            }

                            if (emptyCannons == m_Boat.Cannons.Count)
                                m_Boat.TillerMan.Say("We're out of ammunition, sir!");

                            else
                                m_Boat.TillerMan.Say("Cannons ready, sir!");
                        }

                        this.Stop();
                    }
                }
                */
            }
        }

        public bool SetTargetingMode(TargetingMode targetingMode)
        {
            if (m_TillerMan != null)
            {
                switch (targetingMode)
                {
                    case TargetingMode.Random:
                        m_TillerMan.Say("Aye, aye! We'll target anywhere on their ship!");
                        TargetingMode = TargetingMode.Random;
                    break;

                    case TargetingMode.Hull:
                        m_TillerMan.Say("Aye, aye! We'll target their hull!");
                        TargetingMode = TargetingMode.Hull;
                    break;

                    case TargetingMode.Sails:
                        m_TillerMan.Say("Aye, aye! We'll target their sails!");
                        TargetingMode = TargetingMode.Sails;
                    break;

                    case TargetingMode.Guns:
                        m_TillerMan.Say("Aye, aye! We'll target their guns!");
                        TargetingMode = TargetingMode.Guns;
                    break;
                }
            }

            return true;
        }   

        #endregion        

        #region Destroy Items / Mobiles

        public void DestroyBoatItems(bool sink)
        {
            if (Components == null || Map == null || Map == Map.Internal)
                return;

            MultiComponentList mcl = Components;
            ArrayList toMove = new ArrayList();
            IPooledEnumerable eable = Map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o != null && o != this && !(o is ShipSpawner || o is XmlSpawner || o is TillerMan || o is ShipCannon || o is Hold || o is Plank))
                    toMove.Add(o);
            }

            eable.Free();

            foreach (ShipCannon shipCannon in m_Cannons)
                toMove.Add(shipCannon);

            foreach (BoatFireItem boatFire in m_BoatFires)
                boatFire.Delete();
            
            for (int i = 0; i < toMove.Count; ++i)
            {
                object o = toMove[i];

                if (o is Item && !(o is BaseBoat))
                {
                    Item item = (Item)o;

                    if (!m_ItemsToSink.Contains(item) && sink)
                        continue;

                    if (Contains(item) && item.Z >= Z && !item.OceanStatic)
                        item.Delete();
                }
            }
        }

        public void DestroyBoatMobiles(bool kill, bool sink)
        {
            if (Components == null || Map == null || Map == Map.Internal)
                return;

            List<BaseBoat> m_NearbyBoats = new List<BaseBoat>();
            List<Mobile> m_MobilesOnBoat = this.GetMobilesOnBoat(true, true);
            List<BaseCreature> m_CreaturesToKill = new List<BaseCreature>();
            List<BaseCreature> m_TamedCreaturesToKill = new List<BaseCreature>();
            List<PlayerMobile> m_PlayersToKill = new List<PlayerMobile>();

            foreach (BaseBoat targetBoat in BaseBoat.m_Instances)
            {
                if (targetBoat == this) continue;
                if (targetBoat.Deleted) continue;

                if (targetBoat.GetBoatToBoatDistance(targetBoat, this) <= 25)
                    m_NearbyBoats.Add(targetBoat);
            }

            foreach (Mobile mobile in m_MobilesOnBoat)
            {
                if (mobile.AccessLevel > AccessLevel.Player)
                    continue;

                if (!EmbarkedMobiles.Contains(mobile))
                    continue;

                bool isOnOtherBoat = false;

                foreach (BaseBoat nearbyBoat in m_NearbyBoats)
                {
                    if (nearbyBoat.EmbarkedMobiles.Contains(mobile))
                    {
                        isOnOtherBoat = true;
                        break;
                    }
                }

                if (isOnOtherBoat)
                    continue;

                if (!BaseBoat.IsWaterTile(mobile.Location, mobile.Map))
                    continue;

                BaseCreature bc_Creature = mobile as BaseCreature;
                PlayerMobile player = mobile as PlayerMobile;

                if (bc_Creature != null)
                {
                    if (bc_Creature.IsChamp() || bc_Creature.IsBoss() || bc_Creature.IsLoHBoss() || bc_Creature.IsEventBoss())
                        continue;

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                        m_TamedCreaturesToKill.Add(bc_Creature);

                    else
                    {
                        if (sink)
                            bc_Creature.DiedByShipSinking = true;

                        m_CreaturesToKill.Add(bc_Creature);
                    }
                }

                if (player != null)
                    m_PlayersToKill.Add(player);
            }

            foreach (BaseCreature creature in m_CreaturesToKill)
            {
                if (kill)
                {
                    Effects.PlaySound(creature.Location, creature.Map, 0x021);
                    creature.Kill();
                }

                else
                    creature.Delete();
            }

            foreach (PlayerMobile player in m_PlayersToKill)
            {
                Custom.Pirates.PirateHelper.KillAtSea(player);
            }

            foreach (BaseCreature creature in m_TamedCreaturesToKill)
            {
                creature.Kill();

                if (creature.IsBonded && creature.ControlMaster != null)
                    creature.MoveToWorld(creature.ControlMaster.Location, creature.ControlMaster.Map);
            }
        }

        #endregion

        #region Rotation / CanFit

        public bool CanFit(Point3D p, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || Deleted || m_ScuttleInProgress)
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);

            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {
                    int tx = p.X + newComponents.Min.X + x;
                    int ty = p.Y + newComponents.Min.Y + y;

                    if (newComponents.Tiles[x][y].Length == 0 || Contains(tx, ty))
                        continue;

                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(tx, ty, true);

                    bool hasWater = false;

                    if (landTile.Z == p.Z && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                        hasWater = true;

                    int z = p.Z;

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];
                        bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                        if (tile.Z == p.Z && isWater)
                            hasWater = true;
                        else if (tile.Z >= p.Z && !isWater)
                            return false;
                    }

                    if (!hasWater)
                        return false;
                }
            }

            Queue<Item> q = new Queue<Item>();
            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (Item item in eable)
            {
                if (item is BaseMulti || item.ItemID > TileData.MaxItemValue || item.Z < p.Z || !item.Visible || item is BoatFireItem || item is Corpse || item is ShipSpawner || item is FishingSpotSpawner || item is TimedStatic || item is SingleFireField)
                    continue;

                int x = item.X - p.X + newComponents.Min.X;
                int y = item.Y - p.Y + newComponents.Min.Y;

                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0)
                    continue;

                else if (Contains(item))
                    continue;

                else if (item.Movable || item is Blood)
                {
                    q.Enqueue(item);
                    continue;
                }

                eable.Free();

                return false;
            }

            while (q.Count > 0)
                q.Dequeue().Delete();

            eable.Free();

            return true;
        }

        public Point3D Rotate(Point3D p, int count)
        {
            int rx = p.X - Location.X;
            int ry = p.Y - Location.Y;

            for (int i = 0; i < count; ++i)
            {
                int temp = rx;
                rx = -ry;
                ry = temp;
            }

            return new Point3D(Location.X + rx, Location.Y + ry, p.Z);
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            if (m_TillerMan != null && x == m_TillerMan.X && y == m_TillerMan.Y)
                return true;

            if (m_Hold != null && x == m_Hold.X && y == m_Hold.Y)
                return true;

            if (m_ShipTrashBarrel != null && x == m_ShipTrashBarrel.X && y == m_ShipTrashBarrel.Y)
                return true;

            if (m_PPlank != null && x == m_PPlank.X && y == m_PPlank.Y)
                return true;

            if (m_SPlank != null && x == m_SPlank.X && y == m_SPlank.Y)
                return true;

            return false;
        }

        public static bool IsValidLocation(Point3D p, Map map)
        {
            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                if (wrap[i].Contains(p))
                    return true;
            }

            return false;
        }

        public Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(X + MarkOffset.X, Y + MarkOffset.Y, Z + MarkOffset.Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public Point3D GetRotatedLocation(int offsetX, int offsetY, int offsetZ)
        {
            int mobileNewXOffset = offsetX;
            int mobileNewYOffset = offsetY;

            switch (m_Facing)
            {
                case Direction.North:
                break;

                case Direction.West:
                    mobileNewXOffset = offsetY;
                    mobileNewYOffset = offsetX * -1;
                break;

                case Direction.South:
                    mobileNewXOffset *= -1;
                    mobileNewYOffset *= -1;
                break;

                case Direction.East:
                    mobileNewXOffset = offsetY * -1;
                    mobileNewYOffset = offsetX;
                break;
            }

            Point3D newLocation = new Point3D(mobileNewXOffset, mobileNewYOffset, offsetZ);

            return newLocation;
        }

        public Point3D GetRotatedLocation(int x, int y)
        {
            Point3D p = new Point3D(X + x, Y + y, Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public Point3D GetUnRotatedLocation(Point3D p)
        {
            return Rotate(p, 4 - (int)m_Facing / 2);
        }

        #endregion

        #region Entities / Components

        public List<IEntity> GetMovingEntities(bool ignoreShipItems)
        {
            List<IEntity> list = new List<IEntity>();

            Map map = Map;

            if (map == null || map == Map.Internal)
                return list;

            MultiComponentList mcl = Components;

            foreach (object o in map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height)))
            {
                if (o == this || o is TillerMan || o is Hold || o is Plank || o is ShipCannon || o is BoatFireItem || o is Corpse || o is ShipTrashBarrel || o is Blood || o is TimedStatic || o is SingleFireField)
                    continue;

                if (o is Item)
                {
                    Item item = (Item)o;

                    if (Contains(item) && item.Visible && item.Z >= Z)
                    {
                        if (ignoreShipItems)
                        {
                            if (!m_ShipItems.Contains(item))
                                list.Add(item);
                        }

                        else
                            list.Add(item);
                    }
                }

                else if (o is Mobile)
                {
                    Mobile mobile = o as Mobile;

                    if (mobile.CanSwim)
                        continue;

                    if (Contains(mobile))
                        list.Add(mobile);
                }
            }

            return list;
        }

        public static void UpdateAllComponents()
        {
            for (int i = m_Instances.Count - 1; i >= 0; --i)
                m_Instances[i].UpdateComponents();
        }

        public void UpdateComponents()
        {
            if (m_PPlank != null)
            {
                m_PPlank.MoveToWorld(GetRotatedLocation(PortOffset.X, PortOffset.Y), Map);
                m_PPlank.SetFacing(m_Facing);
            }

            if (m_SPlank != null)
            {
                m_SPlank.MoveToWorld(GetRotatedLocation(StarboardOffset.X, StarboardOffset.Y), Map);
                m_SPlank.SetFacing(m_Facing);
            }

            int xOffset = 0, yOffset = 0;

            Movement.Movement.Offset(m_Facing, ref xOffset, ref yOffset);

            if (m_TillerMan != null)
            {
                m_TillerMan.Location = new Point3D(X + (xOffset * TillerManDistance) + (m_Facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), m_TillerMan.Z);
                m_TillerMan.SetFacing(m_Facing);
                m_TillerMan.InvalidateProperties();
            }

            if (m_Hold != null)
            {
                m_Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z);
                m_Hold.SetFacing(m_Facing);
            }

            if (m_ShipTrashBarrel != null)
            {
                Point3D rotatedShipTrashBarrelLocation = GetRotatedLocation(0, Math.Abs(TillerManDistance), 10);
                Point3D point = new Point3D(rotatedShipTrashBarrelLocation.X + this.X, rotatedShipTrashBarrelLocation.Y + this.Y, rotatedShipTrashBarrelLocation.Z + this.Z);

                if (m_Facing == Direction.West)
                    point.Y++;

                if (m_Facing == Direction.South)
                    point.X++;

                if (m_Facing == Direction.East)
                    point.Z = -2;

                m_ShipTrashBarrel.Location = point;
            }

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon == null) continue;
                if (shipCannon.Deleted) continue;

                Point3D rotatedCannonLocation = GetRotatedLocation(shipCannon.m_xOffset, shipCannon.m_yOffset, shipCannon.m_zOffset);
                
                Point3D point = new Point3D(rotatedCannonLocation.X + X, rotatedCannonLocation.Y + Y, rotatedCannonLocation.Z + Z);

                shipCannon.Location = point;
                shipCannon.BoatFacingChange(m_Facing);
                shipCannon.Z += shipCannon.GetAdjustedCannonZOffset();                
            }

            foreach (BoatFireItem boatFire in m_BoatFires)
            {
                if (boatFire != null)
                {
                    Point3D rotatedBoatFireLocation = GetRotatedLocation(boatFire.xOffset, boatFire.yOffset, boatFire.zOffset);

                    if (!(rotatedBoatFireLocation is Point3D))
                        continue;

                    Point3D point = new Point3D(rotatedBoatFireLocation.X + this.X, rotatedBoatFireLocation.Y + this.Y, rotatedBoatFireLocation.Z + this.Z);

                    if (Facing == Direction.West || Facing == Direction.East)
                        boatFire.ItemID = 0x398C;

                    else
                        boatFire.ItemID = boatFire.ItemID = 0x3996;
                }
            }
        }

        private static void SafeAdd(Item item, List<IEntity> toMove)
        {
            if (item != null)
                toMove.Add(item);
        }

        #endregion

        #region Deletion

        public override void Delete()
        {
            foreach (BoatFireItem boatFire in BoatFires)
            {
                boatFire.Delete();
            }

            base.Delete();
        }

        public override void OnDelete()
        {
            if (m_BoatAITimer != null)
            {
                m_BoatAITimer.Stop();
                m_BoatAITimer = null;
            }

            if (m_DecayTimer != null)
            {
                m_DecayTimer.Stop();
                m_DecayTimer = null;
            }

            foreach (BaseBoat boat in m_Instances)
            {
                if (boat != null)
                {
                    if (boat.BoatCombatant == this)
                        boat.BoatCombatant = null;
                }
            }

            DestroyBoatItems(false);
            DestroyBoatMobiles(false, false);

            base.OnDelete();
        }

        public override void OnAfterDelete()
        {
            Queue m_Queue = new Queue();

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon == null) continue;
                if (shipCannon.Deleted) continue;

                m_Queue.Enqueue(shipCannon);
            }

            while (m_Queue.Count > 0)
            {
                ShipCannon shipCannon = (ShipCannon)m_Queue.Dequeue();

                shipCannon.Delete();
            }
            
            if (m_TillerMan != null)
                m_TillerMan.Delete();

            if (m_Hold != null)
                m_Hold.Delete();

            if (m_ShipTrashBarrel != null)
                m_ShipTrashBarrel.Delete();

            if (m_PPlank != null)
                m_PPlank.Delete();

            if (m_SPlank != null)
                m_SPlank.Delete();

            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            if (m_ScuttleTimer != null)
                m_ScuttleTimer.Stop();

            if (m_ConfigureShipTimer != null)
                m_ConfigureShipTimer.Stop();

            if (m_CannonCooldownTimer != null)
                m_CannonCooldownTimer.Stop();
            
            if (m_ShipDamageEntryTimer != null)
                m_ShipDamageEntryTimer.Stop();

            if (m_ShipSpawner != null)
                m_ShipSpawner.ShipSunk(this);

            if (BoatRune != null)
            {
                if (!BoatRune.Deleted)
                    BoatRune.Delete();
            }

            if (BoatBankRune != null)
            {
                if (!BoatBankRune.Deleted)
                    BoatBankRune.Delete();
            }

            m_ParticipatingMobiles.Clear();

            m_Instances.Remove(this);
        }

        #endregion                                                         

        #region Ship Rename

        public void SetName(SpeechEventArgs e)
        {
            if (m_ScuttleInProgress)
                return;

            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != m_Owner)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }

            else if (!e.Mobile.Alive)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            if (e.Speech.Length > 8)
            {
                string newName = e.Speech.Substring(8).Trim();

                if (newName.Length == 0)
                    newName = null;

                Rename(newName);
            }
        }

        public void Rename(string newName)
        {
            if (m_ScuttleInProgress)
                return;

            if (newName != null && newName.Length > 40)
                newName = newName.Substring(0, 40);

            if (m_ShipName == newName)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(502531); // Yes, sir.

                return;
            }

            ShipName = newName;

            if (m_TillerMan != null && m_ShipName != null)
                m_TillerMan.Say(1042885, m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.

            else if (m_TillerMan != null)
                m_TillerMan.Say(502534); // This ship now has no name.
        }

        public void RemoveName(Mobile m)
        {
            if (m_ScuttleInProgress)
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != m_Owner)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }

            else if (!m.Alive)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            if (m_ShipName == null)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (m_TillerMan != null)
                m_TillerMan.Say(502534); // This ship now has no name.
        }

        public void GiveName(Mobile m)
        {
            if (m_TillerMan == null || m_ScuttleInProgress)
                return;

            if (m_ShipName == null)
                m_TillerMan.Say(502526); // Ar, this ship has no name.
            else
                m_TillerMan.Say(1042881, m_ShipName); // This is the ~1_BOAT_NAME~.
        }

        public void BeginRename(Mobile from)
        {
            if (m_ScuttleInProgress)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != m_Owner)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (m_TillerMan != null)
                m_TillerMan.Say(502580); // What dost thou wish to name thy ship?

            from.Prompt = new RenameBoatPrompt(this);
        }

        public void EndRename(Mobile from, string newName)
        {
            if (Deleted || m_ScuttleInProgress)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != m_Owner)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }

            else if (!from.Alive)
            {
                if (m_TillerMan != null)
                    m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            newName = newName.Trim();

            if (newName.Length == 0)
                newName = null;

            Rename(newName);
        }

        #endregion        

        #region Valid Boat Item

        public bool IsValidBoatItem(Mobile from, Item item)
        {
            if (from == null || item == null)
                return true;

            if (item.GetType() == typeof(Doubloon) && from == item.PlayerClassOwner)
                return true;
            
            if (item.GetType() == typeof(ShipTrashBarrel))
                return true;

            if (item.GetType() == typeof(TillerMan))
                return true;

            if (item.GetType() == typeof(BoatFireItem))
                return true;

            return false;
        }

        #endregion

        #region DryDock

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (m_ScuttleInProgress)
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            if (!from.InRange(Location, 20))
                return DryDockResult.TooFar;

            if (!m_Anchored)
                return DryDockResult.NotAnchored;

            if (m_Hold != null)
            {
                int itemsInHold = 0;

                foreach (Item item in m_Hold.Items)
                {
                    if (IsValidBoatItem(Owner, item))
                        continue;

                    itemsInHold++;
                }

                if (itemsInHold > 0)
                    return DryDockResult.Hold;
            }

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            List<IEntity> ents = GetMovingEntities(true);

            if (ents.Count >= 1)
                return (ents[0] is Mobile) ? DryDockResult.Mobiles : DryDockResult.Items;

            return DryDockResult.Valid;
        }

        public void BeginDryDock(Mobile from)
        {
            if (m_ScuttleInProgress)
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.

            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.

            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!

            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.

            else if (result == DryDockResult.Hold)
                from.SendMessage("Make sure your ship's hold is empty of all items except your doubloons and try again.");

            else if (result == DryDockResult.Valid)
            {
                if (m_LastCombatTime + TimeNeededToBeOutOfCombat > DateTime.UtcNow)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_LastCombatTime + TimeNeededToBeOutOfCombat, false, true, true, true, true);

                    from.SendMessage("The ship has been been in combat too recently to dock. You must wait " + timeRemaining + ".");

                    return;
                }

                else if (m_TimeLastMoved + DryDockMinimumLastMovement > DateTime.UtcNow)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_TimeLastMoved + DryDockMinimumLastMovement, false, true, true, true, true);

                    from.SendMessage("The ship has not been stationary long enough to dock. You must wait " + timeRemaining + ".");

                    return;
                }

                else if (!CanMoveHoldDoubloonsToBank(from))
                {
                    from.SendMessage("Your bankbox would not be able to hold all of the doubloons from your ship's hold. You must clear out some items from your bank before you may dock this ship.");
                    return;
                }

                else
                    from.SendGump(new ConfirmDryDockGump(from, this));                
            }
        }

        public void EndDryDock(Mobile from, int doubloonsMoved)
        {
            if (Deleted || m_ScuttleInProgress)
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.

            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.

            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!

            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.

            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!

            else if (result == DryDockResult.TooFar)
                from.SendMessage("You are too far away to do that!");

            if (result != DryDockResult.Valid)
                return;

            BaseBoatDeed boatDeed = BoatDeed;

            if (boatDeed == null)
                return;

            PullGhostsToPlayer(from);

            //Store Current Boat Properties in BoatDeed
            if (boatDeed.PlayerClassRestricted == true)
                boatDeed.PlayerClassOwner = Owner;

            boatDeed.m_CoOwners = m_CoOwners;
            boatDeed.m_Friends = m_Friends;
            boatDeed.GuildAsFriends = GuildAsFriends;

            boatDeed.HitPoints = HitPoints;
            boatDeed.SailPoints = SailPoints;
            boatDeed.GunPoints = GunPoints;

            boatDeed.m_Owner = Owner;
            boatDeed.m_ShipName = m_ShipName;

            boatDeed.m_TargetingMode = m_TargetingMode;
            boatDeed.m_TimeLastRepaired = m_TimeLastRepaired;
            boatDeed.m_NextTimeRepairable = m_NextTimeRepairable;

            from.AddToBackpack(boatDeed);

            if (BoatRune != null)
            {
                if (!BoatRune.Deleted)
                    BoatRune.Delete();
            }

            if (BoatBankRune != null)
            {
                if (!BoatBankRune.Deleted)
                    BoatBankRune.Delete();
            }

            Delete();

            from.CloseAllGumps();

            if (doubloonsMoved > 0)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player != null)
                    player.PirateScore += doubloonsMoved;

                from.SendMessage("You dry dock the ship and " + doubloonsMoved.ToString() + " doubloons have been moved from the hold to your bankbox.");
            }

            else
                from.SendMessage("You dry dock the ship.");
        }

        public void DeleteDoubloonsInHold()
        {
            if (this == null) return;
            if (Deleted) return;
            if (Hold == null) return;
            if (Hold.Deleted) return;

            Item[] doubloonsPilesInHold = Hold.FindItemsByType(typeof(Doubloon));

            Queue m_Queue = new Queue();

            foreach (Item item in doubloonsPilesInHold)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }
        }        

        #endregion    
     
        #region Trash Deck Items

        public void TrashDeckItems()
        {
            if (m_ShipTrashBarrel == null) return;
            if (m_ShipTrashBarrel.Deleted) return;

            Queue m_Queue = new Queue();

            List<IEntity> ents = GetMovingEntities(true);

            foreach (Item item in ents)
            {
                if (item.Movable)
                    m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();

                m_ShipTrashBarrel.DropItem(item);
            }
        } 

        #endregion

        #region Divide the Plunder

        public void BeginDivideThePlunder(Mobile from)
        {
            if (from == null)
                return;

            if (Deleted)
                return;

            if (GetHoldDoubloonTotal(this) < 50)
            {
                from.SendMessage("You must have at least 50 doubloons in the hold to divide the plunder.");
                return;
            }

            if (m_ScuttleInProgress)
            {
                from.SendMessage("You can cannot divide the plunder while the ship is being scuttled.");
                return;
            }

            if (!GetMobilesOnBoat(true, true).Contains(from))
            {
                from.SendMessage("You must be onboard this boat in order to divide the plunder.");
                return;
            }

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
            {
                from.SendMessage("You must be alive in order to divide the plunder.");
                return;
            }

            else if (result == DryDockResult.NotAnchored)
            {
                from.SendMessage("The anchor must be lowered before you can divide the plunder.");
                return;
            }

            if (m_LastCombatTime + TimeNeededToBeOutOfCombat > DateTime.UtcNow)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_LastCombatTime + TimeNeededToBeOutOfCombat, false, true, true, true, true);

                from.SendMessage("The ship has been been in combat too recently to divide the plunder. You must wait " + timeRemaining + ".");

                return;
            }

            else if (m_TimeLastMoved + DryDockMinimumLastMovement > DateTime.UtcNow)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_TimeLastMoved + DryDockMinimumLastMovement, false, true, true, true, true);

                from.SendMessage("The ship has not been stationary long enough to divide the plunder. You must wait " + timeRemaining + ".");

                return;
            }

            List<Point3D> m_PointsToCheckForLand = new List<Point3D>();

            if (Hold != null)
            {
                for (int a = 0; a < 8; a++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        m_PointsToCheckForLand.Add(new Point3D(Hold.Location.X - 4 + a, Hold.Location.Y - 4 + b, Hold.Location.Z));
                    }
                }
            }

            for (int a = 0; a < 16; a++)
            {
                for (int b = 8; b < 16; b++)
                {
                    m_PointsToCheckForLand.Add(new Point3D(Location.X - 8 + a, Location.Y - 8 + b, Location.Z));
                }
            }

            if (TillerMan != null)
            {
                for (int a = 0; a < 8; a++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        m_PointsToCheckForLand.Add(new Point3D(TillerMan.Location.X - 4 + a, TillerMan.Location.Y - 4 + b, TillerMan.Location.Z));
                    }
                }
            }

            bool foundNearbyLand = false;

            foreach (Point3D point in m_PointsToCheckForLand)
            {
                LandTile landTile = Map.Tiles.GetLandTile(point.X, point.Y);
                StaticTile[] tiles = Map.Tiles.GetStaticTiles(point.X, point.Y, true);

                bool hasWaterLandTile = false;
                bool hasWaterStaticTile = false;
                bool hasGuildDock = false;

                if (((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                    hasWaterLandTile = true;

                for (int i = 0; i < tiles.Length; ++i)
                {
                    StaticTile tile = tiles[i];

                    if (tile.ID >= 1993 && tile.ID <= 2000)
                    {
                        hasGuildDock = true;
                        break;
                    }

                    if (tile.ID >= 0x1796 && tile.ID <= 0x17B2)
                        hasWaterStaticTile = true;
                }

                if ((!hasWaterLandTile && !hasWaterStaticTile) || hasGuildDock)
                {
                    foundNearbyLand = true;
                    break;
                }
            }

            if (!foundNearbyLand)
            {
                from.SendMessage("Your ship is not close enough to land to divide the plunder.");
                return;
            }

            //from.CloseAllGumps();
            //from.SendGump(new DivideThePlunderGump(from, this, DivideMode.CaptainOnly));
        }

        #endregion

        #region Doubloons

        public int GetHoldDoubloonTotal(BaseBoat boat)
        {
            if (boat == null)
                return 0;

            if (boat.Hold == null)
                return 0;

            if (boat.Hold.Deleted)
                return 0;

            int balance = 0;

            Item[] currencyPiles;

            currencyPiles = boat.Hold.FindItemsByType(typeof(Doubloon));

            for (int i = 0; i < currencyPiles.Length; ++i)
            {
                if (boat.MobileControlType == MobileControlType.Player)
                {
                    if (boat.Owner != null && currencyPiles[i].PlayerClassOwner != null)
                    {
                        if (currencyPiles[i].PlayerClassOwner != boat.Owner)
                            continue;
                    }
                }

                balance += currencyPiles[i].Amount;
            }

            return balance;
        }

        public bool TransferDoubloons(Mobile from, BaseBoat boat, int amount, out int deposited)
        {
            deposited = 0;

            if (boat == null)
                return false;

            if (boat.Hold == null)
                return false;

            if (boat.Hold.Deleted)
                return false;

            int amountRemaining = amount;

            foreach (Item item in boat.Hold.FindItemsByType(typeof(Doubloon)))
            {
                if (item.Amount < 60000 && ((item.Amount + amount) <= 60000))
                {
                    item.Amount += amount;
                    deposited += amount;

                    amountRemaining = 0;
                }

                else if (item.Amount < 60000)
                {
                    int incrementAmount = 60000 - item.Amount;

                    item.Amount += incrementAmount;
                    deposited += incrementAmount;

                    amountRemaining -= incrementAmount;
                }
            }

            if (amountRemaining > 0)
            {
                Item newCurrency = (Item)Activator.CreateInstance(typeof(Doubloon));

                newCurrency.PlayerClassOwner = boat.Owner;
                newCurrency.Amount = amountRemaining;

                if (boat.Hold.TryDropItem(from, newCurrency, true))
                {
                }

                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool DepositDoubloons(int amount)
        {
            if (Hold == null)
                return false;

            if (Owner == null)
                return false;

            List<Mobile> m_MobilesOnBoard = GetMobilesOnBoat(true, true);

            foreach (Mobile mobile in m_MobilesOnBoard)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;
                if (mobile is BaseCreature) continue;
                if (!mobile.Alive) continue;
                if (!(IsFriend(mobile) || IsCoOwner(mobile) || IsOwner(mobile))) continue;

                if (!ParticipatingMobiles.Contains(mobile))
                    ParticipatingMobiles.Add(mobile);
            }

            if (Owner != null)
            {
                if (!Owner.Deleted && !ParticipatingMobiles.Contains(Owner))
                    ParticipatingMobiles.Add(Owner);
            }

            int doubloonsRemaining = amount;

            Item[] m_Items = Hold.FindItemsByType(typeof(Doubloon));

            List<Doubloon> m_DoubloonPiles = new List<Doubloon>();

            foreach (Item item in m_Items)
            {
                Doubloon doubloon = item as Doubloon;

                if (doubloon.PlayerClassOwner == Owner)
                    m_DoubloonPiles.Add(doubloon);
            }

            foreach (Doubloon doubloon in m_DoubloonPiles)
            {
                if (doubloon.Amount + doubloonsRemaining <= 60000)
                {
                    doubloon.Amount += doubloonsRemaining;
                    return true;
                }

                else
                {
                    int doubloonsToStack = 60000 - doubloon.Amount;

                    doubloon.Amount = 60000;
                    doubloonsRemaining -= doubloonsToStack;
                }
            }

            if (doubloonsRemaining > 0)
            {
                int doubloonStacks = (int)(Math.Floor((double)doubloonsRemaining / 60000)) + 1;
                int doubloonsToDeposit = doubloonsRemaining % 60000;

                bool unableToDropDoubloons = false;

                for (int a = 0; a < doubloonStacks; a++)
                {
                    Doubloon newDoubloons = new Doubloon();
                    newDoubloons.PlayerClassOwner = Owner;

                    if (doubloonStacks <= 1)
                        newDoubloons.Amount = doubloonsRemaining;

                    else
                    {
                        if (a < (doubloonStacks - 1))
                            newDoubloons.Amount = 60000;

                        else
                            newDoubloons.Amount = doubloonsToDeposit;
                    }

                    if (Hold.Items.Count < Hold.MaxItems)
                        Hold.DropItem(newDoubloons);

                    else
                    {
                        newDoubloons.Delete();
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanMoveHoldDoubloonsToBank(Mobile from)
        {
            int holdDoubloons = GetHoldDoubloonTotal(this);

            if (Banker.CanDepositUniqueCurrency(from, typeof(Doubloon), holdDoubloons))
                return true;

            return false;
        }

        public bool MoveHoldDoubloonsToBank(Mobile from, bool OnlyCheckIfPossible)
        {
            BankBox bankBox = from.FindBankNoCreate();

            if (!(bankBox == null || Hold == null))
            {
                Item[] doubloonsInHold = Hold.FindItemsByType(typeof(Doubloon));

                int doubloonCount = GetHoldDoubloonTotal(this);
                int doubloonPiles = doubloonsInHold.Length;

                for (int a = 0; a < doubloonPiles; a++)
                {
                    doubloonsInHold[0].Delete();
                }

                Banker.DepositUniqueCurrency(from, typeof(Doubloon), doubloonCount);
            }

            return true;
        }

        #endregion

        #region Pull Ghosts to PLayer

        public void PullGhostsToPlayer(Mobile from)
        {
            if (Map == null || Map == Map.Internal || from == null || from.Map == null || from.Map == Map.Internal)
                return;

            Queue q = new Queue();
            MultiComponentList mcl = Components;
            IPooledEnumerable eable = Map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o is Mobile && Contains((Mobile)o))
                    q.Enqueue((Mobile)o);
            }

            eable.Free();

            while (q.Count > 0)
                ((Mobile)q.Dequeue()).MoveToWorld(from.Location, from.Map);
        }

        #endregion

        #region Scuttle

        public void BeginScuttle()
        {
            if (m_TillerMan != null)
                m_TillerMan.Say("Aye, scuttling the ship. May the gods have mercy on our souls...");

            StopMove(false);

            m_ScuttleInProgress = true;
            m_ScuttleTimer = new ScuttleTimer(this, ScuttleInterval);
            m_ScuttleTimer.Start();
        }

        private class ScuttleTimer : Timer
        {
            private BaseBoat m_Boat;

            public ScuttleTimer(BaseBoat boat, TimeSpan interval): base(interval, interval)
            {
                m_Boat = boat;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Boat.HitPoints <= 0)
                    Stop();

                else
                    m_Boat.TakeSinkingDamage(m_Boat, 100);
            }
        }

        #endregion               

        #region On Speech

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile player = from as PlayerMobile;

            if (m_TillermanRelease > DateTime.UtcNow)
                return;

            string text = e.Speech.Trim().ToLower();
            
            if (from.Alive)
            {
                if (text.IndexOf("lower anchor") != -1 && !Contains(from))
                {
                    if (IsOwner(from) || IsCoOwner(from))
                        LowerAnchor(true);
                }

                if (text.IndexOf("lower the anchor") != -1)
                {
                    if (IsOwner(from) || IsCoOwner(from))
                        LowerAnchor(true);
                }

                if (text.IndexOf("raise anchor") != -1 && !Contains(from))
                {
                    if (IsOwner(from) || IsCoOwner(from))
                        RaiseAnchor(true);
                }

                if (text.IndexOf("raise the anchor") != -1)
                {
                    if (IsOwner(from) || IsCoOwner(from))
                        RaiseAnchor(true);
                }

                if (text.IndexOf("dock ship") != -1)
                {
                    if (IsOwner(from))
                        BeginDryDock(from);
                }

                if (text.IndexOf("dock the ship") != -1)
                {
                    if (IsOwner(from))
                        BeginDryDock(from);
                }

                if (text.IndexOf("i wish to divide the plunder") != -1)
                {
                    if (IsOwner(from))
                        BeginDivideThePlunder(from);
                }

                if (text.IndexOf("i wish to dock") != -1)
                {
                    if (IsOwner(from))
                        BeginDryDock(from);
                }

                if (text.IndexOf("fire left cannons") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))
                        FireCannons(from, true);
                }

                if (text.IndexOf("fire the left cannons") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))
                        FireCannons(from, true);
                }

                if (text.IndexOf("fire right cannons") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))
                        FireCannons(from, false);
                }

                if (text.IndexOf("fire the right cannons") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))
                        FireCannons(from, false);
                }

                if (text.IndexOf("target random") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        SetTargetingMode(TargetingMode.Random);                    
                }

                if (text.IndexOf("target anywhere") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        SetTargetingMode(TargetingMode.Random);                    
                }

                if (text.IndexOf("target their hull") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        SetTargetingMode(TargetingMode.Hull);                    
                }

                if (text.IndexOf("target their sail") != -1 && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        SetTargetingMode(TargetingMode.Sails);                    
                }

                if (((text.IndexOf("target their cannon") != -1) || (text.IndexOf("target their gun") != -1)) && Contains(from))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        SetTargetingMode(TargetingMode.Guns);                    
                }

                if (text.IndexOf("i wish to throw overboard") != -1)                
                    ThrowOverboardCommand(from);  

                if (text.IndexOf("i wish to throw someone overboard") != -1)
                    ThrowOverboardCommand(from);              

                if (text.IndexOf("i wish to embark") != -1)
                {
                    if (IsFriend(from) || IsCoOwner(from) || IsOwner(from) )                    
                        Embark(from, false);                    
                }

                if (text.IndexOf("i wish for my followers to embark") != -1)
                {
                    if (IsFriend(from) || IsCoOwner(from) || IsOwner(from))                    
                        EmbarkFollowers(from);                    
                }

                if ((text.IndexOf("i wish to add a coowner") != -1) || (text.IndexOf("i wish to add a co-owner") != -1))
                {
                    if (IsOwner(from))                    
                        AddCoOwnerCommand(from);                    
                }

                if ((text.IndexOf("i wish to add a friend") != -1))
                {
                    if (IsCoOwner(from) || IsOwner(from))                    
                        AddFriendCommand(from);                    
                }

                if ((text.IndexOf("i wish to scuttle the ship") != -1) && !m_ScuttleInProgress)
                {
                    if (IsOwner(from))
                    {
                        from.CloseGump(typeof(BoatScuttleGump));
                        from.SendGump(new BoatScuttleGump(from, this));
                    }
                }
            }

            if (text.IndexOf("i wish to disembark") != -1 && Contains(from))
                Disembark(from);

            if (text.IndexOf("i wish for my followers to disembark") != -1)
                DisembarkFollowers(from);
            
            if ((IsOwner(from) || IsCoOwner(from)) && Contains(from) && from.Alive && !m_ScuttleInProgress)
            {
                for (int i = 0; i < e.Keywords.Length; ++i)
                {
                    int keyword = e.Keywords[i];

                    if (keyword >= 0x42 && keyword <= 0x6B)
                    {
                        switch (keyword)
                        {
                            case 0x42: SetName(e); break;
                            case 0x43: RemoveName(e.Mobile); break;
                            case 0x44: GiveName(e.Mobile); break;

                            case 0x45: StartMove(Forward, true, true); break;
                            case 0x46: StartMove(Backward, true, true); break;
                            case 0x47: StartMove(Left, true, true); break;
                            case 0x48: StartMove(Right, true, true); break;
                            case 0x4B: StartMove(ForwardLeft, true, true); break;
                            case 0x4C: StartMove(ForwardRight, true, true); break;
                            case 0x4D: StartMove(BackwardLeft, true, true); break;
                            case 0x4E: StartMove(BackwardRight, true, true); break;
                            case 0x4F: StopMove(false); break;
                            case 0x50: StartMove(Left, true, true); break;
                            case 0x51: StartMove(Right, true, true); break;
                            case 0x52: StartMove(Forward, true, true); break;
                            case 0x53: StartMove(Backward, true, true); break;
                            case 0x54: StartMove(ForwardLeft, true, true); break;
                            case 0x55: StartMove(ForwardRight, true, true); break;
                            case 0x56: StartMove(BackwardRight, true, false); break;
                            case 0x57: StartMove(BackwardLeft, true, false); break;

                            case 0x58: OneMove(Left); break;
                            case 0x59: OneMove(Right); break;
                            case 0x5A: OneMove(Forward); break;
                            case 0x5B: OneMove(Backward); break;
                            case 0x5C: OneMove(ForwardLeft); break;
                            case 0x5D: OneMove(ForwardRight); break;
                            case 0x5E: OneMove(BackwardRight); break;
                            case 0x5F: OneMove(BackwardLeft); break;

                            case 0x49:
                            case 0x65: StartTurn(2, false); break; // turn right
                            case 0x4A:
                            case 0x66: StartTurn(-2, false); break; // turn left                              
                            //case 0x67: StartTurn( -4, true ); break; // turn around, come about

                            case 0x68: StartMove(Forward, true, true); break;
                            case 0x69: StopMove(false); break;
                            case 0x6A: LowerAnchor(true); break;
                            case 0x6B: RaiseAnchor(true); break;
                        }

                        e.Handled = true;

                        break;
                    }
                }
            }
        }

        #endregion                        

        #region Boat-Relative Distances

        public int GetBoatToLocationDistance(BaseBoat boatFrom, Point3D locationTo)
        {
            if (boatFrom == null)
                return -1;

            List<Point3D> m_FromPoints = new List<Point3D>();

            m_FromPoints.Add(boatFrom.Location);
            
            if (boatFrom.Hold != null)
                m_FromPoints.Add(boatFrom.Hold.Location);

            if (boatFrom.TillerMan != null)
                m_FromPoints.Add(boatFrom.TillerMan.Location);

            List<Point3D> m_ToPoints = new List<Point3D>();

            m_ToPoints.Add(locationTo);

            int closestDistance = 1000000;

            foreach (Point3D pointFrom in m_FromPoints)
            {
                foreach (Point3D pointTo in m_ToPoints)
                {
                    int distanceBetween = (int)(Math.Sqrt(Math.Pow(Math.Abs(pointTo.X - pointFrom.X), 2) + Math.Pow(Math.Abs(pointTo.Y - pointFrom.Y), 2)));

                    if (distanceBetween < closestDistance)
                        closestDistance = distanceBetween;
                }
            }

            return closestDistance;
        }

        public int GetBoatToBoatDistance(BaseBoat boatFrom, BaseBoat boatTo)
        {
            if (boatFrom == null || boatTo == null)
                return -1;

            List<Point3D> m_FromPoints = new List<Point3D>();

            m_FromPoints.Add(boatFrom.Location);
            
            if (boatFrom.Hold != null)
                m_FromPoints.Add(boatFrom.Hold.Location);

            if (boatFrom.TillerMan != null)
                m_FromPoints.Add(boatFrom.TillerMan.Location);

            List<Point3D> m_ToPoints = new List<Point3D>();

            m_ToPoints.Add(boatTo.Location);

            if (boatTo.Hold != null)
                m_ToPoints.Add(boatTo.Hold.Location);

            if (boatTo.TillerMan != null)
                m_ToPoints.Add(boatTo.TillerMan.Location);

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon == null) continue;
                if (shipCannon.Deleted) continue;

                m_ToPoints.Add(shipCannon.Location);
            }
            
            int closestDistance = 1000000;

            foreach (Point3D pointFrom in m_FromPoints)
            {
                foreach (Point3D pointTo in m_ToPoints)
                {
                    int distanceBetween = (int)(Math.Sqrt(Math.Pow(Math.Abs(pointTo.X - pointFrom.X), 2) + Math.Pow(Math.Abs(pointTo.Y - pointFrom.Y), 2)));

                    if (distanceBetween < closestDistance)
                        closestDistance = distanceBetween;
                }
            }

            return closestDistance;
        }

        #endregion

        #region Boat Notoriety

        public static int GetBoatNotoriety(Mobile from, BaseBoat targetBoat)
        {
            return Notoriety.Innocent;
        }

        #endregion

        #region Sector Activation

        public override void OnSectorActivate()
        {
            if (MobileControlType != MobileControlType.Player && MobileControlType != MobileControlType.Null)
            {
                m_BoatAITimer = new BoatAITimer(this);
                m_BoatAITimer.Start();
            }

            base.OnSectorActivate();
        }  

        #endregion

        #region Boat AI

        public bool AcquireShipTarget()
        {
            m_LastAcquireTarget = DateTime.UtcNow;

            Dictionary<BaseBoat, int> m_ShipsCanBeFiredAt = new Dictionary<BaseBoat, int>();
            Dictionary<BaseBoat, int> m_ShipsCannotBeFiredAt = new Dictionary<BaseBoat, int>();

            foreach (BaseBoat targetBoat in BaseBoat.m_Instances)
            {
                if (targetBoat.Deleted) continue;
                if (targetBoat == this) continue;

                if (targetBoat.m_SinkTimer != null)
                {
                    if (targetBoat.m_SinkTimer.Running)
                        continue;
                }

                if (Utility.GetDistanceToSqrt(Location, targetBoat.Location) > ((double)PerceptionRange * 1.5))
                    continue;

                int distance = targetBoat.GetBoatToBoatDistance(targetBoat, this);

                if (distance > PerceptionRange)
                    continue;

                bool isAttackable = false;

                switch (MobileControlType)
                {
                    case MobileControlType.Good:
                        if (targetBoat.MobileControlType == MobileControlType.Evil)
                            isAttackable = true;

                        if (!isAttackable && targetBoat.MobileControlType == MobileControlType.Player)
                        {
                            List<Mobile> m_MobilesOnBoat = targetBoat.GetMobilesOnBoat(true, false);

                            foreach (Mobile mobile in m_MobilesOnBoat)
                            {
                                PlayerMobile player = mobile as PlayerMobile;

                                if (player != null)
                                {
                                    if ((targetBoat.IsFriend(player) || targetBoat.IsCoOwner(player) || targetBoat.IsOwner(player)) && player.Criminal || player.Murderer)
                                    {
                                        isAttackable = true;
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case MobileControlType.Evil:
                        if (targetBoat.MobileFactionType != MobileFactionType && targetBoat.MobileControlType != MobileControlType.Neutral)
                            isAttackable = true;
                        
                        break;
                }

                foreach (ShipDamageEntry entry in m_ShipDamageEntries)
                {
                    if (entry == null) continue;
                    if (entry.m_Boat == null) continue;

                    if (!entry.m_Boat.Deleted && entry.m_Boat == targetBoat && entry.m_Boat.MobileControlType == MobileControlType.Player)
                    {
                        isAttackable = true;
                        break;
                    }
                }

                if (isAttackable)
                {
                    int weightValue = 0;
                    int distanceWeight = 0;
                    double hullPercentLost = 1 - ((double)targetBoat.HitPoints / (double)targetBoat.MaxHitPoints);

                    weightValue += (int)(hullPercentLost * 10);
                    weightValue += GetDistanceWeight(distance);

                    if (CanHitTargetShip(targetBoat, true))
                        m_ShipsCanBeFiredAt.Add(targetBoat, weightValue);

                    else
                        m_ShipsCannotBeFiredAt.Add(targetBoat, weightValue);
                }

            }

            if (m_ShipsCanBeFiredAt.Count > 0)
            {
                int TotalValues = 0;

                foreach (KeyValuePair<BaseBoat, int> pair in m_ShipsCanBeFiredAt)
                {
                    TotalValues += pair.Value;
                }

                double ActionCheck = Utility.RandomDouble();
                double CumulativeAmount = 0.0;
                double AdditionalAmount = 0.0;

                bool foundDirection = true;

                foreach (KeyValuePair<BaseBoat, int> pair in m_ShipsCanBeFiredAt)
                {
                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                    if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                    {
                        m_BoatCombatant = pair.Key;
                        return true;

                        break;
                    }

                    CumulativeAmount += AdditionalAmount;
                }
            }

            else
            {
                int TotalValues = 0;

                foreach (KeyValuePair<BaseBoat, int> pair in m_ShipsCannotBeFiredAt)
                {
                    TotalValues += pair.Value;
                }

                double ActionCheck = Utility.RandomDouble();
                double CumulativeAmount = 0.0;
                double AdditionalAmount = 0.0;

                bool foundDirection = true;

                foreach (KeyValuePair<BaseBoat, int> pair in m_ShipsCannotBeFiredAt)
                {
                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                    if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                    {
                        m_BoatCombatant = pair.Key;
                        return true;

                        break;
                    }

                    CumulativeAmount += AdditionalAmount;
                }
            }

            return false;
        }

        public int GetDistanceWeight(int distance)
        {
            int value = 0;

            if (distance >= 0 && distance <= 3)
                return 10;

            if (distance >= 4 && distance <= 6)
                return 5;

            if (distance >= 7 && distance <= 15)
                return 2;

            if (distance >= 16)
                return 1;

            return value;
        }

        public bool HasCrewAlive()
        {
            int crewAlive = 0;

            for (int a = 0; a < m_Crew.Count; a++)
            {
                Mobile crewman = m_Crew[a] as Mobile;

                if (crewman != null)
                {
                    if (crewman.Alive)
                        crewAlive++;
                }
            }

            if (crewAlive > 0)
                return true;

            else
                return false;
        }

        public bool IsAdminOnboard()
        {
            foreach (Mobile mobile in GetMobilesOnBoat(true, true))
            {
                if (mobile.AccessLevel > AccessLevel.Player)
                    return true;
            }

            return false;
        }

        public Mobile GetRandomCrew()
        {
            Mobile targetCrew = null;

            List<Mobile> m_AliveCrew = new List<Mobile>();

            for (int a = 0; a < m_Crew.Count; a++)
            {
                Mobile crewman = m_Crew[a] as Mobile;

                if (crewman != null)
                {
                    if (crewman.Alive)
                        m_AliveCrew.Add(crewman);
                }
            }

            if (m_AliveCrew.Count > 0)
                targetCrew = m_AliveCrew[Utility.RandomMinMax(0, m_AliveCrew.Count - 1)];

            return targetCrew;
        }

        public bool CanHitTargetShip(BaseBoat targetBoat, bool considerRange)
        {
            if (this == null) return false;
            if (targetBoat == null) return false;
            if (targetBoat.Deleted) return false;

            int modifiedRange = (int)(Math.Round((double)CannonMaxRange * CannonRangeScalar));

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (considerRange && targetBoat.GetBoatToLocationDistance(targetBoat, shipCannon.Location) > modifiedRange)
                    continue;

                if (shipCannon.InAngle(targetBoat.Location))
                    return true;

                if (targetBoat.TillerMan != null)
                {
                    if (shipCannon.InAngle(targetBoat.TillerMan.Location))
                        return true;
                }

                if (targetBoat.Hold != null)
                {
                    if (shipCannon.InAngle(targetBoat.Hold.Location))
                        return true;
                }
            }

            return false;
        }

        public virtual void OnThink()
        {
            if (AdminControlled)
                return;

            //Ship is Sinking
            if (m_SinkTimer != null)
            {
                if (m_SinkTimer.Running)
                {
                    if (m_BoatAITimer != null)
                    {
                        m_BoatAITimer.Stop();
                        m_BoatAITimer = null;
                    }

                    BoatCombatant = null;
                    StopMove(false);

                    return;
                }
            }

            //No Crew Left
            if (!HasCrewAlive())
            {
                BoatCombatant = null;
                StopMove(false);

                return;
            }

            if (m_BoatCombatant != null)
            {
                if (m_BoatCombatant.Deleted || m_BoatCombatant.HitPoints <= 0)
                    m_BoatCombatant = null;

                else if (m_BoatCombatant.m_SinkTimer != null)
                {
                    if (m_BoatCombatant.m_SinkTimer.Running)
                        m_BoatCombatant = null;
                }
            }

            //Ship Doesn't Have Cannons
            if (m_Cannons.Count == 0)
                return;

            //Current Combatant is Out of Active Range
            if (m_BoatCombatant != null)
            {
                if (Utility.GetDistanceToSqrt(Location, m_BoatCombatant.Location) > (double)PerceptionRange * 1.5)
                    m_BoatCombatant = null;
            }

            if (m_BoatCombatant == null)
            {
                //Find Any Ship Target
                if ((m_LastAcquireTarget + TimeSpan.FromSeconds(m_AcquireTargetDelayAmount)) <= DateTime.UtcNow)
                    AcquireShipTarget();
            }

            else
            {
                //Check for Better Target
                if ((m_LastAcquireTarget + TimeSpan.FromSeconds(m_AcquireNewTargetDelayAmount)) <= DateTime.UtcNow)
                    AcquireShipTarget();
            }

            if (m_BoatCombatant != null)
            {
                //Stop Ship if Our Desired Target is Within LOS and Within Range
                if (CanHitTargetShip(m_BoatCombatant, true))
                    StopMove(false);

                //Have Crew Attack Enemy Ship Mobiles If They Currently Haven't Engaged Anyone
                if ((m_LastAcquireTarget + TimeSpan.FromSeconds(m_AcquireTargetDelayAmount)) <= DateTime.UtcNow)
                {
                    List<Mobile> m_MobilesOnBoatCombatant = m_BoatCombatant.GetMobilesOnBoat(false, false);

                    if (m_MobilesOnBoatCombatant.Count > 0)
                    {
                        foreach (Mobile crewman in m_Crew)
                        {
                            if (crewman == null) continue;
                            if (!crewman.Alive) continue;

                            bool needCombatant = false;

                            if (crewman.Combatant == null)
                                needCombatant = true;

                            else if (!crewman.Combatant.Alive || crewman.GetDistanceToSqrt(crewman.Combatant) > 10)
                                needCombatant = true;

                            if (needCombatant)
                                crewman.Combatant = m_MobilesOnBoatCombatant[Utility.RandomMinMax(0, m_MobilesOnBoatCombatant.Count - 1)];
                        }
                    }
                }
            }

            //Reload Cannons         
            bool needReload = false;

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (shipCannon.Ammunition == 0)
                {
                    needReload = true;
                    break;
                }
            }

            double totalReloadTime = 0;

            if (needReload)
            {
                foreach (ShipCannon shipCannon in m_Cannons)
                {
                    if (shipCannon.Ammunition < shipCannon.GetMaxAmmunition())
                    {
                        shipCannon.Ammunition = shipCannon.GetMaxAmmunition();
                        totalReloadTime += CannonReloadTime;
                    }
                }

                if (CannonCooldown < DateTime.UtcNow)
                    CannonCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(totalReloadTime);

                else
                    CannonCooldown += TimeSpan.FromSeconds(totalReloadTime);

                Effects.PlaySound(Location, Map, 0x3e4);

                if (TillerMan != null)
                    TillerMan.Say("*cannons reloaded*");
            }

            bool readyToFire = false;
            bool firedAnyCannon = false;

            ShipCannon cannonToFire = null;

            //Fire Cannons
            if (DateTime.UtcNow >= CannonCooldown && m_BoatCombatant != null)
            {
                readyToFire = true;

                bool canFire = false;
                bool hasAmmo = false;
                bool inRange = false;

                bool canHitHold = false;
                bool canHitCenter = false;
                bool canHitTillerman = false;

                ShipCannon bestCannon = null;

                int modifiedRange = (int)(Math.Round((double)CannonMaxRange * CannonRangeScalar));

                foreach (ShipCannon shipCannon in m_Cannons)
                {
                    if (m_BoatCombatant == null) break;
                    if (m_BoatCombatant.Deleted) break;

                    if (GetBoatToLocationDistance(m_BoatCombatant, Location) <= modifiedRange)
                        inRange = true;

                    if (shipCannon.Ammunition > 0)
                        hasAmmo = true;

                    if (shipCannon.InAngle(m_BoatCombatant.Location))
                    {
                        bestCannon = shipCannon;

                        canHitCenter = true;
                        canFire = true;
                    }

                    if (m_BoatCombatant.Hold != null)
                    {
                        if (shipCannon.InAngle(m_BoatCombatant.Hold.Location))
                        {
                            if (bestCannon == null)
                                bestCannon = shipCannon;

                            canHitHold = true;
                            canFire = true;
                        }
                    }

                    if (m_BoatCombatant.TillerMan != null)
                    {
                        if (shipCannon.InAngle(m_BoatCombatant.TillerMan.Location))
                        {
                            if (bestCannon == null)
                                bestCannon = shipCannon;

                            canHitTillerman = true;
                            canFire = true;
                        }
                    }
                }

                if (inRange && canFire && hasAmmo && bestCannon != null)
                {
                    Mobile firingCrewman = GetRandomCrew();

                    firedAnyCannon = true;

                    bestCannon.OnTarget(firingCrewman, m_BoatCombatant.Location, true, canHitCenter, canHitHold, canHitTillerman);

                    if (m_BoatCombatant.BoatCombatant == null)
                        m_BoatCombatant.BoatCombatant = this;
                }

                if (!hasAmmo)
                    readyToFire = false;
            }

            int baseCannonRange = (int)(Math.Round((double)CannonMaxRange * CannonRangeScalar));

            //Ship Was Ready to Fire At Target But Couldn't: Make Maneuvers To Adjust            
            if (readyToFire && !firedAnyCannon && m_BoatCombatant != null)
            {
                int distance = GetBoatToBoatDistance(this, m_BoatCombatant);
                bool turned = false;
                bool needMovement = false;

                //In LOS But Out of Range
                if (CanHitTargetShip(m_BoatCombatant, false))
                {
                    //Move Towards Ship
                }

                //Within Cannon Range but Not in LOS
                else if (distance <= baseCannonRange)
                {
                    needMovement = true;

                    if (Turn(-2, false))
                        turned = true;

                    else if (Turn(2, false))
                        turned = true;
                }

                //Not in LOS Nor Cannon Range
                if (needMovement && !turned)
                {
                    //Move + Steer Towards Ship
                }
            }            
        }

        #endregion

        #region Serialize

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            //Version 0
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

            writer.Write(m_Crew.Count);
            for (int a = 0; a < m_Crew.Count; a++)
            {
                writer.Write(m_Crew[a]);
            }

            writer.Write(m_EmbarkedMobiles.Count);
            for (int a = 0; a < m_EmbarkedMobiles.Count; a++)
            {
                writer.Write(m_EmbarkedMobiles[a]);
            }

            writer.Write(m_ParticipatingMobiles.Count);
            for (int a = 0; a < m_ParticipatingMobiles.Count; a++)
            {
                writer.Write(m_ParticipatingMobiles[a]);
            }

            writer.Write(m_ShipItems.Count);
            foreach (Item item in m_ShipItems)
            {
                writer.Write(item);
            }

            writer.Write(m_ShipDamageEntries.Count);
            foreach (ShipDamageEntry entry in m_ShipDamageEntries)
            {
                writer.Write(entry.m_Mobile);
                writer.Write(entry.m_Boat);
                writer.Write(entry.m_TotalAmount);
                writer.Write((DateTime)entry.m_lastDamage);
            }

            writer.Write((int)MobileControlType);
            writer.Write((int)MobileFactionType);
            writer.Write((int)m_TargetingMode);
            writer.Write((int)m_Facing);

            writer.Write(HitPoints);
            writer.Write(SailPoints);
            writer.Write(GunPoints);
            writer.Write(m_DecayTime);
            writer.Write(m_GuildAsFriends);
            writer.Write(m_CannonCooldown);   
            writer.Write(m_Owner);
            writer.Write(m_PPlank);
            writer.Write(m_SPlank);
            writer.Write(m_TillerMan);
            writer.Write(m_Hold);
            writer.Write(m_ShipTrashBarrel);
            writer.Write(m_Anchored);
            writer.Write(m_ShipName);
            writer.Write(m_ScuttleInProgress);           
            writer.Write(m_LastCombatTime);
            writer.Write(m_TimeLastMoved);
            writer.Write(m_TimeLastRepaired);
            writer.Write(m_NextTimeRepairable);
            writer.Write(m_ReducedSpeedMode);
            writer.Write(m_ReducedSpeedModeTime);
            writer.Write(m_ShipSpawner);
            writer.Write(m_BoatCombatant);
            writer.Write(BoatRune);
            writer.Write(BoatBankRune);
            writer.Write(m_LastActivated);
            writer.Write(AdminControlled);
            writer.Write(MaxHitPoints);
            writer.Write(MaxSailPoints);
            writer.Write(MaxGunPoints);
            writer.Write(PerceptionRange);
            writer.Write(CannonAccuracyModifer);
            writer.Write(CannonRangeScalar);
            writer.Write(CannonDamageScalar);
            writer.Write(DamageFromPlayerBoatScalar);
            writer.Write(FastInterval);
            writer.Write(FastDriftInterval);
            writer.Write(SlowInterval);
            writer.Write(SlowDriftInterval);
            writer.Write(DoubloonValue);
            writer.Write(m_CannonHue);
            writer.Write(CannonReloadTimeScalar);
            writer.Write(m_TempSpeedModifier);
            writer.Write(m_TempSpeedModifierExpiration);           
        }

        #endregion

        #region Deserialize

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                int coOwnerCount = reader.ReadInt();
                for (int a = 0; a < coOwnerCount; a++)
                {
                    CoOwners.Add(reader.ReadMobile());
                }

                int friendCount = reader.ReadInt();
                for (int a = 0; a < friendCount; a++)
                {
                    Friends.Add(reader.ReadMobile());
                }

                int crewCount = reader.ReadInt();
                for (int a = 0; a < crewCount; a++)
                {
                    Crew.Add(reader.ReadMobile());
                }

                int embarkedMobilesCount = reader.ReadInt();
                for (int a = 0; a < embarkedMobilesCount; a++)
                {
                    EmbarkedMobiles.Add(reader.ReadMobile());
                }

                int participatingMobilesCount = reader.ReadInt();
                for (int a = 0; a < participatingMobilesCount; a++)
                {
                    ParticipatingMobiles.Add(reader.ReadMobile());
                }

                int shipItemsCount = reader.ReadInt();
                for (int a = 0; a < shipItemsCount; a++)
                {
                    m_ShipItems.Add(reader.ReadItem());
                }

                int shipDamageEntriesCount = reader.ReadInt();
                for (int a = 0; a < shipDamageEntriesCount; a++)
                {
                    Mobile mobile = reader.ReadMobile();
                    BaseBoat boat = (BaseBoat)reader.ReadItem();
                    int totalAmount = reader.ReadInt();
                    DateTime lastDamage = (DateTime)reader.ReadDateTime();

                    ShipDamageEntry entry = new ShipDamageEntry(mobile, boat, totalAmount, lastDamage);

                    m_ShipDamageEntries.Add(entry);
                }

                MobileControlType = (MobileControlType)reader.ReadInt();
                MobileFactionType = (MobileFactionType)reader.ReadInt();
                TargetingMode = (TargetingMode)reader.ReadInt();
                Facing = (Direction)reader.ReadInt();

                int storedHitPoints = reader.ReadInt();
                int storedSailPoints = reader.ReadInt();
                int storedGunPoints = reader.ReadInt();

                DecayTime = reader.ReadDateTime();
                GuildAsFriends = reader.ReadBool();
                m_CannonCooldown = reader.ReadDateTime();               

                Owner = reader.ReadMobile();
                PPlank = (Plank)reader.ReadItem();
                SPlank = (Plank)reader.ReadItem();
                TillerMan = (TillerMan)reader.ReadItem();
                Hold = reader.ReadItem() as Hold;
                ShipTrashBarrel = (ShipTrashBarrel)reader.ReadItem();
                Anchored = reader.ReadBool();
                ShipName = reader.ReadString();
                m_ScuttleInProgress = reader.ReadBool();
                LastCombatTime = reader.ReadDateTime();
                TimeLastMoved = reader.ReadDateTime();
                TimeLastRepaired = reader.ReadDateTime();
                NextTimeRepairable = reader.ReadDateTime();
                ReducedSpeedMode = reader.ReadBool();
                ReducedSpeedModeTime = reader.ReadDateTime();
                m_ShipSpawner = (ShipSpawner)reader.ReadItem();
                BoatCombatant = (BaseBoat)reader.ReadItem();
                BoatRune = (BoatRune)reader.ReadItem();
                BoatBankRune = (BoatRune)reader.ReadItem();
                m_LastActivated = reader.ReadDateTime();
                AdminControlled = reader.ReadBool();
                MaxHitPoints = reader.ReadInt();
                MaxSailPoints = reader.ReadInt();
                MaxGunPoints = reader.ReadInt();
                PerceptionRange = reader.ReadInt();
                CannonAccuracyModifer = reader.ReadDouble();
                CannonRangeScalar = reader.ReadDouble();
                CannonDamageScalar = reader.ReadDouble();
                DamageFromPlayerBoatScalar = reader.ReadDouble();
                FastInterval = reader.ReadDouble();
                FastDriftInterval = reader.ReadDouble();
                SlowInterval = reader.ReadDouble();
                SlowDriftInterval = reader.ReadDouble();
                DoubloonValue = reader.ReadInt();
                CannonHue = reader.ReadInt();
                CannonReloadTimeScalar = reader.ReadDouble();
                TempSpeedModifier = reader.ReadDouble();
                TempSpeedModifierExpiration = reader.ReadDateTime();               
                
                //-----

                Movable = false;

                if (m_ScuttleInProgress)
                {
                    if (m_ScuttleTimer == null)
                    {
                        m_ScuttleTimer = new ScuttleTimer(this, ScuttleInterval);
                        m_ScuttleTimer.Start();
                    }

                    else
                        m_ScuttleTimer.Start();
                }

                if (m_ShipDamageEntries.Count > 0)
                {
                    if (m_ShipDamageEntryTimer == null)
                    {
                        m_ShipDamageEntryTimer = new ShipDamageEntryTimer(this);
                        m_ShipDamageEntryTimer.Start();
                    }

                    else
                    {
                        if (!m_ShipDamageEntryTimer.Running)
                            m_ShipDamageEntryTimer.Start();
                    }
                }
                                
                HitPoints = storedHitPoints;
                SailPoints = storedSailPoints;
                GunPoints = storedGunPoints;

                m_Instances.Add(this);

                if (MobileControlType == MobileControlType.Player)
                {
                    m_DecayTimer = new DecayTimer(this);
                    m_DecayTimer.Start();
                }

                Timer.DelayCall(TimeSpan.FromMilliseconds(100), delegate
                {
                    if (this == null) return;
                    if (Deleted) return;

                    List<Mobile> m_MobilesOnBoat = this.GetMobilesOnBoat(true, true);

                    foreach (Mobile mobile in m_MobilesOnBoat)
                    {
                        if (!EmbarkedMobiles.Contains(mobile))
                            EmbarkedMobiles.Add(mobile);

                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature != null)
                        {
                            if (!(bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile))
                                bc_Creature.BoatOccupied = this;
                        }
                    }
                });
            }  
        }

        #endregion        

        #region Packets

        public sealed class MoveBoatHS : Packet
        {
            public MoveBoatHS(Mobile beholder, BaseBoat boat, Direction d, int speed, List<IEntity> ents, int xOffset, int yOffset): base(0xF6)
            {
                EnsureCapacity(3 + 15 + ents.Count * 10);

                m_Stream.Write((int)boat.Serial);
                m_Stream.Write((byte)speed);
                m_Stream.Write((byte)d);
                m_Stream.Write((byte)boat.Facing);
                m_Stream.Write((short)(boat.X + xOffset));
                m_Stream.Write((short)(boat.Y + yOffset));
                m_Stream.Write((short)boat.Z);
                m_Stream.Write((short)0); // count placeholder

                int count = 0;

                foreach (IEntity ent in ents)
                {
                    if (!beholder.CanSee(ent))
                        continue;

                    m_Stream.Write((int)ent.Serial);
                    m_Stream.Write((short)(ent.X + xOffset));
                    m_Stream.Write((short)(ent.Y + yOffset));
                    m_Stream.Write((short)ent.Z);
                    ++count;
                }

                m_Stream.Seek(16, System.IO.SeekOrigin.Begin);
                m_Stream.Write((short)count);
            }
        }

        public sealed class DisplayBoatHS : Packet
        {
            public DisplayBoatHS(Mobile beholder, BaseBoat boat): base(0xF7)
            {
                List<IEntity> ents = boat.GetMovingEntities(false);

                SafeAdd(boat.TillerMan, ents);
                SafeAdd(boat.Hold, ents);
                SafeAdd(boat.PPlank, ents);
                SafeAdd(boat.SPlank, ents);

                ents.Add(boat);

                EnsureCapacity(3 + 2 + ents.Count * 26);

                m_Stream.Write((short)0); // count placeholder

                int count = 0;

                foreach (IEntity ent in ents)
                {
                    if (!beholder.CanSee(ent))
                        continue;

                    // Embedded WorldItemHS packets
                    m_Stream.Write((byte)0xF3);
                    m_Stream.Write((short)0x1);

                    if (ent is BaseMulti)
                    {
                        BaseMulti bm = (BaseMulti)ent;

                        m_Stream.Write((byte)0x02);
                        m_Stream.Write((int)bm.Serial);
                        m_Stream.Write((ushort)(bm.ItemID & 0x3FFF));
                        m_Stream.Write((byte)0);

                        m_Stream.Write((short)bm.Amount);
                        m_Stream.Write((short)bm.Amount);

                        m_Stream.Write((short)(bm.X & 0x7FFF));
                        m_Stream.Write((short)(bm.Y & 0x3FFF));
                        m_Stream.Write((sbyte)bm.Z);

                        m_Stream.Write((byte)bm.Light);
                        m_Stream.Write((short)bm.Hue);
                        m_Stream.Write((byte)bm.GetPacketFlags());
                    }

                    else if (ent is Mobile)
                    {
                        Mobile m = (Mobile)ent;

                        m_Stream.Write((byte)0x01);
                        m_Stream.Write((int)m.Serial);
                        m_Stream.Write((short)m.Body);
                        m_Stream.Write((byte)0);

                        m_Stream.Write((short)1);
                        m_Stream.Write((short)1);

                        m_Stream.Write((short)(m.X & 0x7FFF));
                        m_Stream.Write((short)(m.Y & 0x3FFF));
                        m_Stream.Write((sbyte)m.Z);

                        m_Stream.Write((byte)m.Direction);
                        m_Stream.Write((short)m.Hue);
                        m_Stream.Write((byte)m.GetPacketFlags());
                    }

                    else if (ent is Item)
                    {
                        Item item = (Item)ent;

                        m_Stream.Write((byte)0x00);
                        m_Stream.Write((int)item.Serial);
                        m_Stream.Write((ushort)(item.ItemID & 0xFFFF));
                        m_Stream.Write((byte)0);

                        m_Stream.Write((short)item.Amount);
                        m_Stream.Write((short)item.Amount);

                        m_Stream.Write((short)(item.X & 0x7FFF));
                        m_Stream.Write((short)(item.Y & 0x3FFF));
                        m_Stream.Write((sbyte)item.Z);

                        m_Stream.Write((byte)item.Light);
                        m_Stream.Write((short)item.Hue);
                        m_Stream.Write((byte)item.GetPacketFlags());
                    }

                    m_Stream.Write((short)0x00);
                    ++count;
                }

                m_Stream.Seek(3, System.IO.SeekOrigin.Begin);
                m_Stream.Write((short)count);
            }
        }

        #endregion
    }

    #region Boat AI Timer

    public class BoatAITimer : Timer
    {
        private BaseBoat m_Boat;

        public BoatAITimer(BaseBoat boat)
            : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.5))
        {
            m_Boat = boat;
            Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            if (m_Boat.Deleted)
            {
                Stop();
                return;
            }

            Sector sect = m_Boat.Map.GetSector(m_Boat.Location);

            if (!sect.Active && m_Boat.HasCrewAlive() && m_Boat.MobileControlType != MobileControlType.Player && !m_Boat.IsAdminOnboard() && !m_Boat.AdminControlled)
            {
                if (m_Boat.LastActivated + m_Boat.DeactivateDelay < DateTime.UtcNow)
                {
                    if (m_Boat.m_BoatAITimer != null)
                    {
                        m_Boat.m_BoatAITimer.Stop();
                        m_Boat.m_BoatAITimer = null;
                    }

                    return;
                }
            }

            else
                m_Boat.LastActivated = DateTime.UtcNow;

            if (m_Boat.MobileControlType != MobileControlType.Neutral && !m_Boat.IsAdminOnboard() && !m_Boat.HasCrewAlive() && m_Boat.MobileControlType != MobileControlType.Player && m_Boat.NextSinkDamageAllowed <= DateTime.UtcNow)
            {
                m_Boat.TakeSinkingDamage(m_Boat, 5);
                m_Boat.NextSinkDamageAllowed = DateTime.UtcNow + BaseBoat.NPCShipUncrewedDamageDelay;
            }

            else
                m_Boat.OnThink();
        }
    }

    #endregion

    #region Ship Damage Entry

    public class ShipDamageEntry
    {
        public Mobile m_Mobile;
        public BaseBoat m_Boat;
        public int m_TotalAmount;
        public DateTime m_lastDamage;

        public ShipDamageEntry(Mobile mobile, BaseBoat boat, int totalAmount, DateTime lastDamage)
        {
            m_Mobile = mobile;
            m_Boat = boat;
            m_TotalAmount = totalAmount;
            m_lastDamage = lastDamage;
        }
    }

    #endregion

    #region Boat Fire

    public class BoatFireItem : Item
    {
        private Timer m_Timer;
        private DateTime m_End;

        public int xOffset;
        public int yOffset;
        public int zOffset;

        public BoatFireItem(int itemID, Point3D loc, Map map, TimeSpan duration, int xOff, int yOff, int zOff): base(itemID)
        {
            xOffset = xOff;
            yOffset = yOff;
            zOffset = zOff;

            Visible = true;
            Movable = false;
            Light = LightType.Circle300;
            MoveToWorld(loc, map);

            m_End = DateTime.UtcNow + duration;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public BoatFireItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }

        private class InternalTimer : Timer
        {
            private BoatFireItem m_Item;

            public InternalTimer(BoatFireItem item): base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                m_Item = item;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Item.Deleted)
                    return;

                else if (DateTime.UtcNow > m_Item.m_End)
                {
                    m_Item.Delete();
                    Stop();
                }
            }
        }
    }

    #endregion    
}