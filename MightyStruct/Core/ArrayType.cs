using System;
using System.IO;

namespace MightyStruct.Core
{
    public class ArrayType : IType
    {
        public string Name { get; }

        public IType BaseType { get; }
        public Func<int, IStruct, bool> Condition { get; }

        public ArrayType(string name, IType baseType, Func<int, IStruct, bool> condition)
        {
            Name = name;

            BaseType = baseType;
            Condition = condition;
        }


        public IStruct CreateInstance(IStruct parent, Stream stream)
        {
            return new ArrayStruct(this, parent, stream);
        }
    }
}
