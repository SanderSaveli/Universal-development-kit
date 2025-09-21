using ProceduralUIImage.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK
{
    public class ToggleSwitchEditor : MonoBehaviour
    {
        [MenuItem("GameObject/UI/Custom Components/Toggle Switch", false, 20)]
        public static void CreateVerticalToggle(MenuCommand menuCommand)
        {
            GameObject toggleGO = new GameObject("Vertical toggle", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(toggleGO, "Create ExpandableItem");

            RectTransform toggleTransform = toggleGO.GetComponent<RectTransform>();
            toggleTransform.sizeDelta = new Vector2(90, 50);
            GameObject handleGO = new GameObject("Handle", typeof(RectTransform));
            handleGO.transform.SetParent(toggleTransform);

            ProceduralImage image = toggleGO.AddComponent<ProceduralImage>();
            image.ModifierType = typeof(RoundModifier);
            image.color = Color.gray;

            ProceduralImage handleImage = handleGO.AddComponent<ProceduralImage>();
            handleImage.ModifierType = typeof(RoundModifier);
            handleImage.color = Color.white;

            toggleGO.AddComponent<Button>();

            UIToggleSwitch toggle = toggleGO.AddComponent<UIToggleSwitch>();
            RectTransform handleTransform = handleGO.GetComponent<RectTransform>();
            handleTransform.sizeDelta = new Vector2(40, 40);
            toggle.Handler = handleTransform;

            handleTransform.anchorMax = new Vector2(0, 0.5f);
            handleTransform.anchorMin = new Vector2(0, 0.5f);

            handleTransform.pivot = new Vector2(0, 0.5f);

            handleTransform.anchoredPosition = new Vector2(toggle.AncorMinX, 0);

            GameObjectUtility.SetParentAndAlign(toggleGO, menuCommand.context as GameObject);
        }
    }
}
