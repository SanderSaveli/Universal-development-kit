#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CustomText
{
    public class Custom_TextCreater
    {
        [MenuItem("GameObject/UI/Custom Components/Text", false, 2)]
        static void Create(MenuCommand menuCommand)
        {
            GameObject go = new("CustomText");
            var text = go.AddComponent<CustomText>();
            text.text = "New Text";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif
