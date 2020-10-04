using System;
using System.IO;

namespace MightyStruct.Abstractions
{
    public class SubStream : Stream
    {
        public Stream Parent { get; }

        public long Offset { get; }

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
                throw new InvalidOperationException("Sub-stream is already locked.");
            _locked = true;
        }

        public override bool CanRead => Parent.CanRead;
        public override bool CanWrite => Parent.CanWrite;

        public override bool CanSeek => Parent.CanSeek;

        public override bool CanTimeout => Parent.CanTimeout;

        public override int Read(byte[] buffer, int offset, int count)
        {
            Seek(0, SeekOrigin.Current);
            int bytesRead = Parent.Read(buffer, offset, count);
            if (_position + count > Length)
            {
                if (_locked)
                {
                    bytesRead = (int)(Length - _position);
                    _position = Length;
                }
                else
                {
                    SetLength(_position += count);
                }
            }
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Seek(0, SeekOrigin.Current);
            Parent.Write(buffer, offset, count);
            if (_position + count > Length)
            {
                if (_locked)
                {
                    throw new InvalidOperationException("Attempted to write past the stream's boundaries");
                }
                else
                {
                    SetLength(_position += count);
                }
            }
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
            if (_locked)
                throw new InvalidOperationException("Sub-stream is locked and its length cannot be changed.");
            _length = value;
        }

        public override void Flush()
        {
            return;
        }
    }
}
