using System.Threading.Tasks;

namespace MightyStruct
{
    public interface IStruct
    {
        Task ParseAsync();
        Task UpdateAsync();
    }
}
