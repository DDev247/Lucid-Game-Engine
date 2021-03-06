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
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Lucid_Hub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Data.Initiate();

            Projects.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;

            Create_New_Project.Visibility = Visibility.Collapsed;
            Delete_Project.Visibility = Visibility.Collapsed;
            Update_Project.Visibility = Visibility.Collapsed;
            Spinner.Visibility = Visibility.Collapsed;
            Spinner_Proj.Visibility = Visibility.Collapsed;
            Spinner_Proj1.Visibility = Visibility.Collapsed;
            Spinner_Proj2.Visibility = Visibility.Collapsed;

            BlurEffect blr = Sidebar.Effect as BlurEffect;
            BlurEffect blr1 = Projects.Effect as BlurEffect;
            BlurEffect blr2 = Settings.Effect as BlurEffect;
            blr.Radius = 0;
            blr1.Radius = 0;
            blr2.Radius = 0;

            Code_Editor_Box.Text = Data.settings.EditorPath;
            Launch_Toggle.IsChecked = Data.settings.LaunchDir;
            Project_Dir_Box.Text = Data.settings.SaveProjectDir;
            Project_Dir_Box1.Text = Data.settings.SaveProjectDir;

            UpdateThings();
        }

        private async Task UpdateThings()
        {
            while(true)
            {
                // proj menu  
                ProjectMenu.Items.Clear();

                foreach (string proj in Data.projects.ProjectList)
                {
                    MenuItem item = new MenuItem();
                    item.Header = proj;
                    item.Cursor = Cursors.Hand;
                    item.VerticalContentAlignment = VerticalAlignment.Center;
                    item.HorizontalContentAlignment = HorizontalAlignment.Left;
                    item.Click += MenuItem_Click;

                    ProjectMenu.Items.Add(item);
                }

                // settings toggle and shit
                Launch_Toggle.Content = Launch_Toggle.IsChecked.ToString();

                // proj create shit
                Project_Dest_Label.Content = Data.settings.SaveProjectDir + @"\" + Project_Name_Box.Text;

                // proj delete shit
                Project_Dest_Label1.Content = Project_Dir_Box1.Text + @"\" + Project_Name_Box1.Text;

                // proj update shit
                Project_Dest_Label2.Content = Project_Dir_Box2.Text + @"\" + Project_Name_Box2.Text;

                await Task.Delay(1);
            }
        }

        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            Projects.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Projects.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Visible;
        }

        private void LaunchProject(string directory)
        {
            Spinner.Visibility = Visibility.Visible;
            if (Data.settings.EditorPath == "none")
            {
                Spinner.Visibility = Visibility.Collapsed;
                MessageBox.Show("Error: Code editor executable not set!", "Lucid Hub Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                StartEditor(directory);
            }
        }

        private void StartEditor(string dir)
        {
            using (Process process = new Process())
            {
                if(Data.settings.LaunchDir == true)
                {
                    // Launch like vscode: code {dir}
                    process.StartInfo.FileName = Data.settings.EditorPath;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Arguments = '"' + dir + '"';
                }
                else
                {
                    // Launch .csproj file: cmd {dir} (extract last dir thing).csproj
                    string[] dirs = dir.Split('/', '\\');
                    string fname = dirs[dirs.Length - 1] + ".csproj";
                    process.StartInfo.FileName = Data.settings.EditorPath;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Arguments = '"' + dir + @"\" + fname + '"';
                }

                process.Start();
            }
            Spinner.Visibility = Visibility.Collapsed;
        }

        private async Task CreateProject(string name, string directory, string genDir)
        {
            Data.projects.ProjectList.Add(directory);
            await ProjectGenerator.GenerateProject(name, genDir);
            NewProject_Created();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string dir = item.Header.ToString();
            LaunchProject(dir);
        }

        #region DELETE PROJECT SCREEN

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Blur();
            Delete_Project.Visibility = Visibility.Visible;
        }

        private void Delete_Cancel_Click(object sender, RoutedEventArgs e)
        {
            UnBlur();
            Delete_Project.Visibility = Visibility.Collapsed;
        }

        private void Proj_Delete_Click(object sender, RoutedEventArgs e)
        {
            string dir = Project_Dir_Box1.Text + @"\" + Project_Name_Box1.Text;
            if (Data.projects.ProjectList.Contains(dir))
            {
                DeleteProject(dir);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show($"The project with the path:\n{dir}\nDoesn't exist. If it isn't on the project list press Yes otherwise press No", "Lucid Hub Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    DeleteProject(dir);
                }
            }
        }

        private async Task DeleteProject(string directory)
        {
            if (Directory.Exists(directory))
            {
                Spinner_Proj1.Visibility = Visibility.Visible;
                Directory.Delete(directory, true);

                await Task.Delay(250);
                while (Directory.Exists(directory))
                    await Task.Delay(100);

                Spinner_Proj1.Visibility = Visibility.Collapsed;
                Data.projects.ProjectList.Remove(directory);
                Delete_Project.Visibility=Visibility.Collapsed;
                UnBlur();
            }
            else
            {
                MessageBox.Show($"The project with the directory:\n{directory}\nDoesn't exist.", "Lucid Hub Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void MenuItem_DeleteClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            string dir = item.Header.ToString();
            DeleteProject(dir);
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            Blur();
            Create_New_Project.Visibility = Visibility.Visible;
        }

        private async Task Blur()
        {
            for(int i = 0; i < 10; i++)
            {
                BlurEffect blr = Sidebar.Effect as BlurEffect;
                BlurEffect blr1 = Projects.Effect as BlurEffect;
                BlurEffect blr2 = Settings.Effect as BlurEffect;
                blr.Radius = i;
                blr1.Radius = i;
                blr2.Radius = i;

                await Task.Delay(5);
            }
        }

        private async Task UnBlur()
        {
            for (int i = 10; i > 0; i--)
            {
                BlurEffect blr = Sidebar.Effect as BlurEffect;
                BlurEffect blr1 = Projects.Effect as BlurEffect;
                BlurEffect blr2 = Settings.Effect as BlurEffect;
                blr.Radius = i;
                blr1.Radius = i;
                blr2.Radius = i;

                await Task.Delay(5);
            }
        }

        private void NewProject_Created()
        {
            Spinner_Proj.Visibility = Visibility.Collapsed;
            Create_New_Project.Visibility = Visibility.Collapsed;
            UnBlur();
        }

        private void Proj_Create_Click(object sender, RoutedEventArgs e)
        {
            if(!Directory.Exists(Data.settings.SaveProjectDir + Project_Name_Box.Text.Replace(' ', '_')))
            {
                Spinner_Proj.Visibility = Visibility.Visible;
                CreateProject(Project_Name_Box.Text, Data.settings.SaveProjectDir + @"\" + Project_Name_Box.Text.Replace(' ', '_'), Data.settings.SaveProjectDir);
            }
            else
            {
                // project exists
                MessageBox.Show("This project already exists.", "Lucid Hub Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Proj_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Create_New_Project.Visibility = Visibility.Collapsed;
            UnBlur();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.LaunchDir = (bool) Launch_Toggle.IsChecked;
            settings.SaveProjectDir = Data.settings.SaveProjectDir;
            settings.EditorPath = Code_Editor_Box.Text;

            string newJSON = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(Data.settingsDir, newJSON);

            Data.Initiate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save project list
            string newJSON = JsonConvert.SerializeObject(Data.projects, Formatting.Indented);
            File.WriteAllText(Data.projectDir, newJSON);

            // save settings
            Settings settings = new Settings();
            settings.LaunchDir = (bool)Launch_Toggle.IsChecked;
            settings.SaveProjectDir = Data.settings.SaveProjectDir;
            settings.EditorPath = Code_Editor_Box.Text;

            newJSON = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(Data.settingsDir, newJSON);

            // delete lucid dll
            File.Delete(Environment.CurrentDirectory + @"\Lucid\Lucid Game Engine.dll");
            Directory.Delete(Environment.CurrentDirectory + @"\Lucid");
        }

        private void Project_Dir_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Data.settings != null)
                Data.settings.SaveProjectDir = Project_Dir_Box.Text;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(Environment.CurrentDirectory + @"..\last.json");

            Process.Start(Environment.CurrentDirectory + @"..\LucidHubUpdater.exe");

            Environment.Exit(0);
        }

        private void Appdata_Click(object sender, RoutedEventArgs e)
        {
            using(Process process = new Process())
            {
                process.StartInfo.FileName = "explorer.exe";
                process.StartInfo.Arguments = Environment.GetEnvironmentVariable("appdata") + @"\Lucid_Hub\";
                Debug.WriteLine(process.StartInfo.Arguments);

                process.Start();
            }
        }

        private void UpdateProject_Click(object sender, RoutedEventArgs e)
        {
            Blur();
            Update_Project.Visibility = Visibility.Visible;
            Spinner_Proj.Visibility = Visibility.Visible;
        }

        private void Updt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            UnBlur();
            Update_Project.Visibility = Visibility.Collapsed;
            Spinner_Proj.Visibility = Visibility.Collapsed;
        }

        private async void Updt_Click(object sender, RoutedEventArgs e)
        {
            Spinner_Proj2.Visibility = Visibility.Visible;
            string path = Data.GetProjectDir(Project_Dest_Label2.Content.ToString());

            // Run Wrapper
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "Lucid Wrapper.exe";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = '"' + path + '"';

                process.Start();

                await process.WaitForExitAsync();
            }

            Spinner_Proj2.Visibility = Visibility.Collapsed;

            Spinner_Proj.Visibility = Visibility.Collapsed;
            Update_Project.Visibility = Visibility.Collapsed;
            UnBlur();
        }

        private async void Updt_All_Click(object sender, RoutedEventArgs e)
        {
            Spinner_Proj2.Visibility = Visibility.Visible;

            // Run Wrapper
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "Lucid Wrapper.exe";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = '"' + Environment.CurrentDirectory + '"';

                process.Start();

                await process.WaitForExitAsync();
            }

            foreach(string item in Data.projects.ProjectList)
            {
                // Copy
                string path = item + @"\Lucid\Lucid Game Engine.dll";
                File.Copy(Environment.CurrentDirectory + @"\Lucid\Lucid Game Engine.dll", path, true);
            }

            Spinner_Proj2.Visibility = Visibility.Collapsed;

            Spinner_Proj.Visibility = Visibility.Collapsed;
            Update_Project.Visibility = Visibility.Collapsed;
            UnBlur();
        }
    }

    public static class Data
    {
        public const string settingsJsonAddress = "https://raw.githubusercontent.com/DDev247/Lucid-Game-Engine/main/settings.json";

        public static string savedProjectDir = "";

        public static string settingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Lucid_Hub\settings.json";
        public static string projectDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Lucid_Hub\projects.json";

        public static Settings settings;

        public static Projects projects;

        public static string GetProjectDir(string name)
        {
            string result = "";
            foreach(string item in projects.ProjectList)
            {
                if(item.Contains(name))
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        public static async Task Initiate()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Lucid_Hub\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Lucid_Hub\");

            if (File.Exists(settingsDir))
            {
                string read = File.ReadAllText(settingsDir);
                settings = JsonConvert.DeserializeObject<Settings>(read);

                savedProjectDir = settings.SaveProjectDir;
            }
            else
            {
                settings = new Settings();
                settings.LaunchDir = false;
                settings.SaveProjectDir = @"C:\lgeProjects";
                settings.EditorPath = "none";

                string newJSON = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(settingsDir, newJSON);
            }

            if (File.Exists(projectDir))
            {
                string read = File.ReadAllText(projectDir);
                projects = JsonConvert.DeserializeObject<Projects>(read);
            }
            else
            {
                projects = new Projects();
                projects.ProjectList = new List<string>();
                string newJSON = JsonConvert.SerializeObject(projects, Formatting.Indented);
                File.WriteAllText(projectDir, newJSON);
            }
        }
    }

    public class Settings
    {
        [JsonProperty("launchDir")]
        public bool LaunchDir { get; set; }

        [JsonProperty("saveProjDir")]
        public string SaveProjectDir { get; set; }

        [JsonProperty("editorPath")]
        public string EditorPath { get; set; }
    }

    public class Projects
    {
        [JsonProperty("projects")]
        public List<string> ProjectList { get; set; }
    }

    public static class ProjectGenerator
    {
        //public static async Task GenerateProject(string projectName, string projectDir)
        //{
        //    string templatePath = Directory.GetCurrentDirectory() + @"\templates\";
        //
        //    // Getting the templates
        //    string projP1 = await File.ReadAllTextAsync(templatePath + "csproj-part1.txt");
        //    string projP2 = await File.ReadAllTextAsync(templatePath + "csproj-part2.txt");
        //
        //    string xamlP1 = await File.ReadAllTextAsync(templatePath + "xaml-part1.txt");
        //    string xamlP2 = await File.ReadAllTextAsync(templatePath + "xaml-part2.txt");
        //
        //    string csP1 = await File.ReadAllTextAsync(templatePath + "cs-part1.txt");
        //    string csP2 = await File.ReadAllTextAsync(templatePath + "cs-part2.txt");
        //
        //    // Applying the templates
        //    string projFileContents = projP1 + projectName + projP2;
        //    string projFilePath = projectDir + @"\" + projectName + @"\" + projectName + ".csproj";
        //
        //    string xamlFileContents = xamlP1 + projectName + xamlP2;
        //    string xamlFilePath = projectDir + @"\" + projectName + @"\MainWindow.xaml";
        //
        //    string csFileContents = csP1 + projectName + csP2;
        //    string csFilePath = projectDir + @"\" + projectName + @"\MainWindow.xaml.cs";
        //
        //    string assInfoContents = await File.ReadAllTextAsync(templatePath + "AssemblyInfo.cs");
        //    string assInfoFilePath = projectDir + @"\" + projectName + @"\AssemblyInfo.cs";
        //
        //    // Checking if the directories exist
        //    // if not then create them
        //    if (!Directory.Exists(projectDir))
        //        Directory.CreateDirectory(projectDir);
        //
        //    if(!Directory.Exists(projectDir + @"\" + projectName))
        //        Directory.CreateDirectory(projectDir + @"\" + projectName);
        //
        //    // Write the templates to files
        //    await File.WriteAllTextAsync(projFilePath, projFileContents);
        //    await File.WriteAllTextAsync(xamlFilePath, xamlFileContents);
        //    await File.WriteAllTextAsync(projFilePath, projFileContents);
        //
        //}

        public static async Task GenerateProject(string projectName, string dir)
        {
            try
            {
                // Get json:
                string jsonDir = Directory.GetCurrentDirectory() + @"\templates\templates.json";
                string jsonContents = File.ReadAllText(jsonDir);

                TemplateRoot template = JsonConvert.DeserializeObject<TemplateRoot>(jsonContents);

                string pName = projectName.Replace(' ', '_');

                string CSFile = "";

                // Add \n to all lines and add project name to some lines
                foreach (string line in template.MainCS)
                {
                    if (line == "namespace ")
                        CSFile += line + pName + Environment.NewLine;
                    else
                        CSFile += line + Environment.NewLine;
                }

                string XAMLFile = "";

                // Add \n to all lines and add project name to some lines
                foreach (string line in template.MainXAML)
                {
                    if (line == "<Window x:Class=")
                        XAMLFile += line + '"' + pName + ".MainWindow" + '"' + Environment.NewLine;
                    else if (line == "        xmlns:local=\"clr-namespace:")
                        XAMLFile += line + pName + '"' + Environment.NewLine;
                    else
                        XAMLFile += line + Environment.NewLine;
                }

                string APPCSFile = "";

                // Add \n to all lines and add project name to some lines
                foreach (string line in template.AppCS)
                {
                    if (line == "namespace ")
                        APPCSFile += line + pName + Environment.NewLine;
                    else
                        APPCSFile += line + Environment.NewLine;
                }

                string APPXAMLFile = "";

                // Add \n to all lines and add project name to some lines
                foreach (string line in template.AppXAML)
                {
                    if (line == "<Application x:Class=")
                        APPXAMLFile += line + '"' + pName + ".App" + '"' + Environment.NewLine;
                    else if (line == "        xmlns:local=\"clr-namespace:")
                        APPXAMLFile += line + pName + '"' + Environment.NewLine;
                    else
                        APPXAMLFile += line + Environment.NewLine;
                }

                string CSPFile = File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\csproj-part1.txt") + projectName + File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\csproj-part2.txt");

                string ASMFile = File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\AssemblyInfo.cs");

                // VERY IMPORTANT!
                // Download settings
                string SettingsJson = "";
                using (WebClient client = new WebClient())
                {
                    SettingsJson = await client.DownloadStringTaskAsync(new Uri(Data.settingsJsonAddress));
                    client.Dispose();
                }

                // Basic folder structure:
                // net6.0-windows>
                //     >assets
                //         >data
                //         >sprites
                //         >sound
                //         -settings.json

                // Write all things
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!Directory.Exists(dir + @"\" + pName))
                {
                    Directory.CreateDirectory(dir + @"\" + pName);
                }

                // Create basic assets structure
                // cd {dir}/bin/COPY/ <-- create there

                string path = dir + @"\" + pName + @"\";

                Directory.CreateDirectory(path + @"\bin"); // <--      create the output folder for compilation
                Directory.CreateDirectory(path + @"\bin\COPY"); // <-- create sub directory to bin
                Directory.CreateDirectory(path + @"\bin\COPY\assets");
                Directory.CreateDirectory(path + @"\bin\COPY\assets\data");
                Directory.CreateDirectory(path + @"\bin\COPY\assets\sound");
                Directory.CreateDirectory(path + @"\bin\COPY\assets\sprites");
                await File.WriteAllTextAsync(path + @"\bin\COPY\assets\settings.json", SettingsJson);

                await File.WriteAllTextAsync(path + "MainWindow.xaml", XAMLFile);
                await File.WriteAllTextAsync(path + "MainWindow.xaml.cs", CSFile);

                await File.WriteAllTextAsync(path + "App.xaml", APPXAMLFile);
                await File.WriteAllTextAsync(path + "App.xaml.cs", APPCSFile);

                await File.WriteAllTextAsync(path + $"{pName}.csproj", CSPFile);
                await File.WriteAllTextAsync(path + "AssemblyInfo.cs", ASMFile);

                // Unpack plugin zip
                string pluginZipDir = Directory.GetCurrentDirectory() + @"\templates\plugins.zip";

                ZipFile.ExtractToDirectory(pluginZipDir, path, true);

                // Run Wrapper

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "Lucid Wrapper.exe";
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Arguments = '"' + path + '"';

                    process.Start();

                    await process.WaitForExitAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lucid Hub Error");
            }
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TemplateRoot
    {
        [JsonProperty("mainCS")]
        public List<string> MainCS { get; set; }

        [JsonProperty("mainXAML")]
        public List<string> MainXAML { get; set; }

        [JsonProperty("appCS")]
        public List<string> AppCS { get; set; }

        [JsonProperty("appXAML")]
        public List<string> AppXAML { get; set; }
    }
}
