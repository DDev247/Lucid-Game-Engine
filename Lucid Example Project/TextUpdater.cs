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
using LucidGE.Components;

namespace Lucid_Example_Project
{
    internal class TextUpdater : ScriptBehaviour
    {
        public TextBlock text;
        public Gameobject gameobject;

        public override void OnCreation()
        {
            base.OnCreation();

            Debug.LogWarning("Lucid_Example_Project-TextUpdater", "TextUpdater was created");

            text = new TextBlock();
            text.Name = this.Name;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.Visibility = Visibility.Visible;
            text.FontSize = 32;

            gameobject = new Gameobject();
            gameobject.Name = "My GO";
            gameobject.AddBehaviour(this);
            gameobject.AddElement(text);
            
            Data.UIGrid.Children.Add(text);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            if(text != null)
                text.Text = DateTime.Now.ToString();
            //Debug.Log("Time", DateTime.Now.ToString());

            if (LucidInput.IsKeyDown(Key.Space))
                text.Text = "Pressing Space!";
            
            base.Update();
        }
    }
}
