using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.GravityMaze
{
    public class SliderValueView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Slider _slider;

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(UpdateValue);
            UpdateValue(_slider.value);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(UpdateValue);
        }

        private void UpdateValue(float value)
        {
            _text.text = value.ToString();
        }
    }
}
