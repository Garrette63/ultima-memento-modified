using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class TransmogrifyGump : Gump
	{
		private const int PerPage = 8;    // 2 columns x 4 rows
		private const int ColX0 = 22;
		private const int ColX1 = 286;
		private const int RowY0 = 232;
		private const int RowH = 76;

		private const int ButtonSelect = 100;   // 100 + option index
		private const int ButtonApply = 950;
		private const int ButtonRevert = 903;
		private const int ButtonGear = 902;

		// ---- anchoring ---------------------------------------------------------
		// The picker always opens at the same place so it pairs predictably with the
		// paperdoll (whose window the server cannot position or pin). The gump is
		// aligned by its RIGHT edge: set AnchorRight to the screen column you want
		// that edge on and the X follows. Sized for a 1024-wide client; on an
		// 800-wide client use AnchorRight = 790 so the 560px gump still fits.
		private const int GumpWidth = 560;
		private const int GumpHeight = 600;
		private const int AnchorRight = 1010;
		private const int AnchorTop = 40;

		private readonly Item m_Item;
		private readonly Item m_Scroll;   // one-shot scroll: spent only when a new appearance is Applied
		private readonly int m_Selected;  // -1 when nothing has been picked yet
		private readonly bool m_Revert;   // original art staged; Apply commits it
		private readonly int m_Page;
		private readonly bool m_ShowGear;  // false: naked doll showing only the target
		private readonly int m_GoldCost;   // gypsy's fee, taken on Apply; 0 = free
		private readonly TransmogrifyPreview m_Preview;

		public TransmogrifyGump( Mobile from, Item item ) : this( from, item, null, -1, 0, false, 0 )
		{
		}

		public TransmogrifyGump( Mobile from, Item item, Item scroll ) : this( from, item, scroll, -1, 0, false, 0 )
		{
		}

		public TransmogrifyGump( Mobile from, Item item, Item scroll, int selected, int page ) : this( from, item, scroll, selected, page, false, 0 )
		{
		}

		public TransmogrifyGump( Mobile from, Item item, Item scroll, int selected, int page, bool showGear ) : this( from, item, scroll, selected, page, showGear, 0 )
		{
		}

		public TransmogrifyGump( Mobile from, Item item, Item scroll, int selected, int page, bool showGear, int goldCost ) : this( from, item, scroll, selected, page, showGear, goldCost, false )
		{
		}

		public TransmogrifyGump( Mobile from, Item item, Item scroll, int selected, int page, bool showGear, int goldCost, bool revert ) : base( AnchorRight - GumpWidth, AnchorTop )
		{
			m_Item = item;
			m_Scroll = scroll;
			m_Selected = revert ? -1 : selected;
			m_Page = page;
			m_ShowGear = showGear;
			m_GoldCost = goldCost;
			m_Revert = revert;
			m_Generation = NextGeneration( from );

			List<TransmogrifyOption> options = TransmogrifyCatalog.OptionsFor( item );

			// The recorded pre-transmog art, or the item's current art when nothing has
			// been logged yet. Both Revert and the ORIGINAL pane use this.
			int originalArt;

			if ( !TransmogrifyRegistry.TryGetOriginal( item, out originalArt ) )
				originalArt = item.ItemID;

			// ---- the paperdoll model: the player's own body and outfit with this slot
			// swapped. The mobile is announced once (Present); the window is only asked
			// for when this draw wants it, because every button reply closes the gump
			// client-side and the rebuild must not re-announce the mobile - that is what
			// used to take the paperdoll down with it.
			TransmogrifyPreview preview = null;

			try
			{
				preview = TransmogrifyPreview.For( from, item );

				// the doll is naked apart from the piece being changed unless the player
				// has turned the rest of their outfit on
				preview.SetGear( m_ShowGear, item, from );

				// make sure the slot holds what this draw is showing - the previewed
				// option, or the item's own art when nothing has been picked yet
				int focusArt;

				if ( m_Revert )
					focusArt = originalArt;
				else if ( m_Selected >= 0 && m_Selected < options.Count )
					focusArt = options[m_Selected].ItemID;
				else
					focusArt = item.ItemID;

				if ( !preview.IsFocusedOn( item, focusArt ) )
					preview.SetFocus( item, focusArt, from );

				// Announce the model once. The paperdoll itself is NOT requested here:
				// the caller asks for it after SendGump, so the window request lands after
				// the gump that goes with it (see OpenPaperdoll).
				if ( !preview.Presented )
					preview.Present( from );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[Transmogrify] paperdoll preview failed: {0}", ex );

				if ( preview != null )
				{
					try
					{
						preview.Dismiss( from );
					}
					catch
					{
					}
				}

				preview = null;
			}

			m_Preview = preview;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			// Every appearance is drawn in the *item's* own hue - the list arts, the big
			// picture and the paperdoll - so a preview reads as "this item, in that
			// shape" and never as a differently coloured one.
			int shownArt = m_Revert ? originalArt : item.ItemID;
			int shownHue = item.Hue;
			string shownName = ItemLabel( item );
			bool picking = false;

			if ( !m_Revert && m_Selected >= 0 && m_Selected < options.Count )
			{
				TransmogrifyOption sel = options[m_Selected];

				shownArt = sel.ItemID;
				shownName = sel.Name;
				picking = true;
			}

			// ---- page 0: frame, title, the item shown and the slot line. These stay on
			// screen while the option pages are flipped.
			AddPage( 0 );

			AddBackground( 0, 0, GumpWidth, GumpHeight, 0x1453 );

			AddImageTiled( 12, 12, 536, 22, 0xA40 );
			AddAlphaRegion( 12, 12, 536, 22 );
			AddHtml( 20, 14, 520, 20, Text( "#FFFF66", "<B>Transmogrify Equipment</B>" ), false, false );

			// ---- header: the item as it truly is on the left, the choice on the right

			// left - original appearance and the item's actual name
			AddImageTiled( 16, 44, 256, 120, 0xA40 );
			AddAlphaRegion( 16, 44, 256, 120 );
			AddHtml( 20, 48, 248, 18, Text( "#9FB0BD", "<CENTER>ORIGINAL</CENTER>" ), false, false );
			AddItem( 122, 70, originalArt, shownHue );
			AddHtml( 20, 118, 248, 44, Text( "#FFFFFF", "<CENTER>" + Safe( ItemLabel( item ) ) + "</CENTER>" ), false, false );

			// right - what is being considered (or the item as it stands right now)
			AddImageTiled( 288, 44, 256, 120, 0xA40 );
			AddAlphaRegion( 288, 44, 256, 120 );
			AddHtml( 292, 48, 248, 18, Text( ( picking || m_Revert ) ? "#FFFF66" : "#9FB0BD", "<CENTER>" + ( m_Revert ? "REVERT" : "CURRENT" ) + "</CENTER>" ), false, false );
			AddItem( 394, 70, shownArt, shownHue );
			AddHtml( 292, 118, 248, 44, Text( ( picking || m_Revert ) ? "#FFFF66" : "#FFFFFF", "<CENTER>" + Safe( shownName ) + "</CENTER>" ), false, false );

			string slot;

			if ( m_Item is BaseShield )
				slot = "Shield";
			else if ( m_Item is BaseWeapon )
				slot = ( (BaseWeapon) m_Item ).Skill + " weapon";
			else
				slot = m_Item.EquipLayerName( m_Item.Layer );

			string count = options.Count + " appearance" + ( options.Count == 1 ? "" : "s" );

			// header row: slot on the left, two actions hard right. Both labels are
			// right-aligned in a box that ends 6px before its arrow, so the gap is the
			// same whatever the text length ("Gear off" vs "Revert").
			AddHtml( 24, 200, 290, 20, Text( "#B0C4DE", "slot: " + Safe( slot ) + "  \u2013  " + count ), false, false );

			// right-hand labels are measured back from their arrow: Next sits 6px short of
			// its arrow (it reads best tight), Gear and Revert 10px; the left-hand labels
			// (Back/Cancel/Apply) keep the +30 reference offset used everywhere else.
			AddHtml( 316, 200, 70, 20, Text( m_ShowGear ? "#FFFF66" : "#B0C4DE", "<DIV ALIGN=RIGHT>Gear Toggle</DIV>" ), false, false );
			AddButton( 396, 200, 0xFA5, 0xFA7, ButtonGear, GumpButtonType.Reply, 0 );

			AddHtml( 406, 200, 74, 20, Text( "#B0C4DE", "<DIV ALIGN=RIGHT>Revert</DIV>" ), false, false );
			AddButton( 490, 200, 0xFA5, 0xFA7, ButtonRevert, GumpButtonType.Reply, 0 );

			// ---- the choices. A client always opens a gump on page 1, so the page the
			// player was last looking at is emitted FIRST; the visible numbering (the
			// counter and the pager) stays logical, which is what m_Page carries.
			int pages = ( options.Count + PerPage - 1 ) / PerPage;

			if ( pages < 1 )
				pages = 1;

			int start = m_Page;

			if ( start < 0 || start >= pages )
				start = 0;

			for ( int n = 0; n < pages; n++ )
			{
				int p = ( start + n ) % pages;

				AddPage( n + 1 );

				int first = p * PerPage;
				int last = Math.Min( first + PerPage, options.Count );

				for ( int i = first; i < last; i++ )
				{
					TransmogrifyOption opt = options[i];
					int slotIndex = i - first;

					int x = ( slotIndex % 2 == 0 ) ? ColX0 : ColX1;
					int y = RowY0 + ( slotIndex / 2 ) * RowH;

					if ( i == m_Selected )
					{
						// highlight the option currently shown at the top and on the doll
						AddImageTiled( x - 8, y - 8, 216, 62, 0xA40 );
						AddAlphaRegion( x - 8, y - 8, 216, 62 );
					}

					AddItem( x, y, opt.ItemID, shownHue );
					AddButton( x + 62, y + 14, 0xFA5, 0xFA7, ButtonSelect + i, GumpButtonType.Reply, 0 );

					// names use the client's plain font: the bold face is cramped at this
					// size; the selected entry is picked out by colour instead
					AddHtml( x + 96, y + 14, 116, 44, Text( i == m_Selected ? "#FFFF66" : "#FFFFFF", Safe( opt.Name ) ), false, false );
				}

				if ( pages > 1 )
				{
					// carousel: both directions always work, from any page. Back keeps the +30
					// reference offset to the right of its arrow; Next is right-aligned and
					// deliberately runs into its arrow (see below).
					AddButton( 20, 524, 0xFAE, 0xFB0, 0, GumpButtonType.Page, ( ( n - 1 + pages ) % pages ) + 1 );
					AddHtml( 50, 526, 60, 20, Text( "#FFFFFF", "Back" ), false, false );

					AddButton( 506, 524, 0xFA5, 0xFA7, 0, GumpButtonType.Page, ( ( n + 1 ) % pages ) + 1 );
					// the label box now runs 2px under the arrow: the client insets html text
					// inside its box, so this is what actually lands "Next" next to the arrow
					AddHtml( 463, 526, 45, 20, Text( "#FFFFFF", "<DIV ALIGN=RIGHT>Next</DIV>" ), false, false );

					AddHtml( 210, 526, 140, 20, Text( "#B0C4DE", "<CENTER>" + ( p + 1 ) + " / " + pages + "</CENTER>" ), false, false );
				}
			}

			// ---- apply / cancel, drawn on page 0 so they are always reachable.
			// Every label sits 23px right of its button (arrow ~15px + 8px gap), the
			// same spacing the pager and the header row use.
			AddPage( 0 );

			AddButton( 194, 562, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
			AddHtml( 224, 564, 60, 20, Text( "#FFFFFF", "Cancel" ), false, false );

			AddButton( 285, 562, 0xFA5, 0xFA7, ButtonApply, GumpButtonType.Reply, 0 );
			AddHtml( 315, 564, 160, 20, Text( ( picking || m_Revert ) ? "#FFFF66" : "#B0C4DE", m_Revert ? "Apply Revert" : ( m_GoldCost > 0 ? "Apply (" + m_GoldCost + " gp)" : "Apply" ) ), false, false );
		}

		private static string Text( string colour, string html )
		{
			return "<BODY><BASEFONT Color=" + colour + ">" + html + "</BASEFONT></BODY>";
		}

		// The item's own name when it has one (artifacts, renamed gear), otherwise the
		// base art name. Deliberately different from TransmogrifyCatalog.BaseName, which
		// always names the art so that the option list reads consistently.
		private static string ItemLabel( Item item )
		{
			if ( item == null )
				return "";

			if ( !string.IsNullOrEmpty( item.Name ) )
				return item.Name;

			return TransmogrifyCatalog.BaseName( item );
		}

		// Keep stray markup out of the html blocks.
		private static string Safe( string s )
		{
			if ( string.IsNullOrEmpty( s ) )
				return "";

			return s.Replace( "<", "" ).Replace( ">", "" ).Replace( "&", "" );
		}

		private void Reopen( Mobile from, int selected, int page, bool showGear, bool revert )
		{
			from.SendGump( new TransmogrifyGump( from, m_Item, m_Scroll, selected, page, showGear, m_GoldCost, revert ) );
		}

		// The paperdoll request goes out AFTER the gump packet. Sent before it, some
		// clients open the window and immediately close it again when the gump arrives.
		public void OpenPaperdoll( Mobile from )
		{
			if ( m_Preview == null || from == null )
				return;

			try
			{
				m_Preview.Present( from );       // make sure the client knows the model
				m_Preview.ShowPaperdoll( from );
				m_Preview.SendEquipment( from ); // and dress it, including the target slot
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[Transmogrify] preview paperdoll failed: {0}", ex );
			}
		}

		// The picker must keep working even if the paperdoll model misbehaves.
		private void DismissPreview( Mobile from )
		{
			if ( m_Preview == null )
				return;

			try
			{
				m_Preview.Dismiss( from );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[Transmogrify] preview dismiss failed: {0}", ex );
			}
		}

		// Each redraw is a new gump instance; a replaced gump can still report a close
		// (button 0) after the newer one was sent, which must not dismiss the paperdoll.
		private static readonly Dictionary<Serial, int> m_Generations = new Dictionary<Serial, int>();
		private readonly int m_Generation;

		private static int NextGeneration( Mobile from )
		{
			if ( from == null )
				return 0;

			int gen;

			m_Generations.TryGetValue( from.Serial, out gen );
			m_Generations[from.Serial] = ++gen;

			return gen;
		}

		private static bool IsLatest( Mobile from, int generation )
		{
			if ( from == null )
				return true;

			int gen;

			return !m_Generations.TryGetValue( from.Serial, out gen ) || gen == generation;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( from == null || from.Deleted )
				return;

			if ( m_Item == null || m_Item.Deleted )
			{
				DismissPreview( from );

				return;
			}

			if ( !TransmogrifyCatalog.CanReshape( m_Item ) )
			{
				DismissPreview( from );

				from.SendMessage( "That item can no longer be reshaped." );
				return;
			}

			if ( !TransmogrifyTool.CanReach( from, m_Item ) )
			{
				DismissPreview( from );

				from.SendMessage( "You can only change armor, clothing or weapons you are wearing or carrying." );
				return;
			}

			int button = info.ButtonID;

			// Gump closed or Cancel pressed: both windows go, the scroll is kept. A gump
			// that was replaced has already closed client-side, so ignore its late report.
			if ( button == 0 )
			{
				if ( !IsLatest( from, m_Generation ) )
					return;

				DismissPreview( from );

				return;
			}

			List<TransmogrifyOption> options = TransmogrifyCatalog.OptionsFor( m_Item );

			if ( button == ButtonGear )
			{
				// flips the rest of the outfit on the doll; the paperdoll window is left
				// alone and simply re-dressed through RemoveItem/EquipUpdate
				Reopen( from, m_Selected, m_Page, !m_ShowGear, m_Revert );
				return;
			}

			if ( button == ButtonRevert )
			{
				// Stage the original look instead of applying it: the picker and the
				// paperdoll show it, and nothing about the item changes until Apply.
				int originalArt;

				if ( !TransmogrifyRegistry.TryGetOriginal( m_Item, out originalArt ) )
				{
					// First Revert on an item with no row: capture what it looks like
					// now as the original and keep that baseline from here on.
					TransmogrifyRegistry.EnsureOriginal( m_Item );

					from.SendMessage( "No earlier appearance was on record; the current one has been saved as the original." );
				}

				Reopen( from, -1, m_Page, m_ShowGear, true );

				from.SendMessage( "The original appearance is shown. Press Apply to keep it." );
				return;
			}

			if ( button == ButtonApply )
			{
				if ( m_Revert )
				{
					// Reverting is an undo, not a new look: it is free and does not
					// spend the tool. The record is kept, so a later transmog still
					// reverts to the same original.
					int originalArt;
					bool recorded = TransmogrifyRegistry.TryGetOriginal( m_Item, out originalArt );

					if ( !recorded )
						originalArt = TransmogrifyRegistry.EnsureOriginal( m_Item );

					bool changed = m_Item.ItemID != originalArt;

					if ( !TransmogrifyRegistry.Revert( m_Item ) )
					{
						// The row should have been ensured above. If it somehow vanished
						// between the two calls, still put the known art back rather than
						// sending the player down the "no recorded original" path.
						if ( originalArt <= 0 )
						{
							from.SendMessage( "That item has no recorded original appearance." );
							Reopen( from, -1, m_Page, m_ShowGear, true );
							return;
						}

						TransmogrifyTool.SetAppearance( m_Item, originalArt );
					}

					from.SendSound( 0x57 );
					DismissPreview( from );

					from.SendMessage( changed ? "The item's original appearance has been restored." : "The item already has its original appearance." );
					return;
				}

				if ( m_Selected < 0 || m_Selected >= options.Count )
				{
					from.SendMessage( "Choose an appearance first." );
					Reopen( from, m_Selected, m_Page, m_ShowGear, m_Revert );
					return;
				}

				TransmogrifyOption pick = options[m_Selected];

				// The gypsy's fee is taken here, with the change - not when the picker
				// opens - so changing your mind never costs anything.
				if ( m_GoldCost > 0 )
				{
					Container pack = from.Backpack;

					if ( pack == null || pack.GetAmount( typeof( Gold ) ) < m_GoldCost )
					{
						from.SendMessage( "You cannot afford the spirits' price of {0} gold.", m_GoldCost );
						Reopen( from, m_Selected, m_Page, m_ShowGear, m_Revert );
						return;
					}

					pack.ConsumeTotal( typeof( Gold ), m_GoldCost );
					from.SendMessage( "You pay {0} gold.", m_GoldCost );
				}

				// Record what the item looks like now, before it changes, so Revert can
				// put it back (first transmog only - later ones keep the same original).
				TransmogrifyRegistry.Remember( m_Item );

				// Only the appearance changes; the item keeps its own properties,
				// hue and slot. GraphicID follows when it simply mirrored ItemID.
				TransmogrifyTool.SetAppearance( m_Item, pick.ItemID );

				from.SendSound( 0x57 );

				DismissPreview( from );

				if ( m_Scroll != null && !m_Scroll.Deleted )
					m_Scroll.Delete();

				from.SendMessage( "The appearance has been changed to " + pick.Name + "." );
				return;
			}

			int index = button - ButtonSelect;

			if ( index >= 0 && index < options.Count )
			{
				// Select + preview only; Apply commits and spends the scroll. The rebuild
				// re-dresses the paperdoll through EquipUpdate, so the window is untouched.
				Reopen( from, index, index / PerPage, m_ShowGear, false );
			}
		}


	}
}
