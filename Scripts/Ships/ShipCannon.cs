using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server
{
    public class ShipCannon : Item
    {
        public enum CannonType
        {
            Small
        }

        public enum CannonPosition
        {
            Left,
            Right,
            Front,
            Rear
        }

        public BaseShip m_Ship;

        public CannonType m_CannonType = CannonType.Small;
        public CannonPosition m_CannonPosition = CannonPosition.Left;

        public int m_xOffset;
        public int m_yOffset;
        public int m_zOffset;

        private int m_Ammunition;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Ammunition
        {
            get { return m_Ammunition; }
            set { m_Ammunition = value; }
        }

        private Direction m_Facing;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        }       

        [Constructable]
        public ShipCannon(): base()
        {
            Movable = false;
        }

        public ShipCannon(Serial serial): base(serial)
        {
        }

        #region Place Ship Cannon

        public static void PlaceShipCannon(BaseShip ship, Point3D point, CannonType cannonType, CannonPosition cannonPosition)
        {
            if (ship == null)
                return;

            ShipCannon shipCannon = new ShipCannon();

            shipCannon.Visible = false;

            shipCannon.m_Ship = ship;
            shipCannon.m_CannonType = cannonType;
            shipCannon.m_CannonPosition = cannonPosition;
            shipCannon.m_xOffset = point.X;
            shipCannon.m_yOffset = point.Y;
            shipCannon.m_zOffset = point.Z;

            Point3D cannonLocation = ship.GetRotatedLocation(point.X, point.Y, 0);

            shipCannon.MoveToWorld(new Point3D(ship.Location.X + cannonLocation.X, ship.Location.Y + cannonLocation.Y, ship.Location.Z + cannonLocation.Z), ship.Map);
            shipCannon.ShipFacingChange(ship.Facing);
            shipCannon.Z = ship.Location.Z + cannonLocation.Z + shipCannon.GetAdjustedCannonZOffset();

            shipCannon.Hue = ship.CannonHue;

            if (ship.MobileControlType != MobileControlType.Player)
                shipCannon.Ammunition = shipCannon.GetMaxAmmunition();

            shipCannon.Visible = true;

            ship.m_Cannons.Add(shipCannon);

            switch (cannonPosition)
            {
                case CannonPosition.Left: ship.m_LeftCannons.Add(shipCannon); break;
                case CannonPosition.Right: ship.m_RightCannons.Add(shipCannon); break;
                case CannonPosition.Front: ship.m_FrontCannons.Add(shipCannon); break;
                case CannonPosition.Rear: ship.m_RearCannons.Add(shipCannon); break;
            }
        }

        #endregion

        #region Ship Facing Change

        public void ShipFacingChange(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;
                    }
                break;

                case Direction.South:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;
                    }
                break;

                case Direction.East:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;
                    }
                break;

                case Direction.West:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;
                    }
                break;
            }
        }

        #endregion

        #region GetAdjustedCannonZOffset()

        public int GetAdjustedCannonZOffset()
        {
            if (m_Ship == null)
                return 0;

            int adjustZ = 0;

            switch (m_Ship.Facing)
            {
                case Direction.North:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.East:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;

                case Direction.South:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.West:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;
            }
          
            return adjustZ;
        }

        #endregion

        public bool InAngle(Point3D point)
        {
            Point3D loc = this.Location;
            int x = point.X - loc.X;
            int y = point.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            switch (Facing)
            {
                case Direction.North: { if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.East: { if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
                case Direction.South: { if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.West: { if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
            }

            return false;
        }

        public int GetMaxAmmunition()
        {
            int maxAmmunition = BaseShip.CannonMaxAmmunition;

            return maxAmmunition;
        }

        public override void OnSingleClick(Mobile from)
        {    
            base.OnSingleClick(from);

            if (m_Ship == null)
                return;

            LabelTo(from, "[Ammunition: {0}/{1}]", Ammunition, GetMaxAmmunition());

            if (m_Ship.CannonCooldown <= DateTime.UtcNow)
            {
                if (Ammunition > 0)                    
                    LabelTo(from, "Ready to fire");                    

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }

            else
            {
                if (Ammunition > 0)
                {
                    int secondsToFire = (int)(Math.Ceiling((m_Ship.CannonCooldown - DateTime.UtcNow).TotalSeconds));

                    if (secondsToFire == 0)                        
                        LabelTo(from, "Ready to fire");                        

                    else                        
                        LabelTo(from, "Fireable in " + secondsToFire.ToString() + " seconds");                        
                }

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }            

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Ship == null)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use this.");
                return;
            }

            else if (!(m_Ship.IsOwner(from) || m_Ship.IsCoOwner(from)))
            {
                from.SendMessage("You do not have permission to use this.");
                return;
            }

            else if (!m_Ship.Contains(from))
            {
                from.SendMessage("You cannot reach that.");
                return;
            }

            else if (Ammunition == 0)
            {
                from.SendMessage("Those cannons are out of ammunition.");
                return;
            }

            else if (DateTime.UtcNow < m_Ship.CannonCooldown)
            {
                from.SendMessage("You must wait before firing another cannon volley.");
                return;
            }

            else
            {
                from.SendMessage("Where would you like to fire the cannon volley?");
                from.Target = new CannonTarget(this);
                from.RevealingAction();
            }
        }

        private class CannonTarget : Target
        {
            private ShipCannon m_ShipCannon;

            public CannonTarget(ShipCannon shipCannon): base(25, true, TargetFlags.Harmful)
            {
                m_ShipCannon = shipCannon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_ShipCannon == null) return;
                if (m_ShipCannon.Deleted) return;

                IPoint3D location = targeted as IPoint3D;

                if (location != null)
                {
                    if (location is Item)
                        location = ((Item)location).GetWorldTop();

                    else if (location is Mobile)
                        location = ((Mobile)location).Location;

                    m_ShipCannon.OnTarget(from, new Point3D(location.X, location.Y, location.Z), false, false, false, false);
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point, bool IsNPCShip, bool canHitCenter, bool canHitHold, bool canHitTillerman)
        {
            if (m_Ship == null)
                return;

            Map map = from.Map;
            BaseShip targetShip = BaseShip.FindShipAt(point, map);

            //For Player Ships
            if (m_Ship.MobileControlType == MobileControlType.Player)
            {
                if (!from.Player)
                    return;

                else if (!from.Alive)
                {
                    from.SendMessage("You must be alive to use this.");
                    return;
                }

                else if (!m_Ship.Contains(from))
                {
                    from.SendMessage("You are no longer on the ship.");
                    return;
                }

                else if (targetShip == m_Ship)
                {
                    from.SendMessage("You may not fire onto your own ship!");
                    return;
                }

                else if (Ammunition == 0)
                {
                    from.SendMessage("At least one of your cannons must be loaded to fire a volley.");
                    return;
                }

                else if (DateTime.UtcNow < m_Ship.CannonCooldown)
                {
                    from.SendMessage("You must wait before firing another cannon volley.");
                    return;
                }
            }

            bool volleyValid = false;
            bool tooClose = false;

            double cannonDelayTotal = 0;
            int cannonsFiring = 0;

            //Need At Least One Cannon With LOS to Target and In Range of Target For Volley To Be Valid
            foreach (ShipCannon shipCannon in m_Ship.m_Cannons)
            {
                //Cannon Has Ammunition and is on Correct Ship Side for Volley
                if (shipCannon.Ammunition > 0 && shipCannon.Facing == Facing)
                {
                    cannonDelayTotal += BaseShip.CannonCooldownTime;
                    cannonsFiring++;

                    double modifiedRange = (double)BaseShip.CannonMaxRange * m_Ship.CannonRangeScalar;

                    //Already Deterined to Be Valid Shot: NPC AI Ship
                    if (IsNPCShip)
                    {
                        volleyValid = true;
                        break;
                    }

                    //Cannon is in LOS and Within Range
                    if (shipCannon.InAngle(point) && Utility.GetDistanceToSqrt(shipCannon.Location, point) <= modifiedRange)
                        volleyValid = true;

                    //Cannon is too close
                    if (Utility.GetDistanceToSqrt(shipCannon.Location, point) < 2)
                        tooClose = true;
                }
            }

            //At Least One Cannon Was Too Close to Fire
            if (tooClose)
                volleyValid = false;

            //Can Fire Cannon Volley
            if (volleyValid)
            {
                if (m_Ship.TillerMan != null)
                    m_Ship.TillerMan.Say("Firing cannons!");

                m_Ship.LastCombatTime = DateTime.UtcNow;

                //Ship Cooldown Time (Average of Delay for Each Cannon Type that is Firing)
                double cooldown = cannonDelayTotal / cannonsFiring;

                m_Ship.CannonCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(cooldown);
                m_Ship.StartCannonCooldown();

                List<ShipCannon> cannonsToFire = new List<ShipCannon>();
                
                foreach (ShipCannon shipCannon in m_Ship.m_Cannons)
                {
                    if (shipCannon.Ammunition > 0 && shipCannon.Facing == Facing)                    
                        cannonsToFire.Add(shipCannon);
                }

                int firingLoops = BaseShip.CannonFiringLoops;

                int cannonCount = cannonsToFire.Count;

                for (int a = 0; a < firingLoops; a++)
                {
                    for (int b = 0; b < cannonCount; b++)
                    {
                        bool showSmoke = false;
                        bool lastCannon = false;

                        int cannonIndex = Utility.RandomMinMax(0, cannonsToFire.Count - 1);
                        ShipCannon shipCannon = cannonsToFire[cannonIndex];

                        if (a == 0)
                            showSmoke = true;

                        if (a == (firingLoops - 1))
                        {
                            shipCannon.Ammunition--;
                            cannonsToFire.RemoveAt(cannonIndex);

                            if (b == cannonCount - 1)
                                lastCannon = true;
                        }

                        //Check Accuracy
                        double cannonAccuracy = BaseShip.CannonAccuracy * m_Ship.CannonAccuracyModifer;

                        double opponentMovementPenalty = 0;
                        double movementAccuracyPenalty = 0;

                        //Own Ship Movement Penalty
                        TimeSpan timeStationary = DateTime.UtcNow - m_Ship.TimeLastMoved;
                        double secondsStationary = (double)timeStationary.TotalSeconds;

                        if (secondsStationary > BaseShip.CannonMovementAccuracyCooldown)
                            secondsStationary = BaseShip.CannonMovementAccuracyCooldown;

                        if (targetShip != null)
                        {
                            TimeSpan timeTargetStationary = DateTime.UtcNow - targetShip.TimeLastMoved;
                            double secondsOpponentStationary = (double)timeStationary.TotalSeconds;

                            if (secondsOpponentStationary > BaseShip.CannonMovementAccuracyCooldown)
                                secondsOpponentStationary = BaseShip.CannonMovementAccuracyCooldown;

                            opponentMovementPenalty = 1 - (BaseShip.CannonTargetMovementMaxAccuracyPenalty * (1 - (secondsOpponentStationary / BaseShip.CannonMovementAccuracyCooldown)));

                            //No Movement Penalty to Shoot a Ship That is in Reduced Speed Mode
                            if (targetShip.ReducedSpeedMode)
                                opponentMovementPenalty = 1;
                        }

                        movementAccuracyPenalty = 1 - (BaseShip.CannonMovementMaxAccuracyPenalty * (1 - (secondsStationary / BaseShip.CannonMovementAccuracyCooldown)));

                        double finalAccuracy = cannonAccuracy * movementAccuracyPenalty * opponentMovementPenalty;

                        double chance = Utility.RandomDouble();

                        bool hit = false;

                        //Hit Target
                        if (chance <= finalAccuracy)
                            hit = true;

                        Point3D cannonEndLocation = point;

                        if (IsNPCShip && targetShip != null)
                        {
                            if (canHitCenter)
                                cannonEndLocation = targetShip.GetRandomEmbarkLocation(true);

                            else if (canHitHold && canHitTillerman)
                            {
                                if (Utility.RandomDouble() < .5)
                                    cannonEndLocation = targetShip.Hold.Location;

                                else
                                    cannonEndLocation = targetShip.TillerMan.Location;
                            }

                            else if (canHitHold && !canHitTillerman)
                                cannonEndLocation = targetShip.Hold.Location;

                            else if (!canHitHold && canHitTillerman)
                                cannonEndLocation = targetShip.TillerMan.Location;
                        }

                        double delay = (BaseShip.CannonLoopDelay * (a + 1) / (double)firingLoops) * b;
                        
                        Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                        {
                            FireCannon(shipCannon, from, cannonEndLocation, map, hit, showSmoke);
                        });
                    }
                }
            }

            else
            {
                if (tooClose)
                    from.SendMessage("Your target is too close to the ship to be fired upon.");

                else
                    from.SendMessage("At least one of your cannons must be within range of and in line of sight of your target in order to fire a cannon volley.");
            }
        }

        public void FireCannon(ShipCannon shipCannon, Mobile from, Point3D targetLocation, Map map, bool hit, bool showSmoke)
        {
            if (shipCannon == null)
                return;

            int cannonballItemID = 0xE73;
            int cannonballHue = 0;
            int smokeHue = 0;

            bool fixedDirection = false;

            double shotDelay = .04;
            int shotSpeed = 6;

            Point3D smokeLocation = shipCannon.Location;

            switch (shipCannon.Facing)
            {
                case Direction.North: { } break;
                case Direction.East: { smokeLocation.X++; } break;
                case Direction.South: { smokeLocation.Y++; } break;
                case Direction.West: { smokeLocation.X--; } break;
            }

            if (m_Ship != null)
            {
                double gunsPercent = (double)((float)m_Ship.GunPoints / (float)m_Ship.MaxGunPoints);
                double misfireChance = BaseShip.CannonMaxMisfireChance * (1 - gunsPercent);

                double chance = Utility.RandomDouble();

                double distance = Utility.GetDistanceToSqrt(shipCannon.Location, targetLocation);
                double flatDistance = Utility.GetDistance(shipCannon.Location, targetLocation);

                //Misfire
                if (chance < misfireChance)
                {
                    List<Mobile> m_MobilesOnShip = m_Ship.GetMobilesOnShip(true, true);

                    foreach (Mobile mobile in m_MobilesOnShip)
                    {
                        if (m_Ship.IsOwner(mobile) || m_Ship.IsCoOwner(mobile) || m_Ship.IsFriend(mobile))
                            mobile.SendMessage("Misfire!");
                    }

                    Effects.SendLocationEffect(shipCannon.Location, map, 0x3735, 10);
                    Effects.PlaySound(shipCannon.Location, map, 0x475);

                    return;
                }

                if (m_Ship.MobileFactionType == MobileFactionType.Undead)
                {
                    cannonballItemID = Utility.RandomList(6880, 6881, 6882, 6883, 6884);
                    smokeHue = 2630;
                }

                //Hit
                if (hit)
                {
                    m_Ship.LastCombatTime = DateTime.UtcNow;

                    Effects.PlaySound(shipCannon.Location, map, 0x664);

                    if (showSmoke)
                        Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    SpellHelper.AdjustField(ref targetLocation, map, 12, false);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(shipCannon.Location.X, shipCannon.Location.Y, shipCannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);

                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);
                    double effectDelay = distance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(shipCannon, from, targetLocation, map, hit);
                    });
                }

                //Miss
                else
                {
                    int xOffset = 0;
                    int yOffset = 0;

                    double effectiveDistance = distance;

                    int distanceOffset = (int)(Math.Floor(effectiveDistance / 2));

                    if (distance >= 2)
                    {
                        xOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            xOffset *= -1;

                        yOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            yOffset *= -1;
                    }

                    Effects.PlaySound(shipCannon.Location, map, 0x664);
                    Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(shipCannon.Location.X, shipCannon.Location.Y, shipCannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z + 5), map);

                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);

                    Point3D splashLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, +targetLocation.Z);

                    double newDistance = from.GetDistanceToSqrt(splashLocation);
                    double effectDelay = newDistance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(shipCannon, from, splashLocation, map, hit);
                    });
                }
            }
        }

        public void ResolveCannon(ShipCannon shipCannon, Mobile from, Point3D targetLocation, Map map, bool hit)
        {
            if (hit)
                ResolveCannonHit(from, targetLocation);

            else
                Splash(targetLocation, map);
        }

        public void ResolveCannonHit(Mobile from, Point3D targetLocation)
        {
            ArrayList validTargets = new ArrayList();

            Map map = Map;

            BaseShip shipFrom = BaseShip.FindShipAt(from.Location, map);
            BaseShip targetShip = BaseShip.FindShipAt(targetLocation, map);

            bool hitObject = false;
            bool hitShip = false;
            bool showExplosion = true;

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(targetLocation, BaseShip.CannonExplosionRange);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (!validTargets.Contains(mobile))
                    validTargets.Add(mobile);
            }

            nearbyMobiles.Free();

            List<Mobile> m_MobilesOnSourceShip = new List<Mobile>();
            List<Mobile> m_Targets = new List<Mobile>();

            double baseCannonDamage = (double)(Utility.RandomMinMax(BaseShip.CannonDamageMin, BaseShip.CannonDamageMax));
            
            if (m_Ship == null)
                m_MobilesOnSourceShip.Add(from);

            else
            {
                baseCannonDamage = m_Ship.CannonDamageScalar * baseCannonDamage;

                m_MobilesOnSourceShip = m_Ship.GetMobilesOnShip(false, false);
            }

            bool targetLocationIsShip = false;

            if (targetShip != null)
            {
                targetLocationIsShip = true;
                m_Targets = targetShip.GetMobilesOnShip(false, false);

                validTargets.Add(targetShip);
            }

            else
                m_Targets = new List<Mobile>();

            double damageDealt;

            for (int a = 0; a < validTargets.Count; ++a)
            {
                damageDealt = baseCannonDamage;

                object target = validTargets[a];

                int d = 0;
                int damage = 0;

                bool largeCreatureHit = false;

                PlayerMobile pm_Target;
                BaseCreature bc_Target;

                //Large Boss-Size Creature Hit: Don't Deal Damage to Ship Underneath it
                if (target is Mobile)
                {
                    bc_Target = target as BaseCreature;

                    if (bc_Target != null)
                    {
                        if (bc_Target.IsChamp() || bc_Target.IsBoss() || bc_Target.IsLoHBoss() || bc_Target.IsEventBoss())
                            largeCreatureHit = true;
                    }
                }

                if (target is Mobile)
                {
                    Mobile mobile = target as Mobile;

                    pm_Target = mobile as PlayerMobile;
                    bc_Target = mobile as BaseCreature;

                    if (!mobile.Alive)
                        continue;

                    //Mobile is somehow on ship that cannon is shooting from
                    BaseShip mobileShip = BaseShip.FindShipAt(mobile.Location, mobile.Map);

                    if (m_Ship != null && mobileShip != null)
                    {
                        if (m_Ship == mobileShip)
                            continue;
                    }

                    hitObject = true;

                    bool dealDamage = true;
                    bool directHit = false;

                    if (mobile.InRange(targetLocation, 0))
                        directHit = true;

                    bool isOnWater = BaseShip.IsWaterTile(mobile.Location, mobile.Map);

                    if (from != null || (SpellHelper.ValidIndirectTarget(from, mobile) && from.CanBeHarmful(mobile, false)))
                    {
                        //Player
                        if (pm_Target != null)
                            damageDealt *= BaseShip.CannonPlayerDamageMultiplier;

                        //Creature
                        if (bc_Target != null)
                        {
                            if (bc_Target.IsOceanCreature)
                                damageDealt *= BaseShip.CannonOceanCreatureDamageMultiplier;

                            else
                                damageDealt *= BaseShip.CannonMobileDamageMultiplier;
                        }

                        if (!directHit)
                            damageDealt *= BaseShip.CannonIndirectHitDamageMultiplier;
                        
                        if (dealDamage)
                        {
                            from.DoHarmful(mobile);

                            int finalDamage = (int)Math.Round(damageDealt);

                            BaseCreature bc_Creature = mobile as BaseCreature;

                            if (bc_Creature != null)
                            {
                                bool willKill = false;

                                if (bc_Creature.Hits - finalDamage <= 0)
                                    willKill = true;

                                bc_Creature.OnGotCannonHit(finalDamage, from, willKill);
                            }

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, from, finalDamage, 100, 0, 0, 0, 0);                            
                        }
                    }
                }

                else if (target is DerelictCargo)
                {
                    DerelictCargo crate = target as DerelictCargo;
                    crate.TakeDamage(from, (int)damageDealt);
                }

                else if (target is BaseShip && !largeCreatureHit)
                {
                    BaseShip shipTarget = target as BaseShip;

                    if (from != null && m_Ship != null && shipTarget != null)
                    {
                        //Somehow Hitting Own Ship
                        if (m_Ship == shipTarget)
                            continue;

                        CannonDoHarmful(from, m_MobilesOnSourceShip, m_Targets);

                        hitObject = true;
                        hitShip = true;

                        bool dealDamage = true;

                        if (dealDamage)
                        {
                            DamageType damageType = shipTarget.GetDamageTypeByTargetingMode(m_Ship.TargetingMode);

                            int finalDamage = (int)(Math.Round(damageDealt));

                            shipTarget.ReceiveDamage(from, m_Ship, finalDamage, damageType);
                        }
                    }
                }
            }

            if (hitObject)
            {
                IEntity explosionLocationEntity = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z - 1), map);

                int explosionHue = 0;
                int explosionSound = 0x307;

                if (m_Ship.MobileFactionType == MobileFactionType.Undead)
                {
                    explosionHue = 2630;
                    explosionSound = 0x56E;
                }                

                if (showExplosion)
                {
                    Effects.SendLocationParticles(explosionLocationEntity, Utility.RandomList(14013, 14015, 14027, 14012), 30, 7, explosionHue, 0, 5044, 0);
                    Effects.PlaySound(explosionLocationEntity.Location, map, explosionSound);
                }
            }

            else
                Splash(targetLocation, map);
        }

        public void CannonDoHarmful(Mobile from, List<Mobile> m_ShipAllies, List<Mobile> m_ShipTargets)
        {
            //Whoever shoots the cannon from the source ship attacks everyone on target ship
            foreach (Mobile mobileTarget in m_ShipTargets)
            {
                if (from.CanBeHarmful(mobileTarget, false))
                    from.DoHarmful(mobileTarget);
            }

            //All mobiles on ship attack everyone on target ship
            foreach (Mobile shipAlly in m_ShipAllies)
            {
                if (shipAlly == from)
                    continue;

                //NPC Ship: Everyone Attacks Everyone on Target Ship
                if (m_Ship.MobileControlType != MobileControlType.Player)
                {
                    //Player Currently on an NPC Ship
                    if (shipAlly is PlayerMobile)
                        continue;

                    foreach (Mobile mobileTarget in m_ShipTargets)
                    {
                        if (shipAlly.CanBeHarmful(mobileTarget, false))
                            shipAlly.DoHarmful(mobileTarget);
                    }
                }

                //Player Ship: Everyone on Ship Makes Criminal Checks Against Target Ship Inhabitants
                else
                {
                    bool isCriminal = false;

                    foreach (Mobile mobileTarget in m_ShipTargets)
                    {
                        if (Notoriety.Compute(shipAlly, mobileTarget) == Notoriety.Innocent && mobileTarget.Alive)
                        {
                            isCriminal = true;
                            break;
                        }
                    }

                    if (isCriminal)
                        shipAlly.CriminalAction(false);
                }
            }
        }

        public static void Splash(Point3D point, Map map)
        {
            BaseShip shipCheck = BaseShip.FindShipAt(point, map);

            bool foundAnyItem = false;

            IPooledEnumerable itemsInRange = map.GetItemsInRange(point, 1);

            foreach (Item item in itemsInRange)
            {
                if (item != null)
                {
                    foundAnyItem = true;
                    break;
                }
            }

            itemsInRange.Free();

            //Ship in Location
            if (shipCheck != null)
                Effects.PlaySound(point, map, 0x148);

            //Water
            else if (BaseShip.IsWaterTile(point, map))
            {
                if (!foundAnyItem)
                    Effects.SendLocationEffect(point, map, 0x352D, 7);

                Effects.PlaySound(point, map, 0x027);
            }

            //Anything Else
            else
                Effects.PlaySound(point, map, 0x148);
        }  

        public override void OnDelete()
        {
            if (m_Ship != null)
            {
                if (m_Ship.m_Cannons.Contains(this))
                    m_Ship.m_Cannons.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_Ship);
            writer.Write((int)m_CannonType);
            writer.Write((int)m_CannonPosition);
            writer.Write(m_Ammunition);
            writer.Write(m_xOffset);
            writer.Write(m_yOffset);
            writer.Write(m_zOffset);
            writer.Write((int)m_Facing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Ship = (BaseShip)reader.ReadItem();
                m_CannonType = (CannonType)reader.ReadInt();
                m_CannonPosition = (CannonPosition)reader.ReadInt();
                m_Ammunition = reader.ReadInt();
                m_xOffset = reader.ReadInt();
                m_yOffset = reader.ReadInt();
                m_zOffset = reader.ReadInt();
                Facing = (Direction)reader.ReadInt();
            }

            //-----

            Movable = false;

            if (m_Ship != null)
            {
                m_Ship.m_Cannons.Add(this);

                switch (m_CannonPosition)
                {
                    case CannonPosition.Left: m_Ship.m_LeftCannons.Add(this); break;
                    case CannonPosition.Right: m_Ship.m_RightCannons.Add(this); break;
                    case CannonPosition.Front: m_Ship.m_FrontCannons.Add(this); break;
                    case CannonPosition.Rear: m_Ship.m_RearCannons.Add(this); break;
                }
            }
        }
    }    
}