using System.IO;

namespace MightyStruct.Core
{
    public class SubStream : Stream
    {
        public Stream Parent { get; }

        public long Offset { get; }

        public override long Length { get => _length; }
        private long _length;

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

        public override bool CanRead => Parent.CanRead;
        public override bool CanWrite => Parent.CanWrite;

        public override bool CanSeek => Parent.CanSeek;

        public override bool CanTimeout => Parent.CanTimeout;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if ((_position += count) > Length)
                SetLength(_position);

            Seek(0, SeekOrigin.Current);
            return Parent.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((_position += count) > Length)
                SetLength(_position);

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

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void Flush()
        {
            return;
        }
    }
}
