using BrailleSuite.Api.Features.Documents.Documents.DeleteDocument;
using BrailleSuite.Api.Features.Documents.Documents.GetDocumentDetail;
using BrailleSuite.Api.Features.Documents.Documents.GetDocumentOriginalContent;
using BrailleSuite.Api.Features.Documents.Documents.GetDocuments;
using BrailleSuite.Api.Features.Documents.Documents.GetDocumentTranslatedContent;
using BrailleSuite.Api.Features.Documents.Documents.RetranslateDocument;
using BrailleSuite.Api.Features.Documents.Documents.UpdateDocument;
using BrailleSuite.Api.Features.Documents.Documents.UploadDocument;
using BrailleSuite.Api.Features.Documents.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BrailleSuite.Api.Features.Documents.Documents;

public static class DocumentsEndpoints
{
    public static void MapDocumentsEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/documents").WithTags("Documents").RequireAuthorization();

        group.MapGet("", async (Guid? folderId, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetDocumentsQuery(folderId), cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetDocumentDetailQuery(id), cancellationToken)));

        group.MapGet("/{id:guid}/original", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var file = await sender.Send(new GetDocumentOriginalContentQuery(id), cancellationToken);
            return Results.File(file.Content, file.ContentType, file.FileName);
        });

        group.MapGet("/{id:guid}/translated", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var file = await sender.Send(new GetDocumentTranslatedContentQuery(id), cancellationToken);
            return Results.File(file.Content, file.ContentType, file.FileName);
        });

        group.MapPost("", async ([FromForm] UploadDocumentForm form, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new UploadDocumentCommand(form.Name, form.FolderId, form.File), cancellationToken);
            return Results.Created($"/api/documents/{result.Id}", result);
        }).DisableAntiforgery();

        group.MapPut("/{id:guid}", async (
            Guid id, UpdateDocumentRequest request, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(
                new UpdateDocumentCommand(id, request.Name, request.FolderId), cancellationToken)));

        group.MapPost("/{id:guid}/translate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new RetranslateDocumentCommand(id), cancellationToken)));

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            await sender.Send(new DeleteDocumentCommand(id), cancellationToken);
            return Results.NoContent();
        });
    }
}
