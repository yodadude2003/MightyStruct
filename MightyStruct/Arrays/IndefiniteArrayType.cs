using MightyStruct.Runtime;

using System.Threading.Tasks;

namespace MightyStruct.Arrays
{
    public class IndefiniteArrayType : ArrayType
    {
        public IPotential<bool> LoopCondition { get; }

        public IndefiniteArrayType(IPotential<IType> baseType, IPotential<bool> condition) : base(baseType)
        {
            LoopCondition = condition;
        }

        public override async Task<IStruct> Resolve(Context context)
        {
            var type = await BaseType.Resolve(context);
            return new ArrayStruct(context, type, LoopCondition);
        }
    }
}
