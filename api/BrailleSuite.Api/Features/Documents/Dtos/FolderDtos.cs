namespace BrailleSuite.Api.Features.Documents.Dtos;

public record FolderSummaryDto(
    Guid Id,
    string Name,
    Guid? ParentFolderId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record BreadcrumbItemDto(Guid Id, string Name);

public record FolderDetailResponse(
    Guid Id,
    string Name,
    Guid? ParentFolderId,
    IReadOnlyList<BreadcrumbItemDto> Breadcrumb,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record CreateFolderRequest(string Name, Guid? ParentFolderId);

public record UpdateFolderRequest(string Name, Guid? ParentFolderId);
