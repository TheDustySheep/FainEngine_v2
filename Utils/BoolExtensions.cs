using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Utils;
public static class BoolExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static byte AsByte(this bool b) => *(byte*)&b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static sbyte AsSByte(this bool b) => *(sbyte*)&b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static int AsInt(this bool b) => *(byte*)&b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static uint AsUInt(this bool b) => *(byte*)&b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static long AsLong(this bool b) => *(byte*)&b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ulong AsULong(this bool b) => *(byte*)&b;
}
