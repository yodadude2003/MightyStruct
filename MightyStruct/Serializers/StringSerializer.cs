using MightyStruct.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MightyStruct.Serializers
{
    public class StringSerializer : ISerializer<string>
    {
        public Encoding Encoding { get; }

        public StringSerializer(Encoding encoding)
        {
            Encoding = encoding;
        }

        public async Task<string> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[16];
            List<char> str = new List<char>();

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            char[] chars = Encoding.GetChars(buffer);

            while (bytesRead > 0 && !chars.Contains('\u0000'))
            {
                str.AddRange(chars);

                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                chars = Encoding.GetChars(buffer);
            }

            char[] leftover = chars
                .TakeWhile(c => c != '\u0000')
                .ToArray();

            if (bytesRead > leftover.Length + 1)
            {
                stream.Seek((leftover.Length + 1) - buffer.Length, SeekOrigin.Current);
                stream.SetLength(stream.Position);
                (stream as SubStream)?.Lock();
            }

            str.AddRange(leftover);

            return new string(str.ToArray());
        }

        public async Task WriteToStreamAsync(Stream stream, string value)
        {
            List<byte> buffer = Encoding.GetBytes(value).ToList(); buffer.Add(0);
            await stream.WriteAsync(buffer.ToArray(), 0, buffer.Count);
        }
    }
}
