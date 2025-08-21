using System.Collections.Generic;

namespace SceneLoad
{

    public interface ISceneLoader
    {
        public T GetSceneData<T>() where T : ISceneData;
        public void LoadScene(string sceneName, int transitionIndex = -1);
        public void LoadScene(string sceneName, int inTransitionIndex, int outTransitionIndex);
        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int transitionIndex = -1);
        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int inTransitionIndex, int outTransitionIndex);
        public void LoadSceneWithLoadScreen(string targetSceneName, string loadSceneName, int firstIndex, int secondIndex, int thirdIndex, int fourthIndex);

       
        public void LoadScene<T>(string sceneName, T data, int transitionIndex = -1) where T : ISceneData;
        public void LoadScene<T>(string sceneName, T data, int inTransitionIndex, int outTransitionIndex) where T : ISceneData;
        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int transitionIndex = -1) where T : ISceneData;
        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int inTransitionIndex, int outTransitionIndex) where T : ISceneData;
        public void LoadSceneWithLoadScreen<T>(string targetSceneName, string loadSceneName, T data, int firstIndex, int secondIndex, int thirdIndex, int fourthIndex) where T : ISceneData;

    }
}
