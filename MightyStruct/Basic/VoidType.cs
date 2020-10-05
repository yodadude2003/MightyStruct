using MightyStruct.Runtime;

using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class VoidType : IType
    {
        public IPotential<long> Length { get; }

        public VoidType(IPotential<long> length)
        {
            Length = length;
        }

        public async Task<IStruct> Resolve(Context context)
        {
            return new VoidStruct(context, await Length.Resolve(context));
        }
    }
}
