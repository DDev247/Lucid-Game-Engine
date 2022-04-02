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
        public static async Task GenerateProject(string projectName, string projectDir)
        {
            string templatePath = Directory.GetCurrentDirectory() + @"\templates\";

            // Getting the templates
            string projP1 = await File.ReadAllTextAsync(templatePath + "csproj-part1.txt");
            string projP2 = await File.ReadAllTextAsync(templatePath + "csproj-part2.txt");

            string xamlP1 = await File.ReadAllTextAsync(templatePath + "xaml-part1.txt");
            string xamlP2 = await File.ReadAllTextAsync(templatePath + "xaml-part2.txt");

            string csP1 = await File.ReadAllTextAsync(templatePath + "cs-part1.txt");
            string csP2 = await File.ReadAllTextAsync(templatePath + "cs-part2.txt");

            // Applying the templates
            string projFileContents = projP1 + projectName + projP2;
            string projFilePath = projectDir + @"\" + projectName + @"\" + projectName + ".csproj";

            string xamlFileContents = xamlP1 + projectName + xamlP2;
            string xamlFilePath = projectDir + @"\" + projectName + @"\MainWindow.xaml";

            string csFileContents = csP1 + projectName + csP2;
            string csFilePath = projectDir + @"\" + projectName + @"\MainWindow.xaml.cs";

            string assInfoContents = await File.ReadAllTextAsync(templatePath + "AssemblyInfo.cs");
            string assInfoFilePath = projectDir + @"\" + projectName + @"\AssemblyInfo.cs";

            // Checking if the directories exist
            // if not then create them
            if (!Directory.Exists(projectDir))
                Directory.CreateDirectory(projectDir);

            if(!Directory.Exists(projectDir + @"\" + projectName))
                Directory.CreateDirectory(projectDir + @"\" + projectName);

            // Write the templates to files
            await File.WriteAllTextAsync(projFilePath, projFileContents);
            await File.WriteAllTextAsync(xamlFilePath, xamlFileContents);
            await File.WriteAllTextAsync(projFilePath, projFileContents);

        }
    }
}
