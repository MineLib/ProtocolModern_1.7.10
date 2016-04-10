using System.Collections.Generic;
using System.Text;

using Aragas.Core.Data;
using Aragas.Core.IO;
using Aragas.Core.Wrappers;

using ProtocolModern.Data;
using ProtocolModern.Packets.Server.Play;

namespace ProtocolModern.Client
{
    public enum FMLHandshakeClientState : byte
    {
        START,
        HELLO,
        WAITINGSERVERDATA,
        WAITINGSERVERCOMPLETE,
        PENDINGCOMPLETE,
        COMPLETE,
        DONE,
        ERROR
    }

    public enum FMLHandshakeDiscriminator : byte
    {
        ServerHello     = 0,
        ClientHello     = 1,
        ModList         = 2,
        RegistryData    = 3,
        HandshakeAck    = 255, //-1
        HandshakeReset  = 254, //-2
    }

    public sealed partial class Protocol
    {
        private ForgeModInfo ForgeInfo { get; set; }
        private FMLHandshakeClientState ForgeState { get; set; }
        private int ForgeProtocolVersion { get; set; }

        private bool OnPluginChannelMessageForge(string channel, byte[] data)
        {
            if (channel == "FML|HS")
            {
                using (var reader = new ProtobufDataReader(data))
                {
                    var discriminator = (FMLHandshakeDiscriminator) reader.Read<byte>();
                    if (discriminator == FMLHandshakeDiscriminator.HandshakeReset)
                    {
                        ForgeState = FMLHandshakeClientState.START;
                        return true;
                    }


                    switch (ForgeState)
                    {
                        case FMLHandshakeClientState.START:
                            if (discriminator != FMLHandshakeDiscriminator.ServerHello)
                                return false;

                            var statusClient = CreateStatusClient;
                            var serverInfo = statusClient.GetServerInfo(Host, Port, ProtocolVersion);
                            var response = (ServerResponse) serverInfo.ServerResponse;
                            if (response.Info.ModInfo != null)
                                ForgeInfo = response.Info.ModInfo.Value;

                            // Send the plugin channel registration.
                            // REGISTER is somewhat special in that it doesn't actually include length information,
                            // and is also \0-separated.
                            // Also, yes, "FML" is there twice.  Don't ask me why, but that's the way forge does it.
                            string[] channels = { "FML|HS", "FML", "FML|MP", "FML", "FORGE" };
                            SendPacket(new PluginMessage2Packet { Channel = "REGISTER", Data = Encoding.UTF8.GetBytes(string.Join("\0", channels))});

                            ForgeProtocolVersion = reader.Read<byte>();
                            // There's another value afterwards for the dimension, but we don't need it.

                            // Tell the server we're running the same version.
                            SendForgeHandshakePacket(FMLHandshakeDiscriminator.ClientHello, (byte) ForgeProtocolVersion);

                            // Then tell the server that we're running the same mods.
                            var mods = new byte[ForgeInfo.Mods.Count][];
                            for (var i = 0; i < ForgeInfo.Mods.Count; i++)
                            {
                                var mod = ForgeInfo.Mods[i];
                                mods[i] = ForgeConcatBytes(ForgeGetString(mod.ModID), ForgeGetString(mod.Version));
                            }
                            SendForgeHandshakePacket(FMLHandshakeDiscriminator.ModList,
                                ForgeConcatBytes(new VarInt(ForgeInfo.Mods.Count).Encode(), ForgeConcatBytes(mods)));

                            ForgeState = FMLHandshakeClientState.WAITINGSERVERDATA;

                            return true;
                        case FMLHandshakeClientState.WAITINGSERVERDATA:
                            if (discriminator != FMLHandshakeDiscriminator.ModList)
                                return false;

                            ThreadWrapper.Sleep(2000);

                            // Tell the server that yes, we are OK with the mods it has
                            // even though we don't actually care what mods it has.

                            SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck, (byte) FMLHandshakeClientState.WAITINGSERVERDATA);

                            ForgeState = FMLHandshakeClientState.WAITINGSERVERCOMPLETE;
                            return false;
                        case FMLHandshakeClientState.WAITINGSERVERCOMPLETE:
                            // The server now will tell us a bunch of registry information.
                            // We need to read it all, though, until it says that there is no more.
                            if (discriminator != FMLHandshakeDiscriminator.RegistryData)
                                return false;

                            
                            // 1.7.10 and below have one registry
                            // with blocks and items.
                            int registrySize = reader.Read<VarInt>();

                            ForgeState = FMLHandshakeClientState.PENDINGCOMPLETE;
                            

                            return false;
                        case FMLHandshakeClientState.PENDINGCOMPLETE:
                            // The server will ask us to accept the registries.
                            // Just say yes.
                            if (discriminator != FMLHandshakeDiscriminator.HandshakeAck)
                                return false;
                            SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck, (byte) FMLHandshakeClientState.PENDINGCOMPLETE);
                            ForgeState = FMLHandshakeClientState.COMPLETE;

                            return true;
                        case FMLHandshakeClientState.COMPLETE:
                            // One final "OK".  On the actual forge source, a packet is sent from
                            // the client to the client saying that the connection was complete, but
                            // we don't need to do that.

                            SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck, (byte) FMLHandshakeClientState.COMPLETE );
                            ForgeState = FMLHandshakeClientState.DONE;
                            return true;
                    }
                }
            }

            return false;
        }

        private void SendForgeHandshakePacket(FMLHandshakeDiscriminator discriminator, params byte[] data)
        {
            SendPacket(new PluginMessage2Packet
            {
                Channel = "FML|HS",
                Data = ForgeConcatBytes(new byte[] {(byte) discriminator}, data)
            });
        }
        private static byte[] ForgeConcatBytes(params byte[][] bytes)
        {
            var result = new List<byte>();
            foreach (var array in bytes)
                result.AddRange(array);
            return result.ToArray();
        }
        private static byte[] ForgeGetString(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return ForgeConcatBytes(new VarInt(bytes.Length).Encode(), bytes);
        }
    }
}
