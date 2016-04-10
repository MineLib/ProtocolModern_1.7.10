using Aragas.Core.Packets;

using MineLib.Core;

using ProtocolModern.Packets.Client.Login;

namespace ProtocolModern.PacketHandlers.Server.Login
{
    public class LoginSuccessHandler : ProtocolPacketHandler<LoginSuccessPacket, Packet>
    {
        public override Packet Handle(LoginSuccessPacket packet)
        {
            Context.SetState(ConnectionState.Joined);

            return null;
        }
    }
}
