namespace BaltimoreBot.Utilities;

public static class EnvironmentHandler
{
    public static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name
#if DEBUG // Target the machine if in debug mode because Windows
            , EnvironmentVariableTarget.Machine
#else
#endif
        ) ?? throw new Exception($"Environment variable {name} not found");
    }
}