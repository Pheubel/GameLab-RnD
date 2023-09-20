using System.Runtime.CompilerServices;

namespace Noveler.Compiler;

internal static class ListUtil
{
    /// <summary>
    /// Handles adding a span to a list
    /// </summary>
    /// <remarks> TODO: remove once switching to .net8 </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddRange<T>(List<T> list, ReadOnlySpan<T> span)
    {
#if NET8_0_OR_GREATER
        list.AddRange(span);
#else
        list.AddRange(span.ToArray());
#endif
    }
} 