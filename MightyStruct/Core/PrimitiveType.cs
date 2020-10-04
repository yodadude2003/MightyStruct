using MightyStruct.Abstractions;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class PrimitiveType<T> : IType
    {
        public ISerializer<T> Serializer { get; }

        public PrimitiveType(ISerializer<T> serializer)
        {
            Serializer = serializer;
        }

        public Task<IStruct> Resolve(Context context)
        {
            return Task.FromResult<IStruct>(new PrimitiveStruct<T>(context, Serializer));
        }
    }
}
