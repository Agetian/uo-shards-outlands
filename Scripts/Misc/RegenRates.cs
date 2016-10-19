using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Mobiles;

namespace Server.Misc
{
	public class RegenRates
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			Mobile.DefaultHitsRate = TimeSpan.FromSeconds(  8.0 );
			Mobile.DefaultStamRate = TimeSpan.FromSeconds(  4.0 );
			Mobile.DefaultManaRate = TimeSpan.FromSeconds(  8.0 );

            Mobile.HitsRegenRateHandler = new RegenRateHandler(Mobile_HitsRegenRate);
            Mobile.StamRegenRateHandler = new RegenRateHandler(Mobile_StamRegenRate);
			Mobile.ManaRegenRateHandler = new RegenRateHandler(Mobile_ManaRegenRate);
		}

		private static void CheckBonusSkill( Mobile m, int cur, int max, SkillName skill )
		{
			if ( !m.Alive )
				return;

			double n = (double)cur / max;
			double v = Math.Sqrt( m.Skills[skill].Value * 0.005 );

			n *= (1.0 - v);
			n += v;

			m.CheckSkill( skill, n , 1.0);
		}

		private static bool CheckTransform( Mobile m, Type type )
		{
			return TransformationSpellHelper.UnderTransformation( m, type );
		}
        
		private static TimeSpan Mobile_HitsRegenRate( Mobile from )
		{
            PlayerMobile player = from as PlayerMobile;

            if (from.Skills == null)
                return Mobile.DefaultHitsRate;

            double baseRate = Mobile.DefaultHitsRate.TotalSeconds;

            double armorEffect = 1.0 + (1.0 * GetArmorMeditationAllowance(from));

            double adjustedRate = baseRate / armorEffect;

            if (adjustedRate > baseRate)
                adjustedRate = baseRate;

            if (adjustedRate < .5)
                adjustedRate = .5;

            return TimeSpan.FromSeconds(adjustedRate);
		}

		private static TimeSpan Mobile_StamRegenRate( Mobile from )
		{            
            PlayerMobile player = from as PlayerMobile;

            if (from.Skills == null)
                return Mobile.DefaultStamRate;

            double baseRate = Mobile.DefaultStamRate.TotalSeconds;

            double armorEffect = 1.0 + (1.0 * GetArmorMeditationAllowance(from));

            double adjustedRate = baseRate / armorEffect;

            if (adjustedRate > baseRate)
                adjustedRate = baseRate;

            if (adjustedRate < .5)
                adjustedRate = .5;

            return TimeSpan.FromSeconds(adjustedRate);
		}

		private static TimeSpan Mobile_ManaRegenRate( Mobile from )
		{
            PlayerMobile player = from as PlayerMobile;

			if ( from.Skills == null )
				return Mobile.DefaultManaRate;

			if ( !from.Meditating )
				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Meditation );

            double baseRate = Mobile.DefaultManaRate.TotalSeconds;

            double armorEffect = 1.0 + (3.0 * GetArmorMeditationAllowance(from));
            double skillEffect = 1.0 + (from.Skills[SkillName.Meditation].Value / 100);
            double activeMeditationEffect = 1.0; 

            if (from.Meditating) 
                activeMeditationEffect = 2.0;

            double adjustedRate = baseRate / armorEffect / skillEffect / activeMeditationEffect;

            if (adjustedRate > baseRate)
                adjustedRate = baseRate;

            if (adjustedRate < .5)
                adjustedRate = .5;

            return TimeSpan.FromSeconds(adjustedRate);
		}

		public static double GetArmorMeditationAllowance( Mobile from )
		{      
            double armorPenaltyPercent = 0;

            PlayerMobile player = from as PlayerMobile;
             
            if (player == null)
                return armorPenaltyPercent;            
            
            //m_ArmorScalars = { 0.07, 0.07, 0.14, 0.15, 0.22, 0.35 };
            //Each Layer of Armor Has Different "Effect Amount" on How Much It Can Penalize Meditation
            armorPenaltyPercent += GetArmorMeditationValue(from.NeckArmor as BaseArmor) * BaseArmor.ArmorScalars[0];
            armorPenaltyPercent += GetArmorMeditationValue(from.HandArmor as BaseArmor) * BaseArmor.ArmorScalars[1];
            armorPenaltyPercent += GetArmorMeditationValue(from.HeadArmor as BaseArmor) * BaseArmor.ArmorScalars[2];
            armorPenaltyPercent += GetArmorMeditationValue(from.ArmsArmor as BaseArmor) * BaseArmor.ArmorScalars[3];
            armorPenaltyPercent += GetArmorMeditationValue(from.LegsArmor as BaseArmor) * BaseArmor.ArmorScalars[4];
            armorPenaltyPercent += GetArmorMeditationValue(from.ChestArmor as BaseArmor) * BaseArmor.ArmorScalars[5];

            armorPenaltyPercent -= GetArmorMeditationValue(from.ShieldArmor as BaseArmor) * BaseArmor.ArmorScalars[5];

            if (armorPenaltyPercent > 1.0)
                armorPenaltyPercent = 1.0;

            if (armorPenaltyPercent < 0)
                armorPenaltyPercent = 0;
            
            return armorPenaltyPercent;
		}

		private static double GetArmorMeditationValue( BaseArmor ar )
		{
			if ( ar == null)
				return 1.0;

			switch ( ar.MeditationAllowance )
			{
				default:
				case ArmorMeditationAllowance.None: return 0.0;
                case ArmorMeditationAllowance.TwentyPercent: return 0.2;
                case ArmorMeditationAllowance.FourtyPercent: return 0.4;
				case ArmorMeditationAllowance.SixtyPercent: return 0.6;
                case ArmorMeditationAllowance.EightyPercent: return 0.8;
				case ArmorMeditationAllowance.OneHundredPercent: return 1.0;
			}
		}
	}
}