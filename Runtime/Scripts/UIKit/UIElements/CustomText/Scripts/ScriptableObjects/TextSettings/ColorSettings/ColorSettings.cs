using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CustomText
{
    [Serializable]
    public class ColorParams
    {
        [HideInInspector] public string Name;

        public Custom_ColorStyle TextColorType;

        public Color Color;
    }

    [CreateAssetMenu(fileName = "ColorSettings", menuName = "CustomText/Settings/ColorSettings", order = 0)]
    public class ColorSettings : ScriptableObject
    {
        public List<ColorParams> Colors;

        public event Action OnColorStyleChanged;

        private static ColorSettings _instance;
        public static ColorSettings Instance => _instance ??= LoadInstance();

        public Color GetColorByStyle(Custom_ColorStyle style)=>
            Colors.Find(t => t.TextColorType.Equals(style)).Color;

        private static ColorSettings LoadInstance()
        {
            var instance = Resources.Load<ColorSettings>("TextSettings/ColorSettings");
            if (!instance) Debug.LogError("<color=red>Error!</color> ColorSettings not found");
            return instance;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InitializeIfNeeded();
            Colors.ForEach(c => c.Name = c.TextColorType.ToString());
            OnColorStyleChanged?.Invoke();
        }

        private void InitializeIfNeeded()
        {
            if (Colors == null) Colors = new List<ColorParams>();
            Colors.RemoveAll(c => c == null);

            if (Colors.Count == 0)
                Colors.Add(new ColorParams());
        }
#endif
    }
}