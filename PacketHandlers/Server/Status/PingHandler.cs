using Aragas.Core.Packets;

using ProtocolModern.Packets.Client.Status;

namespace ProtocolModern.PacketHandlers.Server.Status
{
    public class PingHandler : ProtocolPacketHandler<PingPacket, Packet>
    {
        public override Packet Handle(PingPacket packet)
        {
            return null;
        }
    }
}
