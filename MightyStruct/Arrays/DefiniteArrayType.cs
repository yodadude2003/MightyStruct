using MightyStruct.Runtime;

using System.Threading.Tasks;

namespace MightyStruct.Arrays
{
    public class DefiniteLoopCondition : IPotential<bool>
    {
        public int Length { get; }

        public DefiniteLoopCondition(int length)
        {
            Length = length;
        }

        public Task<bool> Resolve(Context context)
        {
            var loopState = context.Variables as LoopVariables;
            bool shouldStop = loopState._index >= Length;
            return Task.FromResult(shouldStop);
        }
    }

    public class DefiniteArrayType : ArrayType
    {
        public IPotential<int> Length { get; }

        public DefiniteArrayType(IPotential<IType> baseType, IPotential<int> length) : base(baseType)
        {
            Length = length;
        }

        public override async Task<IStruct> Resolve(Context context)
        {
            IType baseType = await BaseType.Resolve(context);
            int length = await Length.Resolve(context);

            return new ArrayStruct(context, baseType, new DefiniteLoopCondition(length));
        }
    }
}
