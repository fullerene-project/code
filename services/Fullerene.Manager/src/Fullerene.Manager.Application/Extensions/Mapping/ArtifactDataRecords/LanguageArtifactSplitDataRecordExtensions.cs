using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class LanguageArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this LanguageArtifactSplitDataRecord languageArtifactSplitDataRecord)
    {
        var dto = (languageArtifactSplitDataRecord as ArtifactSplitDataRecord).ToDto();

        dto.LanguageTargeting = languageArtifactSplitDataRecord.LanguageTargeting;

        return dto;
    }
}