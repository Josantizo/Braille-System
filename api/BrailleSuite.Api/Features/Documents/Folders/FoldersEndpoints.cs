using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Features.Documents.Folders.CreateFolder;
using BrailleSuite.Api.Features.Documents.Folders.DeleteFolder;
using BrailleSuite.Api.Features.Documents.Folders.GetFolderDetail;
using BrailleSuite.Api.Features.Documents.Folders.GetFolders;
using BrailleSuite.Api.Features.Documents.Folders.UpdateFolder;
using MediatR;

namespace BrailleSuite.Api.Features.Documents.Folders;

public static class FoldersEndpoints
{
    public static void MapFoldersEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/folders").WithTags("Folders").RequireAuthorization();

        group.MapGet("", async (Guid? parentId, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetFoldersQuery(parentId), cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetFolderDetailQuery(id), cancellationToken)));

        group.MapPost("", async (CreateFolderRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new CreateFolderCommand(request.Name, request.ParentFolderId), cancellationToken);
            return Results.Created($"/api/folders/{result.Id}", result);
        });

        group.MapPut("/{id:guid}", async (
            Guid id, UpdateFolderRequest request, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(
                new UpdateFolderCommand(id, request.Name, request.ParentFolderId), cancellationToken)));

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            await sender.Send(new DeleteFolderCommand(id), cancellationToken);
            return Results.NoContent();
        });
    }
}
