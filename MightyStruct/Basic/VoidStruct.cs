using MightyStruct.Runtime;

using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class VoidStruct : IStruct
    {
        private Context Context { get; }
        
        private long Length { get; }

        public Stream Stream => Context.Stream;

        public VoidStruct(Context context, long length)
        {
            Context = new Context(context, this);

            Length = length;
        }

        public Task ParseAsync()
        {
            Context.Stream.SetLength(Length);
            Context.Stream.Position = Length;

            (Context.Stream as SubStream)?.Lock();

            return Task.CompletedTask;
        }

        public Task UpdateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
