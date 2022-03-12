// Main namespace for the
// Lucid Game Engine

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
    namespace Interaction
    {
        /// <summary>
        /// Main interaction logic for Lucid Game Engine
        /// </summary>
        public static class MainInteraction
        {
            /// <summary>
            /// <para>
            ///     Initiates the Game Engine.
            /// </para>
            /// <para>
            ///     Use to prep the Engine.
            /// </para>
            /// </summary>
            /// <param name="curWindow">The window to use with the Game Engine</param>
            public static void InitGE(Window curWindow)
            {
                InternalData.GEWindow = curWindow;
                InternalDebugger.Init();
                Debug.Init();

                Data.Data.wpfPath = Environment.CurrentDirectory;
                Data.Data.assetPath = Data.Data.wpfPath + @"\assets";
                Data.Data.settingsPath = Data.Data.wpfPath + @"\assets\settings.adp";
                Data.Data.spriteAssetPath = Data.Data.wpfPath + @"\assets\sprites";
                Data.Data.dataAssetPath = Data.Data.wpfPath + @"\assets\data";

                // Subscribing to window events
                curWindow.Loaded += Window_LoadedWindow;
                curWindow.Closed += Window_Closed;

                CheckFocus();
                InternalData.GESettings = new Settings(Data.Data.settingsPath);
                ApplySettings(InternalData.GESettings);
            }

            private static void Window_Closed(object? sender, EventArgs e)
            {
                InternalDebugger.Log("Engine.WindowManager", 0, "The Main Window was closed.");

                Data.Data.DebugWindow.Close();

                SaveLogs();
            }

            private static void Window_LoadedWindow(object? sender, RoutedEventArgs e)
            {
                InternalDebugger.Log("Engine.WindowManager", 0, "The Main Window was loaded.");
            }

            public static void SaveLogs()
            {
                Debug.Log("Engine.MainInteraction.SaveLogs()", "Saving Logs...");
                // Saving Debug Logs
                string fileDir = Environment.CurrentDirectory + @"\debugLog.txt";
                string toSave = "";

                string Date = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                string Time = DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "/" + DateTime.Now.Ticks;
                toSave += "Log file for the Lucid Game Engine saved at: " + Date + "-" + Time + "\n";

                foreach (Log log in Data.Data.DebugLogs)
                {
                    toSave += log.ToString() + Environment.NewLine;
                }

                File.WriteAllText(fileDir, toSave);

                InternalDebugger.Log("Engine.MainInteraction.SaveLogs()", 1, "Saving Logs...");
                // Saving Internal Logs
                fileDir = Environment.CurrentDirectory + @"\internalLog.txt";
                toSave = "";

                Date = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                Time = DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "/" + DateTime.Now.Ticks;
                toSave += "Log file for the Lucid Game Engine saved at: " + Date + "-" + Time + "\n";

                foreach (InternalLog log in InternalData.DebugLogs)
                {
                    toSave += log.ToString() + Environment.NewLine;
                }

                File.WriteAllText(fileDir, toSave);
            }

            private static void ApplySettings(Settings settings)
            {
                if (settings != null)
                {
                    InternalData.GEWindow.Width = settings.GEWidth;
                    InternalData.GEWindow.Height = settings.GEHeight;
                    InternalData.GEWindow.Title = settings.GEWindowTitle;
                }
            }

            static async Task CheckFocus()
            {
                while (true)
                {
                    Data.Data.focused = IsApplicationActive();
                    focusCheck();
                    await Task.Delay(1);
                }
            }

            static bool lastFocus;
            static void focusCheck()
            {
                if (lastFocus != Data.Data.focused)
                {
                    //Value changed!
                    if (lastFocus == false && Data.Data.focused == true)
                    {
                        lastFocus = Data.Data.focused;
                        Debug.Log("Engine.MainInteraction.focusCheck()", "Focus restored");
                        InternalDebugger.Log("Engine.WindowManager", 0, "Focus restored");
                    }
                    else if (lastFocus == true && Data.Data.focused == false)
                    {
                        lastFocus = Data.Data.focused;
                        Debug.Log("Engine.MainInteraction.focusCheck()", "Focus lost");
                        InternalDebugger.Log("Engine.WindowManager", 0, "Focus lost");
                    }
                }
            }

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();

            private static bool IsActive(Window wnd)
            {
                // workaround for minimization bug
                // Managed .IsActive may return wrong value
                if (wnd == null) return false;
                return GetForegroundWindow() == new WindowInteropHelper(wnd).Handle;
            }

            public static bool IsApplicationActive()
            {
                foreach (var wnd in Application.Current.Windows.OfType<Window>())
                    if (IsActive(wnd)) return true;
                return false;
            }
        }
    }

    namespace Data
    {
        /// <summary>
        /// Contains the data for the Engine (Isn't avaiable to other namespaces).
        /// </summary>
        internal static class InternalData
        {
            public static Window? GEWindow;
            public static Settings? GESettings;

            public static List<InternalLog> DebugLogs = new();
        }

        /// <summary>
        /// Contains the public data for the Engine.
        /// </summary>
        public static class Data
        {
            public static string? wpfPath;
            public static string? assetPath;
            public static string? settingsPath;
            public static string? spriteAssetPath;
            public static string? dataAssetPath;

            // Debug stuff
            public static List<Log> DebugLogs = new();
            public static Debugger DebugWindow;

            public static bool focused = true;
        }
    }

    namespace Debuggers
    {
        /// <summary>
        /// The Internal Debugger used to log stuff.
        /// </summary>
        internal static class InternalDebugger
        {
            public static void Init()
            {
                InternalData.DebugLogs = new List<InternalLog>();
            }

            public static void Log(string source, int severity, string content)
            {
                InternalLog add = new(source, severity, content);
                InternalData.DebugLogs.Add(add);
            }

            
        }

        /// <summary>
        /// The Debugger used to log stuff.
        /// </summary>
        public static class Debug
        {
            public static void Init()
            {
                Data.Data.DebugLogs = new List<Log>();
                Data.Data.DebugWindow = new Debugger();
                Data.Data.DebugWindow.Show();
                Data.Data.DebugWindow.Activate();
            }

            //public static void LogFull(string source, int severity, string content)
            //{
            //    Log add = new(source, severity, content);
            //    Data.Data.DebugLogs.Add(add);
            //    Data.Data.DebugWindow.Log(add);
            //}

            public static void Log(string source, string content)
            {
                Log add = new(source, 0, content);
                Data.Data.DebugLogs.Add(add);
                Data.Data.DebugWindow.Log(add);
            }

            public static void LogMessage(string source, string content)
            {
                Log add = new(source, 0, content);
                Data.Data.DebugLogs.Add(add);
                Data.Data.DebugWindow.Log(add);
            }

            public static void LogWarning(string source, string content)
            {
                Log add = new(source, 1, content);
                Data.Data.DebugLogs.Add(add);
                Data.Data.DebugWindow.Log(add);
            }

            public static void LogError(string source, string content)
            {
                Log add = new(source, 2, content);
                Data.Data.DebugLogs.Add(add);
                Data.Data.DebugWindow.Log(add);
            }
        }
    }

    namespace Classes
    {
        [Serializable]
        public class Log
        {
            public string Source;
            public int Severity = 0; // 0- normal, 1- warning, 2- error
            public string Content = "";

            public Log(string source, int severity, string content)
            {
                Source = source;
                Severity = severity;
                Content = content;
            }

            private string Sever()
            {
                string output;

                if (Severity == 0)
                {
                    output = "INFO";
                }
                else if (Severity == 1)
                {
                    output = "WARN";
                }
                else
                {
                    output = "ERR";
                }

                return output;
            }

            public override string ToString()
            {
                string output;

                output = "[ " + Source + " ] [ " + Sever() + " ] " + Content;

                return output;
            }
        }

        [Serializable]
        internal class InternalLog
        {
            public string Source;
            public int Severity = 0; // 0- normal, 1- warning, 2- error
            public string Content = "";

            public string Date;
            public string Time;

            public InternalLog(string source, int severity, string content)
            {
                Source = source;
                Severity = severity;
                Content = content;
                Date = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                Time = DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "/" + DateTime.Now.Ticks;
            }

            private string Sever()
            {
                string output;

                if (Severity == 0)
                {
                    output = "INFO";
                }
                else if (Severity == 1)
                {
                    output = "WARN";
                }
                else
                {
                    output = "ERR";
                }

                return output;
            }

            public override string ToString()
            {
                string output;

                output = "[ " + Source + " ] @" + Date + "-" + Time + " [ " + Sever() + " ] " + Content;

                return output;
            }
        }

        [Serializable]
        class Settings
        {
            public float GEWidth = 800;
            public float GEHeight = 600;

            public string? GEWindowTitle = "Blank Window";
            public bool GEWatermarks = true;

            public Settings(string path)
            {
                string source = File.ReadAllText(path);
                string[] split = source.Split('\n');

                GEWidth = float.Parse(split[0]);
                GEHeight = float.Parse(split[1]);
                GEWatermarks = bool.Parse(split[2]);
                if(GEWatermarks)
                {
                    GEWindowTitle = "Lucid Engine - " + split[3];
                }
                else
                {
                    GEWindowTitle = split[3];
                }
            }
        }
    }
}
