// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static LibreLancer.Net.Protocol.BitPrimitives;

namespace LibreLancer.Net.Protocol
{
    public ref struct BitReader
    {
        private ReadOnlySpan<byte> array;
        private int bitsOffset;
        public int BitsLeft => (array.Length * 8) - bitsOffset;
        public NetHpidReader HpidReader;

        public BitReader(ReadOnlySpan<byte> array, int bitsOffset, NetHpidReader hpidReader = null)
        {
            this.array = array;
            this.bitsOffset = bitsOffset;
            this.HpidReader = hpidReader;
        }

        public string GetHpid()
        {
            if (HpidReader == null) throw new InvalidOperationException();
            var idx = GetVarUInt32();
            if (idx == 0) return null;
            else if (idx == 1) return "";
            else return HpidReader.GetString(idx - 2);
        }

        public int GetInt()
        {
            return (int) GetUInt();
        }

        [StructLayout(LayoutKind.Explicit)]
        struct F2I
        {
            [FieldOffset(0)] public float f;
            [FieldOffset(0)] public uint i;
        }

        public float GetFloat()
        {
            var c = new F2I() {i = GetUInt()};
            return c.f;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(GetFloat(), GetFloat(), GetFloat());
        }

        public int GetVarInt32() => (int) GetVarInt64();

        public long GetVarInt64() => NetPacking.Zag64(GetVarUInt64());

        public uint GetVarUInt32() => (uint) GetVarUInt64();

        public ulong GetVarUInt64()
        {
            long b = GetByte();
            ulong a = (ulong) (b & 0x7f);
            int extraCount = 0;
            //first extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (uint) ((b & 0x7f) << 7);
                extraCount++;
            }

            //second extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (uint) ((b & 0x7f) << 14);
                extraCount++;
            }

            //third extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (uint) ((b & 0x7f) << 21);
                extraCount++;
            }

            //fourth extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (ulong) ((b & 0x7f) << 28);
                extraCount++;
            }

            //fifth extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (ulong) ((b & 0x7f) << 35);
                extraCount++;
            }

            //sixth extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (ulong) ((b & 0x7f) << 42);
                extraCount++;
            }

            //seventh extra
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (ulong) ((b & 0x7f) << 49);
                extraCount++;
            }

            //Full ulong
            if ((b & 0x80) == 0x80)
            {
                b = GetByte();
                a |= (ulong) (((ulong) b) << 57);
                extraCount++;
            }

            switch (extraCount)
            {
                case 1:
                    a += 128;
                    break;
                case 2:
                    a += 16512;
                    break;
                case 3:
                    a += 2113663;
                    break;
                case 4:
                    a += 270549119;
                    break;
                case 5:
                    a += 34630197487;
                    break;
                case 6:
                    a += 4432676708591;
                    break;
                case 7:
                    a += 567382630129903;
                    break;
                case 8:
                    a += 72624976668057839;
                    break;
            }

            return a;
        }


        public Vector3 GetNormal()
        {
            var maxIndex = (int) GetUInt(2);
            var sign = GetBool();
            var a = GetRangedFloat(NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 14);
            var b = GetRangedFloat(NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 15);
            var c = (float) Math.Sqrt(1f - (a * a + b * b));
            if (sign) c = -c;
            if (maxIndex == 0)
                return new Vector3(c, a, b);
            else if (maxIndex == 1)
                return new Vector3(a, c, b);
            else
                return new Vector3(a, b, c);
        }

        public Quaternion GetQuaternion(int precision = NetPacking.BITS_COMPONENT)
        {
            var maxIndex = GetUInt(2);
            var a = GetUInt(precision);
            var b = GetUInt(precision);
            var c = GetUInt(precision);
            return NetPacking.UnpackQuaternion(precision, maxIndex, a, b, c);
        }

        public uint GetUInt(int bits = 32)
        {
            var retval = ReadUInt32(array, bitsOffset, bits);
            bitsOffset += bits;
            return retval;
        }

        public float GetRangedFloat(float min, float max, int bits) =>
            NetPacking.UnquantizeFloat(GetUInt(bits), min, max, bits);

        public Vector3 GetRangedVector3(float min, float max, int bits)
        {
            return new Vector3(GetRangedFloat(min, max, bits), GetRangedFloat(min, max, bits),
                GetRangedFloat(min, max, bits));
        }

        public float GetRadiansQuantized()
        {
            return GetRangedFloat(NetPacking.ANGLE_MIN, NetPacking.ANGLE_MAX, 16);
        }

        public byte GetByte()
        {
            if (!ValidateArgs(array.Length * 8, bitsOffset, 8, 8))
                ThrowArgumentOutOfRangeException();
            uint value = Unsafe.ReadUnaligned<uint>(ref Unsafe.AsRef(in array[bitsOffset >> 3]));
            var retval = (byte)ReadValue32(value, bitsOffset & 7, 8);
            bitsOffset += 8;
            return retval;
        }

        public bool GetBool()
        {
            return ReadBit(array, bitsOffset++);
        }

        public void Align()
        {
            int bitsUsed = bitsOffset & 0x7;
            int bitsFree = 8 - bitsUsed;
            if (bitsFree != 8) {
                bitsOffset += bitsFree;
            }
        }
    }
}
