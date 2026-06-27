#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace SanderSaveli.UDK.Editor
{
    [InitializeOnLoad]
    public static class SceneToolbar
    {
        private static string[] _sceneNames = Array.Empty<string>();
        private static string[] _scenePaths = Array.Empty<string>();

        static SceneToolbar()
        {
            RefreshScenes();

            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            EditorBuildSettings.sceneListChanged += RefreshScenes;
        }

        private static void RefreshScenes()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .ToArray();

            _sceneNames = scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            _scenePaths = scenes
                .Select(s => s.path)
                .ToArray();
        }

        private static void OnToolbarGUI()
        {
            if (_sceneNames.Length == 0)
                return;

            int currentIndex = Array.IndexOf(
                _scenePaths,
                SceneManager.GetActiveScene().path);

            int newIndex = EditorGUILayout.Popup(
                currentIndex,
                _sceneNames,
                GUILayout.Width(150));

            if (newIndex == currentIndex)
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(_scenePaths[newIndex]);
        }
    }
}

#endif