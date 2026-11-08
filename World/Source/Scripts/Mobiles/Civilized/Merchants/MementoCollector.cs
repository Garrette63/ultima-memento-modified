using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class MementoCollector : BaseNPC
	{
		// Coin payouts per artifact tier. The highest tier (legendary) pays 15,
		// the lowest (decorative) pays 3, with the middle tiers spaced evenly.
		public const int LegendaryCoins = 15;
		public const int StandardCoins = 11;
		public const int MinorArtifactCoins = 7;
		public const int DecorativeCoins = 3;

		// Dark grey hue applied to the collector's leather armour.
		private const int DarkLeatherHue = 0x83B;

		public override string TalkGumpTitle{ get{ return "Memento Collector"; } }
		public override string TalkGumpSubject{ get{ return "MementoCollector"; } }

		// Regular vendors are not "always attackable"; match that so the NPC is
		// treated as an innocent vendor rather than a free-for-all target.
		public override bool AlwaysAttackable{ get{ return false; } }

		[Constructable]
		public MementoCollector() : base()
		{
			SpeechHue = Utility.RandomTalkHue();

			// Blue merchant name even though the collector is invulnerable.
			NameHue = Notoriety.GetHue( Notoriety.Innocent );

			Direction = Direction.South;
			Blessed = true;

			// Stay on the same neutral team as the town vendors.
			Team = 0;

			Female = Utility.RandomBool();

			if ( Female )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			Title = "the artefact collector";
			Hue = Utility.RandomSkinColor();

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetDamage( 10, 15 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.FistFighting, 100 );
			Karma = 10000;
			VirtualArmor = 30;

			// Standard-artefact armour across the armour, boot and helm slots. The hued
			// cloak and sash below are kept as the collector's identity pieces.
			DressArmour();

			AddThemeHat();

			AddItem( new Cloak( MementoCoin.MementoHue ) );
			AddItem( new BodySash( MementoCoin.MementoHue ) );

			WieldRandomArtefact();

			// Always hold a lit lantern. It is added after the weapon so it can never be
			// displaced; the lantern uses the two-handed layer while the artefact weapon is
			// one-handed, so the collector carries both at once.
			Lantern lantern = new Lantern();
			lantern.Hue = MementoCoin.MementoHue;
			AddItem( lantern );
			lantern.Ignite();

			Utility.AssignRandomHair( this );

			m_NextEmote = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 10, 30 ) );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();

			// Spawners can assign a team; vendors all share the neutral team 0,
			// so make sure the collector stays on the vendor team.
			Team = 0;
		}

		private DateTime m_NextEmote;

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( !(m is PlayerMobile) || !InRange( m, 12 ) || !InLOS( m ) )
				return;

			if ( DateTime.Now < m_NextEmote )
				return;

			m_NextEmote = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 20, 45 ) );

			switch ( Utility.Random( 14 ) )
			{
				case 0: PlaySound( Female ? 822 : 1096 ); Say( "*yawns*" ); break;
				case 1: PlaySound( Female ? 816 : 1090 ); Say( "*sighs*" ); break;
				case 2: PlaySound( Female ? 786 : 1057 ); Say( "*coughs*" ); break;
				case 3: PlaySound( Female ? 784 : 1055 ); Say( "*clears throat*" ); break;
				case 4: PlaySound( Female ? 817 : 1091 ); Say( "Ahh-choo!" ); break;
				case 5: PlaySound( Female ? 818 : 1092 ); Say( "*sniffs*" ); break;
				case 6: PlaySound( Female ? 798 : 1070 ); Say( "*hiccups*" ); break;
				case 7: PlaySound( Female ? 794 : 1066 ); Say( "*chuckles*" ); break;
				case 8: Say( "*stretches*" ); break;
				case 9: Animate( 5, 5, 1, true, false, 1 ); break;
				case 10: Animate( 6, 5, 1, true, false, 1 ); break;
				case 11: Say( "*mumbles about artefacts*" ); break;
				case 12: Say( "*polishes a memento coin*" ); break;
				case 13: Say( "*looks over the collection*" ); break;
			}
		}

		private void DressArmour()
		{
			// Dress every worn body/boot slot with a standard artefact. The plain leather
			// (dark hue) is only used as a fallback for layers the catalogue does not cover.
			DressSlot( new LeatherGorget() );
			DressSlot( new LeatherArms() );
			DressSlot( new LeatherGloves() );
			DressSlot( new LeatherLegs() );
			DressSlot( new LeatherChest() );
			DressSlot( new Boots() );
		}

		private void DressSlot( Item baseItem )
		{
			if ( baseItem == null )
				return;

			// Swap the plain item for a random standard artefact on the same layer.
			if ( SwapWithArtefact( baseItem ) )
				return;

			baseItem.Hue = DarkLeatherHue;
			AddItem( baseItem );
		}

		private void AddThemeHat()
		{
			// Prefer a standard helm artefact; fall back to a plain themed hat only if the
			// catalogue has nothing wearable for the helm layer.
			Item hat = CreateThemeHat();

			if ( SwapWithArtefact( hat ) )
				return;

			AddItem( hat );
		}

		private Item CreateThemeHat()
		{
			switch ( Utility.Random( 14 ) )
			{
				default:
				case 0:  return new FloppyHat( MementoCoin.MementoHue );
				case 1:  return new WideBrimHat( MementoCoin.MementoHue );
				case 2:  return new Cap( MementoCoin.MementoHue );
				case 3:  return new SkullCap( MementoCoin.MementoHue );
				case 4:  return new Bandana( MementoCoin.MementoHue );
				case 5:  return new StrawHat( MementoCoin.MementoHue );
				case 6:  return new TallStrawHat( MementoCoin.MementoHue );
				case 7:  return new WizardsHat( MementoCoin.MementoHue );
				case 8:  return new FeatheredHat( MementoCoin.MementoHue );
				case 9:  return new TricorneHat( MementoCoin.MementoHue );
				case 10: return new PirateHat( MementoCoin.MementoHue );
				case 11: return new ClothHood( MementoCoin.MementoHue );
				case 12: return new WitchHat( MementoCoin.MementoHue );
				case 13: return new Bonnet( MementoCoin.MementoHue );
			}
		}

		private static Dictionary<Layer, List<string>> m_ArtifactsByLayer;

		private static Dictionary<Layer, List<string>> ArtifactsByLayer
		{
			get
			{
				if ( m_ArtifactsByLayer == null )
				{
					m_ArtifactsByLayer = new Dictionary<Layer, List<string>>();

					foreach ( ArtefactBook.ArtefactEntry entry in ArtefactBook.Entries )
					{
						if ( entry.Layer == Layer.Invalid )
							continue;

						List<string> list;

						if ( !m_ArtifactsByLayer.TryGetValue( entry.Layer, out list ) )
							m_ArtifactsByLayer[entry.Layer] = list = new List<string>();

						list.Add( entry.TypeName );
					}
				}

				return m_ArtifactsByLayer;
			}
		}

		private bool SwapWithArtefact( Item baseItem )
		{
			if ( baseItem == null || baseItem.Deleted )
				return false;

			List<string> options;

			if ( !ArtifactsByLayer.TryGetValue( baseItem.Layer, out options ) || options.Count == 0 )
				return false;

			// Try the layer's artefacts in random order so a gender-locked or otherwise
			// unusable entry does not leave the collector in plain leather.
			List<string> shuffled = new List<string>( options );

			for ( int i = shuffled.Count - 1; i > 0; --i )
			{
				int j = Utility.Random( i + 1 );
				string swap = shuffled[i];
				shuffled[i] = shuffled[j];
				shuffled[j] = swap;
			}

			for ( int i = 0; i < shuffled.Count; ++i )
			{
				Item artefact = ArtefactBook.CreateArtefact( shuffled[i] );

				if ( artefact == null || artefact.Deleted || artefact.Layer != baseItem.Layer || !CanWear( artefact ) )
				{
					if ( artefact != null )
						artefact.Delete();

					continue;
				}

				baseItem.Delete();
				AddItem( artefact );
				return true;
			}

			return false;
		}

		private bool CanWear( Item artefact )
		{
			if ( artefact is BaseArmor )
			{
				BaseArmor armor = (BaseArmor)artefact;
				return Female ? armor.AllowFemaleWearer : armor.AllowMaleWearer;
			}

			if ( artefact is BaseClothing )
			{
				BaseClothing clothing = (BaseClothing)artefact;
				return Female ? clothing.AllowFemaleWearer : clothing.AllowMaleWearer;
			}

			return true;
		}

		private static List<string> m_WieldableArtefacts;

		private static List<string> WieldableArtefacts
		{
			get
			{
				if ( m_WieldableArtefacts == null )
				{
					m_WieldableArtefacts = new List<string>();

					List<string> options;

					if ( ArtifactsByLayer.TryGetValue( Layer.OneHanded, out options ) )
					{
						for ( int i = 0; i < options.Count; ++i )
						{
							Item artefact = ArtefactBook.CreateArtefact( options[i] );

							if ( artefact == null )
								continue;

							try
							{
								if ( !artefact.Deleted && artefact is BaseWeapon && artefact.Layer == Layer.OneHanded )
									m_WieldableArtefacts.Add( options[i] );
							}
							finally
							{
								artefact.Delete();
							}
						}
					}
				}

				return m_WieldableArtefacts;
			}
		}

		private void WieldRandomArtefact()
		{
			List<string> options = WieldableArtefacts;

			if ( options.Count == 0 )
				return;

			string typeName = options[Utility.Random( options.Count )];
			Item artefact = ArtefactBook.CreateArtefact( typeName );

			if ( artefact == null || artefact.Deleted || !(artefact is BaseWeapon) || artefact.Layer != Layer.OneHanded )
			{
				if ( artefact != null )
					artefact.Delete();

				return;
			}

			AddItem( artefact );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( from == null || dropped == null || dropped.Deleted )
				return false;

			if ( dropped is MementoCoin )
				return HandleCoinTurnIn( from, (MementoCoin)dropped );

			int coins = GetCoinValue( dropped.ArtifactLevel );

			if ( coins <= 0 )
			{
				this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "I only pay Memento Coins for artefacts.", from.NetState );
				return false;
			}

			// Consume the artefact before paying so it can never be duplicated.
			dropped.Delete();

			from.AddToBackpack( new MementoCoin( coins ) );
			from.SendMessage( "You receive " + coins + " Memento Coins for that artefact." );
			this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "A fine piece. Here are your coins.", from.NetState );
			from.PlaySound( 0x2E5 );

			return true;
		}

		private bool HandleCoinTurnIn( Mobile from, MementoCoin coins )
		{
			const int bookCost = 75;

			if ( coins.Amount < bookCost )
			{
				this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "Bring me at least 75 Memento Coins and I will trade you a Tome of Wondrous Artifacts.", from.NetState );
				return false;
			}

			int remainder = coins.Amount - bookCost;

			// Consume the entire dropped pile, then return any leftover coins.
			coins.Delete();

			if ( remainder > 0 )
				from.AddToBackpack( new MementoCoin( remainder ) );

			from.AddToBackpack( new ArtefactBook() );

			from.SendMessage( "You hand over 75 Memento Coins and receive a Tome of Wondrous Artifacts." );
			this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "A pleasure doing business. See if one of its artefacts catches your eye.", from.NetState );
			from.PlaySound( 0x2E5 );

			return true;
		}

		/// <summary>Returns the number of Memento Coins paid for the given artifact tier.</summary>
		public static int GetCoinValue( ArtifactLevel level )
		{
			switch ( level )
			{
				case ArtifactLevel.LegendaryArtefact:
					return LegendaryCoins;
				case ArtifactLevel.StandardArtefact:
					return StandardCoins;
				case ArtifactLevel.Artifact:
					return MinorArtifactCoins;
				case ArtifactLevel.DecorativeArtefact:
					return DecorativeCoins;
				default:
					return 0;
			}
		}

		public MementoCollector( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			// Keep already-saved collectors invulnerable too.
			Blessed = true;
		}
	}
}
