using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Entities;

namespace BrailleSuite.Api.Features.Documents.Documents;

internal static class DocumentMapper
{
    public static DocumentDetailResponse ToDetailResponse(Document document) => new(
        document.Id,
        document.Name,
        document.FolderId,
        document.OriginalFileName,
        document.OriginalContentType,
        document.TranslationStatus.ToString(),
        document.TranslationError,
        document.TranslatedContent is not null,
        document.CreatedAt,
        document.UpdatedAt);
}
