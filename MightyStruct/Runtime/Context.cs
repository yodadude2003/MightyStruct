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

namespace MightyStruct.Runtime
{
    public class Context
    {
        public IStruct Self { get; }
        public IStruct Parent { get; }

        public Stream Stream { get; }

        public long? Offset { get; }

        public Variables Variables { get; set; }

        public Context(Stream stream)
        {
            Stream = stream;
        }

        public Context(Context context)
        {
            Self = context.Self;
            Parent = context.Parent;

            Stream = context.Stream;

            Offset = context.Offset;

            Variables = context.Variables;
        }

        public Context(Context context, long offset) : this(context)
        {
            Offset = offset;
        }

        public Context(Context parent, IStruct newSelf)
        {
            Self = newSelf;
            Parent = parent.Self;

            Stream = new SubStream(parent.Stream, parent.Offset ?? parent.Stream.Position);

            Variables = parent.Variables;
        }
    }
}
