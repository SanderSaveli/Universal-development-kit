using UnityEditor;

namespace SanderSaveli.UDK.Audio.Editor
{
    internal sealed class SoundKeyAssetProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (ContainsAsset(importedAssets) || ContainsAsset(deletedAssets)
                || ContainsAsset(movedAssets) || ContainsAsset(movedFromAssetPaths))
                SoundKeyGenerator.QueueValidation();
        }

        private static bool ContainsAsset(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".asset", System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
