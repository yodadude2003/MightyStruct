using System;
using System.IO;

namespace MightyStruct.Core
{
    public abstract class SubStream : Stream
    {
        public Stream Parent { get; }

        public long Offset { get; }
        public override long Length { get; }

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

        public SubStream(Stream parent, long offset, long length)
        {
            Parent = parent;

            Offset = offset;
            Length = length;
        }

        public override bool CanRead => Parent.CanRead;
        public override bool CanWrite => Parent.CanWrite;

        public override bool CanSeek => Parent.CanSeek;

        public override bool CanTimeout => Parent.CanTimeout;

        public override int Read(byte[] buffer, int offset, int count)
        {
            Seek(0, SeekOrigin.Current);

            long toRead = (Position + count > Length) ? Length - Position : count;
            return Parent.Read(buffer, offset, (int)toRead);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position + count > Length)
                throw new ArgumentOutOfRangeException("The requested write operation exceeds the the boundaries of the substream.");

            Seek(0, SeekOrigin.Current);
            Parent.Write(buffer, offset, count);
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
    }
}
