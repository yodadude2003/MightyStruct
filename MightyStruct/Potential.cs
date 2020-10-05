using System.Threading.Tasks;

namespace MightyStruct
{
    using Runtime;

    public interface IPotential<T>
    {
        Task<T> Resolve(Context context);
    }

    public class TrivialPotential<T> : IPotential<T>
    {
        public T Inner { get; }

        public TrivialPotential(T inner)
        {
            Inner = inner;
        }

        public Task<T> Resolve(Context _)
        {
            return Task.FromResult(Inner);
        }
    }
}
