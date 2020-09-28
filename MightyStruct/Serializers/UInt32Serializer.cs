using MightyStruct.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class UInt32Serializer : ISerializer<uint>
    {
        public async Task<uint> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public async Task WriteToStreamAsync(Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
