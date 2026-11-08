using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SanderSaveli.UDK.Audio.Editor
{
    internal sealed class SoundKeyBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!SoundKeyGenerator.ValidateCatalogs(out string error))
                throw new BuildFailedException("Audio configuration validation failed:\n" + error);
        }
    }
}
