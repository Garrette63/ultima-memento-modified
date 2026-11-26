using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 
using Server.Network;
using Server.Regions;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Mobiles 
{
	[CorpseName( "a henchman corpse" )] 
	public class HenchmanFighter : BaseCreature
	{
		private DateTime m_Healing;
		public DateTime Healing{ get{ return m_Healing; } set{ m_Healing = value; } }

		private DateTime m_NextMorale;
		public DateTime NextMorale{ get{ return m_NextMorale; } set{ m_NextMorale = value; } }

		// nStats-derived resistance floors. See GetMinResistance.
		private int m_ArmorResistance;
		private int m_ProtectResistance;

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			bool GoAway = HenchmanFunctions.OnMoving( m, oldLocation, this, m_NextMorale );
			if ( GoAway == true ){ HenchmanCustomizeGump.CloseFor( ControlMaster ); Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerCallback( Delete ) ); }
			else { m_NextMorale = (DateTime.Now + TimeSpan.FromSeconds( 60 )); }
		}

		[Constructable] 
		public HenchmanFighter( int myBody, int nMounted, double nSkills, int nStats ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "henchman";
			Body = myBody;

			// BaseCreature's constructor rolls a random gender for the Jako
			// taming system. Human henchmen must follow the gender stored on
			// their deed instead, otherwise male henchmen end up flagged as
			// female while keeping their male body and name.
			this.Female = ( myBody == 401 );

			int nStr = 100;
			int nDex = 100;
			int nInt = 50;
			int nArmor = (int)(nStats / 4); if ( nArmor > 70 ){ nArmor = 70; }
			int nProtect = (int)(nStats / 2); if ( nProtect > 70 ){ nProtect = 70; }
			m_ArmorResistance = nArmor;
			m_ProtectResistance = nProtect;

       	    SetStr( nStr );
            SetDex( nDex );
            SetInt( nInt );

            SetHits( nStr*2 );
            SetStam( nDex*2 );
            SetMana( nInt*2 );

			// Base damage comes from the equipped weapon, exactly as it does for
			// a player. DamageMin stays at -1, so BaseWeapon.GetBaseDamageRange
			// uses the weapon's MinDamage/MaxDamage instead of an nStats range.

			ControlSlots = 1;

			VirtualArmor = (int)(nStats / 5);

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 10 );
			SetDamageType( ResistanceType.Cold, 10 );
			SetDamageType( ResistanceType.Poison, 10 );
			SetDamageType( ResistanceType.Energy, 10 );

			// The nStats resistances are not set as base resistances. They are
			// returned by GetMinResistance instead, so worn gear is only used
			// when it is higher than them (and the total stays capped at 70).

            SetSkill(SkillName.Swords, nSkills );
            SetSkill(SkillName.Focus, nSkills );
            SetSkill(SkillName.Bludgeoning, nSkills );
            SetSkill(SkillName.Fencing, nSkills );
            SetSkill(SkillName.MagicResist, nSkills );
            SetSkill(SkillName.Tactics, nSkills );
            SetSkill(SkillName.Parry, nSkills );
            SetSkill(SkillName.Anatomy, nSkills );
            SetSkill(SkillName.Healing, nSkills );

			if ( nMounted > 0 )
			{
				new HenchHorse().Rider = this;
				ActiveSpeed = 0.1;
				PassiveSpeed = 0.2;
			}
		}

        public override void OnSpeech( SpeechEventArgs e )
        {
            if (!e.Handled && Insensitive.Equals(e.Speech, "report"))
            {
                HenchmanFunctions.ReportStatus(this);
            }
            base.OnSpeech(e);
        }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from != null && ControlMaster == from )
				list.Add( new HenchmanInfoEntry( from, this ) );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool InitialInnocent{ get{ return true; } }
		public override bool DeleteOnRelease{ get{ return true; } }
		public override bool DeleteCorpseOnDeath{ get{ return true; } }
		public override bool IsDispellable { get { return false; } }
		public override bool IsBondable{ get{ return false; } }
		public override bool CanBeRenamedBy( Mobile from ){ return false; }

		// The nStats-derived resistances behave like a player's Magic Resistance
		// floor: gear is only used when it is higher, rather than being added on
		// top. GetMaxResistance caps the result at 70, and because the core runs
		// this on every recalculation (including after gear is equipped) the cap
		// also applies to items added after the henchman was summoned.
		public override int GetMinResistance( ResistanceType type )
		{
			int resistance = ( type == ResistanceType.Physical ? m_ProtectResistance : m_ArmorResistance );

			if ( resistance > 70 )
				resistance = 70;

			return resistance;
		}

		public override int GetMaxResistance( ResistanceType type )
		{
			return 70;
		}

		// Equipment hit point, stamina and mana bonuses are not part of the
		// seeded creature maxima, so add them the same way PlayerMobile does.
		// The current pools are clamped/re-armed whenever gear changes (see
		// HenchmanFunctions.RefreshEquipmentPools).
		public override int HitsMax
		{
			get { return HenchmanFunctions.GetEquipmentPoolMax( this, AosAttribute.BonusHits, base.HitsMax ); }
		}

		public override int StamMax
		{
			get { return HenchmanFunctions.GetEquipmentPoolMax( this, AosAttribute.BonusStam, base.StamMax ); }
		}

		public override int ManaMax
		{
			get { return HenchmanFunctions.GetEquipmentPoolMax( this, AosAttribute.BonusMana, base.ManaMax ); }
		}

		// The base damage range is the equipped weapon's. Expose it through
		// DamageMin/DamageMax so information gumps show the same range combat
		// uses (with no weapon the core still falls back to fists).
		public override int DamageMin
		{
			get
			{
				BaseWeapon weapon = Weapon as BaseWeapon;

				return ( weapon == null ? base.DamageMin : weapon.MinDamage );
			}
			set { base.DamageMin = value; }
		}

		public override int DamageMax
		{
			get
			{
				BaseWeapon weapon = Weapon as BaseWeapon;

				return ( weapon == null ? base.DamageMax : weapon.MaxDamage );
			}
			set { base.DamageMax = value; }
		}

		public override bool AllowEquipFrom( Mobile mob )
		{
			if ( HenchmanFunctions.CanUseHenchman( this, mob ) )
				return true;

			return base.AllowEquipFrom( mob );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			if ( HenchmanFunctions.IsEquipment( item ) && HenchmanFunctions.CanUseHenchman( this, from ) )
				return true;

			return base.CheckNonlocalLift( from, item );
		}

		public override bool EquipItem( Item item )
		{
			if ( !HenchmanFunctions.ValidateHenchmanEquip( this, item ) )
				return false;

			if ( !base.EquipItem( item ) )
				return false;

			HenchmanFunctions.OnHenchmanEquipped( this, item );
			HenchmanFunctions.AnnounceMeditationWarning( this );
			return true;
		}

		public override void OnItemRemoved( Item item )
		{
			HenchmanFunctions.OnHenchmanItemRemoved( this, item );
			base.OnItemRemoved( item );
		}

		public override void OnDelete()
		{
			HenchmanCustomizeGump.CloseFor( ControlMaster );
			HenchmanFunctions.StashGear( this );
			base.OnDelete();
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			HenchmanFunctions.OnGaveAttack( this );
		}

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);
			HenchmanFunctions.OnSpellAttack( this );
        }

		public override void OnGotMeleeAttack( Mobile defender )
		{
			HenchmanFunctions.OnGotAttack( this );
		}

		public override bool OnBeforeDeath()
		{
			HenchmanFunctions.OnDead( this );
			if ( !base.OnBeforeDeath() )
				return false;

			return true;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			HenchmanFunctions.OnGive( from, dropped, this );
			return base.OnDragDrop( from, dropped );
		}

		public HenchmanFighter( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version
			Loyalty = 100;
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) );
		} 
	} 
}   