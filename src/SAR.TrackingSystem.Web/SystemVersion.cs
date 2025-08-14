namespace SAR.TrackingSystem.Web;

public static class SystemVersion
{
    public const string Major = "1";
    public const string Minor = "5";
    public const string Patch = "1";
    public const string PreRelease = ""; // e.g., "alpha", "beta", "rc"
    public const string BuildMetadata = ""; // e.g., "001"
    public static string FullVersion => $"{Major}.{Minor}.{Patch}{(string.IsNullOrEmpty(PreRelease) ? "" : "-" + PreRelease)}{(string.IsNullOrEmpty(BuildMetadata) ? "" : "+" + BuildMetadata)}";
}
