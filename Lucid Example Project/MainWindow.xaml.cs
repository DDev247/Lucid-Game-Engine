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
using LucidGE.Classes;
using LucidGE.Components;

using LucidGE.Scenes.Classes;
using LucidGE.Scenes;

namespace Lucid_Example_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                string[] fonts =
                {
                    "Trebuchet MS"
                };

                MainInteraction.InitGE(this, mainGrid, fonts);
                Instance = this;

                Debug.Log("Lucid_Example_Project-MainWindow.Constructor", "Hello, World!");

                TextUpdater? updater = new TextUpdater();
                ScriptBehaviourManager.AddBehaviour(updater);

                // Creating a scene bc why not

                //Scene scene = new Scene();
                //scene.Name = "my level";
                //
                ////Rectangle rect = new Rectangle();
                ////rect.Width = 250;
                ////rect.Height = 100;
                ////rect.HorizontalAlignment = HorizontalAlignment.Center;
                ////rect.VerticalAlignment = VerticalAlignment.Center;
                ////rect.Fill = new SolidColorBrush(Colors.Red);
                //
                //StaticUIElement element = new StaticUIElement();
                //element.type = StaticUIElementType.Rectangle;
                //element.style.RWidth = 250;
                //element.style.RHeight = 100;
                //element.style.HorizontalAlignment = HorizontalAlignment.Center;
                //element.style.VerticalAlignment = VerticalAlignment.Center;
                //element.style.EFill = Brushes.Red;
                //
                //StaticTransform transform = new StaticTransform(new Vector2(0, 0), new GRotation(), new Vector2(1, 1));
                //StaticGameObject gameObject = new StaticGameObject(transform);
                ////gameObject.AddElement(rect);
                //
                //scene.AddObject(gameObject);
                //
                //scene.SaveScene(Data.levelAssetPath + @"\joe.scene");
                //scene.Activate();
                //throw new Exception("AAAAAAAAAAAAAAAAAAAAAA");
            }
            catch(Exception ex)
            {
                MainInteraction.CaughtException(ex);
            }
        }
    }
}
