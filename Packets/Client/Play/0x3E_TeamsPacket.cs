using System;

using Aragas.Core.Data;
using Aragas.Core.IO;
using Aragas.Core.Packets;

using MineLib.Core.Data;

using ProtocolModern.Enum;

namespace ProtocolModern.Packets.Client.Play
{
    public class TeamsPacket : ProtobufPacket
    {
		public String TeamName;
		public SByte Mode;
		public String TeamDisplayName;
		public String TeamPrefix;
		public String TeamSuffix;
		public SByte FriendlyFire;
		public Int16 PlayerCount;
		public NotSupportedType Players;

        public override VarInt ID => ClientPlayPacketTypes.Teams;

        public override ProtobufPacket ReadPacket(ProtobufDataReader reader)
        {
			TeamName = reader.Read(TeamName);
			Mode = reader.Read(Mode);
			TeamDisplayName = reader.Read(TeamDisplayName);
			TeamPrefix = reader.Read(TeamPrefix);
			TeamSuffix = reader.Read(TeamSuffix);
			FriendlyFire = reader.Read(FriendlyFire);
			PlayerCount = reader.Read(PlayerCount);
			Players = reader.Read(Players);

            return this;
        }

        public override ProtobufPacket WritePacket(ProtobufStream stream)
        {
			stream.Write(TeamName);
			stream.Write(Mode);
			stream.Write(TeamDisplayName);
			stream.Write(TeamPrefix);
			stream.Write(TeamSuffix);
			stream.Write(FriendlyFire);
			stream.Write(PlayerCount);
			stream.Write(Players);
          
            return this;
        }

    }
}