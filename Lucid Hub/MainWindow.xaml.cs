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
using Newtonsoft.Json;

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
        }
    }

    public static class Data
    {
        public static string savedProjectDir = "";

        public static async Task Initiate()
        {
            if (File.Exists("pdir"))
                savedProjectDir = await File.ReadAllTextAsync("pdir");
        }
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
            // Get json:
            string jsonDir = Directory.GetCurrentDirectory() + @"\templates\templates.json";
            string jsonContents = File.ReadAllText(jsonDir);

            TemplateRoot template = JsonConvert.DeserializeObject<TemplateRoot>(jsonContents);

            string CSFile = "";

            // Add \n to all lines and add project name to some lines
            foreach(string line in template.MainCS)
            {
                if(line == "namespace ")
                    CSFile += line + projectName + Environment.NewLine;
                else
                    CSFile += line + Environment.NewLine;
            }

            string XAMLFile = "";

            // Add \n to all lines and add project name to some lines
            foreach (string line in template.MainXAML)
            {
                if (line == "<Window x:Class=")
                    XAMLFile += line + '"' + projectName + ".MainWindow" + '"' + Environment.NewLine;
                else
                    XAMLFile += line + Environment.NewLine;
            }

            XAMLFile += File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\xaml-tonext.txt");
            XAMLFile += projectName + File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\xaml-toend.txt");

            string CSPFile = File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\csproj-part1.txt") + projectName + File.ReadAllText(Directory.GetCurrentDirectory() + @"\templates\csproj-part2.txt");
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TemplateRoot
    {
        [JsonProperty("mainCS")]
        public List<string> MainCS { get; set; }

        [JsonProperty("mainXAML")]
        public List<string> MainXAML { get; set; }
    }
}
