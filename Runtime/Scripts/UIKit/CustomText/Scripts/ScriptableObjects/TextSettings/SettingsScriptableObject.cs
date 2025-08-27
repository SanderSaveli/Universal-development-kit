using System;
using UnityEngine;

namespace CustomText
{
    public class SettingsScriptableObject : ScriptableObject
    {
#if UNITY_EDITOR
        private void DeleteObject(SettingsScriptableObject obj)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            UnityEditor.FileUtil.DeleteFileOrDirectory(path);
            UnityEditor.AssetDatabase.Refresh();
        }

        public void Delete(Type type, SettingsScriptableObject obj)
        {
            switch (type)
            {
                case Type when type == typeof(ColorSettingsScriptableObject):
                    Debug.Log("<color=red>Attention!</color> ColorSettings already exist");
                    break;

                case Type when type == typeof(TextStyleSettingsScriptableObject):
                    Debug.Log("<color=red>Attention!</color> TextStyleSettings already exist");
                    break;
                case Type when type == typeof(MaterialSettingsScriptableObject):
                    Debug.Log("<color=red>Attention!</color> MaterialSettings already exist");
                    break;
            }

            DeleteObject(obj);
        }
#endif
    }
}