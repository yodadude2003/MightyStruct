/*
 *  Copyright 2020 Chosen Few Software
 *  This file is part of MightyStruct.
 *
 *  MightyStruct is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  MightyStruct is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with MightyStruct.  If not, see <https://www.gnu.org/licenses/>.
 */

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
