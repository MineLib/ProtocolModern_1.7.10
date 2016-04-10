using Aragas.Core.Extensions;
using Aragas.Core.PacketHandlers;
using Aragas.Core.Packets;
using Aragas.Core.Wrappers;

using ProtocolModern.Enum;

namespace ProtocolModern.PacketHandlers
{
    public static class ServerPacketHandlers
    {
        public static class LoginPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static LoginPacketResponses()
            {
                new ClientLoginPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(LoginPacketResponses)));
            }
        }

        public static class PlayPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static PlayPacketResponses()
            {
                new ClientPlayPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(PlayPacketResponses)));
            }
        }

        public static class StatusPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static StatusPacketResponses()
            {
                new ClientStatusPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(StatusPacketResponses)));
            }
        }
    }
}
