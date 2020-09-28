using System;
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

        public IStruct Parent { get; }
        public IStruct Root { get; }

        public Stream Stream { get; }

        public T Value { get; set; }

        object IPrimitiveStruct.Value { get => Value; set => Value = (T)value; }

        public PrimitiveStruct(IType type, IStruct parent, Stream stream)
        {
            Type = type;

            Parent = parent;
            Root = Parent?.Root;

            Stream = stream;
        }

        public async Task ParseAsync()
        {
            Value = await (Type as PrimitiveType<T>).Serializer.ReadFromStreamAsync(Stream);
        }

        public Task UpdateAsync()
        {
            return (Type as PrimitiveType<T>).Serializer.WriteToStreamAsync(Stream, Value);
        }
    }
}
