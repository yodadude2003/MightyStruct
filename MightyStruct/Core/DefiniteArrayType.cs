namespace MightyStruct.Core
{
    public class DefiniteArrayType : ArrayType
    {
        public int Length { get; }

        public DefiniteArrayType(string name, IType baseType, int length) : base(name, baseType, (i, s) => i < length)
        {
            Length = length;
        }
    }
}
