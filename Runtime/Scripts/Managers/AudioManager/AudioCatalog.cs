using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [CreateAssetMenu(menuName = "UDK/Audio/Sound Catalog", fileName = "Sound Catalog")]
    public class AudioCatalog : ScriptableObject
    {
        public IReadOnlyList<SoundConfig> Sounds => _sounds;

        [SerializeField] private List<SoundConfig> _sounds = new List<SoundConfig>();
        private List<SoundConfig> _chache = new();

        private void OnValidate()
        {
            _chache.Clear();
        }

        public bool IsValid()
        {
            if (_sounds == null)
            {
                return false;
            }

            HashSet<string> ids = new HashSet<string>(System.StringComparer.Ordinal);

            for (int i = 0; i < _sounds.Count; i++)
            {
                SoundConfig sound = _sounds[i];

                if (sound == null || string.IsNullOrEmpty(sound.Id) || !ids.Add(sound.Id)
                    || sound.Clips == null || sound.Clips.Count == 0)
                {
                    return false;
                }

                for (int j = 0; j < sound.Clips.Count; j++)
                {
                    if (sound.Clips[j] == null || sound.Clips[j].Clip == null)
                    {
                        return false;
                    }
                }
            }

            _chache.Clear();
            return true;
        }

        public bool TryGetSound(string id, out SoundConfig soundConfig)
        {
            soundConfig = _chache.FirstOrDefault(s => s.Id == id);
            
            if(soundConfig == null)
            {
                soundConfig = _sounds.FirstOrDefault(s => s.Id == id);

                if(soundConfig != null)
                {
                    _chache.Add(soundConfig);
                }
            }

            return soundConfig != null;
        }
    }
}
