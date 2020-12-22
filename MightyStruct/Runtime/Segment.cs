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
using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class Segment
    {
        public Segment Parent { get; }
        public Segment Root => Parent?.Parent ?? this;

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

        public List<Pointer> Pointers { get; }
        public List<Segment> SubSegments { get; }

        public Stream Stream { get; private set; }

        public Segment(Stream stream)
        {
            Stream = stream;
            SubSegments = new List<Segment>();
            Pointers = new List<Pointer>();
        }

        public Segment(Segment parent, long offset)
        {
            Parent = parent;
            Parent?.SubSegments.Add(this);

            Stream = new SubStream(Parent.Stream, offset);

            Pointers = new List<Pointer>();
            SubSegments = new List<Segment>();
        }

        public async Task CopyAllBeforeAsync(Stream stream)
        {
            if (Parent == null) return;

            await Parent.CopyAllBeforeAsync(stream);

            var segments = Parent.SubSegments
                .OrderBy(s => (s.Stream as SubStream).AbsoluteOffset)
                .TakeWhile(s => s != this);

            foreach (var segment in segments)
            {
                segment.Stream.Seek(0, SeekOrigin.Begin);
                await segment.Stream.CopyToAsync(stream);
            }
        }

        public async Task CopyAllAfterAsync(Stream stream)
        {
            if (Parent == null) return;

            var segments = Parent.SubSegments
                .OrderBy(s => (s.Stream as SubStream).AbsoluteOffset)
                .SkipWhile(s => s != this).Skip(1);

            foreach (var segment in segments)
            {
                segment.Stream.Seek(0, SeekOrigin.Begin);
                await segment.Stream.CopyToAsync(stream);
            }

            await Parent.CopyAllAfterAsync(stream);
        }

        public async Task UpdateAllPointersAfterAsync(Stream stream, short offset)
        {
            if (Parent == null) return;

            var segments = Parent.SubSegments
                .OrderBy(s => (s.Stream as SubStream).AbsoluteOffset)
                .SkipWhile(s => s != this).Skip(1);

            foreach (var segment in segments)
            {
                foreach (var pointer in segment.Pointers)
                {
                    if (pointer.Base == null || Ancestors.Contains(pointer.Base))
                        await pointer.AddAsync(offset);
                }
            }

            await Parent.UpdateAllPointersAfterAsync(stream, offset);
        }

        public async Task ResizeAsync(long newSize)
        {
            var stream = File.Create("temp.bin");

            await CopyAllBeforeAsync(stream);

            Stream.Seek(0, SeekOrigin.Begin);
            await Stream.CopyToAsync(stream);

            short offset = (short)(newSize - Stream.Length);
            stream.Seek(offset, SeekOrigin.Current);

            await CopyAllAfterAsync(stream);

            await Root.RebaseAsync(stream);
            await UpdateAllPointersAfterAsync(stream, offset);
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
    }
}