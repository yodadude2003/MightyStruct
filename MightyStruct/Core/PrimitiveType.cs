using System.IO;

namespace MightyStruct.Core
{
    public class PrimitiveType<T> : IType
    {
        public string Name { get; }

        public ISerializer<T> Serializer { get; }

        public PrimitiveType(string name, ISerializer<T> serializer)
        {
            Name = name;
            Serializer = serializer;
        }

        public IStruct CreateInstance(IStruct parent, Stream stream)
        {
            return new PrimitiveStruct<T>(this, parent, stream);
        }
    }
}
