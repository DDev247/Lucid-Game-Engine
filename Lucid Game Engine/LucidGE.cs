// Main namespace for the
// Lucid Game Engine

/*

    catch (Exception ex)
    {
        Debug.LogError("Engine.", "Exception caught: " + ex.Message);
        InternalDebugger.Log("Engine.", 2, "Exception caught: " + ex.Message + " @ " + ex.StackTrace);
        MessageBoxResult result = MessageBox.Show("Exception thrown: " + ex.Message, "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(1);
    }

*/

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
using Newtonsoft.Json;

/// <summary>
/// Main Namespace of the Lucid Game Engine
/// <para>
///     Example try-catch:
///     <code>
///     catch (Exception ex)
///     {
///         Debug.LogError("NAMESPACE.SUBNAMESPACE.CLASS.METHOD() (method not needed)", "Exception caught: " + ex.Message);
///         InternalDebugger.Log("NAMESPACE.SUBNAMESPACE.CLASS.METHOD() (method not needed)", 2, "Exception caught: " + ex.Message + " @ " + ex.StackTrace);
///         MessageBoxResult result = MessageBox.Show("Exception thrown: " + ex.Message, "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Error);
///         Environment.Exit(1);
///     }
///     </code>
/// </para>
/// </summary>
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
            /// <param name="curGrid">The grid to add objects to</param>
            /// <param name="neededFonts">The needed fonts for the game (the ones you need to have for proper display)</param>
            public static void InitGE(Window curWindow, Grid curGrid, string[]? neededFonts = null)
            {
                try
                {
                    InternalData.GEWindow = curWindow;
                    Data.Data.MainWindow = curWindow;
                    Data.Data.MainGrid = curGrid;

                    InternalDebugger.Init();
                    Debug.Init();
                    Behaviours.ScriptBehaviourHandler.Init();

                    Data.Data.wpfPath = Environment.CurrentDirectory;
                    Data.Data.assetPath = Data.Data.wpfPath + @"\assets";
                    Data.Data.settingsPath = Data.Data.wpfPath + @"\assets\settings.json";
                    Data.Data.spriteAssetPath = Data.Data.wpfPath + @"\assets\sprites";
                    Data.Data.dataAssetPath = Data.Data.wpfPath + @"\assets\data";
                    Data.Data.soundPath = Data.Data.wpfPath + @"\assets\sounds";

                    if (neededFonts != null && neededFonts.Length > 0)
                    {
                        foreach (string font in neededFonts)
                        {
                            bool e = FontExists(font);
                            if (!e)
                            {
                                Debug.LogError("Engine.FontCheck", $"Failed to load font: {font}");
                                MessageBoxResult result = MessageBox.Show($"Error loading font: {font}\nDo you want me to search it in google?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                                if (result == MessageBoxResult.Yes)
                                {
                                    var uri = "https://www.google.com/search?q=Font+" + font;
                                    var psi = new System.Diagnostics.ProcessStartInfo();
                                    psi.UseShellExecute = true;
                                    psi.FileName = uri;
                                    System.Diagnostics.Process.Start(psi);
                                }
                                Environment.Exit(1);
                            }
                        }
                    }

                    // Subscribing to window events
                    curWindow.Loaded += Window_LoadedWindow;
                    curWindow.Closed += Window_Closed;

                    // Creating grids
                    Data.Data.BGGrid = new Grid();
                    Data.Data.WorldGrid = new Grid();
                    Data.Data.EffectGrid = new Grid();
                    Data.Data.UIGrid = new Grid();

                    Data.Data.MainGrid.Children.Add(Data.Data.BGGrid);
                    Data.Data.MainGrid.Children.Add(Data.Data.WorldGrid);
                    Data.Data.MainGrid.Children.Add(Data.Data.EffectGrid);
                    Data.Data.MainGrid.Children.Add(Data.Data.UIGrid);

                    // Adding shitty version text
                    TextBlock verText = new TextBlock();

                    verText.Text = "lucid" + Data.Data.GE_VERSION + " net-" + Data.Data.NET_VERSION;
                    //verText.FontFamily = new FontFamily("Trebuchet MS");
                    verText.FontSize = 10;
                    verText.Visibility = Visibility.Visible;

                    Data.Data.UIGrid.Children.Add(verText);

                    InternalData.GEVersionTextBlock = verText;
                    
                    CheckFocus();
                    InternalData.GESettings = new Settings(Data.Data.settingsPath);
                    ApplySettings(InternalData.GESettings);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Engine.Main", "Exception caught: " + ex.Message);
                    InternalDebugger.Log("Engine.Main", 2, "Exception caught: " + ex.Message + " @ " + ex.StackTrace);
                    MessageBoxResult result = MessageBox.Show("Exception thrown: " + ex.Message, "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            private static void Window_Closed(object? sender, EventArgs e)
            {
                InternalDebugger.Log("Engine.WindowManager", 0, "The Main Window was closed.");

                Data.Data.DebugWindow.Close();
                Behaviours.ScriptBehaviourHandler.CallExit();

                SaveLogs();
            }

            private static void Window_LoadedWindow(object? sender, RoutedEventArgs e)
            {
                InternalDebugger.Log("Engine.WindowManager", 0, "The Main Window was loaded.");
                Debug.Log("Engine.WindowManager", "The Main Window was loaded");
            }

            public static bool FontExists(string fontName)
            {
                // Set default font.
                using (System.Drawing.Font DefaultFont = new System.Drawing.Font(fontName, 9))
                {
                    return DefaultFont.Name == fontName ? true : false;
                }
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

                    if(settings.GEResizable)
                        InternalData.GEWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
                    else
                        InternalData.GEWindow.ResizeMode = ResizeMode.CanMinimize;
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

                        Behaviours.ScriptBehaviourHandler.CallRestoredFocus();
                    }
                    else if (lastFocus == true && Data.Data.focused == false)
                    {
                        lastFocus = Data.Data.focused;
                        Debug.Log("Engine.MainInteraction.focusCheck()", "Focus lost");
                        InternalDebugger.Log("Engine.WindowManager", 0, "Focus lost");

                        Behaviours.ScriptBehaviourHandler.CallLostFocus();
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

        //public class LucidWindow : Window
        //{
        //    public Grid windowGrid;
        //    public LucidWindow asLucid;
        //
        //    public LucidWindow()
        //    {
        //        windowGrid = new Grid();
        //        AddChild(windowGrid);
        //        asLucid = this as LucidWindow;
        //    }
        //}
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
            public static TextBlock? GEVersionTextBlock;

            public static List<InternalLog> DebugLogs = new();
        }

        /// <summary>
        /// Contains the public data for the Engine.
        /// </summary>
        public static class Data
        {
            public const string GE_VERSION = "0.0.1";
            public const string NET_VERSION = "core6.0";

            public static string? wpfPath = "";
            public static string? assetPath = "";
            public static string? soundPath = "";
            public static string? settingsPath = "";
            public static string? spriteAssetPath = "";
            public static string? dataAssetPath = "";

            // Window stuff

            /// <summary>
            /// The window the GE uses
            /// </summary>
            public static Window MainWindow;

            /// <summary>
            /// The grid the GE uses
            /// </summary>
            internal static Grid MainGrid;

            /// <summary>
            /// The UI grid the GE uses
            /// <para>
            /// It sits on top of all grids.
            /// </para>
            /// </summary>
            public static Grid UIGrid;

            /// <summary>
            /// The Background grid the GE uses
            /// <para>
            /// It sits below all grids.
            /// </para>
            /// </summary>
            public static Grid BGGrid;

            /// <summary>
            /// The Effects grid the GE uses
            /// <para>
            /// It sits below the UI grid.
            /// </para>
            /// </summary>
            public static Grid EffectGrid;

            /// <summary>
            /// The Effects grid the GE uses
            /// <para>
            /// It sits below the UI grid and on top of the Background grid.
            /// </para>
            /// </summary>
            public static Grid WorldGrid;

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
                Console.Write("\a");
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

            public bool GEResizable = false;

            public Settings(string path)
            {
                if (File.Exists(path))
                {
                    //string source = File.ReadAllText(path);
                    //string[] split = source.Split('\n');
                    //
                    //GEWidth = float.Parse(split[0]);
                    //GEHeight = float.Parse(split[1]);
                    //GEWatermarks = bool.Parse(split[2]);
                    //if (GEWatermarks)
                    //{
                    //    GEWindowTitle = "Lucid Engine - " + split[3];
                    //}
                    //else
                    //{
                    //    GEWindowTitle = split[3];
                    //}

                    string settingsJson = File.ReadAllText(path);

#nullable disable
                    SettingsRoot json = JsonConvert.DeserializeObject<SettingsRoot>(settingsJson);

                    GEWidth = json.Width;
                    GEHeight = json.Height;
                    GEWatermarks = json.Watermarks;

                    if (GEWatermarks)
                    {
                        GEWindowTitle = "Lucid Engine - " + json.ProjectName;
                    }
                    else
                    {
                        GEWindowTitle = json.ProjectName;
                    }

                    GEResizable = json.CanBeResized;

#nullable enable
                }
                else
                {
                    throw new FileNotFoundException($"The settings file does not exist.\nFile path:{path}");
                }
            }
        }

        public class SettingsRoot
        {
            [JsonProperty("watermarks")]
            public bool Watermarks { get; set; }

            [JsonProperty("project-name")]
            public string ProjectName { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("can-be-resized")]
            public bool CanBeResized { get; set; }
        }
    }
}
