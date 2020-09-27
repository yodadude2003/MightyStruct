using MightyStruct.Core;
using System;
using System.IO;

namespace MightyStruct.Serializers
{
    public class UInt16Serializer : ISerializer<ushort>
    {
        public ushort ReadFromStream(Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public void WriteToStream(Stream stream, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
