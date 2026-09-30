using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.GetDocumentTranslatedContent;

public record GetDocumentTranslatedContentQuery(Guid Id) : IRequest<FileContentResult>;

public class GetDocumentTranslatedContentHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetDocumentTranslatedContentQuery, FileContentResult>
{
    public async Task<FileContentResult> Handle(
        GetDocumentTranslatedContentQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.Id}' was not found.");

        if (document.TranslatedContent is null)
        {
            throw new NotFoundException($"Document '{request.Id}' has not been translated yet.");
        }

        return new FileContentResult(
            document.TranslatedContent,
            document.TranslatedContentType ?? "application/octet-stream",
            document.TranslatedFileName ?? document.OriginalFileName);
    }
}
