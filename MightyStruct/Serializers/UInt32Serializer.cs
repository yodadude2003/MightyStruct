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
    public class UInt32Serializer : ISerializer<uint>
    {
        public async Task<uint> ReadFromStreamAsync(Stream stream)
        {
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            (stream as SubStream)?.Lock();
            return BitConverter.ToUInt32(buffer, 0);
        }

        public async Task WriteToStreamAsync(Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
