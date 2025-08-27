using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomText
{
    [Serializable]
    public class TextStyleParams
    {
        [HideInInspector] public string Name;
        public Custom_TextStyle TextType;
        public float TextSize;
        public TMP_FontAsset Font;
        public FontStyles FontStyle;
        public FontWeight FontWeight = FontWeight.Regular;
        public float CharacterSpacing = 0f;
        public float LineSpacing = 0f;
    }

    [CreateAssetMenu(fileName = "TextStyleSettings", menuName = "CustomText/Settings/TextStyleSettings", order = 0)]
    public class TextStyleSettings : TextStyleSettingsScriptableObject
    {
        public List<TextStyleParams> TextStyles;

        public event Action OnTextStyleChanged;

        private static TextStyleSettings _instance;

        public static TextStyleSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TextStyleSettings>("TextSettings/TextStyleSettings");

                    if (_instance == null)
                    {
                        Debug.Log("<color=red>Attention!</color> TextStyleSettings NOT exist");
                    }
                }

                return _instance;
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            foreach (var textStyle in TextStyles)
            {
                textStyle.Name = textStyle.TextType.ToString();
            }

            OnTextStyleChanged?.Invoke();
        }
#endif
    }
}