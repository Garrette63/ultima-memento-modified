using System;
using Server;
using Server.Mobiles;

public class MobileUtilities
{
	/// <summary>
	/// Returns the Luck from the Player that killed or last damaged the provided Mobile
	/// </summary>
	public static int GetLuckFromKiller(BaseCreature m)
	{
		var lastKiller = m.LastKiller;
		var player = TryGetMasterPlayer(lastKiller);
		
		// Check for provocation
		if (player == null && m is BaseCreature)
		{
			BaseCreature bc = (BaseCreature)m;
			if (bc.BardProvoked)
			{
				lastKiller = bc;
				player = TryGetMasterPlayer(bc.BardMaster);
			}
		}

		// I don't know why this is done places, but we'll do it anyways
		if (player == null)
		{
			lastKiller = m.FindMostRecentDamager(true);
			player = TryGetMasterPlayer(lastKiller);
		}

		if (player == null) return 0;

		var luck = player.Luck;
		if (player == lastKiller) return luck;

		// Something other than the Player killed it, only use half the Player's Luck
		return luck / 2;
	}

	/// <summary>
	/// Returns the first ancestor that is a PlayerMobile
	/// </summary>
	public static PlayerMobile TryGetMasterPlayer(Mobile m)
	{
		while (m != null)
		{
			if (m is PlayerMobile) return (PlayerMobile)m;
			if (m is BaseCreature == false) return null; // Only BaseCreature can have masters

			m = ((BaseCreature)m).GetMaster();
		}

		return null;
	}

	/// <summary>
	/// Returns the PlayerMobile that killed or last damaged the provided Mobile
	/// </summary>
	public static PlayerMobile TryGetKillingPlayer(Mobile m)
	{
		var player = TryGetMasterPlayer(m.LastKiller);
		if (player != null) return player;
		
		// Check for provocation
		if (m is BaseCreature)
		{
			BaseCreature bc = (BaseCreature)m;
			if (bc.BardProvoked)
			{
				player = TryGetMasterPlayer(bc.BardMaster);
				if (player != null) return player;
			}
		}

		// I don't know why this is done places, but we'll do it anyways
		var lastDamager = m.FindMostRecentDamager(true);

		return TryGetMasterPlayer(lastDamager);
	}

	/// <summary>
	/// Returns true for the shard's named bosses and every Magical Prison escapee.
	/// Used for the "(boss)" name tag and the Fragment of Infinity drop rule.
	/// </summary>
	public static bool IsBoss(BaseCreature bc)
	{
		if (bc == null)
			return false;

		// SummonPrison stamps all 25 Magical Prison escapees with this hue
		// when it releases them, so this catches every prison boss without
		// catching the ordinary creatures they are based on.
		if (bc.EmoteHue == 505)
			return true;

		Type type = bc.GetType();

		return type == typeof(BlackGateDemon)
			|| type == typeof(BloodDemigod)
			|| type == typeof(DragonKing)
			|| type == typeof(OrkDemigod)
			|| type == typeof(LichKing)
			|| type == typeof(Dracula)
			|| type == typeof(Jormungandr)
			|| type == typeof(Xurtzar)
			|| type == typeof(CaddelliteDragon)
			|| type == typeof(Vulcrum)
			|| type == typeof(Arachnar)
			|| type == typeof(Surtaz)
			|| type == typeof(Mangar)
			|| type == typeof(Shadowlord)
			|| type == typeof(Tarjan)
			|| type == typeof(TitanHydros)
			|| type == typeof(TitanLithos)
			|| type == typeof(TitanPyros)
			|| type == typeof(TitanStratos)
			|| type == typeof(KhumashGor)
			|| type == typeof(Satan)
			|| type == typeof(SlasherOfVoid)
			|| type == typeof(Exodus);
	}
}