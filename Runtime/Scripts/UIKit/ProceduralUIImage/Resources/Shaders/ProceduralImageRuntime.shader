Shader "UI/Procedural UI Image"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _UseGradient ("Use Gradient", Float) = 0
        _GradientType ("Gradient Type", Float) = 0
        _StopCount ("Stops Count", Float) = 2
        _GradientCurveTex ("Gradient Curve Texture", 2D) = "white" {}

        _PolygonRotation ("Polygon Rotation (\u00b0)", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            int _UseGradient;
            int _GradientType;
            int _StopCount;
            float _StopPositions[8];
            float4 _StopColors[8];
            sampler2D _GradientCurveTex;

            int _PolygonPointCount;
            float2 _unusedA;
            float4 _PolygonPoints[12];
            float _PolygonPointRadii[12];
            float _PolygonRotation;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 worldPos : TEXCOORD0;
                float4 radius : TEXCOORD1;
                float2 texcoord : TEXCOORD2;
                float2 wh : TEXCOORD3;
                float lineWeight : TEXCOORD4;
                float pixelWorldScale : TEXCOORD5;
                float2 gradUV : TEXCOORD6;
                float2 localPos : TEXCOORD7;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float2 decode2(float v)
            {
                float2 m = float2(1.0, 65535.0);
                float2 enc = frac(m * v);
                enc.x -= enc.y / 65535.0;
                return enc;
            }

            float2 rotatePoint(float2 pt, float2 center, float cosR, float sinR)
            {
                pt -= center;
                pt = float2(pt.x * cosR - pt.y * sinR, pt.x * sinR + pt.y * cosR);
                return pt + center;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPos = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPos);

                OUT.wh = IN.uv1;
                OUT.texcoord = TRANSFORM_TEX(IN.uv0, _MainTex);
                OUT.gradUV = IN.uv0;
                OUT.localPos = IN.uv0 * IN.uv1;

                float minside = min(OUT.wh.x, OUT.wh.y);
                OUT.lineWeight = IN.uv3.x * minside * 0.5;
                OUT.radius = float4(decode2(IN.uv2.x), decode2(IN.uv2.y)) * minside;
                OUT.pixelWorldScale = clamp(IN.uv3.y, 1 / 2048.0, 2048.0);

                OUT.color = IN.color * _Color;
                return OUT;
            }

            float4 computeGradient(float2 uv)
            {
                float t = (_GradientType == 0) ? uv.y : (_GradientType == 1) ? uv.x : (_GradientType == 2) ? length(uv - 0.5) / 0.7071 : (atan2(uv.y - 0.5, uv.x - 0.5) + UNITY_PI) / (2 * UNITY_PI);
                t = saturate(tex2D(_GradientCurveTex, float2(t, 0.5)).r);
                for (int i = 0; i < _StopCount - 1; ++i)
                {
                    float p0 = _StopPositions[i], p1 = _StopPositions[i + 1];
                    if (t >= p0 && t <= p1)
                        return lerp(_StopColors[i], _StopColors[i + 1], (t - p0) / (p1 - p0));
                }
                return _StopColors[0];
            }

            half visibleRoundedRect(float2 pos, float4 r, float2 wh)
            {
                half4 p = half4(pos, wh.x - pos.x, wh.y - pos.y);
                half v = min(min(min(p.x, p.y), p.z), p.w);
                bool4 b = bool4(all(p.xw < r[0]), all(p.zw < r[1]), all(p.zy < r[2]), all(p.xy < r[3]));
                half4 vis = r - half4(length(p.xw - r[0]), length(p.zw - r[1]), length(p.zy - r[2]), length(p.xy - r[3]));
                half4 foo = min(b * max(vis, 0), v) + (1 - b) * v;
                return any(b) ? min(min(min(foo.x, foo.y), foo.z), foo.w) : v;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // базовые текстура/клиппинг/градиент
                half4 col = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                #ifdef UNITY_UI_CLIP_RECT
        col.a *= UnityGet2DClipping(IN.worldPos.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
        clip(col.a - 0.001);
                #endif
                if (_UseGradient > 0)
                    col *= computeGradient(IN.gradUV);

                float alphaMask = 1.0;

                // если у нас есть полигон
                if (_PolygonPointCount > 2)
                {
                    float2 wh = IN.wh;
                    float2 center = wh * 0.5;

                    // сразу поворачиваем локальную позицию вокруг центра
                    float rad = radians(_PolygonRotation);
                    float cosR = cos(rad), sinR = sin(rad);
                    float2 rotatedPos = rotatePoint(IN.localPos, center, cosR, sinR);
                    float2 p = rotatedPos - center;

                    // 1) Ray-casting для fill
                    bool inside = false;
                    for (int i = 0, j = _PolygonPointCount - 1; i < _PolygonPointCount; j = i++)
                    {
                        float2 A = _PolygonPoints[i].xy * wh;
                        float2 B = _PolygonPoints[j].xy * wh;
                        if (((A.y > p.y) != (B.y > p.y)) &&
                            (p.x < (B.x - A.x) * (p.y - A.y) / (B.y - A.y) + A.x))
                        {
                            inside = !inside;
                        }
                    }

                    // 2) SDF-stroke
                    float D = 1e6;
                    for (int i = 0, j = _PolygonPointCount - 1; i < _PolygonPointCount; j = i++)
                    {
                        float2 A = _PolygonPoints[i].xy * wh;
                        float2 B = _PolygonPoints[j].xy * wh;
                        float2 edge = B - A;
                        float t = saturate(dot(p - A, edge) / dot(edge, edge));
                        float2 H = A + edge * t;
                        float rawR = min(_PolygonPointRadii[i], _PolygonPointRadii[j]);
                        float r = min(rawR, length(edge) * 0.5);
                        float dist = length(p - H) - r;
                        D = min(D, dist);
                    }

                    float edgeW = 1.0 / IN.pixelWorldScale;
                    float strokeMask = saturate((-D + edgeW) * IN.pixelWorldScale);
                    float fillMask = inside ? 1.0 : 0.0;
                    alphaMask = max(fillMask, strokeMask);
                }
                else
                {
                    // если не полигон — твой старый код (rounded rect)
                    float v = visibleRoundedRect(IN.gradUV * IN.wh, IN.radius, IN.wh);
                    float edgeW = (IN.lineWeight + 1.0 / IN.pixelWorldScale) * 0.5;
                    alphaMask = saturate((edgeW - distance(v, edgeW)) * IN.pixelWorldScale);
                }

                col.a *= alphaMask;
                if (col.a <= 0) discard;
                return col;
            }
            ENDCG
        }
    }
}