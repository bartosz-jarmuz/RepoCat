// -----------------------------------------------------------------------
//  <copyright file="VersionStringComparer.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Utilities
{
    /// <summary>
    /// Compares version strings
    /// </summary>
    public class VersionStringComparer : IComparer<string>
    {
        // ReSharper disable once InconsistentNaming
        private readonly bool _substituteForMissingParts;

        /// <summary>
        /// Creates new instance.
        /// <para>The 'substituteForMissingParts' parameter determines whether string 1.0 should be treated as equivalent to 1.0.0.0</para>
        /// <para>Default Version class parse returns -1 for each missing component (essentially 1.0 is like 1.0.-1.-1)</para>
        /// <para>So, consequently, 1.0.0 is larger than 1.0, which is nonsense</para>
        /// </summary>
        /// <param name="substituteForMissingParts"></param>
        public VersionStringComparer(bool substituteForMissingParts = true)
        {
            this._substituteForMissingParts = substituteForMissingParts;
        }

        /// <summary>
        /// Returns 1 if first version is larger, -1 if version is smaller and 0 if they are equal.
        /// </summary>
        /// <param name="firstOne"></param>
        /// <param name="secondOne"></param>
        /// <returns></returns>
        public int Compare(string firstOne, string secondOne)
        {
            return firstOne.CompareVersionStrings(secondOne);
        }
    }
}