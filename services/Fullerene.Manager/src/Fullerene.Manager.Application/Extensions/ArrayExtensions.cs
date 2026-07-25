namespace Fullerene.Manager.Application.Extensions;

public static class ArrayExtensions
{
    public static bool NotNullOrEmpty<T>(this T[]? array)
    {
        return array is not null && array.Length > 0;
    }
}