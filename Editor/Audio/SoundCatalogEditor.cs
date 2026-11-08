using UnityEditor;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [CustomEditor(typeof(SoundCatalog))]
    internal sealed class SoundCatalogEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            SoundKeyGenerator.ValidationChanged += Repaint;
            SoundKeyGenerator.QueueValidation();
        }

        private void OnDisable()
        {
            SoundKeyGenerator.ValidationChanged -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector())
                SoundKeyGenerator.QueueValidation();

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("Validate Catalog"))
                {
                    SoundKeyGenerator.Validate();
                }
            }

            EditorGUILayout.HelpBox("Each theme catalog must contain exactly one SoundConfig for every generated ID. "
                + "Different catalogs can use different configs with the same ID. Generate IDs in a SoundConfig inspector.", MessageType.Info);
            string catalogError = SoundKeyGenerator.GetCatalogError((SoundCatalog)target);
            if (!string.IsNullOrEmpty(catalogError))
                EditorGUILayout.HelpBox(catalogError, MessageType.Error);
            if (!string.IsNullOrEmpty(SoundKeyGenerator.Error))
                EditorGUILayout.HelpBox(SoundKeyGenerator.Error, MessageType.Error);
        }
    }
}
