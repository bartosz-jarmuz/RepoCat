using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RepoCat.Utilities
{
    /// <summary>
    /// Sorts the repository stamps
    /// </summary>
    public static class StampSorter
    {
        /// <summary>
        /// Gets the newest stamp.
        /// </summary>
        /// <param name="stamps">The stamps.</param>
        /// <returns>System.String.</returns>
        public static string GetNewestStamp(ICollection<string> stamps)
        {
            if (!stamps.Any())
            {
                return null;
            }
            try
            {
                if (stamps.Any(s => DateTime.TryParseExact(s, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _)))
                {
                    var dict = new Dictionary<DateTime, string>();
                    foreach (string stamp in stamps)
                    {
                        if(DateTime.TryParseExact(stamp, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime stampDateTime))
                        {
                            dict.Add(stampDateTime, stamp);
                        }
                    }

                    var ordered = dict.OrderByDescending(x => x.Key).First();
                    return ordered.Value;
                }
            }
            catch (Exception)
            {
                //todo
                return stamps.FirstOrDefault();
            }

            return null;
        }
    }
}