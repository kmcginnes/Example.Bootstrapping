using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class Bootstrapper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [STAThread]
        public static void Main(string[] args)
        {
            FreeConsole();

            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };
            app.InitializeComponent();

            var window = new MainWindow();

            window.Show();
            app.Run();
        }
    }
}
