using System.Text.RegularExpressions;
using Fullerene.Shared.Common.Exceptions;

namespace Fullerene.Shared.Common;

public static class ConfigValidationHelper
{
    public static void Throw(string errorMessage) => throw new AppConfigurationException(errorMessage);

    public static void NotNullOrWhiteSpace(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            Throw($"{parameterName} configuration is missing or empty");
    }

    public static void LengthBetweenIncluded(string value, int min, int max, string parameterName)
    {
        var length = value.Length;
        if (length < min || length > max)
            Throw($"{parameterName} length must be between {min} and {max}");
    }

    public static void ValueBetweenIncluded(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            Throw($"{parameterName} value must be between {min} and {max}");
    }

    public static void MatchRegex(string value, string pattern, string parameterName)
    {
        if (!Regex.IsMatch(value, pattern))
            Throw($"{parameterName} configuration must match pattern {pattern}");
    }
}