using System.IO;

namespace MightyStruct.Abstractions
{
    public interface IType
    {
        IStruct CreateInstance(Context context);
    }
}
