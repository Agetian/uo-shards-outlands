using System;
using System.Collections.Generic;
using System.IO;
using Server.Items;

namespace Server.Spells
{
	public class SpellRegistry
	{
        //IPY has only 300 types
		private static Type[] m_Types = new Type[300];
		private static int m_Count;

		public static Type[] Types
		{
			get
			{
				m_Count = -1;
				return m_Types;
			}
		}
		
		//What IS this used for anyways.
		public static int Count
		{
			get
			{
				if ( m_Count == -1 )
				{
					m_Count = 0;
                    //IPY has only 64 types
					for ( int i = 0; i < 64; ++i )
					{
						if ( m_Types[i] != null )
							++m_Count;
					}
				}

				return m_Count;
			}
		}

		private static Dictionary<Type, Int32> m_IDsFromTypes = new Dictionary<Type, Int32>( m_Types.Length );
		
		public static int GetRegistryNumber( ISpell s )
		{
			return GetRegistryNumber( s.GetType() );
		}


		public static int GetRegistryNumber( Type type )
		{
			if( m_IDsFromTypes.ContainsKey( type ) )
				return m_IDsFromTypes[type];

			return -1;
		}

		public static void Register( int spellID, Type type )
		{
			if ( spellID < 0 || spellID >= m_Types.Length )
				return;

			if ( m_Types[spellID] == null )
				++m_Count;

			m_Types[spellID] = type;

			if( !m_IDsFromTypes.ContainsKey( type ) )
				m_IDsFromTypes.Add( type, spellID );
		}
		
		private static object[] m_Params = new object[2];

		public static Spell NewSpell( int spellID, Mobile caster, Item scroll )
		{
			if ( spellID < 0 || spellID >= m_Types.Length )
				return null;

			Type t = m_Types[spellID];

            //IPY doesnt have Special Moves
			if( t != null /*&& !t.IsSubclassOf( typeof( SpecialMove ) ) */)
			{
				m_Params[0] = caster;
				m_Params[1] = scroll;

				try
				{
					return (Spell)Activator.CreateInstance( t, m_Params );
				}
				catch
				{
				}
			}

			return null;
		}

		private static string[] m_CircleNames = new string[]
			{
				"First",
				"Second",
				"Third",
				"Fourth",
				"Fifth",
				"Sixth",
				"Seventh",
				"Eighth",
				"Necromancy",
				"Chivalry",
				"Bushido",
				"Ninjitsu",
				"Spellweaving"
			};

		public static Spell NewSpell( string name, Mobile caster, Item scroll )
		{
			for ( int i = 0; i < m_CircleNames.Length; ++i )
			{
				Type t = ScriptCompiler.FindTypeByFullName( String.Format( "Server.Spells.{0}.{1}", m_CircleNames[i], name ) );

                //IPY Doesnt have Special Move
				if ( t != null /*&& !t.IsSubclassOf( typeof( SpecialMove ) )*/ )
				{
					m_Params[0] = caster;
					m_Params[1] = scroll;

					try
					{
						return (Spell)Activator.CreateInstance( t, m_Params );
					}
					catch
					{
					}
				}
			}

			return null;
		}
	}
}