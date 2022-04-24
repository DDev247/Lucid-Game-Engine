using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LucidGE
{
    public partial class Debugger : Window
    {
        ListBox lb;

        public Debugger()
        {
            InitializeComponent();
            lb = listBox;
            lb.Items.Add($">> Lucid Game Engine version {Data.Data.GE_VERSION} net-{Data.Data.NET_VERSION}");
            lb.Items.Add($">> https://github.com/DDev247/Lucid-Game-Engine/");
            lb.Items.Add($"");
        }

        /// <summary>
        /// Adds a log to the list.
        /// </summary>
        /// <param name="log">The log to add</param>
        public void Log(Classes.Log log)
        {
            lb.Items.Add(log.ToString());
        }
    }
}
