﻿using Aragas.Core.Packets;

using MineLib.Core.Events.ReceiveEvents;

using ProtocolModern.Packets.Client.Play;

namespace ProtocolModern.PacketHandlers.Server.Play
{
    public class RespawnHandler : ProtocolPacketHandler<RespawnPacket, Packet>
    {
        public override Packet Handle(RespawnPacket packet)
        {
            Context.ClientReceiveEvent(new GameInfoEvent(packet.GameMode, packet.Difficulty, (byte) packet.Dimension));

            return null;
        }
    }
}
