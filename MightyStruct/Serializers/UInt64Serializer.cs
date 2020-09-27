using MightyStruct.Core;
using System;
using System.IO;

namespace MightyStruct.Serializers
{
    public class UInt64Serializer : ISerializer<ulong>
    {
        public ulong ReadFromStream(Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public void WriteToStream(Stream stream, ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
