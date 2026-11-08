using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Network; 

namespace Server.Mobiles
{
	[CorpseName( "a pile of metal" )]
	public class Exodus : BaseCreature
	{
		private bool m_FieldActive;
		private ExplosionTimer m_ExplosionTimer;
		public bool FieldActive{ get{ return m_FieldActive; } }
		public bool CanUseField{ get{ return Hits >= HitsMax * 9 / 10; } } // TODO: an OSI bug prevents to verify this
		public override bool IsScaredOfScaryThings{ get{ return false; } }
		public override bool IsScaryToPets{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }

		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 0x9C2; } }
		public override int BreathEffectSound{ get{ return 0x665; } }
		public override int BreathEffectItemID{ get{ return 0x3818; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 14 ); }

		[Constructable]
		public Exodus () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Exodus";
			BaseSoundID = 0x300;
			Body = 451;

			SetStr( 1500, 2100 );
			SetDex( 531, 765 );
			SetInt( 453, 750 );

			SetHits( 2400, 3000 );

			SetDamage( 12, 15 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 20, 40 );
			SetResistance( ResistanceType.Cold, 20, 40 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 20, 40 );

			SetSkill( SkillName.Anatomy, 125.0 );
			SetSkill( SkillName.Psychology, 125.0 );
			SetSkill( SkillName.MagicResist, 125.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.FistFighting, 125.0 );

			Fame = 28000;
			Karma = -28000;

			VirtualArmor = 40;

			PackItem( new IronIngot( Utility.RandomMinMax( 20, 50 ) ) );
			PackItem( new PowerCrystal() );
			PackItem( new ArcaneGem() );
			PackItem( new ClockworkAssembly() );
			PackItem( new BottleOil( Utility.RandomMinMax( 3, 8 ) ) );
			PackItem( new Gears( Utility.RandomMinMax( 3, 8 ) ) );

			m_FieldActive = CanUseField;

			StartExplosionTimer();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override bool BardImmune { get { return true; } }

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( m_FieldActive )
				damage = (int)( damage * 0.75 ); // no melee damage when the field is up
		}

		public override void AlterSpellDamageFrom( Mobile caster, ref int damage )
		{
			if ( !m_FieldActive )
				damage = (int)( damage * 0.75 ); // no spell damage when the field is down
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ reflect = true; } // 25% spells are reflected back to the caster
			else { reflect = false; }
		}

		public override void OnDamagedBySpell( Mobile from )
		{
			if( from != null && from.Alive && 0.4 > Utility.RandomDouble() )
			{
				SendEBolt( from );
			}

			if ( !m_FieldActive )
			{
				this.FixedParticles( 0, 10, 0, 0x2522, EffectLayer.Waist );
			}
			else if ( m_FieldActive && !CanUseField )
			{
				m_FieldActive = false;

				this.FixedParticles( 0x3735, 1, 30, 0x251F, EffectLayer.Waist );
			}
		}

        public override void OnAfterSpawn()
        {
			base.OnAfterSpawn();
			// TEMP (testing): leave Exodus where it is spawned instead of relocating to a random dungeon
			// Worlds.MoveToRandomDungeon( this );
			Server.Misc.IntelligentAction.BurnAway( this );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( m_FieldActive )
			{
				this.FixedParticles( 0x376A, 20, 10, 0x2530, EffectLayer.Waist );

				PlaySound( 0x2F4 );

				attacker.SendAsciiMessage( "Your weapon is less effective with the creature's magical barrier up!" );

				if ( attacker is BaseCreature ) // KILL ANY PETS PLAYERS FOOLISHLY BROUGHT WITH THEM
				{
					BaseCreature pet = (BaseCreature)attacker;
					if ( BaseCreature.IsPet( attacker ) )
					{
						SendEBoltOnPet( attacker );
					}
				}
			}

			if( attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble() )
			{
				SendEBolt( attacker );
			}
		}

		public override bool OnBeforeDeath()
		{
			this.Body = 752;
			Server.Misc.IntelligentAction.BurnAway( this );
			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );
			if ( killer != null )
			{
				int chance = 0;

				Server.Items.QuestSouvenir.GiveReward( killer, "Exodus", 0, 0x65B9 );

				if ( Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "Exodus" ) ){ chance = 9; }

				if ( chance < Utility.RandomMinMax(1,10) )
				{
					if ( !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "Exodus" ) )
					{
						Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "Exodus", true );
						Item reward = new SummonReward();
						reward.Hue = 0x835;
						reward.ItemID = 0x2105;
						reward.Name = "Statue of Exodus";
						c.DropItem( reward );
					}
					c.DropItem( new DarkCoreExodus() );
					Server.Misc.LoggingFunctions.LogSlayingLord( killer, this.Name );
				}
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			if ( !m_FieldActive && !IsHurt() )
				m_FieldActive = true;
		}

		public override bool Move( Direction d )
		{
			bool move = base.Move( d );

			if ( move && m_FieldActive && this.Combatant != null )
				this.FixedParticles( 0, 10, 0, 0x2530, EffectLayer.Waist );

			return move;
		}		

		public void SendEBolt( Mobile to )
		{
			this.MovingParticles( to, 0x3818, 7, 0, false, true, 0xBE3, 0xFCB, 0x211 );
			to.PlaySound( 0x229 );
			this.DoHarmful( to );
			AOS.Damage( to, this, 50, 0, 0, 0, 0, 100 );
		}

		public void SendEBoltOnPet( Mobile to )
		{
			this.MovingParticles( to, 0x3818, 7, 0, false, true, 0xBE3, 0xFCB, 0x211 );
			to.PlaySound( 0x229 );
			this.DoHarmful( to );
			AOS.Damage( to, this, 10000, 0, 0, 0, 0, 100 );
		}

		public override void OnAfterDelete()
		{
			if ( m_ExplosionTimer != null )
			{
				m_ExplosionTimer.Stop();
				m_ExplosionTimer = null;
			}

			base.OnAfterDelete();
		}

		private void StartExplosionTimer()
		{
			if ( m_ExplosionTimer != null )
				m_ExplosionTimer.Stop();

			m_ExplosionTimer = new ExplosionTimer( this );
			m_ExplosionTimer.Start();
		}

		// Every few seconds, release a burst of greater-explosion potions at random spots near
		// Exodus, all at once (players only - never Exodus himself).
		private void DropExplosion()
		{
			Map map = this.Map;

			// Only fire while Exodus is actually fighting someone.
			if ( map == null || map == Map.Internal || !this.Alive || this.Combatant == null )
				return;

			// Target a random player within 20 tiles and drop a single 3x3 block on them.
			List<Mobile> players = new List<Mobile>();

			IPooledEnumerable eable = this.GetMobilesInRange( 20 );

			foreach ( Mobile m in eable )
			{
				if ( m is PlayerMobile && m.Alive && m.Map == map && this.CanBeHarmful( m, false ) )
					players.Add( m );
			}

			eable.Free();

			if ( players.Count == 0 )
				return;

			Mobile target = players[Utility.Random( players.Count )];

			int centerX = target.X;
			int centerY = target.Y;

			for ( int dx = -1; dx <= 1; ++dx )
			{
				for ( int dy = -1; dy <= 1; ++dy )
				{
					LaunchExplosion( map, centerX + dx, centerY + dy );
				}
			}
		}

		private void LaunchExplosion( Map map, int x, int y )
		{
			IPoint3D p = new Point3D( x, y, this.Z );
			Server.Spells.SpellHelper.GetSurfaceTop( ref p );
			Point3D loc = new Point3D( p );

			// Temporary landing indicator so players can step clear.
			Effects.SendLocationEffect( loc, map, 0x3709, 30, 10, 0x489, 0 );
			Effects.PlaySound( loc, map, 0x20C );

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), () => ExplodeArea( loc, map ) );
		}

		private void ExplodeArea( Point3D loc, Map map )
		{
			if ( map == null || map == Map.Internal || this.Deleted )
				return;

			Effects.PlaySound( loc, map, 0x307 );
			Effects.SendLocationEffect( loc, map, 0x36B0, 9, 10, 0, 0 );

			List<Mobile> targets = new List<Mobile>();

			IPooledEnumerable eable = map.GetMobilesInRange( loc, 2 );

			foreach ( Mobile m in eable )
			{
				// Players only - Exodus (and other NPCs) are never harmed by his own blast.
				if ( m is PlayerMobile && m != this && m.Alive && m.Blessed == false && this.CanBeHarmful( m, false ) )
					targets.Add( m );
			}

			eable.Free();

			int count = targets.Count;

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = targets[i];

				this.DoHarmful( m );

				int damage = Utility.RandomMinMax( 35, 40 ); // half of greater-explosion potion damage

				if ( count > 2 )
					damage /= ( count - 1 );

				AOS.Damage( m, this, damage, 0, 100, 0, 0, 0 );
			}
		}

		private class ExplosionTimer : Timer
		{
			private readonly Exodus m_Owner;

			public ExplosionTimer( Exodus owner ) : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 10.0 ) )
			{
				Priority = TimerPriority.OneSecond;
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if ( m_Owner == null || m_Owner.Deleted || m_Owner.Map == null || m_Owner.Map == Map.Internal )
				{
					Stop();
					return;
				}

				m_Owner.DropExplosion();
			}
		}

		public Exodus( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_FieldActive = CanUseField;

			StartExplosionTimer();
		}
	}
}

namespace Server.Items
{
	public class DarkCoreExodus : Item
	{
		[Constructable]
		public DarkCoreExodus() : base( 0x1CD )
		{
			Hue = 0x497;
			Name = "dark core of Exodus";
		}

		public DarkCoreExodus( Serial serial ) : base( serial )
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

namespace Server.Gumps 
{ 
	public class DarkCoreGump : Gump 
	{
		public DarkCoreGump( Mobile from, string items, bool scroller ) : base( 50, 50 ) 
		{
			from.SendSound( 0x4A );

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			string color = "#e3a27e";

			AddPage(0);

			AddImage(0, 0, 7022, Server.Misc.PlayerSettings.GetGumpHue( from ));

			AddButton(427, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 11, 11, 403, 20, @"<BODY><BASEFONT Color=" + color + ">SHRINE OF DILIGENCE</BASEFONT></BODY>", (bool)false, (bool)false);

			string text = "";

			if ( items == "null" )
			{
				text = "To destroy the dark core and absorb its power into one of your items, you can only place a single piece of armor, weapon, trinket, clothing, or jewelry onto the shrine. Remove some of the items and try to destroy the dark core again.";
			}
			else if ( items == "non" )
			{
				text = "To destroy the dark core and absorb its power into one of your items, you can only place a single piece of armor, weapon, trinket, clothing, or jewelry onto the shrine. Remove any of the items, that do not fall into these categories, and try to destroy the dark core again.";
			}
			else
			{
				text = "Some items have an inherent magical aura bestowed upon them, that the magic of the dark core cannot enhance any further. These include Artefacts and Relics where you use enhancement points to modify. Below are the items on the shrine that cannot be affected. Once you remove them, you can try to destroy the dark core of exodus again.<br>" + items + "";
			}

			AddHtml( 13, 43, 440, 301, @"<BODY><BASEFONT Color=" + color + ">" + text + "</BASEFONT></BODY>", (bool)false, (bool)false);
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 
			from.SendSound( 0x4A );
		}
	}
}