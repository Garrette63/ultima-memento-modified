using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	// Displays the stats of a live creature henchman (HenchmanMonster). Creature
	// henchmen cannot swap equipment, so their resistances are shown as totals
	// only and their damage comes from the stat-scaled BaseCreature range.
	public class HenchmanCreatureInfoGump : Gump
	{
		public HenchmanCreatureInfoGump( Mobile henchman, Mobile from ) : base( 50, 50 )
		{
			if ( henchman == null || henchman.Deleted || from == null )
				return;

			from.SendSound( 0x4A );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			int img = 11420;
			string color = "#E4E377";

			AddImage( 1, 1, img, Server.Misc.PlayerSettings.GetGumpHue( from ) );

			string name = henchman.Name;

			if ( !string.IsNullOrEmpty( henchman.Title ) )
				name = name + " " + henchman.Title;

			name = ( name ?? "creature henchman" ).ToUpper();

			AddHtml( 15, 15, 400, 20, @"<BODY><BASEFONT Color=" + color + ">" + name + "</BASEFONT></BODY>", (bool)false, (bool)false );

			AddButton( 667, 12, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			int BandageSpeedMilliseconds = BandageContext.HealTimer( henchman, henchman );

			BaseWeapon weapon = henchman.Weapon as BaseWeapon;
			TimeSpan SwingSpeed = ( weapon == null ? TimeSpan.Zero : weapon.GetDelay( henchman ) );
			if ( SwingSpeed > TimeSpan.FromSeconds( 100 ) ) { SwingSpeed = TimeSpan.FromSeconds( 100 ); }

			var colAB = new YPosition( 80, 35 );

			AddStatLine( 20, 135, 80, "Strength", string.Format( "{0} + {1}", henchman.RawStr, henchman.Str - henchman.RawStr ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Dexterity", string.Format( "{0} + {1}", henchman.RawDex, henchman.Dex - henchman.RawDex ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Intelligence", string.Format( "{0} + {1}", henchman.RawInt, henchman.Int - henchman.RawInt ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Hits", string.Format( "{0} + {1}", henchman.Hits - AosAttributes.GetValue( henchman, AosAttribute.BonusHits ), AosAttributes.GetValue( henchman, AosAttribute.BonusHits ) ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Stamina", string.Format( "{0} + {1}", henchman.Stam - AosAttributes.GetValue( henchman, AosAttribute.BonusStam ), AosAttributes.GetValue( henchman, AosAttribute.BonusStam ) ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Mana", string.Format( "{0} + {1}", henchman.Mana - AosAttributes.GetValue( henchman, AosAttribute.BonusMana ), AosAttributes.GetValue( henchman, AosAttribute.BonusMana ) ), null, color, colAB );

			var colCD = new YPosition( 80, 35 );

			AddStatLine( 260, 375, 80, "Physical Resist", string.Format( "{0}", GetResistance( henchman, ResistanceType.Physical ) ), "Total resistance.", color, colCD );
			AddStatLine( 260, 375, 80, "Fire Resist", string.Format( "{0}", GetResistance( henchman, ResistanceType.Fire ) ), "Total resistance.", color, colCD );
			AddStatLine( 260, 375, 80, "Cold Resist", string.Format( "{0}", GetResistance( henchman, ResistanceType.Cold ) ), "Total resistance.", color, colCD );
			AddStatLine( 260, 375, 80, "Poison Resist", string.Format( "{0}", GetResistance( henchman, ResistanceType.Poison ) ), "Total resistance.", color, colCD );
			AddStatLine( 260, 375, 80, "Energy Resist", string.Format( "{0}", GetResistance( henchman, ResistanceType.Energy ) ), "Total resistance.", color, colCD );

			var colEF = new YPosition( 80, 35 );

			BaseCreature creature = henchman as BaseCreature;
			int damageMin = ( creature == null ? 0 : creature.DamageMin );
			int damageMax = ( creature == null ? 0 : creature.DamageMax );
			if ( damageMin < 0 ) { damageMin = 0; }
			if ( damageMax < 0 ) { damageMax = 0; }

			AddStatLine( 500, 615, 80, "Base Damage", string.Format( "{0} - {1}", damageMin, damageMax ), "Base damage range, scaled from the owner's stats.", color, colEF );
			AddStatLine( 500, 615, 80, "Swing Speed", string.Format( "{0}s", new DateTime( SwingSpeed.Ticks ).ToString( "s.ff" ) ), "Duration between attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Bandage Speed", string.Format( "{0:0.0}s", new DateTime( TimeSpan.FromMilliseconds( BandageSpeedMilliseconds ).Ticks ).ToString( "s.ff" ) ), null, color, colEF );
			AddStatLine( 500, 615, 80, "Bandages", string.Format( "{0}", henchman.Hunger ), "Bandages this henchman can use to heal themselves.", color, colEF );
			AddStatLine( 500, 615, 80, "Time Remaining", string.Format( "{0} minutes", (int)( henchman.Fame / 5 ) ), "Approximate time before the henchman leaves.", color, colEF );
		}

		private static int GetResistance( Mobile m, ResistanceType type )
		{
			switch ( type )
			{
				case ResistanceType.Physical: return m.PhysicalResistance;
				case ResistanceType.Fire: return m.FireResistance;
				case ResistanceType.Cold: return m.ColdResistance;
				case ResistanceType.Poison: return m.PoisonResistance;
				case ResistanceType.Energy: return m.EnergyResistance;
			}

			return 0;
		}

		private class YPosition
		{
			public int Current { get; set; }
			public int LineHeight { get; private set; }

			public YPosition( int startY, int lineHeight )
			{
				Current = startY;
				LineHeight = lineHeight;
			}

			public int Next()
			{
				int currentY = Current;
				Current += LineHeight;
				return currentY;
			}
		}

		private void AddStatLine( int labelX, int valueX, int width, string label, string value, string tooltip, string color, YPosition yPos )
		{
			int currentY = yPos.Next();
			AddHtml( labelX, currentY, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + label + "</BASEFONT></BODY>", false, false );
			if ( !string.IsNullOrEmpty( tooltip ) )
				AddTooltip( tooltip );
			AddHtml( valueX, currentY, width, 20, @"<BODY><BASEFONT Color=" + color + "><div align=right>" + value + "</div></BASEFONT></BODY>", false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( sender.Mobile != null )
				sender.Mobile.SendSound( 0x4A );
		}
	}
}
