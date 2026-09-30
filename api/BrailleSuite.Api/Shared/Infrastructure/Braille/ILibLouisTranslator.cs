namespace BrailleSuite.Api.Shared.Infrastructure.Braille;

public interface ILibLouisTranslator
{
    /// <summary>
    /// Translates plain text into Unicode braille (U+2800 block) using the
    /// given liblouis table (default: Spanish grade 1, uncontracted).
    /// </summary>
    string Translate(string text, string tableName = "es-g1.ctb");
}
