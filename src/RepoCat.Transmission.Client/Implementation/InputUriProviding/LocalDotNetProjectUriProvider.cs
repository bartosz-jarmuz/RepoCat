namespace RepoCat.Transmission.Client
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