using MightyStruct.Runtime;

namespace MightyStruct.Arrays
{
    public class LoopVariables : Variables
    {
        public override int _index { get; }
        public override dynamic _ { get; }

        public LoopVariables(IStruct self, int index, IStruct @struct) : base(self)
        {
            _index = index;
            _ = @struct;
        }
    }
}
