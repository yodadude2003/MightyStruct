using System.Threading.Tasks;

namespace MightyStruct.Abstractions
{
    public interface IStruct
    {
        Task ParseAsync();
        Task UpdateAsync();
    }
}
