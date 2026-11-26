using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	// Displays the stats of a live human henchman. This mirrors the player
	// StatsGump, but leaves out the player-only fields and adds the henchman
	// resistances, bandage supply and remaining hire time.
	public class HenchmanInfoGump : Gump
	{
		private Mobile m_Henchman;
		private Mobile m_From;

		public HenchmanInfoGump( Mobile henchman, Mobile from ) : base( 50, 50 )
		{
			if ( henchman == null || henchman.Deleted || from == null )
				return;

			m_Henchman = henchman;
			m_From = from;

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

			name = ( name ?? "henchman" ).ToUpper();

			// Wide enough for a 32-character name plus a 32-character title on one
			// line (NameTitleMaxLength in HenchmanCustomizeGumpBase), which is the
			// longest pair the customization gumps can store.
			AddHtml( 15, 15, 600, 20, @"<BODY><BASEFONT Color=" + color + ">" + name + "</BASEFONT></BODY>", (bool)false, (bool)false );

			AddButton( 667, 12, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			if ( henchman is HenchmanFighter || henchman is HenchmanArcher || henchman is HenchmanWizard )
			{
				AddButton( 15, 38, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
				AddHtml( 50, 40, 100, 18, @"<BODY><BASEFONT Color=" + color + ">Customize</BASEFONT></BODY>", false, false );
			}

			int SDICap = 1000000;
			if ( SDICap > MyServerSettings.SpellDamageIncreaseVsMonsters() && MyServerSettings.SpellDamageIncreaseVsMonsters() > 0 ) { SDICap = MyServerSettings.SpellDamageIncreaseVsMonsters(); }

			int BandageSpeedMilliseconds = BandageContext.HealTimer( henchman, henchman );

			BaseWeapon weapon = henchman.Weapon as BaseWeapon;
			TimeSpan SwingSpeed = ( weapon == null ? TimeSpan.Zero : weapon.GetDelay( henchman ) );
			if ( SwingSpeed > TimeSpan.FromSeconds( 100 ) ) { SwingSpeed = TimeSpan.FromSeconds( 100 ); }

			int weaponDamage = AosAttributes.GetValue( henchman, AosAttribute.WeaponDamage );
			if ( weapon != null ) { weaponDamage += weapon.GetDamageBonus(); }

			var colAB = new YPosition( 80, 35 );

			AddStatLine( 20, 135, 80, "Strength", string.Format( "{0} + {1}", henchman.RawStr, henchman.Str - henchman.RawStr ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Dexterity", string.Format( "{0} + {1}", henchman.RawDex, henchman.Dex - henchman.RawDex ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Intelligence", string.Format( "{0} + {1}", henchman.RawInt, henchman.Int - henchman.RawInt ), null, color, colAB );
			AddStatLine( 20, 135, 80, "Physical Resist", GetResistanceText( henchman, ResistanceType.Physical ), GetResistanceTooltip(), color, colAB );
			AddStatLine( 20, 135, 80, "Fire Resist", GetResistanceText( henchman, ResistanceType.Fire ), GetResistanceTooltip(), color, colAB );
			AddStatLine( 20, 135, 80, "Cold Resist", GetResistanceText( henchman, ResistanceType.Cold ), GetResistanceTooltip(), color, colAB );
			AddStatLine( 20, 135, 80, "Poison Resist", GetResistanceText( henchman, ResistanceType.Poison ), GetResistanceTooltip(), color, colAB );
			AddStatLine( 20, 135, 80, "Energy Resist", GetResistanceText( henchman, ResistanceType.Energy ), GetResistanceTooltip(), color, colAB );
			AddStatLine( 20, 135, 80, "Bandages", string.Format( "{0}", henchman.Hunger ), "Bandages this henchman can use to heal themselves.", color, colAB );

			var colCD = new YPosition( 80, 35 );

			AddStatLine( 260, 375, 80, "Hits", string.Format( "{0} + {1}", henchman.Hits - AosAttributes.GetValue( henchman, AosAttribute.BonusHits ), AosAttributes.GetValue( henchman, AosAttribute.BonusHits ) ), null, color, colCD );
			AddStatLine( 260, 375, 80, "Stamina", string.Format( "{0} + {1}", henchman.Stam - AosAttributes.GetValue( henchman, AosAttribute.BonusStam ), AosAttributes.GetValue( henchman, AosAttribute.BonusStam ) ), null, color, colCD );
			AddStatLine( 260, 375, 80, "Mana", string.Format( "{0} + {1}", henchman.Mana - AosAttributes.GetValue( henchman, AosAttribute.BonusMana ), AosAttributes.GetValue( henchman, AosAttribute.BonusMana ) ), null, color, colCD );
			AddStatLine( 260, 375, 80, "Hits Regen", string.Format( "{0}", AosAttributes.GetValue( henchman, AosAttribute.RegenHits ) ), "Regenerates hit points over time", color, colCD );
			AddStatLine( 260, 375, 80, "Stamina Regen", string.Format( "{0}", AosAttributes.GetValue( henchman, AosAttribute.RegenStam ) ), null, color, colCD );
			AddStatLine( 260, 375, 80, "Mana Regen", string.Format( "{0}", AosAttributes.GetValue( henchman, AosAttribute.RegenMana ) ), null, color, colCD );

			if ( MyServerSettings.LowerMana() > 0 )
				AddStatLine( 260, 375, 80, "Low Mana", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.LowerManaCost, false ), MyServerSettings.LowerMana() ), "Reduces mana cost of casting spells and using abilities", color, colCD );

			AddStatLine( 260, 375, 80, "Spell Damage +", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.SpellDamage ), SDICap ), "Increases damage done by spells", color, colCD );
			AddStatLine( 260, 375, 80, "Time Remaining", string.Format( "{0} minutes", (int)( henchman.Fame / 5 ) ), "Approximate time before the henchman leaves.", color, colCD );

			var colEF = new YPosition( 80, 35 );

			AddStatLine( 500, 615, 80, "Hit Chance", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.AttackChance ), 45 ), "Increases chance to land weapon attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Defend Chance", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.DefendChance ), 45 ), "Increases chance to dodge weapon attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Swing Speed", string.Format( "{0}s", new DateTime( SwingSpeed.Ticks ).ToString( "s.ff" ) ), "Duration between weapon attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Swing Speed +", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.WeaponSpeed ), 60 ), "Increases attack speed of weapon attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Bandage Speed", string.Format( "{0:0.0}s", new DateTime( TimeSpan.FromMilliseconds( BandageSpeedMilliseconds ).Ticks ).ToString( "s.ff" ) ), null, color, colEF );
			AddStatLine( 500, 615, 80, "Damage Increase", string.Format( "{0}/{1}%", weaponDamage, 100 ), "Increases damage of weapon attacks", color, colEF );
			AddStatLine( 500, 615, 80, "Reflect Damage", string.Format( "{0}/{1}%", AosAttributes.GetValue( henchman, AosAttribute.ReflectPhysical ), 100 ), "Percent of received physical damage that the attacker also receives", color, colEF );
			AddStatLine( 500, 615, 80, "Fast Cast", string.Format( "{0}", AosAttributes.GetValue( henchman, AosAttribute.CastSpeed ) ), string.Format( "Cast Speed Increase: Pure Knights ({0}) / Otherwise ({1})", 4, 2 ), color, colEF );
			AddStatLine( 500, 615, 80, "Cast Recovery", string.Format( "{0}", AosAttributes.GetValue( henchman, AosAttribute.CastRecovery ) ), null, color, colEF );
		}

		private static string GetResistanceTooltip()
		{
			return "Total resistance; the value from worn gear is shown in parentheses.";
		}

		private static string GetResistanceText( Mobile m, ResistanceType type )
		{
			return string.Format( "{0} ({1})", GetTotalResistance( m, type ), GetGearResistance( m, type ) );
		}

		private static int GetTotalResistance( Mobile m, ResistanceType type )
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

		// Mirrors the item contribution used by Mobile.ComputeResistances. The
		// player-stat resistance is a floor for human henchmen rather than being
		// added to gear, so this only reports what the equipment alone provides.
		private static int GetGearResistance( Mobile m, ResistanceType type )
		{
			int value = 0;

			if ( m != null && !m.Deleted )
			{
				for ( int i = 0; i < m.Items.Count; ++i )
				{
					Item item = m.Items[i];

					if ( item == null || item.Deleted || item.CheckPropertyConfliction( m ) )
						continue;

					switch ( type )
					{
						case ResistanceType.Physical: value += item.PhysicalResistance; break;
						case ResistanceType.Fire: value += item.FireResistance; break;
						case ResistanceType.Cold: value += item.ColdResistance; break;
						case ResistanceType.Poison: value += item.PoisonResistance; break;
						case ResistanceType.Energy: value += item.EnergyResistance; break;
					}
				}
			}

			if ( value < 0 )
				value = 0;
			else if ( value > 70 )
				value = 70;

			return value;
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

			if ( info.ButtonID == 1 && sender.Mobile != null && sender.Mobile == m_From && m_Henchman != null && !m_Henchman.Deleted
				&& ( m_Henchman is HenchmanFighter || m_Henchman is HenchmanArcher || m_Henchman is HenchmanWizard ) )
			{
				BaseCreature henchman = m_Henchman as BaseCreature;

				if ( henchman != null && henchman.ControlMaster == sender.Mobile )
				{
					sender.Mobile.CloseGump( typeof( HenchmanInfoGump ) );

					// Customization is shown next to the henchman's paperdoll (the skin
					// and gender gumps re-open it), so ask for it as soon as the owner
					// starts customizing.
					HenchmanCustomizeGumpBase.ShowPaperdoll( sender.Mobile, henchman );

					sender.Mobile.SendGump( new HenchmanCustomizeGump( sender.Mobile, henchman ) );
				}
			}
		}
	}
}
