// -----------------------------------------------------------------------
//  <copyright file="StampSorter.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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
            if (stamps == null) throw new ArgumentNullException(nameof(stamps));

            if (!stamps.Any())
            {
                return null;
            }
            try
            {
                return OrderStamps(stamps).FirstOrDefault();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return stamps.FirstOrDefault();
            }

        }


        /// <summary>
        /// Gets the newest stamp.
        /// </summary>
        /// <param name="stamps">The stamps.</param>
        /// <returns>System.String.</returns>
        public static IEnumerable<string> OrderStamps(ICollection<string> stamps)
        {
            if (stamps == null) throw new ArgumentNullException(nameof(stamps));

            if (!stamps.Any())
            {
                return stamps;
            }
           
            if (stamps.Any(s => DateTime.TryParseExact(s, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _)))
            {
                var dict = new Dictionary<DateTime, string>();
                foreach (string stamp in stamps)
                {
                    if (DateTime.TryParseExact(stamp, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime stampDateTime))
                    {
                        dict.Add(stampDateTime, stamp);
                    }
                }

                var ordered = dict.OrderByDescending(x => x.Key).Select(x=>x.Value);
                return ordered.ToList().AsReadOnly();
            }
            else if (stamps.Any(s => Version.TryParse(s, out _)))
            {
                return stamps.OrderByDescending(s => s, new VersionStringComparer()).ToList().AsReadOnly();
            }
       

            return stamps;
        }
    }
}