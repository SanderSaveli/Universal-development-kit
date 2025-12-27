using System.Collections.Generic;
using UnityEngine;

namespace PustoGames.Samurai
{
    public class RaycastTool : MonoBehaviour, IRaycastTool
    {
        private Camera _camera;
        private void Awake()
        {
            _camera = Camera.main;
        }

        public bool TryGetComponentByRaycast<T>(Vector2 screenPos, LayerMask layerMask, out T component) where T : MonoBehaviour
        {
            component = null;
            Ray ray = _camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out component))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetComponentByRaycast<T>(Vector2 screenPos,out T component) where T : MonoBehaviour
        {
            return TryGetComponentByRaycast(screenPos, ~0, out component);
        }

        public bool TryRaycastAll<T>(Vector2 screenPos, out List<T> components) where T : MonoBehaviour
        {
            components = new List<T>();
            Ray ray = _camera.ScreenPointToRay(screenPos);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach(var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out T component))
                {
                    components.Add(component);
                }
            }
            return components.Count > 0;
        }
    }
}
