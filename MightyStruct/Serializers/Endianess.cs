using System;

namespace MightyStruct.Serializers
{
    public enum Endianness
    {
        LittleEndian = 0,
        BigEndian = 1
    }

    public static class EndianInfo
    {
        public static readonly Endianness SystemEndianness = 
            BitConverter.IsLittleEndian ? 
            Endianness.LittleEndian : 
            Endianness.BigEndian;
    }
}
