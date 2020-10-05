using MightyStruct.Runtime;

using System.Threading.Tasks;

namespace MightyStruct.Arrays
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
