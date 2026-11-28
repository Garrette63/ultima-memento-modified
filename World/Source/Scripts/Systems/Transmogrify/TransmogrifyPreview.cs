using System;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Items
{
	// A throw-away paperdoll model for the transmogrify picker.
	//
	// It copies the player's body, gender, skin, hair and worn outfit, then swaps
	// the target slot to whichever appearance is being previewed. It is built with
	// the full Mobile() initialiser (so Items, Skills, StatMods, ... all exist) and
	// is then taken straight back out of the world, which means it is never saved
	// and never becomes a real entity - the client of the one player who asked is
	// simply told about it with MobileUpdate/MobileIncoming packets and asked to
	// open its paperdoll. Map.Internal also makes Mobile.Delta() a no-op, so it
	// never generates any traffic of its own.
	//
	// Never construct this with base( Serial ): that is the deserialisation
	// constructor and skips DefaultMobileInit(), leaving Items/StatMods null (that
	// is what crashed the shard in entry #127).
	public class TransmogrifyPreview : Mobile
	{
		private static readonly Dictionary<Mobile, TransmogrifyPreview> m_Open = new Dictionary<Mobile, TransmogrifyPreview>();

		// One serial per player, allocated once from the server's own counter. Because
		// Serial.NewMobile is never re-issued, no real mobile can ever collide with it,
		// and clients that remember a paperdoll's position per serial will reopen the
		// preview exactly where the player parked it last time.
		private static readonly Dictionary<Serial, Serial> m_PreviewSerials = new Dictionary<Serial, Serial>();

		private static Serial SerialFor( Mobile owner )
		{
			Serial serial;

			if ( owner != null && m_PreviewSerials.TryGetValue( owner.Serial, out serial ) )
				return serial;

			serial = Serial.NewMobile;

			if ( owner != null )
				m_PreviewSerials[owner.Serial] = serial;

			return serial;
		}

		private readonly Mobile m_Owner;
		private Item m_Focus;
		private Timer m_Timeout;
		private bool m_Presented;
		private bool m_ShowGear;

		public bool Presented{ get{ return m_Presented; } }
		public bool ShowGear{ get{ return m_ShowGear; } }

		public TransmogrifyPreview( Serial serial, Mobile owner ) : base( serial )
		{
			// base( Serial ) is the deserialisation constructor and leaves Items,
			// Skills, StatMods, ... null, so run the full initialiser explicitly.
			DefaultMobileInit();

			// Fully formed but not part of World.Mobiles: never saved, never broadcast.
			World.RemoveMobile( this );

			m_Owner = owner;

			if ( owner == null )
				return;

			Name = owner.Name;
			BodyValue = owner.BodyValue;
			Female = owner.Female;
			Hue = owner.Hue;

			HairItemID = owner.HairItemID;
			HairHue = owner.HairHue;
			FacialHairItemID = owner.FacialHairItemID;
			FacialHairHue = owner.FacialHairHue;

			Direction = owner.Direction;

			Blessed = true;
			Frozen = true;
			Hidden = true;

			// Give the model real coordinates. A client judges a paperdoll by the mobile
			// it knows about, and a mobile standing at 0,0,0 is out of range: it opens the
			// window and drops it again immediately. It is moved onto Map.Internal (the
			// core never broadcasts that map to anyone) and then unregistered from
			// World.Mobiles, so it still cannot be seen by other clients or saved.
			MoveToWorld( owner.Location, Map.Internal );
			World.RemoveMobile( this );
		}

		// Previews are transient; one that somehow reached a save deletes itself on load.
		public TransmogrifyPreview( Serial serial ) : base( serial )
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

			/*int version = */reader.ReadInt();

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
		}

		// One live preview per player; re-drawing the gump reuses it.
		public static TransmogrifyPreview For( Mobile from, Item target )
		{
			TransmogrifyPreview preview;

			if ( from != null && target != null && m_Open.TryGetValue( from, out preview ) && preview != null && !preview.Deleted )
			{
				if ( preview.m_Focus == null || preview.m_Focus.Deleted || preview.m_Focus.Layer != target.Layer )
					preview.Dress( target, null );

				return preview;
			}

			preview = new TransmogrifyPreview( SerialFor( from ), from );

			if ( target != null )
				preview.Dress( target, null );

			if ( from != null )
				m_Open[from] = preview;

			return preview;
		}

		// Rebuilds the model's outfit. With ShowGear off the doll is naked apart from
		// the piece being previewed; with it on, the player's other worn items are
		// mirrored as well. `from` receives the changes (null while it is created).
		public void Dress( Item target, Mobile from )
		{
			if ( target == null )
				return;

			for ( int i = Items.Count - 1; i >= 0; i-- )
			{
				Item piece = Items[i];

				if ( from != null && from.NetState != null )
					from.Send( new RemoveItem( piece ) );

				piece.Delete();
			}

			m_Focus = null;

			if ( m_ShowGear && m_Owner != null )
			{
				for ( int i = 0; i < m_Owner.Items.Count; i++ )
				{
					Item worn = m_Owner.Items[i];
					Layer layer = worn.Layer;

					if ( layer == Layer.Backpack || layer == Layer.Mount || layer == Layer.Bank )
						continue;

					if ( layer == target.Layer )
						continue;

					Item piece = Copy( worn, worn.ItemID );

					AddItem( piece );

					if ( from != null && from.NetState != null )
						from.Send( new EquipUpdate( piece ) );
				}
			}

			SetFocus( target, target.ItemID, from );
		}

		// True when the slot being previewed already holds this art, so the gump can
		// skip a redundant RemoveItem/EquipUpdate pair.
		public bool IsFocusedOn( Item target, int art )
		{
			return target != null && m_Focus != null && !m_Focus.Deleted
				&& m_Focus.Layer == target.Layer && m_Focus.ItemID == art;
		}

		// Toggles the rest of the outfit. Returns true when the state changed.
		public bool SetGear( bool show, Item target, Mobile from )
		{
			if ( m_ShowGear == show )
				return false;

			m_ShowGear = show;
			Dress( target, from );

			return true;
		}

		// Swaps the one slot being previewed and refreshes it on the open paperdoll.
		public void SetFocus( Item target, int art, Mobile from )
		{
			if ( target == null )
				return;

			Item old = m_Focus;

			m_Focus = null;

			if ( old != null && !old.Deleted )
			{
				if ( from != null && from.NetState != null )
					from.Send( new RemoveItem( old ) );

				old.Delete();
			}

			m_Focus = Copy( target, art );
			AddItem( m_Focus );

			// Adding it runs the clothing rules, which can hide a layer covered by a
			// robe - but this is the piece being previewed, so keep it visible.
			m_Focus.Visible = true;

			if ( from != null && from.NetState != null )
				from.Send( new EquipUpdate( m_Focus ) );
		}

		private static Item Copy( Item template, int art )
		{
			Item piece = new Item( art );

			piece.Layer = template.Layer;
			piece.Hue = template.Hue;
			// A two-handed weapon should occupy both hands even where the class only
			// sets Layer.TwoHanded (bows and crossbows never set NeedsBothHands); a
			// shield shares the same layer but stays on one side.
			piece.NeedsBothHands = template.NeedsBothHands
				|| ( template is BaseWeapon && template.Layer == Layer.TwoHanded );
			piece.Movable = false;

			return piece;
		}

		// Tells the player's client about the model (0x20 + 0x78), so its paperdoll can draw it.
		public void Present( Mobile from )
		{
			if ( from == null || from.NetState == null )
				return;

			TransmogrifyPreview old;

			if ( m_Open.TryGetValue( from, out old ) && old != null && old != this )
				old.Dismiss( from );

			m_Open[from] = this;

			m_Presented = true;

			// 0x20 tells the client the mobile exists at all, 0x78 gives it the body,
			// hue and every worn layer. Both are needed: with 0x78 alone some clients
			// ignore the following paperdoll request entirely.
			from.Send( new MobileUpdate( this ) );
			from.Send( new MobileIncoming( from, this ) );

			// Safety net: a preview the player never answers for cleans itself up.
			if ( m_Timeout != null )
				m_Timeout.Stop();

			m_Timeout = new ExpiryTimer( this );
			m_Timeout.Start();
		}

		// Re-states the whole outfit as individual 0x2E packets. 0x78 already carries
		// it, but this guarantees the paperdoll is dressed (in particular the slot
		// being changed, holding the item's current appearance) the moment it opens.
		public void SendEquipment( Mobile from )
		{
			if ( from == null || from.NetState == null )
				return;

			for ( int i = 0; i < Items.Count; i++ )
			{
				Item piece = Items[i];

				if ( piece != null && !piece.Deleted )
					from.Send( new EquipUpdate( piece ) );
			}
		}

		// The window itself: a client-side paperdoll. The server can neither move nor
		// pin it (0x88 carries no coordinates), so re-issuing it is the only lever we
		// have - and doing so re-raises the window alongside the gump.
		//
		// Sent synchronously: NetState traffic belongs on the game thread, so this is
		// deliberately not deferred to a timer.
		public void ShowPaperdoll( Mobile from )
		{
			if ( from != null && from.NetState != null )
				from.Send( new DisplayPaperdoll( this, "Preview", true ) );
		}

		public void Dismiss( Mobile from )
		{
			if ( m_Timeout != null )
			{
				m_Timeout.Stop();
				m_Timeout = null;
			}

			if ( from != null )
			{
				if ( from.NetState != null )
					from.Send( new RemoveMobile( this ) );

				TransmogrifyPreview current;

				if ( m_Open.TryGetValue( from, out current ) && current == this )
					m_Open.Remove( from );
			}

			if ( !Deleted )
				Delete();
		}

		private class ExpiryTimer : Timer
		{
			private readonly TransmogrifyPreview m_Preview;

			public ExpiryTimer( TransmogrifyPreview preview ) : base( TimeSpan.FromMinutes( 10.0 ) )
			{
				m_Preview = preview;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Preview.Dismiss( m_Preview.m_Owner );
			}
		}
	}
}
