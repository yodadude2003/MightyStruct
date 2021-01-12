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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class Segment
    {
        public Segment Parent { get; }
        public Segment Root => Parent?.Root ?? Parent;

        public List<Segment> Ancestors
        {
            get
            {
                if (Parent == null)
                    return new List<Segment>();
                else
                {

                    var list = Parent.Ancestors;
                    list.Add(Parent);
                    return list;
                }
            }
        }

        public Pointer Pointer { get; }
        public List<Segment> SubSegments { get; }

        public Stream Stream { get; private set; }

        public Segment(Stream stream)
        {
            Stream = stream;
            SubSegments = new List<Segment>();
            Pointer = null;
        }

        public Segment(Segment parent, Pointer pointer = null)
        {
            Pointer = pointer;

            if (Pointer == null)
            {
                Parent = parent;
                Stream = new SubStream(Parent.Stream, Parent.Stream.Position);
            }
            else
            {
                Parent = parent.Root;

                long offset = ((Pointer.Base?.Stream as SubStream)?.AbsoluteOffset ?? 0) + Pointer.Inner.Value + Pointer.Offset;
                Stream = new SubStream(Parent.Stream, offset);
            }

            Parent?.SubSegments.Add(this);
            SubSegments = new List<Segment>();
        }

        public List<Segment> GetAllSegments()
        {
            return SubSegments
                .SelectMany(s => s.GetAllSegments())
                .Concat(SubSegments)
                .OrderBy(s => (s.Stream as SubStream).AbsoluteOffset)
                .ToList();
        }

        public async Task UpdatePointersAsync(Segment resized, short offset)
        {
            var segments = GetAllSegments()
                .SkipWhile(s => s != resized).Skip(1);

            foreach (var segment in segments.Where(s => resized.Ancestors.Contains(s.Parent)))
            {
                var subStream = new SubStream(segment.Parent.Stream, (segment.Stream as SubStream).Offset + offset);
                subStream.SetLength(segment.Stream.Length);
                subStream.Lock();

                segment.Stream = subStream;
                foreach (var subSegment in segment.SubSegments)
                {
                    await subSegment.RebaseAsync(segment.Stream);
                }
            }

            foreach (var segment in segments.Where(s => s.Pointer != null))
            {
                if (segment.Pointer.Base == null || segment.Pointer.Base == resized.Pointer?.Base)
                    await segment.Pointer.AddAsync(offset);
            }
        }

        public async Task RebaseAsync(Stream newStream)
        {
            if (Stream is SubStream)
            {
                var subStream = new SubStream(newStream, (Stream as SubStream).Offset);
                subStream.SetLength(Stream.Length);
                subStream.Lock();
                Stream = subStream;
            }
            else
                Stream = newStream;

            foreach (var segment in SubSegments)
            {
                await segment.RebaseAsync(Stream);
            }
        }

        public async Task ResizeAsync(long newSize)
        {
            var root = Root.Stream;
            var newRoot = File.Create($"{DateTime.Now.Ticks}.bin");

            Stream.Seek(0, SeekOrigin.Begin);

            var allBefore = new SubStream(root, 0);
            allBefore.SetLength(root.Position);
            allBefore.Lock();

            await allBefore.CopyToAsync(newRoot);
            await Stream.CopyToAsync(newRoot);

            short offset = (short)(newSize - Stream.Length);
            newRoot.Seek(offset, SeekOrigin.Current);

            var subStream = new SubStream(Parent.Stream, (Stream as SubStream).Offset);
            subStream.SetLength(newSize);
            subStream.Lock();
            Stream = subStream;

            await root.CopyToAsync(newRoot);

            await Root.RebaseAsync(newRoot);
            await Root.UpdatePointersAsync(this, offset);
        }
    }
}