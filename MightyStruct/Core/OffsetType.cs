using MightyStruct.Abstractions;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class OffsetType : IType
    {
        public IPotential<IType> BaseType { get; }
        public IPotential<long> Offset { get; }

        public OffsetType(IPotential<IType> baseType, IPotential<long> offset)
        {
            BaseType = baseType;
            Offset = offset;
        }

        public async Task<IStruct> Resolve(Context context)
        {
            var type = await BaseType.Resolve(context);
            var offset = await Offset.Resolve(context);

            return await type.Resolve(new Context(context, offset));
        }
    }
}
