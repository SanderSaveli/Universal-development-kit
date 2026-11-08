using UnityEditor;
using UnityEngine;

namespace SanderSaveli.UDK
{
    [CustomPropertyDrawer(typeof(Range))]
    [CustomPropertyDrawer(typeof(IntRange))]
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class RangeDrawer : PropertyDrawer
    {
        private const float BUTTON_WIDTH = 24f;
        private const float FIELD_LABEL_WIDTH = 30f;
        private const float BUTTON_SPACING = 8f;
        private const float VALUE_SPACING = 4f;
        private const float VALUE_PADDING = 10f;
        private const string SESSION_KEY_PREFIX = "UDK.RangeDrawer.Fields.";

        private GUIStyle _valueStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            int previousIndent = EditorGUI.indentLevel;
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            bool previousMixedValue = EditorGUI.showMixedValue;

            try
            {
                SerializedProperty min = property.FindPropertyRelative("_min");
                SerializedProperty max = property.FindPropertyRelative("_max");
                if (min == null || max == null)
                {
                    EditorGUI.LabelField(position, label.text, "Requires Range or IntRange");
                    return;
                }

                position.height = EditorGUIUtility.singleLineHeight;
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = FIELD_LABEL_WIDTH;
                MinMaxSliderAttribute slider = attribute as MinMaxSliderAttribute;
                bool isInteger = min.propertyType == SerializedPropertyType.Integer;
                float lowerLimit = slider == null ? float.NegativeInfinity : slider.Min;
                float upperLimit = slider == null ? float.PositiveInfinity : slider.Max;
                if (slider != null && isInteger)
                {
                    lowerLimit = Mathf.CeilToInt(lowerLimit);
                    upperLimit = Mathf.FloorToInt(upperLimit);
                    if (lowerLimit > upperLimit)
                    {
                        EditorGUI.LabelField(position, "No integers within slider limits");
                        return;
                    }
                }

                bool showFields = true;
                if (slider != null)
                {
                    string sessionKey = SESSION_KEY_PREFIX
                        + property.serializedObject.targetObject.GetInstanceID() + "." + property.propertyPath;
                    showFields = SessionState.GetBool(sessionKey, false);
                    Rect buttonPosition = new Rect(position.xMax - BUTTON_WIDTH, position.y,
                        BUTTON_WIDTH, position.height);
                    position.width -= BUTTON_WIDTH + BUTTON_SPACING;
                    GUIContent button = new GUIContent(showFields ? "S" : "V",
                        showFields ? "Show slider" : "Enter exact values");
                    if (GUI.Button(buttonPosition, button, EditorStyles.miniButton))
                    {
                        showFields = !showFields;
                        SessionState.SetBool(sessionKey, showFields);
                    }
                }

                if (showFields)
                    DrawFields(position, min, max, lowerLimit, upperLimit);
                else
                    DrawSlider(position, min, max, lowerLimit, upperLimit);
            }
            finally
            {
                EditorGUI.indentLevel = previousIndent;
                EditorGUIUtility.labelWidth = previousLabelWidth;
                EditorGUI.showMixedValue = previousMixedValue;
                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void DrawFields(Rect position, SerializedProperty min, SerializedProperty max,
            float lowerLimit, float upperLimit)
        {
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            position.width = (position.width - spacing) * 0.5f;
            bool isInteger = min.propertyType == SerializedPropertyType.Integer;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, min, new GUIContent("Min:"));
            if (EditorGUI.EndChangeCheck())
            {
                if (isInteger)
                {
                    int intLower = float.IsNegativeInfinity(lowerLimit) ? int.MinValue : (int)lowerLimit;
                    int intUpper = float.IsPositiveInfinity(upperLimit) ? int.MaxValue : (int)upperLimit;
                    if (!max.hasMultipleDifferentValues)
                        max.intValue = Mathf.Clamp(max.intValue, intLower, intUpper);
                    min.intValue = Mathf.Clamp(min.intValue, intLower, Mathf.Min(max.intValue, intUpper));
                }
                else
                {
                    if (!max.hasMultipleDifferentValues)
                        max.floatValue = Mathf.Clamp(max.floatValue, lowerLimit, upperLimit);
                    min.floatValue = Mathf.Clamp(min.floatValue, lowerLimit, Mathf.Min(max.floatValue, upperLimit));
                }
            }

            position.x += position.width + spacing;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, max, new GUIContent("Max:"));
            if (EditorGUI.EndChangeCheck())
            {
                if (isInteger)
                {
                    int intLower = float.IsNegativeInfinity(lowerLimit) ? int.MinValue : (int)lowerLimit;
                    int intUpper = float.IsPositiveInfinity(upperLimit) ? int.MaxValue : (int)upperLimit;
                    if (!min.hasMultipleDifferentValues)
                        min.intValue = Mathf.Clamp(min.intValue, intLower, intUpper);
                    max.intValue = Mathf.Clamp(max.intValue, Mathf.Max(min.intValue, intLower), intUpper);
                }
                else
                {
                    if (!min.hasMultipleDifferentValues)
                        min.floatValue = Mathf.Clamp(min.floatValue, lowerLimit, upperLimit);
                    max.floatValue = Mathf.Clamp(max.floatValue, Mathf.Max(min.floatValue, lowerLimit), upperLimit);
                }
            }
        }

        private void DrawSlider(Rect position, SerializedProperty min, SerializedProperty max,
            float lowerLimit, float upperLimit)
        {
            bool isInteger = min.propertyType == SerializedPropertyType.Integer;
            float minValue = isInteger ? min.intValue : min.floatValue;
            float maxValue = isInteger ? max.intValue : max.floatValue;
            EditorGUI.showMixedValue = min.hasMultipleDifferentValues || max.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, lowerLimit, upperLimit);
            if (EditorGUI.EndChangeCheck())
            {
                if (isInteger)
                {
                    min.intValue = Mathf.Clamp(Mathf.RoundToInt(minValue), (int)lowerLimit, (int)upperLimit);
                    max.intValue = Mathf.Clamp(Mathf.RoundToInt(maxValue), min.intValue, (int)upperLimit);
                }
                else
                {
                    min.floatValue = Mathf.Clamp(minValue, lowerLimit, upperLimit);
                    max.floatValue = Mathf.Clamp(maxValue, min.floatValue, upperLimit);
                }
            }

            if (position.Contains(Event.current.mousePosition))
            {
                DrawSliderValues(position, min, max, lowerLimit, upperLimit);
            }

            if (Event.current.type == EventType.MouseMove && EditorWindow.mouseOverWindow != null)
                EditorWindow.mouseOverWindow.Repaint();
        }

        private void DrawSliderValues(Rect position, SerializedProperty min, SerializedProperty max,
            float lowerLimit, float upperLimit)
        {
            if (_valueStyle == null)
            {
                _valueStyle = new GUIStyle(GUI.skin.FindStyle("Tooltip") ?? EditorStyles.helpBox);
                _valueStyle.alignment = TextAnchor.MiddleCenter;
                _valueStyle.clipping = TextClipping.Clip;
                _valueStyle.wordWrap = false;
                _valueStyle.fixedHeight = 0f;
                _valueStyle.contentOffset = Vector2.zero;
            }

            bool isInteger = min.propertyType == SerializedPropertyType.Integer;
            float minValue = isInteger ? min.intValue : min.floatValue;
            float maxValue = isInteger ? max.intValue : max.floatValue;
            string format = isInteger ? "0" : "0.#####";
            GUIContent[] values =
            {
                new GUIContent(lowerLimit.ToString(format), "Slider minimum"),
                new GUIContent(min.hasMultipleDifferentValues ? "—" : minValue.ToString(format), "Current minimum"),
                new GUIContent(max.hasMultipleDifferentValues ? "—" : maxValue.ToString(format), "Current maximum"),
                new GUIContent(upperLimit.ToString(format), "Slider maximum")
            };
            float[] anchors =
            {
                position.xMin,
                Mathf.Lerp(position.xMin, position.xMax, Mathf.InverseLerp(lowerLimit, upperLimit, minValue)),
                Mathf.Lerp(position.xMin, position.xMax, Mathf.InverseLerp(lowerLimit, upperLimit, maxValue)),
                position.xMax
            };
            Rect[] boxes = new Rect[values.Length];
            float maxWidth = Mathf.Max(0f, (position.width - VALUE_SPACING * (values.Length - 1)) / values.Length);
            float nextX = position.xMin;
            for (int i = 0; i < values.Length; i++)
            {
                Vector2 contentSize = _valueStyle.CalcSize(values[i]);
                float width = Mathf.Min(maxWidth, contentSize.x + VALUE_PADDING);
                float height = Mathf.Max(contentSize.y,
                    EditorGUIUtility.singleLineHeight + _valueStyle.padding.vertical);
                float x = Mathf.Max(nextX, anchors[i] - width * 0.5f);
                boxes[i] = new Rect(x, position.y - height - VALUE_SPACING, width, height);
                nextX = boxes[i].xMax + VALUE_SPACING;
            }

            float rightEdge = position.xMax;
            for (int i = boxes.Length - 1; i >= 0; i--)
            {
                Rect box = boxes[i];
                box.x = Mathf.Min(box.x, rightEdge - box.width);
                boxes[i] = box;
                rightEdge = box.xMin - VALUE_SPACING;
            }

            EditorGUI.showMixedValue = false;
            for (int i = 0; i < boxes.Length; i++)
            {
                GUI.Label(boxes[i], values[i], _valueStyle);
            }
        }
    }
}
