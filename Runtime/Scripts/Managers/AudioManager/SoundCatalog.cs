using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [CreateAssetMenu(menuName = "UDK/Audio/Sound Catalog", fileName = "Sound Catalog")]
    public class SoundCatalog : ScriptableObject
    {
        public IReadOnlyList<SoundConfig> Sounds => _sounds;

        [SerializeField] private List<SoundConfig> _sounds = new List<SoundConfig>();
    }
}
