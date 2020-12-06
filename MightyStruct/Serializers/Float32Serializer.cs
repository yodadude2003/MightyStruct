using System;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class Float32Serializer : ISerializer<float>
    {
        public Endianness Endianness { get; }

        public Float32Serializer(Endianness endianness)
        {
            Endianness = endianness;
        }

        public async Task<float> ReadFromStreamAsync(Stream stream)
        {
            var buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            if (EndianInfo.SystemEndianness != Endianness)
                Array.Reverse(buffer);

            return BitConverter.ToSingle(buffer, 0);
        }

        public async Task WriteToStreamAsync(Stream stream, float value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (EndianInfo.SystemEndianness != Endianness)
                Array.Reverse(buffer);

            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
