using MightyStruct.Abstractions;
using System.Collections.Generic;

namespace MightyStruct.Core
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

        public IStruct CreateInstance(Context context)
        {
            return new UserStruct(this, context);
        }
    }
}
