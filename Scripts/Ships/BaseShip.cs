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
using Server.Accounting;

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

    public enum ShipAccessLevelType
    {
        None,
        Friend,
        CoOwner,
        Owner
    }

    public abstract class BaseShip : BaseMulti, ILogoutRetain
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
            CommandSystem.Register("Ship", AccessLevel.Player, new CommandEventHandler(ShipSelectionCommand));
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

        #region Ship Construction

        public BaseShip(): base(0x0)
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

        public BaseShip(Serial serial): base(serial)
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
                m_ShipAITimer = new ShipAITimer(this);
                m_ShipAITimer.Start();
            }

            Refresh();
        }

        #endregion

        #region Ship Update Components Timer

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

            ShipHotbarGumpObject shipHotbarGumpObject = new ShipHotbarGumpObject();

            player.CloseGump(typeof(ShipHotbarGump));
            player.SendGump(new ShipHotbarGump(player, shipHotbarGumpObject));
        }

        [Usage("[Ship or [Ship or [Tillerman")]
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.RaiseAnchor(true);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.LowerAnchor(true);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StopMove(false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(Forward, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(ForwardLeft, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(ForwardRight, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(Left, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(Right, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(Backward, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(BackwardLeft, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartMove(BackwardRight, true, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartTurn(-2, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.StartTurn(2, false);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(Forward);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(ForwardLeft);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(ForwardRight);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(Left);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(Right);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(Backward);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(BackwardLeft);
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
            if (player.ShipOccupied == null) return;

            if (player.ShipOccupied.IsOwner(player) || player.ShipOccupied.IsCoOwner(player))
                player.ShipOccupied.OneMove(BackwardRight);
        }

        [Usage("[ShipLocationOffsets")]
        [Description("Gets the players location offsets relative to the ship location")]
        public static void ShipLocationOffsets(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player != null)
            {
                BaseShip playerShip = BaseShip.FindShipAt(player.Location, player.Map);

                if (playerShip != null)
                {
                    int x = player.Location.X - playerShip.Location.X;
                    int y = player.Location.Y - playerShip.Location.Y;
                    int z = player.Location.Z - playerShip.Location.Z;

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

            int shipCount = m_Instances.Count;
            int deleteCount = 0;
            {
                for (int a = 0; a < shipCount; a++)
                {
                    int index = a - deleteCount;

                    BaseShip ship = m_Instances[index];

                    if (ship.MobileControlType != MobileControlType.Player)
                    {
                        m_Instances.RemoveAt(index);
                        ship.Delete();
                        deleteCount++;
                    }
                }
            }

            player.SendMessage("All NPC Ships deleted.");
        }
       
        public static void FireCannons(Mobile from, bool leftSide)
        {
             /*
            BaseShip ship = BaseShip.FindShipAt(from.Location, from.Map);
            Server.Custom.Pirates.BaseCannon cannonToUse = null;

            if (ship == null)
            {
                from.SendMessage("You are not currently on a ship.");
                return;
            }

            if (ship.m_Cannons.Count == 0)
            {
                from.SendMessage("That ship does not have any cannons.");
                return;
            }

            if (!(ship.IsOwner(from) || ship.IsCoOwner(from)))
            {
                from.SendMessage("You do not have permission to fire cannons on that ship.");
                return;
            }

            foreach (Server.Custom.Pirates.BaseCannon cannon in ship.Cannons)
            {
                //Ship North
                if (ship.Facing == Direction.North)
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
                if (ship.Facing == Direction.East)
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
                if (ship.Facing == Direction.South)
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
                if (ship.Facing == Direction.West)
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

            player.SendMessage("Which ship would you like to view?");
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
                PlayerMobile player = m_Mobile as PlayerMobile;

                if (player == null)
                    return;

                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                        p = ((Item)p).GetWorldTop();

                    else if (p is Mobile)
                        p = ((Mobile)p).Location;

                    BaseShip shipAtLocation = BaseShip.FindShipAt(p, from.Map);

                    if (shipAtLocation != null)
                    {
                        ShipGumpObject shipGumpObject = new ShipGumpObject(player, shipAtLocation, null);

                        player.SendSound(0x055);

                        player.CloseGump(typeof(ShipGump));
                        player.SendGump(new ShipGump(player, shipGumpObject));
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

        public const int MaxFriends = 500;
        public const int MaxCoOwners = 500;

        public static double shipBasedDamageToPlayerScalar = 0.5;
        public static double shipBasedDamageToCreatureScalar = 0.5;

        public static double shipBasedAoESpellDamageToPlayerScalar = 0.5;
        public static double shipBasedAoESpellDamageToCreatureScalar = 0.5;

        public TimeSpan TimeNeededToBeOutOfCombat = TimeSpan.FromSeconds(60); //Time Needed to Be Out of Combat For Fast Ship Repair and Dry Docking
        public TimeSpan DryDockMinimumLastMovement = TimeSpan.FromSeconds(10); //Minimum Time Needed Since Last Movement for Ship Docking

        public TimeSpan DamageEntryDuration = TimeSpan.FromMinutes(60);
        public TimeSpan ShipDecayDelay = TimeSpan.FromHours(72);
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
        public double BaseDamageFromPlayerShipScalar = 1.0;

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
        public abstract List<Point3D> m_ShipFireLocations();

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

        private bool m_IPAsCoOwners = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IPAsCoOwners
        {
            get { return m_IPAsCoOwners; }
            set { m_IPAsCoOwners = value; }
        }

        private bool m_GuildAsCoOwners = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuildAsCoOwners
        {
            get { return m_GuildAsCoOwners; }
            set { m_GuildAsCoOwners = value; }
        } 

        private bool m_IPAsFriends = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IPAsFriends
        {
            get { return m_IPAsFriends; }
            set { m_IPAsFriends = value; }
        }

        private bool m_GuildAsFriends = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuildAsFriends
        {
            get { return m_GuildAsFriends; }
            set { m_GuildAsFriends = value; }
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
            set { m_TempSpeedModifier = value; }
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

        private BaseShip m_ShipCombatant = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip ShipCombatant
        {
            get { return m_ShipCombatant; }
            set
            {
                if (value != this)
                    m_ShipCombatant = value;
            }
        }        
       
        private int m_ClientSpeed;  

        public Timer m_TurnTimer;
        public Timer m_ShipDamageEntryTimer;
        public Timer m_CannonCooldownTimer;
        public Timer m_DecayTimer;
        public Timer m_SinkTimer;
        public Timer m_ScuttleTimer;
        public Timer m_ConfigureShipTimer;
        public Timer m_ShipAITimer;

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

                //Determine Ship Fires
                int maxShipFires = (int)((double)m_EmbarkLocations().Count / 3);
                int currentShipFires = m_ShipFires.Count;

                double missingHitPointsPercent = (double)(1 - (float)HitPoints / (float)MaxHitPoints);
                double percentBufferBeforeFires = .05;
                double fireInterval = (1 - percentBufferBeforeFires) / (double)maxShipFires;

                double adjustedMissingHitPointsPercent = missingHitPointsPercent - percentBufferBeforeFires;

                int desiredShipFires = 0;

                if (adjustedMissingHitPointsPercent > 0)
                    desiredShipFires = (int)(Math.Ceiling(adjustedMissingHitPointsPercent / fireInterval));

                if (desiredShipFires > maxShipFires)
                    desiredShipFires = maxShipFires;

                int newShipFires = desiredShipFires - currentShipFires;
                int shipFiresToRemove = 0;

                if (newShipFires < 0)
                    shipFiresToRemove = Math.Abs(newShipFires);

                //Add New Fires
                if (newShipFires > 0)
                {
                    int startingShipFires = m_ShipFires.Count;

                    for (int a = 0; a < newShipFires; a++)
                    {
                        List<Point3D> m_PotentialShipFireLocations = new List<Point3D>();

                        foreach (Point3D embarkLocation in m_EmbarkLocations())
                        {
                            bool foundActiveShipFire = false;

                            foreach (ShipFireItem activeShipFire in m_ShipFires)
                            {
                                Point3D activeShipFireLocation = new Point3D(activeShipFire.xOffset, activeShipFire.yOffset, activeShipFire.zOffset);

                                if (embarkLocation == activeShipFireLocation)
                                {
                                    foundActiveShipFire = true;
                                    break;
                                }
                            }

                            if (foundActiveShipFire == false)
                                m_PotentialShipFireLocations.Add(embarkLocation);
                        }

                        Point3D shipFireLocation = m_PotentialShipFireLocations[Utility.RandomMinMax(0, m_PotentialShipFireLocations.Count - 1)];
                        Point3D rotatedShipFireLocation = GetRotatedLocation(shipFireLocation.X, shipFireLocation.Y, shipFireLocation.Z);

                        if (!(rotatedShipFireLocation is Point3D))
                            continue;

                        Point3D point = new Point3D(rotatedShipFireLocation.X + this.X, rotatedShipFireLocation.Y + this.Y, rotatedShipFireLocation.Z + this.Z);

                        if (Facing == Direction.West || Facing == Direction.East)
                        {
                            ShipFireItem b = new ShipFireItem(0x398C, point, this.Map, TimeSpan.FromMinutes(60), shipFireLocation.X, shipFireLocation.Y, shipFireLocation.Z);
                            m_ShipFires.Add(b);
                        }

                        else
                        {
                            ShipFireItem b = new ShipFireItem(0x3996, point, this.Map, TimeSpan.FromMinutes(60), shipFireLocation.X, shipFireLocation.Y, shipFireLocation.Z);
                            m_ShipFires.Add(b);
                        }
                    }
                }

                //Remove Some Existing Fires
                else if (shipFiresToRemove > 0)
                {
                    for (int a = 0; a < shipFiresToRemove; a++)
                    {
                        ShipFireItem shipFire = m_ShipFires[m_ShipFires.Count - 1];
                        m_ShipFires.RemoveAt(m_ShipFires.Count - 1);
                        shipFire.Delete();
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

        private double m_DamageFromPlayerShipScalar = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double DamageFromPlayerShipScalar
        {
            get
            {
                if (m_DamageFromPlayerShipScalar == -1)
                    return BaseDamageFromPlayerShipScalar;

                return m_DamageFromPlayerShipScalar;
            }

            set
            {
                m_DamageFromPlayerShipScalar = value;

                if (m_DamageFromPlayerShipScalar < 0)
                    m_DamageFromPlayerShipScalar = 0;
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
               

        private List<ShipFireItem> m_ShipFires = new List<ShipFireItem>();
        public List<ShipFireItem> ShipFires
        {
            get { return m_ShipFires; }
            set { m_ShipFires = value; }
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

        private PlayerMobile m_Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner
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

        private string m_ShipName;
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

        private ShipUpgrades.ThemeType m_ThemeUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.ThemeType ThemeUpgrade
        {
            get { return m_ThemeUpgrade; }
            set { m_ThemeUpgrade = value; }
        }

        private ShipUpgrades.PaintType m_PaintUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.PaintType PaintUpgrade
        {
            get { return m_PaintUpgrade; }
            set { m_PaintUpgrade = value; }
        }

        private ShipUpgrades.CannonMetalType m_CannonMetalUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.CannonMetalType CannonMetalUpgrade
        {
            get { return m_CannonMetalUpgrade; }
            set { m_CannonMetalUpgrade = value; }
        }

        private ShipUpgrades.OutfittingType m_OutfittingUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.OutfittingType OutfittingUpgrade
        {
            get { return m_OutfittingUpgrade; }
            set { m_OutfittingUpgrade = value; }
        }

        private ShipUpgrades.FlagType m_FlagUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.FlagType FlagUpgrade
        {
            get { return m_FlagUpgrade; }
            set { m_FlagUpgrade = value; }
        }

        private ShipUpgrades.CharmType m_CharmUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.CharmType CharmUpgrade
        {
            get { return m_CharmUpgrade; }
            set { m_CharmUpgrade = value; }
        }

        private ShipUpgrades.MinorAbilityType m_MinorAbilityUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.MinorAbilityType MinorAbilityUpgrade
        {
            get { return m_MinorAbilityUpgrade; }
            set { m_MinorAbilityUpgrade = value; }
        }

        private ShipUpgrades.MajorAbilityType m_MajorAbilityUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.MajorAbilityType MajorAbilityUpgrade
        {
            get { return m_MajorAbilityUpgrade; }
            set { m_MajorAbilityUpgrade = value; }
        }

        private ShipUpgrades.EpicAbilityType m_EpicAbilityUpgrade;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipUpgrades.EpicAbilityType EpicAbilityUpgrade
        {
            get { return m_EpicAbilityUpgrade; }
            set { m_EpicAbilityUpgrade = value; }
        }

        public virtual BaseShipDeed ShipDeed { get { return null; } }

        public ShipRune ShipRune = null;
        public ShipRune ShipBankRune = null;

        public static List<BaseShip> m_Instances = new List<BaseShip>();

        private static bool NewShipMovement { get { return true; } }

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
            foreach (BaseShip targetShip in m_Instances)
            {
                if (targetShip == this) continue;

                if (targetShip.EmbarkedMobiles.Contains(mobile))
                    targetShip.EmbarkedMobiles.Remove(mobile);
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

                    BaseShip shipAtLocation = BaseShip.FindShipAt(location, from.Map);

                    if (shipAtLocation != null)
                    {
                        if (shipAtLocation.Deleted || shipAtLocation.m_SinkTimer != null)
                            shipAtLocation = null;                        
                    }

                    if (shipAtLocation != null)
                    {
                        if (shipAtLocation.IsFriend(m_From) || shipAtLocation.IsCoOwner(m_From) || shipAtLocation.IsOwner(m_From))
                        {
                            if (m_Followers)
                                shipAtLocation.EmbarkFollowers(m_From);

                            else
                                shipAtLocation.Embark(m_From, false);
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
            private BaseShip m_Ship;
            private Mobile m_From;
            private Boolean m_Followers;

            public DisembarkTarget(BaseShip ship, Mobile from, bool followers) : base(20, true, TargetFlags.None)
            {
                CheckLOS = true;

                m_Followers = followers;
                m_Ship = ship;
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
                bool shipLocation = false;
                bool guildDockLocation = false;

                if (SpellHelper.CheckMulti(pP, map))
                    multiLocation = true;

                if (BaseShip.FindShipAt(pP, map) != null)
                    shipLocation = true;
                
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

                else if (m_Ship.GetShipToLocationDistance(m_Ship, pP) > 8)
                    from.SendMessage("That location is too far away.");

                else if (!SpellHelper.CheckTravel(from, TravelCheckType.TeleportFrom))
                    from.SendLocalizedMessage(501942); // That location is blocked.                

                else if (!SpellHelper.CheckTravel(from, map, pP, TravelCheckType.TeleportTo))
                    from.SendLocalizedMessage(501942); // That location is blocked.                

                else if ((multiLocation && !guildDockLocation) || shipLocation)
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

                            if (m_Ship.Contains(follower) == false)
                                continue;

                            Point3D pFrom = follower.Location;
                            Point3D pTo = pP;

                            follower.Location = pTo;
                            follower.ProcessDelta();

                            m_Ship.RemoveEmbarkedMobile(follower);

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

                        m_Ship.RemoveEmbarkedMobile(from);

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
            from.Target = new BaseShip.EmbarkTarget(from, false);
        }

        public static void TargetedEmbarkFollowers(Mobile from)
        {
            from.SendMessage("Where do you wish to your followers to embark?");
            from.Target = new BaseShip.EmbarkTarget(from, true);
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

            int distance = GetShipToLocationDistance(this, from.Location);

            if (distance > 8 && boarding == false)
            {
                from.SendMessage("You are too far away to embark on this ship.");

                return false;
            }

            bool multiLocation = false;
            bool shipLocation = false;
            bool guildDockLocation = false;

            if (SpellHelper.CheckMulti(from.Location, from.Map))
                multiLocation = true;

            List<BaseMulti> m_Multis = BaseMulti.GetMultisAt(from.Location, from.Map);

            foreach (BaseMulti multi in m_Multis)
            {
                if (multi is BaseShip)
                {
                    shipLocation = true;
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

            if (multiLocation && !shipLocation && !guildDockLocation)
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
                bc_Creature.ShipOccupied = this;

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

                int distance = GetShipToLocationDistance(this, from.Location);

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
                        int followerDistanceToShip = GetShipToLocationDistance(this, follower.Location);

                        BaseShip followerShip = BaseShip.FindShipAt(follower.Location, follower.Map);

                        if (followerShip != null)
                        {
                            int originShipDistance = this.GetShipToShipDistance(followerShip, this);

                            if (originShipDistance < followerDistanceToShip)
                                followerDistanceToShip = originShipDistance;
                        }

                        bool inRange = false;

                        if (Contains(from) && followerDistanceToShip <= 8)
                            inRange = true;

                        if (followerDistanceToController <= 8 && followerDistanceToShip <= 8)
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
                                    bc_Creature.ShipOccupied = this;

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
                from.Target = new BaseShip.DisembarkTarget(this, from, false);
            }

            return true;
        }

        public bool DisembarkFollowers(Mobile from)
        {
            from.SendMessage("Where do you wish to disembark your followers?");
            from.Target = new BaseShip.DisembarkTarget(this, from, true);

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

        public class ShipCoOwnerTarget : Target
        {
            private BaseShip m_Ship;
            private bool m_Add;

            public ShipCoOwnerTarget(bool add, BaseShip ship): base(15, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Ship = ship;
                m_Add = add;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive || m_Ship.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    if (m_Add)
                        m_Ship.AddCoOwner(from, (Mobile)targeted);

                    else
                        m_Ship.RemoveCoOwner(from, (Mobile)targeted);
                }

                else
                    from.SendLocalizedMessage(501362); //That can't be a coowner                
            }
        }

        public class ShipFriendTarget : Target
        {
            private BaseShip m_Ship;
            private bool m_Add;

            public ShipFriendTarget(bool add, BaseShip ship): base(15, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Ship = ship;
                m_Add = add;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive || m_Ship.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    if (m_Add)
                        m_Ship.AddFriend(from, (Mobile)targeted);

                    else
                        m_Ship.RemoveFriend(from, (Mobile)targeted);
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

        public bool IsCoOwner(Mobile mobile)
        {
            if (mobile.AccessLevel > AccessLevel.Player)
                return true;

            if (mobile == null) return false;
            if (m_Owner == null) return false;

            PlayerMobile player = mobile as PlayerMobile;

            if (GuildAsFriends)
            {
                if (player != null)
                {
                    if (m_Owner.Guild == player.Guild && player.Guild != null)
                        return true;
                }
            }

            if (IPAsCoOwners)
            {
                if (player != null)
                {
                    if (PlayerMobile.IPMatch(m_Owner, player))
                        return true;
                }
            }

            return (m_CoOwners.Contains(player));
        }

        public bool IsFriend(Mobile mobile)
        {
            if (mobile.AccessLevel > AccessLevel.Player)
                return true;

            if (mobile == null) return false;
            if (m_Owner == null) return false;

            PlayerMobile player = mobile as PlayerMobile;
            
            if (GuildAsFriends)
            {
                if (player != null)
                {
                    if (m_Owner.Guild == player.Guild && player.Guild != null)
                        return true;
                }
            }

            if (IPAsFriends)
            {
                if (player != null)
                {
                    if (PlayerMobile.IPMatch(m_Owner, player))
                        return true;
                }
            }

            return (m_Friends.Contains(mobile));
        }

        public void AddCoOwnerCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsOwner(from))
            {
                from.SendMessage("Target the person you wish to name a co-owner of this ship.");
                from.Target = new BaseShip.ShipCoOwnerTarget(true, this);
            }
        }

        public void DryDockCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsCoOwner(from) || IsOwner(from))
                BeginDryDock(from);            
        }

        public void ClearTheDeckCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsCoOwner(from) || IsOwner(from))
                ClearTheDeck(true);
        }

        public void AddFriendCommand(Mobile from)
        {
            if (from == null)
                return;

            if (IsCoOwner(from) || IsOwner(from))
            {
                from.SendMessage("Target the person you wish to name a friend of this ship.");
                from.Target = new BaseShip.ShipFriendTarget(true, this);
            }
        }

        public class ThrowOverboardTarget : Target
        {
            private PlayerMobile m_Player;
            private BaseShip m_Ship;

            public ThrowOverboardTarget(Mobile from, BaseShip ship): base(15, false, TargetFlags.None)
            {
                PlayerMobile m_Player = from as PlayerMobile;

                if (m_Player == null || ship == null)
                    return;

                m_Ship = ship;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Player == null) return;
                if (m_Ship.Deleted) return;

                Mobile mobileTarget = targeted as Mobile;

                if (mobileTarget == null)
                {
                    m_Player.SendMessage("You may only target players and creatures with this.");
                    return;
                }

                bool playerIsOwner = m_Ship.IsOwner(m_Player);
                bool playerIsCoOwner = m_Ship.IsCoOwner(m_Player);
                bool playerIsFriend = m_Ship.IsFriend(m_Player);

                bool targetIsOwner = m_Ship.IsOwner(mobileTarget);
                bool targetIsCoOwner = m_Ship.IsCoOwner(mobileTarget);
                bool targetIsFriend = m_Ship.IsFriend(mobileTarget);
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
                        controllerIsOwner = m_Ship.IsOwner(bc_Target.ControlMaster);
                        controllerIsCoOwner = m_Ship.IsCoOwner(bc_Target.ControlMaster);
                        controllerIsFriend = m_Ship.IsFriend(bc_Target.ControlMaster);

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
                    List<Mobile> m_Mobiles = m_Ship.GetMobilesOnShip(true, false);
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

            from.SendMessage("Target the person or creature you wish to throw overboard. You may target yourself.");
            from.Target = new BaseShip.ThrowOverboardTarget(from, this);
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
                shipCannon.ShipFacingChange(m_Facing);
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

            foreach (ShipFireItem shipFire in m_ShipFires)
            {
                if (shipFire != null)
                {
                    Point3D rotatedShipFireLocation = GetRotatedLocation(shipFire.xOffset, shipFire.yOffset, shipFire.zOffset);

                    if (!(rotatedShipFireLocation is Point3D))
                        continue;

                    Point3D point = new Point3D(rotatedShipFireLocation.X + this.X, rotatedShipFireLocation.Y + this.Y, rotatedShipFireLocation.Z + this.Z);

                    shipFire.Location = point;

                    if (Facing == Direction.West || Facing == Direction.East)
                        shipFire.ItemID = 0x398C;

                    else
                        shipFire.ItemID = shipFire.ItemID = 0x3996;
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
            private BaseShip m_Ship;

            public MoveTimer(BaseShip ship, TimeSpan interval, bool single) : base(interval, interval, single ? 1 : 0)
            {
                m_Ship = ship;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                bool drift = (m_Ship.Moving != Forward && m_Ship.Moving != ForwardLeft && m_Ship.Moving != ForwardRight);
                bool fast = !m_Ship.ReducedSpeedMode;

                TimeSpan interval = (fast ? (drift ? TimeSpan.FromSeconds(m_Ship.FastDriftInterval) : TimeSpan.FromSeconds(m_Ship.FastInterval)) : (drift ? TimeSpan.FromSeconds(m_Ship.SlowDriftInterval) : TimeSpan.FromSeconds(m_Ship.SlowInterval)));
                double intervalSeconds = interval.TotalSeconds;               
               
                if (m_Ship.m_TempSpeedModifierExpiration > DateTime.UtcNow)
                {
                    if (m_Ship.m_TempSpeedModifier <= 0)
                    {
                        m_Ship.m_TillerMan.Say("Yar, something be holding us in place!");
                        m_Ship.StopMove(false);
                    }

                    else
                        intervalSeconds *= m_Ship.m_TempSpeedModifier;
                }

                if (intervalSeconds <= .05)
                    intervalSeconds = .05;

                interval = TimeSpan.FromSeconds(intervalSeconds);

                bool foundShipController = false;

                List<Mobile> shipOccupants = m_Ship.GetMobilesOnShip(true, true);

                foreach (Mobile occupant in shipOccupants)
                {
                    if (m_Ship.IsOwner(occupant) || m_Ship.IsCoOwner(occupant))
                    {
                        if (occupant.Alive)
                            foundShipController = true;
                    }
                }

                if (foundShipController == false)
                    m_Ship.StopMove(false);

                else if (!m_Ship.DoMovement(true))
                    m_Ship.StopMove(false);

                else
                {
                    m_Ship.m_TimeLastMoved = DateTime.UtcNow;

                    PlayerMobile pm_ShipOwner = m_Ship.Owner as PlayerMobile;

                    if (!m_Ship.ReducedSpeedMode && pm_ShipOwner != null)
                    {
                        if (pm_ShipOwner.ShipOccupied == m_Ship)
                        {
                            bool smoothSailing = PlayerEnhancementPersistance.IsCustomizationEntryActive(pm_ShipOwner, CustomizationType.SmoothSailing);

                            if (smoothSailing)
                                CustomizationAbilities.SmoothSailing(pm_ShipOwner);
                        }
                    }

                    if (m_Ship.ReducedSpeedMode)
                    {
                        if (DateTime.UtcNow > m_Ship.ReducedSpeedModeTime)
                        {
                            m_Ship.StopMove(false);
                            m_Ship.ReducedSpeedMode = false;
                            m_Ship.StartMove(m_Ship.Moving, true, false);

                            m_Ship.m_TillerMan.Say("Yar, back to full speed!");
                        }
                    }

                    else if (DateTime.UtcNow > m_Ship.ReducedSpeedModeTime + TimeSpan.FromSeconds((double)m_Ship.ReducedSpeedModeCooldown))
                    {
                        double sailsPercent = (double)((float)m_Ship.SailPoints / (float)m_Ship.MaxSailPoints);

                        double slowDownChance = .10 * (1 - sailsPercent);
                        double chance = Utility.RandomDouble();

                        if (chance < slowDownChance)
                        {
                            m_Ship.m_TillerMan.Say("Arr, we've slowed down!");

                            m_Ship.ReducedSpeedModeTime = DateTime.UtcNow + TimeSpan.FromSeconds((double)(Utility.RandomMinMax(m_Ship.ReducedSpeedModeMinDuration, m_Ship.ReducedSpeedModeMaxDuration)));

                            m_Ship.StopMove(false);
                            m_Ship.ReducedSpeedMode = true;
                            m_Ship.StartMove(m_Ship.Moving, false, false);
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

            if (!NewShipMovement || Math.Abs(xOffset) > 1 || Math.Abs(yOffset) > 1)
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
                
                foreach (ShipFireItem shipFire in m_ShipFires)
                {
                    if (shipFire != null)
                        SafeAdd(shipFire, toMove);
                }

                foreach (NetState netstate in Map.GetClientsInRange(Location, GetMaxUpdateRange()))
                {
                    Mobile mobile = netstate.Mobile;

                    if (netstate.HighSeas && mobile.CanSee(this) && mobile.InRange(Location, GetUpdateRange(mobile)))
                        netstate.Send(new MoveShipHS(mobile, this, d, clientSpeed, toMove, xOffset, yOffset));
                }

                foreach (IEntity entity in toMove)
                {
                    if (entity is Item)
                    {
                        Item item = (Item)entity;

                        item.NoMoveHS = true;

                        if (!(item is TillerMan || item is Hold || item is Plank || item is ShipFireItem || item is ShipCannon || item is ShipTrashBarrel))                        
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
            private BaseShip m_Ship;
            private int m_Offset;

            public TurnTimer(BaseShip ship, int offset): base(TimeSpan.FromSeconds(0.5))
            {
                m_Ship = ship;
                m_Offset = offset;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!m_Ship.Deleted)
                    m_Ship.Turn(m_Offset, true);
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

            foreach (ShipFireItem shipFire in m_ShipFires)
            {
                if (shipFire != null)
                    shipFire.Location = new Point3D(X + (shipFire.X - old.X), Y + (shipFire.Y - old.Y), Z + (shipFire.Z - old.Z));
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

        #region Can Use Command

        public bool CanUseCommand(PlayerMobile player, bool requireOnBoard, bool requireAlive, ShipAccessLevelType minimumAccessLevelRequired)
        {
            if (player == null) 
                return false;

            if (requireOnBoard && !Contains(player.Location))
            {
                player.SendMessage("You must be onboard this ship to do that.");
                return false;
            }

            if (requireAlive && !player.Alive)
            {
                player.SendMessage("You cannot do that while dead.");
                return false;
            }

            switch (minimumAccessLevelRequired)
            {
                case ShipAccessLevelType.None:
                    return true;
                break;

                case ShipAccessLevelType.Friend:
                    if (IsFriend(player) || IsCoOwner(player) || IsOwner(player))
                        return true; 
                break;

                case ShipAccessLevelType.CoOwner:
                    if (IsCoOwner(player) || IsOwner(player))                   
                        return true; 
                break;

                case ShipAccessLevelType.Owner:
                    if (IsOwner(player))
                        return true; 
                break;
            }            

            return false;
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
                m_TillerMan.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "Ar, the ship is destroyed sir.");
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

        #region Find Ship At

        public static BaseShip FindShipAt(IPoint2D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseShip ship = sector.Multis[i] as BaseShip;

                if (ship != null && ship.Contains(loc.X, loc.Y))
                    return ship;
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

        #region Get Mobiles on Ship

        public List<Mobile> GetMobilesOnShip(bool considerGhosts, bool considerStaff)
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

            BaseShip fromShip = null;
            BaseShip toShip = null;

            BaseCreature bc_From = from as BaseCreature;
            PlayerMobile pm_From = from as PlayerMobile;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            if (bc_From != null)
                fromShip = bc_From.ShipOccupied;

            if (pm_From != null)
                fromShip = pm_From.ShipOccupied;

            if (bc_Target != null)
            {
                if (bc_Target.IsOceanCreature)
                    return false;

                toShip = bc_Target.ShipOccupied;
            }

            if (pm_Target != null)
                toShip = pm_Target.ShipOccupied;

            bool fromShipValid = false;
            bool toShipValid = false;

            if (fromShip != null)
            {
                if (!fromShip.Deleted)
                {
                    fromShipValid = true;
                    fromShip.m_LastCombatTime = DateTime.UtcNow;
                }
            }

            if (toShip != null)
            {
                if (!toShip.Deleted)
                {
                    toShipValid = true;
                    toShip.m_LastCombatTime = DateTime.UtcNow;
                }
            }

            if (fromShipValid && !toShipValid)
                useShipBasedDamagePenalty = true;

            if (!fromShipValid && toShipValid)
                useShipBasedDamagePenalty = true;

            if (fromShipValid && toShipValid)
            {
                if (fromShip != toShip)
                    useShipBasedDamagePenalty = true;
            }

            return useShipBasedDamagePenalty;
        }

        #endregion

        #region Ship Damage

        public void ReceiveDamage(Mobile attacker, BaseShip attackerShip, int amount, DamageType damageType)
        {
            if (damageType == null)
                damageType = DamageType.Hull;

            if (amount <= 0)
                return;

            m_LastCombatTime = DateTime.UtcNow;

            int actualDamage = 0;

            if (attackerShip != null)
            {
                if (attackerShip.MobileControlType == MobileControlType.Player && MobileControlType != MobileControlType.Player)
                    amount = (int)((double)amount * DamageFromPlayerShipScalar);
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

            if (damageMessage != "" && attackerShip != null)
            {
                List<Mobile> m_MobilesOnShip = attackerShip.GetMobilesOnShip(true, true);

                foreach (Mobile mobile in m_MobilesOnShip)
                {
                    if (attackerShip.IsOwner(mobile) || attackerShip.IsCoOwner(mobile) || attackerShip.IsFriend(mobile))
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

                if (entry.m_Mobile == attacker && entry.m_Ship == attackerShip)
                {
                    entry.m_TotalAmount += damageAmount;
                    entry.m_lastDamage = time;

                    foundExistingEntry = true;

                    break;
                }
            }

            if (!foundExistingEntry)
                m_ShipDamageEntries.Add(new ShipDamageEntry(attacker, attackerShip, damageAmount, time));

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

                    foreach (BaseShip ship in m_Instances)
                    {
                        if (ship != null)
                        {
                            if (ship.ShipCombatant == this)
                                ship.ShipCombatant = null;
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

            Dictionary<BaseShip, int> DictShipDamagers = new Dictionary<BaseShip, int>();

            PlayerMobile shipPlayer = Owner as PlayerMobile;

            foreach (ShipDamageEntry entry in m_ShipDamageEntries)
            {
                PlayerMobile attackerPlayer = entry.m_Mobile as PlayerMobile;
                BaseShip attackerPlayerShip = entry.m_Ship;

                bool addAttackingPlayer = true;

                totalDamageToShip += entry.m_TotalAmount;

                if (attackerPlayer == null || attackerPlayerShip == null)
                    continue;

                if (DictShipDamagers.ContainsKey(attackerPlayerShip))
                {
                    int oldValue = 0;

                    DictShipDamagers.TryGetValue(attackerPlayerShip, out oldValue);
                    DictShipDamagers.Remove(attackerPlayerShip);
                    DictShipDamagers.Add(attackerPlayerShip, entry.m_TotalAmount + oldValue);
                }

                else
                    DictShipDamagers.Add(attackerPlayerShip, entry.m_TotalAmount);
            }

            BaseShip highestDamagingShip = null;
            double highestShipDamagePercent = 0;

            int shipDoubloonValue = DoubloonValue;
            int shipPlayerHoldDoubloonAmount = GetHoldDoubloonTotal(this);

            foreach (KeyValuePair<BaseShip, int> keyPair in DictShipDamagers)
            {
                BaseShip attackingShip = keyPair.Key as BaseShip;

                if (attackingShip == null) continue;
                if (attackingShip.Deleted) continue;
                if (attackingShip.Owner == null) continue;

                PlayerMobile attackingShipPlayerOwner = attackingShip.Owner as PlayerMobile;

                if (attackingShipPlayerOwner == null)
                    continue;

                double damagePercent = (double)keyPair.Value / (double)totalDamageToShip;

                if (damagePercent >= highestShipDamagePercent)
                {
                    highestDamagingShip = attackingShip;
                    highestShipDamagePercent = damagePercent;
                }

                int deposited = 0;

                if (attackingShip.TillerMan != null)
                    attackingShip.TillerMan.Say(RandomSunkOtherShipTillermanSpeech());

                if (shipDoubloonValue > 0 || shipPlayerHoldDoubloonAmount > 0)
                {
                    int shipDoubloonValueAdjusted = (int)((double)shipDoubloonValue * damagePercent);
                    
                    int finalDoubloonAmount = shipDoubloonValueAdjusted + shipPlayerHoldDoubloonAmount;

                    if (finalDoubloonAmount < 1)
                        finalDoubloonAmount = 1;

                    if (attackingShip.DepositDoubloons(finalDoubloonAmount))
                    {
                        Doubloon doubloonPile = new Doubloon(finalDoubloonAmount);
                        attackingShipPlayerOwner.SendSound(doubloonPile.GetDropSound());
                        doubloonPile.Delete();

                        attackingShipPlayerOwner.SendMessage("You've received " + finalDoubloonAmount.ToString() + " doubloons for sinking their ship! The coins have been placed in your ship's hold.");
                    }

                    else
                        attackingShipPlayerOwner.SendMessage("You've sunk a ship but alas there was no room in your ship's hold to place all the doubloons!");
                }
            }
        }

        private class ShipDamageEntryTimer : Timer
        {
            private BaseShip m_Ship;

            public ShipDamageEntryTimer(BaseShip ship)
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                m_Ship = ship;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                int damageEntries = m_Ship.m_ShipDamageEntries.Count;

                List<ShipDamageEntry> m_EntriesToRemove = new List<ShipDamageEntry>();

                for (int a = 0; a < damageEntries; a++)
                {
                    ShipDamageEntry entry = m_Ship.m_ShipDamageEntries[a];

                    if (entry.m_lastDamage + m_Ship.DamageEntryDuration < DateTime.UtcNow)
                        m_EntriesToRemove.Add(entry);
                }

                foreach (ShipDamageEntry entry in m_EntriesToRemove)
                {
                    m_Ship.m_ShipDamageEntries.Remove(entry);
                }

                if (m_Ship.m_ShipDamageEntries.Count == 0)
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
            private BaseShip m_Ship;
            private DateTime m_SinkTime;

            public SinkTimer(BaseShip ship): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Ship = ship;
                m_SinkTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 3));
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow > m_SinkTime)
                {
                    if (m_Ship.Z == -8)
                        m_Ship.DestroyShipMobiles(true, true);

                    if (m_Ship.Z == -13)
                    {
                        m_Ship.DestroyShipItems(true);

                        if (m_Ship.m_ShipSpawner != null)
                            m_Ship.m_ShipSpawner.ShipSunk(m_Ship);

                        if (m_Ship.Owner != null)
                            m_Ship.Owner.SendMessage("Your ship has been sunk.");

                        m_Ship.Delete();

                        this.Stop();
                    }

                    if (m_Ship.Z > -70)
                    {
                        m_Ship.Z -= 1;
                        Effects.PlaySound(m_Ship.Location, m_Ship.Map, 0x020);
                    }
                }
            }
        }

        public void TakeSinkingDamage(BaseShip ship, int damage)
        {
            if (ship == null) return;
            if (ship.Deleted) return;

            double damagePercent = 1 - ((double)ship.HitPoints / (double)ship.MaxHitPoints);

            int maxWater = (int)((double)ship.MaxHitPoints / 150);
            int currentWater = (int)(Math.Ceiling(damagePercent * (double)maxWater));

            for (int a = 0; a < currentWater; a++)
            {
                Blood water = new Blood();
                water.Hue = 2222;
                water.Name = "water";
                water.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                water.MoveToWorld(ship.GetRandomEmbarkLocation(false), ship.Map);
            }

            Effects.PlaySound(ship.Location, ship.Map, 0x020);

            string message = "";

            if (damagePercent < .33)
                message = "*water slowly fills the ship*";

            else if (damagePercent < .67)
                message = "*water rapidly pours into the ship*";

            else if (damagePercent < .95)
                message = "*water has almost flooded the ship*";

            else
                message = "*the ship is about to go under!*";

            ship.PublicOverheadMessage(MessageType.Regular, 0, false, message);

            ship.ReceiveDamage(null, null, 5, DamageType.Hull);
        }

        #endregion

        #region Refresh / Decay

        public void Refresh()
        {
            m_DecayTime = DateTime.UtcNow + ShipDecayDelay;
        }

        private class DecayTimer : Timer
        {
            private BaseShip m_Ship;
            private int m_Count;

            public DecayTimer(BaseShip ship): base(TimeSpan.Zero, BaseShip.PlayerShipDecayDamageDelay)
            {
                m_Ship = ship;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Ship == null) return;
                if (m_Ship.Deleted) return;

                List<Mobile> m_ShipMobiles = m_Ship.GetMobilesOnShip(true, true);

                foreach (Mobile mobile in m_ShipMobiles)
                {
                    if (mobile is PlayerMobile && mobile.Alive)
                    {
                        m_Ship.Refresh();
                        return;
                    }
                }

                if (m_Ship.m_DecayTime < DateTime.UtcNow && m_Ship.MobileControlType == MobileControlType.Player && !m_Ship.IsAdminOnboard())
                    m_Ship.TakeSinkingDamage(m_Ship, 5);
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
            private BaseShip m_Ship;

            public CannonCooldownTimer(BaseShip ship, DateTime cannonCooldown): base(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))
            {
                m_Ship = ship;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                /*
                if (m_Ship.CannonCooldown < DateTime.UtcNow)
                {
                    if (m_Ship.TillerMan != null)
                    {
                        if (m_Ship.m_SinkTimer == null)
                        {
                            int emptyCannons = 0;

                            for (int a = 0; a < m_Ship.Cannons.Count; a++)
                            {
                                if (m_Ship.Cannons[a].CurrentCharges == 0)
                                    emptyCannons++;
                            }

                            if (emptyCannons == m_Ship.Cannons.Count)
                                m_Ship.TillerMan.Say("We're out of ammunition, sir!");

                            else
                                m_Ship.TillerMan.Say("Cannons ready, sir!");
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

        public void DestroyShipItems(bool sink)
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

            foreach (ShipFireItem shipFire in m_ShipFires)
                shipFire.Delete();
            
            for (int i = 0; i < toMove.Count; ++i)
            {
                object o = toMove[i];

                if (o is Item && !(o is BaseShip))
                {
                    Item item = (Item)o;

                    if (!m_ItemsToSink.Contains(item) && sink)
                        continue;

                    if (Contains(item) && item.Z >= Z && !item.OceanStatic)
                        item.Delete();
                }
            }
        }

        public void DestroyShipMobiles(bool kill, bool sink)
        {
            if (Components == null || Map == null || Map == Map.Internal)
                return;

            List<BaseShip> m_NearbyShips = new List<BaseShip>();
            List<Mobile> m_MobilesOnShip = this.GetMobilesOnShip(true, true);
            List<BaseCreature> m_CreaturesToKill = new List<BaseCreature>();
            List<BaseCreature> m_TamedCreaturesToKill = new List<BaseCreature>();
            List<PlayerMobile> m_PlayersToKill = new List<PlayerMobile>();

            foreach (BaseShip targetShip in BaseShip.m_Instances)
            {
                if (targetShip == this) continue;
                if (targetShip.Deleted) continue;

                if (targetShip.GetShipToShipDistance(targetShip, this) <= 25)
                    m_NearbyShips.Add(targetShip);
            }

            foreach (Mobile mobile in m_MobilesOnShip)
            {
                if (mobile.AccessLevel > AccessLevel.Player)
                    continue;

                if (!EmbarkedMobiles.Contains(mobile))
                    continue;

                bool isOnOtherShip = false;

                foreach (BaseShip nearbyShip in m_NearbyShips)
                {
                    if (nearbyShip.EmbarkedMobiles.Contains(mobile))
                    {
                        isOnOtherShip = true;
                        break;
                    }
                }

                if (isOnOtherShip)
                    continue;

                if (!BaseShip.IsWaterTile(mobile.Location, mobile.Map))
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
                if (item is BaseMulti || item.ItemID > TileData.MaxItemValue || item.Z < p.Z || !item.Visible || item is ShipFireItem || item is Corpse || item is ShipSpawner || item is FishingSpotSpawner || item is TimedStatic || item is SingleFireField)
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
                if (o == this || o is TillerMan || o is Hold || o is Plank || o is ShipCannon || o is ShipFireItem || o is Corpse || o is ShipTrashBarrel || o is Blood || o is TimedStatic || o is SingleFireField)
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
                shipCannon.ShipFacingChange(m_Facing);
                shipCannon.Z += shipCannon.GetAdjustedCannonZOffset();                
            }

            foreach (ShipFireItem shipFire in m_ShipFires)
            {
                if (shipFire != null)
                {
                    Point3D rotatedShipFireLocation = GetRotatedLocation(shipFire.xOffset, shipFire.yOffset, shipFire.zOffset);

                    if (!(rotatedShipFireLocation is Point3D))
                        continue;

                    Point3D point = new Point3D(rotatedShipFireLocation.X + this.X, rotatedShipFireLocation.Y + this.Y, rotatedShipFireLocation.Z + this.Z);

                    if (Facing == Direction.West || Facing == Direction.East)
                        shipFire.ItemID = 0x398C;

                    else
                        shipFire.ItemID = shipFire.ItemID = 0x3996;
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
            foreach (ShipFireItem shipFire in ShipFires)
            {
                shipFire.Delete();
            }

            base.Delete();
        }

        public override void OnDelete()
        {
            if (m_ShipAITimer != null)
            {
                m_ShipAITimer.Stop();
                m_ShipAITimer = null;
            }

            if (m_DecayTimer != null)
            {
                m_DecayTimer.Stop();
                m_DecayTimer = null;
            }

            foreach (BaseShip ship in m_Instances)
            {
                if (ship != null)
                {
                    if (ship.ShipCombatant == this)
                        ship.ShipCombatant = null;
                }
            }

            DestroyShipItems(false);
            DestroyShipMobiles(false, false);

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

            if (ShipRune != null)
            {
                if (!ShipRune.Deleted)
                    ShipRune.Delete();
            }

            if (ShipBankRune != null)
            {
                if (!ShipBankRune.Deleted)
                    ShipBankRune.Delete();
            }
            
            m_Instances.Remove(this);
        }

        #endregion   

        #region Valid Ship Item

        public bool IsValidShipItem(Mobile from, Item item)
        {
            if (from == null || item == null)
                return true;

            if (item.GetType() == typeof(Doubloon))
                return true;
            
            if (item.GetType() == typeof(ShipTrashBarrel))
                return true;

            if (item.GetType() == typeof(TillerMan))
                return true;

            if (item.GetType() == typeof(ShipFireItem))
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
                    if (IsValidShipItem(Owner, item))
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
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the ship.

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
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the ship.

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

            BaseShipDeed shipDeed = ShipDeed;

            if (shipDeed == null)
                return;

            PullGhostsToPlayer(from);

            PushShipStoredPropertiesToDeed(this, shipDeed);           

            from.AddToBackpack(shipDeed);

            if (ShipRune != null)
            {
                if (!ShipRune.Deleted)
                    ShipRune.Delete();
            }

            if (ShipBankRune != null)
            {
                if (!ShipBankRune.Deleted)
                    ShipBankRune.Delete();
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

        #region Push Properties Between Ship and Ship Deed

        public static void PushDeedStoredPropertiesToShip(BaseShipDeed shipDeed,  BaseShip ship)
        {
            if (shipDeed == null) return;
            if (ship == null) return;

            ship.ShipName = shipDeed.m_ShipName;
            ship.Owner = shipDeed.m_Owner;

            ship.HitPoints = shipDeed.m_HitPoints;
            ship.SailPoints = shipDeed.m_SailPoints;
            ship.GunPoints = shipDeed.m_GunPoints;

            ship.TargetingMode = shipDeed.m_TargetingMode;

            foreach (Mobile mobile in shipDeed.m_CoOwners)
            {
                ship.CoOwners.Add(mobile);
            }

            foreach (Mobile mobile in shipDeed.m_Friends)
            {
                ship.Friends.Add(mobile);
            }

            ship.IPAsCoOwners = shipDeed.m_IPAsCoOwners;
            ship.GuildAsCoOwners = shipDeed.m_GuildAsCoOwners;
            ship.IPAsFriends = shipDeed.m_IPAsFriends;
            ship.GuildAsFriends = shipDeed.m_GuildAsFriends;

            ship.ThemeUpgrade = shipDeed.m_ThemeUpgrade;
            ship.PaintUpgrade = shipDeed.m_PaintUpgrade;
            ship.CannonMetalUpgrade = shipDeed.m_CannonMetalUpgrade;
            ship.OutfittingUpgrade = shipDeed.m_OutfittingUpgrade;
            ship.FlagUpgrade = shipDeed.m_FlagUpgrade;
            ship.CharmUpgrade = shipDeed.m_CharmUpgrade;
            ship.MinorAbilityUpgrade = shipDeed.m_MinorAbility;
            ship.MajorAbilityUpgrade = shipDeed.m_MajorAbility;
            ship.EpicAbilityUpgrade = shipDeed.m_EpicAbility;

            //TEST: Set Ship Repair Timers + Ability Cooldowns to Maximum Amount
        }

        public static void PushShipStoredPropertiesToDeed(BaseShip ship, BaseShipDeed shipDeed)
        {
            if (ship == null) return;
            if (shipDeed == null) return;

            shipDeed.m_ShipName = ship.ShipName;
            shipDeed.m_Owner = ship.Owner;

            shipDeed.m_HitPoints = ship.HitPoints;
            shipDeed.m_SailPoints = ship.SailPoints;
            shipDeed.m_GunPoints = ship.GunPoints;

            shipDeed.m_TargetingMode = ship.TargetingMode;            

            foreach (Mobile mobile in ship.m_CoOwners)
            {
                shipDeed.m_CoOwners.Add(mobile);
            }

            foreach (Mobile mobile in shipDeed.m_Friends)
            {
                shipDeed.m_Friends.Add(mobile);
            }

            shipDeed.m_IPAsCoOwners = ship.IPAsCoOwners;
            shipDeed.m_GuildAsCoOwners = ship.GuildAsCoOwners;
            shipDeed.m_IPAsFriends = ship.IPAsFriends;
            shipDeed.m_GuildAsFriends = ship.GuildAsFriends;

            shipDeed.m_ThemeUpgrade = ship.ThemeUpgrade;
            shipDeed.m_PaintUpgrade = ship.PaintUpgrade;
            shipDeed.m_CannonMetalUpgrade = ship.CannonMetalUpgrade;
            shipDeed.m_OutfittingUpgrade = ship.OutfittingUpgrade;
            shipDeed.m_FlagUpgrade = ship.FlagUpgrade;
            shipDeed.m_CharmUpgrade = ship.CharmUpgrade;
            shipDeed.m_MinorAbility = ship.MinorAbilityUpgrade;
            shipDeed.m_MajorAbility = ship.MajorAbilityUpgrade;
            shipDeed.m_EpicAbility = ship.EpicAbilityUpgrade;
        }

        #endregion
     
        #region Clear Deck

        public void ClearTheDeck(bool message)
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

            if (m_TillerMan != null && message)
                m_TillerMan.Say("Aye, clearing the decks of rubbish!");            
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

            if (!GetMobilesOnShip(true, true).Contains(from))
            {
                from.SendMessage("You must be onboard this ship in order to divide the plunder.");
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

        public int GetHoldDoubloonTotal(BaseShip ship)
        {
            if (ship == null)
                return 0;

            if (ship.Hold == null)
                return 0;

            if (ship.Hold.Deleted)
                return 0;

            int balance = 0;

            Item[] currencyPiles;

            currencyPiles = ship.Hold.FindItemsByType(typeof(Doubloon));

            for (int i = 0; i < currencyPiles.Length; ++i)
            {
                balance += currencyPiles[i].Amount;
            }

            return balance;
        }

        public bool TransferDoubloons(Mobile from, BaseShip ship, int amount, out int deposited)
        {
            deposited = 0;

            if (ship == null)
                return false;

            if (ship.Hold == null)
                return false;

            if (ship.Hold.Deleted)
                return false;

            int amountRemaining = amount;

            foreach (Item item in ship.Hold.FindItemsByType(typeof(Doubloon)))
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
               
                newCurrency.Amount = amountRemaining;

                if (ship.Hold.TryDropItem(from, newCurrency, true))
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

            int doubloonsRemaining = amount;

            Item[] m_Items = Hold.FindItemsByType(typeof(Doubloon));

            List<Doubloon> m_DoubloonPiles = new List<Doubloon>();

            foreach (Item item in m_Items)
            {
                Doubloon doubloon = item as Doubloon;

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
            private BaseShip m_Ship;

            public ScuttleTimer(BaseShip ship, TimeSpan interval): base(interval, interval)
            {
                m_Ship = ship;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Ship.HitPoints <= 0)
                    Stop();

                else
                    m_Ship.TakeSinkingDamage(m_Ship, 100);
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
                        from.CloseGump(typeof(ShipScuttleGump));
                        from.SendGump(new ShipScuttleGump(from, this));
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

        #region Ship-Relative Distances

        public int GetShipToLocationDistance(BaseShip shipFrom, Point3D locationTo)
        {
            if (shipFrom == null)
                return -1;

            List<Point3D> m_FromPoints = new List<Point3D>();

            m_FromPoints.Add(shipFrom.Location);
            
            if (shipFrom.Hold != null)
                m_FromPoints.Add(shipFrom.Hold.Location);

            if (shipFrom.TillerMan != null)
                m_FromPoints.Add(shipFrom.TillerMan.Location);

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

        public int GetShipToShipDistance(BaseShip shipFrom, BaseShip shipTo)
        {
            if (shipFrom == null || shipTo == null)
                return -1;

            List<Point3D> m_FromPoints = new List<Point3D>();

            m_FromPoints.Add(shipFrom.Location);
            
            if (shipFrom.Hold != null)
                m_FromPoints.Add(shipFrom.Hold.Location);

            if (shipFrom.TillerMan != null)
                m_FromPoints.Add(shipFrom.TillerMan.Location);

            List<Point3D> m_ToPoints = new List<Point3D>();

            m_ToPoints.Add(shipTo.Location);

            if (shipTo.Hold != null)
                m_ToPoints.Add(shipTo.Hold.Location);

            if (shipTo.TillerMan != null)
                m_ToPoints.Add(shipTo.TillerMan.Location);

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

        #region Ship Notoriety

        public static int GetShipNotoriety(Mobile from, BaseShip targetShip)
        {
            return Notoriety.Innocent;
        }

        #endregion

        #region Minor Ability

        public void ActivateMinorAbility(PlayerMobile player)
        {
        }

        #endregion

        #region Major Ability

        public void ActivateMajorAbility(PlayerMobile player)
        {
        }

        #endregion

        #region Epic Ability

        public void ActivateEpicAbility(PlayerMobile player)
        {
        }

        #endregion

        #region Sector Activation

        public override void OnSectorActivate()
        {
            if (MobileControlType != MobileControlType.Player && MobileControlType != MobileControlType.Null)
            {
                m_ShipAITimer = new ShipAITimer(this);
                m_ShipAITimer.Start();
            }

            base.OnSectorActivate();
        }  

        #endregion

        #region Ship AI

        public bool AcquireShipTarget()
        {
            m_LastAcquireTarget = DateTime.UtcNow;

            Dictionary<BaseShip, int> m_ShipsCanBeFiredAt = new Dictionary<BaseShip, int>();
            Dictionary<BaseShip, int> m_ShipsCannotBeFiredAt = new Dictionary<BaseShip, int>();

            foreach (BaseShip targetShip in BaseShip.m_Instances)
            {
                if (targetShip.Deleted) continue;
                if (targetShip == this) continue;

                if (targetShip.m_SinkTimer != null)
                {
                    if (targetShip.m_SinkTimer.Running)
                        continue;
                }

                if (Utility.GetDistanceToSqrt(Location, targetShip.Location) > ((double)PerceptionRange * 1.5))
                    continue;

                int distance = targetShip.GetShipToShipDistance(targetShip, this);

                if (distance > PerceptionRange)
                    continue;

                bool isAttackable = false;

                switch (MobileControlType)
                {
                    case MobileControlType.Good:
                        if (targetShip.MobileControlType == MobileControlType.Evil)
                            isAttackable = true;

                        if (!isAttackable && targetShip.MobileControlType == MobileControlType.Player)
                        {
                            List<Mobile> m_MobilesOnShip = targetShip.GetMobilesOnShip(true, false);

                            foreach (Mobile mobile in m_MobilesOnShip)
                            {
                                PlayerMobile player = mobile as PlayerMobile;

                                if (player != null)
                                {
                                    if ((targetShip.IsFriend(player) || targetShip.IsCoOwner(player) || targetShip.IsOwner(player)) && player.Criminal || player.Murderer)
                                    {
                                        isAttackable = true;
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case MobileControlType.Evil:
                        if (targetShip.MobileFactionType != MobileFactionType && targetShip.MobileControlType != MobileControlType.Neutral)
                            isAttackable = true;
                        
                        break;
                }

                foreach (ShipDamageEntry entry in m_ShipDamageEntries)
                {
                    if (entry == null) continue;
                    if (entry.m_Ship == null) continue;

                    if (!entry.m_Ship.Deleted && entry.m_Ship == targetShip && entry.m_Ship.MobileControlType == MobileControlType.Player)
                    {
                        isAttackable = true;
                        break;
                    }
                }

                if (isAttackable)
                {
                    int weightValue = 0;
                    int distanceWeight = 0;
                    double hullPercentLost = 1 - ((double)targetShip.HitPoints / (double)targetShip.MaxHitPoints);

                    weightValue += (int)(hullPercentLost * 10);
                    weightValue += GetDistanceWeight(distance);

                    if (CanHitTargetShip(targetShip, true))
                        m_ShipsCanBeFiredAt.Add(targetShip, weightValue);

                    else
                        m_ShipsCannotBeFiredAt.Add(targetShip, weightValue);
                }

            }

            if (m_ShipsCanBeFiredAt.Count > 0)
            {
                int TotalValues = 0;

                foreach (KeyValuePair<BaseShip, int> pair in m_ShipsCanBeFiredAt)
                {
                    TotalValues += pair.Value;
                }

                double ActionCheck = Utility.RandomDouble();
                double CumulativeAmount = 0.0;
                double AdditionalAmount = 0.0;

                bool foundDirection = true;

                foreach (KeyValuePair<BaseShip, int> pair in m_ShipsCanBeFiredAt)
                {
                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                    if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                    {
                        m_ShipCombatant = pair.Key;
                        return true;

                        break;
                    }

                    CumulativeAmount += AdditionalAmount;
                }
            }

            else
            {
                int TotalValues = 0;

                foreach (KeyValuePair<BaseShip, int> pair in m_ShipsCannotBeFiredAt)
                {
                    TotalValues += pair.Value;
                }

                double ActionCheck = Utility.RandomDouble();
                double CumulativeAmount = 0.0;
                double AdditionalAmount = 0.0;

                bool foundDirection = true;

                foreach (KeyValuePair<BaseShip, int> pair in m_ShipsCannotBeFiredAt)
                {
                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                    if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                    {
                        m_ShipCombatant = pair.Key;
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
            foreach (Mobile mobile in GetMobilesOnShip(true, true))
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

        public bool CanHitTargetShip(BaseShip targetShip, bool considerRange)
        {
            if (this == null) return false;
            if (targetShip == null) return false;
            if (targetShip.Deleted) return false;

            int modifiedRange = (int)(Math.Round((double)CannonMaxRange * CannonRangeScalar));

            foreach (ShipCannon shipCannon in m_Cannons)
            {
                if (considerRange && targetShip.GetShipToLocationDistance(targetShip, shipCannon.Location) > modifiedRange)
                    continue;

                if (shipCannon.InAngle(targetShip.Location))
                    return true;

                if (targetShip.TillerMan != null)
                {
                    if (shipCannon.InAngle(targetShip.TillerMan.Location))
                        return true;
                }

                if (targetShip.Hold != null)
                {
                    if (shipCannon.InAngle(targetShip.Hold.Location))
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
                    if (m_ShipAITimer != null)
                    {
                        m_ShipAITimer.Stop();
                        m_ShipAITimer = null;
                    }

                    ShipCombatant = null;
                    StopMove(false);

                    return;
                }
            }

            //No Crew Left
            if (!HasCrewAlive())
            {
                ShipCombatant = null;
                StopMove(false);

                return;
            }

            if (m_ShipCombatant != null)
            {
                if (m_ShipCombatant.Deleted || m_ShipCombatant.HitPoints <= 0)
                    m_ShipCombatant = null;

                else if (m_ShipCombatant.m_SinkTimer != null)
                {
                    if (m_ShipCombatant.m_SinkTimer.Running)
                        m_ShipCombatant = null;
                }
            }

            //Ship Doesn't Have Cannons
            if (m_Cannons.Count == 0)
                return;

            //Current Combatant is Out of Active Range
            if (m_ShipCombatant != null)
            {
                if (Utility.GetDistanceToSqrt(Location, m_ShipCombatant.Location) > (double)PerceptionRange * 1.5)
                    m_ShipCombatant = null;
            }

            if (m_ShipCombatant == null)
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

            if (m_ShipCombatant != null)
            {
                //Stop Ship if Our Desired Target is Within LOS and Within Range
                if (CanHitTargetShip(m_ShipCombatant, true))
                    StopMove(false);

                //Have Crew Attack Enemy Ship Mobiles If They Currently Haven't Engaged Anyone
                if ((m_LastAcquireTarget + TimeSpan.FromSeconds(m_AcquireTargetDelayAmount)) <= DateTime.UtcNow)
                {
                    List<Mobile> m_MobilesOnShipCombatant = m_ShipCombatant.GetMobilesOnShip(false, false);

                    if (m_MobilesOnShipCombatant.Count > 0)
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
                                crewman.Combatant = m_MobilesOnShipCombatant[Utility.RandomMinMax(0, m_MobilesOnShipCombatant.Count - 1)];
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
            if (DateTime.UtcNow >= CannonCooldown && m_ShipCombatant != null)
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
                    if (m_ShipCombatant == null) break;
                    if (m_ShipCombatant.Deleted) break;

                    if (GetShipToLocationDistance(m_ShipCombatant, Location) <= modifiedRange)
                        inRange = true;

                    if (shipCannon.Ammunition > 0)
                        hasAmmo = true;

                    if (shipCannon.InAngle(m_ShipCombatant.Location))
                    {
                        bestCannon = shipCannon;

                        canHitCenter = true;
                        canFire = true;
                    }

                    if (m_ShipCombatant.Hold != null)
                    {
                        if (shipCannon.InAngle(m_ShipCombatant.Hold.Location))
                        {
                            if (bestCannon == null)
                                bestCannon = shipCannon;

                            canHitHold = true;
                            canFire = true;
                        }
                    }

                    if (m_ShipCombatant.TillerMan != null)
                    {
                        if (shipCannon.InAngle(m_ShipCombatant.TillerMan.Location))
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

                    bestCannon.OnTarget(firingCrewman, m_ShipCombatant.Location, true, canHitCenter, canHitHold, canHitTillerman);

                    if (m_ShipCombatant.ShipCombatant == null)
                        m_ShipCombatant.ShipCombatant = this;
                }

                if (!hasAmmo)
                    readyToFire = false;
            }

            int baseCannonRange = (int)(Math.Round((double)CannonMaxRange * CannonRangeScalar));

            //Ship Was Ready to Fire At Target But Couldn't: Make Maneuvers To Adjust            
            if (readyToFire && !firedAnyCannon && m_ShipCombatant != null)
            {
                int distance = GetShipToShipDistance(this, m_ShipCombatant);
                bool turned = false;
                bool needMovement = false;

                //In LOS But Out of Range
                if (CanHitTargetShip(m_ShipCombatant, false))
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
            
            writer.Write(m_ShipItems.Count);
            foreach (Item item in m_ShipItems)
            {
                writer.Write(item);
            }

            writer.Write(m_ShipDamageEntries.Count);
            foreach (ShipDamageEntry entry in m_ShipDamageEntries)
            {
                writer.Write(entry.m_Mobile);
                writer.Write(entry.m_Ship);
                writer.Write(entry.m_TotalAmount);
                writer.Write((DateTime)entry.m_lastDamage);
            }

            writer.Write((int)MobileControlType);
            writer.Write((int)MobileFactionType);            
            writer.Write((int)m_Facing);           
            writer.Write(m_DecayTime);
            writer.Write(m_CannonCooldown);              
            writer.Write(m_PPlank);
            writer.Write(m_SPlank);
            writer.Write(m_TillerMan);
            writer.Write(m_Hold);
            writer.Write(m_ShipTrashBarrel);
            writer.Write(m_Anchored);            
            writer.Write(m_ScuttleInProgress);           
            writer.Write(m_LastCombatTime);
            writer.Write(m_TimeLastMoved);
            writer.Write(m_TimeLastRepaired);
            writer.Write(m_NextTimeRepairable);
            writer.Write(m_ReducedSpeedMode);
            writer.Write(m_ReducedSpeedModeTime);
            writer.Write(m_ShipSpawner);
            writer.Write(m_ShipCombatant);
            writer.Write(ShipRune);
            writer.Write(ShipBankRune);
            writer.Write(m_LastActivated);
            writer.Write(AdminControlled);
            writer.Write(MaxHitPoints);
            writer.Write(MaxSailPoints);
            writer.Write(MaxGunPoints);
            writer.Write(PerceptionRange);
            writer.Write(CannonAccuracyModifer);
            writer.Write(CannonRangeScalar);
            writer.Write(CannonDamageScalar);
            writer.Write(DamageFromPlayerShipScalar);
            writer.Write(FastInterval);
            writer.Write(FastDriftInterval);
            writer.Write(SlowInterval);
            writer.Write(SlowDriftInterval);
            writer.Write(DoubloonValue);
            writer.Write(m_CannonHue);
            writer.Write(CannonReloadTimeScalar);
            writer.Write(m_TempSpeedModifier);
            writer.Write(m_TempSpeedModifierExpiration);

            //-----Player Persistant Properties: Must Also Be Stored to Ship Deed

            writer.Write(m_ShipName);
            writer.Write(m_Owner);

            writer.Write(HitPoints);
            writer.Write(SailPoints);
            writer.Write(GunPoints);

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
            writer.Write((int)m_FlagUpgrade);
            writer.Write((int)m_CharmUpgrade);
            writer.Write((int)m_MinorAbilityUpgrade);
            writer.Write((int)m_MajorAbilityUpgrade);
            writer.Write((int)m_EpicAbilityUpgrade);
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

                int shipItemsCount = reader.ReadInt();
                for (int a = 0; a < shipItemsCount; a++)
                {
                    m_ShipItems.Add(reader.ReadItem());
                }

                int shipDamageEntriesCount = reader.ReadInt();
                for (int a = 0; a < shipDamageEntriesCount; a++)
                {
                    Mobile mobile = reader.ReadMobile();
                    BaseShip ship = (BaseShip)reader.ReadItem();
                    int totalAmount = reader.ReadInt();
                    DateTime lastDamage = (DateTime)reader.ReadDateTime();

                    ShipDamageEntry entry = new ShipDamageEntry(mobile, ship, totalAmount, lastDamage);

                    m_ShipDamageEntries.Add(entry);
                }

                MobileControlType = (MobileControlType)reader.ReadInt();
                MobileFactionType = (MobileFactionType)reader.ReadInt();                
                Facing = (Direction)reader.ReadInt();

                DecayTime = reader.ReadDateTime();
                m_CannonCooldown = reader.ReadDateTime();  
                
                PPlank = (Plank)reader.ReadItem();
                SPlank = (Plank)reader.ReadItem();
                TillerMan = (TillerMan)reader.ReadItem();
                Hold = reader.ReadItem() as Hold;
                ShipTrashBarrel = (ShipTrashBarrel)reader.ReadItem();
                Anchored = reader.ReadBool();
                
                m_ScuttleInProgress = reader.ReadBool();
                LastCombatTime = reader.ReadDateTime();
                TimeLastMoved = reader.ReadDateTime();
                TimeLastRepaired = reader.ReadDateTime();
                NextTimeRepairable = reader.ReadDateTime();
                ReducedSpeedMode = reader.ReadBool();
                ReducedSpeedModeTime = reader.ReadDateTime();
                m_ShipSpawner = (ShipSpawner)reader.ReadItem();
                ShipCombatant = (BaseShip)reader.ReadItem();
                ShipRune = (ShipRune)reader.ReadItem();
                ShipBankRune = (ShipRune)reader.ReadItem();
                m_LastActivated = reader.ReadDateTime();
                AdminControlled = reader.ReadBool();
                MaxHitPoints = reader.ReadInt();
                MaxSailPoints = reader.ReadInt();
                MaxGunPoints = reader.ReadInt();
                PerceptionRange = reader.ReadInt();
                CannonAccuracyModifer = reader.ReadDouble();
                CannonRangeScalar = reader.ReadDouble();
                CannonDamageScalar = reader.ReadDouble();
                DamageFromPlayerShipScalar = reader.ReadDouble();
                FastInterval = reader.ReadDouble();
                FastDriftInterval = reader.ReadDouble();
                SlowInterval = reader.ReadDouble();
                SlowDriftInterval = reader.ReadDouble();
                DoubloonValue = reader.ReadInt();
                CannonHue = reader.ReadInt();
                CannonReloadTimeScalar = reader.ReadDouble();
                TempSpeedModifier = reader.ReadDouble();
                TempSpeedModifierExpiration = reader.ReadDateTime();  
                                                
                //-----Player Persistant Properties: Must Also Be Stored to Ship Deed

                ShipName = reader.ReadString();
                Owner = (PlayerMobile)reader.ReadMobile();

                HitPoints = reader.ReadInt();
                SailPoints = reader.ReadInt();
                GunPoints = reader.ReadInt();

                TargetingMode = (TargetingMode)reader.ReadInt();

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

                m_IPAsCoOwners = reader.ReadBool();
                m_GuildAsCoOwners = reader.ReadBool();
                m_IPAsFriends = reader.ReadBool();
                m_GuildAsFriends = reader.ReadBool();

                ThemeUpgrade = (ShipUpgrades.ThemeType)reader.ReadInt();
                PaintUpgrade = (ShipUpgrades.PaintType)reader.ReadInt();
                CannonMetalUpgrade = (ShipUpgrades.CannonMetalType)reader.ReadInt();
                OutfittingUpgrade = (ShipUpgrades.OutfittingType)reader.ReadInt();
                FlagUpgrade = (ShipUpgrades.FlagType)reader.ReadInt();
                CharmUpgrade = (ShipUpgrades.CharmType)reader.ReadInt();
                MinorAbilityUpgrade = (ShipUpgrades.MinorAbilityType)reader.ReadInt();
                MajorAbilityUpgrade = (ShipUpgrades.MajorAbilityType)reader.ReadInt();
                EpicAbilityUpgrade = (ShipUpgrades.EpicAbilityType)reader.ReadInt();

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

                    List<Mobile> m_MobilesOnShip = this.GetMobilesOnShip(true, true);

                    foreach (Mobile mobile in m_MobilesOnShip)
                    {
                        if (!EmbarkedMobiles.Contains(mobile))
                            EmbarkedMobiles.Add(mobile);

                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature != null)
                        {
                            if (!(bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile))
                                bc_Creature.ShipOccupied = this;
                        }
                    }
                });
            }  
        }

        #endregion        

        #region Packets

        public sealed class MoveShipHS : Packet
        {
            public MoveShipHS(Mobile beholder, BaseShip ship, Direction d, int speed, List<IEntity> ents, int xOffset, int yOffset): base(0xF6)
            {
                EnsureCapacity(3 + 15 + ents.Count * 10);

                m_Stream.Write((int)ship.Serial);
                m_Stream.Write((byte)speed);
                m_Stream.Write((byte)d);
                m_Stream.Write((byte)ship.Facing);
                m_Stream.Write((short)(ship.X + xOffset));
                m_Stream.Write((short)(ship.Y + yOffset));
                m_Stream.Write((short)ship.Z);
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

        public sealed class DisplayShipHS : Packet
        {
            public DisplayShipHS(Mobile beholder, BaseShip ship): base(0xF7)
            {
                List<IEntity> ents = ship.GetMovingEntities(false);

                SafeAdd(ship.TillerMan, ents);
                SafeAdd(ship.Hold, ents);
                SafeAdd(ship.PPlank, ents);
                SafeAdd(ship.SPlank, ents);

                ents.Add(ship);

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

    #region Ship AI Timer

    public class ShipAITimer : Timer
    {
        private BaseShip m_Ship;

        public ShipAITimer(BaseShip ship): base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.5))
        {
            m_Ship = ship;
            Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            if (m_Ship.Deleted)
            {
                Stop();
                return;
            }

            Sector sect = m_Ship.Map.GetSector(m_Ship.Location);

            if (!sect.Active && m_Ship.HasCrewAlive() && m_Ship.MobileControlType != MobileControlType.Player && !m_Ship.IsAdminOnboard() && !m_Ship.AdminControlled)
            {
                if (m_Ship.LastActivated + m_Ship.DeactivateDelay < DateTime.UtcNow)
                {
                    if (m_Ship.m_ShipAITimer != null)
                    {
                        m_Ship.m_ShipAITimer.Stop();
                        m_Ship.m_ShipAITimer = null;
                    }

                    return;
                }
            }

            else
                m_Ship.LastActivated = DateTime.UtcNow;

            if (m_Ship.MobileControlType != MobileControlType.Neutral && !m_Ship.IsAdminOnboard() && !m_Ship.HasCrewAlive() && m_Ship.MobileControlType != MobileControlType.Player && m_Ship.NextSinkDamageAllowed <= DateTime.UtcNow)
            {
                m_Ship.TakeSinkingDamage(m_Ship, 5);
                m_Ship.NextSinkDamageAllowed = DateTime.UtcNow + BaseShip.NPCShipUncrewedDamageDelay;
            }

            else
                m_Ship.OnThink();
        }
    }

    #endregion

    #region Ship Damage Entry

    public class ShipDamageEntry
    {
        public Mobile m_Mobile;
        public BaseShip m_Ship;
        public int m_TotalAmount;
        public DateTime m_lastDamage;

        public ShipDamageEntry(Mobile mobile, BaseShip ship, int totalAmount, DateTime lastDamage)
        {
            m_Mobile = mobile;
            m_Ship = ship;
            m_TotalAmount = totalAmount;
            m_lastDamage = lastDamage;
        }
    }

    #endregion

    #region Ship Fire

    public class ShipFireItem : Item
    {
        private Timer m_Timer;
        private DateTime m_End;

        public int xOffset;
        public int yOffset;
        public int zOffset;

        public ShipFireItem(int itemID, Point3D loc, Map map, TimeSpan duration, int xOff, int yOff, int zOff): base(itemID)
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

        public ShipFireItem(Serial serial): base(serial)
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
            private ShipFireItem m_Item;

            public InternalTimer(ShipFireItem item): base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
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