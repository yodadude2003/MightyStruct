using System;

namespace MightyStruct.Abstractions
{
    public class Variables
    {
        public dynamic _self { get; }

        public virtual int _index => throw new Exception("_index can only be used in a looping context.");
        public virtual dynamic _ => throw new Exception("_ can only be used in a looping context.");

        public Variables(IStruct self)
        {
            _self = self;
        }
    }
}
