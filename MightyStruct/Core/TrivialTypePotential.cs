using MightyStruct.Abstractions;

namespace MightyStruct.Core
{
    public class TrivialTypePotential : IPotential<IType>
    {
        public IType InnerType { get; }

        public TrivialTypePotential(IType innerType)
        {
            InnerType = innerType;
        }

        public IType Resolve(Context context)
        {
            return InnerType;
        }
    }
}
