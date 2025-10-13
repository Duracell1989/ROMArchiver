using System;
using RomLister;

namespace RomArchiver
{
    internal static class RomTypeExtensions
    {
        internal static string GetExtension(this RomType romTypeEnum)
        {
            return romTypeEnum switch
            {
                RomType.GBA => "gba",
                RomType.NDS => "nds",
                RomType.THREEDS => "3ds",
                _ => throw new NotSupportedException()
            };
        }

        internal static string GetUserFriendlyName(this RomType romTypeEnum)
        {
            return romTypeEnum switch
            {
                RomType.GBA => "GBA",
                RomType.NDS => "NDS",
                RomType.THREEDS => "3DS",
                _ => throw new NotSupportedException()
            };
        }
    }
}
