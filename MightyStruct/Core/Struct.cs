using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public interface IStruct
    {
        IType Type { get; }

        IStruct Root { get; }
        IStruct Parent { get; }

        Stream Stream { get; }


        Task ParseAsync();
        Task UpdateAsync();
    }
}
