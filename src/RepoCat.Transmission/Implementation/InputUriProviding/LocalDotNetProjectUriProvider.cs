﻿namespace RepoCat.Transmission
{
    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IInputUriProvider" />
    public class LocalDotNetProjectUriProvider : InputUriProviderBase
    {
        protected override string InputUriSuffix { get; } = ".csproj";
    }
}