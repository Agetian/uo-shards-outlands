using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public class ShipHotbarGump : Gump
    {
        public enum DisplayModeType
        {
            Stats,
            StatsAbilities,
            StatsAbilitiesNavigator,
            StatsNavigator,
            Navigator
        }

        public enum MovementMode
        {
            Single,
            Full
        }

        public enum ShipAction
        {
            AddCoOwner,
            AddFriend,
            ClearDeck,       
            Disembark,
            DisembarkFollowers,
            Dock,
            Embark,
            EmbarkFollowers,
            LowerAnchor,              
            RaiseAnchor,
            ThrowTargetOverboard
        }        

        public PlayerMobile m_Player;
        public ShipHotbarGumpObject m_ShipHotbarGumpObject;

        public ShipHotbarGump(PlayerMobile player, ShipHotbarGumpObject shipHotbarGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipHotbarGumpObject = shipHotbarGumpObject;

            if (player == null) return;
            if (m_ShipHotbarGumpObject == null) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int WhiteTextHue = 2499; //2036

            //-----
            
            BaseShip m_Ship = m_Player.ShipOccupied;

            if (m_Ship != null)
            {
                if (m_Ship.Deleted)
                    m_Ship = null;
            }

            #region Ship Property Values

            string shipNameText = "";

            int hullPoints = -1;
            int maxHullPoints = -1;
            double hullPercent = 0;

            int sailPoints = -1;
            int maxSailPoints = -1;
            double sailPercent = 0;

            int gunPoints = -1;
            int maxGunPoints = -1;
            double gunPercent = 0;

            string minorAbilityText = "";
            string minorAbilityTimeRemaining = "";
            bool minorAbilityReady = false;

            string majorAbilityText = "";
            string majorAbilityTimeRemaining = "";
            bool majorAbilityReady = false;

            string epicAbilityText = "";
            string epicAbilityTimeRemaining = "";
            bool epicAbilityReady = false;

            string movementModeText = "";
            string actionText = "";
            int actionButtonID = 4029;
            int actionButtonPressedID = 4029;

            string targetingModeText = "";

            int leftCannonsAmmunition = -1;
            bool leftCannonsReady = false;

            int rightCannonsAmmunition = -1;
            bool rightCannonsReady = false;
            
            #endregion

            #region Populate Values

            switch (m_ShipHotbarGumpObject.m_MovementMode)
            {
                case MovementMode.Single: movementModeText = "Single"; break;
                case MovementMode.Full: movementModeText = "Full"; break;
            } 

            if (m_Ship != null)
            {
                shipNameText = "The Rebellion"; //TEST: OK

                hullPoints = m_Ship.HitPoints;
                maxHullPoints = m_Ship.MaxHitPoints;
                hullPercent = (double)hullPoints / (double)maxHullPoints;

                sailPoints = m_Ship.SailPoints;
                maxSailPoints = m_Ship.MaxSailPoints;
                sailPercent = (double)sailPoints / (double)maxSailPoints;

                gunPoints = m_Ship.GunPoints;
                maxGunPoints = m_Ship.MaxGunPoints;
                gunPercent = (double)gunPoints / (double)maxGunPoints;

                leftCannonsAmmunition = 10; //TEST: FIX
                leftCannonsReady = true;

                rightCannonsAmmunition = 10; //TEST: FIX
                rightCannonsReady = false;

                minorAbilityText = "Expedite Repairs"; //TEST: FIX
                minorAbilityTimeRemaining = "1m 59s"; //TEST: FIX
                minorAbilityReady = false;

                majorAbilityText = "Smokescreen"; //TEST: FIX
                majorAbilityTimeRemaining = "4m 59s"; //TEST: FIX
                majorAbilityReady = false;

                epicAbilityText = "Hellfire"; //TEST: FIX
                epicAbilityTimeRemaining = "9m 59s"; //TEST: FIX
                epicAbilityReady = false;

                switch (m_Ship.TargetingMode)
                {
                    case TargetingMode.Random: targetingModeText = "Random"; break;
                    case TargetingMode.Hull: targetingModeText = "Hull"; break;
                    case TargetingMode.Sails: targetingModeText = "Sails"; break;
                    case TargetingMode.Guns: targetingModeText = "Guns"; break;
                }
            }                                

            switch (m_ShipHotbarGumpObject.m_ShipAction)
            {
                case ShipAction.RaiseAnchor:
                    actionText = "Raise Anchor";
                    actionButtonID = 4014;
                    actionButtonPressedID = 4016;
                break;

                case ShipAction.LowerAnchor:
                    actionText = "Lower Anchor";
                    actionButtonID = 4005;
                    actionButtonPressedID = 4007;
                break;

                case ShipAction.Embark:
                    actionText = "Embark";
                    actionButtonID = 4002;
                    actionButtonPressedID = 4004;
                break;

                case ShipAction.EmbarkFollowers:
                    actionText = "Embark Followers";
                    actionButtonID = 4008;
                    actionButtonPressedID = 4010;
                break;

                case ShipAction.Disembark:
                    actionText = "Disembark";
                    actionButtonID = 4002;
                    actionButtonPressedID = 4004;
                break;

                case ShipAction.DisembarkFollowers:
                    actionText = "Disembark Followers";
                    actionButtonID = 4008;
                    actionButtonPressedID = 4010;
                break;

                case ShipAction.Dock:
                    actionText = "Dock The Ship";
                    actionButtonID = 4017;
                    actionButtonPressedID = 4019;
                break;

                case ShipAction.ClearDeck:
                    actionText = "Clear The Deck";
                    actionButtonID = 4020;
                    actionButtonPressedID = 4022;
                break; 

                case ShipAction.AddFriend:
                    actionText = "Add Friend";
                    actionButtonID = 4003;
                    actionButtonPressedID = 4002;
                break;

                case ShipAction.AddCoOwner:
                    actionText = "Add Co-Owner";
                    actionButtonID = 4003;
                    actionButtonPressedID = 4002;
                break;

                case ShipAction.ThrowTargetOverboard:
                    actionText = "Throw Target Overboard";
                    actionButtonID = 4014;
                    actionButtonPressedID = 4016;
                break;
            }

            #endregion

            #region Gump Areas

            bool showStats = false;
            bool showAbilities = false;
            bool showNavigator = false;

            int startX = 0;
            int startY = 12;

            int statsHeight = 102;
            int abilitiesHeight = 55;

            switch (m_ShipHotbarGumpObject.m_DisplayMode)
            {
                case DisplayModeType.Stats:
                    showStats = true;
                break;

                case DisplayModeType.StatsAbilities:
                    showStats = true;
                    showAbilities = true;
                break;

                case DisplayModeType.StatsAbilitiesNavigator:
                    showStats = true;
                    showAbilities = true;
                    showNavigator = true;
                break;

                case DisplayModeType.StatsNavigator:
                    showStats = true;
                    showNavigator = true;
                break;

                case DisplayModeType.Navigator:               
                    showNavigator = true;
                    startX = 18;
                break;
            }

            #endregion

            if (m_ShipHotbarGumpObject.m_CollapseMode)
            {
                AddAlphaRegion(12, 12, 30, 75);

                AddButton(17, 12, 9906, 9906, 24, GumpButtonType.Reply, 0); //Collapse
                AddItem(6, 39, 5363);
                AddButton(20, 69, 1210, 1209, 27, GumpButtonType.Reply, 0); //Ship Selection
            }

            else
            {
                AddButton(17, 12, 9900, 9900, 24, GumpButtonType.Reply, 0); //Collapse
                AddItem(6, 39, 5363);
                AddButton(20, 69, 1210, 1209, 27, GumpButtonType.Reply, 0); //Ship Selection
                AddButton(20, 93, 2224, 2224, 26, GumpButtonType.Reply, 0); //Change Display Mode
                
                #region Stats 

                if (showStats)
                {
                    //Add Background
                    AddImage(48, startY, 103);
                    AddImage(185, startY, 103);
                    AddImageTiled(62, startY + 14, 257, 77, 2624);

                    //Add Header: shipNameText
                    AddImage(52, startY + 3, 1141);
                    AddLabel(Utility.CenteredTextOffset(190, shipNameText), startY + 5, 149, shipNameText);

                    AddLabel(71, startY + 30, 149, "Hull");
                    AddImage(102, startY + 35, 2057);
                    AddImageTiled(102 + Utility.ProgressBarX(hullPercent), startY + 38, Utility.ProgressBarWidth(hullPercent), 7, 2488);
                    if (maxHullPoints > -1)
                        AddLabel(219, startY + 30, WhiteTextHue, hullPoints.ToString() + "/" + maxHullPoints.ToString());

                    AddLabel(66, startY + 50, 187, "Sails");
                    AddImage(102, startY + 55, 2054);
                    AddImageTiled(102 + Utility.ProgressBarX(sailPercent), startY + 58, Utility.ProgressBarWidth(sailPercent), 7, 2488);
                    if (maxSailPoints > -1)
                        AddLabel(219, startY + 50, WhiteTextHue, sailPoints.ToString() + "/" + maxSailPoints.ToString());

                    AddLabel(66, startY + 70, WhiteTextHue, "Guns");
                    AddImage(102, startY + 75, 2057, 2499);
                    AddImageTiled(102 + Utility.ProgressBarX(gunPercent), startY + 78, Utility.ProgressBarWidth(gunPercent), 7, 2488);
                    if (maxGunPoints > -1)
                        AddLabel(219, startY + 70, WhiteTextHue, gunPoints.ToString() + "/" + maxGunPoints.ToString());


                    startY += statsHeight;                   
                }

                #endregion

                #region Abilities
                
                if (showAbilities)
                {
                    if (minorAbilityText != "")
                    {
                        if (minorAbilityReady)
                        {
                            AddLabel(Utility.CenteredTextOffset(61, minorAbilityText), startY, 2599, minorAbilityText);
                            AddButton(44, startY + 21, 2151, 2154, 21, GumpButtonType.Reply, 0);
                            AddLabel(76, startY + 25, WhiteTextHue, "Ready");
                        }

                        else
                        {
                            AddLabel(Utility.CenteredTextOffset(61, minorAbilityText), startY, 2599, minorAbilityText);
                            AddButton(44, startY + 21, 9721, 9721, 21, GumpButtonType.Reply, 0);
                            AddLabel(76, startY + 25, WhiteTextHue, minorAbilityTimeRemaining);
                        }
                    }

                    if (majorAbilityText != "")
                    {
                        if (majorAbilityReady)
                        {
                            AddLabel(Utility.CenteredTextOffset(185, majorAbilityText), startY, 2603, majorAbilityText);
                            AddButton(168, startY + 21, 2151, 2154, 22, GumpButtonType.Reply, 0);
                            AddLabel(201, startY + 25, WhiteTextHue, "Ready");
                        }

                        else
                        {
                            AddLabel(Utility.CenteredTextOffset(185, majorAbilityText), startY, 2603, majorAbilityText);
                            AddButton(168, startY + 21, 9721, 9721, 22, GumpButtonType.Reply, 0);
                            AddLabel(201, startY + 25, WhiteTextHue, majorAbilityTimeRemaining);
                        }
                    }

                    if (epicAbilityText != "")
                    {
                        if (epicAbilityReady)
                        {
                            AddLabel(Utility.CenteredTextOffset(305, epicAbilityText), startY, 2606, epicAbilityText);
                            AddButton(287, startY + 21, 2151, 2154, 23, GumpButtonType.Reply, 0);
                            AddLabel(321, startY + 25, WhiteTextHue, "Ready");
                        }

                        else
                        {
                            AddLabel(Utility.CenteredTextOffset(305, epicAbilityText), startY, 2606, epicAbilityText);
                            AddButton(287, startY + 21, 9721, 9721, 23, GumpButtonType.Reply, 0);
                            AddLabel(321, startY + 25, WhiteTextHue, epicAbilityTimeRemaining);
                        }
                    }

                    startY += abilitiesHeight;
                }

                #endregion

                #region Navigator                

                if (showNavigator)
                {
                    //Background
                    AddImage(startX + 33, startY + 50, 103);
                    AddImage(startX + 120, startY + 50, 103);
                    AddImage(startX + 196, startY + 50, 103);

                    AddImage(startX + 33, startY + 136, 103);
                    AddImage(startX + 197, startY + 141, 103);

                    AddImage(startX + 33, startY + 160, 103);
                    AddImage(startX + 175, startY + 160, 103);
                    AddImage(startX + 197, startY + 160, 103);

                    AddImageTiled(startX + 46, startY + 61, 280, 190, 2624);

                    //Movement Mode
                    AddLabel(startX + 63, startY + 0, 187, "Movement Mode");
                    AddLabel(Utility.CenteredTextOffset(startX + 115, movementModeText), startY + 20, WhiteTextHue, movementModeText);
                    AddButton(startX + 60, startY + 25, 2223, 2223, 12, GumpButtonType.Reply, 0);
                    AddButton(startX + 140, startY + 25, 2224, 2224, 13, GumpButtonType.Reply, 0);
                    
                    //Action
                    AddLabel(Utility.CenteredTextOffset(startX + 263, actionText), startY + 0, 63, actionText);
                    AddButton(startX + 215, startY + 25, 2223, 2223, 14, GumpButtonType.Reply, 0);
                    AddButton(startX + 285, startY + 25, 2224, 2224, 15, GumpButtonType.Reply, 0);
                    AddButton(startX + 246, startY + 20, actionButtonID, actionButtonPressedID, 16, GumpButtonType.Reply, 0);
                    
                    //Left Cannon
                    AddItem(startX + 33, startY + 46, 733);
                    AddItem(startX + 88, startY + 92, 3700);
                    
                    if (leftCannonsAmmunition > -1)
                    {
                        if (leftCannonsReady && leftCannonsAmmunition > 0)
                        {
                            AddButton(startX + 59, startY + 113, 2151, 2154, 17, GumpButtonType.Reply, 0);
                            AddLabel(startX + 96, startY + 74, WhiteTextHue, leftCannonsAmmunition.ToString());  
                        }

                        else if (!leftCannonsReady && leftCannonsAmmunition > 0)
                        {
                            AddButton(startX + 59, startY + 113, 9721, 9721, 17, GumpButtonType.Reply, 0);
                            AddLabel(startX + 96, startY + 74, WhiteTextHue, leftCannonsAmmunition.ToString());  
                        }

                        else 
                        {
                            AddButton(startX + 59, startY + 113, 2472, 2474, 17, GumpButtonType.Reply, 0);
                            AddLabel(startX + 96, startY + 74, 2115, leftCannonsAmmunition.ToString());  
                        }                                                 
                    }

                    //Right Cannon                   
                    AddItem(startX + 284, startY + 49, 709);
                    AddItem(startX + 255, startY + 92, 3700);

                    if (rightCannonsAmmunition > -1)
                    {
                        if (rightCannonsReady && rightCannonsAmmunition > 0)
                        {
                            AddButton(startX + 287, startY + 115, 2151, 2154, 18, GumpButtonType.Reply, 0);
                            AddLabel(startX + 263, startY + 74, WhiteTextHue, rightCannonsAmmunition.ToString());
                        }

                        else if (!rightCannonsReady && rightCannonsAmmunition > 0)
                        {
                            AddButton(startX + 287, startY + 115, 9721, 9721, 18, GumpButtonType.Reply, 0);
                            AddLabel(startX + 263, startY + 74, WhiteTextHue, rightCannonsAmmunition.ToString());
                        }

                        else
                        {
                            AddButton(startX + 287, startY + 115, 2472, 2474, 18, GumpButtonType.Reply, 0);
                            AddLabel(startX + 263, startY + 74, 2115, rightCannonsAmmunition.ToString());
                        }
                    }

                    //Targeting Mode
                    if (targetingModeText != "")
                    {
                        AddLabel(startX + 136, startY + 65, 2115, "Targeting Mode");
                        AddLabel(Utility.CenteredTextOffset(startX + 186, targetingModeText), startY + 85, WhiteTextHue, targetingModeText);

                        AddButton(startX + 127, startY + 88, 2223, 2223, 19, GumpButtonType.Reply, 0);
                        AddButton(startX + 221, startY + 88, 2224, 2224, 20, GumpButtonType.Reply, 0);
                    }

                    //Directions            
                    AddButton(startX + 160, startY + 109, 4500, 4500, 1, GumpButtonType.Reply, 0); //Forward
                    AddButton(startX + 220, startY + 109, 4501, 4501, 2, GumpButtonType.Reply, 0); //Forward Right
                    AddButton(startX + 267, startY + 152, 4502, 4502, 3, GumpButtonType.Reply, 0); //Right
                    AddButton(startX + 220, startY + 197, 4503, 4503, 4, GumpButtonType.Reply, 0); //Backwards Right
                    AddButton(startX + 160, startY + 197, 4504, 4504, 5, GumpButtonType.Reply, 0); //Backwards
                    AddButton(startX + 100, startY + 197, 4505, 4505, 6, GumpButtonType.Reply, 0); //Backwards Left
                    AddButton(startX + 55, startY + 152, 4506, 4506, 7, GumpButtonType.Reply, 0); //Left
                    AddButton(startX + 100, startY + 109, 4507, 4507, 8, GumpButtonType.Reply, 0); //Forward Right

                    //Center Controls
                    AddButton(startX + 114, startY + 167, 4014, 4016, 9, GumpButtonType.Reply, 0); //Turn Left
                    AddButton(startX + 171, startY + 167, 4017, 4019, 10, GumpButtonType.Reply, 0); //Stop
                    AddButton(startX + 227, startY + 167, 4005, 4007, 11, GumpButtonType.Reply, 0); //Turn Right
                }

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_ShipHotbarGumpObject == null) return;

            BaseShip m_Ship = m_Player.ShipOccupied;

            if (m_Ship != null)
            {
                if (m_Ship.Deleted)
                    m_Ship = null;
            }

            bool closeGump = true;

            int currentMovementMode;
            int movementModeCount = Enum.GetNames(typeof(MovementMode)).Length;

            int currentShipAction;
            int shipActionCount = Enum.GetNames(typeof(ShipAction)).Length;

            switch (info.ButtonID)
            {
                #region Directions

                //Forward
                case 1:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveForward(m_Player);

                        else
                            BaseShip.OneMoveForward(m_Player);
                    }

                    closeGump = false;
                break;

                //Forward Right
                case 2:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveForwardRight(m_Player);

                        else
                            BaseShip.OneMoveForwardRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Right
                case 3:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveRight(m_Player);

                        else
                            BaseShip.OneMoveRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Backwards Right
                case 4:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveBackwardRight(m_Player);

                        else
                            BaseShip.OneMoveBackwardRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Backward
                case 5:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveBackward(m_Player);

                        else
                            BaseShip.OneMoveBackward(m_Player);
                    }

                    closeGump = false;
                break;

                //Backwards Left
                case 6:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveBackwardLeft(m_Player);

                        else
                            BaseShip.OneMoveBackwardLeft(m_Player);
                    }

                    closeGump = false;
                break;

                //Left
                case 7:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveLeft(m_Player);

                        else
                            BaseShip.OneMoveLeft(m_Player);
                    }

                    closeGump = false;
                break;

                //Forward Left
                case 8:
                    if (m_Ship != null)
                    {
                        if (m_ShipHotbarGumpObject.m_MovementMode == MovementMode.Full)
                            BaseShip.StartMoveForwardLeft(m_Player);

                        else
                            BaseShip.OneMoveForwardLeft(m_Player);

                    }

                    closeGump = false;
                break;

                //Turn Left
                case 9:
                    if (m_Ship != null)
                        BaseShip.StartTurnLeft(m_Player);

                    closeGump = false;
                break;

                //Stop
                case 10:
                    if (m_Ship != null)
                        BaseShip.Stop(m_Player);

                    closeGump = false;
                break;

                //Turn Right
                case 11:
                    if (m_Ship != null)
                        BaseShip.StartTurnRight(m_Player);

                    closeGump = false;
                break;

                #endregion

                //Previous Movement Mode
                case 12:
                    currentMovementMode = (int)m_ShipHotbarGumpObject.m_MovementMode;
                    currentMovementMode--;

                    if (currentMovementMode < 0)
                        currentMovementMode = movementModeCount - 1;

                    m_ShipHotbarGumpObject.m_MovementMode = (MovementMode)currentMovementMode;

                    closeGump = false;
                break;

                //Next Movement Mode
                case 13:
                    currentMovementMode = (int)m_ShipHotbarGumpObject.m_MovementMode;
                    currentMovementMode++;

                    if (currentMovementMode > movementModeCount - 1)
                        currentMovementMode = 0;

                    m_ShipHotbarGumpObject.m_MovementMode = (MovementMode)currentMovementMode;

                    closeGump = false;
                break;

                //Previous Ship Action
                case 14:
                    currentShipAction = (int)m_ShipHotbarGumpObject.m_ShipAction;
                    currentShipAction--;

                    if (currentShipAction < 0)
                        currentShipAction = shipActionCount - 1;

                    m_ShipHotbarGumpObject.m_ShipAction = (ShipAction)currentShipAction;

                    closeGump = false;
                break;

                //Next Ship Action
                case 15:
                currentShipAction = (int)m_ShipHotbarGumpObject.m_ShipAction;
                    currentShipAction++;

                    if (currentShipAction > shipActionCount - 1)
                        currentShipAction = 0;

                    m_ShipHotbarGumpObject.m_ShipAction = (ShipAction)currentShipAction;

                    closeGump = false;
                break;

                //Activate Ship Action
                case 16:
                    switch (m_ShipHotbarGumpObject.m_ShipAction)
                    {
                        case ShipAction.RaiseAnchor:
                            if (m_Ship != null)                                                           
                                BaseShip.RaiseAnchor(m_Player);                            
                        break;

                        case ShipAction.LowerAnchor:
                            if (m_Ship != null)  
                                BaseShip.LowerAnchor(m_Player);                            
                        break;

                        case ShipAction.Embark:
                            BaseShip.TargetedEmbark(m_Player);                           
                        break;

                        case ShipAction.EmbarkFollowers:
                            BaseShip.TargetedEmbarkFollowers(m_Player);
                        break;

                        case ShipAction.Disembark:
                            if (m_Ship != null)
                                m_Ship.Disembark(m_Player);
                        break;

                        case ShipAction.DisembarkFollowers:
                            if (m_Ship != null)
                                m_Ship.DisembarkFollowers(m_Player);
                        break;

                        case ShipAction.Dock:
                            if (m_Ship != null)
                                m_Ship.DryDockCommand(m_Player);     
                        break;

                        case ShipAction.ClearDeck:
                            if (m_Ship != null)
                                m_Ship.ClearTheDeckCommand(m_Player);
                        break;
                            
                        case ShipAction.AddFriend:                        
                            if (m_Ship != null)
                                m_Ship.AddFriendCommand(m_Player);                            
                        break;

                        case ShipAction.AddCoOwner:
                            if (m_Ship != null)
                                m_Ship.AddCoOwnerCommand(m_Player);      
                        break;

                        case ShipAction.ThrowTargetOverboard:
                            if (m_Ship != null)
                                m_Ship.ThrowOverboardCommand(m_Player);
                        break;
                    }

                    closeGump = false;
                break;

                //Fire Left Cannons
                case 17:
                    if (m_Ship != null)
                    {
                        if (m_Ship.IsCoOwner(m_Player) || m_Ship.IsOwner(m_Player))
                            BaseShip.FireCannons(m_Player, true);
                    }

                    closeGump = false;
                break;

                //Fire Right Cannons
                case 18:
                    if (m_Ship != null)
                    {
                        if (m_Ship.IsCoOwner(m_Player) || m_Ship.IsOwner(m_Player))
                            BaseShip.FireCannons(m_Player, false);
                    }

                    closeGump = false;
                break;

                //Targeting Mode: Previous
                case 19:
                    if (m_Ship != null)
                    {
                        if (m_Ship.IsCoOwner(m_Player) || m_Ship.IsOwner(m_Player))
                        {
                            switch (m_Ship.TargetingMode)
                            {
                                case TargetingMode.Random: m_Ship.SetTargetingMode(TargetingMode.Guns); break;
                                case TargetingMode.Hull: m_Ship.SetTargetingMode(TargetingMode.Random); break;
                                case TargetingMode.Sails: m_Ship.SetTargetingMode(TargetingMode.Hull); break;
                                case TargetingMode.Guns: m_Ship.SetTargetingMode(TargetingMode.Sails); break;
                            }
                        }   
                    }

                    closeGump = false;
                break;

                //Targeting Mode: Next
                case 20:
                    if (m_Ship != null)
                    {
                        if (m_Ship.IsCoOwner(m_Player) || m_Ship.IsOwner(m_Player))
                        {
                            switch (m_Ship.TargetingMode)
                            {
                                case TargetingMode.Random: m_Ship.SetTargetingMode(TargetingMode.Hull); break;
                                case TargetingMode.Hull: m_Ship.SetTargetingMode(TargetingMode.Sails); break;
                                case TargetingMode.Sails: m_Ship.SetTargetingMode(TargetingMode.Guns); break;
                                case TargetingMode.Guns: m_Ship.SetTargetingMode(TargetingMode.Random); break;
                            }
                        }  
                    }

                    closeGump = false;
                break;

                //Minor Ability Activate
                case 21:
                    if (m_Ship != null)                    
                        m_Ship.ActivateMinorAbility(m_Player);

                    closeGump = false;
                break;

                //Major Ability Activate
                case 22:
                    if (m_Ship != null)
                        m_Ship.ActivateMajorAbility(m_Player);

                    closeGump = false;
                break;

                //Epic Ability Activate
                case 23:
                if (m_Ship != null)
                    m_Ship.ActivateEpicAbility(m_Player);

                closeGump = false;
                break;

                //Collapse + Expand
                case 24:
                m_ShipHotbarGumpObject.m_CollapseMode = !m_ShipHotbarGumpObject.m_CollapseMode;

                    closeGump = false;
                break;

                //Display Mode: Next
                case 26:
                    switch (m_ShipHotbarGumpObject.m_DisplayMode)
                    {
                        case DisplayModeType.Stats: m_ShipHotbarGumpObject.m_DisplayMode = DisplayModeType.StatsAbilities; break;
                        case DisplayModeType.StatsAbilities: m_ShipHotbarGumpObject.m_DisplayMode = DisplayModeType.StatsAbilitiesNavigator; break;
                        case DisplayModeType.StatsAbilitiesNavigator: m_ShipHotbarGumpObject.m_DisplayMode = DisplayModeType.StatsNavigator; break;
                        case DisplayModeType.StatsNavigator: m_ShipHotbarGumpObject.m_DisplayMode = DisplayModeType.Navigator; break;
                        case DisplayModeType.Navigator: m_ShipHotbarGumpObject.m_DisplayMode = DisplayModeType.Stats; break;
                    }

                    closeGump = false;
                break;

                //Ship Gump
                case 27:
                    BaseShip.ShipSelection(m_Player);

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipHotbarGump));
                m_Player.SendGump(new ShipHotbarGump(m_Player, m_ShipHotbarGumpObject));
            }
        }
    }

    public class ShipHotbarGumpObject
    {
        public bool m_CollapseMode = false;
        public ShipHotbarGump.DisplayModeType m_DisplayMode = ShipHotbarGump.DisplayModeType.StatsAbilitiesNavigator;

        public ShipHotbarGump.MovementMode m_MovementMode = ShipHotbarGump.MovementMode.Full;
        public ShipHotbarGump.ShipAction m_ShipAction = ShipHotbarGump.ShipAction.Embark;

        public ShipHotbarGumpObject()
        {
        }
    }
}



