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

// Adding ourselves to the 'using' list
using LucidGE.Interaction;
using LucidGE.Data;
using LucidGE.Debuggers;
using LucidGE.Classes;
using LucidGE.Behaviours;
using LucidGE.Scenes.Classes;
using LucidGE.Components;

// Adding other namespaces
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LucidGE
{
    namespace Scenes
    {
        [Serializable]
        public class Scene
        {
            public string Name { get; set; }
            public List<StaticGameObject> Gameobjects = new List<StaticGameObject>();
            //public StaticGameObject[] Gameobjects = new StaticGameObject[0];
            public bool Activated { get; private set; }

            public static Scene GetScene(string filePath)
            {
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter formatter = new BinaryFormatter();

                Scene returning = formatter.Deserialize(stream) as Scene;
                stream.Close();

                returning.Update();

                Debug.Log("Engine.Scenes.Scene", $"Scene: '{returning.Name}' was loaded from: '{filePath}'");
                return returning;
            }

            public Scene()
            {
                Update();
            }

            public void SaveScene(string filePath)
            {
                Stream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);

                //XmlSerializer serializer = new XmlSerializer(typeof(Scene));
                //serializer.Serialize(stream, scene);
                stream.Close();

                Debug.Log("Engine.Scenes.Scene", $"Scene: '{Name}' was saved to: '{filePath}'");
            }

            public void AddObject(StaticGameObject staticGameObject)
            {
                Gameobjects.Add(staticGameObject);
            }

            private async Task Update()
            {
                while(true)
                {
                    if(Activated)
                    {
                        foreach (StaticGameObject gameObject in Gameobjects)
                        {
                            gameObject.Activated = true;
                        }
                    }
                    else
                    {
                        foreach (StaticGameObject gameObject in Gameobjects)
                        {
                            gameObject.Activated = false;
                        }
                    }
                    await Task.Delay(1);
                }
            }

            /// <summary>
            /// Load scene to grid
            /// </summary>
            public void Activate()
            {
                Activated = true;

                foreach (StaticGameObject gameObject in Gameobjects)
                {
                    gameObject.Activated = true;
                }
            }

            /// <summary>
            /// Unload scene from grid
            /// </summary>
            public void Deactivate()
            {
                Activated = false;

                foreach (StaticGameObject gameObject in Gameobjects)
                {
                    gameObject.Activated = false;
                }
            }
        }

        public class SceneSerializer
        {
            public static string SerializeScene(Scene scene)
            {
                string returning = "";

                return returning;
            }

            private string serializeGO(StaticGameObject gameObject)
            {
                string result = "";

                
                //gameObject.transform;

                return result;
            }
        }

        namespace Classes
        {
            /// <summary>
            /// A gameobject that cannot move and stays in one place
            /// </summary>
            [Serializable]
            public class StaticGameObject
            {
                public StaticTransform transform { get; set; }
                public StaticUIElement[] elements;
                public bool Activated = false;

                public StaticGameObject(StaticTransform Transform)
                {
                    transform = Transform;
                    elements = new StaticUIElement[0];
                    OnCreated();
                }

                public StaticGameObject()
                {
                    transform = new StaticTransform(new Vector2(), new GRotation(), new Vector2(1, 1));
                    elements = new StaticUIElement[0];
                    OnCreated();
                }

                /// <summary>
                /// Adds an element to the element list
                /// </summary>
                /// <param name="element">The element to add</param>
                public void AddElement(StaticUIElement element)
                {
                    elements.Append(element);
                }

                private void OnCreated()
                {
                    Update();
                }

                private async Task Update()
                {
                    while (true)
                    {
                        UpdatePositions();

                        await Task.Delay(1);
                    }
                }

                private void UpdatePositions()
                {
                    if (Activated)
                    {
                        TransformGroup group = new TransformGroup();
                        group.Children.Add(new TranslateTransform(transform.position.X, transform.position.Y));
                        group.Children.Add(new RotateTransform(transform.rotation.Rotation));
                        group.Children.Add(new ScaleTransform(transform.scale.X, transform.scale.Y));

                        foreach (StaticUIElement element in elements)
                        {
                            UIElement ele = element.ConstructFromType();

                            ele.RenderTransform = group;
                            if (!Data.Data.SceneGrid.Children.Contains(ele))
                            {
                                Data.Data.SceneGrid.Children.Add(ele);
                            }
                        }
                    }
                    else
                    {
                        foreach (StaticUIElement element in elements)
                        {
                            UIElement ele = element.ConstructFromType();

                            if (Data.Data.SceneGrid.Children.Contains(ele))
                            {
                                Data.Data.SceneGrid.Children.Remove(ele);
                            }
                        }
                    }
                }
            }

            [Serializable]
            public class StaticTransform
            {
                public Vector2 position { get; set; }
                public GRotation rotation { get; set; }
                public Vector2 scale { get; set; }

                public StaticTransform(Vector2 position, GRotation rotation, Vector2 scale)
                {
                    this.position = position;
                    this.rotation = rotation;
                    this.scale = scale;
                }

                public StaticTransform()
                {
                    this.position = new Vector2();
                    this.rotation = new GRotation();
                    this.scale = new Vector2(1, 1);
                }
            }

            [Serializable]
            public class StaticUIElement
            {
                public StaticUIElementType type = StaticUIElementType.Rectangle;
                public StaticUIStyle style = new StaticUIStyle();

                public UIElement? ConstructFromType()
                {
                    UIElement returning = null;

                    if (type == StaticUIElementType.Text)
                    {
                        TextBlock e = new TextBlock();
                        e.Width = style.TWidth;
                        e.Height = style.THeight;
                        e.Foreground = style.TForeground;
                        e.Opacity = style.TOpacity;

                        e.VerticalAlignment = style.VerticalAlignment;
                        e.HorizontalAlignment = style.HorizontalAlignment;

                        returning = e;
                    }
                    else if (type == StaticUIElementType.Rectangle)
                    {
                        Rectangle e = new Rectangle();
                        e.Width = style.RWidth;
                        e.Height = style.RHeight;
                        e.Fill = style.RFill;
                        e.Opacity = style.ROpacity;

                        e.VerticalAlignment = style.VerticalAlignment;
                        e.HorizontalAlignment = style.HorizontalAlignment;

                        returning = e;
                    }
                    else if (type == StaticUIElementType.Ellipse)
                    {
                        Ellipse e = new Ellipse();
                        e.Width = style.EWidth;
                        e.Height = style.EHeight;
                        e.Fill = style.EFill;
                        e.Opacity = style.EOpacity;

                        e.VerticalAlignment = style.VerticalAlignment;
                        e.HorizontalAlignment = style.HorizontalAlignment;

                        returning = e;
                    }
                    else
                    {
                        returning = null;
                    }

                    return returning;
                }

                public override string? ToString()
                {
                    string returning = "";

                    if (type == StaticUIElementType.Text)
                    {
                        string e = "TEXTBLOCK_";
                        e += "Width-" + style.TWidth + "+";
                        e += "Height-" + style.THeight + "+";
                        e += "Foreground-" + style.BrushToString(style.TForeground) + "+";
                        e += "Opacity-"+ style.TOpacity + "+";
                        
                        e += "VerticalAlignment-" + VeriAli(style.VerticalAlignment) + "+";
                        e += "HorizontalAlignment-" + HoriAli(style.HorizontalAlignment);

                        returning = e;
                    }
                    else if (type == StaticUIElementType.Rectangle)
                    {
                        string e = "RECTANGLE_";
                        e += "Width-" + style.RWidth + "+";
                        e += "Height-" + style.RHeight + "+";
                        e += "Fill-" + style.BrushToString(style.RFill) + "+";
                        e += "Opacity-" + style.ROpacity + "+";

                        e += "VerticalAlignment-" + VeriAli(style.VerticalAlignment) + "+";
                        e += "HorizontalAlignment-" + HoriAli(style.HorizontalAlignment);

                        returning = e;
                    }
                    else if (type == StaticUIElementType.Ellipse)
                    {
                        string e = "ELLIPSE_";
                        e += "Width-" + style.EWidth + "+";
                        e += "Height-" + style.EHeight + "+";
                        e += "Fill-" + style.BrushToString(style.EFill) + "+";
                        e += "Opacity-" + style.EOpacity + "+";

                        e += "VerticalAlignment-" + VeriAli(style.VerticalAlignment) + "+";
                        e += "HorizontalAlignment-" + HoriAli(style.HorizontalAlignment);

                        returning = e;
                    }
                    else
                    {
                        returning = null;
                    }

                    return returning;
                }

                public static StaticUIElement? FromString(string source)
                {
                    StaticUIElement? returning = new StaticUIElement();

                    string TYPE = source.Split('_')[0];
                    if(TYPE == "TEXTBLOCK")
                    {
                        returning.type = StaticUIElementType.Text;

                        string VARS = source.Split('_')[1];
                        string[] VARS_INDI = VARS.Split('+');

                        string VALUE = VARS_INDI[0].Split('-')[1];
                        returning.style.TWidth = double.Parse(VALUE);

                        VALUE = VARS_INDI[1].Split('-')[1];
                        returning.style.THeight = double.Parse(VALUE);

                        VALUE = VARS_INDI[2].Split('-')[1];
                        returning.style.TForeground = returning.style.StringToBrush(VALUE);

                        VALUE = VARS_INDI[3].Split('-')[1];
                        returning.style.TOpacity = double.Parse(VALUE);
                    }
                    else if(TYPE == "RECTANGLE")
                    {
                        returning.type = StaticUIElementType.Rectangle;

                        string VARS = source.Split('_')[1];
                        string[] VARS_INDI = VARS.Split('+');

                        string VALUE = VARS_INDI[0].Split('-')[1];
                        returning.style.RWidth = double.Parse(VALUE);

                        VALUE = VARS_INDI[1].Split('-')[1];
                        returning.style.RHeight = double.Parse(VALUE);

                        VALUE = VARS_INDI[2].Split('-')[1];
                        returning.style.RFill = returning.style.StringToBrush(VALUE);

                        VALUE = VARS_INDI[3].Split('-')[1];
                        returning.style.ROpacity = double.Parse(VALUE);
                    }
                    else if(TYPE == "ELLIPSE")
                    {
                        returning.type = StaticUIElementType.Ellipse;

                        string VARS = source.Split('_')[1];
                        string[] VARS_INDI = VARS.Split('+');

                        string VALUE = VARS_INDI[0].Split('-')[1];
                        returning.style.EWidth = double.Parse(VALUE);

                        VALUE = VARS_INDI[1].Split('-')[1];
                        returning.style.EHeight = double.Parse(VALUE);

                        VALUE = VARS_INDI[2].Split('-')[1];
                        returning.style.EFill = returning.style.StringToBrush(VALUE);

                        VALUE = VARS_INDI[3].Split('-')[1];
                        returning.style.EOpacity = double.Parse(VALUE);
                    }
                    else
                    {
                        returning = null;
                    }

                    return returning;
                }

                private string HoriAli(HorizontalAlignment alignment)
                {
                    if(alignment == HorizontalAlignment.Center)
                    {
                        return "center";
                    }
                    else if(alignment == HorizontalAlignment.Left)
                    {
                        return "left";
                    }
                    else if(alignment == HorizontalAlignment.Right)
                    {
                        return "right";
                    }
                    else
                    {
                        return "left";
                    }
                }

                private string VeriAli(VerticalAlignment alignment)
                {
                    if (alignment == VerticalAlignment.Center)
                    {
                        return "center";
                    }
                    else if (alignment == VerticalAlignment.Top)
                    {
                        return "top";
                    }
                    else if (alignment == VerticalAlignment.Bottom)
                    {
                        return "bottom";
                    }
                    else
                    {
                        return "top";
                    }
                }
            }

            [Serializable]
            public enum StaticUIElementType
            {
                Text,
                Rectangle,
                Ellipse
            }

            [Serializable]
            public class StaticUIStyle
            {
                // Textblock
                public double TWidth = 100;
                public double THeight = 100;
                public Brush TForeground = Brushes.Black;
                public double TOpacity = 0;

                // Rectangle
                public double RWidth = 100;
                public double RHeight = 100;
                public Brush RFill = Brushes.Black;
                public double ROpacity = 0;

                // Ellipse
                public double EWidth = 100;
                public double EHeight = 100;
                public Brush EFill = Brushes.Black;
                public double EOpacity = 0;

                // Shared
                public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;
                public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
            
                public string BrushToString(Brush brush)
                {
                    string returning;
                    SolidColorBrush b = brush as SolidColorBrush;
                    returning = b.Color.R + "/" + b.Color.G + "/" + b.Color.B;

                    return returning;
                }

                public SolidColorBrush StringToBrush(string source)
                {
                    SolidColorBrush returning = new SolidColorBrush();
                    string[] values = source.Split('/');

                    returning.Color = Color.FromRgb(byte.Parse(values[0]), byte.Parse(values[1]), byte.Parse(values[2]));

                    return returning;
                }
            }
        }
    }
}
