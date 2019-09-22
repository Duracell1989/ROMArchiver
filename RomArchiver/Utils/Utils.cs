using System;

namespace RomLister.Utils
{
    internal class Utils
    {
        private static DateTime lowestDate = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string _gbaCacheFileName = @"GBA - Official OfflineList.cache";
        private const string _ndsCacheFileName = @"ADVANsCEne Nintendo DS Scene.cache";
        private const string _3dsCacheFileName = @"ADVANsCEne Nintendo 3DS Scene.cache";

        private const string _gbaRezFileName = @"GBA - Official OfflineList.rez";
        private const string _ndsRezFileName = @"ADVANsCEne Nintendo DS Scene.rez";
        private const string _3dsRezFileName = @"ADVANsCEne Nintendo 3DS Scene.rez";

        internal static UInt32 CalculateModifyNumber(DateTime lastWriteTimeUtc)
        {
            DateTime workingDateTime = new DateTime(lastWriteTimeUtc.Ticks);
            workingDateTime = workingDateTime.ToLocalTime(); /* For UTC+x and summertime correction */

            workingDateTime = workingDateTime.AddSeconds(1);
            if (workingDateTime.Second % 2 == 1)
            {
                workingDateTime = workingDateTime.AddSeconds(1);
            }
            if (workingDateTime.Year >= 2015)
            {
                workingDateTime = workingDateTime.AddSeconds(1);
            }

            UInt32 years = (UInt32)(workingDateTime.Year - lowestDate.Year);
            UInt32 months = (UInt32)((workingDateTime.Month - lowestDate.Month) + 1);
            UInt32 days = (UInt32)((workingDateTime.Day - lowestDate.Day) + 1);
            UInt32 hours = (UInt32)(workingDateTime.Hour - lowestDate.Hour);
            UInt32 minutes = (UInt32)(workingDateTime.Minute - lowestDate.Minute);
            UInt32 secondes = (UInt32)((workingDateTime.Second - lowestDate.Second) / 2);

            bool isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
            if (isDaylight)/* Finally the summertime/wintertime fix? */
            {
                hours = hours++;
            }

            years <<= 25;
            months <<= 21;
            days <<= 16;
            hours <<= 11;
            minutes <<= 5;

            return (years + months + days + hours + minutes + secondes);
        }

        internal static string GetFileTypeValue(RomType fileType)
        {
            switch (fileType)
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

        internal static string GetCacheFileName(RomType fileType)
        {
            switch (fileType)
            {
                case RomType.GBA:
                    return _gbaCacheFileName;
                case RomType.NDS:
                    return _ndsCacheFileName;
                case RomType.THREEDS:
                    return _3dsCacheFileName;
                default:
                    throw new NotSupportedException();
            }
        }

        internal static string GetRezFileName(RomType fileType)
        {
            switch (fileType)
            {
                case RomType.GBA:
                    return _gbaRezFileName;
                case RomType.NDS:
                    return _ndsRezFileName;
                case RomType.THREEDS:
                    return _3dsRezFileName;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
