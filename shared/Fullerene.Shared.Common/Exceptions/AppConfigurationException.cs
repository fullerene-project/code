namespace Fullerene.Shared.Common.Exceptions;

public sealed class AppConfigurationException(string validationError) : Exception(validationError)
{
    public static AppConfigurationException ConfigNotFound(string configName)
    {
        return new AppConfigurationException($"\"{configName}\" configuration not found");
    }
}