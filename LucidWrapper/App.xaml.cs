using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LucidWrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            if (e.Args.Length > 0)
            {
                wnd.Hide();
                wnd.ProjectLocationBox.Text = e.Args[0];
                wnd.SubFolderNameBox.Text = "Lucid";
                wnd.closeAfterDown = true;
                wnd.GoButton_Click(this, null);
            }
            else
            {
                wnd.Show();
            }
        }
    }
}
