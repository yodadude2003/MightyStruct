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
        public IType Type { get; }

        public Context Context { get; }

        public T Value { get; set; }

        object IPrimitiveStruct.Value { get => Value; set => Value = (T)value; }

        public PrimitiveStruct(IType type, Context context)
        {
            Type = type;

            Context = new Context(context, this);
        }

        public async Task ParseAsync()
        {
            Value = await (Type as PrimitiveType<T>).Serializer.ReadFromStreamAsync(Context.Stream);
        }

        public Task UpdateAsync()
        {
            Context.Stream.Seek(0, SeekOrigin.Begin);
            return (Type as PrimitiveType<T>).Serializer.WriteToStreamAsync(Context.Stream, Value);
        }
    }
}
