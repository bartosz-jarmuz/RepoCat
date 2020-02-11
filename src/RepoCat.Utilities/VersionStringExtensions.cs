// -----------------------------------------------------------------------
//  <copyright file="VersionStringExtensions.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RepoCat.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class VersionStringExtensions
    {

        /// <summary>
        /// Return the file version of the assembly based on the type and location
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Doesn't matter what happens, just don't throw.")]
        public static string GetAssemblyFileVersion(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try
            {
                return FileVersionInfo.GetVersionInfo(type.Assembly.Location).FileVersion;
            }
            catch
            {
                return "0.0.0.1";
            }
        }

        /// <summary>
        ///     Checks whether a version string is larger than a comparison one.
        ///     Expects a version in format "1.0.0.0", between 2 and 4 segments
        ///     <para>
        ///         The 'substituteForMissingParts' parameter determines whether string 1.0 should be treated as equivalent to
        ///         1.0.0.0
        ///     </para>
        ///     <para>Default Version class parse returns -1 for each missing component (essentially 1.0 is like 1.0.-1.-1)</para>
        ///     <para>So, consequently, 1.0.0 is larger than 1.0, which would be nonsense</para>
        /// </summary>
        /// <param name="currentVersionString"></param>
        /// <param name="comparisonVersionString"></param>
        /// <param name="substituteForMissingParts">Replaces -1 with 0 in the parsed version objects</param>
        /// <returns></returns>
        public static bool IsNewerVersionThan(this string currentVersionString, string comparisonVersionString, bool substituteForMissingParts = true)
        {
            int result = currentVersionString.CompareVersionStrings(comparisonVersionString, substituteForMissingParts);
            return result > 0;
        }

        /// <summary>
        ///     Checks whether a version string is larger than or equal to the comparison one.
        ///     Expects a version in format "1.0.0.0", between 2 and 4 segments
        ///     <para>
        ///         The 'substituteForMissingParts' parameter determines whether string 1.0 should be treated as equivalent to
        ///         1.0.0.0
        ///     </para>
        ///     <para>Default Version class parse returns -1 for each missing component (essentially 1.0 is like 1.0.-1.-1)</para>
        ///     <para>So, consequently, 1.0.0 is larger than 1.0, which would be nonsense</para>
        /// </summary>
        /// <param name="currentVersionString"></param>
        /// <param name="comparisonVersionString"></param>
        /// <param name="substituteForMissingParts">Replaces -1 with 0 in the parsed version objects</param>
        /// <returns></returns>
        public static bool IsNewerOrEqualVersion(this string currentVersionString, string comparisonVersionString, bool substituteForMissingParts = true)
        {
            int result = currentVersionString.CompareVersionStrings(comparisonVersionString, substituteForMissingParts);
            return result >= 0;
        }

        /// <summary>
        /// Converts a shortened version string such as '1.2' or '1.0.23' into fully-fielded string, e.g. '1.2.0.0' or '1.0.23.0'.
        /// <para>Standard .NET version comparison will consider 1.0 to be a LOWER version than 1.0.0.0, because missing fields (build, revision) are set to -1 when parsing.</para>
        /// </summary>
        /// <param name="versionString"></param>
        /// <returns></returns>
        public static string NormalizeVersionString(this string versionString)
        {
            try
            {
                var parsed = Version.Parse(versionString);
                parsed = FixZerosInVersionString(parsed, versionString);
                return parsed.ToString();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error while parsing [{(object)versionString}] as Version.", ex);
            }

        }

        private static Version FixZerosInVersionString(Version version, string versionString)
        {
            if (version.Minor == -1)
            {
                versionString += ".0";
                version = Version.Parse(versionString);
            }

            if (version.Build == -1)
            {
                versionString += ".0";
                version = Version.Parse(versionString);
            }

            if (version.Revision == -1)
            {
                versionString += ".0";
                version = Version.Parse(versionString);
            }

            return version;
        }

        /// <summary>
        ///     Returns 1 if first version is larger, -1 if version is smaller and 0 if they are equal.
        ///     Expects a version in format "1.0.0.0", between 2 and 4 segments
        ///     <para>
        ///         The 'substituteForMissingParts' parameter determines whether string 1.0 should be treated as equivalent to
        ///         1.0.0.0
        ///     </para>
        ///     <para>Default Version class parse returns -1 for each missing component (essentially 1.0 is like 1.0.-1.-1)</para>
        ///     <para>So, consequently, 1.0.0 is larger than 1.0, which would be nonsense</para>
        /// </summary>
        /// <param name="currentVersionString"></param>
        /// <param name="comparisonVersionString"></param>
        /// <param name="substituteForMissingParts">Replaces -1 with 0 in the parsed version objects</param>
        /// <returns></returns>
        public static int CompareVersionStrings(this string currentVersionString, string comparisonVersionString, bool substituteForMissingParts = true)
        {
            Version version1;
            Version version2;
            try
            {
                version1 = Version.Parse(currentVersionString);
                if (substituteForMissingParts)
                {
                    version1 = FixZerosInVersionString(version1, currentVersionString);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error while parsing [{(object)currentVersionString}] as Version.", ex);
            }

            try
            {
                version2 = Version.Parse(comparisonVersionString);
                if (substituteForMissingParts)
                {
                    version2 = FixZerosInVersionString(version2, comparisonVersionString);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error while parsing [{(object)comparisonVersionString}] as Version.", ex);
            }

            return version1.CompareTo(version2);
        }
    }
}
