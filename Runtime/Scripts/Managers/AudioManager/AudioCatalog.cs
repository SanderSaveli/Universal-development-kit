using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [CreateAssetMenu(menuName = "UDK/Audio/Sound Catalog", fileName = "Sound Catalog")]
    public class AudioCatalog : ScriptableObject
    {
        public IReadOnlyList<SoundConfig> Sounds => _sounds;

        [SerializeField] private List<SoundConfig> _sounds = new List<SoundConfig>();
        private readonly Dictionary<string, SoundConfig> _soundsById =
            new Dictionary<string, SoundConfig>(System.StringComparer.Ordinal);
        private bool _indexBuilt;

        private void OnValidate()
        {
            _soundsById.Clear();
            _indexBuilt = false;
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

            RebuildIndex();
            return true;
        }

        public bool TryGetSound(string id, out SoundConfig soundConfig)
        {
            if (!_indexBuilt)
            {
                RebuildIndex();
            }

            soundConfig = null;
            return id != null && _soundsById.TryGetValue(id, out soundConfig);
        }

        private void RebuildIndex()
        {
            _soundsById.Clear();

            for (int i = 0; i < _sounds.Count; i++)
            {
                SoundConfig sound = _sounds[i];

                if (sound != null && !string.IsNullOrEmpty(sound.Id))
                {
                    _soundsById.TryAdd(sound.Id, sound);
                }
            }

            _indexBuilt = true;
        }
    }
}
