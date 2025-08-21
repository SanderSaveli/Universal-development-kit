using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProceduralUIImage.Scripts
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class GradientImage : MonoBehaviour
    {
        [SerializeField] private Image _image;

        [SerializeField, Tooltip("Глобальная кривая интерполяции градиента от 0→1")]
        private AnimationCurve _gradientCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private static readonly int UseGradientID = Shader.PropertyToID("_UseGradient");
        private static readonly int GradientTypeID = Shader.PropertyToID("_GradientType");
        private static readonly int StopCountID = Shader.PropertyToID("_StopCount");
        private static readonly int StopPositionsID = Shader.PropertyToID("_StopPositions");
        private static readonly int StopColorsID = Shader.PropertyToID("_StopColors");
        private static readonly int CurveTexID = Shader.PropertyToID("_GradientCurveTex");

        public enum GradientType
        {
            Vertical = 0,
            Horizontal = 1,
            Radial = 2,
            Angle = 3
        }

        [Serializable]
        public struct ColorStop
        {
            [Range(0f, 1f)] public float position;
            public Color color;
        }

        public const int MaxStops = 8;

        [Header("Gradient Settings")] public GradientType _gradientType = GradientType.Vertical;

        [Header("Color Stops (max 8)")] public List<ColorStop> _colorStops = new List<ColorStop>
        {
            new ColorStop { position = 0f, color = Color.white },
            new ColorStop { position = 1f, color = Color.black }
        };

        private Material _materialInstance;
        private Texture2D _curveTexture;
        private const int CURVE_TEX_WIDTH = 256;

        private void Awake()
        {
            _image ??= GetComponent<Image>();
            CreateOrUpdateMaterialInstance();
            UpdateMaterial();
        }

        private void OnValidate()
        {
            _image ??= GetComponent<Image>();
            CreateOrUpdateMaterialInstance();
            UpdateMaterial();
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                if (_materialInstance != null) Destroy(_materialInstance);
                if (_curveTexture != null) DestroyImmediate(_curveTexture);
            }
        }

        private void CreateOrUpdateMaterialInstance()
        {
            if (_materialInstance == null || _materialInstance.shader.name != "UI/Procedural UI Image")
            {
                Shader shader = Shader.Find("UI/Procedural UI Image");
                if (shader == null)
                {
                    Debug.LogError("Shader 'UI/Procedural UI Image' not found.");
                    return;
                }

                _materialInstance = new Material(shader) { hideFlags = HideFlags.DontSave };
                _materialInstance.SetInt(UseGradientID, 1);
                _image.material = _materialInstance;
            }
        }

        private void UpdateMaterial()
        {
            if (_materialInstance == null) return;

            // 1) Bake глобальной кривой в текстуру
            BakeCurveTexture();

            // 2) Сортировка и ограничение цветовых остановок
            _colorStops.Sort((a, b) => a.position.CompareTo(b.position));
            int count = Mathf.Clamp(_colorStops.Count, 2, MaxStops);

            float[] positions = new float[MaxStops];
            Vector4[] colors = new Vector4[MaxStops];

            for (int i = 0; i < MaxStops; i++)
            {
                if (i < count)
                {
                    positions[i] = Mathf.Clamp01(_colorStops[i].position);
                    colors[i] = _colorStops[i].color;
                }
                else
                {
                    positions[i] = 1f;
                    colors[i] = _colorStops[count - 1].color;
                }
            }

            _materialInstance.SetInt(GradientTypeID, (int)_gradientType);
            _materialInstance.SetInt(StopCountID, count);
            _materialInstance.SetFloatArray(StopPositionsID, positions);
            _materialInstance.SetVectorArray(StopColorsID, colors);
        }

        private void BakeCurveTexture()
        {
            if (_curveTexture == null)
            {
                _curveTexture = new Texture2D(CURVE_TEX_WIDTH, 1, TextureFormat.RHalf, false, true)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.DontSave
                };
            }

            // Заполняем строку значениями кривой
            for (int x = 0; x < CURVE_TEX_WIDTH; x++)
            {
                float t = (float)x / (CURVE_TEX_WIDTH - 1);
                float v = _gradientCurve.Evaluate(t);
                _curveTexture.SetPixel(x, 0, new Color(v, v, v, v));
            }

            _curveTexture.Apply();

            _materialInstance.SetTexture(CurveTexID, _curveTexture);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                CreateOrUpdateMaterialInstance();
#endif
            UpdateMaterial();
        }
    }
}