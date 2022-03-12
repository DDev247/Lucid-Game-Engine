# Installing it manually
 # Step 1. 
 Download all the files from https://github.com/DDev247/Lucid-Game-Engine/tree/main/Lucid%20Game%20Engine/bin/Debug/net6.0-windows
 # Step 2. 
 Create a WPF C# .NET 6.0 project
 # Step 3. 
 Create a folder in the project's folder
 # Step 4. 
 Drag in all the files from the downloaded zip
 # Step 5. 
 Go to VS and click on Dependencies>Add Project Reference then select Explore and navigate to the folder and select the LucidGE.dll file
 # Step 6. 
 Go to MainWindow.xaml.cs (click the arrow on the MainWindow.xaml) and add to the MainWindow() function the following line:
 LucidGE.Interaction.MainInteraction.InitGE(this);
 # Step 7. 
 Click the "play" button next to the Debug|Any CPU and it can throw an error but don't worry about it. We will fix it.
 # Step 8. 
 Fixing the broken thing: go to the bin/Debug/net----/ and create a folder named "assets" and there create a new text file and name it "settings.adp" and open it with notepad++
 # Step 9. 
 Write the following text to the file:
 "800
 600
 True
 Lucid Example" AND remember to go to File>line ending conversion(or something like that idk) and convert to Unix (LF) line endings, or download the file from https://github.com/DDev247/Lucid-Game-Engine/blob/main/settings.adp
 # Step 10. 
 Relaunch the app from VS or double click the .exe file
# Installing it with the wrapper
 # Step 1.
 Download the wrapper from https://github.com/DDev247/Lucid-Game-Engine/tree/main/LucidWrapper/bin/Debug/net6.0-windows (download the entire directory)
 # Step 2.
 Unpack it somewhere eg. C:\LucidWrapper
 # Step 3.
 Create a WPF C# .NET 6.0 project
 # Step 4.
 Run the wrapper and in the 'Project Location' box insert the directory where the .csproj file is, and you can insert a custom name for the folder(where the .dll file will be stored) in the 'DLL Subfolder Name' box
 # Step 5.
 Go to VS and click on Dependencies>Add Project Reference then select Explore and navigate to the folder and select the Lucid Game Engine.dll file
 # Step 6. 
 Go to MainWindow.xaml.cs (click the arrow on the MainWindow.xaml) and add to the MainWindow() function the following line:
 LucidGE.Interaction.MainInteraction.InitGE(this);
 # Step 7. 
 Click the "play" button next to the Debug|Any CPU and it can throw an error but don't worry about it. We will fix it.
 # Step 8. 
 Fixing the broken thing: go to the bin/Debug/net----/ and create a folder named "assets" and there create a new text file and name it "settings.adp" and open it with notepad++
 # Step 9. 
 Write the following text to the file:
 "800
 600
 True
 Lucid Example" AND remember to go to File>line ending conversion(or something like that idk) and convert to Unix (LF) line endings, or download the file from https://github.com/DDev247/Lucid-Game-Engine/blob/main/settings.adp
 # Step 10. 
 Relaunch the app from VS or double click the .exe file