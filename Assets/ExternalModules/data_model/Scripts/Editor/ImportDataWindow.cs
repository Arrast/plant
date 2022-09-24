using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace versoft.data_model
{
    public class ImportDataWindow : EditorWindow
    {
        private static ImportDataWindow window = null;

        private string _spreadsheetId;
        private bool _taskRunning = false;

        [MenuItem("Versoft/Data Models/Import Data Window")]
        public static void Init()
        {
            window = ScriptableObject.CreateInstance<ImportDataWindow>();
            window.Show();
        }

        private void Awake()
        {
            _spreadsheetId = PlayerPrefs.GetString(Const.SpreadsheetPlayerPrefsKey, string.Empty);
        }

        private void OnDestroy()
        {
            window = null;
        }

        private void OnGUI()
        {
            _spreadsheetId = EditorGUILayout.TextField("Sheet Id", _spreadsheetId);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_spreadsheetId) || _taskRunning);
            if (GUILayout.Button("Build Classes"))
            {
                PlayerPrefs.SetString(Const.SpreadsheetPlayerPrefsKey, _spreadsheetId);
                EditorCoroutineUtility.StartCoroutine(DownloadData(_spreadsheetId), this);
            }

            EditorGUI.EndDisabledGroup();
        }

        public IEnumerator DownloadData(string documentId)
        {
            _taskRunning = true;
            yield return EditorCoroutineUtility.StartCoroutine(DownloadSheetsData.DownloadData(_spreadsheetId), this);
            _taskRunning = false;
            AssetDatabase.Refresh();
        }
    }
}