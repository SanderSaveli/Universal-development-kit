using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    public static class SoundGain
    {
        public const float MIN_GAIN_DB = -20f;
        public const float MAX_GAIN_DB = 12f;
        public const string MIXER_GROUP_PATH = "Master/Sounds/SFX Gain";

        public static float GetLinearGain(float volume, float gainDb)
        {
            float multiplier = Mathf.Pow(10f, Mathf.Clamp(gainDb, MIN_GAIN_DB, MAX_GAIN_DB) / 20f);
            return Mathf.Clamp01(volume) * multiplier;
        }

        public static float GetSourceVolume(float volume, float gainDb)
        {
            return GetLinearGain(volume, gainDb) / Mathf.Pow(10f, MAX_GAIN_DB / 20f);
        }
    }
}
