using System;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class PrimitiveStruct<T> : IStruct
        where T : struct
    {
        public IType Type { get; }

        public IStruct Parent { get; }
        public IStruct Root { get; }

        public Stream Stream { get; }

        public T? Value { get; set; }

        public PrimitiveStruct(IStruct parent, Stream stream)
        {
            Parent = parent;
            Root = Parent?.Root;
        }

        public async Task ParseAsync()
        {
            Value = await (Type as PrimitiveType<T>).Serializer.ReadFromStreamAsync(Stream);
        }

        public Task UpdateAsync()
        {
            if (Value.HasValue)
                return (Type as PrimitiveType<T>).Serializer.WriteToStreamAsync(Stream, Value.Value);
            else
                return Task.FromException(new InvalidOperationException());
        }
    }
}
