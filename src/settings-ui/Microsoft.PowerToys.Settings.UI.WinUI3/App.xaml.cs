﻿using Microsoft.UI.Xaml;
using System.Globalization;
using Microsoft.PowerToys.Settings.UI.Library;

// TODO(stefan)
using WindowsUI = Windows.UI;
using interop;
using System;
using System.Diagnostics;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.WinUI3.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Microsoft.PowerToys.Settings.UI.WinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private enum Arguments
        {
            PTPipeName = 1,
            SettingsPipeName,
            PTPid,
            Theme, // used in the old settings
            ElevatedStatus,
            IsUserAdmin,
            ShowOobeWindow,
            SettingsWindow,
        }

        // Quantity of arguments
        private const int RequiredArgumentsQty = 8;
        private const int RequiredAndOptionalArgumentsQty = 9;

        // Create an instance of the  IPC wrapper.
        private static TwoWayPipeMessageIPCManaged ipcmanager;

        public static bool IsElevated { get; set; }

        public static bool IsUserAnAdmin { get; set; }

        public static int PowerToysPID { get; set; }

        public static Action<string> IPCMessageReceivedCallback { get; set; }
        public bool ShowOobe { get; set; }
        public Type StartupPage { get; set; } = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.GeneralPage);

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
      
        private MainWindow m_window;
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {   
            m_window = new MainWindow();
            m_window.ExtendsContentIntoTitleBar = true;

            m_window.Title = "PowerToys Settings";
            m_window.Activate();

            var cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs != null && cmdArgs.Length >= RequiredArgumentsQty)
            {
                _ = int.TryParse(cmdArgs[(int)Arguments.PTPid], out int powerToysPID);
                PowerToysPID = powerToysPID;
                
                IsElevated = cmdArgs[(int)Arguments.ElevatedStatus] == "true";
                IsUserAnAdmin = cmdArgs[(int)Arguments.IsUserAdmin] == "true";
                ShowOobe = cmdArgs[(int)Arguments.ShowOobeWindow] == "true";

                if (cmdArgs.Length == RequiredAndOptionalArgumentsQty)
                {
                    // open specific window
                    switch (cmdArgs[(int)Arguments.SettingsWindow])
                    {
                        case "Overview": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.GeneralPage); break;
                        case "Awake": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.AwakePage); break;
                        case "ColorPicker": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.ColorPickerPage); break;
                        case "FancyZones": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.FancyZonesPage); break;
                        case "Run": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.PowerLauncherPage); break;
                        case "ImageResizer": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.ImageResizerPage); break;
                        case "KBM": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.KeyboardManagerPage); break;
                        case "MouseUtils": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.MouseUtilsPage); break;
                        case "PowerRename": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.PowerRenamePage); break;
                        case "FileExplorer": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.PowerPreviewPage); break;
                        case "ShortcutGuide": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.ShortcutGuidePage); break;
                        case "VideoConference": StartupPage = typeof(Microsoft.PowerToys.Settings.UI.WinUI3.Views.VideoConferencePage); break;
                        default: break;// TODO(stefan): This crashes Debug.Assert(false, "Unexpected SettingsWindow argument value"); break;
                    }
                    // m_window.Asd().Text = StartupPage.ToString() + " " + cmdArgs[(int)Arguments.SettingsWindow];
                }

                RunnerHelper.WaitForPowerToysRunner(PowerToysPID, () =>
                {
                    Environment.Exit(0);
                });

                ipcmanager = new TwoWayPipeMessageIPCManaged(cmdArgs[(int)Arguments.SettingsPipeName], cmdArgs[(int)Arguments.PTPipeName], (string message) =>
                {
                    if (IPCMessageReceivedCallback != null && message.Length > 0)
                    {
                        m_window.DispatcherQueue.TryEnqueue(() =>
                        {
                            // TODO(stefan): not sure if this is correct
                            IPCMessageReceivedCallback(message);
                        });
                    }
                });
                ipcmanager.Start();
                //app.Run();
            }
            else
            {
/*                MessageBox.Show(
                    "The application cannot be run as a standalone process. Please start the application through the runner.",
                    "Forbidden",
                    MessageBoxButton.OK);
                app.Shutdown();
*/            }
        }
        public static TwoWayPipeMessageIPCManaged GetTwoWayIPCManager()
        {
            return ipcmanager;
        }

        public static bool IsDarkTheme()
        {
            var selectedTheme = SettingsRepository<GeneralSettings>.GetInstance(settingsUtils).SettingsConfig.Theme.ToUpper(CultureInfo.InvariantCulture);
            var defaultTheme = new WindowsUI.ViewManagement.UISettings();
            var uiTheme = defaultTheme.GetColorValue(WindowsUI.ViewManagement.UIColorType.Background).ToString(System.Globalization.CultureInfo.InvariantCulture);
            return selectedTheme == "DARK" || (selectedTheme == "SYSTEM" && uiTheme == "#FF000000");
        }

        private static ISettingsUtils settingsUtils = new SettingsUtils();
    }
}
