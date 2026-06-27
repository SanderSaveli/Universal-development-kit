using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomText
{
    [Serializable]
    public class MaterialParams
    {
        [HideInInspector] public string Name;

        public Custom_MaterialStyle MaterialType;

        public Material Material;
    }

    [CreateAssetMenu(fileName = "MaterialSettings", menuName = "CustomText/Settings/MaterialSettings", order = 0)]
    public class MaterialSettings : MaterialSettingsScriptableObject
    {
        public List<MaterialParams> Materials;

        public event Action OnMaterialStyleChanged;

        private static MaterialSettings _instance;

        public static MaterialSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<MaterialSettings>("TextSettings/MaterialSettings");

                    if (_instance == null)
                    {
                        Debug.Log("<color=red>Attention!</color> MaterialSettings NOT exist");
                    }
                }

                return _instance;
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            foreach (var material in Materials)
            {
                material.Name = material.MaterialType.ToString();
            }

            OnMaterialStyleChanged?.Invoke();
        }
#endif
    }
}