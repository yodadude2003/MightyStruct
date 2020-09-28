using MightyStruct.Core;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class UInt8Serializer : ISerializer<byte>
    {
        public async Task<byte> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[1];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return buffer[0];
        }

        public async Task WriteToStreamAsync(Stream stream, byte value)
        {
            byte[] buffer = new byte[1] { value };
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
