using MightyStruct.Abstractions;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public abstract class ArrayType : IType
    {
        public IPotential<IType> BaseType { get; }

        public ArrayType(IPotential<IType> baseType)
        {
            BaseType = baseType;
        }

        public abstract Task<IStruct> Resolve(Context context);
    }
}
