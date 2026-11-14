using System;
using Server;

namespace Server.Items
{
	// GM/test tool: the same behaviour as the Paragon-dropped gem, but a scroll and
	// blessed so it can be spawned freely with [add TransmogrifyTestItem. The real
	// drop is TransmogrifyGem.
	public class TransmogrifyTestItem : TransmogrifyTool
	{
		[Constructable]
		public TransmogrifyTestItem() : base( 0x1F4C ) // scroll
		{
			Name = "Transmogrification Scroll";
			Hue = TransmogrifyGem.GemHue;
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public TransmogrifyTestItem( Serial serial ) : base( serial )
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
		}
	}
}
