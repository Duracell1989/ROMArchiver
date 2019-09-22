using System;

namespace RomLister
{
    internal static class RomTypeExtensions
    {
        internal static string GetExtension(this RomType romTypeEnum)
        {
            switch (romTypeEnum)
            {
                case RomType.GBA:
                    return "gba";
                case RomType.NDS:
                    return "nds";
                case RomType.THREEDS:
                    return "3ds";
                default:
                    throw new NotSupportedException();
            }
        }

        internal static string GetUserFriendlyName(this RomType romTypeEnum)
        {
            switch (romTypeEnum)
            {
                case RomType.GBA:
                    return "GBA";
                case RomType.NDS:
                    return "NDS";
                case RomType.THREEDS:
                    return "3DS";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
