using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;

namespace Server.Items
{
	public class ArtefactBook : Item
	{
		/// <summary>Number of artefacts shown on a single page (2 columns of 8).</summary>
		public const int ArtefactsPerPage = 16;

		/// <summary>Only artefacts of this level are listed by the book.</summary>
		public const ArtifactLevel RequiredArtefactLevel = ArtifactLevel.StandardArtefact;

		private static List<ArtefactEntry> m_Entries;

		private int m_Charges;

		[Constructable]
		public ArtefactBook() : base( 0x22C5 )
		{
			Weight = 1.0;
			Hue = MementoCoin.MementoHue;
			Name = "Tome of Wondrous Artifacts";
			m_Charges = 1;
		}

		public ArtefactBook( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get { return m_Charges; }
			set { m_Charges = value; InvalidateProperties(); }
		}

		public override string DefaultDescription
		{
			get
			{
				return "This book lists every standard artefact in the world. Double-click it to browse the catalogue and claim an artefact of your choice. A negative number of charges allows the book to be used without limit.";
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Charges < 0 )
				list.Add( "Charges: Unlimited" );
			else
				list.Add( 1060741, m_Charges.ToString() ); // charges: ~1_val~
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from == null || from.Backpack == null )
				return;

			if ( m_Charges == 0 )
			{
				from.SendMessage( "The book is empty." );
				return;
			}

			if ( !CanBeUsedBy( from ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
				return;
			}

			from.CloseGump( typeof( ArtefactBookGump ) );
			from.SendGump( new ArtefactBookGump( from, this, 0 ) );
		}

		public bool CanBeUsedBy( Mobile from )
		{
			if ( from == null || Deleted )
				return false;

			if ( IsChildOf( from.Backpack ) )
				return true;

			return from.InRange( GetWorldLocation(), 3 );
		}

		/// <summary>Consumes a single charge. Returns false when the book has no charges left.</summary>
		public bool ConsumeCharge()
		{
			if ( m_Charges < 0 )
				return true;

			if ( m_Charges == 0 )
				return false;

			m_Charges--;
			InvalidateProperties();
			return true;
		}

		/// <summary>The complete, ordered catalogue of standard artefacts.</summary>
		public static List<ArtefactEntry> Entries
		{
			get
			{
				if ( m_Entries == null )
					m_Entries = BuildEntries();

				return m_Entries;
			}
		}

		private static List<ArtefactEntry> BuildEntries()
		{
			List<ArtefactEntry> entries = new List<ArtefactEntry>();

			// ArtifactQuestList is the master artefact catalogue used by the relic quests.
			// It numbers every artefact sequentially from 1, so keep reading until it returns
			// an empty type name. This keeps the book in sync whenever artefacts are added to
			// or removed from that list.
			for ( int i = 1; i <= 10000; ++i )
			{
				string typeName = ArtifactQuestList.GetArtifact( i, 2 );

				if ( String.IsNullOrEmpty( typeName ) )
					break;

				// Only list artefacts that are actually flagged as standard artefacts. Some
				// entries in the catalogue are lower tier (Artifact) or decorative artefacts.
				Item artefact = CreateArtefact( typeName );

				if ( artefact == null )
					continue;

				Layer layer = Layer.Invalid;

				try
				{
					if ( artefact.Deleted || artefact.ArtifactLevel != RequiredArtefactLevel )
						continue;

					layer = artefact.Layer;
				}
				finally
				{
					artefact.Delete();
				}

				string name = ArtifactQuestList.GetArtifact( i, 1 );

				if ( String.IsNullOrEmpty( name ) )
					name = typeName;

				entries.Add( new ArtefactEntry( name, typeName, layer ) );
			}

			return entries;
		}

		/// <summary>Creates a fresh instance of an artefact from its type name.</summary>
		public static Item CreateArtefact( string typeName )
		{
			if ( String.IsNullOrEmpty( typeName ) )
				return null;

			Type type = ScriptCompiler.FindTypeByName( typeName );

			if ( type == null || !typeof( Item ).IsAssignableFrom( type ) )
				return null;

			try
			{
				return Activator.CreateInstance( type ) as Item;
			}
			catch ( Exception e )
			{
				Console.WriteLine( "ArtefactBook: failed to create artefact '{0}': {1}", typeName, e.Message );
				return null;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Charges = reader.ReadInt();
					break;
				}
				default:
				{
					m_Charges = 1;
					break;
				}
			}
		}

		public class ArtefactEntry
		{
			public string Name { get; private set; }
			public string TypeName { get; private set; }
			public Layer Layer { get; private set; }

			public ArtefactEntry( string name, string typeName, Layer layer )
			{
				Name = name;
				TypeName = typeName;
				Layer = layer;
			}
		}
	}

	public class ArtefactBookGump : Gump
	{
		private const string TitleColor = "#81db9f";
		private const int PageButtonBase = 100000;   // list: previous / next page
		private const int DetailButtonBase = 200000; // list: open the artefact's detail page
		private const int ClaimButtonBase = 300000;  // detail: claim the artefact
		private const int BackButtonBase = 400000;   // detail: return to the list

		private readonly ArtefactBook m_Book;

		// Unique, code-driven powers that are not captured by the standard stat sheet.
		// Keyed by artefact type name.
		private static readonly Dictionary<string, string> m_SpecialPowers = new Dictionary<string, string>
		{
			{ "Artifact_GandalfsStaff", "Double-click this staff while holding it to call forth a dragon to fight at your side. The magic can be invoked once every two hours." },
			{ "Artifact_StaffofSnakes", "Double-click this staff while holding it to summon a swarm of serpents. It may be used once every hour." },
			{ "Artifact_HammerofThor", "Double-click this hammer while holding it to hurl a bolt of lightning at your target." },
			{ "Artifact_HelmOfBrilliance", "Double-click this helm while wearing it to unleash a fireball." },
			{ "Artifact_RobeOfTeleportation", "Double-click this robe while wearing it to cast a teleport and move instantly to a chosen place." },
			{ "Artifact_AcidProofRobe", "Double-click this robe while wearing it to squeeze acid from its cloth into an empty bottle." },
			{ "Artifact_RodOfResurrection", "Double-click this rod while holding it, then target a fallen friend, pet or henchman to restore them to life." },
			{ "Artifact_DaggerOfVenom", "Wounds from this dagger may poison your victim, from lesser poison up to deadly poison." },
			{ "Artifact_BootsofHermes", "Wearing these boots grants unnatural speed, letting you sprint across the land." },
			{ "Artifact_SprintersSandals", "Wearing these sandals grants unnatural speed, letting you sprint across the land." },
		};

		public ArtefactBookGump( Mobile from, ArtefactBook book, int page ) : base( 100, 100 )
		{
			m_Book = book;

			from.SendSound( 0x55 );

			List<ArtefactBook.ArtefactEntry> entries = ArtefactBook.Entries;
			int totalPages = Math.Max( 1, ( entries.Count + ArtefactBook.ArtefactsPerPage - 1 ) / ArtefactBook.ArtefactsPerPage );

			if ( page < 0 )
				page = 0;
			else if ( page >= totalPages )
				page = totalPages - 1;

			int start = page * ArtefactBook.ArtefactsPerPage;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			AddImage( 0, 0, 7005, 2964 );
			AddImage( 0, 0, 7006 );
			AddImage( 0, 0, 7024, 2736 );
			AddButton( 590, 48, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			int[] columnX = { 115, 407 };
			const int startY = 98;
			const int rowHeight = 34;
			const int rowsPerColumn = ArtefactBook.ArtefactsPerPage / 2;

			for ( int i = 0; i < ArtefactBook.ArtefactsPerPage; ++i )
			{
				int index = start + i;

				if ( index >= entries.Count )
					break;

				int column = i / rowsPerColumn;
				int row = i % rowsPerColumn;
				int x = columnX[column];
				int y = startY + ( row * rowHeight );

				ArtefactBook.ArtefactEntry entry = entries[index];

				AddButton( x, y, 2447, 2447, DetailButtonBase + index, GumpButtonType.Reply, 0 );
				AddHtml( x + 20, y, 155, 20, String.Format( "<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", TitleColor, Utility.FixHtml( entry.Name ) ), false, false );
			}

			int prevPage = page - 1 < 0 ? totalPages - 1 : page - 1;
			int nextPage = page + 1 >= totalPages ? 0 : page + 1;

			AddButton( 75, 374, 4014, 4014, PageButtonBase + prevPage, GumpButtonType.Reply, 0 );
			AddButton( 590, 375, 4005, 4005, PageButtonBase + nextPage, GumpButtonType.Reply, 0 );

			AddHtml( 455, 374, 125, 20, String.Format( "<BODY><BASEFONT Color={0}><RIGHT>Page {1} of {2}</RIGHT></BASEFONT></BODY>", TitleColor, page + 1, totalPages ), false, false );
		}

		public ArtefactBookGump( Mobile from, ArtefactBook book, int page, int selectedIndex ) : base( 100, 100 )
		{
			m_Book = book;

			from.SendSound( 0x55 );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			AddImage( 0, 0, 7005, 2964 );
			AddImage( 0, 0, 7006 );
			AddImage( 0, 0, 7024, 2736 );
			AddButton( 590, 48, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			List<ArtefactBook.ArtefactEntry> entries = ArtefactBook.Entries;

			if ( selectedIndex >= 0 && selectedIndex < entries.Count )
				AddDetail( entries[selectedIndex], selectedIndex );
		}

		private void AddDetail( ArtefactBook.ArtefactEntry entry, int index )
		{
			// Name centred at the top of the left page, directly over the picture.
			AddHtml( 80, 49, 230, 20, String.Format( "<BODY><BASEFONT Color={0}><CENTER>{1}</CENTER></BASEFONT></BODY>", TitleColor, Utility.FixHtml( entry.Name ) ), false, false );

			string stats = null;
			Item preview = ArtefactBook.CreateArtefact( entry.TypeName );

			if ( preview != null && !preview.Deleted )
			{
				try
				{
					// Picture on the left page.
					if ( preview.ItemID > 0 )
						GumpUtilities.AddCenteredItemToGump( this, preview.ItemID, 95, 105, 200, 200, preview.Hue );

					stats = ItemProps.ItemProperties( preview, false );
				}
				catch
				{
					stats = null;
				}
				finally
				{
					preview.Delete();
				}
			}

			if ( String.IsNullOrEmpty( stats ) )
				stats = "<BASEFONT COLOR=#FFF000>" + Utility.FixHtml( entry.Name ) + "</BASEFONT><BR>No further details are known about this artefact.<BR>";

			string special = GetSpecialPower( entry.TypeName );

			if ( !String.IsNullOrEmpty( special ) )
				stats = "<BASEFONT COLOR=#C6D11C><B>UNIQUE POWER</B></BASEFONT><BR>" + special + "<BR><BR>" + stats;

			// Information on the right page.
			AddHtml( 380, 86, 240, 266, "<BODY><BASEFONT Color=" + TitleColor + ">" + stats + "</BASEFONT></BODY>", false, true );

			int listPage = index / ArtefactBook.ArtefactsPerPage;

			AddButton( 75, 374, 4014, 4014, BackButtonBase + listPage, GumpButtonType.Reply, 0 );
			AddHtml( 98, 378, 70, 20, String.Format( "<BODY><BASEFONT Color={0}><CENTER>Back</CENTER></BASEFONT></BODY>", TitleColor ), false, false );

			AddButton( 590, 375, 0x837, 0x838, ClaimButtonBase + index, GumpButtonType.Reply, 0 );
			AddHtml( 455, 378, 125, 20, String.Format( "<BODY><BASEFONT Color={0}><RIGHT>Claim</RIGHT></BASEFONT></BODY>", TitleColor ), false, false );
		}

		private static string GetSpecialPower( string typeName )
		{
			string text;

			if ( typeName != null && m_SpecialPowers.TryGetValue( typeName, out text ) )
				return text;

			return null;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( from == null || from.Backpack == null )
				return;

			if ( m_Book == null || m_Book.Deleted )
				return;

			int buttonID = info.ButtonID;

			if ( buttonID == 0 )
				return;

			if ( buttonID >= BackButtonBase )
			{
				int page = buttonID - BackButtonBase;
				from.SendGump( new ArtefactBookGump( from, m_Book, page ) );
				return;
			}

			if ( buttonID >= ClaimButtonBase )
			{
				ClaimArtefact( from, buttonID - ClaimButtonBase );
				return;
			}

			if ( buttonID >= DetailButtonBase )
			{
				int index = buttonID - DetailButtonBase;
				List<ArtefactBook.ArtefactEntry> entries = ArtefactBook.Entries;

				if ( index < 0 || index >= entries.Count )
					return;

				int page = index / ArtefactBook.ArtefactsPerPage;
				from.SendGump( new ArtefactBookGump( from, m_Book, page, index ) );
				return;
			}

			if ( buttonID >= PageButtonBase )
			{
				int page = buttonID - PageButtonBase;
				from.SendGump( new ArtefactBookGump( from, m_Book, page ) );
				return;
			}
		}

		private void ClaimArtefact( Mobile from, int index )
		{
			List<ArtefactBook.ArtefactEntry> entries = ArtefactBook.Entries;

			if ( index < 0 || index >= entries.Count )
				return;

			int selectedPage = index / ArtefactBook.ArtefactsPerPage;

			if ( m_Book.Charges == 0 )
			{
				from.SendMessage( "The book is empty." );
				from.CloseGump( typeof( ArtefactBookGump ) );
				return;
			}

			ArtefactBook.ArtefactEntry entry = entries[index];
			Item reward = ArtefactBook.CreateArtefact( entry.TypeName );

			if ( reward == null || reward.Deleted )
			{
				if ( reward != null )
					reward.Delete();

				from.SendMessage( "That artefact could not be created." );
				from.SendGump( new ArtefactBookGump( from, m_Book, selectedPage ) );
				return;
			}

			if ( !from.AddToBackpack( reward ) )
			{
				// AddToBackpack drops the item at the player's feet when the pack is full.
				from.SendMessage( "Your backpack is too full, so the artefact has been placed at your feet." );
			}

			PlayerMobile pm = from as PlayerMobile;

			if ( pm != null )
				LoggingFunctions.LogCreatedArtifact( pm, entry.Name );

			from.SendMessage( "You have claimed the artefact: " + entry.Name + "." );
			from.FixedParticles( 0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot );
			from.PlaySound( 0x208 );

			m_Book.ConsumeCharge();

			if ( m_Book.Charges == 0 )
			{
				m_Book.Delete();
				from.CloseGump( typeof( ArtefactBookGump ) );
				from.SendMessage( "The book crumbles to dust as its final artefact is claimed." );
			}
			else
			{
				from.SendGump( new ArtefactBookGump( from, m_Book, selectedPage ) );
			}
		}
	}
}

