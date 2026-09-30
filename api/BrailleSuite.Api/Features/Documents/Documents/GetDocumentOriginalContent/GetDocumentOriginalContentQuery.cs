using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.GetDocumentOriginalContent;

// Lets other modules (e.g. the future translation module) read the original
// uploaded book content for a document.
public record GetDocumentOriginalContentQuery(Guid Id) : IRequest<FileContentResult>;

public class GetDocumentOriginalContentHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetDocumentOriginalContentQuery, FileContentResult>
{
    public async Task<FileContentResult> Handle(
        GetDocumentOriginalContentQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.Id}' was not found.");

        return new FileContentResult(document.OriginalContent, document.OriginalContentType, document.OriginalFileName);
    }
}
