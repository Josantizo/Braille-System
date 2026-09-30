using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Folders;

// Shared by GetFolderDetail/CreateFolder/UpdateFolder so the root->self
// breadcrumb list isn't rebuilt three times with slightly different code.
internal static class FolderBreadcrumbBuilder
{
    public static async Task<List<BreadcrumbItemDto>> BuildAsync(
        AppDbContext db, Folder folder, CancellationToken cancellationToken)
    {
        var breadcrumb = new List<BreadcrumbItemDto>();
        Folder? current = folder;

        while (current is not null)
        {
            breadcrumb.Insert(0, new BreadcrumbItemDto(current.Id, current.Name));
            current = current.ParentFolderId is { } parentId
                ? await db.Folders.AsNoTracking().FirstOrDefaultAsync(f => f.Id == parentId, cancellationToken)
                : null;
        }

        return breadcrumb;
    }
}
