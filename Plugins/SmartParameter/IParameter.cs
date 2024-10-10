namespace Core.Utility.SmartParameter
{
    public interface IParameter
    {
        float GetValue();

        bool TrySubtract(float amount);

        bool TryAdd(float amount);
    }
}