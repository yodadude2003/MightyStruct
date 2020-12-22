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
