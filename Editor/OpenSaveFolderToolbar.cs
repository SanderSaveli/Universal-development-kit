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
                    new GUIContent("Tools/Open Save Folder", "Open Application.persistentDataPath"),
                    EditorStyles.toolbarButton,
                    GUILayout.Width(160)))
            {
                OpenSaveFolder();
            }
        }

        [MenuItem("Tools/Open Save Folder")]
        private static void OpenSaveFolder()
        {
            string path = Application.persistentDataPath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            EditorUtility.RevealInFinder(path);
        }
    }
}
#endif
