using Microsoft.AspNetCore.Http;

namespace BrailleSuite.Api.Features.Documents.Dtos;

public record DocumentSummaryDto(
    Guid Id,
    string Name,
    Guid? FolderId,
    string OriginalFileName,
    string OriginalContentType,
    string TranslationStatus,
    string? TranslationError,
    bool HasTranslatedContent,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record DocumentDetailResponse(
    Guid Id,
    string Name,
    Guid? FolderId,
    string OriginalFileName,
    string OriginalContentType,
    string TranslationStatus,
    string? TranslationError,
    bool HasTranslatedContent,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

// Binds the multipart/form-data body of POST /api/documents.
public record UploadDocumentForm(IFormFile File, string? Name, Guid? FolderId);

public record UpdateDocumentRequest(string Name, Guid? FolderId);

public record FileContentResult(byte[] Content, string ContentType, string FileName);
