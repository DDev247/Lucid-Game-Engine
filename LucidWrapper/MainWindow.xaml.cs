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

using System.IO;
using System.Net;

namespace LucidWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TextBox projectLocation;
        public static TextBox dllSubFolderName;
        public static TextBox customURL;

        public MainWindow()
        {
            InitializeComponent();

            projectLocation = ProjectLocationBox;
            dllSubFolderName = SubFolderNameBox;
            customURL = CustomURLBox;

            if(File.Exists(Environment.CurrentDirectory + @"\cfg.txt"))
            {
                // Read the cfg file
                string readFile = File.ReadAllText(Environment.CurrentDirectory + @"\cfg.txt");

                string[] splitRead = readFile.Split(',');
                if (splitRead[0] == "DIRTY")
                {
                    // The previous cfg was not succesful
                    WarningBlock.Text = "Previous attempt was not succesful.";

                    projectLocation.Text = splitRead[1];
                    dllSubFolderName.Text = splitRead[2];
                    customURL.Text = splitRead[3];
                }
                else
                {
                    WarningBlock.Text = "";

                    projectLocation.Text = splitRead[0];
                    dllSubFolderName.Text = splitRead[1];
                    customURL.Text = splitRead[2];
                }
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if(projectLocation.Text != "Project Location")
            {
                if(dllSubFolderName.Text != "DLL Subfolder Name")
                {
                    this.downloadDLL(false);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("You left the 'DLL Subfolder Name' box empty.\nDo you want the folder name to be 'Lucid'?", "Warning", MessageBoxButton.OKCancel);
                    if(result == MessageBoxResult.OK)
                    {
                        // Continue with the name 'Lucid'
                        this.downloadDLL(true);
                    }
                    else if(result == MessageBoxResult.Cancel)
                    {
                        // Cancel the operation: do nothing.

                    }
                }
            }
            else
            {
                MessageBox.Show("Please insert a project location(where is the .csproj file).", "Error");
            }
        }

        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadBar.Value = e.ProgressPercentage;
        }

        private void SaveCFG(bool dirty)
        {
            if (dirty)
            {
                string fileDir = Environment.CurrentDirectory + @"\cfg.txt";
                string toWrite = "DIRTY," + projectLocation.Text + ',' + dllSubFolderName.Text + ',' + customURL.Text;

                File.WriteAllText(fileDir, toWrite);
            }
            else
            {
                string fileDir = Environment.CurrentDirectory + @"\cfg.txt";
                string toWrite = projectLocation.Text + ',' + dllSubFolderName.Text + ',' + customURL.Text;

                File.WriteAllText(fileDir, toWrite);
            }
        }

        private async Task downloadDLL(bool subfolderEmpty)
        {
            try
            {
                if (subfolderEmpty)
                {
                    if (customURL.Text != "Custom Download URL*")
                    {
                        // Use custom URL
                        string url = customURL.Text;

                        if (Directory.Exists(projectLocation.Text))
                        {
                            string folder = projectLocation.Text + @"\Lucid";
                            if (Directory.Exists(folder))
                            {
                                // Don't create the folder

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                            else
                            {
                                // Create the folder
                                Directory.CreateDirectory(folder);
                                // Continue

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The project directory doesn't exist!", "Error");
                        }
                    }
                    else
                    {
                        // Use standard URL: https://github.com/DDev247/Lucid-Game-Engine/raw/main/Lucid%20Game%20Engine/bin/Debug/net6.0-windows/Lucid%20Game%20Engine.dll

                        string url = "https://github.com/DDev247/Lucid-Game-Engine/raw/main/Lucid%20Game%20Engine/bin/Debug/net6.0-windows/Lucid%20Game%20Engine.dll";

                        if (Directory.Exists(projectLocation.Text))
                        {
                            string folder = projectLocation.Text + @"\Lucid";
                            if (Directory.Exists(folder))
                            {
                                // Don't create the folder

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                            else
                            {
                                // Create the folder
                                Directory.CreateDirectory(folder);
                                // Continue

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The project directory doesn't exist!", "Error");
                        }

                    }
                }
                else
                {
                    if (customURL.Text != "Custom Download URL*")
                    {
                        // Use custom URL
                        string url = customURL.Text;

                        if (Directory.Exists(projectLocation.Text))
                        {
                            string folder = projectLocation.Text + @"\" + dllSubFolderName.Text;
                            if (Directory.Exists(folder))
                            {
                                // Don't create the folder

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                            else
                            {
                                // Create the folder
                                Directory.CreateDirectory(folder);
                                // Continue

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The project directory doesn't exist!", "Error");
                        }
                    }
                    else
                    {
                        // Use standard URL: https://github.com/DDev247/Lucid-Game-Engine/raw/main/Lucid%20Game%20Engine/bin/Debug/net6.0-windows/Lucid%20Game%20Engine.dll

                        string url = "https://github.com/DDev247/Lucid-Game-Engine/raw/main/Lucid%20Game%20Engine/bin/Debug/net6.0-windows/Lucid%20Game%20Engine.dll";

                        if (Directory.Exists(projectLocation.Text))
                        {
                            string folder = projectLocation.Text + @"\" + dllSubFolderName.Text;
                            if (Directory.Exists(folder))
                            {
                                // Don't create the folder

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                            else
                            {
                                // Create the folder
                                Directory.CreateDirectory(folder);
                                // Continue

                                WebClient client = new WebClient();
                                client.DownloadProgressChanged += DownloadProgressChanged;
                                byte[] downloaded = await client.DownloadDataTaskAsync(url);
                                client.Dispose();

                                // Data downloaded, continue with writing to a file

                                await File.WriteAllBytesAsync(folder + @"\Lucid Game Engine.dll", downloaded);

                                MessageBox.Show("Finished Downloading!", "Message");
                                SaveCFG(false);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The project directory doesn't exist!", "Error");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex.Message, "Error");
                SaveCFG(true);
            }
        }
    }
}
