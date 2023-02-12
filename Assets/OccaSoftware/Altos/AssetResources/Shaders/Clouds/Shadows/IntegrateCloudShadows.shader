
Shader "Hidden/OccaSoftware/IntegrateCloudShadows"
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
            Name "IntegrateCloudShadows"

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            SamplerState linear_clamp_sampler;
            SamplerState point_clamp_sampler;
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
            };

            Texture2D _CLOUD_SHADOW_CURRENT_FRAME;
            float4 _CLOUD_SHADOW_CURRENT_FRAME_TexelSize;
            Texture2D _CLOUD_SHADOW_PREVIOUS_HISTORY;
            float _IntegrationRate;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float3 frag(Varyings IN) : SV_Target
            {
                float3 cloudShadows = _CLOUD_SHADOW_CURRENT_FRAME.SampleLevel(linear_clamp_sampler, IN.uv, 0).rgb;
                float3 cloudShadowHistory = _CLOUD_SHADOW_PREVIOUS_HISTORY.SampleLevel(point_clamp_sampler, IN.uv, 0).rgb;
                
                
	            float2 offsets[8] =
	            {
		            float2(0, -1),
		            float2(0, 1),
		            float2(1, 0),
		            float2(-1, 0),
		            float2(-1, -1),
		            float2(1, -1),
		            float2(-1, 1),
		            float2(1, 1)
	            };
                
                float3 v[8];
                
                for(int i = 0; i < 8; i++)
                {
                    v[i] = _CLOUD_SHADOW_CURRENT_FRAME.SampleLevel(point_clamp_sampler, IN.uv + _CLOUD_SHADOW_CURRENT_FRAME_TexelSize.xy * offsets[i], 0).rgb;
                }
                
		        float3 minResults[2] =
		        {
			        v[0],
			        v[0]
		        };
		        float3 maxResults[2] =
		        {
			        v[0],
			        v[0]
		        };
                
		        // cross sample
		        for (int j = 0; j < 4; j++)
		        {
			        minResults[0] = min(v[j], minResults[0]);
			        maxResults[0] = max(v[j], maxResults[0]);
		        }
		
		        minResults[1] = minResults[0];
		        maxResults[1] = maxResults[0];
		
		        // box sample 
		        // (leverages the results of the cross sample to avoid rework).
		        for (int k = 4; k < 8; k++)
		        {
			        minResults[1] = min(v[k], minResults[1]);
			        maxResults[1] = max(v[k], maxResults[1]);
		        }
                
		        float3 minResult, maxResult;
		        minResult = (minResults[0] + minResults[1]) * 0.5;
		        maxResult = (maxResults[0] + maxResults[1]) * 0.5;
                
                cloudShadowHistory = clamp(cloudShadowHistory, minResult, maxResult);
                
                return lerp(cloudShadowHistory, cloudShadows, _IntegrationRate);
            }
            ENDHLSL
        }
    }
}