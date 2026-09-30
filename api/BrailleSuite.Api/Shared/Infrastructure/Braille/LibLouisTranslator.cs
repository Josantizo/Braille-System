using System.Text;

namespace BrailleSuite.Api.Shared.Infrastructure.Braille;

// Thin, allocation-light wrapper around LibLouisNative that hides the
// widechar-size (2 vs 4 bytes) marshaling detail from the rest of the app.
// Registered as a Singleton — lou_charSize() only needs to run once, and
// liblouis's translation functions are safe to call concurrently (no shared
// mutable state on our side).
public class LibLouisTranslator : ILibLouisTranslator
{
    private readonly int _widecharSize;
    private readonly string _tablesPath;

    public LibLouisTranslator()
    {
        _tablesPath = Path.Combine(AppContext.BaseDirectory, "tables");

        // Belt-and-suspenders: also set the env var liblouis itself looks for.
        // Not relied upon alone — Translate() passes an absolute table path,
        // since some native CRTs cache the environment block at DLL load time
        // and don't see changes SetEnvironmentVariable makes afterwards.
        Environment.SetEnvironmentVariable("LOUIS_TABLEPATH", _tablesPath);

        _widecharSize = LibLouisNative.lou_charSize();
        if (_widecharSize != 2 && _widecharSize != 4)
        {
            throw new InvalidOperationException(
                $"Unsupported liblouis widechar size ({_widecharSize} bytes) — expected 2 or 4.");
        }
    }

    public string Translate(string text, string tableName = "es-g1.ctb")
    {
        // Translate line by line — liblouis expects a single logical line per
        // call; embedded newlines aren't braille dot patterns and would just
        // pass through untranslated if sent as part of the buffer.
        var lines = text.Replace("\r\n", "\n").Split('\n');
        var result = new StringBuilder();

        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                result.Append('\n');
            }

            if (lines[i].Length > 0)
            {
                result.Append(TranslateLine(lines[i], tableName));
            }
        }

        return result.ToString();
    }

    private string TranslateLine(string line, string tableName)
    {
        // Absolute paths rather than bare names: liblouis resolves a table's
        // own "include" directives relative to the directory of the table
        // that was actually opened, so this works even if LOUIS_TABLEPATH
        // didn't make it into the native CRT's environment view.
        // "unicode.dis" MUST come first — without an explicit display table,
        // lou_translateString only applies rules with no dot-pattern output
        // (capitalization/number signs) and silently passes every letter
        // through unchanged instead of emitting real braille cells.
        var tablePath = $"{Path.Combine(_tablesPath, "unicode.dis")},{Path.Combine(_tablesPath, tableName)}";

        var inbuf = EncodeWidechars(line, out var inCharCount);
        var outCapacity = line.Length * 4 + 64;
        var outbuf = new byte[outCapacity * _widecharSize];

        var inlen = inCharCount;
        var outlen = outCapacity;

        var success = LibLouisNative.lou_translateString(
            tablePath, inbuf, ref inlen, outbuf, ref outlen, IntPtr.Zero, IntPtr.Zero, mode: 0);

        if (success == 0)
        {
            throw new InvalidOperationException($"liblouis failed to translate using table '{tableName}'.");
        }

        return DecodeWidechars(outbuf, outlen);
    }

    private byte[] EncodeWidechars(string text, out int charCount)
    {
        if (_widecharSize == 2)
        {
            var chars = text.ToCharArray();
            var bytes = new byte[chars.Length * 2];
            Buffer.BlockCopy(chars, 0, bytes, 0, bytes.Length);
            charCount = chars.Length;
            return bytes;
        }

        // 4-byte widechar: one Unicode scalar value (codepoint) per unit,
        // correctly merging UTF-16 surrogate pairs via Rune.
        var runes = text.EnumerateRunes().ToArray();
        var buffer = new byte[runes.Length * 4];
        for (var i = 0; i < runes.Length; i++)
        {
            BitConverter.TryWriteBytes(buffer.AsSpan(i * 4, 4), runes[i].Value);
        }

        charCount = runes.Length;
        return buffer;
    }

    private string DecodeWidechars(byte[] buffer, int count)
    {
        if (_widecharSize == 2)
        {
            var chars = new char[count];
            Buffer.BlockCopy(buffer, 0, chars, 0, count * 2);
            return new string(chars);
        }

        var result = new StringBuilder(count);
        for (var i = 0; i < count; i++)
        {
            var codepoint = BitConverter.ToInt32(buffer, i * 4);
            result.Append(new Rune(codepoint).ToString());
        }

        return result.ToString();
    }
}
