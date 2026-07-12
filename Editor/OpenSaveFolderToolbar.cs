#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace SanderSaveli.UDK.Editor
{
    [InitializeOnLoad]
    public static class OpenSaveFolderToolbar
    {
        static OpenSaveFolderToolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.Space(5);

            if (GUILayout.Button(
                    new GUIContent("Tools/OpenFolder/Save", "Open Application.persistentDataPath"),
                    EditorStyles.toolbarButton,
                    GUILayout.Width(160)))
            {
                OpenSaveFolder();
            }
        }

        [MenuItem("Tools/OpenFolder/Save")]
        private static void OpenSaveFolder()
        {
            string path = Application.persistentDataPath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            EditorUtility.RevealInFinder(path);
        }

        [MenuItem("Tools/OpenFolder/Logs")]
        private static void OpenLogsFolder()
        {
            string localAppDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(localAppDataPath, "Unity", "Editor");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            EditorUtility.RevealInFinder(path);
        }
    }
}
#endif
