using System.Collections.Generic;
using System.IO;

namespace MightyStruct.Core
{
    public class UserType : IType
    {
        public string Name { get; }

        public Dictionary<string, IType> Attributes { get; }

        public UserType(string name, Dictionary<string, IType> attrs)
        {
            Name = name;
            Attributes = attrs;
        }

        public IStruct CreateInstance(IStruct parent, Stream stream)
        {
            return new UserStruct(this, parent, stream);
        }
    }
}
