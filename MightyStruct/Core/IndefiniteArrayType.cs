using MightyStruct.Abstractions;

namespace MightyStruct.Core
{
    public class IndefiniteArrayType : IType
    {
        public IPotential<IType> BaseType { get; }
        public IPotential<bool> Condition { get; }

        public IndefiniteArrayType(IPotential<IType> baseType, IPotential<bool> condition)
        {
            BaseType = baseType;
            Condition = condition;
        }

        public IStruct CreateInstance(Context context)
        {
            return new ArrayStruct(this, context);
        }
    }
}
