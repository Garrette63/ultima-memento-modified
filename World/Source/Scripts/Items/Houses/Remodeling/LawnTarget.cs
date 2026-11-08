using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Multis;
using Server.Gumps;

namespace Server.Misc
{
	public class LawnTarget : Target
	{
		private Mobile m_From;
		private int m_SelectedID;
		private int m_Price;
		private string m_Title;
		private LawnTools m_LawnTools;
		private BaseHouse m_House;
		private string m_Category;
		private int m_Page;

		public LawnTarget(LawnTools tools, Mobile from, int itemID, int price, string title, string category, int page): base(-1, true, TargetFlags.None)
		{
			m_LawnTools = tools;
			m_From = from;
			m_SelectedID = itemID;
			m_Price = price;
			m_Title = title;
			m_Category = category;
			m_Page = page;
			CheckLOS = false;
			m_LawnTools.Category = category;
			m_LawnTools.Page = page;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			IPoint3D t = targeted as IPoint3D;
			if (t == null)
				return;
			
			Point3D loc = new Point3D(t);
			if ( t is StaticTarget )
			{
				loc.Z -= TileData.ItemTable[((StaticTarget)t).ItemID & 0x3FFF].CalcHeight;
			}
			else if ( targeted is Item )
			{
				Item item = (Item)targeted;
				ItemData id = item.ItemData;
				int checkZ = item.Z;
				int checkTop = checkZ + id.Height;
				if ( id.Foliage ){ loc.Z = checkZ; }
				else { loc.Z = checkTop; }
			}

			if (ValidatePlacement(loc))
				EndPlace(loc);
			else
				GumpUp();
		}

		public bool ValidatePlacement(Point3D loc)
		{
			Map map = m_From.Map;
			if (map == null)
				return false;

			m_House = BaseHouse.FindHouseAt(m_From.Location, map, 20);

			// Region is intentionally ignored. The only requirement is that the player
			// is standing inside a house they own or co-own.
			if ( m_House == null || !m_House.IsCoOwner( m_From ) )
			{
				m_From.SendMessage("You must be standing in your house to place this!");
				return false;
			}

			// Keep the yard range restriction: the target must fall within the house
			// footprint plus the Remodeling margin (15 tiles per side). No region check.
			if ( !IsWithinHouseLand( m_House, loc ) )
			{
				m_From.SendMessage("You will have to get closer and place within your land!");
				return false;
			}

			return true;
		}

		private static bool IsWithinHouseLand( BaseHouse house, Point3D loc )
		{
			if ( house == null )
				return false;

			MultiComponentList mcl = house.Components;

			int minX = house.X + mcl.Min.X - Remodeling.Left;
			int minY = house.Y + mcl.Min.Y - Remodeling.Back;
			int maxX = house.X + mcl.Min.X + mcl.Width + Remodeling.Right;
			int maxY = house.Y + mcl.Min.Y + mcl.Height + Remodeling.Front;

			return ( loc.X >= minX && loc.X <= maxX && loc.Y >= minY && loc.Y <= maxY );
		}

		public void EndPlace(Point3D loc)
		{
			var cont = m_From.Backpack;
			var Paid = cont != null && cont.ConsumeTotal(typeof(Gold), m_Price);
			if (!Paid) // Fallback to Bank
			{
				Paid = Banker.Withdraw(m_From, m_Price);
			}

			if (Paid)
			{
				Remodeling.EndPut( m_SelectedID, m_From, m_Price, m_Title, m_House, loc );
				m_From.Target = new LawnTarget(m_LawnTools, m_From, m_SelectedID, m_Price, m_Title, m_Category, m_Page);
				m_From.PlaySound( Utility.RandomList( 0x13E, 0x23D, 0x125, 0x126, 0x55, 0x541 ) );
				GumpUp();
			}
			else
			{
				m_From.SendMessage("You do not have enough gold for that");
				GumpUp();
			}
		}

		public void GumpUp()
		{
			m_From.CloseGump( typeof( LawnGump ) );
			m_From.SendGump(new LawnGump(m_From, m_LawnTools, m_Category, m_Page, m_SelectedID, m_Price, m_Title));
		}
	}
}