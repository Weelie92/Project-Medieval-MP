
Shader "Hidden/OccaSoftware/ScreenShadows"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        Cull Back
        Blend Off
        ZWrite Off
        ZTest Always
        
        Pass
        {
            Name "ScreenShadows"

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Assets/OccaSoftware/Altos/AssetResources/ShaderLibrary/CloudShadows.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }
            static float _INV_INV = 1.0 / 1e20;
            float3 frag(Varyings IN) : SV_Target
            {
                float2 UV = IN.positionHCS.xy / _ScaledScreenParams.xy;
                
                float depth = SampleSceneDepth(UV);
                float depth01 = Linear01Depth(depth, _ZBufferParams);
                
                float cloudShadowAttenuation = 1.0;
                if(depth01 < 1.0)
                {
                    float3 positionWS = ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);
                    cloudShadowAttenuation = GetCloudShadowAttenuation(positionWS);
                }
                
                return cloudShadowAttenuation;
            }
            ENDHLSL
        }
    }
}