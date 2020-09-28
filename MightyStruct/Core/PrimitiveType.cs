using System.IO;

namespace MightyStruct.Core
{
    public interface IPrimitiveType : IType
    {
        IStruct CreateInstance(IStruct parent, Stream stream);
    }

    public class PrimitiveType<T> : IPrimitiveType
        where T : struct
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
            return new PrimitiveStruct<T>(parent, stream);
        }
    }
}
