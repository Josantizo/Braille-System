using BrailleSuite.Api.Shared.Common;

namespace BrailleSuite.Api.Shared.Entities;

// A book stored in a user's drive. Both the original upload (.txt/.pdf/.docx)
// and the braille translation result (always rendered as PDF) live directly
// in Postgres as bytea columns.
public class Document : ITimestamped
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public Guid? FolderId { get; set; }
    public required string Name { get; set; }
    public required string OriginalFileName { get; set; }
    public required string OriginalContentType { get; set; }
    public required byte[] OriginalContent { get; set; }
    public byte[]? TranslatedContent { get; set; }
    public string? TranslatedContentType { get; set; }
    public string? TranslatedFileName { get; set; }
    public TranslationStatus TranslationStatus { get; set; } = TranslationStatus.Pending;
    public string? TranslationError { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public User? User { get; set; }
    public Folder? Folder { get; set; }
}
