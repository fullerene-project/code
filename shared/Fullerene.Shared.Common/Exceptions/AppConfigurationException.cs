using Fullerene.Shared.Domain.Exceptions;

namespace Fullerene.Shared.Common.Exceptions;

public sealed class AppConfigurationException(string validationError) : InternalException(validationError)
{
    public static AppConfigurationException ConfigNotFound(string configName)
    {
        return new AppConfigurationException($"\"{configName}\" configuration not found");
    }
}