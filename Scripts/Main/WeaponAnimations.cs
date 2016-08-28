using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class WeaponAnimations
    {
        public static WeaponAnimationDetail GetWeaponAnimationDetail(BaseWeapon weapon, Mobile mobile)
        {
            WeaponAnimationDetail weaponAnimationDetail = new WeaponAnimationDetail(weapon, mobile);

            if (weapon == null) return weaponAnimationDetail;
            if (mobile == null) return weaponAnimationDetail;

            #region Ranged

            if (weapon is BaseRanged)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 27; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 18; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            //Overrides
            if (weapon is Crossbow || weapon is HeavyCrossbow)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 19; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Spear

            if (weapon is BaseSpear)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;                       
                        case 4: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        
                        case 5: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 4))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        
                        case 2: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        
                        case 4: weaponAnimationDetail.AnimationID = 19; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            //Overrides
            if (weapon is WarFork)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                       
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 2: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                                               
                        case 4: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            if (weapon is ShortSpear)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;

                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 2: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 4: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Bashing

            if (weapon is BaseBashing)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 3: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 4: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 6: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 7: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 8: weaponAnimationDetail.AnimationID = 31; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Staff

            if (weapon is BaseStaff)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        case 4: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;

                        case 5: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;

                        case 4: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 18; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        case 7: weaponAnimationDetail.AnimationID = 19; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        case 8: weaponAnimationDetail.AnimationID = 30; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }
            }

            #endregion

            #region Axe

            if (weapon is BaseAxe)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 4))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        
                        case 2: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 4: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 3: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 4: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 5: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 19; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Polearm

            if (weapon is BasePoleArm)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 3: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 4: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        case 6: weaponAnimationDetail.AnimationID = 29; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                                                
                        case 4: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 6: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 7: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 8: weaponAnimationDetail.AnimationID = 19; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Sword

            if (weapon is BaseSword)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        
                        case 3: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 4: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion

            #region Knife

            if (weapon is BaseKnife)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 4: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 6: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 7: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                        case 8: weaponAnimationDetail.AnimationID = 13; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }
            }

            #endregion

            #region Fist

            if (weapon is Fists)
            {
                if (mobile.Mounted)
                {
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 3: weaponAnimationDetail.AnimationID = 26; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;

                        case 4: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 28; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = true; break;
                    }
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 9))
                    {
                        case 1: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 2: weaponAnimationDetail.AnimationID = 9; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 3: weaponAnimationDetail.AnimationID = 10; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 4: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 5: weaponAnimationDetail.AnimationID = 11; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;

                        case 6: weaponAnimationDetail.AnimationID = 12; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 7: weaponAnimationDetail.AnimationID = 14; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 8: weaponAnimationDetail.AnimationID = 30; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                        case 9: weaponAnimationDetail.AnimationID = 31; weaponAnimationDetail.FrameCount = 5; weaponAnimationDetail.Reverse = false; break;
                    }
                }
            }

            #endregion           

            return weaponAnimationDetail;
        }
    }

    public class WeaponAnimationDetail
    {
        public BaseWeapon m_Weapon;
        public Mobile m_Mobile;

        public int AnimationID = 9;
        public int FrameCount = 5;
        public bool Reverse = false;

        public WeaponAnimationDetail(BaseWeapon weapon, Mobile mobile)
        {
            m_Weapon = weapon;
            m_Mobile = mobile;
        }
    }
}