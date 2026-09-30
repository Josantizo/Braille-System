using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders.GetFolders;

public record GetFoldersQuery(Guid? ParentFolderId) : IRequest<List<FolderSummaryDto>>;

public class GetFoldersHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetFoldersQuery, List<FolderSummaryDto>>
{
    public async Task<List<FolderSummaryDto>> Handle(GetFoldersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (request.ParentFolderId is { } parentFolderId)
        {
            var parentExists = await db.Folders
                .AnyAsync(f => f.Id == parentFolderId && f.UserId == userId, cancellationToken);

            if (!parentExists)
            {
                throw new NotFoundException($"Folder '{parentFolderId}' was not found.");
            }
        }

        return await db.Folders
            .Where(f => f.UserId == userId && f.ParentFolderId == request.ParentFolderId)
            .OrderBy(f => f.Name)
            .Select(f => new FolderSummaryDto(f.Id, f.Name, f.ParentFolderId, f.CreatedAt, f.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
