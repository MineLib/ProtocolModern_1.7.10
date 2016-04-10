﻿using System;
using System.Collections;

using Aragas.Core.IO;

namespace ProtocolModern.Data
{
    // TODO: Equatable performance
    public struct DisplayedSkinParts : IEquatable<DisplayedSkinParts>
    {
        public bool CapeEnabled;
        public bool JackedEnabled;
        public bool LeftSleeveEnabled;
        public bool RightSleeveEnabled;
        public bool LeftPantsEnabled;
        public bool RightPantsEnabled;
        public bool HatEnabled;
        public bool Unused;

        public static DisplayedSkinParts FromReader(PacketDataReader reader)
        {
            var value = reader.Read<byte>();

            return FromByte(value);
        }

        public static DisplayedSkinParts FromByte(byte value)
        {
            var bitArray = new BitArray(new byte[value]);
            var boolArray = new bool[7];
            ((ICollection) bitArray).CopyTo(boolArray, 0);

            return new DisplayedSkinParts
            {
                CapeEnabled = boolArray[0],
                JackedEnabled = boolArray[1],
                LeftSleeveEnabled = boolArray[2],
                RightSleeveEnabled = boolArray[3],
                LeftPantsEnabled = boolArray[4],
                RightPantsEnabled = boolArray[5],
                HatEnabled = boolArray[6],
                Unused = boolArray[7]
            };
        }

        public void ToStream(PacketStream stream)
        {
            var value = ToByte();

            stream.Write(value);
        }

        public byte ToByte()
        {
            var bitArray = new BitArray(new bool[] { CapeEnabled, JackedEnabled, LeftSleeveEnabled, RightSleeveEnabled, LeftPantsEnabled, RightPantsEnabled, HatEnabled, Unused });
            var byteArray = new byte[1];
            ((ICollection) bitArray).CopyTo(byteArray, 0);

            return byteArray[0];
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((DisplayedSkinParts) obj);
        }

        public bool Equals(DisplayedSkinParts other)
        {
            return ToByte() == other.ToByte();
        }

        public override int GetHashCode()
        {
            return ToByte().GetHashCode();
        }
    }
}
