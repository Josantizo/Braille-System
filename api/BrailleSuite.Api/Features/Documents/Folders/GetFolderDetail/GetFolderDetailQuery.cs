using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders.GetFolderDetail;

public record GetFolderDetailQuery(Guid Id) : IRequest<FolderDetailResponse>;

public class GetFolderDetailHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetFolderDetailQuery, FolderDetailResponse>
{
    public async Task<FolderDetailResponse> Handle(GetFolderDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var folder = await db.Folders
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Folder '{request.Id}' was not found.");

        var breadcrumb = await FolderBreadcrumbBuilder.BuildAsync(db, folder, cancellationToken);

        return new FolderDetailResponse(
            folder.Id, folder.Name, folder.ParentFolderId, breadcrumb, folder.CreatedAt, folder.UpdatedAt);
    }
}
