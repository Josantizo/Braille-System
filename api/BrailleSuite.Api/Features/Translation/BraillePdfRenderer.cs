using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BrailleSuite.Api.Features.Translation;

// Renders a braille translation result as a PDF. Requires "DejaVu Sans" to
// already be registered with QuestPDF.Drawing.FontManager (see Program.cs) —
// it's the font actually used to draw the U+2800 braille glyphs.
public static class BraillePdfRenderer
{
    public const string BrailleFontFamily = "DejaVu Sans";

    public static byte[] Render(string title, string brailleText)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontFamily(BrailleFontFamily).FontSize(14).LineHeight(1.6f));

                page.Header().Text(title).FontSize(16).SemiBold();

                page.Content().PaddingTop(10).Column(column =>
                {
                    foreach (var line in brailleText.Split('\n'))
                    {
                        column.Item().Text(line.Length == 0 ? " " : line);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
