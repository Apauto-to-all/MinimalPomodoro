using System.Diagnostics;

namespace MinimalPomodoro.Services;

public static class StartupManager
{
    private static readonly string StartupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    private static readonly string ShortcutPath = Path.Combine(StartupFolderPath, "MinimalPomodoro.lnk");
    private static readonly string AppPath = Process.GetCurrentProcess().MainModule?.FileName ?? "";

    public static bool IsEnabled()
    {
        return File.Exists(ShortcutPath);
    }

    public static void SetEnabled(bool enable)
    {
        if (enable)
        {
            if (!File.Exists(ShortcutPath) && !string.IsNullOrEmpty(AppPath))
            {
                CreateShortcut(AppPath, ShortcutPath);
            }
        }
        else
        {
            if (File.Exists(ShortcutPath))
            {
                File.Delete(ShortcutPath);
            }
        }
    }

    private static void CreateShortcut(string targetPath, string shortcutPath)
    {
        try
        {
            // Using PowerShell to create the shortcut to avoid adding COM references (WshShell)
            // which keeps the project file clean and minimal.
            string command = $"$WshShell = New-Object -ComObject WScript.Shell; " +
                             $"$Shortcut = $WshShell.CreateShortcut('{shortcutPath}'); " +
                             $"$Shortcut.TargetPath = '{targetPath}'; " +
                             $"$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}'; " +
                             $"$Shortcut.Save()";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -WindowStyle Hidden -Command \"{command}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process? process = Process.Start(psi))
            {
                process?.WaitForExit();
            }
        }
        catch (Exception)
        {
            // Log or handle error
        }
    }
}
