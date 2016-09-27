using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Custom;

namespace Server.Items
{
    [FlipableAttribute(0x1EBA, 0x1EBB)]
    public class ShipRepairTool : Item
    {
        public enum DamageType
        {
            Hull,
            Sails,
            Guns
        }

        public int m_CurrentCharges;

        private Timer shipRepairTimer;

        [Constructable]
        public ShipRepairTool()
            : base(0x1EBA)
        {
            Weight = 5.0;
            m_CurrentCharges = 25;
        }

        public ShipRepairTool(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int CurrentCharges
        {
            get { return m_CurrentCharges; }
            set
            {
                m_CurrentCharges = value;

                if (m_CurrentCharges == 0)
                    Delete();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Durability: " + m_CurrentCharges.ToString());
            LabelTo(from, "a ship repair tool");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (RootParent != from)
            {
                from.SendMessage("This item must be in your pack in order to use it");
                return;
            }

            else if (!from.Alive)
            {
                from.SendMessage("You cannot use this while dead.");
                return;
            }

            if (!from.CanBeginAction(typeof(ShipRepairTool)))
            {
                from.SendMessage("You must wait a few moments before attempting to repair a ship again.");
                return;
            }

            from.Target = new ShipRepairTarget(this);
            from.SendMessage("Which ship do you wish to repair?");
            from.RevealingAction();

            base.OnDoubleClick(from);
        }

        private class ShipRepairTarget : Target
        {
            private ShipRepairTool m_ShipRepairTools;

            public ShipRepairTarget(ShipRepairTool b)
                : base(25, true, TargetFlags.None, false)
            {
                m_ShipRepairTools = b;
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

                    m_ShipRepairTools.OnTarget(from, new Point3D(p.X, p.Y, p.Z));
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point)
        {
            BaseShip ship = BaseShip.FindShipAt(point, this.Map);

            if (ship != null)
            {

                if (ship.m_ScuttleInProgress)
                {
                    from.SendMessage("You cannot repair a ship that is being scuttled.");
                    return;
                }

                if (!from.CanBeginAction(typeof(ShipRepairTool)))
                {
                    from.SendMessage("You must wait a few moments before attempting to use that again.");
                    return;
                }

                TimeSpan repairCooldownRequired = TimeSpan.FromSeconds(ship.m_ShipStatsProfile.RepairCooldownDuration);

                if (ship.LastCombatTime + ship.TimeNeededToBeOutOfCombat <= DateTime.UtcNow)
                    repairCooldownRequired = BaseShip.RepairDuration;

                if (ship.m_TimeLastRepaired + repairCooldownRequired > DateTime.UtcNow)
                {
                    string repairString = Utility.CreateTimeRemainingString(DateTime.UtcNow, ship.m_TimeLastRepaired + repairCooldownRequired, false, false, false, true, true);

                    from.SendMessage("This ship cannot be repaired for another " + repairString + ".");
                    return;
                }

                if (!(ship.Contains(from) || ship.GetShipToLocationDistance(ship, from.Location) <= 6))
                {
                    from.SendMessage("You are too far away from that ship to repair it.");

                    return;
                }

                if (ship.IsOwner(from) || ship.IsCoOwner(from))
                {
                    from.CloseGump(typeof(ShipRepairGump));
                    from.SendGump(new ShipRepairGump(ship, from, this));
                    from.SendMessage("What do you wish to repair on the ship?");
                }

                else
                    from.SendMessage("You must be the owner or co-owner of the ship in order to repair it.");
            }
        }

        public class ShipRepairGump : Gump
        {
            ShipRepairTool m_ShipRepairTools;
            BaseShip m_Ship;
            Mobile m_From;

            public ShipRepairGump(BaseShip ship, Mobile from, ShipRepairTool shipRepairTools)
                : base(50, 50)
            {
                m_ShipRepairTools = shipRepairTools;
                m_Ship = ship;
                m_From = from;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);

                AddBackground(0, 0, 210, 210, 5054);
                AddBackground(10, 10, 190, 190, 3000);

                int hullPercent = (int)(Math.Floor(100 * (float)m_Ship.HitPoints / (float)m_Ship.MaxHitPoints));
                int sailsPercent = (int)(Math.Floor(100 * (float)m_Ship.SailPoints / (float)m_Ship.MaxSailPoints));
                int gunsPercent = (int)(Math.Floor(100 * (float)m_Ship.GunPoints / (float)m_Ship.MaxGunPoints));

                //Hull
                AddItem(16, 22, 7132);
                AddButton(16, 22, 82, 82, 1, GumpButtonType.Reply, 0);
                AddHtml(60, 20, 140, 20, "Repair Hull", false, false);
                AddHtml(60, 40, 140, 20, m_Ship.HitPoints.ToString() + "/" + m_Ship.MaxHitPoints.ToString() + " (" + hullPercent.ToString() + "%)", false, false);

                //Sails
                AddItem(20, 82, 5984);
                AddButton(20, 82, 82, 82, 2, GumpButtonType.Reply, 0);
                AddHtml(60, 80, 140, 20, "Repair Sails", false, false);
                AddHtml(60, 100, 140, 20, m_Ship.SailPoints.ToString() + "/" + m_Ship.MaxSailPoints.ToString() + " (" + sailsPercent.ToString() + "%)", false, false);

                //Guns
                AddItem(18, 143, 7153);
                AddButton(18, 143, 82, 82, 3, GumpButtonType.Reply, 0);
                AddHtml(60, 140, 140, 20, "Repair Guns", false, false);
                AddHtml(60, 160, 140, 20, m_Ship.GunPoints.ToString() + "/" + m_Ship.MaxGunPoints.ToString() + " (" + gunsPercent.ToString() + "%)", false, false);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Ship.Deleted)
                    return;

                Mobile from = sender.Mobile;

                if (!from.CanBeginAction(typeof(ShipRepairTool)) && info.ButtonID >= 1 && info.ButtonID <= 3)
                {
                    from.SendMessage("You must wait a few moments before attempting to use that again.");
                    return;
                }

                switch (info.ButtonID)
                {
                    case 1: //Repair Hull
                        {
                            if (m_ShipRepairTools.IsDamaged(m_Ship, m_From, DamageType.Hull) == false)
                            {
                                from.SendMessage("The ship's hull is not damaged.");
                                return;
                            }

                            else if (m_ShipRepairTools.CheckRepairMaterials(m_Ship, m_From, DamageType.Hull))
                            {
                                from.SendMessage("You begin repairing the ship's hull.");
                                m_ShipRepairTools.RepairStart(m_Ship, from, DamageType.Hull);
                                return;
                            }

                            else
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }

                    case 2: //Repair Sails
                        {
                            if (m_ShipRepairTools.IsDamaged(m_Ship, m_From, DamageType.Sails) == false)
                            {
                                from.SendMessage("The ship's sails are not damaged.");
                                return;
                            }

                            else if (m_ShipRepairTools.CheckRepairMaterials(m_Ship, m_From, DamageType.Sails))
                            {
                                from.SendMessage("You begin repairing the ship's sails.");
                                m_ShipRepairTools.RepairStart(m_Ship, from, DamageType.Sails);
                                return;
                            }

                            else
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }

                    case 3: //Repair Guns
                        {
                            if (m_ShipRepairTools.IsDamaged(m_Ship, m_From, DamageType.Guns) == false)
                            {
                                from.SendMessage("The ship's guns are not damaged.");
                                return;
                            }

                            else if (m_ShipRepairTools.CheckRepairMaterials(m_Ship, m_From, DamageType.Guns))
                            {
                                from.SendMessage("You begin repairing the ship's guns.");
                                m_ShipRepairTools.RepairStart(m_Ship, from, DamageType.Guns);

                                return;
                            }

                            else
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }
                }
            }
        }

        public bool IsDamaged(BaseShip ship, Mobile from, DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Hull:
                    {
                        if (ship.HitPoints < ship.MaxHitPoints)
                            return true;
                        break;
                    }

                case DamageType.Sails:
                    {
                        if (ship.SailPoints < ship.MaxSailPoints)
                            return true;
                        break;
                    }

                case DamageType.Guns:
                    {
                        if (ship.GunPoints < ship.MaxGunPoints)
                            return true;
                        break;
                    }
            }

            return false;
        }

        public bool CheckRepairMaterials(BaseShip ship, Mobile from, DamageType damageType)
        {
            if (ship == null)
                return false;

            if (from == null)
                return false;

            if (damageType == null)
                return false;

            switch (damageType)
            {
                case DamageType.Hull:
                    {
                        int totalBoards = 0;
                        int boardsNeeded = (int)((double)ship.MaxHitPoints * BaseShip.HullRepairPercent * BaseShip.RepairMaterialFactor);

                        Item[] playerBoards = from.Backpack.FindItemsByType(typeof(BaseResourceBoard));
                        Item[] holdBoards = ship.Hold.FindItemsByType(typeof(BaseResourceBoard));

                        foreach (Item item in playerBoards)
                        {
                            BaseResourceBoard board = item as BaseResourceBoard;
                            totalBoards += board.Amount;
                        }

                        foreach (Item item in holdBoards)
                        {
                            BaseResourceBoard board = item as BaseResourceBoard;
                            totalBoards += board.Amount;
                        }

                        if (totalBoards > boardsNeeded)
                            return true;

                        break;
                    }

                case DamageType.Sails:
                    {
                        int totalCloth = 0;
                        int clothNeeded = (int)((double)ship.MaxHitPoints * BaseShip.SailRepairPercent * BaseShip.RepairMaterialFactor);

                        Item[] playerCloth = from.Backpack.FindItemsByType(typeof(Cloth));
                        Item[] holdCloth = ship.Hold.FindItemsByType(typeof(Cloth));

                        foreach (Item item in playerCloth)
                        {
                            Cloth cloth = item as Cloth;
                            totalCloth += cloth.Amount;
                        }

                        foreach (Item item in holdCloth)
                        {
                            Cloth cloth = item as Cloth;
                            totalCloth += cloth.Amount;
                        }

                        if (totalCloth > clothNeeded)
                            return true;

                        break;
                    }

                case DamageType.Guns:
                    {
                        int totalIronIngots = 0;
                        int ironIngotsNeeded = (int)((double)ship.MaxHitPoints * BaseShip.GunRepairPercent * BaseShip.RepairMaterialFactor);

                        Item[] playerIronIngots = from.Backpack.FindItemsByType(typeof(IronIngot));
                        Item[] holdIronIngots = ship.Hold.FindItemsByType(typeof(IronIngot));

                        foreach (Item item in playerIronIngots)
                        {
                            IronIngot ironIngot = item as IronIngot;
                            totalIronIngots += ironIngot.Amount;
                        }

                        foreach (Item item in holdIronIngots)
                        {
                            IronIngot ironIngot = item as IronIngot;
                            totalIronIngots += ironIngot.Amount;
                        }

                        if (totalIronIngots > ironIngotsNeeded)
                            return true;

                        break;
                    }
            }

            return false;
        }

        public void UseRepairMaterials(BaseShip ship, Mobile from, DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Hull:
                    {
                        int boardsUsed = 0;
                        int boardsNeeded = 0;
                        int repairAmount = ship.MaxHitPoints - ship.HitPoints;
                        int maximumRepairAmount = (int)((double)ship.MaxHitPoints * BaseShip.HullRepairPercent);

                        if (repairAmount > maximumRepairAmount)
                            repairAmount = maximumRepairAmount;

                        boardsNeeded = (int)((double)repairAmount * BaseShip.RepairMaterialFactor);

                        if (boardsNeeded < 0)
                            boardsNeeded = 1;

                        Item[] playerBoards = from.Backpack.FindItemsByType(typeof(BaseResourceBoard));
                        Item[] holdBoards = ship.Hold.FindItemsByType(typeof(BaseResourceBoard));

                        foreach (Item item in playerBoards)
                        {
                            if (boardsNeeded <= 0)
                                return;

                            BaseResourceBoard board = item as BaseResourceBoard;

                            if (board.Amount > boardsNeeded)
                            {
                                board.Amount -= boardsNeeded;
                                boardsNeeded = 0;
                            }

                            else
                            {
                                boardsNeeded -= board.Amount;
                                board.Delete();
                            }
                        }

                        foreach (Item item in holdBoards)
                        {
                            if (boardsNeeded <= 0)
                                return;

                            BaseResourceBoard board = item as BaseResourceBoard;

                            if (board.Amount > boardsNeeded)
                            {
                                board.Amount -= boardsNeeded;
                                boardsNeeded = 0;
                            }

                            else
                            {
                                boardsNeeded -= board.Amount;
                                board.Delete();
                            }
                        }

                        break;
                    }

                case DamageType.Sails:
                    {
                        int clothUsed = 0;
                        int clothNeeded = 0;
                        int repairAmount = ship.MaxSailPoints - ship.SailPoints;
                        int maximumRepairAmount = (int)((double)ship.MaxSailPoints * BaseShip.HullRepairPercent);

                        if (repairAmount > maximumRepairAmount)
                            repairAmount = maximumRepairAmount;

                        clothNeeded = (int)((double)repairAmount * BaseShip.RepairMaterialFactor);

                        if (clothNeeded < 0)
                            clothNeeded = 1;

                        Item[] playerCloth = from.Backpack.FindItemsByType(typeof(Cloth));
                        Item[] holdCloth = ship.Hold.FindItemsByType(typeof(Cloth));

                        foreach (Item item in playerCloth)
                        {
                            if (clothNeeded <= 0)
                                return;

                            Cloth cloth = item as Cloth;

                            if (cloth.Amount > clothNeeded)
                            {
                                cloth.Amount -= clothNeeded;
                                clothNeeded = 0;
                            }

                            else
                            {
                                clothNeeded -= cloth.Amount;
                                cloth.Delete();
                            }
                        }

                        foreach (Item item in holdCloth)
                        {
                            if (clothNeeded <= 0)
                                return;

                            Cloth cloth = item as Cloth;

                            if (cloth.Amount > clothNeeded)
                            {
                                cloth.Amount -= clothNeeded;
                                clothNeeded = 0;
                            }

                            else
                            {
                                clothNeeded -= cloth.Amount;
                                cloth.Delete();
                            }
                        }

                        break;
                    }


                case DamageType.Guns:
                    {
                        int ironIngotsUsed = 0;
                        int ironIngotsNeeded = 0;
                        int repairAmount = ship.MaxGunPoints - ship.GunPoints;
                        int maximumRepairAmount = (int)((double)ship.MaxGunPoints * BaseShip.HullRepairPercent);

                        if (repairAmount > maximumRepairAmount)
                            repairAmount = maximumRepairAmount;

                        ironIngotsNeeded = (int)((double)repairAmount * BaseShip.RepairMaterialFactor);

                        if (ironIngotsNeeded < 0)
                            ironIngotsNeeded = 1;

                        Item[] playerIronIngots = from.Backpack.FindItemsByType(typeof(IronIngot));
                        Item[] holdIronIngots = ship.Hold.FindItemsByType(typeof(IronIngot));

                        foreach (Item item in playerIronIngots)
                        {
                            if (ironIngotsNeeded <= 0)
                                return;

                            IronIngot ironIngot = item as IronIngot;

                            if (ironIngot.Amount > ironIngotsNeeded)
                            {
                                ironIngot.Amount -= ironIngotsNeeded;
                                ironIngotsNeeded = 0;
                            }

                            else
                            {
                                ironIngotsNeeded -= ironIngot.Amount;
                                ironIngot.Delete();
                            }
                        }

                        foreach (Item item in holdIronIngots)
                        {
                            if (ironIngotsNeeded <= 0)
                                return;

                            IronIngot ironIngot = item as IronIngot;

                            if (ironIngot.Amount > ironIngotsNeeded)
                            {
                                ironIngot.Amount -= ironIngotsNeeded;
                                ironIngotsNeeded = 0;
                            }

                            else
                            {
                                ironIngotsNeeded -= ironIngot.Amount;
                                ironIngot.Delete();
                            }
                        }

                        break;
                    }
            }
        }

        private void RepairStart(BaseShip ship, Mobile from, DamageType damageType)
        {
            if (ship == null || from == null)
                return;

            from.BeginAction(typeof(ShipRepairTool));
            Timer.DelayCall(BaseShip.RepairDuration, delegate { from.EndAction(typeof(ShipRepairTool)); });

            Effects.PlaySound(from.Location, from.Map, 0x23D);

            if (!from.Mounted && from.Body.IsHuman)
                from.Animate(11, 5, 1, true, false, 0);

            shipRepairTimer = new ShipRepairTimer(ship, from, this, damageType, BaseShip.RepairInterval);
            shipRepairTimer.Start();

            //Henchman Repair Assistance
            List<Mobile> m_MobilesOnShip = ship.GetMobilesOnShip(false, false);

            foreach (Mobile mobile in m_MobilesOnShip)
            {
                if (mobile is HenchmanNavyCarpenter)
                {
                    HenchmanNavyCarpenter navyCarpenter = mobile as HenchmanNavyCarpenter;

                    mobile.Say("Assisting with ship repair!");

                    navyCarpenter.AssistRepair();
                }

                if (mobile is HenchmanPirateCarpenter)
                {
                    HenchmanPirateCarpenter pirateCarpenter = mobile as HenchmanPirateCarpenter;

                    mobile.Say("Assisting with ship repair!");

                    pirateCarpenter.AssistRepair();
                }
            }
        }

        private class ShipRepairTimer : Timer
        {
            private BaseShip m_Ship;
            private Mobile m_From;
            private ShipRepairTool m_ShipRepairTools;
            private DamageType m_DamageType;

            private DateTime m_Start;

            public ShipRepairTimer(BaseShip ship, Mobile from, ShipRepairTool shipRepairTools, DamageType damageType, TimeSpan interval)
                : base(interval, interval)
            {
                m_Ship = ship;
                m_From = from;
                m_ShipRepairTools = shipRepairTools;
                m_DamageType = damageType;

                m_Start = DateTime.UtcNow;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                //No Longer Alive
                if (!m_From.Alive)
                {
                    this.Stop();

                    return;
                }

                //No Longer On Ship or Close Enough to the Ship to Repair
                if (!(m_Ship.Contains(m_From) || m_Ship.GetShipToLocationDistance(m_Ship, m_From.Location) <= 6))
                {
                    m_From.SendMessage("You are not close enough to the ship to finish your repairs.");

                    this.Stop();
                    return;
                }

                m_From.RevealingAction();

                //Repair Time Remains
                if ((m_Start + BaseShip.RepairDuration) > DateTime.UtcNow)
                {
                    Effects.PlaySound(m_From.Location, m_From.Map, 0x23D);

                    if (!m_From.Mounted)
                        m_From.Animate(11, 5, 1, true, false, 0);
                }

                //Repairs Complete
                else
                {
                    Effects.PlaySound(m_From.Location, m_From.Map, 0x23D);

                    if (!m_From.Mounted)
                        m_From.Animate(11, 5, 1, true, false, 0);

                    Stop();

                    m_ShipRepairTools.FinishRepairs(m_Ship, m_From, m_DamageType, true);
                }

                //Henchman Repair Assistance
                List<Mobile> m_MobilesOnShip = m_Ship.GetMobilesOnShip(false, false);

                foreach (Mobile mobile in m_MobilesOnShip)
                {
                    if (mobile is HenchmanNavyCarpenter)
                    {
                        HenchmanNavyCarpenter navyCarpenter = mobile as HenchmanNavyCarpenter;
                        navyCarpenter.AssistRepair();
                    }

                    if (mobile is HenchmanPirateCarpenter)
                    {
                        HenchmanPirateCarpenter pirateCarpenter = mobile as HenchmanPirateCarpenter;
                        pirateCarpenter.AssistRepair();
                    }
                }
            }
        }

        private void FinishRepairs(BaseShip ship, Mobile from, DamageType damageType, bool needMaterials)
        {
            if (ship == null || from == null)
                return;

            TimeSpan repairCooldownRequired = TimeSpan.FromSeconds(ship.m_ShipStatsProfile.RepairCooldownDuration);

            if (ship.LastCombatTime + ship.TimeNeededToBeOutOfCombat <= DateTime.UtcNow)
                repairCooldownRequired = BaseShip.RepairDuration;

            //Repair Timer Refreshed (Other player repaired before this player finished)
            if (ship.m_TimeLastRepaired + repairCooldownRequired > DateTime.UtcNow)
            {
                from.SendMessage("You finish your repairs, however someone has more recently completed repairs on the ship.");
                return;
            }

            if (needMaterials == true)
            {
                switch (damageType)
                {
                    case DamageType.Hull:
                        {
                            if (IsDamaged(ship, from, DamageType.Hull) == false)
                            {
                                from.SendMessage("The ship's hull is no longer damaged.");
                                return;
                            }

                            else if (!CheckRepairMaterials(ship, from, DamageType.Hull))
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }

                    case DamageType.Sails:
                        {
                            if (IsDamaged(ship, from, DamageType.Sails) == false)
                            {
                                from.SendMessage("The ship's sails are no longer damaged.");
                                return;
                            }

                            else if (!CheckRepairMaterials(ship, from, DamageType.Sails))
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }

                    case DamageType.Guns:
                        {
                            if (IsDamaged(ship, from, DamageType.Guns) == false)
                            {
                                from.SendMessage("The ship's guns are no longer damaged.");
                                return;
                            }

                            else if (!CheckRepairMaterials(ship, from, DamageType.Guns))
                            {
                                from.SendMessage("You lack the materials needed for repairs.");
                                return;
                            }

                            break;
                        }
                }
            }

            int repairAmount = 0;
            double skillPercentBonus = 0;

            bool doubleTimeActive = false;
            double doubleTimeBonus = 0;

            int repairAssistants = 0;
            double bonusPerRepairAssistant = .25;

            //Henchman Repair Assistance           
            List<Mobile> m_MobilesOnShip = ship.GetMobilesOnShip(false, false);

            foreach (Mobile mobile in m_MobilesOnShip)
            {
                if (mobile is HenchmanNavyCarpenter)
                    repairAssistants++;

                if (mobile is HenchmanPirateCarpenter)
                    repairAssistants++;
            }

            double repairBonusScalar = 1.0;

            switch (damageType)
            {
                case DamageType.Hull:
                    {
                        skillPercentBonus = ((from.Skills[SkillName.Carpentry].Value / 100) * BaseShip.SkillBonusPercent / 2);

                        repairAmount = (int)((double)ship.MaxHitPoints * (BaseShip.HullRepairPercent + skillPercentBonus) * (1 + doubleTimeBonus + ((double)repairAssistants * bonusPerRepairAssistant)));
                        ship.HitPoints += repairAmount;

                        if (needMaterials)
                            UseRepairMaterials(ship, from, DamageType.Hull);

                        if (ship.TillerMan != null)
                            ship.TillerMan.Say("Ship's hull repaired!!");

                        from.SendMessage("You repair the ship's hull.");
                    }
                    break;

                case DamageType.Sails:
                    {
                        skillPercentBonus += ((from.Skills[SkillName.Tailoring].Value / 100) * BaseShip.SkillBonusPercent);

                        repairAmount = (int)((double)ship.MaxSailPoints * (BaseShip.SailRepairPercent + skillPercentBonus) * (1 + ((double)repairAssistants * bonusPerRepairAssistant)));
                        ship.SailPoints += repairAmount;

                        if (needMaterials)
                            UseRepairMaterials(ship, from, DamageType.Sails);

                        if (ship.TillerMan != null)
                            ship.TillerMan.Say("Ship's sails repaired!");

                        from.SendMessage("You repair the ship's sails.");
                    }
                    break;

                case DamageType.Guns:
                    {
                        skillPercentBonus += ((from.Skills[SkillName.Blacksmith].Value / 100) * BaseShip.SkillBonusPercent);

                        repairAmount = (int)((double)ship.MaxGunPoints * (BaseShip.GunRepairPercent + skillPercentBonus) * (1 + ((double)repairAssistants * bonusPerRepairAssistant)));
                        ship.GunPoints += repairAmount;

                        if (needMaterials)
                            UseRepairMaterials(ship, from, DamageType.Guns);

                        if (ship.TillerMan != null)
                            ship.TillerMan.Say("Ship's guns repaired!");

                        from.SendMessage("You repair the ship's guns.");
                    }
                    break;
            }

            if (from.AccessLevel == AccessLevel.Player)
                ship.m_TimeLastRepaired = DateTime.UtcNow;

            CurrentCharges--;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write((int)m_CurrentCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version
            if (version >= 0)
            {
                m_CurrentCharges = reader.ReadInt();
            }
        }
    }
}