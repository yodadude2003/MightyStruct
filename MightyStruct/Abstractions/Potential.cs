using System.Threading.Tasks;

namespace MightyStruct.Abstractions
{
    public interface IPotential<T>
    {
        Task<T> Resolve(Context context);
    }
}
