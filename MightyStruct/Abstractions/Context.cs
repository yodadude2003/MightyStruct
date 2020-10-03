using System.Collections.Generic;
using System.IO;

namespace MightyStruct.Abstractions
{
    public class Context
    {
        public IStruct Self { get; }
        public IStruct Parent { get; }

        public Stream Stream { get; }

        public Dictionary<string, IType> Types { get; }
        public Dictionary<string, object> Variables { get; }

        public Context(IStruct self, IStruct parent, Stream stream)
        {
            Self = self;
            Parent = parent;

            Stream = stream;

            Types = new Dictionary<string, IType>();
            Variables = new Dictionary<string, object>();
        }

        public Context(Context context)
        {
            Self = context.Self;
            Parent = context.Parent;

            Stream = context.Stream;

            Types = new Dictionary<string, IType>(context.Types);
            Variables = new Dictionary<string, object>(context.Variables);
        }

        public Context(Context context, IStruct newSelf)
        {
            Self = newSelf;
            Parent = context.Self;

            Stream = new SubStream(context.Stream, context.Stream.Position);

            Types = new Dictionary<string, IType>(context.Types);
            Variables = new Dictionary<string, object>(context.Variables);
        }
    }
}
