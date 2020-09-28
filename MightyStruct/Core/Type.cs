using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public interface IType
    {
        string Name { get; }

        IStruct CreateInstance(IStruct parent, Stream stream);
    }
}
