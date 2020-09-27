using System.IO;

namespace MightyStruct.Core
{
    public interface ISerializer<T>
    {
        T ReadFromStream(Stream stream);
        void WriteToStream(Stream stream, T value);
    }
}
