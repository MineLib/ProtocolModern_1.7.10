using Aragas.Core.Packets;

using ProtocolModern.Packets.Client.Login;

namespace ProtocolModern.PacketHandlers.Server.Login
{
    public class Disconnect2Handler : ProtocolPacketHandler<Disconnect2Packet, Packet>
    {
        public override Packet Handle(Disconnect2Packet packet)
        {
            Context.Disconnect();

            return null;
        }
    }
}
