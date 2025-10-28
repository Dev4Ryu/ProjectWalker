Shader "Custom/URP/2D_SpriteUnlit_Bloom_Safe"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [Toggle(_EMISSION)] _UseEmission("Use Emission", Float) = 0
        [HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
        _EmissionIntensity("Emission Intensity", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _EMISSION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _UseEmission;

            Varyings vert(Attributes input)
            {
                Varyings output;
                // Let Unity handle skinning; don't override vertex positions
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                #ifdef _EMISSION
                texColor.rgb += _EmissionColor.rgb * _EmissionIntensity * step(0.5, _UseEmission);
                #endif
                return texColor;
            }
            ENDHLSL
        }
    }
}
