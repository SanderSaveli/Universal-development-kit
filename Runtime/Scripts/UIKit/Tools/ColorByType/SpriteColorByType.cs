using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColorByType : ColorByType
    {
        [SerializeField] private SpriteRenderer _sprite;

        private void Reset()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        protected override void OnDestroy()
        {
            DOTween.Kill(_sprite);
            base.OnDestroy();
        }

        protected override void ApplyColorWithAnimation(Color color, float duration)
        {
            _sprite.DOColor(color, duration).SetLink(gameObject);
        }

        protected override void ApplyColor(Color color) => _sprite.color = color;
        protected override float GetCurrentColorAlpha() => _sprite.color.a;
    }
}
