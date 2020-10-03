using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Abstractions
{
    public interface IStruct
    {
        IType Type { get; }

        Context Context { get; }


        Task ParseAsync();
        Task UpdateAsync();
    }
}
