namespace MinimalPomodoro
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string appGuid = "MinimalPomodoro-Unique-Guid-2024";
            // Create the mutex without initially owning it
            using (var mutex = new System.Threading.Mutex(false, appGuid))
            {
                bool isOwned = false;
                try
                {
                    // Wait up to 200ms. This is nearly instant for humans but enough for OS to cleanup after Restart()
                    isOwned = mutex.WaitOne(TimeSpan.FromMilliseconds(200), false);
                }
                catch (System.Threading.AbandonedMutexException)
                {
                    isOwned = true;
                }

                if (!isOwned)
                {
                    // A real second instance is running and hasn't exited within 200ms
                    var config = Services.ConfigManager.Load();
                    Services.Localization.SetLanguage(config.Language);

                    using (var notifyIcon = new NotifyIcon())
                    {
                        notifyIcon.Icon = SystemIcons.Information;
                        notifyIcon.Visible = true;
                        notifyIcon.ShowBalloonTip(2000,
                            Services.Localization.Get("极简番茄钟"),
                            Services.Localization.Get("应用已在运行"),
                            ToolTipIcon.Info);
                        System.Threading.Thread.Sleep(100); // Very short sleep just to ensure message is sent
                    }
                    return;
                }

                try
                {
                    ApplicationConfiguration.Initialize();
                    Application.Run(new UI.TrayAppContext());
                }
                finally
                {
                    // Crucial: always release ownership on exit
                    if (isOwned) mutex.ReleaseMutex();
                }
            }
        }
    }
}