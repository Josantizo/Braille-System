using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Features.Translation;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.UploadDocument;

public record UploadDocumentCommand(string? Name, Guid? FolderId, IFormFile File) : IRequest<DocumentDetailResponse>;

public class UploadDocumentHandler(AppDbContext db, ICurrentUserContext currentUser, ISender sender)
    : IRequestHandler<UploadDocumentCommand, DocumentDetailResponse>
{
    private static readonly string[] SupportedExtensions = [".txt", ".pdf", ".docx"];

    public async Task<DocumentDetailResponse> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (request.File.Length == 0)
        {
            throw new ValidationException("An uploaded file is required.");
        }

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!SupportedExtensions.Contains(extension))
        {
            throw new ValidationException(
                $"Unsupported file type '{extension}'. Supported types: {string.Join(", ", SupportedExtensions)}.");
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

        await using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);

        var document = new Document
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FolderId = request.FolderId,
            Name = string.IsNullOrWhiteSpace(request.Name) ? request.File.FileName : request.Name,
            OriginalFileName = request.File.FileName,
            OriginalContentType = string.IsNullOrWhiteSpace(request.File.ContentType)
                ? "application/octet-stream"
                : request.File.ContentType,
            OriginalContent = stream.ToArray(),
            TranslationStatus = TranslationStatus.Pending
        };

        db.Documents.Add(document);
        await db.SaveChangesAsync(cancellationToken);

        // Translate right away (synchronous, v1 scope) so the upload response
        // already reflects the outcome instead of leaving it perpetually "Pending".
        await sender.Send(new TranslateDocumentCommand(document.Id), cancellationToken);
        await db.Entry(document).ReloadAsync(cancellationToken);

        return DocumentMapper.ToDetailResponse(document);
    }
}
