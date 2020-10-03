using MightyStruct.Abstractions;
using System.Collections.Generic;

namespace MightyStruct.Core
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

        public IType Resolve(Context context)
        {
            return Types[TypeName.Resolve(context)];
        }
    }
}
