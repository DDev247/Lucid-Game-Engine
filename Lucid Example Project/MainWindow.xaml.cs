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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public static Grid Grid { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MainInteraction.InitGE(this);
            Instance = this;
            Grid = grid;

            Debug.Log("Lucid_Example_Project-MainWindow.Constructor", "Hello, World!");

            TextUpdater? updater = new TextUpdater();
            ScriptBehaviourManager.AddBehaviour(updater);

            /*
            if (updaterBehaviour == null)
                Debug.LogWarning("Lucid_Example_Project-MainWindow.Constructor", "updaterBehaviour is null");
            else if (updaterBehaviour != null)
                Debug.LogMessage("Lucid_Example_Project-MainWindow.Constructor", "updaterBehaviour is not null");
            */

            if (updater == null)
                Debug.LogWarning("Lucid_Example_Project-MainWindow.Constructor", "updater is null");
            else if (updater != null)
                Debug.LogMessage("Lucid_Example_Project-MainWindow.Constructor", "updater is not null");

            //this.AddChild(updater.text);
        }
    }
}
