using System.Collections.Generic;

namespace MightyStruct.Core
{
    public class UserType : IType
    {
        public string Name { get; }

        public Dictionary<string, IType> Attributes { get; }

        public UserType(string name)
        {
            Name = name;
        }
    }
}
