using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server;

namespace Server.Items
{
	// Remembers what a transmogrified item looked like before it was changed, so the
	// original can be put back later.
	//
	// The record lives in Saves/Transmogrify/Originals.xml - one row per item, keyed
	// by serial, holding the ItemID and hue it had before its FIRST transmog. Rows
	// are dropped when the item no longer exists, and a revert deletes its row, so a
	// later transmog records the (restored) original again.
	public static class TransmogrifyRegistry
	{
		private class Original
		{
			public int ItemID;
			public int Hue;
		}

		private static readonly Dictionary<int, Original> m_Table = new Dictionary<int, Original>();

		private static string FilePath
		{
			get
			{
				return Path.Combine( Core.BaseDirectory, Path.Combine( "Saves", Path.Combine( "Transmogrify", "Originals.xml" ) ) );
			}
		}

		public static void Initialize()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( OnWorldLoad );
			EventSink.WorldSave += new WorldSaveEventHandler( OnWorldSave );
		}

		public static bool Has( Item item )
		{
			return item != null && m_Table.ContainsKey( item.Serial.Value );
		}

		// Looks up the art the item had before its first transmog. False when nothing
		// is recorded, so the caller keeps the item's current appearance.
		public static bool TryGetOriginal( Item item, out int itemID )
		{
			itemID = 0;

			if ( item == null )
				return false;

			Original original;

			if ( !m_Table.TryGetValue( item.Serial.Value, out original ) )
				return false;

			itemID = original.ItemID;

			return true;
		}

		public static int Count
		{
			get { return m_Table.Count; }
		}

		// Records the item's current visuals as its original - unless one is already
		// stored, because the first transmog is the real original.
		public static void Remember( Item item )
		{
			if ( item == null || item.Deleted )
				return;

			int serial = item.Serial.Value;

			if ( m_Table.ContainsKey( serial ) )
				return;

			Original original = new Original();

			original.ItemID = item.ItemID;
			original.Hue = item.Hue;

			m_Table[serial] = original;
		}

		// Puts the recorded visuals back and clears the record. False when the item
		// has nothing recorded, so the caller can say so.
		public static bool Revert( Item item )
		{
			if ( item == null || item.Deleted )
				return false;

			int serial = item.Serial.Value;

			Original original;

			if ( !m_Table.TryGetValue( serial, out original ) )
				return false;

			// Only the art is put back. Transmogging never changes the hue, so the one
			// we captured can only differ if the player dyed the item afterwards - and
			// a revert must not undo their own dye work.
			item.ItemID = original.ItemID;

			m_Table.Remove( serial );

			return true;
		}

		private static void OnWorldLoad()
		{
			Load();
		}

		private static void OnWorldSave( WorldSaveEventArgs e )
		{
			try
			{
				Save();
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[Transmogrify] saving the originals failed: {0}", ex );
			}
		}

		private static void Load()
		{
			m_Table.Clear();

			string path = FilePath;

			if ( !File.Exists( path ) )
				return;

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load( path );

				XmlElement root = doc["transmogrify"];

				if ( root == null )
					return;

				int dropped = 0;

				foreach ( XmlElement node in root.GetElementsByTagName( "item" ) )
				{
					int serial = ParseInt( node.GetAttribute( "serial" ) );
					int itemID = ParseInt( node.GetAttribute( "id" ) );
					int hue = ParseInt( node.GetAttribute( "hue" ) );

					if ( serial == 0 || itemID <= 0 )
						continue;

					// the item may have been deleted since the file was written
					if ( World.FindItem( (Serial) serial ) == null )
					{
						dropped++;
						continue;
					}

					Original original = new Original();

					original.ItemID = itemID;
					original.Hue = hue;

					m_Table[serial] = original;
				}

				Console.WriteLine( "[Transmogrify] {0} original appearance(s) loaded from Saves/Transmogrify/Originals.xml{1}.",
					m_Table.Count, dropped > 0 ? " (" + dropped + " stale row(s) dropped)" : "" );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[Transmogrify] loading Saves/Transmogrify/Originals.xml failed: {0}", ex );
			}
		}

		private static void Save()
		{
			string path = FilePath;
			string dir = Path.GetDirectoryName( path );

			if ( !string.IsNullOrEmpty( dir ) && !Directory.Exists( dir ) )
				Directory.CreateDirectory( dir );

			List<int> stale = new List<int>();

			// written to a side file and swapped in, so a half-written save can never
			// replace a good file
			string tmp = path + ".tmp";

			XmlTextWriter xml = new XmlTextWriter( tmp, System.Text.Encoding.UTF8 );

			try
			{
				xml.Formatting = Formatting.Indented;
				xml.Indentation = 2;

				xml.WriteStartDocument( true );
				xml.WriteStartElement( "transmogrify" );

				foreach ( KeyValuePair<int, Original> kv in m_Table )
				{
					// do not carry rows for items that are gone
					if ( World.FindItem( (Serial) kv.Key ) == null )
					{
						stale.Add( kv.Key );
						continue;
					}

					xml.WriteStartElement( "item" );
					xml.WriteAttributeString( "serial", "0x" + kv.Key.ToString( "X8" ) );
					xml.WriteAttributeString( "id", "0x" + kv.Value.ItemID.ToString( "X4" ) );
					xml.WriteAttributeString( "hue", kv.Value.Hue.ToString() );
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
				xml.WriteEndDocument();
			}
			finally
			{
				xml.Close();
			}

			if ( File.Exists( path ) )
				File.Copy( tmp, path, true );
			else
				File.Move( tmp, path );

			if ( File.Exists( tmp ) )
				File.Delete( tmp );

			for ( int i = 0; i < stale.Count; i++ )
				m_Table.Remove( stale[i] );
		}

		private static int ParseInt( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return 0;

			try
			{
				if ( value.StartsWith( "0x" ) )
					return Convert.ToInt32( value.Substring( 2 ), 16 );

				return Convert.ToInt32( value );
			}
			catch
			{
				return 0;
			}
		}
	}
}
