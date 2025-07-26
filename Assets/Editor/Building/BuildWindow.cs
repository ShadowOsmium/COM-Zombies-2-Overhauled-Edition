using UnityEditor;
using UnityEngine;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class BuildWindow : EditorWindow // For Unity 2017 Projects
{
    [MenuItem("Build Tools/Epic Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("Custom Build");
    }

    void OnGUI()
    {
        GUILayout.Label("Build Your Project. Made By Nick :)", EditorStyles.boldLabel);

        if (GUILayout.Button("Build Windows Standalone"))
        {
            StartBuild(BuildTarget.StandaloneWindows64, "COM-Zombies-2");            // Change the project name here if your executable should be named differently
        }

        if (GUILayout.Button("Build Android APK"))
        {
            StartBuild(BuildTarget.Android, "COM-Zombies-2");  // Change the project name here if you want a different APK file name
        }

        if (GUILayout.Button("Build iOS Xcode Project"))
        {
            StartBuild(BuildTarget.iOS, "COM-Zombies-2");  // Change the project name here if you want a different Xcode file name
        }
    }

    void StartBuild(BuildTarget target, string baseFileName)
    {
        string[] scenes = GetEnabledScenes();

        string version = PlayerSettings.bundleVersion;
        string baseFolder = @"E:\Mod Exports\" + PlayerSettings.productName;  // CHANGE THIS PATH to your desired root output folder where builds will be saved
        string versionFolder = System.IO.Path.Combine(baseFolder, version);

        string platformFolderName = target == BuildTarget.Android ? "Android" : "Windows";
        string outputFolder = System.IO.Path.Combine(versionFolder, platformFolderName);

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string extension;
        string fullPath;

        // iOS builds generate an Xcode project in a folder
        if (target == BuildTarget.iOS)
        {
            extension = "";
            fullPath = System.IO.Path.Combine(outputFolder, baseFileName + "_iOS_XcodeProject");
        }
        else
        {
            extension = (target == BuildTarget.Android) ? ".apk" : ".exe";
            fullPath = System.IO.Path.Combine(outputFolder, baseFileName + extension);  // Full path to the build output file (e.g. ".../Windows/COM-Zombies-2.exe")
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = scenes;
        buildOptions.locationPathName = fullPath;
        buildOptions.target = target;
        buildOptions.options = BuildOptions.None;

        Debug.Log("Build started for target: " + target);
        BuildPipeline.BuildPlayer(buildOptions);
        Debug.Log("Build finished. Check the output folder.");

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.iOS)
        {
            string platformName = (target == BuildTarget.StandaloneWindows) ? "Windows" : "iOS";

            bool shouldZip = EditorUtility.DisplayDialog(
                "Zip Build?",
                "Build completed successfully for " + platformName + ".\n\nWould you like to create a .zip archive?",
                "Yes, zip it",
                "No, skip"
            );

            if (shouldZip)
            {
                string zipPath = System.IO.Path.Combine(outputFolder, baseFileName + ".zip");

                if (File.Exists(zipPath))
                    File.Delete(zipPath);

                if (target == BuildTarget.StandaloneWindows)
                {
                    string dataFolder = System.IO.Path.Combine(outputFolder, baseFileName + "_Data");
                    CreateZip(zipPath, fullPath, dataFolder, baseFileName + "_Data");
                }
                else if (target == BuildTarget.iOS)
                {
                    CreateZipFromFolder(zipPath, fullPath, baseFileName + "_iOS_XcodeProject");
                }

                Debug.Log("Zipped build created at: " + zipPath);
            }
        }

        EditorUtility.DisplayDialog("Build Complete", "Build saved to:\n" + fullPath, "OK");
        System.Diagnostics.Process.Start(outputFolder);
    }

    void CreateZipFromFolder(string zipFilePath, string folderPath, string rootFolderInZip) // For Xcode Projects (IOS)
    {
        using (FileStream fsOut = File.Create(zipFilePath))
        using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
        {
            zipStream.SetLevel(9); // max compression

            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string relativePath = file.Substring(folderPath.Length + 1).Replace('\\', '/');
                string entryName = rootFolderInZip + "/" + relativePath;

                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = File.GetLastWriteTime(file);
                newEntry.Size = new FileInfo(file).Length;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(file))
                {
                    int sourceBytes;
                    while ((sourceBytes = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        zipStream.Write(buffer, 0, sourceBytes);
                    }
                }

                zipStream.CloseEntry();
            }

            zipStream.Finish();
            zipStream.Close();
        }
    }

    void CreateZip(string zipFilePath, string exeFilePath, string dataFolderPath, string dataFolderInZip)
    {
        using (FileStream fsOut = File.Create(zipFilePath))
        using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
        {
            zipStream.SetLevel(9); // 0-9, 9 = highest compression

            AddFileToZip(zipStream, exeFilePath, System.IO.Path.GetFileName(exeFilePath));

            AddFolderToZip(zipStream, dataFolderPath, dataFolderInZip);

            zipStream.Finish();
            zipStream.Close();
        }
    }

    void AddFileToZip(ZipOutputStream zipStream, string filePath, string entryName)
    {
        ZipEntry newEntry = new ZipEntry(entryName)
        {
            DateTime = File.GetLastWriteTime(filePath),
            Size = new FileInfo(filePath).Length
        };

        zipStream.PutNextEntry(newEntry);

        byte[] buffer = new byte[4096];
        using (FileStream fsInput = File.OpenRead(filePath))
        {
            int sourceBytes;
            while ((sourceBytes = fsInput.Read(buffer, 0, buffer.Length)) > 0)
            {
                zipStream.Write(buffer, 0, sourceBytes);
            }
        }

        zipStream.CloseEntry();
    }

    void AddFolderToZip(ZipOutputStream zipStream, string folderPath, string rootFolderInZip)
    {
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string relativePath = file.Substring(folderPath.Length + 1).Replace('\\', '/');
            string entryName = rootFolderInZip + "/" + relativePath;
            AddFileToZip(zipStream, file, entryName);
        }
    }

    string[] GetEnabledScenes()
    {
        var scenesList = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenesList.Add(scene.path);
        }
        return scenesList.ToArray();
    }
}