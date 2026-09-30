using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.UpdateDocument;

public record UpdateDocumentCommand(Guid Id, string Name, Guid? FolderId) : IRequest<DocumentDetailResponse>;

public class UpdateDocumentHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<UpdateDocumentCommand, DocumentDetailResponse>
{
    public async Task<DocumentDetailResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.Id}' was not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Document name is required.", nameof(request.Name));
        }

        if (request.FolderId is { } folderId)
        {
            var folderExists = await db.Folders
                .AnyAsync(f => f.Id == folderId && f.UserId == userId, cancellationToken);

            if (!folderExists)
            {
                throw new NotFoundException($"Folder '{folderId}' was not found.");
            }
        }

        document.Name = request.Name;
        document.FolderId = request.FolderId;
        await db.SaveChangesAsync(cancellationToken);

        return DocumentMapper.ToDetailResponse(document);
    }
}
