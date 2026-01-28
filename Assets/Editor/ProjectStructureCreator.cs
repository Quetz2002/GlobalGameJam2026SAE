using UnityEditor;
using UnityEngine;
using System.IO;

public class ProjectStructureCreator
{
    [MenuItem("Tools/Create Platformer Structure")]
    public static void CreateFolders()
    {
        string[] folders =
        {
            "Assets/Scripts",
            "Assets/Scripts/Core",
            "Assets/Scripts/Player",
            "Assets/Scripts/Enemies",
            "Assets/Scripts/Setup",
            "Assets/Prefabs",
            "Assets/Prefabs/Player",
            "Assets/Prefabs/Enemies",
            "Assets/Prefabs/Level",
            "Assets/Art",
            "Assets/Audio",
            "Assets/Scenes"
        };

        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        AssetDatabase.Refresh();
        Debug.Log("Platformer structure created!");
    }
}
