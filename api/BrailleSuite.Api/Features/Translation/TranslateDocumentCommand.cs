using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using BrailleSuite.Api.Shared.Infrastructure.Braille;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Translation;

// Runs the real liblouis translation for a document and stores the result.
// Called synchronously right after upload (see UploadDocumentCommand) and
// on-demand via POST /api/documents/{id}/translate (RetranslateDocumentCommand).
// Never throws on a translation failure — it records TranslationStatus.Failed
// with a message instead, so upload/retranslate can still return a normal
// response describing what happened.
public record TranslateDocumentCommand(Guid DocumentId) : IRequest;

public class TranslateDocumentHandler(AppDbContext db, ILibLouisTranslator translator) : IRequestHandler<TranslateDocumentCommand>
{
    public async Task Handle(TranslateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.DocumentId}' was not found.");

        try
        {
            var text = DocumentTextExtractor.Extract(document.OriginalContent, document.OriginalFileName);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("No text could be extracted from the file.");
            }

            var brailleText = translator.Translate(text);
            var pdfBytes = BraillePdfRenderer.Render(document.Name, brailleText);

            document.TranslatedContent = pdfBytes;
            document.TranslatedContentType = "application/pdf";
            document.TranslatedFileName = Path.GetFileNameWithoutExtension(document.OriginalFileName) + "-braille.pdf";
            document.TranslationStatus = TranslationStatus.Translated;
            document.TranslationError = null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            document.TranslationStatus = TranslationStatus.Failed;
            document.TranslationError = ex.Message;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
