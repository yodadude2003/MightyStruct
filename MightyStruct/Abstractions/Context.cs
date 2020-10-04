using System.IO;

namespace MightyStruct.Abstractions
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
