Shader "EdgeGame/TilemapEdgeSwap" {
    Properties {
        [PerRendererData] _MainTex ("Tilesheet", 2D) = "white" {}
        _AltTex ("Alternate Tilesheet", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _AltTint ("Alternate Tint", Color) = (1, 1, 1, 1)
        _EdgeFeather ("Edge Feather (world units)", Range(0, 0.5)) = 0
    }

    SubShader {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
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

            #define MAX_EDGES 16

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 positionWS : TEXCOORD1;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_AltTex);
            SAMPLER(sampler_AltTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _AltTint;
                float _EdgeFeather;
            CBUFFER_END

            // Filled from EdgeShaderUniforms: xy = edge start, zw = edge direction.
            float4 _Edges[MAX_EDGES];
            int _EdgeCount;
            // 0 = union of the right half planes, 1 = their intersection.
            int _EdgeCombineMode;

            // Positive on the left of the edge, negative on the right.
            float SignedDistance(float4 edge, float2 position) {
                float2 direction = edge.zw;
                float edgeLength = max(length(direction), 1e-5);
                float2 toPosition = position - edge.xy;

                return (direction.x * toPosition.y - direction.y * toPosition.x) / edgeLength;
            }

            float RightSideMask(float2 positionWS) {
                if (_EdgeCount <= 0)
                    return 0;

                float feather = max(_EdgeFeather, 1e-5);
                bool isIntersection = _EdgeCombineMode != 0;
                float mask = isIntersection ? 1 : 0;

                for (int i = 0; i < _EdgeCount; i++) {
                    float distance = SignedDistance(_Edges[i], positionWS);
                    float halfPlane = smoothstep(0, -feather, distance);

                    mask = isIntersection ? min(mask, halfPlane) : max(mask, halfPlane);
                }

                return mask;
            }

            Varyings Vertex(Attributes input) {
                Varyings output;
                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = positions.positionCS;
                output.positionWS = positions.positionWS.xy;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;

                return output;
            }

            float4 Fragment(Varyings input) : SV_Target {
                // Both sheets share the same layout, so the same UV lands on the
                // matching tile in the alternate sheet.
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                float4 altColor = SAMPLE_TEXTURE2D(_AltTex, sampler_AltTex, input.uv) * _AltTint;

                float mask = RightSideMask(input.positionWS);
                float4 color = lerp(baseColor, altColor, mask);

                return color * input.color;
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/2D/Sprite-Unlit-Default"
}
