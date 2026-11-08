using UnityEditor;

namespace SanderSaveli.UDK.Audio.Editor
{
    internal sealed class SoundKeySaveProcessor : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".asset", System.StringComparison.OrdinalIgnoreCase))
                {
                    SoundKeyGenerator.QueueValidation();
                    break;
                }
            }

            return paths;
        }
    }
}
