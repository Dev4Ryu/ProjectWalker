Shader "Custom/RedEdgeTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1, 0, 0, 1)
        _EdgePower ("Edge Power", Range(0.1, 8)) = 2
        _EdgeIntensity ("Edge Intensity", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _EdgeColor;
            float _EdgePower;
            float _EdgeIntensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.uv = IN.uv;
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.viewDirWS = normalize(GetWorldSpaceViewDir(positionWS));
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float edge = 1 - saturate(dot(IN.normalWS, IN.viewDirWS));
                edge = pow(edge, _EdgePower);

                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float4 edgeColor = _EdgeColor * edge * _EdgeIntensity;

                // Transparent center, visible edge
                float alpha = saturate(edge * 2.0);
                return float4(edgeColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
