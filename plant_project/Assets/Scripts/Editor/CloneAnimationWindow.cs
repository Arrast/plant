using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloneAnimationWindow : EditorWindow
{
    private bool _showFolders = true;
    private List<AnimationFolderInformation> _folderInformation = new List<AnimationFolderInformation>();

    [MenuItem("Project Plant/Content/Animations")]
    public static void ShowWindow()
    {
        CloneAnimationWindow window = (CloneAnimationWindow)GetWindow(typeof(CloneAnimationWindow));
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
            _folderInformation = CloneAnimationLogic.GetListOfFolders();
        }

        if (GUILayout.Button("Build Animations") && _folderInformation.Count > 0)
        {
            foreach (var folderInformation in _folderInformation)
            {
                CloneAnimationLogic.CreateAnimations(folderInformation);
            }
        }
    }

    private void DrawFolderInfo(AnimationFolderInformation folderInfo)
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
