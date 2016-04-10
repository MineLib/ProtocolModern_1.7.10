using Aragas.Core.PacketHandlers;
using Aragas.Core.Packets;

using ProtocolModern.Client;

namespace ProtocolModern.PacketHandlers
{
    public abstract class ProtocolPacketHandler<TRequestPacket, TReplyPacket> :
        PacketHandler<TRequestPacket, TReplyPacket, Protocol> where TRequestPacket : Packet where TReplyPacket : Packet { }
}
