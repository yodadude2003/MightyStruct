using System.Collections.Generic;
using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class NamedTypePotential : IPotential<IType>
    {
        public Dictionary<string, IType> Types { get; }
        public IPotential<string> TypeName { get; }

        public NamedTypePotential(Dictionary<string, IType> types, IPotential<string> typeName)
        {
            Types = new Dictionary<string, IType>(types);
            TypeName = typeName;
        }

        public async Task<IType> Resolve(Context context)
        {
            return Types[await TypeName.Resolve(context)];
        }
    }
}
