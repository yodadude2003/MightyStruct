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

namespace MightyStruct
{
    public class SubStream : Stream
    {
        public Stream Parent { get; }

        public long Offset { get; }

        public long AbsoluteOffset => (Parent is SubStream) ? (Parent as SubStream).AbsoluteOffset + Offset : Offset;
        public Stream Root => (Parent is SubStream) ? (Parent as SubStream).Root : Parent;

        public override long Length { get => _length; }
        private long _length;
        private bool _locked;

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                Parent.Seek(Offset + _position, SeekOrigin.Begin);
            }
        }
        private long _position;

        public SubStream(Stream parent, long offset)
        {
            Parent = parent;
            Offset = offset;
        }

        public void Lock()
        {
            if (_locked)
                throw new InvalidOperationException("Cannot redudantly lock a SubStream is locked.");
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
        }

        public override bool CanRead => Parent.CanRead;
        public override bool CanWrite => Parent.CanWrite;

        public override bool CanSeek => Parent.CanSeek;

        public override bool CanTimeout => Parent.CanTimeout;

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead;
            Seek(0, SeekOrigin.Current);
            if (_locked)
            {
                if (_position + count > Length)
                    bytesRead = Parent.Read(buffer, offset, (int)(Length - _position));
                else
                    bytesRead = Parent.Read(buffer, offset, count);

                _position += bytesRead;
            }
            else
            {
                bytesRead = Parent.Read(buffer, offset, count);
                SetLength(_position += bytesRead);
            }
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Seek(0, SeekOrigin.Current);

            Parent.Write(buffer, offset, count);
            SetLength(_position += count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            if (Offset + value > Parent.Length)
                Parent.SetLength(Offset + value);
            _length = value;
        }

        public override void Flush()
        {
            Parent.Flush();
        }
    }
}
