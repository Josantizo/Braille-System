using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.GetDocuments;

public record GetDocumentsQuery(Guid? FolderId) : IRequest<List<DocumentSummaryDto>>;

public class GetDocumentsHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetDocumentsQuery, List<DocumentSummaryDto>>
{
    public async Task<List<DocumentSummaryDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (request.FolderId is { } folderId)
        {
            var folderExists = await db.Folders
                .AnyAsync(f => f.Id == folderId && f.UserId == userId, cancellationToken);

            if (!folderExists)
            {
                throw new NotFoundException($"Folder '{folderId}' was not found.");
            }
        }

        // Metadata-only projection — never load OriginalContent/TranslatedContent in a list query.
        return await db.Documents
            .Where(d => d.UserId == userId && d.FolderId == request.FolderId)
            .OrderBy(d => d.Name)
            .Select(d => new DocumentSummaryDto(
                d.Id,
                d.Name,
                d.FolderId,
                d.OriginalFileName,
                d.OriginalContentType,
                d.TranslationStatus.ToString(),
                d.TranslationError,
                d.TranslatedContent != null,
                d.CreatedAt,
                d.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
