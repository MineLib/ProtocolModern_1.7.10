using Aragas.Core.Extensions;
using Aragas.Core.PacketHandlers;
using Aragas.Core.Packets;
using Aragas.Core.Wrappers;

using ProtocolModern.Enum;

namespace ProtocolModern.PacketHandlers
{
    public static class ClientPacketHandlers
    {
        public static class LoginPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static LoginPacketResponses()
            {
                new ServerLoginPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(LoginPacketResponses)));
            }
        }

        public static class PlayPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static PlayPacketResponses()
            {
                new ServerPlayPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(PlayPacketResponses)));
            }
        }

        public static class StatusPacketResponses
        {
            public static readonly ContextFunc<ProtobufPacket>[] Handlers;

            static StatusPacketResponses()
            {
                new ServerStatusPacketTypes().CreateHandlerInstancesOut(out Handlers, AppDomainWrapper.GetAssembly(typeof(StatusPacketResponses)));
            }
        }
    }
}
