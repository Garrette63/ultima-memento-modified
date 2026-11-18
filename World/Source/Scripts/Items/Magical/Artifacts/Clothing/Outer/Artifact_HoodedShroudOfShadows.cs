namespace Server.Items
{
	[Flipable( 0x2684, 0x2683 )]
	public class Artifact_HoodedShroudOfShadows : GiftRobe
	{
		[Constructable]
		public Artifact_HoodedShroudOfShadows()
		{
			ItemID = 0x2684;
			Hue = 0x455;
			Name = "Hooded Shroud of Shadows";
			SkillBonuses.SetValues( 0, SkillName.Hiding, 50 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 50 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 10, "" );
		}

		public Artifact_HoodedShroudOfShadows( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 2 )
			{
				// Earlier saves used the plain robe art (0x2B69). Move those copies to the
				// hooded shroud art without touching anything else. A transmogrified item
				// keeps the art its owner picked, and a hidden item (the clothing system's
				// placeholder graphic) only has its stored original art moved.
				if ( Item.isModded( this ) )
				{
					if ( GraphicID == 0x2B69 )
						GraphicID = 0x2684;
				}
				else if ( ItemID == 0x2B69 )
				{
					ItemID = 0x2684;
					GraphicID = 0x2684;
				}

				if ( Name == "Shroud of Shadows" )
					Name = "Hooded Shroud of Shadows";
			}
		}
	}
}
