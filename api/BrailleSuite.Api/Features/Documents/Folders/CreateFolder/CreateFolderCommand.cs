using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders.CreateFolder;

public record CreateFolderCommand(string Name, Guid? ParentFolderId) : IRequest<FolderDetailResponse>;

public class CreateFolderHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<CreateFolderCommand, FolderDetailResponse>
{
    public async Task<FolderDetailResponse> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Folder name is required.", nameof(request.Name));
        }

        if (request.ParentFolderId is { } parentFolderId)
        {
            var parentExists = await db.Folders
                .AnyAsync(f => f.Id == parentFolderId && f.UserId == userId, cancellationToken);

            if (!parentExists)
            {
                throw new NotFoundException($"Parent folder '{parentFolderId}' was not found.");
            }
        }

        var folder = new Folder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            ParentFolderId = request.ParentFolderId
        };

        db.Folders.Add(folder);
        await db.SaveChangesAsync(cancellationToken);

        var breadcrumb = await FolderBreadcrumbBuilder.BuildAsync(db, folder, cancellationToken);

        return new FolderDetailResponse(
            folder.Id, folder.Name, folder.ParentFolderId, breadcrumb, folder.CreatedAt, folder.UpdatedAt);
    }
}
