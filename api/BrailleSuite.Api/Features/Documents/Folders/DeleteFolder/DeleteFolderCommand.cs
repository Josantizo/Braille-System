using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders.DeleteFolder;

public record DeleteFolderCommand(Guid Id) : IRequest;

public class DeleteFolderHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<DeleteFolderCommand>
{
    public async Task Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var folder = await db.Folders
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Folder '{request.Id}' was not found.");

        var hasChildFolders = await db.Folders.AnyAsync(f => f.ParentFolderId == folder.Id, cancellationToken);
        var hasDocuments = await db.Documents.AnyAsync(d => d.FolderId == folder.Id, cancellationToken);

        if (hasChildFolders || hasDocuments)
        {
            throw new ConflictException("The folder is not empty.");
        }

        db.Folders.Remove(folder);
        await db.SaveChangesAsync(cancellationToken);
    }
}
