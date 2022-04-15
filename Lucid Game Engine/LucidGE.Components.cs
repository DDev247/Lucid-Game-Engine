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

// Adding other namespaces
using System.IO;
using System.Xml;
using System.Windows.Interop;

namespace LucidGE
{
    namespace Components
    {
        /// <summary>
        /// A dialogue box component
        /// </summary>
        [Obsolete("please dont use this shit it will freeze the engine. thank you")]
        public class DialogueBox : ScriptBehaviour
        {
            private TextBlock text;

            private int length;
            private int maxLength;
            private string stringText;

            public override void OnCreation()
            {
                text = new TextBlock();
                text.MinHeight = 55;
                text.Width = InternalData.GESettings.GEWidth - 25;

                Data.Data.MainGrid.Children.Add(text);

                stringText = "Hello World!";
                length = 0;
                maxLength = stringText.Length;

                base.OnCreation();
            }

            public override void Update()
            {
                if(length < maxLength)
                {
                    text.Text = "";
                    for(int i = 0; i < length; i++)
                        text.Text += stringText.ToCharArray()[i];
                    length += 1;
                }

                base.Update();
            }
        }

        /// <summary>
        /// The music manager
        /// </summary>
        public static class MusicPlayer
        {

            private static SoundPlayer player;
            private static string fileName;

            public static void PlayFile(string name, bool looping = false)
            {
                Debug.Log("a0", "a0");
                player = new SoundPlayer();
                FileInfo fi = new FileInfo(Data.Data.soundPath + @"\" + name);
                fileName = name;

                try
                {
                    Debug.Log("a1", "a1");
                    player.Load(fi.FullName, true, looping);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Engine.Components.MusicPlayer", "Exception caught: " + ex.Message);
                    InternalDebugger.Log("Engine.Components.MusicPlayer", 2, "Exception caught: " + ex.Message + " @ " + ex.StackTrace);
                    MessageBoxResult result = MessageBox.Show("Exception thrown: " + ex.Message, "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            public static void Stop()
            {
                player.Stop();
            }

            public static void LoadFile(string name)
            {
                player = new SoundPlayer();

                try
                {
                    player.Load(name);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Engine.Components.MusicPlayer", "Exception caught: " + ex.Message);
                    InternalDebugger.Log("Engine.Components.MusicPlayer", 2, "Exception caught: " + ex.Message + " @ " + ex.StackTrace);
                    MessageBoxResult result = MessageBox.Show("Exception thrown: " + ex.Message, "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

        }

        /// <summary>
        /// A class that can play sounds
        /// </summary>
        public class SoundPlayer
        {
            private System.Windows.Media.MediaPlayer player = new System.Windows.Media.MediaPlayer();
            private bool loop = false;

            public void Load(string path, bool playAfterLoad = false, bool loopingPlay = false)
            {
                Debug.Log("a2", "a2");
                player.Open(new Uri(Data.Data.soundPath + @"\" + path));
                Debug.Log("a3", "a3");

                loop = loopingPlay;
                player.Volume = 1;

                if (playAfterLoad)
                    player.Play(); Debug.Log("a4", "a4");
            }

            private void Player_MediaEnded(object? sender, EventArgs e)
            {
                if (loop)
                    player.Play();
            }

            public void Play(bool looping = false)
            {
                loop = looping;
                player.Play();
            }

            public void Stop()
            {
                player.Stop();
            }
        }

        public static class LucidInput
        {
            public static bool IsKeyDown(Key key)
            {
                return Keyboard.IsKeyDown(key);
            }

            public static bool IsKeyUp(Key key)
            {
                return Keyboard.IsKeyUp(key);
            }

            public static bool IsMouseButtonDown(MouseButton button)
            {
                if (button == MouseButton.Left)
                    return Mouse.LeftButton == MouseButtonState.Pressed;
                else if (button == MouseButton.Right)
                    return Mouse.RightButton == MouseButtonState.Pressed;
                else if (button == MouseButton.Middle)
                    return Mouse.MiddleButton == MouseButtonState.Pressed;
                else if (button == MouseButton.XButton1)
                    return Mouse.XButton1 == MouseButtonState.Pressed;
                else if (button == MouseButton.XButton2)
                    return Mouse.XButton2 == MouseButtonState.Pressed;
                else
                    return false;
            }

            public static bool IsMouseButtonUp(MouseButton button)
            {
                if (button == MouseButton.Left)
                    return Mouse.LeftButton == MouseButtonState.Released;
                else if (button == MouseButton.Right)
                    return Mouse.RightButton == MouseButtonState.Released;
                else if (button == MouseButton.Middle)
                    return Mouse.MiddleButton == MouseButtonState.Released;
                else if (button == MouseButton.XButton1)
                    return Mouse.XButton1 == MouseButtonState.Released;
                else if (button == MouseButton.XButton2)
                    return Mouse.XButton2 == MouseButtonState.Released;
                else
                    return false;
            }
        }
    }
}
