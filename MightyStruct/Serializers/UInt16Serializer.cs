using System;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class UInt16Serializer : ISerializer<ushort>
    {
        public async Task<ushort> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[2];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            (stream as SubStream)?.Lock();
            return BitConverter.ToUInt16(buffer, 0);
        }

        public async Task WriteToStreamAsync(Stream stream, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
