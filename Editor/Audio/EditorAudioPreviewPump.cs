using System;
using System.Reflection;
using UnityEditor;

namespace SanderSaveli.UDK.Audio.Editor
{
    internal static class EditorAudioPreviewPump
    {
        private static Action _updateAudio;

        internal static bool TryInitialize(out string error)
        {
            if (_updateAudio != null)
            {
                error = null;
                return true;
            }

            Type audioUtil = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtil?.GetMethod("UpdateAudio", BindingFlags.Static | BindingFlags.Public
                | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (method == null)
            {
                error = "Audio preview is unavailable: this Unity editor has no AudioUtil.UpdateAudio API.";
                return false;
            }

            _updateAudio = (Action)Delegate.CreateDelegate(typeof(Action), method);
            error = null;
            return true;
        }

        internal static void Update()
        {
            _updateAudio();
        }
    }
}
