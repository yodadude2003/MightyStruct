using MightyStruct.Core;
using System;
using System.IO;

namespace MightyStruct.Serializers
{
    public class UInt32Serializer : ISerializer<uint>
    {
        public uint ReadFromStream(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public void WriteToStream(Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
