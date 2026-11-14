using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	// What the gypsy charges to reshape a piece of equipment.
	//
	// Gift equipment implements IGiftable, and every Artifact_* class in the shard
	// derives from a Gift* base (Artifact_HuntersGorget : GiftLeatherGorget, ...), so
	// one "is a gift" test covers the gift and the artifact tier together.
	public static class TransmogrifyFees
	{
		public const int Base = 1000;   // default rate - anything unusual
		public const int Basic = 200;   // plain gear, nothing gift or artefactual
		public const int Gift = 2000;   // gift / artifact equipment

		public static int For( Item item )
		{
			if ( item == null )
				return Base;

			if ( item is IGiftable )
				return Gift;

			// a plain piece of gear: no gift wrapper and no artifact level
			if ( item.ArtifactLevel == ArtifactLevel.None )
				return Basic;

			return Base;
		}

		// Her other services knock the price down by 0.5% per point of Begging while
		// the player is actually begging, so this one does too.
		public static int Discount( Mobile from, int cost )
		{
			if ( from == null || cost <= 0 )
				return cost;

			if ( BaseVendor.BeggingPose( from ) > 0 )
				cost -= (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * cost );

			return cost;
		}
	}
}
