using MightyStruct.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public interface IPrimitiveStruct : IStruct
    {
        object Value { get; set; }
    }

    public class PrimitiveStruct<T> : IPrimitiveStruct
    {
        private ISerializer<T> Serializer { get; }

        private Context Context { get; }

        public T Value { get; set; }

        object IPrimitiveStruct.Value { get => Value; set => Value = (T)value; }

        public PrimitiveStruct(Context context, ISerializer<T> serializer)
        {
            Context = new Context(context, this);
            Serializer = serializer;
        }

        public async Task ParseAsync()
        {
            Value = await Serializer.ReadFromStreamAsync(Context.Stream);
        }

        public Task UpdateAsync()
        {
            Context.Stream.Seek(0, SeekOrigin.Begin);
            return Serializer.WriteToStreamAsync(Context.Stream, Value);
        }
    }
}
