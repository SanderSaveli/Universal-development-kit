using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    [Serializable]
    public class TabTypePair<T>
    {
        public T Type;
        public CanvasGroup Tab;
    }

    public class RadioButtonTabSwitcher<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private RadioButtonGroup<T> _radioButtonGroup;
        [SerializeField] private List<TabTypePair<T>> _tabs;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private RectTransform _rebuild;

        private CanvasGroup _previousTab;

        private void OnEnable()
        {
            DisableAll(_radioButtonGroup.Value);
            _radioButtonGroup.OnValueChanged += HadnleChangeTab;
        }

        private void Start()
        {
            HadnleChangeTab(_radioButtonGroup.Value);
        }

        private void OnDisable()
        {
            _radioButtonGroup.OnValueChanged -= HadnleChangeTab;
        }

        private void HadnleChangeTab(T value)
        {
            TabTypePair<T> currentPair = _tabs.FirstOrDefault(t => t.Type.Equals(value));
            CanvasGroup currentTab = currentPair.Tab;
            currentTab.gameObject.SetActive(true);
            if(_previousTab != null)
            {
                _previousTab.gameObject.SetActive(false);
            }
            currentTab.alpha = 0.05f;
            currentTab.DOFade(1, _animationDuration);
            _previousTab = currentTab;
            StartCoroutine(WaitAndRebuild());
        }

        private void DisableAll(T value)
        {
            foreach(var tab in _tabs) 
            { 
                if(!tab.Type.Equals(value))
                {
                    tab.Tab.gameObject.SetActive(false);
                }
                else
                {
                    tab.Tab.gameObject.SetActive(true);
                }
            }
        }

        private IEnumerator WaitAndRebuild()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_previousTab.transform as RectTransform);
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rebuild);
        }
    }
}
