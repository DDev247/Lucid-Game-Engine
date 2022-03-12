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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LucidGE.Interaction;
using LucidGE.Data;
using LucidGE.Debuggers;
using LucidGE.Behaviours;

namespace Lucid_Example_Project
{
    internal class TextUpdater : ScriptBehaviour
    {
        public TextBlock text;

        public override void Start()
        {
            text = new TextBlock();
            text.Name = this.Name;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;

            MainWindow.Grid.Children.Add(text);

            base.Start();
        }

        public override void Update()
        {
            text.Text = DateTime.Now.ToString();
            base.Update();
        }
    }
}
