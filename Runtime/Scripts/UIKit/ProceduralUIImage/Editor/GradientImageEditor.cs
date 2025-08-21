using ProceduralUIImage.Scripts;
using UnityEditor;
using UnityEngine;

namespace ProceduralUIImage.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(GradientImage))]
    public class GradientImageEditor : UnityEditor.Editor
    {
        private SerializedProperty _imageProp;
        private SerializedProperty _gradTypeProp;
        private SerializedProperty _stopsProp;
        private SerializedProperty _curveProp;

        private void OnEnable()
        {
            _imageProp = serializedObject.FindProperty("_image");
            _gradTypeProp = serializedObject.FindProperty("_gradientType");
            _stopsProp = serializedObject.FindProperty("_colorStops");
            _curveProp = serializedObject.FindProperty("_gradientCurve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_imageProp);
            EditorGUILayout.PropertyField(_gradTypeProp);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Color Stops (0..1 positions)", EditorStyles.boldLabel);
            for (int i = 0; i < _stopsProp.arraySize; i++)
            {
                var elem = _stopsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(elem, new GUIContent($"Stop {i}"));
            }

            if (GUILayout.Button("Add Stop") && _stopsProp.arraySize < GradientImage.MaxStops)
            {
                _stopsProp.InsertArrayElementAtIndex(_stopsProp.arraySize);
                var newElem = _stopsProp.GetArrayElementAtIndex(_stopsProp.arraySize - 1);
                newElem.FindPropertyRelative("position").floatValue = 1f;
                newElem.FindPropertyRelative("color").colorValue = Color.black;
            }

            if (GUILayout.Button("Remove Last Stop") && _stopsProp.arraySize > 2)
            {
                _stopsProp.DeleteArrayElementAtIndex(_stopsProp.arraySize - 1);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Gradient Curve", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_curveProp, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}