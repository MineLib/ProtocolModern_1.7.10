using Aragas.Core.Packets;

using ProtocolModern.Packets.Client.Status;

namespace ProtocolModern.PacketHandlers.Server.Status
{
    public class ResponseHandler : ProtocolPacketHandler<ResponsePacket, Packet>
    {
        public override Packet Handle(ResponsePacket packet)
        {
            return null;
        }
    }
}
