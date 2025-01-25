using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WebLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        private string targetURL;
        public MainWindow()
        {
            InitializeComponent();
            CreateRegistryKey();
            Application.Current.Exit += OnApplicationExit;
            Browser.AddressChanged += BrowserAddressChanged;
        }
        //Form loaded event handler
        void FormLoaded(object sender, RoutedEventArgs args)
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 2)
            {
                Browser.Address = commandLineArgs[1];
                targetURL = commandLineArgs[2];
            }
            else
            {
                MessageBox.Show("No URL provided. Please run the application with 2 URL parameters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }
        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            DeleteRegistryKey();
        }
        private void CreateRegistryKey()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableChangePassword", 1);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableLockWorkstation", 1);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableSwichUser", 1);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoLogoff", 1);
        }
        private void DeleteRegistryKey()
        {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DisableTaskMgr", false);
                        key.DeleteValue("DisableChangePassword", false);
                        key.DeleteValue("DisableLockWorkstation", false);
                        key.DeleteValue("DisableSwichUser", false);
                        key.DeleteValue("DisableLockWorkstation", false);
                    }
                }
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("NoLogoff", false);
                    }
                }
        }
        private void BrowserAddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           if (Browser.Address == targetURL)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
