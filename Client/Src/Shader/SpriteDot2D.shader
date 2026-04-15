Shader "Custom/SpriteDot2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // === 도트(픽셀화) 설정 ===
        [Header(Pixelation)]
        _PixelSize ("Pixel Size", Range(1, 64)) = 8
        _PixelateOn ("Enable Pixelation", Range(0, 1)) = 1

        // === 아웃라인 설정 ===
        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 5)) = 1
        _OutlineOn ("Enable Outline", Range(0, 1)) = 0

        // === 팔레트 제한 (색 단계) ===
        [Header(Color Palette)]
        _ColorSteps ("Color Steps (2-256)", Range(2, 256)) = 8
        _PaletteOn ("Enable Palette Limit", Range(0, 1)) = 0

        // === 디더링 ===
        [Header(Dithering)]
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.5
        _DitherOn ("Enable Dithering", Range(0, 1)) = 0

        // === 그림자/하이라이트 ===
        [Header(Shadow and Highlight)]
        _ShadowColor ("Shadow Color", Color) = (0.2, 0.1, 0.3, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0.8, 1)
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.3
        _HighlightThreshold ("Highlight Threshold", Range(0, 1)) = 0.7
        _CelShadingOn ("Enable Cel Shading", Range(0, 1)) = 0

        // === 스프라이트 기본 ===
        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)

        // Stencil / Masking
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            // ─────────────────────────────────────
            // Structs
            // ─────────────────────────────────────
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ─────────────────────────────────────
            // Properties
            // ─────────────────────────────────────
            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // (1/w, 1/h, w, h)
            fixed4 _Color;
            fixed4 _RendererColor;
            float4 _Flip;

            float _PixelSize;
            float _PixelateOn;

            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineOn;

            float _ColorSteps;
            float _PaletteOn;

            float _DitherStrength;
            float _DitherOn;

            fixed4 _ShadowColor;
            fixed4 _HighlightColor;
            float _ShadowThreshold;
            float _HighlightThreshold;
            float _CelShadingOn;

            // ─────────────────────────────────────
            // Vertex Shader
            // ─────────────────────────────────────
            v2f SpriteVert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif

                return o;
            }

            // ─────────────────────────────────────
            // Helper: Pixelate UV
            // ─────────────────────────────────────
            float2 PixelateUV(float2 uv, float pixelSize)
            {
                float2 texSize = _MainTex_TexelSize.zw; // 텍스처 원본 크기
                float2 pixelCount = texSize / pixelSize;
                float2 snapped = floor(uv * pixelCount) / pixelCount;
                // 픽셀 중앙 보정
                snapped += 0.5 / pixelCount;
                return snapped;
            }

            // ─────────────────────────────────────
            // Helper: 4x4 Bayer Dither Matrix
            // ─────────────────────────────────────
            float BayerDither4x4(float2 pos)
            {
                // 4x4 Bayer matrix (normalized 0~1)
                const float dither[16] = {
                     0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                    12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
                     3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                    15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
                };
                int x = int(fmod(pos.x, 4.0));
                int y = int(fmod(pos.y, 4.0));
                return dither[y * 4 + x];
            }

            // ─────────────────────────────────────
            // Helper: Quantize Color (팔레트 제한)
            // ─────────────────────────────────────
            fixed3 QuantizeColor(fixed3 col, float steps)
            {
                return floor(col * (steps - 1.0) + 0.5) / (steps - 1.0);
            }

            // ─────────────────────────────────────
            // Helper: Luminance
            // ─────────────────────────────────────
            float Luminance01(fixed3 col)
            {
                return dot(col, float3(0.299, 0.587, 0.114));
            }

            // ─────────────────────────────────────
            // Fragment Shader
            // ─────────────────────────────────────
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord;

                // ── 1) 픽셀화 ──
                float2 pixUV = uv;
                if (_PixelateOn > 0.5)
                {
                    pixUV = PixelateUV(uv, _PixelSize);
                }

                fixed4 col = tex2D(_MainTex, pixUV);
                col *= i.color;

                // ── 2) 아웃라인 ──
                if (_OutlineOn > 0.5 && col.a > 0.01)
                {
                    float2 offset = _MainTex_TexelSize.xy * _OutlineThickness;

                    // 픽셀화 적용된 UV 기준으로 상하좌우 샘플
                    float2 uvUp    = pixUV + float2(0, offset.y);
                    float2 uvDown  = pixUV - float2(0, offset.y);
                    float2 uvLeft  = pixUV - float2(offset.x, 0);
                    float2 uvRight = pixUV + float2(offset.x, 0);

                    float aU = tex2D(_MainTex, uvUp).a;
                    float aD = tex2D(_MainTex, uvDown).a;
                    float aL = tex2D(_MainTex, uvLeft).a;
                    float aR = tex2D(_MainTex, uvRight).a;

                    // 가장자리 감지: 이웃 중 하나라도 투명이면 아웃라인
                    float edge = step(0.01, col.a) * (1.0 - step(0.01, aU * aD * aL * aR));

                    // 외곽 아웃라인: 현재 픽셀이 투명이고 이웃이 불투명
                    float outerA = tex2D(_MainTex, pixUV).a;
                    if (outerA < 0.01)
                    {
                        float neighborAlpha = max(max(aU, aD), max(aL, aR));
                        if (neighborAlpha > 0.5)
                        {
                            col = _OutlineColor;
                            col.rgb *= col.a;
                            return col;
                        }
                    }

                    col.rgb = lerp(col.rgb, _OutlineColor.rgb, edge * _OutlineColor.a);
                }

                // ── 3) 셀 셰이딩 (간이) ──
                if (_CelShadingOn > 0.5)
                {
                    float lum = Luminance01(col.rgb);

                    if (lum < _ShadowThreshold)
                    {
                        col.rgb = lerp(col.rgb, _ShadowColor.rgb, _ShadowColor.a);
                    }
                    else if (lum > _HighlightThreshold)
                    {
                        col.rgb = lerp(col.rgb, _HighlightColor.rgb, _HighlightColor.a);
                    }
                }

                // ── 4) 디더링 ──
                if (_DitherOn > 0.5)
                {
                    // 화면 좌표 기준 디더링
                    float2 screenPos = i.vertex.xy;
                    float bayerVal = BayerDither4x4(screenPos);
                    float ditherOffset = (bayerVal - 0.5) * _DitherStrength;

                    col.rgb += ditherOffset;
                    col.rgb = saturate(col.rgb);
                }

                // ── 5) 팔레트 제한 ──
                if (_PaletteOn > 0.5)
                {
                    col.rgb = QuantizeColor(col.rgb, _ColorSteps);
                }

                // ── 최종 출력 (Premultiplied Alpha) ──
                col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
