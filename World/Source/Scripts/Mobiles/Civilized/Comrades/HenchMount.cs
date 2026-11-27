using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	// The rideable mount a human henchman brings along when its owner mounts or
	// is otherwise fast. One class covers all six appearances so a henchman
	// mount is never a wild, tamable creature and never carries loot; the
	// choice is stored on the summoning deed (HenchmanItem.HenchMount).
	public enum HenchMountType
	{
		Horse = 0,
		Llama = 1,
		Kirin = 2,
		Unicorn = 3,
		Nightmare = 4,
		HellSteed = 5
	}

	[CorpseName( "a mount corpse" )]
	public class HenchMount : BaseMount
	{
		public const int Count = 6;

		public static int Clamp( int mount )
		{
			if ( mount < 0 || mount >= Count )
				return (int) HenchMountType.Horse;

			return mount;
		}

		public static string GetName( int mount )
		{
			switch ( Clamp( mount ) )
			{
				case (int) HenchMountType.Llama: return "Llama";
				case (int) HenchMountType.Kirin: return "Ki-rin";
				case (int) HenchMountType.Unicorn: return "Unicorn";
				case (int) HenchMountType.Nightmare: return "Nightmare";
				case (int) HenchMountType.HellSteed: return "Hellsteed";
				default: return "Horse";
			}
		}

		[Constructable]
		public HenchMount( int mount ) : base( "a horse", 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			switch ( Clamp( mount ) )
			{
				case (int) HenchMountType.Llama:
				{
					Name = "a llama";
					Body = 0xDC;
					ItemID = 0x3EA6;
					BaseSoundID = 0x3F3;
					break;
				}
				case (int) HenchMountType.Kirin:
				{
					Name = "a ki-rin";
					Body = 132;
					ItemID = 0x3EAD;
					BaseSoundID = 0x3C5;
					break;
				}
				case (int) HenchMountType.Unicorn:
				{
					Name = "a unicorn";
					Body = 0x7A;
					ItemID = 0x3EB4;
					BaseSoundID = 0x4BC;
					break;
				}
				case (int) HenchMountType.Nightmare:
				{
					Name = "a nightmare";
					Body = 226;
					ItemID = 0x3EA0;
					BaseSoundID = 0xA8;
					Hue = 1109;
					break;
				}
				case (int) HenchMountType.HellSteed:
				{
					Name = "a hellsteed";
					Body = 793;
					ItemID = 0x3EBB;
					BaseSoundID = 0xA8;
					break;
				}
				default:
				{
					Name = "a horse";
					Body = 0xE2;
					ItemID = 0x3EA0;
					Hue = Utility.RandomList( 0, 0, 0, 0, 0, 0x967, 0x968, 0x969, 0x96A, 0x96B, 0x96C, 0x973, 0x974, 0x975, 0x976, 0x977, 0x978, 0x979, 0x97A, 0x97B, 0x97C, 0x97D, 0x97E );
					BaseSoundID = 0xA8;
					break;
				}
			}

			SetStr( 22, 98 );
			SetDex( 56, 75 );
			SetInt( 6, 10 );

			SetHits( 28, 45 );
			SetMana( 0 );

			SetDamage( 3, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.FistFighting, 29.3, 44.0 );

			Fame = 0;
			Karma = 0;

			Tamable = false;
			ControlSlots = 1;
			MinTameSkill = 29.1;
		}

		public HenchMount( Serial serial ) : base( serial )
		{
		}

		// A mount whose henchman dies or is dismissed is left standing in the
		// world by BaseMount/MountItem. Delete it on its first think instead of
		// waiting for the next world save to clean it up.
		public override void OnThink()
		{
			if ( Rider == null && Map != null && Map != Map.Internal )
			{
				Delete();
				return;
			}

			base.OnThink();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			if ( Rider == null ){ Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) ); }
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if ( Rider == null ){ Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) ); }
		}
	}
}
