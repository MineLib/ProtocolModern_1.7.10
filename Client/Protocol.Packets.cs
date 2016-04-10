using Aragas.Core.Extensions;
using Aragas.Core.PacketHandlers;
using Aragas.Core.Packets;

using MineLib.Core;

using ProtocolModern.PacketHandlers;

namespace ProtocolModern.Client
{
    public sealed partial class Protocol
    {
        private void OnPacketHandled(int id, ProtobufPacket packet, ConnectionState state)
        {
            if(!Connected)
                return;

            ContextFunc<ProtobufPacket> handler = null;

            switch (state)
            {
                case ConnectionState.Joining:
                    handler = ServerPacketHandlers.LoginPacketResponses.Handlers[id];
                    break;

                case ConnectionState.Joined:
                    handler = ServerPacketHandlers.PlayPacketResponses.Handlers[id];
                    break;

                case ConnectionState.InfoRequest:
                    handler = ServerPacketHandlers.StatusPacketResponses.Handlers[id];
                    break;
            }

            if (handler != null)
            {
                handler.SetContext(this);
                var response = handler.Handle(packet);
                if (response != null)
                    SendPacket(response);
            }

            DoCustomReceiving(packet);
        }
    }
}