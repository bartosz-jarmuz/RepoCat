// -----------------------------------------------------------------------
//  <copyright file="LocalDotNetProjectUriProvider.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission
{
    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IInputUriProvider" />
    public class LocalDotNetProjectUriProvider : InputUriProviderBase
    {
        protected override string InputUriSuffix { get; } = ".csproj";

        public LocalDotNetProjectUriProvider(ILogger logger) : base(logger)
        {
        }
    }
}