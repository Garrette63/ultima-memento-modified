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

			// Leather armour, with random artefact swaps.
			DressArmour();

			// Random cloth hat matching the cloak and sash.
			AddThemeHat();

			AddItem( new Cloak( MementoCoin.MementoHue ) );
			AddItem( new BodySash( MementoCoin.MementoHue ) );
			AddItem( new Boots( DarkLeatherHue ) );

			// Hold a lit lantern by default.
			Lantern lantern = new Lantern();
			lantern.Hue = MementoCoin.MementoHue;
			AddItem( lantern );
			lantern.Ignite();

			WieldRandomArtefact();

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
			List<Item> pieces = new List<Item>();

			pieces.Add( AddLeather( new LeatherGorget() ) );
			pieces.Add( AddLeather( new LeatherArms() ) );
			pieces.Add( AddLeather( new LeatherGloves() ) );
			pieces.Add( AddLeather( new LeatherLegs() ) );
			pieces.Add( AddLeather( new LeatherChest() ) );

			int swapped = 0;

			for ( int i = 0; i < pieces.Count; ++i )
			{
				if ( Utility.Random( 2 ) == 1 && SwapWithArtefact( pieces[i] ) )
					++swapped;
			}

			// Guarantee at least one artefact piece so the swap is visible.
			if ( swapped == 0 )
			{
				int start = Utility.Random( pieces.Count );

				for ( int i = 0; i < pieces.Count; ++i )
				{
					if ( SwapWithArtefact( pieces[(start + i) % pieces.Count] ) )
						break;
				}
			}
		}

		private Item AddLeather( Item leather )
		{
			leather.Hue = DarkLeatherHue;
			AddItem( leather );
			return leather;
		}

		private void AddThemeHat()
		{
			Item hat;

			switch ( Utility.Random( 14 ) )
			{
				default:
				case 0:  hat = new FloppyHat( MementoCoin.MementoHue ); break;
				case 1:  hat = new WideBrimHat( MementoCoin.MementoHue ); break;
				case 2:  hat = new Cap( MementoCoin.MementoHue ); break;
				case 3:  hat = new SkullCap( MementoCoin.MementoHue ); break;
				case 4:  hat = new Bandana( MementoCoin.MementoHue ); break;
				case 5:  hat = new StrawHat( MementoCoin.MementoHue ); break;
				case 6:  hat = new TallStrawHat( MementoCoin.MementoHue ); break;
				case 7:  hat = new WizardsHat( MementoCoin.MementoHue ); break;
				case 8:  hat = new FeatheredHat( MementoCoin.MementoHue ); break;
				case 9:  hat = new TricorneHat( MementoCoin.MementoHue ); break;
				case 10: hat = new PirateHat( MementoCoin.MementoHue ); break;
				case 11: hat = new ClothHood( MementoCoin.MementoHue ); break;
				case 12: hat = new WitchHat( MementoCoin.MementoHue ); break;
				case 13: hat = new Bonnet( MementoCoin.MementoHue ); break;
			}

			AddItem( hat );
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

		private bool SwapWithArtefact( Item leather )
		{
			if ( leather == null || leather.Deleted )
				return false;

			List<string> options;

			if ( !ArtifactsByLayer.TryGetValue( leather.Layer, out options ) || options.Count == 0 )
				return false;

			string typeName = options[Utility.Random( options.Count )];
			Item artefact = ArtefactBook.CreateArtefact( typeName );

			if ( artefact == null || artefact.Deleted || artefact.Layer != leather.Layer || !CanWear( artefact ) )
			{
				if ( artefact != null )
					artefact.Delete();

				return false;
			}

			leather.Delete();
			AddItem( artefact );
			return true;
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
