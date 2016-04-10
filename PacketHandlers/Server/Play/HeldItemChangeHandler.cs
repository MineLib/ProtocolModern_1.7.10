using Aragas.Core.Packets;

using MineLib.Core.Events.ReceiveEvents;

using ProtocolModern.Packets.Client.Play;

namespace ProtocolModern.PacketHandlers.Server.Play
{
    public class HeldItemChangeHandler : ProtocolPacketHandler<HeldItemChangePacket, Packet>
    {
        public override Packet Handle(HeldItemChangePacket packet)
        {
            Context.ClientReceiveEvent(new HeldItemChangeEvent((byte) packet.Slot));

            return null;
        }
    }
}
