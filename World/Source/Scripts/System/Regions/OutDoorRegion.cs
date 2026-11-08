using System;
using System.Xml;
using Server;
using Server.Mobiles;
using System.Text;
using System.IO;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Multis;

namespace Server.Regions
{
	public class OutDoorRegion : BaseRegion
	{
		public OutDoorRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			if ( from.Region.IsPartOf( "the Ranger Outpost" ) )
			{
				// A player who already owns/co-owns a home here may build/decorate on their own land,
				// regardless of their Camping/Tracking. The skill check below still gates *new* homes.
				BaseHouse owned = BaseHouse.FindHouseAt( from );
				if ( owned != null && owned.IsCoOwner( from ) && InHouseLand( owned, p ) )
					return true;

				if ( from.Skills[SkillName.Camping].Base >= 90 || from.Skills[SkillName.Tracking].Base >= 90 )
				{
					return true;
				}
				else
				{
					from.SendMessage( "Only a master explorer or ranger can build a home here." );
					return false;
				}
			}

			return false;
		}

		// True when p falls within the house footprint (union of house.Area) plus the
		// Remodeling yard margin. Lets owners decorate their own land.
		private static bool InHouseLand( BaseHouse house, Point3D p )
		{
			if ( house == null )
				return false;

			int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

			foreach ( Rectangle2D rect in house.Area )
			{
				int x0 = house.X + rect.Start.X;
				int y0 = house.Y + rect.Start.Y;
				int x1 = x0 + rect.Width;
				int y1 = y0 + rect.Height;

				if ( x0 < minX ) minX = x0;
				if ( y0 < minY ) minY = y0;
				if ( x1 > maxX ) maxX = x1;
				if ( y1 > maxY ) maxY = y1;
			}

			if ( minX > maxX || minY > maxY )
				return false;

			minX -= Remodeling.Left;
			minY -= Remodeling.Back;
			maxX += Remodeling.Right;
			maxY += Remodeling.Front;

			return ( p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY );
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );
			if ( m is PlayerMobile )
			{
				LoggingFunctions.LogRegions( m, this.Name, "enter" );

				if ( this.Name == "the Ranger Outpost" ){ PlayerSettings.SetKeys( m, "RangerOutpost", true ); }
			}
		}

		public override void OnExit( Mobile m )
		{
			base.OnExit( m );
			if ( m is PlayerMobile )
			{
				LoggingFunctions.LogRegions( m, this.Name, "exit" );
			}
		}
	}
}