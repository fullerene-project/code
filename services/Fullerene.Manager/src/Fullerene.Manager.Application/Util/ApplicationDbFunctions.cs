namespace Fullerene.Manager.Application.Util;

public static class ApplicationDbFunctions
{
    /// <summary>
    /// Is two strings fuzzy similar
    /// </summary>
    public static bool FuzzySimilar(string s1, string s2)
    {
        throw new InvalidOperationException($"\"{nameof(ApplicationDbFunctions)}.{nameof(FuzzySimilar)}\" " +
                                            $"method only available in db query expression");
    }

    /// <summary>
    /// Represents fuzzy similarity between two strings.
    /// </summary>
    /// <returns>Double value between 0.0 and 1.0. Greater - less similar</returns>
    public static double FuzzySimilarityDistance(string s1, string s2)
    {
        throw new InvalidOperationException($"\"{nameof(ApplicationDbFunctions)}.{nameof(FuzzySimilarityDistance)}\" " +
                                            $"method only available in db query expression");
    }
}