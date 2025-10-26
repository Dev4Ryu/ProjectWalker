Shader "URP/CartoonGrassDirtLit_SimpleLighting_Full"
{
    Properties
    {
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _DirtTex  ("Dirt Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}
        _BlendAmount ("Blend Amount", Range(0,1)) = 0.5
        _NoiseScale  ("Noise Scale", Float) = 5
        _Metallic    ("Metallic", Range(0,1)) = 0.0
        _Smoothness  ("Smoothness", Range(0,1)) = 0.2
        _SpecularStrength ("Specular Strength", Range(0,2)) = 0.5
        _AmbientAmount ("Ambient", Range(0,1)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "UniversalMaterialType" = "Lit"
        }
        LOD 300

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // ✅ URP lighting variants
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            // Textures + params
            TEXTURE2D(_GrassTex);
            SAMPLER(sampler_GrassTex);
            TEXTURE2D(_DirtTex);
            SAMPLER(sampler_DirtTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _BlendAmount;
            float _NoiseScale;
            float _Metallic;
            float _Smoothness;
            float _SpecularStrength;
            float _AmbientAmount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS  = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS    = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample noise and blend mask
                float2 noiseUV = IN.uv * _NoiseScale;
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
                float mask = smoothstep(_BlendAmount - 0.1, _BlendAmount + 0.1, noise);

                // Base albedo from blended grass/dirt
                half4 g = SAMPLE_TEXTURE2D(_GrassTex, sampler_GrassTex, IN.uv);
                half4 d = SAMPLE_TEXTURE2D(_DirtTex, sampler_DirtTex, IN.uv);
                float3 albedo = lerp(d.rgb, g.rgb, mask);

                // Normal & view dir
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(GetWorldSpaceNormalizeViewDir(IN.positionWS));

                float3 finalColor = 0.0;
                float3 ambient = _AmbientAmount * albedo;

                // === MAIN DIRECTIONAL LIGHT ===
                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);
                float3 lightColor = mainLight.color.rgb;

                float NdotL = saturate(dot(N, L));
                float toonDiffuse = step(0.2, NdotL); // toon banding
                float3 H = normalize(L + V);
                float NdotH = saturate(dot(N, H));
                float shininess = lerp(8.0, 200.0, _Smoothness);
                float spec = pow(NdotH, shininess) * _SpecularStrength;

                float shadowAtten = 1.0;
                #if defined(_MAIN_LIGHT_SHADOWS)
                    float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                    shadowAtten = MainLightRealtimeShadow(shadowCoord);
                #endif

                finalColor += (albedo * lightColor * toonDiffuse * NdotL + lightColor * spec) * shadowAtten;

                // === ADDITIONAL LIGHTS (point, spot, etc.) ===
                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < lightCount; i++)
                {
                    Light light = GetAdditionalLight(i, IN.positionWS);
                    float3 L2 = normalize(light.direction);
                    float3 lightCol = light.color.rgb;

                    float NdotL2 = saturate(dot(N, L2));
                    float toonDiffuse2 = step(0.2, NdotL2);

                    float3 H2 = normalize(L2 + V);
                    float NdotH2 = saturate(dot(N, H2));
                    float spec2 = pow(NdotH2, shininess) * _SpecularStrength;

                    finalColor += (albedo * lightCol * toonDiffuse2 * NdotL2) + (lightCol * spec2);
                }
                #endif

                // === Ambient light ===
                finalColor += ambient;

                // Simple gamma correction for stylized look
                finalColor = pow(finalColor, 1.0 / 2.2);

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
