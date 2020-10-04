using MightyStruct.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class UInt64Serializer : ISerializer<ulong>
    {
        public async Task<ulong> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[8];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            (stream as SubStream)?.Lock();
            return BitConverter.ToUInt64(buffer, 0);
        }

        public async Task WriteToStreamAsync(Stream stream, ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
