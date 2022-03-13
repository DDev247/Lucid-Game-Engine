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
                foreach(ScriptBehaviour s in ScriptBehaviourManager.behaviours)
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
                while(true)
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
            /// TYPE myBehaviourScript = ScriptBehaviourManager.AddBehaviour("myScript");
            /// </c>
            /// </example>
            /// <param name="behaviour">The behaviour to add</param>
            /// <returns>The constructed Behaviour</returns>
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
            public static bool HehaviourExists(string Name)
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

            }

            /// <summary>
            /// Method called at the beginning
            /// </summary>
            public virtual void Start()
            {

            }

            /// <summary>
            /// Method called at every update on the game loop
            /// </summary>
            public virtual void Update()
            {

            }

            /// <summary>
            /// Method called when the 'Closed' event in the main window is called
            /// </summary>
            public virtual void OnExit()
            {

            }

            /// <summary>
            /// Method called when the focus is lost
            /// </summary>
            public virtual void OnLostFocus()
            {

            }

            /// <summary>
            /// Method called when the focus is restored
            /// </summary>
            public virtual void OnRestoredFocus()
            {

            }
        }
    }
}
