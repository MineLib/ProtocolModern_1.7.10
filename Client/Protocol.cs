using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Aragas.Core;
using Aragas.Core.Data;
using Aragas.Core.IO;
using Aragas.Core.Packets;
using Aragas.Core.Wrappers;

using MineLib.Core;
using MineLib.Core.Exceptions;
using MineLib.Core.Interfaces;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

using ProtocolModern.Packets;
using ProtocolModern.Packets.Client.Play;
using ProtocolModern.Packets.Server.Login;

namespace ProtocolModern.Client
{
    public sealed partial class Protocol : MineLib.Core.Protocol
    {
        static Protocol()
        {
            Extensions.PacketExtensions.Init();
        }

        #region Properties

        public override string Name => "Modern";
        public override string Version => "1.7.10";
        public override int ProtocolVersion => 5;

        public override IStatusClient CreateStatusClient => new StatusClient();

        public override string Host => Stream?.Host ?? string.Empty;
        public override ushort Port => Stream?.Port ?? 0;
        public override bool Connected => Stream?.Connected ?? false;

        private bool IsForge { get; set; }

        #endregion Properties


        #region Debug

#if DEBUG

        private List<ProtobufPacket> PacketsReceived { get; } = new List<ProtobufPacket>();
        private List<ProtobufPacket> PacketsSended { get; } = new List<ProtobufPacket>();
        private List<ProtobufPacket> PluginMessage { get; } = new List<ProtobufPacket>();

        private List<ProtobufPacket> LastPackets => PacketsReceived?.GetRange(PacketsReceived.Count - 50, 50);
        private ProtobufPacket LastPacket => PacketsReceived[PacketsReceived.Count - 1];
#endif

        #endregion Debug


        private IThread ReadThread { get; set; }
        private CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();

        private ProtobufStream Stream { get; }


        public Protocol(MineLibClient client, ProtocolMode mode) : base(client, mode)
        {
            Stream = new ProtobufStream(TCPClientWrapper.Create());

            RegisterSupportedSendings();
        }


        private void ReadCycle()
        {
            while (!CancellationToken.IsCancellationRequested)
                PacketReceiver();
        }
        private void PacketReceiver()
        {
            var packetLength = Stream.ReadVarInt();
            if (packetLength == 0)
                throw new ProtocolException("Reading error: Packet Length size is 0");

            HandleData(Stream.Receive(packetLength));
        }


        /// <summary>
        /// Data is handled here. Compression and encryption are handled here too
        /// </summary>
        /// <param name="data">Packet byte[] data</param>
        private void HandleData(byte[] data)
        {
            using (var reader = new ProtobufDataReader(data))
            {
                var id = reader.Read<VarInt>();
                var packet = default(ProtobufPacket);

                switch (State)
                {
                    #region Status

                    case ConnectionState.InfoRequest:
                        if (ClientPackets.StatusPacketResponses.Packets[id] == null)
                            throw new ProtocolException("Reading error: Wrong Status packet ID.");

                        packet = ClientPackets.StatusPacketResponses.Packets[id]().ReadPacket(reader);

                        OnPacketHandled(id, packet, ConnectionState.InfoRequest);
                        break;

                    #endregion Status

                    #region Login

                    case ConnectionState.Joining:
                        if (ClientPackets.LoginPacketResponses.Packets[id] == null)
                            throw new ProtocolException("Reading error: Wrong Login packet ID.");

                        packet = ClientPackets.LoginPacketResponses.Packets[id]().ReadPacket(reader);

                        OnPacketHandled(id, packet, ConnectionState.Joining);
                        break;

                    #endregion Login

                    #region Play

                    case ConnectionState.Joined:
                        if (ClientPackets.PlayPacketResponses.Packets[id] == null)
                            break;
                            //throw new ProtocolException("Reading error: Wrong Play packet ID.");

                        packet = ClientPackets.PlayPacketResponses.Packets[id]().ReadPacket(reader);

                        OnPacketHandled(id, packet, ConnectionState.Joined);
                        break;

                    #endregion Play
                }

                if(packet == null)
                    return;

#if DEBUG
                if(packet is PluginMessagePacket)
                    PluginMessage.Add(packet);

                PacketsReceived.Add(packet);
#endif
            }
        }
        

        internal void SetState(ConnectionState state) { State = state; }


        #region Network

        public override void Connect(string host, ushort port)
        {
            if (Connected)
                throw new ProtocolException("Connection error: Already connected to server.");

            // -- Connect to server.
            Stream.Connect(host, port);

            // -- Begin data reading.
            if (ReadThread != null && ReadThread.IsRunning)
                throw new ProtocolException("Connection error: Thread already running.");
            else
            {
                ReadThread = ThreadWrapper.Create(ReadCycle);
                ReadThread.IsBackground = true;
                ReadThread.Name = "PacketReaderThread";
                ReadThread.Start();
            }

        }
        public override void Disconnect()
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            CancellationToken.Cancel();

            Stream.Disconnect();
        }
        private void SendPacket(ProtobufPacket packet)
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            Stream.SendPacket(packet);

#if DEBUG
            PacketsSended.Add(packet);
#endif
        }

        public void ModernEnableEncryption(string serverID, byte[] publicKey, byte[] verifyToken)
        {
            var generator = new CipherKeyGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 16 * 8));
            var sharedKey = generator.GenerateKey();

            var hash = GetServerIDHash(publicKey, sharedKey, serverID);
            if (!Yggdrasil.JoinSession(AccessToken, SelectedProfile, hash).Result.Response)
                throw new Exception("Yggdrasil error: Not authenticated.");

            var signer = new PKCS1Signer(publicKey);
            SendPacket(new EncryptionResponsePacket
            {
                SharedSecret = signer.SignData(sharedKey),
                VerifyToken = signer.SignData(verifyToken)
            });
            Stream.InitializeEncryption(sharedKey);
        }
        private static string GetServerIDHash(byte[] publicKey, byte[] secretKey, string serverID)
        {
            var hashlist = new List<byte>();
            hashlist.AddRange(Encoding.UTF8.GetBytes(serverID));
            hashlist.AddRange(secretKey);
            hashlist.AddRange(publicKey);

            return JavaHelper.JavaHexDigest(hashlist.ToArray());
        }

        #endregion


        public override void Dispose()
        {
            CancellationToken?.Cancel();

            Stream?.Dispose();

#if DEBUG
            PacketsReceived?.Clear();
            PacketsSended?.Clear();
#endif

            CancellationToken?.Dispose();
        }
    }
}
