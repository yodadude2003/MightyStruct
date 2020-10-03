namespace MightyStruct.Abstractions
{
    public interface IPotential<T>
    {
        T Resolve(Context context);
    }
}
