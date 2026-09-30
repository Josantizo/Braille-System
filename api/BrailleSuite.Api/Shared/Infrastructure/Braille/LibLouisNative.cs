using System.Runtime.InteropServices;

namespace BrailleSuite.Api.Shared.Infrastructure.Braille;

// Raw P/Invoke bindings to the liblouis C API (see liblouis/liblouis.h.in
// upstream). Buffers are passed as raw byte[] instead of widechar[]/char[]
// so this wrapper controls the exact marshaling — liblouis's "widechar" is
// either 2 or 4 bytes depending on how the native binary was built
// (lou_charSize() tells us at runtime), which .NET's built-in char/string
// marshaling can't express directly.
internal static class LibLouisNative
{
    private const string LibraryName = "liblouis";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int lou_charSize();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr lou_version();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int lou_translateString(
        string tableList,
        byte[] inbuf,
        ref int inlen,
        byte[] outbuf,
        ref int outlen,
        IntPtr typeform,
        IntPtr spacing,
        int mode);
}
