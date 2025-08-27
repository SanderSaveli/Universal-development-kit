using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using CustomText;
using ProceduralUIImage.Scripts;
using Unity.VisualScripting;

namespace SanderSaveli.UDK.UI
{

    public class ExpandableItemEditor : MonoBehaviour
    {
        [MenuItem("GameObject/UI/Custom Components/Expandable Item", false, 10)]
        public static void CreateExpandableItem(MenuCommand menuCommand)
        {
            GameObject expandableItem = new GameObject("ExpandableItem", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(expandableItem, "Create ExpandableItem");

            RectTransform expandableTransform = expandableItem.GetComponent<RectTransform>();
            SetupRectTransform(expandableTransform, new Vector2(400, 0));
            expandableItem.AddComponent<ProceduralImage>();

            VerticalLayoutGroup layoutGroup = expandableItem.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;

            ContentSizeFitter fitter = expandableItem.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ExpandableMenuItem expandableMenuItem = expandableItem.AddComponent<ExpandableMenuItem>();

            Button button = expandableItem.AddComponent<Button>();

            GameObject header = CrateTextPanel(expandableTransform.transform, "Header");

            GameObject description = CrateTextPanel(expandableTransform.transform, "Description");
            expandableMenuItem.DescriptionPanel = description;
            expandableMenuItem.ToggleButton = button;

            GameObjectUtility.SetParentAndAlign(expandableItem, menuCommand.context as GameObject);

            Selection.activeGameObject = expandableItem;
        }

        private static GameObject CreateCustomText(string name, Transform parent)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CustomText.CustomText));
            textObject.transform.SetParent(parent);
            RectTransform textTransform = textObject.GetComponent<RectTransform>();
            SetupRectTransform(textTransform, new Vector2(400, 80));

            CustomText.CustomText customText = textObject.GetComponent<CustomText.CustomText>();
            customText.ChangeColor(Custom_ColorStyle.Black);
            customText.text = "New Text";

            return textObject;
        }

        private static GameObject CrateTextPanel(Transform parent, string name)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent);
            RectTransform descriptionTransform = panel.GetComponent<RectTransform>();
            SetupRectTransform(descriptionTransform, new Vector2(400, 100));
            VerticalLayoutGroup layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.padding = new RectOffset(20,20, 20, 20);
            ContentSizeFitter sizeFitter = panel.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            GameObject descriptionText = CreateCustomText("CustomText", panel.transform);
            ContentSizeFitter textSizeFitter = descriptionText.AddComponent<ContentSizeFitter>();
            textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return panel;
        }

        private static void SetupRectTransform(RectTransform rectTransform, Vector2 sizeDelta = default)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
