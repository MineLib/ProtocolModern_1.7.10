using System;
using System.Collections.Generic;
using System.Text;

using Aragas.Core;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

using ProtocolModern.Packets.Client.Login;
using ProtocolModern.Packets.Server.Login;

namespace ProtocolModern.PacketHandlers.Server.Login
{
    public class EncryptionRequestHandler : ProtocolPacketHandler<EncryptionRequestPacket, EncryptionResponsePacket>
    {
        public override EncryptionResponsePacket Handle(EncryptionRequestPacket packet)
        {
            Context.ModernEnableEncryption(packet.ServerID, packet.PublicKey, packet.VerifyToken);

            return null;
            /*
            var generator = new CipherKeyGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 16 * 8));
            var sharedKey = generator.GenerateKey();
            Context.InitializeEncryption(sharedKey);

            var hash = GetServerIDHash(packet.PublicKey, sharedKey, packet.ServerID);
            if (!Yggdrasil.JoinSession(Context.AccessToken, Context.SelectedProfile, hash).Result.Response)
                throw new Exception("Yggdrasil error: Not authenticated.");

            var signer = new PKCS1Signer(packet.PublicKey);
            return new EncryptionResponsePacket
            {
                SharedSecret = signer.SignData(sharedKey),
                VerifyToken = signer.SignData(packet.VerifyToken)
            };
            */
        }


    }
}
