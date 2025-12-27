using System.Collections.Generic;
using UnityEngine;

namespace PustoGames.Samurai
{
    public interface IRaycastTool
    {
        public bool TryGetComponentByRaycast<T>(Vector2 screenPos, LayerMask layerMask, out T component) where T : MonoBehaviour;
        public bool TryGetComponentByRaycast<T>(Vector2 screenPos, out T component) where T : MonoBehaviour;
        public bool TryRaycastAll<T>(Vector2 screenPos, out List<T> components) where T : MonoBehaviour;
    }
}
