using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;

namespace BrailleSuite.Api.Features.Translation;

// Extracts plain text from the original upload so it can be fed to liblouis.
// Scope v1: .txt, .pdf, .docx. Legacy .doc (binary OLE format) is out of
// scope — it needs a much heavier parser (or a conversion step) and isn't
// worth it until there's real demand for it.
public static class DocumentTextExtractor
{
    public static string Extract(byte[] content, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => Encoding.UTF8.GetString(content),
            ".pdf" => ExtractFromPdf(content),
            ".docx" => ExtractFromDocx(content),
            _ => throw new NotSupportedException($"Text extraction is not supported for '{extension}' files.")
        };
    }

    private static string ExtractFromPdf(byte[] content)
    {
        using var stream = new MemoryStream(content);
        using var pdf = PdfDocument.Open(stream);

        var pages = pdf.GetPages().Select(page => page.Text);
        return string.Join("\n\n", pages);
    }

    private static string ExtractFromDocx(byte[] content)
    {
        using var stream = new MemoryStream(content);
        using var wordDocument = WordprocessingDocument.Open(stream, isEditable: false);

        var body = wordDocument.MainDocumentPart?.Document?.Body
            ?? throw new InvalidOperationException("The .docx file has no document body.");

        var paragraphs = body.Descendants<Paragraph>().Select(p => p.InnerText);
        return string.Join("\n", paragraphs);
    }
}
