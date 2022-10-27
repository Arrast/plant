using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MoveSpritesLogic
{
    private static HashSet<string> Stages = new HashSet<string>
    {
        "bud",
        "seedling",
        "grown",
        "dead"
    };

    private static string AssetsPath
    {
        get { return Path.Combine(Directory.GetParent(Application.dataPath).Parent.ToString(), "Art_Assets", "Sprites"); }
    }

    public static List<PlantContentFolderInformation> GetFoldersInfo()
    {
        List<PlantContentFolderInformation> plantContentInformation = new List<PlantContentFolderInformation>();
        var folders = Directory.EnumerateDirectories(AssetsPath);
        foreach (var folder in folders)
        {
            if (Path.GetFileName(folder) == Const.DefaultPlantAsset)
            { continue; }

            if (CheckIfFolderCorrect(folder))
            {
                bool exists = Directory.Exists(Const.PlantAssetPath + Path.GetFileName(folder));
                plantContentInformation.Add(new PlantContentFolderInformation()
                {
                    PlantName = Path.GetFileName(folder),
                    Exists = exists,
                    Override = false
                });
            }
        }

        return plantContentInformation;
    }

    public static void MoveFolderContents(PlantContentFolderInformation plantContentFolderInformation)
    {
        if (plantContentFolderInformation.Exists && !plantContentFolderInformation.Override)
        { return; }

        var targetFolder = Path.Combine(AssetsPath, plantContentFolderInformation.PlantName);
        MoveFolderContents(targetFolder);
    }

    public static void MoveFolderContents(string folder)
    {
        var destinationFolder = Const.PlantAssetPath + Path.GetFileName(folder);
        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }

        var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(folder, file);
            var destinationPath = Path.Combine(destinationFolder, Path.GetDirectoryName(relativePath));
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)), true);
        }
    }

    private static bool CheckIfFolderCorrect(string folder)
    {
        var folders = Directory.EnumerateDirectories(folder).ToList();
        if (folders.Count != Stages.Count)
        { return false; }

        foreach (var childFolder in folders)
        {
            if (!Stages.Contains(Path.GetFileName(childFolder)))
            { return false; }
        }
        return true;
    }
}
