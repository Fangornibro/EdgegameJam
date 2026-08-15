Shader "EdgeGame/EdgeParticleMasked" {
    Properties {
        [PerRendererData] _MainTex ("Particle Texture", 2D) = "white" {}
        _TintColor ("Tint", Color) = (1, 1, 1, 1)
        _EdgeFeather ("Edge Feather (world units)", Range(0, 1)) = 0.25
        _BoundsFeather ("Bounds Feather (world units)", Range(0, 2)) = 0.5
        [Toggle] _InvertZone ("Show Outside The Zone", Float) = 0
        [Toggle] _UseLuminanceAsAlpha ("Use Luminance As Alpha", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5   // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 1   // One
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
            Blend [_SrcBlend] [_DstBlend]
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

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _TintColor;
                float _EdgeFeather;
                float _BoundsFeather;
                float _InvertZone;
                float _UseLuminanceAsAlpha;
            CBUFFER_END

            // Shared with TilemapEdgeSwap, written by EdgeShaderUniforms.
            float4 _Edges[MAX_EDGES];
            int _EdgeCount;
            int _EdgeCombineMode;
            // xy = min corner, zw = max corner of the area particles may live in.
            float4 _EdgeMaskRect;

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
                    float halfPlane = smoothstep(0, -feather, SignedDistance(_Edges[i], positionWS));
                    mask = isIntersection ? min(mask, halfPlane) : max(mask, halfPlane);
                }

                return mask;
            }

            // Fades the particles out before they leave the playable area instead of letting
            // them pop over the black borders.
            float BoundsMask(float2 positionWS) {
                if (_EdgeMaskRect.z <= _EdgeMaskRect.x || _EdgeMaskRect.w <= _EdgeMaskRect.y)
                    return 1;

                float feather = max(_BoundsFeather, 1e-5);
                float2 fromMin = smoothstep(0, feather, positionWS - _EdgeMaskRect.xy);
                float2 fromMax = smoothstep(0, feather, _EdgeMaskRect.zw - positionWS);

                return fromMin.x * fromMin.y * fromMax.x * fromMax.y;
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
                float zoneMask = RightSideMask(input.positionWS);

                if (_InvertZone > 0.5)
                    zoneMask = 1 - zoneMask;

                float mask = zoneMask * BoundsMask(input.positionWS);

                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _TintColor * input.color;

                // Additive particle textures often have a fully opaque black background, which
                // would paint black instead of disappearing. Their brightness is the real alpha.
                if (_UseLuminanceAsAlpha > 0.5)
                    color.a *= dot(color.rgb, float3(0.299, 0.587, 0.114));

                color.a *= mask;
                color.rgb *= mask;

                clip(color.a - 0.001);

                return color;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
