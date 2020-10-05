using System.IO;
using System.Threading.Tasks;

namespace MightyStruct
{
    public interface ISerializer<T>
    {
        Task<T> ReadFromStreamAsync(Stream stream);
        Task WriteToStreamAsync(Stream stream, T value);
    }
}
