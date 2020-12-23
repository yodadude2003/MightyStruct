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

        public Segment Segment { get; }
        public Stream Stream => Segment.Stream;

        public Pointer Pointer { get; }

        public Variables Variables { get; set; }

        public Context(Segment segment)
        {
            Segment = segment;
        }

        public Context(Context context)
        {
            Self = context.Self;
            Parent = context.Parent;

            Pointer = context.Pointer;
            Segment = context.Segment;

            Variables = context.Variables;
        }

        public Context(Context context, Pointer pointer) : this(context)
        {
            Pointer = pointer;
        }

        public Context(Context parent, IStruct newSelf)
        {
            Self = newSelf;
            Parent = parent.Self;

            if (parent.Pointer != null)
            {
                long offset = ((parent.Pointer.Base?.Stream as SubStream)?.AbsoluteOffset ?? 0) + parent.Pointer.Inner.Value + parent.Pointer.Offset;

                Segment = new Segment(parent.Segment.Root, offset);
                Segment.Pointers.Add(parent.Pointer);
            }
            else
            {
                Segment = new Segment(parent.Segment, parent.Stream.Position);
            }

            Variables = parent.Variables;
        }
    }
}
