using TMPro.EditorUtilities;
using UnityEditor;

namespace CustomText
{
    [CustomEditor(typeof(CustomText), true), CanEditMultipleObjects]
    public class Custom_Text_propDrawer : TMP_EditorPanelUI
    {
        SerializedProperty _textStyle;
        SerializedProperty _textColor;
        SerializedProperty _textMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _textStyle = serializedObject.FindProperty("_textStyle");
            _textColor = serializedObject.FindProperty("_textColor");
            _textMaterial = serializedObject.FindProperty("_textMaterial");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_textStyle);
            EditorGUILayout.PropertyField(_textColor);
            EditorGUILayout.PropertyField(_textMaterial);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
