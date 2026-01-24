global using static GitHubLanguageStats.Secrets;

namespace GitHubLanguageStats;

internal static class Secrets
{
    // Change this to the user you want to read the language stats of
    public const string AUTHOR = "Generalisk";

    // Put your GitHub personal access token here.
    // You can generate one here: https://github.com/settings/personal-access-tokens
    // Note: GitHub token must have read access to repository metadata in order to work!
    public const string GITHUB_AUTH_TOKEN = "";

    // Important: DO NOT SHARE NOR COMMIT YOUR PERSONAL ACCESS TOKEN!!!
}
