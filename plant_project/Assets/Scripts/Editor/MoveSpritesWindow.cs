using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MoveSpritesWindow : EditorWindow
{
    private bool _showFolders = true;
    private List<PlantContentFolderInformation> _folderInformation = new List<PlantContentFolderInformation>();

    [MenuItem("Project Plant/Content/Move Sprites")]
    public static void ShowWindow()
    {
        MoveSpritesWindow window = (MoveSpritesWindow)GetWindow(typeof(MoveSpritesWindow));
        window.minSize = new Vector2(250, 150);
        window.Show();
    }

    private void OnGUI()
    {
        _showFolders = EditorGUILayout.Foldout(_showFolders, "Folders");
        if (_showFolders)
        {
            EditorGUILayout.BeginVertical();
            foreach (var folderInfo in _folderInformation)
            {
                DrawFolderInfo(folderInfo);
            }
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Scan Folders"))
        {
            _folderInformation.Clear();
            _folderInformation = MoveSpritesLogic.GetFoldersInfo();
        }

        if (GUILayout.Button("Move Sprites") && _folderInformation.Count > 0)
        {
            foreach (var folderInformation in _folderInformation)
            {
                MoveSpritesLogic.MoveFolderContents(folderInformation);
            }
            AssetDatabase.Refresh();
        }
    }


    private void DrawFolderInfo(PlantContentFolderInformation folderInfo)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Plant Name", folderInfo.PlantName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField((folderInfo.Exists) ? "Exists" : "New", GUILayout.Width(50));
        if (folderInfo.Exists)
        {
            folderInfo.Override = EditorGUILayout.Toggle("Override?", folderInfo.Override);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
