using Fullerene.Shared.Common.Exceptions;

namespace Fullerene.Shared.Common.Abstractions;

public interface ISelfValidatingConfiguration
{
    /// <summary>
    /// Method for self-validation
    /// </summary>
    /// <exception cref="AppConfigurationException">Thrown if validation fails</exception>
    void ValidateOrThrow();
}