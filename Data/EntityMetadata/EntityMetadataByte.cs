﻿using Aragas.Core.IO;

using MineLib.Core.Data;

namespace ProtocolModern.Data.EntityMetadata
{
    /// <summary>
    /// Byte Metadata
    /// </summary>
    public class EntityMetadataByte : EntityMetadataEntry
    {
        protected override byte Identifier => 0;
        protected override string FriendlyName => "byte";

        public byte Value;

        public static implicit operator EntityMetadataByte(byte value) => new EntityMetadataByte(value);

        public EntityMetadataByte() { }
        public EntityMetadataByte(byte value) { Value = value; }

        public override void FromReader(PacketDataReader reader)
        {
            Value = reader.Read(Value);
        }
        public override void ToStream(PacketStream stream, byte index)
        {
            stream.Write(GetKey(index));
            stream.Write(Value);
        }
    }
}
