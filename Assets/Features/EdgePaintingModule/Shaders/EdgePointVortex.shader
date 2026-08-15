Shader "EdgeGame/EdgePointVortex" {
    Properties {
        [Header(Colors)]
        _CoreColor ("Core Color", Color) = (0.6, 0.9, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.2, 0.4, 1, 1)
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)

        [Header(Outline)]
        _OutlineWidth ("Outline Width (pixels)", Range(1, 8)) = 1

        [Header(Vortex)]
        _SwirlSpeed ("Swirl Speed", Float) = 2
        _Twist ("Twist By Radius", Float) = 4
        _FlowSpeed ("Inward Flow Speed", Float) = 1

        [Header(Noise)]
        _NoiseScale ("Noise Scale", Float) = 6
        _NoiseContrast ("Noise Contrast", Range(0.1, 8)) = 2
        _NoiseFloor ("Noise Floor", Range(0, 1)) = 0.25
        _EdgeNoise ("Edge Noise", Range(0, 1)) = 0.25

        [Header(Fill Noise)]
        _FillNoiseScale ("Fill Noise Scale", Float) = 12
        _FillNoiseStrength ("Fill Noise Strength", Range(0, 1)) = 0.6

        [Header(Shape)]
        _Pixels ("Pixels Across", Float) = 24
        _InnerRadius ("Inner Radius", Range(0, 0.9)) = 0
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.1
    }

    SubShader {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }

        Pass {
            Name "ForwardUnlit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Lighting Off

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _CoreColor;
                float4 _GlowColor;
                float4 _OutlineColor;
                float _OutlineWidth;
                float _SwirlSpeed;
                float _Twist;
                float _FlowSpeed;
                float _NoiseScale;
                float _NoiseContrast;
                float _NoiseFloor;
                float _EdgeNoise;
                float _FillNoiseScale;
                float _FillNoiseStrength;
                float _Pixels;
                float _InnerRadius;
                float _AlphaCutoff;
            CBUFFER_END

            float Hash(float2 position) {
                return frac(sin(dot(position, float2(127.1, 311.7))) * 43758.5453123);
            }

            float ValueNoise(float2 position) {
                float2 cell = floor(position);
                float2 local = frac(position);
                float2 blend = local * local * (3.0 - 2.0 * local);

                float bottomLeft = Hash(cell);
                float bottomRight = Hash(cell + float2(1, 0));
                float topLeft = Hash(cell + float2(0, 1));
                float topRight = Hash(cell + float2(1, 1));

                return lerp(lerp(bottomLeft, bottomRight, blend.x),
                            lerp(topLeft, topRight, blend.x), blend.y);
            }

            float Fbm(float2 position) {
                float value = 0;
                float amplitude = 0.5;

                for (int i = 0; i < 3; i++) {
                    value += ValueNoise(position) * amplitude;
                    position *= 2.0;
                    amplitude *= 0.5;
                }

                return value;
            }

            // Rotating the sample point instead of using the polar angle directly
            // keeps the swirl seamless: there is no 0 / 2PI cut across the disc.
            float2 Swirl(float2 position, float radius) {
                float angle = _Time.y * _SwirlSpeed + radius * _Twist;
                float sinAngle = sin(angle);
                float cosAngle = cos(angle);

                return float2(position.x * cosAngle - position.y * sinAngle,
                              position.x * sinAngle + position.y * cosAngle);
            }

            Varyings Vertex(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            float4 Fragment(Varyings input) : SV_Target {
                float pixels = max(_Pixels, 1);
                float2 uv = (floor(input.uv * pixels) + 0.5) / pixels;

                float2 centered = (uv - 0.5) * 2.0;
                float radius = length(centered);

                float2 swirled = Swirl(centered, radius);
                float noise = Fbm(swirled * _NoiseScale - float2(0, _Time.y * _FlowSpeed));
                noise = saturate(pow(abs(noise), _NoiseContrast));
                noise = lerp(_NoiseFloor, 1.0, noise);

                // Border and outline live on the same pixel grid, so the ring keeps
                // a constant thickness no matter where the noise pushes the edge.
                float rowSize = 2.0 / pixels;
                float noisyRadius = 1.0 - _EdgeNoise * (1.0 - noise);
                float border = floor(noisyRadius / rowSize) * rowSize;
                float outlineThickness = max(round(_OutlineWidth), 1) * rowSize;

                float body = step(radius, border) * step(_InnerRadius, radius);
                float outerRing = step(border - outlineThickness, radius);
                float innerRing = _InnerRadius <= 0 ? 0 : step(radius, _InnerRadius + outlineThickness);
                float outline = max(outerRing, innerRing) * body;

                float2 fillSwirled = Swirl(centered, radius * 1.7);
                float fillNoise = Fbm(fillSwirled * _FillNoiseScale + 17.3);
                float fill = lerp(1.0, fillNoise, _FillNoiseStrength);

                float4 color = lerp(_GlowColor, _CoreColor, saturate(noise * fill * 2.0));
                color = lerp(color, _OutlineColor, outline);
                color *= input.color;
                color.a *= body * max(outline, fill);

                clip(color.a - _AlphaCutoff);

                return color;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
