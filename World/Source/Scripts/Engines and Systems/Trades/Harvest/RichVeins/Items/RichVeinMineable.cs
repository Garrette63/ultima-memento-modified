using System;
using System.Collections.Generic;
using Server.Items.Abstractions;
using Server.Network;

namespace Server.Engines.Harvest
{
    public class RichVeinMineable : Item
    {
        private readonly RichVeinSpawner _parent;
        private DateTime m_nextSound;

        public RichVeinMineable() : base(Utility.RandomList(RichVeinMining.System.RareNodeDefinition.Tiles))
        {
            Name = "A glistening ore vein";
            Light = LightType.Circle300;
        }

        public RichVeinMineable(RichVeinSpawner parent) : this()
        {
            _parent = parent;
        }

        public RichVeinMineable(Serial serial) : base(serial)
        {
        }

        // Players may only see a glistening ore vein once their trained Mining skill reaches this value
        // (matches RichVeinMining.CheckHarvest, which requires Mining Base >= 65 to mine a vein).
        private const double VisibleMiningRequirement = 65.0;

        public override bool HandlesOnMovement { get { return true; } }

        public bool HasSpawner { get { return _parent != null; } }

        public static bool CanBeSeenBy(Mobile m)
        {
            if (m == null)
                return false;

            // Staff need to see veins for placement and debugging.
            if (m.AccessLevel > AccessLevel.Player)
                return true;

            return m.Skills[SkillName.Mining].Base >= VisibleMiningRequirement;
        }

        public override void SendInfoTo(NetState state, bool sendOplPacket)
        {
            // Don't send the vein to clients that cannot yet see it, keeping it invisible to them.
            if (!CanBeSeenBy(state != null ? state.Mobile : null))
                return;

            base.SendInfoTo(state, sendOplPacket);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            // Automatically deleted
            Visible = false;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (_parent != null && _parent.AutomaticCleanup)
            {
                _parent.Delete();
            }
        }

        public override void OnDelete()
        {
            if (Deleted) return;

            // Clean up existing resource Bank
            if (Map != Map.Internal)
            {
                Dictionary<Point2D, HarvestBank> banks;
                if (!RichVeinMining.System.RareNodeDefinition.Banks.TryGetValue(Map, out banks)) return;

                banks.Remove(new Point2D(Location));
            }

            base.OnDelete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            Item harvestTool = null;

            var tool = from.FindItemOnLayer(Layer.OneHanded) as IHarvestTool;
            if (tool is Item && tool != null && tool.HarvestSystem == Mining.System) harvestTool = (Item)tool; // Pickaxe

            if (harvestTool == null)
            {
                tool = from.FindItemOnLayer(Layer.TwoHanded) as IHarvestTool;
                if (tool is Item && tool != null && tool.HarvestSystem == Mining.System) harvestTool = (Item)tool; // Shovel
            }

            if (harvestTool != null)
            {
                tool.HarvestSystem.StartHarvesting(from, (Item)tool, this);
            }
            else
            {
                from.SendMessage("This looks like you can mine it with a pickaxe or shovel.");
            }
        }

        public override bool OnDragLift(Mobile from)
        {
            // Entity is Visible so Ctrl+Shift can show it
            // Disallow moving it
            return false;
        }

        // Tell the core that we implement OnMovement
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Player && Utility.InRange(Location, m.Location, 16) && !Utility.InRange(Location, oldLocation, 16))
            {
                var now = DateTime.UtcNow;
                if (now < m_nextSound) return;

                // The tell-tale glistening only exists for miners who can actually see the vein.
                if (CanBeSeenBy(m))
                {
                    m_nextSound = now.AddSeconds(25);
                    SendSparkleToMiners();

                    if (m.Skills[SkillName.Mining].Value >= 100) // GM+ Required for sounds
                        m.PlaySound(0x511); // Mirror image
                }
            }
        }

        // Send the sparkle effect only to clients that are allowed to see the vein.
        private void SendSparkleToMiners()
        {
            Map map = Map;
            if (map == null || map == Map.Internal)
                return;

            Packet sparkle = null;

            IPooledEnumerable eable = map.GetClientsInRange(Location);
            foreach (NetState state in eable)
            {
                if (!CanBeSeenBy(state.Mobile))
                    continue;

                if (sparkle == null)
                    sparkle = Packet.Acquire(new TargetEffect(this, 0x3039, 10, 32, 0, 0));

                state.Send(sparkle);
            }
            eable.Free();

            Packet.Release(sparkle);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }
    }
}