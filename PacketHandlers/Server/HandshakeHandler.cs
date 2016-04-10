using Aragas.Core.Packets;

using MineLib.Core;

using ProtocolModern.Enum;
using ProtocolModern.Packets.Server.Handshake;

namespace ProtocolModern.PacketHandlers.Server
{
    public class HandshakeHandler : ProtocolPacketHandler<HandshakePacket, Packet>
    {
        public override Packet Handle(HandshakePacket packet)
        {
            switch ((NextState) (int) packet.NextState)
            {
                case NextState.Status:
                    Context.SetState(ConnectionState.InfoRequest);
                    break;

                case NextState.Login:
                    Context.SetState(ConnectionState.Joining);
                    break;
            }

            return null;
        }
    }
}
