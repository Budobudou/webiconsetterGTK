using System;
using Gtk;

namespace webiconsetter
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            
            var app = new Application("org.webiconsetter.webiconsetter", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);
            var win = new MainWindow();
            MainWindow.SetDefaultIconFromFile("assets/logo.png");
            app.AddWindow(win);
            win.Show();
            Application.Run();
        }
    }
}
