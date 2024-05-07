using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SceneLoad
{
    public interface ISceneData
    { }

    [RequireComponent(typeof(ITransitionAnimator))]
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        public static SceneLoader instance => _instance;
        private static SceneLoader _instance;
        private ISceneData _sceneData;
        private ITransitionAnimator _animator;
        private Action<bool> sceneLoaded;

        private void Start()
        {
            _animator = GetComponent<ITransitionAnimator>();
        }

        public T GetSceneData<T>() where T : ISceneData
        {
            if (_instance == null)
            {
                Debug.LogWarning("LevelLoader instance is null. Make sure it's active in the scene.");
                return default(T);
            }

            return _instance._sceneData != null ? (T)_instance._sceneData : default(T);
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("Instance for LevelLoader already exist!");
                Destroy(gameObject);
            }
        }

        public void LoadScene<T>(string sceneName, T data, int transitionIndex = -1) where T : ISceneData
        {
            _sceneData = data;
            LoadScene(sceneName, transitionIndex, transitionIndex);
        }

        public void LoadScene(string sceneName, int transitionIndex = -1)
        {
            LoadScene(sceneName, transitionIndex, transitionIndex);
        }

        public void LoadScene<T>(string sceneName, T data, int inTransitionIndex, int outTransitionIndex) where T : ISceneData
        {
            _sceneData = data;
            LoadScene(sceneName, inTransitionIndex, outTransitionIndex);
        }

        public void LoadScene(string sceneName, int inTransitionIndex, int outTransitionIndex)
        {
            StartCoroutine(LoadSceneWithAnimation(sceneName, inTransitionIndex, outTransitionIndex));
        }

        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int transitionIndex = -1)
        {
            int[] indexes = new int[4] { transitionIndex, transitionIndex, transitionIndex, transitionIndex };
            StartCoroutine(LoadSceneWithLoadScreenCoroutine(targetSceneName, loadSceneName, indexes));
        }
        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int inTransitionIndex, int outTransitionIndex)
        {
            int[] indexes = new int[4] { inTransitionIndex, outTransitionIndex, outTransitionIndex, inTransitionIndex };
            StartCoroutine(LoadSceneWithLoadScreenCoroutine(targetSceneName, loadSceneName, indexes));
        }

        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int firstIndex, int secondIndex, int thirdIndex, int fourthIndex)
        {
            int[] indexes = new int[4] { firstIndex, secondIndex, thirdIndex, fourthIndex };
            StartCoroutine(LoadSceneWithLoadScreenCoroutine(targetSceneName, loadSceneName, indexes));
        }


        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int transitionIndex = -1) where T : ISceneData
        {
            _sceneData = data;
            LoadSceneWithLoadScreen(targetSceneName, loadSceneName, transitionIndex);
        }


        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int inTransitionIndex, int outTransitionIndex) where T : ISceneData
        {
            _sceneData = data;
            LoadSceneWithLoadScreen(targetSceneName, loadSceneName, inTransitionIndex, outTransitionIndex);
        }
        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int firstIndex, int secondIndex, int thirdIndex, int fourthIndex) where T : ISceneData
        {
            _sceneData = data;
            LoadSceneWithLoadScreen(targetSceneName, loadSceneName, firstIndex, secondIndex, thirdIndex, fourthIndex);
        }

        private IEnumerator LoadSceneWithLoadScreenCoroutine(string targetSceneName, string loadSceneName, int[] transitionIndex = null)
        {
            yield return StartCoroutine(LoadSceneWithAnimation(loadSceneName, transitionIndex[0], transitionIndex[1]));

            yield return StartCoroutine(LoadSceneWithAnimation(targetSceneName, transitionIndex[2], transitionIndex[3]));
        }

        private IEnumerator LoadScene(string sceneName)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            while (!loadOperation.isDone)
            {
                yield return null;
            }
            sceneLoaded?.Invoke(true);
        }


        private IEnumerator LoadSceneWithAnimation(string sceneName, int inMaskIndex, int outMaskIndex)
        {
            if (inMaskIndex >= 0)
            {
                _animator.PlayTransistAnimation(inMaskIndex, out float animationDuration);
                yield return new WaitForSeconds(animationDuration);
            }

            yield return StartCoroutine(LoadScene(sceneName));
            if (inMaskIndex >= 0)
            {
                _animator.PlayTransistAnimationReverse(outMaskIndex, out float animationDuration);
                yield return new WaitForSeconds(animationDuration);
            }

            sceneLoaded?.Invoke(true);
        }
    }

}
