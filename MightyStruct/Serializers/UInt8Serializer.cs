using MightyStruct.Core;
using System.IO;

namespace MightyStruct.Serializers
{
    public class UInt8Serializer : ISerializer<byte>
    {
        public byte ReadFromStream(Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, buffer.Length);
            return buffer[0];
        }

        public void WriteToStream(Stream stream, byte value)
        {
            byte[] buffer = new byte[1] { value };
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
