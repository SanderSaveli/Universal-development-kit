using Cysharp.Threading.Tasks;
using SanderSaveli.UDK.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SanderSaveli.UDK.Managers
{
    public abstract class SceneLoader : MonoBehaviour, ISceneLoader
    {
        public bool IsLoading => _isLoading;
        public SceneName CurrentTargetScene { get; private set; }
        public float LoadingProgress { get; private set; }
        public Action<float> OnProgressChange { get; set; }
        public Action<SceneName> OnTargetSceneChange { get; set; }

        [Header("Components")]
        [SerializeField] private List<SceneTransitionPair> _transitions;
        [SerializeField] private TransitionScreen _defaultScreen;

        [Header("Parameters")]
        [SerializeField] private bool _isUseDefaultScreen;

        private bool _isLoading;

        public async UniTask LoadScene(SceneName sceneName)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"Scene loading is already in progress. Ignored request: {sceneName}");
                return;
            }

            string sceneNameString = sceneName.ToString();

            if (!IsSceneExists(sceneNameString))
            {
                Debug.LogError($"Scene '{sceneNameString}' does not exist or is not added to Build Settings.");
                return;
            }

            _isLoading = true;
            SetCurrentTargetScene(sceneName);
            SetLoadingProgress(0f);

            UiScreen transitionScreen = GetTransitionScreen(sceneName);

            try
            {
                if (transitionScreen != null)
                    await transitionScreen.Show();

                await LoadSceneAsync(sceneName);

                SetLoadingProgress(1f);
            }
            finally
            {
                if (transitionScreen != null)
                    await transitionScreen.Hide();

                _isLoading = false;
            }
        }

        public async UniTask ReloadCurrentScene()
        {
            if (_isLoading)
            {
                Debug.LogWarning("Scene loading is already in progress. Reload request ignored.");
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (!Enum.TryParse(currentSceneName, out SceneName sceneName))
            {
                Debug.LogError($"Current active scene '{currentSceneName}' is not defined in SceneName enum.");
                return;
            }

            await LoadScene(sceneName);
        }

        private UiScreen GetTransitionScreen(SceneName sceneName)
        {
            UiScreen transitionScreen = GetTransitionScreenBySceneName(sceneName);

            return transitionScreen != null
                ? transitionScreen
                : GetDefaultTransitionScreen();
        }

        private UiScreen GetTransitionScreenBySceneName(SceneName sceneName)
        {
            if (_transitions == null)
                return null;

            for (int i = 0; i < _transitions.Count; i++)
            {
                SceneTransitionPair transitionPair = _transitions[i];

                if (transitionPair.SceneName == sceneName)
                    return transitionPair.TransitionScreen;
            }

            return null;
        }

        private UiScreen GetDefaultTransitionScreen()
        {
            return _isUseDefaultScreen ? _defaultScreen : null;
        }

        private bool IsSceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);

                if (name == sceneName)
                    return true;
            }

            return false;
        }

        private async UniTask LoadSceneAsync(SceneName sceneName)
        {
            string sceneNameString = sceneName.ToString();

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNameString);

            while (!operation.isDone)
            {
                SetLoadingProgress(operation.progress);
                await UniTask.Yield();
            }

            SetLoadingProgress(1f);
        }

        private void SetLoadingProgress(float progress)
        {
            LoadingProgress = progress;
            OnProgressChange?.Invoke(LoadingProgress);
        }

        private void SetCurrentTargetScene(SceneName sceneName)
        {
            CurrentTargetScene = sceneName;
            OnTargetSceneChange?.Invoke(CurrentTargetScene);
        }

        [Serializable]
        private class SceneTransitionPair
        {
            public SceneName SceneName => _sceneName;
            public UiScreen TransitionScreen => _transitionScreen;

            [SerializeField] private SceneName _sceneName;
            [SerializeField] private UiScreen _transitionScreen;
        }
    }
}
