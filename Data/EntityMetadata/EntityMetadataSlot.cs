﻿using Aragas.Core.IO;

using MineLib.Core.Data;

namespace ProtocolModern.Data.EntityMetadata
{
    /// <summary>
    /// Slot Metadata
    /// </summary>
    public class EntityMetadataSlot : EntityMetadataEntry
    {
        protected override byte Identifier => 5;
        protected override string FriendlyName => "slot";

        public ItemStack Value;

        public static implicit operator EntityMetadataSlot(ItemStack value) => new EntityMetadataSlot(value);

        public EntityMetadataSlot() { }
        public EntityMetadataSlot(ItemStack value) { Value = value; }

        public override void FromReader(PacketDataReader reader)
        {
            Value = reader.Read<ItemStack>();
        }
        public override void ToStream(PacketStream stream, byte index)
        {
            stream.Write(GetKey(index));
            stream.Write(Value);
        }
    }
}