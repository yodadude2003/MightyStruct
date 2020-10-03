using MightyStruct.Abstractions;

namespace MightyStruct.Core
{
    public class PrimitiveType<T> : IType
    {
        public ISerializer<T> Serializer { get; }

        public PrimitiveType(ISerializer<T> serializer)
        {
            Serializer = serializer;
        }

        public IStruct CreateInstance(Context context)
        {
            return new PrimitiveStruct<T>(this, context);
        }
    }
}
