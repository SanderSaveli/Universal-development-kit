using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralUIImage.Scripts
{
    [ModifierID("Arbitrary Polygon")]
    [DisallowMultipleComponent]
    public class ArbitraryPolygonModifier : ProceduralImageModifier
    {
        [SerializeField, Range(0f, 360f)] private float rotation;

        public float Rotation
        {
            get => rotation;
            set => rotation = Mathf.Repeat(value, 360f); // гарантируем 0–360
        }

        [Serializable]
        public struct Point
        {
            [Tooltip("Position of the vertex in percent [0..100] within the rectangle")]
            public Vector2 uvPercent;

            [Tooltip("Corner rounding radius in pixels")]
            public float cornerRadiusPx;
        }

        [Header("Polygon Points (in % UV)")]
        [SerializeField]
        private List<Point> _points = new List<Point>
        {
            new Point { uvPercent = new Vector2(10, 10), cornerRadiusPx = 0 },
            new Point { uvPercent = new Vector2(20, 10), cornerRadiusPx = 0 },
            new Point { uvPercent = new Vector2(20, 20), cornerRadiusPx = 0 },
            new Point { uvPercent = new Vector2(10, 20), cornerRadiusPx = 0 },
        };

        public int PointCount => _points.Count;
        public IReadOnlyList<Point> Points => _points;

        private void OnValidate()
        {
            if (_points == null)
                _points = new List<Point>();

            // Ensure 3..12 points
            while (_points.Count < 3)
                _points.Add(new Point { uvPercent = Vector2.zero, cornerRadiusPx = 0f });
            if (_points.Count > 12)
                _points.RemoveRange(12, _points.Count - 12);

            // Clamp values
            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                pt.uvPercent.x = Mathf.Clamp(pt.uvPercent.x, -100f, 100f);
                pt.uvPercent.y = Mathf.Clamp(pt.uvPercent.y, -100f, 100f);
                pt.cornerRadiusPx = Mathf.Max(0f, pt.cornerRadiusPx);
                _points[i] = pt;
            }

            // Check winding: compute signed area in normalized UV
            float signedArea = 0f;
            for (int i = 0; i < _points.Count; ++i)
            {
                Vector2 a = _points[i].uvPercent * 0.01f;
                Vector2 b = _points[(i + 1) % _points.Count].uvPercent * 0.01f;
                signedArea += (b.x - a.x) * (b.y + a.y);
            }

            if (Mathf.Abs(signedArea) < 1e-6f)
            {
                Debug.LogWarning("ArbitraryPolygonModifier: Polygon points are colinear or degenerate.", this);
            }
            else if (signedArea > 0f)
            {
                Debug.LogWarning("ArbitraryPolygonModifier: Polygon points are in clockwise order; expected counter-clockwise.", this);
            }

            _Graphic.SetVerticesDirty();
        }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            // disable default rounded-rect
            return Vector4.zero;
        }
    }
}