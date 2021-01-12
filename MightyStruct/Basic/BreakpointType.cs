using MightyStruct.Runtime;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class BreakpointType : IType
    {
        public IPotential<IType> BaseType { get; }

        public BreakpointType(IPotential<IType> baseType)
        {
            BaseType = baseType;
        }

        public async Task<IStruct> Resolve(Context context)
        {
            Debugger.Break();
            var type = await BaseType.Resolve(context);
            return await type.Resolve(context);
        }
    }
}
