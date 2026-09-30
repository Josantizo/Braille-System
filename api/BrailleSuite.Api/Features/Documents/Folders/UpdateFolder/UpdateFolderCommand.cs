using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders.UpdateFolder;

public record UpdateFolderCommand(Guid Id, string Name, Guid? ParentFolderId) : IRequest<FolderDetailResponse>;

public class UpdateFolderHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<UpdateFolderCommand, FolderDetailResponse>
{
    public async Task<FolderDetailResponse> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var folder = await db.Folders
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Folder '{request.Id}' was not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Folder name is required.", nameof(request.Name));
        }

        if (request.ParentFolderId is { } newParentId)
        {
            await EnsureValidNewParentAsync(db, folder, newParentId, userId, cancellationToken);
        }

        folder.Name = request.Name;
        folder.ParentFolderId = request.ParentFolderId;
        await db.SaveChangesAsync(cancellationToken);

        var breadcrumb = await FolderBreadcrumbBuilder.BuildAsync(db, folder, cancellationToken);

        return new FolderDetailResponse(
            folder.Id, folder.Name, folder.ParentFolderId, breadcrumb, folder.CreatedAt, folder.UpdatedAt);
    }

    private static async Task EnsureValidNewParentAsync(
        AppDbContext db, Folder folder, Guid newParentId, long userId, CancellationToken cancellationToken)
    {
        if (newParentId == folder.Id)
        {
            throw new ConflictException("A folder cannot be moved into itself.");
        }

        var newParent = await db.Folders
            .FirstOrDefaultAsync(f => f.Id == newParentId && f.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Parent folder '{newParentId}' was not found.");

        var ancestor = newParent;
        while (ancestor is not null)
        {
            if (ancestor.Id == folder.Id)
            {
                throw new ConflictException("A folder cannot be moved into one of its own descendants.");
            }

            ancestor = ancestor.ParentFolderId is { } ancestorParentId
                ? await db.Folders.FirstOrDefaultAsync(f => f.Id == ancestorParentId, cancellationToken)
                : null;
        }
    }
}
