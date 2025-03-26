namespace Aspire.Common;

public static class TestUtils
{
    public static DirectoryInfo? FindRepoRoot()
    {
        DirectoryInfo? repoRoot = new(AppContext.BaseDirectory);
        while (repoRoot != null)
        {
            // To support git worktrees, check for either a directory or a file named ".git"
            if (Directory.Exists(Path.Combine(repoRoot.FullName, ".git")) || File.Exists(Path.Combine(repoRoot.FullName, ".git")))
            {
                return repoRoot;
            }

            repoRoot = repoRoot.Parent;
        }

        return null;
    }
}