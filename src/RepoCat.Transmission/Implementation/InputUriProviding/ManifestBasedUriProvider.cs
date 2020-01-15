// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedUriProvider.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Transmission
{


    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IInputUriProvider" />
    public class ManifestBasedUriProvider : InputUriProviderBase
    {
        protected override string InputUriSuffix { get; } = Strings.ManifestSuffix;

        public ManifestBasedUriProvider(ILogger logger) : base(logger)
        {
        }
    }
}