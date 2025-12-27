using UnityEditor;

namespace SanderSaveli.GravityMaze
{
    [CustomEditor(typeof(StepSlider))]
    public class StepSliderEditor : UnityEditor.UI.SliderEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Step Settings", EditorStyles.boldLabel);

            SerializedProperty step = serializedObject.FindProperty("Step");
            EditorGUILayout.PropertyField(step);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
