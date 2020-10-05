using MightyStruct.Runtime;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class UserType : IType
    {
        public string Name { get; }

        public Dictionary<string, IPotential<IType>> Attributes { get; }

        public UserType(string name)
        {
            Name = name;

            Attributes = new Dictionary<string, IPotential<IType>>();
        }

        public Task<IStruct> Resolve(Context context)
        {
            return Task.FromResult<IStruct>(new UserStruct(this, context));
        }
    }
}
