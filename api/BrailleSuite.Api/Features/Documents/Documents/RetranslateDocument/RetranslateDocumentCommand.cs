using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Features.Translation;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.RetranslateDocument;

// Manually re-runs the liblouis translation for a document the user already
// owns — e.g. after a previous attempt failed, or to pick up a table change.
public record RetranslateDocumentCommand(Guid Id) : IRequest<DocumentDetailResponse>;

public class RetranslateDocumentHandler(AppDbContext db, ICurrentUserContext currentUser, ISender sender)
    : IRequestHandler<RetranslateDocumentCommand, DocumentDetailResponse>
{
    public async Task<DocumentDetailResponse> Handle(RetranslateDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var exists = await db.Documents
            .AnyAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Document '{request.Id}' was not found.");
        }

        await sender.Send(new TranslateDocumentCommand(request.Id), cancellationToken);

        var document = await db.Documents.FirstAsync(d => d.Id == request.Id, cancellationToken);
        return DocumentMapper.ToDetailResponse(document);
    }
}
