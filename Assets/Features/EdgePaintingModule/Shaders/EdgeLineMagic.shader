Shader "EdgeGame/EdgeLineMagic" {
    Properties {
        [Header(Colors)]
        _CoreColor ("Core Color", Color) = (0.6, 0.9, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.2, 0.4, 1, 1)
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)

        [Header(Outline)]
        _OutlineWidth ("Outline Width (pixels)", Range(1, 8)) = 1

        [Header(Noise)]
        _NoiseScale ("Noise Scale", Vector) = (1, 2, 0, 0)
        _NoiseSpeed ("Noise Speed", Vector) = (-1.5, 0.2, 0, 0)
        _NoiseContrast ("Noise Contrast", Range(0.1, 8)) = 2
        _NoiseFloor ("Noise Floor", Range(0, 1)) = 0.25

        [Header(Fill Noise)]
        _FillNoiseScale ("Fill Noise Scale", Vector) = (2, 5, 0, 0)
        _FillNoiseSpeed ("Fill Noise Speed", Vector) = (-2.5, 0.6, 0, 0)
        _FillNoiseStrength ("Fill Noise Strength", Range(0, 1)) = 0.6

        [Header(Pixelation)]
        _PixelsAcross ("Pixels Across Line", Float) = 8
        // 0 = pattern stretches over the whole line, 1 = pattern tiles with square pixels.
        _AspectInfluence ("Aspect Influence", Range(0, 1)) = 1
        _AspectScale ("Aspect Scale", Range(0.05, 4)) = 1
        // Length / width of the line, written by EdgeLineAspect.
        _LineAspect ("Line Aspect", Float) = 1

        [Header(Shape)]
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.1
        _EndFade ("End Fade", Range(0, 0.5)) = 0.0
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
                float4 _NoiseScale;
                float4 _NoiseSpeed;
                float _NoiseContrast;
                float _NoiseFloor;
                float4 _FillNoiseScale;
                float4 _FillNoiseSpeed;
                float _FillNoiseStrength;
                float _PixelsAcross;
                float _AspectInfluence;
                float _AspectScale;
                float _LineAspect;
                float _AlphaCutoff;
                float _EndFade;
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

            Varyings Vertex(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            float4 Fragment(Varyings input) : SV_Target {
                // Snap to a virtual pixel grid: the noise is evaluated per pixel block.
                // The horizontal resolution follows the line length, so blocks stay square.
                float aspect = lerp(1.0, max(_LineAspect, 0.0001) * _AspectScale, _AspectInfluence);
                float across = max(_PixelsAcross, 1);
                float2 pixels = float2(max(across * aspect, 1), across);
                float2 uv = (floor(input.uv * pixels) + 0.5) / pixels;

                // Length-proportional coordinate: the pattern tiles instead of stretching.
                float2 tiledUv = float2(uv.x * aspect, uv.y);

                // Distance from the line center across its width: 0 in the middle, 1 at the border.
                float distanceFromCenter = abs(uv.y - 0.5) * 2.0;

                float2 noiseUv = tiledUv * _NoiseScale.xy + _Time.y * _NoiseSpeed.xy;
                float noise = Fbm(noiseUv);
                noise = saturate(pow(abs(noise), _NoiseContrast));
                noise = lerp(_NoiseFloor, 1.0, noise);

                // The noise eats into the line from the sides, so the shape flickers like a magic link.
                // Snapping the border and the outline to the same pixel grid keeps the outline
                // exactly _OutlineWidth pixels thick everywhere instead of wobbling between rows.
                float rowSize = 2.0 / max(_PixelsAcross, 1);
                float border = floor(noise / rowSize) * rowSize;
                float outlineThickness = max(round(_OutlineWidth), 1) * rowSize;

                float body = step(distanceFromCenter, border);
                float outline = step(border - outlineThickness, distanceFromCenter) * body;

                float endFade = _EndFade <= 0
                    ? 1
                    : smoothstep(0, _EndFade, uv.x) * smoothstep(0, _EndFade, 1 - uv.x);

                // Second noise layer that only shimmers inside the line, the outline stays solid.
                float2 fillNoiseUv = tiledUv * _FillNoiseScale.xy + _Time.y * _FillNoiseSpeed.xy;
                float fillNoise = Fbm(fillNoiseUv + 17.3);
                float fill = lerp(1.0, fillNoise, _FillNoiseStrength);

                float4 color = lerp(_GlowColor, _CoreColor, saturate(noise * fill * 2.0));
                color = lerp(color, _OutlineColor, outline);
                color *= input.color;
                color.a *= body * max(outline, fill) * endFade;

                clip(color.a - _AlphaCutoff);

                return color;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
