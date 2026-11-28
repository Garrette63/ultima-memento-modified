using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Server;

namespace Server.Items
{
	// One selectable appearance: the art, the name we show the player, and the
	// base item that art comes from.
	public class TransmogrifyOption
	{
		public int ItemID;
		public int Hue;
		public string Name;
		public string Source;
		public Layer Layer;

		// What the art actually belongs to. A single art can be shared by several
		// classes, so these flags are merged rather than trusting the first class found.
		public bool IsShield;
		public bool IsWeapon;
		public bool IsArmor;
		public bool IsClothing;
		public bool IsTrinket;

		// Weapon arts can be shared too; every weapon skill that can use this art is
		// kept, so a weapon may only be reshaped into another weapon of its own skill.
		public List<SkillName> WeaponSkills;

		public TransmogrifyOption( int itemID, int hue, string name, string source, Layer layer, Type sourceType, bool hasWeaponSkill, SkillName weaponSkill )
		{
			ItemID = itemID;
			Hue = hue;
			Name = name;
			Source = source;
			Layer = layer;
			WeaponSkills = new List<SkillName>();

			AddSource( sourceType, hasWeaponSkill, weaponSkill );
		}

		public void AddSource( Type sourceType, bool hasWeaponSkill, SkillName weaponSkill )
		{
			if ( sourceType == null )
				return;

			if ( typeof( BaseShield ).IsAssignableFrom( sourceType ) )
				IsShield = true;

			if ( typeof( BaseArmor ).IsAssignableFrom( sourceType ) )
				IsArmor = true;

			if ( typeof( BaseClothing ).IsAssignableFrom( sourceType ) )
				IsClothing = true;

			if ( typeof( BaseTrinket ).IsAssignableFrom( sourceType ) )
				IsTrinket = true;

			if ( typeof( BaseWeapon ).IsAssignableFrom( sourceType ) )
			{
				IsWeapon = true;

				if ( hasWeaponSkill && !WeaponSkills.Contains( weaponSkill ) )
					WeaponSkills.Add( weaponSkill );
			}
		}

		public bool UsesWeaponSkill( SkillName skill )
		{
			return WeaponSkills.Contains( skill );
		}
	}

	// Builds (once) a table of every wearable appearance in the shard, grouped by
	// the equipment slot it occupies. "Wearable" is decided by Item.Layer, which
	// this shard derives from the art's tiledata, so containers/spawners/etc. fall
	// out automatically (their Layer is Invalid).
	public static class TransmogrifyCatalog
	{
		private static Dictionary<Layer, List<TransmogrifyOption>> m_Table;
		private static readonly object m_Sync = new object();

		public static List<TransmogrifyOption> OptionsFor( Layer layer )
		{
			Ensure();

			List<TransmogrifyOption> list;

			if ( m_Table != null && m_Table.TryGetValue( layer, out list ) )
				return list;

			return new List<TransmogrifyOption>();
		}

		public static int CountFor( Layer layer )
		{
			return OptionsFor( layer ).Count;
		}

		// The filtered list for a particular item. Layer alone is not enough for the
		// hand slots: shields, two-handed weapons, torches and lanterns all share
		// Layer.TwoHanded, and pugilist gloves share Layer.Gloves with ordinary
		// gloves. A shield may only become another shield; a weapon may only become
		// another weapon that uses the same weapon skill; other hand-slot equipment
		// stays within its own family.
		public static List<TransmogrifyOption> OptionsFor( Item source )
		{
			if ( source == null || source.Deleted )
				return new List<TransmogrifyOption>();

			List<TransmogrifyOption> all = OptionsFor( source.Layer );
			List<TransmogrifyOption> filtered = new List<TransmogrifyOption>();

			if ( source is BaseShield )
			{
				for ( int i = 0; i < all.Count; i++ )
				{
					if ( all[i].IsShield )
						filtered.Add( all[i] );
				}

				return filtered;
			}

			if ( source is BaseWeapon )
			{
				SkillName skill = ( (BaseWeapon) source ).Skill;

				for ( int i = 0; i < all.Count; i++ )
				{
					if ( all[i].IsWeapon && all[i].UsesWeaponSkill( skill ) )
						filtered.Add( all[i] );
				}

				return filtered;
			}

			if ( !IsHandSlot( source.Layer ) )
				return all;

			bool clothing = source is BaseClothing;
			bool trinket = source is BaseTrinket;
			bool armor = source is BaseArmor;

			// A hand-slot item that is not one of the known families (a tool or a
			// light, for example) gets no options rather than being mixed with weapons.
			if ( !clothing && !trinket && !armor )
				return filtered;

			for ( int i = 0; i < all.Count; i++ )
			{
				if ( clothing && !all[i].IsClothing )
					continue;

				if ( trinket && !all[i].IsTrinket )
					continue;

				if ( armor && ( !all[i].IsArmor || all[i].IsShield ) )
					continue;

				filtered.Add( all[i] );
			}

			return filtered;
		}

		public static int CountFor( Item source )
		{
			return OptionsFor( source ).Count;
		}

		// What the transmogrifier will accept as a target. Shields are armor and are
		// therefore covered by BaseArmor; weapons are now accepted too, but their
		// options are restricted to weapons of the same skill above.
		public static bool CanReshape( Item item )
		{
			return item != null && !item.Deleted
				&& ( item is BaseArmor || item is BaseClothing || item is BaseWeapon );
		}

		private static bool IsHandSlot( Layer layer )
		{
			return layer == Layer.OneHanded || layer == Layer.TwoHanded || layer == Layer.Gloves;
		}

		// Diagnostics: total appearances known across every slot.
		public static int TotalCount
		{
			get
			{
				Ensure();

				int total = 0;

				if ( m_Table != null )
				{
					foreach ( List<TransmogrifyOption> list in m_Table.Values )
						total += list.Count;
				}

				return total;
			}
		}

		public static int SlotCount
		{
			get
			{
				Ensure();

				return m_Table == null ? 0 : m_Table.Count;
			}
		}

		// Lets a GM regenerate the table after new items are added (scripted or live).
		public static void Rebuild()
		{
			lock ( m_Sync )
			{
				m_Table = null;
			}

			Ensure();
		}

		private static void Ensure()
		{
			if ( m_Table != null )
				return;

			lock ( m_Sync )
			{
				if ( m_Table != null )
					return;

				m_Table = Build();
			}
		}

		private static Dictionary<Layer, List<TransmogrifyOption>> Build()
		{
			Dictionary<Layer, List<TransmogrifyOption>> table = new Dictionary<Layer, List<TransmogrifyOption>>();
			Dictionary<string, TransmogrifyOption> seen = new Dictionary<string, TransmogrifyOption>();

			// Item types live in the script assemblies, not in the core assembly that
			// defines Server.Item, so scan the core plus every loaded script assembly
			// (the same set GenCategorization uses).
			List<Assembly> assemblies = new List<Assembly>();

			AddAssembly( assemblies, Server.Core.Assembly );
			AddAssembly( assemblies, typeof( TransmogrifyCatalog ).Assembly );

			if ( Server.ScriptCompiler.Assemblies != null )
			{
				for ( int i = 0; i < Server.ScriptCompiler.Assemblies.Length; i++ )
					AddAssembly( assemblies, Server.ScriptCompiler.Assemblies[i] );
			}

			for ( int a = 0; a < assemblies.Count; a++ )
			{
				Type[] types = SafeTypes( assemblies[a] );

			for ( int i = 0; i < types.Length; i++ )
			{
				Type t = types[i];

				if ( t == null || !t.IsClass || t.IsAbstract || t.IsGenericTypeDefinition )
					continue;

				// In this shard [Constructable] marks the parameterless constructor,
				// not the type (this is the same test GenCategorization uses).
				ConstructorInfo ctor = t.GetConstructor( Type.EmptyTypes );

				if ( ctor == null || !ctor.IsDefined( typeof( ConstructableAttribute ), false ) )
					continue;

				// Only the wearable families. Everything else (containers, spawners,
				// decorations, ...) cannot fill a slot anyway, and this keeps the scan
				// from running arbitrary item constructors that populate contents.
				if ( !IsWearableFamily( t ) )
					continue;

				Item made = null;

				try
				{
					made = ctor.Invoke( null ) as Item;

					if ( made == null || made.Deleted || made.ItemID <= 0 )
						continue;

					Layer layer = made.Layer;

					if ( !IsWearable( layer ) )
						continue;

					BaseWeapon weapon = made as BaseWeapon;
					SkillName weaponSkill = weapon != null ? weapon.Skill : SkillName.Swords;
					bool hasWeaponSkill = weapon != null;

					string key = ( (int)layer ) + ":" + made.ItemID;

					TransmogrifyOption existing;

					if ( seen.TryGetValue( key, out existing ) )
					{
						// The same art can come from a gift/artifact wrapper or from a
						// second class; merge its family and weapon skill into the entry
						// already in the table.
						existing.AddSource( t, hasWeaponSkill, weaponSkill );
						continue;
					}

					List<TransmogrifyOption> list;

					if ( !table.TryGetValue( layer, out list ) )
					{
						list = new List<TransmogrifyOption>();
						table[layer] = list;
					}

					TransmogrifyOption option = new TransmogrifyOption( made.ItemID, 0, DisplayName( made, t ), t.Name, layer, t, hasWeaponSkill, weaponSkill );

					seen[key] = option;
					list.Add( option );
				}
				catch
				{
					// Some item constructors are not parameterless or have side effects;
					// anything that misbehaves is simply not offered as an appearance.
				}
				finally
				{
					if ( made != null && !made.Deleted )
						made.Delete();
				}
			}
			}

			foreach ( List<TransmogrifyOption> list in table.Values )
			{
				list.Sort( delegate( TransmogrifyOption a, TransmogrifyOption b )
				{
					return string.Compare( a.Name, b.Name, StringComparison.OrdinalIgnoreCase );
				} );
			}

			int total = 0;

			foreach ( List<TransmogrifyOption> list in table.Values )
				total += list.Count;

			Console.WriteLine( "[Transmogrify] appearance catalog: {0} entries across {1} slots, from {2} assembl{3}.",
				total, table.Count, assemblies.Count, assemblies.Count == 1 ? "y" : "ies" );

			return table;
		}

		private static void AddAssembly( List<Assembly> list, Assembly asm )
		{
			if ( asm == null )
				return;

			for ( int i = 0; i < list.Count; i++ )
			{
				if ( list[i] == asm )
					return;
			}

			list.Add( asm );
		}

		private static Type[] SafeTypes( Assembly asm )
		{
			try
			{
				return asm.GetTypes();
			}
			catch ( ReflectionTypeLoadException ex )
			{
				List<Type> ok = new List<Type>();

				if ( ex.Types != null )
				{
					for ( int i = 0; i < ex.Types.Length; i++ )
					{
						if ( ex.Types[i] != null )
							ok.Add( ex.Types[i] );
					}
				}

				return ok.ToArray();
			}
			catch
			{
				return new Type[0];
			}
		}

		private static bool IsWearableFamily( Type t )
		{
			return typeof( BaseArmor ).IsAssignableFrom( t )
				|| typeof( BaseClothing ).IsAssignableFrom( t )
				|| typeof( BaseWeapon ).IsAssignableFrom( t )
				|| typeof( BaseTrinket ).IsAssignableFrom( t );
		}

		private static bool IsWearable( Layer layer )
		{
			switch ( layer )
			{
				case Layer.Invalid:
				case Layer.Hair:
				case Layer.FacialHair:
				case Layer.Special:
				case Layer.Backpack:
				case Layer.Mount:
					return false;
			}

			return layer >= Layer.OneHanded && layer <= Layer.LastUserValid;
		}

		private static string DisplayName( Item item, Type t )
		{
			return BaseName( item );
		}

		// The player-facing name is the name of the *base* item - the art - never
		// the class, the artifact or the gift wrapper that happens to use it. An
		// artifact gorget therefore reads "Leather Gorget" and a stuffed one
		// "Stuffed Gorget", which is how the shard's tiledata names them.
		public static string BaseName( Item item )
		{
			if ( item == null )
				return "";

			string art = TileData.ItemTable[item.ItemID & TileData.MaxItemValue].Name;

			if ( !string.IsNullOrEmpty( art ) )
				return TitleCase( art.Trim() );

			return Prettify( item.GetType().Name );
		}

		// "leather gorget" -> "Leather Gorget"
		private static string TitleCase( string text )
		{
			char[] chars = text.ToCharArray();
			bool start = true;

			for ( int i = 0; i < chars.Length; i++ )
			{
				if ( start )
					chars[i] = char.ToUpper( chars[i] );

				start = char.IsWhiteSpace( chars[i] );
			}

			return new string( chars );
		}

		// PlateGorget -> Plate Gorget
		private static string Prettify( string name )
		{
			StringBuilder sb = new StringBuilder();

			for ( int i = 0; i < name.Length; i++ )
			{
				char c = name[i];

				if ( i > 0 && char.IsUpper( c ) && !char.IsUpper( name[i - 1] ) )
					sb.Append( ' ' );

				sb.Append( c );
			}

			return sb.ToString();
		}
	}
}
