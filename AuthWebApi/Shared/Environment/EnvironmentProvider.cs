namespace Shared.Environment;
public static class EnvironmentProvider
{
    public static readonly string CurrentEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

    public static bool IsDevelopment()
    {
        return CurrentEnvironment == "Development";
    }
}