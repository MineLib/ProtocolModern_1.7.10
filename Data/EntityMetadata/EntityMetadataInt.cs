﻿using Aragas.Core.IO;

using MineLib.Core.Data;

namespace ProtocolModern.Data.EntityMetadata
{
    /// <summary>
    /// Int32 Metadata
    /// </summary>
    public class EntityMetadataInt : EntityMetadataEntry
    {
        protected override byte Identifier => 2;
        protected override string FriendlyName => "int";

        public int Value;

        public static implicit operator EntityMetadataInt(int value) => new EntityMetadataInt(value);

        public EntityMetadataInt() { }
        public EntityMetadataInt(int value) { Value = value; }

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