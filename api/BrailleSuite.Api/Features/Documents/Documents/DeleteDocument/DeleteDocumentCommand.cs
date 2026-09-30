using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.DeleteDocument;

public record DeleteDocumentCommand(Guid Id) : IRequest;

public class DeleteDocumentHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<DeleteDocumentCommand>
{
    public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.Id}' was not found.");

        db.Documents.Remove(document);
        await db.SaveChangesAsync(cancellationToken);
    }
}
