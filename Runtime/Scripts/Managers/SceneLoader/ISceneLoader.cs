using Cysharp.Threading.Tasks;
using System;

namespace SanderSaveli.UDK.Managers
{
    public interface ISceneLoader
    {
        public bool IsLoading { get; }
        public float LoadingProgress { get; }
        public SceneName CurrentTargetScene { get; }
        public Action<float> OnProgressChange { get; set; }
        public Action<SceneName> OnTargetSceneChange { get; set; }

        public UniTask LoadScene(SceneName sceneName);
        public UniTask ReloadCurrentScene();
    }
}
