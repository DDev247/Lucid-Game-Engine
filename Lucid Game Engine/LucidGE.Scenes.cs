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
            //public List<StaticGameObject> Gameobjects = new List<StaticGameObject>();
            public StaticGameObject[] Gameobjects = new StaticGameObject[0];

            public static Scene GetScene(string filePath)
            {
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter formatter = new BinaryFormatter();

                Scene returning = formatter.Deserialize(stream) as Scene;
                stream.Close();

                Debug.Log("Engine.Scenes.Scene", $"Scene: '{returning.Name}' was loaded from: '{filePath}'");
                return returning;
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
                Gameobjects.Append(staticGameObject);
            }

            /// <summary>
            /// Load scene to grid
            /// </summary>
            public void Activate()
            {
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
                foreach (StaticGameObject gameObject in Gameobjects)
                {
                    gameObject.Activated = false;
                }
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

            }
        }
    }
}
