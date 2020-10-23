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

using System.IO;

namespace MightyStruct.Basic
{
    public class SafeStream
    {
        private Stream Stream { get; }

        public SafeStream _parent => new SafeStream((Stream as SubStream)?.Parent);

        public long pos => Stream.Position;
        public long size => Stream.Length;

        public SafeStream(Stream stream)
        {
            Stream = stream;
        }
    }
}
