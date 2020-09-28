using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public interface ISerializer<T>
    {
        Task<T> ReadFromStreamAsync(Stream stream);
        Task WriteToStreamAsync(Stream stream, T value);
    }
}
