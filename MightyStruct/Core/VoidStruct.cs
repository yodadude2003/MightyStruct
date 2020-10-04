using MightyStruct.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class VoidStruct : IStruct
    {
        private Context Context { get; }

        public Stream Stream => Context.Stream;

        public VoidStruct(Context context, long length)
        {
            Context = new Context(context, this);

            Context.Stream.SetLength(length);
            (Context.Stream as SubStream)?.Lock();
        }

        public Task ParseAsync()
        {
            Context.Stream.Position = Context.Stream.Length;
            return Task.CompletedTask;
        }

        public Task UpdateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
