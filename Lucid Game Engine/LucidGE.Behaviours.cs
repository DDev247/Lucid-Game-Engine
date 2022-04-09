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

// Adding other namespaces
using System.IO;
using System.Xml;
using System.Windows.Interop;

namespace LucidGE
{
    namespace Behaviours
    {
        internal static class ScriptBehaviourHandler
        {
            public static void Init()
            {
                CallStart();
                GameLoop();
            }

            public static void CallStart()
            {
                foreach (ScriptBehaviour s in ScriptBehaviourManager.behaviours)
                {
                    s.Start();
                }
            }

            public static void CallUpdate()
            {
                foreach (ScriptBehaviour s in ScriptBehaviourManager.behaviours)
                {
                    s.Update();
                }
            }

            public static void CallExit()
            {
                foreach (ScriptBehaviour s in ScriptBehaviourManager.behaviours)
                {
                    s.OnExit();
                }
            }

            public static void CallLostFocus()
            {
                foreach (ScriptBehaviour s in ScriptBehaviourManager.behaviours)
                {
                    s.OnLostFocus();
                }
            }

            public static void CallRestoredFocus()
            {
                foreach (ScriptBehaviour s in ScriptBehaviourManager.behaviours)
                {
                    s.OnRestoredFocus();
                }
            }

            public static async Task GameLoop()
            {
                while (true)
                {
                    CallUpdate();
                    await Task.Delay(1);
                }
            }
        }

        /// <summary>
        /// This class manages all 'ScriptBehaviour' derived classes
        /// </summary>
        public static class ScriptBehaviourManager
        {
            public static List<ScriptBehaviour> behaviours = new List<ScriptBehaviour>();

            /// <summary>
            /// Creates a ScriptBehaviour.
            /// </summary>
            /// <example>
            /// How to use:
            /// <c>
            /// TYPE myBehaviourScript = new TYPE();
            /// ScriptBehaviourManager.AddBehaviour(myBehaviourScript);
            /// </c>
            /// </example>
            /// <param name="behaviour">The behaviour to add</param>
            public static void AddBehaviour(ScriptBehaviour behaviour)
            {
                //ScriptBehaviour behaviour = new ScriptBehaviour();
                //behaviour.Name = Name;

                behaviours.Add(behaviour);
                behaviour.OnCreation();

                InternalDebugger.Log("Engine.Behaviours.ScriptBehaviourManager.AddBehaviour()", 0, $"The behaviour with the name {behaviour.Name} was added.");
                //return behaviour;
            }

            /// <summary>
            /// Removes <paramref name="behaviour"/> from the list
            /// </summary>
            /// <param name="behaviour">The behaviour to remove</param>
            public static void RemoveBehaviour(ScriptBehaviour behaviour)
            {
                InternalDebugger.Log("Engine.Behaviours.ScriptBehaviourManager.AddBehaviour()", 0, $"The behaviour with the name {behaviour.Name} was removed.");
                behaviours.Remove(behaviour);
            }

            /// <summary>
            /// Checks if a behaviour of the name <paramref name="Name"/> exists
            /// </summary>
            /// <param name="Name">The name of the behaviour</param>
            public static bool BehaviourExists(string Name)
            {
                return behaviours.Exists(x => x.Name == Name);
            }
        }

        /// <summary>
        /// The basic class that in-game scripts derieve from
        /// </summary>
        public class ScriptBehaviour
        {
            public string Name { get; set; }

            //public ScriptBehaviour(string name)
            //{
            //    Name = name;
            //}

            public ScriptBehaviour()
            {
                Name = "Behaviour" + new Random().Next(int.MaxValue).ToString();
            }

            /// <summary>
            /// Method called when the Behaviour is created
            /// </summary>
            public virtual void OnCreation()
            {
                InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"OnCreation() in behaviour '{Name}' was called.");
            }

            /// <summary>
            /// Method called at the beginning
            /// </summary>
            public virtual void Start()
            {
                InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"Start() in behaviour '{Name}' was called.");
            }

            /// <summary>
            /// Method called at every update on the game loop
            /// </summary>
            public virtual void Update()
            {
                // Don't log updates because they will fill the entire log
                // InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"Update() in behaviour '{Name}' was called.");
            }

            /// <summary>
            /// Method called when the 'Closed' event in the main window is called
            /// </summary>
            public virtual void OnExit()
            {
                InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"OnExit() in behaviour '{Name}' was called.");
            }

            /// <summary>
            /// Method called when the focus is lost
            /// </summary>
            public virtual void OnLostFocus()
            {
                InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"OnLostFocus() in behaviour '{Name}' was called.");
            }

            /// <summary>
            /// Method called when the focus is restored
            /// </summary>
            public virtual void OnRestoredFocus()
            {
                InternalDebugger.Log($"ScriptBehaviour-{Name}", 0, $"OnRestoredFocus() in behaviour '{Name}' was called.");
            }
        }

        public class Gameobject
        {
            public GTransform transform;

            public ScriptBehaviour[] scripts;

            public UIElement[] elements;

            /// <summary>
            /// Creates a new Gameobject with everything empty
            /// </summary>
            public Gameobject()
            {
                transform = new GTransform();

                scripts = new ScriptBehaviour[0];

                elements = new UIElement[0];
            }

            /// <summary>
            /// Adds a behaviour to the script list
            /// </summary>
            /// <param name="behaviour">The behaviour to add</param>
            public void AddBehaviour(ScriptBehaviour behaviour)
            {
                scripts.Append(behaviour);
            }

            /// <summary>
            /// Adds an element to the element list
            /// </summary>
            /// <param name="element">The element to add</param>
            public void AddElement(UIElement element)
            {
                elements.Append(element);
            }

            void UpdatePositions()
            {
                foreach(UIElement element in elements)
                {
                    
                }
            }
        }

        public class GTransform
        {
            public Vector2 position;
            public GRotation rotation;

            /// <summary>
            /// The position change in the next frame
            /// </summary>
            public Vector2 velocity;

            public GTransform()
            {
                position = new Vector2();
                rotation = new GRotation();
                velocity = new Vector2();
            }

            public GTransform(Vector2 pos, GRotation rot, Vector2 vel)
            {
                position = pos;
                rotation = rot;
                velocity = vel;
            }
        }

        public class GRotation
        {
            public float Rotation;

            /// <summary>
            /// Creates a GRotation with the rotation set to 0
            /// </summary>
            public GRotation()
            {
                Rotation = 0;
            }

            /// <summary>
            /// Creates a GRotation with the given rotation
            /// </summary>
            /// <param name="rotation">The value to set the rotation</param>
            public GRotation(float rotation)
            {
                Rotation = rotation;
            }
        }

        public class Vector2
        {
            public float X { get; set; }
            public float Y { get; set; }

            /// <summary>
            /// Creates a Vector2 with the X and Y set to 0
            /// </summary>
            public Vector2()
            {
                X = 0;
                Y = 0;
            }

            /// <summary>
            /// Creates a Vector2 with the given X and Y coordinates
            /// </summary>
            /// <param name="x">The value to set the X coordinate to</param>
            /// <param name="y">The value to set the Y coordinate to</param>
            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// Converts a Vector to Vector2
            /// </summary>
            /// <param name="vector">The vector to convert</param>
            public Vector2(Vector vector)
            {
                X = (float)vector.X;
                Y = (float)vector.Y;
            }
        }
    }
}
